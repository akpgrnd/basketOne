using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Checkout.Demo
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static List<Guid> productGuids = new List<Guid> {
            new Guid("55490aed-6962-4b25-8cbd-60aa444d6f36"),
            new Guid("3ef3bb2b-0e99-4900-9fa0-3742a329d6c6"),
            new Guid("5efa4a52-3b4c-4cbe-ac8e-b9cbce371700"),
            new Guid("8ee9bab6-2590-4b9f-8900-9e9f1431c420"),
            new Guid("048ad2f0-8984-4331-8a47-8e7433ce26db"),
            new Guid("8de85adb-e753-49b9-8aa4-9873acde2da6")
        };

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:50582/api/basket");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

           try
            {

                Console.WriteLine("Showing basket...");
                await ShowBasket();

                Console.WriteLine("");
                Console.WriteLine("Adding items to basket...");
                await AddItems();
                await ShowBasket();

                Console.WriteLine("");
                Console.WriteLine("Updating quantities...");
                await UpdateQuantity();
                await ShowBasket();

                Console.WriteLine("");
                Console.WriteLine("Removing item...");
                await RemoveItem();
                await ShowBasket();

                Console.WriteLine("");
                Console.WriteLine("Clearing basket...");
                await ClearBasket();
                await ShowBasket();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        static async Task ShowBasket()
        {
            HttpResponseMessage response = await client.GetAsync("");
            string token = response.Headers.GetValues("X-Token").First();
            Context.BasketToken = Guid.Parse(token);
            if (!client.DefaultRequestHeaders.Contains("X-Token"))
            {
                client.DefaultRequestHeaders.Add("X-Token", token);
            } 
            if (response.IsSuccessStatusCode)
            {   
                Console.WriteLine($"BasketId {token}");
                Console.WriteLine(await response.Content.ReadAsStringAsync());               
            }
            else
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                Console.WriteLine("Could not obtain basket (FATAL ERROR)");
            }

        }

        static async Task AddItems()
        {
            for (int i = 0; i < 3; i++)
            {
                string productId = productGuids[i].ToString();
                Console.WriteLine($"adding item {productId}");
                HttpResponseMessage response = await client.PostAsync($"{client.BaseAddress}/{productId}", 
                    new StringContent((i + 2).ToString(), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {                    
                    Console.WriteLine("success");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    Console.WriteLine("Could not add item");
                }
            }
        }

        static async Task UpdateQuantity() {
            for (int i = 0; i < 3; i++)
            {
                string productId = productGuids[i].ToString();
                Console.WriteLine($"removing one item from each line {productId}");
                HttpResponseMessage response = await client.PutAsync($"{client.BaseAddress}/{productId}",
                    new StringContent((i + 1).ToString(), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("success");
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    Console.WriteLine("Could not remove/reduce item");
                }
            }
        }
        static async Task RemoveItem() {
            string productId = productGuids[0].ToString();
            Console.WriteLine($"removing first item from basket {productId}");
            HttpResponseMessage response = await client.DeleteAsync($"{client.BaseAddress}/{productId}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("success");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                Console.WriteLine("Could not remove item");
            }
        }
        static async Task ClearBasket() {
            string productId = productGuids[0].ToString();
            Console.WriteLine($"removing all items from basket");
            HttpResponseMessage response = await client.DeleteAsync(client.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("success");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            else
            {
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                Console.WriteLine("Could not remove item");
            }
        }        
    }
}
