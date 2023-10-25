#version 330 core

in vec2 fUv;
uniform sampler2D uTexture;
out vec4 FragColor;

void main() {
    // Sample the original color
    vec4 texColor = texture(uTexture, fUv);

    // Apply color quantization to reduce color bit depth
    texColor.rgb = floor(texColor.rgb * 8.0) / 8.0; // Reduces to 2 bits per channel

    //apply dithering
    float ditherAmount = 0.05f;
    float noise = fract(sin(dot(fUv.xy, vec2(12.9898, 78.233))) * 43758.5453);
    texColor.rgb += noise * ditherAmount;
    
    

    // Add a vignette effect
    vec2 fromCenter = fUv - vec2(0.5, 0.5);
    float vignetteAmount = 0.3;
    float vignette = 1.0 + vignetteAmount * dot(fromCenter, fromCenter);
    texColor.rgb *= vignette;

    FragColor = texColor;
}
