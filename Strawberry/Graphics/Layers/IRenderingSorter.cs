using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strawberry.Graphics.Layers
{
    public interface IRenderingSorter
    {
        void Sort(List<SpriteQuad> quads);
    }
}
