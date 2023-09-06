using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using DeusEngine;

class Program
{
    #region CustomComponents

    class PlayerController : Component
    {
        float fAxisY = 0, fAxisX = 0;

        float fSpeed = 50f;
        float fScale = 0.025f;
        public override void OnUpdate()
        {
            if (Application.IsKeyPressed(Keyboard.Key.W))
            {
                fAxisY -= fScale;
            }
            else if (Application.IsKeyPressed(Keyboard.Key.S))
            {
                fAxisY += fScale;
            }
            else if (fAxisY != 0)
            {
                fAxisY = 0;
            }

            if (Application.IsKeyPressed(Keyboard.Key.D))
            {
                fAxisX += fScale;
            }
            else if (Application.IsKeyPressed(Keyboard.Key.A))
            {
                fAxisX -= fScale;
            }
            else if (fAxisX != 0)
            {
                fAxisX = 0;
            }

            //handle rotation
            if(Application.IsKeyPressed (Keyboard.Key.Right))
            {
                transform.fRotation += 90f * Application.DeltaTime.AsSeconds();
            }
            else if(Application.IsKeyPressed(Keyboard.Key.Left))
            {
                transform.fRotation -= 90f * Application.DeltaTime.AsSeconds();

            }



            fAxisY = Math.Clamp(fAxisY, -1, 1);
            fAxisX = Math.Clamp(fAxisX, -1, 1);


            //move the player towards the mouse

            Vector2f _vResult = new Vector2f(fAxisX, fAxisY) * fSpeed * Application.DeltaTime.AsSeconds();


            entity.transform.position += (_vResult);
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
            Application.Destroy(entity);
        }

    }

    #endregion

    const float fScale = 21f;
    public class Player : Entity
    {
        PlayerController _controller;
        Renderable _renderable;
        Collider2D collider;

        public Player()
        {
            _controller = AddComponent<PlayerController>();
            _renderable = AddComponent<Renderable>();
            collider = AddComponent<Collider2D>();
            _renderable.FillColour = Color.White;
            transform.size = new Vector2f(5, 5);

            collider.bShouldDrawBounds = true;
            //ray.fDistance = 20f;
        }

        public override void OnUpdate()
        {
            float fCurrentRot = transform.fRotation;
            float fRadian = (((fCurrentRot + 90) * DMath.PI) / 180);

        }
    }
    public class Cell : Entity
    {
        Renderable _renderable;
        Collider2D collider;
        Vector2f CellSize;
        public int iData = 0;
        public Cell(Vector2f size, int iNewData)
        {
            iData = iNewData;
            CellSize = size;
            _renderable = AddComponent<Renderable>();
            if(iData == 1)
            {
                collider = AddComponent<Collider2D>();
            }
            transform.size = new Vector2f(CellSize.X, CellSize.Y);
        }
    }
    public class Test : Application
    {
        Player _player; 
        const int _iWidth = 6, _iHeight = 6;
        Cell[] _cells;
        int[] _iMap =
        {
            1,1,1,1,1,1,
            1,0,1,0,0,1,
            1,0,1,0,1,1,
            1,0,0,0,0,1,
            1,0,1,0,0,1,
            1,1,1,1,1,1
        };

        public Test() 
        {
            iWidth = 1920 / 2;
            iHeight = 1080 / 2;
        }

        public override void OnStart()
        {
            _cells = new Cell[_iWidth * _iHeight];
            _player = new Player();
            //initialize the map

            for (int y = 0; y < _iHeight; y++) 
            {
                for(int x = 0; x < _iWidth; x++)
                {
                    //calculate the index for a 2d map on a 1d array
                    int iIndex = y * _iWidth + x;
                    //cache the current cells
                    Cell cache = _cells[iIndex] = new Cell(new Vector2f(20,20), _iMap[iIndex]);

                    if (cache.iData == 0)
                        continue;
                    //cache the renderable
                    Renderable RenderCache = cache.GetComponent<Renderable>();
                    //set the position 
                    cache.transform.position = new Vector2f(x * fScale + fScale, y * fScale + fScale);
                    //set the color and to delete

                    if (_iMap[iIndex] == 1)
                    {
                        RenderCache.FillColour = (Color.White);
                    }

                    //add it to the game
                    Instantiate(cache);
                    cache.GetComponent<Collider2D>().bShouldDrawBounds = true;

                }
            }

            Instantiate(_player);
        }


    }

    

    static void Main()
    {
        Test game = new Test();
        game.Start();

    }


};