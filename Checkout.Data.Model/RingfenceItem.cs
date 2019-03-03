using System;

namespace Checkout.Data.Model
{
    public class RingfenceItem
    {
        public Guid ProductId { get; set; }
        public Guid BasketId { get; set; }
        public int Quantity { get; set; }
    }
}
