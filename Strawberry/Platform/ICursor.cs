namespace Strawberry.Platform;

public interface ICursor : IPlatformService
{
    bool Visible { get; set; }
}