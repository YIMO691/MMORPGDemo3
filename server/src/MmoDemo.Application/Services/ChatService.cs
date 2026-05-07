using MmoDemo.Contracts;

namespace MmoDemo.Application;

public class ChatService : IChatService
{
    public string BuildBroadcast(string senderName, string text) =>
        System.Text.Json.JsonSerializer.Serialize(new
        {
            t = MessageTypes.ChatBroadcast,
            ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            p = new ChatBroadcastPayload { SenderName = senderName, Text = text, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() }
        });
}
