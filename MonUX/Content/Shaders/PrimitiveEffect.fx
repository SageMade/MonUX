#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float4x4 MatrixTransform;

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput SpriteVS(float4 position	: POSITION0, float4 color	: COLOR0, float2 texCoord	: TEXCOORD0 )
{
	VertexShaderOutput output;
    output.Position = mul(position, MatrixTransform);
	output.Color = color;
	output.TextureCoordinates = texCoord;
	return output;
}

float4 TexturePS(VertexShaderOutput input) : COLOR
{	
	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
}

float4 ColorPS(VertexShaderOutput input) : COLOR
{	
	return input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL SpriteVS();
		PixelShader = compile PS_SHADERMODEL TexturePS();
	}
};

technique ColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL SpriteVS();
		PixelShader = compile PS_SHADERMODEL ColorPS();
	}
};