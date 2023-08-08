using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineCoursePlatform.DataLayer.Entities.Question
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }

        [Required]
        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        [Required]
        public int UserId { get; set; }
        public User.User? User { get; set; }

        [Required]
        public string AnswerBody { get; set; }

        public bool IsTrue { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }
    }
}
