using System;
using System.Collections.Generic;
using System.Text;

namespace Checkout.Data.Model
{
    public class Basket
    {
        public Basket()
        {
            ItemList = new List<BasketItem>();
        }

        public Guid Id { get; set; }
        public DateTime Expiry { get; private set; }
        public bool IsExpired => Expiry != DateTime.MinValue && DateTime.Compare(DateTime.UtcNow, Expiry) > 0;
        public List<BasketItem> ItemList { get; private set; }
    }
}
