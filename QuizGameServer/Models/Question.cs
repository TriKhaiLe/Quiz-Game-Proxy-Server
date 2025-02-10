namespace QuizGameServer.Models
{
    public class Question
    {
        public string QuestionText { get; set; }
        public string[] Answers { get; set; }
        public string CorrectAnswer { get; set; }
    }
}