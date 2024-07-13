using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public int DocumentNo { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
    }

    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int Rate { get; set; }
        public int Amount { get; set; }

        // Foreign key
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public class Billing
    {
        [Key]
        public int BillingId { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }

        // Foreign key
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public class BillingViewModel
    {
        public int CustomerId { get; set; }
        public int DocumentNo { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public int BillingId { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public List<ProductViewModel> Products { get; set; }  // List of products
    }

    public class ProductViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int Rate { get; set; }
        public int Amount { get; set; }
    }

}
