using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace PaaminnerPaal.Dialogs
{
    [Serializable]
    public class AllergyDialog : IDialog<object>
    {
        // For bold enclose the string in aterisk i.e "*".
        // Example: Show this as *Bold* will be displayed as Show this as Bold

        public async Task StartAsync(IDialogContext context)
        {
            var json = ContextStates.Instance.Json;

            // Change allergies if new allergies detected
            var types = await Luis.Instance.JsonTypes(json);
            if (types.Contains("AddAllergy"))
            {
                //ContextStates.Instance.RegisteredAllergies.Clear();
            }

            if (!await ContextIntents(context, json))
            {
                await RouteToMethods(context, true, json);
            }
        }

        private async Task<bool> ContextIntents(IDialogContext context, string json)
        {
            var intent = await Luis.Instance.JsonTopIntent(json);

            if (intent == "Confirm")
            {
                if (ContextStates.Instance.RegisteredAllergies.Count <= 0)
                {
                    await context.PostAsync("Ingen allergener registrert.");
                }

                else
                {
                    await context.PostAsync("Allergener registrert.");
                }
                ContextStates.Instance.ResetStates();
                return true;
            }

            if (intent == "Cancel")
            {
                await context.PostAsync("Ingen flere allergener registrert.");
                ContextStates.Instance.ResetStates();
                return true;
            }

            if (intent == "Deny")
            {
                await context.PostAsync("Prøv igjen! :)"); // TODO: Legg til flere svar
                ContextStates.Instance.RegisteredAllergies.Clear();
                await DialogCategories(context);
            }
            return false;
        }

        private async Task RouteToMethods(IDialogContext context, bool displayCategories, string json)
        {
            await BotMethods.Instance.GetUniqueEntities(json, "AddAllergy");

            if (ContextStates.Instance.Entities.Count <= 0)
            {
                if (displayCategories)
                    await DialogCategories(context);
            }

            else
            {
                await DialogCheck(context, json);
            }
        }

        private async Task DialogCategories(IDialogContext context)
        {
            await BotMethods.Instance.AddList(context,
                "*Ved å legge inn allergier vil du få varsel fra meg dersom du " +
                "forsøker å kjøpe et produkt som inneholder det aktuelle allergenet!*\n" +
                "(du kan selvsagt skrive flere) :)", "", ConfigurationOptions.Instance.Allergies);
            
            if (ContextStates.Instance.RegisteredAllergies.Count > 0)
            {
                await BotMethods.Instance.AddList(context, "Allergener registrert: ", "", ContextStates.Instance.RegisteredAllergies.ToArray());
            }

            await context.PostAsync("Hvilke allergener skal jeg være obs på?");
        }

        private async Task DialogCheck(IDialogContext context, string json)
        {
            var list = await Luis.Instance.JsonEntities(json);
            var types = await Luis.Instance.JsonTypes(json);

            for (var i = 0; i < list.Count; ++i)
            {
                if (types[i] != "AddAllergy") continue;
                if (!ContextStates.Instance.RegisteredAllergies.Contains(list[i]))
                {
                    ContextStates.Instance.RegisteredAllergies.Add(list[i]);
                }
            }

            if (ContextStates.Instance.RegisteredAllergies.Count <= 0)
            {
                return;
            }
            ContextStates.Instance.RegisteredAllergies.Sort();

            await BotMethods.Instance.AddList(context, "Gjenkjente følgende allergener:", "", ContextStates.Instance.RegisteredAllergies.ToArray());
            await context.PostAsync("Er dette riktig?");
        }
    }
}