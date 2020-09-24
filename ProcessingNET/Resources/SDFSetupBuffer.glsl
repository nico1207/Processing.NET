#version 460
layout(local_size_x = 1) in;
layout(std430, binding = 1) buffer inputData {
	uint colors[];
};
layout(std430, binding = 2) buffer outputData {
	vec4 positions[];
};
uniform int imageWidth;

void main() {
	uint pos = gl_GlobalInvocationID.x;
	uint x = pos % imageWidth;
	uint y = pos / imageWidth;
	uint color = colors[pos];
	uint alpha = color & 0xFF000000;
	if (alpha > 0) {
		positions[pos] = vec4(x, y, 0, 0);
	}
	else {
		positions[pos] = vec4(0, 0, x, y);
	}
	
}