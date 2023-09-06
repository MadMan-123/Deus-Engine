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
   
    public class Collider : Component
    {
        protected List<float> Points = new List<float>();
        protected List<Collider> collisions = new List<Collider>();
        public CollisionType collisionType;
        public bool bIsColiding = false;
        public bool bShouldDrawBounds = false;
        public virtual bool CheckCollision(Collider other) { return false; }

        public enum CollisionType
        {
            Box,
            Circle
        }
    }

    public class Collider2D : Collider
    {
        public Vector2f OriginOffset { get; set; }
        RectangleShape outline = new RectangleShape();
        //4 vertices to hold the square collider
        /*
         * 1: left
         * 2: top 
         * 3: width
         * 4: height
         */
        public Collider2D()
        {
            Points.Add(new float());//1 : 0
            Points.Add(new float());//2 : 1
            Points.Add(new float());//3 : 2
            Points.Add(new float());//4 : 3
            collisionType = CollisionType.Box;

            //setting the collider bounds visual
            outline.OutlineThickness = 1;
            outline.OutlineColor = Color.White;
            outline.FillColor = new Color(0, 0, 0, 0);
            OriginOffset = new Vector2f(-0.5f, -0.5f); // Default origin in the middle

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
            Points[0] = rotatedPosition.X;
            Points[1] = rotatedPosition.Y;
            Points[2] = transform.size.X;
            Points[3] = transform.size.Y;

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
                Application.Instance.window.Draw(outline);
            }
        }
        public override bool CheckCollision(Collider other)
        {
            switch (other.collisionType)
            {
                case CollisionType.Circle:
                    // Check if any of the 4 points are within the distance of the only point in the other sphere collider
                    if (other is Collider2D sphereCollider2D)
                    {
                        Vector2f sphereCenter = new Vector2f(
                            sphereCollider2D.Points[0] + (sphereCollider2D.Points[2] / 2f),
                            sphereCollider2D.Points[1] + (sphereCollider2D.Points[3] / 2f)
                        );

                        Vector2f closestPoint = new Vector2f(
                            Math.Clamp(Points[0], sphereCenter.X - (sphereCollider2D.Points[2] / 2f), sphereCenter.X + (sphereCollider2D.Points[2] / 2f)),
                            Math.Clamp(Points[1], sphereCenter.Y - (sphereCollider2D.Points[3] / 2f), sphereCenter.Y + (sphereCollider2D.Points[3] / 2f))
                        );

                        float distanceSquared = (closestPoint.X - sphereCenter.X) * (closestPoint.X - sphereCenter.X) +
                                                (closestPoint.Y - sphereCenter.Y) * (closestPoint.Y - sphereCenter.Y);

                        return distanceSquared <= ((Collider2D)other).Points[2] * ((Collider2D)other).Points[2] / 4f;
                    }
                    break;

                case CollisionType.Box:
                    // Use AABB to check if this box collider overlaps with the other box collider
                    if (other is Collider2D boxCollider2D)
                    {
                        float left1 = Points[0];
                        float top1 = Points[1];
                        float right1 = Points[0] + Points[2];
                        float bottom1 = Points[1] + Points[3];

                        float left2 = boxCollider2D.Points[0];
                        float top2 = boxCollider2D.Points[1];
                        float right2 = boxCollider2D.Points[0] + boxCollider2D.Points[2];
                        float bottom2 = boxCollider2D.Points[1] + boxCollider2D.Points[3];

                        return !(right1 < left2 || left1 > right2 || bottom1 < top2 || top1 > bottom2);
                    }
                    break;
            }

            return false;
        }

    }


}
    }
    //old collider
    /*    class Collider2D : Component
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
                    Application.Instance.window.Draw(outline);
                }
            }



            public unsafe bool IsColiding(Collider2D other)
            {
                //check if the other variable exists
                if (other == null)
                    return false;

                //check if the collisions in the collider are still in
                bool bTemp = Collider.Intersects(other.Collider);
                if ( bTemp && !ColliderList.Any(c => c == other))
                {
                    //log that entity was added
                    //Game.Log($"{other}: Added");
                    //add the collider
                    ColliderList.Add(other);
                    //invoke on collide
                    OnCollisionEvent?.Invoke(other);

                }
                else if ( !bTemp && ColliderList.Any(c => c == other))
                {
                    //log that entity was removed
                    //Game.Log($"{other}: Removed");
                    //remove the collider
                    ColliderList.Remove(other);
                    //invoke on collide
                    OnExitCollisionEvent?.Invoke(other);

                }

                //return value
                return bTemp;

            }

        }*/

}