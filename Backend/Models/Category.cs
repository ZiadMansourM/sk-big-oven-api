namespace Backend.Models;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Category(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public List<Category> ToList()
    {
        return new List<Category> { this };
    }
}