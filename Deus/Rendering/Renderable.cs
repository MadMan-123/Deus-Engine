using System.Numerics;
using Silk.NET;
using Silk.NET.OpenGL;

//define a struct for the a cube
public struct CubeMesh
{
    public float[] Vertices = 
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
    public uint[] Indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    public CubeMesh()
    {
    }
}

struct Square
{
    //Vertex data, uploaded to the VBO.
       
    private float[] Vertices =
    {
        //X    Y      Z     U   V
        0.5f,  0.5f, 0.0f, 1f, 0f,
        0.5f, -0.5f, 0.0f, 1f, 1f,
        -0.5f, -0.5f, 0.0f, 0f, 1f,
        -0.5f,  0.5f, 0.5f, 0f, 0f
    };
        
    //Index data, uploaded to the EBO.
    private uint[] Indices =
    {
        0, 1, 3,
        1, 2, 3
    };

    public Square()
    {
    }
}

namespace DeusEngine
{
    //to render on screen 
    public class Renderable : Component
    {
        private VertexArrayObject<float, uint> _vao;
        private BufferObject<float> _vbo;
        private BufferObject<uint> _ebo;

        private Texture _texture;
        private Shader _shader;
        private CubeMesh _mesh = new CubeMesh(); // Use CubeMesh or Square

        public override void OnStart()
        {
            _ebo = new BufferObject<uint>(_mesh.Indices, BufferTargetARB.ElementArrayBuffer);
            _vbo = new BufferObject<float>(_mesh.Vertices, BufferTargetARB.ArrayBuffer);
            _vao = new VertexArrayObject<float, uint>(_vbo, _ebo);

            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

            _shader = new Shader(Application.sAssetsPath +"shader.vert", Application.sAssetsPath + "shader.frag");
            _texture = new Texture(Application.sAssetsPath + "Marat - Copy.jpg");
        }

        public void Render(Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection)
        {
            Matrix4x4 TransformModel = transform.ViewMatrix * model;

            _vao.Bind();
            _shader.Use();
            _shader.SetUniform("uTexture", 0);
            _shader.SetUniform("uModel", TransformModel);
            _shader.SetUniform("uView", view);
            _shader.SetUniform("uProjection", projection);

            _texture.Bind();
            _ebo.Bind();
            uint indicesCount = (uint)_mesh.Vertices.Length / 5;

            RenderingEngine.Gl.DrawArrays(PrimitiveType.Triangles, 0, indicesCount);
        }

        public override void OnDestroy()
        {
            _vao.Dispose();
            _vbo.Dispose();
            _ebo.Dispose();
            _shader.Dispose();
            _texture.Dispose();
        }
        
        public virtual void OnRender(double t)
        {
        }
    }


    
}
