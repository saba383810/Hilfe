Shader "Unlit/DashLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Rate ("Rate", Range(0, 1)) = 0.5
        _DivisionNumber ("Division", Range(0, 100)) = 10
    }
    SubShader
    {
        // 破線部分は透過が必要なため Transparent と AlphaBlend を設定
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Rate;
            int _DivisionNumber;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // UVマップのX軸の位置に沿って色の表示/非表示を切り替える
                // step:第一引数より第二引数が大きい場合は 1 を小さい場合は 0 を返す
                // frac:第一引数の少数値を取得する
                return step(_Rate, frac(i.uv.x * _DivisionNumber)) * _Color;
            }
            ENDCG
        }
    }
}