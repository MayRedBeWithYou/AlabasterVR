Shader "Custom/NewApproach"
{
//#pragma target 5.0

    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        #pragma target 5.0

        sampler2D _MainTex;

#ifdef SHADER_API_D3D11 
        StructuredBuffer<float3> data;
#endif
        float4x4 model;
        float4x4 invModel;

        struct Input
        {
            float2 uv_MainTex;
            half4 color;
        };
        //float4 vertex    : POSITION;  // The vertex position in model space.
        //float3 normal    : NORMAL;    // The vertex normal in model space.
        //float4 texcoord  : TEXCOORD0; // The first UV coordinate.
        //float4 texcoord1 : TEXCOORD1; // The second UV coordinate.
        //float4 tangent   : TANGENT;   // The tangent vector in Model Space (used for normal mapping).
        //float4 color     : COLOR;
        struct appdata
        {
            float3 normal : POSITION;
            float4 vertex : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 color : COLOR;
            uint id : SV_VertexID;
            uint instanceId : SV_InstanceID;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata i, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
#ifdef SHADER_API_D3D11
            uint id = i.id;
            uint instanceId = i.instanceId;
            float3 apos;
            float3 anorm;
            float3 acolor;

            apos = data[instanceId * 9 + id];
            anorm = data[instanceId * 9 + id + 3];
            acolor = data[instanceId * 9 + id + 6];
//#else
//            apos = float3(0,0,0);
//            anorm = float3(0,0,0);
//            acolor = float3(0,0,0);
//#endif
            float4 pos = float4(apos, 1);
            pos = mul(model, pos);
            i.vertex = pos; // UnityObjectToClipPos(pos);
            i.normal = mul(transpose((float3x3)invModel), anorm);
            i.normal = UnityObjectToWorldNormal(i.normal);
            half4 lightColor = half4(ShadeVertexLights(i.vertex, i.normal), 1);
            i.color = half4(acolor, 1);//half4(ShadeVertexLights(o.vertex, o.normal), 1);
            i.color = clamp(half4(0, 0, 0, 0), lightColor, i.color);
            o.color = i.color;
#endif
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;//_Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
