using ProductCategories.Data;
using ProductCategories.Models;
using ProductCategories.Models.ViewModels;
using System.Collections.Generic;

namespace ProductCategories.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<AssignedCategory> PopulateAssignedCategoryData(ProductCategories.Models.Product product)
        {


            var allCategory = _context.Category;
            var productCategory = new HashSet<int>(product.ProductCategories.Select(c => c.CategoryId));
            var viewModel = new List<AssignedCategory>();
            foreach (var category in allCategory)
            {
                viewModel.Add(new AssignedCategory
                {
                    CategoryId = category.Id,
                    CategoryName = category.Title,
                    IsAssigned = productCategory.Contains(category.Id)
                });
            }
            return viewModel;
            //ViewData["Category"] = viewModel;
        }

        public void UpdateProductCategory(string[] selectedCategories, Models.Product product)
        {
            if (selectedCategories == null)
            {
                product.ProductCategories = new List<ProductCategory>();
                return;
            }
            var selectedCategoriesHS = new HashSet<string>(selectedCategories);
            var productCategories = new HashSet<int>(product.ProductCategories.Select(c => c.Category.Id));
            var categories = _context.Category;
            foreach (var category in categories)
            {
                if (selectedCategoriesHS.Contains(category.Id.ToString()))
                {
                    if (!productCategories.Contains(category.Id))
                    {
                        product.ProductCategories.Add(new ProductCategory
                        {
                            ProductId = product.Id,
                            CategoryId = category.Id
                        });
                    }
                }
                else
                {
                    if (productCategories.Contains(category.Id))
                    {
                        ProductCategory categoryToRemove = product.ProductCategories.FirstOrDefault(c => c.CategoryId == category.Id);
                        _context.ProductCategory.Remove(categoryToRemove);
                    }
                }
            }
        }
    }
}
