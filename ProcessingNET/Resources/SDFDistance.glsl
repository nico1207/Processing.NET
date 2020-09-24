#version 460
layout(local_size_x = 1, local_size_y = 1) in;
layout(std430, binding = 1) buffer inputData {
	uint colors[];
};
layout(std430, binding = 2) buffer data {
	vec4 positions[];
};
layout(r8, binding = 0) uniform image2D outputImage;
uniform int imageWidth;
uniform int maxDistanceOutside;
uniform int maxDistanceInside;

void main() {
	uint pos = gl_GlobalInvocationID.x;
	uint x = pos % imageWidth;
	uint y = pos / imageWidth;
	uint color = colors[pos];
	uint alpha = color & 255;

	vec4 currentPos = positions[pos];
	vec2 myPos = vec2(x, y);

	float val = 0f;
	if (alpha > 0) { // Inside
		float dist = distance(currentPos.zw, myPos);
		if (dist > maxDistanceInside) {
			val = 1f;
		} else {
			val = 0.5 + ((dist-1) / (maxDistanceInside-1)) * 0.5;
		}
	} else { // Outside
		float dist = distance(currentPos.xy, myPos);
		if (dist > maxDistanceOutside) {
			val = 0f;
		}
		else {
			val = 0.5 - (dist / maxDistanceOutside) * 0.5;
		}
	}

	imageStore(outputImage, ivec2(x, y), vec4(val));
}