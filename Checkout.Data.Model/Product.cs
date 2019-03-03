using System;

namespace Checkout.Data.Model
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int? LimitPerOrder { get; set; }
    }
}
