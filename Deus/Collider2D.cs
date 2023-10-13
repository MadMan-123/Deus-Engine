using System.Numerics;
using Silk.NET;
using Silk.NET.Maths;


namespace DeusEngine
{

    public class Collider : Component
    {
        public List<string> sTagsToAvoid = new List<string>();
        protected List<Vector2D<float>> Points = new List<Vector2D<float>>();
        public List<Collider> collisions = new List<Collider>();
        public CollisionType collisionType;
        public bool bIsColiding = false;
        public bool bShouldDrawBounds = false;
        public virtual bool CheckCollision(Collider other) { return false; }
        bool bIsTrigger = false;
        

        public enum CollisionType
        {
            Box,
            Circle,
            RayCast
        }

        public Collider()
        {
            ColliderManager.Instance.AddCollider(this);
        }

        public bool CheckIfColliderShouldBeAvoided(Collider other)
        {
            foreach (string tag in sTagsToAvoid)
            {
                if (other.entity.sTag == tag)
                {
                    return true;
                }
            }
            return false;
        }
        

   

        public override void OnUpdate(double t)
        {
        
        }
    }

    public class Collider2D : Collider
    {
        public Vector2D<float> OriginOffset { get; set; }
        //RectangleShape outline = new RectangleShape();
        //4 vertices to hold the square collider
        /*
         * 1: left
         * 2: top 
         * 3: width
         * 4: height
         */
        public Collider2D()
        {
            collisionType = CollisionType.Box;

            Points.Add(new Vector2D<float>());//1 : 0
            Points.Add(new Vector2D<float>());//2 : 1
            Points.Add(new Vector2D<float>());//3 : 2
            Points.Add(new Vector2D<float>());//4 : 3

            OriginOffset = new Vector2D<float>(-0.5f, -0.5f); // Default origin in the middle
            /*
            //setting the collider bounds visual
            outline.OutlineThickness = 1;
            outline.OutlineColor = Color.Green;
            outline.FillColor = new Color(0, 0, 0, 0);
            */

        }


        public override void OnUpdate(double t)
        {
            /*// Convert rotation to radians
            float rotationRadians = (float)(transform.fRotation * Math.PI / 180.0f);

            // Calculate the rotation matrix components
            float rotationCos = (float)Math.Cos(rotationRadians);
            float rotationSin = (float)Math.Sin(rotationRadians);

            float xOffset = transform.size.X * OriginOffset.X;
            float yOffset = transform.size.Y * OriginOffset.Y;
            Vector2D<float> rotatedOffset = new Vector2D<float>(
                xOffset * rotationCos - yOffset * rotationSin,
                xOffset * rotationSin + yOffset * rotationCos
            );

            // Calculate the rotated collider position
            Vector2D<float> rotatedPosition = new Vector2D<float>(
                transform.position.X + rotatedOffset.X,
                transform.position.Y + rotatedOffset.Y
            );

            // Update the collider's position and size
            Points[0] = rotatedPosition;
            Points[1] = rotatedPosition + new Vector2D<float>(0, transform.size.Y);
            Points[2] = rotatedPosition + new Vector2D<float>(transform.size.X, transform.size.Y);
            Points[3] = rotatedPosition + new Vector2D<float>(transform.size.X, 0);*/

        }
    
        public override bool CheckCollision(Collider other)
        {
            /*switch (other.collisionType)
            {
                case CollisionType.Circle:
                    // Check if any of the 4 points are within the distance of the only point in the other sphere collider
                    return CheckCircleBoxCollision((CircleCollider2D)(other), this);

                case CollisionType.Box:
                    // Use AABB to check if this box collider overlaps with the other box collider
                    return CheckBoxCollision((Collider2D)other,this);
            }*/

            return false;
        }

        

    }


    public class CircleCollider2D : Collider
    {
        public float Radius { get; set; }

        public CircleCollider2D()
        {
            collisionType = CollisionType.Circle;
        }
       
        public void SetRadius(float radius)
        {
            Radius = radius;
        }

        public override bool CheckCollision(Collider other)
        {
            bool bCollided = false;

            /*switch (other.collisionType)
            {
                case CollisionType.Circle:
                    bCollided = CheckCircleCollision(this, (CircleCollider2D)other);
                    break;

                case CollisionType.Box:
                    bCollided = CheckCircleBoxCollision(this, (Collider2D)other);
                    break;

                case CollisionType.RayCast:
                    return CheckCircleCollision(this, (RayCastCollider)(other));

                    // Add more collision checks for other collider types as needed
            }*/

            

            if (bCollided && !collisions.Contains(other))
            {
                // Add the other collider if it collided and is not already in the collisions list
                collisions.Add(other);
            }
            else if (!bCollided && collisions.Contains(other))
            {
                // Remove the other collider if it didn't collide but is in the collisions list
                collisions.Remove(other);
            }

            return bCollided;
        }

        /*public override void OnUpdate()
        {
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
                outline.Position = transform.position;
                outline.Radius = Radius;
                outline.Rotation = transform.fRotation; // Set the rotation of the outline

                // Draw the collider bounds
                Application.Draw(outline);
            }
        }*/


        


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
            public Vector2D<float> OriginOffset { get; set; }



            public Collider2D()
            {
                ColliderList = new HashSet<Collider2D>();
                //setting the collider bounds visual
                outline.OutlineThickness = 1;
                outline.OutlineColor = Color.White;
                outline.FillColor = new Color(0, 0, 0, 0);
                OriginOffset = new Vector2D<float>(-0.5f,-0.5f); // Default origin in the middle

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
                Vector2D<float> rotatedOffset = new Vector2D<float>(
                    xOffset * rotationCos - yOffset * rotationSin,
                    xOffset * rotationSin + yOffset * rotationCos
                );

                // Calculate the rotated collider position
                Vector2D<float> rotatedPosition = new Vector2D<float>(
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

