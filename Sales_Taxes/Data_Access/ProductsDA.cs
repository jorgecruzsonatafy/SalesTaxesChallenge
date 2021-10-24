using Data_Access.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Data_Access
{
    public class ProductsDA : IProductsDA
    {
        private readonly XElement xelement;
        private readonly string path;
       
        

        public ProductsDA()
        {
            path = Environment.CurrentDirectory + @"\XmlBD.xml";
            xelement = XElement.Load(path);           
            
        }
        
        public async Task<IEnumerable<XElement>> GetProducts()
        {
           return xelement.Elements("Products").ToList();

        }

        public async Task<XElement> GetProductById(string idCategory)
        {
            var query = xelement.Elements("Products")
                .Where(e => (string)e.Element("ProductId") == idCategory)
                .FirstOrDefault();

            return query;
        }

        public async Task<bool> Add(ProductsModel entity)
        {
            XDocument xmlDoc = XDocument.Load(path);
            var result = false;
            try
            {
                xmlDoc.Element("Sales").Add(
               new XElement("Products",
               new XElement("ProductId", entity.ProductId.ToString().Trim()),
               new XElement("Description", entity.Description.Trim()),
               new XElement("ListPrice", entity.ListPrice.ToString().Trim()),
               new XElement("Category", entity.Category == null ? string.Empty: entity.Category.Trim()),               
               new XElement("IsImported", entity.IsImported.ToString().Trim())
               ));

                xmlDoc.Save(path);
                result= true;
            }
            catch (Exception exception)
            {
                throw;
                
            }

            return result;

        }

        public async Task<bool> Update(ProductsModel entity)
        {
            var q = await GetProductById(entity.ProductId.ToString());
            var result = false;
            try
            {
                
                q.Element("Description").SetValue(entity.Description);
                q.Element("ListPrice").SetValue(entity.ListPrice);
                q.Element("Category").SetValue(entity.Category);
                q.Element("IsImported").SetValue(entity.IsImported);


                xelement.Save(path);
                result= true;
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public async Task<bool> Delete(string id)
        {
            var  result = false;
            try
            {
                var q = await GetProductById(id);

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
