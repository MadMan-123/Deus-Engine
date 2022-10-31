using Wolstencroft;
using SFML.Graphics;
using SFML.System;
using SFML.Audio;
using SFML.Window;

class Program
{
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

        public override void OnUpdate()
        {
            transform.position -= DirectionToFire * fSpeed;
        }


    }

    class ProjectileOBJ : Entity
    {
        public Projectile projectile;
        public ProjectileOBJ()
        {
            projectile = AddComponent<Projectile>();
            AddComponent<Renderable>();

            

        }
    }


    class ProjectileHandler : Component
    {
        public Vector2f DirectionToFire;

        int iTimer = 0;
        int iMax = 1000;
        bool bCanFire = true;

        public override void OnStart()
        {
        }
        public override void OnUpdate()
        {
            DirectionToFire = entity.transform.position - (Vector2f)Mouse.GetPosition(Game.Instance.window);
            DirectionToFire = WMaths.Normalize(DirectionToFire);

            if (Mouse.IsButtonPressed(Mouse.Button.Left) && bCanFire)
            {
                iTimer = 0;
                bCanFire = false;
                ProjectileOBJ projectile = new ProjectileOBJ();
                projectile.transform.position = transform.position;
                projectile.projectile.DirectionToFire = DirectionToFire;
                Game.Instance.AddEntity(projectile);
                

            }

            if (!bCanFire)
            {
                iTimer += 1;
                bCanFire = iTimer > iMax;
            }



        }
    }


    static Game game = new Game();
    static void Main()
    {
        Entity Player = new Entity();
        Player.AddComponent<PlayerController>();
        Player.AddComponent<Renderable>();
        Player.AddComponent<ProjectileHandler>();

        game.AddEntity(Player);

  
        Game.Log(WMaths.Lerp(new Vector2f(0, 0), new Vector2f(10, 10), .5f));

        game.Start();

    }   


};

