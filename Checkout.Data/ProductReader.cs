using Checkout.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using Checkout.Data.Model;
using System.Threading.Tasks;
using Checkout.Data.Stubs;
using System.Linq;

namespace Checkout.Data
{
    public class ProductReader : BaseDatabaseReaderWriter, IProductReader
    {
        public ProductReader(Db db) : base(db) { }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await Task.Run(() => db.Products);
        }

        public async Task<Product> GetProduct(Guid id)
        {
            return await Task.Run(() => db.Products.FirstOrDefault(x => x.Id == id));
        }
    }
}
