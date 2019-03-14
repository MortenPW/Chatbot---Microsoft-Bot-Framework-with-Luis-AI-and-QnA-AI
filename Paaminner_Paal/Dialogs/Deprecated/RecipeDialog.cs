using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PaaminnerPaal.Dialogs
{
    [Serializable]
    public class RecipeDialogDeprecated : IDialog<object>
    {
        private bool _onlyCancel = true;
        private bool _stopDialog; // Context.Done only works within MessageReceived
        private List<int> _displayedRecipes;
        private List<string> _entities;
        private Dictionary<string, string> _map;

        private const int NumberOfRecipesInCarousel = 5;

        public async Task StartAsync(IDialogContext context)
        {
            var json = await Luis.Instance.RequestJson(context.Activity.AsMessageActivity().Text);

            _displayedRecipes = new List<int>();
            _entities = new List<string>();

            await RouteToMethods(context, true, json);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (!_stopDialog)
            {
                var message = await result;
                var json = await Luis.Instance.RequestJson(message.Text);

                // Change recipe category if new category detected
                var types = await Luis.Instance.JsonTypes(json);
                if (types.Contains("RecipeCategories"))
                {
                    _displayedRecipes.Clear();
                    _entities.Clear();
                }

                await Confirm(context, json);
            }

            if (_stopDialog)
            {
                context.Done<object>(null);
                return;
            }
            context.Wait(MessageReceived);
        }

        private async Task RouteToMethods(IDialogContext context, bool displayCategories, string json)
        {
            await GetCategory(json);

            if (_entities.Count <= 0)
            {
                if (displayCategories)
                    await RecipeCategories(context);
                else await PromptUnkown(context);
            }

            else
            {
                await RecipeCarousel(context, _map[_entities[0]]);
            }
        }

        private async Task<List<string>> GetCategory(string json)
        {
            var jsonKolonial = await Kolonial.Instance.RequestJson("recipe-tags/");
            _map = await Kolonial.Instance.JsonRecipeCategories(jsonKolonial);

            var list = await Luis.Instance.JsonEntities(json);
            var types = await Luis.Instance.JsonTypes(json);

            for (var i = 0; i < list.Count; ++i)
            {
                if (types[i] != "RecipeCategories") continue;
                if (!_entities.Contains(list[i]))
                {
                    _entities.Add(list[i]);
                }
            }
            return list;
        }

        private async Task<bool> Confirm(IDialogContext context, string json)
        {
            var intent = await Luis.Instance.JsonTopIntent(json);

            if (intent == "Cancel")
            {
                _stopDialog = true;
            }

            if (!_onlyCancel)
            {
                if (intent == "Deny")
                {
                    _stopDialog = true;
                }

                else if (intent == "Confirm")
                {
                    await RecipeCarousel(context, _map[_entities[0]]);
                    return false;
                }
            }

            if (!_stopDialog)
            {
                await RouteToMethods(context, false, json);
                return false;
            }
            await context.PostAsync("Den er god :)");
            return true;
        }

        private async Task PromptUnkown(IDialogContext context)
        {
            await context.PostAsync("Kjente ikke igjen kategori, prøv å omformulere :)");
        }

        private async Task RecipeCategories(IDialogContext context)
        {
            await BotMethods.Instance.AddList(context, "Ulike oppskriftskategorier å velge mellom:\n", "", _map.Keys.ToArray());
            await context.PostAsync("Hvilken kategori vil du ha oppskrifter fra? :)");
        }

        private async Task RecipeCarousel(IDialogContext context, string id)
        {
            var jsonKolonialRecipes = await Kolonial.Instance.RequestJson("recipe-tags/" + id);
            var listRecipes = await Kolonial.Instance.JsonRecipes(jsonKolonialRecipes);

            var card = context.MakeMessage();
            card.AttachmentLayout = "carousel";

            for (var i = 0; i < NumberOfRecipesInCarousel; ++i)
            {
                if (listRecipes[0].Count == _displayedRecipes.Count) break;
                var random = new Random().Next(0, listRecipes[0].Count);

                if (!_displayedRecipes.Contains(random))
                {
                    card.Attachments.Add(BotMethods.Instance.AddRecipeCard(listRecipes[0][random], listRecipes[1][random], 
                        listRecipes[2][random], listRecipes[3][random], listRecipes[4][random]));
                    _displayedRecipes.Add(random);
                }
                else
                {
                    --i;
                }
            }
            await context.PostAsync(card);

            if (listRecipes[0].Count == _displayedRecipes.Count)
            {
                await context.PostAsync("Alle oppskrifter i denne kategorien er sett.");
                _stopDialog = true;
                context.Wait(MessageReceived);
            }
            else
            {
                _onlyCancel = false;
                await context.PostAsync(BotMethods.Instance.RandomString(ConfigurationOptions.Instance.More));
            }
        }
    }
}