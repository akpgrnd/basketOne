using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Checkout.Basket.API.Middleware;
using Checkout.Basket.Token.Contracts;
using Checkout.Basket.TokenService;
using Checkout.Data.Contracts;
using Checkout.Data.Stubs;
using Checkout.Data;
using Checkout.Basket.Business.Contracts;
using Checkout.Basket.Business;
using Checkout.Basket.Ringfence.Contracts;
using Checkout.Basket.RingfenceService;
using Checkout.Basket.API.Filters;

namespace Checkout.Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<Db>();
            services.AddScoped<IBasketTokenService, BasketTokenService>();
            services.AddScoped<IBasketReader, BasketReader>();
            services.AddScoped<IBasketWriter, BasketWriter>();
            services.AddScoped<IBasketManager, BasketManager>();
            services.AddScoped<IProductValidatorService, ProductValidatorService>();
            services.AddScoped<IRingfenceService, RingfenceService.RingfenceService>();
            services.AddScoped<IProductReader, ProductReader>();
            services.AddScoped<IRingfenceReader, RingfenceReader>();
            services.AddScoped<IRingfenceWriter, RingfenceWriter>();

            services.AddScoped<BasketTokenFilter>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<BasketTokenRequest>();
            app.UseMiddleware<BasketTokenResponse>();
            app.UseMvc();
            


        }
    }
}
