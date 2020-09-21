using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepoUowAPI.Models;

namespace RepoMVC.Controllers
{
    public class ProductController : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Product> products = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44392/");
                //HTTP GET
                var responseTask = client.GetAsync("/api/Product/getproducts");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Product>>();
                    readTask.Wait();
                    products = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    products = Enumerable.Empty<Product>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(products);


        }


        public ActionResult create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult create(Product prod)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44392/");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<Product>("/api/Product/addproduct", prod);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(prod);
        }    
     }
}
