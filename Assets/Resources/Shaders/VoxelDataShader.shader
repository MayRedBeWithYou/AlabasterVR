Shader "GPU/VoxelDataShader"
{
	Properties
	{
		_InsideColor("Inside Color", Color) = (0,1,0,1)
		_OutsideColor("Outside Color", Color) = (1,0,0,1)
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Cull Off
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
			half4 _InsideColor;
			half4 _OutsideColor;

			struct v2f
			{
				float4 vertex : SV_POSITION;
				uint id : TEXCOORD1;
			};

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
				float newval = ((data[id]/spacing)+1)/2; // transform [-spacing,spacing] interval into  [0,1] interval for lerp puprose
				return lerp(_InsideColor, _OutsideColor, newval);
			}
			ENDCG
		}
	}
}
