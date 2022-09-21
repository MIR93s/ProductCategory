using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductCategories.Authorization;
using ProductCategories.Data;
using ProductCategories.Models;
using ProductCategories.Models.ViewModels;
using ProductCategories.Services;

namespace ProductCategories.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;
        private readonly IAuthorizationService _authorizationService;

        public ProductsController(ApplicationDbContext context,
            IProductService productService,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _productService = productService;
            _authorizationService = authorizationService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Product
                .Include(p => p.ProductCategories).ThenInclude(p => p.Category)
                .ToListAsync();
              return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var product = new ProductCategories.Models.Product()
            {
                ProductCategories = new List<ProductCategory>()
            };
            ViewData["Category"] = _productService.PopulateAssignedCategoryData(product);
            return View(product);
        }       

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string[] selectedCategory, [Bind("Id,Name,ProductCategories")] ProductCategories.Models.Product product)
        {
            if (selectedCategory != null)
            {
                foreach (var category in selectedCategory)
                {
                    var categoryToAdd = new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = int.Parse(category)
                    };
                    product.ProductCategories.Add(categoryToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, product,
                                                                              Operations.Create);

                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductCategories).ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, product,
                                                                             Operations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            ViewData["Category"] = _productService.PopulateAssignedCategoryData(product);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedCategory)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productToUpdate = await _context.Product
               .Include(p => p.ProductCategories).ThenInclude(p => p.Category)
               .FirstOrDefaultAsync(c => c.Id == id);

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, productToUpdate,
                                                                             Operations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            if (await TryUpdateModelAsync<ProductCategories.Models.Product>(
                 productToUpdate,
               "",
                c => c.Name ))
            {
                try
                {
                    _productService.UpdateProductCategory(selectedCategory, productToUpdate);
                    _context.Update(productToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(productToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            _productService.UpdateProductCategory(selectedCategory, productToUpdate);
            return View(productToUpdate);
        }

       
        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
               .Include(p => p.ProductCategories).ThenInclude(p => p.Category)
               .FirstOrDefaultAsync(c => c.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, product,
                                                                         Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var product = await _context.Product
              .Include(p => p.ProductCategories).ThenInclude(p => p.Category)
              .FirstOrDefaultAsync(c => c.Id == id);

            if (product != null)
            {
                _context.Product.Remove(product);
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, product,
                                                                         Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return _context.Product.Any(e => e.Id == id);
        }
    }
}
