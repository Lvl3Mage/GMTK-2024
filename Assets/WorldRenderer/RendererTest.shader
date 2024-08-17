Shader "Unlit/RendererTest"
{
    Properties
    {
        _Textures ("Textures", 2DArray) = "" {}
        _SliceRange ("Slices", Range(0,16)) = 6
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SliceRange;
            UNITY_DECLARE_TEX2DARRAY(_Textures);
            
            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                // sample the texture
                
                float4 color = UNITY_SAMPLE_TEX2DARRAY(_Textures, float3(i.uv,_SliceRange));
                // fixed4 col = tex2D(_MainTex, i.uv);
                return color;
            }
            ENDCG
        }
    }
}
