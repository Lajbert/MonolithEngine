#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

/*Texture2D SpriteTexture;

sampler s0;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};
*/
sampler s0;
texture2D lightMask;
sampler2D lightSampler = sampler_state {
	Texture = <lightMask>; 
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//return input.Color;
	//return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
	/*float4 color = tex2D(s0, input.TextureCoordinates);
	color.rgb = 0;
    return color;*/

	float4 color = tex2D(s0, input.TextureCoordinates);
	float4 lightColor = tex2D(lightSampler, input.TextureCoordinates);
	return color * lightColor;
	//return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};