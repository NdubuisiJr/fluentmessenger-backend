using FluentMessenger.API.DBContext;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FluentMessenger.API.Repositories {
    public class TemplateRepository : IRepository<MessageTemplate> {
        private FluentDbContext _dbContext;

        public TemplateRepository(FluentDbContext messengerDbContext) {
            _dbContext = messengerDbContext;
        }
        public void Add(MessageTemplate item) {
            _dbContext.Add(item);
        }

        public void Delete(MessageTemplate item) {
            _dbContext.Remove(item);
        }

        public void Update(MessageTemplate item) {
            _dbContext.MessageTemplates.Update(item);
        }

        public void Delete(int id) {
            var template = Get(id);
            Delete(template);
        }

        public MessageTemplate Get(int Id) {
            return _dbContext.MessageTemplates.AsQueryable<MessageTemplate>()
                                       .SingleOrDefault(x => x.Id == Id);
        }

        public MessageTemplate Get(int Id, bool include) {
            var template = _dbContext.MessageTemplates.Find(Id);
            if (template != null) {
                _dbContext.Entry(template).Reference(x => x.User).Load();
            }
            return template;
        }

        public IQueryable<MessageTemplate> GetAll() {
            return _dbContext.MessageTemplates.AsQueryable<MessageTemplate>();
        }

        public IQueryable<MessageTemplate> GetAll(bool include) {
            var set = _dbContext.Set<MessageTemplate>();
            set.Include(x => x.User).Load();
            return set;
        }

        public MessageTemplate LoadRefrencesTypes(MessageTemplate entity) {
            _dbContext.Entry(entity).Reference(x => x.User).Load();
            return entity;
        }

        public bool SaveChanges() {
            return _dbContext.SaveChanges() >= 0;
        }
    }
}
