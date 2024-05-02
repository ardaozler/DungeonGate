Shader "UI/FireUIShader"
{
    Properties
    {
        _Speed("Speed", Vector) = (0, -0.5, 0, 0)
        [NoScaleOffset]_FireTexture("FireTexture", 2D) = "white" {}
        [HDR]_Color("Color", Color) = (10.68063, 10.68063, 10.68063, 1)
        _Edge("Edge", Vector) = (0.8, 0.8, 0.8, 0.8)
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
        }
        Pass
        {
            Name "Sprite Unlit"
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 VertexColor;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float2 _Speed;
        float4 _FireTexture_TexelSize;
        float4 _Color;
        float4 _Edge;
        CBUFFER_END
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_FireTexture);
        SAMPLER(sampler_FireTexture);
        
        // Graph Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        float2 Unity_Voronoi_RandomVector_Deterministic_float (float2 UV, float offset)
        {
            Hash_Tchou_2_2_float(UV, UV);
            return float2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;
        }
        
        void Unity_Voronoi_Deterministic_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
        {
            float2 g = floor(UV * CellDensity);
            float2 f = frac(UV * CellDensity);
            float t = 8.0;
            float3 res = float3(8.0, 0.0, 0.0);
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    float2 lattice = float2(x, y);
                    float2 offset = Unity_Voronoi_RandomVector_Deterministic_float(lattice + g, AngleOffset);
                    float d = distance(lattice + offset, f);
                    if (d < res.x)
                    {
                        res = float3(d, offset.x, offset.y);
                        Out = res.x;
                        Cells = res.y;
                    }
                }
            }
        }
        
        float Unity_SimpleNoise_ValueNoise_Deterministic_float (float2 uv)
        {
            float2 i = floor(uv);
            float2 f = frac(uv);
            f = f * f * (3.0 - 2.0 * f);
            uv = abs(frac(uv) - 0.5);
            float2 c0 = i + float2(0.0, 0.0);
            float2 c1 = i + float2(1.0, 0.0);
            float2 c2 = i + float2(0.0, 1.0);
            float2 c3 = i + float2(1.0, 1.0);
            float r0; Hash_Tchou_2_1_float(c0, r0);
            float r1; Hash_Tchou_2_1_float(c1, r1);
            float r2; Hash_Tchou_2_1_float(c2, r2);
            float r3; Hash_Tchou_2_1_float(c3, r3);
            float bottomOfGrid = lerp(r0, r1, f.x);
            float topOfGrid = lerp(r2, r3, f.x);
            float t = lerp(bottomOfGrid, topOfGrid, f.y);
            return t;
        }
        
        void Unity_SimpleNoise_Deterministic_float(float2 UV, float Scale, out float Out)
        {
            float freq, amp;
            Out = 0.0f;
            freq = pow(2.0, float(0));
            amp = pow(0.5, float(3-0));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(1));
            amp = pow(0.5, float(3-1));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
            freq = pow(2.0, float(2));
            amp = pow(0.5, float(3-2));
            Out += Unity_SimpleNoise_ValueNoise_Deterministic_float(float2(UV.xy*(Scale/freq)))*amp;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Step_float4(float4 Edge, float4 In, out float4 Out)
        {
            Out = step(Edge, In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_232ec18f8f0549538458fee0f3b99ab1_Out_0_Vector4 = IsGammaSpace() ? LinearToSRGB(_Color) : _Color;
            float4 _Multiply_5073baf1d66549f6af6bb4b8ced0031f_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_232ec18f8f0549538458fee0f3b99ab1_Out_0_Vector4, IN.VertexColor, _Multiply_5073baf1d66549f6af6bb4b8ced0031f_Out_2_Vector4);
            float4 _Property_d9ef75e9241640a7a9689f2620811a29_Out_0_Vector4 = _Edge;
            float4 _UV_13a0fb6e004d49958c2ff8b97a92b101_Out_0_Vector4 = IN.uv0;
            float _Split_b63397adcbdc410da04a829997603c92_R_1_Float = _UV_13a0fb6e004d49958c2ff8b97a92b101_Out_0_Vector4[0];
            float _Split_b63397adcbdc410da04a829997603c92_G_2_Float = _UV_13a0fb6e004d49958c2ff8b97a92b101_Out_0_Vector4[1];
            float _Split_b63397adcbdc410da04a829997603c92_B_3_Float = _UV_13a0fb6e004d49958c2ff8b97a92b101_Out_0_Vector4[2];
            float _Split_b63397adcbdc410da04a829997603c92_A_4_Float = _UV_13a0fb6e004d49958c2ff8b97a92b101_Out_0_Vector4[3];
            float _OneMinus_2196e8fe4774428bb8545ca03be49c40_Out_1_Float;
            Unity_OneMinus_float(_Split_b63397adcbdc410da04a829997603c92_G_2_Float, _OneMinus_2196e8fe4774428bb8545ca03be49c40_Out_1_Float);
            float _Power_141851bff3634e10b23ee27a8f647bd5_Out_2_Float;
            Unity_Power_float(_OneMinus_2196e8fe4774428bb8545ca03be49c40_Out_1_Float, 2, _Power_141851bff3634e10b23ee27a8f647bd5_Out_2_Float);
            UnityTexture2D _Property_33b5fca787a14e7ca9857b79e35bb830_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_FireTexture);
            float4 _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_33b5fca787a14e7ca9857b79e35bb830_Out_0_Texture2D.tex, _Property_33b5fca787a14e7ca9857b79e35bb830_Out_0_Texture2D.samplerstate, _Property_33b5fca787a14e7ca9857b79e35bb830_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_R_4_Float = _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_RGBA_0_Vector4.r;
            float _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_G_5_Float = _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_RGBA_0_Vector4.g;
            float _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_B_6_Float = _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_RGBA_0_Vector4.b;
            float _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_A_7_Float = _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_RGBA_0_Vector4.a;
            float4 _Multiply_186d19953d1a4e29bd104c2d0fa86cd5_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Power_141851bff3634e10b23ee27a8f647bd5_Out_2_Float.xxxx), _SampleTexture2D_1d0b4c954c714a099447045a18e52f0f_RGBA_0_Vector4, _Multiply_186d19953d1a4e29bd104c2d0fa86cd5_Out_2_Vector4);
            float2 _Property_dca3e0dc62c94bad8ab6cb688fa8bea5_Out_0_Vector2 = _Speed;
            float2 _Multiply_b17aac60f0bf4eb39360f91d651ba66b_Out_2_Vector2;
            Unity_Multiply_float2_float2((IN.TimeParameters.x.xx), _Property_dca3e0dc62c94bad8ab6cb688fa8bea5_Out_0_Vector2, _Multiply_b17aac60f0bf4eb39360f91d651ba66b_Out_2_Vector2);
            float2 _TilingAndOffset_bfb3ae01d55c40b7834e7f3534a6380c_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Multiply_b17aac60f0bf4eb39360f91d651ba66b_Out_2_Vector2, _TilingAndOffset_bfb3ae01d55c40b7834e7f3534a6380c_Out_3_Vector2);
            float _Voronoi_2d3d510d90db4fdd9e6f8d38980e469f_Out_3_Float;
            float _Voronoi_2d3d510d90db4fdd9e6f8d38980e469f_Cells_4_Float;
            Unity_Voronoi_Deterministic_float(_TilingAndOffset_bfb3ae01d55c40b7834e7f3534a6380c_Out_3_Vector2, IN.TimeParameters.x, 5, _Voronoi_2d3d510d90db4fdd9e6f8d38980e469f_Out_3_Float, _Voronoi_2d3d510d90db4fdd9e6f8d38980e469f_Cells_4_Float);
            float _SimpleNoise_765fc77fd2624b11a479a2c524a042ed_Out_2_Float;
            Unity_SimpleNoise_Deterministic_float(_TilingAndOffset_bfb3ae01d55c40b7834e7f3534a6380c_Out_3_Vector2, 50, _SimpleNoise_765fc77fd2624b11a479a2c524a042ed_Out_2_Float);
            float _Split_327fa7941e7146eab4a5c8decf604a68_R_1_Float = _SimpleNoise_765fc77fd2624b11a479a2c524a042ed_Out_2_Float;
            float _Split_327fa7941e7146eab4a5c8decf604a68_G_2_Float = 0;
            float _Split_327fa7941e7146eab4a5c8decf604a68_B_3_Float = 0;
            float _Split_327fa7941e7146eab4a5c8decf604a68_A_4_Float = 0;
            float _Multiply_2e6f0d902a1e4d6f8b5d3b11a09bac21_Out_2_Float;
            Unity_Multiply_float_float(_Voronoi_2d3d510d90db4fdd9e6f8d38980e469f_Out_3_Float, _Split_327fa7941e7146eab4a5c8decf604a68_R_1_Float, _Multiply_2e6f0d902a1e4d6f8b5d3b11a09bac21_Out_2_Float);
            float4 _Add_4a8499243d2648759218b9766416c729_Out_2_Vector4;
            Unity_Add_float4(_Multiply_186d19953d1a4e29bd104c2d0fa86cd5_Out_2_Vector4, (_Multiply_2e6f0d902a1e4d6f8b5d3b11a09bac21_Out_2_Float.xxxx), _Add_4a8499243d2648759218b9766416c729_Out_2_Vector4);
            float4 _Step_3db6b1d47ccf44609273f001af14de1b_Out_2_Vector4;
            Unity_Step_float4(_Property_d9ef75e9241640a7a9689f2620811a29_Out_0_Vector4, _Add_4a8499243d2648759218b9766416c729_Out_2_Vector4, _Step_3db6b1d47ccf44609273f001af14de1b_Out_2_Vector4);
            float4 _Multiply_08cb504f619b446ca9daf5a1c0ca62e0_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Multiply_5073baf1d66549f6af6bb4b8ced0031f_Out_2_Vector4, _Step_3db6b1d47ccf44609273f001af14de1b_Out_2_Vector4, _Multiply_08cb504f619b446ca9daf5a1c0ca62e0_Out_2_Vector4);
            float _Split_fdc0b21999f744598ce9b292a8d7b4eb_R_1_Float = _Step_3db6b1d47ccf44609273f001af14de1b_Out_2_Vector4[0];
            float _Split_fdc0b21999f744598ce9b292a8d7b4eb_G_2_Float = _Step_3db6b1d47ccf44609273f001af14de1b_Out_2_Vector4[1];
            float _Split_fdc0b21999f744598ce9b292a8d7b4eb_B_3_Float = _Step_3db6b1d47ccf44609273f001af14de1b_Out_2_Vector4[2];
            float _Split_fdc0b21999f744598ce9b292a8d7b4eb_A_4_Float = _Step_3db6b1d47ccf44609273f001af14de1b_Out_2_Vector4[3];
            surface.BaseColor = (_Multiply_08cb504f619b446ca9daf5a1c0ca62e0_Out_2_Vector4.xyz);
            surface.Alpha = _Split_fdc0b21999f744598ce9b292a8d7b4eb_A_4_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
            output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}