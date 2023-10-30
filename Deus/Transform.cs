using System.Numerics;
using Silk.NET.Maths;

namespace DeusEngine
{
    // Handle the transform of an object
    public class Transform : Component
    {
        // Position
        public Vector3 Position = new Vector3(0, 0,0);
        public Vector3 Forward
        {
            get
            {
                return new Vector3(Rotation.X, Rotation.Y, Rotation.Z);
            }
        }        
        public Vector3 Right {
            get
            {
                //calculate the right vector
                return (Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY)));
            }
            
        }

        public Vector3 Up
        {
            get
            {
                return Vector3.Cross(Forward, Right);

            }
        }

        // Size
        public float Scale = 1f;

        public Quaternion Rotation { get; set; } = Quaternion.CreateFromYawPitchRoll(0, 0, 0);
            


        //Note: The order here does matter.
        public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);

    }
}