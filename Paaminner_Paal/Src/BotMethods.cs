using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PaaminnerPaal.Dialogs;

namespace PaaminnerPaal
{
    [Serializable]
    public sealed class BotMethods
    {
        private static readonly Lazy<BotMethods> Lazy =
            new Lazy<BotMethods>(() => new BotMethods());

        private BotMethods()
        {
        }

        public static BotMethods Instance => Lazy.Value;

        public string RandomString(string[] text)
        {
            return text[new Random().Next(0, text.Length)];
        }

        public async Task AddList(IDialogContext context, string topText, string bottomText, string[] list)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(topText);

            foreach (var entity in list)
            {
                stringBuilder.Append("\n- " + entity);
            }

            stringBuilder.Append(bottomText);
            await context.PostAsync(stringBuilder.ToString());
        }

        public async Task AddCarousel(IDialogContext context, Attachment attachment)
        {
            // Add item carousel
            var card = context.MakeMessage();

            card.AttachmentLayout = "carousel";
            card.Attachments.Add(attachment);
            await context.PostAsync(card);
        }

        // Message to display when starting conversation
        public Attachment WelcomeMessage()
        {
            // Card actions:
            // https://docs.botframework.com/en-us/node/builder/chat-reference/classes/_botbuilder_d_.cardaction.html#postback

            // Card image
            var cardImages = new List<CardImage>
            {
                new CardImage(ConfigurationOptions.Instance.RootUri + "paal.png")
            };

            // Buttons array
            var cardButtons = new List<CardAction>();

            // Button kolonial.no
            var kolonialButton = new CardAction
            {
                Title = "Gå til Kolonial.no",
                Type = "openUrl",
                Value = "https://kolonial.no/"
            };

            // Buttons
            var varerButton = new CardAction { Title = "Legg til varer", Type = "imBack", Value = "Legg til varer" };
            var allergierButton = new CardAction { Title = "Legg inn allergier", Type = "imBack", Value = "Legg inn allergier" };
            var oppskrifterButton = new CardAction { Title = "Finn oppskrift", Type = "imBack", Value = "Finn oppskrift" };
            var showListButton = new CardAction { Title = "Vis liste", Type = "imBack", Value = "Vis liste" };
            var kundeserviceButton = new CardAction
            {
                //Title = "Snakk med kundeservice",
                Title = "Hjelp",
                Type = "imBack",
                Value = "Hjelp"
            };

            // Add buttons
            cardButtons.Add(varerButton);
            cardButtons.Add(allergierButton);
            cardButtons.Add(oppskrifterButton);
            cardButtons.Add(showListButton);
            cardButtons.Add(kundeserviceButton);
            cardButtons.Add(kolonialButton);

            // Hero card
            var heroCard = new HeroCard
            {
                Title = "Hei, jeg er Påminner-Pål! Hva kan jeg hjelpe deg med?",
                Subtitle = "Jeg lærer om dine preferanser og behov jo bedre kjent vi blir!",
                //Subtitle = "Bruk meg så jeg blir kjent med deg, da får jeg hjulpet deg bedre!",
                Images = cardImages,
                Buttons = cardButtons
            };
            return heroCard.ToAttachment();
        }

        public Attachment AddRecipeCard(string title, string difficulty_string, 
            string cooking_duration_string, string front_url, string feature_image_url)
        {
            /*
            var card = context.MakeMessage();
            var attachment = AddAllergyCard();

            card.AttachmentLayout = "list";
            card.Attachments.Add(attachment);
            await context.PostAsync(card);
            */

            var cardImages = new List<CardImage>
            {
                new CardImage(feature_image_url)
            };

            var button = new CardAction
            {
                Title = "Gå til oppskrift",
                Type = "openUrl",
                Value = front_url
            };

            var cardButtons = new List<CardAction> {button};

            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = "Vanskelighetsgrad: " + difficulty_string + ". " +
                           "Beregnet tid: " + cooking_duration_string + ".",
                Images = cardImages,
                Buttons = cardButtons
            };
            return heroCard.ToAttachment();
        }

        public Attachment AddItemCard(string id, string fullName, string brand, string frontUrl, string image, string grossPrice, string grossUnitPrice,
            string unitPriceQuantityAbbreviation, string discount, string promotion, string availability)
        {
            var cardImages = new List<CardImage>
            {
                new CardImage(image)
            };

            var button = new CardAction
            {
                Title = "Gå til vare",
                Type = "openUrl",
                Value = frontUrl
            };

            var cardButtons = new List<CardAction> { button };

            var heroCard = new HeroCard
            {
                Title = fullName,
                Subtitle = "Pris: Kr " + grossPrice + ". (Kr " + grossUnitPrice + " per " + unitPriceQuantityAbbreviation + "). Merke: " + brand + ".",
                Images = cardImages,
                Buttons = cardButtons
            };
            return heroCard.ToAttachment();
        }

        public Attachment AddHeroCard()
        {
            /*
            var card = context.MakeMessage();
            var attachment = AddAllergyCard();

            card.AttachmentLayout = "list";
            card.Attachments.Add(attachment);
            await context.PostAsync(card);
            */

            var actions = ConfigurationOptions.Instance.Allergies.Select(entity => new CardAction("imBack", entity, entity)).ToList();
            var heroCard = new HeroCard
            {
                Title = "Hvilke allergener skal jeg være obs på?",
                Subtitle = "Ved å legge inn allergier vil du få varsel fra meg dersom du forsøker å kjøpe " +
                           "et produkt som inneholder det aktuelle allergenet (du kan selvsagt skrive flere) :)",
                Buttons = actions
            };
            return heroCard.ToAttachment();
        }

        // Dialog architecture methods
        public async Task<List<string>> GetUniqueEntities(string json, string type)
        {
            var list = await Luis.Instance.JsonEntities(json);
            var types = await Luis.Instance.JsonTypes(json);

            for (var i = 0; i < list.Count; ++i)
            {
                if (type != null && types[i] != type) continue;
                if (!ContextStates.Instance.Entities.Contains(list[i]))
                {
                    ContextStates.Instance.Entities.Add(list[i]);
                }
            }
            return list;
        }
    }
}