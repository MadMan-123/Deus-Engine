using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusEngine
{
    class Entity
    {
        // List of components attached to the entity
        public List<Component> components = new List<Component>();

        // Transform component for position, size, and rotation
        public Transform2D transform;

        // Name of the entity
        public string Name = "Entity";

        // Reference to the entity itself
        private Entity entityRef;

        // List of tags associated with the entity
        public List<string> sTags = new List<string>();

        // Executes when the entity starts
        public virtual void OnStart() { }

        // Executes when the entity updates
        public virtual void OnUpdate() { }

        // Constructor
        public Entity()
        {
            entityRef = this;
            // Add the Transform2D component by default
            transform = AddComponent<Transform2D>();
            Name = ToString();
        }

        // Runs the OnStart method of the entity and all attached components
        public void RunStarts()
        {
            OnStart();
            foreach (Component component in components)
            {
                component.OnStart();
            }
        }

        // Runs the OnUpdate method of the entity and all attached components
        public void RunUpdates()
        {
            OnUpdate();
            foreach (Component component in components)
            {
                component.OnUpdate();
            }
        }

        // Adds a component of type T to the entity
        public T AddComponent<T>() where T : Component, new()
        {
            T NewComp = new T();
            NewComp.SetEntityParent(ref entityRef);
            components.Add(NewComp);

            return NewComp;
        }

        // Gets a component of type T from the entity
        public T GetComponent<T>() where T : Component, new()
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    return (T)components[i];
                }
            }

            return null;
        }

        // Checks if the entity has a specific tag
        public bool CompareTag(string sTag) => sTags.Any(s => s == sTag);

        // Checks if the entity has any of the specified tags
        public bool CompareTags(List<string> strings) => sTags.Any(x => strings.Any(y => EqualityComparer<string>.Default.Equals(x, y)));
    }
}
