using static internship_task_backend.Model.AddLesson;
using static internship_task_backend.Model.GetLesson;
using static internship_task_backend.Model.LogIn;
using static internship_task_backend.Model.SignUp;

namespace internship_task_backend.DataAccessLayer
{
    public interface IAuthDL
    {
        public Task<SignUpResponse> SignUp(SignUpRequest request);
        public Task<LogInResponse> LogIn(LogInRequest request);

        public Task<AddLessonResponse> AddLesson(AddLessonRequest request);

        public Task<GetLessonResponse> GetLesson();
    }
}
