namespace Strawberry.Platform.Web;

public interface IAOTDownloader : IPlatformService
{
    Task AOTDownload(string path);
}