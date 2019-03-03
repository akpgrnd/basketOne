using System;

namespace Checkout.Basket.Business.Contracts.Model
{
    public class BasketItem
    {
        public Guid ItemId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Currency = "Sterling";
    }
}
