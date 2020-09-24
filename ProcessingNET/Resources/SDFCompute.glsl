#version 460
layout(local_size_x = 1, local_size_y = 1) in;
layout(std430, binding = 2) buffer data {
	vec4 positions[];
};
uniform int imageWidth;
uniform int sampleOffset;

float distanceSq(vec2 pos1, vec2 pos2) {
	vec2 dif = pos1 - pos2;
	return dot(dif, dif);
}

void main() {
	ivec2 offsets[9] = ivec2[](ivec2(-1, -1), ivec2(0, -1), ivec2(1, -1), ivec2(-1, 0), ivec2(0, 0), ivec2(1, 0), ivec2(-1, 1), ivec2(0, 1), ivec2(1, 1));

	uint pos = gl_GlobalInvocationID.x;
	uint x = pos % imageWidth;
	uint y = pos / imageWidth;

	vec4 currentPos = positions[pos];
	vec2 myPos = vec2(x, y);
	float closestInside = 1.0 / 0.0;
	float closestOutside = 1.0 / 0.0;
	vec2 closestInsidePos = vec2(0.0, 0.0);
	vec2 closestOutsidePos = vec2(0.0, 0.0);
	for (int i = 0; i < 9; i++) {
		ivec2 offset = offsets[i];
		uint sampleX = x + offset.x * sampleOffset;
		uint sampleY = y + offset.y * sampleOffset;
		vec4 samplePosition = positions[sampleX + imageWidth * sampleY];

		if (samplePosition.x != 0 && samplePosition.y != 0) {
			vec2 insidePos = samplePosition.xy;
			float distSq = distanceSq(myPos, insidePos);
			if (distSq < closestInside) {
				closestInside = distSq;
				closestInsidePos = insidePos;
			}
		}

		if (samplePosition.z != 0 && samplePosition.w != 0) {
			vec2 outsidePos = samplePosition.zw;
			float distSq = distanceSq(myPos, outsidePos);
			if (distSq < closestOutside) {
				closestOutside = distSq;
				closestOutsidePos = outsidePos;
			}
		}
	}

	positions[pos] = vec4(closestInsidePos, closestOutsidePos);
}