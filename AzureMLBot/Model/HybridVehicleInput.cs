using System;
using Microsoft.Bot.Builder.FormFlow;

namespace AzureMLBot.Model
{
    [Serializable]
    public class HybridVehicleInput
    {
        [Prompt("What is the model year?")]
        public string ModelYear { get; set; }

        [Prompt("What is the acceleration rate? (km/hour/second)")]
        public string AccelerationRate { get; set; }

        [Prompt("What is the fuel economy (MPG)? (miles/gallon)")]
        public string Mpg { get; set; }

        [Prompt("What is the max value of MPG and MPGe?")]
        public string MaxMpgMpge { get; set; }

        [Prompt("What is the Model class?")]
        public string ModelClass { get; set; }
    }
}