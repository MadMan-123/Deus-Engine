using System.Numerics;
using Silk.NET;
using Silk.NET.OpenGL;

namespace DeusEngine
{
    //to render on screen 
    public class Renderable : Component
    {
        private Texture _texture;
        private Shader _shader;
        public Model Model;
        private GL _gl;
        public override void OnStart()
        {
            _gl = RenderingEngine.Gl;
            _shader = new Shader(Application.sAssetsPath + "shader.vert", Application.sAssetsPath + "shader.frag");
        }

        public void Render(Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection)
        {
            //do checks on the model
            if(Model == null || _texture == null)
                return;
            Matrix4x4 TransformModel = transform.ViewMatrix ;

            foreach (var mesh in Model.Meshes)
            {
                mesh.Bind();
                _shader.Use();
                _texture.Bind();
                _shader.SetUniform("uTexture", 0);
                _shader.SetUniform("uModel", TransformModel );
                _shader.SetUniform("uView", view);
                _shader.SetUniform("uProjection", projection);

                _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Vertices.Length);
            }
        }

        public void SetModel(string sFileName)
        {
            string sFullPath = Application.sAssetsPath + sFileName;

            Model = new Model(sFullPath);
        }  
        public void SetModel(Model model)
        {
            Model = model;
        }

        public void SetTexture(Texture texture)
        {
            _texture = texture;
        }
        
        public override void OnDestroy()
        {
            Model.Dispose();
            _shader.Dispose();
            _texture.Dispose();
        }
        
        public virtual void OnRender(double t)
        {
        }
    }


    
}
