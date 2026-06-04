using LuckySpin.Dto;
using LuckySpin.Models;

namespace LuckySpin.DTO
{
    public class BillWithProductsDto
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string StoreId { get; set; }
        public string StoreLocate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
