using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusEngine
{

    //handles multiple entitys
    class EntityManager
    {
        // List of active entities
        public List<Entity> Entities = new List<Entity>();

        // List of entities to be destroyed
        public List<Entity> EntitiesToBeDestroyed = new List<Entity>();

        // Time in seconds to clean up destroyed entities
        public float SecondsToCleanUp = 2.5f;

        // Flag indicating if the cleanup is allowed
        private bool CanClean = true;

        // Collider manager to handle collisions
        private ColliderManager colliderManager = new ColliderManager();

        // Cleans up the destroyed entities
        public async void CleanUp()
        {
            if (!CanClean || EntitiesToBeDestroyed.Count == 0)
                return;

            // Set the flag to false
            CanClean = false;

            Game.Log("Start Clean Memory");

            // Collect the memory
            GC.Collect();
            GC.SuppressFinalize(this);

            // Remove any null elements
            EntitiesToBeDestroyed.RemoveAll(entity => entity == null);

            await Task.Delay((int)(SecondsToCleanUp * 1000f));

            // Remove destroyed entities from the main list
            Entities.RemoveAll(entity => EntitiesToBeDestroyed.Contains(entity));

            // Clear the destroyed entities list
            EntitiesToBeDestroyed.Clear();

            // Clean up the collider manager
            colliderManager.CleanUp();

            CanClean = true;
        }

        // Handles updating all entities
        public void HandleEntityUpdates()
        {
            HandlePhysicsUpdates();

            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] != null)
                    Entities[i].RunUpdates();
            }
        }

        // Handles updating physics and collisions
        public void HandlePhysicsUpdates()
        {
            colliderManager.CheckAllForCollision();
        }

        // Adds an entity to the manager
        public Entity AddEntity(Entity entity)
        {
            Game.Log($"Added: {entity.Name}");
            entity.RunStarts();

            // Attempt to add a collider to the manager
            colliderManager.AddCollider(entity);

            Entities.Add(entity);

            return entity;
        }

        // Destroys an entity
        public void Destroy(Entity entity)
        {
            Game.Log($"Added: {entity.Name} To Be Destroyed");

            // Remove the entity from the list
            Entities.Remove(entity);

            // Clean up the list
            Entities.TrimExcess();

            // Remove the collider from the collider manager
            Collider2D cache = entity.GetComponent<Collider2D>();

            if (cache != null)
            {
                colliderManager.RemoveCollider(cache);
            }

            // Clean up the components list
            entity.components.TrimExcess();

            // Add the entity to the list of entities to be destroyed
            EntitiesToBeDestroyed.Add(entity);
        }
    }
}
