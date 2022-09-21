using ProductCategories.Models;
using ProductCategories.Models.ViewModels;

namespace ProductCategories.Services
{
    public interface IProductService
    {
        List<AssignedCategory> PopulateAssignedCategoryData(ProductCategories.Models.Product product);

        void UpdateProductCategory(string[] selectedCategories, Models.Product product);

    }
}
