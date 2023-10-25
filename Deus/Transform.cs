using System.Numerics;
using Silk.NET.Maths;

namespace DeusEngine
{
    // Handle the transform of an object
    public class Transform : Component
    {
        // Position
        public Vector3 Position = new Vector3(0, 0,0);
        public Vector3 Front
        {
            get
            {
                // Calculate the Front vector based on the current rotation
                float cosX = MathF.Cos(DMath.DegToRad(Rotation.Y)) * MathF.Cos(DMath.DegToRad(Rotation.X));
                float sinX = MathF.Sin(DMath.DegToRad(Rotation.X));
                float cosY = MathF.Cos(DMath.DegToRad(Rotation.Y));
                float sinY = MathF.Sin(DMath.DegToRad(Rotation.Y));
        
                return new Vector3(cosY * cosX, sinX, sinY * cosX);
            }
        }        
        public Vector3 Right { get; set; }
        public Vector3 Up = Vector3.UnitY;

        // Size
        public float Scale = 1f;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        //Note: The order here does matter.
        public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);

        // Origin

        public override void OnUpdate(double t)
        {

            
            //calculate the right vector
            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            
        }
    }
}