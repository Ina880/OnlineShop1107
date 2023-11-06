using System.ComponentModel.DataAnnotations;

namespace OnlineShopCMS.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string? Name { get; set; }            //商品名稱
        public string? Description { get; set; }     //商品簡介
        public string? Content { get; set; }         //商品內容
        public int? Price { get; set; }              //商品價格
        public int? Stock { get; set; }              //商品庫存
        public byte[]? Image { get; set; }           //商品圖片

        [Display(Name = "Category")]
        public int CategoryId { get; set; }         //類別 (Foreign Key)
        public Category? Category { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }            //類別名稱
        public List<Product> Products { get; set; }
    }

}

