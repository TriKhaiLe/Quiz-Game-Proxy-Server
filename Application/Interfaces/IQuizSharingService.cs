using QuizGameServer.Application.Contracts;
using QuizGameServer.Domain.Entities;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace QuizGameServer.Application.Interfaces
{
    public interface IQuizSharingService
    {
        Task<Guid> ShareQuizAsync(string topic, int difficulty, int currentQuestionIndex, List<SharedQuizQuestionDto> questions, CancellationToken cancellationToken = default);
        Task<SharedQuiz> GetSharedQuizAsync(Guid id, CancellationToken cancellationToken = default);
    }
}