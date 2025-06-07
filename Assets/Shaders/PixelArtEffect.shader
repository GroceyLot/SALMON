Shader "Custom/PixelArtEffect"
{
    Properties
    {
        _MainTex    ("Main Texture",    2D)   = "white" {}
        _PixelSize  ("Pixel Size",      Float) = 4.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "PixelArt"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Main color, depth & normals
            TEXTURE2D(_MainTex);              SAMPLER(sampler_MainTex);
            TEXTURE2D(_CameraDepthTexture);   SAMPLER(sampler_CameraDepthTexture);
            TEXTURE2D(_CameraNormalsTexture); SAMPLER(sampler_CameraNormalsTexture);

            float _PixelSize;

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };
            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv          = IN.uv;
                return OUT;
            }

            // Sobel on normals (rgb vector)
            float SobelNormals(float2 uv)
            {
                float2 ts = 1.0 / _ScreenParams.xy;
                float3 s[9];
                int k = 0;
                for (int y = -1; y <= 1; y++)
                    for (int x = -1; x <= 1; x++) {
                        float4 n = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv + float2(x,y)*ts);
                        s[k++]  = n.rgb;
                    }
                float3 gx = s[2] + 2*s[5] + s[8] - (s[0] + 2*s[3] + s[6]);
                float3 gy = s[0] + 2*s[1] + s[2] - (s[6] + 2*s[7] + s[8]);
                return length(gx) + length(gy);
            }

            // Sobel on depth (scalars)
            float SobelDepth(float2 uv)
            {
                float2 ts = 1.0 / _ScreenParams.xy;
                float d[9];
                int k = 0;
                for (int y = -1; y <= 1; y++)
                    for (int x = -1; x <= 1; x++)
                        d[k++] = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(x,y)*ts).r;

                float gx =  d[2] + 2*d[5] + d[8]
                          - (d[0] + 2*d[3] + d[6]);
                float gy =  d[0] + 2*d[1] + d[2]
                          - (d[6] + 2*d[7] + d[8]);
                return abs(gx) + abs(gy);
            }

            float2 SnapUV(float2 uv)
            {
                return floor(uv * _ScreenParams.xy / _PixelSize) * (_PixelSize / _ScreenParams.xy);
            }

            float Dither4x4(int2 pos)
            {
                static const float d[16] = {
                     0,  8,  2, 10,
                    12,  4, 14,  6,
                     3, 11,  1,  9,
                    15,  7, 13,  5
                };
                int idx = (pos.y & 3)*4 + (pos.x & 3);
                return d[idx] / 16.0;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv  = IN.uv;
                float2 pUV = SnapUV(uv);

                float eN = SobelNormals(pUV);
                float eD = SobelDepth(pUV);
                float edge = saturate(eN + eD);

                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, pUV);

                int2 pixPos = int2(uv * _ScreenParams.xy);
                float dither = Dither4x4(pixPos);
                col.rgb = step(dither, col.rgb);

                col.rgb = lerp(col.rgb, 0, edge);
                return col;
            }
            ENDHLSL
        }
    }
}
