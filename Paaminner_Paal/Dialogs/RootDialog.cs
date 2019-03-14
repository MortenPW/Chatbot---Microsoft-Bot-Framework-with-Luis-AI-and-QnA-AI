using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PaaminnerPaal.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        // StartAsync is basically Dialog constructor
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);
        }

        // MessageReceived is Dialog main loop
        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            // Ensure a single request per message
            var json = ContextStates.Instance.Json = await Luis.Instance.RequestJson(message.Text);
            var intent = await Luis.Instance.JsonTopIntent(json);
            var entityTypes = await Luis.Instance.JsonTypes(json);

            // Say hello if Greeting entity found- allows us to greet and answer questions at once
            if (entityTypes.Contains("Greeting"))
                await context.PostAsync(BotMethods.Instance.RandomString(ConfigurationOptions.Instance.Greeting));

            // Route intent to correct context (context gets original intent from ContextStates json)
            if (!ContextStates.Instance.ContextIntent.Equals(""))
            {
                // If confirm or message contains entity types corresponding to context- send to intent
                if (entityTypes.Contains(ContextStates.Instance.ContextIntent)
                    || intent == "Deny"
                    || intent == "Cancel"
                    || intent == "Confirm")
                {
                    intent = ContextStates.Instance.ContextIntent; // Intent replaced with context (previous) for routing to correct dialog
                }
            }

            // Simple responses
            if (ConfigurationOptions.Instance.IntentResponses.ContainsKey(intent))
                await context.PostAsync(BotMethods.Instance.RandomString(ConfigurationOptions.Instance.IntentResponses[intent]));

            else
            {
                // Unique responses
                switch (intent)
                {
                    case "Greeting":
                        await BotMethods.Instance.AddCarousel(context, BotMethods.Instance.WelcomeMessage());
                        break;

                    case "AddItem":
                        SwitchContext(intent);
                        await context.Forward(new ItemDialog(), ResumeAfterDialog, context.Activity, CancellationToken.None);
                        break;

                    case "AddAllergy":
                        SwitchContext(intent);
                        await context.Forward(new AllergyDialog(), ResumeAfterDialog, context.Activity, CancellationToken.None);
                        break;

                    case "GetRecipe":
                        SwitchContext(intent);
                        await context.Forward(new RecipeDialog(), ResumeAfterDialog, context.Activity, CancellationToken.None);
                        break;

                    case "Help":
                        await context.PostAsync("Ulike ting jeg kan hjelpe deg med:" +
                                                "\n- besvare relevante spørsmål angående Kolonial.no" +
                                                "\n- finne oppskrifter til deg (\"finn taco oppskrift\" eller \"finn oppskrift\")" +
                                                "\n- advare deg dersom du legger til produkter som inneholder allergener du reagerer på" +
                                                "\n- legge til varer i handlekurven din for neste ordre eller vise deg varer" +
                                                "\n- gi deg gode råd" +
                                                "\n- fortelle deg en vits eller to" +
                                                "\n- Nb. dette er kun en beta- funksjonalitet har visse forbehold"
                        );

                        //await context.PostAsync("Vil du heller snakke med en menneskelig kollega? :)");
                        //await context.PostAsync("Legger til menneskelig kollega i samtalen du kan snakke videre med!");
                        break;

                    // Send to QnA
                    default:
                        await context.Forward(new QnaDialog(), ResumeAfterDialog, context.Activity, CancellationToken.None);
                        break;
                }
            }
            //context.Wait(MessageReceived); // Causes error when sending to QnA- afraid of feedback loop.
        }

        private void SwitchContext(string intent)
        {
            if (intent != ContextStates.Instance.ContextIntent)
            {
                ContextStates.Instance.ResetStates();
            }
            ContextStates.Instance.ContextIntent = intent;
        }

        private async Task ResumeAfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceived);
        }

        private async Task ResumeAfterDialogConfirm(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Er det noe mer jeg kan være behjelpelig med?");
            context.Wait(MessageReceived);
        }
    }
}