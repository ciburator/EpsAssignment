namespace Database.Models
{
    public class DiscountModel : BaseModel
    {
        public string Code { get; set; }
        public int IsUsed { get; set; }
        public string[] ProductCodes { get; set; }
    }
}
