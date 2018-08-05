//
// Noise Shader Library for Unity - https://github.com/keijiro/NoiseShader
//
// Original work (webgl-noise) Copyright (C) 2011 Ashima Arts.
// Translation and modification was made by Keijiro Takahashi.
//
// This shader is based on the webgl-noise GLSL shader. For further details
// of the original shader, please see the following description from the
// original source code.
//

//
// Description : Array and textureless GLSL 2D/3D/4D simplex
//               noise functions.
//      Author : Ian McEwan, Ashima Arts.
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
//               Distributed under the MIT License. See LICENSE file.
//               https://github.com/ashima/webgl-noise
//

float nrand(float2 uv)
{
	return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}

uint rng_state;


uint rand_xorshift()
{
	// Xorshift algorithm from George Marsaglia's paper
	rng_state ^= (rng_state << 13);
	rng_state ^= (rng_state >> 17);
	rng_state ^= (rng_state << 5);
	return rng_state;
}

// https://github.com/cabbibo/glsl-curl-noise/blob/master/curl.glsl
float3 snoiseVec3(float3 x) {

	float s = snoise(x);
	float s1 = snoise(float3(x.y - 19.1, x.z + 33.4, x.x + 47.2));
	float s2 = snoise(float3(x.z + 74.2, x.x - 124.5, x.y + 99.4));
	float3 c = float3(s, s1, s2);
	return c;
}


float3 curlNoise(float3 p) {

	const float e = .1;
	float3 dx = float3(e, 0.0, 0.0);
	float3 dy = float3(0.0, e, 0.0);
	float3 dz = float3(0.0, 0.0, e);

	float3 p_x0 = snoiseVec3(p - dx);
	float3 p_x1 = snoiseVec3(p + dx);
	float3 p_y0 = snoiseVec3(p - dy);
	float3 p_y1 = snoiseVec3(p + dy);
	float3 p_z0 = snoiseVec3(p - dz);
	float3 p_z1 = snoiseVec3(p + dz);

	float x = p_y1.z - p_y0.z - p_z1.y + p_z0.y;
	float y = p_z1.x - p_z0.x - p_x1.z + p_x0.z;
	float z = p_x1.y - p_x0.y - p_y1.x + p_y0.x;

	const float divisor = 1.0 / (2.0 * e);
	return normalize(float3(x, y, z) * divisor);

}