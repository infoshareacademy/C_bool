﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace C_bool.BLL.DAL.Entities
{
    public class UserGameTask : IEntity
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }
        [Key, Column(Order = 1)]
        public int GameTaskId { get; set; }
        public virtual User User { get; set; }
        public virtual GameTask GameTask { get; set; }
        public bool IsDone { get; set; }
        public DateTime DoneOn { get; set; }
        [NotMapped]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Photo { get; set; }
        public string TextCriterion { get; set; }
        public DateTime ArrivalTime { get; set; }
        public IList<Message> Messages { get; set; }

        public UserGameTask()
        {
        }

        public UserGameTask(User user, GameTask gameTask)
        {
            User = user;
            GameTask = gameTask;
            IsDone = false;
            CreatedOn = DateTime.UtcNow;
            Messages = new List<Message>();
        }

    }
}