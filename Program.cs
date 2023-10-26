
using System.Numerics;
using DeusEngine;
using Silk.NET.Input;
using static DeusEngine.RenderingEngine;


class Program
{
        class Bullet : Entity
        {
                private Renderable _renderable;

                private float fSpeed = 0.5f;

                public Bullet()
                {
                        _renderable = AddComponent<Renderable>();

                        transform.Scale = 0.05f;
                }
                public override void OnUpdate(double t)
                {
                        transform.Position += transform.Front * fSpeed * (float)t;

                }
        }
        class EntityOBJ : Entity
        {
                private Renderable _renderable;
                public EntityOBJ()
                {
                        _renderable = AddComponent<Renderable>();

                }

                public override void OnUpdate(double t)
                {
                        float fDeltaTime = (float)t;
                        
                        //rotate the cube on the y axis
                        transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, 1f * fDeltaTime);
    
  
                }
        }
        
   
        class App : Application
        {
                private EntityOBJ[] entitys = new EntityOBJ[2];
                private Camera _camera;

                public override void OnLoad()
                {
                        for (int i = 0; i < entitys.Length; i++)
                        {
                                entitys[i] = new EntityOBJ();
                                entitys[i].transform.Position = new Vector3(i , 0,0);

                                Instantiate(entitys[i]);
                        }
                        _camera = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY, window.Size.X / window.Size.Y);
                        Camera.SetMain(ref _camera);
                }

                private Vector2 LastMousePos = Vector2.Zero;
                bool isFirstRightMousePress = false;

                public override void OnUpdate(double t)
                {
                        entitys[0].transform.Position = new Vector3(0,MathF.Sin((float)(window.Time)) ,0);

                        if (IsKeyPressed(Key.W))
                        {
                                _camera.Position += _camera.Front * (float)t;
                        }
                        else if (IsKeyPressed(Key.S))
                        {
                                _camera.Position -= _camera.Front * (float)t;
                        }
                        if (IsKeyPressed(Key.A))
                        {
                                _camera.Position -= Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * (float)t;

                        }
                        else if (IsKeyPressed(Key.D))
                        {
                                _camera.Position += Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * (float)t;

                        }

                        if (IsMousePressed(MouseButton.Right))
                        {
                                float lookSensitivity = 100f;
                                if (isFirstRightMousePress)
                                {
                                        LastMousePos = MousePosition;
                                        isFirstRightMousePress = false;
                                }
                                float xOffset = (MousePosition.X - LastMousePos.X) * lookSensitivity * (float)t;
                                float yOffset = (MousePosition.Y - LastMousePos.Y) * lookSensitivity * (float)t;
                                LastMousePos = MousePosition;

                                _camera.ModifyDirection(xOffset, yOffset);
                        }
                        else
                        {
                                // Reset the flag when right mouse button is released
                                isFirstRightMousePress = true;
                        }

                        if (IsMousePressed(MouseButton.Left))
                        {
                                //spawn a bullet at the camera's position and rotation 
                                Bullet bullet = new Bullet();

                                bullet.transform.Position = _camera.Position;
                                bullet.transform.Rotation = _camera.Rotation;
                                
                                //log the rotation of the bullet    
                                Log("Bullet Rotation: ");
                                
                                Instantiate(bullet);
                        }
                   

                }
        };
        static void Main()
        {
                App app = new App();
                app.Start();
        }

};