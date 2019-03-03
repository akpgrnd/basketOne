using System;

namespace Checkout.Demo
{
    public class BasketItem
    {
        public Guid ItemId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Currency { get; set;}
    }
}
