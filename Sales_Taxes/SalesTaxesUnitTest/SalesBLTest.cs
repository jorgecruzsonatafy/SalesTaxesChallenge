using AutoFixture;
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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
//using Xunit;

namespace SalesTaxesUnitTest
{

    [TestClass]
    public class SalesBLTest
    {
        private readonly Fixture _fixture;
        private readonly ISalesBL _salesBL;
        private readonly Mock<IProductsBL> _productBL = new Mock<IProductsBL>();
        private readonly Mock<ILogger<SalesBL>> _logger = new Mock<ILogger<SalesBL>>();
        private readonly Mock<ISalesDA> _salesDA = new Mock<ISalesDA>();

        public SalesBLTest()
        {
            _fixture = new Fixture();
            _salesBL = new SalesBL(_productBL.Object, _salesDA.Object, _logger.Object) ;
        }

        [TestMethod]        
        public async Task AvailableProducts_Success() 
        {
            
            List<ProductsModel> products = new List<ProductsModel>();
            ProductsModel product = new ProductsModel() 
            {
                ProductId = 0,
                Description = "",
                ListPrice = 0,
                Category = "",
                IsImported = false
            };

            products.Add(product);

            _productBL.Setup(x => x.GetProducts().Result).Returns(products);

            var result = await _salesBL.AvailableProducts();

            Assert.IsNotNull(result);            

        }

        [TestMethod]
        public async Task AvailableProducts_Fail()
        {

            _productBL.Setup(x => x.GetProducts().Result).Throws(new ArgumentException());

            //_logger.Setup(x => x.LogError(It.IsAny<ArgumentException>(), typeof(SalesBL).FullName, "Unable Get Data"));

            var result = await _salesBL.AvailableProducts();

            Assert.ThrowsExceptionAsync<ArgumentException>(() =>   _salesBL.AvailableProducts());
        }

        [TestMethod]
        public async Task GetProductInfo_Success()
        {
                        
            ProductsModel product = new ProductsModel()
            {
                ProductId = 0,
                Description = "",
                ListPrice = 0,
                Category = "",
                IsImported = false
            };                       

            _productBL.Setup(x => x.GetProductById(It.IsAny<string>()).Result).Returns(product);

            var result = await _salesBL.GetProductInfo("TestProduct");

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public async Task GetProductInfo_Fail()
        {


            _productBL.Setup(x => x.GetProductById(It.IsAny<string>()).Result).Throws(new ArgumentException());

        
            Assert.ThrowsExceptionAsync<ArgumentException>(() => _salesBL.GetProductInfo("Test"));

        }

        [TestMethod]
        public async Task AddProducts_Taxes10_Success()
        {

            ProductRequestDto product = new ProductRequestDto()
            {
                ProductId = 10,
                Description = "Music CD",
                ListPrice = 100,
                Category = "",
                IsImported = false
            };


            var result = await _salesBL.AddProducts(product);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.currentProducts.Count);
            Assert.AreEqual(10, result.Taxes);
            Assert.AreEqual(110, result.Total);
        }

        [TestMethod]
        public async Task AddProducts_Taxes15_Success()
        {

            ProductRequestDto product = new ProductRequestDto()
            {
                ProductId = 10,
                Description = "Product",
                ListPrice = 14.99,
                Category = "",
                IsImported = true
            };


            var result = await _salesBL.AddProducts(product);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.currentProducts.Count);
            Assert.AreEqual(2.25, result.Taxes);
            Assert.AreEqual(17.24, result.Total);
        }

        [TestMethod]
        public async Task AddProducts_Taxes05_Success()
        {

            ProductRequestDto product = new ProductRequestDto()
            {
                ProductId = 1,
                Description = "Product",
                ListPrice = 11.25,
                Category = "Any",
                IsImported = true
            };


            var result = await _salesBL.AddProducts(product);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.currentProducts.Count);
            Assert.AreEqual(0.6, result.Taxes);
            Assert.AreEqual(11.85, result.Total);
        }

        [TestMethod]
        public async Task AddProducts_ToList_Success()
        {

            ProductRequestDto product = new ProductRequestDto()
            {
                ProductId = 10,
                Description = "Music CD",
                ListPrice = 100,
                Category = "",
                IsImported = false
            };

            var elements = new List<XElement>();
            var element= new XElement("Sale",
                       new XElement("ProductKey", product.ProductId.ToString().Trim()),
                       new XElement("Product", product.ProductId.ToString().Trim()),
                       new XElement("Quantity", "1"),
                       new XElement("Taxes", "10"),
                       new XElement("PriceWithTaxes", "110"),
                       new XElement("Total", "110")
                       );

            elements.Add(element);

            _salesDA.Setup(x => x.GetSales().Result).Returns(elements);
            await _salesBL.RetreivePurchasedProducts();

            var result = await _salesBL.AddProducts(product);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.currentProducts.Count);
            Assert.AreEqual(20, result.Taxes);
            Assert.AreEqual(220, result.Total);
        }


        [TestMethod]
        public async Task AddProducts_Fail()
        {

            ProductRequestDto product = new ProductRequestDto()
            {
                ProductId = 0,
                Description = null,
                ListPrice = 0,
                Category = null,
                IsImported = false
            };


            Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _salesBL.AddProducts(product));
           
        }

        [TestMethod]
        public async Task DeleteAll_Success()
        {

            _salesDA.Setup(x => x.DeleteAll().Result).Returns(true);

            var result = await _salesBL.DeletAll();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteAll_Fail()
        {

            _salesDA.Setup(x => x.DeleteAll().Result).Throws(new ArgumentException());


            Assert.ThrowsExceptionAsync<ArgumentException>(() => _salesBL.DeletAll());
        }

        [TestMethod]
        public async Task RmoveItmem_Success()
        {
            _salesDA.Setup(x => x.Delete(It.IsAny<string>()).Result).Returns(true);

            var result =await _salesBL.RemoveProduct(It.IsAny<string>());

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task RmoveItmem_Fail()
        {
            _salesDA.Setup(x => x.Delete(It.IsAny<string>()).Result).Throws(new ArgumentException());
                     

            Assert.ThrowsExceptionAsync<ArgumentException>(() => _salesBL.RemoveProduct(It.IsAny<string>()));
        }
    }
}
