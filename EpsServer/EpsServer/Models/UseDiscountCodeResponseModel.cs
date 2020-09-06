namespace EpsServer.Models
{
    public class UseDiscountCodeResponseModel
    {
        public int Result { get; set; }

        public UseDiscountCodeResponseModel(int result)
        {
            Result = result;
        }
    }
}
