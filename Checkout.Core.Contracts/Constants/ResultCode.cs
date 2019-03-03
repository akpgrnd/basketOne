using System;
using System.Collections.Generic;
using System.Text;

namespace Checkout.Core.Contracts.Constants
{
    // update as/when needed
    public enum ResultCode
    {
        Unknown = 0,
        Ok,
        GenericError,
        InvalidProduct,
        BasketExpired,
        BasketInvalid,
        InsufficientInventory,
        InvalidQuantity,
        ItemOrderLimitExceeded,
        RingfenceError
    }
}
