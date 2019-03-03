using Checkout.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Checkout.Data.Stubs
{
    public class Db
    {        
        public List<Product> Products { get; private set; }
        public List<Basket> Baskets { get; private set; }
        public List<RingfenceItem> Ringfences { get; private set; }

        public Db()
        {
            Seed();
        }

        public bool AddUpdateBasket(Basket basket)
        {
            if (basket == null) return false;

            var b = Baskets.FirstOrDefault(x => x.Id == basket.Id);

            if (b != null) b = basket;
            else Baskets.Add(basket);

            return true;
        }

        public bool RemoveBasket(Basket basket)
        {
            if (basket == null) return false;

            var b = Baskets.FirstOrDefault(x => x.Id == basket.Id);
            if (b != null) Baskets.Remove(basket);

            return true;
        }

        public bool AddUpdateRingfences(RingfenceItem rf)
        {
            if (rf == null) return false;

            var r = Ringfences.FirstOrDefault(x => x.BasketId == rf.BasketId && x.ProductId == rf.ProductId);

            if (r != null) r.Quantity = rf.Quantity;
            else Ringfences.Add(rf);

            return true;
        }

        public bool RemoveRingfences(RingfenceItem rf)
        {
            if (rf == null) return false;
            
            int res = Ringfences.RemoveAll(x => x.BasketId == rf.BasketId && x.ProductId == rf.ProductId);

            return res > 0;
        }

        public bool UpdateProductQuantity(Guid productId, int quantity)
        {
            if (quantity < 0) return false;

            foreach (Product p in Products)
            {
                if (p.Id == productId)
                {
                    p.Quantity = quantity;
                    return true;
                }
            }
            
            return false;
        }

        private void Seed()
        {
            Baskets = new List<Basket>();
            Ringfences = new List<RingfenceItem>();

            SeedProducts();
        }

        private void SeedProducts()
        {
            var guids = new List<Guid>
            {
                new Guid("55490aed-6962-4b25-8cbd-60aa444d6f36"),
                new Guid("3ef3bb2b-0e99-4900-9fa0-3742a329d6c6"),
                new Guid("5efa4a52-3b4c-4cbe-ac8e-b9cbce371700"),
                new Guid("8ee9bab6-2590-4b9f-8900-9e9f1431c420"),
                new Guid("048ad2f0-8984-4331-8a47-8e7433ce26db"),
                new Guid("8de85adb-e753-49b9-8aa4-9873acde2da6")
            };

            Products = new List<Product>
            {
                new Product{ Id = guids[0], Title = "T-Shirt", Price = 10.22d, Quantity = 15 },
                new Product{ Id = guids[1], Title = "Samsung Galaxy 10 Sim Free", Price = 799d, Quantity = 13 },
                new Product{ Id = guids[2], Title = "Computer Table", Price = 232.59d, Quantity = 11 },
                new Product{ Id = guids[3], Title = "HDMI Cable", Price = 18.34d, Quantity = 9 },
                new Product{ Id = guids[4], Title = "Crypto currencies at home", Price = 24.99d, Quantity = 5 },
                new Product{ Id = guids[5], Title = "Alt-J An awesome wave CD", Price = 11.99d, Quantity = 0 },
            };
        }
    }
}
