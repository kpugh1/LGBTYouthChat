using System.Text.Json;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace YouthChat.Models;

public class ChatCompletionResult
{
    [JsonProperty("object")]
    public required string object_type { get; set; }
    public required string id { get; set; }
    public required string created { get; set; }
    public required string model { get; set; }
    public required string system_fingerprint { get; set; }
    public required List<Choice> choices { get; set; }
    public class Choice
    {
        public int index { get; set; }
        public required Message message { get; set; }
        public required string logprobs { get; set; }
        public required string finish_reason { get; set;}
    }
    public required Usage usage { get; set; }
    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }
}