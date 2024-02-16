Shader "Unlit/MinimapIcon"
{
    Properties
    {
        _MainTex("Minimap Icon", 2D) = "white" {}
        _Color("Color", Color) = (0, 1.0, 0)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
            
            TEXTURE2D(_MainTex);
            half3 _Color;
            
            CBUFFER_END

            
            v2f vert (appdata v)
            {
                v2f o = (v2f) 0;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                col.rgb += _Color;
                
                return col;
            }
            ENDHLSL
        }
    }
}
