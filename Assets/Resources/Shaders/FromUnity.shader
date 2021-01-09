Shader "Instanced/InstancedShader" {
    Properties{
        _MainTex("Albedo (RGB)", 2D) = "white" {}
    }
        SubShader{

            Pass {

                Tags {"LightMode" = "ForwardBase"}

                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
                #pragma target 4.5

                #include "UnityCG.cginc"
                #include "UnityLightingCommon.cginc"
                #include "AutoLight.cginc"

                sampler2D _MainTex;

            #if SHADER_TARGET >= 45
                StructuredBuffer<float3> data;
            #endif
                float4x4 model;
                float4x4 invModel;


                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv_MainTex : TEXCOORD0;
                    float3 ambient : TEXCOORD1;
                    float3 diffuse : TEXCOORD2;
                    float3 color : TEXCOORD3;
                    SHADOW_COORDS(4)
                };

                v2f vert(appdata_full v, uint instanceID : SV_InstanceID, uint id : SV_VertexID)
                {
                #if SHADER_TARGET >= 45
                    float4 pos = float4(data[instanceID * 9 + id + 0],1);
                    float3 normal = data[instanceID * 9 + id + 3];
                    float3 color = data[instanceID * 9 + id + 6];
                #else
                    float4 pos = 0;
                    float3 normal = 0;
                    float3 color = 0;
                #endif
                    pos = mul(model, pos);
                    normal = mul(transpose((float3x3)invModel), normal);
                    normal = UnityObjectToWorldNormal(normal);
                    float3 worldNormal = normal;

                    float3 ndotl = saturate(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                    float3 ambient = ShadeSH9(float4(worldNormal, 1.0f));
                    float3 diffuse = (ndotl * _LightColor0.rgb);

                    v2f o;
                    o.pos = UnityObjectToClipPos(pos);
                    o.uv_MainTex = v.texcoord;
                    o.ambient = ambient;
                    o.diffuse = diffuse;
                    o.color = color;
                    TRANSFER_SHADOW(o)
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed shadow = SHADOW_ATTENUATION(i);
                    fixed4 albedo = tex2D(_MainTex, i.uv_MainTex);
                    float3 lighting = i.diffuse * shadow + i.ambient;
                    fixed4 output = fixed4(albedo.rgb * i.color * lighting, albedo.w);
                    UNITY_APPLY_FOG(i.fogCoord, output);
                    return output;
                }

                ENDCG
            }
    }
}