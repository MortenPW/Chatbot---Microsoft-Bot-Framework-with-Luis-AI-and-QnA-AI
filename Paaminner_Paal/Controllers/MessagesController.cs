using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PaaminnerPaal.Dialogs;

// https://github.com/EricDahlvang/Microsoft.Bot.Sample.AzureSql

namespace PaaminnerPaal.Controllers
{
    [BotAuthentication]
    public sealed class MessagesController : ApiController
    {
        /// <summary>
        ///     POST: api/Messages
        ///     receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // Handle images and emoticons, etc. by ignoring them for now
            if (activity.Attachments?.Any() == true)
            {
                activity.Attachments.Clear();
                activity.AsMessageActivity().Attachments.Clear();
            }

            // check if activity is of type message
            else if (activity.GetActivityType() == ActivityTypes.Message)
            {
                
                await Conversation.SendAsync(activity, () => new RootDialog());
            }
            else
                HandleSystemMessage(activity);
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        private void HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                /*
                IConversationUpdateActivity update = message;
                var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                if (update.MembersAdded == null || !update.MembersAdded.Any()) return;
                foreach (var newMember in update.MembersAdded)
                {
                    if (newMember.Id == message.Recipient.Id) continue;
                    var reply = message.CreateReply();

                    // Add card to reply
                    reply.Attachments = new List<Attachment> { WelcomeMessage().ToAttachment() };
                    client.Conversations.ReplyToActivityAsync(reply);
                }
                */
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
        }
    }
}