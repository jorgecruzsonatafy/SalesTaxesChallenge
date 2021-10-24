using Newtonsoft.Json;


namespace Models.DTO
{
    public class ProductRequestDto
    {
        [JsonProperty("productId")]
        public  int ProductId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("listPrice")]
        public double ListPrice { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("isImported")]
        public bool IsImported { get; set; }
    }
}
