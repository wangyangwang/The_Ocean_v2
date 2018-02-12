Shader "Custom/NoiseSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,2)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
        _NoiseScale ("Noise Scale", Range(0,2)) = 0.1
        _Speed("Time Speed", Range(0.5, 10)) = 0.5
        _VertexDistortionScale("Vertex Distortion Scale", Range(0.0, 10.0)) = 0.2

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows finalcolor:mycolor vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
        #include "noiseSimplex.cginc"

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
            float vNoiseValue;
           
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
        float _NoiseScale;
        float _Speed;
        float _VertexDistortionScale;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END
        
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

          float Epsilon = 1e-10;
 
          float3 RGBtoHCV(in float3 RGB)
          {
            // Based on work by Sam Hocevar and Emil Persson
            float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
            float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
            float C = Q.x - min(Q.w, Q.y);
            float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
            return float3(H, C, Q.x);
          }



          void vert (inout appdata_full v) {
            v.vertex += float4(v.normal * snoise( float4(v.vertex.xyz , _Time.x) ) * _VertexDistortionScale, 0.0);
          }


            void mycolor (Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
                //IN.uv_MainTex *= float2(sin(_Time.x/20)*10,cos(_Time.y/20)*10);
                float noiseValue = snoise(float3(IN.uv_MainTex*_NoiseScale, _Time.x * _Speed));
                float mappedValue = noiseValue * 0.4 + 0.5;

         
                float3 finalCol = HSVtoRGB(float3(mappedValue, 1, 1));
                fixed4 col = fixed4(finalCol, 1.0);

           
            
                color = col;
        }


		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo =  c.rgb;
         
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}

      

      

     

		ENDCG
	}
	FallBack "Diffuse"
}
