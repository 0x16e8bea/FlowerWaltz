 Shader "Transparent/Cutout/lambert" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
        LOD 400
        
        CGPROGRAM
        // Physically based Standard lighting model
        //#pragma surface surf Standard addshadow fullforwardshadows
        #pragma surface surf Lambert alphatest:_Cutoff
        #pragma multi_compile_instancing
        #pragma instancing_options procedural:setup
        
        #include "Common.cginc"
        #include "Quaternion.cginc"

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<float4> positionBuffer;
        StructuredBuffer<float4> rotationBuffer;
    #endif

        void setup()
        {
        #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        
          			//float4x4 rotation = quaternion_to_matrix(nlerp(previousQuaternions[unity_InstanceID], quaternions[unity_InstanceID], blendAlpha));
			float4x4 rotation = quaternion_to_matrix(rotationBuffer[unity_InstanceID]);
			//float3 position 	= lerp(previousPositions[unity_InstanceID], positions[unity_InstanceID], blendAlpha);
			float3 position 	= positionBuffer[unity_InstanceID];
			float4x4 translation = {
				1,0,0,position.x,
				0,1,0,position.y,
				0,0,1,position.z,
				0,0,0,1
			};
			unity_ObjectToWorld = mul(translation, rotation);

			
			// inverse transform matrix
			// taken from richardkettlewell's post on
			// https://forum.unity3d.com/threads/drawmeshinstancedindirect-example-comments-and-questions.446080/

			float3x3 w2oRotation;
			w2oRotation[0] = unity_ObjectToWorld[1].yzx * unity_ObjectToWorld[2].zxy - unity_ObjectToWorld[1].zxy * unity_ObjectToWorld[2].yzx;
			w2oRotation[1] = unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[2].yzx - unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[2].zxy;
			w2oRotation[2] = unity_ObjectToWorld[0].yzx * unity_ObjectToWorld[1].zxy - unity_ObjectToWorld[0].zxy * unity_ObjectToWorld[1].yzx;

			float det = dot(unity_ObjectToWorld[0], w2oRotation[0]);

			w2oRotation = transpose(w2oRotation);

			w2oRotation *= rcp(det);

			float3 w2oPosition = mul(w2oRotation, -unity_ObjectToWorld._14_24_34);

			
			unity_WorldToObject._11_21_31_41 = float4(w2oRotation._11_21_31, 0.0f);
			unity_WorldToObject._12_22_32_42 = float4(w2oRotation._12_22_32, 0.0f);
			unity_WorldToObject._13_23_33_43 = float4(w2oRotation._13_23_33, 0.0f);
			unity_WorldToObject._14_24_34_44 = float4(w2oPosition, 1.0f);

        #endif
        }

        void surf (Input IN, inout SurfaceOutput o) {
        
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb * _Color;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}