namespace Strawberry.Core
{
    /// <summary>
    /// A collection of scenes
    /// </summary>
    public class SceneCollection : List<Scene>
    {
        public Scene this[string key]
        {
            get
            {
                return Find(x => x.Name == key);
            }
        }
    }
}
