using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PaaminnerPaal.Dialogs
{
    [Serializable]
    public class AddAllergyDialog : IDialog<object>
    {
        private int attempts;

        public async Task StartAsync(IDialogContext context)
        {
            // List of allergies
            // https://www.naaf.no/subsites/matallergi/kostrad-ved-allergi/14-allergifremkallende-ingredienser/

            attempts = 0;

            // Allergy menus
            var card = context.MakeMessage();
            var attachment = AddAllergyCard();
            var attachment2 = AddAllergyCard2();

            card.AttachmentLayout = "carousel";

            card.Attachments.Add(attachment);
            card.Attachments.Add(attachment2);
            await context.PostAsync(card);

            context.Wait(MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.ToLower().Contains("nei") || message.Text.ToLower().Contains("avbryt") || attempts >= 3)
            {
                context.Done<object>(null);
                return;
            }

            if (message.Text.ToLower().Contains("ja"))
            {
                await context.PostAsync("Det er notert. Noen flere?");
            }

            else
            {
                ++attempts;
                await context.PostAsync("Beklager, jeg kjenner ikke igjen det allergenet. Forsøk å skrive om...");
            }
            context.Wait(MessageReceived);
        }

        private static Attachment AddAllergyCard()
        {
            var heroCard = new HeroCard
            {
                Title = "Hvilke allergener skal jeg være obs på?",
                Subtitle = "Ved å legge inn allergier vil du få varsel fra meg dersom du forsøker å kjøpe et produkt som inneholder det aktuelle allergenet :)",
                Buttons = new List<CardAction>
                {
                    new CardAction("imBack", "Glutenholdig korn", value: "Glutenholdig korn"),
                    new CardAction("imBack", "Skalldyr", value: "Skalldyr"),
                    new CardAction("imBack", "Egg", value: "Egg"),
                    new CardAction("imBack", "Fisk", value: "Fisk"),
                    new CardAction("imBack", "Peanøtter", value: "Peanøtter"),
                    new CardAction("imBack", "Soya", value: "Soya"),
                    new CardAction("imBack", "Melk (herunder laktose)", value: "Melk"),
                }
            };
            return heroCard.ToAttachment();
        }

        private static Attachment AddAllergyCard2()
        {
            var heroCard = new HeroCard
            {
                Title = ":)",
                Subtitle = "............................................................................................................................................:)",
                Buttons = new List<CardAction>
                {
                    new CardAction("imBack", "Nøtter", value: "Nøtter"),
                    new CardAction("imBack", "Selleri", value: "Selleri"),
                    new CardAction("imBack", "Sennep", value: "Sennep"),
                    new CardAction("imBack", "Sesamfrø", value: "Sesamfrø"),
                    new CardAction("imBack", "Svoveldioksid og sulfitt", value: "Svoveldioksid og sulfitt"),
                    new CardAction("imBack", "Lupin", value: "Lupin"),
                    new CardAction("imBack", "Bløtdyr ", value: "Bløtdyr ")
                }
            };
            return heroCard.ToAttachment();
        }
    }
}