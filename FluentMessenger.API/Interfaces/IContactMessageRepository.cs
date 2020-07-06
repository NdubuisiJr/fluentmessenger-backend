using FluentMessenger.API.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Interfaces {
    public interface IContactMessageRepository<T> where T: class {
        void Add(T item);
        void Delete(T item);
        void Update(T item);
        bool SaveChanges();
        T Get(int contactId, int messageId);
        IQueryable<T> GetAll();
        IEnumerable<Contact> GetContacts(int messageId);
        IEnumerable<Message> GetMessages(int contactId);
    }
}
