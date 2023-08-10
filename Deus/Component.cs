using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusEngine
{
    class Component
    {
        //reference to the game
        protected Game game;
        //reference to Parent Entity
        public Entity entity { get; set; }
        //reference to the transform
        public Transform2D transform;

        //overridable functions 
        public virtual void OnUpdate() { }
        public virtual void OnStart() { }

        public Component() =>
            //set the game reference 
            game = Game.Instance;

        //set the entity parent as reference 
        public void SetEntityParent(ref Entity entity)
        {
            this.entity = entity;
            transform = entity.transform;
        }

    }
}
