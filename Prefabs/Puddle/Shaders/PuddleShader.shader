Shader "Custom/PuddleShader"
{
    Properties {
        _Color("Color",Color) = (1.0,1.0,1.0,1.0)
    }
    
    SubShader {
        Tags { "Queue" = "transparent" } 
            // draw after all opaque geometry has been drawn
        Stencil {
            Ref 2
            Comp always
            Pass replace
        }
        Pass {
            ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects

            Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

            CGPROGRAM 
 
            #pragma vertex vert 
            #pragma fragment frag

            uniform float4 _Color;
 
            float4 vert(float4 vertexPos : POSITION) : SV_POSITION 
            {
                return UnityObjectToClipPos(vertexPos);
            }
 
            float4 frag(void) : COLOR 
            {
                return _Color; 
            }
 
            ENDCG  
        }
    }
}