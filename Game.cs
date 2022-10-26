using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using SFML.Window;

namespace Wolstencroft
{

    class Component
    {
        protected Game game;
        public Entity entity { get; private set; }


        public virtual void OnUpdate() { }
        public virtual void OnStart() { }

        public Component()
        {
            game = Game.Instance;
        }

        public void SetEntityParent(ref Entity entity)
        {
            this.entity = entity;
        }

    }

    class Renderable : Component
    {
        RectangleShape Body = new RectangleShape();

        public override void OnUpdate()
        {
            Body.Position = entity.GetComponent<Transform2D>().position;
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

        private Entity entityRef;


        public Entity()
        {
            entityRef = this;
            //add transform2d by default
            transform = AddComponent<Transform2D>();
            
        }

        public void RunStarts()
        {
            foreach (var Comp in components)
            {
                Comp.OnStart();
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

        public Keyboard.Key CurrentPressedKey;

        public Game()
        {
            if (Instance == null)
                Instance = this;
            window = new RenderWindow(new VideoMode(iWidth, iHeight), sName);
        }

        public void Draw(SFML.Graphics.Drawable drawable)
        {
            if (window != null || drawable != null)
                window.Draw(drawable);
        }


        public void Start()
        {
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
            foreach (var Ent in Entitys)
            {
                Ent.RunUpdates();
            }
        }

        public void AddEntity(Entity entity)
        {
            entity.RunStarts();
            Entitys.Add(entity);

        }

        void HandleKeyPress(object sender, SFML.Window.KeyEventArgs e)
        {
            CurrentPressedKey = e.Code;
        }

        public void Log<T>(T Data)
        {
            Console.WriteLine($"[{Data}]");
        }

    };
}

