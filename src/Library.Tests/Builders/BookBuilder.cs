using Library.Domain;
using Library.Domain.Entities;

namespace Library.Tests.Builders;

public class BookBuilder
{
    private string _title = "Test Book";
    private Guid _categoryId = Guid.NewGuid();
    private Party _author = Party.Create("Default Author", "author@test.com", RoleType.Author);
    private int _copies = 1;

    public BookBuilder WithTitle(string title) { _title = title; return this; }
    public BookBuilder WithCategoryId(Guid id) { _categoryId = id; return this; }
    public BookBuilder WithAuthor(Party author) { _author = author; return this; }
    public BookBuilder WithCopies(int copies) { _copies = copies; return this; }

    public Book Build() => Book.Create(_title, _author, _categoryId, _copies);
}
