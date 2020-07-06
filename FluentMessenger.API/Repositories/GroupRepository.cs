using FluentMessenger.API.DBContext;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FluentMessenger.API.Repositories {
    public class GroupRepository : IRepository<Group> {
        public GroupRepository(FluentDbContext messangerContext) {
            _dbContext = messangerContext;
        }

        public void Add(Group item) {
            _dbContext.Groups.Add(item);
        }

        public void Delete(Group item) {
            _dbContext.Groups.Remove(item);
        }

        public void Delete(int id) {
            var groupToDelete = Get(id);
            Delete(groupToDelete);
        }

        public void Update(Group item) {
            _dbContext.Groups.Update(item);
        }

        public Group Get(int Id) {
            return _dbContext.Groups.AsQueryable<Group>()
                             .SingleOrDefault(x => x.Id == Id);
        }

        public Group Get(int Id, bool include) {
            var group = _dbContext.Groups.Find(Id);
            if (group != null) {
                _dbContext.Entry(group).Collection(x => x.Messages).Load();
                _dbContext.Entry(group).Collection(x => x.Contacts).Load();
                _dbContext.Entry(group).Reference(x => x.User).Load();
            }
            return group;
        }

        public IQueryable<Group> GetAll() {
            return _dbContext.Groups.AsQueryable<Group>();
        }

        public IQueryable<Group> GetAll(bool include) {
            var set = _dbContext.Set<Group>();
            set.Include(x => x.Contacts).Load();
            set.Include(x => x.Messages).Load();
            set.Include(x => x.User).Load();
            return set;
        }

        public bool SaveChanges() {
            return _dbContext.SaveChanges() >= 0;
        }

        public Group LoadRefrencesTypes(Group entity) {
            _dbContext.Entry(entity).Collection(x => x.Messages).Load();
            _dbContext.Entry(entity).Collection(x => x.Contacts).Load();
            _dbContext.Entry(entity).Reference(x => x.User).Load();
            return entity;
        }

        private readonly FluentDbContext _dbContext;
    }
}
