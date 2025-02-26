﻿using System;
using System.Collections.Generic;
using System.Linq;
using C_bool.BLL.DAL.Context;
using C_bool.BLL.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace C_bool.BLL.Repositories
{
    public class Repository <T> : IRepository<T> where T: class, IEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _entities;
        public Repository(ApplicationDbContext context)
        {
            this._context = context;
            _entities = context.Set<T>();
        }
        public void Add(T entity)
        {
            //check if place with Googleid is in database (same place) - if so, don't add to repository
            if (typeof(Place).IsAssignableFrom(typeof(T)))
            {
                var place = entity as Place;
                if (_context.Places.Any(x => place.GoogleId.Contains(x.GoogleId))) return;
            }
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            entity.CreatedOn = DateTime.Now;
            _entities.Add(entity);
            _context.SaveChanges();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _entities.Remove(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _entities.SingleOrDefault(s => s.Id == id);
            Delete(entity);
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _entities.Update(entity);
            _context.SaveChanges();
        }

        public T GetById(int id)
        {
            return this._entities.SingleOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.AsEnumerable();
        }

        public IQueryable<T> GetAllQueryable()
        {
            return this._entities.AsQueryable();
        }
    }
}
