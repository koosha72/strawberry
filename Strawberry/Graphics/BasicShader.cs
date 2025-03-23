namespace Strawberry.Graphics
{
    public class BasicShader : Shader
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
	                            return color * tex0.Sample(texSampler, texcoord);
                            }</HLSL>
                            <GLSL>#version 100
                            uniform sampler2D tex0;
                            precision mediump float;
 
                            varying vec4 ColorV;
                            varying vec2 TexCoord;
                            
                            void main()
                            {
	                            gl_FragColor = vec4(ColorV.x,ColorV.y,ColorV.z,ColorV.w) * texture2D(tex0,TexCoord);
                            }</GLSL>";

        string vse = "VShader";
        string pse = "PShader";

        protected virtual string VertexShader { get { return vs; } }

        protected virtual string PixelShader { get { return ps; } }

        protected virtual string VertexShaderEntry { get { return vse; } }

        protected virtual string PixelShaderEntry { get { return pse; } }

        public BasicShader(IGraphicsContext context, VertexElementContainer elements)
            : base(context)
        {
            base.Initialize(VertexShader, PixelShader, VertexShaderEntry, PixelShaderEntry, elements);
        }

        protected BasicShader(IGraphicsContext context, VertexElementContainer elements, string vs, string ps, string vse, string pse)
            : base(context)
        {
            base.Initialize(VertexShader, PixelShader, VertexShaderEntry, PixelShaderEntry, elements);
        }

        Math.Matrix4 projection;
        public Math.Matrix4 Projection
        {
            get
            {
                return projection;
            }
            set
            {
                this.projection = value;
                BaseShader.SetMatrixParameterByName("", "projection", projection, false);
            }
        }

        public void SetTexture(ITexture tex)
        {
            tex.Activate(this.BaseShader, "tex0");
        }
    }
}
