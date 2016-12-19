using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AzureMLBot.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Newtonsoft.Json;

namespace AzureMLBot.FormFlow
{
    public class HybridVehicleFormFlow
    {
        public IForm<HybridVehicleInput> BuildForm()
        {
            var builder = new FormBuilder<HybridVehicleInput>()
                .Field(nameof(HybridVehicleInput.ModelYear))
                .Field(nameof(HybridVehicleInput.AccelerationRate))
                .Field(nameof(HybridVehicleInput.Mpg))
                .Field(nameof(HybridVehicleInput.MaxMpgMpge))
                .Field(nameof(HybridVehicleInput.ModelClass))
                .Field(new FieldReflector<HybridVehicleInput>(nameof(HybridVehicleInput.ModelClass))
                    .SetType(null)
                    .SetAllowsMultiple(false)
                    .SetFieldDescription("model class")
                    .SetDefine(DefineModelClass))
                .OnCompletion(processScheduling);

            return builder.Build();
        }

        #region Engagement Form definition

        private static Task<bool> DefineModelClass(
            HybridVehicleInput state, Field<HybridVehicleInput> field)
        {
            field.AddDescription("1", "Compact").AddTerms("1", "Compact");
            field.AddDescription("2", "Midsize").AddTerms("2", "Midsize");
            field.AddDescription("3", "2 Seater, L and Large").AddTerms("3", "2 Seater, L and Large");
            field.AddDescription("4", "Pickup Truck").AddTerms("4", "Pickup Truck");
            field.AddDescription("5", "Minivan").AddTerms("5", "Minivan");
            field.AddDescription("6", "Sport Utility Vehicle (SUV)").AddTerms("6", "Sport Utility Vehicle (SUV)");

            return Task.FromResult(true);
        }
        
        #endregion

        #region Engagement Form validation

        readonly OnCompletionAsyncDelegate<HybridVehicleInput> processScheduling = async (context, state) =>
        {
            await context.PostAsync("Loading...");

            using (var client = new HttpClient())
            {
                // Replace this with the API key for the web service
                const string apiKey = "E7jRNGK7dVYEtynyWxUYOua3SoLO2/8mbOtnthjGbcTKiDWbTowVQwJtvzL8cUOXga6dAcmmvSN5CEa45ULekA==";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/subscriptions/0902f742ab9b4345a01cbb2bbd91fad2/services/32095e53901f4a82bd2c6c824ea8bb0f/execute?api-version=2.0&format=swagger");

                var scoreRequest = GetScoreRequest(state);
                var response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    // Avoiding deadlock with ConfigureAwait(false)
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    // Deserializing the JSON returned by the Azure ML web service
                    var hybridVehicleOutput = JsonConvert.DeserializeObject<HybridVehicleOutput>(result);

                    // Getting the output array
                    var output = hybridVehicleOutput.Results.Output;

                    // Getting the prediction value
                    var retailPrice = float.Parse(output[0].RetailPrice);

                    await context.PostAsync($"Suggested retail price: {retailPrice.ToString("c2")}");
                }
                else
                {
                    await context.PostAsync($"The request failed with status code: {response.StatusCode}");

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure

                    await context.PostAsync(response.Headers.ToString());

                    var responseContent = await response.Content.ReadAsStringAsync();

                    await context.PostAsync(responseContent);
                }
            }
        };

        private static object GetScoreRequest(HybridVehicleInput state)
        {
            return new
            {
                Inputs = GetInputs(state),
                GlobalParameters = new Dictionary<string, string>()
            };
        }

        private static Dictionary<string, List<Dictionary<string, string>>> GetInputs(HybridVehicleInput state)
        {
            return new Dictionary<string, List<Dictionary<string, string>>>
            {
                {
                    "input1",
                    GetInputValues(state)
                },
            };
        }

        private static List<Dictionary<string, string>> GetInputValues(HybridVehicleInput state)
        {
            return new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    {
                        "year", state.ModelYear
                    },
                    {
                        "accelrate", state.AccelerationRate
                    },
                    {
                        "mpg", state.Mpg
                    },
                    {
                        "mpgmpge", state.MaxMpgMpge
                    },
                    {
                        "carclass_id", state.ModelClass
                    }
                }
            };
        }

        private static Task<ValidateResult> ValidateStartDate(HybridVehicleInput state, object value)
        {
            var result = new ValidateResult
            {
                IsValid = true,
                Value = value
            };

            var startDate = (DateTime)value;

            DateTime formattedDt;

            if (DateTime.TryParse(startDate.ToString(new CultureInfo("pt-BR")), out formattedDt))
            {
                startDate = formattedDt;
            }

            if (startDate < DateTime.Today)
            {
                result.Feedback = "Data inicial inválida";
                result.IsValid = false;
            }

            return Task.FromResult(result);
        }
        
        #endregion
    }
}