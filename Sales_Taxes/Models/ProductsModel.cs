using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ProductsModel
    {
        [Required(ErrorMessage = "Id is mandatory")]
        [StringLength(10)]
        [Display(Name = "Product Id")]        
        public int ProductId { get; set; }
               
        [Required(ErrorMessage = "Description is mandatory")]
        [StringLength(100)]        
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is mandatory")]
        [StringLength(10)]
        [Display(Name = "List Price")]        
        public double ListPrice { get; set; }

        
        [StringLength(10)]
        [Display(Name = "Category")]        
        public string Category { get; set; }

       
        
        [Display(Name = "Imported")]        
        public bool IsImported { get; set; }
    }
}
