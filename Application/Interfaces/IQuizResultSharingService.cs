using QuizGameServer.Application.Contracts;
using QuizGameServer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QuizGameServer.Application.Interfaces
{
    public interface IQuizResultSharingService
    {
        Task<Guid> ShareQuizResultAsync(string topic, int difficulty, List<string> userAnswers, List<SharedQuizQuestionDto> questions, CancellationToken cancellationToken = default);
        Task<QuizResult?> GetQuizResultAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
