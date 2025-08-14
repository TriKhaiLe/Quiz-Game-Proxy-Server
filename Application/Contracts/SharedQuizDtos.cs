namespace QuizGameServer.Application.Contracts
{
    public class ShareQuizRequest
    {
        public string Topic { get; set; }
        public int Difficulty { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public List<SharedQuizQuestionDto> Questions { get; set; }
    }
    public class SharedQuizQuestionDto
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
    }
    public class GetSharedQuizResponse
    {
        public string Topic { get; set; }
        public int Difficulty { get; set; }
        public int StartIndex { get; set; }
        public List<SharedQuizQuestionDto> Questions { get; set; }
    }
}
