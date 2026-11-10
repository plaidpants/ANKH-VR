// Originally From 
// http://www.shaderslab.com/demo-94---wireframe-without-diagonal.html
// License information
// http://www.shaderslab.com/index.html
// All the shaders of this website are totally free-to-use.
// Feel free to use them in any project without any credits.

Shader "Custom/Geometry/Wireframe"
{
	Properties
	{
		[PowerSlider(3.0)]
		_WireframeVal("Wireframe width", Range(0., 0.5)) = 0.05
		_FrontColor("Front color", color) = (1., 1., 1., 1.)
		_BackColor("Back color", color) = (1., 1., 1., 1.)
	}
		SubShader
		{
			Tags { "Queue" = "Geometry" "RenderType" = "Opaque" }

			Pass
			{
				Cull Front
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma geometry geom

			#include "UnityCG.cginc"

			struct v2g {
				float4 worldPos : SV_POSITION;
			};

			struct g2f {
				float4 pos : SV_POSITION;
				float3 bary : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			v2g vert(appdata_base v) {
				v2g o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream) {
				float3 param = float3(0., 0., 0.);

				float EdgeA = length(IN[0].worldPos - IN[1].worldPos);
				float EdgeB = length(IN[1].worldPos - IN[2].worldPos);
				float EdgeC = length(IN[2].worldPos - IN[0].worldPos);

				if (EdgeA > EdgeB && EdgeA > EdgeC)
					param.y = 1.;
				else if (EdgeB > EdgeC && EdgeB > EdgeA)
					param.x = 1.;
				else
					param.z = 1.;

				g2f o;
				o.pos = mul(UNITY_MATRIX_VP, IN[0].worldPos);
				o.bary = float3(1., 0., 0.) + param;
				o.worldPos = IN[0].worldPos.xyz;
				triStream.Append(o);
				o.pos = mul(UNITY_MATRIX_VP, IN[1].worldPos);
				o.bary = float3(0., 0., 1.) + param;
				o.worldPos = IN[1].worldPos.xyz;
				triStream.Append(o);
				o.pos = mul(UNITY_MATRIX_VP, IN[2].worldPos);
				o.bary = float3(0., 1., 0.) + param;
				o.worldPos = IN[2].worldPos.xyz;
				triStream.Append(o);
			}

			float _WireframeVal;
			fixed4 _BackColor;
			float _TabletopClipEnabled;
			float4x4 _TabletopWorldToLocal;
			float4 _TabletopClipHalfSize;

			fixed4 frag(g2f i) : SV_Target {
			if (!any(bool3(i.bary.x < _WireframeVal, i.bary.y < _WireframeVal, i.bary.z < _WireframeVal)))
				 discard;
			if (_TabletopClipEnabled > 0.5)
			{
				float3 boardPos = mul(_TabletopWorldToLocal, float4(i.worldPos, 1.0)).xyz;
				float distX = _TabletopClipHalfSize.x - abs(boardPos.x);
				float distZ = _TabletopClipHalfSize.z - abs(boardPos.z);
				clip(min(distX, distZ));
				clip(boardPos.y - _TabletopClipHalfSize.y);
			}

				return _BackColor;
			}

			ENDCG
		}

		Pass
		{
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

				#include "UnityCG.cginc"

				struct v2g {
					float4 worldPos : SV_POSITION;
				};

				struct g2f {
					float4 pos : SV_POSITION;
					float3 bary : TEXCOORD0;
					float3 worldPos : TEXCOORD1;
				};

				v2g vert(appdata_base v) {
					v2g o;
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					return o;
				}

				[maxvertexcount(3)]
				void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream) {
					float3 param = float3(0., 0., 0.);

					float EdgeA = length(IN[0].worldPos - IN[1].worldPos);
					float EdgeB = length(IN[1].worldPos - IN[2].worldPos);
					float EdgeC = length(IN[2].worldPos - IN[0].worldPos);

					if (EdgeA > EdgeB && EdgeA > EdgeC)
						param.y = 1.;
					else if (EdgeB > EdgeC && EdgeB > EdgeA)
						param.x = 1.;
					else
						param.z = 1.;

					g2f o;
					o.pos = mul(UNITY_MATRIX_VP, IN[0].worldPos);
					o.bary = float3(1., 0., 0.) + param;
					o.worldPos = IN[0].worldPos.xyz;
					triStream.Append(o);
					o.pos = mul(UNITY_MATRIX_VP, IN[1].worldPos);
					o.bary = float3(0., 0., 1.) + param;
					o.worldPos = IN[1].worldPos.xyz;
					triStream.Append(o);
					o.pos = mul(UNITY_MATRIX_VP, IN[2].worldPos);
					o.bary = float3(0., 1., 0.) + param;
					o.worldPos = IN[2].worldPos.xyz;
					triStream.Append(o);
				}

				float _WireframeVal;
				fixed4 _FrontColor;
				float _TabletopClipEnabled;
				float4x4 _TabletopWorldToLocal;
				float4 _TabletopClipHalfSize;

				fixed4 frag(g2f i) : SV_Target {
				if (!any(bool3(i.bary.x <= _WireframeVal, i.bary.y <= _WireframeVal, i.bary.z <= _WireframeVal)))
					 discard;
				if (_TabletopClipEnabled > 0.5)
				{
					float3 boardPos = mul(_TabletopWorldToLocal, float4(i.worldPos, 1.0)).xyz;
					float distX = _TabletopClipHalfSize.x - abs(boardPos.x);
					float distZ = _TabletopClipHalfSize.z - abs(boardPos.z);
					clip(min(distX, distZ));
					clip(boardPos.y - _TabletopClipHalfSize.y);
				}

					return _FrontColor;
				}

				ENDCG
			}
		}
}