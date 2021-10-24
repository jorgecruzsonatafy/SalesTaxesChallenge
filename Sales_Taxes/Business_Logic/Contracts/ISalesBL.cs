using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Contracts
{
    public interface ISalesBL
    {
        Task<IEnumerable<SelectListItem>> AvailableProducts();

        Task<SalesModel> GetProductInfo(string productId);

        Task<ResponseModel> AddProducts(ProductRequestDto products);

        Task<bool> DeletAll();
        Task<ResponseModel> RemoveProduct(string id);

        Task<IEnumerable<Totals>> RetreivePurchasedProducts();
    }
}
