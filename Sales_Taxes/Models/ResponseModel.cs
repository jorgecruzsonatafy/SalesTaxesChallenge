using System.Collections.Generic;

namespace Models
{
    public class ResponseModel
    {
        public double Taxes { get; set; }

        public double Total { get; set; }

        public List<Totals> currentProducts { get; set; }
    }
}
