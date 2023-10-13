
using Silk.NET.Windowing;

namespace DeusEngine
{
    public class Entity
    {
        // List of components attached to the entity
        public List<Component> components = new List<Component>();

        // Transform component for position, size, and rotation
        public Transform transform;

        // Name of the entity
        public string Name = "Entity";

        // Reference to the entity itself
        private Entity entityRef;

        // List of tags associated with the entity
        public string sTag = "";

        // Executes when the entity starts
        public virtual void OnLoad() { }

        // Executes when the entity updates
        public virtual void OnUpdate(double t) { }

        public virtual void OnStart() {}
        
        
        // Constructor
        public Entity()
        {
            entityRef = this;
            // Add the Transform2D component by default
            transform = AddComponent<Transform>();
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
        public void SubscribeComponents(IWindow window)
        {
            foreach (Component component in components)
            {
                window.Load += component.OnLoad;
                window.Update += component.OnUpdate;
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
        public bool CompareTag(string sNewTag) => sTag == sNewTag;

        // Checks if the entity has any of the specified tags
        //public bool CompareTags(List<string> strings) => sTags.Any(x => strings.Any(y => EqualityComparer<string>.Default.Equals(x, y)));

        //add a tag to the entity
        public void SetTag(string sNewTag)
        {
            sTag = sNewTag;
        }
    }
}
