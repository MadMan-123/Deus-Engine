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

    // Width and height of the window
    public uint iWidth = 400, iHeight = 400;
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
        options.Size = new Vector2D<int>(800, 600);

        window = Window.Create(options);
                
        Application.Instance.SubscribeEvents();
        window.Render += OnRender;
        window.Closing += OnClose;
        window.Load += OnLoad;

        
        _renderables = new List<Renderable>();
        
        

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
        Gl.Clear((uint) ClearBufferMask.ColorBufferBit);
        
    }

    void OnClose()
    {

        for (int i = 0; i < _renderables.Count; i++)
        {
            _renderables[i].OnDestroy();
        }
    }
}