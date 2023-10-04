using SFML.System;

namespace DeusEngine
{
    // Handle the transform of an object
    public class Transform2D : Component
    {
        // Position
        private Vector2f _position = new Vector2f(0, 0);

        // Size
        public Vector2f size = new Vector2f(0, 0);

        // Rotation
        public float fRotation;

        // Origin
        public Vector2f Origin = new Vector2f();

        public Vector2f Forward = new Vector2f();
        public Vector2f Left = new Vector2f();
        public Vector2f Right = new Vector2f();

        // Public property to get or set the position while considering the origin
        public Vector2f position
        {
            get { return _position + Origin; }
            set { _position = value - Origin; }
        }

        public override void OnStart()
        {
            Origin = size / 2;
        }

        public override void OnUpdate()
        {
            float fRadians = DMath.DegToRad(fRotation);
            Forward.X = MathF.Cos(fRadians);
            Forward.Y = MathF.Sin(fRadians);

            Right = DMath.RotationToDirection(fRotation + 90);
            Left = DMath.RotationToDirection(fRotation - 90);
        }
    }
}