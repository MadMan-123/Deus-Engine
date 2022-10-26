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
            if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.W))
            {
                fAxisY -= fScale;
            }
            else if (SFML.Window.Keyboard.IsKeyPressed(SFML.Window.Keyboard.Key.S))
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

            Game.Instance.Log(_vResult);

           entity.transform.position += (_vResult);
        }
    }


    static Game game = new Game();
    static void Main()
    {
        Entity Player = new Entity();
        Player.AddComponent<PlayerController>();
        Player.AddComponent<Renderable>();
        game.AddEntity(Player);
        game.Start();

    }   


};

