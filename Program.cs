using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Wolstencroft;

class Program
{
    #region CustomComponents
    #region Controller
    #endregion

    class PlayerController : Component
    {
        float fAxisY = 0, fAxisX = 0;

        float fSpeed = 50f;
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

            Vector2f _vResult = new Vector2f(fAxisX, fAxisY) * fSpeed * Game.DeltaTime.AsSeconds();


            entity.transform.position += (_vResult);
        }
    }

    class Projectile : Component
    {
        public Vector2f DirectionToFire;
        public float fSpeed = 100f;
        public float fTimeToWait = 2f;
        public int iDamage = 25;

        public override void OnStart()
        {
            entity.sTags.Add("Projectile");

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

    class Health : Component
    {
        public int iMaxHealth = 100;
        public int iCurrentHealth { get; private set; }

        public Action<Health>? OnDeath;
        public override void OnStart()
        {
            iCurrentHealth = iMaxHealth;
        }

        public void TakeDamage(int Damage)
        {

            iCurrentHealth -= Damage;

            if (iCurrentHealth < 1)
            {
                HandleDeath();
            }
        }

        void HandleDeath()
        {
            OnDeath?.Invoke(this);
            Game.Destroy(entity);
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

            collider.OnCollisionEvent += OnHit;

            transform.size = new Vector2f(5, 5);

        }

        void OnHit(Collider2D other)
        {
            if (other.entity.CompareTag("Enemy"))
                Game.Destroy(this);

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

            collider.bShouldDrawBounds = false;

            renderable.SetTexture("..\\..\\..\\Marat - copy.jpg");


        }
    }

    class Enemy : Entity
    {
        Collider2D colliderComp;
        Renderable renderable;
        Health health;

        string soundFilePath = "..\\..\\..\\Scream.wav";
        SoundBuffer soundBuffer;
        Sound sound;

        public Enemy()
        {
            soundBuffer = new SoundBuffer(soundFilePath);
            sound = new Sound(soundBuffer);
            transform.size = new Vector2f(10, 10);

            colliderComp = AddComponent<Collider2D>();
            renderable = AddComponent<Renderable>();
            health = AddComponent<Health>();

            health.iMaxHealth = 100;

            colliderComp.OnCollisionEvent += Collision;
            colliderComp.OnExitCollisionEvent += Exit;

            colliderComp.bShouldDrawBounds = true;

            transform.position = new Vector2f(100, 100);

            health.OnDeath += Death;
        }

        void Collision(Collider2D other)
        {

            if (other.entity.CompareTag("Projectile"))
            {
                Projectile proj = other.entity.GetComponent<Projectile>();
                if (proj != null)
                {
                    health.TakeDamage(proj.iDamage);
                    transform.position -= proj.DirectionToFire * 5;


                }
            }
        }

        void Exit(Collider2D other)
        {

        }

        void Death(Health NewHealth)
        {
            sound.Play();
        }

    };


    public class SpawenerOBJ : Entity
    {
        ProjectileOBJ projectile = new ProjectileOBJ();

        float fRot = 0f;

        public SpawenerOBJ()
        {

        }

        public override void OnStart()
        {
            Spawn(1);

        }

        async void Spawn(int iTime)
        {

            await Task.Delay((iTime));
            Game.Instantiate(projectile);
            Spawn(iTime);

        }
    }


    public class MaratOBJ : Entity
    {
        Renderable renderable;

        public MaratOBJ()
        {
            renderable = AddComponent<Renderable>();
            renderable.SetTexture("..\\..\\..\\Marat - copy.jpg");

            transform.position = new Vector2f(200, 200);
            transform.size = new Vector2f(200, 200);
        }

    };

    #endregion
    #endregion

    public class MazeGenGame : Game
    {

        public class Cell : Entity
        {
            Renderable renderable;

            TextComponent text;

            Shader shader;
            float fScale = 20;



            public override void OnStart()
            {
                text = AddComponent<TextComponent>();
                renderable = AddComponent<Renderable>();

                shader = new Shader(null, null, "..\\..\\..\\BackGround.frag");
                shader.SetUniform("Resolution", new Vector2f(Game.Instance.iWidth,Game.Instance.iHeight));
                shader.SetUniform("fScale", fScale);
                renderable.SetShader(shader);
            }


            public override void OnUpdate()
            {
                shader.SetUniform("MousePos", Game.MousePos);
                shader.SetUniform("uTime", Game.GetTime().AsSeconds());
                if(Game.IsKeyPressed(Keyboard.Key.Up))
                {
                    fScale += 0.1f;
                }
                else if(Game.IsKeyPressed(Keyboard.Key.Down))
                {
                    fScale -= 0.1f;
                }

                if (fScale <= 0)
                {
                    fScale = 0;
                }
                shader.SetUniform("fScale", fScale);

            }

        }


        public override void OnStart()
        {
            Cell cell = new Cell();
            cell.transform.size = new Vector2f(1000, 1000);
            Entities.AddEntity(cell);
        }

        public override void OnUpdate()
        {
        }
    }
    static void Main()
    {
        MazeGenGame game = new MazeGenGame();
        game.Start();


    }


};