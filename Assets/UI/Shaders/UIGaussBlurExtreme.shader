// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// modified by leon
Shader "Custom/UIGaussBlurExtreme" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _BumpAmt("Distortion", Range(0,128)) = 10
        _MainTex("Tint Color (RGB)", 2D) = "white" {}
        _BumpMap("Normalmap", 2D) = "bump" {}
        _Size("Size", Range(0, 20)) = 1
    }

        Category{

            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }


            SubShader {
                Blend SrcAlpha OneMinusSrcAlpha
                GrabPass {
                    // Tags { "LightMode" = "Always"  }
                }
                Pass {
                    Tags { "LightMode" = "Always" }
                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float4 texcoord: TEXCOORD0;
                        float4 color    : COLOR;     //    The color value of this vertex specifically. //  Name&type must be the same!
                    };

                    struct v2f {
                        float4 vertex : POSITION;
                        float4 uvgrab : TEXCOORD0;
                        float4 color    : TEXCOORD1;
                    };

                    v2f vert(appdata_t v) {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                        o.uvgrab.zw = o.vertex.zw;
                        o.color = v.color;
                        return o;
                    }

                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;
                    float _Size;

                    float4 frag(v2f i) : COLOR {

                        float4 sum = float4(0,0,0,0);
                        #define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx*_Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight

                        sum += GRABPIXEL(0.000664777262, -20.0);
sum += GRABPIXEL(0.001030915004, -19.0);
sum += GRABPIXEL(0.001563140222, -18.0);
sum += GRABPIXEL(0.002317402070, -17.0);
sum += GRABPIXEL(0.003359179544, -16.0);
sum += GRABPIXEL(0.004760947775, -15.0);
sum += GRABPIXEL(0.006597539397, -14.0);
sum += GRABPIXEL(0.008939205910, -13.0);
sum += GRABPIXEL(0.011842523745, -12.0);
sum += GRABPIXEL(0.015339738685, -11.0);
sum += GRABPIXEL(0.019427639350, -10.0);
sum += GRABPIXEL(0.024057499101, -9.0);
sum += GRABPIXEL(0.029127908247, -8.0);
sum += GRABPIXEL(0.034482321103, -7.0);
sum += GRABPIXEL(0.039912787485, -6.0);
sum += GRABPIXEL(0.045170614823, -5.0);
sum += GRABPIXEL(0.049983690434, -4.0);
sum += GRABPIXEL(0.054079044369, -3.0);
sum += GRABPIXEL(0.057208172319, -2.0);
sum += GRABPIXEL(0.059171899636, -1.0);
sum += GRABPIXEL(0.059841342060, 0.0);
sum += GRABPIXEL(0.059171899636, 1.0);
sum += GRABPIXEL(0.057208172319, 2.0);
sum += GRABPIXEL(0.054079044369, 3.0);
sum += GRABPIXEL(0.049983690434, 4.0);
sum += GRABPIXEL(0.045170614823, 5.0);
sum += GRABPIXEL(0.039912787485, 6.0);
sum += GRABPIXEL(0.034482321103, 7.0);
sum += GRABPIXEL(0.029127908247, 8.0);
sum += GRABPIXEL(0.024057499101, 9.0);
sum += GRABPIXEL(0.019427639350, 10.0);
sum += GRABPIXEL(0.015339738685, 11.0);
sum += GRABPIXEL(0.011842523745, 12.0);
sum += GRABPIXEL(0.008939205910, 13.0);
sum += GRABPIXEL(0.006597539397, 14.0);
sum += GRABPIXEL(0.004760947775, 15.0);
sum += GRABPIXEL(0.003359179544, 16.0);
sum += GRABPIXEL(0.002317402070, 17.0);
sum += GRABPIXEL(0.001563140222, 18.0);
sum += GRABPIXEL(0.001030915004, 19.0);
sum += GRABPIXEL(0.000664777262, 20.0);


//sum *= i.color;
sum.a *= i.color.a;
return sum;
}
ENDCG
}

GrabPass {
    Tags { "LightMode" = "Always" }
}
Pass {
    Tags { "LightMode" = "Always" }

    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma fragmentoption ARB_precision_hint_fastest
    #include "UnityCG.cginc"

    struct appdata_t {
        float4 vertex : POSITION;
        float2 texcoord: TEXCOORD0;
        float4 color    : TEXCOORD1;     //    The color value of this vertex specifically. //  Name&type must be the same!
    };

    struct v2f {
        float4 vertex : POSITION;
        float4 uvgrab : TEXCOORD0;
        float4 color    : TEXCOORD1;
    };

    v2f vert(appdata_t v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        #if UNITY_UV_STARTS_AT_TOP
        float scale = -1.0;
        #else
        float scale = 1.0;
        #endif
        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
        o.uvgrab.zw = o.vertex.zw;
        o.color = v.color;
        return o;
    }

    sampler2D _GrabTexture;
    float4 _GrabTexture_TexelSize;
    float _Size;

    float4 frag(v2f i) : COLOR {

        float4 sum = float4(0,0,0,0);
        #define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*_Size, i.uvgrab.z, i.uvgrab.w))) * weight

        //sum += GRABPIXEL(0.05, -4.0);
        //sum += GRABPIXEL(0.09, -3.0);
        //sum += GRABPIXEL(0.12, -2.0);
        //sum += GRABPIXEL(0.15, -1.0);
        //sum += GRABPIXEL(0.18,  0.0);
        //sum += GRABPIXEL(0.15, +1.0);
        //sum += GRABPIXEL(0.12, +2.0);
        //sum += GRABPIXEL(0.09, +3.0);
        //sum += GRABPIXEL(0.05, +4.0);

        sum += GRABPIXEL(0.000664777262, -20.0);
sum += GRABPIXEL(0.001030915004, -19.0);
sum += GRABPIXEL(0.001563140222, -18.0);
sum += GRABPIXEL(0.002317402070, -17.0);
sum += GRABPIXEL(0.003359179544, -16.0);
sum += GRABPIXEL(0.004760947775, -15.0);
sum += GRABPIXEL(0.006597539397, -14.0);
sum += GRABPIXEL(0.008939205910, -13.0);
sum += GRABPIXEL(0.011842523745, -12.0);
sum += GRABPIXEL(0.015339738685, -11.0);
sum += GRABPIXEL(0.019427639350, -10.0);
sum += GRABPIXEL(0.024057499101, -9.0);
sum += GRABPIXEL(0.029127908247, -8.0);
sum += GRABPIXEL(0.034482321103, -7.0);
sum += GRABPIXEL(0.039912787485, -6.0);
sum += GRABPIXEL(0.045170614823, -5.0);
sum += GRABPIXEL(0.049983690434, -4.0);
sum += GRABPIXEL(0.054079044369, -3.0);
sum += GRABPIXEL(0.057208172319, -2.0);
sum += GRABPIXEL(0.059171899636, -1.0);
sum += GRABPIXEL(0.059841342060, 0.0);
sum += GRABPIXEL(0.059171899636, 1.0);
sum += GRABPIXEL(0.057208172319, 2.0);
sum += GRABPIXEL(0.054079044369, 3.0);
sum += GRABPIXEL(0.049983690434, 4.0);
sum += GRABPIXEL(0.045170614823, 5.0);
sum += GRABPIXEL(0.039912787485, 6.0);
sum += GRABPIXEL(0.034482321103, 7.0);
sum += GRABPIXEL(0.029127908247, 8.0);
sum += GRABPIXEL(0.024057499101, 9.0);
sum += GRABPIXEL(0.019427639350, 10.0);
sum += GRABPIXEL(0.015339738685, 11.0);
sum += GRABPIXEL(0.011842523745, 12.0);
sum += GRABPIXEL(0.008939205910, 13.0);
sum += GRABPIXEL(0.006597539397, 14.0);
sum += GRABPIXEL(0.004760947775, 15.0);
sum += GRABPIXEL(0.003359179544, 16.0);
sum += GRABPIXEL(0.002317402070, 17.0);
sum += GRABPIXEL(0.001563140222, 18.0);
sum += GRABPIXEL(0.001030915004, 19.0);
sum += GRABPIXEL(0.000664777262, 20.0);


                        return sum;
                    }
                    ENDCG
                }

                GrabPass {
                    Tags { "LightMode" = "Always" }
                }
                Pass {
                    Tags { "LightMode" = "Always" }

                    CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord: TEXCOORD0;
                        float4 color    : COLOR;     //    The color value of this vertex specifically. //  Name&type must be the same!
                    };

                    struct v2f {
                        float4 vertex : POSITION;
                        float4 uvgrab : TEXCOORD0;
                        float2 uvbump : TEXCOORD1;
                        float2 uvmain : TEXCOORD2;
                        float4  color : TEXCOORD3;
                    };

                    float _BumpAmt;
                    float4 _BumpMap_ST;
                    float4 _MainTex_ST;

                    v2f vert(appdata_t v) {
                        v2f o;
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                        #else
                        float scale = 1.0;
                        #endif
                        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                        o.uvgrab.zw = o.vertex.zw;
                        o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap);
                        o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
                        o.color = v.color;
                        return o;
                    }

                    fixed4 _Color;
                    sampler2D _GrabTexture;
                    float4 _GrabTexture_TexelSize;
                    sampler2D _BumpMap;
                    sampler2D _MainTex;

                    float4 frag(v2f i) : COLOR {

                        half2 bump = UnpackNormal(tex2D(_BumpMap, i.uvbump)).rg;
                        float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
                        i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;

                        float4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                        float4 tint = tex2D(_MainTex, i.uvmain) * _Color;

                        return col * tint;
                        //return i.color;
                    }
                    ENDCG
                }
            }
        }
}