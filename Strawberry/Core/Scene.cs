using Strawberry.Graphics;
using Strawberry.Graphics.Layers;
using Strawberry.Math;
using Strawberry.Misc;
using tainicom.Aether.Physics2D.Dynamics;

namespace Strawberry.Core
{
    public class Scene : ReferenceObject
    {
        public event EventHandler Update = null;

        public event EventHandler FixedUpdate = null;

        public event EventHandler InitInstances = null;

        public event EventHandler Finish = null;

        public IGameContext GameContext { get; private set; }

        public EntityCollection Entities { get; private set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ViewportCollection Viewports { get; set; }

        //public Viewport GuiViewport { get; set; }

        /*public event EventHandler Render = null;

        public event GuiRenderEventHandler GuiRender = null;

        public event EventHandler Update = null;

        public event EventHandler BeginUpdate = null;*/

        public Vector2[] MousePosition { get; private set; }

        //public QuadTree<Collider> CollisionTree { get; private set; }

        List<string> destroyList;

        //public Background ClearBackground { get; set; }

        public int DestroyCount { get; internal set; }

        public Color ClearColor { get; set; }

        public string Name { get; private set; }

        //public World PhysicalWorld { get; set; }

        //public Vector2 Gravity { get { return PhysicalWorld.Gravity; } set { PhysicalWorld.Gravity = value; } }

        public bool IsInitialized { get { return GameContext != null; } }

        public LayerCollection Layers { get; set; } = new LayerCollection();

        public World PhysicsWorld { get; set; }

        public bool PhysicsEnabled { get; private set; }

        public float PixelPerMeter { get; set; } = 32f;

        public Rectangle Bounds
        {
            get { return new Rectangle(0f, 0f, this.Width, this.Height); }
        }

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

        /*public Collider[] FindColliderAtPos(Vector2 position)
        {
            QuadTreeLeaf<Collider> quadTree = CollisionTree.GetLeaf(position);

            List<Collider> result = new List<Collider>();

            foreach (QuadTreeItem<Collider> q in quadTree.Items)
            {
                if (q.Bounds.IsPointInside(position))
                    result.Add(q.Item);
            }

            return result.ToArray();
        }*/

        /*public bool IsFree(Collider collider, Vector2 position)
        {
            Collider[] masks = FindColliderAtPos(position);

            foreach (Collider m in masks)
            {
                if (m == collider)
                    return false;
            }

            return true;
        }*/


        public virtual void Initialize(IGameContext context)
        {
            this.GameContext = context;
            /*GuiViewport = new Viewport(new Vector2(), new Vector2(context.Width, context.Height),
                new Vector2(), new Vector2(context.Width, context.Height));*/
            if (Viewports.Count == 0)
            {
                Viewports.Add(new Viewport("Default", new Vector2(0f, 0f), new Vector2((float)context.Width, (float)context.Height),
                    new Vector2(0f, 0f), new Vector2((float)Width, (float)Height)));
            }
            /*SpriteRenderer = new SpriteRenderer(context.GraphicsContext);
            guiSpriteRenderer = new Graphics.SpriteRenderer(context.GraphicsContext);
            CollisionTree = new QuadTree<Collider>(5, 5, new Rectangle(0f, 0f, Width, Height));
            BeginEntities();*/
        }

        /*public virtual void BeginEntities()
        {
            foreach (Entity en in Entities.Values)
            {
                en.Enable();
            }
        }*/

        public virtual void OnInitInstances()
        {
            if (InitInstances != null)
            {
                InitInstances(this, new EventArgs());
            }
        }

        public virtual void OnFinish()
        {
            if (Finish != null)
            {
                Finish(this, new EventArgs());
            }
        }

        public virtual void OnEndUpdate()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entity en = Entities.Values[i];
                if (i < 0)
                    i = 0;
                if (!en.Destroyed && !en.UpdateEnded && en.Parent == null)
                    Entities.Values[i].OnEndUpdate();
                if (en.Destroyed)
                    destroyList.Add(en.ID);
                i -= DestroyCount;
                DestroyCount = 0;
            }
            foreach (string s in destroyList)
                Entities.Remove(s);
            destroyList.Clear();
        }

        public Vector2 GetMousePositionOnViewport(int viewPortIndex, int pointerIndex)
        {
            Vector2 result = new Vector2(MousePosition[pointerIndex]);
            Viewport vp = Viewports[viewPortIndex];


            result.X = ((result.X - vp.ScreenPos.X) / ((float)GameContext.Width / vp.SceneSize.X) * (float)GameContext.Width / vp.ScreenSize.X) + vp.ScenePos.X;
            result.Y = ((result.Y - vp.ScreenPos.Y) / ((float)GameContext.Height / vp.SceneSize.Y) * (float)GameContext.Height / vp.ScreenSize.Y) + vp.ScenePos.Y;

            return result;
        }

        /*public Vector2 GetMousePositionOnGui(int pointerIndex)
        {
            Vector2 result = new Vector2(MousePosition[pointerIndex]);
            Viewport vp = GuiViewport;


            result.X = ((result.X - vp.ScreenPos.X) / ((float)GameContext.Width / vp.SceneSize.X) * (float)GameContext.Width / vp.ScreenSize.X) + vp.ScenePos.X;
            result.Y = ((result.Y - vp.ScreenPos.Y) / ((float)GameContext.Height / vp.SceneSize.Y) * (float)GameContext.Height / vp.ScreenSize.Y) + vp.ScenePos.Y;

            return result;
        }*/

        public virtual void OnBeginUpdate()
        {
            /*for (int i = 0; i < MouseState.PointerCount; i++)
            {
                if (MouseState.Pointers[i] != null)
                {
                    if (MouseState.Pointers[i].HasPosition)
                    {
                        float x = MouseState.Pointers[i].Position.X * GameContext.Width
                            / (float)GameContext.GraphicsContext.WindowWidth;

                        float y = MouseState.Pointers[i].Position.Y * GameContext.Height
                            / (float)GameContext.GraphicsContext.WindowHeight;

                        MousePosition[i] = new Vector2(x, y);
                    }
                }
            }

            CollisionTree.Clear();*/
            for (int i = 0; i < Entities.Count; i++)
            {
                if (i < 0)
                    i = 0;
                if (!Entities.Values[i].Destroyed && !Entities.Values[i].UpdateBegan && Entities.Values[i].Parent == null)
                    Entities.Values[i].OnBeginUpdate();

                i -= DestroyCount;
                DestroyCount = 0;
            }
        }

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

            /*foreach (ParticleSystem p in ParticleSystems.Values)
            {
                p.Update();
            }*/

            for (int i = 0; i < Entities.Count; i++)
            {
                if (i < 0)
                    i = 0;
                if (!Entities.Values[i].Destroyed && !Entities.Values[i].Updated && Entities.Values[i].Parent == null)
                    Entities.Values[i].OnUpdate();
                i -= DestroyCount;
                DestroyCount = 0;
                //if (Entities.Values[i].Destroyed)
                //i--;
            }
            //foreach (string s in destroyList)
            //Entities.Remove(s);
            destroyList.Clear();
        }
        public virtual void OnFixedUpdate()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (i < 0)
                    i = 0;
                if (!Entities.Values[i].Destroyed && Entities.Values[i].Parent == null)
                    Entities.Values[i].OnFixedUpdate();
                i -= DestroyCount;
                DestroyCount = 0;
                //if (Entities.Values[i].Destroyed)
                //i--;
            }

            if(PhysicsEnabled)
            {
                PhysicsWorld?.Step(FrameInfo.Information.FixedDeltaTime);
            }

            /*if (FixedUpdate != null)
            {
                FixedUpdate(this, new EventArgs());
            }

            if (PhysicalWorld != null)
            {
                PhysicalWorld.Step(1.0f / Game.GameSpeed);
            }
            //foreach (string s in destroyList)
            //Entities.Remove(s);
            destroyList.Clear();*/
        }


        public void DestroyEntity(Type t)
        {
            /*for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities.Values[i].GetType() == t)
                {
                    if (!Entities.Values[i].Destroyed)
                        Entities.Values[i--].Destroy();
                }
            }*/
        }

        /*public int EntityCount(string tag)
        {
            int c = 0;
            foreach (Entity e in Entities.Values)
            {
                if (e.HasTag(tag))
                    c++;
            }

            return c;
        }*/

        public void DestroyEntity(string tag)
        {
            foreach (Entity e in Entities.Values)
            {
                if (e.HasTag(tag))
                    DestroyEntity(e);
            }
        }

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

        /*public Entity[] FindEntities(string tag)
        {
            return (from a in Entities where a.Value.HasTag(tag) select a.Value).ToArray();
        }*/

        public void DestroyEntity(Entity entity)
        {
            if (!entity.Destroyed)
                Entities[entity.ID].Destroy();
        }

        public virtual void DoRender()
        {

        }

        public virtual void DoGuiRender()
        {

        }

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

                /*GameContext.GraphicsContext.SetViewport(GuiViewport);

                if (ClearBackground != null)
                {
                    //SpriteRenderer.PushBackground(ClearBackground, int.MaxValue, new Vector2(),
                    //new Vector2((float)Width / ClearBackground.Width, (float)Height / ClearBackground.Height), new Vector2(1f, 1f));
                }

                SpriteRenderer.Render();

                if (Render != null)
                {
                    Render(this, new RenderEventArgs(SpriteRenderer));
                }

                foreach (Viewport vp in Viewports)
                {
                    GameContext.GraphicsContext.SetViewport(vp);

                    foreach (ParticleSystem p in ParticleSystems.Values)
                    {
                        p.Render(SpriteRenderer);
                    }
                    DoRender();
                    for (int i = 0; i < Entities.Count; i++)
                    {
                        if (!Entities.Values[i].Destroyed && Entities.Values[i].Parent == null)
                            Entities.Values[i].OnRender();
                    }

                    foreach (Layer layer in Layers.Values)
                    {
                        layer.Render();
                    }

                    SpriteRenderer.Render();
                }

                OnGuiRender();*/
            }
        }

        public virtual void OnGuiRender()
        {
            /*GameContext.GraphicsContext.SetViewport(GuiViewport);

            DoGuiRender();

            for (int i = 0; i < Entities.Count; i++)
            {
                if (!Entities.Values[i].Destroyed && Entities.Values[i].Parent == null)
                    Entities.Values[i].OnGuiRender(new GuiRenderEventArgs(guiSpriteRenderer));
                //if (Entities.Values[i].Destroyed)
                //i--;
            }

            guiSpriteRenderer.Render();*/
        }

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

        public void AddLayer(string key, Layer layer)
        {
            if (layer.Scene != null)
            {
                string k = layer.Scene.RemoveLayer(layer);
            }
            Layers.Add(key, layer);
            layer.Initialize(this);
        }

        public virtual void RemoveLayer(string key)
        {
            Layers.Remove(key);
        }

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

        public virtual void EnablePhysics(Vector2 gravity)
        {
            PhysicsWorld = new World(gravity);
            PhysicsEnabled = true;
        }
    }
}
