using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;


namespace DeusEngine
{

    #region Application

    //main game logic
    public class Application
    {
        // Singleton instance of the game
        public static Application Instance;

        // Render window for the game
        public static RenderWindow window;

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
        static public Font BaseFont = new Font("F:\\Dev\\Deus-Engine\\Deus\\consolas\\consola.ttf");

        static public Vector2f MousePos;

        protected static bool bShouldLog = true;


        // Background music
        public Music BackGroundMusic;

        // Constructor
        public Application()
        {
            if (Instance == null)
                Instance = this;
            Entities = new EntityManager();
            colliderManager = new ColliderManager();
            OnInit();

        }

        // Draw a drawable object on the window
        public static void Draw(Drawable drawable)
        {
            if (window != null && drawable != null)
                window.Draw(drawable);
        }



        // Start the game
        public void Start()
        {
            window = new RenderWindow(new VideoMode(iWidth, iHeight), sName);
                
            RunTimeClock.Restart();
            window.Closed += HandleClose;
            window.KeyPressed += HandleKeyPress;

            OnStart();

            HandleUpdate();
        }

        // Size of the text displayed
        static uint iSize = 15;
        protected bool bShouldShowDebug = false;
        Text FPSCounter = new Text("", BaseFont, iSize);
        Text EntityCount = new Text("", BaseFont, iSize);
        Text EntitiesToDestroyCount = new Text("", BaseFont, iSize);
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
                MousePos = (Vector2f)Mouse.GetPosition(window);
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
            OnEnd();
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

        //check if a specific mouse button is pressed
        static public bool IsMouseButtonPressed(Mouse.Button button)
        {
            return Mouse.IsButtonPressed(button);
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

        public virtual void OnInit() 
        { }

        public virtual void OnEnd() 
        { }

        public static void DrawPoint(Vector2f position, Color color, float fRadius)
        {
            if (window != null)
            {
                //handle drawing a circle with fRadius as the radius
                CircleShape circle = new CircleShape(fRadius);
                circle.Position = position;
                circle.FillColor = color;
                window.Draw(circle);
            }
        }

        public static void DrawLine(Vector2f Start, Vector2f End, Color debugLineColor)
        {
           //add line to vertex array
           if (window != null)
            {
                VertexArray lines = new VertexArray(PrimitiveType.Lines);
                lines.Append(new Vertex(Start, debugLineColor));
                lines.Append(new Vertex(End, debugLineColor));

                window.Draw(lines);
            }

        }

        public static void DrawRay(Vector2f Position,Vector2f Direction, float fLength, Color debugLineColor)
        {
            if(window != null)
            {
                //store the vertex array
                VertexArray lines = new VertexArray(PrimitiveType.Lines);
                //from starting point
                lines.Append(new Vertex(Position, debugLineColor));
                //to starting point + (direction * length)
                lines.Append(new Vertex(Position + (Direction * fLength), debugLineColor));
                //draw line
                window.Draw(lines);
            }
        }
        // DrawRect function to draw a colored rectangle using vertices
        public static void DrawRect( float x, float y, float width, float height, Color color)
        {
            VertexArray vertices = new VertexArray(PrimitiveType.Quads, 4);

            // Define the vertices of the rectangle
            vertices[0] = new Vertex(new Vector2f(x, y), color);
            vertices[1] = new Vertex(new Vector2f(x + width, y), color);
            vertices[2] = new Vertex(new Vector2f(x + width, y + height), color);
            vertices[3] = new Vertex(new Vector2f(x, y + height), color);

            Application.Draw(vertices);
        }
    }
    #endregion
}
