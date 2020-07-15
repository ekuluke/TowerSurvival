// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Outline2D"
{
    Properties{
            _MainTex("Image", 2D) = "white" {}
            _OutlineC("Outline Color", Color) = (0,0,0,1)
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                half4 _OutlineC;

                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                v2f vert(appdata_base v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    half4 c = tex2D(_MainTex, i.uv);
                    if (i.uv.x <= 0.0f || i.uv.y <= 0.0f || i.uv.x >= 1.0f || i.uv.y >= 1.0f)
                        c = _OutlineC;
                    return c;
                }
                ENDCG
            }
    }
        Fallback "Legacy/Diffuse"
}