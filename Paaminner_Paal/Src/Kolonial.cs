using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

// Usage: await context.PostAsync(await Kolonial.Instance.RequestJson("recipe-tags/"));

namespace PaaminnerPaal
{
    public sealed class Kolonial
    {
        private static readonly Lazy<Kolonial> Lazy =
            new Lazy<Kolonial>(() => new Kolonial());

        private Kolonial()
        {
        }

        public static Kolonial Instance => Lazy.Value;

        public async Task<string> RequestJson(string message)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(message);

            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            client.DefaultRequestHeaders.UserAgent.ParseAdd(ConfigurationOptions.Instance.KolonialId);
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Client-Token", ConfigurationOptions.Instance.KolonialKey);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            // Request parameters
            var uri = ConfigurationOptions.Instance.KolonialHost + queryString;

            var response = await client.GetAsync(uri);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<Dictionary<string, string>> JsonRecipeCategories(string json)
        {
            // Usage:
            // var jsonKolonial = await Kolonial.Instance.RequestJson("recipe-tags/");
            // var map = await Kolonial.Instance.JsonRecipeCategories(jsonKolonial);
            // await BotMethods.Instance.AddList(context, "Ulike oppskriftskategorier å velge mellom:\n", "", map.Keys.ToArray());
            // await context.PostAsync("Hvilken kategori vil du ha oppskrifter fra? :)");

            var entities = JObject.Parse(json);
            IList<JToken> results = entities["recipe_tags"].Children().ToList();

            var map = new Dictionary<string, string>();
            var titles = results.Select(result => result.SelectToken("title").ToString()).ToList();
            var ids = results.Select(result => result.SelectToken("id").ToString()).ToList();

            for (var i = 0; i < titles.Count; ++i)
            {
                map.Add(titles[i], ids[i]);
            }
            return map;
        }

        public async Task<List<List<string>>> JsonRecipes(string json)
        {
            // Usage:
            // var jsonKolonialRecipes = await Kolonial.Instance.RequestJson("recipe-tags/1"); 1 = id
            // var listRecipes = await Kolonial.Instance.JsonRecipes(jsonKolonialRecipes);
            // await BotMethods.Instance.AddList(context, "", "", listRecipes.ToArray());

            var entities = JObject.Parse(json);
            IList<JToken> results = entities["recipes"].Children().ToList();

            var list = new List<List<string>>
            {
                results.Select(result => result.SelectToken("title").ToString()).ToList(),
                results.Select(result => result.SelectToken("difficulty_string").ToString()).ToList(),
                results.Select(result => result.SelectToken("cooking_duration_string").ToString()).ToList(),
                results.Select(result => result.SelectToken("front_url").ToString()).ToList(),
                results.Select(result => result.SelectToken("feature_image_url").ToString()).ToList()
            };
            return list;
        }

        // Main categories
        public async Task<Dictionary<string, string>> JsonProductCategories(string json)
        {
            var entities = JObject.Parse(json);
            IList<JToken> results = entities["results"].Children().ToList();

            var map = new Dictionary<string, string>();
            var titles = results.Select(result => result.SelectToken("name").ToString()).ToList();
            var ids = results.Select(result => result.SelectToken("id").ToString()).ToList();

            for (var i = 0; i < titles.Count; ++i)
            {
                map.Add(titles[i], ids[i]);
            }
            return map;
        }

        // Find under categories
        public async Task<Dictionary<string, string>> JsonUnderCategories(string json)
        {
            var entities = JObject.Parse(json);
            IList<JToken> results = entities["results"].Children().ToList();
            var list = results.Select(result => result.SelectToken("children")).ToList();

            var map = new Dictionary<string, string>();
            foreach (var variable in list)
            {
                var names = variable.Select(result => result.SelectToken("name").ToString()).ToList();
                var ids = variable.Select(result => result.SelectToken("id").ToString()).ToList();
                for (var i = 0; i < names.Count; ++i)
                {
                    map.Add(names[i], ids[i]);
                }
            }
            return map;
        }

        // Gets items from under categories
        public async Task<List<List<string>>> JsonProducts(string json)
        {
            var entities = JObject.Parse(json);
            IList<JToken> results = entities["products"].Children().ToList();

            var list = new List<List<string>>
            {
                results.Select(result => result.SelectToken("id").ToString()).ToList(),
                results.Select(result => result.SelectToken("full_name").ToString()).ToList(),
                results.Select(result => result.SelectToken("brand").ToString()).ToList(),
                results.Select(result => result.SelectToken("front_url").ToString()).ToList(),
                results.Select(delegate(JToken result)
                {
                    var variable = result.SelectToken("images").ToString();
                    return variable.Length > 5 ? variable.Split('"')[5] : "";
                }).ToList(),
                results.Select(result => result.SelectToken("gross_price").ToString()).ToList(),
                results.Select(result => result.SelectToken("gross_unit_price").ToString()).ToList(),
                results.Select(result => result.SelectToken("unit_price_quantity_abbreviation").ToString()).ToList(),
                results.Select(result => result.SelectToken("discount").ToString()).ToList(),
                results.Select(result => result.SelectToken("promotion").ToString()).ToList(),
                results.Select(result => result.SelectToken("availability").SelectToken("is_available").ToString()).ToList()
            };
            return list;
        }
    }
}