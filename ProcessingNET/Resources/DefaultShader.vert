#version 460
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aTexcoord;

uniform mat4 projectionMatrix;
  
out vec3 color;
out vec2 texcoord;

void main()
{
    gl_Position = projectionMatrix * vec4(aPos, 1.0);
    color = aColor;
    texcoord = aTexcoord;
}  
