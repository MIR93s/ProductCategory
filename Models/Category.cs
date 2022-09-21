namespace ProductCategories.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}