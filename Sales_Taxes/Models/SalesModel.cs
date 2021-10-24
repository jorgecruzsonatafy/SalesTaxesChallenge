using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class SalesModel
    {
        [Display(Name = "Product Id: ")]
        [JsonProperty("prodcutId")]
        public int ProductId { get; set; }

        public IEnumerable<SelectListItem> ListProducts { get; set; }

        [Display(Name = "Description: ")]
        [JsonProperty ("description")]
        public string Description { get; set; }

        [Display(Name = "Category: ")]
        [JsonProperty("category")]
        public string Category { get; set; }

        [Display(Name = "Is Imported: ")]
        [JsonProperty("isImported")]
        public bool IsImported { get; set; }

        [Display(Name = "Price List: ")]
        [JsonProperty("price")]
        public double Price { get; set; }

    }
}
