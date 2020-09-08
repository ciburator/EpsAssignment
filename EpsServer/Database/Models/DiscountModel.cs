namespace Database.Models
{
    public class DiscountModel : BaseModel
    {
        public string Code { get; set; }
        public bool IsUsed { get; set; }
    }
}
