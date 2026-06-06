/*
 * Strawberry Game Engine
 * File: ActionComponent.cs
 * Author: Koosha Aabedini Nassab
 *
 * Component that supports registering actions for update, render, and fixed update callbacks.
 */

using Strawberry.Core;

namespace Strawberry.Components
{
    public class EventSubscription
    {
        Action callback;
        List<Action> list;

        internal EventSubscription(Action callback, List<Action> list)
        {
            this.callback = callback;
            this.list = list;
        }

        public void Cancel()
        {
            if (callback == null)
                return;
            list?.Remove(callback);
            callback = null;
            list = null;
        }

        public bool IsActive => callback != null;
    }

    public class ActionComponent : BaseComponent
    {
        List<Action> updateActions = new List<Action>();
        List<Action> renderActions = new List<Action>();
        List<Action> fixedUpdateActions = new List<Action>();

        public EventSubscription Update(Action action)
        {
            updateActions.Add(action);
            return new EventSubscription(action, updateActions);
        }

        public EventSubscription Render(Action action)
        {
            renderActions.Add(action);
            return new EventSubscription(action, renderActions);
        }

        public EventSubscription FixedUpdate(Action action)
        {
            fixedUpdateActions.Add(action);
            return new EventSubscription(action, fixedUpdateActions);
        }

        public void ClearUpdate() => updateActions.Clear();
        public void ClearRender() => renderActions.Clear();
        public void ClearFixedUpdate() => fixedUpdateActions.Clear();

        public void ClearAll()
        {
            updateActions.Clear();
            renderActions.Clear();
            fixedUpdateActions.Clear();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            for (int i = 0; i < updateActions.Count; i++)
                updateActions[i]();
        }

        public override void OnRender()
        {
            for (int i = 0; i < renderActions.Count; i++)
                renderActions[i]();
        }

        public override void OnFixedUpdate()
        {
            for (int i = 0; i < fixedUpdateActions.Count; i++)
                fixedUpdateActions[i]();
        }
    }
}
