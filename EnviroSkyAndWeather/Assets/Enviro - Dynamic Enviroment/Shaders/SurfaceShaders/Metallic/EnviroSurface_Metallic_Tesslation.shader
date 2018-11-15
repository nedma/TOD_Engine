// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Enviro/EnviroSurfaceShader/Metallic_Tesslation" {
    Properties {
 		_Albedo ("Albedo", 2D) = "white" {}
        _AlbedoColor ("Albedo Color", Color) = (1,1,1,1)
        _Metallic ("Metallic (R) and Smoothness (A)", 2D) = "black" {}
        [Normal]_NormalMap ("Normal Map", 2D) = "bump" {}
     	_Occlusion ("Occlusion", 2D) = "white" {}
        _OcclusionStrenght ("Occlusion Strenght", Range(0, 1)) = 0.07361673
        _HeightMap ("Height Map (RGB)", 2D) = "white" {}
        _TesslationAmount ("Tesslation Amount", Float ) = 1
        _TesslationStrenght ("Tesslation Strenght", Float ) = 0.1
   		_DetailIntensity ("Detail Intensity", Range(0, 1)) = 0
        _DetailAlbedo ("Detail Albedo (RGB)", 2D) = "white" {}
        _DetailNormal ("Detail Normal", 2D) = "bump" {}
        _Raining ("Raining", Range(0, 1)) = 0
        _Smoothness_dry ("Smoothness Dry", Range(-1, 1)) = 0
        _Smoothness_wet ("Smoothness Wet", Range(-1, 1)) = 0.5811966
        [Normal]_WetNormal ("Wet Normal Map", 2D) = "bump" {}
        [HideInInspector]_Ripple ("Ripple", 2D) = "bump" {}
        _SnowStrenght ("Snow Strenght", Range(0, 1)) = 0
        _SnowAmount ("Snow Amount", Float ) = 0.1
        _SnowAlbedo ("Snow Albedo (RGB)", 2D) = "white" {}
        [Normal]_SnowNormal ("Snow Normal", 2D) = "bump" {}
        [NoScaleOffset]_Mask ("Mask (R) Detail,(G) Snow, (B) Wet", 2D) = "white" {}
        [HideInInspector]_NormalClear ("NormalClear", 2D) = "bump" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma target 5.0
            #pragma glsl
            uniform sampler2D _WetNormal; uniform float4 _WetNormal_ST;
            uniform float _Smoothness_dry;
            uniform sampler2D _Ripple; uniform float4 _Ripple_ST;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float4 _AlbedoColor;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform sampler2D _SnowAlbedo; uniform float4 _SnowAlbedo_ST;
            uniform sampler2D _SnowNormal; uniform float4 _SnowNormal_ST;
            uniform float _SnowStrenght;
            uniform sampler2D _Mask;
            uniform sampler2D _NormalClear; uniform float4 _NormalClear_ST;
            uniform float _Raining;
            uniform float _SnowAmount;
            uniform sampler2D _Metallic; uniform float4 _Metallic_ST;
            uniform float _Smoothness_wet;
            uniform sampler2D _DetailNormal; uniform float4 _DetailNormal_ST;
            uniform sampler2D _DetailAlbedo; uniform float4 _DetailAlbedo_ST;
            uniform sampler2D _Occlusion; uniform float4 _Occlusion_ST;
            uniform float _OcclusionStrenght;
            uniform sampler2D _HeightMap; uniform float4 _HeightMap_ST;
            uniform float _TesslationStrenght;
            uniform float _TesslationAmount;
            uniform float _DetailIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 _Mask_var = tex2Dlod(_Mask,float4(o.uv0,0.0,0));
                float3 node_1603 = normalize(v.normal);
                v.vertex.xyz += (lerp(float3(0,0,0),float3(_Mask_var.g,_Mask_var.g,_Mask_var.g),(_SnowStrenght*_SnowAmount))*node_1603);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float3 node_1603 = normalize(v.normal);
                    float4 _HeightMap_var = tex2Dlod(_HeightMap,float4(TRANSFORM_TEX(v.texcoord1, _HeightMap),0.0,0));
                    v.vertex.xyz += ((node_1603*_HeightMap_var.rgb)*_TesslationStrenght);
                }
                float Tessellation(TessVertex v){
                    return _TesslationAmount;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 _DetailNormal_var = UnpackNormal(tex2D(_DetailNormal,TRANSFORM_TEX(i.uv0, _DetailNormal)));
                float3 _NormalClear_var = UnpackNormal(tex2D(_NormalClear,TRANSFORM_TEX(i.uv0, _NormalClear)));
                float4 _Mask_var = tex2D(_Mask,i.uv0);
                float node_8317 = (_DetailIntensity*_Mask_var.r);
                float3 node_498_nrm_base = _NormalMap_var.rgb + float3(0,0,1);
                float3 node_498_nrm_detail = lerp(_DetailNormal_var.rgb,_NormalClear_var.rgb,node_8317) * float3(-1,-1,1);
                float3 node_498_nrm_combined = node_498_nrm_base*dot(node_498_nrm_base, node_498_nrm_detail)/node_498_nrm_base.z - node_498_nrm_detail;
                float3 node_498 = node_498_nrm_combined;
                float3 _WetNormal_var = UnpackNormal(tex2D(_WetNormal,TRANSFORM_TEX(i.uv0, _WetNormal)));
                float3 _Ripple_var = UnpackNormal(tex2D(_Ripple,TRANSFORM_TEX(i.uv0, _Ripple)));
                float3 node_4233_nrm_base = _WetNormal_var.rgb + float3(0,0,1);
                float3 node_4233_nrm_detail = _Ripple_var.rgb * float3(-1,-1,1);
                float3 node_4233_nrm_combined = node_4233_nrm_base*dot(node_4233_nrm_base, node_4233_nrm_detail)/node_4233_nrm_base.z - node_4233_nrm_detail;
                float3 node_4233 = node_4233_nrm_combined;
                float3 node_980_nrm_base = node_498 + float3(0,0,1);
                float3 node_980_nrm_detail = lerp(_NormalClear_var.rgb,node_4233,(_Raining*_Mask_var.b)) * float3(-1,-1,1);
                float3 node_980_nrm_combined = node_980_nrm_base*dot(node_980_nrm_base, node_980_nrm_detail)/node_980_nrm_base.z - node_980_nrm_detail;
                float3 node_980 = node_980_nrm_combined;
                float3 _SnowNormal_var = UnpackNormal(tex2D(_SnowNormal,TRANSFORM_TEX(i.uv0, _SnowNormal)));
                float3 normalLocal = lerp(node_980,lerp(node_980,_SnowNormal_var.rgb,_Mask_var.g),_SnowStrenght);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform ));
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
                float4 _Metallic_var = tex2D(_Metallic,TRANSFORM_TEX(i.uv0, _Metallic));
                float node_1938 = (-1.0);
                float gloss = (_Metallic_var.a*lerp(lerp(_Smoothness_dry,_Smoothness_wet,(_Raining*_Mask_var.b)),node_1938,_SnowStrenght));
                float specPow = exp2( gloss * 10.0+1.0);
                
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                d.boxMax[0] = unity_SpecCube0_BoxMax;
                d.boxMin[0] = unity_SpecCube0_BoxMin;
                d.probePosition[0] = unity_SpecCube0_ProbePosition;
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.boxMax[1] = unity_SpecCube1_BoxMax;
                d.boxMin[1] = unity_SpecCube1_BoxMin;
                d.probePosition[1] = unity_SpecCube1_ProbePosition;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;

                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float3 node_6517 = (_Albedo_var.rgb*_AlbedoColor.rgb);
                float4 _DetailAlbedo_var = tex2D(_DetailAlbedo,TRANSFORM_TEX(i.uv0, _DetailAlbedo));
                float3 node_6374 = lerp(node_6517,_DetailAlbedo_var.rgb,node_8317);
                float4 _SnowAlbedo_var = tex2D(_SnowAlbedo,TRANSFORM_TEX(i.uv0, _SnowAlbedo));
                float3 diffuseColor = lerp(node_6374,lerp(node_6374,_SnowAlbedo_var.rgb,_SnowStrenght),_Mask_var.g);
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, lerp(_Metallic_var.r,_SnowAlbedo_var.a,_SnowStrenght), specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * UNITY_PI / 4.0);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);

                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; 
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; 
                float4 _Occlusion_var = tex2D(_Occlusion,TRANSFORM_TEX(i.uv0, _Occlusion));
                indirectDiffuse *= pow(_Occlusion_var.r,saturate((_OcclusionStrenght+(_SnowStrenght*node_1938)))); 
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;

                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma target 5.0
            #pragma glsl
            uniform sampler2D _WetNormal; uniform float4 _WetNormal_ST;
            uniform float _Smoothness_dry;
            uniform sampler2D _Ripple; uniform float4 _Ripple_ST;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float4 _AlbedoColor;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform sampler2D _SnowAlbedo; uniform float4 _SnowAlbedo_ST;
            uniform sampler2D _SnowNormal; uniform float4 _SnowNormal_ST;
            uniform float _SnowStrenght;
            uniform sampler2D _Mask;
            uniform sampler2D _NormalClear; uniform float4 _NormalClear_ST;
            uniform float _Raining;
            uniform float _SnowAmount;
            uniform sampler2D _Metallic; uniform float4 _Metallic_ST;
            uniform float _Smoothness_wet;
            uniform sampler2D _DetailNormal; uniform float4 _DetailNormal_ST;
            uniform sampler2D _DetailAlbedo; uniform float4 _DetailAlbedo_ST;
            uniform sampler2D _HeightMap; uniform float4 _HeightMap_ST;
            uniform float _TesslationStrenght;
            uniform float _TesslationAmount;
            uniform float _DetailIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                LIGHTING_COORDS(6,7)
                UNITY_FOG_COORDS(8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 _Mask_var = tex2Dlod(_Mask,float4(o.uv0,0.0,0));
                float3 node_1603 = normalize(v.normal);
                v.vertex.xyz += (lerp(float3(0,0,0),float3(_Mask_var.g,_Mask_var.g,_Mask_var.g),(_SnowStrenght*_SnowAmount))*node_1603);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float3 node_1603 = normalize(v.normal);
                    float4 _HeightMap_var = tex2Dlod(_HeightMap,float4(TRANSFORM_TEX(v.texcoord1, _HeightMap),0.0,0));
                    v.vertex.xyz += ((node_1603*_HeightMap_var.rgb)*_TesslationStrenght);
                }
                float Tessellation(TessVertex v){
                    return _TesslationAmount;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 _DetailNormal_var = UnpackNormal(tex2D(_DetailNormal,TRANSFORM_TEX(i.uv0, _DetailNormal)));
                float3 _NormalClear_var = UnpackNormal(tex2D(_NormalClear,TRANSFORM_TEX(i.uv0, _NormalClear)));
                float4 _Mask_var = tex2D(_Mask,i.uv0);
                float node_8317 = (_DetailIntensity*_Mask_var.r);
                float3 node_498_nrm_base = _NormalMap_var.rgb + float3(0,0,1);
                float3 node_498_nrm_detail = lerp(_DetailNormal_var.rgb,_NormalClear_var.rgb,node_8317) * float3(-1,-1,1);
                float3 node_498_nrm_combined = node_498_nrm_base*dot(node_498_nrm_base, node_498_nrm_detail)/node_498_nrm_base.z - node_498_nrm_detail;
                float3 node_498 = node_498_nrm_combined;
                float3 _WetNormal_var = UnpackNormal(tex2D(_WetNormal,TRANSFORM_TEX(i.uv0, _WetNormal)));
                float3 _Ripple_var = UnpackNormal(tex2D(_Ripple,TRANSFORM_TEX(i.uv0, _Ripple)));
                float3 node_4233_nrm_base = _WetNormal_var.rgb + float3(0,0,1);
                float3 node_4233_nrm_detail = _Ripple_var.rgb * float3(-1,-1,1);
                float3 node_4233_nrm_combined = node_4233_nrm_base*dot(node_4233_nrm_base, node_4233_nrm_detail)/node_4233_nrm_base.z - node_4233_nrm_detail;
                float3 node_4233 = node_4233_nrm_combined;
                float3 node_980_nrm_base = node_498 + float3(0,0,1);
                float3 node_980_nrm_detail = lerp(_NormalClear_var.rgb,node_4233,(_Raining*_Mask_var.b)) * float3(-1,-1,1);
                float3 node_980_nrm_combined = node_980_nrm_base*dot(node_980_nrm_base, node_980_nrm_detail)/node_980_nrm_base.z - node_980_nrm_detail;
                float3 node_980 = node_980_nrm_combined;
                float3 _SnowNormal_var = UnpackNormal(tex2D(_SnowNormal,TRANSFORM_TEX(i.uv0, _SnowNormal)));
                float3 normalLocal = lerp(node_980,lerp(node_980,_SnowNormal_var.rgb,_Mask_var.g),_SnowStrenght);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); 
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
                float4 _Metallic_var = tex2D(_Metallic,TRANSFORM_TEX(i.uv0, _Metallic));
                float node_1938 = (-1.0);
                float gloss = (_Metallic_var.a*lerp(lerp(_Smoothness_dry,_Smoothness_wet,(_Raining*_Mask_var.b)),node_1938,_SnowStrenght));
                float specPow = exp2( gloss * 10.0+1.0);
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float3 node_6517 = (_Albedo_var.rgb*_AlbedoColor.rgb);
                float4 _DetailAlbedo_var = tex2D(_DetailAlbedo,TRANSFORM_TEX(i.uv0, _DetailAlbedo));
                float3 node_6374 = lerp(node_6517,_DetailAlbedo_var.rgb,node_8317);
                float4 _SnowAlbedo_var = tex2D(_SnowAlbedo,TRANSFORM_TEX(i.uv0, _SnowAlbedo));
                float3 diffuseColor = lerp(node_6374,lerp(node_6374,_SnowAlbedo_var.rgb,_SnowStrenght),_Mask_var.g);
                float specularMonochrome;
                float3 specularColor;
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, lerp(_Metallic_var.r,_SnowAlbedo_var.a,_SnowStrenght), specularColor, specularMonochrome );
                specularMonochrome = 1-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));
                float specularPBL = max(0, (NdotL*visTerm*normTerm) * UNITY_PI / 4.0 );
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;

                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma target 5.0
            #pragma glsl
            uniform float _SnowStrenght;
            uniform sampler2D _Mask;
            uniform float _SnowAmount;
            uniform sampler2D _HeightMap; uniform float4 _HeightMap_ST;
            uniform float _TesslationStrenght;
            uniform float _TesslationAmount;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 _Mask_var = tex2Dlod(_Mask,float4(o.uv0,0.0,0));
                float3 node_1603 = normalize(v.normal);
                v.vertex.xyz += (lerp(float3(0,0,0),float3(_Mask_var.g,_Mask_var.g,_Mask_var.g),(_SnowStrenght*_SnowAmount))*node_1603);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float2 texcoord0 : TEXCOORD0;
                    float2 texcoord1 : TEXCOORD1;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float3 node_1603 = normalize(v.normal);
                    float4 _HeightMap_var = tex2Dlod(_HeightMap,float4(TRANSFORM_TEX(v.texcoord1, _HeightMap),0.0,0));
                    v.vertex.xyz += ((node_1603*_HeightMap_var.rgb)*_TesslationStrenght);
                }
                float Tessellation(TessVertex v){
                    return _TesslationAmount;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
