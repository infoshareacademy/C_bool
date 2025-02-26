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
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserPlace> _userPlaceRepository;
        private readonly IRepository<UserGameTask> _userGameTaskRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public UserService(IRepository<User> userRepository, IRepository<UserPlace> userPlaceRepository, IRepository<UserGameTask> userGameTaskRepository , IHttpContextAccessor httpContextAccessor, IPlaceService placesService)
        { 
            _userRepository = userRepository;
            _userGameTaskRepository = userGameTaskRepository;
            _userPlaceRepository = userPlaceRepository;
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
           
            return _userRepository.GetById(userId);
        }
        public void AddFavPlace(Place place)
        {
            var currentUser = GetCurrentUser();
            currentUser.FavPlaces.Add(new UserPlace(currentUser, place));
            _userRepository.Update(currentUser);
        }

        public List<Place> GetFavPlaces()
        {
            var usersPlaces = _userPlaceRepository.GetAllQueryable();
            var currentUserId = GetCurrentUserId();

            return usersPlaces.Where(up => up.UserId == currentUserId).Select(up => up.Place).ToList();
        }

        public void AddTaskToUser(GameTask gameTask)
        {
            var currentUser = GetCurrentUser();

            currentUser.UserGameTasks.Add(new UserGameTask(currentUser, gameTask));
            _userRepository.Update(currentUser);
        }

        public void SetTaskAsDone(int gameTaskId)
        {
            var usersGameTasks = _userGameTaskRepository.GetAllQueryable();
            var currentUserId = GetCurrentUserId();
            var currentUser = _userRepository.GetById(currentUserId);

            var gameTaskToBeDone =
                usersGameTasks.Single(ugt => ugt.UserId == currentUserId && ugt.GameTaskId == gameTaskId);
            
            gameTaskToBeDone.IsDone = true;
            SetUserPoints(currentUser);

            _userRepository.Update(currentUser);
        }

        public void SetTaskAsDone(int userId, int gameTaskId)
        {
            var usersGameTasks = _userGameTaskRepository.GetAllQueryable();
            var user = _userRepository.GetById(userId);

            var gameTaskToBeDone =
                usersGameTasks.Single(ugt => ugt.UserId == userId && ugt.GameTaskId == gameTaskId);

            gameTaskToBeDone.IsDone = true;
            SetUserPoints(user);

            _userRepository.Update(user);
        }

        public void SetUserPoints(User user)
        {
            var usersGameTasks = _userGameTaskRepository.GetAllQueryable();
            var points = usersGameTasks.Where(ugt => ugt.UserId == user.Id).Sum(ugt => ugt.GameTask.Points);

            user.Points = points;
            _userRepository.Update(user);
        }

        public List<GameTask> GetToDoTasks()
        {
            var usersGameTasks = _userGameTaskRepository.GetAllQueryable();
            var currentUserId = GetCurrentUserId();

            return usersGameTasks.Where(ugt => ugt.UserId == currentUserId && !ugt.IsDone)
                .Select(ugt => ugt.GameTask)
                .Where(gt => gt.ValidFrom >= DateTime.UtcNow && gt.IsActive).ToList();
        }

        public List<GameTask> GetToDoTasks(int userId)
        {
            var usersGameTasks = _userGameTaskRepository.GetAllQueryable();

            return usersGameTasks.Where(ugt => ugt.UserId == userId && !ugt.IsDone)
                .Select(ugt => ugt.GameTask)
                .Where(gt => gt.ValidFrom >= DateTime.UtcNow && gt.IsActive).ToList();
        }

        public List<GameTask> GetInProgressTasks()
        {
            var usersGameTasks = _userGameTaskRepository.GetAllQueryable();
            var currentUserId = GetCurrentUserId();

            return usersGameTasks.Where(ugt => ugt.UserId == currentUserId && !ugt.IsDone)
                .Select(ugt => ugt.GameTask)
                .Where(gt => gt.ValidFrom <= DateTime.UtcNow && gt.ValidThru >= DateTime.UtcNow && gt.IsActive).ToList();
        }

        public List<GameTask> GetInProgressTasks(int userId)
        {
            var usersGameTasks = _userGameTaskRepository.GetAllQueryable();

            return usersGameTasks.Where(ugt => ugt.UserId == userId && !ugt.IsDone)
                .Select(ugt => ugt.GameTask)
                .Where(gt => gt.ValidFrom <= DateTime.UtcNow && gt.ValidThru >= DateTime.UtcNow && gt.IsActive).ToList();
        }

        public List<GameTask> GetDoneTasks()
        {
            var usersGameTasks = _userGameTaskRepository.GetAllQueryable();
            var currentUserId = GetCurrentUserId();

            return usersGameTasks.Where(ugt => ugt.UserId == currentUserId && ugt.IsDone).Select(ugt => ugt.GameTask).ToList();
        }

        public List<GameTask> GetDoneTasks(int userId)
        {
            var usersGameTasks = _userGameTaskRepository.GetAllQueryable();

            return usersGameTasks.Where(ugt => ugt.UserId == userId && ugt.IsDone).Select(ugt => ugt.GameTask).ToList();
        }

        public List<User> SearchByName(string name)
        {
            var users = _userRepository.GetAllQueryable();
            
            return users.Where(u => u.UserName.Equals(name)).ToList();
        }

        public List<User> SearchByEmail(string email)
        {
            var users = _userRepository.GetAllQueryable();
            
            return users.Where(u => u.Email.Equals(email)).ToList();
        }

        public List<User> SearchByGender(Gender gender)
        {
            var users = _userRepository.GetAllQueryable();
            
            return users.Where(u => u.Gender == gender).ToList();
        }

        public List<User> SearchActive(bool isActive)
        {
            var users = _userRepository.GetAllQueryable();
            
            return users.Where(u => u.IsActive == isActive).ToList();
        }

        public List<User> OrderByPoints(bool isDescending)
        {
            var users = _userRepository.GetAllQueryable();
            
            return isDescending ? users.OrderByDescending(u => u.Points).ToList() : users.OrderBy(u => u.Points).ToList();
        }

        public List<User> SearchUsers(string name, string email, string gender, string isActive, string isDescending)
        {
            var users = _userRepository.GetAllQueryable();

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
    }
}