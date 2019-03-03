using Checkout.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using Checkout.Core.Contracts;
using Checkout.Data.Model;
using System.Threading.Tasks;
using Checkout.Data.Stubs;
using System.Linq;
using Checkout.Core.Contracts.Constants;

namespace Checkout.Data
{
    public class BasketWriter : BaseDatabaseReaderWriter, IBasketWriter
    {
        readonly IProductReader prodReader;
        public BasketWriter(Db db, IProductReader prodReader) : base(db) {
            this.prodReader = prodReader;
        }
    
        public async Task<Result> AddAsync(Guid basketId, Guid productId, int quantity)
        {
            Basket bsk = db.Baskets.FirstOrDefault(x => x.Id == basketId)
                ?? await InitBasketAsync(basketId);

            var prod = bsk.ItemList.FirstOrDefault(x => x.ProductId == productId);
            if (prod != null)
            {
                prod.Quantity = quantity;
            }
            else
            {
                var product = await prodReader.GetProduct(productId);
                if (product != null)
                {
                    var newProd = new BasketItem
                    {
                        ProductId = productId,
                        Title = product.Title,
                        Price = product.Price,
                        Quantity = quantity
                    };

                    bsk.ItemList.Add(newProd);
                }                
            }

            return db.AddUpdateBasket(bsk)
                ? new Result { Code = ResultCode.Ok }
                : new Result { Code = ResultCode.GenericError };

        }

        public async Task<Result> ClearBasketAsync(Guid basketId)
        {
            Basket bsk = await Task.Run(() => db.Baskets.FirstOrDefault(x => x.Id == basketId));

            if (bsk != null)
            {
                bsk.ItemList.Clear();
            }
            else
            {
                return new Result { Code = ResultCode.BasketInvalid };
            }

            return db.AddUpdateBasket(bsk)
                ? new Result { Code = ResultCode.Ok }
                : new Result { Code = ResultCode.GenericError };
        }

        public async Task<Result> RemoveAsync(Guid basketId, Guid productId, int quantity)
        {
            Basket bsk = await Task.Run(() => db.Baskets.FirstOrDefault(x => x.Id == basketId));

            if (bsk != null)
            {
                if (quantity == 0)
                {
                    bsk.ItemList.RemoveAll(x => x.ProductId == productId);
                }
                else
                {
                    var item = bsk.ItemList.FirstOrDefault(x => x.ProductId == productId);
                    item.Quantity = quantity;
                }                
            }
            else {
                return new Result { Code = ResultCode.BasketInvalid };
            }

            return db.AddUpdateBasket(bsk)
                ? new Result { Code = ResultCode.Ok }
                : new Result { Code = ResultCode.GenericError };
        }

        public async Task<Basket> InitBasketAsync()
        {
            return await InitBasketAsync(Guid.NewGuid());
        }

        private async Task<Basket> InitBasketAsync(Guid id)
        {
            var basket = new Basket
            {
                Id = id
            };

            bool res = await Task.Run(() => db.AddUpdateBasket(basket));

            return res ? basket : throw new Exception("Basket couldn't be created");
        }
    }
}
