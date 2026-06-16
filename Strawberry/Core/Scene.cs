/*
 * Strawberry Game Engine
 * File: Scene.cs
 * Author: Koosha Aabedini Nassab
 *
 * Scene container for entities, layers, viewports, and physics simulation.
 */

using Strawberry.Common;
using Strawberry.Graphics;
using Strawberry.Graphics.Layers;
using Strawberry.Math;
using tainicom.Aether.Physics2D.Dynamics;

namespace Strawberry.Core
{
    /// <summary>
    /// Represents a game scene that manages entities, viewports, layers, and physics.
    /// Acts as a container for all objects and logic within a specific state or level of the game.
    /// </summary>
    public class Scene : ReferenceObject
    {
        /// <summary>
        /// Occurs when the scene is updated.
        /// </summary>
        public event EventHandler Update = null;

        /// <summary>
        /// Occurs when instances within the scene are being initialized.
        /// </summary>
        public event EventHandler InitInstances = null;

        /// <summary>
        /// Occurs when the scene is finishing.
        /// </summary>
        public event EventHandler Finish = null;

        /// <summary>
        /// Gets the game context associated with this scene.
        /// </summary>
        public IGameContext GameContext { get; private set; }

        /// <summary>
        /// Gets the collection of entities currently in the scene.
        /// </summary>
        public EntityCollection Entities { get; private set; }

        /// <summary>
        /// Gets or sets the width of the scene in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the scene in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the collection of viewports used to render the scene.
        /// </summary>
        public ViewportCollection Viewports { get; set; }

        /// <summary>
        /// Gets an array of mouse positions, indexed by pointer ID.
        /// </summary>
        public Vector2[] MousePosition { get; private set; }

        //public QuadTree<Collider> CollisionTree { get; private set; }

        /// <summary>
        /// A list of entity IDs pending destruction.
        /// </summary>
        List<string> destroyList;

        //public Background ClearBackground { get; set; }

        /// <summary>
        /// Gets the total number of entities that have been destroyed in the scene.
        /// </summary>
        public int DestroyCount { get; internal set; }

        /// <summary>
        /// Gets or sets the color used to clear the screen before rendering the scene.
        /// If set to transparent, the previous frame's content may remain.
        /// </summary>
        public Color ClearColor { get; set; }

        /// <summary>
        /// Gets the name of the scene.
        /// </summary>
        public string Name { get; private set; }

        //public World PhysicalWorld { get; set; }

        //public Vector2 Gravity { get { return PhysicalWorld.Gravity; } set { PhysicalWorld.Gravity = value; } }

        /// <summary>
        /// Gets a value indicating whether the scene has been initialized and has a valid <see cref="GameContext"/>.
        /// </summary>
        public bool IsInitialized { get { return GameContext != null; } }

        /// <summary>
        /// Gets or sets the collection of rendering layers used to order draw calls.
        /// </summary>
        public LayerCollection Layers { get; set; } = new LayerCollection();

        /// <summary>
        /// Gets or sets the Aether.Physics2D world used for physics simulation in this scene.
        /// </summary>
        public World PhysicsWorld { get; set; }

        /// <summary>
        /// Gets a value indicating whether the physics simulation is enabled for this scene.
        /// </summary>
        public bool PhysicsEnabled { get; private set; }

        /// <summary>
        /// Gets or sets the pixel-to-meter conversion ratio used by the physics simulation.
        /// </summary>
        public float PixelPerMeter { get; set; } = 32f;

        /// <summary>
        /// Gets the bounding rectangle of the scene, defined by its width and height.
        /// </summary>
        public Rectangle Bounds
        {
            get { return new Rectangle(0f, 0f, this.Width, this.Height); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class.
        /// </summary>
        /// <param name="name">The name of the scene.</param>
        /// <param name="width">The width of the scene in pixels.</param>
        /// <param name="height">The height of the scene in pixels.</param>
        public Scene(string name, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Name = name;
            Viewports = new ViewportCollection();
            Entities = new EntityCollection();
            MousePosition = new Vector2[10];
            destroyList = new List<string>();
            //ParticleSystems = new Dictionary<string, ParticleSystem>();
            this.ClearColor = Color.Transparent;
            DestroyCount = 0;
            //PhysicalWorld = new World(new Vector2());
        }

        /// <summary>
        /// Initializes the scene with the specified game context and creates a default viewport if none exist.
        /// </summary>
        /// <param name="context">The game context to associate with this scene.</param>
        public virtual void Initialize(IGameContext context)
        {
            this.GameContext = context;
            if (Viewports.Count == 0)
            {
                Viewports.Add(new Viewport("Default", new Vector2(0f, 0f), new Vector2((float)context.Width, (float)context.Height),
                    new Vector2(0f, 0f), new Vector2((float)Width, (float)Height)));
            }
        }

        /// <summary>
        /// Raises the <see cref="InitInstances"/> event.
        /// </summary>
        public virtual void OnInitInstances()
        {
            if (InitInstances != null)
            {
                InitInstances(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the <see cref="Finish"/> event.
        /// </summary>
        public virtual void OnFinish()
        {
            if (Finish != null)
            {
                Finish(this, new EventArgs());
            }
        }

        /// <summary>
        /// Calls <see cref="Entity.OnEndUpdate"/> on all active root entities and processes the destruction of entities marked for removal.
        /// </summary>
        public virtual void OnEndUpdate()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entity en = Entities.Values[i];

                if (!en.Destroyed && !en.UpdateEnded && en.Parent == null)
                    Entities.Values[i].OnEndUpdate();
                if (en.Destroyed)
                    destroyList.Add(en.ID);
            }
            Entities.Flush();
            destroyList.Clear();
        }

        /// <summary>
        /// Calculates the position of the mouse pointer relative to a specific viewport's scene coordinates.
        /// </summary>
        /// <param name="viewPortIndex">The index of the viewport in the <see cref="Viewports"/> collection.</param>
        /// <param name="pointerIndex">The index of the pointer (e.g., for multi-touch).</param>
        /// <returns>The translated mouse position in scene-space coordinates.</returns>
        public Vector2 GetMousePositionOnViewport(int viewPortIndex, int pointerIndex)
        {
            Vector2 mousePosition = new Vector2(GameContext.InputManager.PointingDevice.GetPosition(pointerIndex));
            Viewport vp = Viewports[viewPortIndex];

            return vp.ScreenToScene(mousePosition);
        }

        /// <summary>
        /// Calls <see cref="Entity.OnBeginUpdate"/> on all active root entities.
        /// </summary>
        public virtual void OnBeginUpdate()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (i < 0)
                    i = 0;
                if (!Entities.Values[i].Destroyed && !Entities.Values[i].UpdateBegan && Entities.Values[i].Parent == null)
                    Entities.Values[i].OnBeginUpdate();
            }
        }

        /// <summary>
        /// Updates the scene by raising the <see cref="Update"/> event, updating layers, and calling <see cref="Entity.OnUpdate"/> on all active root entities.
        /// </summary>
        public virtual void OnUpdate()
        {
            if (Update != null)
            {
                Update(this, new EventArgs());
            }

            foreach (Layer layer in Layers.Values)
            {
                layer.Update();
            }

            for (int i = 0; i < Entities.Count; i++)
            {
                if (i < 0)
                    i = 0;
                if (!Entities.Values[i].Destroyed && !Entities.Values[i].Updated && Entities.Values[i].Parent == null)
                    Entities.Values[i].OnUpdate();
            }
        }

        /// <summary>
        /// Performs fixed-time step updates for entities and steps the physics world if enabled.
        /// </summary>
        public virtual void OnFixedUpdate()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (i < 0)
                    i = 0;
                if (!Entities.Values[i].Destroyed && Entities.Values[i].Parent == null)
                    Entities.Values[i].OnFixedUpdate();
                //if (Entities.Values[i].Destroyed)
                //i--;
            }

            if (PhysicsEnabled)
            {
                PhysicsWorld?.Step(FrameInfo.Information.FixedDeltaTime);
            }
        }


        /// <summary>
        /// Destroys all entities of a specific type. Note: This method is currently not implemented.
        /// </summary>
        /// <param name="t">The type of entity to destroy.</param>
        public void DestroyEntity(Type t)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities.Values[i].GetType() == t)
                {
                    if (!Entities.Values[i].Destroyed)
                        Entities.Values[i--].Destroy();
                }
            }
        }

        public int EntityCount(string tag)
        {
            int c = 0;
            foreach (Entity e in Entities.Values)
            {
                if (e.HasTag(tag))
                    c++;
            }

            return c;
        }

        /// <summary>
        /// Destroys all entities that have the specified tag.
        /// </summary>
        /// <param name="tag">The tag used to identify entities to destroy.</param>
        public void DestroyEntity(string tag)
        {
            foreach (Entity e in Entities.Values)
            {
                if (e.HasTag(tag))
                    DestroyEntity(e);
            }
        }

        /// <summary>
        /// Adds an entity to the scene. If the scene is already initialized, the entity is enabled immediately.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual void AddEntity(Entity entity)
        {
            if (Entities.ContainsKey(entity.ID))
            {
                if (Entities[entity.ID].Destroyed)
                    Entities.Remove(entity.ID);
            }

            Entities.Add(entity.ID, entity);
            if (IsInitialized)
                entity.Enable();
        }

        public Entity[] FindEntities(string tag)
        {
            return (from a in Entities where a.Value.HasTag(tag) select a.Value).ToArray();
        }

        /// <summary>
        /// Marks the specified entity for destruction.
        /// </summary>
        /// <param name="entity">The entity to destroy.</param>
        public void DestroyEntity(Entity entity)
        {
            if (!entity.Destroyed)
                Entities[entity.ID].Destroy();
        }

        /// <summary>
        /// Performs custom rendering logic. Override this to add rendering outside of the standard entity rendering.
        /// </summary>
        public virtual void DoRender()
        {

        }

        /// <summary>
        /// Performs custom GUI rendering logic. Override this to add GUI rendering.
        /// </summary>
        public virtual void DoGuiRender()
        {

        }

        /// <summary>
        /// Renders the scene by iterating through viewports and drawing active entities and layers.
        /// </summary>
        public virtual void OnRender()
        {
            if (IsInitialized)
            {
                foreach (Viewport vp in Viewports)
                {
                    GameContext.GraphicsContext.SetViewport(vp);

                    for (int i = 0; i < Entities.Count; i++)
                    {
                        if (!Entities.Values[i].Destroyed && Entities.Values[i].Parent == null)
                            Entities.Values[i].OnRender();
                    }

                    foreach (Layer layer in Layers.Values)
                    {
                        layer.Render();
                    }
                }
            }
        }

        /// <summary>
        /// Clears the rendering surface using the <see cref="ClearColor"/> if it is not transparent.
        /// </summary>
        /// <returns><c>true</c> if the screen was cleared; <c>false</c> if the clear color was transparent.</returns>
        public bool OnBeginRender()
        {
            if (ClearColor != Color.Transparent)
            {
                this.GameContext.GraphicsContext.Clear(ClearColor);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Adds a rendering layer to the scene. If the layer belongs to another scene, it is removed from that scene first.
        /// </summary>
        /// <param name="key">The unique key identifying the layer.</param>
        /// <param name="layer">The layer to add.</param>
        public void AddLayer(string key, Layer layer)
        {
            if (layer.Scene != null)
            {
                string k = layer.Scene.RemoveLayer(layer);
            }
            Layers.Add(key, layer);
            layer.Initialize(this);
        }

        /// <summary>
        /// Removes the rendering layer with the specified key.
        /// </summary>
        /// <param name="key">The key of the layer to remove.</param>
        public virtual void RemoveLayer(string key)
        {
            Layers.Remove(key);
        }

        /// <summary>
        /// Removes the specified rendering layer from the scene.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        /// <returns>The key associated with the removed layer, or <c>null</c> if the layer was not found.</returns>
        public virtual string RemoveLayer(Layer layer)
        {
            string key = null;
            foreach (KeyValuePair<string, Layer> kvp in Layers)
            {
                if (kvp.Value == layer)
                {
                    key = kvp.Key;
                    break;
                }
            }
            if (!String.IsNullOrEmpty(key))
                Layers.Remove(key);
            return key;
        }

        /// <summary>
        /// Enables the physics simulation with the specified gravity vector.
        /// </summary>
        /// <param name="gravity">The gravity vector applied to the physics world.</param>
        public virtual void EnablePhysics(Vector2 gravity)
        {
            PhysicsWorld = new World(gravity);
            PhysicsEnabled = true;
        }
    }
}