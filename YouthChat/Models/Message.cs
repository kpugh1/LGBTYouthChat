using Microsoft.AspNetCore.Identity;
using OpenAI.Chat;

namespace YouthChat.Models;

public class Message {
    public required string role { get; set; }
    public required string content { get; set; }
    
}