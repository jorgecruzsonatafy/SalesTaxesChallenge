using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data_Access.Contracts
{
    public interface IProductsDA
    {
        Task<IEnumerable<XElement>> GetProducts();

        Task<XElement> GetProductById(string idCategory);

        Task<bool> Add(ProductsModel entity);

        Task<bool> Update(ProductsModel entity);

        Task<bool> Delete(string id);
    }
}
