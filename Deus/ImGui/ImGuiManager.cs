using DeusEngine;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using Silk.NET.Windowing;
using Silk.NET.OpenGL.Extensions.ImGui;

public class ImGuiManager
{
    public static ImGuiManager Instance;
    ImGuiController controller;

    public ImGuiManager()
    {
        //singleton boilerplate
        if (Instance == null)
            Instance = this;
        
        // Initialize ImGui
        controller =
            new ImGuiController(
                RenderingEngine.Gl, 
                RenderingEngine.window,
                InputManager.Instance.CurrentInputContext);
      
    }


    
    public void OnUpdate(double t)
    {
        controller.Update((float) t);
        ImGuiNET.ImGui.ShowDemoWindow();
        // Make sure ImGui renders too!
        controller.Render();
    }

    public void Dispose()
    {
        controller?.Dispose();
    }

}