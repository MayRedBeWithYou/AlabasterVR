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
			float3 offset;
			float4x4 modelMatrix;
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(uint id : SV_VertexID, uint instanceId : SV_InstanceID)
			{
				v2f o;
				//o.vertex = float4(0, 0, 0, 1);
				//if (id == 0)
				//{
				//    o.vertex = UnityObjectToClipPos(float3(0, 0, 0));
				//}
				//if (id == 1)
				//{
				//    o.vertex = UnityObjectToClipPos(float3(1, 0, 0));
				//}
				//if (id == 2)
				//{
				//    o.vertex = UnityObjectToClipPos(float3(0, 1, 0));
				//}
				//o.vertex = float4(data[id],1);
				o.vertex = UnityObjectToClipPos(data[instanceId * 6 + id] + offset);
				o.normal = UnityObjectToWorldNormal(data[instanceId * 6 + id + 3]);
				//o.normal = float3(1, 1, 1);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = {1,0,0,1};// tex2D(_MainTex, i.uv);
				//fixed4 col;
				//col.rgb = i.normal;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);

				half3 lightColor = ShadeVertexLights(i.vertex, i.normal);
				return half4(lightColor, 1);
				//return col;
			}
			ENDCG
		}
	}
}
