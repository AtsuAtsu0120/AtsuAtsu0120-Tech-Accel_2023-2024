Shader "Unlit/MinimapObject"
{
    Properties
    {
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags 
        {
             "RenderType"="Opaque" 
             "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Varying
            {
                float4 position : POSITION;
            };

            struct Attributes
            {
                float4 positionHCS : POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
            
            half4 _Color;

            CBUFFER_END

            Attributes vert(Varying IN)
            {
                Attributes o = (Attributes)0;
                o.positionHCS = TransformObjectToHClip(IN.position.xyz);
                
                return o;
            }

            half4 frag(Attributes _) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}