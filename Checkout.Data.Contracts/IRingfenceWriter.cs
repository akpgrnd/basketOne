using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Checkout.Data.Contracts
{
    public interface IRingfenceWriter
    {
        Task<bool> ReserveItem(Guid basketId, Guid itemId, int quantity); //, DateTime expiry);
        Task<bool> ReleaseItem(Guid basketId, Guid itemId, int newQuantity);
    }
}
