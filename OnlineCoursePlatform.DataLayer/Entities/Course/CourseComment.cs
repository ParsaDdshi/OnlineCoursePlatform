using System.ComponentModel.DataAnnotations;

namespace OnlineCoursePlatform.DataLayer.Entities.Course
{
    public class CourseComment
    {
        [Key]
        public int CommentId { get; set; }

        public int CourseId { get; set; }

        public int UserId { get; set; }

        [MaxLength(700)]
        public string Comment { get; set; }

        public DateTime CreateDate { get; set; }

        public Course Course { get; set; }
        public User.User User { get; set; }
    }
}
