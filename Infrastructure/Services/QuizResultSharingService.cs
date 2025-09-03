using Microsoft.EntityFrameworkCore;
using QuizGameServer.Application.Contracts;
using QuizGameServer.Application.Interfaces;
using QuizGameServer.Domain.Entities;
using QuizGameServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace QuizGameServer.Infrastructure.Services
{
    public class QuizResultSharingService : IQuizResultSharingService
    {
        private readonly QuizGameDbContext _db;
        public QuizResultSharingService(QuizGameDbContext db)
        {
            _db = db;
        }

        public async Task<Guid> ShareQuizResultAsync(string topic, int difficulty, List<string> userAnswers, List<SharedQuizQuestionDto> questions, CancellationToken cancellationToken = default)
        {
            // Tính hash cho bộ câu hỏi
            var hashInput = $"{topic}|{difficulty}|" + string.Join("|", questions.Select(q => q.Question + string.Join(",", q.Options) + q.CorrectAnswer));
            string hash;
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(hashInput);
                hash = BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "");
            }

            // Tìm QuizContent đã tồn tại
            var quizContent = await _db.QuizContents.Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.ContentHash == hash, cancellationToken);
            if (quizContent == null)
            {
                quizContent = new QuizContent
                {
                    Topic = topic,
                    Difficulty = difficulty,
                    ContentHash = hash,
                    Questions = questions.Select(q => new QuizContentQuestion
                    {
                        Question = q.Question,
                        Options = q.Options,
                        CorrectAnswer = q.CorrectAnswer
                    }).ToList()
                };
                _db.QuizContents.Add(quizContent);
                await _db.SaveChangesAsync(cancellationToken);
            }

            var result = new QuizResult
            {
                Topic = topic,
                Difficulty = difficulty,
                UserAnswers = userAnswers,
                QuizContentId = quizContent.Id
            };
            _db.QuizResults.Add(result);
            await _db.SaveChangesAsync(cancellationToken);
            return result.Id;
        }

        public async Task<QuizResult?> GetQuizResultAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.QuizResults.Include(q => q.QuizContent)
                .ThenInclude(qc => qc.Questions)
                .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        }
    }
}
