#version 460
out vec4 FragColor;
in vec2 uv;

uniform sampler2D fontAtlas;
uniform float smoothing;

void main()
{
    float distance = texture(fontAtlas, uv).r;
    float alpha = smoothstep(0.5 - smoothing, 0.5 + smoothing, distance);
    FragColor = vec4(1.0, 1.0, 1.0, alpha);
}
 