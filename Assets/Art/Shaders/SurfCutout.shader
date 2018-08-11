
Shader "BoidFlockSimple" { // StructuredBuffer + SurfaceShader

   Properties {
		_Color1 ("Color1", Color) = (1,1,1,1)
		_Color2 ("Color1", Color) = (1,1,1,1)
		_Emission ("Emission", Range(0,1)) = 0.0
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_MetallicGlossMap("Metallic", 2D) = "white" {}
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Glossiness ("Smoothness", Range(0,1)) = 1.0
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}

   SubShader {
 
		CGPROGRAM

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _MetallicGlossMap;
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 worldPos;
		};
		half _Glossiness;
		half _Emission;
		half _Metallic;
		
		fixed4 _Color1;
		fixed4 _Color2;

 
        #pragma surface surf Standard vertex:vert BlinnPhong addshadow nolightmap alphatest:_Cutoff
        #pragma instancing_options procedural:setup

        float4x4 _LookAtMatrix;
        float3 _BoidPosition;
        float _mixAmount;

         #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

            StructuredBuffer<float3> positionBuffer; 
            StructuredBuffer<float> lifeTimeBuffer; 
            StructuredBuffer<float4> rotationBuffer; 
            float maxLifeTime;
            float scale;

         #endif

        float4x4 look_at_matrix(float3 at, float3 eye, float3 up) {
            float3 zaxis = normalize(at - eye);
            float3 xaxis = normalize(cross(up, zaxis));
            float3 yaxis = cross(zaxis, xaxis);
            return float4x4(
                xaxis.x, yaxis.x, zaxis.x, 0,
                xaxis.y, yaxis.y, zaxis.y, 0,
                xaxis.z, yaxis.z, zaxis.z, 0,
                0, 0, 0, 1
            );
        }
        
        void setup()
        {
            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                _BoidPosition = positionBuffer[unity_InstanceID];
                _mixAmount = max(lifeTimeBuffer[unity_InstanceID] / maxLifeTime, 0);
                _LookAtMatrix = look_at_matrix(_BoidPosition, _BoidPosition + (rotationBuffer[unity_InstanceID].xyz * -1), float3(0.0, 1.0, 0.0));
            #endif
        }
     
         void vert(inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);

            #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                v.vertex = mul(_LookAtMatrix, v.vertex);
                v.vertex.xyz *= scale;
                v.vertex.xyz += _BoidPosition;
            #endif
        }
 
         void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * lerp(_Color1, _Color2, _mixAmount);
			fixed4 m = tex2D (_MetallicGlossMap, IN.uv_MainTex); 
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			o.Metallic = _Metallic * m.r;
			o.Emission = _Emission * c.rgb * lerp(_Color1, _Color2, _mixAmount);
			o.Smoothness = _Glossiness * m.a;
         }
 
         ENDCG
   }
}