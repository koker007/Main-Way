Shader "Custom/BlockShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        //_Shadow ("Albedo (RGB)", 2D) = "white" {}
        _ShadowU0 ("ShadowU0", Range(0,1)) = 0.0
        _ShadowU1 ("ShadowU1", Range(0,1)) = 0.0
        _ShadowV0 ("ShadowV0", Range(0,1)) = 0.0
        _ShadowV1 ("ShadowV1", Range(0,1)) = 0.0

        _Transparent ("Transparent", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}//{ "RenderType"="Opaque" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        //#pragma surface surf Lambert //fullforwardshadows
        #pragma surface surf NoLighting noambient //alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        //sampler2D _Shadow;

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float2 uv2_Shadow : TEXCOORD1;
        };

        fixed4 _Color;
        half _ShadowU0;
        half _ShadowU1;
        half _ShadowV0;
        half _ShadowV1;

        half _Transparent;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            half shadowU = lerp(_ShadowU0, _ShadowU1, IN.uv_MainTex.x);
            half shadowV = lerp(_ShadowV0, _ShadowV1, IN.uv_MainTex.y);
            half shadow = shadowU;
            if(shadow > shadowV)
                shadow = shadowV;

            c = lerp(c, float4(0,0,0,1), shadow);

            o.Albedo = c.rgb;
            o.Alpha = _Transparent;
        }

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            return fixed4(s.Albedo, s.Alpha);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
