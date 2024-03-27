using System.Text;

namespace Rule_GPT
{
    public class GPTUtils
    {
        private static List<string> apiKeys = new List<string>
        {
            "Insert your API key here",
            "Insert your API key here",
            "Insert your API key here"
        };

        private const string apiUrl = "https://api.openai.com/v1/chat/completions";
        private const string openaiModel = "gpt-3.5-turbo";
        
        public static async Task<Tuple<bool, string, OpenAIAPIDTO>> Prompt(
            string userPrompt, 
            double temperature = 0.7
        ) {

            bool success = true;
            string message = "";
            OpenAIAPIDTO? requestResult = new OpenAIAPIDTO();

            Random random = new Random();
            int index = random.Next(0, apiKeys.Count); // Rate limiting issue
            string selectedKey = apiKeys[index];

            using (var client = new HttpClient())
            {
                // Request headers
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {selectedKey}");

                // Request body content
                var body = string.Concat(
                    $"{{\"model\": \"{openaiModel}\",",
                    $" \"messages\": [{{\"role\": \"user\", \"content\": \"{userPrompt}\"}}],",
                    $"\"temperature\": {temperature}}}"
                );

                string mediaType = "application/json";

                var content = new StringContent(body, Encoding.UTF8, mediaType);

                try
                {
                    // Call API
                    HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;
                    //var response = await client.PostAsync(apiUrl, content);

                    // Check the status code of the response
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();

                        responseContent = responseContent.Replace("\"object\": \"", "\"object_info\": \"");
                        
                        (success, message, requestResult) = JSONUtils<OpenAIAPIDTO>.Deserialize(responseContent);
                    }
                    else
                    {
                        success = false;
                        message = $"Error in Prompt function: {response.StatusCode} - {response.ReasonPhrase}";
                        requestResult = null;
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    message = string.Concat(
                        "You might be disconnected from the internet. Error: ",
                        ex.Message
                    );
                    requestResult = null;
                }
            }

            return new Tuple<bool, string, OpenAIAPIDTO>(
                success,
                message,
                requestResult!
            );
        }
    }
}
