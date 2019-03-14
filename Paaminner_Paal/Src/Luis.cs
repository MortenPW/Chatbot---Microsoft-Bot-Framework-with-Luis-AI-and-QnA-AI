using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;

// LuisResult
// https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Builder/Luis/Models/LuisResult.cs
// var list = new List<EntityRecommendation>();
// var luisResult = new LuisResult(await Luis.Instance.RequestJson(message.Text), list);
// Does not fully suffice, use functions in this class instead.

// Json https://www.newtonsoft.com/json/help/html/SerializingCollections.htm

// Train and add data to LUIS programmatically:
// https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-get-started-cs-add-utterance
// https://westus.dev.cognitive.microsoft.com/docs/services/5890b47c39e2bb17b84a55ff/operations/5890b47c39e2bb052c5b9c27

namespace PaaminnerPaal
{
    public sealed class Luis
    {
        // Luis singleton
        private static readonly Lazy<Luis> Lazy =
            new Lazy<Luis>(() => new Luis());

        private Luis()
        {
        }

        public static Luis Instance => Lazy.Value;
        private double IntentThreshold => 0.1;

        public async Task<string> RequestJson(string message)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(message);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationOptions.Instance.LuisKey);

            // Request parameters
            queryString["timezoneOffset"] = "1";
            queryString["verbose"] = "false";
            queryString["spellCheck"] = "false";
            queryString["staging"] = "false";
            var uri = ConfigurationOptions.Instance.LuisHost + ConfigurationOptions.Instance.LuisId + "?q=" + queryString;

            var response = await client.GetAsync(uri);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> RequestTopIntent(string message)
        {
            var json = JObject.Parse(await RequestJson(message));
            var score = Convert.ToDouble(json.SelectToken("topScoringIntent.score"));
            if (score < IntentThreshold)
            {
                return "None";
            }
            return (string) json.SelectToken("topScoringIntent.intent");
        }

        public async Task<List<string>> RequestEntities(string message)
        {
            // https://www.newtonsoft.com/json/help/html/SerializingJSONFragments.htm

            var entities = JObject.Parse(await RequestJson(message));

            // get JSON result objects into a list
            IList<JToken> results = entities["entities"].Children().ToList();
            //return results.Select(result => result.SelectToken("entity").ToString()).ToList();

            // Return translated value- "Fesk" becomes "Fisk", etc
            return results.Select(result => result.SelectToken("resolution.values").First.ToString()).ToList();
        }

        public async Task<List<string>> RequestTypes(string message)
        {
            var entities = JObject.Parse(await RequestJson(message));
            IList<JToken> results = entities["entities"].Children().ToList();

            return results.Select(result => result.SelectToken("type").ToString()).ToList();
        }

        // Methods to avoid double API calls on a single message
        public async Task<string> JsonTopIntent(string json)
        {
            var jsonObject = JObject.Parse(json);
            var score = Convert.ToDouble(jsonObject.SelectToken("topScoringIntent.score"));
            if (score < IntentThreshold)
            {
                return "None";
            }
            return (string)jsonObject.SelectToken("topScoringIntent.intent");
        }

        public async Task<List<string>> JsonEntities(string json)
        {
            var entities = JObject.Parse(json);
            IList<JToken> results = entities["entities"].Children().ToList();

            return results.Select(result => result.SelectToken("resolution.values").First.ToString()).ToList();
        }

        public async Task<List<string>> JsonTypes(string json)
        {
            var entities = JObject.Parse(json);
            IList<JToken> results = entities["entities"].Children().ToList();

            return results.Select(result => result.SelectToken("type").ToString()).ToList();
        }

        public async void AddClosedEntityList()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationOptions.Instance.LuisKey);
            var uri = "https://westus.api.cognitive.microsoft.com/luis/api/v2.0/apps/{appId}/versions/{versionId}/closedlists/{clEntityId}?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PutAsync(uri, content);
            }
        }
    }
}