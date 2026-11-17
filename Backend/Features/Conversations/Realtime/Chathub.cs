using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PrjA.Features.Conversations.SendMessage;

namespace PrjA.Features.Conversations.Realtime;

public interface IChatClient
{
    public Task SendMessage(SendMessageResponse message);
    public Task ConversationAccepted(Guid conversationId);
}

[Authorize]
public class Chathub : Hub<IChatClient>
{

}