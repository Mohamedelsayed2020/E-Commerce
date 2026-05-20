using E_Commerce.Models;
using E_Commerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicarionDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicarionDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending(p => p.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image is required");
            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            // save the image 

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName = newFileName + Path.GetExtension(productDto.ImageFile.FileName);
            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }
            Product product = new Product
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now
            };
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            var productDto = new ProductDto
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description
            };
            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt;

            return View(productDto);
        }
        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt;
                return View(productDto);
            }
            if (productDto.ImageFile != null)
            {
                // save the new image 
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName = newFileName + Path.GetExtension(productDto.ImageFile.FileName);
                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }
                // delete the old image

                var productsDir = Path.Combine(environment.WebRootPath, "products");

                if (!string.IsNullOrWhiteSpace(product.ImageFileName))
                {
                    var oldImageFullPath = Path.Combine(productsDir, product.ImageFileName);
                    // Ensure we are deleting a file and it exists
                    if (System.IO.File.Exists(oldImageFullPath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldImageFullPath);
                        }
                        catch (Exception ex)
                        {
                            // log the exception (do not reveal to user)
                            // e.g. _logger.LogWarning(ex, "Failed to delete old product image: {Path}", oldImageFullPath);
                        }
                    }
                }
                product.ImageFileName = newFileName;
            }
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            context.Products.Update(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }
        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            // delete the image

            var productsDir = Path.Combine(environment.WebRootPath, "products");

            if (!string.IsNullOrWhiteSpace(product.ImageFileName))
            {
                var imageFullPath = Path.Combine(productsDir, product.ImageFileName);
                // Ensure we are deleting a file and it exists
                if (System.IO.File.Exists(imageFullPath))
                {
                    try
                    {
                        System.IO.File.Delete(imageFullPath);
                    }
                    catch (Exception ex)
                    {
                        // log the exception (do not reveal to user)
                        // e.g. _logger.LogWarning(ex, "Failed to delete product image: {Path}", imageFullPath);
                    }
                }
            }
            context.Products.Remove(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Products");
        }


        }

}
