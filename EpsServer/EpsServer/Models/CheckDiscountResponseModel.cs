namespace EpsServer.Models
{
    public class CheckDiscountResponseModel
    {
        public int Action { get; set; }
        public string[] ProductCodes { get; set; }

        public CheckDiscountResponseModel(
            int action,
            string[] productCodes)
        {
            this.Action = action;
            this.ProductCodes = productCodes;
        }
    }
}
