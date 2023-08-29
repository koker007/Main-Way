Shader "Custom/ChankBasic" {
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _llluminationTex ("lllumination Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
    }
    
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1; // UV2 координаты
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1; // UV2 координаты
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _llluminationTex;
            sampler2D _NormalMap;
            
            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = v.uv2; // Передача UV2 координат
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // Сэмплирование текстур по координатам
                fixed4 mainTexColor = tex2D(_MainTex, i.uv);
                fixed4 secondaryTexColor = tex2D(_llluminationTex, i.uv2); // Использование UV2 координат
                
                // Сэмплирование нормалей из текстурного канала
                fixed3 normal = UnpackNormal(tex2D(_NormalMap, i.uv)).rgb * 2.0f - 1.0f;
                
                fixed3 finalColor = lerp(float4(0,0,0,0), mainTexColor, secondaryTexColor.r);
                // Определение цвета и нормали
                //fixed3 finalColor = mainTexColor.rgb * secondaryTexColor.rgb;
                return fixed4(finalColor, 1.0f);
            }
            ENDCG
        }
    }
}
