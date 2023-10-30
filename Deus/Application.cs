
using System.Diagnostics;
using System.Numerics;
using Silk.NET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;


namespace DeusEngine
{
    public class FPSTimer
    {
        private Stopwatch stopwatch;
        private int frameCount;

        public int FPS { get; private set; }

        public FPSTimer()
        {
            stopwatch = new Stopwatch();
            frameCount = 0;
            stopwatch.Start();
        }

        public void Frame()
        {
            frameCount++;
            if (stopwatch.Elapsed.TotalSeconds >= 1)
            {
                FPS = (int)(frameCount / stopwatch.Elapsed.TotalSeconds);
                frameCount = 0;
                stopwatch.Restart();
            }
        }
    }







    #region Application

    //main game logic
    public class Application
    {
        // Singleton instance of the game
        public static Application Instance;


        // Name of the window
        public string sName = "Window";

        // Clock to measure the runtime
        //static Clock RunTimeClock = new Clock();

        // Entity manager to handle entities
        private EntityManager Entities;

        private ColliderManager colliderManager;
        private RenderingEngine renderingEngine;
        private InputManager inputManager;
        private FPSTimer timer = new FPSTimer();

        // Font for text display
        //static public Font BaseFont = new Font("F:\\Dev\\Deus-Engine\\Deus\\consolas\\consola.ttf");
        
        protected static bool bShouldLog = true;
        public static Vector2 MousePosition = new Vector2();
        public static int MouseScroll = 0;
        public static string sCurrentPath = System.IO.Directory.GetCurrentDirectory();
        public static string sCurrentProjectPath
        {
            get
            {
                string currentPath = sCurrentPath;
        
                // Navigate up from the current directory
                string parentDirectory = System.IO.Directory.GetParent(currentPath)?.FullName;

                while (!string.IsNullOrEmpty(parentDirectory))
                {
                    // Check if the current directory contains "Deus" and "Assets" folders
                    if (System.IO.Directory.Exists(System.IO.Path.Combine(currentPath, "Deus")) &&
                        System.IO.Directory.Exists(System.IO.Path.Combine(currentPath, "Assets")))
                    {
                        return currentPath;
                    }

                    // Navigate up one level in the directory structure
                    currentPath = parentDirectory;
                    parentDirectory = System.IO.Directory.GetParent(currentPath)?.FullName;
                }

                return string.Empty; // Not in a Deus Engine project
            }
        }

        public static string sAssetsPath = sCurrentProjectPath + "\\Assets\\";


        

        
        private ImGuiManager imguiManager;
        // Constructor
        public Application()
        {
            if (Instance == null)
                Instance = this;
            Entities = new EntityManager();
            colliderManager = new ColliderManager();
            renderingEngine = new RenderingEngine();
            OnInit();
            //imguiManager = new ImGuiManager();
            Application.Log(sCurrentProjectPath);
        }
        

        // Start the game
        public void Start()
        {
            OnStart();

            RenderingEngine.Instance.OnStart();
        }

        public virtual void OnStart()
        {
            
        }

        public void SubscribeEvents()
        {
            RenderingEngine.window.Load += HandleLoad;
            RenderingEngine.window.Update += HandleUpdate;
            
        }
        
        // Size of the text displayed
        static uint iSize = 15;
        protected bool bShouldShowDebug = false;

        // Update loop for the game
        


        // Event handler for window close
        public void HandleClose()
        {
            //imguiManager.Dispose();
            OnEnd();
            Entities.CleanUp();
            

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

        public void HandleUpdate(double t)
        {
            if(Entities.EntitiesToBeDestroyed.Count > 0)
                Entities.CleanUp();
            //if debug is on, set the title of the window to the stats
            if (bShouldLog)
            {
                RenderingEngine.window.Title = $"FPS:{timer.FPS}, Size:{RenderingEngine.window.Size}, Entities:{Entities.Entities.Count}";
            }
            //imguiManager.OnUpdate(t);
            
            OnUpdate(t);
            timer.Frame();

        }

        public void HandleLoad()
        {
            IInputContext input = RenderingEngine.window.CreateInput();
            
            inputManager = new InputManager(
                input.Keyboards[0],
                input.Mice[0]
                );

            //imguiManager.OnLoad();
            OnLoad();
        }
        
        // Game update method to be overridden in derived classes
        public virtual void OnUpdate(double t) { }
        // Game start method to be overridden in derived classes
        public virtual void OnLoad() { }
        public virtual void OnInit() { }
        public virtual void OnEnd() { }
        
        
        // DrawRect function to draw a colored rectangle using vertices
        public static void DrawRect( float x, float y, float width, float height)
        {
          
        }

        public static bool IsKeyPressed(Key key)
        {
            return InputManager.Instance.IsKeyDown(key);
        }

        public static bool IsMousePressed(MouseButton button)
        {
            return InputManager.Instance.IsMouseButtonDown(button);
        }


    }
    #endregion
}
