Shader "GPU/VoxelDataShader"
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
		//Lighting On
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
			StructuredBuffer<float> data;
			int res;
			float3 offset;
			float spacing;
			struct v2f
			{
				float4 vertex : SV_POSITION;
				uint id : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			uint3 idToGridCoords(uint id)
			{
				uint3 coords;
				int resSq = res * res;
				coords.z = id / resSq;
				id -= coords.z * resSq;
				coords.y = id / res;
				coords.x = id % res;
				return coords;
			}

			v2f vert(uint id : SV_VertexID, uint instanceId : SV_InstanceID)
			{
				v2f o;
				uint3 gridCoords = idToGridCoords(instanceId);
				o.vertex = UnityObjectToClipPos(spacing*gridCoords + offset);
				o.id = instanceId;
				return o;
			}

			fixed4 frag(float4 i:SV_POSITION, uint id : TEXCOORD1) : SV_Target
			{
				// sample the texture
				//fixed4 col = {1,0,0,1};// tex2D(_MainTex, i.uv);
				//fixed4 col;
				//col.rgb = i.normal;
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				float newval = (data[id]+1)/2;
				half3 lightColor = {newval,1-newval, 0.0f};//ShadeVertexLights(i.vertex, i.normal);
				return half4(lightColor, 1.0f);
			}
				//return col;
			ENDCG
		}
	}
}
