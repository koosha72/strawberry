namespace Strawberry.Misc;

public interface IStorage
{
    Stream Open(string path);

    byte[] ReadAllBytes(string path);

    public string[] List(string path);

}
