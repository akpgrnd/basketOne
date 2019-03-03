using Checkout.Data.Contracts;
using System;
using Checkout.Data.Model;
using System.Threading.Tasks;
using Checkout.Data.Stubs;
using System.Linq;

namespace Checkout.Data
{
    public class BasketReader : BaseDatabaseReaderWriter, IBasketReader
    {
        public BasketReader(Db db) : base(db) { }

        public async Task<Basket> GetBasketAsync(Guid id)
        {
            return await Task.Run(() => db.Baskets.FirstOrDefault(x => x.Id == id));
        }
    }
}
