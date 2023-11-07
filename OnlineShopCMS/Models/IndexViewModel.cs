namespace OnlineShopCMS.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Product> Data { get; set; }
        public PaginatedList<Product> PageInfo { get; set; }
    }
}
