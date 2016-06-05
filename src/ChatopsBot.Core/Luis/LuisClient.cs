using System.Threading.Tasks;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace ChatopsBot.Core.Luis
{
    public class LuisClient
    {
        private static readonly string _baseUrl =
            "https://api.projectoxford.ai/luis/v1/application?id=27785d76-8fd7-4694-a518-c9cf04c6dcc0&subscription-key=07f3071d341549b7a76ded7ccf532c58";

        private static readonly string _modelId = "27785d76-8fd7-4694-a518-c9cf04c6dcc0";
        private static readonly string _subscriptionKey = "07f3071d341549b7a76ded7ccf532c58";

        public static async Task<LuisResult> SendMessage(string text)
        {
            ILuisService s = new LuisService(new LuisModelAttribute(_modelId, _subscriptionKey));
            return await s.QueryAsync(text);
        }


    }
}
