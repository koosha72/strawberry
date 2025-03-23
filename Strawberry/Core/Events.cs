using System.Reflection;

namespace Strawberry.Core
{
    class EventHolder
    {
        public MethodInfo Signature { get; set; }

        public Type DelegateType { get; set; }
    }
}
