using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusEngine
{
    class Collider2D : Component
    {
        //should draw the colliders bounds
        public bool bShouldDrawBounds = false;
        //the actual collider
        public FloatRect Collider;
        //bool to tell if coliding
        public bool bIsColiding = false;

        //events to trigger when colliding
        public Action<Collider2D> OnCollisionEvent;
        public Action<Collider2D> OnExitCollisionEvent;

        //a collider manager 
        private ColliderManager collisions = new ColliderManager();
        //the collider outline 
        RectangleShape outline = new RectangleShape();


        public Collider2D()
        {

            //setting the collider bounds visual
            outline.OutlineThickness = 1;
            outline.OutlineColor = Color.Green;
            outline.FillColor = new Color(0, 0, 0, 0);
        }

        public override void OnUpdate()
        {
            //set all the corrisponding points to the same of the transform
            Collider.Left = transform.position.X;
            Collider.Top = transform.position.Y;
            Collider.Width = transform.size.X;
            Collider.Height = transform.size.Y;

            //if the collider should draw its bounds
            if (bShouldDrawBounds)
            {
                //set the position and scale to that of the transform
                outline.Position = transform.position;
                outline.Size = transform.size;

                //draw the collider bounds
                Game.Instance.window.Draw(outline);
            }

        }

        //checks for collision with other collider
        public unsafe bool IsColiding(Collider2D other)
        {
            //check if the other variable exists
            if (other == null)
                return false;

            //check if the collisions in the collider are still in
            bool bTemp = Collider.Intersects(other.Collider);
            if (bTemp && !collisions.colliders.Any(c => c == other))
            {
                //log that entity was added
                //Game.Log($"{collider}: Added");
                //add the collider
                collisions.AddCollider(other);
                //invoke on collide
                OnCollisionEvent?.Invoke(other);

            }
            else if (!bTemp && collisions.colliders.Any(c => c == other))
            {
                //log that entity was removed
                //Game.Log($"{collider}: Removed");
                //remove the collider
                collisions.RemoveCollider(other);
                //invoke on collide
                OnExitCollisionEvent?.Invoke(other);
            }

            //return value
            return bTemp;

        }

    }
}
