using System;

namespace EvernoteClone.Model;

public interface IHasId
{
    public string Id { get; set; }
}

public class Note : IHasId
{
    public string Id { get; set; }
    public string NotebookId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public string FileLocation { get; set; }
}