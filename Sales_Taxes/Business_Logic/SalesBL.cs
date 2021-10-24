using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Business_Logic.Contracts;
using Models;
using Models.DTO;
using Data_Access.Contracts;
using Microsoft.Extensions.Logging;

namespace Business_Logic
{
    public class SalesBL : ISalesBL
    {
        private readonly IProductsBL _productsBL;
        private readonly ISalesDA _salesDA;
        private readonly ILogger _logger;

        public SalesBL(IProductsBL productsBL, ISalesDA salesDA, ILogger<SalesBL> logger)
        {
            _productsBL = productsBL;
            _salesDA = salesDA;
            _logger = logger;
        }

        public async Task<IEnumerable<SelectListItem>> AvailableProducts()
        {

            SelectList result = null;
            try
            {
                var items = await _productsBL.GetProducts();
                if (items.Any())
                {
                    var list = items.Select(s => new
                    {
                        Value = s.ProductId,
                        Text = s.Description
                    });

                    result = new SelectList(list, "Value", "Text");
                }
            }
            catch (Exception exception)
            {

                _logger.LogError(exception.Message, string.Empty);
            }


            return result;
        }

        public async Task<SalesModel> GetProductInfo(string productId)
        {
            var result = new SalesModel();

            try
            {
                if (!string.IsNullOrEmpty(productId))
                {
                    var product = await _productsBL.GetProductById(productId.Trim());
                    if (product != null)
                    {
                        result.ProductId = product.ProductId;
                        result.Description = product.Description.Trim();
                        result.Category = product.Category.Trim();
                        result.IsImported = product.IsImported;
                        result.Price = product.ListPrice;
                    }
                }
            }
            catch (Exception exception)
            {
                result = null;
                _logger.LogError(exception.Message, productId);
            }

            return result;
        }

        /// <summary>
        /// This method is in charge to process the inputs a returns the all complete information about inserted products.
        /// </summary>
        /// <param name="productsModel">Intial information about each input</param>
        /// <param name="currentProducts">contains the list of each added product</param>       
        /// <returns>Response object with total and updated product list</returns>
        public async Task<ResponseModel> AddProducts(ProductRequestDto product )
        {
            var response = new ResponseModel();

            var currentProducts = await RetreivePurchasedProducts();
            try
            {
                var newProduct = await AddNewProduct(product);
                var dictionary = new Dictionary<string, Totals>();

                if (currentProducts.Any())
                {
                    dictionary = await UpdateProducts(currentProducts.ToList(), newProduct);
                }
                else
                {
                    var listTotals = new List<Totals>();
                    listTotals.Add(newProduct);
                    dictionary = listTotals.ToDictionary(x => x.ProductKey, x => x);
                }

                var listResult = dictionary.Select(s => s.Value).ToList();

                if (listResult.Any())
                {
                    response.currentProducts = listResult;
                    response.Taxes = Math.Round( listResult.Select(s => s.Taxes).Sum(),2);
                    response.Total = Math.Round( listResult.Select(s => s.Total).Sum(),2);

                    await _salesDA.AddMany(listResult);
                }
                else 
                {
                    response.currentProducts = null;
                    response.Taxes = 0;
                    response.Total = 0;
                }


            }
            catch (Exception exception)
            {
                response = null;
                _logger.LogError(exception.Message, product);
            }


            return response;

        }

        public async Task<bool> DeletAll() 
        {
            bool result = false;
            try
            {
                result = await _salesDA.DeleteAll();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message, string.Empty);
            }
            return result;
        }

        public async Task<ResponseModel> RemoveProduct(string id) 
        {
            var response = new ResponseModel();
            try
            {
                var removeProduct = await _salesDA.Delete(id);
                if (removeProduct)
                {
                    var retreivePurchasedProducts = await RetreivePurchasedProducts();
                    if (retreivePurchasedProducts.Any())
                    {
                        response.currentProducts = retreivePurchasedProducts.ToList();
                        response.Taxes = Math.Round(retreivePurchasedProducts.Select(s => s.Taxes).Sum(), 2);
                        response.Total = Math.Round(retreivePurchasedProducts.Select(s => s.Total).Sum(), 2);
                    }
                }

            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message, id);
            }

            return response;
        }

        public async Task<IEnumerable<Totals>> RetreivePurchasedProducts() 
        {
            List<Totals> currentProducts = new List<Totals>();
           
           
            var getSales = await _salesDA.GetSales();

            if (getSales.Any())
            {
                currentProducts = getSales.Select(s => new Totals()
                {
                    ProductKey = s.Element("ProductKey").Value,
                    Product = s.Element("Product").Value,
                    Quantity = int.Parse(s.Element("Quantity").Value),
                    Taxes = double.Parse( s.Element("Taxes").Value),
                    PriceWithTaxes = double.Parse(s.Element("PriceWithTaxes").Value),
                    Total = double.Parse(s.Element("Total").Value)

                }).ToList();
            }          


            return currentProducts;
        }


        /// <summary>
        /// Build an object with the information about the new porduct that will be inserted to product list
        /// </summary>
        /// <param name="product"></param>
        /// <returns>Totals object with key, taxes and total for added item.</returns>
        private async Task<Totals> AddNewProduct(ProductRequestDto product)
        {
            var result = new Totals();

            var category = product.Category == null ? string.Empty : product.Category.Trim().ToLower();
            var key = $@"{product.Description.Trim().ToLower()}{category}{product.IsImported.ToString().ToLower()}{product.ListPrice.ToString().ToLower()}";

            double price = product.ListPrice;
            double taxesProduc = 0;

            if (string.IsNullOrEmpty(product.Category) && product.IsImported)
            {
                taxesProduc = Math.Round(price * 0.15, 2);
            }
            if (string.IsNullOrEmpty(product.Category) && !product.IsImported)
            {
                taxesProduc = Math.Round(price * 0.10, 2);
            }
            if (!string.IsNullOrEmpty(product.Category) && product.IsImported)
            {
                taxesProduc = Math.Round(price * 0.05, 2);
            }

            if (taxesProduc > 0)
            {
                taxesProduc = await UpdateTaxes(taxesProduc);
            }


            result.ProductKey = key;
            result.Product = product.Description;
            result.Quantity = 1;
            result.PriceWithTaxes = Math.Round( price + taxesProduc,2);
            result.Taxes = taxesProduc;
            result.Total = Math.Round(price + taxesProduc,2);

            return result;
        }


        /// <summary>
        /// This method in charge to: add a new product or Update values in case to exist the same product.
        /// </summary>
        /// <param name="currentProducts"></param>
        /// <param name="totals"></param>
        /// <returns>a dictionary with the key and updated products</returns>
        private async Task< Dictionary<string, Totals>> UpdateProducts(List<Totals> currentProducts, Totals totals)
        {
            await _salesDA.DeleteAll();
            var dictionary = currentProducts.ToDictionary(x => x.ProductKey, x => x);

            var item = new Totals();
            if (dictionary.TryGetValue(totals.ProductKey, out item))
            {
                item.Quantity += 1;
                item.Taxes += totals.Taxes;
                item.Total = Math.Round(item.Quantity * item.PriceWithTaxes, 2);
                dictionary[totals.ProductKey] = item;

            }
            else
            {
                dictionary.Add(totals.ProductKey, totals);
            }

            return dictionary;
        }

        /// <summary>
        /// This Method is in charge to round decimals
        /// </summary>
        /// <param name="currentTaxe"></param>
        /// <returns>the updated taxes</returns>
        private async Task<double> UpdateTaxes(double currentTaxe)
        {
            double result = 0;

            if (currentTaxe.ToString().Contains("."))
            {


                var split = currentTaxe.ToString().Split('.');
                var decimals = split[1].ToArray();


                if (decimals.Length >= 2)
                {
                    if (int.Parse(decimals[1].ToString()) > 0 && int.Parse(decimals[1].ToString()) <= 5)
                    {
                        result = Convert.ToDouble($"{split[0]}.{decimals[0]}5");
                    }
                    if (int.Parse(decimals[1].ToString()) >= 6)
                    {
                        var diference = Math.Round(1 - Convert.ToDouble($"0.{split[1]}"), 2);
                        var splitDiference = diference.ToString().Split('.');
                        var decimalDiference = splitDiference[1].ToArray();
                        var part1 = Convert.ToInt32(split[1]);
                        var part2 = Convert.ToInt32(decimalDiference[1].ToString());
                        var addition = part1 + part2;

                        result = Convert.ToDouble($"{split[0]}.{addition}");
                    }
                }
                else
                {
                    result = currentTaxe;
                }
            }
            else
            {
                result = currentTaxe;
            }

            return result;
        }
    }
}
