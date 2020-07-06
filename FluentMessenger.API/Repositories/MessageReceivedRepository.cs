using FluentMessenger.API.DBContext;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Repositories {
    public class MessageReceivedRepository : IContactMessageRepository<ContactMessagesReceived> {
        public MessageReceivedRepository(FluentDbContext dbContext) {
            _dbContext = dbContext;
        }
        public void Add(ContactMessagesReceived item) {
            _dbContext.ContactMessagesReceived.Add(item);
        }

        public void Delete(ContactMessagesReceived item) {
            _dbContext.ContactMessagesReceived.Remove(item);
        }

        public void Update(ContactMessagesReceived item) {
            _dbContext.ContactMessagesReceived.Update(item);
        }

        public ContactMessagesReceived Get(int contactId, int messageId) {
            var contactMessage = _dbContext.ContactMessagesReceived.SingleOrDefault(x =>
                                       x.ContactId == contactId &&
                                       x.MessageId == messageId);
            _dbContext.Entry(contactMessage).Reference(x => x.Message).Load();
            _dbContext.Entry(contactMessage).Reference(x => x.Contact).Load();
            return contactMessage;
        }

        public IQueryable<ContactMessagesReceived> GetAll() {
            var set = _dbContext.Set<ContactMessagesReceived>();
            set.Include(x => x.Contact).Load();
            set.Include(x => x.Message).Load();
            return set;
        }

        public IEnumerable<Contact> GetContacts(int messageId) {
            return GetAll().Where(x => x.MessageId == messageId)
                           .Select(x => x.Contact);
        }

        public IEnumerable<Message> GetMessages(int contactId) {
            return GetAll().Where(x => x.ContactId == contactId)
                           .Select(x => x.Message);
        }

        public bool SaveChanges() {
            return _dbContext.SaveChanges() >= 0;
        }

        private FluentDbContext _dbContext;
    }
}
