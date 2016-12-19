using System;
using System.Threading.Tasks;
using AzureMLBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace AzureMLBot.Model
{
    [LuisModel(Constants.LuisId, Constants.LuisSubscriptionKey)]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        [LuisIntent("Predict")]
        public async Task PredictPdProgressAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ok! Just a moment please...");

            context.Call(new HybridVehicleDialog(), ResumeAfterOptionDialog);

            //context.Wait(MessageReceived);
        }

        [LuisIntent("None")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Desculpe, eu não entendi...");
            
            context.Wait(MessageReceived);
        }

        private static async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Error: {ex.Message}");
            }
            finally
            {
                context.Done<object>(null);
            }
        }
    }
}