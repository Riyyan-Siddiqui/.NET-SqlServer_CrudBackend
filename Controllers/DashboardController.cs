using CrudBackend_1_.Data;
using CrudBackend_1_.Dto;
using CrudBackend_1_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudBackend_1_.Controllers
{
    [Route("Dashboard")]
    [Authorize] // This attribute ensures that only authenticated users can access the actions in this controller.
    public class DashboardController(AppDBContextcs context) : Controller
    {
        // GET: /Dashboard
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var list = await context.Products
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    Description = x.Description,
                    Price = x.Price,
                    Color = x.Color
                })
                .ToListAsync();

            return View(list);
        }


        // GET: /Dashboard/AddProductForm
        [HttpGet("AddProductForm")]
        public IActionResult AddProductForm()
        {
            return View("ProductForm", new ProductDto());
        }


        // POST: /Dashboard/CreateProduct
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(ProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.errorMessage = "Please fill the complete details!";
                return View("ProductForm", dto);
            }

            var newProduct = new Product
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                Color = dto.Color
            };

            context.Products.Add(newProduct);

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // POST: /Dashboard/DeleteProduct
        [HttpPost("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int productid)
        {
            var product = await context.Products
                .FirstOrDefaultAsync(x => x.Id == productid);

            if (product == null)
            {
                return NotFound();
            }

            context.Products.Remove(product);

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: /Dashboard/EditProduct/5
        [HttpGet("EditProduct/{productid}")]
        public async Task<IActionResult> EditProduct(int productid)
        {
            var data = await context.Products
                .Where(x => x.Id == productid)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    Description = x.Description,
                    Price = x.Price,
                    Color = x.Color
                })
                .FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }

            return View("ProductForm", data);
        }


        // POST: /Dashboard/EditProduct
        [HttpPost("EditProduct/{id}")]
        public async Task<IActionResult> EditProduct(ProductDto product)
        {
            if (!ModelState.IsValid)
            {
                return View("ProductForm", product);
            }

            var existingProduct = await context.Products
                .FirstOrDefaultAsync(x => x.Id == product.Id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Color = product.Color;

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}