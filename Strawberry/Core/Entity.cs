using Strawberry.Serialization;
using System.Reflection;

namespace Strawberry.Core
{
    public class Entity : ReferenceObject
    {
        protected ComponentCollection Components;

        public List<BaseComponent> AllComponents
        {
            get { return Components; }
        }

        Dictionary<string, EventHolder> registeredEvents = new Dictionary<string, EventHolder>();

        Dictionary<BaseComponent, Dictionary<string, Delegate>> componentEvents =
            new Dictionary<BaseComponent, Dictionary<string, Delegate>>();

        public Scene Scene { get; private set; }

        public string ID { get; set; }

        public bool Destroyed { get; private set; }

        public string Tag
        {
            set
            {
                string[] temp = value.Split(',');
                foreach (var t in temp)
                    tags.Add(t);
            }
            get
            {
                if (tags.Count > 0)
                    return string.Join(",", tags);
                else
                    return null;
            }
        }

        public bool UpdateBegan { get; private set; }

        public bool Updated { get; private set; }

        public bool UpdateEnded { get; private set; }

        public PauseStateFlags PauseState { get; set; }

        Entity parent;

        public Entity Parent
        {
            get { return parent; }
            set
            {
                if (parent != null)
                    parent.children.Remove(this.ID);

                if (value != null)
                {
                    if (value.Scene == Scene)
                    {
                        this.parent = value;
                        parent.children.Add(this.ID, this);
                    }
                }
                else
                {
                    parent = value;
                }
                //NotifyPropertyChanged("Parent");
            }
        }

        EntityCollection children = new EntityCollection();

        public EntityCollection Children { get { return children; } }

        HashSet<string> tags = new HashSet<string>();

        public Entity()
        {
            Components = new ComponentCollection();
            PauseState = PauseStateFlags.None;
            Destroyed = false;
        }

        public void Initialize(string id, Scene owner)
        {
            Scene = owner;
            ID = id;
            owner.AddEntity(this);
            Parent = null;
        }

        public void Initialize(string id, Entity parent)
        {
            Scene = parent.Scene;
            ID = id;
            parent.children.Add(ID, this);
            Scene.AddEntity(this);
            this.parent = parent;
        }

        public void Enable()
        {
            OnInitialize(ID, Scene);
        }

        public Entity Clone(string newId)
        {
            Entity en = new Entity();
            if (Parent != null)
                en.Initialize(newId, Parent);
            else
                en.Initialize(newId, Scene);

            SBSerializer serializer = new SBSerializer();
            serializer.Serialize(this.AllComponents.ToArray());
            var newCmps = serializer.Deserialize();

            foreach (BaseComponent component in newCmps)
            {
                en.AddComponent(component);
            }

            return en;
        }

        public bool IsChildOf(Entity parent)
        {
            Entity p = Parent;
            while (p != null)
            {
                if (p == parent)
                    return true;
                p = p.Parent;
            }
            return false;
        }

        #region public methods
        /*public void Initialize(string id, List<IComponent> components, World owner)
        {
            this.Initialize(id, owner);

            foreach (IComponent c in components)
                this.AddComponent(c);
        }*/

        internal void AddComponent(BaseComponent component, bool init)
        {
            Components.Add(component);
            component.Owner = this;
            if (init)
            {
                component.Initialize(this);
                MethodInfo begin = component.GetType().GetMethod("Begin", BindingFlags.Instance | BindingFlags.Public,
                       null, Type.EmptyTypes, null);
                if (begin != null)
                {
                    if (begin.ReturnType == typeof(void))
                        begin.Invoke(component, null);
                }
                InvokeEvents("Enabled");
            }

            MethodInfo loaded = component.GetType().GetMethod("Loaded", BindingFlags.Instance | BindingFlags.Public,
                       null, Type.EmptyTypes, null);
            if (loaded != null)
            {
                if (loaded.ReturnType == typeof(void))
                    loaded.Invoke(component, null);
            }
        }

        public T AddComponent<T>() where T : BaseComponent, new()
        {
            T component = new T();
            return AddComponent(component);
        }

        public T AddComponent<T>(T component) where T : BaseComponent
        {
            Components.Add(component);
            component.Owner = this;
            component.Initialize(this);
            RegisterEventsForComponent(component);
            InvokeEvent(component, "Begin");
            InvokeEvent(component, "Enabled");
            InvokeEvents("ComponentAdded", component);

            return component;
        }


        public void AddComponents<T>(List<T> components) where T : BaseComponent, new()
        {
            foreach (T c in components)
            {
                AddComponent<T>();
            }
        }

        public T GetComponent<T>() where T : BaseComponent
        {
            Type t = typeof(T);
            T c = (T)(from cmp in Components where cmp.GetType() == t select cmp).FirstOrDefault();

            return c;
        }

        public bool HasComponent<T>() where T : BaseComponent
        {
            Type t = typeof(T);
            return (from cmp in Components where cmp.GetType() == t select cmp).Count() > 0;
        }

        public T[] GetComponents<T>() where T : BaseComponent
        {
            Type t = typeof(T);
            T[] c = (from cmp in Components where cmp.GetType() == t select cmp as T).ToArray();

            return c;
        }

        public BaseComponent[] GetComponents()
        {
            List<BaseComponent> result = new List<BaseComponent>();
            foreach (BaseComponent cmp in Components)
            {
                result.Add(cmp);
            }

            return result.ToArray();
        }

        public int GetComponentIndex(BaseComponent component)
        {
            return Components.IndexOf(component);
        }

        public void SetComponentIndex(BaseComponent component, int index)
        {

        }

        public void RemoveComponent<T>() where T : BaseComponent
        {
            Type t = typeof(T);
            T c = (T)(from cmp in Components where cmp.GetType() == t select cmp).First();
            //RemoveEvents(c);
            InvokeEvent(c, "Disabled");
            InvokeEvent(c, "Finished");
            UnRegisterEventsForComponent(c);
            Components.Remove(c);
            c.Destroy();
        }

        public void RemoveComponent(BaseComponent component)
        {
            Type t = component.GetType();
            BaseComponent c = (BaseComponent)(from cmp in Components where cmp.GetType() == t select cmp).First();
            //RemoveEvents(c);
            InvokeEvent(c, "Disabled");
            InvokeEvent(c, "Finished");
            UnRegisterEventsForComponent(c);
            Components.Remove(c);
            c.Destroy();
        }

        public void ClearComponents()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                var c = Components[i];
                InvokeEvent(c, "Disabled");
                InvokeEvent(c, "Finished");
                UnRegisterEventsForComponent(c);
                c.Destroy();
            }
            Components.Clear();
        }

        public override void Destroy()
        {
            if (Destroyed)
                return;
            Destroyed = true;
            OnDestroy();
            foreach (Entity child in Children.Values)
                child.Destroy();
            Updated = false;
            UpdateBegan = false;
            UpdateEnded = false;
            if (Parent != null)
            {
                Parent.children.Remove(this.ID);
                Parent = null;
            }
            base.Destroy();
        }

        public void AddTag(string tag)
        {
            tags.Add(tag);
        }

        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
        }

        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }

        public void RegisterEvent<T>(string name) where T : class
        {
            Type delegateType = typeof(T);
            if (delegateType.IsSubclassOf(typeof(Delegate)))
            {
                if (!registeredEvents.ContainsKey(name))
                {
                    MethodInfo signature = delegateType.GetMethod("Invoke");
                    registeredEvents.Add(name, new EventHolder() { DelegateType = delegateType, Signature = signature });
                }
            }
            else
                throw new NotSupportedException("T should be delegate");
            RegisterEventsForComponents();
        }

        public void InvokeEvents(string name, params object[] args)
        {
            foreach (var ev in componentEvents)
            {
                try
                {
                    if (ev.Value.ContainsKey(name))
                        ev.Value[name].Method.Invoke(ev.Key, args);
                }
                catch (TargetInvocationException e)
                {
                    ExceptionHelper.Throw(this, e.InnerException);
                }
            }
        }

        public void InvokeEvent(BaseComponent component, string name, params object[] args)
        {
            try
            {
                if (componentEvents.ContainsKey(component))
                {
                    var ev = componentEvents[component];
                    if (ev.ContainsKey(name))
                        ev[name].Method.Invoke(component, args);
                }
            }
            catch (TargetInvocationException e)
            {
                ExceptionHelper.Throw(this, e.InnerException);
            }
        }

        #endregion

        #region On...
        public virtual void OnInitialize(string id, Scene owner)
        {
            RegisterEvent<Action>("Begin");
            RegisterEvent<Action>("Enabled");
            RegisterEvent<Action>("Disabled");
            RegisterEvent<Action>("Finished");
            RegisterEvent<Action<BaseComponent>>("ComponentAdded");
            //RegisterEvent<Action<SpriteRenderer>>("EditorRender");

            foreach (BaseComponent c in Components)
            {
                InvokeEvent(c, "Begin");
                InvokeEvent(c, "Enabled");
                InvokeEvents("ComponentAdded", c);
            }
        }

        public void OnDestroy()
        {
            InvokeEvents("Disabled");
            InvokeEvents("Finished");

            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].OnFinished();
            }
            foreach (BaseComponent c in Components)
            {
                c.Removed();
                c.Destroy();
            }

            Components.Clear();
        }

        public void OnBeginUpdate()
        {
            if (!Destroyed && (PauseStateFlags.Update & this.PauseState) != PauseStateFlags.Update)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    try
                    {
                        Components[i].OnBeginUpdate();
                    }
                    catch (Exception e)
                    {
                        ExceptionHelper.Throw(this, e);
                    }
                }
                UpdateBegan = true;
                UpdateEnded = false;
                foreach (Entity child in Children.Values)
                {
                    if (child.Destroyed)
                        continue;
                    child.OnBeginUpdate();
                }
            }
            else
            {
                UpdateBegan = false;
                Updated = false;
                UpdateEnded = false;
            }
        }

        public void OnEndUpdate()
        {
            if (!Destroyed && (PauseStateFlags.Update & this.PauseState) != PauseStateFlags.Update)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    try
                    {
                        Components[i].OnEndUpdate();
                    }
                    catch (Exception e)
                    {
                        ExceptionHelper.Throw(this, e);
                    }
                }
                UpdateEnded = true;
                Updated = false;
                foreach (Entity child in Children.Values)
                {
                    if (child.Destroyed)
                        continue;
                    child.OnEndUpdate();
                }
            }
            else
            {
                UpdateBegan = false;
                Updated = false;
                UpdateEnded = false;
            }
        }

        public void OnUpdate()
        {
            if (!Destroyed && (PauseStateFlags.Update & this.PauseState) != PauseStateFlags.Update)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    try
                    {
                        Components[i].OnUpdate();
                        if (Destroyed)
                            return;
                    }
                    catch (Exception e)
                    {
                        ExceptionHelper.Throw(this, e);
                    }
                }
                Updated = true;
                UpdateBegan = false;
                if (!Destroyed)
                {
                    foreach (Entity child in Children.Values)
                    {
                        if (child.Destroyed)
                            continue;
                        child.OnUpdate();
                    }
                }
            }
            else
            {
                UpdateBegan = false;
                Updated = false;
                UpdateEnded = false;
            }
        }

        public void OnFixedUpdate()
        {
            if (!Destroyed && (PauseStateFlags.Update & this.PauseState) != PauseStateFlags.Update)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    try
                    {
                        Components[i].OnFixedUpdate();
                    }
                    catch (Exception e)
                    {
                        ExceptionHelper.Throw(this, e);
                    }
                }
                foreach (Entity child in Children.Values)
                {
                    if (child.Destroyed)
                        continue;
                    child.OnFixedUpdate();
                }
            }
        }

        public void OnRender()
        {
            if (!Destroyed && (PauseStateFlags.Render & this.PauseState) != PauseStateFlags.Render)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    try
                    {
                        Components[i].OnRender();
                    }
                    catch (Exception e)
                    {
                        ExceptionHelper.Throw(this, e);
                    }
                }
                foreach (Entity child in Children.Values)
                {
                    if (child.Destroyed)
                        continue;
                    child.OnRender();
                }
            }
        }
        #endregion

        #region private

        void RegisterEventsForComponents()
        {
            foreach (BaseComponent component in Components)
                RegisterEventsForComponent(component);
        }

        void RegisterEventsForComponent(BaseComponent component)
        {
            Type t = component.GetType();
            foreach (KeyValuePair<string, EventHolder> ev in registeredEvents)
            {
                MethodInfo[] methods = GetMethodInfo(t, ev.Key);

                for (int i = 0; i < methods.Length; i++)
                {
                    object del = Delegate.CreateDelegate(ev.Value.DelegateType, component, methods[i], false);
                    if (del != null)
                    {
                        if (!componentEvents.ContainsKey(component))
                            componentEvents.Add(component, new Dictionary<string, Delegate>());
                        else
                        {
                            if (componentEvents[component].ContainsKey(ev.Key))
                                break;
                        }
                        componentEvents[component].Add(ev.Key, del as Delegate);
                        break;
                    }
                }
            }
        }

        void UnRegisterEventsForComponent(BaseComponent component)
        {
            componentEvents.Remove(component);
        }

        MethodInfo[] GetMethodInfo(Type t, string methodName)
        {

            MethodInfo[] method = (from m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                                   where m.Name == methodName
                                   select m).ToArray();

            return method;
        }
        #endregion
    }
}
