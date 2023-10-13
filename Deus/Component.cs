
namespace DeusEngine
{
    public class Component
    { 
        //reference to Parent Entity
        public Entity entity { get; set; }
        //reference to the transform
        public Transform transform;

        //overridable functions 
        //on update
        public virtual void OnUpdate(double t) { }
        public virtual void OnLoad() { }
        public virtual void OnStart() { }
        public virtual void OnDestroy(){}


        //set the entity parent as reference 
        public void SetEntityParent(ref Entity entity)
        {
            this.entity = entity;
            transform = entity.transform;
        }

    }
}
