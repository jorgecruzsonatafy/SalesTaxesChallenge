using Data_Access.Contracts;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data_Access
{
    public class SalesDA : ISalesDA
    {
        private readonly XElement xelement;
        private readonly string path;

        public SalesDA()
        {
            path = Environment.CurrentDirectory + @"\XmlBD.xml";
            xelement = XElement.Load(path);
        }

        public async Task<IEnumerable<XElement>> GetSales()
        {
            return xelement.Elements("Sale").ToList();

        }

        public async Task<XElement> GetSaleById(string productKey)
        {
            var query = xelement.Elements("Sale")
                .Where(e => (string)e.Element("ProductKey") == productKey)
                .FirstOrDefault();

            return query;
        }

        public async Task<bool> Add(Totals entity)
        {
            XDocument xmlDoc = XDocument.Load(path);
            var result = false;
            try
            {
                xmlDoc.Element("Sales").Add(
               new XElement("Sale",
               new XElement("ProductKey", entity.ProductKey.ToString().Trim()),
               new XElement("Product", entity.Product.Trim()),
               new XElement("Quantity", entity.Quantity.ToString().Trim()),
               new XElement("Taxes", entity.Taxes.ToString().Trim()),
               new XElement("PriceWithTaxes", entity.PriceWithTaxes.ToString().Trim()),
               new XElement("Total", entity.Total.ToString().Trim())
               ));

                xmlDoc.Save(path);
                result = true;
            }
            catch (Exception)
            {

                throw;
            }

            return result;

        }

        public async Task<bool> AddMany(IEnumerable<Totals> totals)
        {
            XDocument xmlDoc = XDocument.Load(path);
            var result = false;
            try
            {
                foreach (var item in totals)
                {
                    xmlDoc.Element("Sales").Add(
                       new XElement("Sale",
                       new XElement("ProductKey", item.ProductKey.ToString().Trim()),
                       new XElement("Product", item.Product.Trim()),
                       new XElement("Quantity", item.Quantity.ToString().Trim()),
                       new XElement("Taxes", item.Taxes.ToString().Trim()),
                       new XElement("PriceWithTaxes", item.PriceWithTaxes.ToString().Trim()),
                       new XElement("Total", item.Total.ToString().Trim())
                       ));
                    xmlDoc.Save(path);
                }
                
                result = true;
            }
            catch (Exception)
            {

                throw;
            }

            return result;

        }

        public async Task<bool> DeleteAll()
        {
            var result = false;
            try
            {
                var sales = await GetSales();

                foreach (var item in sales)
                {
                    item.Remove();
                }                             

                xelement.Save(path);
                result = true;

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }

        public async Task<bool> Delete(string id)
        {
            var result = false;
            try
            {
                var q = await GetSaleById(id);

                q.Remove();

                xelement.Save(path);
                result = true;

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }
    }
}
