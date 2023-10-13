
using System.Diagnostics;
using System.Numerics;
using Silk.NET;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;


namespace DeusEngine
{

    #region Application

    //main game logic
    public class Application
    {
        // Singleton instance of the game
        public static Application Instance;

        public RenderingEngine renderingEngine;

        // Name of the window
        public string sName = "Window";

        // Clock to measure the runtime
        //static Clock RunTimeClock = new Clock();

        // Entity manager to handle entities
        public EntityManager Entities;

        public ColliderManager colliderManager;

        // Delta time between frames
        static public float DeltaTime;

        // Frames per second
        static public int FPS;
       //private Clock fpsClock;

        private int frameCount = 0;
        //private Clock frameTimer = new Clock();

        // Font for text display
        //static public Font BaseFont = new Font("F:\\Dev\\Deus-Engine\\Deus\\consolas\\consola.ttf");

        static public Vector2D<float> MousePos;

        protected static bool bShouldLog = true;
        

        // Constructor
        public Application()
        {
            if (Instance == null)
                Instance = this;
            Entities = new EntityManager();
            colliderManager = new ColliderManager();
            renderingEngine = new RenderingEngine();

            OnInit();

        }
        

        // Start the game
        public void Start()
        {

            RenderingEngine.Instance.OnStart();
        }

        public void SubscribeEvents()
        {
            RenderingEngine.window.Load += this.OnLoad;
            RenderingEngine.window.Update += this.OnUpdate;
        }
        
        // Size of the text displayed
        static uint iSize = 15;
        protected bool bShouldShowDebug = false;

        // Update loop for the game
        


        // Event handler for window close
        void HandleClose(object sender, EventArgs e)
        {
            OnEnd();
            Entities.CleanUp();
            RenderingEngine.window.Close();

        }
        



        // Log a message
        static public void Log<T>(T Data)
        {
            if (bShouldLog)
                Console.WriteLine($"[{0}][{Data}]");
        }
        
        // Instantiate an entity and add it to the entity manager
        public static Entity Instantiate(Entity entity)
        {
            
            Instance.Entities.AddEntity(entity);
            //set the function subscriptions
            //subscribe to actions
            RenderingEngine.window.Load += entity.OnLoad;
            RenderingEngine.window.Update += entity.OnUpdate;
            entity.SubscribeComponents( RenderingEngine.window);

            

            return entity;
        }

        // Destroy an entity
        public static void Destroy(Entity entity)
        {
            Instance.Entities.Destroy(entity);
        }


        // Game update method to be overridden in derived classes
        public virtual void OnUpdate(double t)
        {
            
            if(Entities.EntitiesToBeDestroyed.Count > 0)
                Entities.CleanUp();
        }
        // Game start method to be overridden in derived classes
        public virtual void OnLoad()
        {
        }

        public virtual void OnInit() 
        { }

        public virtual void OnEnd() 
        { }

 
        
        // DrawRect function to draw a colored rectangle using vertices
        public static void DrawRect( float x, float y, float width, float height)
        {
          
        }
    }
    #endregion
}
