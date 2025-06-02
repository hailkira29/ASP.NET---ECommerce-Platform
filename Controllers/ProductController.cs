using Microsoft.AspNetCore.Mvc;
using ECommercePlatform.Repositories;
using ECommercePlatform.Models;

namespace ECommercePlatform.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
