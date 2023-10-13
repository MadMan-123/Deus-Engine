
using System.Numerics;
using DeusEngine;
using static DeusEngine.RenderingEngine;


class Program
{
        class EntityOBJ : Entity
        {
                private Renderable _renderable;
                private Camera _camera;

                public EntityOBJ()
                {
                        _renderable = AddComponent<Renderable>();
                         _camera = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY, window.Size.X / window.Size.Y);
                }
        }
        class App : Application
        {
                private EntityOBJ[] entitys = new EntityOBJ[1];

                public override void OnInit()
                {
                        for (int i = 0; i < entitys.Length; i++)
                        {
                                entitys[i] = new EntityOBJ();
                                Instantiate(entitys[i]);
                        }
                }
        };
        static void Main()
        {
                App app = new App();
                app.Start();
        }

};