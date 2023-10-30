#version 330 core

in vec2 fUv;
uniform sampler2D uTexture;
out vec4 FragColor;

void main() 
{
    // Sample the original color
    vec4 texColor = texture(uTexture, fUv);
    FragColor = texColor;
}
