using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Core
{
    class EventHolder
    {
        public MethodInfo Signature { get; set; }

        public Type DelegateType { get; set; }
    }
}
