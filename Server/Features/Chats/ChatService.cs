using Microsoft.EntityFrameworkCore;
using Server.Domain;
using Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;


namespace Server.Features.Chats
{
    public class ChatService
    {
        private readonly ApplicationContext _context;
        private readonly ICurrentUser _currentUser;

        public ChatService(ApplicationContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Message> CreateMessage(string text, int chatId, CancellationToken cancellationToken)
        {
            var person = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

            var message = new Message()
            {
                Text = text,
                WhenSent = DateTime.Now,
                ChatId = chatId,
                PersonId = person.PersonId
            };

            await _context.Messages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return message;
        }

        public async Task<Chat> CreateChat(string name,  CancellationToken cancellationToken)
        {
            var person = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

            var newChat = new Chat()
            {
              Name = name,
              PersonId = person.PersonId
            };

            var chatPerson = new ChatPerson()
            {
                Chat = newChat,
                ChatId = newChat.ChatId,
                Person = person,
                PersonId = person.PersonId
            };

            await _context.Chats.AddAsync(newChat, cancellationToken);
            await _context.ChatPersons.AddAsync(chatPerson, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newChat;
        }

        public async Task<List<Chat>> GetChats(CancellationToken cancellationToken)
        {
            var currentUser = await _context.Persons.FirstAsync(x => x.Username == _currentUser.GetCurrentUsername(), cancellationToken);

            var query = from c in _context.Chats
                        join cp in _context.ChatPersons on c.ChatId equals cp.ChatId
                        where cp.PersonId == currentUser.PersonId
                        select c;

            var chats = await query.ToListAsync(cancellationToken);

            return chats;
        }
    }
}
