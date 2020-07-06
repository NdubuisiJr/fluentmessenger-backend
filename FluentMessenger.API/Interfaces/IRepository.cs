using FluentMessenger.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluentMessenger.API.Interfaces {
    public interface IRepository<T> where T : class {
        IQueryable<T> GetAll();

        IQueryable<T> GetAll(bool include);

        T Get(int Id);

        T Get(int Id, bool include);

        T LoadRefrencesTypes(T entity);

        void Add(T item);

        void AddRange(IEnumerable<T> items) => throw new NotImplementedException();

        void Update(T item);

        void Delete(T item);

        void Delete(int id);

        bool SaveChanges();
    }
}
