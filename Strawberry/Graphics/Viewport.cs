using Strawberry.Math;

namespace Strawberry.Graphics
{
    [Serializable]
    public class Viewport
    {
        public Vector2 ScreenPos { get; set; }

        public Vector2 ScreenSize { get; set; }

        public Vector2 ScenePos { get; set; }

        public Vector2 SceneSize { get; set; }

        public bool UsePercent { get; set; } = false;

        public Vector2 PercentPos { get; set; } = new Vector2();

        public Vector2 PercentSize { get; set; } = new Vector2(100);

        public string Name { get; set; }


        public Viewport(string name, Vector2 screenPos, Vector2 screenSize, Vector2 scenePos, Vector2 sceneSize)
        {
            ScreenPos = screenPos;
            ScreenSize = screenSize;
            ScenePos = scenePos;
            SceneSize = sceneSize;
            Name = name;
        }
    }
}
