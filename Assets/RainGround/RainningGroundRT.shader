Shader "CustomRenderTexture/RainningGroundRT"
{
Properties{
	_SelfTex("Self", 2D) = "white"{}
	_BumpMap("Bump Map", 2D) = "bump"{}
	_NoiseMap("Noise Map", 2D) = "white"{}
	_BumpScale("Bump Scale", float) = 1
}

SubShader
{ Lighting Off Blend srcalpha oneminussrcalpha
CGINCLUDE
#include "UnityCustomRenderTexture.cginc"
#pragma target 3.0
sampler2D _SelfTex;
sampler2D _BumpMap;
float _BumpScale;
sampler2D _NoiseMap;
float4 _RandomNumber;
ENDCG

Pass
{
NAME "Random"
CGPROGRAM

#pragma vertex CustomRenderTextureVertexShader
#pragma fragment frag

//x: color, y: color z: length




float4 frag(v2f_customrendertexture IN) : COLOR
{
	float2 uv = IN.localTexcoord;
	fixed4 noise_color = tex2D(_NoiseMap,uv + _RandomNumber.xy);
	uv.x += _Time.x * (noise_color.r * 2 - 1);
	fixed4 bump_color = (tex2D(_BumpMap, uv));
	bump_color.xy *= _BumpScale;
	bump_color = normalize(bump_color);
	bump_color.a = 1;
	return bump_color;
}
ENDCG
}



}
}