// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SurvivalTemplatePro/HealthIndicator"
{
	Properties
	{
		[HDR]_HealthColor("Health Color", Color) = (0.3410663,0.754717,0.1459594,1)
		[HDR]_EmptyColor("Empty Color", Color) = (0.08627451,0.154902,0.05098039,0.3921569)
		_HealthAmount("Health Amount", Range( 0 , 1)) = 0
		_Albedo("Albedo", 2D) = "black" {}
		[HideInInspector]_Alpha("Alpha", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
		};

		uniform float4 _EmptyColor;
		uniform float4 _HealthColor;
		uniform sampler2D _Albedo;
		uniform float _HealthAmount;
		uniform float _Alpha;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 panner28 = ( _Time.y * float2( 0,1 ) + ase_screenPosNorm.xy);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 temp_cast_1 = ((ase_vertex3Pos.x*1.0 + ( _HealthAmount + -0.5 ))).xxxx;
			float4 temp_output_33_0 = ( (0.75 + (tex2D( _Albedo, panner28 ).r - 0.0) * (1.0 - 0.75) / (1.0 - 0.0)) * saturate( CalculateContrast(500.0,temp_cast_1) ) );
			float4 lerpResult5 = lerp( _EmptyColor , _HealthColor , temp_output_33_0);
			o.Emission = lerpResult5.rgb;
			float lerpResult13 = lerp( _EmptyColor.a , _HealthColor.a , temp_output_33_0.r);
			o.Alpha = ( lerpResult13 * _Alpha );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=15800
224;418;1520;904;711.6945;952.6661;1.462183;True;True
Node;AmplifyShaderEditor.RangedFloatNode;4;-1163.356,-554.9043;Float;False;Property;_HealthAmount;Health Amount;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;8;-1065.313,-387.8693;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;26;-662.876,-609.2073;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-820.8853,-518.7441;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;62;-748.5198,-853.9481;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;71;-634.8372,-464.0405;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;28;-437.2181,-792.9823;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;66;-432.606,-445.5649;Float;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;500;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;21;-258.2426,-853.6631;Float;True;Property;_Albedo;Albedo;3;0;Create;True;0;0;False;0;2373dd8e3fd70db48886625c9c632524;2373dd8e3fd70db48886625c9c632524;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;11;-238.8066,-367.0743;Float;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;64;85.86538,-632.4767;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.75;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;66.04297,-349.1617;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;3;-558.9733,-70.93234;Float;False;Property;_EmptyColor;Empty Color;1;1;[HDR];Create;True;0;0;False;0;0.08627451,0.154902,0.05098039,0.3921569;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-568.2291,138.9971;Float;False;Property;_HealthColor;Health Color;0;1;[HDR];Create;True;0;0;False;0;0.3410663,0.754717,0.1459594,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;74;291.0717,1.491313;Float;False;Property;_Alpha;Alpha;4;1;[HideInInspector];Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;13;347.841,148.6294;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;5;423.3155,-152.515;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;594.8442,45.0184;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;78;455.1278,-528.6329;Float;False;Constant;_Color0;Color 0;5;0;Create;True;0;0;False;0;0.490566,0.490566,0.490566,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;757.0436,-227.8978;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SurvivalTemplatePro/HealthIndicator;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;72;0;4;0
WireConnection;71;0;8;1
WireConnection;71;2;72;0
WireConnection;28;0;62;0
WireConnection;28;1;26;0
WireConnection;66;1;71;0
WireConnection;21;1;28;0
WireConnection;11;0;66;0
WireConnection;64;0;21;1
WireConnection;33;0;64;0
WireConnection;33;1;11;0
WireConnection;13;0;3;4
WireConnection;13;1;1;4
WireConnection;13;2;33;0
WireConnection;5;0;3;0
WireConnection;5;1;1;0
WireConnection;5;2;33;0
WireConnection;77;0;13;0
WireConnection;77;1;74;0
WireConnection;0;2;5;0
WireConnection;0;9;77;0
ASEEND*/
//CHKSM=19CCB7E7918FCEE5E23942EC671BBDAFB9CF1E1C