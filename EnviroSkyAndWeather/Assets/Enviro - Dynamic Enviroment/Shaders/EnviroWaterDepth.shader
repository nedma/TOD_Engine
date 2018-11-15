Shader "Enviro/Depth" {
	Properties {

	}
	SubShader {

		Tags { "RenderType"="Opaque" }
		LOD 200

		Colormask 0

		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 2.0

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			o.Alpha = 0;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
