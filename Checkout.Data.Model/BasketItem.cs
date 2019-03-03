using System;

namespace Checkout.Data.Model
{
    public class BasketItem
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
