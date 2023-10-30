namespace DeusEngine;

public class Light : Component
{
    private Shader lightingShader; // You should define your lighting shader here

    public override void OnStart()
    {
        lightingShader =  new Shader(
            @Application.sAssetsPath+"shader.vert", 
            @Application.sAssetsPath+"Lighting.vert");
    }

    public void ApplyLighting()
    {
        // Apply lighting calculations here using the lightingShader
        // You can set uniforms for light position, intensity, etc.
    }

    public override void OnDestroy()
    {
        lightingShader.Dispose();
    }
}