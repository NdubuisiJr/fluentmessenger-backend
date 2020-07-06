using FluentMessenger.API.DBContext;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Repositories {
    public class MessageNotReceivedRepository : IContactMessageRepository<ContactMessagesNotReceived> {
        public MessageNotReceivedRepository(FluentDbContext dbContext) {
            _dbContext = dbContext;
        }
        public void Add(ContactMessagesNotReceived item) {
            _dbContext.ContactMessagesNotReceived.Add(item);
        }

        public void Delete(ContactMessagesNotReceived item) {
            _dbContext.ContactMessagesNotReceived.Remove(item);
        }

        public void Update(ContactMessagesNotReceived item) {
            _dbContext.ContactMessagesNotReceived.Update(item);
        }

        public ContactMessagesNotReceived Get(int contactId, int messageId) {
            var contactMessage = _dbContext.ContactMessagesNotReceived.SingleOrDefault(x =>
                                    x.ContactId == contactId &&
                                    x.MessageId == messageId);
            _dbContext.Entry(contactMessage).Reference(x => x.Message).Load();
            _dbContext.Entry(contactMessage).Reference(x => x.Contact).Load();
            return contactMessage;
        }

        public IQueryable<ContactMessagesNotReceived> GetAll() {
            var set = _dbContext.Set<ContactMessagesNotReceived>();
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
