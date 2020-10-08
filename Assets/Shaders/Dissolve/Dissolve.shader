
Shader "Ultimate 10+ Shaders/Dissolve"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _Cutoff ("Cut off", Range(0, 1)) = 0.25
        _EdgeWidth ("Edge Width", Range(0, 1)) = 0.05
        [HDR] _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Geometry" "Queue"="Transparent" }
		LOD 200
        Cull [_Cull]

        CGPROGRAM
        #pragma surface surf Standard addshadow fullforwardshadows

        #ifndef SHADER_API_D3D11
            #pragma target 3.0
        #else
            #pragma target 4.0
        #endif

        sampler2D _MainTex;
        sampler2D _NoiseTex;

        half _Cutoff;
        half _EdgeWidth;

        fixed4 _Color;
        fixed4 _EdgeColor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NoiseTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 noisePixel, pixel;
        half cutoff;
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            pixel = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = pixel.rgb;

            noisePixel = tex2D (_NoiseTex, IN.uv_NoiseTex);

            clip(noisePixel.r >= _Cutoff ? 1 : -1);
            o.Emission = noisePixel.r >= (_Cutoff * (_EdgeWidth + 1.0)) ? 0 : _EdgeColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
