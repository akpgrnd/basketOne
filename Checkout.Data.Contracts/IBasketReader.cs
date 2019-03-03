using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Checkout.Data.Contracts
{
    public interface IBasketReader
    {
        Task<Model.Basket> GetBasketAsync(Guid id);
    }
}
