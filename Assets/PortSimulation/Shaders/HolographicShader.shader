Shader "PortSimulation/Placement/HolographicPlacement"
{
    Properties
    {
        _ValidColor ("Valid Color", Color) = (0,1,1,0.45)
        _InvalidColor ("Invalid Color", Color) = (1,0,0,0.45)
        _IsValid ("Is Valid Placement", Float) = 1

        _ScanSpeed ("Scan Speed", Float) = 2
        _ScanDensity ("Scan Density", Float) = 20
        _FresnelPower ("Fresnel Power", Range(0.1, 8)) = 4
        _GlowIntensity ("Glow Intensity", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha One
        ZWrite Off
        Cull Back

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };

            float4 _ValidColor;
            float4 _InvalidColor;
            float _IsValid;

            float _ScanSpeed;
            float _ScanDensity;
            float _FresnelPower;
            float _GlowIntensity;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);

                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.viewDirWS = normalize(GetWorldSpaceViewDir(o.positionWS));

                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // Base color switch
                half4 baseColor = lerp(_InvalidColor, _ValidColor, _IsValid);

                // Scanline effect
                float scan = sin((i.positionWS.y * _ScanDensity) + (_Time.y * _ScanSpeed));
                scan = saturate(scan);

                // Fresnel edge glow
                float fresnel = pow(1.0 - saturate(dot(i.normalWS, i.viewDirWS)), _FresnelPower);

                baseColor.rgb += (scan * 0.15 + fresnel * _GlowIntensity);
                baseColor.a *= (0.6 + scan * 0.4);

                return baseColor;
            }
            ENDHLSL
        }
    }
}
