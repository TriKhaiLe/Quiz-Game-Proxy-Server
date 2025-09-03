namespace QuizGameServer.Application.Contracts
{
    public class ShareQuizResultDto
    {
        public required string Topic { get; set; }
        public int Difficulty { get; set; }
        public required List<string> UserAnswers { get; set; }
        public required List<SharedQuizQuestionDto> Questions { get; set; }
    }
}
