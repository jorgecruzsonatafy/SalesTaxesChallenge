using Business_Logic.Contracts;
using Data_Access.Contracts;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Models.DTO;
using Microsoft.Extensions.Logging;

namespace Business_Logic
{
    public class ProductsBL : IProductsBL
    {
        public readonly IProductsDA _productsDA;
        private readonly ILogger _logger;

        public ProductsBL(IProductsDA productsDA, ILogger<ProductsBL> logger)
        {
            _productsDA = productsDA;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductsModel>> GetProducts()
        {
            var result = new List<ProductsModel>();           

            try
            {
                var data = await _productsDA.GetProducts();
                if (data.Any())
                {
                    result = data.Select(s => new ProductsModel()
                    {
                        ProductId = int.Parse( s.Element("ProductId").Value),
                        Description = s.Element("Description").Value,
                        ListPrice = double.Parse(s.Element("ListPrice").Value),
                        Category = s.Element("Category").Value,
                        IsImported = bool.Parse( s.Element("IsImported").Value)

                    }).ToList();
                }
            }
            catch (Exception exception)
            {
                result = null;
                _logger.LogError(exception.Message, string.Empty);
            }
            return result.OrderBy(o => o.ProductId);
        }

        public async Task<ProductsModel> GetProductById(string idCategory)
        {
            var result = new ProductsModel();

            try
            {
                var data = await _productsDA.GetProductById(idCategory);
                if (data != null)
                {
                    result.ProductId = int.Parse(data.Element("ProductId").Value);
                    result.Description = data.Element("Description").Value;
                    result.ListPrice = double.Parse(data.Element("ListPrice").Value);
                    result.Category = data.Element("Category").Value;
                    result.IsImported = bool.Parse(data.Element("IsImported").Value);
                }
                else 
                {
                    result = null;
                }
            }
            catch (Exception exception)
            {
                result = null;
                _logger.LogError(exception.Message, idCategory);
            }

            return result;
        }

        public async Task<bool> Add(ProductRequestDto requestDto) 
        {
            var result = false;
            var productsModel = new ProductsModel();
            try
            {
                if (requestDto != null)
                {
                    
                    productsModel.ProductId = requestDto.ProductId;
                    productsModel.Description = requestDto.Description;
                    productsModel.ListPrice = requestDto.ListPrice;
                    productsModel.Category = requestDto.Category;
                    productsModel.IsImported = requestDto.IsImported;

                    result = await _productsDA.Add(productsModel);                    
                    
                }               
                
            }
            catch (Exception exception)
            {

                _logger.LogError(exception.Message, requestDto);
            }

            return result;
        }

        public async Task<bool> Update(ProductRequestDto requestDto)
        {
            var result = false;
            var productsModel = new ProductsModel();
            try
            {
                if (requestDto != null)
                {
                    productsModel.ProductId = requestDto.ProductId;
                    productsModel.Description = requestDto.Description;
                    productsModel.ListPrice = requestDto.ListPrice;
                    productsModel.Category = requestDto.Category;
                    productsModel.IsImported = requestDto.IsImported;

                    result = await _productsDA.Update(productsModel);
                }
            }
            catch (Exception exception)
            {

                _logger.LogError(exception.Message, requestDto);
            }

            return result;
        }

        public async Task<bool> Delete(string productId)
        {
            var result = false;

            try
            {
                result = await _productsDA.Delete(productId);
            }
            catch (Exception exception)
            {

                _logger.LogError(exception.Message, productId);
            }

            return result;
        }
    }
}
