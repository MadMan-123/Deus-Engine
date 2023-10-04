
using SFML.Graphics;
using SFML.System;

namespace DeusEngine
{
    public class ColliderManager
    {
        public static ColliderManager Instance;
        //hash map for the colliders
        public HashSet<Collider>? colliders;

        public ColliderManager() 
        {
            if(Instance == null)
            {
                Instance = this; 
                colliders = new HashSet<Collider>();
            }
            
        }

        //handles removing a collider
        public void RemoveCollider(Collider col)
        {
            //if the collider is in the hashset
            if (colliders.Contains(col))
            {
                //remove
                colliders.Remove(col);
                //clean the hashset
                colliders.TrimExcess();
            }
        }

        //handles adding a collider through entity
        public void AddCollider(Collider collider)
        {
            // Check if the collider is null or already in the set
            if (collider == null || colliders.Contains(collider))
                return;

            // Determine the type of the collider
            if (collider is Collider2D boxCollider)
            {
                colliders.Add(boxCollider);
                Application.Log("Box Collider Added");
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                colliders.Add(circleCollider);
                Application.Log("Circle Collider Added");
            }
            else
            {
                // Handle unsupported collider types or log an error
                Application.Log($"Unsupported collider type: {collider.collisionType}");
            }
        }

        public void AddCollider(Entity entity)
        {

            // Cache the collider
            Collider col = entity.GetComponent<Collider>();
            //if the collider exists, return
            if (colliders.Contains(col))
                return;
            if (col != null)
            {
                // Determine the type of the collider
                switch (col.collisionType)
                {
                    case Collider.CollisionType.Box:
                        // Attempt to get a Collider2D component from the entity
                        Collider2D boxCollider = entity.GetComponent<Collider2D>();

                        if (boxCollider != null)
                        {
                            // Add the box collider to the hashset
                            AddCollider(boxCollider);
                            Application.Log("Box Collider Added");
                        }
                        else
                        {
                            Application.Log("Box Collider not found on the entity.");
                        }
                        break;

                    case Collider.CollisionType.Circle:
                        // Attempt to get a CircleCollider2D component from the entity
                        CircleCollider2D circleCollider = entity.GetComponent<CircleCollider2D>();

                        if (circleCollider != null)
                        {
                            // Add the circle collider to the hashset
                            AddCollider(circleCollider);
                            Application.Log("Circle Collider Added");
                        }
                        else
                        {
                            Application.Log("Circle Collider not found on the entity.");
                        }
                        break;

                    default:
                        Application.Log($"Unsupported collider type: {col.collisionType}");
                        break;
                }
            }
        }

        //handles checking the collision in all the colliders
        public void CheckAllForCollision()
        {
            // Iterate over each collider in the colliders HashSet
            foreach (Collider col in colliders)
            {
                // Perform null checks
                if (col == null)
                    continue;

                // Reset the collision flag for this collider
                col.bIsColiding = false;

                // Iterate over each other collider for collision checking
                foreach (Collider colx in colliders)
                {
                    // Perform null checks
                    if (colx == null || col == colx)
                        continue;

                    bool bIsColliding = col.CheckCollision(colx);

                    if (!bIsColliding)
                        continue;

                    //is the collision a raycast
                    bool bIsColRaycast = col.entity.sTag == "Raycast";

                    //handle raycast logic
                    if(bIsColRaycast)
                    {
                        if (col.sTagsToAvoid.Contains(colx.entity.sTag))
                        {
                            col.bIsColiding=false;
                            continue;
                        }
                        
                    }
                    // Check collision between the colliders
                    else
                    {
                        //check if tags are to be ignored
                        if(col.sTagsToAvoid.Contains(colx.entity.sTag) || colx.sTagsToAvoid.Contains(col.entity.sTag) )
                        {
                            continue;
                        }
                        col.bIsColiding = true;
                        break; // Exit the inner loop once a collision is detected
                    }
                }
            }
        }
        public struct CircleCastHit
        {
            public Vector2f Point;      // The point of intersection
            public Vector2f Normal;     // The surface normal at the point of intersection
            public bool IsHit;          // Was there a hit?
            public Collider Collider;   // The collider that was hit (or null if not hit)

            public CircleCastHit(Vector2f point, Vector2f normal, bool isHit, Collider collider)
            {
                Point = point;
                Normal = normal;
                IsHit = isHit;
                Collider = collider;
            }
        }
        /*
        public bool RayCast(Vector2f origin, Vector2f direction, float maxDistance, out RaycastHit? hit)
        {
            hit = null;

            // Iterate over each collider in the colliders HashSet
            foreach (Collider col in colliders)
            {
                // Perform null checks
                if (col == null)
                    continue;

                // Reset the collision flag for this collider
                col.bIsColiding = false;

                // Calculate the vector from the origin to the collider's center
                Vector2f toCollider = col.transform.position - origin;

                // Calculate the dot product of the direction and the vector to the collider
                float dotProduct = DMath.DotProduct(toCollider, direction);

                // Check if the dot product is negative (collider is behind the ray)
                if (dotProduct < 0)
                    continue;

                // Calculate the projected point on the ray
                Vector2f projection = origin + direction * dotProduct;

                // Calculate the distance between the projected point and the collider's center
                float distance = DMath.Magnitude(projection - col.transform.position);

                // Check if the distance is within the max distance of the ray
                if (distance <= maxDistance)
                {
                    // Check if tags should be avoided
                    if (col.sTagsToAvoid.Contains("Raycast")) // Adjust this tag as needed
                    {
                        continue; // Avoid this collider
                    }

                    // Set the collision flag
                    col.bIsColiding = true;

                    // Create a RaycastHit with relevant information
                    hit = new RaycastHit(projection, distance, DMath.VectorZero, true, col.entity);
                    return true;
                }
            }

            return false; // No collision detected
        }
        */
        public bool RayCast(Vector2f origin, Vector2f direction, float maxDistance, List<string> sTagsToIgnore, out RaycastHit? hit)
        {
            hit = null;

            // Start raycasting from the origin
            Vector2f currentPosition = origin;

            for (float distance = 0; distance < maxDistance;)
            {
                // Circle cast at the current position
                if (CircleCast(currentPosition, 0.01f, direction, maxDistance - distance, sTagsToIgnore, out CircleCastHit circleHit))
                {
                    // Calculate the exact hit point on the ray
                    Vector2f hitPoint = currentPosition + direction * (float)Math.Sqrt(DMath.SquaredMagnitude(circleHit.Point - currentPosition));

                    hit = new RaycastHit(hitPoint, distance + DMath.Distance(hitPoint, origin), circleHit.Normal, true, circleHit.Collider.entity);
                    return true;
                }

                // Calculate the remaining distance to the max distance
                float remainingDistance = maxDistance - distance;

                // Adjust the step size based on the remaining distance
                float stepSize = Math.Min(remainingDistance, 1.0f);

                // Move to the next position using the adjusted step size
                currentPosition += direction * stepSize;

                // Increment the distance
                distance += stepSize;
            }

            return false;
        }

        public bool CircleCast(Vector2f origin, float radius, Vector2f direction, float maxDistance, List<string> sTagsToIgnore, out CircleCastHit hit)
        {
            hit = new CircleCastHit(DMath.VectorZero, DMath.VectorZero, false, null);

            // Calculate squared values once
            float maxSquaredDistance = maxDistance * maxDistance;
            float squaredRadius = radius * radius;

            foreach (Collider col in colliders)
            {
                // Perform null checks
                if (col == null)
                    continue;

                // Check if tags should be avoided
                if ( sTagsToIgnore != null && sTagsToIgnore.Contains(col.entity.sTag))
                    continue;

                // Calculate the vector from the origin to the collider's center
                Vector2f toCollider = col.transform.position - origin;

                // Calculate the dot product of the direction and the vector to the collider
                float dotProduct = DMath.DotProduct(toCollider, direction);

                // Check if the dot product is negative (collider is behind the ray or too far)
                if (dotProduct < 0 || dotProduct > maxDistance)
                    continue;

                // Calculate the projected point on the ray
                Vector2f projection = origin + direction * dotProduct;

                // Calculate the squared distance between the projection and the collider's center
                float squaredDistance = DMath.SquaredMagnitude(projection - col.transform.position);

                // Check if the squared distance is less than or equal to the squared max distance
                if (squaredDistance <= maxSquaredDistance)
                {
                    // Check if the collider intersects with the circle cast
                    if (col.IntersectsCircleCast(origin, radius, direction, maxDistance))
                    {
                        // Calculate the collision normal (from the collider center to the projection)
                        Vector2f normal = DMath.Normalize(projection - col.transform.position);

                        // Set the collision flag
                        col.bIsColiding = true;

                        // Create a CircleCastHit with relevant information
                        hit = new CircleCastHit(projection, normal, true, col);
                        return true;
                    }
                }
            }

            return false; // No collision detected
        }






        //clean up the hashset
        public void CleanUp()
        {
            colliders.RemoveWhere(col => col == null);
        }

        public static bool CircleIntersectsCircle(Vector2f circle1Center, float radius1, Vector2f circle2Center, float radius2)
        {
            // Calculate the vector between the centers of the two circles
            Vector2f centerVector = circle2Center - circle1Center;

            // Calculate the distance between the centers
            float distance = (float)Math.Sqrt(centerVector.X * centerVector.X + centerVector.Y * centerVector.Y);

            // Check if the distance is less than or equal to the sum of the radii
            return distance <= (radius1 + radius2);
        }
    }
}
