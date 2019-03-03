using Checkout.Basket.Business.Contracts;
using System;
using Checkout.Core.Contracts;
using System.Threading.Tasks;
using Checkout.Basket.Ringfence.Contracts;
using Checkout.Core.Contracts.Constants;
using Checkout.Data.Contracts;
using System.Linq;
using Checkout.Basket.Business.Contracts.Model;

namespace Checkout.Basket.Business
{
    public class BasketManager : IBasketManager
    {
        readonly IProductValidatorService productValidatorService;
        readonly IRingfenceService ringfenceService;
        readonly IBasketWriter basketWriter;
        readonly IBasketReader basketReader;

        public BasketManager(IProductValidatorService productValidatorService, 
            IRingfenceService ringfenceService, 
            IBasketWriter basketWriter,
            IBasketReader basketReader)
        {
            this.productValidatorService = productValidatorService ?? throw new ArgumentNullException(nameof(productValidatorService));
            this.ringfenceService = ringfenceService ?? throw new ArgumentNullException(nameof(ringfenceService));
            this.basketWriter = basketWriter ?? throw new ArgumentNullException(nameof(basketWriter));
            this.basketReader = basketReader ?? throw new ArgumentNullException(nameof(basketReader));
        }

        public async Task<Result> ClearBasketAsync(Guid basketId)
        {
            if (basketId == Guid.Empty) throw new ArgumentException();

            var res = await basketWriter.ClearBasketAsync(basketId);

            return res;
        }

        public async Task<CustomerBasket> GetBasketAsync(Guid basketId)
        {
            var basket = await this.basketReader.GetBasketAsync(basketId);

            if (basket == null) return null;

            var result = new CustomerBasket();
            if (basket.ItemList?.Count > 0)
            {
                var items = basket.ItemList.Select(x => new BasketItem {
                    ItemId = x.ProductId,
                    Title = x.Title,
                    Quantity = x.Quantity,
                    Price = x.Price
                });

                result.BasketItemList.AddRange(items);
            }

            return result;
        }

        public async Task<Result> RemoveItemAsync(Guid basketId, Guid itemId)
        {
            return await UpdateBasketAsync(basketId, itemId, 0);
        }

        public async Task<Result> UpdateBasketAsync(Guid basketId, Guid itemId, int newQuantity)
        {
            if (newQuantity < 0) throw new ArgumentException(nameof(newQuantity));

            var basket = await basketReader.GetBasketAsync(basketId);
            if (basket == null || basket.Id == Guid.Empty) return new Result { Code = ResultCode.BasketInvalid };
            if (basket.IsExpired) return new Result { Code = ResultCode.BasketExpired };

            var product = basket.ItemList?.FirstOrDefault(x => x.ProductId == itemId);           
            if (product == null || product.Quantity < newQuantity)
            {
                return await AddToBasketAsync(basket.Id, itemId, newQuantity, newQuantity - product?.Quantity ?? 0);
            }
            else if (product.Quantity > newQuantity)
            {
                return await RemoveFromBasketAsync(basket, itemId, newQuantity);
            }

            return new Result { Code = ResultCode.Ok };
        }

        private async Task<Result> AddToBasketAsync(Guid basketId, Guid itemId, int newQuantity, int addedItems)
        {
            Result validationResult = await productValidatorService.IsProductAvailable(itemId, addedItems);
            if (validationResult.Code != ResultCode.Ok) return validationResult;

            Result ringfenceResult = await ringfenceService.TryReserveAsync(basketId, itemId, newQuantity);
            if (ringfenceResult.Code != ResultCode.Ok) return ringfenceResult;

            return await basketWriter.AddAsync(basketId, itemId, newQuantity);
        }

        private async Task<Result> RemoveFromBasketAsync(Data.Model.Basket basket, Guid itemId, int newQuantity)
        {
            Result validationResult = await productValidatorService.IsProductValid(itemId);
            if (validationResult.Code != ResultCode.Ok) return validationResult;

            Result ringfenceResult = await ringfenceService.ReleaseAsync(basket.Id, itemId, newQuantity);
            if (ringfenceResult.Code != ResultCode.Ok) return ringfenceResult;

            return await basketWriter.RemoveAsync(basket.Id, itemId, newQuantity);
        }        
    }
}
