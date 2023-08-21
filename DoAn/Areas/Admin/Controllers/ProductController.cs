﻿using DoAn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace DoAn.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        DlctContext db = new DlctContext();
        private readonly HttpClient _httpClient;
        public ProductController()
        {
            _httpClient = new HttpClient();
        }

        //button choose image
        [HttpPost]
        public IActionResult ProcessUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json("No file uploaded");
            }

            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return Json("/images/" + fileName);
        }

        //edit
        [HttpGet]
        public IActionResult Edit(int productId)
        {
            var product = db.Products.Find(productId);
            var ProductType = db.Producttypes.ToList();
            var providers = db.Providers.ToList();

            ViewBag.ProductType = new SelectList(ProductType, "ProductTypeId", "Name");
            ViewBag.providers = new SelectList(providers, "ProviderId", "Name");
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int productId, Product updateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(updateModel);
            }
            var apiUrl = $"https://localhost:7109/api/ProductApi/update/{productId}";

            var json = JsonConvert.SerializeObject(updateModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response Content: " + responseContent);

                var errorResponse = JsonConvert.DeserializeObject<object>(responseContent);

                ModelState.AddModelError("", errorResponse.ToString());
                return View(updateModel);
            }
        }

        //create
        public IActionResult Create()
        {
            var ProductType = db.Producttypes.ToList();
            var providers = db.Providers.ToList();

            ViewBag.ProductType = new SelectList(ProductType, "ProductTypeId", "Name");
            ViewBag.providers = new SelectList(providers, "ProviderId", "Name");

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product registrationModel)
        {
            var apiUrl = "https://localhost:7109/api/ProductApi/create";

            var json = JsonConvert.SerializeObject(registrationModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response Content: " + responseContent);


                var errorResponse = JsonConvert.DeserializeObject<object>(responseContent);

                ModelState.AddModelError("", errorResponse.ToString());
                return View(registrationModel);
            }
        }
        //View List
        public async Task<IActionResult> Index()
        {

            var apiResponse = await _httpClient.GetAsync("https://localhost:7109/api/ProductApi/");
            if (apiResponse.IsSuccessStatusCode)
            {
                var responseContent = await apiResponse.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(responseContent);

                return View(products);
            }
            else
            {
                var productsList = await db.Products
                   .Include(s => s.ProductType)
                   .Include(s => s.Provider)
                   .ToListAsync();
                return View(productsList);
            }
        }
    }
}
