using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Serialization
{
    public abstract class SerializeTemplate
    {
        public abstract byte[] GetBytes(object obj);

        public abstract object GetObjectBack(byte[] bytes);
    }
}
