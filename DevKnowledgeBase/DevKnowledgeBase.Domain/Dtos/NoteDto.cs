namespace DevKnowledgeBase.Domain.Dtos
{
    public record NoteDto(Guid Id, string Title, string Content, List<string> Tags);
}
