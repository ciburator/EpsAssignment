namespace EpsServer.Models
{
    public class UseDiscountCodeRequestModel
    {
        public string Code { get; set; }

        public UseDiscountCodeRequestModel(string code)
        {
            Code = code;
        }
    }
}
