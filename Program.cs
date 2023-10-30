﻿
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
                        transform.Position += transform.Forward * fSpeed * (float)t;

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
                        
                        //set the fps
                        RenderingEngine.window.VSync = true;

                        _camera.transform.Scale = 2f;
                }

                private Vector2 LastMousePos = Vector2.Zero;
                bool isFirstRightMousePress = false;
                
                public override void OnUpdate(double t)
                {
                        entitys[0].transform.Position = new Vector3(0,MathF.Sin((float)(window.Time)) ,0);

                        if (IsKeyPressed(Key.W))
                        {
                                _camera.transform.Position +=  _camera.transform.Forward * (float)t;
                        }
                        else if (IsKeyPressed(Key.S))
                        {
                                _camera.transform.Position -= _camera.transform.Forward * (float)t;
                        }
                        if (IsKeyPressed(Key.A))
                        {
                                _camera.transform.Position += _camera.transform.Right * (float)t;

                        }
                        else if (IsKeyPressed(Key.D))
                        {
                                _camera.transform.Position -= _camera.transform.Right * (float)t;

                        }

                        if (IsMousePressed(MouseButton.Right))
                        {
                                float lookSensitivity = 10f;
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

                        //if the mouse wheel is scrolled, zoom in or out
                        if(Application.MouseScroll != 0)
                        {
                                _camera.ModifyZoom(Application.MouseScroll);
                        }
                        
                        if ( bShouldShoot && IsMousePressed(MouseButton.Left))
                        {
                                Shoot(1000);
                        }
                   

                }
                bool bShouldShoot = true;

                private Quaternion DesiredRotation = Quaternion.Identity;
                Quaternion RotationDelta = Quaternion.Identity;
                //shoot cooroutine
                async void Shoot(int iDelay)
                {
                        bShouldShoot = false;

                        // Instantiate a bullet or create it as per your engine's requirements
                        Bullet bullet = new Bullet(); 

                        // Set the bullet's position to the camera's position
                        bullet.transform.Position = _camera.transform.Position + (_camera.transform.Forward);

                        // Set the desired rotation to the camera's rotation
                        DesiredRotation = _camera.transform.Rotation;

                        // Calculate the rotation delta
                        RotationDelta = DesiredRotation * Quaternion.Inverse(bullet.transform.Rotation);

                        // Apply the rotation delta to the bullet's rotation
                        bullet.transform.Rotation = RotationDelta * bullet.transform.Rotation;

                        // Spawn the bullet or instantiate it as per your engine's requirements
                        Instantiate(bullet);

                        await Task.Delay(iDelay);
                        bShouldShoot = true;
                }

        };
        static void Main()
        {
                App app = new App();
                app.Start();
        }

};