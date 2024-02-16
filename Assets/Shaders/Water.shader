Shader "Unlit/Water"
{
    Properties
    {
        _BumpMap("Normal", 2D) = "BumpMap" {}
        _BumpMapScale("Bump Scale", Float) = 5.0
        _NormalTextureScaleX("Normal Texture Scale X", Float) = 1.0
        _NormalTextureScaleY("Normal Texture Scale Y", Float) = 1.0
        
        _ScrollSpeedX("Wave Speed X", Float) = 1.0
        _ScrollSpeedY("Wave Speed Y", Float) = 1.0
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
            
            
            struct Varying
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;

                float2 uv : TEXCOORD0;
            };

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 tangent : TANGENT;
                float3 WorldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 pos : TEXCOORD2;

                float2 uv : TEXCOORD3;
                float3 normal : TEXCOORD4;
            };

            CBUFFER_START(UnityPerMaterial)
            
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            float4 _Color;
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            float _BumpMapScale;
            float _NormalTextureScaleX;
            float _NormalTextureScaleY;
            
            CBUFFER_END

            Attributes vert(Varying v)
            {
                Attributes o = (Attributes)0;
                float3x3 tangentWorld = CreateTangentToWorld(v.normal, v.tangent, 1.0);
                o.tangent = TransformTangentToWorld(v.tangent, tangentWorld, true);
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.worldNormal = TransformObjectToWorldNormal(v.normal, true);
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }

            half4 frag(Attributes i) : SV_Target
            {
                const float2 scroll = float2(_ScrollSpeedX, _ScrollSpeedY) * _Time.xy;
                
                //ノーマルマップ・テストでオブジェクトのUVを渡す。
                i.uv *= float2(_NormalTextureScaleX, _NormalTextureScaleY);
                float3 localNormal = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv + scroll), _BumpMapScale);
                const float3 binormal = cross(localNormal, i.tangent.xyz) * 1;
                i.worldNormal = (i.tangent * localNormal.x
                        + binormal * localNormal.y
                        + i.worldNormal * localNormal.z);

                const half3 worldViewDir = normalize(_WorldSpaceCameraPos - i.WorldPos);
                const half3 reflectDir = normalize(reflect(-worldViewDir, i.worldNormal));

                half4 envCol = SAMPLE_TEXTURECUBE(unity_SpecCube0, samplerunity_SpecCube0, reflectDir);
                envCol.rgb = DecodeHDREnvironment(envCol, unity_SpecCube0_HDR);
                
                float F0 = 0.02;
                // フレネル反射率を反映
                const half vdotn = dot(worldViewDir, i.worldNormal);
                const half fresnel = F0 + (1 - F0) * pow(1 - vdotn, 5);
                envCol.a = fresnel;
                
                return envCol;
            }
            ENDHLSL
        }
    }
}
