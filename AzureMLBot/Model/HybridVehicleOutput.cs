using Newtonsoft.Json;

namespace AzureMLBot.Model
{
    public class HybridVehicleOutput
    {
        public Results Results { get; set; }
    }

    public class Results
    {
        [JsonProperty(PropertyName = "output1")]
        public Output[] Output { get; set; }
    }

    public class Output
    {
        [JsonProperty(PropertyName = "retail_price")]
        public string RetailPrice { get; set; }
    }
}
