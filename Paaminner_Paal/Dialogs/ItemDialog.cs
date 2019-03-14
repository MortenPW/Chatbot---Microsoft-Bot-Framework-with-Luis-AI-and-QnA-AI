using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace PaaminnerPaal.Dialogs
{
    [Serializable]
    public class ItemDialog : IDialog<object>
    {
        private Dictionary<string, string> _map;
        private const int NumberOfEntitiesInCarousel = 5;

        public async Task StartAsync(IDialogContext context)
        {
            var json = ContextStates.Instance.Json;

            var jsonKolonial = await Kolonial.Instance.RequestJson("productcategories/");
            _map = await Kolonial.Instance.JsonUnderCategories(jsonKolonial);

            // Change category if new category detected
            var types = await Luis.Instance.JsonTypes(json);
            if (types.Contains("AddItem"))
            {
                ContextStates.Instance.DisplayedEntities.Clear();
                ContextStates.Instance.Entities.Clear();
            }

            if (!await ContextIntents(context, json))
            {
                await RouteToMethods(context, true, json);
            }
        }

        private async Task<bool> ContextIntents(IDialogContext context, string json)
        {
            var intent = await Luis.Instance.JsonTopIntent(json);
            if (intent == "Confirm" && ContextStates.Instance.OnlyCancel == false)
            {
                if (_map.ContainsKey(ContextStates.Instance.Entities.First())) { }
                    await DialogCheck(context, _map[ContextStates.Instance.Entities.First()]);
                return true;
            }

            if (intent == "Deny" || intent == "Cancel")
            {
                await context.PostAsync("Den er god! :)"); // TODO: Legg til flere svar
                ContextStates.Instance.ResetStates();
                return true;
            }
            return false;
        }

        private async Task RouteToMethods(IDialogContext context, bool displayCategories, string json)
        {
            await BotMethods.Instance.GetUniqueEntities(json, "AddItem");

            if (ContextStates.Instance.Entities.Count <= 0)
            {
                if (displayCategories)
                    await DialogCategories(context);
            }
            else
            {
                await DialogCheck(context, _map[ContextStates.Instance.Entities.First()]);
            }
        }

        private async Task DialogCategories(IDialogContext context)
        {
            await BotMethods.Instance.AddList(context, "Ulike varekategorier å velge mellom: ", "", _map.Keys.ToArray());
            await context.PostAsync("Hvilken kategori vil du ha varer fra? :)");
            ContextStates.Instance.OnlyCancel = true;
        }

        private async Task DialogCheck(IDialogContext context, string id)
        {
            var jsonKolonial = await Kolonial.Instance.RequestJson("productcategories/" + id);
            var list = await Kolonial.Instance.JsonProducts(jsonKolonial);

            var card = context.MakeMessage();
            card.AttachmentLayout = "carousel";

            for (var i = 0; i < NumberOfEntitiesInCarousel; ++i)
            {
                if (list[0].Count == ContextStates.Instance.DisplayedEntities.Count) break;
                var random = new Random().Next(0, list[0].Count);

                if (!ContextStates.Instance.DisplayedEntities.Contains(random))
                {
                    card.Attachments.Add(BotMethods.Instance.AddItemCard(list[0][random], list[1][random], list[2][random], 
                        list[3][random], list[4][random], list[5][random], list[6][random], list[7][random], list[8][random], 
                        list[9][random], list[10][random]));
                    ContextStates.Instance.DisplayedEntities.Add(random);
                }
                else
                {
                    --i;
                }
            }
            await context.PostAsync(card);

            if (list[0].Count == ContextStates.Instance.DisplayedEntities.Count)
            {
                await context.PostAsync("Alle varer i denne kategorien er sett.");
                ContextStates.Instance.ResetStates();
                ContextStates.Instance.OnlyCancel = true;
            }
            else
            {
                // More
                await context.PostAsync(BotMethods.Instance.RandomString(ConfigurationOptions.Instance.More));
                ContextStates.Instance.OnlyCancel = false;
            }
        }
    }
}