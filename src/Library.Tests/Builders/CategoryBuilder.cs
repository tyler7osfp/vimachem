using Library.Domain.Entities;

namespace Library.Tests.Builders;

public class CategoryBuilder
{
    private string _name = "Test Category";

    public CategoryBuilder WithName(string name) { _name = name; return this; }

    public Category Build() => Category.Create(_name);
}
