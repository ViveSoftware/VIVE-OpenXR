// Made with Amplify Shader Editor
Shader "Wave/Essence/Hand/Model"
{
	Properties
	{
		[HDR]_GraColorA("GradianColorA", Color) = (0.1058824,0.6901961,0.9019608,0)
		[HDR]_GraColorB("GradianColorB", Color) = (1,1,1,0)
		[HDR]_ConGraColorA("ContourColorA", Color) = (0.1058824,0.6901961,0.9019608,0)
		[HDR]_ConGraColorB("ContourColorB", Color) = (1,1,1,0)
		_Gradient_Blur("Gradient_Blur", Range(0 , 1)) = 0.2854134
		_Gradient_level("Gradient_level", Range(0 , 1)) = 0.5225084
		_Opacity("Opacity", Range(0 , 1)) = 0.45
		_line_opacity("line_opacity", Range(0 , 1)) = 0.5
		_OutlineThickness("OutlineThickness", Range(0 , 0.002)) = 0.001
		_AlphaText("AlphaText", 2D) = "white" {}
		[HideInInspector] _texcoord3("", 2D) = "white" {}
		[HideInInspector] _texcoord("", 2D) = "white" {}
		[HideInInspector] __dirty("", Int) = 1
	}

		SubShader
		{
			Pass
			{
				ColorMask 0
				ZWrite On
			}

			Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0"}
			Cull Front
			CGPROGRAM
			#pragma target 3.0
			#pragma surface outlineSurf Outline nofog alpha:fade  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 




			struct Input
			{
				float2 uv3_texcoord3;
				float2 uv_texcoord;
			};
			uniform float _OutlineThickness;
			uniform float4 _ConGraColorA;
			uniform float4 _ConGraColorB;
			uniform float _Gradient_level;
			uniform float _Gradient_Blur;
			uniform float _line_opacity;
			uniform sampler2D _AlphaText;

			void outlineVertexDataFunc(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				float outlineVar = _OutlineThickness;
				v.vertex.xyz += (v.normal * outlineVar);
			}
			inline half4 LightingOutline(SurfaceOutput s, half3 lightDir, half atten) { return half4 (0,0,0, s.Alpha); }
			void outlineSurf(Input i, inout SurfaceOutput o)
			{
				float clampResult114 = clamp((_Gradient_level + ((_Gradient_level - i.uv3_texcoord3.y) / _Gradient_Blur)) , 0.0 , 1.0);
				float4 lerpResult100 = lerp(_ConGraColorA, _ConGraColorB, clampResult114);
				float4 tex2DNode92 = tex2D(_AlphaText, i.uv_texcoord);
				o.Emission = lerpResult100.rgb;
				o.Alpha = (_line_opacity * tex2DNode92).r;
			}
			ENDCG


			Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma target 3.0
			#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
			struct Input
			{
				float2 uv3_texcoord3;
				float2 uv_texcoord;
			};

			uniform float4 _GraColorA;
			uniform float4 _GraColorB;
			uniform float _Gradient_level;
			uniform float _Gradient_Blur;
			uniform float _Opacity;
			uniform sampler2D _AlphaText;

			void vertexDataFunc(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				v.vertex.xyz += 0;
			}

			inline half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
			{
				return half4 (0, 0, 0, s.Alpha);
			}

			void surf(Input i , inout SurfaceOutput o)
			{
				float clampResult114 = clamp((_Gradient_level + ((_Gradient_level - i.uv3_texcoord3.y) / _Gradient_Blur)) , 0.0 , 1.0);
				float4 lerpResult100 = lerp(_GraColorA , _GraColorB , clampResult114);
				o.Emission = lerpResult100.rgb;
				float4 color104 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float4 color102 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float smoothstepResult103 = smoothstep(-0.05 , 1.0 , i.uv3_texcoord3.y);
				float4 lerpResult105 = lerp(color104 , color102 , smoothstepResult103);
				float4 tex2DNode92 = tex2D(_AlphaText, i.uv_texcoord);
				o.Alpha = (lerpResult105 * _Opacity * tex2DNode92).r;
			}

			ENDCG
		}
			CustomEditor "ASEMaterialInspector"
}
