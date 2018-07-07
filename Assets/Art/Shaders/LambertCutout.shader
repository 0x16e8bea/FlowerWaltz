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

        sampler2D _MainTex;
        fixed4 _Color;

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
            float4 data = positionBuffer[unity_InstanceID];
            float4 rot = rotationBuffer[unity_InstanceID];

            unity_ObjectToWorld._11_21_31_41 = float4(cos(rot.y) * cos(rot.z), -cos(rot.y) * sin(rot.z), sin(rot.y), 0);
            unity_ObjectToWorld._12_22_32_42 = float4(sin(rot.x) * sin(rot.y) *cos(rot.z) + cos(rot.x)*sin(rot.z), -sin(rot.x)*sin(rot.y)*sin(rot.z)+cos(rot.x)*cos(rot.z), -sin(rot.x)*cos(rot.y), 0);
            unity_ObjectToWorld._13_23_33_43 = float4(-cos(rot.x)*sin(rot.y)*cos(rot.z)+sin(rot.x)*sin(rot.z), cos(rot.x)*sin(rot.y)*sin(rot.z) + sin(rot.x) * cos(rot.z),  cos(rot.x) * cos(rot.y), 0);


            //unity_ObjectToWorld._11_21_31_41 = float4(1, 0, 0, 0);
            //unity_ObjectToWorld._12_22_32_42 = float4(0, 1, 0, 0);
            //unity_ObjectToWorld._13_23_33_43 = float4(0, 0, 1, 0);
            unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);
            unity_WorldToObject = unity_ObjectToWorld;
            unity_WorldToObject._14_24_34 *= -1;
            unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
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