using Wolstencroft;
using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using SFML.Window;
class Program
{
    #region CustomComponents
    #region Controller
    #endregion

    class PlayerController : Component
    {
        float fAxisY = 0, fAxisX = 0;

        float fSpeed = .05f;
        float fScale = 0.025f;
        public override void OnUpdate()
        {
            if (Game.IsKeyPressed(Keyboard.Key.W))
            {
                fAxisY -= fScale;
            }
            else if (Game.IsKeyPressed(Keyboard.Key.S))
            {
                fAxisY += fScale;
            }
            else if (fAxisY != 0)
            {
                fAxisY = 0;
            }

            if (Game.IsKeyPressed(Keyboard.Key.D))
            {
                fAxisX += fScale;
            }
            else if (Game.IsKeyPressed(Keyboard.Key.A))
            {
                fAxisX -= fScale;
            }
            else if (fAxisX != 0)
            {
                fAxisX = 0;
            }



            fAxisY = Math.Clamp(fAxisY, -1, 1);
            fAxisX = Math.Clamp(fAxisX, -1, 1);


            //move the player towards the mouse

            Vector2f _vResult = new Vector2f(fAxisX, fAxisY) * fSpeed;


            entity.transform.position += (_vResult);
        }
    }

    class Projectile : Component
    {
        public Vector2f DirectionToFire;
        public float fSpeed = 100f;
        public float fTimeToWait = 2f;

        public override void OnStart()
        {
            DestroyObject();
        }

        public override void OnUpdate()
        {

            transform.position -= DirectionToFire * fSpeed * Game.DeltaTime.AsSeconds();
        }

        async void DestroyObject()
        {
            await Task.Delay((int)(fTimeToWait * 1000f));
            Game.Destroy(entity);

        }

    }

    class ProjectileHandler : Component
    {
        public Vector2f DirectionToFire;

        bool bCanFire = true;
        public float fTimeToWait = 0.25f;


        public override void OnStart()
        {
        }
        public override void OnUpdate()
        {
            DirectionToFire = entity.transform.position - (Vector2f)Mouse.GetPosition(Game.Instance.window);
            DirectionToFire = WMaths.Normalize(DirectionToFire);

            if (Mouse.IsButtonPressed(Mouse.Button.Left) && bCanFire)
            {
                Shoot();
            }
        }

        async void Shoot()
        {
            bCanFire = false;
            ProjectileOBJ projectile = new ProjectileOBJ();
            projectile.transform.position = transform.position;
            projectile.projectile.DirectionToFire = DirectionToFire;
            Game.Instantiate(projectile);
            await Task.Delay((int)(fTimeToWait * 1000f));
            bCanFire = true;

        }
    }
    #endregion
    #region CustomEntities
    #region Test

    class ProjectileOBJ : Entity
    {
        public Projectile projectile;
        public Renderable renderable;
        Collider2D collider;

        public ProjectileOBJ()
        {
            //reference the projectile
            projectile = AddComponent<Projectile>();
            renderable = AddComponent<Renderable>();
            collider = AddComponent<Collider2D>();


            transform.size = new Vector2f(5, 5);
        }
    }

    class Player : Entity
    {
        PlayerController playerController;
        Renderable renderable;
        ProjectileHandler projectileHandler;
        Collider2D collider;

        public Player()
        {
            transform.size = new Vector2f(10, 10);

            playerController = AddComponent<PlayerController>();
            renderable = AddComponent<Renderable>();
            projectileHandler = AddComponent<ProjectileHandler>();
            collider = AddComponent<Collider2D>();


            transform.position += transform.size / 2;

        }
    }

    class TestCollider : Entity
    {
        Collider2D collider;
        Renderable renderable;

        public TestCollider()
        {
            transform.size = new Vector2f(10, 10);

            collider = AddComponent<Collider2D>();
            renderable = AddComponent<Renderable>();

            collider.OnCollisionEvent += Collision;
            collider.OnExitCollisionEvent += Exit;

            transform.position = new Vector2f(100, 100);
        }

        void Collision(Collider2D collider)
        {
            renderable.Body.FillColor = Color.Red;
        }

        void Exit(Collider2D collider)
        {
            renderable.Body.FillColor = Color.White;

        }


    };


    #endregion
    #endregion




    static void Main()
    {
        Game game = new Game();
        Player player = new Player();
        TestCollider testCollider = new TestCollider();
        TestCollider testCollider1 = new TestCollider();

        testCollider1.transform.position = new Vector2f(200, 200);


        Game.Instantiate(player);
        Game.Instantiate(testCollider);
        Game.Instantiate(testCollider1);


        game.Start();


    }


};