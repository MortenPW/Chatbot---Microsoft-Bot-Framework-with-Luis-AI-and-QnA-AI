using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Rest;

// Go to https://qnamaker.ai and feed data, train & publish QnA Knowledgebase.

namespace PaaminnerPaal.Dialogs
{
    [Serializable]
    public class QnaDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            /* Wait until the first message is received from the conversation and call MessageReceviedAsync 
             *  to process that message. */
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,
             *  await the result. */
            var message = await result;
            var checkString = TextAnalyticsRequest(message.Text).ToString(); // Prune message

            if (checkString.Length > 1) // Avoid using if everything is pruned away
                message.Text = checkString;

            await QnaRequest(context, message);
        }

        private async Task QnaRequest(IDialogContext context, IMessageActivity message)
        {
            //var qnaAuthKey = GetSetting("QnAAuthKey");
            //var qnaKBId = Utils.GetAppSetting("QnAKnowledgebaseId");
            //var endpointHostName = Utils.GetAppSetting("QnAEndpointHostName");

            // QnA Subscription Key and KnowledgeBase Id null verification
            if (!string.IsNullOrEmpty(ConfigurationOptions.Instance.QnaAuthKey) && !string.IsNullOrEmpty(ConfigurationOptions.Instance.QnaKbId))
                if (string.IsNullOrEmpty(ConfigurationOptions.Instance.EndpointHostName))
                    await context.Forward(new BasicQnAMakerPreviewDialog(), AfterAnswerAsync, message,
                        CancellationToken.None);
                else
                    await context.Forward(new BasicQnAMakerDialog(), AfterAnswerAsync, message, CancellationToken.None);
            else
                await context.PostAsync(
                    "Please set QnAKnowledgebaseId, QnAAuthKey and QnAEndpointHostName (if applicable) in App Settings. Learn how to get them at https://aka.ms/qnaabssetup.");
        }

        private async Task AfterAnswerAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            // wait for the next user message
            //context.Wait(MessageReceivedAsync);
            //context.Call(new BasicLuisDialog(), MyRootLuisDialogComplete);
            context.Done("user_data");
        }

        // Pruning QnA input via Text Analytics for better responses
        public StringBuilder TextAnalyticsRequest(string message)
        {
            // Create client
            ITextAnalyticsAPI client = new TextAnalyticsAPI(new ApiKeyServiceClientCredentials());
            client.AzureRegion = AzureRegions.Northeurope;

            // Getting key-phrases
            var result = client.KeyPhrasesAsync(new MultiLanguageBatchInput(
                new List<MultiLanguageInput>
                {
                    new MultiLanguageInput("no", "1", message)
                })).Result;

            // Print / string build keyphrases
            var stringBuilder = new StringBuilder();

            foreach (var document in result.Documents)
            foreach (var keyphrase in document.KeyPhrases)
                stringBuilder.Append(keyphrase + " ");
            return stringBuilder;
        }

        public class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationOptions.Instance.ApiKeyTextAnalytics);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }
    }

    // Dialog for QnAMaker Preview service
    [Serializable]
    public class BasicQnAMakerPreviewDialog : QnAMakerDialog
    {
        public BasicQnAMakerPreviewDialog() : base(new QnAMakerService(new QnAMakerAttribute(
            ConfigurationOptions.Instance.QnaAuthKey,
            ConfigurationOptions.Instance.QnaKbId,
            BotMethods.Instance.RandomString(ConfigurationOptions.Instance.NotUnderstood), 0.3)))
        {
        }
    }

    // Dialog for QnAMaker GA service
    [Serializable]
    public class BasicQnAMakerDialog : QnAMakerDialog
    {
        // NB: Threshold for acceptable answers can be adjusted here

        // public QnAMakerAttribute(string subscriptionKey, string knowledgebaseId, string defaultMessage = null, double scoreThreshold = 0.3, int top = 1);
        public BasicQnAMakerDialog() : base(new QnAMakerService(new QnAMakerAttribute(
            ConfigurationOptions.Instance.QnaAuthKey, ConfigurationOptions.Instance.QnaKbId,
            BotMethods.Instance.RandomString(ConfigurationOptions.Instance.NotUnderstood), 0.3, 1, ConfigurationOptions.Instance.EndpointHostName)))
        {
        }
    }
}