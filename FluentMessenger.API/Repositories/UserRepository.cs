using FluentMessenger.API.DBContext;
using FluentMessenger.API.Entities;
using FluentMessenger.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FluentMessenger.API.Repositories {
    public class UserRepository : IRepository<User> {
        public UserRepository(FluentDbContext dbContext) {
            _dbContext = dbContext;
        }
        public IQueryable<User> GetAll() {
            return _dbContext.Users.AsQueryable<User>();
        }

        public IQueryable<User> GetAll(bool include) {
            var set = _dbContext.Set<User>();
            set.Include(x => x.Groups).Load();
            return set;
        }
        public User Get(int Id) {
            return _dbContext.Users.SingleOrDefault(x => x.Id == Id);
        }

        public User Get(int Id, bool include) {
            var user = _dbContext.Users.Find(Id);
            if (user != null) {
                _dbContext.Entry(user).Collection(x => x.Groups).Load();
            }
            return user;
        }

        public void Add(User item) {
            _dbContext.Users.Add(item);
        }

        public void Update(User item) {
            _dbContext.Users.Update(item);
        }

        public void Delete(User item) {
            _dbContext.Users.Remove(item);
        }

        public void Delete(int id) {
            var userToDelete = Get(id);
            Delete(userToDelete);
        }

        public bool SaveChanges() {
            return _dbContext.SaveChanges() >= 0;
        }

        public User LoadRefrencesTypes(User entity) {
            _dbContext.Entry(entity).Collection(x => x.Groups).Load();
            return entity;
        }

        private readonly FluentDbContext _dbContext;
    }
}
