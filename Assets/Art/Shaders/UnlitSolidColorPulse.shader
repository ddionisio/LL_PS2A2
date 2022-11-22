// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Game/Unlit Solid Color Pulse"
{
    Properties
    {
        _ColorStart ("Color Start Tint", Color) = (1,1,1,1)
		_ColorEnd ("Color End Tint", Color) = (1,1,1,1)
		_PulsePerSecond ("Pulse Per Second", Float) = 1
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
        Lighting Off
        //ZTest LEqual
        //ZWrite On
        Cull Back
        //Blend SrcAlpha OneMinusSrcAlpha
		Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _ColorStart;
			float4 _ColorEnd;
			float _PulsePerSecond;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            half4 frag (v2f i) : COLOR
            {
				float t = sin(3.1415 * _Time.y * _PulsePerSecond);
				t *= t;
				
                return lerp(_ColorStart, _ColorEnd, t);
            }

            ENDCG
        }
    }
}
