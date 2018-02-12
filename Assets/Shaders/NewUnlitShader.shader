Shader "Unlit/NewUnlitShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseScale("noise scale", Range(0.0, 1.0)) = 0.0
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal: NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _NoiseScale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
               
				//o.vertex = float4(o.vertex.xy, snoise(float4(o.vertex.xyz, _Time.x)), 1);
		        //o.vertex = UnityObjectToClipPos(o.vertex);
            	//o.vertex.y += snoise(float4(o.vertex.xyz / 7, _Time.x/10) * 10);
                //o.vertex += float4(v.normal * snoise(float4(o.vertex.xyz * 10, _Time.y)) * 2, 1);

				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			float3 HUEtoRGB(in float H)
			  {
			    float R = abs(H * 6 - 3) - 1;
			    float G = 2 - abs(H * 6 - 2);
			    float B = 2 - abs(H * 6 - 4);
			    return saturate(float3(R,G,B));
			  }


			 float3 HSVtoRGB(in float3 HSV)
			  {
			    float3 RGB = HUEtoRGB(HSV.x);
			    return ((RGB - 1) * HSV.y + 1) * HSV.z;
			  }

			fixed4 frag (v2f i) : SV_Target
			{
				float noiseValue = snoise(float3(i.uv*_NoiseScale, _Time.x));
				float mappedValue = noiseValue * 0.4 + 0.6;

				// sample the texture
				float3 finalCol = HSVtoRGB(float3(mappedValue, 1.0, 1));
				fixed4 col = fixed4(finalCol, 1.0);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}


			ENDCG
		}
	}
}
