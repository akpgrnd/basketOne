using Checkout.Data.Contracts;
using System;
using Checkout.Data.Model;
using System.Threading.Tasks;
using Checkout.Data.Stubs;
using System.Linq;

namespace Checkout.Data
{
    public class RingfenceReader : BaseDatabaseReaderWriter, IRingfenceReader
    {
        public RingfenceReader(Db db) : base(db) { }

        public async Task<int> CountRingfencedProduct(Guid productId)
        {
            return await Task.Run(() => db.Ringfences
                            .Where(x => x.ProductId == productId)
                            .Sum(x => x.Quantity));
        }

        public async Task<int> CountRingfencedProduct(Guid basketId, Guid productId)
        {
            return await Task.Run(() => db.Ringfences
                            .Where(x => x.ProductId == productId && x.BasketId == basketId)
                            .Sum(x => x.Quantity));
        }

        public async Task<Basket> GetBasketAsync(Guid id)
        {
            return await Task.Run(() => db.Baskets.FirstOrDefault(x => x.Id == id));
        }
    }
}
