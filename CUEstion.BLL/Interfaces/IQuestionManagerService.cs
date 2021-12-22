using System;
using System.Collections.Generic;
using CUEstion.BLL.ModelsDTO;

namespace CUEstion.BLL.Interfaces
{
    public interface IQuestionManagerService
    {
        public IEnumerable<QuestionDTO> GetAllQuestions();
        public IEnumerable<String> GetAllTags();
        public IEnumerable<QuestionDTO> GetNewestQuestions(int count);
        public QuestionDTO GetQuestion(int questionId);
        public IEnumerable<QuestionDTO> Search(string query, string[] tags);
        public IEnumerable<QuestionDTO> FilterQuestions(string[] tags);
        public IEnumerable<QuestionDTO> GetUsersQuestions(int userId);
        public void CreateQuestion(QuestionDTO questionDto);
        public void UpdateQuestion(QuestionDTO questionDto);
        public void DeleteQuestion(int questionId);
        public void SubscribeToQuestion(int questionId, int userId);
        public IEnumerable<AnswerDTO> GetAnswers(int questionId);
        public void CreateAnswer(AnswerDTO answerDto, int questionId);
        public void UpdateAnswer(AnswerDTO answerDto);
        public void DeleteAnswer(int answerId);
        public IEnumerable<CommentDTO> GetComments(int? questionId, int? answerId);
        public void CreateComment(CommentDTO commentDto, int? questionId, int? answerId);
        public void UpdateComment(CommentDTO commentDto);
        public void DeleteComment(int commentId);
        public void MarkQuestion(int userId, int questionId, int mark);
        public void MarkAnswer(int userId, int answerId, int mark);
        public void MarkComment(int userId, int commentId, int mark);
        public void CreateTag(string tag);
        public void UpdateTag(string oldTag, string newTag);
        public void DeleteTag(string tag);
        public bool IsSubscribedToQuestion(int questionId, int userId);
        public void UnsubscribeFromQuestion(int questionId, int userId);
    }
}