/*
 * Strawberry Game Engine
 * File: Layer.cs
 * Author: Koosha Aabedini Nassab
 *
 * Base class for rendering and updating scene layers.
 */

using Strawberry.Core;

namespace Strawberry.Graphics.Layers
{
    /// <summary>
    /// Represents a rendering layer that can be initialized, updated, and rendered within a scene.
    /// </summary>
    public abstract class Layer : ReferenceObject
    {
        /// <summary>
        /// Gets the scene to which this layer belongs.
        /// </summary>
        public Scene Scene
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the collection of viewport names associated with this layer.
        /// </summary>
        public List<string> Viewports { get; private set; } = new List<string>();

        /// <summary>
        /// Initializes the layer with the specified scene.
        /// </summary>
        /// <param name="scene">The scene that owns the layer.</param>
        public virtual void Initialize(Scene scene)
        {
            this.Scene = scene;
            Viewports.Add("Default");
        }

        /// <summary>
        /// Gets or sets the rendering sorter used by this layer.
        /// </summary>
        public IRenderingSorter Sorter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the layer is enabled for updates and rendering.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Renders the contents of the layer.
        /// </summary>
        public virtual void Render() { }

        /// <summary>
        /// Updates the contents of the layer.
        /// </summary>
        public virtual void Update() { }
    }
}
