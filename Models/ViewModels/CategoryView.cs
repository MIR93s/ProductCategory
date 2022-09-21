namespace ProductCategories.Models.ViewModels
{
    public class CategoryView
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
