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
    public class QuizSharingService : IQuizSharingService
    {
        private readonly QuizGameDbContext _db;
        public QuizSharingService(QuizGameDbContext db)
        {
            _db = db;
        }

        public async Task<Guid> ShareQuizAsync(string topic, int difficulty, int currentQuestionIndex, List<SharedQuizQuestionDto> questions, CancellationToken cancellationToken = default)
        {
            // Tính hash cho b? câu h?i
            var hashInput = $"{topic}|{difficulty}|" + string.Join("|", questions.Select(q => q.Question + string.Join(",", q.Options) + q.CorrectAnswer));
            string hash;
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(hashInput);
                hash = BitConverter.ToString(sha.ComputeHash(bytes)).Replace("-", "");
            }

            // Tìm QuizContent ?ã t?n t?i
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

            // not wrap in else block to avoid multiple queries at the same time
            var sharedQuiz = await _db.SharedQuizzes
                .FirstOrDefaultAsync(q => q.QuizContentId == quizContent.Id && q.CurrentQuestionIndex == currentQuestionIndex, cancellationToken);
            if (sharedQuiz != null)
            {
                return sharedQuiz.Id;
            }

            sharedQuiz = new SharedQuiz
            {
                CurrentQuestionIndex = currentQuestionIndex,
                QuizContentId = quizContent.Id
            };
            _db.SharedQuizzes.Add(sharedQuiz);
            await _db.SaveChangesAsync(cancellationToken);
            return sharedQuiz.Id;
        }

        public async Task<SharedQuiz> GetSharedQuizAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _db.SharedQuizzes
                .Include(q => q.QuizContent)
                .ThenInclude(qc => qc.Questions)
                .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
        }
    }
}
