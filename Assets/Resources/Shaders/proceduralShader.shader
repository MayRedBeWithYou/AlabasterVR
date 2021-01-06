Shader "GPU/ProceduralShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		//Cull Off
		Cull Back
		Lighting On
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#pragma target 5.0

			#include "UnityCG.cginc"
			StructuredBuffer<float3> data;
			float4x4 model;
			float4x4 invModel;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				half4 color : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(uint id : SV_VertexID, uint instanceId : SV_InstanceID)
			{
				v2f o;
				float4 pos = float4(data[instanceId * 9 + id], 1);
				pos = mul(model, pos);
				o.vertex = UnityObjectToClipPos(pos);
				o.normal = mul(transpose((float3x3)invModel),data[instanceId * 9 + id + 3]);
				o.normal = UnityObjectToWorldNormal(o.normal);
				half4 lightColor = half4(ShadeVertexLights(o.vertex, o.normal), 1);
				o.color = half4(data[instanceId * 9 + id + 6],1);//half4(ShadeVertexLights(o.vertex, o.normal), 1);
				o.color = clamp(half4(0, 0, 0, 0), lightColor, o.color);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
