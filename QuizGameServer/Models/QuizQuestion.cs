namespace QuizGameServer.Models
{
    public class QuizRequest
    {
        public string Topic { get; set; }
        public string Difficulty { get; set; }
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
