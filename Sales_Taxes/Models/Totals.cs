

namespace Models
{
    public class Totals
    {
        public string ProductKey { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public double PriceWithTaxes { get; set; }
        public double Taxes { get; set; }
        public double Total { get; set; }
    }
}
