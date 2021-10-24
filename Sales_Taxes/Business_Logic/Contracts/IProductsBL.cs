using Models;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Contracts
{
    public interface IProductsBL
    {
        Task<IEnumerable<ProductsModel>> GetProducts();

        Task<ProductsModel> GetProductById(string idCategory);

        Task<bool> Add(ProductRequestDto requestDto);

        Task<bool> Update(ProductRequestDto requestDto);
        Task<bool> Delete(string productId);
    }
}
