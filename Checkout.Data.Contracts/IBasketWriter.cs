using Checkout.Core.Contracts;
using System;
using System.Threading.Tasks;

namespace Checkout.Data.Contracts
{
    public interface IBasketWriter
    {
        Task<Model.Basket> InitBasketAsync();
        Task<Result> ClearBasketAsync(Guid basket);
        Task<Result> AddAsync(Guid basket, Guid productId, int quantity);
        Task<Result> RemoveAsync(Guid basket, Guid productId, int quantity);
    }
}
