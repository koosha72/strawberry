using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry
{
    public interface IBase : IDisposable
    {
        bool IsDisposed { get; }

        void Dispose(bool disposing);
    }
}
