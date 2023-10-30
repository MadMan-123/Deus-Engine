#version 330 core
out vec4 FragColor;

uniform vec3 objectColor;
uniform vec3 lightColor;

in vec2 fUv;
uniform sampler2D uTexture;

void main()
{
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;
    vec3 result = ambient * objectColor;

    // Sample the original color
    vec4 texColor = texture(uTexture, fUv);

    FragColor = vec4(texColor.rgb * result, texColor.a);
}
