Shader "Unlit/Hamon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Power ("Power",Range(0,100)) = 0
        _Period("Period",Range(0,100))=0
        _Speed("Speed",Range(0,1000))=0
        _Detail("Detail",Range(0,100))=0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"
                "Queue"="Transparent" }
        LOD 100
        GrabPass
        {
            "_BackTexture"
        }
        Cull off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            static const float2 center = float2(0.5,0.5);
            static const float PI = 3.14159265f;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BackTexture;
            float _Power;
            float _Period;
            float _Speed;
            float _Detail;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.screenPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            half2 ReturnUV(half2 uv,half2 screenUV)
            {
                //中心座標から現在処理中のuv座標へのベクトル
                float2 c2n = uv - center;
                //c2nの長さ
                float length_of_c2n = sqrt(c2n.x*c2n.x + c2n.y*c2n.y);
                //c2nと平行な単位ベクトル
                float c2n_unit = c2n / length_of_c2n;

                //波
                float nami = (sin(((length_of_c2n * 10 *_Period - _Time*_Speed)%(2*PI))/(length_of_c2n*_Detail))/100 * _Power) * (0.2 - length_of_c2n);

                if(length_of_c2n < 0.2){
                screenUV += c2n_unit * nami;
                screenUV += c2n_unit * nami;
                }
                return screenUV;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half2 grabUV = i.screenPos.xy / i.screenPos.w;
                // sample the texture
                half2 mainUV = ReturnUV(i.uv,grabUV);
                fixed4 col = tex2D(_BackTexture, mainUV);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}	