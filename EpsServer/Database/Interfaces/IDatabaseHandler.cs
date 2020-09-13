namespace Database.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface IDatabaseHandler
    {
        bool CheckProduct(string[] products);

        void GenerateCodes(HashSet<string> codes, string[] products);

        DiscountModel CheckDiscountCode(string discountCode);

        int UseCode(string discountCode);
    }
}
