using Moq;
using DevKnowledgeBase.Infrastructure.Data;
using DevKnowledgeBase.Application.Commands;
using DevKnowledgeBase.Domain.Entities;
using FluentAssertions;
using Xunit;
using Microsoft.EntityFrameworkCore;
using MediatR;
using DevKnowledgeBase.Application.Events;

namespace DevKnowledgeBase.Tests
{
    public class CreateNoteCommandHandlerTests
    {
        private readonly Mock<DevDatabaseContext> _mockDbContext;
        private readonly CreateNoteHandler _handler;
        private readonly Mock<DbSet<Note>> _mockNotesDbSet;
        private readonly Mock<IMediator> _mockMediator;

        public CreateNoteCommandHandlerTests()
        {
            _mockNotesDbSet = new Mock<DbSet<Note>>();
            _mockDbContext = new Mock<DevDatabaseContext>();
            _mockMediator = new Mock<IMediator>();
            _mockDbContext.Setup(db => db.Notes).Returns(_mockNotesDbSet.Object);
            _handler = new CreateNoteHandler(_mockDbContext.Object,_mockMediator.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldCreateNote()
        {
            // Arrange
            var command = new CreateNoteCommand
            {
                Title = "Test Note",
                Content = "This is a test note."
            };

            _mockDbContext.Setup(db => db.Notes.Add(It.IsAny<Note>()));
            _mockMediator.Setup(x => x.Publish(It.IsAny<NewNoteCreatedEvent>(), It.IsAny<CancellationToken>()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeEmpty();
            _mockDbContext.Verify(db => db.Notes.Add(It.Is<Note>(x => Equals(x.Content, command.Content))), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockMediator.Verify(med => med.Publish(It.IsAny<NewNoteCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
