namespace internship_task_backend.Model
{
    public class GetLesson
    {
        public class GetLessonRequest
        {
            public int Id { get; set; }
            public string? Title { get; set; }
        }
        public class GetLessonResponse
        {
            public bool success { get; set; }
            public string? message { get; set; }

            public int id { get; set; }
            public string? title { get; set; }
            public string? description { get; set; }
            public int credit_hour { get; set; }

        }

    }
}
