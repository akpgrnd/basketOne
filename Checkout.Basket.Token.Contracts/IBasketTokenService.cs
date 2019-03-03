using Checkout.Core.Contracts;
using System;
using System.Threading.Tasks;

namespace Checkout.Basket.Token.Contracts
{
    public interface IBasketTokenService
    {
        //Task<Result> ClearBasket(Guid basketId);
        Task<bool> BasketExistAsync(Guid basketId);
        Task<Guid> InitBasket();
    }
}
