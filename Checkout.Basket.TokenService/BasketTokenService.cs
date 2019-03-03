using Checkout.Basket.Token.Contracts;
using Checkout.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Checkout.Basket.TokenService
{
    public class BasketTokenService : IBasketTokenService
    {
        private readonly IBasketWriter writer;
        private readonly IBasketReader reader;
        private readonly ILogger<BasketTokenService> logger;

        public BasketTokenService(IBasketWriter writer, IBasketReader reader, ILogger<BasketTokenService> logger)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> BasketExistAsync(Guid basketId)
        {
            if (basketId == Guid.Empty) return false;
            var basket = await GetBasketAsync(basketId);

            return  basket != null && !basket.IsExpired;
        }
        
        public async Task<Guid> InitBasket()
        {
            var basket = await this.writer.InitBasketAsync();

            if (basket == null)
            {
                logger.LogError("Basket cannot be created");
                throw new ApplicationException("Basket cannot be created");
            }

            return basket.Id;
        }

        private async Task<Data.Model.Basket> GetBasketAsync(Guid basketId)
        {
            if (basketId == Guid.Empty) return null;

            return await reader.GetBasketAsync(basketId);
        }      
    }
}
