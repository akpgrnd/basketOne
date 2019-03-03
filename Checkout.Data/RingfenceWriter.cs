using Checkout.Data.Contracts;
using System;
using Checkout.Data.Model;
using System.Threading.Tasks;
using Checkout.Data.Stubs;
using System.Linq;

namespace Checkout.Data
{
    public class RingfenceWriter : BaseDatabaseReaderWriter, IRingfenceWriter
    {
        public RingfenceWriter(Db db) : base(db) { }

        public async Task<Basket> GetBasketAsync(Guid id)
        {
            return await Task.Run(() => db.Baskets.FirstOrDefault(x => x.Id == id));
        }

        public async Task<bool> ReleaseItem(Guid basketId, Guid itemId, int newQuantity)
        {
            return await Task.Run(() =>
            {
                var rf = new RingfenceItem { BasketId = basketId, ProductId = itemId, Quantity = newQuantity };

                return newQuantity == 0
                        ? db.RemoveRingfences(rf)
                        : db.AddUpdateRingfences(rf);                
            });
        }

        public async Task<bool> ReserveItem(Guid basketId, Guid itemId, int newQuantity) //, DateTime expiry)
        {
            return await Task.Run(() =>
            {
                var rf = new RingfenceItem { BasketId = basketId, ProductId = itemId, Quantity = newQuantity };

                return db.AddUpdateRingfences(rf);
            });
        }
    }
}
