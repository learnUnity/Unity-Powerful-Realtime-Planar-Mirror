/*
 This is just an example of mirror's post processing shader.
 */
Shader "Hidden/Mirror-Blur"
{
properties{
	_MainTex("TEX", 2D) = "white"{}
}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			//It's just an blur example
			//You can write your own post processing effects!
fixed4 frag(v2f IN) : COLOR
{
	float2 uv = IN.uv.xy;
	fixed4 c = tex2D(_MainTex, uv);

	c += tex2D(_MainTex, uv + float2(0.003,0.003)) * 0.2;
	c += tex2D(_MainTex, uv + float2(0.003,-0.003)) * 0.2;
	c += tex2D(_MainTex, uv + float2(-0.003,0.003)) * 0.2;
	c += tex2D(_MainTex, uv + float2(-0.003,-0.003)) * 0.2;
	c += tex2D(_MainTex, uv + float2(0.003,0)) * 0.3;
	c += tex2D(_MainTex, uv + float2(0,-0.003)) * 0.3;
	c += tex2D(_MainTex, uv + float2(-0.003,0)) * 0.3;
	c += tex2D(_MainTex, uv + float2(0,0.003)) * 0.3;

	c += tex2D(_MainTex, uv + float2(0.002,0.002)) * 0.4;
	c += tex2D(_MainTex, uv + float2(0.002,-0.002)) * 0.4;
	c += tex2D(_MainTex, uv + float2(-0.002,0.002)) * 0.4;
	c += tex2D(_MainTex, uv + float2(-0.002,-0.002)) * 0.4;
	c += tex2D(_MainTex, uv + float2(0.002,0)) * 0.5;
	c += tex2D(_MainTex, uv + float2(0,-0.002)) * 0.5;
	c += tex2D(_MainTex, uv + float2(-0.002,0)) * 0.5;
	c += tex2D(_MainTex, uv + float2(0,0.002)) * 0.5;

	c += tex2D(_MainTex, uv + float2(0.001,0.001)) * 0.7;
	c += tex2D(_MainTex, uv + float2(0.001,-0.001)) * 0.7;
	c += tex2D(_MainTex, uv + float2(-0.001,0.001)) * 0.7;
	c += tex2D(_MainTex, uv + float2(-0.001,-0.001)) * 0.7;
	c += tex2D(_MainTex, uv + float2(0.001,0)) * 0.8;
	c += tex2D(_MainTex, uv + float2(0,-0.001)) * 0.8;
	c += tex2D(_MainTex, uv + float2(-0.001,0)) * 0.8;
	c += tex2D(_MainTex, uv + float2(0,0.001)) * 0.8;
	c /= 12.6;
	 return c;
}
			ENDCG
		}
	}
}
