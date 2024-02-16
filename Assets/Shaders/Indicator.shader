Shader "Custom/Indicator"
{
    Properties
    {
        _MainTex("Minimap Icon", 2D) = "white" {}
        _SliderValue("Slider Value", Range(-0.1, 1)) = -0.1
        _VigilanceColor("Vigilance Color", Color) = (1.0, 1.0, 0, 1.0)
        _ConfirmColor("Confirm Color", Color) = (1.0, 0, 0, 1)
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

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Varying
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                float4 color : COLOR;
            };

            struct Attributes
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;

                float4 color : COLOR;
            };

            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
            
            TEXTURE2D(_MainTex);
            float _SliderValue;
            half4 _ConfirmColor;
            half4 _VigilanceColor;

            CBUFFER_END
            
            Attributes vert (Varying v)
            {
                Attributes o = (Attributes) 0;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half4 frag (Attributes i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                col.rgb = step(col, _SliderValue * 0.75f).xyz;
                // 指数関数的に色を変える。（小数をn乗したら小さくなるので、10倍してその後1/10してる）
                col.rgb *= lerp(_VigilanceColor, _ConfirmColor, pow(abs(_SliderValue * 10), 1.5f) / 10).xyz;
                
                return col;
            }
            ENDHLSL
        }
    }
}