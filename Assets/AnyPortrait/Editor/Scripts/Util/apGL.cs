﻿/*
*	Copyright (c) 2017-2019. RainyRizzle. All rights reserved
*	Contact to : https://www.rainyrizzle.com/ , contactrainyrizzle@gmail.com
*
*	This file is part of [AnyPortrait].
*
*	AnyPortrait can not be copied and/or distributed without
*	the express perission of [Seungjik Lee].
*
*	Unless this file is downloaded from the Unity Asset Store or RainyRizzle homepage, 
*	this file and its users are illegal.
*	In that case, the act may be subject to legal penalties.
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

using AnyPortrait;

namespace AnyPortrait
{

	public static class apGL
	{
		//private static Material _mat_Color = null;
		//private static Material _mat_Texture = null;

		//private static int _windowPosX = 0;
		//private static int _windowPosY = 0;
		public static int _windowWidth = 0;
		public static int _windowHeight = 0;
		public static int _totalEditorWidth = 0;
		public static int _totalEditorHeight = 0;
		public static Vector2 _scrol_NotCalculated = Vector2.zero;
		public static int _posX_NotCalculated = 0;
		public static int _posY_NotCalculated = 0;

		public static Vector2 _windowScroll = Vector2.zero;

		public static float _zoom = 1.0f;
		public static float Zoom { get { return _zoom; } }

		public static Vector2 WindowSize { get { return new Vector2(_windowWidth, _windowHeight); } }
		public static Vector2 WindowSizeHalf { get { return new Vector2(_windowWidth / 2, _windowHeight / 2); } }

		private static GUIStyle _textStyle = GUIStyle.none;

		private static Vector4 _glScreenClippingSize = Vector4.zero;

		private static bool _isAnyCursorEvent = false;
		private static bool _isDelayedCursorEvent = false;
		private static Vector2 _delayedCursorPos = Vector2.zero;
		private static MouseCursor _delayedCursorType = MouseCursor.Zoom;

		[Flags]
		public enum RENDER_TYPE : int
		{
			Default = 0,

			ShadeAllMesh = 1,
			AllMesh = 2,

			Vertex = 4,

			Outlines = 8,
			AllEdges = 16,

			VolumeWeightColor = 32,
			PhysicsWeightColor = 64,
			BoneRigWeightColor = 128,

			TransformBorderLine = 256,
			PolygonOutline = 512,

			ToneColor = 1024,
			BoneOutlineOnly = 2048
		}

		private static Color _textureColor_Gray = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		private static Color _textureColor_Shade = new Color(0.3f, 0.3f, 0.3f, 1.0f);

		//private static Color _vertColor_NotSelected = new Color(0.0f, 0.3f, 1.0f, 0.6f);
		//private static Color _vertColor_Selected = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		private static Color _vertColor_NextSelected = new Color(1.0f, 0.0f, 1.0f, 0.6f);
		private static Color _vertColor_Outline = new Color(0.0f, 0.0f, 0.0f, 0.8f);
		private static Color _vertColor_Outline_White = new Color(1.0f, 1.0f, 1.0f, 0.8f);

		//Weight인 경우 보(0)-파(25)-초(50)-노(75)-빨(100)로 이어진다.
		private static Color _vertColor_Weighted_0 = new Color(1.0f, 0.0f, 1.0f, 1.0f);
		private static Color _vertColor_Weighted_25 = new Color(0.0f, 0.5f, 1.0f, 1.0f);
		private static Color _vertColor_Weighted_50 = new Color(0.0f, 1.0f, 0.5f, 1.0f);
		private static Color _vertColor_Weighted_75 = new Color(1.0f, 1.0f, 0.0f, 1.0f);


		private static Color _vertColor_Weighted3_0 = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		private static Color _vertColor_Weighted3_25 = new Color(0.0f, 0.0f, 1.0f, 1.0f);
		private static Color _vertColor_Weighted3_50 = new Color(1.0f, 1.0f, 0.0f, 1.0f);
		private static Color _vertColor_Weighted3_75 = new Color(1.0f, 0.5f, 0.0f, 1.0f);
		private static Color _vertColor_Weighted3_100 = new Color(1.0f, 0.0f, 0.0f, 1.0f);

		private static Color _vertColor_Weighted3Vert_0 = new Color(0.2f, 0.2f, 0.2f, 1.0f);
		private static Color _vertColor_Weighted3Vert_25 = new Color(0.2f, 0.2f, 1.0f, 1.0f);
		private static Color _vertColor_Weighted3Vert_50 = new Color(1.0f, 1.0f, 0.2f, 1.0f);
		private static Color _vertColor_Weighted3Vert_75 = new Color(1.0f, 0.5f, 0.2f, 1.0f);
		private static Color _vertColor_Weighted3Vert_100 = new Color(1.0f, 0.2f, 0.2f, 1.0f);


		private static Color _vertColor_Weighted4_0_Null = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		private static Color _vertColor_Weighted4_0 = new Color(1.0f, 0.5f, 0.0f, 1.0f);
		private static Color _vertColor_Weighted4_33 = new Color(0.0f, 1.0f, 0.0f, 1.0f);
		private static Color _vertColor_Weighted4_66 = new Color(0.0f, 1.0f, 1.0f, 1.0f);
		private static Color _vertColor_Weighted4_100 = new Color(1.0f, 0.0f, 1.0f, 1.0f);

		private static Color _vertColor_Weighted4Vert_Null = new Color(0.2f, 0.2f, 0.2f, 1.0f);
		private static Color _vertColor_Weighted4Vert_0 = new Color(1.0f, 0.5f, 0.2f, 1.0f);
		private static Color _vertColor_Weighted4Vert_33 = new Color(0.2f, 1.0f, 0.2f, 1.0f);
		private static Color _vertColor_Weighted4Vert_66 = new Color(0.2f, 1.0f, 1.0f, 1.0f);
		private static Color _vertColor_Weighted4Vert_100 = new Color(1.0f, 0.2f, 1.0f, 1.0f);


		//private static Color _lineColor_Tri = new Color(1.0f, 0.5f, 0.0f, 0.9f);
		//private static Color _lineColor_HiddenEdge = new Color(1.0f, 1.0f, 0.0f, 0.7f);
		//private static Color _lineColor_Outline = new Color(0.0f, 0.5f, 1.0f, 0.7f);
		//private static Color _lineColor_TFBorder = new Color(0.0f, 1.0f, 1.0f, 1.0f);

		private static Color _lineColor_BoneOutline = new Color(1.0f, 0.0f, 0.2f, 0.8f);
		private static Color _lineColor_BoneOutlineRollOver = new Color(1.0f, 0.2f, 0.0f, 0.5f);

		private static Texture2D _img_VertPhysicMain = null;
		private static Texture2D _img_VertPhysicConstraint = null;
		private static Texture2D _img_RigCircle = null;

		private static Color _toneColor = new Color(0.1f, 0.3f, 0.5f, 0.7f);
		private static float _toneLineThickness = 0.0f;
		private static float _toneShapeRatio = 0.0f;
		private static Vector2 _tonePosOffset = Vector2.zero;
		private static Color _toneBoneColor = new Color(1, 1, 1, 0.9f);


		//애니메이션 GUI를 위한 타이머
		private static System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();
		private static float _animationTimeRatio = 0.0f;
		private static float _animationTimeCount = 0.0f;
		private const float ANIMATION_TIME_LENGTH = 0.9f;
		private const float ANIMATED_LINE_UNIT_LENGTH = 10.0f;
		private const float ANIMATED_LINE_SPACE_LENGTH = 6.0f;


		//------------------------------------------------------------------------
		public class MaterialBatch
		{
			public enum MatType
			{
				None, Color,
				Texture_Normal, Texture_VColorAdd,
				//MaskedTexture,//<<구형 방식
				MaskOnly, Clipped,
				GUITexture,
				ToneColor_Normal, ToneColor_Clipped,
				Alpha2White,//Capture용 Shader
			}
			private Material _mat_Color = null;
			private Material _mat_MaskOnly = null;

			//추가 : 일반 Texture Transparent같지만 GUI 전용이며 _Color가 없고 Vertex Color를 사용하여 Batch하기에 좋다.
			private Material _mat_GUITexture = null;

			private Material[] _mat_Texture_Normal = null;
			private Material[] _mat_Texture_VColorAdd = null;
			//private Material[] _mat_MaskedTexture = null;
			private Material[] _mat_Clipped = null;

			private Material _mat_ToneColor_Normal = null;
			private Material _mat_ToneColor_Clipped = null;

			private Material _mat_Alpha2White = null;



			private MatType _matType = MatType.None;

			//마지막 입력 값
			private Vector4 _glScreenClippingSize = Vector4.zero;

			public Color _color = Color.black;
			private Texture2D _texture = null;

			//마스크 버전은 좀 많다..
			private RenderTexture _renderTexture = null;
			private int _renderTextureSize_Width = -1;
			private int _renderTextureSize_Height = -1;
			public RenderTexture RenderTex { get { return _renderTexture; } }

			//private Color _clipColor_1 = Color.black;
			//private Color _clipColor_2 = Color.black;
			//private Color _clipColor_3 = Color.black;

			//private Texture2D _clipTexture_1 = null;
			//private Texture2D _clipTexture_2 = null;
			//private Texture2D _clipTexture_3 = null;

			public const int ALPHABLEND = 0;
			public const int ADDITIVE = 1;
			public const int SOFT_ADDITIVE = 2;
			public const int MULTIPLICATIVE = 3;

			//private static Color[] ShaderTypeColor = new Color[] {  new Color(1.0f, 0.0f, 0.0f, 0.0f),
			//													new Color(0.0f, 1.0f, 0.0f, 0.0f),
			//													new Color(0.0f, 0.0f, 1.0f, 0.0f),
			//													new Color(0.0f, 0.0f, 0.0f, 1.0f)};

			private int _shaderType_Main = 0;
			//private int _shaderType_Clip1 = 0;
			//private int _shaderType_Clip2 = 0;
			//private int _shaderType_Clip3 = 0;
			//private Color _shaderTypeColor_Clip1 = new Color(1.0f, 0.0f, 0.0f, 0.0f);
			//private Color _shaderTypeColor_Clip2 = new Color(1.0f, 0.0f, 0.0f, 0.0f);
			//private Color _shaderTypeColor_Clip3 = new Color(1.0f, 0.0f, 0.0f, 0.0f);

			//private bool _isNeedReset = true;

			public MaterialBatch()
			{

			}

			public void SetShader(Shader shader_Color,
									Shader[] shader_Texture_Normal_Set,
									Shader[] shader_Texture_VColorAdd_Set,
									//Shader[] shader_MaskedTexture_Set,
									Shader shader_MaskOnly,
									Shader[] shader_Clipped_Set,
									Shader shader_GUITexture,
									Shader shader_ToneColor_Normal,
									Shader shader_ToneColor_Clipped,
									Shader shader_Alpha2White
									)
			{
				//_mat_Color = mat_Color;
				//_mat_Texture = mat_Texture;
				//_mat_MaskedTexture = mat_MaskedTexture;

				_mat_Color = new Material(shader_Color);
				_mat_Color.color = new Color(1, 1, 1, 1);

				_mat_MaskOnly = new Material(shader_MaskOnly);
				_mat_MaskOnly.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);

				//추가 : GUI용 텍스쳐
				_mat_GUITexture = new Material(shader_GUITexture);


				//AlphaBlend, Add, SoftAdditive
				_mat_Texture_Normal = new Material[4];
				_mat_Texture_VColorAdd = new Material[4];
				//_mat_MaskedTexture = new Material[4];
				_mat_Clipped = new Material[4];

				for (int i = 0; i < 4; i++)
				{
					_mat_Texture_Normal[i] = new Material(shader_Texture_Normal_Set[i]);
					_mat_Texture_Normal[i].color = new Color(0.5f, 0.5f, 0.5f, 1.0f);

					_mat_Texture_VColorAdd[i] = new Material(shader_Texture_VColorAdd_Set[i]);
					_mat_Texture_VColorAdd[i].color = new Color(0.5f, 0.5f, 0.5f, 1.0f);

					//_mat_MaskedTexture[i] = new Material(shader_MaskedTexture_Set[i]);
					//_mat_MaskedTexture[i].color = new Color(0.5f, 0.5f, 0.5f, 1.0f);

					_mat_Clipped[i] = new Material(shader_Clipped_Set[i]);
					_mat_Clipped[i].color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
				}

				_mat_ToneColor_Normal = new Material(shader_ToneColor_Normal);
				_mat_ToneColor_Normal.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);

				_mat_ToneColor_Clipped = new Material(shader_ToneColor_Clipped);
				_mat_ToneColor_Clipped.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);

				_mat_Alpha2White = new Material(shader_Alpha2White);
				_mat_Alpha2White.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			}

			#region [미사용 코드]
			//public void SetMaterialType_Color()
			//{
			//	_matType = MatType.Color;
			//}
			//public void SetMaterialType_Texture_Normal(apPortrait.SHADER_TYPE shaderType)
			//{
			//	_matType = MatType.Texture_Normal;
			//	_shaderType_Main = (int)shaderType;
			//}
			//public void SetMaterialType_Texture_VColorAdd(apPortrait.SHADER_TYPE shaderType)
			//{
			//	_matType = MatType.Texture_VColorAdd;
			//	_shaderType_Main = (int)shaderType;
			//}
			//public void SetMaterialType_MaskedTexture(apPortrait.SHADER_TYPE shaderTypeMain,
			//											apPortrait.SHADER_TYPE shaderTypeClip1,
			//											apPortrait.SHADER_TYPE shaderTypeClip2,
			//											apPortrait.SHADER_TYPE shaderTypeClip3)
			//{
			//	_matType = MatType.MaskedTexture;
			//	_shaderType_Main = (int)shaderTypeMain;
			//	_shaderType_Clip1 = (int)shaderTypeClip1;
			//	_shaderType_Clip2 = (int)shaderTypeClip2;
			//	_shaderType_Clip3 = (int)shaderTypeClip3;

			//	_shaderTypeColor_Clip1 = ShaderTypeColor[_shaderType_Clip1];
			//	_shaderTypeColor_Clip2 = ShaderTypeColor[_shaderType_Clip2];
			//	_shaderTypeColor_Clip3 = ShaderTypeColor[_shaderType_Clip3];
			//}

			//public void SetMaterialType_MaskAndClipped(apPortrait.SHADER_TYPE shaderTypeMain,
			//											apPortrait.SHADER_TYPE shaderTypeClip1,
			//											apPortrait.SHADER_TYPE shaderTypeClip2,
			//											apPortrait.SHADER_TYPE shaderTypeClip3)
			//{
			//	_shaderType_Main = (int)shaderTypeMain;
			//	_shaderType_Clip1 = (int)shaderTypeClip1;
			//	_shaderType_Clip2 = (int)shaderTypeClip2;
			//	_shaderType_Clip3 = (int)shaderTypeClip3;

			//	_shaderTypeColor_Clip1 = ShaderTypeColor[_shaderType_Clip1];
			//	_shaderTypeColor_Clip2 = ShaderTypeColor[_shaderType_Clip2];
			//	_shaderTypeColor_Clip3 = ShaderTypeColor[_shaderType_Clip3];
			//} 
			#endregion

			public void SetClippingSize(Vector4 screenSize)
			{
				_glScreenClippingSize = screenSize;

				switch (_matType)
				{
					case MatType.Color:
						_mat_Color.SetVector("_ScreenSize", _glScreenClippingSize);
						break;

					case MatType.Texture_Normal:
						_mat_Texture_Normal[_shaderType_Main].SetVector("_ScreenSize", _glScreenClippingSize);
						break;

					case MatType.Texture_VColorAdd:
						_mat_Texture_VColorAdd[_shaderType_Main].SetVector("_ScreenSize", _glScreenClippingSize);
						break;

					//case MatType.MaskedTexture:
					//	_mat_MaskedTexture[_shaderType_Main].SetVector("_ScreenSize", _glScreenClippingSize);
					//	break;

					case MatType.Clipped:
						_mat_Clipped[_shaderType_Main].SetVector("_ScreenSize", _glScreenClippingSize);
						break;

					case MatType.MaskOnly:
						_mat_MaskOnly.SetVector("_ScreenSize", _glScreenClippingSize);
						break;

					case MatType.GUITexture:
						_mat_GUITexture.SetVector("_ScreenSize", _glScreenClippingSize);
						break;

					case MatType.ToneColor_Normal:
						_mat_ToneColor_Normal.SetVector("_ScreenSize", _glScreenClippingSize);
						break;

					case MatType.ToneColor_Clipped:
						_mat_ToneColor_Clipped.SetVector("_ScreenSize", _glScreenClippingSize);
						break;

					case MatType.Alpha2White:
						_mat_Alpha2White.SetVector("_ScreenSize", _glScreenClippingSize);
						break;
				}



				//GL.Flush();
			}


			public void SetClippingSizeToAllMaterial(Vector4 screenSize)
			{
				_glScreenClippingSize = screenSize;

				_mat_Color.SetVector("_ScreenSize", _glScreenClippingSize);
				_mat_Texture_Normal[_shaderType_Main].SetVector("_ScreenSize", _glScreenClippingSize);
				_mat_Texture_VColorAdd[_shaderType_Main].SetVector("_ScreenSize", _glScreenClippingSize);
				//_mat_MaskedTexture[_shaderType_Main].SetVector("_ScreenSize", _glScreenClippingSize);
				_mat_Clipped[_shaderType_Main].SetVector("_ScreenSize", _glScreenClippingSize);
				_mat_MaskOnly.SetVector("_ScreenSize", _glScreenClippingSize);
				_mat_GUITexture.SetVector("_ScreenSize", _glScreenClippingSize);
				_mat_ToneColor_Normal.SetVector("_ScreenSize", _glScreenClippingSize);
				_mat_ToneColor_Clipped.SetVector("_ScreenSize", _glScreenClippingSize);
				_mat_Alpha2White.SetVector("_ScreenSize", _glScreenClippingSize);
			}

			/// <summary>
			/// RenderTexture를 사용하는 GL계열에서는 이 함수를 윈도우 크기 호출시에 같이 호출한다.
			/// </summary>
			/// <param name="windowWidth"></param>
			/// <param name="windowHeight"></param>
			public void CheckMaskTexture(int windowWidth, int windowHeight)
			{
				//if(_renderTexture == null || _renderTextureSize_Width != windowWidth || _renderTextureSize_Height != windowHeight)
				//{
				//	if(_renderTexture != null)
				//	{
				//		//UnityEngine.Object.DestroyImmediate(_renderTexture);
				//		RenderTexture.ReleaseTemporary(_renderTexture);
				//		_renderTexture = null;
				//	}
				//	//_renderTexture = new RenderTexture(windowWidth, windowHeight, 24);
				//	_renderTexture = RenderTexture.GetTemporary(windowWidth, windowHeight, 24);
				//	_renderTexture.wrapMode = TextureWrapMode.Clamp;
				//	_renderTextureSize_Width = windowWidth;
				//	_renderTextureSize_Height = windowHeight;
				//}

				_renderTextureSize_Width = windowWidth;
				_renderTextureSize_Height = windowHeight;
			}

			public void SetPass_Color()
			{
				_mat_Color.SetPass(0);

				_mat_Color.color = new Color(1, 1, 1, 1);
				_matType = MatType.Color;

				//GL.sRGBWrite = true;
			}

			public void SetPass_GUITexture(Texture2D texture)
			{
				_texture = texture;
				_mat_GUITexture.SetTexture("_MainTex", _texture);

				_mat_GUITexture.SetPass(0);
				_matType = MatType.GUITexture;

				//GL.sRGBWrite = true;
			}

			public void SetPass_Texture_Normal(Color color, Texture2D texture, apPortrait.SHADER_TYPE shaderType)
			{
				_shaderType_Main = (int)shaderType;
				_color = color;
				_mat_Texture_Normal[_shaderType_Main].SetColor("_Color", _color);

				_texture = texture;
				_mat_Texture_Normal[_shaderType_Main].SetTexture("_MainTex", _texture);

				_mat_Texture_Normal[_shaderType_Main].SetPass(0);
				_matType = MatType.Texture_Normal;

				//GL.sRGBWrite = true;
				//_isNeedReset = false;
			}

			public void SetPass_ToneColor_Normal(Color color, Texture2D texture)
			{
				_matType = MatType.ToneColor_Normal;
				_color = color;
				
				_mat_ToneColor_Normal.SetColor("_Color", _color);
				_mat_ToneColor_Normal.SetFloat("_Thickness", _toneLineThickness);
				_mat_ToneColor_Normal.SetFloat("_ShapeRatio", _toneShapeRatio);
				_mat_ToneColor_Normal.SetFloat("_PosOffsetX", _tonePosOffset.x * _zoom);
				_mat_ToneColor_Normal.SetFloat("_PosOffsetY", _tonePosOffset.y * _zoom);

				_texture = texture;
				_mat_ToneColor_Normal.SetTexture("_MainTex", _texture);
				_mat_ToneColor_Normal.SetPass(0);
				
				//GL.sRGBWrite = true;

				//_isNeedReset = false;
			}

			public void SetPass_ToneColor_Custom(Color color, Texture2D texture, float thickness, float shapeRatio)
			{
				_matType = MatType.ToneColor_Normal;
				_color = color;
				
				_mat_ToneColor_Normal.SetColor("_Color", _color);
				_mat_ToneColor_Normal.SetFloat("_Thickness", thickness);
				_mat_ToneColor_Normal.SetFloat("_ShapeRatio", shapeRatio);
				_mat_ToneColor_Normal.SetFloat("_PosOffsetX", 0.0f);
				_mat_ToneColor_Normal.SetFloat("_PosOffsetY", 0.0f);

				_texture = texture;
				_mat_ToneColor_Normal.SetTexture("_MainTex", _texture);
				_mat_ToneColor_Normal.SetPass(0);
				
				//GL.sRGBWrite = true;

				//_isNeedReset = false;
			}

			public void SetPass_Texture_VColor(Color color, Texture2D texture, float vertColorRatio, apPortrait.SHADER_TYPE shaderType)
			{
				_shaderType_Main = (int)shaderType;

				_color = color;
				_mat_Texture_VColorAdd[_shaderType_Main].SetColor("_Color", _color);

				_texture = texture;
				_mat_Texture_VColorAdd[_shaderType_Main].SetTexture("_MainTex", _texture);

				_mat_Texture_VColorAdd[_shaderType_Main].SetFloat("_vColorITP", vertColorRatio);
				//_isNeedReset = true;

				_mat_Texture_VColorAdd[_shaderType_Main].SetPass(0);
				_matType = MatType.Texture_VColorAdd;

				//GL.sRGBWrite = true;
				//_isNeedReset = false;
			}

			

			public void SetPass_Mask(Color color, Texture2D texture,
									float vertColorRatio, apPortrait.SHADER_TYPE shaderType,
									bool isRenderMask)
			{
				_shaderType_Main = (int)shaderType;

				_color = color;
				_texture = texture;

				if (isRenderMask)
				{
					//RenderTexture로 만든다.
					_matType = MatType.MaskOnly;

					//RenderTexture를 활성화한다.
					_renderTexture = RenderTexture.GetTemporary(_renderTextureSize_Width, _renderTextureSize_Height, 8);
					_renderTexture.wrapMode = TextureWrapMode.Clamp;

					//RenderTexture를 사용
					RenderTexture.active = _renderTexture;

					//[중요] Temp RenderTexture는 색상 초기화가 안되어있다. 꼭 해준다.
					GL.Clear(true, true, Color.clear, 0.0f);


					_mat_MaskOnly.SetColor("_Color", _color);
					_mat_MaskOnly.SetTexture("_MainTex", _texture);
					_mat_MaskOnly.SetFloat("_vColorITP", vertColorRatio);
					_mat_MaskOnly.SetFloat("_PosOffsetX", 0);
					_mat_MaskOnly.SetFloat("_PosOffsetY", 0);
					_mat_MaskOnly.SetPass(0);
				}
				else
				{
					_matType = MatType.Texture_VColorAdd;

					_mat_Texture_VColorAdd[_shaderType_Main].SetColor("_Color", _color);
					_mat_Texture_VColorAdd[_shaderType_Main].SetTexture("_MainTex", _texture);
					_mat_Texture_VColorAdd[_shaderType_Main].SetFloat("_vColorITP", vertColorRatio);

					_mat_Texture_VColorAdd[_shaderType_Main].SetPass(0);
				}
			}

			public void SetPass_Clipped(Color color, Texture2D texture, float vertColorRatio, apPortrait.SHADER_TYPE shaderType, Color parentColor)
			{
				_matType = MatType.Clipped;
				_shaderType_Main = (int)shaderType;

				_color = color;
				_texture = texture;
				_mat_Clipped[_shaderType_Main].SetColor("_Color", _color);
				_mat_Clipped[_shaderType_Main].SetTexture("_MainTex", _texture);
				_mat_Clipped[_shaderType_Main].SetFloat("_vColorITP", vertColorRatio);

				_mat_Clipped[_shaderType_Main].SetTexture("_MaskRenderTexture", _renderTexture);//<<Mask를 넣자
				_mat_Clipped[_shaderType_Main].SetColor("_MaskColor", parentColor);


				_mat_Clipped[_shaderType_Main].SetPass(0);

				//Debug.Log("SetPass Clipped");
			}




			public void SetPass_Mask_ToneColor(Color color, Texture2D texture,
									bool isRenderMask)
			{
				_color = color;
				_texture = texture;

				if (isRenderMask)
				{
					//RenderTexture로 만든다.
					_matType = MatType.MaskOnly;

					//RenderTexture를 활성화한다.
					_renderTexture = RenderTexture.GetTemporary(_renderTextureSize_Width, _renderTextureSize_Height, 8);
					_renderTexture.wrapMode = TextureWrapMode.Clamp;

					//RenderTexture를 사용
					RenderTexture.active = _renderTexture;

					//[중요] Temp RenderTexture는 색상 초기화가 안되어있다. 꼭 해준다.
					GL.Clear(true, true, Color.clear, 0.0f);


					_mat_MaskOnly.SetColor("_Color", _color);
					_mat_MaskOnly.SetTexture("_MainTex", _texture);
					_mat_MaskOnly.SetFloat("_vColorITP", 0.0f);
					_mat_MaskOnly.SetFloat("_PosOffsetX", _tonePosOffset.x * _zoom);
					_mat_MaskOnly.SetFloat("_PosOffsetY", _tonePosOffset.y * _zoom);
					_mat_MaskOnly.SetPass(0);
				}
				else
				{
					_matType = MatType.ToneColor_Normal;

					_mat_ToneColor_Normal.SetColor("_Color", _color);
					_mat_ToneColor_Normal.SetTexture("_MainTex", _texture);
					_mat_ToneColor_Normal.SetFloat("_Thickness", _toneLineThickness);
					_mat_ToneColor_Normal.SetFloat("_ShapeRatio", _toneShapeRatio);
					_mat_ToneColor_Normal.SetFloat("_PosOffsetX", _tonePosOffset.x * _zoom);
					_mat_ToneColor_Normal.SetFloat("_PosOffsetY", _tonePosOffset.y * _zoom);
					_mat_ToneColor_Normal.SetPass(0);
				}
			}



			public void SetPass_Clipped_ToneColor(Color color, Texture2D texture, Color parentColor)
			{
				_matType = MatType.ToneColor_Clipped;

				_color = color;
				_texture = texture;
				_mat_ToneColor_Clipped.SetColor("_Color", _color);
				_mat_ToneColor_Clipped.SetTexture("_MainTex", _texture);
				_mat_ToneColor_Clipped.SetTexture("_MaskRenderTexture", _renderTexture);//<<Mask를 넣자
				_mat_ToneColor_Clipped.SetColor("_MaskColor", parentColor);
				_mat_ToneColor_Clipped.SetFloat("_Thickness", _toneLineThickness);
				_mat_ToneColor_Clipped.SetFloat("_ShapeRatio", _toneShapeRatio);
				_mat_ToneColor_Clipped.SetFloat("_PosOffsetX", _tonePosOffset.x * _zoom);
				_mat_ToneColor_Clipped.SetFloat("_PosOffsetY", _tonePosOffset.y * _zoom);


				_mat_ToneColor_Clipped.SetPass(0);

				//Debug.Log("SetPass Clipped");
			}

			public void SetPass_ClippedWithMaskedTexture(Color color, Texture2D texture, float vertColorRatio,
														apPortrait.SHADER_TYPE shaderType, Color parentColor,
														Texture2D maskedTexture
				)
			{
				_matType = MatType.Clipped;
				_shaderType_Main = (int)shaderType;

				_color = color;
				_texture = texture;
				_mat_Clipped[_shaderType_Main].SetColor("_Color", _color);
				_mat_Clipped[_shaderType_Main].SetTexture("_MainTex", _texture);
				_mat_Clipped[_shaderType_Main].SetFloat("_vColorITP", vertColorRatio);

				_mat_Clipped[_shaderType_Main].SetTexture("_MaskRenderTexture", maskedTexture);//<<Mask를 넣자
				_mat_Clipped[_shaderType_Main].SetColor("_MaskColor", parentColor);

				_mat_Clipped[_shaderType_Main].SetPass(0);

				//Debug.Log("SetPass Clipped");
			}

			public void SetPass_Alpha2White(Color color, Texture2D texture)
			{
				_matType = MatType.Alpha2White;
				_shaderType_Main = 0;

				_color = color;
				_texture = texture;

				_mat_Alpha2White.SetColor("_Color", _color);
				_mat_Alpha2White.SetTexture("_MainTex", _texture);
				_mat_Alpha2White.SetPass(0);
			}

			/// <summary>
			/// MultiPass에서 사용한 RenderTexture를 해제한다.
			/// 다만, 삭제하지는 않는다.
			/// </summary>
			public void DeactiveRenderTexture()
			{
				RenderTexture.active = null;
			}
			/// <summary>
			/// MultiPass의 모든 과정이 끝나면 사용했던 RenderTexture를 해제한다.
			/// </summary>
			public void ReleaseRenderTexture()
			{
				if (_renderTexture != null)
				{
					RenderTexture.active = null;
					RenderTexture.ReleaseTemporary(_renderTexture);
					_renderTexture = null;
				}
			}
			public bool IsNotReady()
			{
				return (_mat_Color == null
					|| _mat_Texture_Normal == null
					|| _mat_Texture_VColorAdd == null
					//|| _mat_MaskedTexture == null
					|| _mat_Clipped == null
					|| _mat_MaskOnly == null
					|| _mat_GUITexture == null
					|| _mat_ToneColor_Normal == null
					|| _mat_ToneColor_Clipped == null
					|| _mat_Alpha2White == null);
			}

			

		}

		private static MaterialBatch _matBatch = new MaterialBatch();
		public static MaterialBatch MatBatch { get { return _matBatch; } }
		//------------------------------------------------------------------------

		public static void SetShader(Shader shader_Color,
									Shader[] shader_Texture_Normal_Set,
									Shader[] shader_Texture_VColorAdd_Set,
									//Shader[] shader_MaskedTexture_Set,
									Shader shader_MaskOnly,
									Shader[] shader_Clipped_Set,
									Shader shader_GUITexture,
									Shader shader_ToneColor_Normal,
									Shader shader_ToneColor_Clipped,
									Shader shader_Alpha2White)
		{
			//_mat_Color = mat_Color;
			//_mat_Texture = mat_Texture;

			//_matBatch.SetMaterial(mat_Color, mat_Texture, mat_MaskedTexture);
			_matBatch.SetShader(shader_Color,
								shader_Texture_Normal_Set,
								shader_Texture_VColorAdd_Set,
								//shader_MaskedTexture_Set,
								shader_MaskOnly,
								shader_Clipped_Set,
								shader_GUITexture,
								shader_ToneColor_Normal,
								shader_ToneColor_Clipped,
								shader_Alpha2White);
		}

		public static void SetTexture(	Texture2D img_VertPhysicMain, 
										Texture2D img_VertPhysicConstraint,
										Texture2D img_RigCircle)
		{
			_img_VertPhysicMain = img_VertPhysicMain;
			_img_VertPhysicConstraint = img_VertPhysicConstraint;
			_img_RigCircle = img_RigCircle;
		}


		public static void SetWindowSize(int windowWidth, int windowHeight, Vector2 scroll, float zoom,
			int posX, int posY, int totalEditorWidth, int totalEditorHeight)
		{
			_windowWidth = windowWidth;
			_windowHeight = windowHeight;
			_scrol_NotCalculated = scroll;
			_windowScroll.x = scroll.x * _windowWidth * 0.1f;
			_windowScroll.y = scroll.y * windowHeight * 0.1f;
			_totalEditorWidth = totalEditorWidth;
			_totalEditorHeight = totalEditorHeight;

			_posX_NotCalculated = posX;
			_posY_NotCalculated = posY;

			_zoom = zoom;

			totalEditorHeight += 30;
			posY += 30;
			posX += 5;
			windowWidth -= 25;
			windowHeight -= 20;

			//_windowPosX = posX;
			//_windowPosY = posY;

			//float leftMargin = posX;
			//float rightMargin = totalEditorWidth - (posX + windowWidth);
			//float topMargin = posY;
			//float bottomMargin = totalEditorHeight - (posY + windowHeight);

			_glScreenClippingSize.x = (float)posX / (float)totalEditorWidth;
			_glScreenClippingSize.y = (float)(posY) / (float)totalEditorHeight;
			_glScreenClippingSize.z = (float)(posX + windowWidth) / (float)totalEditorWidth;
			_glScreenClippingSize.w = (float)(posY + windowHeight) / (float)totalEditorHeight;

			_matBatch.CheckMaskTexture(_windowWidth, _windowHeight);

			

			//추가: 타이머
			_animationTimeCount += (float)_stopWatch.Elapsed.TotalSeconds;
			_stopWatch.Stop();
			_stopWatch.Reset();
			_stopWatch.Start();
			while(_animationTimeCount > ANIMATION_TIME_LENGTH)
			{
				_animationTimeCount -= ANIMATION_TIME_LENGTH;
			}
			_animationTimeRatio = Mathf.Clamp01(_animationTimeCount / ANIMATION_TIME_LENGTH);
		}

		




		public static void SetToneOption(Color toneColor, float toneLineThickness, bool isToneOutlineRender, float tonePosOffsetX, float tonePosOffsetY, Color toneBoneColor)
		{
			_toneColor = toneColor;
			_toneLineThickness = Mathf.Clamp01(toneLineThickness);
			_toneShapeRatio = isToneOutlineRender ? 0.0f : 1.0f;
			_tonePosOffset.x = tonePosOffsetX;
			_tonePosOffset.y = tonePosOffsetY;
			_toneBoneColor = toneBoneColor;
		}


		public static Vector2 World2GL(Vector2 pos)
		{
			return new Vector2(
				(pos.x * _zoom) + (_windowWidth * 0.5f)
				- _windowScroll.x,

				(_windowHeight - (pos.y * _zoom)) - (_windowHeight * 0.5f)
				- _windowScroll.y
				);
		}

		public static Vector2 GL2World(Vector2 glPos)
		{
			return new Vector2(
				(glPos.x + (_windowScroll.x) - (_windowWidth * 0.5f)) / _zoom,
				(-1.0f * (glPos.y + _windowScroll.y + (_windowHeight * 0.5f) - (_windowHeight))) / _zoom
				);
		}

		private static bool IsVertexClipped(Vector2 posGL)
		{
			return (posGL.x < 1.0f || posGL.x > _windowWidth - 1 ||
									posGL.y < 1.0f || posGL.y > _windowHeight - 1);
		}

		private static bool Is2VertexClippedAll(Vector2 pos1GL, Vector2 pos2GL)
		{
			bool isPos1Clipped = IsVertexClipped(pos1GL);

			bool isPos2Clipped = IsVertexClipped(pos2GL);


			if (!isPos1Clipped || !isPos2Clipped)
			{
				//둘중 하나라도 안에 들어있다.
				return false;
			}


			//두 점이 밖에 나갔어도, 중간 점이 걸쳐서 들어올 수 있다.
			Vector2 posDir = pos2GL - pos1GL;
			for (int i = 1; i < 5; i++)
			{
				Vector2 posSub = pos1GL + posDir * ((float)i / 5.0f);

				bool isPosSubClipped = IsVertexClipped(posSub);

				//중간점 하나가 들어와있다.
				if (!isPosSubClipped)
				{
					return false;
				}
			}
			return true;
		}

		private static Vector2 GetClippedVertex(Vector2 posTargetGL, Vector2 posBaseGL)
		{
			Vector2 pos1_Real = posTargetGL;
			Vector2 pos2_Real = posBaseGL;

			Vector2 dir1To2 = (pos2_Real - pos1_Real).normalized;
			Vector2 dir2To1 = -dir1To2;

			if (pos1_Real.x < 0.0f || pos1_Real.x > _windowWidth ||
				pos1_Real.y < 0.0f || pos1_Real.y > _windowHeight)
			{
				//2 + dir(2 -> 1) * t = 1'
				//dir * t = 1' - 2
				//t = (1' - 2) / dir

				float tX = 0.0f;
				float tY = 0.0f;
				float tResult = 0.0f;

				bool isClipX = false;
				bool isClipY = false;


				if (posTargetGL.x < 0.0f)
				{
					pos1_Real.x = 0.0f;
					isClipX = true;
				}
				else if (posTargetGL.x > _windowWidth)
				{
					pos1_Real.x = _windowWidth;
					isClipX = true;
				}

				if (posTargetGL.y < 0.0f)
				{
					pos1_Real.y = 0.0f;
					isClipY = true;
				}
				else if (posTargetGL.y > _windowHeight)
				{
					pos1_Real.y = _windowHeight;
					isClipY = true;
				}

				if (isClipX)
				{
					if (Mathf.Abs(dir2To1.x) > 0.0f)
					{ tX = (pos1_Real.x - pos2_Real.x) / dir2To1.x; }
					else
					{ return new Vector2(-100.0f, -100.0f); }//둘다 나갔다...
				}

				if (isClipY)
				{
					if (Mathf.Abs(dir2To1.y) > 0.0f)
					{ tY = (pos1_Real.y - pos2_Real.y) / dir2To1.y; }
					else
					{ return new Vector2(-100.0f, -100.0f); }//둘다 나갔다...
				}
				if (isClipX && isClipY)
				{
					if (Mathf.Abs(tX) < Mathf.Abs(tY))
					{
						tResult = tX;
					}
					else
					{
						tResult = tY;
					}
				}
				else if (isClipX)
				{
					tResult = tX;
				}
				else if (isClipY)
				{
					tResult = tY;
				}

				//2 + dir(2 -> 1) * t = 1'
				pos1_Real = pos2_Real + dir2To1 * tResult;
				return pos1_Real;
			}
			else
			{
				return pos1_Real;
			}
		}


		private static Vector2 GetClippedVertexNoBase(Vector2 posTargetGL)
		{
			Vector2 pos1_Real = posTargetGL;

			if (pos1_Real.x < 0.0f || pos1_Real.x > _windowWidth ||
				pos1_Real.y < 0.0f || pos1_Real.y > _windowHeight)
			{
				if (posTargetGL.x < 0.0f)
				{
					pos1_Real.x = 0.0f;
				}
				else if (posTargetGL.x > _windowWidth)
				{
					pos1_Real.x = _windowWidth;
				}

				if (posTargetGL.y < 0.0f)
				{
					pos1_Real.y = 0.0f;
				}
				else if (posTargetGL.y > _windowHeight)
				{
					pos1_Real.y = _windowHeight;
				}
				return pos1_Real;
			}
			else
			{
				return pos1_Real;
			}
		}

		// 최적화형
		//-------------------------------------------------------------------------------
		public static void BeginBatch_ColoredPolygon()
		{
			_matBatch.SetPass_Color();
			_matBatch.SetClippingSize(_glScreenClippingSize);

			GL.Begin(GL.TRIANGLES);
		}

		public static void BeginBatch_ColoredLine()
		{
			_matBatch.SetPass_Color();
			_matBatch.SetClippingSize(_glScreenClippingSize);

			GL.Begin(GL.LINES);
		}

		public static void EndBatch()
		{
			GL.End();
		}

		public static void RefreshScreenSizeToBatch()
		{
			_matBatch.SetClippingSizeToAllMaterial(_glScreenClippingSize);
		}
		//-------------------------------------------------------------------------------
		// Draw Line
		//-------------------------------------------------------------------------------
		public static void DrawLine(Vector2 pos1, Vector2 pos2, Color color)
		{
			DrawLine(pos1, pos2, color, true);
		}

		public static void DrawLine(Vector2 pos1, Vector2 pos2, Color color, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{ return; }

			if (Vector2.Equals(pos1, pos2))
			{ return; }

			pos1 = World2GL(pos1);
			pos2 = World2GL(pos2);

			if (isNeedResetMat)
			{
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);
			}

			GL.Color(color);
			GL.Vertex(new Vector3(pos1.x, pos1.y, 0.0f));
			GL.Vertex(new Vector3(pos2.x, pos2.y, 0.0f));

			if (isNeedResetMat)
			{
				GL.End();
			}
		}

		public static void DrawLineGL(Vector2 pos1_GL, Vector2 pos2_GL, Color color, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{ return; }

			if (Vector2.Equals(pos1_GL, pos2_GL))
			{ return; }


			if (isNeedResetMat)
			{
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);
			}

			GL.Color(color);
			GL.Vertex(new Vector3(pos1_GL.x, pos1_GL.y, 0.0f));
			GL.Vertex(new Vector3(pos2_GL.x, pos2_GL.y, 0.0f));

			if (isNeedResetMat)
			{
				GL.End();
			}
		}




		//추가 : 애니메이션되는 라인
		public static void DrawAnimatedLine(Vector2 pos1, Vector2 pos2, Color color, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{ return; }

			if (Vector2.Equals(pos1, pos2))
			{ return; }

			pos1 = World2GL(pos1);
			pos2 = World2GL(pos2);

			if (isNeedResetMat)
			{
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);
			}

			GL.Color(color);

			Vector2 vLine = (pos2 - pos1);
			float remainedLength = vLine.magnitude;
			vLine.Normalize();
			float startOffset = _animationTimeRatio * (ANIMATED_LINE_UNIT_LENGTH + ANIMATED_LINE_SPACE_LENGTH);
			Vector2 curPos = pos1 + vLine * startOffset;
			remainedLength -= startOffset;

			if(startOffset - ANIMATED_LINE_SPACE_LENGTH > 0)
			{
				GL.Vertex(new Vector3(pos1.x, pos1.y, 0.0f));
				GL.Vertex(new Vector3(	pos1.x + vLine.x * (startOffset - ANIMATED_LINE_SPACE_LENGTH), 
										pos1.y + vLine.y * (startOffset - ANIMATED_LINE_SPACE_LENGTH), 0.0f));
			}
			
			//움직이는 점선라인을 그리자
			while(true)
			{
				if(remainedLength < 0.0f)
				{
					break;
				}

				GL.Vertex(new Vector3(curPos.x, curPos.y, 0.0f));
				if(remainedLength > ANIMATED_LINE_UNIT_LENGTH)
				{
					GL.Vertex(new Vector3(	curPos.x + vLine.x * ANIMATED_LINE_UNIT_LENGTH, 
											curPos.y + vLine.y * ANIMATED_LINE_UNIT_LENGTH, 
											0.0f));
				}
				else
				{
					GL.Vertex(new Vector3(	curPos.x + vLine.x * remainedLength, 
											curPos.y + vLine.y * remainedLength, 
											0.0f));
					break;
				}
				//이동
				curPos += vLine * (ANIMATED_LINE_UNIT_LENGTH + ANIMATED_LINE_SPACE_LENGTH);
				remainedLength -= ANIMATED_LINE_UNIT_LENGTH + ANIMATED_LINE_SPACE_LENGTH;
			}
			

			if (isNeedResetMat)
			{
				GL.End();
			}
		}

		public static void DrawAnimatedLineGL(Vector2 pos1_GL, Vector2 pos2_GL, Color color, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{ return; }

			if (Vector2.Equals(pos1_GL, pos2_GL))
			{ return; }


			if (isNeedResetMat)
			{
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);
			}

			GL.Color(color);

			Vector2 vLine = (pos2_GL - pos1_GL);
			float remainedLength = vLine.magnitude;
			vLine.Normalize();
			float startOffset = _animationTimeRatio * (ANIMATED_LINE_UNIT_LENGTH + ANIMATED_LINE_SPACE_LENGTH);
			Vector2 curPos = pos1_GL + vLine * startOffset;
			remainedLength -= startOffset;

			if(startOffset - ANIMATED_LINE_SPACE_LENGTH > 0)
			{
				GL.Vertex(new Vector3(pos1_GL.x, pos1_GL.y, 0.0f));
				GL.Vertex(new Vector3(	pos1_GL.x + vLine.x * (startOffset - ANIMATED_LINE_SPACE_LENGTH), 
										pos1_GL.y + vLine.y * (startOffset - ANIMATED_LINE_SPACE_LENGTH), 0.0f));
			}

			//움직이는 점선라인을 그리자
			while(true)
			{
				if(remainedLength < 0.0f)
				{
					break;
				}

				GL.Vertex(new Vector3(curPos.x, curPos.y, 0.0f));
				if(remainedLength > ANIMATED_LINE_UNIT_LENGTH)
				{
					GL.Vertex(new Vector3(	curPos.x + vLine.x * ANIMATED_LINE_UNIT_LENGTH, 
											curPos.y + vLine.y * ANIMATED_LINE_UNIT_LENGTH, 
											0.0f));
				}
				else
				{
					GL.Vertex(new Vector3(	curPos.x + vLine.x * remainedLength, 
											curPos.y + vLine.y * remainedLength, 
											0.0f));
					break;
				}
				//이동
				curPos += vLine * (ANIMATED_LINE_UNIT_LENGTH + ANIMATED_LINE_SPACE_LENGTH);
				remainedLength -= ANIMATED_LINE_UNIT_LENGTH + ANIMATED_LINE_SPACE_LENGTH;
			}
			

			if (isNeedResetMat)
			{
				GL.End();
			}
		}




		public static void DrawDotLine(Vector2 pos1, Vector2 pos2, Color color, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{ return; }

			if (Vector2.Equals(pos1, pos2))
			{ return; }

			pos1 = World2GL(pos1);
			pos2 = World2GL(pos2);

			if (isNeedResetMat)
			{
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);
			}

			GL.Color(color);

			Vector2 vLine = (pos2 - pos1);
			float remainedLength = vLine.magnitude;
			vLine.Normalize();
			//float startOffset = _animationTimeRatio * (ANIMATED_LINE_UNIT_LENGTH + ANIMATED_LINE_SPACE_LENGTH);
			//Vector2 curPos = pos1 + vLine * startOffset;
			Vector2 curPos = pos1 + vLine;
			//remainedLength -= startOffset;

			//if(startOffset - ANIMATED_LINE_SPACE_LENGTH > 0)
			//{
			//	GL.Vertex(new Vector3(pos1.x, pos1.y, 0.0f));
			//	GL.Vertex(new Vector3(	pos1.x + vLine.x * (startOffset - ANIMATED_LINE_SPACE_LENGTH), 
			//							pos1.y + vLine.y * (startOffset - ANIMATED_LINE_SPACE_LENGTH), 0.0f));
			//}
			
			//움직이는 점선라인을 그리자
			while(true)
			{
				if(remainedLength < 0.0f)
				{
					break;
				}

				GL.Vertex(new Vector3(curPos.x, curPos.y, 0.0f));
				if(remainedLength > ANIMATED_LINE_UNIT_LENGTH)
				{
					GL.Vertex(new Vector3(	curPos.x + vLine.x * ANIMATED_LINE_UNIT_LENGTH, 
											curPos.y + vLine.y * ANIMATED_LINE_UNIT_LENGTH, 
											0.0f));
				}
				else
				{
					GL.Vertex(new Vector3(	curPos.x + vLine.x * remainedLength, 
											curPos.y + vLine.y * remainedLength, 
											0.0f));
					break;
				}
				//이동
				curPos += vLine * (ANIMATED_LINE_UNIT_LENGTH + ANIMATED_LINE_SPACE_LENGTH);
				remainedLength -= ANIMATED_LINE_UNIT_LENGTH + ANIMATED_LINE_SPACE_LENGTH;
			}
			

			if (isNeedResetMat)
			{
				GL.End();
			}
		}

		//-------------------------------------------------------------------------------
		// Draw Box
		//-------------------------------------------------------------------------------
		public static void DrawBox(Vector2 pos, float width, float height, Color color, bool isWireframe)
		{
			DrawBox(pos, width, height, color, isWireframe, true);
		}
		public static void DrawBox(Vector2 pos, float width, float height, Color color, bool isWireframe, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{ return; }

			pos = World2GL(pos);

			float halfWidth = width * 0.5f * _zoom;
			float halfHeight = height * 0.5f * _zoom;

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)
			Vector2 pos_0 = new Vector2(pos.x - halfWidth, pos.y - halfHeight);
			Vector2 pos_1 = new Vector2(pos.x + halfWidth, pos.y - halfHeight);
			Vector2 pos_2 = new Vector2(pos.x + halfWidth, pos.y + halfHeight);
			Vector2 pos_3 = new Vector2(pos.x - halfWidth, pos.y + halfHeight);

			if (isWireframe)
			{
				if (isNeedResetMat)
				{
					_matBatch.SetPass_Color();
					_matBatch.SetClippingSize(_glScreenClippingSize);

					GL.Begin(GL.LINES);
				}

				GL.Color(color);
				GL.Vertex(pos_0);
				GL.Vertex(pos_1);

				GL.Vertex(pos_1);
				GL.Vertex(pos_2);

				GL.Vertex(pos_2);
				GL.Vertex(pos_3);

				GL.Vertex(pos_3);
				GL.Vertex(pos_0);
				if (isNeedResetMat)
				{
					GL.End();
				}
			}
			else
			{
				//CW
				// -------->
				// | 0   1
				// | 		
				// | 3   2
				if (isNeedResetMat)
				{
					_matBatch.SetPass_Color();
					_matBatch.SetClippingSize(_glScreenClippingSize);


					GL.Begin(GL.TRIANGLES);
				}
				GL.Color(color);
				GL.Vertex(pos_0); // 0
				GL.Vertex(pos_1); // 1
				GL.Vertex(pos_2); // 2

				GL.Vertex(pos_2); // 2
				GL.Vertex(pos_3); // 3
				GL.Vertex(pos_0); // 0

				if (isNeedResetMat)
				{
					GL.End();
				}
			}

			//GL.Flush();
		}


		public static void DrawBoxGL(Vector2 pos, float width, float height, Color color, bool isWireframe, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{ return; }

			float halfWidth = width * 0.5f * _zoom;
			float halfHeight = height * 0.5f * _zoom;

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)
			Vector2 pos_0 = new Vector2(pos.x - halfWidth, pos.y - halfHeight);
			Vector2 pos_1 = new Vector2(pos.x + halfWidth, pos.y - halfHeight);
			Vector2 pos_2 = new Vector2(pos.x + halfWidth, pos.y + halfHeight);
			Vector2 pos_3 = new Vector2(pos.x - halfWidth, pos.y + halfHeight);

			if (isWireframe)
			{
				if (isNeedResetMat)
				{
					_matBatch.SetPass_Color();
					_matBatch.SetClippingSize(_glScreenClippingSize);

					GL.Begin(GL.LINES);
				}

				GL.Color(color);
				GL.Vertex(pos_0);
				GL.Vertex(pos_1);
				GL.Vertex(pos_1);
				GL.Vertex(pos_2);
				GL.Vertex(pos_2);
				GL.Vertex(pos_3);
				GL.Vertex(pos_3);
				GL.Vertex(pos_0);
				if (isNeedResetMat)
				{
					GL.End();
				}
			}
			else
			{
				//CW
				// -------->
				// | 0   1
				// | 		
				// | 3   2
				if (isNeedResetMat)
				{
					_matBatch.SetPass_Color();
					_matBatch.SetClippingSize(_glScreenClippingSize);


					GL.Begin(GL.TRIANGLES);
				}
				GL.Color(color);
				// 0 - 1 - 2
				GL.Vertex(pos_0);
				GL.Vertex(pos_1);
				GL.Vertex(pos_2);

				// 2 - 3 - 0
				GL.Vertex(pos_2);
				GL.Vertex(pos_3);
				GL.Vertex(pos_0);

				if (isNeedResetMat)
				{
					GL.End();
				}
			}

			//GL.Flush();
		}



		public static void DrawCircle(Vector2 pos, float radius, Color color, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{
				return;
			}

			pos = World2GL(pos);

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)

			if (isNeedResetMat)
			{
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);
			}

			float radiusGL = radius * _zoom;
			GL.Color(color);
			for (int i = 0; i < 36; i++)
			{
				float angleRad_0 = (i / 36.0f) * Mathf.PI * 2.0f;
				float angleRad_1 = ((i + 1) / 36.0f) * Mathf.PI * 2.0f;

				Vector2 pos0 = pos + new Vector2(Mathf.Cos(angleRad_0) * radiusGL, Mathf.Sin(angleRad_0) * radiusGL);
				Vector2 pos1 = pos + new Vector2(Mathf.Cos(angleRad_1) * radiusGL, Mathf.Sin(angleRad_1) * radiusGL);

				GL.Vertex(pos0);
				GL.Vertex(pos1);
			}
			if (isNeedResetMat)
			{
				GL.End();
			}


			//GL.Flush();
		}



		public static void DrawBoldCircleGL(Vector2 posGL, float radius, float lineWidth, Color color, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{
				return;
			}

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)

			if (isNeedResetMat)
			{
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.TRIANGLES);
			}

			float radiusGL = radius * _zoom;
			//float radiusGL = radius;
			GL.Color(color);
			for (int i = 0; i < 36; i++)
			{
				float angleRad_0 = (i / 36.0f) * Mathf.PI * 2.0f;
				float angleRad_1 = ((i + 1) / 36.0f) * Mathf.PI * 2.0f;

				Vector2 pos0 = posGL + new Vector2(Mathf.Cos(angleRad_0) * radiusGL, Mathf.Sin(angleRad_0) * radiusGL);
				Vector2 pos1 = posGL + new Vector2(Mathf.Cos(angleRad_1) * radiusGL, Mathf.Sin(angleRad_1) * radiusGL);

				//GL.Vertex(pos0);
				//GL.Vertex(pos1);

				DrawBoldLineGL(pos0, pos1, lineWidth, color, false);
			}

			if (isNeedResetMat)
			{
				GL.End();
			}


			//GL.Flush();
		}



		public static void DrawBoldLine(Vector2 pos1, Vector2 pos2, float width, Color color, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{
				return;
			}

			pos1 = World2GL(pos1);
			pos2 = World2GL(pos2);

			if (pos1 == pos2)
			{
				return;
			}

			//float halfWidth = width * 0.5f / _zoom;
			float halfWidth = width * 0.5f;

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)

			// -------->
			// |    1
			// | 0     2
			// | 
			// | 
			// | 
			// | 5     3
			// |    4

			Vector2 dir = (pos1 - pos2).normalized;
			Vector2 dirRev = new Vector2(-dir.y, dir.x);

			Vector2 pos_0 = pos1 - dirRev * halfWidth;
			Vector2 pos_1 = pos1 + dir * halfWidth;
			//Vector2 pos_1 = pos1;
			Vector2 pos_2 = pos1 + dirRev * halfWidth;

			Vector2 pos_3 = pos2 + dirRev * halfWidth;
			Vector2 pos_4 = pos2 - dir * halfWidth;
			//Vector2 pos_4 = pos2;
			Vector2 pos_5 = pos2 - dirRev * halfWidth;

			if (isNeedResetMat)
			{
				//_mat_Color.SetPass(0);
				//_mat_Color.SetVector("_ScreenSize", _glScreenClippingSize);
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.TRIANGLES);
			}
			GL.Color(color);
			// 0 - 1 - 2
			GL.Vertex(pos_0);	GL.Vertex(pos_1);	GL.Vertex(pos_2);
			GL.Vertex(pos_2);	GL.Vertex(pos_1);	GL.Vertex(pos_0);

			// 0 - 2 - 3
			GL.Vertex(pos_0);	GL.Vertex(pos_2);	GL.Vertex(pos_3);
			GL.Vertex(pos_3);	GL.Vertex(pos_2);	GL.Vertex(pos_0);

			// 3 - 5 - 0
			GL.Vertex(pos_3);	GL.Vertex(pos_5);	GL.Vertex(pos_0);
			GL.Vertex(pos_0);	GL.Vertex(pos_5);	GL.Vertex(pos_3);

			// 3 - 4 - 5
			GL.Vertex(pos_3);	GL.Vertex(pos_4);	GL.Vertex(pos_5);
			GL.Vertex(pos_5);	GL.Vertex(pos_4);	GL.Vertex(pos_3);

			if (isNeedResetMat)
			{
				GL.End();
			}
		}


		public static void DrawBoldLineGL(Vector2 pos1, Vector2 pos2, float width, Color color, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{
				return;
			}

			if (pos1 == pos2)
			{ return; }

			//float halfWidth = width * 0.5f / _zoom;
			float halfWidth = width * 0.5f;

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)

			// -------->
			// |    1
			// | 0     2
			// | 
			// | 
			// | 
			// | 5     3
			// |    4

			Vector2 dir = (pos1 - pos2).normalized;
			Vector2 dirRev = new Vector2(-dir.y, dir.x);

			Vector2 pos_0 = pos1 - dirRev * halfWidth;
			Vector2 pos_1 = pos1 + dir * halfWidth;
			//Vector2 pos_1 = pos1;
			Vector2 pos_2 = pos1 + dirRev * halfWidth;

			Vector2 pos_3 = pos2 + dirRev * halfWidth;
			Vector2 pos_4 = pos2 - dir * halfWidth;
			//Vector2 pos_4 = pos2;
			Vector2 pos_5 = pos2 - dirRev * halfWidth;

			if (isNeedResetMat)
			{
				//_mat_Color.SetPass(0);
				//_mat_Color.SetVector("_ScreenSize", _glScreenClippingSize);
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.TRIANGLES);
			}
			GL.Color(color);
			// 0 - 1 - 2
			GL.Vertex(pos_0);	GL.Vertex(pos_1);	GL.Vertex(pos_2);
			GL.Vertex(pos_2);	GL.Vertex(pos_1);	GL.Vertex(pos_0);

			// 0 - 2 - 3
			GL.Vertex(pos_0);	GL.Vertex(pos_2);	GL.Vertex(pos_3);
			GL.Vertex(pos_3);	GL.Vertex(pos_2);	GL.Vertex(pos_0);

			// 3 - 5 - 0
			GL.Vertex(pos_3);	GL.Vertex(pos_5);	GL.Vertex(pos_0);
			GL.Vertex(pos_0);	GL.Vertex(pos_5);	GL.Vertex(pos_3);

			// 3 - 4 - 5
			GL.Vertex(pos_3);	GL.Vertex(pos_4);	GL.Vertex(pos_5);
			GL.Vertex(pos_5);	GL.Vertex(pos_4);	GL.Vertex(pos_3);

			if (isNeedResetMat)
			{
				GL.End();
			}
		}
		//-------------------------------------------------------------------------------
		// Draw Text
		//-------------------------------------------------------------------------------
		public static void DrawText(string text, Vector2 pos, float width, Color color)
		{
			//if(_mat_Color == null || _mat_Texture == null)
			//{
			//	return;
			//}
			if (_matBatch.IsNotReady())
			{
				return;
			}

			pos = World2GL(pos);

			if (IsVertexClipped(pos))
			{
				return;
			}

			if (IsVertexClipped(pos + new Vector2(width * _zoom, 15)))
			{
				return;
			}
			_textStyle.normal.textColor = color;


			GUI.Label(new Rect(pos.x, pos.y, 100.0f, 30.0f), text, _textStyle);
		}


		public static void DrawTextGL(string text, Vector2 pos, float width, Color color)
		{
			if (_matBatch.IsNotReady())
			{
				return;
			}

			if (IsVertexClipped(pos))
			{
				return;
			}

			if (IsVertexClipped(pos + new Vector2(width, 15)))
			{
				return;
			}
			_textStyle.normal.textColor = color;


			GUI.Label(new Rect(pos.x, pos.y, width + 50, 30.0f), text, _textStyle);
		}


		//-------------------------------------------------------------------------------
		// Draw Texture
		//-------------------------------------------------------------------------------
		public static void DrawTexture(Texture2D image, Vector2 pos, float width, float height, Color color2X)
		{
			DrawTexture(image, pos, width, height, color2X, 0.0f);
		}

		public static void DrawTexture(Texture2D image, Vector2 pos, float width, float height, Color color2X, float depth)
		{
			if (_matBatch.IsNotReady())
			{
				return;
			}

			pos = World2GL(pos);


			float realWidth = width * _zoom;
			float realHeight = height * _zoom;

			float realWidth_Half = realWidth * 0.5f;
			float realHeight_Half = realHeight * 0.5f;

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)
			Vector2 pos_0 = new Vector2(pos.x - realWidth_Half, pos.y - realHeight_Half);
			Vector2 pos_1 = new Vector2(pos.x + realWidth_Half, pos.y - realHeight_Half);
			Vector2 pos_2 = new Vector2(pos.x + realWidth_Half, pos.y + realHeight_Half);
			Vector2 pos_3 = new Vector2(pos.x - realWidth_Half, pos.y + realHeight_Half);


			float widthResize = (pos_1.x - pos_0.x);
			float heightResize = (pos_3.y - pos_0.y);

			if (widthResize < 1.0f || heightResize < 1.0f)
			{
				return;
			}


			float u_left = 0.0f;
			float u_right = 1.0f;

			float v_top = 0.0f;
			float v_bottom = 1.0f;

			Vector3 uv_0 = new Vector3(u_left, v_bottom, 0.0f);
			Vector3 uv_1 = new Vector3(u_right, v_bottom, 0.0f);
			Vector3 uv_2 = new Vector3(u_right, v_top, 0.0f);
			Vector3 uv_3 = new Vector3(u_left, v_top, 0.0f);

			//CW
			// -------->
			// | 0   1
			// | 		
			// | 3   2
			_matBatch.SetPass_Texture_Normal(color2X, image, apPortrait.SHADER_TYPE.AlphaBlend);
			_matBatch.SetClippingSize(_glScreenClippingSize);


			GL.Begin(GL.TRIANGLES);

			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos_0.x, pos_0.y, depth)); // 0
			GL.TexCoord(uv_1);	GL.Vertex(new Vector3(pos_1.x, pos_1.y, depth)); // 1
			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos_2.x, pos_2.y, depth)); // 2

			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos_2.x, pos_2.y, depth)); // 2
			GL.TexCoord(uv_3);	GL.Vertex(new Vector3(pos_3.x, pos_3.y, depth)); // 3
			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos_0.x, pos_0.y, depth)); // 0

			GL.End();

			//GL.Flush();
		}






		public static void DrawTexture(Texture2D image, apMatrix3x3 matrix, float width, float height, Color color2X, float depth)
		{
			if (_matBatch.IsNotReady())
			{
				return;
			}

			float width_Half = width * 0.5f;
			float height_Half = height * 0.5f;

			//Zero 대신 mesh Pivot 위치로 삼자
			Vector2 pos_0 = World2GL(matrix.MultiplyPoint(new Vector2(-width_Half, +height_Half)));
			Vector2 pos_1 = World2GL(matrix.MultiplyPoint(new Vector2(+width_Half, +height_Half)));
			Vector2 pos_2 = World2GL(matrix.MultiplyPoint(new Vector2(+width_Half, -height_Half)));
			Vector2 pos_3 = World2GL(matrix.MultiplyPoint(new Vector2(-width_Half, -height_Half)));

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)
			float u_left = 0.0f;
			float u_right = 1.0f;

			float v_top = 0.0f;
			float v_bottom = 1.0f;

			Vector3 uv_0 = new Vector3(u_left, v_bottom, 0.0f);
			Vector3 uv_1 = new Vector3(u_right, v_bottom, 0.0f);
			Vector3 uv_2 = new Vector3(u_right, v_top, 0.0f);
			Vector3 uv_3 = new Vector3(u_left, v_top, 0.0f);

			//CW
			// -------->
			// | 0   1
			// | 		
			// | 3   2
			_matBatch.SetPass_Texture_Normal(color2X, image, apPortrait.SHADER_TYPE.AlphaBlend);
			_matBatch.SetClippingSize(_glScreenClippingSize);


			GL.Begin(GL.TRIANGLES);

			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos_0.x, pos_0.y, depth)); // 0
			GL.TexCoord(uv_1);	GL.Vertex(new Vector3(pos_1.x, pos_1.y, depth)); // 1
			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos_2.x, pos_2.y, depth)); // 2

			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos_2.x, pos_2.y, depth)); // 2
			GL.TexCoord(uv_3);	GL.Vertex(new Vector3(pos_3.x, pos_3.y, depth)); // 3
			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos_0.x, pos_0.y, depth)); // 0


			GL.End();
		}

		public static void DrawTextureGL(Texture2D image, Vector2 pos, float width, float height, Color color2X, float depth)
		{
			if (_matBatch.IsNotReady())
			{
				return;
			}

			float realWidth = width * _zoom;
			float realHeight = height * _zoom;

			float realWidth_Half = realWidth * 0.5f;
			float realHeight_Half = realHeight * 0.5f;

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)
			Vector2 pos_0 = new Vector2(pos.x - realWidth_Half, pos.y - realHeight_Half);
			Vector2 pos_1 = new Vector2(pos.x + realWidth_Half, pos.y - realHeight_Half);
			Vector2 pos_2 = new Vector2(pos.x + realWidth_Half, pos.y + realHeight_Half);
			Vector2 pos_3 = new Vector2(pos.x - realWidth_Half, pos.y + realHeight_Half);


			float widthResize = (pos_1.x - pos_0.x);
			float heightResize = (pos_3.y - pos_0.y);

			if (widthResize < 1.0f || heightResize < 1.0f)
			{
				return;
			}


			float u_left = 0.0f;
			float u_right = 1.0f;

			float v_top = 0.0f;
			float v_bottom = 1.0f;

			Vector3 uv_0 = new Vector3(u_left, v_bottom, 0.0f);
			Vector3 uv_1 = new Vector3(u_right, v_bottom, 0.0f);
			Vector3 uv_2 = new Vector3(u_right, v_top, 0.0f);
			Vector3 uv_3 = new Vector3(u_left, v_top, 0.0f);

			//CW
			// -------->
			// | 0   1
			// | 		
			// | 3   2
			_matBatch.SetPass_Texture_Normal(color2X, image, apPortrait.SHADER_TYPE.AlphaBlend);
			_matBatch.SetClippingSize(_glScreenClippingSize);


			GL.Begin(GL.TRIANGLES);

			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos_0.x, pos_0.y, depth)); // 0
			GL.TexCoord(uv_1);	GL.Vertex(new Vector3(pos_1.x, pos_1.y, depth)); // 1
			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos_2.x, pos_2.y, depth)); // 2

			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos_2.x, pos_2.y, depth)); // 2
			GL.TexCoord(uv_3);	GL.Vertex(new Vector3(pos_3.x, pos_3.y, depth)); // 3
			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos_0.x, pos_0.y, depth)); // 0

			GL.End();

			//GL.Flush();
		}



		public static void DrawTextureGL(Texture2D image, Vector2 pos, float width, float height, Color color2X, float depth, bool isNeedResetMat)
		{
			if (_matBatch.IsNotReady())
			{
				return;
			}

			float realWidth = width * _zoom;
			float realHeight = height * _zoom;

			float realWidth_Half = realWidth * 0.5f;
			float realHeight_Half = realHeight * 0.5f;

			//CW
			// -------->
			// | 0(--) 1
			// | 		
			// | 3   2 (++)
			Vector2 pos_0 = new Vector2(pos.x - realWidth_Half, pos.y - realHeight_Half);
			Vector2 pos_1 = new Vector2(pos.x + realWidth_Half, pos.y - realHeight_Half);
			Vector2 pos_2 = new Vector2(pos.x + realWidth_Half, pos.y + realHeight_Half);
			Vector2 pos_3 = new Vector2(pos.x - realWidth_Half, pos.y + realHeight_Half);


			float widthResize = (pos_1.x - pos_0.x);
			float heightResize = (pos_3.y - pos_0.y);

			if (widthResize < 1.0f || heightResize < 1.0f)
			{
				return;
			}


			float u_left = 0.0f;
			float u_right = 1.0f;

			float v_top = 0.0f;
			float v_bottom = 1.0f;

			Vector3 uv_0 = new Vector3(u_left, v_bottom, 0.0f);
			Vector3 uv_1 = new Vector3(u_right, v_bottom, 0.0f);
			Vector3 uv_2 = new Vector3(u_right, v_top, 0.0f);
			Vector3 uv_3 = new Vector3(u_left, v_top, 0.0f);

			//CW
			// -------->
			// | 0   1
			// | 		
			// | 3   2
			if (isNeedResetMat)
			{
				_matBatch.SetPass_Texture_Normal(color2X, image, apPortrait.SHADER_TYPE.AlphaBlend);
				_matBatch.SetClippingSize(_glScreenClippingSize);


				GL.Begin(GL.TRIANGLES);
			}

			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos_0.x, pos_0.y, depth)); // 0
			GL.TexCoord(uv_1);	GL.Vertex(new Vector3(pos_1.x, pos_1.y, depth)); // 1
			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos_2.x, pos_2.y, depth)); // 2

			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos_2.x, pos_2.y, depth)); // 2
			GL.TexCoord(uv_3);	GL.Vertex(new Vector3(pos_3.x, pos_3.y, depth)); // 3
			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos_0.x, pos_0.y, depth)); // 0

			if (isNeedResetMat)
			{
				GL.End();
			}


			//GL.Flush();
		}

		/// <summary>
		/// 이미 VColor Texture Pass가 시작될 때 사용하는 Box 그리기 함수
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="vColor"></param>
		private static void DrawBoxWithVColorAndUV(Vector2 pos, float width, float height, Color vColor)
		{
			float width_Half = width * 0.5f;
			float height_Half = height * 0.5f;

			Vector3 uv_0 = new Vector3(0, 1, 0.0f);
			Vector3 uv_1 = new Vector3(1, 1, 0.0f);
			Vector3 uv_2 = new Vector3(1, 0, 0.0f);
			Vector3 uv_3 = new Vector3(0, 0, 0.0f);

			GL.Color(vColor);

			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos.x - width_Half, pos.y - height_Half, 0)); // 0
			GL.TexCoord(uv_1);	GL.Vertex(new Vector3(pos.x + width_Half, pos.y - height_Half, 0)); // 1
			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos.x + width_Half, pos.y + height_Half, 0)); // 2

			GL.TexCoord(uv_2);	GL.Vertex(new Vector3(pos.x + width_Half, pos.y + height_Half, 0)); // 2
			GL.TexCoord(uv_3);	GL.Vertex(new Vector3(pos.x - width_Half, pos.y + height_Half, 0)); // 3
			GL.TexCoord(uv_0);	GL.Vertex(new Vector3(pos.x - width_Half, pos.y - height_Half, 0)); // 0
		}
		//-------------------------------------------------------------------------------
		// Draw Mesh
		//-------------------------------------------------------------------------------
		public static void DrawMesh(apMesh mesh, apMatrix3x3 matrix, Color color2X, RENDER_TYPE renderType, apVertexController vertexController, apEditor editor, Vector2 mousePosition)
		{
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				//if (mesh == null || mesh._textureData == null || mesh._textureData._image == null)//이전 코드
				if (mesh == null || mesh.LinkedTextureData == null || mesh.LinkedTextureData._image == null)//변경 코드
				{
					DrawBox(Vector2.zero, 512, 512, Color.red, true);
					DrawText("No Image", Vector2.zero, 80, Color.cyan);
					return;
				}

				//1. 모든 메시를 보여줄때 (또는 클리핑된 메시가 없을 때) => 
				bool isShowAllTexture = false;
				Color textureColor = _textureColor_Gray;
				if ((renderType & RENDER_TYPE.ShadeAllMesh) != 0 || mesh._indexBuffer.Count < 3)
				{
					isShowAllTexture = true;
					textureColor = _textureColor_Shade;
				}
				else if ((renderType & RENDER_TYPE.AllMesh) != 0)
				{
					isShowAllTexture = true;
				}

				matrix *= mesh.Matrix_VertToLocal;

				if (isShowAllTexture)
				{
					//DrawTexture(mesh._textureData._image, matrix, mesh._textureData._width, mesh._textureData._height, textureColor, -10);
					DrawTexture(mesh.LinkedTextureData._image, matrix, mesh.LinkedTextureData._width, mesh.LinkedTextureData._height, textureColor, -10);
				}

				apVertex selectedVertex = null;
				List<apVertex> selectedVertices = null;
				apVertex nextSelectedVertex = null;
				apBone selectedBone = null;
				apMeshPolygon selectedPolygon = null;
				if (vertexController != null)
				{
					selectedVertex = vertexController.Vertex;
					selectedVertices = vertexController.Vertices;
					nextSelectedVertex = vertexController.LinkedNextVertex;
					selectedBone = vertexController.Bone;
					selectedPolygon = vertexController.Polygon;
				}


				Vector2 pos2_0 = Vector2.zero;
				Vector2 pos2_1 = Vector2.zero;
				Vector2 pos2_2 = Vector2.zero;

				Vector3 pos_0 = Vector3.zero;
				Vector3 pos_1 = Vector3.zero;
				Vector3 pos_2 = Vector3.zero;

				Vector2 uv_0 = Vector2.zero;
				Vector2 uv_1 = Vector2.zero;
				Vector2 uv_2 = Vector2.zero;

				//2. 메시를 렌더링하자
				if (mesh._indexBuffer.Count >= 3)
				{
					//Debug.Log("Draw Mesh.. [" + (mesh.LinkedTextureData._image != null) + "]");
					//Debug.Log("Size : " + mesh.LinkedTextureData._uniqueID + " / " + mesh.LinkedTextureData._name + " / " + mesh.LinkedTextureData._width + "x" + mesh.LinkedTextureData._height);
					//Debug.Log("Texture : " + mesh.LinkedTextureData._image.name + " / " + mesh.LinkedTextureData._image.width + "x" + mesh.LinkedTextureData._image.height);
					//------------------------------------------
					// Drawcall Batch를 했을때
					// <참고> Weight를 출력하고 싶다면 Normal 대신 VColor를 넣고, VertexColor를 넣어주자
					if ((renderType & RENDER_TYPE.VolumeWeightColor) != 0)
					{
						//_matBatch.SetPass_Texture_VColor(_textureColor_Gray, mesh._textureData._image, 1.0f, apPortrait.SHADER_TYPE.AlphaBlend);
						_matBatch.SetPass_Texture_VColor(_textureColor_Gray, mesh.LinkedTextureData._image, 1.0f, apPortrait.SHADER_TYPE.AlphaBlend);
					}
					else
					{
						//_matBatch.SetPass_Texture_Normal(color2X, mesh._textureData._image, apPortrait.SHADER_TYPE.AlphaBlend);
						_matBatch.SetPass_Texture_Normal(color2X, mesh.LinkedTextureData._image, apPortrait.SHADER_TYPE.AlphaBlend);

					}
					
					_matBatch.SetClippingSize(_glScreenClippingSize);

					GL.Begin(GL.TRIANGLES);
					//------------------------------------------
					apVertex vert0, vert1, vert2;
					//Color color0 = Color.black, color1 = Color.black, color2 = Color.black;
					Color color0 = Color.white, color1 = Color.white, color2 = Color.white;
					int iVertColor = 0;
					if ((renderType & RENDER_TYPE.VolumeWeightColor) != 0)
					{
						iVertColor = 1;
					}
					else
					{
						iVertColor = 0;
						color0 = Color.white;
						color1 = Color.white;
						color2 = Color.white;
					}
					for (int i = 0; i < mesh._indexBuffer.Count; i += 3)
					{
						if (i + 2 >= mesh._indexBuffer.Count)
						{ break; }

						if (mesh._indexBuffer[i + 0] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 1] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 2] >= mesh._vertexData.Count)
						{
							break;
						}

						vert0 = mesh._vertexData[mesh._indexBuffer[i + 0]];
						vert1 = mesh._vertexData[mesh._indexBuffer[i + 1]];
						vert2 = mesh._vertexData[mesh._indexBuffer[i + 2]];

						
						pos2_0 = World2GL(matrix.MultiplyPoint(vert0._pos));
						pos2_1 = World2GL(matrix.MultiplyPoint(vert1._pos));
						pos2_2 = World2GL(matrix.MultiplyPoint(vert2._pos));

						pos_0 = new Vector3(pos2_0.x, pos2_0.y, vert0._zDepth * 0.1f);
						pos_1 = new Vector3(pos2_1.x, pos2_1.y, vert1._zDepth * 0.5f);
						pos_2 = new Vector3(pos2_2.x, pos2_2.y, vert2._zDepth * 0.5f);//<<Z값이 반영되었다.

						uv_0 = mesh._vertexData[mesh._indexBuffer[i + 0]]._uv;
						uv_1 = mesh._vertexData[mesh._indexBuffer[i + 1]]._uv;
						uv_2 = mesh._vertexData[mesh._indexBuffer[i + 2]]._uv;

						switch (iVertColor)
						{
							case 1: //VolumeWeightColor
								color0 = GetWeightGrayscale(vert0._zDepth);
								color1 = GetWeightGrayscale(vert1._zDepth);
								color2 = GetWeightGrayscale(vert2._zDepth);
								break;
						}
						////------------------------------------------

						GL.Color(color0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
						GL.Color(color1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(color2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

						//Back Side
						GL.Color(color2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
						GL.Color(color1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(color0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0

						

						////------------------------------------------
					}
					GL.End();

				}

				//미러 모드
				//--------------------------------------------------------------
				if(editor._meshEditMode == apEditor.MESH_EDIT_MODE.MakeMesh &&
					editor._meshEditMirrorMode == apEditor.MESH_EDIT_MIRROR_MODE.Mirror)
				{
					DrawMeshMirror(mesh);
				}


				// Atlas 외곽선
				//--------------------------------------------------------------
				if (mesh._isPSDParsed)
				{
					Vector2 pos_LT = matrix.MultiplyPoint(new Vector2(mesh._atlasFromPSD_LT.x, mesh._atlasFromPSD_LT.y));
					Vector2 pos_RT = matrix.MultiplyPoint(new Vector2(mesh._atlasFromPSD_RB.x, mesh._atlasFromPSD_LT.y));
					Vector2 pos_LB = matrix.MultiplyPoint(new Vector2(mesh._atlasFromPSD_LT.x, mesh._atlasFromPSD_RB.y));
					Vector2 pos_RB = matrix.MultiplyPoint(new Vector2(mesh._atlasFromPSD_RB.x, mesh._atlasFromPSD_RB.y));


					_matBatch.SetPass_Color();
					_matBatch.SetClippingSize(_glScreenClippingSize);
					GL.Begin(GL.LINES);

					DrawLine(pos_LT, pos_RT, editor._colorOption_AtlasBorder, false);
					DrawLine(pos_RT, pos_RB, editor._colorOption_AtlasBorder, false);
					DrawLine(pos_RB, pos_LB, editor._colorOption_AtlasBorder, false);
					DrawLine(pos_LB, pos_LT, editor._colorOption_AtlasBorder, false);

					GL.End();
				}

				//외곽선을 그려주자
				//float imageWidthHalf = mesh._textureData._width * 0.5f;
				//float imageHeightHalf = mesh._textureData._height * 0.5f;

				float imageWidthHalf = mesh.LinkedTextureData._width * 0.5f;
				float imageHeightHalf = mesh.LinkedTextureData._height * 0.5f;

				Vector2 pos_TexOutline_LT = matrix.MultiplyPoint(new Vector2(-imageWidthHalf, -imageHeightHalf));
				Vector2 pos_TexOutline_RT = matrix.MultiplyPoint(new Vector2(imageWidthHalf, -imageHeightHalf));
				Vector2 pos_TexOutline_LB = matrix.MultiplyPoint(new Vector2(-imageWidthHalf, imageHeightHalf));
				Vector2 pos_TexOutline_RB = matrix.MultiplyPoint(new Vector2(imageWidthHalf, imageHeightHalf));


				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);
				GL.Begin(GL.LINES);

				DrawLine(pos_TexOutline_LT, pos_TexOutline_RT, editor._colorOption_AtlasBorder, false);
				DrawLine(pos_TexOutline_RT, pos_TexOutline_RB, editor._colorOption_AtlasBorder, false);
				DrawLine(pos_TexOutline_RB, pos_TexOutline_LB, editor._colorOption_AtlasBorder, false);
				DrawLine(pos_TexOutline_LB, pos_TexOutline_LT, editor._colorOption_AtlasBorder, false);

				GL.End();


				//3. Edge를 렌더링하자 (전체 / Ouline)
				if ((renderType & RENDER_TYPE.AllEdges) != 0)
				{
					Vector2 pos0 = Vector2.zero, pos1 = Vector2.zero;
					if (mesh._edges.Count > 0)
					{
						_matBatch.SetPass_Color();
						_matBatch.SetClippingSize(_glScreenClippingSize);
						GL.Begin(GL.LINES);
						for (int i = 0; i < mesh._edges.Count; i++)
						{
							pos0 = matrix.MultiplyPoint(mesh._edges[i]._vert1._pos);
							pos1 = matrix.MultiplyPoint(mesh._edges[i]._vert2._pos);

							DrawLine(pos0, pos1, editor._colorOption_MeshEdge, false);
						}

						for (int iPoly = 0; iPoly < mesh._polygons.Count; iPoly++)
						{
							for (int iHE = 0; iHE < mesh._polygons[iPoly]._hidddenEdges.Count; iHE++)
							{
								apMeshEdge hiddenEdge = mesh._polygons[iPoly]._hidddenEdges[iHE];

								pos0 = matrix.MultiplyPoint(hiddenEdge._vert1._pos);
								pos1 = matrix.MultiplyPoint(hiddenEdge._vert2._pos);

								DrawLine(pos0, pos1, editor._colorOption_MeshHiddenEdge, false);
							}

						}

						GL.End();
					}
				}
				else if ((renderType & RENDER_TYPE.Outlines) != 0)
				{
					Vector2 pos0 = Vector2.zero, pos1 = Vector2.zero;
					if (mesh._edges.Count > 0)
					{
						_matBatch.SetPass_Color();
						_matBatch.SetClippingSize(_glScreenClippingSize);
						//GL.Begin(GL.LINES);
						GL.Begin(GL.TRIANGLES);
						for (int i = 0; i < mesh._edges.Count; i++)
						{
							if (!mesh._edges[i]._isOutline)
							{ continue; }
							pos0 = matrix.MultiplyPoint(mesh._edges[i]._vert1._pos);
							pos1 = matrix.MultiplyPoint(mesh._edges[i]._vert2._pos);

							//DrawLine(pos0, pos1, _lineColor_Outline, false);
							DrawBoldLine(pos0, pos1, 6.0f, editor._colorOption_Outline, false);
						}
						GL.End();
					}


				}

				if ((renderType & RENDER_TYPE.PolygonOutline) != 0)
				{
					if (selectedPolygon != null)
					{
						if (selectedPolygon._edges.Count > 0)
						{
							Vector2 pos0 = Vector2.zero, pos1 = Vector2.zero;

							_matBatch.SetPass_Color();
							_matBatch.SetClippingSize(_glScreenClippingSize);
							//GL.Begin(GL.LINES);
							GL.Begin(GL.TRIANGLES);
							for (int i = 0; i < selectedPolygon._edges.Count; i++)
							{
								pos0 = matrix.MultiplyPoint(selectedPolygon._edges[i]._vert1._pos);
								pos1 = matrix.MultiplyPoint(selectedPolygon._edges[i]._vert2._pos);

								//DrawLine(pos0, pos1, _lineColor_Outline, false);
								DrawBoldLine(pos0, pos1, 6.0f, editor._colorOption_Outline, false);
							}
							GL.End();
						}
					}
				}

				//3. 버텍스를 렌더링하자
				if ((renderType & RENDER_TYPE.Vertex) != 0)
				{
					bool isWireFramePoint = false;
					if (isWireFramePoint)
					{
						_matBatch.SetPass_Color();
						_matBatch.SetClippingSize(_glScreenClippingSize);

						GL.Begin(GL.LINES);
					}
					else
					{
						_matBatch.SetPass_Color();
						_matBatch.SetClippingSize(_glScreenClippingSize);

						GL.Begin(GL.TRIANGLES);
					}


					float pointSize = 10.0f / _zoom;
					Vector2 pos = Vector2.zero;
					for (int i = 0; i < mesh._vertexData.Count; i++)
					{
						Color vColor = editor._colorOption_VertColor_NotSelected;
						if (mesh._vertexData[i] == selectedVertex)
						{
							vColor = editor._colorOption_VertColor_Selected;
						}
						else if (mesh._vertexData[i] == nextSelectedVertex)
						{
							vColor = _vertColor_NextSelected;
						}
						else if(selectedVertices != null)
						{
							if(selectedVertices.Contains(mesh._vertexData[i]))
							{
								vColor = editor._colorOption_VertColor_Selected;
							}
						}
						pos = matrix.MultiplyPoint(mesh._vertexData[i]._pos);
						DrawBox(pos, pointSize, pointSize, vColor, isWireFramePoint, false);

						AddCursorRect(mousePosition, World2GL(pos), 10, 10, MouseCursor.MoveArrow);
					}

					GL.End();
				}



			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}


		//------------------------------------------------------------------------------------------------
		// Make Mesh 중 AutoGenerate 메뉴에서의 프리뷰 기능
		//------------------------------------------------------------------------------------------------
		public static void DrawMeshAutoGenerationPreview(apMesh mesh, 
															apMatrix3x3 matrix, 
															apEditor editor,
															apMeshGenerator meshGenerator)
		{
			if (mesh == null || mesh.LinkedTextureData == null || mesh.LinkedTextureData._image == null || editor == null || meshGenerator == null)
			{
				return;
			}

			if(!meshGenerator.IsScanned)
			{
				return;
			}
			
			//아웃라인을 그리자
			apMeshGenerator.ScanTileGroup outlineGroup = null;
			//apMeshGenerator.ScanTile scanTile = null;
			apMeshGenerator.OuterPoint outerPoint = null;
			
			Vector2 imageHalfOffset = new Vector2(mesh.LinkedTextureData._width * 0.5f, mesh.LinkedTextureData._height * 0.5f);
			
			Vector2 pos_A;
			Vector2 pos_B;
			Vector2 pos_0;
			Vector2 pos_1;
			Vector2 pos_2;
			Vector2 pos_3;


			//_matBatch.SetPass_Color();
			//_matBatch.SetClippingSize(_glScreenClippingSize);

			//GL.Begin(GL.LINES);
			//GL.Color(new Color(0.0f, 1.0f, 1.0f, 0.5f));

			//Vector2 pos_Min;
			//Vector2 pos_Max;

			//for (int iGroup = 0; iGroup < meshGenerator._outlineGroups.Count; iGroup++)
			//{
			//	outlineGroup = meshGenerator._outlineGroups[iGroup];
			//	for (int iTile = 0; iTile < outlineGroup._tiles.Count; iTile++)
			//	{
			//		scanTile = outlineGroup._tiles[iTile];
			//		pos_Min = World2GL(meshGenerator.GetTilePos_Min(scanTile._iX, scanTile._iY) - (mesh._offsetPos + imageHalfOffset));
			//		pos_Max = World2GL(meshGenerator.GetTilePos_Max(scanTile._iX, scanTile._iY) - (mesh._offsetPos + imageHalfOffset));

			//		GL.Vertex(new Vector3(pos_Min.x, pos_Min.y, 0));
			//		GL.Vertex(new Vector3(pos_Min.x, pos_Max.y, 0));

			//		GL.Vertex(new Vector3(pos_Min.x, pos_Max.y, 0));
			//		GL.Vertex(new Vector3(pos_Max.x, pos_Max.y, 0));

			//		GL.Vertex(new Vector3(pos_Max.x, pos_Max.y, 0));
			//		GL.Vertex(new Vector3(pos_Max.x, pos_Min.y, 0));

			//		GL.Vertex(new Vector3(pos_Max.x, pos_Min.y, 0));
			//		GL.Vertex(new Vector3(pos_Min.x, pos_Min.y, 0));
			//	}
			//}
			//GL.End();
			
			

			if (!meshGenerator.IsPreviewed)
			{
				//Scan 상태
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				Color lineColor_Enabled_Selected = new Color(1.0f, 1.0f, 0.0f, 0.8f);
				Color lineColor_Disabled_Selected = new Color(1.0f, 1.0f, 1.0f, 0.6f);
				Color lineColor_Enabled = new Color(1.0f, 0.0f, 1.0f, 0.8f);
				Color lineColor_Disabled = new Color(1.0f, 1.0f, 1.0f, 0.4f);
				bool isSelected = false;
				

				GL.Begin(GL.LINES);
				//GL.Color(new Color(1.0f, 0.0f, 1.0f, 0.8f));
				for (int iGroup = 0; iGroup < meshGenerator._outlineGroups.Count; iGroup++)
				{
					outlineGroup = meshGenerator._outlineGroups[iGroup];
					isSelected = meshGenerator._selectedOuterGroup == outlineGroup;

					for (int iPoint = 0; iPoint < outlineGroup._outerPoints.Count; iPoint++)
					{
						outerPoint = outlineGroup._outerPoints[iPoint];
						if (outerPoint._nextPoint != null)
						{
							pos_A = World2GL(outerPoint._pos - (mesh._offsetPos + imageHalfOffset));
							pos_B = World2GL(outerPoint._nextPoint._pos - (mesh._offsetPos + imageHalfOffset));
							if (isSelected)
							{
								DrawAnimatedLineGL(pos_A, pos_B, outlineGroup._isEnabled ? lineColor_Enabled_Selected : lineColor_Disabled_Selected, false);
							}
							else
							{
								DrawLineGL(pos_A, pos_B, outlineGroup._isEnabled ? lineColor_Enabled : lineColor_Disabled, false);
							}
							//GL.Vertex(pos_A);
							//GL.Vertex(pos_B);
						}
					}
				}
				GL.End();

				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				float pointHalfSize = 2;
				GL.Begin(GL.TRIANGLES);
				
				
				for (int iGroup = 0; iGroup < meshGenerator._outlineGroups.Count; iGroup++)
				{
					outlineGroup = meshGenerator._outlineGroups[iGroup];
					if(!outlineGroup._isEnabled)
					{
						continue;
					}
					for (int iPoint = 0; iPoint < outlineGroup._outerPoints.Count; iPoint++)
					{
						outerPoint = outlineGroup._outerPoints[iPoint];
						pos_A = World2GL(outerPoint._pos - (mesh._offsetPos + imageHalfOffset));

						pos_0 = pos_A + new Vector2(-pointHalfSize, -pointHalfSize);
						pos_1 = pos_A + new Vector2(pointHalfSize, -pointHalfSize);
						pos_2 = pos_A + new Vector2(pointHalfSize, pointHalfSize);
						pos_3 = pos_A + new Vector2(-pointHalfSize, pointHalfSize);

						GL.Color(new Color(1.0f, 1.0f, 0.0f, 0.8f));

						GL.Vertex(pos_0);
						GL.Vertex(pos_1);
						GL.Vertex(pos_2);
						GL.Vertex(pos_2);
						GL.Vertex(pos_3);
						GL.Vertex(pos_0);

						GL.Vertex(pos_2);
						GL.Vertex(pos_1);
						GL.Vertex(pos_0);
						GL.Vertex(pos_0);
						GL.Vertex(pos_3);
						GL.Vertex(pos_2);
					}
				}
				GL.End();

				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);


				for (int iGroup = 0; iGroup < meshGenerator._outlineGroups.Count; iGroup++)
				{
					outlineGroup = meshGenerator._outlineGroups[iGroup];
					if(!outlineGroup._isEnabled)
					{
						continue;
					}
					for (int iPoint = 0; iPoint < outlineGroup._outerPoints.Count; iPoint++)
					{
						outerPoint = outlineGroup._outerPoints[iPoint];
						pos_0 = World2GL(outerPoint._pos - (mesh._offsetPos + imageHalfOffset));
						//pos_1 = World2GL(outerPoint._pos + (outerPoint._normal_Prev * 3) - (mesh._offsetPos + imageHalfOffset));
						pos_2 = World2GL(outerPoint._pos + (outerPoint._normal_Avg * 7) - (mesh._offsetPos + imageHalfOffset));
						//pos_3 = World2GL(outerPoint._pos + (outerPoint._normal_Next * 3) - (mesh._offsetPos + imageHalfOffset));

						if (outerPoint._isInversedNormal_Prev || outerPoint._isInversedNormal_Next)
						{
							GL.Color(new Color(1.0f, 0.0f, 0.0f, 0.8f));
						}
						else
						{
							GL.Color(new Color(1.0f, 1.0f, 0.0f, 0.8f));
						}

						//Normal Prev
						//GL.Vertex(pos_0);
						//GL.Vertex(pos_1);

						//Normal Avg
						GL.Vertex(pos_0);
						GL.Vertex(pos_2);

						//Normal Next
						//GL.Vertex(pos_0);
						//GL.Vertex(pos_3);



					}
				}
				GL.End();
			}
			else
			{
				//Previewed 상태

				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);
				GL.Color(new Color(1.0f, 0.0f, 1.0f, 0.8f));
				for (int iPoint = 0; iPoint < meshGenerator._outerPoints_Preview.Count; iPoint++)
				{
					outerPoint = meshGenerator._outerPoints_Preview[iPoint];
					if (outerPoint._nextPoint != null)
					{
						pos_A = World2GL(outerPoint._pos - (mesh._offsetPos + imageHalfOffset));
						pos_B = World2GL(outerPoint._nextPoint._pos - (mesh._offsetPos + imageHalfOffset));

						GL.Vertex(pos_A);
						GL.Vertex(pos_B);
					}
				}
				GL.End();

				////Normal 표시
				//_matBatch.SetPass_Color();
				//_matBatch.SetClippingSize(_glScreenClippingSize);

				//GL.Begin(GL.LINES);
				//GL.Color(new Color(1.0f, 0.0f, 1.0f, 0.8f));
				//for (int iPoint = 0; iPoint < meshGenerator._outerPoints_Preview.Count; iPoint++)
				//{
				//	outerPoint = meshGenerator._outerPoints_Preview[iPoint];
				//	pos_0 = World2GL(outerPoint._pos - (mesh._offsetPos + imageHalfOffset));
				//	pos_2 = World2GL(outerPoint._pos + (outerPoint._normal_Avg * 7) - (mesh._offsetPos + imageHalfOffset));

				//	GL.Color(new Color(1.0f, 1.0f, 1.0f, 0.8f));

				//	GL.Vertex(pos_0);
				//	GL.Vertex(pos_2);
				//}
				//GL.End();

				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				float pointHalfSize = 2;
				GL.Begin(GL.TRIANGLES);
				GL.Color(new Color(1.0f, 1.0f, 0.0f, 0.8f));
				for (int iPoint = 0; iPoint < meshGenerator._outerPoints_Preview.Count; iPoint++)
				{
						outerPoint = meshGenerator._outerPoints_Preview[iPoint];
						pos_A = World2GL(outerPoint._pos - (mesh._offsetPos + imageHalfOffset));

						pos_0 = pos_A + new Vector2(-pointHalfSize, -pointHalfSize);
						pos_1 = pos_A + new Vector2(pointHalfSize, -pointHalfSize);
						pos_2 = pos_A + new Vector2(pointHalfSize, pointHalfSize);
						pos_3 = pos_A + new Vector2(-pointHalfSize, pointHalfSize);
						GL.Vertex(pos_0);	GL.Vertex(pos_1);	GL.Vertex(pos_2);
						GL.Vertex(pos_2);	GL.Vertex(pos_3);	GL.Vertex(pos_0);

						GL.Vertex(pos_2);	GL.Vertex(pos_1);	GL.Vertex(pos_0);
						GL.Vertex(pos_0);	GL.Vertex(pos_3);	GL.Vertex(pos_2);
				}
				GL.End();


				apMeshGenerator.InnerPoint curInnerPoint = null;
				
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);
				
				for (int iPoint = 0; iPoint < meshGenerator._innerPoints.Count; iPoint++)
				{
					curInnerPoint = meshGenerator._innerPoints[iPoint];

					//인접한 Inner Point과의 선분
					GL.Color(new Color(0.0f, 1.0f, 1.0f, 0.8f));
					if(curInnerPoint._linkedPoint_GUI != null)
					{
						for (int iNext = 0; iNext < curInnerPoint._linkedPoint_GUI.Count; iNext++)
						{
							pos_A = World2GL(curInnerPoint._pos - (mesh._offsetPos + imageHalfOffset));
							pos_B = World2GL(curInnerPoint._linkedPoint_GUI[iNext]._pos - (mesh._offsetPos + imageHalfOffset));

							GL.Vertex(pos_A);
							GL.Vertex(pos_B);
						}
					}

					//인접한 Inner -> Outer Point과의 선분
					GL.Color(new Color(0.0f, 1.0f, 0.5f, 0.8f));
					if(curInnerPoint._linkedOuterPoints != null)
					{
						for (int iOut = 0; iOut < curInnerPoint._linkedOuterPoints.Count; iOut++)
						{
							pos_A = World2GL(curInnerPoint._pos - (mesh._offsetPos + imageHalfOffset));
							pos_B = World2GL(curInnerPoint._linkedOuterPoints[iOut]._pos - (mesh._offsetPos + imageHalfOffset));
							DrawAnimatedLineGL(pos_A, pos_B, new Color(0.0f, 1.0f, 0.5f, 0.8f), false);
							//GL.Vertex(pos_A);
							//GL.Vertex(pos_B);
						}
					}
				}
				GL.End();
				
				bool isAnyLockedPoint = false;
				bool isAnyAxisLimitedPoint = false;
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.TRIANGLES);
				GL.Color(new Color(1.0f, 1.0f, 1.0f, 0.8f));
				for (int iPoint = 0; iPoint < meshGenerator._innerPoints.Count; iPoint++)
				{
					curInnerPoint = meshGenerator._innerPoints[iPoint];
					pos_A = World2GL(curInnerPoint._pos - (mesh._offsetPos + imageHalfOffset));

					pos_0 = pos_A + new Vector2(-pointHalfSize, -pointHalfSize);
					pos_1 = pos_A + new Vector2(pointHalfSize, -pointHalfSize);
					pos_2 = pos_A + new Vector2(pointHalfSize, pointHalfSize);
					pos_3 = pos_A + new Vector2(-pointHalfSize, pointHalfSize);
					GL.Vertex(pos_0);	GL.Vertex(pos_1);	GL.Vertex(pos_2);
					GL.Vertex(pos_2);	GL.Vertex(pos_3);	GL.Vertex(pos_0);

					GL.Vertex(pos_2);	GL.Vertex(pos_1);	GL.Vertex(pos_0);
					GL.Vertex(pos_0);	GL.Vertex(pos_3);	GL.Vertex(pos_2);

					if(curInnerPoint._moveLock == apMeshGenerator.INNER_MOVE_LOCK.Locked)
					{
						isAnyLockedPoint = true;
					}
					else if(curInnerPoint._moveLock == apMeshGenerator.INNER_MOVE_LOCK.AxisLimited)
					{
						isAnyAxisLimitedPoint = true;
					}
				}
				GL.End();

				//Inner Point가 Lock 상태인 경우

				if(isAnyLockedPoint || isAnyAxisLimitedPoint)
				{
					float iconSize = 16 / _zoom;
					Vector2 posOffset = new Vector2(8, 8);
					
					Texture2D img_innerAxisLimited = editor.ImageSet.Get(apImageSet.PRESET.MeshEdit_InnerPointAxisLimited);
					Texture2D img_innerLocked = editor.ImageSet.Get(apImageSet.PRESET.MeshEdit_InnerPointLocked);

					for (int iPoint = 0; iPoint < meshGenerator._innerPoints.Count; iPoint++)
					{
						curInnerPoint = meshGenerator._innerPoints[iPoint];
						pos_A = World2GL(curInnerPoint._pos - (mesh._offsetPos + imageHalfOffset));
						if(curInnerPoint._moveLock == apMeshGenerator.INNER_MOVE_LOCK.Locked)
						{
							DrawTextureGL(img_innerLocked, pos_A + posOffset, iconSize, iconSize, _textureColor_Gray, 0.0f);
						}
						else if(curInnerPoint._moveLock == apMeshGenerator.INNER_MOVE_LOCK.AxisLimited)
						{
							DrawTextureGL(img_innerAxisLimited, pos_A + posOffset, iconSize, iconSize, _textureColor_Gray, 0.0f);

							//DrawBoldLineGL(pos_A, World2GL(curInnerPoint._limitedPosA - (mesh._offsetPos + imageHalfOffset)), 4, Color.red, true);
							//DrawBoldLineGL(pos_A, World2GL(curInnerPoint._limitedPosB - (mesh._offsetPos + imageHalfOffset)), 4, Color.yellow, true);
						}
					}
				}
			}


			//컨트롤 포인트
			Texture2D img_controlPoint = editor.ImageSet.Get(apImageSet.PRESET.TransformAutoGenMapperCtrl);

			List<apMeshGenMapper.ControlPoint> controlPoints = meshGenerator.Mapper.ControlPoints;
			List<apMeshGenMapper.ControlPoint> selectedPoints = meshGenerator._selectedControlPoints;
			if (controlPoints != null)
			{
				//float controlPointHalfSize = 13;//(전체 26)
				float controlPointSize = (float)26 / _zoom;//(전체 26)
				apMeshGenMapper.ControlPoint curPoint = null;

				_matBatch.SetPass_Color();
				
				_matBatch.SetClippingSize(_glScreenClippingSize);
				
				//GL.Begin(GL.LINES);
				GL.Begin(GL.TRIANGLES);
				
				for (int iPoint = 0; iPoint < controlPoints.Count; iPoint++)
				{
					curPoint = controlPoints[iPoint];
					pos_A = World2GL(curPoint._pos_Cur - (mesh._offsetPos + imageHalfOffset));
					if (curPoint._linkedOuterPoints.Count > 0)
					{
						for (int i = 0; i < curPoint._linkedOuterPoints.Count; i++)
						{
							pos_B = World2GL(curPoint._linkedOuterPoints[i]._pos_Cur - (mesh._offsetPos + imageHalfOffset));

							DrawBoldLineGL(pos_A, pos_B, 3, new Color(1.0f, 0.5f, 0.0f, 0.8f), false);
							//GL.Color(new Color(1.0f, 0.5f, 0.0f, 1.0f));
							//GL.Vertex(pos_A);
							//GL.Vertex(pos_B);
						}
					}
					if(curPoint._linkedInnerOrCrossPoints.Count > 0)
					{
						for (int i = 0; i < curPoint._linkedInnerOrCrossPoints.Count; i++)
						{
							pos_B = World2GL(curPoint._linkedInnerOrCrossPoints[i]._pos_Cur - (mesh._offsetPos + imageHalfOffset));

							DrawBoldLineGL(pos_A, pos_B, 2, new Color(1.0f, 1.0f, 0.0f, 0.5f), false);
							//GL.Color(new Color(1.0f, 1.0f, 0.0f, 0.8f));
							//GL.Vertex(pos_A);
							//GL.Vertex(pos_B);
						}
					}
				}
				GL.End();



				//_matBatch.SetPass_Color();
				//_matBatch.SetPass_Texture_Normal(Color.gray, img_controlPoint, apPortrait.SHADER_TYPE.AlphaBlend);
				//_matBatch.SetClippingSize(_glScreenClippingSize);
				
				//GL.Begin(GL.TRIANGLES);
				
				//GL.Color(new Color(1.0f, 1.0f, 1.0f, 1.0f));
				for (int iPoint = 0; iPoint < controlPoints.Count; iPoint++)
				{
					curPoint = controlPoints[iPoint];
					pos_A = World2GL(curPoint._pos_Cur - (mesh._offsetPos + imageHalfOffset));
					
					if(selectedPoints.Contains(curPoint))
					{
						DrawTextureGL(img_controlPoint, pos_A, controlPointSize, controlPointSize, Color.red, 0.0f, true);
					}
					else
					{
						DrawTextureGL(img_controlPoint, pos_A, controlPointSize, controlPointSize, Color.gray, 0.0f, true);
					}
					
					//pos_0 = pos_A + new Vector2(-controlPointHalfSize, -controlPointHalfSize);
					//pos_1 = pos_A + new Vector2(controlPointHalfSize, -controlPointHalfSize);
					//pos_2 = pos_A + new Vector2(controlPointHalfSize, controlPointHalfSize);
					//pos_3 = pos_A + new Vector2(-controlPointHalfSize, controlPointHalfSize);

					//GL.Vertex(pos_0); GL.Vertex(pos_1); GL.Vertex(pos_2);
					//GL.Vertex(pos_2); GL.Vertex(pos_3); GL.Vertex(pos_0);

					//GL.Vertex(pos_2); GL.Vertex(pos_1); GL.Vertex(pos_0);
					//GL.Vertex(pos_0); GL.Vertex(pos_3); GL.Vertex(pos_2);
					AddCursorRect(editor.Mouse.PosLast, pos_A, controlPointSize, controlPointSize, MouseCursor.MoveArrow);
				}
				//GL.End();
			}

			
		}



		//------------------------------------------------------------------------------------------------
		// Mesh 모드에서 Mirror 라인 긋기
		//------------------------------------------------------------------------------------------------
		public static void DrawMeshMirror(apMesh mesh)
		{
			if (mesh == null || mesh.LinkedTextureData == null || mesh.LinkedTextureData._image == null)
			{
				return;
			}

			//Vector2 imageHalfOffset = new Vector2(mesh.LinkedTextureData._width * 0.5f, mesh.LinkedTextureData._height * 0.5f);
			
			Vector2 posW = Vector2.zero;
			Vector2 posA_GL = Vector2.zero;
			Vector2 posB_GL = Vector2.zero;
			if(mesh._isMirrorX)
			{
				//세로 줄을 긋는다.
				//posW.x = mesh._mirrorAxis.x - (mesh._offsetPos.x + imageHalfOffset.x);
				posW.x = mesh._mirrorAxis.x - (mesh._offsetPos.x);
				posA_GL = World2GL(posW);
				posB_GL = posA_GL;

				posA_GL.y = -500;
				posB_GL.y = _windowHeight + 500;
			}
			else
			{
				//가로 줄을 긋는다.
				//posW.y = mesh._mirrorAxis.y - (mesh._offsetPos.y + imageHalfOffset.y);
				posW.y = mesh._mirrorAxis.y - (mesh._offsetPos.y);
				posA_GL = World2GL(posW);
				posB_GL = posA_GL;

				posA_GL.x = -500;
				posB_GL.x = _windowWidth + 500;
			}
			
			DrawBoldLineGL(posA_GL, posB_GL, 3, new Color(0.0f, 1.0f, 0.5f, 0.4f), true);

			
		}

		//------------------------------------------------------------------------------------------------
		// Draw Mesh의 Edge Wire
		//------------------------------------------------------------------------------------------------
		public static void DrawMeshWorkSnapNextVertex(apMesh mesh, apVertexController vertexController)
		{
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				//if (mesh == null || mesh._textureData == null || mesh._textureData._image == null)
				if (mesh == null || mesh.LinkedTextureData == null || mesh.LinkedTextureData._image == null)
				{
					return;
				}

				if (vertexController.LinkedNextVertex != null && vertexController.LinkedNextVertex != vertexController.Vertex)
				{
					Vector2 linkedVertPosW = vertexController.LinkedNextVertex._pos - mesh._offsetPos;
					float size = 20.0f / _zoom;
					DrawBox(linkedVertPosW, size, size, Color.green, true);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("AnyPortrait : DrawMeshWorkSnapNextVertex() Exception : " + ex);
			}
		}
		public static void DrawMeshWorkEdgeSnap(apMesh mesh, apVertexController vertexController)
		{
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				//if (mesh == null || mesh._textureData == null || mesh._textureData._image == null)
				if (mesh == null || mesh.LinkedTextureData == null || mesh.LinkedTextureData._image == null)
				{
					return;
				}

				//if (vertexController.IsTmpSnapToEdge && vertexController.Vertex == null)
				if (vertexController.IsTmpSnapToEdge)
				{
					float size = 20.0f / _zoom;
					DrawBox(vertexController.TmpSnapToEdgePos - mesh._offsetPos, size, size, new Color(0.0f, 1.0f, 1.0f, 1.0f), true);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("AnyPortrait : DrawMeshWorkEdgeSnap() Exception : " + ex);
			}
		}
		public static void DrawMeshWorkEdgeWire(apMesh mesh, apMatrix3x3 matrix, apVertexController vertexController, bool isCross, bool isCrossMultiple)
		{
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				//if (mesh == null || mesh._textureData == null || mesh._textureData._image == null)
				if (mesh == null || mesh.LinkedTextureData == null || mesh.LinkedTextureData._image == null)
				{
					return;
				}


				
				if (vertexController.Vertex == null)
				{
					return;
				}

				matrix *= mesh.Matrix_VertToLocal;

				Vector2 mouseW = GL2World(vertexController.TmpEdgeWirePos);
				Vector2 vertPosW = matrix.MultiplyPoint(vertexController.Vertex._pos);

				Color lineColor = Color.green;
				if (isCross)
				{
					lineColor = Color.red;
				}
				else if (isCrossMultiple)
				{
					lineColor = new Color(0.2f, 0.8f, 1.0f, 1.0f);
				}

				//DrawLine(vertPosW, mouseW, lineColor, true);
				DrawAnimatedLine(vertPosW, mouseW, lineColor, true);

				//if (vertexController.LinkedNextVertex != null && vertexController.LinkedNextVertex != vertexController.Vertex)
				//{
				//	Vector2 linkedVertPosW = matrix.MultiplyPoint(vertexController.LinkedNextVertex._pos);
				//	float size = 20.0f / _zoom;
				//	DrawBox(linkedVertPosW, size, size, lineColor, true);
				//}
				if (isCross)
				{
					Vector2 crossPointW = matrix.MultiplyPoint(vertexController.EdgeWireCrossPoint());
					float size = 20.0f / _zoom;
					DrawBox(crossPointW, size, size, Color.cyan, true);
				}
				else if (isCrossMultiple)
				{
					List<Vector2> crossVerts = vertexController.EdgeWireMultipleCrossPoints();
					float size = 20.0f / _zoom;

					for (int i = 0; i < crossVerts.Count; i++)
					{
						Vector2 crossPointW = matrix.MultiplyPoint(crossVerts[i]);
						DrawBox(crossPointW, size, size, Color.yellow, true);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}


		public static void DrawMeshWorkMirrorEdgeWire(apMesh mesh, apMirrorVertexSet mirrorSet)
		{
			try
			{
				if (mesh == null)
				{
					return;
				}

				//DrawBox(-mesh._offsetPos, 50, 50, Color.yellow, false);

				Color lineColor = new Color(1, 1, 0, 0.5f);
				Color nearVertColor = new Color(1, 1, 0, 0.5f);

				//둘다 유효하다면 => 선을 긋는다.
				//하나만 유효하다면 => 점 또는 Vertex 외곽선을 만든다.
				bool isPrevEnabled = mirrorSet._meshWork_TypePrev != apMirrorVertexSet.MIRROR_MESH_WORK_TYPE.None;
				bool isNextEnabled = mirrorSet._meshWork_TypeNext != apMirrorVertexSet.MIRROR_MESH_WORK_TYPE.None;
				bool isPrevNearVert = false;
				bool isNextNearVert = false;
				Vector2 posPrev = Vector2.zero;
				Vector2 posNext = Vector2.zero;

				float pointSize = 10.0f / _zoom;//<<원래 크기
				//float pointSize = 8.0f / _zoom;
				float pointSize_Wire = 18.0f / _zoom;

				if(isPrevEnabled)
				{
					posPrev = mirrorSet._meshWork_PosPrev - mesh._offsetPos;
					if(mirrorSet._meshWork_VertPrev != null)
					{
						//버텍스 위치로 이동
						isPrevNearVert = true;
						posPrev = mirrorSet._meshWork_VertPrev._pos - mesh._offsetPos;
					}
				}
				if(isNextEnabled)
				{
					posNext = mirrorSet._meshWork_PosNext - mesh._offsetPos;
					if(mirrorSet._meshWork_VertNext != null)
					{
						//버텍스 위치로 이동
						isNextNearVert = true;
						posNext = mirrorSet._meshWork_VertNext._pos - mesh._offsetPos;
					}
				}
				if(isPrevEnabled && isNextEnabled)
				{
					//선을 긋자
					DrawAnimatedLine(posPrev, posNext, lineColor, true);
				}

				//점을 찍자
				if(isPrevEnabled)
				{
					if(isPrevNearVert)
					{
						DrawBox(posPrev, pointSize_Wire, pointSize_Wire, nearVertColor, true);
					}
					else
					{
						DrawBox(posPrev, pointSize, pointSize, nearVertColor, true);
					}
				}
				if(isNextEnabled)
				{
					if(isNextNearVert)
					{
						DrawBox(posNext, pointSize_Wire, pointSize_Wire, nearVertColor, true);
					}
					else
					{
						DrawBox(posNext, pointSize, pointSize, nearVertColor, true);
					}
				}

				//만약 Snap이 되는 상황이면, Snap되는 위치를 찍자
				if(mirrorSet._meshWork_SnapToAxis)
				{
					DrawBox(mirrorSet._meshWork_PosNextSnapped - mesh._offsetPos, pointSize_Wire, pointSize_Wire, new Color(0, 1, 1, 0.8f), true);
				}
			}

			catch(Exception ex)
			{
				Debug.LogError("AnyPortrait : DrawMeshWorkMirrorEdgeWire Exception : " + ex);
			}
		}

		//------------------------------------------------------------------------------------------------
		// Draw Mirror Mesh PreviewLines
		//------------------------------------------------------------------------------------------------
		public static void DrawMirrorMeshPreview(apMesh mesh, apMirrorVertexSet mirrorSet, apEditor editor, apVertexController vertexController)
		{
			try
			{
				if (mesh == null || mesh.LinkedTextureData == null || mesh.LinkedTextureData._image == null)//변경 코드
				{
					return;
				}

				if(mirrorSet._cloneVerts.Count == 0)
				{
					return;
				}

				Vector2 offsetPos = mesh._offsetPos;
				apMirrorVertexSet.CloneEdge cloneEdge = null;
				apMirrorVertexSet.CloneVertex cloneVert = null;
				Vector2 pos1 = Vector2.zero;
				Vector2 pos2 = Vector2.zero;

				//Edge 렌더
				if (mirrorSet._cloneEdges.Count > 0)
				{
					Color edgeColor = editor._colorOption_MeshHiddenEdge;
					edgeColor.a *= 0.5f;

					_matBatch.SetPass_Color();
					_matBatch.SetClippingSize(_glScreenClippingSize);
					GL.Begin(GL.LINES);

					for (int iEdge = 0; iEdge < mirrorSet._cloneEdges.Count; iEdge++)
					{
						cloneEdge = mirrorSet._cloneEdges[iEdge];
						pos1 = cloneEdge._cloneVert1._pos - offsetPos;
						pos2 = cloneEdge._cloneVert2._pos - offsetPos;
						DrawDotLine(pos1, pos2, edgeColor, false);
					}

					GL.End();
				}

				//Vertex 렌더 (Clone / Cross)
				Color vertColor_Mirror = new Color(0.0f, 1.0f, 0.5f, 0.5f);
				Color vertColor_Cross = new Color(1.0f, 1.0f, 0.0f, 0.5f);
				float pointSize = 10.0f / _zoom;
				float pointSize_Wire = 18.0f / _zoom;

				//1) Mirror
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.TRIANGLES);
				for (int iVert = 0; iVert < mirrorSet._cloneVerts.Count; iVert++)
				{
					cloneVert = mirrorSet._cloneVerts[iVert];
					pos1 = cloneVert._pos - offsetPos;
					if (!cloneVert._isOnAxis)
					{
						DrawBox(pos1, pointSize, pointSize, vertColor_Mirror, false, false);
					}
				}
				GL.End();

				//2) On Axis
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.LINES);
				for (int iVert = 0; iVert < mirrorSet._cloneVerts.Count; iVert++)
				{
					cloneVert = mirrorSet._cloneVerts[iVert];
					pos1 = cloneVert._pos - offsetPos;
					if (cloneVert._isOnAxis)
					{
						DrawBox(pos1, pointSize_Wire, pointSize_Wire, vertColor_Cross, true, false);
					}
				}
				GL.End();

				//3) Cross
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				GL.Begin(GL.TRIANGLES);
				for (int iVert = 0; iVert < mirrorSet._crossVerts.Count; iVert++)
				{
					cloneVert = mirrorSet._crossVerts[iVert];
					pos1 = cloneVert._pos - offsetPos;
					DrawBox(pos1, pointSize, pointSize, vertColor_Cross, false, false);
				}
				GL.End();
			}
			catch(Exception ex)
			{
				Debug.LogError("AnyPortrait : DrawMirrorMeshPreview Exception : " + ex);
			}
		}

		//------------------------------------------------------------------------------------------------
		// Draw Render Unit (Mesh / Outline)
		//------------------------------------------------------------------------------------------------
		private static List<apRenderVertex> _tmpSelectedVertices = new List<apRenderVertex>();
		private static List<apRenderVertex> _tmpSelectedVertices_Weighted = new List<apRenderVertex>();
		//private static List<float> _tmpSelectedVertices_WeightedValue = new List<float>();

		public static void DrawRenderUnit(apRenderUnit renderUnit,
											RENDER_TYPE renderType,
											apVertexController vertexController,
											apSelection select,
											apEditor editor,
											Vector2 mousePos)
		{
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				if (renderUnit == null || renderUnit._meshTransform == null || renderUnit._meshTransform._mesh == null)
				{
					return;
				}

				if (renderUnit._renderVerts.Count == 0)
				{
					return;
				}
				Color textureColor = renderUnit._meshColor2X;

				
				apMesh mesh = renderUnit._meshTransform._mesh;
				bool isVisible = renderUnit._isVisible;

				apTextureData linkedTextureData = mesh.LinkedTextureData;

				//추가 12.4 : Extra Option에 의해 Texture가 바귀었을 경우
				if(renderUnit.IsExtraTextureChanged)
				{
					linkedTextureData = renderUnit.ChangedExtraTextureData;
				}

				//if (mesh.LinkedTextureData == null)//이전
				if(linkedTextureData == null)
				{
					return;
				}
				

				//미리 GL 좌표를 연산하고, 나중에 중복 연산(World -> GL)을 하지 않도록 하자
				apRenderVertex rVert = null;
				for (int i = 0; i < renderUnit._renderVerts.Count; i++)
				{
					rVert = renderUnit._renderVerts[i];
					rVert._pos_GL = World2GL(rVert._pos_World);
				}


				bool isAnyVertexSelected = false;
				//apBone selectedBone = null;
				bool isWeightedSelected = false;

				bool isBoneWeightColor = (renderType & RENDER_TYPE.BoneRigWeightColor) != 0;
				bool isPhyVolumeWeightColor = (renderType & RENDER_TYPE.PhysicsWeightColor) != 0 || (renderType & RENDER_TYPE.VolumeWeightColor) != 0;
				bool isBoneColor = false;
				bool isCircleRiggingVert = editor._rigViewOption_CircleVert;
				float vertexColorRatio = 0.0f;

				if (select != null)
				{
					_tmpSelectedVertices.Clear();
					_tmpSelectedVertices_Weighted.Clear();
					//_tmpSelectedVertices_WeightedValue.Clear();

					//Soft Selection + TODO 나중에 Volume 등에서 Weighted 설정을 하자
					if (select.Editor.Gizmos.IsSoftSelectionMode)
					{
						isWeightedSelected = true;
					}

					//isBoneColor = select._rigEdit_isBoneColorView;//이전
					isBoneColor = editor._rigViewOption_BoneColor;//변경 19.7.31

					if (isBoneWeightColor)
					{
						//if (select._rigEdit_viewMode == apSelection.RIGGING_EDIT_VIEW_MODE.WeightColorOnly)
						if(editor._rigViewOption_WeightOnly)
						{
							vertexColorRatio = 1.0f;
						}
						else
						{
							vertexColorRatio = 0.5f;
						}
					}
					else if (isPhyVolumeWeightColor)
					{
						vertexColorRatio = 0.7f;
					}

					isAnyVertexSelected = true;
					if (select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup)
					{
						if (select.ModRenderVertListOfMod != null && select.ModRenderVertListOfMod.Count > 0)
						{
							for (int i = 0; i < select.ModRenderVertListOfMod.Count; i++)
							{
								_tmpSelectedVertices.Add(select.ModRenderVertListOfMod[i]._renderVert);
							}

							if (isWeightedSelected)
							{
								for (int i = 0; i < select.ModRenderVertListOfMod_Weighted.Count; i++)
								{
									_tmpSelectedVertices_Weighted.Add(select.ModRenderVertListOfMod_Weighted[i]._renderVert);
									//_tmpSelectedVertices_WeightedValue.Add(select.ModRenderVertListOfMod_Weighted[i]._vertWeightByTool);
									select.ModRenderVertListOfMod_Weighted[i]._renderVert._renderWeightByTool = select.ModRenderVertListOfMod_Weighted[i]._vertWeightByTool;
								}
							}
						}


					}
					else if (select.SelectionType == apSelection.SELECTION_TYPE.Animation)
					{
						if (select.ModRenderVertListOfAnim != null && select.ModRenderVertListOfAnim.Count > 0)
						{
							for (int i = 0; i < select.ModRenderVertListOfAnim.Count; i++)
							{
								_tmpSelectedVertices.Add(select.ModRenderVertListOfAnim[i]._renderVert);
							}

							if (isWeightedSelected)
							{
								for (int i = 0; i < select.ModRenderVertListOfAnim_Weighted.Count; i++)
								{
									_tmpSelectedVertices_Weighted.Add(select.ModRenderVertListOfAnim_Weighted[i]._renderVert);
									//_tmpSelectedVertices_WeightedValue.Add(select.ModRenderVertListOfAnim_Weighted[i]._vertWeightByTool);
									select.ModRenderVertListOfAnim_Weighted[i]._renderVert._renderWeightByTool = select.ModRenderVertListOfAnim_Weighted[i]._vertWeightByTool;
								}
							}
						}
					}
				}


				//렌더링 방식은 Mesh (with Color) 또는 Vertex / Outline이 있다.
				bool isMeshRender = false;
				bool isVertexRender = ((renderType & RENDER_TYPE.Vertex) != 0);
				bool isOutlineRender = ((renderType & RENDER_TYPE.Outlines) != 0);
				bool isAllEdgeRender = ((renderType & RENDER_TYPE.AllEdges) != 0);
				bool isToneColor = ((renderType & RENDER_TYPE.ToneColor) != 0);
				if (!isVertexRender && !isOutlineRender)
				{
					isMeshRender = true;
				}


				bool isDrawTFBorderLine = ((int)(renderType & RENDER_TYPE.TransformBorderLine) != 0);

				//2. 메시를 렌더링하자
				if (mesh._indexBuffer.Count >= 3 && isMeshRender && isVisible)
				{
					//------------------------------------------
					// Drawcall Batch를 했을때
					// Debug.Log("Texture Color : " + textureColor);
					Color color0 = Color.black, color1 = Color.black, color2 = Color.black;

					int iVertColor = 0;

					if ((renderType & RENDER_TYPE.VolumeWeightColor) != 0)
					{
						iVertColor = 1;
					}
					else if ((renderType & RENDER_TYPE.PhysicsWeightColor) != 0)
					{
						iVertColor = 2;
					}
					else if ((renderType & RENDER_TYPE.BoneRigWeightColor) != 0)
					{
						iVertColor = 3;
					}
					else
					{
						iVertColor = 0;
						color0 = Color.black;
						color1 = Color.black;
						color2 = Color.black;
					}

					if(isToneColor)
					{
						//_matBatch.SetPass_ToneColor_Normal(_toneColor, mesh.LinkedTextureData._image);//이전
						_matBatch.SetPass_ToneColor_Normal(_toneColor, linkedTextureData._image);//변경
						
					}
					else if (!isBoneWeightColor && !isPhyVolumeWeightColor)
					{
						//_matBatch.SetPass_Texture_VColor(textureColor, mesh.LinkedTextureData._image, 0.0f, renderUnit.ShaderType);//이전
						_matBatch.SetPass_Texture_VColor(textureColor, linkedTextureData._image, 0.0f, renderUnit.ShaderType);//변경
					}
					else
					{
						//_matBatch.SetPass_Texture_VColor(textureColor, mesh.LinkedTextureData._image, vertexColorRatio, renderUnit.ShaderType);//이전
						_matBatch.SetPass_Texture_VColor(textureColor, linkedTextureData._image, vertexColorRatio, renderUnit.ShaderType);//변경
					}
					_matBatch.SetClippingSize(_glScreenClippingSize);


					GL.Begin(GL.TRIANGLES);
					//------------------------------------------
					//apVertex vert0, vert1, vert2;
					apRenderVertex rVert0 = null, rVert1 = null, rVert2 = null;


					Vector3 pos_0 = Vector3.zero;
					Vector3 pos_1 = Vector3.zero;
					Vector3 pos_2 = Vector3.zero;


					Vector2 uv_0 = Vector2.zero;
					Vector2 uv_1 = Vector2.zero;
					Vector2 uv_2 = Vector2.zero;

					for (int i = 0; i < mesh._indexBuffer.Count; i += 3)
					{
						if (i + 2 >= mesh._indexBuffer.Count)
						{ break; }

						if (mesh._indexBuffer[i + 0] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 1] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 2] >= mesh._vertexData.Count)
						{
							break;
						}

						rVert0 = renderUnit._renderVerts[mesh._indexBuffer[i + 0]];
						rVert1 = renderUnit._renderVerts[mesh._indexBuffer[i + 1]];
						rVert2 = renderUnit._renderVerts[mesh._indexBuffer[i + 2]];

						pos_0.x = rVert0._pos_GL.x;
						pos_0.y = rVert0._pos_GL.y;
						pos_0.z = rVert0._vertex._zDepth * 0.5f;

						pos_1.x = rVert1._pos_GL.x;
						pos_1.y = rVert1._pos_GL.y;
						pos_1.z = rVert1._vertex._zDepth * 0.5f;

						pos_2.x = rVert2._pos_GL.x;
						pos_2.y = rVert2._pos_GL.y;
						pos_2.z = rVert2._vertex._zDepth * 0.5f;


						uv_0 = mesh._vertexData[mesh._indexBuffer[i + 0]]._uv;
						uv_1 = mesh._vertexData[mesh._indexBuffer[i + 1]]._uv;
						uv_2 = mesh._vertexData[mesh._indexBuffer[i + 2]]._uv;

						switch (iVertColor)
						{
							case 1: //VolumeWeightColor
								color0 = GetWeightGrayscale(rVert0._renderWeightByTool);
								color1 = GetWeightGrayscale(rVert1._renderWeightByTool);
								color2 = GetWeightGrayscale(rVert2._renderWeightByTool);
								break;

							case 2: //PhysicsWeightColor
								color0 = GetWeightColor4(rVert0._renderWeightByTool);
								color1 = GetWeightColor4(rVert1._renderWeightByTool);
								color2 = GetWeightColor4(rVert2._renderWeightByTool);
								break;

							case 3: //BoneRigWeightColor
									//TODO : 본 리스트를 받아서 해야하는디..
								if (isBoneColor)
								{
									color0 = rVert0._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
									color1 = rVert1._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
									color2 = rVert2._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
								}
								else
								{
									color0 = GetWeightColor3(rVert0._renderWeightByTool);
									color1 = GetWeightColor3(rVert1._renderWeightByTool);
									color2 = GetWeightColor3(rVert2._renderWeightByTool);
								}
								color0.a = 1.0f;
								color1.a = 1.0f;
								color2.a = 1.0f;
								
								break;
						}
						////------------------------------------------

						GL.Color(color0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
						GL.Color(color1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(color2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

						// Back Side
						GL.Color(color2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
						GL.Color(color1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(color0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0

						////------------------------------------------
					}
					GL.End();

				}

				//3. Edge를 렌더링하자
				if (isAllEdgeRender)
				{
					Vector2 pos0 = Vector2.zero, pos1 = Vector2.zero;

					apRenderVertex rVert0 = null, rVert1 = null;
					if (mesh._edges.Count > 0)
					{
						_matBatch.SetPass_Color();
						_matBatch.SetClippingSize(_glScreenClippingSize);
						GL.Begin(GL.LINES);
						for (int i = 0; i < mesh._edges.Count; i++)
						{
							rVert0 = renderUnit._renderVerts[mesh._edges[i]._vert1._index];
							rVert1 = renderUnit._renderVerts[mesh._edges[i]._vert2._index];

							pos0 = rVert0._pos_GL;
							pos1 = rVert1._pos_GL;

							DrawLineGL(pos0, pos1, editor._colorOption_MeshEdge, false);
						}

						GL.End();
					}
				}
				else if (isOutlineRender)
				{
					Vector2 pos0 = Vector2.zero, pos1 = Vector2.zero;
					apRenderVertex rVert0 = null, rVert1 = null;
					if (mesh._edges.Count > 0)
					{
						_matBatch.SetPass_Color();
						_matBatch.SetClippingSize(_glScreenClippingSize);

						GL.Begin(GL.TRIANGLES);

						for (int i = 0; i < mesh._edges.Count; i++)
						{
							if (!mesh._edges[i]._isOutline)
							{ continue; }

							rVert0 = renderUnit._renderVerts[mesh._edges[i]._vert1._index];
							rVert1 = renderUnit._renderVerts[mesh._edges[i]._vert2._index];

							
							pos0 = rVert0._pos_GL;
							pos1 = rVert1._pos_GL;

							DrawBoldLineGL(pos0, pos1, 6.0f, editor._colorOption_Outline, false);
						}

						GL.End();
					}
				}

				if (isDrawTFBorderLine)
				{
					float minPosLocal_X = float.MaxValue;
					float maxPosLocal_X = float.MinValue;
					float minPosLocal_Y = float.MaxValue;
					float maxPosLocal_Y = float.MinValue;

					Vector2 pos0 = Vector2.zero, pos1 = Vector2.zero;
					apRenderVertex rVert0 = null, rVert1 = null;

					if (mesh._edges.Count > 0)
					{
						for (int i = 0; i < mesh._edges.Count; i++)
						{
							if (!mesh._edges[i]._isOutline)
							{ continue; }

							rVert0 = renderUnit._renderVerts[mesh._edges[i]._vert1._index];
							rVert1 = renderUnit._renderVerts[mesh._edges[i]._vert2._index];

							pos0 = rVert0._pos_World;
							pos1 = rVert1._pos_World;

							if (rVert0._pos_LocalOnMesh.x < minPosLocal_X) { minPosLocal_X = rVert0._pos_LocalOnMesh.x; }
							if (rVert0._pos_LocalOnMesh.x > maxPosLocal_X) { maxPosLocal_X = rVert0._pos_LocalOnMesh.x; }
							if (rVert0._pos_LocalOnMesh.y < minPosLocal_Y) { minPosLocal_Y = rVert0._pos_LocalOnMesh.y; }
							if (rVert0._pos_LocalOnMesh.y > maxPosLocal_Y) { maxPosLocal_Y = rVert0._pos_LocalOnMesh.y; }

							if (rVert1._pos_LocalOnMesh.x < minPosLocal_X) { minPosLocal_X = rVert1._pos_LocalOnMesh.x; }
							if (rVert1._pos_LocalOnMesh.x > maxPosLocal_X) { maxPosLocal_X = rVert1._pos_LocalOnMesh.x; }
							if (rVert1._pos_LocalOnMesh.y < minPosLocal_Y) { minPosLocal_Y = rVert1._pos_LocalOnMesh.y; }
							if (rVert1._pos_LocalOnMesh.y > maxPosLocal_Y) { maxPosLocal_Y = rVert1._pos_LocalOnMesh.y; }
						}


						DrawTransformBorderFormOfRenderUnit(editor._colorOption_TransformBorder, minPosLocal_X, maxPosLocal_X, maxPosLocal_Y, minPosLocal_Y, renderUnit.WorldMatrix);
					}
				}


				//3. 버텍스를 렌더링하자
				if (isVertexRender)
				{
					float pointSize = 10.0f / _zoom;
					float pointSizeOutline = 14.0f / _zoom;
					Vector2 pos = Vector2.zero;
					bool isVertSelected = false;

					

					if (isAnyVertexSelected)
					{
						bool isDrawRigCircle = (isBoneWeightColor && isCircleRiggingVert);
						if(isDrawRigCircle)
						{
							//원형의 Rigging 버텍스
							_matBatch.SetPass_Texture_VColor(_textureColor_Gray, _img_RigCircle, 1.0f, apPortrait.SHADER_TYPE.AlphaBlend);
							//_matBatch.SetPass_Color();
						}
						else
						{
							//기본 사각형 버텍스
							_matBatch.SetPass_Color();
						}
						
						_matBatch.SetClippingSize(_glScreenClippingSize);

						GL.Begin(GL.TRIANGLES);


						Color vColor = Color.black;
						Color vColorOutline = _vertColor_Outline;
						for (int i = 0; i < renderUnit._renderVerts.Count; i++)
						{
							vColor = editor._colorOption_VertColor_NotSelected;
							vColorOutline = _vertColor_Outline;
							rVert = renderUnit._renderVerts[i];
							isVertSelected = false;

							if (isBoneWeightColor)
							{
								if (isBoneColor)
								{
									vColor = rVert._renderColorByTool;
								}
								else
								{
									vColor = GetWeightColor3_Vert(rVert._renderWeightByTool);
								}
							}
							else if (isPhyVolumeWeightColor)
							{
								vColor = GetWeightColor4_Vert(rVert._renderWeightByTool);
							}

							if (_tmpSelectedVertices != null)
							{
								if (_tmpSelectedVertices.Contains(rVert))
								{
									//선택된 경우
									isVertSelected = true;

									if (isBoneWeightColor || isPhyVolumeWeightColor)
									{
										//vColorOutline = vColor;
										vColorOutline = _vertColor_Outline_White;
									}
									else
									{
										vColor = editor._colorOption_VertColor_Selected;
									}
									
								}
								else if (isWeightedSelected && _tmpSelectedVertices_Weighted != null)
								{
									if (_tmpSelectedVertices_Weighted.Contains(rVert))
									{
										vColor = GetWeightColor2(rVert._renderWeightByTool, editor);
									}
								}
							}
							//pos = rVert._pos_World;
							//DrawBox(pos, pointSize, pointSize, vColor, isWireFramePoint, false);

							pos = rVert._pos_GL;
							if (isBoneWeightColor && isCircleRiggingVert)
							{
								//테스트
								DrawRiggingRenderVert(rVert, vColorOutline, isBoneColor);
							}
							else
							{
								//이전의 Box
								if (isVertSelected || isBoneWeightColor || isPhyVolumeWeightColor)
								{
									DrawBoxGL(pos, pointSizeOutline, pointSizeOutline, vColorOutline, false, false);
								}
								DrawBoxGL(pos, pointSize, pointSize, vColor, false, false);
							}
							

							AddCursorRect(mousePos, pos, 10, 10, MouseCursor.MoveArrow);
						}

						GL.End();
					}
					

					if (isPhyVolumeWeightColor)
					{
						float pointSize_PhysicImg = 40.0f / _zoom;
						//추가적인 Vertex 이미지를 추가한다.
						//RenderVertex의 Param으로 이미지를 추가한다.

						//1. Physic Main
						_matBatch.SetPass_Texture_Normal(_textureColor_Gray, _img_VertPhysicMain, apPortrait.SHADER_TYPE.AlphaBlend);
						_matBatch.SetClippingSize(_glScreenClippingSize);
						GL.Begin(GL.TRIANGLES);

						for (int i = 0; i < renderUnit._renderVerts.Count; i++)
						{
							rVert = renderUnit._renderVerts[i];
							if (rVert._renderParam == 1)
							{
								DrawTextureGL(_img_VertPhysicMain, rVert._pos_GL, pointSize_PhysicImg, pointSize_PhysicImg, _textureColor_Gray, 0.0f, false);
							}
						}

						GL.End();


						//2. Physic Constraint
						_matBatch.SetPass_Texture_Normal(_textureColor_Gray, _img_VertPhysicConstraint, apPortrait.SHADER_TYPE.AlphaBlend);
						_matBatch.SetClippingSize(_glScreenClippingSize);
						GL.Begin(GL.TRIANGLES);

						for (int i = 0; i < renderUnit._renderVerts.Count; i++)
						{
							rVert = renderUnit._renderVerts[i];
							if (rVert._renderParam == 2)
							{
								DrawTextureGL(_img_VertPhysicConstraint, rVert._pos_GL, pointSize_PhysicImg, pointSize_PhysicImg, _textureColor_Gray, 0.0f, false);
							}
						}

						GL.End();
					}
				}

				//DrawText("<-[" + renderUnit.Name + "_" + renderUnit._debugID + "]", renderUnit.WorldMatrixWrap._pos + new Vector2(10.0f, 0.0f), 100, Color.yellow);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}



		public static void DrawRenderUnit_Basic(apRenderUnit renderUnit)
		{
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				if (renderUnit == null || renderUnit._meshTransform == null || renderUnit._meshTransform._mesh == null)
				{
					return;
				}

				if (renderUnit._renderVerts.Count == 0)
				{
					return;
				}
				Color textureColor = renderUnit._meshColor2X;
				apMesh mesh = renderUnit._meshTransform._mesh;
				bool isVisible = renderUnit._isVisible;


				apTextureData linkedTextureData = mesh.LinkedTextureData;

				//추가 12.4 : Extra Option에 의해 Texture가 바귀었을 경우
				if(renderUnit.IsExtraTextureChanged)
				{
					linkedTextureData = renderUnit.ChangedExtraTextureData;
				}


				//if (mesh.LinkedTextureData == null)//이전
				if(linkedTextureData == null)
				{
					return;
				}

				//미리 GL 좌표를 연산하고, 나중에 중복 연산(World -> GL)을 하지 않도록 하자
				apRenderVertex rVert = null;
				for (int i = 0; i < renderUnit._renderVerts.Count; i++)
				{
					rVert = renderUnit._renderVerts[i];
					rVert._pos_GL = World2GL(rVert._pos_World);
				}



				//2. 메시를 렌더링하자
				if (mesh._indexBuffer.Count >= 3 && isVisible)
				{
					//------------------------------------------
					// Drawcall Batch를 했을때
					// Debug.Log("Texture Color : " + textureColor);
					Color color0 = Color.black, color1 = Color.black, color2 = Color.black;

					//int iVertColor = 0;
					color0 = Color.black;
					color1 = Color.black;
					color2 = Color.black;


					//_matBatch.SetPass_Texture_VColor(textureColor, mesh.LinkedTextureData._image, 0.0f, renderUnit.ShaderType);//이전
					_matBatch.SetPass_Texture_VColor(textureColor, linkedTextureData._image, 0.0f, renderUnit.ShaderType);//변경
					_matBatch.SetClippingSize(new Vector4(0, 0, 1, 1));


					GL.Begin(GL.TRIANGLES);
					//------------------------------------------
					//apVertex vert0, vert1, vert2;
					apRenderVertex rVert0 = null, rVert1 = null, rVert2 = null;

					Vector3 pos_0 = Vector3.zero;
					Vector3 pos_1 = Vector3.zero;
					Vector3 pos_2 = Vector3.zero;


					Vector2 uv_0 = Vector2.zero;
					Vector2 uv_1 = Vector2.zero;
					Vector2 uv_2 = Vector2.zero;


					for (int i = 0; i < mesh._indexBuffer.Count; i += 3)
					{
						if (i + 2 >= mesh._indexBuffer.Count)
						{ break; }

						if (mesh._indexBuffer[i + 0] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 1] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 2] >= mesh._vertexData.Count)
						{
							break;
						}

						rVert0 = renderUnit._renderVerts[mesh._indexBuffer[i + 0]];
						rVert1 = renderUnit._renderVerts[mesh._indexBuffer[i + 1]];
						rVert2 = renderUnit._renderVerts[mesh._indexBuffer[i + 2]];

						//Vector3 pos_0 = World2GL(rVert0._pos_World3);
						//Vector3 pos_1 = World2GL(rVert1._pos_World3);
						//Vector3 pos_2 = World2GL(rVert2._pos_World3);

						pos_0.x = rVert0._pos_GL.x;
						pos_0.y = rVert0._pos_GL.y;
						pos_0.z = rVert0._vertex._zDepth * 0.5f;

						pos_1.x = rVert1._pos_GL.x;
						pos_1.y = rVert1._pos_GL.y;
						pos_1.z = rVert1._vertex._zDepth * 0.5f;

						pos_2.x = rVert2._pos_GL.x;
						pos_2.y = rVert2._pos_GL.y;
						pos_2.z = rVert2._vertex._zDepth * 0.5f;


						uv_0 = mesh._vertexData[mesh._indexBuffer[i + 0]]._uv;
						uv_1 = mesh._vertexData[mesh._indexBuffer[i + 1]]._uv;
						uv_2 = mesh._vertexData[mesh._indexBuffer[i + 2]]._uv;


						////------------------------------------------

						GL.Color(color0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
						GL.Color(color1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(color2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

						// Back Side
						GL.Color(color2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
						GL.Color(color1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(color0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0

						////------------------------------------------
					}
					GL.End();

				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}


		public static void DrawRenderUnit_Basic_Alpha2White(apRenderUnit renderUnit)
		{
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				if (renderUnit == null || renderUnit._meshTransform == null || renderUnit._meshTransform._mesh == null)
				{
					return;
				}

				if (renderUnit._renderVerts.Count == 0)
				{
					return;
				}
				Color textureColor = renderUnit._meshColor2X;
				apMesh mesh = renderUnit._meshTransform._mesh;
				bool isVisible = renderUnit._isVisible;

				apTextureData linkedTextureData = mesh.LinkedTextureData;

				//추가 12.4 : Extra Option에 의해 Texture가 바귀었을 경우
				if(renderUnit.IsExtraTextureChanged)
				{
					linkedTextureData = renderUnit.ChangedExtraTextureData;
				}

				//if (mesh.LinkedTextureData == null)//이전
				if(linkedTextureData == null)
				{
					return;
				}

				//미리 GL 좌표를 연산하고, 나중에 중복 연산(World -> GL)을 하지 않도록 하자
				apRenderVertex rVert = null;
				for (int i = 0; i < renderUnit._renderVerts.Count; i++)
				{
					rVert = renderUnit._renderVerts[i];
					rVert._pos_GL = World2GL(rVert._pos_World);
				}



				//2. 메시를 렌더링하자
				if (mesh._indexBuffer.Count >= 3 && isVisible)
				{
					//------------------------------------------
					// Drawcall Batch를 했을때
					// Debug.Log("Texture Color : " + textureColor);
					Color color0 = Color.black, color1 = Color.black, color2 = Color.black;

					//int iVertColor = 0;
					color0 = Color.black;
					color1 = Color.black;
					color2 = Color.black;


					//_matBatch.SetPass_Alpha2White(textureColor, mesh.LinkedTextureData._image);//<<Shader를 Alpha2White로 한다.
					_matBatch.SetPass_Alpha2White(textureColor, linkedTextureData._image);//<<Shader를 Alpha2White로 한다. + ExtraOption
					_matBatch.SetClippingSize(new Vector4(0, 0, 1, 1));


					GL.Begin(GL.TRIANGLES);
					//------------------------------------------
					//apVertex vert0, vert1, vert2;
					apRenderVertex rVert0 = null, rVert1 = null, rVert2 = null;

					Vector3 pos_0 = Vector3.zero;
					Vector3 pos_1 = Vector3.zero;
					Vector3 pos_2 = Vector3.zero;


					Vector2 uv_0 = Vector2.zero;
					Vector2 uv_1 = Vector2.zero;
					Vector2 uv_2 = Vector2.zero;


					for (int i = 0; i < mesh._indexBuffer.Count; i += 3)
					{
						if (i + 2 >= mesh._indexBuffer.Count)
						{ break; }

						if (mesh._indexBuffer[i + 0] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 1] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 2] >= mesh._vertexData.Count)
						{
							break;
						}

						rVert0 = renderUnit._renderVerts[mesh._indexBuffer[i + 0]];
						rVert1 = renderUnit._renderVerts[mesh._indexBuffer[i + 1]];
						rVert2 = renderUnit._renderVerts[mesh._indexBuffer[i + 2]];

						//Vector3 pos_0 = World2GL(rVert0._pos_World3);
						//Vector3 pos_1 = World2GL(rVert1._pos_World3);
						//Vector3 pos_2 = World2GL(rVert2._pos_World3);

						pos_0.x = rVert0._pos_GL.x;
						pos_0.y = rVert0._pos_GL.y;
						pos_0.z = rVert0._vertex._zDepth * 0.5f;

						pos_1.x = rVert1._pos_GL.x;
						pos_1.y = rVert1._pos_GL.y;
						pos_1.z = rVert1._vertex._zDepth * 0.5f;

						pos_2.x = rVert2._pos_GL.x;
						pos_2.y = rVert2._pos_GL.y;
						pos_2.z = rVert2._vertex._zDepth * 0.5f;


						uv_0 = mesh._vertexData[mesh._indexBuffer[i + 0]]._uv;
						uv_1 = mesh._vertexData[mesh._indexBuffer[i + 1]]._uv;
						uv_2 = mesh._vertexData[mesh._indexBuffer[i + 2]]._uv;


						////------------------------------------------

						GL.Color(color0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
						GL.Color(color1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(color2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

						// Back Side
						GL.Color(color2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
						GL.Color(color1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(color0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0

						////------------------------------------------
					}
					GL.End();

				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}




		//---------------------------------------------------------------------------------------
		// Draw Render Unit : Clipping
		// RenderType은 MeshColor에 영향을 주는 것들만 허용한다.
		//---------------------------------------------------------------------------------------
		#region [미사용 코드] : Parent가 Clip Mesh를 그려주는 방식은 사용하지 않는다.
		//public static void DrawRenderUnit_ClippingParent(	apRenderUnit renderUnit, 
		//													RENDER_TYPE renderType,
		//													apTransform_Mesh[] childMeshTransforms, 
		//													apRenderUnit[] childRenderUnits, 
		//													apVertexController vertexController, 
		//													apSelection select)
		//{
		//	try
		//	{
		//		//0. 메시, 텍스쳐가 없을 때
		//		if (renderUnit == null || renderUnit._meshTransform == null || renderUnit._meshTransform._mesh == null)
		//		{
		//			return;
		//		}

		//		if(renderUnit._renderVerts.Count == 0)
		//		{
		//			return;
		//		}
		//		Color textureColor = renderUnit._meshColor2X;
		//		apMesh mesh = renderUnit._meshTransform._mesh;

		//		Color[] clipColors = new Color[] { Color.clear, Color.clear, Color.clear };
		//		Texture2D[] clipTextures = new Texture2D[] { null, null, null };
		//		apMesh[] clipMeshes = new apMesh[] { null, null, null };

		//		if(mesh._textureData == null)
		//		{
		//			return;
		//		}

		//		for (int i = 0; i < 3; i++)
		//		{
		//			apTransform_Mesh childMeshTransform = childMeshTransforms[i];
		//			if(childMeshTransform != null)
		//			{
		//				if(childMeshTransform._mesh != null && childMeshTransform._mesh._textureData != null)
		//				{
		//					clipMeshes[i] = childMeshTransform._mesh;
		//					clipTextures[i] = clipMeshes[i]._textureData._image;
		//					clipColors[i] = childRenderUnits[i]._meshColor2X;
		//				}
		//			}
		//		}


		//		bool isBoneWeightColor = (int)(renderType & RENDER_TYPE.BoneRigWeightColor) != 0;


		//		bool isBoneColor = false;
		//		float vertexColorRatio = 0.0f;

		//		if(select != null)
		//		{
		//			isBoneColor = select._rigEdit_isBoneColorView;
		//			if (isBoneWeightColor)
		//			{
		//				if (select._rigEdit_viewMode == apSelection.RIGGING_EDIT_VIEW_MODE.WeightColorOnly)
		//				{
		//					vertexColorRatio = 1.0f;
		//				}
		//				else
		//				{
		//					vertexColorRatio = 0.5f;
		//				}
		//			}
		//		}

		//		apPortrait.SHADER_TYPE shaderType_Clip1 = apPortrait.SHADER_TYPE.AlphaBlend;
		//		apPortrait.SHADER_TYPE shaderType_Clip2 = apPortrait.SHADER_TYPE.AlphaBlend;
		//		apPortrait.SHADER_TYPE shaderType_Clip3 = apPortrait.SHADER_TYPE.AlphaBlend;


		//		if(childMeshTransforms[0] != null) { shaderType_Clip1 = childMeshTransforms[0]._shaderType; }
		//		if(childMeshTransforms[1] != null) { shaderType_Clip2 = childMeshTransforms[1]._shaderType; }
		//		if(childMeshTransforms[2] != null) { shaderType_Clip3 = childMeshTransforms[2]._shaderType; }

		//		//렌더링 방식은 Mesh (with Color) 또는 Vertex / Outline이 있다.

		//		//GL.RenderTargetBarrier();
		//		//2. 메시를 렌더링하자
		//		if(mesh._indexBuffer.Count >= 3)
		//		{
		//			_matBatch.SetMaterialType_MaskedTexture(renderUnit.ShaderType, shaderType_Clip1, shaderType_Clip2, shaderType_Clip3);
		//			_matBatch.SetClippingSize(_glScreenClippingSize);
		//			//------------------------------------------
		//			// Drawcall Batch를 했을때

		//			for (int iPass = 0; iPass < 2; iPass++)
		//			{
		//				// 이건 MultiPass라서 2번 돌려야 한다.
		//				//(여기서는 RenderTexture를 이용했다)

		//				_matBatch.SetPass_MaskedTexture(textureColor, mesh._textureData._image,
		//												clipColors[0], clipTextures[0],
		//												clipColors[1], clipTextures[1],
		//												clipColors[2], clipTextures[2],
		//												iPass, 
		//												vertexColorRatio);


		//				GL.Begin(GL.TRIANGLES);
		//				//------------------------------------------
		//				//1. Mask 먼저 그린다.
		//				apRenderVertex rVert0 = null, rVert1 = null, rVert2 = null;

		//				Color vertexChannelColor = Color.black;
		//				Color vColor0 = Color.black, vColor1 = Color.black, vColor2 = Color.black;

		//				//Vector2 posOffset = Vector2.zero;
		//				////posOffset.z = 0.5f;

		//				for (int i = 0; i < mesh._indexBuffer.Count; i += 3)
		//				{
		//					if (i + 2 >= mesh._indexBuffer.Count)
		//					{ break; }

		//					if (mesh._indexBuffer[i + 0] >= mesh._vertexData.Count ||
		//						mesh._indexBuffer[i + 1] >= mesh._vertexData.Count ||
		//						mesh._indexBuffer[i + 2] >= mesh._vertexData.Count)
		//					{
		//						break;
		//					}

		//					rVert0 = renderUnit._renderVerts[mesh._indexBuffer[i + 0]];
		//					rVert1 = renderUnit._renderVerts[mesh._indexBuffer[i + 1]];
		//					rVert2 = renderUnit._renderVerts[mesh._indexBuffer[i + 2]];

		//					vColor0 = Color.black;
		//					vColor1 = Color.black;
		//					vColor2 = Color.black;

		//					if(isBoneWeightColor)
		//					{
		//						if(isBoneColor)
		//						{
		//							vColor0 = rVert0._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
		//							vColor1 = rVert1._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
		//							vColor2 = rVert2._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
		//						}
		//						else
		//						{
		//							vColor0 = GetWeightColor3(rVert0._renderWeightByTool);
		//							vColor1 = GetWeightColor3(rVert1._renderWeightByTool);
		//							vColor2 = GetWeightColor3(rVert2._renderWeightByTool);
		//						}
		//					}

		//					Vector3 pos_0 = World2GL(rVert0._pos_World);
		//					Vector3 pos_1 = World2GL(rVert1._pos_World);
		//					Vector3 pos_2 = World2GL(rVert2._pos_World);

		//					pos_0.z = 0.5f;
		//					pos_1.z = 0.5f;
		//					pos_2.z = 0.5f;
		//					//pos_0 += posOffset;
		//					//pos_1 += posOffset;
		//					//pos_2 += posOffset;

		//					Vector2 uv_0 = mesh._vertexData[mesh._indexBuffer[i + 0]]._uv;
		//					Vector2 uv_1 = mesh._vertexData[mesh._indexBuffer[i + 1]]._uv;
		//					Vector2 uv_2 = mesh._vertexData[mesh._indexBuffer[i + 2]]._uv;


		//					GL.Color(vertexChannelColor); GL.TexCoord(uv_0); GL.MultiTexCoord3(1, vColor0.r, vColor0.g, vColor0.b); GL.Vertex(pos_0); // 0
		//					GL.Color(vertexChannelColor); GL.TexCoord(uv_1); GL.MultiTexCoord3(1, vColor1.r, vColor1.g, vColor1.b); GL.Vertex(pos_1); // 1
		//					GL.Color(vertexChannelColor); GL.TexCoord(uv_2); GL.MultiTexCoord3(1, vColor2.r, vColor2.g, vColor2.b); GL.Vertex(pos_2); // 2

		//					// Back Side
		//					GL.Color(vertexChannelColor); GL.TexCoord(uv_2); GL.MultiTexCoord3(1, vColor2.r, vColor2.g, vColor2.b); GL.Vertex(pos_2); // 2
		//					GL.Color(vertexChannelColor); GL.TexCoord(uv_1); GL.MultiTexCoord3(1, vColor1.r, vColor1.g, vColor1.b); GL.Vertex(pos_1); // 1
		//					GL.Color(vertexChannelColor); GL.TexCoord(uv_0); GL.MultiTexCoord3(1, vColor0.r, vColor0.g, vColor0.b); GL.Vertex(pos_0); // 0


		//					////if (iPass == 1)
		//					////{
		//					////	GL.MultiTexCoord3(1, 1.0f, 0.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
		//					////	GL.MultiTexCoord3(1, 1.0f, 0.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
		//					////	GL.MultiTexCoord3(1, 1.0f, 0.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

		//					////	// Back Side
		//					////	GL.MultiTexCoord3(1, 1.0f, 0.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
		//					////	GL.MultiTexCoord3(1, 1.0f, 0.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
		//					////	GL.MultiTexCoord3(1, 1.0f, 0.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
		//					////}
		//					////else
		//					////{
		//					//	GL.Color(vertexColor); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
		//					//	GL.Color(vertexColor); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
		//					//	GL.Color(vertexColor); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

		//					//	// Back Side
		//					//	GL.Color(vertexColor); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
		//					//	GL.Color(vertexColor); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
		//					//	GL.Color(vertexColor); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
		//					////}
		//				}


		//				//2. Clip Child를 그린다.
		//				for (int iClip = 0; iClip < 3; iClip++)
		//				{
		//					apMesh clipMesh = clipMeshes[iClip];
		//					apRenderUnit clipRenderUnit = childRenderUnits[iClip];
		//					if (clipMesh == null || clipRenderUnit == null)			{ continue; }
		//					if (clipRenderUnit._meshTransform == null)				{ continue; }
		//					if (!clipRenderUnit._isVisible)	{ continue; }


		//					switch (iClip)
		//					{
		//						case 0:
		//							vertexChannelColor = Color.red;
		//							posOffset.z = 0.4f;
		//							break;

		//						case 1:
		//							vertexChannelColor = Color.green;
		//							posOffset.z = 0.3f;
		//							break;

		//						case 2:
		//							vertexChannelColor = Color.blue;
		//							posOffset.z = 0.2f;
		//							break;
		//					}

		//					for (int i = 0; i < clipMesh._indexBuffer.Count; i += 3)
		//					{
		//						if (i + 2 >= clipMesh._indexBuffer.Count)
		//						{ break; }

		//						if (clipMesh._indexBuffer[i + 0] >= clipMesh._vertexData.Count ||
		//							clipMesh._indexBuffer[i + 1] >= clipMesh._vertexData.Count ||
		//							clipMesh._indexBuffer[i + 2] >= clipMesh._vertexData.Count)
		//						{
		//							break;
		//						}

		//						rVert0 = clipRenderUnit._renderVerts[clipMesh._indexBuffer[i + 0]];
		//						rVert1 = clipRenderUnit._renderVerts[clipMesh._indexBuffer[i + 1]];
		//						rVert2 = clipRenderUnit._renderVerts[clipMesh._indexBuffer[i + 2]];


		//						vColor0 = Color.black;
		//						vColor1 = Color.black;
		//						vColor2 = Color.black;

		//						if(isBoneWeightColor)
		//						{
		//							if(isBoneColor)
		//							{
		//								vColor0 = rVert0._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
		//								vColor1 = rVert1._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
		//								vColor2 = rVert2._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
		//							}
		//							else
		//							{
		//								vColor0 = GetWeightColor3(rVert0._renderWeightByTool);
		//								vColor1 = GetWeightColor3(rVert1._renderWeightByTool);
		//								vColor2 = GetWeightColor3(rVert2._renderWeightByTool);
		//							}
		//						}


		//						Vector3 pos_0 = World2GL(rVert0._pos_World3);
		//						Vector3 pos_1 = World2GL(rVert1._pos_World3);
		//						Vector3 pos_2 = World2GL(rVert2._pos_World3);

		//						pos_0 += posOffset;
		//						pos_1 += posOffset;
		//						pos_2 += posOffset;

		//						Vector2 uv_0 = clipMesh._vertexData[clipMesh._indexBuffer[i + 0]]._uv;
		//						Vector2 uv_1 = clipMesh._vertexData[clipMesh._indexBuffer[i + 1]]._uv;
		//						Vector2 uv_2 = clipMesh._vertexData[clipMesh._indexBuffer[i + 2]]._uv;


		//						GL.Color(vertexChannelColor); GL.TexCoord(uv_0); GL.MultiTexCoord3(1, vColor0.r, vColor0.g, vColor0.b); GL.Vertex(pos_0); // 0
		//						GL.Color(vertexChannelColor); GL.TexCoord(uv_1); GL.MultiTexCoord3(1, vColor1.r, vColor1.g, vColor1.b); GL.Vertex(pos_1); // 1
		//						GL.Color(vertexChannelColor); GL.TexCoord(uv_2); GL.MultiTexCoord3(1, vColor2.r, vColor2.g, vColor2.b); GL.Vertex(pos_2); // 2

		//						//Back Side
		//						GL.Color(vertexChannelColor); GL.TexCoord(uv_2); GL.MultiTexCoord3(1, vColor2.r, vColor2.g, vColor2.b); GL.Vertex(pos_2); // 2
		//						GL.Color(vertexChannelColor); GL.TexCoord(uv_1); GL.MultiTexCoord3(1, vColor1.r, vColor1.g, vColor1.b); GL.Vertex(pos_1); // 1
		//						GL.Color(vertexChannelColor); GL.TexCoord(uv_0); GL.MultiTexCoord3(1, vColor0.r, vColor0.g, vColor0.b); GL.Vertex(pos_0); // 0


		//						////if(iPass == 1)
		//						////{
		//						////	GL.MultiTexCoord3(2, 0.0f, 1.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
		//						////	GL.MultiTexCoord3(2, 0.0f, 1.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
		//						////	GL.MultiTexCoord3(2, 0.0f, 1.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

		//						////	// Back Side
		//						////	GL.MultiTexCoord3(2, 0.0f, 1.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
		//						////	GL.MultiTexCoord3(2, 0.0f, 1.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
		//						////	GL.MultiTexCoord3(2, 0.0f, 1.0f, 1.0f); GL.Color(vertexColor); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
		//						////}
		//						////else
		//						////{
		//						//	GL.Color(vertexColor); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
		//						//	GL.Color(vertexColor); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
		//						//	GL.Color(vertexColor); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

		//						//	// Back Side
		//						//	GL.Color(vertexColor); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
		//						//	GL.Color(vertexColor); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
		//						//	GL.Color(vertexColor); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
		//						////}

		//					}
		//				}
		//				//------------------------------------------
		//				GL.End();
		//			}

		//			//사용했던 RenderTexture를 해제한다.
		//			_matBatch.ReleaseRenderTexture();

		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Debug.LogException(ex);
		//	}
		//} 
		#endregion






		public static void DrawRenderUnit_ClippingParent_Renew(apRenderUnit renderUnit,
																RENDER_TYPE renderType,
																List<apTransform_Mesh.ClipMeshSet> childClippedSet,
																//List<apTransform_Mesh> childMeshTransforms, 
																//List<apRenderUnit> childRenderUnits, 
																apVertexController vertexController,
																apEditor editor,
																apSelection select,
																RenderTexture externalRenderTexture = null)
		{
			//렌더링 순서
			//Parent - 기본
			//Parent - Mask
			//(For) Child - Clipped
			//Release RenderMask
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				if (renderUnit == null || renderUnit._meshTransform == null || renderUnit._meshTransform._mesh == null)
				{
					return;
				}

				if (renderUnit._renderVerts.Count == 0)
				{
					return;
				}
				Color textureColor = renderUnit._meshColor2X;
				apMesh mesh = renderUnit._meshTransform._mesh;


				apTextureData linkedTextureData = mesh.LinkedTextureData;

				//추가 12.4 : Extra Option에 의해 Texture가 바귀었을 경우
				if(renderUnit.IsExtraTextureChanged)
				{
					linkedTextureData = renderUnit.ChangedExtraTextureData;
				}

				//if (mesh.LinkedTextureData == null)//이전
				if(linkedTextureData == null)
				{
					return;
				}



				int nClipMeshes = childClippedSet.Count;


				bool isBoneWeightColor = (int)(renderType & RENDER_TYPE.BoneRigWeightColor) != 0;
				bool isPhyVolumeWeightColor = (renderType & RENDER_TYPE.PhysicsWeightColor) != 0 || (renderType & RENDER_TYPE.VolumeWeightColor) != 0;

				int iVertColor = 0;

				if ((renderType & RENDER_TYPE.VolumeWeightColor) != 0)
				{
					iVertColor = 1;
				}
				else if ((renderType & RENDER_TYPE.PhysicsWeightColor) != 0)
				{
					iVertColor = 2;
				}
				else if ((renderType & RENDER_TYPE.BoneRigWeightColor) != 0)
				{
					iVertColor = 3;
				}
				else
				{
					iVertColor = 0;
				}

				bool isBoneColor = false;
				//bool isCircleRiggingVert = editor._rigViewOption_CircleVert;
				float vertexColorRatio = 0.0f;

				bool isToneColor = (int)(renderType & RENDER_TYPE.ToneColor) != 0;

				if (select != null)
				{
					//isBoneColor = select._rigEdit_isBoneColorView;//이전
					isBoneColor = editor._rigViewOption_BoneColor;//변경 19.7.31

					if (isBoneWeightColor)
					{
						//if (select._rigEdit_viewMode == apSelection.RIGGING_EDIT_VIEW_MODE.WeightColorOnly)
						if(editor._rigViewOption_WeightOnly)
						{
							vertexColorRatio = 1.0f;
						}
						else
						{
							vertexColorRatio = 0.5f;
						}
					}
					else if (isPhyVolumeWeightColor)
					{
						vertexColorRatio = 0.7f;
					}
				}


				//렌더링 방식은 Mesh (with Color) 또는 Vertex / Outline이 있다.

				//1. Parent의 기본 렌더링을 하자
				//+2. Parent의 마스크를 렌더링하자
				if (mesh._indexBuffer.Count < 3)
				{
					return;
				}

				apRenderVertex rVert0 = null, rVert1 = null, rVert2 = null;

				Color vertexChannelColor = Color.black;
				Color vColor0 = Color.black, vColor1 = Color.black, vColor2 = Color.black;

				Vector2 posGL_0 = Vector2.zero;
				Vector2 posGL_1 = Vector2.zero;
				Vector2 posGL_2 = Vector2.zero;

				Vector3 pos_0 = Vector3.zero;
				Vector3 pos_1 = Vector3.zero;
				Vector3 pos_2 = Vector3.zero;
		
				Vector2 uv_0 = Vector2.zero;
				Vector2 uv_1 = Vector2.zero;
				Vector2 uv_2 = Vector2.zero;


				
				RenderTexture.active = null;

				for (int iPass = 0; iPass < 2; iPass++)
				{
					bool isRenderTexture = false;
					if (iPass == 1)
					{
						isRenderTexture = true;
					}
					if(isToneColor)
					{
						// ToneColor Mask
						//_matBatch.SetPass_Mask_ToneColor(_toneColor, mesh.LinkedTextureData._image, isRenderTexture);//이전
						_matBatch.SetPass_Mask_ToneColor(_toneColor, linkedTextureData._image, isRenderTexture);
						
					}
					else
					{
						//일반적인 Mask
						//_matBatch.SetPass_Mask(textureColor, mesh.LinkedTextureData._image, vertexColorRatio, renderUnit.ShaderType, isRenderTexture);//이전
						_matBatch.SetPass_Mask(textureColor, linkedTextureData._image, vertexColorRatio, renderUnit.ShaderType, isRenderTexture);
					}
					
					_matBatch.SetClippingSize(_glScreenClippingSize);


					GL.Begin(GL.TRIANGLES);
					//------------------------------------------
					for (int i = 0; i < mesh._indexBuffer.Count; i += 3)
					{

						if (i + 2 >= mesh._indexBuffer.Count)
						{ break; }

						if (mesh._indexBuffer[i + 0] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 1] >= mesh._vertexData.Count ||
							mesh._indexBuffer[i + 2] >= mesh._vertexData.Count)
						{
							break;
						}

						rVert0 = renderUnit._renderVerts[mesh._indexBuffer[i + 0]];
						rVert1 = renderUnit._renderVerts[mesh._indexBuffer[i + 1]];
						rVert2 = renderUnit._renderVerts[mesh._indexBuffer[i + 2]];

						vColor0 = Color.black;
						vColor1 = Color.black;
						vColor2 = Color.black;

						//if (isBoneWeightColor)
						//{
						//	if (isBoneColor)
						//	{
						//		vColor0 = rVert0._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
						//		vColor1 = rVert1._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
						//		vColor2 = rVert2._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
						//	}
						//	else
						//	{
						//		vColor0 = GetWeightColor3(rVert0._renderWeightByTool);
						//		vColor1 = GetWeightColor3(rVert1._renderWeightByTool);
						//		vColor2 = GetWeightColor3(rVert2._renderWeightByTool);
						//	}
						//}
						//else if (isPhyVolumeWeightColor)
						//{
						//	vColor0 = GetWeightGrayscale(rVert0._renderWeightByTool);
						//	vColor1 = GetWeightGrayscale(rVert1._renderWeightByTool);
						//	vColor2 = GetWeightGrayscale(rVert2._renderWeightByTool);
						//}

						switch (iVertColor)
						{
							case 1: //VolumeWeightColor
								vColor0 = GetWeightGrayscale(rVert0._renderWeightByTool);
								vColor1 = GetWeightGrayscale(rVert1._renderWeightByTool);
								vColor2 = GetWeightGrayscale(rVert2._renderWeightByTool);
								break;

							case 2: //PhysicsWeightColor
								vColor0 = GetWeightColor4(rVert0._renderWeightByTool);
								vColor1 = GetWeightColor4(rVert1._renderWeightByTool);
								vColor2 = GetWeightColor4(rVert2._renderWeightByTool);
								break;

							case 3: //BoneRigWeightColor
								if (isBoneColor)
								{
									vColor0 = rVert0._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
									vColor1 = rVert1._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
									vColor2 = rVert2._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
								}
								else
								{
									vColor0 = GetWeightColor3(rVert0._renderWeightByTool);
									vColor1 = GetWeightColor3(rVert1._renderWeightByTool);
									vColor2 = GetWeightColor3(rVert2._renderWeightByTool);
								}
								vColor0.a = 1.0f;
								vColor1.a = 1.0f;
								vColor2.a = 1.0f;
								break;
						}


						posGL_0 = World2GL(rVert0._pos_World);
						posGL_1 = World2GL(rVert1._pos_World);
						posGL_2 = World2GL(rVert2._pos_World);

						pos_0.x = posGL_0.x;
						pos_0.y = posGL_0.y;
						pos_0.z = rVert0._vertex._zDepth * 0.5f;

						pos_1.x = posGL_1.x;
						pos_1.y = posGL_1.y;
						pos_1.z = rVert1._vertex._zDepth * 0.5f;

						pos_2.x = posGL_2.x;
						pos_2.y = posGL_2.y;
						pos_2.z = rVert2._vertex._zDepth * 0.5f;
						
						uv_0 = mesh._vertexData[mesh._indexBuffer[i + 0]]._uv;
						uv_1 = mesh._vertexData[mesh._indexBuffer[i + 1]]._uv;
						uv_2 = mesh._vertexData[mesh._indexBuffer[i + 2]]._uv;

						
						GL.Color(vColor0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
						GL.Color(vColor1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(vColor2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

						// Back Side
						GL.Color(vColor2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
						GL.Color(vColor1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(vColor0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
					}



					//------------------------------------------
					GL.End();
				}

				if (externalRenderTexture == null)
				{
					_matBatch.DeactiveRenderTexture();
				}
				else
				{
					RenderTexture.active = externalRenderTexture;
				}

				

				//3. Child를 렌더링하자
				
				apTransform_Mesh.ClipMeshSet clipMeshSet = null;

				for (int iClip = 0; iClip < nClipMeshes; iClip++)
				{
					clipMeshSet = childClippedSet[iClip];
					if(clipMeshSet == null || clipMeshSet._meshTransform == null)
					{
						continue;
					}
					apMesh clipMesh = clipMeshSet._meshTransform._mesh;
					apRenderUnit clipRenderUnit = clipMeshSet._renderUnit;

					if (clipMesh == null || clipRenderUnit == null) { continue; }
					if (clipRenderUnit._meshTransform == null) { continue; }
					if (!clipRenderUnit._isVisible) { continue; }

					if (clipMesh._indexBuffer.Count < 3)
					{
						continue;
					}

					//추가 12.04 : Extra 옵션 적용
					apTextureData childTextureData = clipMesh.LinkedTextureData;
					if(clipRenderUnit.IsExtraTextureChanged)
					{
						childTextureData = clipRenderUnit.ChangedExtraTextureData;
					}

					if(isToneColor)
					{
						//Onion ToneColor Clipping
						//_matBatch.SetPass_Clipped_ToneColor(_toneColor, clipMesh.LinkedTextureData._image, renderUnit._meshColor2X);//이전
						_matBatch.SetPass_Clipped_ToneColor(_toneColor, childTextureData._image, renderUnit._meshColor2X);
					}
					else
					{
						//일반 Clipping
						//_matBatch.SetPass_Clipped(clipRenderUnit._meshColor2X, clipMesh.LinkedTextureData._image, vertexColorRatio, clipRenderUnit.ShaderType, renderUnit._meshColor2X);//이전
						_matBatch.SetPass_Clipped(clipRenderUnit._meshColor2X, childTextureData._image, vertexColorRatio, clipRenderUnit.ShaderType, renderUnit._meshColor2X);
					}
					
					_matBatch.SetClippingSize(_glScreenClippingSize);

					GL.Begin(GL.TRIANGLES);
					//------------------------------------------
					//try
					//{
						for (int i = 0; i < clipMesh._indexBuffer.Count; i += 3)
						{
							if (i + 2 >= clipMesh._indexBuffer.Count)
							{ break; }

							if (clipMesh._indexBuffer[i + 0] >= clipMesh._vertexData.Count ||
								clipMesh._indexBuffer[i + 1] >= clipMesh._vertexData.Count ||
								clipMesh._indexBuffer[i + 2] >= clipMesh._vertexData.Count)
							{
								break;
							}

							rVert0 = clipRenderUnit._renderVerts[clipMesh._indexBuffer[i + 0]];
							rVert1 = clipRenderUnit._renderVerts[clipMesh._indexBuffer[i + 1]];
							rVert2 = clipRenderUnit._renderVerts[clipMesh._indexBuffer[i + 2]];


							vColor0 = Color.black;
							vColor1 = Color.black;
							vColor2 = Color.black;

							if (isBoneWeightColor)
							{
								if (isBoneColor)
								{
									vColor0 = rVert0._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
									vColor1 = rVert1._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
									vColor2 = rVert2._renderColorByTool * (vertexColorRatio) + Color.black * (1.0f - vertexColorRatio);
								}
								else
								{
									vColor0 = GetWeightColor3(rVert0._renderWeightByTool);
									vColor1 = GetWeightColor3(rVert1._renderWeightByTool);
									vColor2 = GetWeightColor3(rVert2._renderWeightByTool);
								}
							}
							else if (isPhyVolumeWeightColor)
							{
								vColor0 = GetWeightGrayscale(rVert0._renderWeightByTool);
								vColor1 = GetWeightGrayscale(rVert1._renderWeightByTool);
								vColor2 = GetWeightGrayscale(rVert2._renderWeightByTool);
							}



							posGL_0 = World2GL(rVert0._pos_World);
							posGL_1 = World2GL(rVert1._pos_World);
							posGL_2 = World2GL(rVert2._pos_World);

							pos_0.x = posGL_0.x;
							pos_0.y = posGL_0.y;
							pos_0.z = rVert0._vertex._zDepth * 0.5f;

							pos_1.x = posGL_1.x;
							pos_1.y = posGL_1.y;
							pos_1.z = rVert1._vertex._zDepth * 0.5f;

							pos_2.x = posGL_2.x;
							pos_2.y = posGL_2.y;
							pos_2.z = rVert2._vertex._zDepth * 0.5f;

							uv_0 = clipMesh._vertexData[clipMesh._indexBuffer[i + 0]]._uv;
							uv_1 = clipMesh._vertexData[clipMesh._indexBuffer[i + 1]]._uv;
							uv_2 = clipMesh._vertexData[clipMesh._indexBuffer[i + 2]]._uv;


							GL.Color(vColor0);
							GL.TexCoord(uv_0);
							GL.Vertex(pos_0); // 0
							GL.Color(vColor1);
							GL.TexCoord(uv_1);
							GL.Vertex(pos_1); // 1
							GL.Color(vColor2);
							GL.TexCoord(uv_2);
							GL.Vertex(pos_2); // 2

							//Back Side
							GL.Color(vColor2);
							GL.TexCoord(uv_2);
							GL.Vertex(pos_2); // 2
							GL.Color(vColor1);
							GL.TexCoord(uv_1);
							GL.Vertex(pos_1); // 1
							GL.Color(vColor0);
							GL.TexCoord(uv_0);
							GL.Vertex(pos_0); // 0


						}
					//}
					//catch(Exception ex)
					//{
					//	Debug.LogError("Draw Render Unit Clipping Parent Exception : " + ex);
					//	Debug.LogError("Index Buffer : " + clipMesh._indexBuffer.Count);
					//	Debug.LogError("Mesh Vertex : " + clipMesh._vertexData.Count);
					//	Debug.LogError("Render Unit Vertex : " + clipRenderUnit._renderVerts.Count);
						
					//}
					//------------------------------------------------
					GL.End();

				}

				//사용했던 RenderTexture를 해제한다.
				_matBatch.ReleaseRenderTexture();
				//_matBatch.DeactiveRenderTexture();

			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}




		/// <summary>
		/// Clipping Render의 Mask Texture만 취하는 함수
		/// RTT 후 실제 Texture2D로 굽기 때문에 실시간으로는 사용하기 힘들다.
		/// 클리핑을 하지 않는다.
		/// </summary>
		/// <param name="renderUnit"></param>
		/// <returns></returns>
		public static Texture2D GetMaskTexture_ClippingParent(apRenderUnit renderUnit)
		{
			//렌더링 순서
			//Parent - 기본
			//Parent - Mask
			//(For) Child - Clipped
			//Release RenderMask
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				if (renderUnit == null || renderUnit._meshTransform == null || renderUnit._meshTransform._mesh == null)
				{
					return null;
				}

				if (renderUnit._renderVerts.Count == 0)
				{
					return null;
				}
				apMesh mesh = renderUnit._meshTransform._mesh;

				apTextureData linkedTextureData = mesh.LinkedTextureData;

				//추가 12.4 : Extra Option에 의해 Texture가 바귀었을 경우
				if(renderUnit.IsExtraTextureChanged)
				{
					linkedTextureData = renderUnit.ChangedExtraTextureData;
				}

				//if (mesh.LinkedTextureData == null)//이전
				if(linkedTextureData == null)
				{
					return null;
				}
				

				//렌더링 방식은 Mesh (with Color) 또는 Vertex / Outline이 있다.

				//1. Parent의 기본 렌더링을 하자
				//+2. Parent의 마스크를 렌더링하자
				if (mesh._indexBuffer.Count < 3)
				{
					return null;
				}

				apRenderVertex rVert0 = null, rVert1 = null, rVert2 = null;

				//Pass는 RTT용 Pass 한개만 둔다.
				bool isRenderTexture = true; //<<RTT만 한다.
				//_matBatch.SetPass_Mask(Color.gray, mesh.LinkedTextureData._image, 0.0f, renderUnit.ShaderType, isRenderTexture);//이전
				_matBatch.SetPass_Mask(Color.gray, linkedTextureData._image, 0.0f, renderUnit.ShaderType, isRenderTexture);

				_matBatch.SetClippingSize(new Vector4(0, 0, 1, 1));//<<클리핑을 하지 않는다.

				Vector2 posGL_0 = Vector2.zero;
				Vector2 posGL_1 = Vector2.zero;
				Vector2 posGL_2 = Vector2.zero;

				Vector3 pos_0 = Vector3.zero;
				Vector3 pos_1 = Vector3.zero;
				Vector3 pos_2 = Vector3.zero;

				Vector2 uv_0 = Vector2.zero;
				Vector2 uv_1 = Vector2.zero;
				Vector2 uv_2 = Vector2.zero;

				GL.Begin(GL.TRIANGLES);
				//------------------------------------------
				for (int i = 0; i < mesh._indexBuffer.Count; i += 3)
				{

					if (i + 2 >= mesh._indexBuffer.Count)
					{ break; }

					if (mesh._indexBuffer[i + 0] >= mesh._vertexData.Count ||
						mesh._indexBuffer[i + 1] >= mesh._vertexData.Count ||
						mesh._indexBuffer[i + 2] >= mesh._vertexData.Count)
					{
						break;
					}

					rVert0 = renderUnit._renderVerts[mesh._indexBuffer[i + 0]];
					rVert1 = renderUnit._renderVerts[mesh._indexBuffer[i + 1]];
					rVert2 = renderUnit._renderVerts[mesh._indexBuffer[i + 2]];

					posGL_0 = World2GL(rVert0._pos_World);
					posGL_1 = World2GL(rVert1._pos_World);
					posGL_2 = World2GL(rVert2._pos_World);


					pos_0.x = posGL_0.x;
					pos_0.y = posGL_0.y;
					pos_0.z = rVert0._vertex._zDepth * 0.5f;

					pos_1.x = posGL_1.x;
					pos_1.y = posGL_1.y;
					pos_1.z = rVert1._vertex._zDepth * 0.5f;			   

					pos_2.x = posGL_2.x;
					pos_2.y = posGL_2.y;
					pos_2.z = rVert2._vertex._zDepth * 0.5f;			   

					uv_0 = mesh._vertexData[mesh._indexBuffer[i + 0]]._uv;
					uv_1 = mesh._vertexData[mesh._indexBuffer[i + 1]]._uv;
					uv_2 = mesh._vertexData[mesh._indexBuffer[i + 2]]._uv;


					GL.Color(Color.black); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
					GL.Color(Color.black); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
					GL.Color(Color.black); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

					// Back Side
					GL.Color(Color.black); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
					GL.Color(Color.black); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
					GL.Color(Color.black); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
				}



				//------------------------------------------
				GL.End();

				//Texture2D로 굽자
				Texture2D resultTex = new Texture2D(_matBatch.RenderTex.width, _matBatch.RenderTex.height, TextureFormat.RGBA32, false);
				resultTex.ReadPixels(new Rect(0, 0, _matBatch.RenderTex.width, _matBatch.RenderTex.height), 0, 0);
				resultTex.Apply();

				//사용했던 RenderTexture를 해제한다.
				_matBatch.ReleaseRenderTexture();
				//_matBatch.DeactiveRenderTexture();

				return resultTex;

			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			return null;
		}


		/// <summary>
		/// RTT 없이 "이미 구워진 MaskTexture"를 이용해서 Clipping 렌더링을 한다.
		/// 클리핑을 하지 않는다.
		/// </summary>
		/// <param name="renderUnit"></param>
		/// <param name="renderType"></param>
		/// <param name="childClippedSet"></param>
		/// <param name="vertexController"></param>
		/// <param name="select"></param>
		/// <param name="externalRenderTexture"></param>
		public static void DrawRenderUnit_ClippingParent_Renew_WithoutRTT(apRenderUnit renderUnit,
																			List<apTransform_Mesh.ClipMeshSet> childClippedSet,
																			Texture2D maskedTexture
																		)
		{
			//렌더링 순서
			//Parent - 기본
			//Parent - Mask
			//(For) Child - Clipped
			//Release RenderMask
			try
			{
				//0. 메시, 텍스쳐가 없을 때
				if (renderUnit == null || renderUnit._meshTransform == null || renderUnit._meshTransform._mesh == null)
				{
					return;
				}

				if (renderUnit._renderVerts.Count == 0)
				{
					return;
				}
				Color textureColor = renderUnit._meshColor2X;
				apMesh mesh = renderUnit._meshTransform._mesh;


				apTextureData linkedTextureData = mesh.LinkedTextureData;

				//추가 12.4 : Extra Option에 의해 Texture가 바귀었을 경우
				if(renderUnit.IsExtraTextureChanged)
				{
					linkedTextureData = renderUnit.ChangedExtraTextureData;
				}

				//if (mesh.LinkedTextureData == null)//이전
				if(linkedTextureData == null)
				{
					return;
				}

				

				int nClipMeshes = childClippedSet.Count;


				//렌더링 방식은 Mesh (with Color) 또는 Vertex / Outline이 있다.

				//1. Parent의 기본 렌더링을 하자
				//+2. Parent의 마스크를 렌더링하자
				if (mesh._indexBuffer.Count < 3)
				{
					return;
				}

				apRenderVertex rVert0 = null, rVert1 = null, rVert2 = null;

				Color vertexChannelColor = Color.black;
				Color vColor0 = Color.black, vColor1 = Color.black, vColor2 = Color.black;


				Vector2 posGL_0 = Vector2.zero;
				Vector2 posGL_1 = Vector2.zero;
				Vector2 posGL_2 = Vector2.zero;

				Vector3 pos_0 = Vector3.zero;
				Vector3 pos_1 = Vector3.zero;
				Vector3 pos_2 = Vector3.zero;

				Vector2 uv_0 = Vector2.zero;
				Vector2 uv_1 = Vector2.zero;
				Vector2 uv_2 = Vector2.zero;

				//RTT 관련 코드는 모두 뺀다. Pass도 한번이고 기본 렌더링
				//_matBatch.SetPass_Texture_VColor(textureColor, mesh.LinkedTextureData._image, 0.0f, renderUnit.ShaderType);//이전
				_matBatch.SetPass_Texture_VColor(textureColor, linkedTextureData._image, 0.0f, renderUnit.ShaderType);
				
				//_matBatch.SetClippingSize(_glScreenClippingSize);
				_matBatch.SetClippingSize(new Vector4(0, 0, 1, 1));//<<클리핑을 하지 않는다.


				GL.Begin(GL.TRIANGLES);
				//------------------------------------------
				for (int i = 0; i < mesh._indexBuffer.Count; i += 3)
				{

					if (i + 2 >= mesh._indexBuffer.Count)
					{ break; }

					if (mesh._indexBuffer[i + 0] >= mesh._vertexData.Count ||
						mesh._indexBuffer[i + 1] >= mesh._vertexData.Count ||
						mesh._indexBuffer[i + 2] >= mesh._vertexData.Count)
					{
						break;
					}

					rVert0 = renderUnit._renderVerts[mesh._indexBuffer[i + 0]];
					rVert1 = renderUnit._renderVerts[mesh._indexBuffer[i + 1]];
					rVert2 = renderUnit._renderVerts[mesh._indexBuffer[i + 2]];

					vColor0 = Color.black;
					vColor1 = Color.black;
					vColor2 = Color.black;

					posGL_0 = World2GL(rVert0._pos_World);
					posGL_1 = World2GL(rVert1._pos_World);
					posGL_2 = World2GL(rVert2._pos_World);

					pos_0.x = posGL_0.x;
					pos_0.y = posGL_0.y;
					pos_0.z = rVert0._vertex._zDepth * 0.5f;

					pos_1.x = posGL_1.x;
					pos_1.y = posGL_1.y;
					pos_1.z = rVert1._vertex._zDepth * 0.5f;

					pos_2.x = posGL_2.x;
					pos_2.y = posGL_2.y;
					pos_2.z = rVert2._vertex._zDepth * 0.5f;
					


					uv_0 = mesh._vertexData[mesh._indexBuffer[i + 0]]._uv;
					uv_1 = mesh._vertexData[mesh._indexBuffer[i + 1]]._uv;
					uv_2 = mesh._vertexData[mesh._indexBuffer[i + 2]]._uv;


					GL.Color(vColor0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
					GL.Color(vColor1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
					GL.Color(vColor2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

					// Back Side
					GL.Color(vColor2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
					GL.Color(vColor1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
					GL.Color(vColor0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
				}



				//------------------------------------------
				GL.End();


				//3. Child를 렌더링하자. MaskedTexture를 직접 이용
				apTransform_Mesh.ClipMeshSet clipMeshSet = null;
				for (int iClip = 0; iClip < nClipMeshes; iClip++)
				{
					clipMeshSet = childClippedSet[iClip];
					if (clipMeshSet == null || clipMeshSet._meshTransform == null)
					{
						continue;
					}
					apMesh clipMesh = clipMeshSet._meshTransform._mesh;
					apRenderUnit clipRenderUnit = clipMeshSet._renderUnit;

					if (clipMesh == null || clipRenderUnit == null)		{ continue; }
					if (clipRenderUnit._meshTransform == null)			{ continue; }
					if (!clipRenderUnit._isVisible)						{ continue; }

					if (clipMesh._indexBuffer.Count < 3)
					{
						continue;
					}

					//추가 12.4 : Extra Option
					apTextureData clipTextureData = clipMesh.LinkedTextureData;
					if(clipRenderUnit.IsExtraTextureChanged)
					{
						clipTextureData = clipRenderUnit.ChangedExtraTextureData;
					}

					_matBatch.SetPass_ClippedWithMaskedTexture(clipRenderUnit._meshColor2X,
																//clipMesh.LinkedTextureData._image,//이전
																clipTextureData._image,
																0.0f,
																clipRenderUnit.ShaderType,
																renderUnit._meshColor2X,
																maskedTexture);

					//_matBatch.SetClippingSize(_glScreenClippingSize);
					_matBatch.SetClippingSize(new Vector4(0, 0, 1, 1));//<<클리핑을 하지 않는다.

					GL.Begin(GL.TRIANGLES);
					//------------------------------------------
					for (int i = 0; i < clipMesh._indexBuffer.Count; i += 3)
					{
						if (i + 2 >= clipMesh._indexBuffer.Count)
						{ break; }

						if (clipMesh._indexBuffer[i + 0] >= clipMesh._vertexData.Count ||
							clipMesh._indexBuffer[i + 1] >= clipMesh._vertexData.Count ||
							clipMesh._indexBuffer[i + 2] >= clipMesh._vertexData.Count)
						{
							break;
						}

						rVert0 = clipRenderUnit._renderVerts[clipMesh._indexBuffer[i + 0]];
						rVert1 = clipRenderUnit._renderVerts[clipMesh._indexBuffer[i + 1]];
						rVert2 = clipRenderUnit._renderVerts[clipMesh._indexBuffer[i + 2]];


						vColor0 = Color.black;
						vColor1 = Color.black;
						vColor2 = Color.black;


						posGL_0 = World2GL(rVert0._pos_World);
						posGL_1 = World2GL(rVert1._pos_World);
						posGL_2 = World2GL(rVert2._pos_World);

						pos_0.x = posGL_0.x;
						pos_0.y = posGL_0.y;
						pos_0.z = rVert0._vertex._zDepth * 0.5f;

						pos_1.x = posGL_1.x;
						pos_1.y = posGL_1.y;
						pos_1.z = rVert1._vertex._zDepth * 0.5f;

						pos_2.x = posGL_2.x;
						pos_2.y = posGL_2.y;
						pos_2.z = rVert2._vertex._zDepth * 0.5f;


						uv_0 = clipMesh._vertexData[clipMesh._indexBuffer[i + 0]]._uv;
						uv_1 = clipMesh._vertexData[clipMesh._indexBuffer[i + 1]]._uv;
						uv_2 = clipMesh._vertexData[clipMesh._indexBuffer[i + 2]]._uv;


						GL.Color(vColor0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0
						GL.Color(vColor1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(vColor2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2

						//Back Side
						GL.Color(vColor2); GL.TexCoord(uv_2); GL.Vertex(pos_2); // 2
						GL.Color(vColor1); GL.TexCoord(uv_1); GL.Vertex(pos_1); // 1
						GL.Color(vColor0); GL.TexCoord(uv_0); GL.Vertex(pos_0); // 0


					}
					//------------------------------------------------
					GL.End();

				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		//------------------------------------------------------------------------------------------------
		// Rigging Vertex (Circle)
		//------------------------------------------------------------------------------------------------
		private static void DrawRiggingRenderVert(apRenderVertex renderVertex, Color colorOutline, bool isUseBoneColor)
		{
			/*
			_matBatch.SetPass_Color();
			_matBatch.SetClippingSize(_glScreenClippingSize);

			GL.Begin(GL.TRIANGLES);
			*/

			Vector2 posCenterGL = renderVertex._pos_GL;
			

			
			

			colorOutline.a = 1.0f;//<<여기선 반투명 Outline을 지원하지 않는다.

			if (renderVertex._renderRigWeightParam._nParam == 0)
			{
				//데이터가 없는 경우 
				float size_None = 10.0f;
				float size_None_Outline = 14.0f;

				//그냥 박스
				//Outline 그리기
				DrawBoxWithVColorAndUV(posCenterGL, size_None_Outline, size_None_Outline, colorOutline);

				DrawBoxWithVColorAndUV(posCenterGL, size_None, size_None, Color.grey);
			}
			else
			{
				//N개의 데이터

				//Degree 방식으로 단순 증가
				//> -1 * Deg2Rad - (0.5 * PI)
				//UV 중요
				//45도마다 분할
				float prevAngle_Deg = 0.0f;
				float nextAngle_Deg = 0.0f;
				//float prevAngle_Rad = 0.0f;
				//float nextAngle_Rad = 0.0f;

				float radius_Small = 12.0f;
				float radius_Large = 17.0f;
				float radius_Small_Outline = 14.0f;
				float radius_Large_Outline = 20.0f;

				float curRatio = 0.0f;
				float prevRatio = 0.0f;
				float nextRatio = 0.0f;
				Color curColor = Color.black;
				bool curSelected = false;

				for (int iRig = 0; iRig < renderVertex._renderRigWeightParam._nParam; iRig++)
				{
					curRatio = renderVertex._renderRigWeightParam._ratios[iRig];
					curSelected = renderVertex._renderRigWeightParam._isSelecteds[iRig];

					if (isUseBoneColor)
					{
						curColor = renderVertex._renderRigWeightParam._colors[iRig];
						curColor.a = 1.0f;
					}
					else
					{
						if (curSelected)
						{
							curColor = GetWeightColor3(curRatio);
						}
						else
						{
							curColor = renderVertex._renderRigWeightParam._colors[iRig];
							curColor *= 0.5f;
							curColor.a = 1.0f;
						}
					}
					nextRatio = prevRatio + curRatio;
					nextAngle_Deg = prevAngle_Deg + (curRatio * 360.0f);

					//TODO. 그리자!
					//prevAngle_Rad = (prevAngle_Deg * Mathf.Deg2Rad * -1.0f) - Mathf.PI * 0.5f;
					//nextAngle_Rad = (nextAngle_Deg * Mathf.Deg2Rad * -1.0f) - Mathf.PI * 0.5f;

					//0도부터 45도 마다 분리하여 삼각형으로 만들어서 렌더링
					//DrawRigCirclePart(posCenterGL, )
					if (curSelected)
					{
						DrawRigCirclePart(posCenterGL, radius_Large, radius_Large_Outline, curColor, colorOutline, prevAngle_Deg, nextAngle_Deg);
					}
					else
					{
						DrawRigCirclePart(posCenterGL, radius_Small, radius_Small_Outline, curColor, colorOutline, prevAngle_Deg, nextAngle_Deg);
					}


					//다음으로 이동
					prevRatio = nextRatio;
					prevAngle_Deg = nextAngle_Deg;
				}
			}


			#region [미사용 코드]



			//float angleRad_0 = 0.0f;
			//float angleRad_1 = 0.0f;
			//Vector2 pos0 = Vector2.zero;
			//Vector2 pos1 = Vector2.zero;
			//float lastAngle = 0.0f;



			//if (renderVertex._renderRigWeightParam._nParam == 0)
			//{
			//	//데이터가 없는 경우 
			//	float radius_None = 5.0f;
			//	float radius_None_Outline = 7.0f;
			//	int nDiv = 12;

			//	//Outline 먼저
			//	GL.Color(colorOutline);
			//	for (int i = 0; i < nDiv; i++)
			//	{
			//		angleRad_0 = (i * Mathf.PI * 2.0f) / (float)nDiv;
			//		angleRad_1 = ((i + 1) * Mathf.PI * 2.0f) / (float)nDiv;

			//		pos0 = posCenterGL + new Vector2(Mathf.Cos(angleRad_0) * radius_None_Outline, Mathf.Sin(angleRad_0) * radius_None_Outline);
			//		pos1 = posCenterGL + new Vector2(Mathf.Cos(angleRad_1) * radius_None_Outline, Mathf.Sin(angleRad_1) * radius_None_Outline);

			//		GL.Vertex(posCenterGL);
			//		GL.Vertex(pos0);
			//		GL.Vertex(pos1);
			//	}

			//	//	내부 버텍스 렌더링
			//	GL.Color(Color.grey);
			//	for (int i = 0; i < nDiv; i++)
			//	{
			//		angleRad_0 = (i * Mathf.PI * 2.0f) / (float)nDiv;
			//		angleRad_1 = ((i + 1) * Mathf.PI * 2.0f) / (float)nDiv;

			//		pos0 = posCenterGL + new Vector2(Mathf.Cos(angleRad_0) * radius_None, Mathf.Sin(angleRad_0) * radius_None);
			//		pos1 = posCenterGL + new Vector2(Mathf.Cos(angleRad_1) * radius_None, Mathf.Sin(angleRad_1) * radius_None);

			//		GL.Vertex(posCenterGL);
			//		GL.Vertex(pos0);
			//		GL.Vertex(pos1);
			//	}
			//}
			//else
			//{
			//	float radius_Small = 10.0f;
			//	float radius_Large = 16.0f;
			//	float radius_Small_Outline = 12.0f;
			//	float radius_Large_Outline = 18.0f;

			//	float ratioPerAngle = 1.0f / 20.0f;
			//	lastAngle = 0.0f;

			//	Color curColor = Color.black;
			//	float curRatio = 0.0f;
			//	bool curSelected = false;

			//	//float startAngle_In = 0.0f;
			//	float endAngle_Out = 0.0f;
			//	//float endAngle_In = 0.0f;

			//	//float angleRad_0_In = 0.0f;
			//	//float angleRad_1_In = 0.0f;

			//	int curDiv = 0;
			//	float itp_A = 0.0f;
			//	float itp_B = 0.0f;

			//	//float angleOverlapBias = 3.0f * Mathf.Deg2Rad;//부채꼴이 서로 겹치는 영역에서 구멍이 발생할 수 있다.
			//	//float angleInnerBias = 5.0f * Mathf.Deg2Rad;//내부의 색상은 2도씩 약간 적게 출력된다.

			//	lastAngle = -Mathf.PI * 0.5f;//-90도 = 12시 방향

			//	for (int iRig = 0; iRig < renderVertex._renderRigWeightParam._nParam; iRig++)
			//	{
			//		curRatio = renderVertex._renderRigWeightParam._ratios[iRig];
			//		curSelected = renderVertex._renderRigWeightParam._isSelecteds[iRig];

			//		if (isUseBoneColor)
			//		{
			//			curColor = renderVertex._renderRigWeightParam._colors[iRig];
			//			curColor.a = 1.0f;
			//		}
			//		else
			//		{
			//			if (curSelected)
			//			{
			//				curColor = GetWeightColor3(curRatio);
			//			}
			//			else
			//			{
			//				curColor = renderVertex._renderRigWeightParam._colors[iRig];
			//				curColor *= 0.5f;
			//				curColor.a = 1.0f;
			//			}
			//		}


			//		//startAngle_In = lastAngle - angleInnerBias;
			//		endAngle_Out = lastAngle - (Mathf.PI * 2.0f * curRatio);//감소하는 방향으로 만들자.
			//																//endAngle_In = endAngle_Out + angleInnerBias;

			//		//if(startAngle_In - endAngle_In < angleInnerBias)
			//		//{
			//		//	startAngle_In = lastAngle;
			//		//	endAngle_In = endAngle_Out;
			//		//}

			//		curDiv = (int)((lastAngle - endAngle_Out) / ratioPerAngle) + 1;

			//		for (int iSubDiv = 0; iSubDiv < curDiv; iSubDiv++)
			//		{
			//			itp_A = (float)iSubDiv / (float)curDiv;
			//			itp_B = (float)(iSubDiv + 1) / (float)curDiv;

			//			angleRad_0 = lastAngle * (1.0f - itp_A) + endAngle_Out * itp_A;
			//			angleRad_1 = lastAngle * (1.0f - itp_B) + endAngle_Out * itp_B;

			//			//angleRad_0_In = startAngle_In * (1.0f - itp_A) + endAngle_In * itp_A;
			//			//angleRad_1_In = startAngle_In * (1.0f - itp_B) + endAngle_In * itp_B;

			//			//if(iSubDiv == 0)
			//			//{
			//			//	angleRad_0_In -= angleInnerBias;
			//			//}
			//			//else if(iSubDiv == curDiv - 1)
			//			//{
			//			//	angleRad_1_In += angleInnerBias;
			//			//}
			//			//if (iSubDiv < curDiv - 1)
			//			//{
			//			//	angleRad_1_In -= angleOverlapBias;
			//			//}

			//			if (curSelected)
			//			{
			//				//선택된 경우
			//				//Outline 먼저
			//				GL.Color(colorOutline);

			//				pos0 = posCenterGL + new Vector2(Mathf.Cos(angleRad_0) * radius_Large_Outline, Mathf.Sin(angleRad_0) * radius_Large_Outline);
			//				pos1 = posCenterGL + new Vector2(Mathf.Cos(angleRad_1) * radius_Large_Outline, Mathf.Sin(angleRad_1) * radius_Large_Outline);

			//				GL.Color(curColor);
			//				GL.Vertex(posCenterGL);

			//				GL.Color(colorOutline);
			//				GL.Vertex(pos1);
			//				GL.Vertex(pos0);


			//				//본 색상 설정
			//				GL.Color(curColor);

			//				pos0 = posCenterGL + new Vector2(Mathf.Cos(angleRad_0) * radius_Large, Mathf.Sin(angleRad_0) * radius_Large);
			//				pos1 = posCenterGL + new Vector2(Mathf.Cos(angleRad_1) * radius_Large, Mathf.Sin(angleRad_1) * radius_Large);

			//				GL.Vertex(posCenterGL);
			//				GL.Vertex(pos1);
			//				GL.Vertex(pos0);

			//			}
			//			else
			//			{
			//				//선택되지 않은 경우
			//				//Outline 먼저


			//				pos0 = posCenterGL + new Vector2(Mathf.Cos(angleRad_0) * radius_Small_Outline, Mathf.Sin(angleRad_0) * radius_Small_Outline);
			//				pos1 = posCenterGL + new Vector2(Mathf.Cos(angleRad_1) * radius_Small_Outline, Mathf.Sin(angleRad_1) * radius_Small_Outline);

			//				GL.Color(curColor);
			//				GL.Vertex(posCenterGL);

			//				GL.Color(colorOutline);
			//				GL.Vertex(pos1);
			//				GL.Vertex(pos0);


			//				//본 색상 설정
			//				GL.Color(curColor);

			//				pos0 = posCenterGL + new Vector2(Mathf.Cos(angleRad_0) * radius_Small, Mathf.Sin(angleRad_0) * radius_Small);
			//				pos1 = posCenterGL + new Vector2(Mathf.Cos(angleRad_1) * radius_Small, Mathf.Sin(angleRad_1) * radius_Small);

			//				GL.Vertex(posCenterGL);
			//				GL.Vertex(pos1);
			//				GL.Vertex(pos0);

			//			}
			//		}

			//		lastAngle = endAngle_Out;
			//	}
			//}
			#endregion
		}


		private static void DrawRigCirclePart(Vector2 posCenterGL, 
												float radiusInner, float radiusOuter, 
												Color colorInner, Color colorOutline,
												float angleDeg_Prev, float angleDeg_Next)
		{
			//45, 135, 225, 315를 사이에 두면 꼭지점을 추가해야한다.
			int iPart_Prev = 0;
			int iPart_Next = 0;

			colorInner.a = 1.0f;

			Vector2 uv_Center = new Vector2(0.5f, 0.5f);

			if(angleDeg_Prev > angleDeg_Next)
			{
				float tmpAngle = angleDeg_Next;
				angleDeg_Next = angleDeg_Prev;
				angleDeg_Prev = tmpAngle;
			}

			if (angleDeg_Prev < 45.0f)			{ iPart_Prev = 0; }
			else if(angleDeg_Prev < 135.0f)		{ iPart_Prev = 1; }
			else if(angleDeg_Prev < 225.0f)		{ iPart_Prev = 2; }
			else if(angleDeg_Prev < 315.0f)		{ iPart_Prev = 3; }
			else								{ iPart_Prev = 4; }

			if (angleDeg_Next < 45.0f)			{ iPart_Next = 0; }
			else if(angleDeg_Next < 135.0f)		{ iPart_Next = 1; }
			else if(angleDeg_Next < 225.0f)		{ iPart_Next = 2; }
			else if(angleDeg_Next < 315.0f)		{ iPart_Next = 3; }
			else								{ iPart_Next = 4; }

			Vector2 pos_Inner_Prev = Vector2.zero;
			Vector2 pos_Inner_Next = Vector2.zero;
			Vector2 pos_Out_Prev = Vector2.zero;
			Vector2 pos_Out_Next = Vector2.zero;

			Vector2 uv_Prev = Vector2.zero;
			Vector2 uv_Next = Vector2.zero;

			float c2S_Ratio_Prev = 1.0f;
			float c2S_Ratio_Next = 1.0f;

			float angleRad_Prev = 0.0f;
			float angleRad_Next = 0.0f;

			//위치 좌표계 : LT > RB (CCW?)
			
			//UV 좌표계 : LB > RT

			while(true)
			{
				//만약 같은 대각-사분면에 위치한다면 > 바로 삼각형을 만들어서 그리기 > break;
				//그렇지 않다면 > next에 해당 꼭지점과 삼각형을 만들어서 그리기 > 그 꼭지점을 prev로 하여 한번 더 반복. 단, 사분면 증가
				if(iPart_Prev == iPart_Next)
				{
					angleRad_Prev = angleDeg_Prev * Mathf.Deg2Rad;
					angleRad_Next = angleDeg_Next * Mathf.Deg2Rad;

					switch (iPart_Prev)
					{
						
						//위/아래 : Y 고정으로 비율 계산
						case 0:
						case 4:
						case 2:
							c2S_Ratio_Prev = 1.0f / Mathf.Abs(Mathf.Cos(angleRad_Prev));
							c2S_Ratio_Next = 1.0f / Mathf.Abs(Mathf.Cos(angleRad_Next));
							break;

						//좌/우 : X 고정으로 비율 계산
						case 1:
						case 3:
							c2S_Ratio_Prev = 1.0f / Mathf.Abs(Mathf.Sin(angleRad_Prev));
							c2S_Ratio_Next = 1.0f / Mathf.Abs(Mathf.Sin(angleRad_Next));
							break;
					}
					
					//Sin, Cos을 반대로
					pos_Inner_Prev.x = posCenterGL.x + (Mathf.Sin(angleRad_Prev) * radiusInner * c2S_Ratio_Prev);
					pos_Inner_Prev.y = posCenterGL.y + (-Mathf.Cos(angleRad_Prev) * radiusInner * c2S_Ratio_Prev);

					pos_Inner_Next.x = posCenterGL.x + (Mathf.Sin(angleRad_Next) * radiusInner * c2S_Ratio_Next);
					pos_Inner_Next.y = posCenterGL.y + (-Mathf.Cos(angleRad_Next) * radiusInner * c2S_Ratio_Next);

					pos_Out_Prev.x = posCenterGL.x + (Mathf.Sin(angleRad_Prev) * radiusOuter * c2S_Ratio_Prev);
					pos_Out_Prev.y = posCenterGL.y + (-Mathf.Cos(angleRad_Prev) * radiusOuter * c2S_Ratio_Prev);

					pos_Out_Next.x = posCenterGL.x + (Mathf.Sin(angleRad_Next) * radiusOuter * c2S_Ratio_Next);
					pos_Out_Next.y = posCenterGL.y + (-Mathf.Cos(angleRad_Next) * radiusOuter * c2S_Ratio_Next);

					uv_Prev.x = Mathf.Cos(angleRad_Prev) * c2S_Ratio_Prev;
					uv_Prev.y = Mathf.Sin(angleRad_Prev) * c2S_Ratio_Prev;

					uv_Next.x = Mathf.Cos(angleRad_Next) * c2S_Ratio_Next;
					uv_Next.y = Mathf.Sin(angleRad_Next) * c2S_Ratio_Next;

					uv_Prev.x = (uv_Prev.x * 0.5f) + 0.5f;
					uv_Prev.y = (uv_Prev.y * -0.5f) + 0.5f;

					uv_Next.x = (uv_Next.x * 0.5f) + 0.5f;
					uv_Next.y = (uv_Next.y * -0.5f) + 0.5f;

					//바깥쪽
					GL.Color(colorOutline);
					
					GL.TexCoord(uv_Center);	GL.Vertex(posCenterGL);
					GL.TexCoord(uv_Prev);	GL.Vertex(pos_Out_Prev);
					GL.TexCoord(uv_Next);	GL.Vertex(pos_Out_Next);
					

					//안쪽
					GL.Color(colorInner);
					
					GL.TexCoord(uv_Center);	GL.Vertex(posCenterGL);
					GL.TexCoord(uv_Prev);	GL.Vertex(pos_Inner_Prev);
					GL.TexCoord(uv_Next);	GL.Vertex(pos_Inner_Next);
					

					//종료!
					break;
				}
				else
				{
					//일단 Prev의 각도부터
					angleRad_Prev = angleDeg_Prev * Mathf.Deg2Rad;

					switch (iPart_Prev)
					{
						case 0:case 4:case 2:	c2S_Ratio_Prev = 1.0f / Mathf.Abs(Mathf.Cos(angleRad_Prev));	break;
						case 1:case 3:			c2S_Ratio_Prev = 1.0f / Mathf.Abs(Mathf.Sin(angleRad_Prev));	break;
					}

					//Sin, Cos를 반대로
					pos_Inner_Prev.x = posCenterGL.x + (Mathf.Sin(angleRad_Prev) * radiusInner * c2S_Ratio_Prev);
					pos_Inner_Prev.y = posCenterGL.y + (-Mathf.Cos(angleRad_Prev) * radiusInner * c2S_Ratio_Prev);

					pos_Out_Prev.x = posCenterGL.x + (Mathf.Sin(angleRad_Prev) * radiusOuter * c2S_Ratio_Prev);
					pos_Out_Prev.y = posCenterGL.y + (-Mathf.Cos(angleRad_Prev) * radiusOuter * c2S_Ratio_Prev);

					uv_Prev.x = Mathf.Cos(angleRad_Prev) * c2S_Ratio_Prev;
					uv_Prev.y = Mathf.Sin(angleRad_Prev) * c2S_Ratio_Prev;

					//Next의 각도는 꼭지점이다.
					switch (iPart_Prev)
					{
						case 0:
						case 4:
							//45도보다 작은 경우 (Pos : 1, -1 / UV : 1, 1)
							angleRad_Next = 45.0f * Mathf.Deg2Rad;
							break;

						case 1:
							//135도보다 작은 경우 (Pos : 1, 1 / UV : 1, 0)
							angleRad_Next = 135.0f * Mathf.Deg2Rad;
							break;

						case 2:
							//225도보다 작은 경우 (Pos : -1, 1 / UV : 0, 0)
							angleRad_Next = 225.0f * Mathf.Deg2Rad;
							break;

						case 3:
							//315도보다 작은 경우 (Pos : -1, -1 / UV : 0, 1)
							angleRad_Next = 315.0f * Mathf.Deg2Rad;
							break;

					}

					switch (iPart_Prev)
					{
						case 0:case 4:case 2:	c2S_Ratio_Next = 1.0f / Mathf.Abs(Mathf.Cos(angleRad_Next));	break;
						case 1:case 3:			c2S_Ratio_Next = 1.0f / Mathf.Abs(Mathf.Sin(angleRad_Next));	break;
					}
					
					//Sin, Cos를 반대로
					pos_Inner_Next.x = posCenterGL.x + (Mathf.Sin(angleRad_Next) * radiusInner * c2S_Ratio_Next);
					pos_Inner_Next.y = posCenterGL.y + (-Mathf.Cos(angleRad_Next) * radiusInner * c2S_Ratio_Next);

					pos_Out_Next.x = posCenterGL.x + (Mathf.Sin(angleRad_Next) * radiusOuter * c2S_Ratio_Next);
					pos_Out_Next.y = posCenterGL.y + (-Mathf.Cos(angleRad_Next) * radiusOuter * c2S_Ratio_Next);
					
					uv_Next.x = Mathf.Cos(angleRad_Next) * c2S_Ratio_Next;
					uv_Next.y = Mathf.Sin(angleRad_Next) * c2S_Ratio_Next;

					uv_Prev.x = (uv_Prev.x * 0.5f) + 0.5f;
					uv_Prev.y = (uv_Prev.y * -0.5f) + 0.5f;

					uv_Next.x = (uv_Next.x * 0.5f) + 0.5f;
					uv_Next.y = (uv_Next.y * -0.5f) + 0.5f;

					//바깥쪽
					GL.Color(colorOutline);
					
					GL.TexCoord(uv_Center);	GL.Vertex(posCenterGL);
					GL.TexCoord(uv_Prev);	GL.Vertex(pos_Out_Prev);
					GL.TexCoord(uv_Next);	GL.Vertex(pos_Out_Next);
					

					//안쪽
					GL.Color(colorInner);
					
					GL.TexCoord(uv_Center);	GL.Vertex(posCenterGL);
					GL.TexCoord(uv_Prev);	GL.Vertex(pos_Inner_Prev);
					GL.TexCoord(uv_Next);	GL.Vertex(pos_Inner_Next);
					

					//일부의 렌더링이 끝나고 Prev 꼭지점을 옮긴다.
					switch (iPart_Prev)
					{
						case 0:
							angleDeg_Prev = 45.0f;
							iPart_Prev = 1;
							break;

						case 1:
							angleDeg_Prev = 135.0f;
							iPart_Prev = 2;
							break;

						case 2:
							angleDeg_Prev = 225.0f;
							iPart_Prev = 3;
							break;

						case 3:
							angleDeg_Prev = 315.0f;
							iPart_Prev = 4;
							break;

						case 4:
							return;
					}
				}
			}
		}


		//------------------------------------------------------------------------------------------------
		// Draw Transform Border Form of Render Unit
		//------------------------------------------------------------------------------------------------
		public static void DrawTransformBorderFormOfRenderUnit(Color lineColor, float posL, float posR, float posT, float posB, apMatrix3x3 worldMatrix)
		{
			float marginOffset = 10;
			posL -= marginOffset;
			posR += marginOffset;
			posT += marginOffset;
			posB -= marginOffset;

			//Vector3 pos3W_LT = worldMatrix.MultiplyPoint3x4(new Vector3(posL, posT, 0));
			//Vector3 pos3W_RT = worldMatrix.MultiplyPoint3x4(new Vector3(posR, posT, 0));
			//Vector3 pos3W_LB = worldMatrix.MultiplyPoint3x4(new Vector3(posL, posB, 0));
			//Vector3 pos3W_RB = worldMatrix.MultiplyPoint3x4(new Vector3(posR, posB, 0));

			//Vector2 posW_LT = new Vector2(pos3W_LT.x, pos3W_LT.y);
			//Vector2 posW_RT = new Vector2(pos3W_RT.x, pos3W_RT.y);
			//Vector2 posW_LB = new Vector2(pos3W_LB.x, pos3W_LB.y);
			//Vector2 posW_RB = new Vector2(pos3W_RB.x, pos3W_RB.y);


			Vector2 posW_LT = worldMatrix.MultiplyPoint(new Vector2(posL, posT));
			Vector2 posW_RT = worldMatrix.MultiplyPoint(new Vector2(posR, posT));
			Vector2 posW_LB = worldMatrix.MultiplyPoint(new Vector2(posL, posB));
			Vector2 posW_RB = worldMatrix.MultiplyPoint(new Vector2(posR, posB));


			float tfFormLineLength = 32.0f;

			_matBatch.SetPass_Color();
			_matBatch.SetClippingSize(_glScreenClippingSize);
			GL.Begin(GL.LINES);

			DrawLine(posW_LT, GetUnitLineEndPoint(posW_LT, posW_RT, tfFormLineLength), lineColor, false);
			DrawLine(posW_RT, GetUnitLineEndPoint(posW_RT, posW_RB, tfFormLineLength), lineColor, false);
			DrawLine(posW_RB, GetUnitLineEndPoint(posW_RB, posW_LB, tfFormLineLength), lineColor, false);
			DrawLine(posW_LB, GetUnitLineEndPoint(posW_LB, posW_LT, tfFormLineLength), lineColor, false);

			DrawLine(posW_LT, GetUnitLineEndPoint(posW_LT, posW_LB, tfFormLineLength), lineColor, false);
			DrawLine(posW_LB, GetUnitLineEndPoint(posW_LB, posW_RB, tfFormLineLength), lineColor, false);
			DrawLine(posW_RB, GetUnitLineEndPoint(posW_RB, posW_RT, tfFormLineLength), lineColor, false);
			DrawLine(posW_RT, GetUnitLineEndPoint(posW_RT, posW_LT, tfFormLineLength), lineColor, false);

			GL.End();
		}

		private static Vector2 GetUnitLineEndPoint(Vector2 startPos, Vector2 endPos, float maxLength)
		{
			Vector2 dir = endPos - startPos;
			if (dir.sqrMagnitude <= maxLength * maxLength)
			{
				return endPos;
			}
			return startPos + dir.normalized * maxLength;
		}


		//------------------------------------------------------------------------------------------------
		// Draw Bone
		//------------------------------------------------------------------------------------------------
		//private static Color _boneColor_Selected = new Color(1.0f, 0.1f, 0.0f, 1.0f);

		//public static void DrawBone(apBone bone, apSelection select)
		public static void DrawBone(apBone bone, bool isDrawOutline, bool isBoneIKUsing, bool isUseBoneToneColor, bool isAvailable)
		{
			if (bone == null)
			{
				return;
			}

			Color boneColor = bone._color;
			if(isUseBoneToneColor)
			{
				boneColor = _toneBoneColor;
			}
			else if(!isAvailable)
			{
				//추가 : 사용 불가능하다면 회색으로 보인다.
				boneColor = Color.gray;
			}

			Color boneOutlineColor = boneColor * 0.5f;
			boneOutlineColor.a = 1.0f;

			
			apMatrix worldMatrix = null;
			Vector2 posW_Start = Vector2.zero;

			bool isHelperBone = bone._shapeHelper;
			Vector2 posGL_Start = Vector2.zero;
			Vector2 posGL_Mid1 = Vector2.zero;
			Vector2 posGL_Mid2 = Vector2.zero;
			Vector2 posGL_End1 = Vector2.zero;
			Vector2 posGL_End2 = Vector2.zero;

			if(isBoneIKUsing)
			{
				//IK 값이 포함될 때
				worldMatrix = bone._worldMatrix_IK;
				posW_Start = worldMatrix._pos;

				if(!isUseBoneToneColor)
				{
					posGL_Start = apGL.World2GL(posW_Start);
					posGL_Mid1 = apGL.World2GL(bone._shapePoint_Mid1_IK);
					posGL_Mid2 = apGL.World2GL(bone._shapePoint_Mid2_IK);
					posGL_End1 = apGL.World2GL(bone._shapePoint_End1_IK);
					posGL_End2 = apGL.World2GL(bone._shapePoint_End2_IK);
				}
				else
				{
					//Onion Skin 전용 좌표 계산
					Vector2 deltaOnionPos = apGL._tonePosOffset * apGL._zoom;
					//Vector2 deltaOnionPos = apGL._tonePosOffset * 0.001f;
					//Vector2 deltaOnionPos = apGL._tonePosOffset;
					
					posGL_Start = apGL.World2GL(posW_Start) + deltaOnionPos;
					posGL_Mid1 = apGL.World2GL(bone._shapePoint_Mid1_IK) + deltaOnionPos;
					posGL_Mid2 = apGL.World2GL(bone._shapePoint_Mid2_IK) + deltaOnionPos;
					posGL_End1 = apGL.World2GL(bone._shapePoint_End1_IK) + deltaOnionPos;
					posGL_End2 = apGL.World2GL(bone._shapePoint_End2_IK) + deltaOnionPos;
				}
				
			}
			else
			{
				worldMatrix = bone._worldMatrix;
				posW_Start = worldMatrix._pos;

				if (!isUseBoneToneColor)
				{
					posGL_Start = apGL.World2GL(posW_Start);
					posGL_Mid1 = apGL.World2GL(bone._shapePoint_Mid1);
					posGL_Mid2 = apGL.World2GL(bone._shapePoint_Mid2);
					posGL_End1 = apGL.World2GL(bone._shapePoint_End1);
					posGL_End2 = apGL.World2GL(bone._shapePoint_End2);
				}
				else
				{
					//Onion Skin 전용 좌표 계산
					Vector2 deltaOnionPos = apGL._tonePosOffset * apGL._zoom;
					//Vector2 deltaOnionPos = apGL._tonePosOffset * 0.001f;
					//Vector2 deltaOnionPos = apGL._tonePosOffset;

					posGL_Start = apGL.World2GL(posW_Start) + deltaOnionPos;
					posGL_Mid1 = apGL.World2GL(bone._shapePoint_Mid1) + deltaOnionPos;
					posGL_Mid2 = apGL.World2GL(bone._shapePoint_Mid2) + deltaOnionPos;
					posGL_End1 = apGL.World2GL(bone._shapePoint_End1) + deltaOnionPos;
					posGL_End2 = apGL.World2GL(bone._shapePoint_End2) + deltaOnionPos;
				}
				
			}

			

			float orgSize = 10.0f * Zoom;
			Vector3 orgPos_Up = new Vector3(posGL_Start.x, posGL_Start.y + orgSize, 0);
			Vector3 orgPos_Left = new Vector3(posGL_Start.x - orgSize, posGL_Start.y, 0);
			Vector3 orgPos_Down = new Vector3(posGL_Start.x, posGL_Start.y - orgSize, 0);
			Vector3 orgPos_Right = new Vector3(posGL_Start.x + orgSize, posGL_Start.y, 0);

			if (!isDrawOutline)
			{
				//1. 전부다 그릴때
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				if (!isHelperBone)//<헬퍼가 아닐때
				{
					GL.Begin(GL.TRIANGLES);

					GL.Color(boneColor);

					//1. 사다리꼴 모양을 먼저 그리자
					//    [End1]    [End2]
					//
					//
					//
					//[Mid1]            [Mid2]
					//        [Start]

					//1) Start - Mid1 - End1
					//2) Start - Mid2 - End2
					//3) Start - End1 - End2

					//1) Start - Mid1 - End1
					GL.Vertex(posGL_Start);
					GL.Vertex(posGL_Mid1);
					GL.Vertex(posGL_End1);
					GL.Vertex(posGL_Start);
					GL.Vertex(posGL_End1);
					GL.Vertex(posGL_Mid1);

					//2) Start - Mid2 - End2
					GL.Vertex(posGL_Start);
					GL.Vertex(posGL_Mid2);
					GL.Vertex(posGL_End2);
					GL.Vertex(posGL_Start);
					GL.Vertex(posGL_End2);
					GL.Vertex(posGL_Mid2);

					//3) Start - End1 - End2 (taper가 100 미만일 때)
					if (bone._shapeTaper < 100)
					{
						GL.Vertex(posGL_Start);
						GL.Vertex(posGL_End1);
						GL.Vertex(posGL_End2);
						GL.Vertex(posGL_Start);
						GL.Vertex(posGL_End2);
						GL.Vertex(posGL_End1);
					}
					GL.End();

					GL.Begin(GL.LINES);

					DrawLineGL(posGL_Start, posGL_Mid1, boneOutlineColor, false);
					DrawLineGL(posGL_Mid1, posGL_End1, boneOutlineColor, false);
					DrawLineGL(posGL_End1, posGL_End2, boneOutlineColor, false);
					DrawLineGL(posGL_End2, posGL_Mid2, boneOutlineColor, false);
					DrawLineGL(posGL_Mid2, posGL_Start, boneOutlineColor, false);

					GL.End();
				}

				GL.Begin(GL.TRIANGLES);

				GL.Color(boneColor);

				//2. 원점 부분은 다각형 형태로 다시 그려주자
				//다이아몬드 형태로..



				//       Up
				// Left  |   Right
				//      Down

				GL.Vertex(orgPos_Up);
				GL.Vertex(orgPos_Left);
				GL.Vertex(orgPos_Down);
				GL.Vertex(orgPos_Up);
				GL.Vertex(orgPos_Down);
				GL.Vertex(orgPos_Left);

				GL.Vertex(orgPos_Up);
				GL.Vertex(orgPos_Right);
				GL.Vertex(orgPos_Down);
				GL.Vertex(orgPos_Up);
				GL.Vertex(orgPos_Down);
				GL.Vertex(orgPos_Right);

				GL.End();

				GL.Begin(GL.LINES);

				DrawLineGL(orgPos_Up, orgPos_Left, boneOutlineColor, false);
				DrawLineGL(orgPos_Left, orgPos_Down, boneOutlineColor, false);
				DrawLineGL(orgPos_Down, orgPos_Right, boneOutlineColor, false);
				DrawLineGL(orgPos_Right, orgPos_Up, boneOutlineColor, false);

				GL.End();
			}
			else
			{
				_matBatch.SetPass_Color();
				_matBatch.SetClippingSize(_glScreenClippingSize);

				//2. Outline만 그릴때
				//1> 헬퍼가 아니라면 사다리꼴만
				//2> 헬퍼라면 다이아몬드만
				GL.Begin(GL.LINES);
				if (!isHelperBone)
				{
					DrawLineGL(posGL_Start, posGL_Mid1, boneColor, false);
					DrawLineGL(posGL_Mid1, posGL_End1, boneColor, false);
					DrawLineGL(posGL_End1, posGL_End2, boneColor, false);
					DrawLineGL(posGL_End2, posGL_Mid2, boneColor, false);
					DrawLineGL(posGL_Mid2, posGL_Start, boneColor, false);
				}
				else
				{
					DrawLineGL(orgPos_Up, orgPos_Left, boneColor, false);
					DrawLineGL(orgPos_Left, orgPos_Down, boneColor, false);
					DrawLineGL(orgPos_Down, orgPos_Right, boneColor, false);
					DrawLineGL(orgPos_Right, orgPos_Up, boneColor, false);
				}

				GL.End();
			}

			//if (isBoneIKUsing)
			//{
			//	//계산 전의 결과를 확인하자
			//	Vector2 dir = bone._shapePoint_End1 - bone._worldMatrix._pos;
			//	DrawAnimatedLine(bone._worldMatrix_IK._pos, 
			//		bone._worldMatrix_IK._pos + dir, Color.yellow, true);

			//	if(bone._isIKCalculated_Controlled)
			//	{
			//		//DrawText(bone._worldMatrix._angleDeg + " > " + bone._worldMatrix_IK._angleDeg , bone._shapePoint_End1 + new Vector2(0, 25), 100, Color.white);
			//		DrawText(bone._IKRequestAngleResult_Controlled + " (" + bone._IKRequestWeight_Controlled + ")" , 
			//			bone._worldMatrix_IK._pos + dir + new Vector2(0, 10), 100, Color.green);
			//	}
			//}
		}

		public static void DrawSelectedBone(apBone bone, bool isMainSelect = true, bool isBoneIKUsing = false)
		{
			if (bone == null)
			{
				return;
			}

			apMatrix worldMatrix = null;
			Vector2 posW_Start = Vector2.zero;

			bool isHelperBone = bone._shapeHelper;
			Vector2 posGL_Start = Vector2.zero;
			Vector2 posGL_Mid1 = Vector2.zero;
			Vector2 posGL_Mid2 = Vector2.zero;
			Vector2 posGL_End1 = Vector2.zero;
			Vector2 posGL_End2 = Vector2.zero;
			
			if(isBoneIKUsing)
			{
				worldMatrix = bone._worldMatrix_IK;
				posW_Start = worldMatrix._pos;

				posGL_Start = apGL.World2GL(posW_Start);
				posGL_Mid1 = apGL.World2GL(bone._shapePoint_Mid1_IK);
				posGL_Mid2 = apGL.World2GL(bone._shapePoint_Mid2_IK);
				posGL_End1 = apGL.World2GL(bone._shapePoint_End1_IK);
				posGL_End2 = apGL.World2GL(bone._shapePoint_End2_IK);
			}
			else
			{
				worldMatrix = bone._worldMatrix;
				posW_Start = worldMatrix._pos;
			
				posGL_Start = apGL.World2GL(posW_Start);
				posGL_Mid1 = apGL.World2GL(bone._shapePoint_Mid1);
				posGL_Mid2 = apGL.World2GL(bone._shapePoint_Mid2);
				posGL_End1 = apGL.World2GL(bone._shapePoint_End1);
				posGL_End2 = apGL.World2GL(bone._shapePoint_End2);
			}



			//2. 원점 부분은 다각형 형태로 다시 그려주자
			//다이아몬드 형태로..
			float orgSize = 10.0f * Zoom;
			Vector3 orgPos_Up = new Vector3(posGL_Start.x, posGL_Start.y + orgSize, 0);
			Vector3 orgPos_Left = new Vector3(posGL_Start.x - orgSize, posGL_Start.y, 0);
			Vector3 orgPos_Down = new Vector3(posGL_Start.x, posGL_Start.y - orgSize, 0);
			Vector3 orgPos_Right = new Vector3(posGL_Start.x + orgSize, posGL_Start.y, 0);

			_matBatch.SetPass_Color();
			_matBatch.SetClippingSize(_glScreenClippingSize);

			GL.Begin(GL.TRIANGLES);
			Color lineColor = _lineColor_BoneOutline;
			if (!isMainSelect)
			{
				lineColor = _lineColor_BoneOutlineRollOver;
			}
			float lineThickness = 8.0f;

			if (!isHelperBone)
			{
				//헬퍼가 아닐때
				//1. 사다리꼴 모양을 먼저 그리자
				//    [End1]    [End2]
				//
				//
				//
				//[Mid1]            [Mid2]
				//        [Start]

				//1) Start - Mid1 - End1
				//2) Start - Mid2 - End2
				//3) Start - End1 - End2

				//1) Start - Mid1 - End1
				

				
				DrawBoldLineGL(posGL_Start, posGL_Mid1, lineThickness, lineColor, false);
				DrawBoldLineGL(posGL_Mid1, posGL_End1, lineThickness, lineColor, false);

				if (bone._shapeTaper < 100)
				{
					DrawBoldLineGL(posGL_End1, posGL_End2, lineThickness, lineColor, false);
				}
				DrawBoldLineGL(posGL_End2, posGL_Mid2, lineThickness, lineColor, false);
				DrawBoldLineGL(posGL_Mid2, posGL_Start, lineThickness, lineColor, false);
			}
			DrawBoldLineGL(orgPos_Up, orgPos_Left, lineThickness, lineColor, false);
			DrawBoldLineGL(orgPos_Left, orgPos_Down, lineThickness, lineColor, false);
			DrawBoldLineGL(orgPos_Down, orgPos_Right, lineThickness, lineColor, false);
			DrawBoldLineGL(orgPos_Right, orgPos_Up, lineThickness, lineColor, false);

			GL.End();

			//추가 : IK 속성이 있는 경우, GUI에 표시하자
			if (bone._IKTargetBone != null)
			{
				Vector2 IKTargetPos = Vector2.zero;
				if (isBoneIKUsing)
				{
					IKTargetPos = World2GL(bone._IKTargetBone._worldMatrix_IK._pos);
				}
				else
				{
					IKTargetPos = World2GL(bone._IKTargetBone._worldMatrix._pos);
				}
				DrawAnimatedLineGL(posGL_Start, IKTargetPos, Color.magenta, true);
			}

			if (bone._IKHeaderBone != null)
			{
				Vector2 IKHeadPos = Vector2.zero;
				if (isBoneIKUsing)
				{
					IKHeadPos = World2GL(bone._IKHeaderBone._worldMatrix_IK._pos);
				}
				else
				{
					IKHeadPos = World2GL(bone._IKHeaderBone._worldMatrix._pos);
				}
				DrawAnimatedLineGL(IKHeadPos, posGL_Start, Color.magenta, true);
			}

			if(bone._IKController._controllerType != apBoneIKController.CONTROLLER_TYPE.None)
			{
				if(bone._IKController._effectorBone != null)
				{
					Color lineColorIK = Color.yellow;
					if(bone._IKController._controllerType == apBoneIKController.CONTROLLER_TYPE.LookAt)
					{
						lineColorIK = Color.cyan;
					}
					Vector2 effectorPos = Vector2.zero;
					if(isBoneIKUsing)
					{
						effectorPos = World2GL(bone._IKController._effectorBone._worldMatrix_IK._pos);
					}
					else
					{
						effectorPos = World2GL(bone._IKController._effectorBone._worldMatrix._pos);
					}
					DrawAnimatedLineGL(posGL_Start, effectorPos, lineColorIK, true);
				}
			}
		}


		public static void DrawSelectedBonePost(apBone bone, bool isBoneIKUsing)
		{
			if (bone == null)
			{
				return;
			}

			if (bone._isIKAngleRange 
				&& bone._optionIK != apBone.OPTION_IK.Disabled
				//&& bone._parentBone != null
				)
			{
				//IK Angle 범위를 그려준다.
				//apMatrix worldMatrix = bone._parentBone._worldMatrix;

				Vector2 posW_Start = Vector2.zero;
				apMatrix worldMatrix = null;
				if(isBoneIKUsing)
				{
					worldMatrix = bone._worldMatrix_IK;
				}
				else
				{
					worldMatrix = bone._worldMatrix;
				}
				posW_Start = worldMatrix._pos;

				Vector2 unitVector = new Vector2(0, 1);
				if (bone._parentBone != null)
				{
					if(isBoneIKUsing)
					{
						unitVector = bone._parentBone._worldMatrix_IK.MtrxOnlyRotation.MultiplyPoint(new Vector2(0, 1));
					}
					else
					{
						unitVector = bone._parentBone._worldMatrix.MtrxOnlyRotation.MultiplyPoint(new Vector2(0, 1));
					}
					
				}
				
				
				float defaultAngle = bone._defaultMatrix._angleDeg;
				if(bone._renderUnit != null)
				{
					defaultAngle += bone._renderUnit.WorldMatrixWrap._angleDeg;
				}
				Vector2 unitVector_Lower = apMatrix3x3.TRS(Vector2.zero, defaultAngle + bone._IKAngleRange_Lower, Vector2.one).MultiplyPoint(unitVector);
				Vector2 unitVector_Upper = apMatrix3x3.TRS(Vector2.zero, defaultAngle + bone._IKAngleRange_Upper, Vector2.one).MultiplyPoint(unitVector);
				Vector2 unitVector_Pref = apMatrix3x3.TRS(Vector2.zero, defaultAngle + bone._IKAnglePreferred, Vector2.one).MultiplyPoint(unitVector);

				unitVector_Lower.Normalize();
				unitVector_Upper.Normalize();
				unitVector_Pref.Normalize();

				unitVector_Lower *= bone._shapeLength * worldMatrix._scale.y * 1.2f;
				unitVector_Upper *= bone._shapeLength * worldMatrix._scale.y * 1.2f;
				unitVector_Pref *= bone._shapeLength * worldMatrix._scale.y * 1.5f;

				DrawBoldLine(posW_Start, posW_Start + new Vector2(unitVector_Lower.x, unitVector_Lower.y), 3, Color.magenta, true);
				DrawBoldLine(posW_Start, posW_Start + new Vector2(unitVector_Upper.x, unitVector_Upper.y), 3, Color.magenta, true);
				DrawBoldLine(posW_Start, posW_Start + new Vector2(unitVector_Pref.x, unitVector_Pref.y), 3, Color.green, true);


			}

			//if(bone._isIKtargetDebug)
			//{
			//	DrawBox(bone._calculatedIKTargetPosDebug, 30, 30, new Color(1.0f, 0.0f, 1.0f, 1.0f), false);
			//	int nBosDebug = bone._calculatedIKBonePosDebug.Count;
			//	for (int i = 0; i < nBosDebug; i++)
			//	{
			//		Color debugColor = (new Color(0.0f, 1.0f, 0.0f, 1.0f) * ((nBosDebug - 1)- i) + new Color(0.0f, 0.0f, 1.0f, 1.0f) * i) / (float)(nBosDebug - 1);

			//		DrawBox(bone._calculatedIKBonePosDebug[i], 20 + i * 5, 20 + i * 5, debugColor, false);
			//	}

			//}
		}



		public static void DrawBoneOutline(apBone bone, Color outlineColor, bool isBoneIKUsing)
		{
			if (bone == null)
			{
				return;
			}

			
			apMatrix worldMatrix = bone._worldMatrix;
			Vector2 posW_Start = worldMatrix._pos;
			bool isHelperBone = bone._shapeHelper;

			Vector2 posGL_Start = Vector2.zero;
			Vector2 posGL_Mid1 = Vector2.zero;
			Vector2 posGL_Mid2 = Vector2.zero;
			Vector2 posGL_End1 = Vector2.zero;
			Vector2 posGL_End2 = Vector2.zero;

			if(isBoneIKUsing)
			{
				worldMatrix = bone._worldMatrix_IK;
				posW_Start = worldMatrix._pos;

				posGL_Start = apGL.World2GL(posW_Start);
				posGL_Mid1 = apGL.World2GL(bone._shapePoint_Mid1_IK);
				posGL_Mid2 = apGL.World2GL(bone._shapePoint_Mid2_IK);
				posGL_End1 = apGL.World2GL(bone._shapePoint_End1_IK);
				posGL_End2 = apGL.World2GL(bone._shapePoint_End2_IK);
			}
			else
			{
				worldMatrix = bone._worldMatrix;
				posW_Start = worldMatrix._pos;

				posGL_Start = apGL.World2GL(posW_Start);
				posGL_Mid1 = apGL.World2GL(bone._shapePoint_Mid1);
				posGL_Mid2 = apGL.World2GL(bone._shapePoint_Mid2);
				posGL_End1 = apGL.World2GL(bone._shapePoint_End1);
				posGL_End2 = apGL.World2GL(bone._shapePoint_End2);
			}

			float orgSize = 10.0f * Zoom;
			Vector3 orgPos_Up = new Vector3(posGL_Start.x, posGL_Start.y + orgSize, 0);
			Vector3 orgPos_Left = new Vector3(posGL_Start.x - orgSize, posGL_Start.y, 0);
			Vector3 orgPos_Down = new Vector3(posGL_Start.x, posGL_Start.y - orgSize, 0);
			Vector3 orgPos_Right = new Vector3(posGL_Start.x + orgSize, posGL_Start.y, 0);

			_matBatch.SetPass_Color();
			_matBatch.SetClippingSize(_glScreenClippingSize);

			//2. Outline만 그릴때
			//1> 헬퍼가 아니라면 사다리꼴만
			//2> 헬퍼라면 다이아몬드만
			float width = 3.0f;
			GL.Begin(GL.TRIANGLES);
			if (!isHelperBone)
			{
				DrawBoldLineGL(posGL_Start, posGL_Mid1, width, outlineColor, false);
				DrawBoldLineGL(posGL_Mid1, posGL_End1, width, outlineColor, false);
				if (Mathf.Abs(posGL_End1.x - posGL_End2.x) > 2f
					&& Mathf.Abs(posGL_End1.y - posGL_End2.y) > 2f)
				{
					DrawBoldLineGL(posGL_End1, posGL_End2, width, outlineColor, false);
				}
				DrawBoldLineGL(posGL_End2, posGL_Mid2, width, outlineColor, false);
				DrawBoldLineGL(posGL_Mid2, posGL_Start, width, outlineColor, false);
			}
			else
			{
				DrawBoldLineGL(orgPos_Up, orgPos_Left, width, outlineColor, false);
				DrawBoldLineGL(orgPos_Left, orgPos_Down, width, outlineColor, false);
				DrawBoldLineGL(orgPos_Down, orgPos_Right, width, outlineColor, false);
				DrawBoldLineGL(orgPos_Right, orgPos_Up, width, outlineColor, false);
			}

			GL.End();


		}


		//------------------------------------------------------------------------------------------------
		// Draw Grid
		//------------------------------------------------------------------------------------------------
		public static void DrawGrid(Color lineColor_Center, Color lineColor)
		{
			int pixelSize = 50;

			//Color lineColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
			//Color lineColor_Center = new Color(0.7f, 0.7f, 0.3f, 1.0f);

			if (_zoom < 0.2f + 0.05f)
			{
				pixelSize = 200;
				lineColor.a = 0.4f;
			}
			else if (_zoom < 0.5f + 0.05f)
			{
				pixelSize = 100;
				lineColor.a = 0.7f;
			}

			//Vector2 centerPos = World2GL(Vector2.zero);

			//Screen의 Width, Height에 해당하는 극점을 찾자
			//Vector2 pos_LT = GL2World(new Vector2(0, 0));
			//Vector2 pos_RB = GL2World(new Vector2(_windowWidth, _windowHeight));
			Vector2 pos_LT = GL2World(new Vector2(-500, -500));
			Vector2 pos_RB = GL2World(new Vector2(_windowWidth + 500, _windowHeight + 500));

			float yWorld_Max = Mathf.Max(pos_LT.y, pos_RB.y) + 100;
			float yWorld_Min = Mathf.Min(pos_LT.y, pos_RB.y) - 200;
			float xWorld_Max = Mathf.Max(pos_LT.x, pos_RB.x);
			float xWorld_Min = Mathf.Min(pos_LT.x, pos_RB.x);

			// 가로줄 먼저 (+- Y로 움직임)
			Vector2 curPos = Vector2.zero;
			//Vector2 curPosGL = Vector2.zero;
			Vector2 posA, posB;

			curPos.y = (int)(yWorld_Min / pixelSize) * pixelSize;

			// + Y 방향 (아래)
			while (true)
			{
				//curPosGL = World2GL(curPos);

				//if(curPosGL.y < 0 || curPosGL.y > _windowHeight)
				//{
				//	break;
				//}
				if (curPos.y > yWorld_Max)
				{
					break;
				}


				posA.x = pos_LT.x;
				posA.y = curPos.y;

				posB.x = pos_RB.x;
				posB.y = curPos.y;

				DrawLine(posA, posB, lineColor);

				curPos.y += pixelSize;
			}


			curPos = Vector2.zero;
			curPos.x = (int)(xWorld_Min / pixelSize) * pixelSize;

			// + X 방향 (오른쪽)
			while (true)
			{
				//curPosGL = World2GL(curPos);

				//if(curPosGL.x < 0 || curPosGL.x > _windowWidth)
				//{
				//	break;
				//}
				if (curPos.x > xWorld_Max)
				{
					break;
				}

				posA.y = pos_LT.y;
				posA.x = curPos.x;

				posB.y = pos_RB.y;
				posB.x = curPos.x;

				DrawLine(posA, posB, lineColor);

				curPos.x += pixelSize;
			}

			//중앙선

			curPos = Vector2.zero;

			posA.x = pos_LT.x;
			posA.y = curPos.y;

			posB.x = pos_RB.x;
			posB.y = curPos.y;

			DrawLine(posA, posB, lineColor_Center);


			posA.y = pos_LT.y;
			posA.x = curPos.x;

			posB.y = pos_RB.y;
			posB.x = curPos.x;

			DrawLine(posA, posB, lineColor_Center);

		}


		// Editing Border
		public static void DrawEditingBorderline()
		{
			//Vector2 pos = new Vector2(_windowPosX + (_windowWidth / 2), _windowPosY + (_windowHeight / 2));
			Vector2 pos = new Vector2((_windowWidth / 2), (_windowHeight));

			Color borderColor = new Color(0.7f, 0.0f, 0.0f, 0.8f);
			DrawBox(GL2World(pos), (float)(_windowWidth + 100) / _zoom, 50.0f / _zoom, borderColor, false);

			pos.y = -12;

			DrawBox(GL2World(pos), (float)(_windowWidth + 100) / _zoom, 50.0f / _zoom, borderColor, false);
		}

		//-----------------------------------------------------------------------------------------
		public static void ResetCursorEvent()
		{
			_isAnyCursorEvent = false;
			_isDelayedCursorEvent = false;
			_delayedCursorPos = Vector2.zero;
			_delayedCursorType = MouseCursor.Zoom;
		}

		/// <summary>
		/// 마우스 커서를 나오게 하자
		/// </summary>
		/// <param name="mousePos"></param>
		/// <param name="pos"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="cursorType"></param>
		private static void AddCursorRect(Vector2 mousePos, Vector2 pos, float width, float height, MouseCursor cursorType)
		{
			if (pos.x < 0 || pos.x > _windowWidth || pos.y < 0 || pos.y > _windowHeight)
			{
				return;
			}

			if (mousePos.x < pos.x - width * 2 ||
				mousePos.x > pos.x + width * 2 ||
				mousePos.y < pos.y - height * 2 ||
				mousePos.y > pos.y + height * 2)
			{
				//영역을 벗어났다.
				return;
			}

			pos.x -= width / 2;
			pos.y -= height / 2;

			EditorGUIUtility.AddCursorRect(new Rect(pos.x, pos.y, width, height), cursorType);

			_isAnyCursorEvent = true;

		}

		/// <summary>
		/// 마우스 커서를 바꾼다.
		/// 범위는 GL영역 전부이다.
		/// 한번만 호출되며, GUI가 모두 끝날 때 ProcessDelayedCursor()함수를 호출해야한다.
		/// </summary>
		/// <param name="mousePos"></param>
		/// <param name="cursorType"></param>
		public static void AddCursorRectDelayed(Vector2 mousePos, MouseCursor cursorType)
		{
			if(_isAnyCursorEvent || _isDelayedCursorEvent)
			{
				return;
			}
			if (mousePos.x < 0 || mousePos.x > _windowWidth || mousePos.y < 0 || mousePos.y > _windowHeight)
			{
				return;
			}

			_isDelayedCursorEvent = true;
			_delayedCursorPos = mousePos;
			_delayedCursorType = cursorType;

			//Debug.Log("AddCursorRectDelayed : " + mousePos);

			
		}

		public static void ProcessDelayedCursor()
		{
			//Debug.Log("ProcessDelayedCursor : " + _isDelayedCursorEvent);

			if(!_isDelayedCursorEvent
				|| _isAnyCursorEvent
				)
			{
				_isDelayedCursorEvent = false;
				return;
			}
			_isDelayedCursorEvent = false;
			float bias = 20;
			
			EditorGUIUtility.AddCursorRect(
								new Rect(
									_posX_NotCalculated + _delayedCursorPos.x - bias, 
									_posY_NotCalculated + _delayedCursorPos.y - bias, 
									bias * 2, bias * 2), 
								//new Rect(mousePos.x - bias, mousePos.y - bias, bias * 2, bias * 2), 
								//new Rect(_posX_NotCalculated, _posY_NotCalculated, _windowWidth, _windowHeight),
								_delayedCursorType);
			//Debug.Log("AddCurRect : " + _delayedCursorType);
		}


		//-----------------------------------------------------------------------------------------
		private static Color _weightColor_Gray = new Color(0.2f, 0.2f, 0.2f, 1.0f);
		private static Color _weightColor_Blue = new Color(0.0f, 0.2f, 1.0f, 1.0f);
		private static Color _weightColor_Yellow = new Color(1.0f, 1.0f, 0.0f, 1.0f);
		private static Color _weightColor_Red = new Color(1.0f, 0.0f, 0.0f, 1.0f);
		public static Color GetWeightColor(float weight)
		{
			if (weight < 0.0f)
			{
				return _weightColor_Gray;
			}
			else if (weight < 0.5f)
			{
				return _weightColor_Blue * (1.0f - weight * 2.0f) + _weightColor_Yellow * (weight * 2.0f);
			}
			else if (weight < 1.0f)
			{
				return _weightColor_Yellow * (1.0f - (weight - 0.5f) * 2.0f) + _weightColor_Red * ((weight - 0.5f) * 2.0f);
			}
			else
			{
				return _weightColor_Red;
			}
		}

		public static Color GetWeightColor2(float weight, apEditor editor)
		{
			if (weight < 0.0f)
			{
				return editor._colorOption_VertColor_NotSelected;
			}
			else if (weight < 0.25f)
			{
				return (_vertColor_Weighted_0 * (0.25f - weight) + _vertColor_Weighted_25 * (weight)) / 0.25f;
			}
			else if (weight < 0.5f)
			{
				return (_vertColor_Weighted_25 * (0.25f - (weight - 0.25f)) + _vertColor_Weighted_50 * (weight - 0.25f)) / 0.25f;
			}
			else if (weight < 0.75f)
			{
				return (_vertColor_Weighted_50 * (0.25f - (weight - 0.5f)) + _vertColor_Weighted_75 * (weight - 0.5f)) / 0.25f;
			}
			else if (weight < 1.0f)
			{
				return (_vertColor_Weighted_75 * (0.25f - (weight - 0.75f)) + editor._colorOption_VertColor_Selected * (weight - 0.75f)) / 0.25f;
			}
			else
			{
				//return _weightColor_Red;
				return editor._colorOption_VertColor_Selected;
			}
		}

		public static Color GetWeightColor3(float weight)
		{
			if (weight < 0.0f)
			{
				return _vertColor_Weighted3_0;
			}
			else if (weight < 0.25f)
			{
				return (_vertColor_Weighted3_0 * (0.25f - weight) + _vertColor_Weighted3_25 * (weight)) / 0.25f;
			}
			else if (weight < 0.5f)
			{
				return (_vertColor_Weighted3_25 * (0.25f - (weight - 0.25f)) + _vertColor_Weighted3_50 * (weight - 0.25f)) / 0.25f;
			}
			else if (weight < 0.75f)
			{
				return (_vertColor_Weighted3_50 * (0.25f - (weight - 0.5f)) + _vertColor_Weighted3_75 * (weight - 0.5f)) / 0.25f;
			}
			else if (weight < 1.0f)
			{
				return (_vertColor_Weighted3_75 * (0.25f - (weight - 0.75f)) + _vertColor_Weighted3_100 * (weight - 0.75f)) / 0.25f;
			}
			else
			{
				//return _weightColor_Red;
				return _vertColor_Weighted3_100;
			}
		}


		public static Color GetWeightColor3_Vert(float weight)
		{
			if (weight < 0.0f)
			{
				return _vertColor_Weighted3Vert_0;
			}
			else if (weight < 0.25f)
			{
				return (_vertColor_Weighted3Vert_0 * (0.25f - weight) + _vertColor_Weighted3Vert_25 * (weight)) / 0.25f;
			}
			else if (weight < 0.5f)
			{
				return (_vertColor_Weighted3Vert_25 * (0.25f - (weight - 0.25f)) + _vertColor_Weighted3Vert_50 * (weight - 0.25f)) / 0.25f;
			}
			else if (weight < 0.75f)
			{
				return (_vertColor_Weighted3Vert_50 * (0.25f - (weight - 0.5f)) + _vertColor_Weighted3Vert_75 * (weight - 0.5f)) / 0.25f;
			}
			else if (weight < 1.0f)
			{
				return (_vertColor_Weighted3Vert_75 * (0.25f - (weight - 0.75f)) + _vertColor_Weighted3Vert_100 * (weight - 0.75f)) / 0.25f;
			}
			else
			{
				//return _weightColor_Red;
				return _vertColor_Weighted3Vert_100;
			}
		}



		public static Color GetWeightColor4(float weight)
		{
			if (weight <= 0.0001f)
			{
				return _vertColor_Weighted4_0_Null;
			}
			else if (weight < 0.33f)
			{
				return (_vertColor_Weighted4_0 * (0.33f - weight) + _vertColor_Weighted4_33 * (weight)) / 0.33f;
			}
			else if (weight < 0.66f)
			{
				return (_vertColor_Weighted4_33 * (0.33f - (weight - 0.33f)) + _vertColor_Weighted4_66 * (weight - 0.33f)) / 0.33f;
			}
			else if (weight < 1.0f)
			{
				return (_vertColor_Weighted4_66 * (0.34f - (weight - 0.66f)) + _vertColor_Weighted4_100 * (weight - 0.66f)) / 0.34f;
			}
			else
			{
				return _vertColor_Weighted4_100;
			}
		}


		public static Color GetWeightColor4_Vert(float weight)
		{
			if (weight <= 0.0001f)
			{
				return _vertColor_Weighted4Vert_Null;
			}
			else if (weight < 0.33f)
			{
				return (_vertColor_Weighted4Vert_0 * (0.33f - weight) + _vertColor_Weighted4Vert_33 * (weight)) / 0.33f;
			}
			else if (weight < 0.66f)
			{
				return (_vertColor_Weighted4Vert_33 * (0.33f - (weight - 0.33f)) + _vertColor_Weighted4Vert_66 * (weight - 0.33f)) / 0.33f;
			}
			else if (weight < 1.0f)
			{
				return (_vertColor_Weighted4Vert_66 * (0.34f - (weight - 0.66f)) + _vertColor_Weighted4Vert_100 * (weight - 0.66f)) / 0.34f;
			}
			else
			{
				return _vertColor_Weighted4Vert_100;
			}
		}

		public static Color GetWeightGrayscale(float weight)
		{
			//return _weightColor_Gray * (1.0f - weight) + Color.black * weight;
			return Color.black * (1.0f - weight) + Color.white * weight;
		}

		// Window 파라미터 복사 및 복구
		//------------------------------------------------------------------------------------
		public class WindowParameters
		{
			public int _windowWidth;
			public int _windowHeight;
			public Vector2 _scrol_NotCalculated;
			public Vector2 _windowScroll;
			public int _totalEditorWidth;
			public int _totalEditorHeight;
			public int _posX_NotCalculated;
			public int _posY_NotCalculated;
			public float _zoom;
			public Vector4 _glScreenClippingSize;
			public float _animationTimeCount;
			public float _animationTimeRatio;

			public WindowParameters() { }
		}

		public static void GetWindowParameters(WindowParameters inParam)
		{
			inParam._windowWidth = _windowWidth;
			inParam._windowHeight = _windowHeight;
			inParam._scrol_NotCalculated = _scrol_NotCalculated;
			inParam._windowScroll = _windowScroll;
			inParam._totalEditorWidth = _totalEditorWidth;
			inParam._totalEditorHeight = _totalEditorHeight;
			inParam._posX_NotCalculated = _posX_NotCalculated;
			inParam._posY_NotCalculated = _posY_NotCalculated;
			inParam._zoom = _zoom;
			inParam._glScreenClippingSize = _glScreenClippingSize;
			inParam._animationTimeCount = _animationTimeCount;
			inParam._animationTimeRatio = _animationTimeRatio;
		}

		public static void RecoverWindowSize(WindowParameters winParam)
		{
			_windowWidth = winParam._windowWidth;
			_windowHeight = winParam._windowHeight;
			_scrol_NotCalculated = winParam._scrol_NotCalculated;
			_windowScroll = winParam._windowScroll;
			_totalEditorWidth = winParam._totalEditorWidth;
			_totalEditorHeight = winParam._totalEditorHeight;
			_posX_NotCalculated = winParam._posX_NotCalculated;
			_posY_NotCalculated = winParam._posY_NotCalculated;
			_zoom = winParam._zoom;
			_glScreenClippingSize = winParam._glScreenClippingSize;
			_animationTimeCount = winParam._animationTimeCount;
			_animationTimeRatio = winParam._animationTimeRatio;
		}

		public static void SetScreenClippingSizeTmp(Vector4 clippingSize)
		{
			_glScreenClippingSize = clippingSize;
		}
	}

}