Shader "Custom/TerrainSplatShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		// Splat Map Control Texture
		[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}

		// Textures
		[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}

		// Normal Maps
		[HideInInspector] _Normal0("Normal 0 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal1("Normal 1 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal2("Normal 2 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal3("Normal 3 (R)", 2D) = "bump" {}

    }
    SubShader
    {
        Tags { "SplatCount" = "4"
				"Queue" = "Geometry-100"
				"RenderType" = "Opaque"
				}
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.0

        sampler2D _Control;
		sampler2D _Splat0;
		sampler2D _Splat1;
		sampler2D _Splat2;
		sampler2D _Splat3;

        struct Input
        {
            float2 uv_Control : TEXCORD0;
			float2 uv_Splat0 : TEXCORD1;
			float2 uv_Splat1 : TEXCORD2;
			float2 uv_Splat2 : TEXCORD3;
			float2 uv_Splat3 : TEXCORD4;
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

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_Control, IN.uv_Control);
			fixed3 col;
			col = c.r * tex2D(_Splat0, IN.uv_Splat0).rgb;
			col += c.g * tex2D(_Splat1, IN.uv_Splat1).rgb;
			col += c.b * tex2D(_Splat2, IN.uv_Splat2).rgb;
			col += c.a * tex2D(_Splat3, IN.uv_Splat3).rgb;

			o.Albedo = col * _Color;
            //o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
