Shader "CustomRenderTexture/CustomRT"
{
SubShader
{ Lighting Off 

CGINCLUDE
#include "UnityCustomRenderTexture.cginc"
#pragma target 3.0
sampler2D _Tex;
float _RainVariable;
ENDCG

Pass
{
CGPROGRAM

#pragma vertex CustomRenderTextureVertexShader
#pragma fragment frag

//x: color, y: color z: length




float4 frag(v2f_customrendertexture IN) : COLOR
{
	float2 uv = IN.localTexcoord.xy;
	 float4 c = tex2D(_Tex, uv);
	 c.a = lerp(c.a, 1, _RainVariable);
	 c.rg = lerp(c.rg, 0.5, _RainVariable);
	 return c;
}
ENDCG
}



Pass
{
CGPROGRAM

#pragma vertex CustomRenderTextureVertexShader
#pragma fragment frag

float4 frag(v2f_customrendertexture IN) : COLOR
{
	float2 uv = IN.localTexcoord.xy;
	float distance1 = distance(uv, 0.5);
	float4 targetColor = float4(normalize((uv - 0.5) * 0.5 + 0.5),distance1 * 2,0);
	clip(step(distance1, 0.5) - 0.5);
	return targetColor;
	
}
ENDCG
}


Pass
{
CGPROGRAM

#pragma vertex CustomRenderTextureVertexShader
#pragma fragment frag

float4 frag(v2f_customrendertexture IN) : COLOR
{
	float2 uv = IN.localTexcoord.xy;
	float distance1 = distance(uv, 0.5);
	float4 targetColor = float4(normalize((uv - 0.5) * 0.5 + 0.5),distance1 * 2,0);
	clip(step(distance1, 0.5) - 0.5);
	return targetColor;
	
}
ENDCG
}

Pass
{
CGPROGRAM

#pragma vertex CustomRenderTextureVertexShader
#pragma fragment frag

float4 frag(v2f_customrendertexture IN) : COLOR
{
	float2 uv = IN.localTexcoord.xy;
	float distance1 = distance(uv, 0.5);
	float4 targetColor = float4(normalize((uv - 0.5) * 0.5 + 0.5),distance1 * 2,0);
	clip(step(distance1, 0.5) - 0.5);
	return targetColor;
	
}
ENDCG
}

Pass
{
CGPROGRAM

#pragma vertex CustomRenderTextureVertexShader
#pragma fragment frag

float4 frag(v2f_customrendertexture IN) : COLOR
{
	float2 uv = IN.localTexcoord.xy;
	float distance1 = distance(uv, 0.5);
	float4 targetColor = float4(normalize((uv - 0.5) * 0.5 + 0.5),distance1 * 2,0);
	clip(step(distance1, 0.5) - 0.5);
	return targetColor;
	
}
ENDCG
}



}
}