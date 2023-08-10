using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusEngine
{
    //handle the transform of an object
    class Transform2D : Component
    {
        //position
        public Vector2f position = new Vector2f(0, 0);
        //size
        public Vector2f size = new Vector2f(0, 0);
        //rotation
        public float fRotation;
        //origin
        public Vector2f Origin;

        public override void OnStart()
        {
            Origin = new Vector2f(size.X / 2, size.Y / 2);
        }
    }
}
