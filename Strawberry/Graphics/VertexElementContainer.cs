/*
 * Strawberry Game Engine
 * File: VertexElementContainer.cs
 * Author: Koosha Aabedini Nassab
 *
 * Container describing vertex element layouts used by shaders.
 */

namespace Strawberry.Graphics
{
    /// <summary>
    /// Represents a container for vertex element layouts used by shaders.
    /// </summary>
    public class VertexElementContainer
    {
        Dictionary<string, ElementFormats> elements;

        public Dictionary<string, ElementFormats> Elements { get { return elements; } }

        public int Size { get; private set; }

        public VertexElementContainer()
        {
            elements = new Dictionary<string, ElementFormats>();
            Size = 0;
        }
        /// <summary>
        /// Adds a vertex element to the container.
        /// </summary>
        /// <param name="name">The name of the vertex element</param>
        /// <param name="format">The format of the vertex element</param>
        public void Add(string name, ElementFormats format)
        {
            elements.Add(name, format);
            switch (format)
            {
                case ElementFormats.Color:
                    Size += 16;
                    break;
                case ElementFormats.Position2:
                    Size += 8;
                    break;
            }
        }

        public static VertexElementContainer VertexPositionTexColor
        {
            get
            {
                VertexElementContainer elementContainer = new VertexElementContainer();
                elementContainer.Add("POSITION", ElementFormats.Position2);
                elementContainer.Add("TEXCOORD", ElementFormats.Position2);
                elementContainer.Add("COLOR", ElementFormats.Color);

                return elementContainer;
            }
        }

        public static VertexElementContainer VertexPositionColor
        {
            get
            {
                VertexElementContainer elementContainer = new VertexElementContainer();
                elementContainer.Add("POSITION", ElementFormats.Position2);
                elementContainer.Add("COLOR", ElementFormats.Color);

                return elementContainer;
            }
        }
    }
}
