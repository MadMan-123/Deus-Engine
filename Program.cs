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
        public float fSpeed = 500f;
        public float fTimeToWait = 2f;
        public int iDamage = 10;

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


        public override void OnStart()
        {
            iCurrentHealth = iMaxHealth;
        }

        public void TakeDamage(int Damage)
        {

            iCurrentHealth -= Damage;

            if(iCurrentHealth < 1)
            {
                HandleDeath();
            }
        }

        void HandleDeath()
        {
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
            if(other.entity.CompareTag("Enemy"))
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

            collider.bShouldDrawBounds = true;

            transform.position += transform.size / 2;

        }
    }

    class TestCollider : Entity
    {
        Collider2D colliderComp;
        Renderable renderable;
        Health health;

        public TestCollider()
        {
            transform.size = new Vector2f(10, 10);

            colliderComp = AddComponent<Collider2D>();
            renderable = AddComponent<Renderable>();
            health = AddComponent<Health>();

            health.iMaxHealth = 100;

            colliderComp.OnCollisionEvent += Collision;
            colliderComp.OnExitCollisionEvent += Exit;

            colliderComp.bShouldDrawBounds = true;

            transform.position = new Vector2f(100, 100);
        }

        void Collision(Collider2D other)
        {
            renderable.Body.FillColor = Color.Red;

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
            renderable.Body.FillColor = Color.White;

        }


    };

    class WaveOBJ : Entity
    {
        List<TestCollider> Nodes = new List<TestCollider>();
        int iAmmountOfNodes = 10;
        private int fScale = 50;

        public override void OnStart()
        {
            //create a pool of x ammount of nodes
            for (int i = 0; i < iAmmountOfNodes; i++)
            {
                Nodes.Add((TestCollider)Game.Instantiate(new TestCollider()));

                Nodes[i].transform.position = new Vector2f(i * fScale, (MathF.Sin(i) * fScale) + 100);
            }

        }

        public override void OnUpdate()
        {
            //create a pool of x ammount of nodes
            for (int i = 0; i < iAmmountOfNodes; i++)
            {
                ref Vector2f pos = ref Nodes[i].transform.position;
                pos = new Vector2f((pos.X + 1), (MathF.Sin(Game.GetTime().AsSeconds() + i) * fScale) + 100);
                if (pos.X > 500)
                {
                    pos.X = 0;
                }
            }

        }

    }
    #endregion
    #endregion




    static void Main()
    {
        Game game = new Game();
        Player player = new Player();
        player.sTags.Add("Player");

        TestCollider collider = new TestCollider();
        collider.sTags.Add("Enemy");

        collider.transform.position = new Vector2f(100, 100);

        Game.Instantiate(player);
        Game.Instantiate(collider);


        game.Start();


    }


};