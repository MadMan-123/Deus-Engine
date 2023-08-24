using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        public HashSet<Collider2D> ColliderList;
        //the collider outline 
        RectangleShape outline = new RectangleShape();
        public Vector2f OriginOffset { get; set; }



        public Collider2D()
        {
            ColliderList = new HashSet<Collider2D>();
            //setting the collider bounds visual
            outline.OutlineThickness = 1;
            outline.OutlineColor = Color.White;
            outline.FillColor = new Color(0, 0, 0, 0);
            OriginOffset = new Vector2f(-0.5f,-0.5f); // Default origin in the middle

            ColliderManager.Instance.AddCollider(this);

        }

        public override void OnUpdate()
        {
            // Convert rotation to radians
            float rotationRadians = (float)(transform.fRotation * Math.PI / 180.0f);

            // Calculate the rotation matrix components
            float rotationCos = (float)Math.Cos(rotationRadians);
            float rotationSin = (float)Math.Sin(rotationRadians);

            float xOffset = transform.size.X * OriginOffset.X;
            float yOffset = transform.size.Y * OriginOffset.Y;
            Vector2f rotatedOffset = new Vector2f(
                xOffset * rotationCos - yOffset * rotationSin,
                xOffset * rotationSin + yOffset * rotationCos
            );

            // Calculate the rotated collider position
            Vector2f rotatedPosition = new Vector2f(
                transform.position.X + rotatedOffset.X,
                transform.position.Y + rotatedOffset.Y
            );

            // Update the collider's position and size
            Collider.Left = rotatedPosition.X;
            Collider.Top = rotatedPosition.Y;
            Collider.Width = transform.size.X;
            Collider.Height = transform.size.Y;

            if (bIsColiding)
            {
                outline.OutlineColor = Color.Green;
            }
            else if (!bIsColiding)
            {
                outline.OutlineColor = Color.Red;
            }

            //if the collider should draw its bounds
            if (bShouldDrawBounds)
            {
                // Set the position and scale of the outline
                outline.Position = rotatedPosition;
                outline.Size = transform.size;
                outline.Rotation = transform.fRotation; // Set the rotation of the outline

                // Draw the collider bounds
                Game.Instance.window.Draw(outline);
            }
        }



        public unsafe bool IsColiding(Collider2D other)
        {
            //check if the other variable exists
            if (other == null)
                return false;

            //check if the collisions in the collider are still in
            bool bTemp = Collider.Intersects(other.Collider);
            if (bTemp && !ColliderList.Any(c => c == other))
            {
                //log that entity was added
                //Game.Log($"{other}: Added");
                //add the collider
                //ColliderList.Add(other);
                //invoke on collide
                OnCollisionEvent?.Invoke(other);
                bIsColiding = true;


            }
            else if (!bTemp && ColliderList.Any(c => c == other))
            {
                //log that entity was removed
                Game.Log($"{other}: Removed");
                //remove the collider
                ColliderList.Remove(other);
                //invoke on collide
                OnExitCollisionEvent?.Invoke(other);
                bIsColiding = false;   
            }

            //return value
            return bTemp;

        }

    }
}
