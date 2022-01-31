using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain;
using Server.Features.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Features.Chats
{
    [ApiController]
    [Route("Chats")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("add")]
        public Task<Chat> CreateChat([FromBody] string name, CancellationToken cancellationToken)
        {
            return _chatService.CreateChat(name, cancellationToken);
        }

        [HttpPost("messages/add")]
        public Task<Message> CreateMessage(string text, int chatId, CancellationToken cancellationToken)
        {
            return _chatService.CreateMessage(text, chatId, cancellationToken);
        }

        [HttpGet("messages")]
        public Task<MessagesResponse> GetMessages(int chatId, CancellationToken cancellationToken)
        {
            return _chatService.GetMessages(chatId, cancellationToken);
        }

        [HttpGet]
        public Task<ChatsResponse> GetChats(CancellationToken cancellationToken)
        {
            return _chatService.GetChats(cancellationToken);
        }

        [HttpGet("users")]
        public Task<ChatUsersResponse> GetUsers(int chatId, CancellationToken cancellationToken)
        {
            return _chatService.GetUsers(chatId, cancellationToken);
        }

        [HttpGet("chatusers")]
        public Task<ChatUsersResponse> GetChatUsers(int chatId, CancellationToken cancellationToken)
        {
            return _chatService.GetChatUsers(chatId, cancellationToken);
        }

        [HttpPost("users/add")]
        public Task<ChatUsersResponse> AddUserToChat(int chatId, string email, CancellationToken cancellationToken)
        {
            return _chatService.AddUserToChat(chatId, email, cancellationToken);
        }

        [HttpDelete("users/delete")]
        public Task<ChatUsersResponse> DeleteUserFromChat(int chatId, string email, CancellationToken cancellationToken)
        {
            return _chatService.RemoveUserFromChat(chatId, email, cancellationToken);
        }

        [HttpDelete("users/leave")]
        public Task<ChatsResponse> LeaveFromChat(int chatId,  CancellationToken cancellationToken)
        {
            return _chatService.LeaveChat(chatId, cancellationToken);
        }


    }
}


