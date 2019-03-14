using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PaaminnerPaal.Dialogs;

namespace PaaminnerPaal
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class RootDialog_OLD : LuisDialog<object>
    {
        public RootDialog_OLD() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Gretting" with the name of your newly created intent in the following handler
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Cancel")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            // TODO: Fiks så man ikke blir låst til dialoger (og fjern 'avbryt')
            // TODO: https://docs.microsoft.com/en-us/bot-framework/dotnet/bot-builder-dotnet-scorable-dialogs

            // TODO: Also consider:
            // TODO: https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-formflow?view=azure-bot-service-3.0

            // TODO: Send inn velkomstmeny når man hilser, ie. skriver hei, hallo, etc

            // Add items
            if (message.Text.ToLower().Contains("legg til"))
            {
                // User said 'legg inn', so invoke the Add Item Dialog and wait for it to finish.
                // Then, call ResumeAfterAddItemDialog.
                //TODO: create ResumeAfterAddItemDialog
                await context.Forward(new AddItemDialog(), ResumeAfterAddAllergyDialog, message, CancellationToken.None);
                return;
            }

            // Add allergies
            if (message.Text.ToLower().Contains("legg inn allergi"))
            {
                // User said 'legg inn allergi', so invoke the Add Allergy Dialog and wait for it to finish.
                // Then, call ResumeAfterAddAllergyDialog.
                await context.Forward(new AddAllergyDialog(), ResumeAfterAddAllergyDialog, message, CancellationToken.None);
                return;
            }

            // Talk to customer support
            if (message.Text.ToLower().Contains("snakk med kundeser"))
            {
                await context.PostAsync("Den er god!");
                await context.PostAsync("Legger til menneskelig kollega i samtalen du kan snakke videre med :)");
            }

            // User typed something we can't interpret, inform and wait for input
            else
            {
                await context.PostAsync("Beklager, jeg forstår ikke hva du mener. Kan du omformulere det?");
            }

            context.Wait(MessageReceivedAsync);
        }

        private async Task ResumeAfterAddAllergyDialog(IDialogContext context, IAwaitable<object> result)
        {
            // Store the value that AddAllergyDialog returned. 
            // (At this point, add allergy dialog has finished and returned some value to use within the root dialog.)
            //var resultFromAddAllergy = await result;
            //await context.PostAsync($"Add allergy dialog just told me this: {resultFromAddAllergy}");

            await context.PostAsync("OK! Er det noe mer jeg kan være behjelpelig med?");

            // Again, wait for the next message from the user.
            context.Wait(MessageReceivedAsync);
        }
    }
}