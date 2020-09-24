#version 460
layout(location = 0) in vec3 position;
layout(location = 1) in vec2 texcoord;

uniform mat4 projection;

out vec2 uv;

void main()
{
    gl_Position = projection * vec4(position, 1.0);
    uv = texcoord;
}
