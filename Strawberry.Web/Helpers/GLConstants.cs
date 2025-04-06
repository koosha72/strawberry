namespace Strawberry.Web.Helpers;

public static partial class GL
{
    // Summary:
    //     [requires: v1.0] Original was GL_POINT_SMOOTH = 0x0B10
    public const int PointSmooth = 2832;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LINE_SMOOTH = 0x0B20
    public const int LineSmooth = 2848;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LINE_STIPPLE = 0x0B24
    public const int LineStipple = 2852;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_POLYGON_SMOOTH = 0x0B41
    public const int PolygonSmooth = 2881;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_POLYGON_STIPPLE = 0x0B42
    public const int PolygonStipple = 2882;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_CULL_FACE = 0x0B44
    public const int _CullFace = 2884;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LIGHTING = 0x0B50
    public const int Lighting = 2896;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_COLOR_MATERIAL = 0x0B57
    public const int ColorMaterial = 2903;
    //
    // Summary:
    //     [requires: v1.0 or NV_register_combiners] Original was GL_FOG = 0x0B60
    public const int Fog = 2912;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_DEPTH_TEST = 0x0B71
    public const int DepthTest = 2929;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_STENCIL_TEST = 0x0B90
    public const int StencilTest = 2960;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_NORMALIZE = 0x0BA1
    public const int Normalize = 2977;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_ALPHA_TEST = 0x0BC0
    public const int AlphaTest = 3008;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_DITHER = 0x0BD0
    public const int Dither = 3024;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_BLEND = 0x0BE2
    public const int Blend = 3042;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_INDEX_LOGIC_OP = 0x0BF1
    public const int IndexLogicOp = 3057;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_COLOR_LOGIC_OP = 0x0BF2
    public const int ColorLogicOp = 3058;
    //
    // Summary:
    //     [requires: v1.0 or ARB_viewport_array] Original was GL_SCISSOR_TEST = 0x0C11
    public const int ScissorTest = 3089;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_TEXTURE_GEN_S = 0x0C60
    public const int TextureGenS = 3168;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_TEXTURE_GEN_T = 0x0C61
    public const int TextureGenT = 3169;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_TEXTURE_GEN_R = 0x0C62
    public const int TextureGenR = 3170;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_TEXTURE_GEN_Q = 0x0C63
    public const int TextureGenQ = 3171;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_AUTO_NORMAL = 0x0D80
    public const int AutoNormal = 3456;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP1_COLOR_4 = 0x0D90
    public const int Map1Color4 = 3472;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP1_INDEX = 0x0D91
    public const int Map1Index = 3473;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP1_NORMAL = 0x0D92
    public const int Map1Normal = 3474;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP1_TEXTURE_COORD_1 = 0x0D93
    public const int Map1TextureCoord1 = 3475;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP1_TEXTURE_COORD_2 = 0x0D94
    public const int Map1TextureCoord2 = 3476;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP1_TEXTURE_COORD_3 = 0x0D95
    public const int Map1TextureCoord3 = 3477;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP1_TEXTURE_COORD_4 = 0x0D96
    public const int Map1TextureCoord4 = 3478;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP1_VERTEX_3 = 0x0D97
    public const int Map1Vertex3 = 3479;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP1_VERTEX_4 = 0x0D98
    public const int Map1Vertex4 = 3480;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP2_COLOR_4 = 0x0DB0
    public const int Map2Color4 = 3504;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP2_INDEX = 0x0DB1
    public const int Map2Index = 3505;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP2_NORMAL = 0x0DB2
    public const int Map2Normal = 3506;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP2_TEXTURE_COORD_1 = 0x0DB3
    public const int Map2TextureCoord1 = 3507;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP2_TEXTURE_COORD_2 = 0x0DB4
    public const int Map2TextureCoord2 = 3508;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP2_TEXTURE_COORD_3 = 0x0DB5
    public const int Map2TextureCoord3 = 3509;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP2_TEXTURE_COORD_4 = 0x0DB6
    public const int Map2TextureCoord4 = 3510;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP2_VERTEX_3 = 0x0DB7
    public const int Map2Vertex3 = 3511;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_MAP2_VERTEX_4 = 0x0DB8
    public const int Map2Vertex4 = 3512;
    //
    // Summary:
    //     [requires: v1.0 or ARB_internalformat_query2] Original was GL_TEXTURE_1D = 0x0DE0
    public const int Texture1D = 3552;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_POLYGON_OFFSET_POINT = 0x2A01
    public const int PolygonOffsetPoint = 10753;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_POLYGON_OFFSET_LINE = 0x2A02
    public const int PolygonOffsetLine = 10754;
    //
    // Summary:
    //     [requires: v3.0] Original was GL_CLIP_DISTANCE0 = 0x3000
    public const int ClipDistance0 = 12288;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_CLIP_PLANE0 = 0x3000
    public const int ClipPlane0 = 12288;
    //
    // Summary:
    //     [requires: v3.0] Original was GL_CLIP_DISTANCE1 = 0x3001
    public const int ClipDistance1 = 12289;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_CLIP_PLANE1 = 0x3001
    public const int ClipPlane1 = 12289;
    //
    // Summary:
    //     [requires: v3.0] Original was GL_CLIP_DISTANCE2 = 0x3002
    public const int ClipDistance2 = 12290;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_CLIP_PLANE2 = 0x3002
    public const int ClipPlane2 = 12290;
    //
    // Summary:
    //     [requires: v3.0] Original was GL_CLIP_DISTANCE3 = 0x3003
    public const int ClipDistance3 = 12291;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_CLIP_PLANE3 = 0x3003
    public const int ClipPlane3 = 12291;
    //
    // Summary:
    //     [requires: v3.0] Original was GL_CLIP_DISTANCE4 = 0x3004
    public const int ClipDistance4 = 12292;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_CLIP_PLANE4 = 0x3004
    public const int ClipPlane4 = 12292;
    //
    // Summary:
    //     [requires: v3.0] Original was GL_CLIP_DISTANCE5 = 0x3005
    public const int ClipDistance5 = 12293;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_CLIP_PLANE5 = 0x3005
    public const int ClipPlane5 = 12293;
    //
    // Summary:
    //     [requires: v3.0] Original was GL_CLIP_DISTANCE6 = 0x3006
    public const int ClipDistance6 = 12294;
    //
    // Summary:
    //     [requires: v3.0] Original was GL_CLIP_DISTANCE7 = 0x3007
    public const int ClipDistance7 = 12295;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LIGHT0 = 0x4000
    public const int Light0 = 16384;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LIGHT1 = 0x4001
    public const int Light1 = 16385;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LIGHT2 = 0x4002
    public const int Light2 = 16386;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LIGHT3 = 0x4003
    public const int Light3 = 16387;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LIGHT4 = 0x4004
    public const int Light4 = 16388;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LIGHT5 = 0x4005
    public const int Light5 = 16389;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LIGHT6 = 0x4006
    public const int Light6 = 16390;
    //
    // Summary:
    //     [requires: v1.0] Original was GL_LIGHT7 = 0x4007
    public const int Light7 = 16391;
    //
    // Summary:
    //     [requires: v4.5 or ARB_imaging] Original was GL_CONVOLUTION_1D = 0x8010
    public const int Convolution1D = 32784;
    //
    // Summary:
    //     [requires: EXT_convolution] Original was GL_CONVOLUTION_1D_EXT = 0x8010
    public const int Convolution1DExt = 32784;
    //
    // Summary:
    //     [requires: v4.5 or ARB_imaging] Original was GL_CONVOLUTION_2D = 0x8011
    public const int Convolution2D = 32785;
    //
    // Summary:
    //     [requires: EXT_convolution] Original was GL_CONVOLUTION_2D_EXT = 0x8011
    public const int Convolution2DExt = 32785;
    //
    // Summary:
    //     [requires: v4.5 or ARB_imaging] Original was GL_SEPARABLE_2D = 0x8012
    public const int Separable2D = 32786;
    //
    // Summary:
    //     [requires: EXT_convolution] Original was GL_SEPARABLE_2D_EXT = 0x8012
    public const int Separable2DExt = 32786;
    //
    // Summary:
    //     [requires: v4.5 or ARB_imaging] Original was GL_HISTOGRAM = 0x8024
    public const int Histogram = 32804;
    //
    // Summary:
    //     [requires: EXT_histogram] Original was GL_HISTOGRAM_EXT = 0x8024
    public const int HistogramExt = 32804;
    //
    // Summary:
    //     [requires: EXT_histogram] Original was GL_MINMAX_EXT = 0x802E
    public const int MinmaxExt = 32814;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_POLYGON_OFFSET_FILL = 0x8037
    public const int PolygonOffsetFill = 32823;
    //
    // Summary:
    //     [requires: v1.2] Original was GL_RESCALE_NORMAL = 0x803A
    public const int RescaleNormal = 32826;
    //
    // Summary:
    //     [requires: EXT_rescale_normal] Original was GL_RESCALE_NORMAL_EXT = 0x803A
    public const int RescaleNormalExt = 32826;
    //
    // Summary:
    //     [requires: v1.1 or KHR_debug] Original was GL_VERTEX_ARRAY = 0x8074
    public const int VertexArray = 32884;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_NORMAL_ARRAY = 0x8075
    public const int NormalArray = 32885;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_COLOR_ARRAY = 0x8076
    public const int ColorArray = 32886;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_INDEX_ARRAY = 0x8077
    public const int IndexArray = 32887;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_TEXTURE_COORD_ARRAY = 0x8078
    public const int TextureCoordArray = 32888;
    //
    // Summary:
    //     [requires: v1.1] Original was GL_EDGE_FLAG_ARRAY = 0x8079
    public const int EdgeFlagArray = 32889;
    //
    // Summary:
    //     [requires: SGIX_interlace] Original was GL_INTERLACE_SGIX = 0x8094
    public const int InterlaceSgix = 32916;
    //
    // Summary:
    //     [requires: v1.3] Original was GL_MULTISAMPLE = 0x809D
    public const int Multisample = 32925;
    //
    // Summary:
    //     [requires: SGIS_multisample] Original was GL_MULTISAMPLE_SGIS = 0x809D
    public const int MultisampleSgis = 32925;
    //
    // Summary:
    //     [requires: v1.3] Original was GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E
    public const int SampleAlphaToCoverage = 32926;
    //
    // Summary:
    //     [requires: SGIS_multisample] Original was GL_SAMPLE_ALPHA_TO_MASK_SGIS = 0x809E
    public const int SampleAlphaToMaskSgis = 32926;
    //
    // Summary:
    //     [requires: v1.3] Original was GL_SAMPLE_ALPHA_TO_ONE = 0x809F
    public const int SampleAlphaToOne = 32927;
    //
    // Summary:
    //     [requires: SGIS_multisample] Original was GL_SAMPLE_ALPHA_TO_ONE_SGIS = 0x809F
    public const int SampleAlphaToOneSgis = 32927;
    //
    // Summary:
    //     [requires: v1.3] Original was GL_SAMPLE_COVERAGE = 0x80A0
    public const int SampleCoverage = 32928;
    //
    // Summary:
    //     [requires: SGIS_multisample] Original was GL_SAMPLE_MASK_SGIS = 0x80A0
    public const int SampleMaskSgis = 32928;
    //
    // Summary:
    //     [requires: SGI_texture_color_table] Original was GL_TEXTURE_COLOR_TABLE_SGI =
    //     0x80BC
    public const int TextureColorTableSgi = 32956;
    //
    // Summary:
    //     [requires: v4.5 or ARB_imaging] Original was GL_COLOR_TABLE = 0x80D0
    public const int ColorTable = 32976;
    //
    // Summary:
    //     [requires: SGI_color_table] Original was GL_COLOR_TABLE_SGI = 0x80D0
    public const int ColorTableSgi = 32976;
    //
    // Summary:
    //     [requires: v4.5 or ARB_imaging] Original was GL_POST_CONVOLUTION_COLOR_TABLE
    //     = 0x80D1
    public const int PostConvolutionColorTable = 32977;
    //
    // Summary:
    //     [requires: SGI_color_table] Original was GL_POST_CONVOLUTION_COLOR_TABLE_SGI
    //     = 0x80D1
    public const int PostConvolutionColorTableSgi = 32977;
    //
    // Summary:
    //     [requires: v4.5 or ARB_imaging] Original was GL_POST_COLOR_MATRIX_COLOR_TABLE
    //     = 0x80D2
    public const int PostColorMatrixColorTable = 32978;
    //
    // Summary:
    //     [requires: SGI_color_table] Original was GL_POST_COLOR_MATRIX_COLOR_TABLE_SGI
    //     = 0x80D2
    public const int PostColorMatrixColorTableSgi = 32978;
    //
    // Summary:
    //     [requires: SGIS_texture4D] Original was GL_TEXTURE_4D_SGIS = 0x8134
    public const int Texture4DSgis = 33076;
    //
    // Summary:
    //     [requires: SGIX_pixel_texture] Original was GL_PIXEL_TEX_GEN_SGIX = 0x8139
    public const int PixelTexGenSgix = 33081;
    //
    // Summary:
    //     [requires: SGIX_sprite] Original was GL_SPRITE_SGIX = 0x8148
    public const int SpriteSgix = 33096;
    //
    // Summary:
    //     [requires: SGIX_reference_plane] Original was GL_REFERENCE_PLANE_SGIX = 0x817D
    public const int ReferencePlaneSgix = 33149;
    //
    // Summary:
    //     [requires: SGIX_ir_instrument1] Original was GL_IR_INSTRUMENT1_SGIX = 0x817F
    public const int IrInstrument1Sgix = 33151;
    //
    // Summary:
    //     [requires: SGIX_calligraphic_fragment] Original was GL_CALLIGRAPHIC_FRAGMENT_SGIX
    //     = 0x8183
    public const int CalligraphicFragmentSgix = 33155;
    //
    // Summary:
    //     [requires: SGIX_framezoom] Original was GL_FRAMEZOOM_SGIX = 0x818B
    public const int FramezoomSgix = 33163;
    //
    // Summary:
    //     [requires: SGIX_fog_offset] Original was GL_FOG_OFFSET_SGIX = 0x8198
    public const int FogOffsetSgix = 33176;
    //
    // Summary:
    //     [requires: EXT_shared_texture_palette] Original was GL_SHARED_TEXTURE_PALETTE_EXT
    //     = 0x81FB
    public const int SharedTexturePaletteExt = 33275;
    //
    // Summary:
    //     [requires: v4.3 or KHR_debug] Original was GL_DEBUG_OUTPUT_SYNCHRONOUS = 0x8242
    public const int DebugOutputSynchronous = 33346;
    //
    // Summary:
    //     [requires: SGIX_async_histogram] Original was GL_ASYNC_HISTOGRAM_SGIX = 0x832C
    public const int AsyncHistogramSgix = 33580;
    //
    // Summary:
    //     [requires: SGIS_pixel_texture] Original was GL_PIXEL_TEXTURE_SGIS = 0x8353
    public const int PixelTextureSgis = 33619;
    //
    // Summary:
    //     [requires: SGIX_async_pixel] Original was GL_ASYNC_TEX_IMAGE_SGIX = 0x835C
    public const int AsyncTexImageSgix = 33628;
    //
    // Summary:
    //     [requires: SGIX_async_pixel] Original was GL_ASYNC_DRAW_PIXELS_SGIX = 0x835D
    public const int AsyncDrawPixelsSgix = 33629;
    //
    // Summary:
    //     [requires: SGIX_async_pixel] Original was GL_ASYNC_READ_PIXELS_SGIX = 0x835E
    public const int AsyncReadPixelsSgix = 33630;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_LIGHTING_SGIX = 0x8400
    public const int FragmentLightingSgix = 33792;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_COLOR_MATERIAL_SGIX
    //     = 0x8401
    public const int FragmentColorMaterialSgix = 33793;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_LIGHT0_SGIX = 0x840C
    public const int FragmentLight0Sgix = 33804;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_LIGHT1_SGIX = 0x840D
    public const int FragmentLight1Sgix = 33805;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_LIGHT2_SGIX = 0x840E
    public const int FragmentLight2Sgix = 33806;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_LIGHT3_SGIX = 0x840F
    public const int FragmentLight3Sgix = 33807;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_LIGHT4_SGIX = 0x8410
    public const int FragmentLight4Sgix = 33808;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_LIGHT5_SGIX = 0x8411
    public const int FragmentLight5Sgix = 33809;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_LIGHT6_SGIX = 0x8412
    public const int FragmentLight6Sgix = 33810;
    //
    // Summary:
    //     [requires: SGIX_fragment_lighting] Original was GL_FRAGMENT_LIGHT7_SGIX = 0x8413
    public const int FragmentLight7Sgix = 33811;
    //
    // Summary:
    //     [requires: v1.5] Original was GL_FOG_COORD_ARRAY = 0x8457
    public const int FogCoordArray = 33879;
    //
    // Summary:
    //     [requires: v1.4] Original was GL_COLOR_SUM = 0x8458
    public const int ColorSum = 33880;
    //
    // Summary:
    //     [requires: v1.4] Original was GL_SECONDARY_COLOR_ARRAY = 0x845E
    public const int SecondaryColorArray = 33886;
    //
    // Summary:
    //     [requires: v3.1 or ARB_internalformat_query2] Original was GL_TEXTURE_RECTANGLE
    //     = 0x84F5
    public const int TextureRectangle = 34037;
    //
    // Summary:
    //     [requires: ARB_texture_rectangle] Original was GL_TEXTURE_RECTANGLE_ARB = 0x84F5
    public const int TextureRectangleArb = 34037;
    //
    // Summary:
    //     [requires: NV_texture_rectangle] Original was GL_TEXTURE_RECTANGLE_NV = 0x84F5
    public const int TextureRectangleNv = 34037;
    //
    // Summary:
    //     [requires: v1.3 or ARB_internalformat_query2] Original was GL_TEXTURE_CUBE_MAP
    //     = 0x8513
    public const int TextureCubeMap = 34067;
    //
    // Summary:
    //     [requires: ARB_texture_cube_map] Original was GL_TEXTURE_CUBE_MAP_ARB = 0x8513
    public const int TextureCubeMapArb = 34067;
    //
    // Summary:
    //     [requires: EXT_texture_cube_map] Original was GL_TEXTURE_CUBE_MAP_EXT = 0x8513
    public const int TextureCubeMapExt = 34067;
    //
    // Summary:
    //     Original was GL_TEXTURE_CUBE_MAP_OES = 0x8513
    public const int TextureCubeMapOes = 34067;
    //
    // Summary:
    //     [requires: v3.2] Original was GL_PROGRAM_POINT_SIZE = 0x8642
    public const int ProgramPointSize = 34370;
    //
    // Summary:
    //     [requires: v2.0] Original was GL_VERTEX_PROGRAM_POINT_SIZE = 0x8642
    public const int VertexProgramPointSize = 34370;
    //
    // Summary:
    //     [requires: v2.0] Original was GL_VERTEX_PROGRAM_TWO_SIDE = 0x8643
    public const int VertexProgramTwoSide = 34371;
    //
    // Summary:
    //     [requires: v3.2 or ARB_depth_clamp] Original was GL_DEPTH_CLAMP = 0x864F
    public const int DepthClamp = 34383;
    //
    // Summary:
    //     [requires: v3.2 or AMD_seamless_cubemap_per_texture; ARB_seamless_cube_map; ARB_seamless_cubemap_per_texture]
    //     Original was GL_TEXTURE_CUBE_MAP_SEAMLESS = 0x884F
    public const int TextureCubeMapSeamless = 34895;
    //
    // Summary:
    //     [requires: v2.0] Original was GL_POINT_SPRITE = 0x8861
    public const int PointSprite = 34913;
    //
    // Summary:
    //     [requires: v4.0] Original was GL_SAMPLE_SHADING = 0x8C36
    public const int SampleShading = 35894;
    //
    // Summary:
    //     [requires: v3.0] Original was GL_RASTERIZER_DISCARD = 0x8C89
    public const int RasterizerDiscard = 35977;
    //
    // Summary:
    //     Original was GL_TEXTURE_GEN_STR_OES = 0x8D60
    public const int TextureGenStrOes = 36192;
    //
    // Summary:
    //     [requires: v4.3 or ARB_ES3_compatibility] Original was GL_PRIMITIVE_RESTART_FIXED_INDEX
    //     = 0x8D69
    public const int PrimitiveRestartFixedIndex = 36201;
    //
    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_sRGB] Original was GL_FRAMEBUFFER_SRGB = 0x8DB9
    public const int FramebufferSrgb = 36281;
    //
    // Summary:
    //     [requires: v3.2 or ARB_texture_multisample] Original was GL_SAMPLE_MASK = 0x8E51
    public const int SampleMask = 36433;
    //
    // Summary:
    //     Original was GL_FETCH_PER_SAMPLE_ARM = 0x8F65
    public const int FetchPerSampleArm = 36709;
    //
    // Summary:
    //     [requires: v3.1] Original was GL_PRIMITIVE_RESTART = 0x8F9D
    public const int PrimitiveRestart = 36765;
    //
    // Summary:
    //     [requires: v4.3 or KHR_debug] Original was GL_DEBUG_OUTPUT = 0x92E0
    public const int DebugOutput = 37600;
    //
    // Summary:
    //     [requires: NV_primitive_shading_rate] Original was GL_SHADING_RATE_IMAGE_PER_PRIMITIVE_NV
    //     = 0x95B1
    public const int ShadingRateImagePerPrimitiveNv = 38321;
    //
    // Summary:
    //     Original was GL_FRAMEBUFFER_FETCH_NONCOHERENT_QCOM = 0x96A2
    public const int FramebufferFetchNoncoherentQcom = 38562;
    //
    // Summary:
    //     Original was GL_SHADING_RATE_PRESERVE_ASPECT_RATIO_QCOM = 0x96A5
    public const int ShadingRatePreserveAspectRatioQcom = 38565;

    // Summary:
    //     [requires: v1.0 or KHR_context_flush_control, NV_register_combiners] Original
    //     was GL_NONE = 0
    public const int None = 0;

    // Summary:
    //     [requires: v1.0] Original was GL_DEPTH_BUFFER_BIT = 0x00000100
    public const int DepthBufferBit = 0x100;

    // Summary:
    //     [requires: v1.0] Original was GL_ACCUM_BUFFER_BIT = 0x00000200
    public const int AccumBufferBit = 0x200;

    // Summary:
    //     [requires: v1.0] Original was GL_STENCIL_BUFFER_BIT = 0x00000400
    public const int StencilBufferBit = 0x400;

    // Summary:
    //     [requires: v1.0] Original was GL_COLOR_BUFFER_BIT = 0x00004000
    public const int ColorBufferBit = 0x4000;

    // Summary:
    //     Original was GL_COVERAGE_BUFFER_BIT_NV = 0x00008000
    public const int CoverageBufferBitNv = 0x8000;

    // Summary:
    //     [requires: v1.4 or ARB_imaging] Original was GL_FUNC_ADD = 0x8006
    public const int FuncAdd = 0x8006; // 32774

    // Summary:
    //     [requires: v1.4 or ARB_imaging] Original was GL_MIN = 0x8007
    public const int Min = 0x8007; // 32775

    // Summary:
    //     [requires: v1.4 or ARB_imaging] Original was GL_MAX = 0x8008 
    public const int Max = 0x8008; // 32776

    // Summary:
    //     [requires: v1.4 or ARB_imaging] Original was GL_FUNC_SUBTRACT = 0x800A
    public const int FuncSubtract = 0x800A; // 32778

    // Summary:
    //     [requires: v1.4 or ARB_imaging] Original was GL_FUNC_REVERSE_SUBTRACT = 0x800B
    public const int FuncReverseSubtract = 0x800B; // 32779
                                                   // Summary:
                                                   //     [requires: v1.0 or NV_blend_equation_advanced, NV_register_combiners] Original
                                                   //     was GL_ZERO = 0
    public const int Zero = 0;

    // Summary:
    //     [requires: v1.0] Original was GL_ONE = 1
    public const int One = 1;

    // Summary:
    //     [requires: v1.0] Original was GL_SRC_COLOR = 0x0300
    public const int SrcColor = 0x0300; // 768

    // Summary:
    //     [requires: v1.0] Original was GL_ONE_MINUS_SRC_COLOR = 0x0301
    public const int OneMinusSrcColor = 0x0301; // 769

    // Summary:
    //     [requires: v1.0] Original was GL_SRC_ALPHA = 0x0302
    public const int SrcAlpha = 0x0302; // 770

    // Summary:
    //     [requires: v1.0] Original was GL_ONE_MINUS_SRC_ALPHA = 0x0303
    public const int OneMinusSrcAlpha = 0x0303; // 771

    // Summary:
    //     [requires: v1.0] Original was GL_DST_ALPHA = 0x0304
    public const int DstAlpha = 0x0304; // 772

    // Summary:
    //     [requires: v1.0] Original was GL_ONE_MINUS_DST_ALPHA = 0x0305
    public const int OneMinusDstAlpha = 0x0305; // 773

    // Summary:
    //     [requires: v1.0] Original was GL_DST_COLOR = 0x0306
    public const int DstColor = 0x0306; // 774

    // Summary:
    //     [requires: v1.0] Original was GL_ONE_MINUS_DST_COLOR = 0x0307
    public const int OneMinusDstColor = 0x0307; // 775

    // Summary:
    //     [requires: v1.0] Original was GL_SRC_ALPHA_SATURATE = 0x0308
    public const int SrcAlphaSaturate = 0x0308; // 776

    // Summary:
    //     [requires: v1.4 or ARB_imaging] Original was GL_CONSTANT_COLOR = 0x8001
    public const int ConstantColor = 0x8001; // 32769

    // Summary:
    //     [requires: v1.4 or ARB_imaging] Original was GL_ONE_MINUS_CONSTANT_COLOR = 0x8002
    public const int OneMinusConstantColor = 0x8002; // 32770

    // Summary:
    //     [requires: v1.4 or ARB_imaging] Original was GL_CONSTANT_ALPHA = 0x8003
    public const int ConstantAlpha = 0x8003; // 32771

    // Summary:
    //     [requires: v1.4 or ARB_imaging] Original was GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004
    public const int OneMinusConstantAlpha = 0x8004; // 32772

    // Summary:
    //     [requires: v1.5 or ARB_blend_func_extended] Original was GL_SRC1_ALPHA = 0x8589
    public const int Src1Alpha = 0x8589; // 34185

    // Summary:
    //     [requires: v3.3 or ARB_blend_func_extended] Original was GL_SRC1_COLOR = 0x88F9
    public const int Src1Color = 0x88F9; // 35065

    // Summary:
    //     [requires: v3.3 or ARB_blend_func_extended] Original was GL_ONE_MINUS_SRC1_COLOR = 0x88FA
    public const int OneMinusSrc1Color = 0x88FA; // 35066

    // Summary:
    //     [requires: v3.3 or ARB_blend_func_extended] Original was GL_ONE_MINUS_SRC1_ALPHA = 0x88FB
    public const int OneMinusSrc1Alpha = 0x88FB; // 35067

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_READ_FRAMEBUFFER = 0x8CA8
    public const int ReadFramebuffer = 0x8CA8; // 36008

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_DRAW_FRAMEBUFFER = 0x8CA9
    public const int DrawFramebuffer = 0x8CA9; // 36009

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_FRAMEBUFFER = 0x8D40
    public const int Framebuffer = 0x8D40; // 36160

    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_FRAMEBUFFER_EXT = 0x8D40
    public const int FramebufferExt = 0x8D40; // 36160

    // Summary:
    //     Original was GL_FRAMEBUFFER_OES = 0x8D40
    public const int FramebufferOes = 0x8D40; // 36160

    //
    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object, ARB_internalformat_query2, NV_internalformat_sample_query]
    //     Original was GL_RENDERBUFFER = 0x8D41
    public const int Renderbuffer = 36161;
    //
    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_RENDERBUFFER_EXT = 0x8D41
    public const int RenderbufferExt = 36161;
    //
    // Summary:
    //     Original was GL_RENDERBUFFER_OES = 0x8D41
    public const int RenderbufferOes = 36161;

    // Basic formats
    public const int DepthComponent = 0x1902; // 6402
    public const int R3G3B2 = 0x2A10; // 10768

    // Alpha formats
    public const int Alpha4 = 0x803B; // 32827
    public const int Alpha8 = 0x803C; // 32828
    public const int Alpha12 = 0x803D; // 32829
    public const int Alpha16 = 0x803E; // 32830

    // RGB formats
    public const int Rgb4 = 0x804F; // 32847
    public const int Rgb5 = 0x8050; // 32848
    public const int Rgb8 = 0x8051; // 32849
    public const int Rgb10 = 0x8052; // 32850
    public const int Rgb12 = 0x8053; // 32851
    public const int Rgb16 = 0x8054; // 32852

    // RGBA formats
    public const int Rgba2 = 0x8055; // 32853
    public const int Rgba4 = 0x8056; // 32854
    public const int Rgba8 = 0x8058; // 32856
    public const int Rgb10A2 = 0x8059; // 32857
    public const int Rgba12 = 0x805A; // 32858
    public const int Rgba16 = 0x805B; // 32859

    // Depth formats
    public const int DepthComponent16 = 0x81A5; // 33189
    public const int DepthComponent24 = 0x81A6; // 33190
    public const int DepthComponent32 = 0x81A7; // 33191
    public const int DepthComponent32f = 0x8CAC; // 36012

    // Red and RG formats
    public const int R8 = 0x8229; // 33321
    public const int R16 = 0x822A; // 33322
    public const int Rg8 = 0x822B; // 33323
    public const int Rg16 = 0x822C; // 33324

    // Floating point formats
    public const int R16f = 0x822D; // 33325
    public const int R32f = 0x822E; // 33326
    public const int Rg16f = 0x822F; // 33327
    public const int Rg32f = 0x8230; // 33328
    public const int Rgba16f = 0x881A; // 34842
    public const int Rgb16f = 0x881B; // 34843
    public const int Rgba32f = 0x8814; // 34836
    public const int Rgb32f = 0x8815; // 34837

    // Integer formats
    public const int R8i = 0x8231; // 33329
    public const int R8ui = 0x8232; // 33330
    public const int R16i = 0x8233; // 33331
    public const int R16ui = 0x8234; // 33332
    public const int R32i = 0x8235; // 33333
    public const int R32ui = 0x8236; // 33334
    public const int Rg8i = 0x8237; // 33335
    public const int Rg8ui = 0x8238; // 33336
    public const int Rg16i = 0x8239; // 33337
    public const int Rg16ui = 0x823A; // 33338
    public const int Rg32i = 0x823B; // 33339
    public const int Rg32ui = 0x823C; // 33340

    // Depth/stencil formats
    public const int DepthStencil = 0x84F9; // 34041
    public const int Depth24Stencil8 = 0x88F0; // 35056
    public const int Depth32fStencil8 = 0x8CAD; // 36013

    // Packed formats
    public const int R11fG11fB10f = 0x8C3A; // 35898
    public const int Rgb9E5 = 0x8C3D; // 35901

    // sRGB formats
    public const int Srgb8 = 0x8C41; // 35905
    public const int Srgb8Alpha8 = 0x8C43; // 35907

    // Stencil formats
    public const int StencilIndex1 = 0x8D46; // 36166
    public const int StencilIndex1Ext = 0x8D46; // 36166
    public const int StencilIndex4 = 0x8D47; // 36167
    public const int StencilIndex4Ext = 0x8D47; // 36167
    public const int StencilIndex8 = 0x8D48; // 36168
    public const int StencilIndex8Ext = 0x8D48; // 36168
    public const int StencilIndex16 = 0x8D49; // 36169
    public const int StencilIndex16Ext = 0x8D49; // 36169

    // Integer texture formats
    public const int Rgba32ui = 0x8D70; // 36208
    public const int Rgb32ui = 0x8D71; // 36209
    public const int Rgba16ui = 0x8D76; // 36214
    public const int Rgb16ui = 0x8D77; // 36215
    public const int Rgba8ui = 0x8D7C; // 36220
    public const int Rgb8ui = 0x8D7D; // 36221
    public const int Rgba32i = 0x8D82; // 36226
    public const int Rgb32i = 0x8D83; // 36227
    public const int Rgba16i = 0x8D88; // 36232
    public const int Rgb16i = 0x8D89; // 36233
    public const int Rgba8i = 0x8D8E; // 36238
    public const int Rgb8i = 0x8D8F; // 36239

    // Special packed format
    public const int Rgb10A2ui = 0x906F; // 36975
                                         // Summary:
                                         //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_FRAMEBUFFER_UNDEFINED = 0x8219
    public const int FramebufferUndefined = 0x8219; // 33305

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_FRAMEBUFFER_COMPLETE = 0x8CD5
    public const int FramebufferComplete = 0x8CD5; // 36053

    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_FRAMEBUFFER_COMPLETE_EXT = 0x8CD5
    public const int FramebufferCompleteExt = 0x8CD5; // 36053

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 0x8CD6
    public const int FramebufferIncompleteAttachment = 0x8CD6; // 36054

    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT_EXT = 0x8CD6
    public const int FramebufferIncompleteAttachmentExt = 0x8CD6; // 36054

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 0x8CD7
    public const int FramebufferIncompleteMissingAttachment = 0x8CD7; // 36055

    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT_EXT = 0x8CD7
    public const int FramebufferIncompleteMissingAttachmentExt = 0x8CD7; // 36055

    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_DIMENSIONS_EXT = 0x8CD9
    public const int FramebufferIncompleteDimensionsExt = 0x8CD9; // 36057

    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_FORMATS_EXT = 0x8CDA
    public const int FramebufferIncompleteFormatsExt = 0x8CDA; // 36058

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER = 0x8CDB
    public const int FramebufferIncompleteDrawBuffer = 0x8CDB; // 36059

    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER_EXT = 0x8CDB
    public const int FramebufferIncompleteDrawBufferExt = 0x8CDB; // 36059

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER = 0x8CDC
    public const int FramebufferIncompleteReadBuffer = 0x8CDC; // 36060

    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER_EXT = 0x8CDC
    public const int FramebufferIncompleteReadBufferExt = 0x8CDC; // 36060

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_FRAMEBUFFER_UNSUPPORTED = 0x8CDD
    public const int FramebufferUnsupported = 0x8CDD; // 36061

    // Summary:
    //     [requires: EXT_framebuffer_object] Original was GL_FRAMEBUFFER_UNSUPPORTED_EXT = 0x8CDD
    public const int FramebufferUnsupportedExt = 0x8CDD; // 36061

    // Summary:
    //     [requires: v3.0 or ARB_framebuffer_object] Original was GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE = 0x8D56
    public const int FramebufferIncompleteMultisample = 0x8D56; // 36182

    // Summary:
    //     [requires: v3.2] Original was GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS = 0x8DA8
    public const int FramebufferIncompleteLayerTargets = 0x8DA8; // 36264

    // Summary:
    //     Original was GL_FRAMEBUFFER_INCOMPLETE_LAYER_COUNT = 0x8DA9
    public const int FramebufferIncompleteLayerCount = 0x8DA9; // 36265

    // Buffer targets
    public const int FrontLeft = 0x0400; // 1024
    public const int FrontRight = 0x0401; // 1025
    public const int BackLeft = 0x0402; // 1026
    public const int BackRight = 0x0403; // 1027
    public const int Aux0 = 0x0409; // 1033
    public const int Aux1 = 0x040A; // 1034
    public const int Aux2 = 0x040B; // 1035
    public const int Aux3 = 0x040C; // 1036

    // Attachment types
    public const int Color = 0x1800; // 6144
    public const int Depth = 0x1801; // 6145
    public const int Stencil = 0x1802; // 6146
    public const int DepthStencilAttachment = 0x821A; // 33306

    // Color attachments (0-31)
    public const int ColorAttachment0 = 0x8CE0; // 36064
    public const int ColorAttachment0Ext = 0x8CE0;
    public const int ColorAttachment1 = 0x8CE1; // 36065
    public const int ColorAttachment1Ext = 0x8CE1;
    public const int ColorAttachment2 = 0x8CE2; // 36066
    public const int ColorAttachment2Ext = 0x8CE2;
    public const int ColorAttachment3 = 0x8CE3; // 36067
    public const int ColorAttachment3Ext = 0x8CE3;
    public const int ColorAttachment4 = 0x8CE4; // 36068
    public const int ColorAttachment4Ext = 0x8CE4;
    public const int ColorAttachment5 = 0x8CE5; // 36069
    public const int ColorAttachment5Ext = 0x8CE5;
    public const int ColorAttachment6 = 0x8CE6; // 36070
    public const int ColorAttachment6Ext = 0x8CE6;
    public const int ColorAttachment7 = 0x8CE7; // 36071
    public const int ColorAttachment7Ext = 0x8CE7;
    public const int ColorAttachment8 = 0x8CE8; // 36072
    public const int ColorAttachment8Ext = 0x8CE8;
    public const int ColorAttachment9 = 0x8CE9; // 36073
    public const int ColorAttachment9Ext = 0x8CE9;
    public const int ColorAttachment10 = 0x8CEA; // 36074
    public const int ColorAttachment10Ext = 0x8CEA;
    public const int ColorAttachment11 = 0x8CEB; // 36075
    public const int ColorAttachment11Ext = 0x8CEB;
    public const int ColorAttachment12 = 0x8CEC; // 36076
    public const int ColorAttachment12Ext = 0x8CEC;
    public const int ColorAttachment13 = 0x8CED; // 36077
    public const int ColorAttachment13Ext = 0x8CED;
    public const int ColorAttachment14 = 0x8CEE; // 36078
    public const int ColorAttachment14Ext = 0x8CEE;
    public const int ColorAttachment15 = 0x8CEF; // 36079
    public const int ColorAttachment15Ext = 0x8CEF;
    public const int ColorAttachment16 = 0x8CF0; // 36080
    public const int ColorAttachment17 = 0x8CF1; // 36081
    public const int ColorAttachment18 = 0x8CF2; // 36082
    public const int ColorAttachment19 = 0x8CF3; // 36083
    public const int ColorAttachment20 = 0x8CF4; // 36084
    public const int ColorAttachment21 = 0x8CF5; // 36085
    public const int ColorAttachment22 = 0x8CF6; // 36086
    public const int ColorAttachment23 = 0x8CF7; // 36087
    public const int ColorAttachment24 = 0x8CF8; // 36088
    public const int ColorAttachment25 = 0x8CF9; // 36089
    public const int ColorAttachment26 = 0x8CFA; // 36090
    public const int ColorAttachment27 = 0x8CFB; // 36091
    public const int ColorAttachment28 = 0x8CFC; // 36092
    public const int ColorAttachment29 = 0x8CFD; // 36093
    public const int ColorAttachment30 = 0x8CFE; // 36094
    public const int ColorAttachment31 = 0x8CFF; // 36095

    // Depth/stencil attachments
    public const int DepthAttachment = 0x8D00; // 36096
    public const int DepthAttachmentExt = 0x8D00;
    public const int StencilAttachment = 0x8D20; // 36128
    public const int StencilAttachmentExt = 0x8D20;

    // Special attachments
    public const int ShadingRateAttachmentExt = 0x96D1; // 38609

    // Basic texture targets
    public const int Texture2D = 0x0DE1; // 3553
    public const int Texture3D = 0x806F; // 32879
    public const int Texture3DExt = 0x806F;
    public const int Texture3DOes = 0x806F;

    // Proxy textures (for checking texture completeness without storage)
    public const int ProxyTexture1D = 0x8063; // 32867
    public const int ProxyTexture1DExt = 0x8063;
    public const int ProxyTexture2D = 0x8064; // 32868
    public const int ProxyTexture2DExt = 0x8064;
    public const int ProxyTexture3D = 0x8070; // 32880
    public const int ProxyTexture3DExt = 0x8070;

    public const int ProxyTextureRectangle = 0x84F7; // 34039
    public const int ProxyTextureRectangleArb = 0x84F7;
    public const int ProxyTextureRectangleNv = 0x84F7;

    public const int TextureBindingCubeMap = 0x8514; // 34068

    // Cube map faces
    public const int TextureCubeMapPositiveX = 0x8515; // 34069
    public const int TextureCubeMapPositiveXArb = 0x8515;
    public const int TextureCubeMapPositiveXExt = 0x8515;
    public const int TextureCubeMapPositiveXOes = 0x8515;
    public const int TextureCubeMapNegativeX = 0x8516; // 34070
    public const int TextureCubeMapNegativeXArb = 0x8516;
    public const int TextureCubeMapNegativeXExt = 0x8516;
    public const int TextureCubeMapNegativeXOes = 0x8516;
    public const int TextureCubeMapPositiveY = 0x8517; // 34071
    public const int TextureCubeMapPositiveYArb = 0x8517;
    public const int TextureCubeMapPositiveYExt = 0x8517;
    public const int TextureCubeMapPositiveYOes = 0x8517;
    public const int TextureCubeMapNegativeY = 0x8518; // 34072
    public const int TextureCubeMapNegativeYArb = 0x8518;
    public const int TextureCubeMapNegativeYExt = 0x8518;
    public const int TextureCubeMapNegativeYOes = 0x8518;
    public const int TextureCubeMapPositiveZ = 0x8519; // 34073
    public const int TextureCubeMapPositiveZArb = 0x8519;
    public const int TextureCubeMapPositiveZExt = 0x8519;
    public const int TextureCubeMapPositiveZOes = 0x8519;
    public const int TextureCubeMapNegativeZ = 0x851A; // 34074
    public const int TextureCubeMapNegativeZArb = 0x851A;
    public const int TextureCubeMapNegativeZExt = 0x851A;
    public const int TextureCubeMapNegativeZOes = 0x851A;
    public const int ProxyTextureCubeMap = 0x851B; // 34075
    public const int ProxyTextureCubeMapArb = 0x851B;
    public const int ProxyTextureCubeMapExt = 0x851B;

    // Texture arrays
    public const int Texture1DArray = 0x8C18; // 35864
    public const int ProxyTexture1DArray = 0x8C19; // 35865
    public const int ProxyTexture1DArrayExt = 0x8C19;
    public const int Texture2DArray = 0x8C1A; // 35866
    public const int ProxyTexture2DArray = 0x8C1B; // 35867
    public const int ProxyTexture2DArrayExt = 0x8C1B;

    // Texture buffers
    public const int TextureBuffer = 0x8C2A; // 35882

    // Cube map arrays
    public const int TextureCubeMapArray = 0x9009; // 36873
    public const int TextureCubeMapArrayArb = 0x9009;
    public const int TextureCubeMapArrayExt = 0x9009;
    public const int TextureCubeMapArrayOes = 0x9009;
    public const int ProxyTextureCubeMapArray = 0x900B; // 36875
    public const int ProxyTextureCubeMapArrayArb = 0x900B;

    // Multisample textures
    public const int Texture2DMultisample = 0x9100; // 37120
    public const int ProxyTexture2DMultisample = 0x9101; // 37121
    public const int Texture2DMultisampleArray = 0x9102; // 37122
    public const int ProxyTexture2DMultisampleArray = 0x9103; // 37123

    // Special formats
    public const int DetailTexture2DSgis = 0x8095; // 32917
    public const int ProxyTexture4DSgis = 0x8135; // 33077

    // Texture parameters
    public const int TextureWidth = 4096;
    public const int TextureHeight = 4097;
    public const int TextureComponents = 4099;
    public const int TextureInternalFormat = 4099;
    public const int TextureBorderColor = 4100;
    public const int TextureBorderColorNv = 4100;
    public const int TextureBorder = 4101;

    // Texture filtering
    public const int TextureMagFilter = 10240;
    public const int TextureMinFilter = 10241;

    // Texture wrapping
    public const int TextureWrapS = 10242;
    public const int TextureWrapT = 10243;
    public const int TextureWrapR = 32882;
    public const int TextureWrapRExt = 32882;
    public const int TextureWrapROes = 32882;
    public const int ClampToBorder = 33069;
    public const int ClampToEdge = 33071;
    public const int Repeat = 10497;
    public const int MirroredRepeat = 33648;

    // Texture component sizes
    public const int TextureRedSize = 32860;
    public const int TextureGreenSize = 32861;
    public const int TextureBlueSize = 32862;
    public const int TextureAlphaSize = 32863;
    public const int TextureLuminanceSize = 32864;
    public const int TextureIntensitySize = 32865;

    // Texture management
    public const int TexturePriority = 32870;
    public const int TexturePriorityExt = 32870;
    public const int TextureResident = 32871;
    public const int TextureDepth = 32881;
    public const int TextureDepthExt = 32881;

    // Mipmapping
    public const int TextureMinLod = 33082;
    public const int TextureMinLodSgis = 33082;
    public const int TextureMaxLod = 33083;
    public const int TextureMaxLodSgis = 33083;
    public const int TextureBaseLevel = 33084;
    public const int TextureBaseLevelSgis = 33084;
    public const int TextureMaxLevel = 33085;
    public const int TextureMaxLevelSgis = 33085;
    public const int GenerateMipmapSgis = 33169;
    public const int TextureLodBias = 34049;

    // Texture comparison
    public const int TextureCompareMode = 34892;
    public const int TextureCompareFunc = 34893;
    public const int TextureCompareFailValue = 32959;
    public const int DepthTextureMode = 34891;
    public const int DepthStencilTextureMode = 37098;

    // Advanced features
    public const int TextureMaxAnisotropy = 34046;

    // Texture swizzle
    public const int TextureSwizzleR = 36418;
    public const int TextureSwizzleG = 36419;
    public const int TextureSwizzleB = 36420;
    public const int TextureSwizzleA = 36421;
    public const int TextureSwizzleRgba = 36422;

    // Vendor extensions
    public const int TextureUnnormalizedCoordinatesArm = 36714;
    public const int TextureTilingExt = 38272;
    public const int TextureFoveatedCutoffDensityQcom = 38560;
    public const int TextureYDegammaQcom = 38672;
    public const int TextureCbcrDegammaQcom = 38673;

    // SGIS extensions
    public const int DetailTextureLevelSgis = 32922;
    public const int DetailTextureModeSgis = 32923;
    public const int DetailTextureFuncPointsSgis = 32924;
    public const int SharpenTextureFuncPointsSgis = 32944;
    public const int ShadowAmbientSgix = 32959;
    public const int DualTextureSelectSgis = 33060;
    public const int QuadTextureSelectSgis = 33061;
    public const int Texture4DsizeSgis = 33078;
    public const int TextureWrapQSgis = 33079;
    public const int TextureFilter4SizeSgis = 33095;
    public const int TextureClipmapCenterSgix = 33137;
    public const int TextureClipmapFrameSgix = 33138;
    public const int TextureClipmapOffsetSgix = 33139;
    public const int TextureClipmapVirtualDepthSgix = 33140;
    public const int TextureClipmapLodOffsetSgix = 33141;
    public const int TextureClipmapDepthSgix = 33142;
    public const int PostTextureFilterBiasSgix = 33145;
    public const int PostTextureFilterScaleSgix = 33146;
    public const int TextureLodBiasSSgix = 33166;
    public const int TextureLodBiasTSgix = 33167;
    public const int TextureLodBiasRSgix = 33168;
    public const int TextureCompareSgix = 33178;
    public const int TextureCompareOperatorSgix = 33179;
    public const int TextureLequalRSgix = 33180;
    public const int TextureGequalRSgix = 33181;
    public const int TextureMaxClampSSgix = 33641;
    public const int TextureMaxClampTSgix = 33642;
    public const int TextureMaxClampRSgix = 33643;

    // Intel extension
    public const int TextureMemoryLayoutIntel = 33791;

    // Basic filtering modes
    public const int Nearest = 0x2600; // 9728
    public const int Linear = 0x2601;  // 9729

    // Detail texture filtering (SGIS extension)
    public const int LinearDetailSgis = 0x8097;       // 32919
    public const int LinearDetailAlphaSgis = 0x8098;  // 32920
    public const int LinearDetailColorSgis = 0x8099;  // 32921

    // Sharpening filters (SGIS extension)
    public const int LinearSharpenSgis = 0x80AD;       // 32941
    public const int LinearSharpenAlphaSgis = 0x80AE;  // 32942
    public const int LinearSharpenColorSgis = 0x80AF;  // 32943

    // Special filtering modes
    public const int Filter4Sgis = 0x8146;  // 33094 (SGIS 4-sample filter)

    // Pixel texture generation modes (SGIX extension)
    public const int PixelTexGenQCeilingSgix = 0x8184;  // 33156
    public const int PixelTexGenQRoundSgix = 0x8185;    // 33157
    public const int PixelTexGenQFloorSgix = 0x8186;    // 33158

    // Data types
    public const int UnsignedShort = 0x1403; // 5123
    public const int UnsignedInt = 0x1405;   // 5125

    // Color formats
    public const int ColorIndex = 0x1900;    // 6400
    public const int StencilIndex = 0x1901;  // 6401

    // Component formats
    public const int Red = 0x1903;          // 6403
    public const int RedExt = 0x1903;
    public const int Green = 0x1904;        // 6404
    public const int Blue = 0x1905;         // 6405
    public const int Alpha = 0x1906;        // 6406

    // Packed formats
    public const int Rgb = 0x1907;          // 6407
    public const int Rgba = 0x1908;         // 6408
    public const int Luminance = 0x1909;    // 6409
    public const int LuminanceAlpha = 0x190A; // 6410

    // Extended color formats
    public const int AbgrExt = 0x8000;      // 32768
    public const int CmykExt = 0x800C;      // 32780
    public const int CmykaExt = 0x800D;     // 32781
    public const int Bgr = 0x80E0;          // 32992
    public const int BgrExt = 0x80E0;
    public const int Bgra = 0x80E1;         // 32993
    public const int BgraExt = 0x80E1;
    public const int BgraImg = 0x80E1;

    // YUV formats
    public const int Ycrcb422Sgix = 0x81BB; // 33211
    public const int Ycrcb444Sgix = 0x81BC; // 33212

    // Red-green formats
    public const int Rg = 0x8227;           // 33319
    public const int RgInteger = 0x8228;    // 33320

    // ICC color profiles
    public const int R5G6B5IccSgix = 0x8466;  // 33894
    public const int R5G6B5A8IccSgix = 0x8467; // 33895
    public const int Alpha16IccSgix = 0x8468;  // 33896
    public const int Luminance16IccSgix = 0x8469; // 33897
    public const int Luminance16Alpha8IccSgix = 0x846B; // 33899


    // Integer formats
    public const int RedInteger = 0x8D94;   // 36244
    public const int GreenInteger = 0x8D95; // 36245
    public const int BlueInteger = 0x8D96;  // 36246
    public const int AlphaInteger = 0x8D97; // 36247
    public const int RgbInteger = 0x8D98;   // 36248
    public const int RgbaInteger = 0x8D99;  // 36249
    public const int BgrInteger = 0x8D9A;   // 36250
    public const int BgraInteger = 0x8D9B;  // 36251

    // Basic data types
    public const int Byte = 0x1400;           // 5120
    public const int UnsignedByte = 0x1401;   // 5121
    public const int Short = 0x1402;          // 5122
    public const int Int = 0x1404;            // 5124
    public const int Float = 0x1406;          // 5126

    // Half-float types
    public const int HalfFloat = 0x140B;      // 5131
    public const int HalfFloatArb = 0x140B;
    public const int HalfFloatNv = 0x140B;
    public const int HalfApple = 0x140B;

    // Special types
    public const int Bitmap = 0x1A00;         // 6656

    // Packed pixel formats
    public const int UnsignedByte332 = 0x8032;       // 32818
    public const int UnsignedByte332Ext = 0x8032;
    public const int UnsignedShort4444 = 0x8033;     // 32819
    public const int UnsignedShort4444Ext = 0x8033;
    public const int UnsignedShort5551 = 0x8034;     // 32820
    public const int UnsignedShort5551Ext = 0x8034;
    public const int UnsignedInt8888 = 0x8035;       // 32821
    public const int UnsignedInt8888Ext = 0x8035;
    public const int UnsignedInt1010102 = 0x8036;    // 32822
    public const int UnsignedInt1010102Ext = 0x8036;

    // Reversed packed formats
    public const int UnsignedByte233Rev = 0x8362;    // 33634
    public const int UnsignedByte233Reversed = 0x8362;
    public const int UnsignedByte233RevExt = 0x8362;
    public const int UnsignedShort565 = 0x8363;      // 33635
    public const int UnsignedShort565Ext = 0x8363;
    public const int UnsignedShort565Rev = 0x8364;   // 33636
    public const int UnsignedShort565Reversed = 0x8364;
    public const int UnsignedShort565RevExt = 0x8364;
    public const int UnsignedShort4444Rev = 0x8365;  // 33637
    public const int UnsignedShort4444Reversed = 0x8365;
    public const int UnsignedShort4444RevExt = 0x8365;
    public const int UnsignedShort4444RevImg = 0x8365;
    public const int UnsignedShort1555Rev = 0x8366;  // 33638
    public const int UnsignedShort1555Reversed = 0x8366;
    public const int UnsignedShort1555RevExt = 0x8366;
    public const int UnsignedInt8888Rev = 0x8367;    // 33639
    public const int UnsignedInt8888Reversed = 0x8367;
    public const int UnsignedInt8888RevExt = 0x8367;
    public const int UnsignedInt2101010Rev = 0x8368; // 33640
    public const int UnsignedInt2101010Reversed = 0x8368;
    public const int UnsignedInt2101010RevExt = 0x8368;

    // Depth/stencil formats
    public const int UnsignedInt248 = 0x84FA;        // 34042
    public const int UnsignedInt248Ext = 0x84FA;
    public const int UnsignedInt248Nv = 0x84FA;
    public const int UnsignedInt248Oes = 0x84FA;

    // Special floating point formats
    public const int UnsignedInt10F11F11FRev = 0x8C3B;   // 35899
    public const int UnsignedInt10F11F11FRevApple = 0x8C3B;
    public const int UnsignedInt10F11F11FRevExt = 0x8C3B;
    public const int UnsignedInt5999Rev = 0x8C3E;        // 35902
    public const int UnsignedInt5999RevApple = 0x8C3E;
    public const int UnsignedInt5999RevExt = 0x8C3E;

    // Advanced depth formats
    public const int Float32UnsignedInt248Rev = 0x8DAD;  // 36269
    public const int Float32UnsignedInt248RevNv = 0x8DAD;

    public const int Texture0 = 0x84C0;
    public const int Texture1 = 0x84C1;
    public const int Texture2 = 0x84C2;
    public const int Texture3 = 0x84C3;
    public const int Texture4 = 0x84C4;
    public const int Texture5 = 0x84C5;
    public const int Texture6 = 0x84C6;
    public const int Texture7 = 0x84C7;
    public const int Texture8 = 0x84C8;
    public const int Texture9 = 0x84C9;
    public const int Texture10 = 0x84CA;
    public const int Texture11 = 0x84CB;
    public const int Texture12 = 0x84CC;
    public const int Texture13 = 0x84CD;
    public const int Texture14 = 0x84CE;
    public const int Texture15 = 0x84CF;
    public const int Texture16 = 0x84D0;
    public const int Texture17 = 0x84D1;
    public const int Texture18 = 0x84D2;
    public const int Texture19 = 0x84D3;
    public const int Texture20 = 0x84D4;
    public const int Texture21 = 0x84D5;
    public const int Texture22 = 0x84D6;
    public const int Texture23 = 0x84D7;
    public const int Texture24 = 0x84D8;
    public const int Texture25 = 0x84D9;
    public const int Texture26 = 0x84DA;
    public const int Texture27 = 0x84DB;
    public const int Texture28 = 0x84DC;
    public const int Texture29 = 0x84DD;
    public const int Texture30 = 0x84DE;
    public const int Texture31 = 0x84DF;

    public const int VertexShader = 0x8B31;
    public const int FragmentShader = 0x8B30;
    public const int CompileStatus = 0x8B81;
    public const int LinkStatus = 0x8B82;
    public const int InfoLogLength = 0x8B84;

    public const int ArrayBuffer = 0x8892;
    public const int ElementArrayBuffer = 0x8893;
    public const int StaticDraw = 0x88E4;
    public const int DynamicDraw = 0x88E8;
    public const int StreamDraw = 0x88E0;

    // ===== Primitive Types (Constants) =====
    public const int Points = 0x0000;
    public const int Lines = 0x0001;
    public const int LineLoop = 0x0002;
    public const int LineStrip = 0x0003;
    public const int Triangles = 0x0004;
    public const int TriangleStrip = 0x0005;
    public const int TriangleFan = 0x0006;

    public const int Front = 0x0404;
    public const int Back = 0x0405;
    public const int FrontAndBack = 0x0408;
}