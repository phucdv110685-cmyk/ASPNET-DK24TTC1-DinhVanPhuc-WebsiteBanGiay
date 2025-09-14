namespace ShoeShop.Helpers
{
    public class VnPayRequest
    {
        public string vnp_Version { get; set; } = "2.1.0";
        public string vnp_Command { get; set; } = "pay";
        public string vnp_TmnCode { get; set; }
        public string vnp_Amount { get; set; }
        public string vnp_CurrCode { get; set; } = "VND";
        public string vnp_TxnRef { get; set; }
        public string vnp_OrderInfo { get; set; }
        public string vnp_Locale { get; set; } = "vn";
        public string vnp_ReturnUrl { get; set; }
        public string vnp_IpAddr { get; set; }
        public string vnp_CreateDate { get; set; }
        public string vnp_ExpireDate { get; set; }
    }
}
