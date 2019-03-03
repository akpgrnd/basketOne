using Checkout.Data.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Checkout.Data.Contracts
{
    public interface IProductReader
    {
       Task<IEnumerable<Product>> GetAll();
       Task<Product> GetProduct(Guid id);
    }
}
