using FluentMessenger.API.DBContext;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentMessenger.API.Repositories {
    public class ContactRepository : IRepository<Contact> {

        public ContactRepository(FluentDbContext messangerDbContext) {
            _dbContext = messangerDbContext;
        }

        public void Add(Contact item) {
            _dbContext.Contacts.Add(item);
        }

        public void AddRange(IEnumerable<Contact> items) {
            _dbContext.AddRange(items);
        }

        public void Delete(Contact item) {
            _dbContext.Contacts.Remove(item);
        }

        public void Delete(int id) {
            var contactToDelete = Get(id);
            Delete(contactToDelete);
        }

        public void Update(Contact item) {
            _dbContext.Contacts.Update(item);
        }

        public Contact Get(int Id) {
            return _dbContext.Contacts.AsQueryable<Contact>()
                                      .SingleOrDefault(x => x.Id == Id);
        }

        public Contact Get(int Id, bool include) {
            var contact = _dbContext.Contacts.Find(Id);
            if (contact != null) {
                _dbContext.Entry(contact).Reference(x => x.Group).Load();
            }
            return contact;
        }

        public IQueryable<Contact> GetAll() {
            return _dbContext.Contacts.AsQueryable<Contact>();
        }

        public IQueryable<Contact> GetAll(bool include) {
            var set = _dbContext.Set<Contact>();
            set.Include(x => x.Group).Load();
            return set;
        }

        public bool SaveChanges() {
            return _dbContext.SaveChanges() >= 0;
        }

        public Contact LoadRefrencesTypes(Contact entity) {
            _dbContext.Entry(entity).Reference(x => x.Group).Load();
            return entity;
        }

        private readonly FluentDbContext _dbContext;
    }
}
