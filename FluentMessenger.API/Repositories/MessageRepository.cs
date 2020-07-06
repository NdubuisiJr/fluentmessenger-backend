using FluentMessenger.API.DBContext;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Repositories {
    public class MessageRepository : IRepository<Message> {
        public MessageRepository(FluentDbContext messengerDbContext) {
            _dbContext = messengerDbContext;
        }

        public void Add(Message item) {
            _dbContext.Messages.Add(item);
        }

        public void AddRange(IEnumerable<Message> items) {
            _dbContext.Messages.AddRange(items);
        }

        public void Delete(Message item) {
            _dbContext.Messages.Remove(item);
        }

        public void Delete(int id) {
            var messageToDelete = Get(id);
            Delete(messageToDelete);
        }

        public void Update(Message item) {
            _dbContext.Messages.Update(item);
        }

        public Message Get(int Id) {
            return _dbContext.Messages.AsQueryable<Message>()
                                      .SingleOrDefault(x => x.Id == Id);
        }
        public Message Get(int Id, bool include) {

            var message = _dbContext.Messages.Find(Id);
            if (message != null) {
                _dbContext.Entry(message).Reference(x => x.Group).Load();
            }
            return message;

        }

        public IQueryable<Message> GetAll() {
            return _dbContext.Messages.AsQueryable<Message>();
        }

        public IQueryable<Message> GetAll(bool include) {
            var set = _dbContext.Set<Message>();
            set.Include(x => x.Group).Load();
            return set;
        }

        public bool SaveChanges() {
            return _dbContext.SaveChanges() >= 0;
        }

        public Message LoadRefrencesTypes(Message entity) {
            _dbContext.Entry(entity).Reference(x => x.Group).Load();
            return entity;
        }

        private readonly FluentDbContext _dbContext;
    }
}
