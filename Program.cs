using Wolstencroft;
using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using SFML.Window;

class Program
{
    #region CustomComponents
    class PlayerController : Component
    {
        float fAxisY = 0;
        float fSpeed = .05f;
        float fScale = 0.1f;
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

            fAxisY = Math.Clamp(fAxisY, -1, 1);

            //move the player towards the mouse

            //get direction
            Vector2f _vDir = entity.transform.position - (Vector2f)Mouse.GetPosition(Game.Instance.window);

            Vector2f _vDirNorm = WMaths.Normalize(_vDir);
            Vector2f _vResult = _vDirNorm * fAxisY * fSpeed;


           entity.transform.position += (_vResult);
        }
    }

    class Projectile : Component
    {
        public Vector2f DirectionToFire;
        public float fSpeed = .25f;
        public float fTimeToWait = 5f;

        public override void OnStart()
        {
            DestroyObject();
        }

        public override void OnUpdate()
        {
            
            transform.position -= DirectionToFire * fSpeed;
        }

        async void DestroyObject()
        {
            await Task.Delay((int)(fTimeToWait * 1000f));
            Game.Instance.Destroy(entity);

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
            Game.Instance.AddEntity(projectile);
            await Task.Delay((int)(fTimeToWait * 1000f));
            bCanFire = true;

        }
    }

    #endregion
    #region CustomEntities
    class ProjectileOBJ : Entity
    {
        public Projectile projectile;
        public Renderable renderable;
        public ProjectileOBJ()
        {
            //reference the projectile
            projectile = AddComponent<Projectile>();
            renderable = AddComponent<Renderable>();

            transform.size = new Vector2f(5,5);
        }
    }

    class Player : Entity
    {
        PlayerController playerController;
        Renderable renderable;
        ProjectileHandler projectileHandler;

        public Player()
        {
            playerController = AddComponent<PlayerController>();
            renderable = AddComponent<Renderable>();
            projectileHandler = AddComponent<ProjectileHandler>();

            transform.size = new Vector2f(10, 10);

            transform.position += transform.size / 2;
            
        }
    }

    #endregion

    static Game game = new Game();
    static void Main()
    {
        Player player = new Player();
        game.AddEntity(player);
  
        Game.Log(WMaths.Lerp(new Vector2f(0, 0), new Vector2f(10, 10), .5f));

        game.Start();

    }   


};

