using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using SFML.Window;
using static System.Reflection.Metadata.BlobBuilder;
using System.Diagnostics;
using System.Globalization;

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

        static public Vector2i V2FtoV2I(Vector2f input) => new Vector2i((int)input.X, (int)input.Y);

        static public float Distance(Vector2f a, Vector2f b)
        {
            float A = ( b.X - a.X);
            float B = ( b.Y - a.Y);
            return MathF.Sqrt(MathF.Pow(A, 2) + MathF.Pow(B, 2));
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

        public Component()
        {
            game = Game.Instance;
        }

        public void SetEntityParent(ref Entity entity)
        {
            this.entity = entity;
            transform = entity.transform;
        }

    }

    //to render on screen 
    class Renderable : Component
    {
        public RectangleShape Body = new RectangleShape();


        public Renderable()
        {
        }

        public override void OnUpdate()
        {
            Body.Position = entity.transform.position;
            Body.Rotation = entity.transform.fRotation;
            Body.Size = entity.transform.size;
            Game.Instance.Draw(Body);
        }
    }

    //handle the transform of an object
    class Transform2D : Component
    {
        public Vector2f position = new Vector2f(0, 0);
        public Vector2f size = new Vector2f(0, 0);
        public float fRotation = 0f;
    }

    class Collider2D : Component
    {
        public bool bShouldDrawBounds = false;
        public FloatRect Collider;
        public bool bIsColiding = false;

        public Action<Collider2D> OnCollisionEvent;
        public Action<Collider2D> OnExitCollisionEvent;

        private ColliderManager collisions = new ColliderManager();
        RectangleShape outline = new RectangleShape();

        public Collider2D()
        {
            outline.OutlineThickness = 1;
            outline.OutlineColor = Color.Green;
        }

        public override void OnUpdate()
        {
            Collider.Left = transform.position.X;
            Collider.Top = transform.position.Y;
            Collider.Width = transform.size.X;
            Collider.Height = transform.size.Y;
            

            if(bShouldDrawBounds)
            {
                outline.Position = transform.position;
                outline.Size = transform.size;
                Game.Instance.window.Draw(outline);
            }

        }

        public unsafe bool IsColiding(Collider2D other)
        {
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

            return bTemp;

        }

    }

    class ColliderManager : Component
    {
        public List<Collider2D> colliders = new List<Collider2D>();
        public int iColCheckPerFrame = 0;
  

        public void RemoveCollider(Collider2D col)
        {
            if (colliders.Contains(col))
                colliders.Remove(col);
        }

        public void AddCollider(Entity entity)
        {
            Collider2D col = entity.GetComponent<Collider2D>();
            if (col != null)
            {
                colliders.Add(col);

            }
        }

        public void AddCollider(Collider2D col)
        {
            if (colliders.Contains(col))
                return;

            colliders.Add(col);
        }


        public void CheckAllForCollision()
        {
            int colliderCount = colliders.Count;

            for (int i = 0; i < colliderCount; i++)
            {
                Collider2D col = colliders[i];
                for (int j = 0 ; j < colliderCount; j++)
                {
                    if (i == j)
                        continue;

                    Collider2D colx = colliders[j];

                    bool bProduct = col.IsColiding(colx);
                    col.bIsColiding = bProduct;
                    
                }
            }

    
            

        }

        public unsafe bool* CheckAndReturnAllForCollision()
        {
            int colliderCount = colliders.Count;

            //if there are no colliders return
            if (colliderCount == 0)
                return null;

            bool* bIsCollidingArray = stackalloc bool[colliderCount];


            for (int i = 0; i < colliderCount; i++)
            {
                Collider2D col = colliders[i];

                for (int j = i + 1; j < colliderCount; j++)
                {
                    Collider2D colx = colliders[j];

                    if (WMaths.Distance(colx.transform.position, col.transform.position) > 2)
                        continue;
                    bool bProduct = col.IsColiding(colx);
                    bIsCollidingArray[i] = (bProduct);
                }
            }



            return bIsCollidingArray;
        }

    }
    class EntityManager : Component
    {
        public List<Entity> Entitys = new List<Entity>();
        public List<Entity> EntitysToBeDestroyed = new List<Entity>();
        public float fSeccondsToCleanUp = 2.5f;
        bool bCanClean = true;
        ColliderManager colliderManager = new ColliderManager();


        public override void OnStart()
        {
        }



        public async void CleanUp()
        {
            if (!bCanClean || EntitysToBeDestroyed.Count == 0)
                return;


            //set the bool false 
            bCanClean = false;

            Game.Log("Start Clean Memory");


            //for each entity remove it from the list
            for (int i = 0; i < EntitysToBeDestroyed.Count; i++)
            {
                EntitysToBeDestroyed.RemoveAt(i);
            }


            //collect the memory
            GC.Collect();
            GC.SuppressFinalize(this);

            //remove any null elements
            EntitysToBeDestroyed.TrimExcess();

            await Task.Delay((int)(fSeccondsToCleanUp * 1000f));

            bCanClean = true;


        }

        public void HandleEntityUpdates()
        {
            HandlePhysicsUpdates();

            for (int i = 0; i < Entitys.Count; i++)
            {
                if (Entitys[i] != null)
                    Entitys[i].RunUpdates();
            }

        }

        public void HandlePhysicsUpdates()
        {
            colliderManager.CheckAllForCollision();

           
            
        }

        public Entity AddEntity(Entity entity)
        {
            Game.Log($"Added: {(entity.Name)}");
            entity.RunStarts();

            //attempt to add a collider to the manager
            colliderManager.AddCollider(entity);

            Entitys.Add(entity);

            return entity;
        }

        public void Destroy(Entity entity)
        {
            Game.Log($"Added: {entity.Name} To Be Destroyed");
            //remove entity from list
            Entitys.Remove(entity);
            //clean up list
            Entitys.TrimExcess();

            //add entity to list of entitys to be destroyed but disable it from view
            EntitysToBeDestroyed.Add(entity);
        }


    }
    #endregion
    #region Entity
    class Entity
    {
        public List<Component> components = new List<Component>();
        public Transform2D transform;
        public string Name = "Entity";
        private Entity entityRef;
        public List<string> sTags = new List<string>();
       // public List<String> sIgnoreTags = new List<String>();

        public virtual void OnStart() { }
        public virtual void OnUpdate() { }


        public Entity()
        {
            entityRef = this;
            //add transform2d by default
            transform = AddComponent<Transform2D>();
            if (this != null)
                Name = ToString();
        }

        //run all component starts
        public void RunStarts()
        {
            OnStart();
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnStart();
            }
        }
        public void RunUpdates()
        {
            OnUpdate();
            //runs all component updates 
            foreach (var Comp in components)
            {
                Comp.OnUpdate();
            }
        }

        //add a component to the list
        public T AddComponent<T>() where T : Component, new()
        {
            T NewComp = new T();
            NewComp.SetEntityParent(ref entityRef);
            components.Add(NewComp);

            return NewComp;
        }

        //get a component from the list
        public T GetComponent<T>() where T : Component, new()
        {
            T NewComp = new T();

            for (int i = 0; i < components.Count; i++)
            {
                if (NewComp.GetType() == components[i].GetType())
                {
                    return (T)(components[i]);
                }
            }

            return null;


        }

       public bool CompareTag(string sTag) => sTags.Any(s => s == sTag);

       public bool CompareTags(List<string> strings) => sTags.Any(x => strings.Any(y => EqualityComparer<string>.Default.Equals(x, y)));

    };

    class EntityManagerOBJ : Entity
    {
        public EntityManager entityManager;



        public EntityManagerOBJ()
        {
            entityManager = AddComponent<EntityManager>();
        }
    }


    #endregion
    #region Game

    //main game logic
    class Game
    {
        public static Game Instance = null;

        public RenderWindow window;

        public uint iWidth = 400, iHeight = 400;

        public string sName = "Window";

        static Clock RunTimeClock = new Clock();

        public EntityManagerOBJ Entities;

        static public Time DeltaTime;

        static public int FPS;
        private Clock fpsClock;

        private int frameCount = 0;
        private Clock frameTimer = new Clock();

        static public Font EngineFont = new Font("F:\\Dev\\WolstencroftEngine\\consolas\\consola.ttf");

        protected static bool bShouldLog = true;
        Process currentProcess;
        long memoryUsageBytes = 0;
        double memoryUsageMB = 0;


        public Game()
        {
            if (Instance == null)
                Instance = this;
            window = new RenderWindow(new VideoMode(iWidth, iHeight), sName);
            Entities = new EntityManagerOBJ();
        }

        public void Draw(SFML.Graphics.Drawable drawable)
        {
            if (window != null && drawable != null)
                window.Draw(drawable);
        }


        public void Start()
        {
            RunTimeClock.Restart();
            window.Closed += HandleClose;
            window.KeyPressed += HandleKeyPress;

            OnStart();

            HandleUpdate();
        }

        static uint iSize = 15;

        Text FPSCounter = new Text("", EngineFont, iSize);
        Text EntityCount = new Text("", EngineFont, iSize);
        Text EntitiesToDestroyCount = new Text("", EngineFont, iSize);
        Text MemoryCounter = new Text("", EngineFont, iSize);
        void HandleUpdate()
        {
            Clock clock = new Clock();
            fpsClock = new Clock();

            EntityCount.Position = new Vector2f(0, 20);
            EntitiesToDestroyCount.Position = new Vector2f(0, 40);
            MemoryCounter.Position = new Vector2f(0, 60);
            while (window.IsOpen)
            {
                currentProcess = Process.GetCurrentProcess();
                DeltaTime = clock.Restart();
                window.DispatchEvents();
                window.Clear();

                //call game update
                OnUpdate();

                //call all entity updates
                HandleEntityUpdates();

       
                Entities.entityManager.CleanUp();

                FPSCounter.DisplayedString = FPS.ToString();
                EntityCount.DisplayedString = Entities.entityManager.Entitys.Count.ToString();
                EntitiesToDestroyCount.DisplayedString = Entities.entityManager.EntitysToBeDestroyed.Count.ToString();

                window.Draw(FPSCounter);
                window.Draw(EntityCount);
                window.Draw(EntitiesToDestroyCount);
                window.Draw(MemoryCounter);

                window.Display();

                frameCount++;
                if (fpsClock.ElapsedTime.AsSeconds() >= 1.0f)
                {
                    FPS = frameCount;
                    frameCount = 0;
                    fpsClock.Restart();
                }

                memoryUsageBytes = currentProcess.PrivateMemorySize64;
                memoryUsageMB = memoryUsageBytes / (1024.0 * 1024.0);

               

                MemoryCounter.DisplayedString = Math.Round(memoryUsageMB, 2).ToString(CultureInfo.CurrentCulture);


            }
        }
        void HandleClose(object sender, EventArgs e)
        {
            window.Close();
            window.Closed -= HandleClose;
            window.KeyPressed -= HandleKeyPress;



        }


        void HandleKeyPress(object sender, SFML.Window.KeyEventArgs e)
        {
        }

        static public bool IsKeyPressed(SFML.Window.Keyboard.Key key)
        {
            return SFML.Window.Keyboard.IsKeyPressed(key);
        }

        static public void Log<T>(T Data)
        {
            if (bShouldLog)
                Console.WriteLine($"[{RunTimeClock.ElapsedTime.AsSeconds()}][{Data}]");
        }

        static public Time GetTime()
        {
            return RunTimeClock.ElapsedTime;
        }

        public static Entity Instantiate(Entity entity)
        {
            Instance.Entities.entityManager.AddEntity(entity);

            return entity;
        }
        public static void Destroy(Entity entity)
        {
            Instance.Entities.entityManager.Destroy(entity);
        }
        static void HandleEntityUpdates()
        {
            Instance.Entities.entityManager.HandleEntityUpdates();
        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnStart()
        {

        }
    };
    #endregion
}
