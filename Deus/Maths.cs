

using Silk.NET.Maths;

namespace DeusEngine
{
    #region Maths
    //maths class for specific functions
    public static class DMath
    {
        static public float PI = 3.1415926535f;
        static public Vector2D<float> VectorZero = new Vector2D<float>(0,0);
        //convert degrees to radians
        static public float DegToRad(float degrees) => degrees * (PI / 180);
        static public float DegToRad(Double degrees) => (float)(degrees * (PI / 180));
        //convert radians to degrees
        static public float RadToDeg(float radians) => radians * (180 / PI);
        //normalize a vector
        static public Vector2D<float> Normalize(Vector2D<float> input)
        {
            float fLength = MathF.Sqrt(input.X * input.X + input.Y * input.Y);

            return input / fLength;

        }
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            else if (value.CompareTo(max) > 0)
            {
                return max;
            }
            else
            {
                return value;
            }
        }
        public static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static Vector2D<float> Clamp(Vector2D<float> value, Vector2D<float> min, Vector2D<float> max)
        {
            float clampedX = Clamp(value.X, min.X, max.X);
            float clampedY = Clamp(value.Y, min.Y, max.Y);
            return new Vector2D<float>(clampedX, clampedY);
        }

        //float lerp 
        static public float Lerp(float a, float b, float T)
        {
            return (1 - T) * a + b * T;
        }

        //vector lerp
        static public Vector2D<float> Lerp(Vector2D<float> a, Vector2D<float> b, float T)
        {
            return (1 - T) * a + b * T;
        }

        //convert a integer based vector to a float based vector
        static public Vector2D<int> V2FtoV2I(Vector2D<float> input) => new Vector2D<int>((int)input.X, (int)input.Y);


        //Pythagoras c^2=a^2+b^2
        static public float Distance(Vector2D<float> a, Vector2D<float> b)
        {
            float A = b.X - a.X;
            float B = b.Y - a.Y;
            return MathF.Sqrt(A * A + B * B);
        }

        static public float Magnitude(Vector2D<float> a)
        {
            float A = a.X;
            float B = a.Y;
            return MathF.Sqrt(A * A + B * B);
        }

        public static float DotProduct(Vector2D<float> v1, Vector2D<float> v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        static public float SquaredMagnitude(Vector2D<float> vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y;
        }


        public static Vector2D<float> RotateVector(this Vector2D<float> vector, float angleDegrees)
        {
            float angleRadians = (float)(angleDegrees * Math.PI / 180.0);
            float cosAngle = (float)Math.Cos(angleRadians);
            float sinAngle = (float)Math.Sin(angleRadians);

            float newX = vector.X * cosAngle - vector.Y * sinAngle;
            float newY = vector.X * sinAngle + vector.Y * cosAngle;

            return new Vector2D<float>(newX, newY);
        }

        public static Vector2D<float> RotationToDirection(float rotationDegrees)
        {
            // Convert the rotation from degrees to radians
            float radians = DMath.DegToRad(rotationDegrees);

            // Calculate the X and Y components of the direction vector
            float directionX = MathF.Cos(radians);
            float directionY = MathF.Sin(radians);

            // Create and return the 2D direction vector
            return new Vector2D<float>(directionX, directionY);
        }
        
        public static float AngleDifference(float angleA, float angleB)
        {
            float difference = (angleB - angleA + 180.0f) % 360.0f - 180.0f;
            if (difference < -180.0f)
                difference += 360.0f;
            return difference;
        }

    }

    #endregion
}
