using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusEngine
{
    //to render on screen 
    class Renderable : Component
    {
        //the body to render
        public RectangleShape Body = new RectangleShape();
        public Texture texture;
        public Color FillColour, OutlineColour;

        // Shader and fragment shader objects
        private Shader shader;

        public override void OnStart()
        {
            Body.FillColor = FillColour;
            Body.OutlineColor = OutlineColour;


        }

        // Set the shader for the renderable
        public void SetShader(Shader shader)
        {
            this.shader = shader;
        }


        //use only when the game is running
        public void SetFillColour(Color color)
        {
            Body.FillColor = color;
        }

        public void SetTexture(string sPathToTexture)
        {
            texture = new Texture(sPathToTexture);
            Body.Texture = texture;

        }
        //overriden function
        public override void OnUpdate()
        {
            //set the body to be the position of the parent entity 
            Body.Position = entity.transform.position;
            //set the rotation to the rotation of the parent rotation
            Body.Rotation = entity.transform.fRotation;
            //set the size to the size of the entity
            Body.Size = entity.transform.size;
            //set the origin
            Body.Origin = entity.transform.Origin;


            // Draw the body with the shader if it's set
            if (shader != null)
            {
                Application.Instance.window.Draw(Body, new RenderStates(shader));
            }
            else
            {
                Application.Instance.Draw(Body);
            }

        }
    }
}
