using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusEngine
{
    class ColliderManager
    {
        public static ColliderManager Instance;
        //hash map for the colliders
        public HashSet<Collider2D> colliders;

        public ColliderManager() 
        {
            if(Instance == null)
            {
                Instance = this;
                colliders = new HashSet<Collider2D>();
            }
            
        }

        //handles removing a collider
        public void RemoveCollider(Collider2D col)
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
        public void AddCollider(Entity entity)
        {
            //cache the collider
            Collider2D col = entity.GetComponent<Collider2D>();
            if (col != null)
            {
                //add the collider to hashset
                AddCollider(col);
            }
        }

        //handles adding a collider through collider
        public void AddCollider(Collider2D col)
        {
            //if the collider exists, return
            if (colliders.Contains(col))
                return;

            Game.Log($"{col} is added");

            //add to hashset
            colliders.Add(col);
        }



        //handles checking the collision in all the colliders
        public void CheckAllForCollision()
        {
            // Get the count of colliders
            //int colliderCount = colliders.Count;

            // Create a copy of the colliders using a HashSet
            HashSet<Collider2D> collidersCopy = new HashSet<Collider2D>(colliders);

            // Iterate over each collider in the collidersCopy HashSet
            foreach (Collider2D col in collidersCopy)
            {
                // Iterate over each collider again for collision checking
                foreach (Collider2D colx in collidersCopy)
                {
                    // Skip if it's the same collider
                    if (col == colx)
                        continue;

                    // Perform null checks
                    if (col == null || colx == null)
                        break;

                    // Check collision between the colliders
                    bool bProduct = col.IsColiding(colx);
                    col.bIsColiding = bProduct;
                }
            }
        }




        //clean up the hashset
        public void CleanUp()
        {
            colliders.RemoveWhere(col => col == null);
        }
    }
}
