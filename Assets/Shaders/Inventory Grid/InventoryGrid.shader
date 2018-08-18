Shader "Hidden/InventoryGrid"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SizeX ("Size X", Int) = 5
		_SizeY ("Size Y", Int) = 5
	}
	SubShader
	{
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

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv * float2(_SizeX, _SizeY);
				fixed4 col = tex2D(_MainTex, uv);

				int x = (int)uv.x;
				int y = (int)uv.y;

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
				col.rgb *= col.a;

				return col;
			}
			ENDCG
		}
	}
}
