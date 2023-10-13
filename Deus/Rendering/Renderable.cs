using Silk.NET;
using Silk.NET.OpenGL;

namespace DeusEngine
{
    //to render on screen 
    public unsafe class Renderable : Component
    {
        private VertexArrayObject<float, uint> _vao;
        private BufferObject<float> _vbo;
        private BufferObject<uint> _ebo;

        private Texture _texture;
        private Shader _shader;
        
        //Vertex data, uploaded to the VBO.
        private  float[] Vertices =
        {
            //X    Y      Z     U   V
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };




        //Index data, uploaded to the EBO.
        private uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public override void OnLoad()
        {
            _ebo = new BufferObject<uint>(Indices, BufferTargetARB.ElementArrayBuffer);
            _vbo = new BufferObject<float>(Vertices, BufferTargetARB.ArrayBuffer);
            _vao = new VertexArrayObject<float, uint>(_vbo,_ebo);
            
            //Telling the VAO object how to lay out the attribute pointers
            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);
            
            _shader = new Shader( "F:\\Dev\\Deus-Engine\\Deus\\Rendering\\shader.vert", "F:\\Dev\\Deus-Engine\\Deus\\Rendering\\shader.frag");
            
            _texture = new Texture( "F:\\Dev\\Deus-Engine\\Deus\\Rendering\\Marat - Copy.jpg");
            
            
        }

        public virtual void OnRender(double t)
        {
            //bind the geometry
            _vao.Bind();
            //bind the shader
            _shader.Use();
            
            _texture.Bind(TextureUnit.Texture0);
            
            //Setting a uniform.
            _shader.SetUniform("uTexture", 0);

            _shader.SetUniform("uModel", transform.ViewMatrix);
            //draw the geometry
            RenderingEngine.Gl.DrawElements(
                PrimitiveType.Triangles,
                (uint)Indices.Length,
                DrawElementsType.UnsignedInt,
                null);
        }

        public override void OnDestroy()
        {
            _vao.Dispose();
            _vbo.Dispose();
            _vao.Dispose();
            _shader.Dispose();
            _texture.Dispose();
        }

    }

    
}
