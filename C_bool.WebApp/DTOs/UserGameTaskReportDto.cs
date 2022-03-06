﻿using System;

namespace C_bool.WebApp.DTOs
{
    public class UserGameTaskReportDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int GameTaskId { get; set; }
        public string GameTaskName { get; set; }
        public string GameTaskType { get; set; }
        public int PlaceId { get; set; }
        public string PlaceName { get; set; }
        public bool IsDone { get; set; }
        public DateTime DoneOn { get; set; }
    }
}