// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SurvivalTemplatePro/Interactable_Standard"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0.8962264,0.7633126,0.2832413,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0.01
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Color("Base Color", Color) = (1,1,1,0)
		_MainTex("Base Color Map", 2D) = "white" {}
		_MetallicGlossMap("Metallic & Smoothness", 2D) = "white" {}
		[Normal]_BumpMap("Normal Map", 2D) = "bump" {}
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		_LineMap("Line Map", 2D) = "white" {}
		[HDR]_LineColor("Line Color", Color) = (1.991077,1.306363,0.3513666,1)
		_LineAlpha("Line Alpha", Range( 0 , 1)) = 1
		_LineSize("Line Size", Range( 0.1 , 2)) = 1
		_LineSpeed("Line Speed", Range( 0.1 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		
		struct Input {
			half filler;
		};
		float4 _ASEOutlineColor;
		float _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma exclude_renderers xboxseries playstation switch nomrt 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _LineAlpha;
		uniform sampler2D _LineMap;
		uniform float _LineSpeed;
		uniform float _LineSize;
		uniform float4 _LineColor;
		uniform sampler2D _AmbientOcclusion;
		uniform float4 _AmbientOcclusion_ST;
		uniform sampler2D _MetallicGlossMap;
		uniform float4 _MetallicGlossMap_ST;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 tex2DNode2 = UnpackNormal( tex2D( _BumpMap, uv_BumpMap ) );
			float3 NormalMap266 = tex2DNode2;
			o.Normal = NormalMap266;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( _Color * tex2DNode1 ).rgb;
			float2 temp_cast_1 = (_LineSpeed).xx;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float2 temp_cast_2 = (( ase_vertex3Pos.x * _LineSize )).xx;
			float2 panner243 = ( _Time.y * temp_cast_1 + temp_cast_2);
			float4 temp_output_119_0 = ( tex2D( _LineMap, panner243 ) * _LineColor );
			float3 desaturateInitialColor282 = float3( (tex2DNode2).xy ,  0.0 );
			float desaturateDot282 = dot( desaturateInitialColor282, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar282 = lerp( desaturateInitialColor282, desaturateDot282.xxx, 1.0 );
			float4 lerpResult248 = lerp( float4( 0,0,0,0 ) , temp_output_119_0 , float4( saturate( desaturateVar282 ) , 0.0 ));
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			float4 tex2DNode4 = tex2D( _AmbientOcclusion, uv_AmbientOcclusion );
			float4 lerpResult278 = lerp( float4( 0,0,0,0 ) , ( lerpResult248 + ( temp_output_119_0 * float4( 0.245283,0.245283,0.245283,0 ) ) ) , tex2DNode4);
			float4 Line32 = ( _LineAlpha * lerpResult278 );
			o.Emission = Line32.rgb;
			float2 uv_MetallicGlossMap = i.uv_texcoord * _MetallicGlossMap_ST.xy + _MetallicGlossMap_ST.zw;
			float4 tex2DNode3 = tex2D( _MetallicGlossMap, uv_MetallicGlossMap );
			o.Metallic = tex2DNode3.r;
			o.Smoothness = tex2DNode3.a;
			o.Occlusion = tex2DNode4.r;
			o.Alpha = 1;
			clip( tex2DNode1.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18912
-2556;8;1476;826;-517.5134;606.8481;1.266579;True;True
Node;AmplifyShaderEditor.RangedFloatNode;155;-564.9153,756.0869;Float;False;Property;_LineSize;Line Size;9;0;Create;True;0;0;0;False;0;False;1;0;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;156;-554.7211,592.4086;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;245;-500.458,914.8726;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;120;-562.5779,836.1566;Float;False;Property;_LineSpeed;Line Speed;10;0;Create;True;0;0;0;False;0;False;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;-262.9349,660.7156;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;86.71446,500.9938;Inherit;True;Property;_BumpMap;Normal Map;4;1;[Normal];Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;243;-106.8876,762.6415;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;281;454.1779,596.9496;Inherit;False;True;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;11;131.0591,946.0563;Float;False;Property;_LineColor;Line Color;7;1;[HDR];Create;True;0;0;0;False;0;False;1.991077,1.306363,0.3513666,1;0.8679245,0.5701144,0.1514774,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DesaturateOpNode;282;687.4052,574.8496;Inherit;False;2;0;FLOAT3;1,1,1;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;116;85.38297,736.3163;Inherit;True;Property;_LineMap;Line Map;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;428.7954,893.3885;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;258;877.7032,519.3886;Inherit;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;285;1131.722,928.1147;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.245283,0.245283,0.245283,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;248;1084.967,664.9897;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;284;1389.963,858.3701;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;1265.576,335.8212;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;290;1712.875,667.6327;Float;False;Property;_LineAlpha;Line Alpha;8;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;278;1734.285,750.0388;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;2027.783,721.9277;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;266;449.7814,482.8833;Float;False;NormalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;2184.24,726.838;Float;False;Line;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;1263.556,-273.1095;Inherit;True;Property;_MainTex;Base Color Map;2;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;292;1311.934,-453.5423;Float;False;Property;_Color;Base Color;1;0;Create;False;0;0;0;False;0;False;1,1,1,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;1264.116,136.0759;Inherit;True;Property;_MetallicGlossMap;Metallic & Smoothness;3;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;265;1317.82,24.22769;Inherit;False;32;Line;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;269;1312.638,-53.34413;Inherit;False;266;NormalMap;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;1607.918,-321.2689;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;50;1825.649,21.80332;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;SurvivalTemplatePro/Interactable_Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.01;0.8962264,0.7633126,0.2832413,0;VertexOffset;False;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;246;0;156;1
WireConnection;246;1;155;0
WireConnection;243;0;246;0
WireConnection;243;2;120;0
WireConnection;243;1;245;0
WireConnection;281;0;2;0
WireConnection;282;0;281;0
WireConnection;116;1;243;0
WireConnection;119;0;116;0
WireConnection;119;1;11;0
WireConnection;258;0;282;0
WireConnection;285;0;119;0
WireConnection;248;1;119;0
WireConnection;248;2;258;0
WireConnection;284;0;248;0
WireConnection;284;1;285;0
WireConnection;278;1;284;0
WireConnection;278;2;4;0
WireConnection;291;0;290;0
WireConnection;291;1;278;0
WireConnection;266;0;2;0
WireConnection;32;0;291;0
WireConnection;293;0;292;0
WireConnection;293;1;1;0
WireConnection;50;0;293;0
WireConnection;50;1;269;0
WireConnection;50;2;265;0
WireConnection;50;3;3;0
WireConnection;50;4;3;4
WireConnection;50;5;4;0
WireConnection;50;10;1;4
ASEEND*/
//CHKSM=EEBCE8B1C85DE3417836B3488DBC6985B5E34515