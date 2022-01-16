﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using C_bool.BLL.DAL.Entities;
using C_bool.BLL.Enums;
using C_bool.BLL.Repositories;
using Microsoft.AspNetCore.Http;

namespace C_bool.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _repository;
        private IHttpContextAccessor _httpContextAccessor;

        public UserService(IRepository<User> repository, IHttpContextAccessor httpContextAccessor, IPlaceService placesService)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        //Andrzeju dopisałem
        public int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public User GetCurrentUser()
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            return _repository.GetById(userId);
        }

        public List<User> SearchByName(string name)
        {
            var users = _repository.GetAllQueryable();
            return users.Where(u => u.UserName.Equals(name)).ToList();
        }

        public List<User> SearchByEmail(string email)
        {
            var users = _repository.GetAllQueryable();
            return users.Where(u => u.Email.Equals(email)).ToList();
        }
            

        public List<User> SearchByGender(Gender gender)
        {
            var users = _repository.GetAllQueryable();
            return users.Where(u => u.Gender == gender).ToList();
        }
        

        public List<User> SearchActive(bool isActive)
        {
            var users = _repository.GetAllQueryable();
            return users.Where(u => u.IsActive == isActive).ToList();
        }

        public List<User> OrderByPoints(bool isDescending)
        {
            var users = _repository.GetAllQueryable();
            return isDescending ? users.OrderByDescending(u => u.Points).ToList() : users.OrderBy(u => u.Points).ToList();
        }
        

        public void AddUser(User user)
        {
            user.CreatedOn = DateTime.UtcNow;
            _repository.Add(user);
        }

        //TODO should be implemented by sum of points from completed GameTasks list 
        public void SetPoints(User user)
        {
            var users = _repository.GetAllQueryable();

            users.Single(u => u.Id == user.Id).Points = users.Single(u => u.Id == user.Id).UserGameTasks.Sum(ugt => ugt.GameTask.Points);

            _repository.Update(user);
        }

        public void AddFileDataToRepository()
        {
            throw new NotImplementedException();
        }

        public List<User> SearchUsers(string name, string email, string gender, string isActive, string isDescending)
        {
            var users = _repository.GetAllQueryable();

            if (name != null)
            {
                users = users.Where(u => u.UserName.Contains(name));
            }
            if (email != null)
            {
                users = users.Where(u => u.Email.Contains(email));
            }
            if (gender != null)
            {
                users = users.Where(u => u.Gender == Enum.Parse<Gender>(gender));
            }
            if (isActive != null)
            {
                users = users.Where(u => u.IsActive == bool.Parse(isActive));
            }
            if (isDescending != null)
            {
                users = bool.Parse(isDescending) ? users.OrderByDescending(u => u.Points) : users.OrderBy(u => u.Points);
            }

            return users.ToList();
        }

        public void AddFavPlace(Place place)
        {
            var currentUser = GetCurrentUser();
            currentUser.FavPlaces.Add(new UserPlace(currentUser, place));
            _repository.Update(currentUser);
        }

        public void AddFavPlace(User user, Place place)
        {
            user.FavPlaces.Add(new UserPlace(user, place));
            _repository.Update(user);
        }

        public void AddTaskToUser(GameTask gameTask)
        {
            var currentUser = GetCurrentUser();

            currentUser.UserGameTasks.Add(new UserGameTask(currentUser, gameTask));
            _repository.Update(currentUser);
        }

        public void AddTaskToUser(User user, GameTask gameTask)
        {
            user.UserGameTasks.Add(new UserGameTask(user, gameTask));
            _repository.Update(user);
        }

        public void SetTaskAsDone(GameTask gameTask)
        {
            var users = _repository.GetAllQueryable();
            var currentUser = GetCurrentUser();

            var gameTaskToBeDone =
                users.Single(u => u.Id == currentUser.Id).UserGameTasks.Single(ugt => ugt.GameTask.Id == gameTask.Id);
            gameTaskToBeDone.IsDone = true;

            SetPoints(currentUser);

            _repository.Update(currentUser);

        }

        public void SetTaskAsDone(User user, GameTask gameTask)
        {
            var users = _repository.GetAllQueryable();

            var gameTaskToBeDone =
                users.Single(u => u.Id == user.Id).UserGameTasks.Single(ugt => ugt.GameTask.Id == gameTask.Id);
            gameTaskToBeDone.IsDone = true;

            SetPoints(user);

            _repository.Update(user);
                
        }

        public List<GameTask> GetToDoTasks()
        {
            var users = _repository.GetAllQueryable();
            var currentUser = GetCurrentUser();

            var userGameTasks = users.Single(u => u.Id == currentUser.Id).UserGameTasks
                .Where(ugt =>
                    ugt.GameTask.ValidFrom <= DateTime.UtcNow && ugt.GameTask.IsActive && ugt.IsDone == false);

            return userGameTasks.Select(ugt => ugt.GameTask).ToList();
        }

        public List<GameTask> GetToDoTasks(User user)
        {
            var users = _repository.GetAllQueryable();

            var userGameTasks = users.Single(u => u.Id == user.Id).UserGameTasks
                .Where(ugt =>
                    ugt.GameTask.ValidFrom <= DateTime.UtcNow && ugt.GameTask.IsActive && ugt.IsDone == false);

            return userGameTasks.Select(ugt => ugt.GameTask).ToList();
        }

        public List<GameTask> GetInProgressTasks()
        {
            var users = _repository.GetAllQueryable();
            var currentUser = GetCurrentUser();

            var userGameTasks = users.Single(u => u.Id == currentUser.Id).UserGameTasks
                .Where(ugt => ugt.GameTask.ValidFrom <= DateTime.UtcNow && ugt.GameTask.IsActive && ugt.IsDone == false);

            return userGameTasks.Select(ugt => ugt.GameTask).ToList();
        }

        public List<GameTask> GetInProgressTasks(User user)
        {
            var users = _repository.GetAllQueryable();

            var userGameTasks = users.Single(u => u.Id == user.Id).UserGameTasks
                .Where(ugt => ugt.GameTask.ValidFrom <= DateTime.UtcNow && ugt.GameTask.IsActive && ugt.IsDone == false);

            return userGameTasks.Select(ugt => ugt.GameTask).ToList();
        }

        public List<GameTask> GetDoneTasks()
        {
            var users = _repository.GetAllQueryable();
            var currentUser = GetCurrentUser();

            var userGameTasks = users.Single(u => u.Id == currentUser.Id).UserGameTasks
                .Where(ugt => ugt.IsDone == true);

            return userGameTasks.Select(ugt => ugt.GameTask).ToList();
        }

        public List<GameTask> GetDoneTasks(User user)
        {
            var users = _repository.GetAllQueryable();

            var userGameTasks = users.Single(u => u.Id == user.Id).UserGameTasks
                .Where(ugt => ugt.IsDone == true);

            return userGameTasks.Select(ugt => ugt.GameTask).ToList();
        }
    }
}