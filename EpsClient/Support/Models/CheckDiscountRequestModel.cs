namespace Support.Models
{
    public class CheckDiscountRequestModel
    {
        public string CardNumber { get; set; }

        public CheckDiscountRequestModel(string cardNumber)
        {
            this.CardNumber = cardNumber;
        }
    }
}
