using Business_Logic;
using Business_Logic.Contracts;
using Data_Access;
using Data_Access.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sales_Taxes.InversionOfcontrol
{
    public static class IoC
    {
        /// <summary>
        /// Below method is the place for register the Interfaces and return the implementation
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRegistration(this IServiceCollection services)
        {
            services.AddTransient<IProductsBL, ProductsBL>();

            services.AddTransient<IProductsDA, ProductsDA>();

            services.AddTransient<ISalesBL, SalesBL>();

            services.AddTransient<ISalesDA, SalesDA>();

            return services;
        }
    }
}
