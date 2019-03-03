using Checkout.Basket.Business.Contracts.Model;
using Checkout.Core.Contracts;
using System;
using System.Threading.Tasks;

namespace Checkout.Basket.Business.Contracts
{
    public interface IBasketManager
    {
        Task<Result> ClearBasketAsync(Guid basketId);
        Task<Result> RemoveItemAsync(Guid basketId, Guid itemId);
        Task<Result> UpdateBasketAsync(Guid basketId, Guid itemId, int quantity);
        Task<CustomerBasket> GetBasketAsync(Guid basketId);
    }
}
