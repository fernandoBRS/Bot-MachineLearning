using System;
using System.Threading;
using System.Threading.Tasks;
using AzureMLBot.FormFlow;
using AzureMLBot.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace AzureMLBot.Dialogs
{
    [Serializable]
    public class HybridVehicleDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("If you have any doubt during the form filling, type 'help'.");

            var formDialog = FormDialog.FromForm(BuildPredictionForm, FormOptions.PromptInStart);

            context.Call(formDialog, ResumeAfterFormDialog);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var message = await result;

                if (message.Text.ToLower().Contains("help"))
                {
                    await context.Forward(new SupportDialog(),
                        ResumeAfterSupportDialog, message, CancellationToken.None);
                }
                else
                {
                    //ShowOptions(context);
                    var engagementFormDialog =
                    FormDialog.FromForm(BuildPredictionForm, FormOptions.PromptInStart);

                    context.Call(engagementFormDialog, ResumeAfterFormDialog);
                }
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Too many invalid attempts, try again.");
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Thank you for contacting the support team. Your ticket number is {ticketNumber}.");

            context.Wait(MessageReceivedAsync);
        }

        private static IForm<HybridVehicleInput> BuildPredictionForm()
        {
            var formFlow = new HybridVehicleFormFlow();

            return formFlow.BuildForm();
        }

        private static async Task ResumeAfterFormDialog(IDialogContext context, IAwaitable<HybridVehicleInput> result)
        {
            try
            {
                var state = await result;

                // TODO: Integrate with Azure ML
                
            }
            catch (Exception ex)
            {
                string reply;

                if (ex.InnerException == null)
                {
                    reply = "Request cancelled.";
                }
                else
                {
                    reply = "Oops! Something wrong happened. :( \n\n" +
                            $"Technical details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }
    }
}