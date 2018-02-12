// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "KRZ" {
Properties {
//		_Color ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_DustDirection("Dust Direction", Vector) = (0.0,0.0,0.0)
		_DustColor("Dust Color", Color) = (0.0, 0.0, 0.0, 0.0)
		_DustAmount("Dust Amount", Range(0.0, 1.0)) = 0.0

		_LightCutoff ("Light Cutoff", Range(0.0, 1.0)) = 0.2
		_ColorChangeCutoff("Color Sculpt", Range(0.0, 1.0)) = 0.5
		_LightColor("Light Color Brightness", Range(0.0, 1.0)) = 1.0
		_DarkColor("Dark Color Brightness", Range(0.0, 1.0)) = 0.2

		_GradientTex ("Gradient Texture", 2D) = "white" {}
		_GradientStrength("Gradient Strength", Range(0.0, 1.0)) = 0.5
		_GradientScaler("Gradient Scale", Range(-1.5, 1.5)) = 0.1

		_Gradient1("CrossGradient 1", Color) = (1.0, 1.0, 1.0, 1.0)
		_Gradient2("CrossGradient 2", Color) = (1.0, 1.0, 1.0, 1.0)

		_CrossGradientFactor("CrossGradient Strength", Range(0.0, 1.0)) = 0.25
		_CrossGradientScaler("CrossGradient Scale", Range(-1.5, 1.5)) = 0.25

		_StairStepSize("Stair Step Repititions", Float) = 4
		_StairStepOffset("Stair Step Offset", Float) = 1

		_NoiseResolution("Noise Resolution", Float) = 50
		_NoiseGradient("Gradient Noise", Range(0, 4)) = .5
		_NoiseStairStep("StairStep Noise", Range(0, 1)) = .05

		_ModulationSpeed("Modulation TimeScale", Float) = .5
	}

	SubShader {
		Tags { "RenderType"="Opaque" }

		CGPROGRAM

		#include "fastnoise.cginc"

		//#pragma surface surf Lambert finalcolor:finalcolor vertex:vert
		#pragma surface surf KRZ addshadow fullforwardshadows finalcolor:finalcolor vertex:vert
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _Color;
		float _LightCutoff;

		float4 _FogColor;
		float _FogHeight;
		float _FogStart;
		float _FogRangeInverse;
		float _FogSaturationPercent;

		sampler2D _ColorStrip;

		fixed4 _DustDirection;
		fixed4 _DustColor;
		fixed _DustAmount;
		fixed3 _CameraPosition;
		float _ColorChangeCutoff;
		float _LightColor;
		float _DarkColor;
		float _CrossGradientFactor;

		float _CrossGradientScaler;
		float _GradientScaler;
		float _GradientStrength;
		fixed4 _Gradient1;
		fixed4 _Gradient2;

		float _StairStepSize;
		float _StairStepOffset;

		float _NoiseResolution;
		float _NoiseGradient;
		float _NoiseStairStep;

		//for time
		float _ModulationSpeed;

		sampler2D _GradientTex;

		struct SurfaceOutputKRZ {
			fixed3 Albedo;
			fixed Alpha;
			fixed3 Emission;
			fixed3 Normal;
			fixed3 Modulator;
			fixed Specular;
		};


		struct Input {
			float4 pos : TEXCOORD;
			fixed3 normal : TEXCOORD1;
			float2 screenPos : TEXCOORD2;
			half4 packedData : TEXCOORD3;
			float4 color : COLOR;
		};


		//quaternion rotation helper
		float3 Rotate(float3 vec, float4 q){
			float3 t = 2 * cross(q.xyz, vec);
			return vec + q.w * t + cross(q.xyz, t);
		}

		void vert(inout appdata_full v, out Input o){
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float4 hpos = mul (UNITY_MATRIX_MV, v.vertex);
			float dist = distance(hpos.xyz, _CameraPosition);
            o.pos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0) );
            o.normal = mul( unity_ObjectToWorld, float4( v.normal, 0.0 ) ).xyz;

            float localPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 0.0) );
			float localPosNorm = normalize(localPos);

			o.packedData.x = clamp((dist - _FogStart) / _FogRangeInverse, 0.0, 1.0);

			float vertNorm = normalize(o.pos);

			float3 deltaToCam = o.pos - _WorldSpaceCameraPos;

			half3 tangent = half3(v.tangent.xyz);
			tangent = Rotate(tangent, float4(deltaToCam.zxy, deltaToCam.x));

			float3 tangentWorld = normalize(mul( unity_ObjectToWorld, v.tangent ).xyz);
			float tangentDot = dot(o.pos.xyz, normalize(tangent)) * .5 + .5;

			float3 biTangent = cross(o.normal, tangent);
			float biTangentDot = dot(o.pos.xyz, normalize(biTangent)) * .5 + .5;

			float3 normDelta = normalize(deltaToCam);
			float normDeltaDot = dot(normDelta, normalize(o.normal)) * .5 + .5;

			o.packedData.y = tangentDot;
			o.packedData.z = biTangentDot * _CrossGradientScaler;
		}


		void surf (Input IN, inout SurfaceOutputKRZ o) {
            float dustDot = -dot(mul((float3x3)unity_WorldToObject, WorldNormalVector(IN, o.Normal)), normalize(_DustDirection.xyz));
			o.Emission = fixed3(0.0,0.0,0.0); // Stop DX11 complaining.
			o.Normal = WorldNormalVector(IN, o.Normal);

			//sample gradientColors
			//wrap the data
			half wrappedGrad = fmod( fmod(IN.packedData.z, 1) + 1, 1);

			float noiseVal = noise(IN.pos.xyz * _NoiseResolution) - 0.5;


			//stairstepping using the CrossGradient (bitangent inside packedData.z)
			half stairStepOffset = _StairStepOffset;
			half stairStepSize = _StairStepSize + noiseVal * _NoiseStairStep;
			//do rounding
			half gradUVOffset = round(IN.packedData.z * stairStepSize) * stairStepOffset;

			//get gradient color from GRAD1 and GRAD2
			half4 gradColor = lerp(_Gradient1, _Gradient2, IN.packedData.z + fmod(IN.packedData.y, .5));

			//wrap the gradientUV
//			half2 gradUV = fmod( (fmod(IN.packedData.yz, 1) + 1), 1);
			half2 gradUV = (IN.packedData.yy) + IN.pos.xy + gradUVOffset + _Time.y * _ModulationSpeed;

			//apply noise to sample gradient with noise
			gradUV += noiseVal * _NoiseGradient;
			//sample texture
			half4 texColor = tex2D(_GradientTex, gradUV * _GradientScaler);


			//lerp
			half4 finalGrad = lerp(texColor, gradColor, _CrossGradientFactor);

			//multiply
//			half4 finalGrad = gradColor * texColor;

			//add
//			half4 finalGrad = gradColor*_AmbientStrength + texColor;

			o.Albedo = lerp(IN.color, finalGrad, _GradientStrength);
			o.Albedo = lerp(o.Albedo, _DustColor, clamp(dustDot,0.0,1.0) * _DustAmount);
		}

		float mod(float x, float y){
			return x - y * floor(x/y);
		}

		inline fixed4 LightingKRZ (SurfaceOutputKRZ s, fixed3 lightDir, fixed3 viewDir, fixed atten){
			atten = step(_LightCutoff, atten) * atten;
			float4 c;
			half diffuse = dot(normalize(s.Normal), normalize(lightDir));
			_LightColor = clamp(_LightColor, _ColorChangeCutoff, 1.0);
			_DarkColor = clamp(_DarkColor, 0.0, _ColorChangeCutoff);
			diffuse = diffuse >= _ColorChangeCutoff ? _LightColor : _DarkColor;
			float3 diffuseLight = (_LightColor0.rgb * s.Albedo) * diffuse * atten;

			float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
			float3 ambientLight = ((ambient) * s.Albedo);

			c.rgb = diffuseLight + ambientLight;
			c.a = s.Alpha;
			return c;
		}
		
		void finalcolor(Input IN, SurfaceOutputKRZ o, inout fixed4 color){
			fixed4 fogColor = fixed4(0.0, 0.0, 0.0, 0.0);
			#ifdef UNITY_PASS_FORWARDADD
			fogColor = 0;
			#endif

			fogColor = tex2D(_ColorStrip, float2(IN.packedData.x, 0));

			color.rgb = lerp(color.rgb, fogColor.rgb, clamp(_FogSaturationPercent * fogColor.a, 0.0, 1.0));

			#ifndef UNITY_PASS_FORWARDADD
            float lerpValue = clamp(IN.pos.y / _FogHeight, 0, 1);
            color.rgb = lerp (_FogColor.rgb, color.rgb, lerpValue);
            #endif
        }





		ENDCG
	}
	FallBack "VertexLit"
}