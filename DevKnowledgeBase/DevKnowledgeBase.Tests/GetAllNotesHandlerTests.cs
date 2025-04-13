using AutoMapper;
using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Application.Common;
using DevKnowledgeBase.Application.Queries;
using DevKnowledgeBase.Domain.Dtos;
using DevKnowledgeBase.Domain.Entities;
using DevKnowledgeBase.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
namespace DevKnowledgeBase.Tests
{
    public class GetAllNotesHandlerTests
    {
        private readonly DevDatabaseContext _dbContext;
        private readonly GetAllNotesHandler _handler;
        private readonly IMapper _mapper;

        public GetAllNotesHandlerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            var dbContextOptions = new DbContextOptionsBuilder<DevDatabaseContext>()
               .UseInMemoryDatabase("InMemoryDatabase")
               .Options;

            _dbContext = new DevDatabaseContext(dbContextOptions);

            var fakeNotes = new List<Note>
            {
                new() { Id = Guid.NewGuid(), Title = "Test Note 1", Content = "Content 1" },
                new() { Id = Guid.NewGuid(), Title = "Test Note 2", Content = "Content 2" }
            };

            _dbContext.Notes.AddRange(fakeNotes);
            _dbContext.SaveChanges();
            _handler = new GetAllNotesHandler(_dbContext, _mapper);
        }

        [Fact]
        public async Task Handle_ValidQuery_ShouldReturnAllNotes()
        {
            // Arrange
            var query = new GetAllNotesQuery(1, 100, null);
           
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            var expectedResult = _dbContext.Notes.Select(x => new NoteDto(x.Id, x.Title, x.Content, x.Tags)).ToList();

            // Assert
            result.Equals(expectedResult);
        }
    }
}
