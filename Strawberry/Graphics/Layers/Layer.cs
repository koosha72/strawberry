using Strawberry.Core;

namespace Strawberry.Graphics.Layers
{
    public abstract class Layer : ReferenceObject
    {
        public Scene Scene
        {
            get;
            private set;
        }

        public List<string> Viewports { get; private set; } = new List<string>();

        public virtual void Initialize(Scene scene)
        {
            this.Scene = scene;
            Viewports.Add("Default");
        }

        public IRenderingSorter Sorter { get; set; }

        public bool Enabled { get; set; } = true;

        public virtual void Render() { }

        public virtual void Update() { }
    }
}
