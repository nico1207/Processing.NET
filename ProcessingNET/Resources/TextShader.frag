#version 460
out vec4 FragColor;  
in vec2 texcoord;

layout(binding = 0) uniform sampler2D fontAtlas;
uniform vec3 textColor;

void main()
{
    FragColor = vec4(textColor, texture(fontAtlas, texcoord).a);
}
