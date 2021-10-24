using Business_Logic.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using Sales_Taxes.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;



namespace Sales_Taxes.Controllers
{
    public class SalesController : Controller
    {
        public readonly ISalesBL _salesBL;
        public SalesController(ISalesBL salesBL)
        {
            _salesBL = salesBL;
        }

        public async Task<IActionResult> Index()
        {
            SalesModel salesModel = new SalesModel();

            salesModel.ListProducts = await _salesBL.AvailableProducts();

            await _salesBL.DeletAll();

            return View("SalesIndex",salesModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetProductInfo(string productId)
        {
                      
            var salesModel = await _salesBL.GetProductInfo(productId);

            if (salesModel != null)
            {
                return Json(new { Success = true, description = salesModel.Description, category = salesModel.Category, isImported = salesModel.IsImported, price = salesModel.Price });
            }
            else
            {
                return Json(new { Success = false });
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductRequestDto requestDto)
        {
            var response = new ResponseModel();
           
            if (requestDto.ProductId> 0 && requestDto.ListPrice>0 )
            {
                response = await _salesBL.AddProducts(requestDto);
            }           

            return PartialView("_detailSales", response);
        }

        [HttpPost]       
        public async Task<IActionResult> RemoveProduct(string id)
        {

            var response = await _salesBL.RemoveProduct(id);
            
            return PartialView("_detailSales", response);
           

        }

        [HttpGet]
        public async Task<IActionResult> NewSale()
        {
            var response = new ResponseModel();

            await _salesBL.DeletAll();
            return PartialView("_detailSales", response);
        }


    }
}
