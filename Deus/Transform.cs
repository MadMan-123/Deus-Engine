using System.Numerics;
using Silk.NET.Maths;

namespace DeusEngine
{
    // Handle the transform of an object
    public class Transform : Component
    {
        // Position
        public Vector3 Position = new Vector3(0, 0,0);

        // Size
        public float Scale = 1f;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        //Note: The order here does matter.
        public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);

        // Origin
        



        public override void OnLoad()
        {
        }


    }
}