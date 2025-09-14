namespace ShoeShop.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public bool Status { get; set; }
        public bool Type {  get; set; } 
        public double Discount { get; set; }
        public int? Number { get; set; }
    }
}
