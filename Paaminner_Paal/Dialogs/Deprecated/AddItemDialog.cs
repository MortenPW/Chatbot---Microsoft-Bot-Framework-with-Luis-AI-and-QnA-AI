using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PaaminnerPaal.Dialogs
{
    [Serializable]
    public class AddItemDialog : IDialog<object>
    {
        private int attempts;

        public async Task StartAsync(IDialogContext context)
        {
            attempts = 0;

            await context.PostAsync(
                "Hensikten med denne funksjonen er at man skal kunne legge inn varer i handlekurven " +
                "slik man ville skrevet en handleliste på en lapp. Kan det bli enklere?");

            // Add item carousel
            var card = context.MakeMessage();
            var attachment = AddItemCard();
            var attachment2 = AddItemCard2();
            var attachment3 = AddItemCard3();

            card.AttachmentLayout = "carousel";

            card.Attachments.Add(attachment);
            card.Attachments.Add(attachment2);
            card.Attachments.Add(attachment3);
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
                await context.PostAsync("Det er notert. Noen flere?");

            else
                ++attempts;

            context.Wait(MessageReceived);
        }

        private static Attachment AddItemCard()
        {
            // Card image
            var cardImages = new List<CardImage>
            {
                new CardImage(ConfigurationOptions.Instance.RootUri + ConfigurationOptions.Instance.ImgDir +
                              "items/32-ef8d1-product_list.jpg")
            };

            var heroCard = new HeroCard
            {
                Title = "Coca-Cola",
                Subtitle = "1,5 l Coca-Cola",
                Text = "kr 26.90",
                Images = cardImages,
                Buttons = new List<CardAction>
                {
                    new CardAction("imBack", "Fjern", value: "Fjern"),
                    new CardAction("imBack", "Andre alternativer (endre)", value: "Endre")
                }
            };
            return heroCard.ToAttachment();
        }

        private static Attachment AddItemCard2()
        {
            // Card image
            var cardImages = new List<CardImage>
            {
                new CardImage(ConfigurationOptions.Instance.RootUri + ConfigurationOptions.Instance.ImgDir +
                              "items/632804-91096-product_list.jpg")
            };

            var heroCard = new HeroCard
            {
                Title = "Tomtegløgg",
                Subtitle = "1 l Tomtegløgg",
                Text = "kr 69.90",
                Images = cardImages,
                Buttons = new List<CardAction>
                {
                    new CardAction("imBack", "Fjern", value: "Fjern"),
                    new CardAction("imBack", "Andre alternativer (endre)", value: "Endre")
                }
            };
            return heroCard.ToAttachment();
        }

        private static Attachment AddItemCard3()
        {
            // Card image
            var cardImages = new List<CardImage>
            {
                new CardImage(ConfigurationOptions.Instance.RootUri + ConfigurationOptions.Instance.ImgDir +
                              "items/645763-65125-product_list.jpg")
            };

            var heroCard = new HeroCard
            {
                Title = "Pinnekjøtt Av Lam",
                Subtitle = "1 kg Urøkt",
                Text = "kr 99.90",
                Images = cardImages,
                Buttons = new List<CardAction>
                {
                    new CardAction("imBack", "Fjern", value: "Fjern"),
                    new CardAction("imBack", "Andre alternativer (endre)", value: "Endre")
                }
            };
            return heroCard.ToAttachment();
        }
    }
}