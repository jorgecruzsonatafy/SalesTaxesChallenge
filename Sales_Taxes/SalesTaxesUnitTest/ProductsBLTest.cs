using Business_Logic;
using Business_Logic.Contracts;
using Data_Access.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Models.DTO;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SalesTaxesUnitTest
{
    [TestClass]
    public class ProductsBLTest
    {
        private readonly IProductsBL _productsBL;
        public readonly Mock<IProductsDA> _productsDA = new Mock<IProductsDA>();
        private readonly Mock<ILogger<ProductsBL>> _logger = new Mock<ILogger<ProductsBL>>();

        public ProductsBLTest()
        {
            _productsBL = new ProductsBL(_productsDA.Object, _logger.Object);
        }

        [TestMethod]
        public async Task GetProducts_Success() 
        {
            var elements = new List<XElement>();
            var element = new XElement("Products",                       
                       new XElement("ProductId", "1"),
                       new XElement("Description", "Description product"),
                       new XElement("ListPrice", "10"),
                       new XElement("Category", ""),
                       new XElement("IsImported", "false")
                       );

            elements.Add(element);

            _productsDA.Setup(x => x.GetProducts().Result).Returns(elements);

            var result = await _productsBL.GetProducts();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task GetProducts_Fail()
        {
            _productsDA.Setup(x => x.GetProducts().Result).Throws(new ArgumentException());

            //_logger.Setup(x => x.LogError(new ArgumentException(),  "Unable Get Data", string.Empty));

            Assert.ThrowsExceptionAsync<ArgumentException>(() => _productsBL.GetProducts());
            
        }

        [TestMethod]
        public async Task GetProductById_Success()
        {            
            var element = new XElement("Products",
                       new XElement("ProductId", "1"),
                       new XElement("Description", "Description product"),
                       new XElement("ListPrice", "10"),
                       new XElement("Category", ""),
                       new XElement("IsImported", "false")
                       );
            
            _productsDA.Setup(x => x.GetProductById(It.IsAny<string>()).Result).Returns(element);

            var result = await _productsBL.GetProductById(It.IsAny<string>());

            Assert.IsNotNull(result);
            
        }

        [TestMethod]
        public async Task GetProductById_Fail()
        {
            
            _productsDA.Setup(x => x.GetProductById(It.IsAny<string>()).Result).Throws(new ArgumentException());
                       

            Assert.ThrowsExceptionAsync<ArgumentException>(() => _productsBL.GetProductById(It.IsAny<string>()));

        }

        [TestMethod]
        public async Task AddProduct_Success()
        {
            var productRequestDto = new ProductRequestDto() 
            {
                ProductId = 123,
                Description = "",
                ListPrice = 150.0,
                Category = "",
                IsImported = false
            };

            _productsDA.Setup(x => x.Add(It.IsAny<ProductsModel>()).Result).Returns(true);

            var result = await _productsBL.Add(productRequestDto);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task AddProduct_Fail()
        {
            var productRequestDto = new ProductRequestDto()
            {
                ProductId = 123,
                Description = "",
                ListPrice = 150.0,
                Category = "",
                IsImported = false
            };

            _productsDA.Setup(x => x.Add(It.IsAny<ProductsModel>()).Result).Throws( new ArgumentException());
                        
            Assert.ThrowsExceptionAsync<ArgumentException>(() => _productsBL.Add(productRequestDto));
        }

        [TestMethod]
        public async Task UpdateProduct_Success()
        {
            var productRequestDto = new ProductRequestDto()
            {
                ProductId = 123,
                Description = "",
                ListPrice = 150.0,
                Category = "",
                IsImported = false
            };

            _productsDA.Setup(x => x.Update(It.IsAny<ProductsModel>()).Result).Returns(true);

            var result = await _productsBL.Update(productRequestDto);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UpdateProduct_Fail()
        {
            var productRequestDto = new ProductRequestDto()
            {
                ProductId = 123,
                Description = "",
                ListPrice = 150.0,
                Category = "",
                IsImported = false
            };

            _productsDA.Setup(x => x.Update(It.IsAny<ProductsModel>()).Result).Throws(new ArgumentException());

            Assert.ThrowsExceptionAsync<ArgumentException>(() => _productsBL.Update(productRequestDto));
        }

        [TestMethod]
        public async Task DeleteProduct_Success()
        {
            _productsDA.Setup(x => x.Delete(It.IsAny<string>()).Result).Returns(true);

            var result = await _productsBL.Delete(It.IsAny<string>());

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteProduct_Fail()
        {
            _productsDA.Setup(x => x.Delete(It.IsAny<string>()).Result).Throws(new ArgumentException());
                       

            Assert.ThrowsExceptionAsync<ArgumentException>(() =>  _productsBL.Delete(It.IsAny<string>()));
        }

    }
}
