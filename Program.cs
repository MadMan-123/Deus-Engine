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

        float fSpeed = .5f;
        float fRotationSpeed = 40f;
        public override void OnUpdate()
        {
            if (Application.IsKeyPressed(Keyboard.Key.W))
            {
                entity.transform.position += (transform.Forward) * fSpeed;
            }
            else if (Application.IsKeyPressed(Keyboard.Key.S))
            {
                entity.transform.position -= (transform.Forward) * fSpeed;
            }

            const int iTurnScale = 2;
            if (Application.IsKeyPressed(Keyboard.Key.D))
            {
                entity.transform.position += (transform.Right) * fSpeed / iTurnScale;
            }
            else if (Application.IsKeyPressed(Keyboard.Key.A))
            {
                entity.transform.position += (transform.Left) * fSpeed / iTurnScale;
            }


            //handle rotation
            if(Application.IsKeyPressed (Keyboard.Key.Right))
            {
                transform.fRotation += fRotationSpeed  * Application.DeltaTime.AsSeconds();
            }
            else if(Application.IsKeyPressed(Keyboard.Key.Left))
            {
                transform.fRotation -= fRotationSpeed  * Application.DeltaTime.AsSeconds();

            }

            //handle rotation with mouse
            Vector2f mousePos = Application.MousePos;
            
            //if mouse button is held down
            if (Application.IsMouseButtonPressed(Mouse.Button.Right))
            {
                //see if the mouse is on the right side of the screen
                if (mousePos.X > Application.window.Size.X / 2)
                {
                    transform.fRotation += fRotationSpeed * Application.DeltaTime.AsSeconds();
                }
                else if (mousePos.X < Application.window.Size.X / 2)
                {
                    transform.fRotation -= fRotationSpeed * Application.DeltaTime.AsSeconds();
                }
            }




           


            
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

    //true fact
    //- Jack Grub
    bool bJackIsGay = true;

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
            transform.position = new Vector2f(50,50);
            collider.bShouldDrawBounds = true;
            //ray.fDistance = 20f;

        }

        public override void OnStart()
        {
            SetTag("Player");

        }

        public override void OnUpdate()
        {
        }
    }
    public class Cell : Entity
    {
        Renderable _renderable;
        public Collider2D collider;
        Vector2f CellSize;
        public int iData = 0;
        public Cell(Vector2f size, int iNewData)
        {
            iData = iNewData;
            CellSize = size;
            _renderable = AddComponent<Renderable>();
            if(iData > 0)
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
        int[] _iMap;
        int iColumnWidth = 1;
        int numRays = 1080;
        float fFov = 60f;

        Texture marat = new Texture("F:\\Dev\\Deus-Engine\\Marat - Copy.jpg");
        public Test()
        {
            _iMap = new[] {
                2, 4, 1, 1, 1, 1,
                2, 0, 2, 0, 0, 1,
                2, 0, 2, 0, 1, 1,
                2, 0, 0, 0, 0, 1,
                2, 0, 2, 0, 0, 1,
                2, 3, 2, 1, 1, 1
            };
            iWidth = 1920 / 2;
            iHeight = 1080 / 2;
            
            //set the window to full screan
        }

        public override void OnStart()
        {
            bShouldShowDebug = true;
            _cells = new Cell[_iWidth * _iHeight];
            _player = new Player();

            for (int y = 0; y < _iHeight; y++)
            {
                for (int x = 0; x < _iWidth; x++)
                {
                    int iIndex = y * _iWidth + x;
                    Cell cache = _cells[iIndex] = new Cell(new Vector2f(20, 20), _iMap[iIndex]);

                    if (cache.iData == 0)
                        continue;

                    Renderable renderCache = cache.GetComponent<Renderable>();
                    cache.transform.position = new Vector2f(x * fScale, y * fScale);

                    if (_iMap[iIndex] > 0)
                    {
                        renderCache.FillColour = Color.White;
                    }

                    Instantiate(cache);
                    cache.collider.bShouldDrawBounds = true;
                }
            }
            Instantiate(_player);


        }

        public override void OnUpdate()
        {
            Vector2f playerPosition = _player.transform.position;
            float playerRotation = _player.transform.fRotation;
            float halfFov = fFov / 2.0f; // Half of the field of view in degrees
            int iMapData = 0;

            for (int i = 0; i < numRays; i++)
            {
                // Calculate the ray angle in degrees
                float rayAngle = playerRotation - halfFov + (i / (float)(numRays - 1)) * fFov;

                bool hitWall = false;
                Vector2f hitPoint = CastRay(playerPosition, DMath.DegToRad(rayAngle), out hitWall, out iMapData);
                float distanceToHit = DMath.Distance(hitPoint, playerPosition);
                Application.DrawLine(playerPosition, hitPoint, Color.Green);

                float wallHeight = iHeight / distanceToHit;
                float wallTop = iHeight / 2 - wallHeight / 2;
                float wallBottom = iHeight / 2 + wallHeight / 2;
                //get the hit cell index


                // Render the wall segment with floor shading
                RenderWallSegment(i, wallTop, wallBottom, distanceToHit, 50, .75f, iMapData);
            }

            //DrawFloor(_player.transform.position, playerRotation);
        }
        

        private Vector2f CastRay(Vector2f origin, float angle, out bool hitWall, out int iMapData)
        {
            Vector2f rayDirection = new Vector2f(MathF.Cos(angle), MathF.Sin(angle));
            Vector2f rayPosition = origin;
            float stepSize = 0.05f;

            hitWall = false;
            iMapData = 0; 

            while (true)
            {
                int cellX = (int)(rayPosition.X / fScale);
                int cellY = (int)(rayPosition.Y / fScale);

                if (cellX < 0 || cellY < 0 || cellX >= _iWidth || cellY >= _iHeight)
                {
                    return DMath.VectorZero;
                }

                int cellIndex = cellY * _iWidth + cellX;
                if (_iMap[cellIndex] > 0)
                {
                    iMapData = _iMap[cellIndex];
                    hitWall = true;
                    return rayPosition;
                }

                rayPosition += rayDirection * stepSize;
            }
        }

        private void RenderWallSegment(int column, float wallTop, float wallBottom, float distance, float maxDistance, float fLighting, int mapValue)
        {
            float lightingLevel = fLighting - (distance / maxDistance) * fLighting;
            lightingLevel = DMath.Clamp(lightingLevel, 0.0f, 1.0f);

            float x = column * iColumnWidth;
            float wallHeight = (wallBottom - wallTop);

            // Calculate the color based on the map value
            Color wallColor;
            switch (mapValue)
            {
                case 1:
                    wallColor = new Color(255, 0, 0); // Red
                    break;
                case 2:
                    wallColor = new Color(0, 255, 0); // Green
                    break;
                case 3:
                    wallColor = new Color(0, 0, 255); // Blue
                    break;
                case 4:
                    wallColor = new Color(255, 255, 255); // White
                    break;
                default:
                    wallColor = new Color(255, 255, 0); // Yellow
                    break;
            }

            // Adjust wall color based on lighting
            wallColor = new Color(
                (byte)(wallColor.R * lightingLevel),
                (byte)(wallColor.G * lightingLevel),
                (byte)(wallColor.B * lightingLevel)
            );

            // Draw a vertical line for the wall segment
            DrawLine(new Vector2f(x, wallTop), new Vector2f(x, wallBottom), wallColor);
        }
        
        

        void DrawFloor(Vector2f playerPosition, float playerRotation)
        {
            float floorTileSize = fScale; // Adjust as needed
            int numTilesX = (int)(iWidth / floorTileSize);
            int numTilesY = (int)(iHeight / floorTileSize);

            for (int y = 0; y < numTilesY; y++)
            {
                for (int x = 0; x < numTilesX; x++)
                {
                    float tileX = x * floorTileSize;
                    float tileY = y * floorTileSize;

                    // Calculate the position of the center of the floor tile
                    float tileCenterX = tileX + floorTileSize / 2;
                    float tileCenterY = tileY + floorTileSize / 2;

                    // Calculate the angle between the player's position and the floor tile
                    float angleToTile = DMath.RadToDeg(MathF.Atan2(tileCenterY - playerPosition.Y, tileCenterX - playerPosition.X));

                    // Calculate the angle difference between the player's rotation and the angle to the tile
                    float angleDifference = DMath.AngleDifference(playerRotation, angleToTile);

                    bool hitWall;
                    int iData = 0;
                    CastRay(playerPosition, DMath.DegToRad(angleToTile), out hitWall, out iData);

                    if (!hitWall)
                    {
                        // Calculate lighting based on angle difference
                        float lightingLevel = 1.0f - MathF.Abs(angleDifference) / (fFov / 2);
                        lightingLevel = DMath.Clamp(lightingLevel, 0.0f, 1.0f);

                        // Adjust the floor color based on lighting
                        Color floorColor = new Color(
                            (byte)(Color.Green.R * lightingLevel),
                            (byte)(Color.Green.G * lightingLevel),
                            (byte)(Color.Green.B * lightingLevel)
                        );

                        RectangleShape floorTile = new RectangleShape(new Vector2f(floorTileSize, floorTileSize))
                        {
                            Position = new Vector2f(tileX, tileY),
                            FillColor = floorColor
                        };

                        Draw(floorTile);
                    }
                }
            }
        }

        static void Main()
        {
            Test game = new Test();
            game.Start();
        }
    }

};