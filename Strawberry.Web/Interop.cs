using System.Runtime.InteropServices.JavaScript;

namespace Strawberry.Web;

public static partial class Interop
{
    [JSImport("initialize", "main.js")]
    public static partial void Initialize();
}
