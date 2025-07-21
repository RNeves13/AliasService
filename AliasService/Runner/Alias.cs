namespace AliasService.Runner;

public class Alias(string name, string path)
{
    public string Name { get; } = name;
    public string Path { get; } = path;
}