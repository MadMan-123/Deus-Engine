using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using SFML.Window;

namespace Wolstencroft
{
    static class WMaths
    {
        //find the square root 
        static public Vector2f Normalize(Vector2f input)
        {
            float fLength = MathF.Sqrt((input.X * input.X) + (input.Y * input.Y));

            return (input / fLength);
            
        }

        static public float Lerp(float a, float b, float T)
        {
            return (1-T)*a + b*T;
        }
        
        static public Vector2f Lerp(Vector2f a, Vector2f b, float T)
        {
            return (1-T)*a + b*T;
        }

    }

    class Component
    {
        protected Game game;
        public Entity entity { get; private set; }
        public Transform2D transform;


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

    class Renderable : Component
    {
        RectangleShape Body = new RectangleShape();

        public Vector2f Size = new Vector2f(10,10);

        public Renderable()
        {
            Body.Size = Size;
        }

        public override void OnUpdate()
        {
            Body.Position = entity.transform.position;
            Body.Rotation = entity.transform.fRotation;
            Game.Instance.Draw(Body);
        }
    }

    class Transform2D : Component
    {
        public Vector2f position = new Vector2f(0, 0);
        public float fRotation = 0f;
    }

    class Entity
    {
        public List<Component> components = new List<Component>();
        public Transform2D transform;
        public string Name = "Entity";
        private Entity entityRef ;


        public Entity()
        {
            entityRef = this;
            //add transform2d by default
            transform = AddComponent<Transform2D>();
            Name = this.ToString();
        }

        public void RunStarts()
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].OnStart();
            }
        }
        public void RunUpdates()
        {
            //runs all component updates 
            foreach (var Comp in components)
            {
                Comp.OnUpdate();
            }
        }

        public T AddComponent<T>() where T : Component, new()
        {
            T NewComp = new T();
            NewComp.SetEntityParent(ref entityRef);
            components.Add(NewComp);
            return NewComp;
        }

        public T GetComponent<T>() where T : Component, new()
        {
            T NewComp = new T();

            foreach (var Comp in components)
            {
                if (NewComp.GetType() == Comp.GetType())
                {
                    return (T)Comp;
                }
            }

            return null;


        }


    };

    class Game
    {

        
        public static Game Instance = null;

        public RenderWindow window;

        public uint iWidth = 200, iHeight = 200;

        public string sName = "Window";
        List<Entity> Entitys = new List<Entity>();

        static Clock RunTimeClock = new Clock();

        public Game()
        {
            if (Instance == null)
                Instance = this;
            window = new RenderWindow(new VideoMode(iWidth, iHeight), sName);
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

            HandleUpdate();
        }

        void HandleUpdate()
        {

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();

                //call all entity updates
                HandleEntityUpdates();

                window.Display();
            }
        }
        void HandleClose(object sender, EventArgs e)
        {
            window.Close();
            window.Closed -= HandleClose;
            window.KeyPressed -= HandleKeyPress;



        }
        void HandleEntityUpdates()
        {
            for(int i = 0; i < Entitys.Count; i++)
            {
                Entitys[i].RunUpdates();
            }
        }

        public void AddEntity(Entity entity)
        {
            Log($"Added: {( entity.Name)}");
            entity.RunStarts();
            Entitys.Add(entity);

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
            Console.WriteLine($"[{RunTimeClock.ElapsedTime.AsSeconds()}][{Data}]");
        }
        
        static public Time GetTime()
        {
            return RunTimeClock.ElapsedTime;
        }

    };
}

