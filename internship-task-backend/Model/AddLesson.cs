using System.ComponentModel.DataAnnotations;

namespace internship_task_backend.Model
{
    public class AddLesson
    {
        public class AddLessonRequest
        {
            [Required]
            public int Id { get; set; }
            [Required]
            public string? Title { get; set; }
            [Required]
            public string? Description { get; set; }
            [Required]
            public int credit_hour { get; set; }
        }
        public class AddLessonResponse
        {
            public bool success { get; set; }
            public string? message { get; set; }
        }
    }
}
