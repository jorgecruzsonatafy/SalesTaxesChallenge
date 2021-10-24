using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data_Access.Contracts
{
    public interface ISalesDA
    {
        Task<IEnumerable<XElement>> GetSales();
        Task<XElement> GetSaleById(string productKey);
        Task<bool> Add(Totals entity);
        Task<bool> AddMany(IEnumerable<Totals> totals);
        Task<bool> DeleteAll();
        Task<bool> Delete(string id);
    }
}
