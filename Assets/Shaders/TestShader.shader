Shader "Unlit/URP-Smallest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AmbientLight ("Ambient Light", Color) = (0.5,0.5,0.5,1)
        _RimColor ("RimColor", Color) = (0.5, 0.5, 0.5)
        _RimPower ("RimPower", float) = 0.0
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
            // make fog work
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            //頂点シェーダーの引数に使う構造体
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            //頂点シェーダーの返り値兼、ピクセルシェーダーの引数に使う構造体
            struct v2f
            {
                float2 uv : TEXCOORD0;       //：のあとはセマンティクス。
                float fogFactor : TEXCOORD1; // 3Dモデルのどのデータを使用するか
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 normalDir : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial);
            float4 _MainTex_ST;
            float3 _AmbientLight;
            float _RimPower;
            float3 _RimColor;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.fogFactor = ComputeFogFactor(o.vertex.z);
                o.normal = TransformObjectToWorldNormal(v.normal);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                //// Diffuseシェーダーのアルゴリズムを書く
                Light light = GetMainLight();

                // ピクセルの法線とライトの方向の内積を計算する。
                float t = dot(i.normal, light.direction);
                // 内積の値を0以上の値にする
                t = max(0, t);
                // 拡散反射光を計算する
                const float3 diffuseLight = light.color * t;

                // 拡散反射光を反映
                col.rgb *= diffuseLight + _AmbientLight;

                //// RimLightのシェーダーのアルゴリズムを書く
                float rim = 1.0 - abs(dot(i.vertex, i.normalDir));
                float3 emission = _RimColor.rbg * pow(rim, _RimPower) * _RimPower;
                col.rbg += emission;
                
                // apply fog
                col.rbg = MixFog(col.rgb, i.fogFactor);
                return col;
            }
            ENDHLSL
        }
    }
}
