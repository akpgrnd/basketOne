using System;
using System.Collections.Generic;
using System.Text;

namespace Checkout.Core.Contracts.Config
{
    public interface IBasketConfig
    {
        bool IsRingFenceEnabled { get; }
        int BasketTimeoutSeconds { get; }
    }
}
