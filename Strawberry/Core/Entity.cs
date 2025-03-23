using Strawberry.Serialization;
using System.Reflection;

namespace Strawberry.Core
{
    public class Entity : ReferenceObject
    {
        Dictionary<BaseComponent, ComponentMethods> componentMethods = new Dictionary<BaseComponent, ComponentMethods>();

        protected ComponentCollection Components;

        public List<BaseComponent> AllComponents
        {
            get { return Components; }
        }

        Dictionary<string, EventHolder> registeredEvents = new Dictionary<string, EventHolder>();

        Dictionary<BaseComponent, Dictionary<string, Delegate>> componentEvents =
            new Dictionary<BaseComponent, Dictionary<string, Delegate>>();

        public Scene Scene { get; private set; }

        public string ID { get; internal set; }

        public bool Destroyed { get; private set; }

        public string Tag
        {
            set
            {
                string[] temp = value.Split(',');
                tags.AddRange(temp);
            }
            get
            {
                if (tags.Count > 0)
                    return tags[0];
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

        List<string> tags = new List<string>();

        public void Initialize(string id, Scene owner)
        {
            this.Components = new ComponentCollection();
            this.Scene = owner;
            this.ID = id;
            Destroyed = false;
            owner.AddEntity(this);
            this.PauseState = PauseStateFlags.None;
            this.Parent = null;
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

        public void Initialize(string id, Entity parent)
        {
            this.Components = new ComponentCollection();
            this.Scene = parent.Scene;
            this.ID = id;
            Destroyed = false;
            parent.children.Add(this.ID, this);
            Scene.AddEntity(this);

            this.PauseState = PauseStateFlags.None;
            this.parent = parent;
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
            componentMethods.Add(component, new ComponentMethods());
            component.Owner = this;
            component.Initialize(this);
            FindMethods(component);
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
            componentMethods.Remove(c);
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
            componentMethods.Remove(c);
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
            componentMethods.Clear();
        }

        public override void Destroy()
        {
            Destroyed = true;
            OnDestroy();
            foreach (Entity child in Children.Values)
                child.Destroy();
            Updated = false;
            UpdateBegan = false;
            UpdateEnded = false;
            Scene.DestroyEntity(this);
            Scene.DestroyCount++;
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
                if (componentMethods.ContainsKey(Components[i]))
                {
                    if (componentMethods[Components[i]].Finish != null)
                        componentMethods[Components[i]].Finish.Invoke();
                }
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
                    if (componentMethods.ContainsKey(Components[i]))
                    {
                        try
                        {
                            if (componentMethods[Components[i]].BeginUpdate != null)
                                componentMethods[Components[i]].BeginUpdate.Invoke();
                        }
                        catch (Exception e)
                        {
                            ExceptionHelper.Throw(this, e);
                        }
                    }
                }
                UpdateBegan = true;
                UpdateEnded = false;
                foreach (Entity child in Children.Values)
                    child.OnBeginUpdate();
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
                    if (componentMethods.ContainsKey(Components[i]))
                    {
                        try
                        {
                            if (componentMethods[Components[i]].EndUpdate != null)
                                componentMethods[Components[i]].EndUpdate.Invoke();
                        }
                        catch (Exception e)
                        {
                            ExceptionHelper.Throw(this, e);
                        }
                    }
                }
                UpdateEnded = true;
                Updated = false;
                foreach (Entity child in Children.Values)
                    child.OnEndUpdate();
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
                    if (componentMethods.ContainsKey(Components[i]))
                    {
                        if (componentMethods[Components[i]].Update != null)
                            componentMethods[Components[i]].Update.Invoke();
                    }
                }
                Updated = true;
                UpdateBegan = false;
                foreach (Entity child in Children.Values)
                    child.OnUpdate();
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
                    if (componentMethods.ContainsKey(Components[i]))
                    {
                        try
                        {
                            if (componentMethods[Components[i]].FixedUpdate != null)
                                componentMethods[Components[i]].FixedUpdate.Invoke();
                        }
                        catch (Exception e)
                        {
                            ExceptionHelper.Throw(this, e);
                        }
                    }
                }
                foreach (Entity child in Children.Values)
                    child.OnFixedUpdate();
            }
        }

        public void OnRender()
        {
            if (!Destroyed && (PauseStateFlags.Render & this.PauseState) != PauseStateFlags.Render)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    if (componentMethods.ContainsKey(Components[i]))
                    {
                        try
                        {
                            if (componentMethods[Components[i]].Render != null)
                                componentMethods[Components[i]].Render.Invoke();
                        }
                        catch (Exception e)
                        {
                            ExceptionHelper.Throw(this, e);
                        }
                    }
                }
                foreach (Entity child in Children.Values)
                    child.OnRender();
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

        void FindMethods(BaseComponent component)
        {
            Type t = component.GetType();
            MethodInfo[] update = GetMethodInfo(t, "Update");
            for (int i = 0; i < update.Length; i++)
            {
                Action del = (Action)Delegate.CreateDelegate(typeof(Action), component, update[i], false);
                if (del != null)
                {
                    componentMethods[component].Update = del;
                    break;
                }
            }

            t = component.GetType();
            MethodInfo[] fupdate = GetMethodInfo(t, "FixedUpdate");
            for (int i = 0; i < fupdate.Length; i++)
            {
                Action del = (Action)Delegate.CreateDelegate(typeof(Action), component, fupdate[i], false);
                if (del != null)
                {
                    componentMethods[component].FixedUpdate = del;
                    break;
                }
            }

            t = component.GetType();
            MethodInfo[] beginUpdate = GetMethodInfo(t, "BeginUpdate");
            for (int i = 0; i < beginUpdate.Length; i++)
            {
                Action del = (Action)Delegate.CreateDelegate(typeof(Action), component, beginUpdate[i], false);
                if (del != null)
                {
                    componentMethods[component].BeginUpdate = del;
                    break;
                }
            }

            t = component.GetType();
            MethodInfo[] endUpdate = GetMethodInfo(t, "EndUpdate");
            for (int i = 0; i < endUpdate.Length; i++)
            {
                Action del = (Action)Delegate.CreateDelegate(typeof(Action), component, endUpdate[i], false);
                if (del != null)
                {
                    componentMethods[component].EndUpdate = del;
                    break;
                }
            }

            MethodInfo[] finish = GetMethodInfo(t, "Finish");
            for (int i = 0; i < finish.Length; i++)
            {
                Action del = (Action)Delegate.CreateDelegate(typeof(Action), component, finish[i], false);
                if (del != null)
                {
                    componentMethods[component].Finish = del;
                    break;
                }
            }

            MethodInfo[] render = GetMethodInfo(t, "Render");
            for (int i = 0; i < render.Length; i++)
            {
                Action del = (Action)Delegate.CreateDelegate
                    (typeof(Action), component, render[i], false);
                if (del != null)
                {
                    componentMethods[component].Render = del;
                    break;
                }
            }


            /*MethodInfo[] guiRender = GetMethodInfo(t, "GuiRender");
            for (int i = 0; i < guiRender.Length; i++)
            {
                Action<StrawBerry.Graphics.SpriteRenderer> del = (Action<StrawBerry.Graphics.SpriteRenderer>)Delegate.CreateDelegate
                    (typeof(Action<StrawBerry.Graphics.SpriteRenderer>), component, guiRender[i], false);
                if (del != null)
                {
                    componentMethods[component].GuiRender = del;
                    break;
                }
            }*/
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
