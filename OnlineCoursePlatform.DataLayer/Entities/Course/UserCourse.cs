﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCoursePlatform.DataLayer.Entities.Course
{
    public class UserCourse
    {
        [Key]
        public int UserCourseId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }

        #region Relations

        public Course Course { get; set; }
        public User.User User { get; set; }

        #endregion
    }
}
