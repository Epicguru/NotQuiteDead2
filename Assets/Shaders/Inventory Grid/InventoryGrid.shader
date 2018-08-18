Shader "Hidden/InventoryGrid"
{
	Properties
	{
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1, 1, 1, 1)

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15

		_SizeX ("Size X", Int) = 5
		_SizeY ("Size Y", Int) = 5
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			int _SizeX, _SizeY;
			float _GreenCells[128];
			fixed4 _Color;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv * float2(_SizeX, _SizeY);
				fixed4 col = tex2D(_MainTex, uv);

				int x = (int)uv.x;
				int y = _SizeY - 1 - (int)uv.y;

				float index = x + y * _SizeX + 1;
				bool contained = false;
				for (int i = 0; i < 128; i++) {
					if (_GreenCells[i] == index) {
						contained = true;
						break;
					}
				}
				if (contained) {
					col.rgb *= half4(0.0, 1.0, 0.0, 1.0);
				}
				col.rgba *= _Color;

				return col;
			}
			ENDCG
		}
	}
}
