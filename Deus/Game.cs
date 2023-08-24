using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;


namespace DeusEngine
{

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

        public ColliderManager colliderManager;

        // Delta time between frames
        static public Time DeltaTime;

        // Frames per second
        static public int FPS;
        private Clock fpsClock;

        private int frameCount = 0;
        private Clock frameTimer = new Clock();

        // Font for text display
        static public Font EngineFont = new Font("..\\..\\..\\consolas/consola.ttf");

        static public Vector2f MousePos;

        protected static bool bShouldLog = true;


        // Background music
        public Music BackGroundMusic;

        // Constructor
        public Game()
        {
            if (Instance == null)
                Instance = this;
            window = new RenderWindow(new VideoMode(iWidth, iHeight), sName);
            Entities = new EntityManager();
            colliderManager = new ColliderManager();
        }

        // Draw a drawable object on the window
        public void Draw(Drawable drawable)
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
        bool bShouldShowDebug = false;
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
                MousePos = (Vector2f)Mouse.GetPosition(Instance.window);
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

                if (bShouldShowDebug)
                {
                    window.Draw(FPSCounter);
                    window.Draw(EntityCount);
                    window.Draw(EntitiesToDestroyCount);
                    //window.Draw(MemoryCounter);
                }

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
        void HandleKeyPress(object sender, KeyEventArgs e)
        {
        }

        // Check if a specific key is pressed
        static public bool IsKeyPressed(Keyboard.Key key)
        {
            return Keyboard.IsKeyPressed(key);
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
