using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusEngine
{
    #region Maths
    //maths class for specific functions
    public static class DMath
    {
        static public float PI = 3.1415926535f;
        //normalize a vector
        static public Vector2f Normalize(Vector2f input)
        {
            float fLength = MathF.Sqrt(input.X * input.X + input.Y * input.Y);

            return input / fLength;

        }

        //float lerp 
        static public float Lerp(float a, float b, float T)
        {
            return (1 - T) * a + b * T;
        }

        //vector lerp
        static public Vector2f Lerp(Vector2f a, Vector2f b, float T)
        {
            return (1 - T) * a + b * T;
        }

        //convert a integer based vector to a float based vector
        static public Vector2i V2FtoV2I(Vector2f input) => new Vector2i((int)input.X, (int)input.Y);


        //Pythagoras c^2=a^2+b^2
        static public float Distance(Vector2f a, Vector2f b)
        {
            float A = b.X - a.X;
            float B = b.Y - a.Y;
            return MathF.Sqrt(A * A + B * B);
        }

        static public float Magnitude(Vector2f a)
        {
            float A = a.X;
            float B = a.Y;
            return MathF.Sqrt(A * A + B * B);
        }

    }

    #endregion
}
