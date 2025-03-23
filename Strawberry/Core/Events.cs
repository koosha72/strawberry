using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Core
{
    class ComponentMethods
    {
        public Action Update = null;
        public Action BeginUpdate = null;
        public Action EndUpdate = null;
        public Action FixedUpdate = null;
        public Action Render = null;
        public Action GuiRender = null;
        public Action Finish = null;
    }

    class EventHolder
    {
        public MethodInfo Signature { get; set; }

        public Type DelegateType { get; set; }
    }
}
