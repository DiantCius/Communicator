using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Server.Features.Users;
using AutoMapper;
using Server.Infrastructure.Errors;
using System.Net;

namespace Server.Features.Chats
{
    public class ChatService
    {
        private readonly ApplicationContext _context;
        private readonly CurrentUser _currentUser;
        private readonly IMapper _mapper;

        public ChatService(ApplicationContext context, CurrentUser currentUser, IMapper mapper)
        {
            _context = context;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<Message> CreateMessage(string text, int chatId, CancellationToken cancellationToken)
        {
            var person = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

            var message = new Message()
            {
                Text = text,
                WhenSent = DateTime.Now,
                ChatId = chatId,
                PersonId = person.PersonId,
                Username = person.Username
            };

            await _context.Messages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return message;
        }

        public async Task<MessagesResponse> GetMessages(int chatId, CancellationToken cancellationToken)
        {
            var messages = await _context.Messages.Where(x => x.ChatId == chatId).ToListAsync(cancellationToken);

            return new MessagesResponse
            {
                Messages = messages,
                Count = messages.Count
            };
        }

        public async Task<Chat> CreateChat(string name,  CancellationToken cancellationToken)
        {
            var person = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

            var newChat = new Chat()
            {
              Name = name,
              Username = person.Username
            };

            var chatPerson = new ChatPerson()
            {
                Chat = newChat,
                ChatId = newChat.ChatId,
                Person = person,
                PersonId = person.PersonId
            };

            await _context.ChatPersons.AddAsync(chatPerson, cancellationToken);
            await _context.Chats.AddAsync(newChat, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newChat;
        }

        public async Task<ChatsResponse> GetChats(CancellationToken cancellationToken)
        {
            var currentUser = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);
            List<Chat> chats = await GetChatsForCurrentUser(currentUser, cancellationToken);

            return new ChatsResponse
            {
                Chats = chats,
                Count = chats.Count
            };
        }

        private async Task<List<Chat>> GetChatsForCurrentUser(Person currentUser, CancellationToken cancellationToken)
        {
            var query = from c in _context.Chats
                        join cp in _context.ChatPersons on c.ChatId equals cp.ChatId
                        where cp.PersonId == currentUser.PersonId
                        select c;

            var chats = await query.ToListAsync(cancellationToken);
            return chats;
        }

        public async Task<ChatUsersResponse> GetUsers (int chatId, CancellationToken cancellationToken)
        {
            List<User> userList = await FindUsersForChat(chatId, cancellationToken);

            return new ChatUsersResponse
            {
                Users = userList,
                Count = userList.Count
            };
        }

        public async Task<ChatUsersResponse> GetChatUsers(int chatId, CancellationToken cancellationToken)
        {
            List<User> userList = await GetUsersFromChat(chatId, cancellationToken);

            return new ChatUsersResponse
            {
                Users = userList,
                Count = userList.Count
            };
        }

        private async Task<List<User>> GetUsersFromChat(int chatId, CancellationToken cancellationToken)
        {
            var query = from p in _context.Persons
                        join cp in _context.ChatPersons on p.PersonId equals cp.PersonId
                        where cp.ChatId == chatId
                        select p;

            var persons = await query.ToListAsync(cancellationToken);

            var userList = _mapper.Map<List<Person>, List<User>>(persons);
            return userList;
        }

        public async Task<ChatUsersResponse> AddUserToChat(int chatId, string email, CancellationToken cancellationToken)
        {
            var person = await _context.Persons.FirstAsync(x => x.Email == email, cancellationToken);

            var chat = await _context.Chats.FirstAsync(x => x.ChatId == chatId, cancellationToken);

            var chatPerson = new ChatPerson()
            {
                ChatId = chat.ChatId,
                Chat = chat,
                PersonId = person.PersonId,
                Person = person
            };

            await _context.ChatPersons.AddAsync(chatPerson, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            List<User> userList = await FindUsersForChat(chatId, cancellationToken);

            return new ChatUsersResponse
            {
                Users = userList,
                Count = userList.Count
            };
        }

        private async Task<List<User>> FindUsersForChat(int chatId, CancellationToken cancellationToken)
        {
            var query = from pe in _context.Persons
                        where !(from p in _context.Persons
                                join cp in _context.ChatPersons on p.PersonId equals cp.PersonId
                                where cp.ChatId == chatId
                                select p.PersonId).Contains(pe.PersonId)
                        select pe;

            var persons = await query.ToListAsync(cancellationToken);

            var userList = _mapper.Map<List<Person>, List<User>>(persons);
            return userList;
        }

        public async Task<ChatUsersResponse> RemoveUserFromChat (int chatId, string email, CancellationToken cancellationToken)
        {
            var person = await _context.Persons.FirstAsync(x => x.Email == email, cancellationToken);

            var chat = await _context.Chats.FirstAsync(x => x.ChatId == chatId, cancellationToken);

            var personToDelete = await _context.ChatPersons.FirstAsync(x => x.ChatId == chat.ChatId && x.PersonId == person.PersonId);

            if(personToDelete == null)
            {
                throw new ApiException("user not found ", HttpStatusCode.NotFound);
            }

            _context.ChatPersons.Remove(personToDelete);

            await _context.SaveChangesAsync(cancellationToken);

            List<User> userList = await GetUsersFromChat(chatId, cancellationToken);

            return new ChatUsersResponse
            {
                Users = userList,
                Count = userList.Count
            };
        }

        public async Task<ChatsResponse> LeaveChat(int chatId, CancellationToken cancellationToken)
        {
            var currentUser = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

            var chat = await _context.Chats.FirstAsync(x => x.ChatId == chatId, cancellationToken);

            var personToDelete = await _context.ChatPersons.FirstAsync(x => x.ChatId == chat.ChatId && x.PersonId == currentUser.PersonId);

            List<User> userList = await GetUsersFromChat(chatId, cancellationToken);

            int chatUserCount = userList.Count;

            if(chatUserCount == 1)
            {
                _context.Chats.Remove(chat);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                _context.ChatPersons.Remove(personToDelete);
                await _context.SaveChangesAsync(cancellationToken);
            }

            List<Chat> chats = await GetChatsForCurrentUser(currentUser, cancellationToken);

            return new ChatsResponse
            {
                Chats = chats,
                Count = chats.Count
            };
        }

    }
}
