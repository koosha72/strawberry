using Strawberry.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Strawberry.Core
{
    /// <summary>
    /// Represents a game entity that can contain components, children, tags, and event handlers.
    /// </summary>
    public class Entity : ReferenceObject
    {
        /// <summary>
        /// Holds the collection of components attached to this entity.
        /// </summary>
        protected ComponentCollection Components;

        /// <summary>
        /// Gets all components attached to this entity.
        /// </summary>
        public List<BaseComponent> AllComponents
        {
            get { return Components; }
        }

        Dictionary<string, EventHolder> registeredEvents = new Dictionary<string, EventHolder>();

        Dictionary<BaseComponent, Dictionary<string, Delegate>> componentEvents =
            new Dictionary<BaseComponent, Dictionary<string, Delegate>>();

        /// <summary>
        /// Gets the scene that owns this entity.
        /// </summary>
        public Scene Scene { get; private set; }

        /// <summary>
        /// Gets or sets the identifier for this entity.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets a value indicating whether this entity has been destroyed.
        /// </summary>
        public bool Destroyed { get; private set; }

        /// <summary>
        /// Gets or sets the comma-separated tags assigned to this entity.
        /// Setting this property adds tags from the comma-delimited string.
        /// </summary>
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

        /// <summary>
        /// Gets a value indicating whether the entity has started its update phase.
        /// </summary>
        public bool UpdateBegan { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the entity has completed its update phase.
        /// </summary>
        public bool Updated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the entity has ended its update phase.
        /// </summary>
        public bool UpdateEnded { get; private set; }

        /// <summary>
        /// Gets or sets the pause state flags for this entity.
        /// </summary>
        public PauseStateFlags PauseState { get; set; }

        Entity parent;

        /// <summary>
        /// Gets or sets the parent entity of this entity.
        /// Setting the parent updates the child collection on the old and new parents.
        /// </summary>
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
            }
        }

        EntityCollection children = new EntityCollection();

        /// <summary>
        /// Gets the child entities owned by this entity.
        /// </summary>
        public EntityCollection Children { get { return children; } }

        HashSet<string> tags = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        public Entity()
        {
            Components = new ComponentCollection();
            PauseState = PauseStateFlags.None;
            Destroyed = false;
        }

        /// <summary>
        /// Initializes the entity with an identifier and owning scene.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="owner">The scene that owns the entity.</param>
        public void Initialize(string id, Scene owner)
        {
            Scene = owner;
            ID = id;
            owner.AddEntity(this);
            Parent = null;
        }

        /// <summary>
        /// Initializes the entity as a child of another entity.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="parent">The parent entity.</param>
        public void Initialize(string id, Entity parent)
        {
            Scene = parent.Scene;
            ID = id;
            parent.children.Add(ID, this);
            Scene.AddEntity(this);
            this.parent = parent;
        }

        /// <summary>
        /// Enables the entity and triggers its initialization logic.
        /// </summary>
        public void Enable()
        {
            OnInitialize(ID, Scene);
        }

        /// <summary>
        /// Creates a clone of this entity using a new identifier.
        /// </summary>
        /// <param name="newId">The identifier for the cloned entity.</param>
        /// <returns>The cloned entity.</returns>
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

        /// <summary>
        /// Determines whether this entity is a descendant of the specified parent entity.
        /// </summary>
        /// <param name="parent">The potential ancestor entity.</param>
        /// <returns><c>true</c> if this entity is a child or descendant of the parent; otherwise, <c>false</c>.</returns>
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
        /// <summary>
        /// Adds a component to the entity and optionally initializes it.
        /// </summary>
        /// <param name="component">The component instance to add.</param>
        /// <param name="init">If set to <c>true</c>, the component is initialized immediately.</param>
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

        /// <summary>
        /// Creates and adds a new component of type <typeparamref name="T"/> to the entity.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The created component instance.</returns>
        public T AddComponent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>() where T : BaseComponent, new()
        {
            T component = new T();
            return AddComponent(component);
        }

        /// <summary>
        /// Adds an existing component instance to the entity.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="component">The component instance to add.</param>
        /// <returns>The added component.</returns>
        public T AddComponent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(T component) where T : BaseComponent
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

        /// <summary>
        /// Adds a set of new components to the entity.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="components">The list of components to add.</param>
        public void AddComponents<T>(List<T> components) where T : BaseComponent, new()
        {
            foreach (T c in components)
            {
                AddComponent<T>();
            }
        }

        /// <summary>
        /// Gets the first component of type <typeparamref name="T"/> attached to the entity.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>The component instance or <c>null</c> if not found.</returns>
        public T GetComponent<T>() where T : BaseComponent
        {
            Type t = typeof(T);
            T c = (T)(from cmp in Components where cmp.GetType() == t select cmp).FirstOrDefault();

            return c;
        }

        /// <summary>
        /// Determines whether this entity contains a component of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns><c>true</c> if the component exists; otherwise, <c>false</c>.</returns>
        public bool HasComponent<T>() where T : BaseComponent
        {
            Type t = typeof(T);
            return (from cmp in Components where cmp.GetType() == t select cmp).Count() > 0;
        }

        /// <summary>
        /// Gets all components of type <typeparamref name="T"/> attached to this entity.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>An array of matching components.</returns>
        public T[] GetComponents<T>() where T : BaseComponent
        {
            Type t = typeof(T);
            T[] c = (from cmp in Components where cmp.GetType() == t select cmp as T).ToArray();

            return c;
        }

        /// <summary>
        /// Gets all components attached to this entity.
        /// </summary>
        /// <returns>An array of attached components.</returns>
        public BaseComponent[] GetComponents()
        {
            List<BaseComponent> result = new List<BaseComponent>();
            foreach (BaseComponent cmp in Components)
            {
                result.Add(cmp);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets the index of the specified component in the entity component list.
        /// </summary>
        /// <param name="component">The component to locate.</param>
        /// <returns>The zero-based index, or -1 if not found.</returns>
        public int GetComponentIndex(BaseComponent component)
        {
            return Components.IndexOf(component);
        }

        /// <summary>
        /// Sets the index of the specified component in the entity component list.
        /// </summary>
        /// <param name="component">The component to reposition.</param>
        /// <param name="index">The new index position.</param>
        public void SetComponentIndex(BaseComponent component, int index)
        {

        }

        /// <summary>
        /// Removes the first component of type <typeparamref name="T"/> from the entity.
        /// </summary>
        /// <typeparam name="T">The component type to remove.</typeparam>
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

        /// <summary>
        /// Removes the specified component instance from the entity.
        /// </summary>
        /// <param name="component">The component instance to remove.</param>
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

        /// <summary>
        /// Removes and destroys all components attached to the entity.
        /// </summary>
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

        /// <summary>
        /// Destroys the entity and all of its children.
        /// </summary>
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

        /// <summary>
        /// Adds a tag to this entity.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        public void AddTag(string tag)
        {
            tags.Add(tag);
        }

        /// <summary>
        /// Removes a tag from this entity.
        /// </summary>
        /// <param name="tag">The tag to remove.</param>
        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
        }

        /// <summary>
        /// Checks whether this entity contains the specified tag.
        /// </summary>
        /// <param name="tag">The tag to check.</param>
        /// <returns><c>true</c> if the entity has the tag; otherwise, <c>false</c>.</returns>
        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }

        /// <summary>
        /// Registers a named event type for entity components.
        /// </summary>
        /// <typeparam name="T">The delegate type for the event.</typeparam>
        /// <param name="name">The event name.</param>
        public void RegisterEvent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(string name) where T : class
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

        /// <summary>
        /// Invokes the named event on all registered component listeners.
        /// </summary>
        /// <param name="name">The event name.</param>
        /// <param name="args">The event arguments.</param>
        public void InvokeEvents(string name, params object[] args)
        {
            foreach (var ev in componentEvents)
            {
                if (ev.Value.ContainsKey(name))
                    ev.Value[name].Method.Invoke(ev.Key, args);

            }
        }

        /// <summary>
        /// Invokes a named event on the specified component.
        /// </summary>
        /// <param name="component">The component that should receive the event.</param>
        /// <param name="name">The event name.</param>
        /// <param name="args">The event arguments.</param>
        public void InvokeEvent(BaseComponent component, string name, params object[] args)
        {
            if (componentEvents.ContainsKey(component))
            {
                var ev = componentEvents[component];
                if (ev.ContainsKey(name))
                    ev[name].Method.Invoke(component, args);
            }
        }

        #endregion

        #region On...
        /// <summary>
        /// Initializes the entity event registrations and triggers component startup events.
        /// </summary>
        /// <param name="id">The entity identifier.</param>
        /// <param name="owner">The owning scene.</param>
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

        /// <summary>
        /// Handles entity cleanup by invoking disable and finish events on attached components.
        /// </summary>
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

        /// <summary>
        /// Performs the beginning of the update cycle for this entity and its children.
        /// </summary>
        public void OnBeginUpdate()
        {
            if (!Destroyed && (PauseStateFlags.Update & this.PauseState) != PauseStateFlags.Update)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].OnBeginUpdate();
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

        /// <summary>
        /// Performs the end of the update cycle for this entity and its children.
        /// </summary>
        public void OnEndUpdate()
        {
            if (!Destroyed && (PauseStateFlags.Update & this.PauseState) != PauseStateFlags.Update)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].OnEndUpdate();
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

        /// <summary>
        /// Updates this entity and its children during the update cycle.
        /// </summary>
        public void OnUpdate()
        {
            if (!Destroyed && (PauseStateFlags.Update & this.PauseState) != PauseStateFlags.Update)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].OnUpdate();
                    if (Destroyed)
                        return;
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

        /// <summary>
        /// Performs fixed-step updates for this entity and its children.
        /// </summary>
        public void OnFixedUpdate()
        {
            if (!Destroyed && (PauseStateFlags.Update & this.PauseState) != PauseStateFlags.Update)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].OnFixedUpdate();
                }
                foreach (Entity child in Children.Values)
                {
                    if (child.Destroyed)
                        continue;
                    child.OnFixedUpdate();
                }
            }
        }

        /// <summary>
        /// Renders this entity and its children.
        /// </summary>
        public void OnRender()
        {
            if (!Destroyed && (PauseStateFlags.Render & this.PauseState) != PauseStateFlags.Render)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    Components[i].OnRender();
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

        MethodInfo[] GetMethodInfo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type t, string methodName)
        {

            MethodInfo[] method = (from m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                                   where m.Name == methodName
                                   select m).ToArray();

            return method;
        }
        #endregion
    }
}
