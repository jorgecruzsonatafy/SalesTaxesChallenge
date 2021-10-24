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
    public class ProductsController : Controller
    {
        public readonly IProductsBL _productsBL;
        public ProductsController(IProductsBL productsBL)
        {
            _productsBL = productsBL;
        }

     

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var model = await _productsBL.GetProducts();

            return View("ProductsIndex", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ViewProduct(string id)
        {
            var model = new ProductsModel();
            var operation = "Add";
            if (! string.IsNullOrEmpty(id))
            {
                model = await _productsBL.GetProductById(id);
                operation = "Update";
            }
            ViewBag.Update = operation;
            
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> InsertProduct(ProductRequestDto requestDto)
        {
            
            var result = false;
            var message = string.Empty;
          
            if (ModelState.IsValid)
            {
                var data = await _productsBL.GetProductById(requestDto.ProductId.ToString());
                if (data == null)
                {
                    if (await _productsBL.Add(requestDto))
                    {
                        result = true;
                        message = "Successfully saved";
                    }
                    else
                    {
                        result = false;
                        message = "Error while saving data";
                    }
                }
                else 
                {
                    result = false;
                    message = "Product Id already exist";
                }
                
            }
            return Json(new { success = result, msg = message });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteProduct(string id)
        {

            if (await _productsBL.Delete(id))
            {
                var model = await _productsBL.GetProducts();

                return View("ProductsIndex", model);
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }          
           
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Update(ProductRequestDto requestDto)
        {

            var result = false;
            var message = string.Empty;

            if (ModelState.IsValid)
            {

                if (await _productsBL.Update(requestDto))
                {
                    result = true;
                    message = "Successfully saved";
                }
                else
                {
                    result = false;
                    message = "Error while saving data";
                }



            }
            return Json(new { success = result, msg = message });
        }
    }
}
