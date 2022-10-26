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
        float fSpeed = 2f;
        public override void OnUpdate()
        {
            if (Game.Instance.CurrentPressedKey == SFML.Window.Keyboard.Key.W)
            {
                fAxisY += .001f;
            }
            else if (Game.Instance.CurrentPressedKey == SFML.Window.Keyboard.Key.S)
            {
                fAxisY -= .001f;
            }

            fAxisY = Math.Clamp(fAxisY, -1, 1);

            //move the player towards the mouse

            //get direction
            Vector2f _vDir = entity.transform.position - (Vector2f)Mouse.GetPosition();

            //get normal
            Vector2f _vDirNorm = new Vector2f(
                _vDir.X / MathF.Sqrt(MathF.Pow(_vDir.X, 2)) + MathF.Pow(_vDir.X, 2),
                _vDir.Y / MathF.Sqrt(MathF.Pow(_vDir.Y, 2) + MathF.Pow(_vDir.Y, 2))
                );

            Game.Instance.Log(_vDirNorm);

           entity.transform.position += new Vector2f();
        }
    }


    static Game game = new Game();
    static void Main()
    {
        Entity Player = new Entity();
        Player.AddComponent<PlayerController>();
        game.AddEntity(Player);
        game.Start();

    }   


};

