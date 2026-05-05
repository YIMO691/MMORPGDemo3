namespace MmoDemo.Application;

public interface IChatService
{
    string BuildBroadcast(string senderName, string text);
}
