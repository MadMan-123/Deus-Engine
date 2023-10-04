using SFML.Graphics;
using SFML.System;


namespace DeusEngine
{
    public struct RaycastHit
    {
        public Vector2f Point;  // The point of intersection
        public float Distance;  // The distance from the ray origin to the intersection point
        public Vector2f Normal; // The surface normal at the point of intersection (optional)
        public bool bIsHit;     // Was there a hit?    
        public Entity hitEntity; // The entity that was hit
                                 // Constructor
        public RaycastHit(Vector2f point, float distance, Vector2f normal, bool bHit, Entity hitEntity)
        {
            Point = point;
            Distance = distance;
            Normal = normal;
            bIsHit = bHit;
            this.hitEntity = hitEntity;
        }
    }

    public class RayCastCollider : CircleCollider2D
    {
        public RayCastCollider() 
        {
            collisionType = CollisionType.RayCast;
            entity = new Entity();
            entity.sTag = "RaycastCollision";
        }

        public override bool CheckCollision(Collider other)
        {
            bool bCollided = false;

            switch (other.collisionType)
            {
                case CollisionType.Circle:
                    bCollided = CheckCircleCollision(this, (CircleCollider2D)other);
                    break;

                case CollisionType.Box:
                    bCollided = CheckCircleBoxCollision(this, (Collider2D)other);
                    break;

                case CollisionType.RayCast:
                    bCollided = CheckCircleCollision(this, (RayCastCollider)other);
                    break;

                    // Add more collision checks for other collider types as needed
            }

            // Check if the collider should be avoided
            bool bShouldCollide = CheckIfColliderShouldBeAvoided(other);

            // If it shouldn't collide, remove it from collisions
            if (!bShouldCollide && collisions.Contains(other))
            {
                collisions.Remove(other);
            }

            return bCollided && bShouldCollide;
        }

    }
    public class Raycast
    {

        static int numPoints = 25;
        Vector2f pointToCheck;
        public List<string> sTagsToIgnore = new List<string>();

        public bool CastRay(Vector2f StartOrigin, Vector2f NormalizedDirection, float fLength, out RaycastHit? hit)
        {
            /*           float fInterval = fLength / numPoints;

                        // Initialize the hit info outside the loop
                        hit = null;
                        for (int i = 0; i < numPoints; i++)
                        {
                            // Calculate the new position
                            pointToCheck = StartOrigin + NormalizedDirection * fInterval * i;


                            // Set the position of the temporary collider at the ray point
                            RayCols[i].transform.position = pointToCheck;
                            RayCols[i].SetRadius(0.01f);
                            CircleShape circle = new CircleShape(1f);

                            circle.Position = RayCols[i].transform.position;
                            //draw the point
                            Application.Draw(circle);

                            // Check for collision at the ray point
                            if (RayCols[i].bIsColiding)
                            {
                                var colls = RayCols[i].collisions;

                                // Ensure there are colliders in the collection
                                if (colls.Count > 0)
                                {
                                    Entity hitEntity = colls[0].entity;

                                    // Store hit information for the intersection point
                                    hit = new RaycastHit(pointToCheck, fInterval * i, new Vector2f(0,0) , RayCols[i].bIsColiding, hitEntity);
                                    return true;
                                }
                            }
                        }

                        return hit != null; // Return true if a collision was detected*/
            

            return Application.Instance.colliderManager.RayCast(StartOrigin, NormalizedDirection, fLength,null ,out hit);
        }


        public bool CastRay(Vector2f StartOrigin, Vector2f NormalizedDirection, float fLength, out RaycastHit? hit, List<string> TagsToAvoid)
        {
            return Application.Instance.colliderManager.RayCast(StartOrigin, NormalizedDirection, fLength, TagsToAvoid, out hit);
        }

    }
}


