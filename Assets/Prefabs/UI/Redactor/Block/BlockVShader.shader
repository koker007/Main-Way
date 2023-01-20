Shader "Custom/BlockVShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ShadowTex ("Albedo (RGB)", 2D) = "white" {}
        _ShadowDown ("ShadowDown", Range(0,1)) = 0.0
        _ShadowUp ("ShadowUp", Range(0,1)) = 0.0
        _ShadowLeft ("ShadowLeft", Range(0,1)) = 0.0
        _ShadowRight ("ShadowRight", Range(0,1)) = 0.0
        _ShadowFace ("ShadowFace", Range(0,1)) = 0.0
        _ShadowBack ("ShadowBack", Range(0,1)) = 0.0
        _LightVec ("LightVec", Color) = (1,1,1,1)

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
        sampler2D _Shadow;

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float2 uv2_Shadow : TEXCOORD1;
        };

        fixed4 _Color;
        half _ShadowDown;
        half _ShadowUp;
        half _ShadowLeft;
        half _ShadowRight;
        half _ShadowFace;
        half _ShadowBack;
        float4 _LightVec;

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

            float size16X = 1.0f/16.0f;
            float uvX = (IN.uv_MainTex.x%size16X)/size16X;
            float uvZ = (IN.uv_MainTex.x/size16X)/16.0f;

            half shadowLR = lerp(_ShadowLeft, _ShadowRight, uvX);
            half shadowDU = lerp(_ShadowDown, _ShadowUp, IN.uv_MainTex.y);
            half shadowFB = lerp(_ShadowFace, _ShadowBack, uvZ);
            half shadow = shadowDU;
            if(shadow > shadowLR)
                shadow = shadowLR;
            if(shadow > shadowFB)
                shadow = shadowFB;

            float3 lightDir = float3(_ShadowLeft - _ShadowRight, _ShadowDown - _ShadowUp, _ShadowFace - _ShadowBack);

            float difLight = max(0, dot (o.Normal, lightDir));
            float difShadow = min(1, dot(o.Normal*-1,lightDir));
            float standartLight = 1 - shadow;

            c = lerp(float4(0,0,0,1), c, standartLight);

            o.Albedo = c.rgb * (standartLight + (difLight - difShadow)/2);
            o.Alpha = _Transparent;
        }

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            return fixed4(s.Albedo, s.Alpha);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
