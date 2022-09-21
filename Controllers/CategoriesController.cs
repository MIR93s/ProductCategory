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

namespace ProductCategories.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public CategoriesController(ApplicationDbContext context,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }
        [AllowAnonymous]
        // GET: Categories
        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new CategoryView();
            viewModel.Categories = await _context.Category
                .Include(c => c.ProductCategories)
                    .ThenInclude(c => c.Product)
                .ToListAsync();

            if (id != null)
            {
                ViewData["CategoryId"] = id.Value;
                Category category = viewModel.Categories.Where(
                     c => c.Id == id.Value).Single();
                viewModel.Products = category.ProductCategories.Select(s => s.Product);
            }


            return View(viewModel);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            var category = new Category
            {
                ProductCategories = new List<ProductCategory>()
            };
            return View(category);
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ProductCategories")] Category category)
        {
            var categories = from c in _context.Category
                             where c.Title == category.Title
                             select c;
            if (categories.Count() != 0)
            {
                ViewData["Message"] = "Category is already existed";
                return View(category);
            }


            if (ModelState.IsValid)
            {
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, category,
                                                                         Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category.Include(c => c.ProductCategories).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();

            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, category,
                                                                         Operations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ProductCategories")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            var categories = from c in _context.Category
                             where c.Title == category.Title && c.Id != id
                             select c;


            if (categories.Count() != 0)
            {
                ViewData["Message"] = "Category is already existed";
                return View(category);
            }

            if (ModelState.IsValid)
            {
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, category,
                                                                         Operations.Update);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, category,
                                                                         Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, category,
                                                                         Operations.Delete);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
                _context.Category.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
