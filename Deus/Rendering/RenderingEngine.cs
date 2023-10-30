using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace DeusEngine;

public class RenderingEngine
{
    public static RenderingEngine Instance;
    private static List<Renderable> _renderables;
    public static IWindow window;
    //Open GL API
    public static GL Gl;
    public Vector2D<int> WindowSize = new Vector2D<int>(1920/2, 1080/2 );
    

    public static void AddRenderable(Renderable renderable)
    {
        if (!_renderables.Contains(renderable) && _renderables != null)
        {
            //subscribe function
            window.Render += renderable.OnRender;
            _renderables.Add(renderable);
        }
    }
    
    public RenderingEngine()
    {
        if (Instance == null)
            Instance = this;

        WindowOptions options = WindowOptions.Default;
        options.Size = WindowSize;
        options.VSync = false;
        window = Window.Create(options);
                
        window.Render += OnRender;
        window.Closing += Application.Instance.HandleClose;
        window.Closing += OnClose;
        window.Load += OnLoad;
        window.Resize += OnResize;

        Application.Instance.SubscribeEvents();

        _renderables = new List<Renderable>();

    }

    private void OnResize(Vector2D<int> obj)
    {
        //refresh the window to the new resolution
        Gl.Viewport(0, 0, (uint)obj.X, (uint)obj.Y);
    }

    private void OnLoad()
    {
        Gl = GL.GetApi(window);
    }
    
    public void OnStart()
    {

        window.Run();
        window.Dispose();

    }
    void OnRender(double t)
    {
        Gl.Enable(EnableCap.DepthTest);
        Gl.Clear((uint) (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

        for (int i = 0; i < _renderables.Count; i++)
        {
            
            Matrix4x4 model = Matrix4x4.CreateRotationY(
                DMath.DegToRad(t)) * Matrix4x4.CreateRotationX(DMath.DegToRad(t)
            );

            Matrix4x4 view = Camera.Main.GetViewMatrix();
            //It's super important for the width / height calculation to regard each value as a float, otherwise
            //it creates rounding errors that result in viewport distortion
            Matrix4x4 projection = Camera.Main.GetProjectionMatrix(); 
            
            _renderables[i].Render(model,view,projection);
        }
    }

    void OnClose()
    {
        for (int i = 0; i < _renderables.Count; i++)
        {
            _renderables[i].OnDestroy();
        }
    }
    
    
}