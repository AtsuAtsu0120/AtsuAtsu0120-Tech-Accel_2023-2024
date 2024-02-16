Shader "Custom/ConfirmSlider"
{
    Properties
    {
        _MainTex("Minimap Icon", 2D) = "white" {}
        _SliderValue("Slider Value", Range(0, 1)) = 0
        _ColorA("Normal Color", Color) = (0.0, 1.0, 0, 1.0)
        _ColorB("Confirm Color", Color) = (1.0, 0, 0, 1)
        
        _GradientSpeed("Gradient Speed", Float) = 1.2
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue" = "Transparent"
            "RenderPipeline"="Universal"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Varying
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct Attributes
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _SliderValue;
            float _GradientSpeed;
            half4 _ColorA;
            half4 _ColorB;
            
            Attributes vert (Varying v)
            {
                Attributes o = (Attributes) 0;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag (Attributes i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                float4 endCol = lerp(_ColorA, _ColorB, _SliderValue);
                float3 gradation = lerp(_ColorA, endCol, pow(i.uv.x, _GradientSpeed));

                col.xyz *= gradation;
                return col;
            }
            ENDHLSL
        }
    }
}