using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;


namespace Wolstencroft
{
    #region Maths
    //maths class for specific functions
    static class WMaths
    {
        //normalize a vector
        static public Vector2f Normalize(Vector2f input)
        {
            float fLength = MathF.Sqrt((input.X * input.X) + (input.Y * input.Y));

            return (input / fLength);

        }

        //float lerp 
        static public float Lerp(float a, float b, float T)
        {
            return (1 - T) * a + b * T;
        }

        //vector lerp
        static public Vector2f Lerp(Vector2f a, Vector2f b, float T)
        {
            return (1 - T) * a + b * T;
        }

        //convert a integer based vector to a float based vector
        static public Vector2i V2FtoV2I(Vector2f input) => new Vector2i((int)input.X, (int)input.Y);


        //Pythagoras c^2=a^2+b^2
        static public float Distance(Vector2f a, Vector2f b)
        {
            float A = b.X - a.X;
            float B = b.Y - a.Y;
            return MathF.Sqrt(A * A + B * B);
        }

    }
    #endregion
    #region Component
    //polymorphic class
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

    //to render on screen 
    class Renderable : Component
    {
        //the body to render
        public RectangleShape Body = new RectangleShape();
        public Texture texture;
        public Color FillColour = Color.White, OutlineColour;

        // Shader and fragment shader objects
        private Shader shader;

        public override void OnStart()
        {
            Body.FillColor = FillColour;
            Body.OutlineColor = OutlineColour;

            
        }

        // Set the shader for the renderable
        public void SetShader(Shader shader)
        {
            this.shader = shader;
        }



        public void SetFillColour(Color color)
        {
            Body.FillColor = color;
        }

        public void SetTexture(string sPathToTexture)
        {
            texture = new Texture(sPathToTexture);
            Body.Texture = texture;

        }
        //overriden function
        public override void OnUpdate()
        {
            //set the body to be the position of the parent entity 
            Body.Position = entity.transform.position;
            //set the rotation to the rotation of the parent rotation
            Body.Rotation = entity.transform.fRotation;
            //set the size to the size of the entity
            Body.Size = entity.transform.size;


            // Draw the body with the shader if it's set
            if (shader != null)
            {
                Game.Instance.window.Draw(Body, new RenderStates(shader));
            }
            else
            {
                Game.Instance.Draw(Body);
            }

        }
    }

    //handle the transform of an object
    class Transform2D : Component
    {
        //position
        public Vector2f position = new Vector2f(0, 0);
        //size
        public Vector2f size = new Vector2f(0, 0);
        //rotation
        public float fRotation = 0f;
    }

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
            outline.FillColor = new SFML.Graphics.Color(0, 0, 0, 0);
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

    class TextComponent : Component
    {
        string sText = "";
        Text text;

        public override void OnStart()
        {
            text = new Text();
            text.DisplayedString = sText;

        }

        public override void OnUpdate()
        {
            text.Position = transform.position;
            text.Rotation = transform.fRotation;
            Game.Instance.Draw(text);
        }

        public void SetText(string Text)
        {
            sText = Text;
        }

    }

    class ColliderManager
    {
        //hash map for the colliders
        public HashSet<Collider2D> colliders = new HashSet<Collider2D>();

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
                colliders.Add(col);
            }
        }

        //handles adding a collider through collider
        public void AddCollider(Collider2D col)
        {
            //if the collider exists, return
            if (colliders.Contains(col))
                return;

            //add to hashset
            colliders.Add(col);
        }



        //handles checking the collision in all the colliders
        public void CheckAllForCollision()
        {
            // Get the count of colliders
            int colliderCount = colliders.Count;

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

    #endregion
    #region Entity
    class Entity
    {
        // List of components attached to the entity
        public List<Component> components = new List<Component>();

        // Transform component for position, size, and rotation
        public Transform2D transform;

        // Name of the entity
        public string Name = "Entity";

        // Reference to the entity itself
        private Entity entityRef;

        // List of tags associated with the entity
        public List<string> sTags = new List<string>();

        // Executes when the entity starts
        public virtual void OnStart() { }

        // Executes when the entity updates
        public virtual void OnUpdate() { }

        // Constructor
        public Entity()
        {
            entityRef = this;
            // Add the Transform2D component by default
            transform = AddComponent<Transform2D>();
            Name = ToString();
        }

        // Runs the OnStart method of the entity and all attached components
        public void RunStarts()
        {
            OnStart();
            foreach (Component component in components)
            {
                component.OnStart();
            }
        }

        // Runs the OnUpdate method of the entity and all attached components
        public void RunUpdates()
        {
            OnUpdate();
            foreach (Component component in components)
            {
                component.OnUpdate();
            }
        }

        // Adds a component of type T to the entity
        public T AddComponent<T>() where T : Component, new()
        {
            T NewComp = new T();
            NewComp.SetEntityParent(ref entityRef);
            components.Add(NewComp);

            return NewComp;
        }

        // Gets a component of type T from the entity
        public T GetComponent<T>() where T : Component, new()
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    return (T)(components[i]);
                }
            }

            return null;
        }

        // Checks if the entity has a specific tag
        public bool CompareTag(string sTag) => sTags.Any(s => s == sTag);

        // Checks if the entity has any of the specified tags
        public bool CompareTags(List<string> strings) => sTags.Any(x => strings.Any(y => EqualityComparer<string>.Default.Equals(x, y)));
    }


    class SoundManager
    {
        public static SoundManager Instance { get; private set; }
        SoundBuffer soundBuffer;

        public SoundManager()
        {

            if (Instance == null)
                Instance = this;

        }
    }

    #endregion
    #region Game

    //main game logic
    class Game
    {
        // Singleton instance of the game
        public static Game Instance;

        // Render window for the game
        public RenderWindow window;

        // Width and height of the window
        public uint iWidth = 400, iHeight = 400;

        // Name of the window
        public string sName = "Window";

        // Clock to measure the runtime
        static Clock RunTimeClock = new Clock();

        // Entity manager to handle entities
        public EntityManager Entities;

        // Delta time between frames
        static public Time DeltaTime;

        // Frames per second
        static public int FPS;
        private Clock fpsClock;

        private int frameCount = 0;
        private Clock frameTimer = new Clock();

        // Font for text display
        static public Font EngineFont = new Font("..\\..\\..\\fonts/consola.ttf");

        static public Vector2f MousePos;

        protected static bool bShouldLog = true;
        Process currentProcess;
        long memoryUsageBytes = 0;
        double memoryUsageMB = 0;

        // Background music
        public Music BackGroundMusic;

        // Constructor
        public Game()
        {
            if (Instance == null)
                Instance = this;
            window = new RenderWindow(new VideoMode(iWidth, iHeight), sName);
            Entities = new EntityManager();
        }

        // Draw a drawable object on the window
        public void Draw(SFML.Graphics.Drawable drawable)
        {
            if (window != null && drawable != null)
                window.Draw(drawable);
        }

        // Start the game
        public void Start()
        {
            RunTimeClock.Restart();
            window.Closed += HandleClose;
            window.KeyPressed += HandleKeyPress;

            OnStart();

            HandleUpdate();
        }

        // Size of the text displayed
        static uint iSize = 15;

        Text FPSCounter = new Text("", EngineFont, iSize);
        Text EntityCount = new Text("", EngineFont, iSize);
        Text EntitiesToDestroyCount = new Text("", EngineFont, iSize);
        //Text MemoryCounter = new Text("", EngineFont, iSize);

        // Update loop for the game
        void HandleUpdate()
        {
            Clock clock = new Clock();
            fpsClock = new Clock();

            EntityCount.Position = new Vector2f(0, 20);
            EntitiesToDestroyCount.Position = new Vector2f(0, 40);
            //MemoryCounter.Position = new Vector2f(0, 60);

            if (BackGroundMusic != null)
            {
                BackGroundMusic.Play();
            }

            while (window.IsOpen)
            {
                MousePos = (Vector2f)Mouse.GetPosition(Game.Instance.window);
                currentProcess = Process.GetCurrentProcess();
                DeltaTime = clock.Restart();
                window.DispatchEvents();
                window.Clear();

                // Call game update
                OnUpdate();

                // Call all entity updates
                HandleEntityUpdates();

                Entities.CleanUp();

                FPSCounter.DisplayedString = FPS.ToString();
                EntityCount.DisplayedString = Entities.Entities.Count.ToString();
                EntitiesToDestroyCount.DisplayedString = Entities.EntitiesToBeDestroyed.Count.ToString();

                window.Draw(FPSCounter);
                window.Draw(EntityCount);
                window.Draw(EntitiesToDestroyCount);
                //window.Draw(MemoryCounter);

                window.Display();

                frameCount++;
                if (fpsClock.ElapsedTime.AsSeconds() >= 1.0f)
                {
                    FPS = frameCount;
                    frameCount = 0;
                    fpsClock.Restart();
                }

                /*memoryUsageBytes = currentProcess.WorkingSet64;
                memoryUsageMB = memoryUsageBytes / (1024.0 * 1024.0);*/

                //MemoryCounter.DisplayedString = Math.Round(memoryUsageMB, 2).ToString(CultureInfo.CurrentCulture);
            }
        }

        // Event handler for window close
        void HandleClose(object sender, EventArgs e)
        {
            Entities.CleanUp();
            window.Close();
            window.Closed -= HandleClose;
            window.KeyPressed -= HandleKeyPress;
        }

        // Event handler for key press
        void HandleKeyPress(object sender, SFML.Window.KeyEventArgs e)
        {
        }

        // Check if a specific key is pressed
        static public bool IsKeyPressed(SFML.Window.Keyboard.Key key)
        {
            return SFML.Window.Keyboard.IsKeyPressed(key);
        }

        // Log a message
        static public void Log<T>(T Data)
        {
            if (bShouldLog)
                Console.WriteLine($"[{RunTimeClock.ElapsedTime.AsSeconds()}][{Data}]");
        }

        // Get the current time
        static public Time GetTime()
        {
            return RunTimeClock.ElapsedTime;
        }

        // Instantiate an entity and add it to the entity manager
        public static Entity Instantiate(Entity entity)
        {
            Instance.Entities.AddEntity(entity);
            return entity;
        }

        // Destroy an entity
        public static void Destroy(Entity entity)
        {
            Instance.Entities.Destroy(entity);
        }

        // Handle entity updates
        static void HandleEntityUpdates()
        {
            Instance.Entities.HandleEntityUpdates();
        }

        // Game update method to be overridden in derived classes
        public virtual void OnUpdate()
        {
        }

        // Game start method to be overridden in derived classes
        public virtual void OnStart()
        {
        }
    }

    #endregion
}
