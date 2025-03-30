namespace Strawberry.Graphics.Text;

public class TextShader : BasicShader
{
    string vs = @"<HLSL>struct VOut
                            {
	                            float4 position : SV_POSITION;
                                float2 texcoord : TEXCOORD0;
	                            float4 color : COLOR;
                            };

                            float4x4 projection;

                            VOut VShader(float4 position : POSITION, float2 texcoord : TEXCOORD, float4 color : COLOR)
                            {
	                            VOut output;
	                            output.position = mul(projection,position);
                                output.texcoord = texcoord;
	                            output.color = color;
	                            return output;
                            }</HLSL>
                            <GLSL>#version 100
 
                            attribute vec2 POSITION;
                            attribute vec2 TEXCOORD;
                            attribute vec4 COLOR;

                            uniform mat4 projection;

                            varying vec4 ColorV;
                            varying vec2 TexCoord;

 
                            void main()
                            {
	                            gl_Position = projection * vec4(POSITION.x,
	                                               POSITION.y,
	                                               0, 1.0);
                                ColorV = COLOR;
                                TexCoord = TEXCOORD;
                            }</GLSL>";

    string ps = @"<HLSL>
                            Texture2D tex0;
                            SamplerState texSampler;

                            float4 PShader(float4 position : SV_POSITION, float2 texcoord : TEXCOORD0, float4 color : COLOR) : SV_TARGET
                            {
                                float dist = tex0.Sample(texSampler, texcoord).r;
                                float alpha = 0.0f;
                                if(dist >= 0.4f)
	                                alpha = dist*2;

                                alpha *= smoothstep(0.42f,0.51f, dist);
	                            return float4(color.rgb,alpha * color.a);
                            }</HLSL>
                            <GLSL>#version 100
                            uniform sampler2D tex0;
                            precision mediump float;
 
                            varying vec4 ColorV;
                            varying vec2 TexCoord;
                            
                            void main()
                            {
                                float dist = texture2D(tex0,TexCoord).r;

                                float alpha = 0.;
                                if(dist >= 0.4)
	                                alpha = dist*2.;

                                alpha *= smoothstep(0.42,0.51, dist);

	                            gl_FragColor = vec4(ColorV.x,ColorV.y,ColorV.z,ColorV.w * alpha);
                            }</GLSL>";

    string vse = "VShader";
    string pse = "PShader";

    protected override string VertexShader { get { return vs; } }

    protected override string PixelShader { get { return ps; } }

    protected override string VertexShaderEntry { get { return vse; } }

    protected override string PixelShaderEntry { get { return pse; } }

    public TextShader(IGraphicsContext context, VertexElementContainer elements)
        : base(context, elements)
    {

    }
}