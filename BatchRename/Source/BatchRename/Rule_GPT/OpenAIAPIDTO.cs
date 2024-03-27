using System.Text;

namespace Rule_GPT
{
    public class OpenAIAPIDTO
    {
        public class Choice
        {
            public class Message
            {
                public string role { get; set; } = "";
                public string content { get; set; } = "";

                public override string ToString()
                {
                    return string.Concat(
                        $"Role: {role}\n",
                        $"Content: {content}"
                    );
                }
            }
            public int index { get; set; }
            public Message message { get; set; } = new Message();
            public object? logprobs { get; set; } = null;
            public string finish_reason { get; set; } = "";
            public override string ToString()
            {
                return string.Concat(
                    $"Index: {index}\n",
                    $"Message:\n{message.ToString()}\n",
                    $"Log Probs: {(logprobs == null ? "null" : logprobs)}\n",
                    $"Finish Reason: {finish_reason}"
                );
            }
        }
        public class Usage
        {
            public int prompt_tokens { get; set; } = 0;
            public int completion_tokens { get; set; } = 0;
            public int total_tokens { get; set; } = 0;
            public override string ToString()
            {
                return string.Concat(
                    $"Prompt Tokens: {prompt_tokens}\n",
                    $"Completion Tokens:{completion_tokens}\n",
                    $"Total Tokens: {total_tokens}"
                );
            }
        }
        public string id { get; set; } = "";
        public string object_info { get; set; } = "";
        public int created { get; set; } = 0;
        public string model { get; set; } = "";
        public List<Choice> choices { get; set; } = new List<Choice>();
        public Usage usage { get; set; } = new Usage();
        public object? system_fingerprint { get; set; } = null;
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"ID: {id}");
            sb.AppendLine($"Object: {object_info}");
            sb.AppendLine($"Created: {created}");
            sb.AppendLine($"Model: {model}");

            sb.AppendLine("Choices:");
            foreach (var choice in choices)
            {
                sb.AppendLine(choice.ToString());
            }

            sb.AppendLine($"Usage: {usage.ToString()}");
            sb.AppendLine($"System Fingerprint: {(system_fingerprint != null ? system_fingerprint : "null")}");
       
            return sb.ToString();
        }
    }
}
