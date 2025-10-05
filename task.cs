using System.Runtime.CompilerServices;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    



    public Task(int id, string title, string description)
    {
        Id = id;
        Title = title;
        Description = description;
        IsCompleted = false;
        
    }
    public override string ToString()
    {
        string  status = IsCompleted ? "Done" : "Pending...";
        return $"[{Id}] {Title} - {(IsCompleted ? "Done" : "Pending...")}\nDescription: {Description}";
    }

    }
