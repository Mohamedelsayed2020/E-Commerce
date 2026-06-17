using E_Commerce.Models;
using E_Commerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicarionDbContext context;
        private readonly int pageSize = 8;
        public StoreController(ApplicarionDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index(int pageIndex, string? search, string? brand, string? category, string? sort)
        {
            IQueryable<Product> query = context.Products;

            // search logic
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }
            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(p => p.Brand == brand);
            }
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }
            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "price_asc":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case "name_asc":
                        query = query.OrderBy(p => p.Name);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(p => p.Name);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // default sorting by Id descending
                query = query.OrderByDescending(p => p.Id);
            }


            //query = query.OrderByDescending(p => p.Id);
            // pagination logic 
            if (pageIndex < 1)
            {
                pageIndex = 1;

            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var products = query.ToList();

            ViewBag.Products = products;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;
            var stotreSearchModel = new storeSearchModel()
            {
                Search = search,
                Brand = brand,
                Category = category,
                Sort = sort
            };
            return View(stotreSearchModel);
        }

        public IActionResult Details(int id)
        {
            var product = context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
               return RedirectToAction("Index","Store");
            }
            return View(product);
        }
    }
}
