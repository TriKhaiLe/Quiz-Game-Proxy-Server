namespace QuizGameServer.Models
{
    public class Question
    {
        public string Content { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }
}