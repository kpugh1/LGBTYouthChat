using Microsoft.AspNetCore.Mvc;
using YouthChat.Models;
using OpenAI.Chat;
using HuggingFace;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net;

namespace YouthChat.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{   
    private IConfiguration _config;
    private static List<Message> History { get; set; }
    private static int RateLimit { get; set; }  = 3;
    public ChatController(IConfiguration config)
    {
        _config = config;
        
        string? Prompt = System.IO.File.ReadAllText(config.GetConnectionString("Prompt_Path"));
        History = new List<Message>() {
            new Message{
                role = "system",
                content = Prompt
            }
        };
    }
    
    [HttpPost("/Chat")]
    public async Task<IActionResult> PostChat(Chat? InputChat, string UserResponse)
    {
        //Limit to calling the API 3 times per session. 
        if( RateLimit == 0)
        {
            return  new JsonResult( new { data = "You have reached your limit of 3 responses."});
        }
        //Create User Message Response
        Message UserResponseMessage = new Message {
            role = "user",
            content = UserResponse
        };
       
        History.AddRange(InputChat.Messages);
        History.Add(UserResponseMessage);

        using(HttpClient client = new HttpClient())
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://router.huggingface.co/hf-inference/models/HuggingFaceH4/zephyr-7b-beta/v1/chat/completions");
            //Add Headers
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.GetConnectionString("HuggingFace_API")}");
            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            //Add data 
            var data = new {
                model = "HuggingFaceH4/zephyr-7b-beta",
                messages = History
            };
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            
            //Send Request
            HttpResponseMessage responseMessage =  await client.SendAsync(request);                        
            try{
                
                responseMessage.EnsureSuccessStatusCode();
                ChatCompletionResult responseJson = JsonConvert.DeserializeObject<ChatCompletionResult>(await responseMessage.Content.ReadAsStringAsync());
                //This might not need a list to be created but I don't know if this will change in the future
                    //and it will always get all the messages returned to the list
                List<Message> AImessage = responseJson.choices.Select(x => x.message).ToList();
                string Response = string.Join(" ", AImessage.Select(x => x.content).ToList());
                
                RateLimit--;
                
                return new JsonResult( new { data = Response});                
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        };
    }
}