namespace GardensRu.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string Article { get; set; }
        public string ProductName { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; }
        public string Description { get; set; }
    }
}


