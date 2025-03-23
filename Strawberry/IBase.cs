namespace Strawberry
{
    public interface IBase : IDisposable
    {
        bool IsDisposed { get; }

        void Dispose(bool disposing);
    }
}
