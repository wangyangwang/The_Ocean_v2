// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "gradient02"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _FogStart("Fog Start", Float) = 0.0
        _FogEnd("Fog End", Float) = 0.0
        _FogColor("Fog Color", Color) =  (0.5,0.5,0.0,0.0)
         _VertexDistortionScale("Vertex Distortion Scale", Range(0.0, 10.0)) = 0.2
         _DeepColor("Deep Color", Color) = (0,0,0,1)
         _ShadowColor("Shadow Color", Color) = (0,0,0,1)
         _ShadowFactor("Shadow Factor", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members vertexNoiseValue)
#pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma target 3.0
            
            #include "UnityCG.cginc"
            #include "noiseSimplex.cginc"
            #include "UnityLightingCommon.cginc" // for _LightColor0

            

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                half3 worldRefl : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float vertexNoiseValue : COLOR;
                float nl : FLOAT;
     
            };
            
            sampler2D _NoiseTex;

            float _FogStart, _FogEnd;
            fixed4 _FogColor;
            float _VertexDistortionScale;
            fixed4 _DeepColor;
            fixed4 _ShadowColor;
            float _ShadowFactor;
            
            
            v2f vert (appdata v)
            {
                v2f o;
                float vertexNoiseValue = snoise( float4(v.vertex.xyz , _Time.x * 3) );
                v.vertex +=  float4(v.normal * vertexNoiseValue * _VertexDistortionScale, 0.0);
                o.vertex = UnityObjectToClipPos(v.vertex);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.nl = nl;
                o.uv = v.uv;
                o.normal = v.normal;
                o.vertexNoiseValue = vertexNoiseValue;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {

               // sample the default reflection cubemap, using the reflection vector
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.worldRefl);
                // decode cubemap data into actual color
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);


                fixed4 col = fixed4(lerp(i.normal, fixed3(1.0,1.0,1.0), i.vertex.z-1) ,1.0);
       
                col = lerp( lerp(col, _DeepColor, _VertexDistortionScale), col, i.vertexNoiseValue);
                col = lerp(_ShadowColor, col, i.nl+(1-_ShadowFactor));
              
                //col = fixed4(i.nl,i.nl,i.nl,1.0);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }

           


            ENDCG
        }
    
    }
}
