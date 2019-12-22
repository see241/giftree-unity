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
//using UnityEngine.Profiling;
using System.Collections;
using System;
using System.Collections.Generic;


using AnyPortrait;

namespace AnyPortrait
{

	public class apEditor : EditorWindow
	{
		private static apEditor s_window = null;

		public static bool IsOpen()
		{
			return (s_window != null);
		}

		public static apEditor CurrentEditor
		{
			get
			{
				return s_window;
			}
		}



		/// <summary>
		/// 작업을 위해서 상세하게 디버그 로그를 출력할 것인가. (True인 경우 정상적인 처리 중에도 Debug가 나온다.)
		/// </summary>
		public static bool IS_DEBUG_DETAILED
		{
			get
			{
				return false;
			}
		}


		//--------------------------------------------------------------


		[MenuItem("Window/AnyPortrait/2D Editor", false, 10), ExecuteInEditMode]
		public static apEditor ShowWindow()
		{
			if (s_window != null)
			{
				try
				{
					s_window._isLockOnEnable = true;
					s_window.Close();
					s_window = null;
				}
				catch (Exception) { }
			}
			EditorWindow curWindow = EditorWindow.GetWindow(typeof(apEditor), false, "AnyPortrait");
			apEditor curTool = curWindow as apEditor;

			//에러가 발생하는 Dialog는 여기서 미리 꺼준다.
			apDialog_PortraitSetting.CloseDialog();
			apDialog_Bake.CloseDialog();

			if (curTool != null)
			{
				curTool.LoadEditorPref();
			}


			if (curTool != null && curTool != s_window)
			//if(curTool != null)
			{
				Debug.ClearDeveloperConsole();
				s_window = curTool;
				//s_window.position = new Rect(0, 0, 200, 200);
				s_window.Init(true);
				s_window._isFirstOnGUI = true;
			}




			return curTool;

		}

		public static void CloseEditor()
		{
			if (s_window != null)
			{
				try
				{
					s_window.Close();
				}
				catch (Exception ex)
				{
					Debug.LogError("Close Exception : " + ex);

				}

				s_window = null;
			}
		}

		//--------------------------------------------------------------
		private bool _isRepaintable = false;
		private bool _isUpdateWhenInputEvent = false;
		public void SetRepaint()
		{
			_isRepaintable = true;
			_isUpdateWhenInputEvent = true;
		}

		private bool _isRefreshClippingGL = false;//<<렌더링을 1프레임 쉰다. 모드 바뀔때 호출 필요
		public void RefreshClippingGL()
		{
			_isRefreshClippingGL = true;
		}

		//--------------------------------------------------------------
		private apEditorController _controller = null;
		public apEditorController Controller
		{
			get
			{
				if (_controller == null)
				{
					_controller = new apEditorController();
					_controller.SetEditor(this);
				}
				return _controller;
			}
		}


		private apVertexController _vertController = new apVertexController();
		public apVertexController VertController { get { return _vertController; } }

		private apGizmoController _gizmoController = new apGizmoController();
		public apGizmoController GizmoController { get { return _gizmoController; } }

		public apController ParamControl
		{
			get
			{
				if (_portrait == null)
				{
					return null;
				}
				else
				{
					return _portrait._controller;
				}
			}
		}

		private apPhysicsPreset _physicsPreset = new apPhysicsPreset();
		public apPhysicsPreset PhysicsPreset { get { return _physicsPreset; } }

		private apControlParamPreset _controlParamPreset = new apControlParamPreset();
		public apControlParamPreset ControlParamPreset { get { return _controlParamPreset; } }

		private apHotKey _hotKey = new apHotKey();
		public apHotKey HotKey { get { return _hotKey; } }

		private apExporter _exporter = null;
		public apExporter Exporter { get { if (_exporter == null) { _exporter = new apExporter(this); } return _exporter; } }

		private apSequenceExporter _sequenceExporter = null;
		public apSequenceExporter SeqExporter { get { if (_sequenceExporter == null) { _sequenceExporter = new apSequenceExporter(this); } return _sequenceExporter; } }

		private apLocalization _localization = new apLocalization();
		public apLocalization Localization { get { return _localization; } }


		private apBackup _backup = new apBackup();
		public apBackup Backup { get { return _backup; } }

		private apOnion _onion = new apOnion();
		public apOnion Onion { get { return _onion; } }

		private apMeshGenerator _meshGenerator = null;
		public apMeshGenerator MeshGenerator { get { if (_meshGenerator == null) { _meshGenerator = new apMeshGenerator(this); } return _meshGenerator; } }

		public apControlParam.CATEGORY _curParamCategory = apControlParam.CATEGORY.Head |
															apControlParam.CATEGORY.Body |
															apControlParam.CATEGORY.Face |
															apControlParam.CATEGORY.Hair |
															apControlParam.CATEGORY.Equipment |
															apControlParam.CATEGORY.Force |
															apControlParam.CATEGORY.Etc;



		public enum TAB_LEFT
		{
			Hierarchy = 0, Controller = 1
		}
		private TAB_LEFT _tabLeft = TAB_LEFT.Hierarchy;
		public TAB_LEFT LeftTab { get { return _tabLeft; } }

		public Vector2 _scroll_MainLeft = Vector2.zero;
		public Vector2 _scroll_MainCenter = Vector2.zero;
		public Vector2 _scroll_MainRight = Vector2.zero;
		public Vector2 _scroll_MainRight2 = Vector2.zero;
		public Vector2 _scroll_Bottom = Vector2.zero;

		public Vector2 _scroll_MainRight_Lower_MG_Mesh = Vector2.zero;
		public Vector2 _scroll_MainRight_Lower_MG_Bone = Vector2.zero;
		public Vector2 _scroll_MainRight_Lower_Anim = Vector2.zero;

		public int _iZoomX100 = 36;//36 => 100
		public const int ZOOM_INDEX_DEFAULT = 36;
		public int[] _zoomListX100 = new int[] {    4,  6,  8,  10, 12, 14, 16, 18, 20, 22, //9
													24, 26, 28, 30, 32, 34, 36, 38, 40, 42, //19
													44, 46, 48, 50, 52, 54, 56, 58, 60, 65, //29
													70, 75, 80, 85, 90, 95, 100, //39
													105, 110, 115, 120, 125, 130, 140, 150, 160, 180, 200,
													220, 240, 260, 280, 300, 350, 400, 450,
													500, 600, 700, 800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000,
													2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900, 3000 };

		public Color _guiMainEditorColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
		public Color _guiSubEditorColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);

		public Material _mat_Color = null;
		public Material _mat_GUITexture = null;
		public Material[] _mat_Texture_Normal = null;//기본 White * Multiply (VColor 기본값이 White)
		public Material[] _mat_Texture_VertAdd = null;//Weight가 가능한 Add Vertex Color (VColor 기본값이 Black)
													  //public Material[] _mat_MaskedTexture = null;//<<구형 버전
		public Material[] _mat_Clipped = null;
		public Material _mat_MaskOnly = null;
		//Onion을 위한 ToneColor
		public Material _mat_ToneColor_Normal = null;
		public Material _mat_ToneColor_Clipped = null;
		public Material _mat_Alpha2White = null;

		public enum BONE_RENDER_MODE
		{
			None,
			Render,
			RenderOutline,
		}
		/// <summary>
		/// Bone을 렌더링 하는가
		/// </summary>
		public BONE_RENDER_MODE _boneGUIRenderMode = BONE_RENDER_MODE.Render;

		public enum MESH_RENDER_MODE
		{
			None,
			Render,
		}
		public MESH_RENDER_MODE _meshGUIRenderMode = MESH_RENDER_MODE.Render;

		[Flags]
		public enum BONE_RENDER_TARGET
		{
			None = 0,
			AllBones = 1,//기본 본 렌더링
			SelectedOnly = 2,//기본 본은 렌더링 안하고 선택한 본만 렌더링 하는 경우
			SelectedOutline = 4,//선택한 본의 붉은 아웃라인을 표시하고자 하는 경우
			Default = 1 | 4
		}



		//public Material _mat_Color2 = null;
		//public Material _mat_Texture2 = null;


		//private Texture2D _btnIcon_Select = null;
		//private Texture2D _btnIcon_Move = null;
		//private Texture2D _btnIcon_Rotate = null;
		//private Texture2D _btnIcon_Scale = null;
		public enum LANGUAGE
		{
			/// <summary>영어</summary>
			English = 0,
			/// <summary>한국어</summary>
			Korean = 1,
			/// <summary>프랑스어</summary>
			French = 2,
			/// <summary>독일어</summary>
			German = 3,
			/// <summary>스페인어</summary>
			Spanish = 4,
			/// <summary>이탈리아어</summary>
			Italian = 5,
			/// <summary>덴마크어</summary>
			Danish = 6,
			/// <summary>일본어</summary>
			Japanese = 7,
			/// <summary>중국어-번체</summary>
			Chinese_Traditional = 8,
			/// <summary>중국어-간체</summary>
			Chinese_Simplified = 9,
			/// <summary>폴란드어</summary>
			Polish = 10,
		}

		public LANGUAGE _language = LANGUAGE.English;

		//색상 옵션
		public Color _colorOption_Background = new Color(0.2f, 0.2f, 0.2f, 1.0f);
		public Color _colorOption_GridCenter = new Color(0.7f, 0.7f, 0.3f, 1.0f);
		public Color _colorOption_Grid = new Color(0.3f, 0.3f, 0.3f, 1.0f);


		public Color _colorOption_MeshEdge = new Color(1.0f, 0.5f, 0.0f, 0.9f);
		public Color _colorOption_MeshHiddenEdge = new Color(1.0f, 1.0f, 0.0f, 0.7f);
		public Color _colorOption_Outline = new Color(0.0f, 0.5f, 1.0f, 0.7f);
		public Color _colorOption_TransformBorder = new Color(0.0f, 1.0f, 1.0f, 1.0f);
		public Color _colorOption_AtlasBorder = new Color(0.0f, 1.0f, 1.0f, 0.5f);

		public Color _colorOption_VertColor_NotSelected = new Color(0.0f, 0.3f, 1.0f, 0.6f);
		public Color _colorOption_VertColor_Selected = new Color(1.0f, 0.0f, 0.0f, 1.0f);

		public Color _colorOption_GizmoFFDLine = new Color(1.0f, 0.5f, 0.2f, 0.9f);
		public Color _colorOption_GizmoFFDInnerLine = new Color(1.0f, 0.7f, 0.2f, 0.7f);

		public Color _colorOption_OnionToneColor = new Color(0.1f, 0.43f, 0.5f, 0.7f);
		public Color _colorOption_OnionAnimPrevColor = new Color(0.5f, 0.2f, 0.1f, 0.7f);
		public Color _colorOption_OnionAnimNextColor = new Color(0.1f, 0.5f, 0.2f, 0.7f);
		public Color _colorOption_OnionBoneColor = new Color(0.4f, 1.0f, 1.0f, 0.9f);
		public Color _colorOption_OnionBonePrevColor = new Color(1.0f, 0.6f, 0.3f, 0.9f);
		public Color _colorOption_OnionBoneNextColor = new Color(0.3f, 1.0f, 0.6f, 0.9f);

		public bool _guiOption_isFPSVisible = true;
		public bool _guiOption_isStatisticsVisible = false;

		//추가 Onion 옵션
		public bool _onionOption_IsOutlineRender = true;//False일 때는 Solid 렌더
		public float _onionOption_OutlineThickness = 0.5f;
		public bool _onionOption_IsRenderOnlySelected = false;
		public bool _onionOption_IsRenderBehind = false;//뒤에 렌더링하기. false일 때에는 앞쪽에 렌더링
		public bool _onionOption_IsRenderAnimFrames = false;//True이면 마커가 아닌 프레임 단위로 렌더링
		public int _onionOption_PrevRange = 1;
		public int _onionOption_NextRange = 1;
		public int _onionOption_RenderPerFrame = 1;
		public float _onionOption_PosOffsetX = 0.0f;
		public float _onionOption_PosOffsetY = 0.0f;
		public bool _onionOption_IKCalculateForce = false;



		//백업 옵션
		//자동 백업 옵션 처리
		public bool _backupOption_IsAutoSave = true;//자동 백업을 지원하는가
		public string _backupOption_BaseFolderName = "AnyPortraitBackup";//폴더를 지정해야한다. (프로젝트 폴더 기준 + 씬이름+에셋)
		public int _backupOption_Minute = 30;//기본은 30분마다 한번씩 저장한다.

		public string _bonePose_BaseFolderName = "AnyPortraitBonePose";


		//시작 화면 옵션
		//매번 시작할 것인가
		//마지막으로 열린 날짜 (날짜가 바뀌면 열린다.)
		public bool _startScreenOption_IsShowStartup = true;
		public int _startScreenOption_LastMonth = 0;
		public int _startScreenOption_LastDay = 0;

		public int _updateLogScreen_LastVersion = 0;

		//Bake시 Color Space를 어디에 맞출 것인가
		public bool _isBakeColorSpaceToGamma = true;

		//RenderPipeline 옵션
		public bool _isUseSRP = false;

		//Modifier Lock 옵션
		public bool _modLockOption_CalculateIfNotAddedOther = false;
		public bool _modLockOption_ColorPreview_Lock = false;
		public bool _modLockOption_ColorPreview_Unlock = true;//<<
		public bool _modLockOption_BonePreview_Lock = false;
		public bool _modLockOption_BonePreview_Unlock = true;//<<
															 //public bool _modLockOption_MeshPreview_Lock = false;
															 //public bool _modLockOption_MeshPreview_Unlock = false;
		public bool _modLockOption_ModListUI_Lock = false;
		public bool _modLockOption_ModListUI_Unlock = false;

		//public Color _modLockOption_MeshPreviewColor = new Color(1.0f, 0.45f, 0.1f, 0.8f);
		public Color _modLockOption_BonePreviewColor = new Color(1.0f, 0.8f, 0.1f, 0.8f);


		//추가 3.1 : 에디터가 유휴 상태일때는 프레임을 낮추자
		public bool _isLowCPUOption = false;
		//추가 3.22 : 모디파이어의 칼라 옵션과 선태 잠금에 대한 옵션
		//public bool _isUseMeshDefualtColorAsModifierInitValueOption = true;
		public bool _isSelectionLockOption_RiggingPhysics = true;
		public bool _isSelectionLockOption_Morph = true;
		public bool _isSelectionLockOption_Transform = true;
		public bool _isSelectionLockOption_ControlParamTimeline = true;



		//CPU 저하 옵션
		public enum LOW_CPU_STATUS
		{
			None,//Option이 꺼져있다.
			Full,//Option이 켜져있지만 해당 안됨
			LowCPU_Mid,//업데이트가 조금 제한된다.
			LowCPU_Low,//업데이트가 많이 제한된다.
		}

		private LOW_CPU_STATUS _lowCPUStatus = LOW_CPU_STATUS.None;
		private Texture2D _imgLowCPUStatus = null;


		//추가 3.29 : Ambient 자동 보정 옵션
		public bool _isAmbientCorrectionOption = true;

		//추가 19.6.28 : 자동으로 Controller 탭으로 전환할 지 여부 옵션 (Mod, AnimClip)
		public bool _isAutoSwitchControllerTab_Mod = true;
		public bool _isAutoSwitchControllerTab_Anim = false;

		//추가 19.6.28 : 메시의 작업용 보이기/숨기기를 작업 끝날때 자동으로 복원하기
		public bool _isRestoreTempMeshVisibilityWhenTackEnded = true;


		//추가 19.7.29 : Blur 브러시 인자를 Gizmo에서 Editor로 옮김
		public bool _blurEnabled = false;
		public int _blurRadius = 50;
		public int _blurIntensity = 50;

		//추가 19.8.13 : 본/리깅 옵션
		public bool _rigOption_NewChildBoneColorIsLikeParent = true;//새로 추가되는 자식 본은 부모의 색상과 유사



		//캡쳐 옵션
		//스샷용
		public enum CAPTURE_GIF_QUALITY
		{
			Low = 0,
			Medium = 1,
			High = 2,
			Maximum = 3
		}
		public int _captureFrame_PosX = 0;
		public int _captureFrame_PosY = 0;
		public int _captureFrame_SrcWidth = 500;
		public int _captureFrame_SrcHeight = 500;
		public int _captureFrame_DstWidth = 500;
		public int _captureFrame_DstHeight = 500;
		public int _captureFrame_SpriteUnitWidth = 500;
		public int _captureFrame_SpriteUnitHeight = 500;
		public int _captureFrame_SpriteMargin = 0;
		public Color _captureFrame_Color = Color.black;
		public bool _captureFrame_IsPhysics = false;
		public bool _isShowCaptureFrame = true;//Capture Frame을 GUI에서 보여줄 것인가
		public bool _isCaptureAspectRatioFixed = true;
		//public int _captureFrame_GIFSampleQuality = 10;//낮을수록 용량이 높은거
		public CAPTURE_GIF_QUALITY _captureFrame_GIFQuality = CAPTURE_GIF_QUALITY.High;
		public int _captureFrame_GIFSampleLoopCount = 1;

		////추가 11.15 : 화면 캡쳐시 ComputeShader를 지원하지 않는다면, 안내 메시지를 띄워야 한다.
		////이 메시지는 하루 한번만 떠야 하므로, "무시하기" 버튼을 눌렀을 때 더이상 뜨지 않게 만들어야 한다.
		//public int _captureComputeShaderNoSupport_Month = 0; 
		//public int _captureComputeShaderNoSupport_Day = 0; 



		public enum CAPTURE_SPRITE_PACK_IMAGE_SIZE
		{
			s256 = 0,
			s512 = 1,
			s1024 = 2,
			s2048 = 3,
			s4096 = 4
		}
		public CAPTURE_SPRITE_PACK_IMAGE_SIZE _captureSpritePackImageWidth = CAPTURE_SPRITE_PACK_IMAGE_SIZE.s1024;
		public CAPTURE_SPRITE_PACK_IMAGE_SIZE _captureSpritePackImageHeight = CAPTURE_SPRITE_PACK_IMAGE_SIZE.s1024;

		//public enum CAPTURE_SPRITE_ANIM_FPS_METHOD
		//{
		//	AllFrames = 0,
		//	CommonFPS = 1
		//}
		//public CAPTURE_SPRITE_ANIM_FPS_METHOD _captureSpriteAnimFPSMethod = CAPTURE_SPRITE_ANIM_FPS_METHOD.AllFrames;
		//public int _captureSpriteAnimCommonFPS = 20;

		public enum CAPTURE_SPRITE_TRIM_METHOD
		{
			/// <summary>설정했던 크기 그대로</summary>
			Fixed = 0,
			/// <summary>설정 크기를 기준으로 여백이 있으면 크기가 줄어든다.</summary>
			Compressed = 1,
		}
		public CAPTURE_SPRITE_TRIM_METHOD _captureSpriteTrimSize = CAPTURE_SPRITE_TRIM_METHOD.Fixed;
		public bool _captureSpriteMeta_XML = false;
		public bool _captureSpriteMeta_JSON = false;
		public bool _captureSpriteMeta_TXT = false;


		//캡쳐시 위치/줌을 고정할 수 있다. (수동)
		public Vector2 _captureSprite_ScreenPos = Vector2.zero;
		public int _captureSprite_ScreenZoom = ZOOM_INDEX_DEFAULT;


		//추가 8.27 : 메시 자동 생성 기능에 대한 옵션
		public float _meshTRSOption_MirrorOffset = 0.5f;
		public bool _meshTRSOption_MirrorSnapVertOnRuler = false;
		public bool _meshTRSOption_MirrorRemoved = false;
		public float _meshAutoGenOption_AlphaCutOff = 0.02f;
		public int _meshAutoGenOption_GridDivide = 2;//최소 1개
		public int _meshAutoGenOption_Margin = 2;
		public int _meshAutoGenOption_numControlPoint_ComplexQuad_X = 3;//최소 3개
		public int _meshAutoGenOption_numControlPoint_ComplexQuad_Y = 3;//최소 3개
		public int _meshAutoGenOption_numControlPoint_CircleRing = 4;//최소 4개
		public bool _meshAutoGenOption_IsLockAxis = false;


		//추가 19.7.31 : 리깅 GUI에 대한 옵션. 일부는 Selection에 있는 값을 가져왔다.
		public bool _rigViewOption_WeightOnly = false;
		public bool _rigViewOption_BoneColor = true;
		public bool _rigViewOption_CircleVert = false;

		private List<apPortrait> _portraitsInScene = new List<apPortrait>();
		private bool _isPortraitListLoaded = false;

		//Portrait를 생성한다는 요청이 있다. => 이 요청은 Repaint 이벤트가 아닐때 실행된다.
		private bool _isMakePortraitRequest = false;
		private string _requestedNewPortraitName = "";

		private bool _isMakePortraitRequestFromBackupFile = false;
		private string _requestedLoadedBackupPortraitFilePath = "";

		//상단 버튼 탭 상태
		private enum GUITOP_TAB
		{
			Tab1_BakeAndSetting,
			Tab2_TRSTools,
			Tab3_Visibility,
			Tab4_FFD_Soft_Blur,
			Tab5_GizmoValue,
			Tab6_Capture
		}
		private Dictionary<GUITOP_TAB, bool> _guiTopTabStaus = new Dictionary<GUITOP_TAB, bool>();

		//추가 3.28 : 메인 Hierarchy의 SortMode 기능
		//Hierarchy의 순서를 바꿀 수 있다.
		//SortMode를 켠 상태에서
		//- SortMode를 직접 끄거나
		//- 어떤 항목을 선택하거나
		//- Portrait가 바뀌거나
		//- 에디터가 리셋 될때
		//SortMode는 꺼진다.
		private bool _isHierarchyOrderEditEnabled = false;
		public void TurnOffHierarchyOrderEdit() { _isHierarchyOrderEditEnabled = false; }

		public enum HIERARCHY_SORT_MODE
		{
			RegOrder = 0,//등록된 순서
			AlphaNum = 1,//이름 순서
			Custom = 2,//변경된 순서
		}
		private HIERARCHY_SORT_MODE _hierarchySortMode = HIERARCHY_SORT_MODE.RegOrder;
		public HIERARCHY_SORT_MODE HierarchySortMode { get { return _hierarchySortMode; } }

		// Gizmos
		//-------------------------------------------------------------
		private apGizmos _gizmos = null;
		public apGizmos Gizmos { get { return _gizmos; } }
		//-------------------------------------------------------------

		// Hierarchy
		//--------------------------------------------------------------
		private apEditorHierarchy _hierarchy = null;
		public apEditorHierarchy Hierarchy { get { return _hierarchy; } }


		private apEditorMeshGroupHierarchy _hierarchy_MeshGroup = null;
		public apEditorMeshGroupHierarchy Hierarchy_MeshGroup { get { return _hierarchy_MeshGroup; } }

		private apEditorAnimClipTargetHierarchy _hierarchy_AnimClip = null;
		public apEditorAnimClipTargetHierarchy Hierarchy_AnimClip { get { return _hierarchy_AnimClip; } }
		//--------------------------------------------------------------

		// Timeline GUI
		//--------------------------------------------------------------
		private apAnimClip _prevAnimClipForTimeline = null;
		private List<apTimelineLayerInfo> _timelineInfoList = new List<apTimelineLayerInfo>();
		public List<apTimelineLayerInfo> TimelineInfoList { get { return _timelineInfoList; } }

		public enum TIMELINE_INFO_SORT
		{
			Registered,//등록 순서대로..
			ABC,//가나다 순
			Depth,//깊이 순서대로 (RenderUnit 한정)
		}
		public TIMELINE_INFO_SORT _timelineInfoSortType = TIMELINE_INFO_SORT.Registered;//<<이게 기본값 (저장하자)
																						//--------------------------------------------------------------


		//Left, Right, Middle Mouse
		//-------------------------------------------------------------------------------
		//수정 : apMouse로 분리 되었던 것을 apMouseSet으로 변경한다.
		//public int _curMouseBtn = -1;
		//public int MOUSE_BTN_LEFT { get { return 0; } }
		//public int MOUSE_BTN_RIGHT { get { return 1; } }
		//public int MOUSE_BTN_MIDDLE { get { return 2; } }
		//public int MOUSE_BTN_LEFT_NOT_BOUND { get { return 3; } }
		//public apMouse[] _mouseBtn = new apMouse[] { new apMouse(), new apMouse(), new apMouse(), new apMouse() };
		private apMouseSet _mouseSet = new apMouseSet();
		public apMouseSet Mouse { get { return _mouseSet; } }
		//------------------------------------------------------------------------------

		public Rect _mainGUIRect = new Rect();


		//Mesh Edit
		//public enum BRUSH_TYPE
		//{
		//	SetValue = 0, AddSubtract = 1,
		//}
		//public float[] _brushPreset_Size = new float[]
		//{
		//	1, 2, 3, 4, 5, 6, 7, 8, 9,
		//	10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95,
		//	100, 110, 120, 130, 140, 150, 160, 170, 180, 190,
		//	200, 220, 240, 260, 280,
		//	300, 320, 340, 360, 380,
		//	400, 450, 500,
		//	//550, 600, 650, 700, 750, 800, 850, 900, 950,
		//	//1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000
		//};
		//public float BRUSH_MIN_SIZE { get { return _brushPreset_Size[0]; } }
		//public float BRUSH_MAX_SIZE { get { return _brushPreset_Size[_brushPreset_Size.Length - 1]; } }

		#region [미사용 코드]
		//public float BRUSH_MIN_HARDNESS { get { return _brushPreset_Hardness[0]; } }
		//public float BRUSH_MAX_HARDNESS { get { return _brushPreset_Hardness[_brushPreset_Hardness.Length - 1]; } }
		//public float[] _brushPreset_Hardness = new float[]
		//{
		//	0.0f, 10.0f, 20.0f, 30.0f, 40.0f, 50.0f, 60.0f, 70.0f, 80.0f, 90.0f, 100.0f
		//};

		//public float[] _brushPreset_Alpha = new float[]
		//{
		//	0.0f, 10.0f, 20.0f, 30.0f, 40.0f, 50.0f, 60.0f, 70.0f, 80.0f, 90.0f, 100.0f
		//};
		//public float BRUSH_MIN_ALPHA { get { return _brushPreset_Alpha[0]; } }
		//public float BRUSH_MAX_ALPHA { get { return _brushPreset_Alpha[_brushPreset_Alpha.Length - 1]; } } 
		#endregion

		//public BRUSH_TYPE _brushType_VolumeEdit = BRUSH_TYPE.SetValue;
		//public float _brushValue_VolumeEdit = 50.0f;
		//public float _brushSize_VolumeEdit = 20.0f;
		//public float _brushHardness_VolumeEdit = 50.0f;
		//public float _brushAlpha_VolumeEdit = 50.0f;
		//public float _paintValue_VolumeEdit = 50.0f;


		//public BRUSH_TYPE _brushType_Physic = BRUSH_TYPE.SetValue;
		//public float _brushValue_Physic = 50.0f;
		//public float _brushSize_Physic = 20.0f;
		//public float _brushHardness_Physic = 50.0f;
		//public float _brushAlpha_Physic = 50.0f;
		//public float _paintValue_Physic = 50.0f;


		//Window 호출
		private enum DIALOG_SHOW_CALL
		{
			None,
			Setting,
			Bake,
			Capture,
		}
		private DIALOG_SHOW_CALL _dialogShowCall = DIALOG_SHOW_CALL.None;
		private EventType _curEventType = EventType.Ignore;
		//--------------------------------------------------------------

		public apPortrait _portrait = null;

		public Dictionary<string, int> _tmpValues = new Dictionary<string, int>();

		//--------------------------------------------------------------
		// 프레임 업데이트 변수
		private enum FRAME_TIMER_TYPE
		{
			Update, Repaint, None
		}
		#region [미사용 변수] apTimer로 옮겨서 처리한다.

		//private FRAME_TIMER_TYPE _frameTimerType = FRAME_TIMER_TYPE.None;
		//private DateTime _dateTime_Update = DateTime.Now;
		//private DateTime _dateTime_GUI = DateTime.Now;

		//private bool _isValidFrame = false;
		//private float _deltaTimePerValidFrame = 0.0f;
		//private bool _isCountDeltaTime = false;

		//private double _tDelta_Update = 0.0f;
		//private double _tDelta_GUI = 0.0f;

		////아주 짧은 시간동안 너무 큰 FPS로 시간 연산이 이루어진 경우,
		////tDelta를 0으로 리턴하는 대신, 그동안 시간을 누적시키고 있자.
		//private const double MIN_EDITOR_UPDATE_TIME = 0.01;//60FPS(0.0167), 100FPS
		//private float _tDeltaDelayed_Update = 0.0f;
		//private float _tDeltaDelayed_GUI = 0.0f;



		//public bool IsValidFrame {  get { return _isValidFrame; } }
		//public float DeltaFrameTime
		//{
		//	//수정 : Update와 OnGUI에서 값을 가져갈때
		//	//따로 체크를 해야한다. (Update에서 체크했던 타이머 값을 여러번 호출해서 사용하는 듯 하다)
		//	get
		//	{
		//		//if (_isCountDeltaTime)
		//		//{
		//		//	return _deltaTimePerValidFrame;
		//		//}
		//		switch (_frameTimerType)
		//		{
		//			case FRAME_TIMER_TYPE.Update:
		//				//return _tDeltaDelayed_Update;
		//				return apTimer.I.DeltaTime;

		//			case FRAME_TIMER_TYPE.GUIRepaint:
		//				//return _tDeltaDelayed_GUI;
		//				return 0.0f;

		//			case FRAME_TIMER_TYPE.None:
		//				return 0.0f;
		//		}
		//		return 0.0f;
		//	}
		//} 
		#endregion

		public float DeltaTime_Update { get { return apTimer.I.DeltaTime_Update; } }
		public float DeltaTime_UpdateAllFrame { get { return apTimer.I.DeltaTime_UpdateAllFrame; } }
		public float DeltaTime_Repaint { get { return apTimer.I.DeltaTime_Repaint; } }

		//이전 : 보기가 어려운 방식 ㅜㅜ
		//private float _avgFPS = 0.0f;
		//private int _iAvgFPS = 0;
		//public int FPS { get { return _iAvgFPS; } }

		//변경 19.11.23 : 별도의 클래스를 이용
		private apFPSCounter _fpsCounter = new apFPSCounter();

		//private System.Text.StringBuilder _sb_FPSText = new System.Text.StringBuilder(16);
		private apStringWrapper _fpsString = null;
		private const string TEXT_FPS = "FPS ";

		//--------------------------------------------------------------
		// 단축키 관련 변수
		private bool _isHotKeyProcessable = true;
		private bool _isHotKeyEvent = false;
		private KeyCode _hotKeyCode = KeyCode.A;
		private bool _isHotKey_Ctrl = false;
		private bool _isHotKey_Alt = false;
		private bool _isHotKey_Shift = false;


		//--------------------------------------------------------------
		// OnGUI 이벤트 성격을 체크하기 위한 변수
		private bool _isGUIEvent = false;
		//추가 19.11.23 : String대신 Enum 타입으로 변경한다.
		public enum DELAYED_UI_TYPE
		{
			None,
			Right2GUI,
			GUI_Top_Onion_Visible,
			Top_UI__Vertex_Transform,
			Top_UI__Position,
			Top_UI__Rotation,
			Top_UI__Scale,
			Top_UI__Depth,
			Top_UI__Color,
			Top_UI__Extra,
			Top_UI__BoneIKController,
			Top_UI__VTF_FFD,
			Top_UI__VTF_Soft,
			Top_UI__VTF_Blur,
			Top_UI__Overall,
			GUI_MeshGroup_Hierarchy_Delayed,
			GUI_Anim_Hierarchy_Delayed__Meshes,
			GUI_Anim_Hierarchy_Delayed__Bone,
			GUI_Anim_Hierarchy_Delayed__ControlParam,
			Capture_GIF_ProgressBar,
			Capture_GIF_Clips,
			Capture_Spritesheet_ProgressBar,
			Capture_Spritesheet_Settings,
			Mesh_Property_Modify_UI_Single,
			Mesh_Property_Modify_UI_Multiple,
			Mesh_Property_Modify_UI_No_Info,
			BoneEditMode__Editable,
			BoneEditMode__Select,
			BoneEditMode__Add,
			BoneEditMode__Link,
			Bottom2_Transform_Mod_Vert,
			Animation_Bottom_Property__MK,
			Animation_Bottom_Property__SK,
			Animation_Bottom_Property__L,
			Animation_Bottom_Property__T,
			Bottom_Right_Anim_Property__ControlParamUI,
			Bottom_Right_Anim_Property__ModifierUI,
			Anim_Property__SameKeyframe,
			AnimProperty_MultipleCurve__Sync,
			AnimProperty_MultipleCurve__NotSync,
			MeshGroupRight_Setting_ObjectSelected,
			MeshGroupRight_Setting_ObjectNotSelected,
			Render_Unit_Detail_Status__MeshTransform,
			Render_Unit_Detail_Status__MeshGroupTransform,
			MeshGroup_Mesh_Setting__CustomShader,
			MeshGroup_Mesh_Setting__MaterialLibrary,
			MeshGroup_Mesh_Setting__MatLib_NotUseDefault,
			MeshGroup_Mesh_Setting__Same_Mesh,
			Mesh_Transform_Detail_Status__Clipping_Child,
			Mesh_Transform_Detail_Status__Clipping_Parent,
			Mesh_Transform_Detail_Status__Clipping_None,
			Update_Child_Bones,
			MeshGroupRight2_Bone,
			MeshGroup_Bone__Child_Bone_Drawable,
			Bone_Mirror_Axis_Option_Visible,
			MeshGroupBottom_Modifier,
			CP_Selected_ParamSetGroup,
			Modifier_Add_Transform_Check,
			Modifier_Add_Transform_Check_Inverse,
			Modifier_Add_Transform_Check__Rigging,
			Rigging_UI_Info__MultipleVert,
			Rigging_UI_Info__SingleVert,
			Rigging_UI_Info__UnregRigData,
			Rigging_UI_Info__SameMode,
			Rig_Mod__RigDataCount_Refreshed,
			Modifier_Add_Transform_Check__Physic__Valid,
			Modifier_Add_Transform_Check__Physic__Invalid,
			AnimationRight2GUI_AnimClip,
			AnimationRight2GUI_Timeline,
			AnimationRight2GUI_Timeline_SelectedObject,
			AnimationRight2GUI_Timeline_Layers,
		}
		private Dictionary<DELAYED_UI_TYPE, bool> _delayedGUIShowList = new Dictionary<DELAYED_UI_TYPE, bool>();
		private Dictionary<DELAYED_UI_TYPE, bool> _delayedGUIToggledList = new Dictionary<DELAYED_UI_TYPE, bool>();

		//--------------------------------------------------------------
		// Inspector를 위한 변수들
		private apSelection _selection = null;
		public apSelection Select { get { return _selection; } }


		// 이미지 세트
		//--------------------------------------------------------------
		private apImageSet _imageSet = null;
		public apImageSet ImageSet { get { return _imageSet; } }


		// 재질 라이브러리
		//--------------------------------------------------------------
		private apMaterialLibrary _materialLibrary = null;
		public apMaterialLibrary MaterialLibrary { get { return _materialLibrary; } }

		//--------------------------------------------------------------
		// Mesh Edit 모드
		public enum MESH_EDIT_MODE
		{
			Setting,
			MakeMesh,//AddVertex + LinkEdge
			Modify,
			//AddVertex,//삭제합니더 => 
			//LinkEdge,//삭제
			PivotEdit,
			//VolumeWeight,//>삭제합니더
			//PhysicWeight,//>>삭제합니더

		}


		public enum MESH_EDIT_MODE_MAKEMESH
		{
			//Add Sub Tab일때 (이 값중 하나면 Add SubTab이 켜진다.)
			VertexAndEdge,
			VertexOnly,
			EdgeOnly,
			Polygon,
			//Auto Sub Tab일때
			AutoGenerate,
			//TRS Sub Tab일때
			TRS
		}

		public enum MESH_EDIT_RENDER_MODE
		{
			Normal, ZDepth
		}
		public MESH_EDIT_MODE _meshEditMode = MESH_EDIT_MODE.Setting;
		public MESH_EDIT_RENDER_MODE _meshEditZDepthView = MESH_EDIT_RENDER_MODE.Normal;
		public MESH_EDIT_MODE_MAKEMESH _meshEditeMode_MakeMesh = MESH_EDIT_MODE_MAKEMESH.VertexAndEdge;

		public enum MESH_EDIT_MIRROR_MODE
		{
			None,
			Mirror,
		}
		public MESH_EDIT_MIRROR_MODE _meshEditMirrorMode = MESH_EDIT_MIRROR_MODE.None;

		private apMirrorVertexSet _mirrorVertexSet = null;
		public apMirrorVertexSet MirrorSet { get { if (_mirrorVertexSet == null) { _mirrorVertexSet = new apMirrorVertexSet(this); } return _mirrorVertexSet; } }

		public enum ROOTUNIT_EDIT_MODE
		{
			Setting,
			Capture
		}
		public enum ROOTUNIT_CAPTURE_MODE
		{
			Thumbnail,
			ScreenShot,
			GIFAnimation,
			SpriteSheet
		}

		public ROOTUNIT_EDIT_MODE _rootUnitEditMode = ROOTUNIT_EDIT_MODE.Setting;
		public ROOTUNIT_CAPTURE_MODE _rootUnitCaptureMode = ROOTUNIT_CAPTURE_MODE.Thumbnail;

		// MeshGroup Edit 모드
		public enum MESHGROUP_EDIT_MODE
		{
			Setting,
			Bone,
			Modifier,
		}

		public MESHGROUP_EDIT_MODE _meshGroupEditMode = MESHGROUP_EDIT_MODE.Setting;

		


		public enum TIMELINE_LAYOUTSIZE
		{
			Size1, Size2, Size3,
		}
		public TIMELINE_LAYOUTSIZE _timelineLayoutSize = TIMELINE_LAYOUTSIZE.Size2;//<<기본값은 2

		public int[] _timelineZoomWPFPreset = new int[]
		//{ 1, 2, 5, 7, 10, 12, 15, 17, 20, 22, 25, 27, 30, 32, 35, 37, 40, 42, 45, 47, 50 };
		{ 50, 45, 40, 35, 30, 25, 22, 20, 17, 15, 12, 11, 10, 9, 8 };
		public const int DEFAULT_TIMELINE_ZOOM_INDEX = 10;
		public int _timelineZoom_Index = DEFAULT_TIMELINE_ZOOM_INDEX;
		public int WidthPerFrameInTimeline { get { return _timelineZoomWPFPreset[_timelineZoom_Index]; } }

		public bool _isAnimAutoScroll = true;
		public bool _isAnimAutoKey = false;

		//UI 숨기기 기능은 아래의 변수로 모두 변경되었다.
		//public enum RIGHT_UPPER_LAYOUT
		//{
		//	Show,
		//	Hide,
		//}
		//public RIGHT_UPPER_LAYOUT _right_UpperLayout = RIGHT_UPPER_LAYOUT.Show;

		//// MeshGroup Right의 하위 메뉴
		//public enum RIGHT_LOWER_LAYOUT
		//{
		//	Hide,
		//	ChildList,
		//	//Add,//<<이거 삭제하자
		//	//AddMeshGroup
		//}
		//public RIGHT_LOWER_LAYOUT _rightLowerLayout = RIGHT_LOWER_LAYOUT.ChildList;


		///전체화면 기능 추가
		public bool _isFullScreenGUI = false;//<<기본값은 false이다.

		//추가 19.8.17 : UI의 숨기기 기능을 통합한다.
		// UI 숨기기 버튼 누른 결과 이벤트
		public enum UI_FOLD_BTN_RESULT
		{
			None,
			ToggleFold_Horizontal,
			ToggleFold_Vertical,
		}

		// UI가 어떻게 숨겨져있는지 여부
		public enum UI_FOLD_TYPE
		{
			Unfolded,
			Folded,
		}

		private UI_FOLD_TYPE _uiFoldType_Left = UI_FOLD_TYPE.Unfolded;
		private UI_FOLD_TYPE _uiFoldType_Right1 = UI_FOLD_TYPE.Unfolded;
		private UI_FOLD_TYPE _uiFoldType_Right1_Upper = UI_FOLD_TYPE.Unfolded;
		private UI_FOLD_TYPE _uiFoldType_Right1_Lower = UI_FOLD_TYPE.Unfolded;
		private UI_FOLD_TYPE _uiFoldType_Right2 = UI_FOLD_TYPE.Unfolded;

		



		[Flags]
		public enum HIERARCHY_FILTER
		{
			None = 0,
			RootUnit = 1,
			Image = 2,
			Mesh = 4,
			MeshGroup = 8,
			Animation = 16,
			Param = 32,
			All = 63
		}
		public HIERARCHY_FILTER _hierarchyFilter = HIERARCHY_FILTER.All;







		//--------------------------------------------------------------
		// UI를 위한 변수들
		//private bool _isFold_PS_Main = false;
		//public bool _isFold_PS_Image = false;
		//public bool _isFold_PS_Mesh = false;

		//public bool _isFold_PS_MeshGroup = false;
		//public bool _isFold_PS_Bone = false;

		//public bool _isFold_PS_Face = false;
		//private bool _isMouseEvent = false;

		private bool _isNotification = false;
		private float _tNotification = 0.0f;
		private const float NOTIFICATION_TIME_LONG = 4.5f;
		private const float NOTIFICATION_TIME_SHORT = 1.2f;

		//GUI에 그려지는 작은 Noti를 출력하자
		private bool _isNotification_GUI = false;
		private float _tNotification_GUI = 0.0f;
		private string _strNotification_GUI = "";

		//GUI에 백업 처리를 하는 경우 업데이트를 하자
		private bool _isBackupProcessing = false;
		private float _tBackupProcessing_Label = 0.0f;
		private float _tBackupProcessing_Icon = 0.0f;
		private const float BACKUP_LABEL_TIME_LENGTH = 2.0f;
		private const float BACKUP_ICON_TIME_LENGTH = 0.8f;
		private Texture2D _imgBackupIcon_Frame1 = null;
		private Texture2D _imgBackupIcon_Frame2 = null;

		private bool _isUpdateAfterEditorRunning = false;

		public bool _isFirstOnGUI = false;

		/// <summary>
		/// Gizmo 등으로 강제로 업데이트를 한 경우에는 업데이트를 Skip한다.
		/// </summary>
		private bool _isUpdateSkip = false;

		public void SetUpdateSkip()
		{
			_isUpdateSkip = true;
			apTimer.I.ResetTime_Update();
		}

		/// <summary>
		/// RootUnit/MeshGroup/AnimClip을 선택한 상태에서 Control Parameter나 Frame이 변경되면 Bone Matrix를 임시로 업데이트 하는데,
		/// 이때 GUI와 맞지 않을 수 있다.
		/// 이 값이 True이고 IK가 켜져 있다면 업데이트를 강제로 한번 더 해야한다.
		/// </summary>
		private bool _isMeshGroupChangedByEditor = false;

		/// <summary>
		/// RootUnit/MeshGroup/AnimClip을 선택중일 때 "Control Parameter", "Frame"이 변경되었다면 이 함수를 호출한다.
		/// </summary>
		public void SetMeshGroupChanged()
		{
			_isMeshGroupChangedByEditor = true;
		}



		// 화면 캡쳐 요청
		// 추가:맥버전은 매프레임마다 RenderTexture를 돌리면 안된다.
		// 카운트를 해야한다.
		private bool _isScreenCaptureRequest = false;
#if UNITY_EDITOR_OSX
		private bool _isScreenCaptureRequest_OSXReady = false;
		private int _screenCaptureRequest_Count = 0;
		private const int SCREEN_CAPTURE_REQUEST_OSX_COUNT = 5;
#endif
		private apScreenCaptureRequest _screenCaptureRequest = null;


		//서비스 중인 최신 버전
		private bool _isCheckLiveVersion = false;
		private string _currentLiveVersion = "";
		private int _lastCheckLiveVersion_Month = 0;
		private int _lastCheckLiveVersion_Day = 0;
		public bool _isCheckLiveVersion_Option = false;
		//추가 : 버전 체크 후 업데이트 알람 무시하기 기능
		private bool _isVersionNoticeIgnored = false;
		private int _versionNoticeIvnored_Year = 0;
		private int _versionNoticeIvnored_Month = 0;
		private int _versionNoticeIvnored_Day = 0;

		//종료 요청 이후 카운트가 계속 늘어나면 강제로 종료해야한다. (무한 루프에 빠질 수 있다)
		private bool _isLockOnEnable = false;

		// 추가 4.1 : 객체의 추가/삭제가 있는 경우 전체 Link를 다시 해야한다.

		/// <summary>객체가 추가되거나 삭제된 경우 이 함수를 호출해야한다. 이후 Refresh될 때 다시 Link해야할 필요가 있기 때문</summary>
		public void OnAnyObjectAddedOrRemoved()
		{
			if (_portrait == null)
			{
				_recordList_TextureData.Clear();
				_recordList_Mesh.Clear();
				_recordList_MeshGroup.Clear();
				_recordList_AnimClip.Clear();
				_recordList_ControlParam.Clear();
				_recordList_Modifier.Clear();
				_recordList_AnimTimeline.Clear();
				_recordList_AnimTimelineLayer.Clear();
				_recordList_Transform.Clear();
				_recordList_Bone.Clear();
			}
			else
			{
				_recordList_TextureData.Clear();
				_recordList_Mesh.Clear();
				_recordList_MeshGroup.Clear();
				_recordList_AnimClip.Clear();
				_recordList_ControlParam.Clear();
				_recordList_Modifier.Clear();
				_recordList_AnimTimeline.Clear();
				_recordList_AnimTimelineLayer.Clear();
				_recordList_Transform.Clear();
				_recordList_Bone.Clear();

				if (_portrait._textureData != null && _portrait._textureData.Count > 0)
				{
					for (int i = 0; i < _portrait._textureData.Count; i++)
					{
						_recordList_TextureData.Add(_portrait._textureData[i]._uniqueID);
					}
				}

				if (_portrait._meshes != null && _portrait._meshes.Count > 0)
				{
					for (int i = 0; i < _portrait._meshes.Count; i++)
					{
						_recordList_Mesh.Add(_portrait._meshes[i]._uniqueID);
					}
				}

				apMeshGroup meshGroup = null;
				if (_portrait._meshGroups != null && _portrait._meshGroups.Count > 0)
				{
					for (int iMeshGroup = 0; iMeshGroup < _portrait._meshGroups.Count; iMeshGroup++)
					{
						meshGroup = _portrait._meshGroups[iMeshGroup];

						_recordList_MeshGroup.Add(meshGroup._uniqueID);

						//MeshGroup -> Modifier
						for (int iMod = 0; iMod < meshGroup._modifierStack._modifiers.Count; iMod++)
						{
							_recordList_Modifier.Add(meshGroup._modifierStack._modifiers[iMod]._uniqueID);
						}

						//MeshGroup -> Transform
						for (int iMeshTF = 0; iMeshTF < meshGroup._childMeshTransforms.Count; iMeshTF++)
						{
							_recordList_Transform.Add(meshGroup._childMeshTransforms[iMeshTF]._transformUniqueID);
						}

						for (int iMeshGroupTF = 0; iMeshGroupTF < meshGroup._childMeshGroupTransforms.Count; iMeshGroupTF++)
						{
							_recordList_Transform.Add(meshGroup._childMeshGroupTransforms[iMeshGroupTF]._transformUniqueID);
						}

						for (int iBone = 0; iBone < meshGroup._boneList_All.Count; iBone++)
						{
							_recordList_Bone.Add(meshGroup._boneList_All[iBone]._uniqueID);
						}

					}
				}

				apAnimClip animClip = null;
				apAnimTimeline timeline = null;
				apAnimTimelineLayer timelineLayer = null;

				if (_portrait._animClips != null && _portrait._animClips.Count > 0)
				{
					for (int iAnimClip = 0; iAnimClip < _portrait._animClips.Count; iAnimClip++)
					{
						animClip = _portrait._animClips[iAnimClip];
						_recordList_AnimClip.Add(animClip._uniqueID);

						for (int iTimeline = 0; iTimeline < animClip._timelines.Count; iTimeline++)
						{
							timeline = animClip._timelines[iTimeline];
							_recordList_AnimTimeline.Add(timeline._uniqueID);

							for (int iLayer = 0; iLayer < timeline._layers.Count; iLayer++)
							{
								timelineLayer = timeline._layers[iLayer];
								_recordList_AnimTimelineLayer.Add(timelineLayer._uniqueID);
							}
						}
					}
				}

				if (_portrait._controller._controlParams != null && _portrait._controller._controlParams.Count > 0)
				{
					for (int i = 0; i < _portrait._controller._controlParams.Count; i++)
					{
						_recordList_ControlParam.Add(_portrait._controller._controlParams[i]._uniqueID);
					}
				}
			}
		}

		private List<int> _recordList_TextureData = new List<int>();
		private List<int> _recordList_Mesh = new List<int>();
		private List<int> _recordList_MeshGroup = new List<int>();
		private List<int> _recordList_AnimClip = new List<int>();
		private List<int> _recordList_AnimTimeline = new List<int>();
		private List<int> _recordList_AnimTimelineLayer = new List<int>();
		private List<int> _recordList_ControlParam = new List<int>();
		private List<int> _recordList_Modifier = new List<int>();
		private List<int> _recordList_Transform = new List<int>();
		private List<int> _recordList_Bone = new List<int>();

		//현재 씬
		private UnityEngine.SceneManagement.Scene _currentScene;



		//리소스 경로
		//--------------------------------------------------------------
		// 추가 19.11.20 : GUIContent들
		private apGUIContentWrapper _guiContent_Notification = null;
		private apGUIContentWrapper _guiContent_TopBtn_Setting = null;
		private apGUIContentWrapper _guiContent_TopBtn_Bake = null;
		private apGUIContentWrapper _guiContent_MainLeftUpper_MakeNewPortrait = null;
		private apGUIContentWrapper _guiContent_MainLeftUpper_RefreshToLoad = null;
		private apGUIContentWrapper _guiContent_MainLeftUpper_LoadBackupFile = null;
		private apGUIContentWrapper _guiContent_GUITopTab_Open = null;
		private apGUIContentWrapper _guiContent_GUITopTab_Folded = null;
		private apGUIContentWrapper _guiContent_Top_GizmoIcon_Move = null;
		private apGUIContentWrapper _guiContent_Top_GizmoIcon_Depth = null;
		private apGUIContentWrapper _guiContent_Top_GizmoIcon_Rotation = null;
		private apGUIContentWrapper _guiContent_Top_GizmoIcon_Scale = null;
		private apGUIContentWrapper _guiContent_Top_GizmoIcon_Color = null;
		private apGUIContentWrapper _guiContent_Top_GizmoIcon_Extra = null;

		//주의 : GUIContent 추가시 ResetGUIContents() 함수에 리셋 코드 추가

		//--------------------------------------------------------------
		// 추가 19.11.21 : GUIStyle Wrapper 객체
		private apGUIStyleWrapper _guiStyleWrapper = null;
		public apGUIStyleWrapper GUIStyleWrapper {  get {  return _guiStyleWrapper; } }
		//--------------------------------------------------------------

		//--------------------------------------------------------------
		void OnDisable()
		{
			//재빌드 등을 통해서 다시 시작하는 경우 -> 다시 Init
			Init(false);
			SaveEditorPref();

			Undo.undoRedoPerformed -= OnUndoRedoPerformed;
			//SceneView.onSceneGUIDelegate -= OnSceneViewEvent;
			//EditorApplication.modifierKeysChanged -= OnKeychanged;

			//추가 3.25 : 씬이 바뀌면 작업을 초기화해야한다.
			//EditorApplication.hierarchyWindowChanged -= OnEditorHierarchyChanged;
#if UNITY_2018_1_OR_NEWER
			EditorApplication.hierarchyChanged -= OnEditorHierarchyChanged;
#else
			EditorApplication.hierarchyWindowChanged -= OnEditorHierarchyChanged;
#endif

			if (_backup != null)
			{
				_backup.StopForce();
			}

			//에디터를 끌때 EditorDirty를 수행한다.
			apEditorUtil.SetEditorDirty();
		}



		void OnEnable()
		{
			if (_isLockOnEnable)
			{
				//Debug.Log("apEditor : OnEnable >> Locked");
				return;
			}
			//Debug.Log("apEditor : OnEnable");
			if (this.maximized)
			{

			}
			if (s_window != this && s_window != null)
			{
				try
				{
					apEditor closedEditor = s_window;
					s_window = null;
					if (closedEditor != null)
					{
						closedEditor.Close();
					}
				}
				catch (Exception)
				{
					//Debug.LogError("OnEnable -> Close Exception : " + ex);
					return;
				}

			}
			s_window = this;

			autoRepaintOnSceneChange = true;


			_isFirstOnGUI = true;
			if (apEditorUtil.IsGammaColorSpace())
			{
				Notification(apVersion.I.APP_VERSION, false, false);
			}
			else
			{
				Notification(apVersion.I.APP_VERSION + " (Linear Color Space Mode)", false, false);
			}

			//Debug.Log("Add Scene Delegate");
			Undo.undoRedoPerformed += OnUndoRedoPerformed;

			//추가 3.25 : 씬이 바뀌면 작업을 초기화해야한다.

			_currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

#if UNITY_2018_1_OR_NEWER
			EditorApplication.hierarchyChanged += OnEditorHierarchyChanged;
#else
			EditorApplication.hierarchyWindowChanged += OnEditorHierarchyChanged;
#endif
			//EditorApplication.hierarchyWindowChanged += OnEditorHierarchyChanged;

			//SceneView.onSceneGUIDelegate += OnSceneViewEvent;
			//EditorApplication.modifierKeysChanged += OnKeychanged;
			PhysicsPreset.Load();
			ControlParamPreset.Load();




		}

		private void OnUndoRedoPerformed()
		{
			apEditorUtil.OnUndoRedoPerformed();


			if (_portrait != null)
			{
				//실제로 오브젝트가 복원되었거나 추가된게 사라지는 내역이 있는가
				apSelection.RestoredResult restoreResult = Select.SetAutoSelectWhenUndoPerformed(_portrait,
											_recordList_TextureData,
											_recordList_Mesh,
											_recordList_MeshGroup,
											_recordList_AnimClip,
											_recordList_ControlParam,
											_recordList_Modifier,
											_recordList_AnimTimeline,
											_recordList_AnimTimelineLayer,
											_recordList_Transform,
											_recordList_Bone);



				//if(restoreResult._isAnyRestored)
				//{
				//	Debug.LogError("Undo에서 오브젝트가 추가되거나 삭제된 것을 되돌린다.");
				//}

				//4.1 추가
				//Undo 이후에 텍스쳐가 리셋되는 문제가 있다.
				for (int iTexture = 0; iTexture < _portrait._textureData.Count; iTexture++)
				{
					apTextureData textureData = _portrait._textureData[iTexture];
					if (textureData == null)
					{
						continue;
					}
					if (textureData._image == null)
					{
						//Debug.Log("Image가 없는 경우 발견 : " + textureData._name + " / [" + textureData._assetFullPath + "]");
						if (!string.IsNullOrEmpty(textureData._assetFullPath))
						{
							//Debug.Log("저장된 경로 : " + textureData._assetFullPath);
							Texture2D restoreImage = AssetDatabase.LoadAssetAtPath<Texture2D>(textureData._assetFullPath);
							if (restoreImage != null)
							{
								//Debug.Log("이미지 복원 완료");
								textureData._image = restoreImage;
							}
						}
					}
				}


				//Mesh의 TextureData를 다시 확인해봐야한다.
				for (int iMesh = 0; iMesh < _portrait._meshes.Count; iMesh++)
				{
					apMesh mesh = _portrait._meshes[iMesh];
					if (!mesh.IsTextureDataLinked)
					{
						//Link가 풀렸다면..
						mesh.SetTextureData(_portrait.GetTexture(mesh.LinkedTextureDataID));
					}
				}



				_portrait.LinkAndRefreshInEditor(restoreResult._isAnyRestored, null);

				if (Select.SelectionType == apSelection.SELECTION_TYPE.Mesh)
				{
					if (Select.Mesh != null)
					{
						Select.Mesh.MakeOffsetPosMatrix();
						Select.Mesh.RefreshPolygonsToIndexBuffer();
					}
				}
				else if (Select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup)
				{
					if (Select.MeshGroup != null)
					{
						apMeshGroup meshGroup = Select.MeshGroup;

						if (restoreResult._isAnyRestored || _meshGroupEditMode == MESHGROUP_EDIT_MODE.Setting)
						{
							//추가 : Setting 탭에서는 무조건 Sort를 하자
							meshGroup.SortRenderUnits(true);
							meshGroup.SetDirtyToReset();
							meshGroup.SetDirtyToSort();

							Hierarchy.SetNeedReset();
						}
						meshGroup.RefreshForce(true, 0.0f);

						//추가 : 계층적으로 MeshGroup/Modifier가 연결된 경우 이게 코드들이 추가되어야 함
						meshGroup.LinkModMeshRenderUnits();
						meshGroup.RefreshModifierLink();

						//<BONE_EDIT>
						//if(Select.MeshGroup._boneList_All != null &&
						//	Select.MeshGroup._boneList_All.Count > 0)
						//{

						//	Select.MeshGroup.UpdateBonesWorldMatrix();//<<전체 갱신 5.17
						//	for (int i = 0; i < Select.MeshGroup._boneList_Root.Count; i++)
						//	{
						//		//Select.MeshGroup._boneList_Root[i].MakeWorldMatrix(true);//<<이전 코드
						//		Select.MeshGroup._boneList_Root[i].GUIUpdate(true);
						//	}

						//	Select.MeshGroup.LinkBoneListToChildMeshGroupsAndRenderUnits();
						//	Select.MeshGroup.RefreshForce(true, 0.0f);
						//}

						//>> BoneSet으로 변경
						apMeshGroup.BoneListSet boneSet = null;
						if (meshGroup._boneListSets.Count > 0)
						{
							meshGroup.UpdateBonesWorldMatrix();//<<전체 갱신 5.17

							for (int iSet = 0; iSet < meshGroup._boneListSets.Count; iSet++)
							{
								boneSet = meshGroup._boneListSets[iSet];
								for (int iRoot = 0; iRoot < boneSet._bones_Root.Count; iRoot++)
								{
									boneSet._bones_Root[iRoot].GUIUpdate(true);
								}
							}

							meshGroup.LinkBoneListToChildMeshGroupsAndRenderUnits();
							meshGroup.RefreshForce(true, 0.0f);
						}




					}
				}
				else if (Select.SelectionType == apSelection.SELECTION_TYPE.Animation)
				{
					if (Select.AnimClip != null)
					{
						apAnimClip animClip = Select.AnimClip;
						animClip.RefreshTimelines(null);//변경 19.5.21 : 전체 Refresh일 경우 null입력
						animClip.UpdateMeshGroup_Editor(true, 0.0f, true, true);

						if (animClip._targetMeshGroup != null)
						{
							apMeshGroup meshGroup = animClip._targetMeshGroup;
							if (restoreResult._isAnyRestored)
							{
								meshGroup.SortRenderUnits(true);
								meshGroup.SetDirtyToReset();
								meshGroup.SetDirtyToSort();
							}
							meshGroup.RefreshForce(true, 0.0f);

							//추가 : 계층적으로 MeshGroup/Modifier가 연결된 경우 이게 코드들이 추가되어야 함
							meshGroup.LinkModMeshRenderUnits();
							meshGroup.RefreshModifierLink();

							//<BONE_EDIT>
							//if (animClip._targetMeshGroup._boneList_All != null &&
							//	animClip._targetMeshGroup._boneList_All.Count > 0)
							//{

							//	//5.17 변경
							//	animClip._targetMeshGroup.UpdateBonesWorldMatrix();

							//	for (int i = 0; i < animClip._targetMeshGroup._boneList_Root.Count; i++)
							//	{
							//		animClip._targetMeshGroup._boneList_Root[i].GUIUpdate(true);
							//	}

							//	animClip._targetMeshGroup.LinkBoneListToChildMeshGroupsAndRenderUnits();
							//	animClip._targetMeshGroup.RefreshForce(true, 0.0f);
							//}

							//>> BoneSet으로 변경
							apMeshGroup.BoneListSet boneSet = null;
							if (meshGroup._boneListSets.Count > 0)
							{
								meshGroup.UpdateBonesWorldMatrix();//<<전체 갱신 5.17

								for (int iSet = 0; iSet < meshGroup._boneListSets.Count; iSet++)
								{
									boneSet = meshGroup._boneListSets[iSet];
									for (int iRoot = 0; iRoot < boneSet._bones_Root.Count; iRoot++)
									{
										boneSet._bones_Root[iRoot].GUIUpdate(true);
									}
								}

								meshGroup.LinkBoneListToChildMeshGroupsAndRenderUnits();
								meshGroup.RefreshForce(true, 0.0f);
							}

						}

						//추가 3.31 : 공통 커브 갱신
						Select.AutoRefreshCommonCurve();
					}
				}


				//만약 FFD 모드 중이었다면 FFD 중단
				if (Gizmos.IsFFDMode)
				{
					Gizmos.RevertTransformObjects(null);
				}

				if (restoreResult._isAnyRestored)
				{
					Hierarchy.SetNeedReset();
					//+ ID 리셋해야함
					_portrait.RefreshAllUniqueIDs();
				}

				RefreshControllerAndHierarchy(false);
				if (Select.SelectionType == apSelection.SELECTION_TYPE.Animation)
				{
					//RefreshTimelineLayers(restoreResult._isAnyRestored);//<<이걸 True로 할 때가 있는데;
					RefreshTimelineLayers(
						(restoreResult._isAnyRestored ?
						REFRESH_TIMELINE_REQUEST.All :
						REFRESH_TIMELINE_REQUEST.Timelines), null);
				}

				//자동으로 페이지 전환
				if (restoreResult._isAnyRestored)
				{
					Select.SetAutoSelectOrUnselectFromRestore(restoreResult, _portrait);
					RefreshControllerAndHierarchy(true);
				}

				OnAnyObjectAddedOrRemoved();

			}
			Repaint();


			Notification("Undo / Redo", true, false);

		}

		//----------------------------------------------------------------------------------------------------

		/// <summary>
		/// 화면내에 Notification Ballon을 출력합니다.
		/// </summary>
		/// <param name="strText"></param>
		/// <param name="_isShortTime"></param>
		public void Notification(string strText, bool _isShortTime, bool isDrawBalloon)
		{
			if (string.IsNullOrEmpty(strText))
			{
				return;

			}
			if (isDrawBalloon)
			{
				RemoveNotification();

				if(_guiContent_Notification == null)
				{
					_guiContent_Notification = apGUIContentWrapper.Make(strText, true);
				}
				else
				{
					_guiContent_Notification.SetText(strText);
				}
				ShowNotification(_guiContent_Notification.Content);
				_isNotification = true;
				if (_isShortTime)
				{
					_tNotification = NOTIFICATION_TIME_SHORT;
				}
				else
				{
					_tNotification = NOTIFICATION_TIME_LONG;
				}
			}
			else
			{
				_isNotification_GUI = true;
				if (_isShortTime)
				{
					_tNotification_GUI = NOTIFICATION_TIME_SHORT;
				}
				else
				{
					_tNotification_GUI = NOTIFICATION_TIME_LONG;
				}
				_strNotification_GUI = strText;
			}
		}

		//-------------------------------------------------------------------------------------------------
		private void OnEditorHierarchyChanged()
		{
			//Debug.Log("OnEditorHierarchyChanged");
			UnityEngine.SceneManagement.Scene curScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

			string prevScenePath = (_currentScene.path == null ? "" : _currentScene.path);
			string nextScenePath = (curScene.path == null ? "" : curScene.path);

			//Debug.Log("이전 : " + prevScenePath);
			//Debug.Log("현재 : " + nextScenePath);


			//추가 3.25 : 현재 작업 중인 Scene과 다른지 확인
			if (!prevScenePath.Equals(nextScenePath))
			{
				_currentScene = curScene;
				OnEditorSceneUnloadedOrChanged();
			}
		}

		private void OnEditorSceneUnloadedOrChanged()
		{
			try
			{
				Debug.Log("AnyPortrait : The scene you are working on has changed, so the editor is initialized.");

				Init(false);

				Controller.InitTmpValues();
				_selection.SetNone();
				_portrait = null;

				_hierarchy.ResetAllUnits();
				_hierarchy_MeshGroup.ResetSubUnits();
				_hierarchy_AnimClip.ResetSubUnits();

				_portraitsInScene.Clear();
			}
			catch (Exception ex)
			{
				Debug.LogError("AnyPortrait : Error of Changed Scene\n" + ex);
			}
		}

		//-------------------------------------------------------------------------------------------------
		private void Init(bool isShowEditor)
		{
			//_tab = TAB.ProjectSetting;
			_portrait = null;
			_portraitsInScene.Clear();
			_selection = new apSelection(this);
			_controller = new apEditorController();
			_controller.SetEditor(this);

			_hierarchy = new apEditorHierarchy(this);
			_hierarchy_MeshGroup = new apEditorMeshGroupHierarchy(this);
			_hierarchy_AnimClip = new apEditorAnimClipTargetHierarchy(this);
			_imageSet = new apImageSet();
			_materialLibrary = new apMaterialLibrary();



			_mat_Color = null;
			_mat_GUITexture = null;
			//_mat_MaskedTexture = null;
			_mat_Texture_Normal = null;
			_mat_Texture_VertAdd = null;

			_gizmos = new apGizmos(this);
			_gizmoController = new apGizmoController();
			_gizmoController.SetEditor(this);

			wantsMouseMove = true;

			_dialogShowCall = DIALOG_SHOW_CALL.None;

			//설정값을 로드하자
			LoadEditorPref();


			PhysicsPreset.Load();//<<로드!
			PhysicsPreset.Save();//그리고 바로 저장 한번 더

			ControlParamPreset.Load();
			ControlParamPreset.Save();


			_isMakePortraitRequest = false;
			_isMakePortraitRequestFromBackupFile = false;

			//이전
			_isFullScreenGUI = false;

			//변경 19.8.18
			_uiFoldType_Left = UI_FOLD_TYPE.Unfolded;
			_uiFoldType_Right1 = UI_FOLD_TYPE.Unfolded;
			_uiFoldType_Right1_Upper = UI_FOLD_TYPE.Unfolded;
			_uiFoldType_Right1_Lower = UI_FOLD_TYPE.Unfolded;
			_uiFoldType_Right2 = UI_FOLD_TYPE.Unfolded;


			apDebugLog.I.Clear();

			_meshGUIRenderMode = MESH_RENDER_MODE.Render;

			if (_meshGenerator == null)
			{
				_meshGenerator = new apMeshGenerator(this);
			}

			if (_mirrorVertexSet == null)
			{
				_mirrorVertexSet = new apMirrorVertexSet(this);
			}

			if (isShowEditor)
			{
				_isLockOnEnable = false;
			}

			_isHierarchyOrderEditEnabled = false;//<<추가 : SortMode 해제

			//추가 19.11.21
			_guiStyleWrapper = new apGUIStyleWrapper();
		}

		//private DateTime _prevDateTime = DateTime.Now;
		private float _memGC = 0.0f;

		//private DateTime _prevUpdateFrameDateTime = DateTime.Now;
		//private float _tRepaintTimer = 0.0f;
		//private const float REPAINT_MIN_TIME = 0.01f;//1/100초 이내에서는 Repaint를 하지 않는다.
		private bool _isRepaintTimerUsable = false;//<<이게 True일때만 Repaint된다. Repaint하고나면 이 값은 False가 됨
		private bool _isValidGUIRepaint = false;//<<이게 True일때 OnGUI의 Repaint 이벤트가 "제어가능한 유효한 호출"임을 나타낸다. (False일땐 유니티가 자체적으로 Repaint 한 것)

		//private bool _isDelayedFrameSkip = false;//만약 강제로 업데이트를 했다면, 그 직후엔 업데이트를 할 필요가 없다.
		//private float _tDelayedFrameSkipCount = 0.0f;
		//private const float DELAYED_FRAME_SKIP_TIME = 0.1f;

		//public void SetDelayedFrameSkip()
		//{
		//	_isDelayedFrameSkip = true;
		//	_tDelayedFrameSkipCount = 0.0f;
		//}

		void Update()
		{
			if (EditorApplication.isPlaying)
			{
				return;
			}

			//업데이트 시간과 상관없이 Update를 호출하는 시간 간격을 모두 기록한다.
			//재생 시간과 별도로 "Repaint 하지 않아도 되는 불필요한 시간"을 체크하기 위함

			//추가 3.1 : CPU 프레임이 낮아도 되는지 체크
			CheckLowCPUOption();


			//Update 타입의 타이머를 작동한다.
			if (UpdateFrameCount(FRAME_TIMER_TYPE.Update))
			{
				//동기화가 되었다.
				//Update에 의한 Repaint가 유효하다.
				_isRepaintTimerUsable = true;
			}

			//Debug.Log("Update [" + _isRepaintTimerUsable + "]");

			_memGC += DeltaTime_UpdateAllFrame;
			if (_memGC > 30.0f)
			{
				//System.GC.AddMemoryPressure(1024 * 200);//200MB 정도 압박을 줘보자
				System.GC.Collect();

				_memGC = 0.0f;
			}

			if (_isRepaintable)
			{
				//바로 Repaint : 강제로 Repaint를 하는 경우
				_isRepaintable = false;
				//_prevDateTime = DateTime.Now;

				//강제로 호출한 건 Usable의 영향을 받지 않는다.
				_isValidGUIRepaint = true;
				Repaint();
				_isRepaintTimerUsable = false;
			}
			else
			{
				if (!_isUpdateSkip)
				{
					if (_isRepaintTimerUsable)
					{
						_isValidGUIRepaint = true;
						Repaint();
					}
					_isRepaintTimerUsable = false;
				}

				_isUpdateSkip = false;


			}

			//Notification이나 없애주자
			if (_isNotification)
			{
				_tNotification -= DeltaTime_UpdateAllFrame;
				if (_tNotification < 0.0f)
				{
					_isNotification = false;
					_tNotification = 0.0f;
					RemoveNotification();
				}
			}
			if (_isNotification_GUI)
			{
				_tNotification_GUI -= DeltaTime_UpdateAllFrame;
				if (_tNotification_GUI < 0.0f)
				{
					_isNotification_GUI = false;
					_tNotification_GUI = 0.0f;
				}
			}
			//백업 레이블 애니메이션 처리를 하자
			if (_isBackupProcessing != Backup.IsAutoSaveWorking())
			{
				_isBackupProcessing = Backup.IsAutoSaveWorking();
				_tBackupProcessing_Icon = 0.0f;
				_tBackupProcessing_Label = 0.0f;
			}
			if (_isBackupProcessing)
			{
				_tBackupProcessing_Icon += DeltaTime_UpdateAllFrame;
				_tBackupProcessing_Label += DeltaTime_UpdateAllFrame;

				if (_tBackupProcessing_Icon > BACKUP_ICON_TIME_LENGTH)
				{
					_tBackupProcessing_Icon -= BACKUP_ICON_TIME_LENGTH;
				}

				if (_tBackupProcessing_Label > BACKUP_LABEL_TIME_LENGTH)
				{
					_tBackupProcessing_Label -= BACKUP_LABEL_TIME_LENGTH;
				}
			}
			//_isCountDeltaTime = false;

			//_prevUpdateFrameDateTime = DateTime.Now;

			//타이머 종료
			//UpdateFrameCount(FRAME_TIMER_TYPE.None);


		}




		void LateUpdate()
		{



			if (EditorApplication.isPlaying)
			{
				return;
			}

			//Debug.Log("a");
			//Repaint();
		}


		private void CheckEditorResources()
		{
			if (_gizmos == null)
			{
				_gizmos = new apGizmos(this);
			}
			if (_selection == null)
			{
				_selection = new apSelection(this);
			}

			//apGUIPool.Init();

			//_hierarchy.ReloadImageResources();

			if (_imageSet == null)
			{
				_imageSet = new apImageSet();
			}

			//bool isImageReload = _imageSet.ReloadImages();
			apImageSet.ReloadResult imageReloadResult = _imageSet.ReloadImages();

			//if (isImageReload)
			if (imageReloadResult == apImageSet.ReloadResult.NewLoaded)
			{
				apControllerGL.SetTexture(
					_imageSet.Get(apImageSet.PRESET.Controller_ScrollBtn),
					_imageSet.Get(apImageSet.PRESET.Controller_ScrollBtn_Recorded),
					_imageSet.Get(apImageSet.PRESET.Controller_SlotDeactive),
					_imageSet.Get(apImageSet.PRESET.Controller_SlotActive)
					);

				apTimelineGL.SetTexture(
					_imageSet.Get(apImageSet.PRESET.Anim_Keyframe),
					_imageSet.Get(apImageSet.PRESET.Anim_KeyframeDummy),
					_imageSet.Get(apImageSet.PRESET.Anim_KeySummary),
					_imageSet.Get(apImageSet.PRESET.Anim_KeySummaryMove),
					_imageSet.Get(apImageSet.PRESET.Anim_PlayBarHead),
					_imageSet.Get(apImageSet.PRESET.Anim_KeyLoopLeft),
					_imageSet.Get(apImageSet.PRESET.Anim_KeyLoopRight),
					_imageSet.Get(apImageSet.PRESET.Anim_TimelineBGStart),
					_imageSet.Get(apImageSet.PRESET.Anim_TimelineBGEnd),
					_imageSet.Get(apImageSet.PRESET.Anim_CurrentKeyframe),
					_imageSet.Get(apImageSet.PRESET.Anim_KeyframeCursor),
					_imageSet.Get(apImageSet.PRESET.Anim_EventMark),
					_imageSet.Get(apImageSet.PRESET.Anim_OnionMark),
					_imageSet.Get(apImageSet.PRESET.Anim_OnionRangeStart),
					_imageSet.Get(apImageSet.PRESET.Anim_OnionRangeEnd)
					//_imageSet.Get(apImageSet.PRESET.Anim_KeyframeMoveSrc),
					//_imageSet.Get(apImageSet.PRESET.Anim_KeyframeMove),
					//_imageSet.Get(apImageSet.PRESET.Anim_KeyframeCopy)
					);

				apAnimCurveGL.SetTexture(
					_imageSet.Get(apImageSet.PRESET.Curve_ControlPoint)
					);

				apGL.SetTexture(_imageSet.Get(apImageSet.PRESET.Physic_VertMain),
								_imageSet.Get(apImageSet.PRESET.Physic_VertConst),
								_imageSet.Get(apImageSet.PRESET.Gizmo_RigCircle)
								);
			}
			else if (imageReloadResult == apImageSet.ReloadResult.Error)
			{
				//에러가 발생했다.
				EditorUtility.DisplayDialog("Failed to load", "Failed to load editor resources.\nPlease re-install the AnyPortrait package.", "Okay");
				CloseEditor();
				return;
			}



			if (_controller == null || _controller.Editor == null)
			{
				_controller = new apEditorController();
				_controller.SetEditor(this);
			}

			if (_gizmoController == null || _gizmoController.Editor == null)
			{
				_gizmoController = new apGizmoController();
				_gizmoController.SetEditor(this);
			}

			//수정 : 폴더 변경 ("Assets/Editor/AnyPortraitTool/" => apEditorUtil.ResourcePath_Material);

			bool isResetMat = false;

			//감마 / 선형 색상 공간에 따라 Shader를 다른 것을 사용해야한다.
			bool isGammaColorSpace = apEditorUtil.IsGammaColorSpace();


			if (_mat_Color == null)
			{
				//감마/선형에 따라 다르다
				if (isGammaColorSpace)
				{
					_mat_Color = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Color.mat");
				}
				else
				{
					_mat_Color = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Color.mat");
				}

				isResetMat = true;
			}

			if (_mat_GUITexture == null)
			{
				//감마/선형에 따라 다르다
				if (isGammaColorSpace)
				{
					_mat_GUITexture = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_GUITexture.mat");
				}
				else
				{
					_mat_GUITexture = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_GUITexture.mat");
				}
				isResetMat = true;
			}


			//AlphaBlend = 0,
			//Additive = 1,
			//SoftAdditive = 2,
			//Multiplicative = 3


			if (_mat_Texture_Normal == null || _mat_Texture_Normal.Length != 4)
			{
				_mat_Texture_Normal = new Material[4];

				//감마/선형에 따라 다르다
				if (isGammaColorSpace)
				{
					_mat_Texture_Normal[0] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Texture.mat");
					_mat_Texture_Normal[1] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Texture Additive.mat");
					_mat_Texture_Normal[2] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Texture SoftAdditive.mat");
					_mat_Texture_Normal[3] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Texture Multiplicative.mat");
				}
				else
				{
					_mat_Texture_Normal[0] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Texture.mat");
					_mat_Texture_Normal[1] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Texture Additive.mat");
					_mat_Texture_Normal[2] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Texture SoftAdditive.mat");
					_mat_Texture_Normal[3] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Texture Multiplicative.mat");
				}

				isResetMat = true;
			}

			if (_mat_Texture_VertAdd == null || _mat_Texture_VertAdd.Length != 4)
			{
				_mat_Texture_VertAdd = new Material[4];

				//감마/선형에 따라 다르다
				if (isGammaColorSpace)
				{
					_mat_Texture_VertAdd[0] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Texture_VColorAdd.mat");
					_mat_Texture_VertAdd[1] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Texture_VColorAdd Additive.mat");
					_mat_Texture_VertAdd[2] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Texture_VColorAdd SoftAdditive.mat");
					_mat_Texture_VertAdd[3] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Texture_VColorAdd Multiplicative.mat");
				}
				else
				{
					_mat_Texture_VertAdd[0] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Texture_VColorAdd.mat");
					_mat_Texture_VertAdd[1] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Texture_VColorAdd Additive.mat");
					_mat_Texture_VertAdd[2] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Texture_VColorAdd SoftAdditive.mat");
					_mat_Texture_VertAdd[3] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Texture_VColorAdd Multiplicative.mat");
				}
				isResetMat = true;
			}

			//if (_mat_MaskedTexture == null || _mat_MaskedTexture.Length != 4)
			//{
			//	_mat_MaskedTexture = new Material[4];
			//	_mat_MaskedTexture[0] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_MaskedTexture.mat");
			//	_mat_MaskedTexture[1] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_MaskedTexture Additive.mat");
			//	_mat_MaskedTexture[2] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_MaskedTexture SoftAdditive.mat");
			//	_mat_MaskedTexture[3] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_MaskedTexture Multiplicative.mat");
			//	isResetMat = true;
			//}

			if (_mat_Clipped == null || _mat_Clipped.Length != 4)
			{
				_mat_Clipped = new Material[4];

				//감마/선형에 따라 다르다
				if (isGammaColorSpace)
				{
					_mat_Clipped[0] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_ClippedTexture.mat");
					_mat_Clipped[1] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_ClippedTexture Additive.mat");
					_mat_Clipped[2] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_ClippedTexture SoftAdditive.mat");
					_mat_Clipped[3] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_ClippedTexture Multiplicative.mat");
				}
				else
				{
					_mat_Clipped[0] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_ClippedTexture.mat");
					_mat_Clipped[1] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_ClippedTexture Additive.mat");
					_mat_Clipped[2] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_ClippedTexture SoftAdditive.mat");
					_mat_Clipped[3] = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_ClippedTexture Multiplicative.mat");
				}

				isResetMat = true;
			}

			if (_mat_MaskOnly == null)
			{
				//감마/선형에 따라 다르다
				if (isGammaColorSpace)
				{
					_mat_MaskOnly = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_MaskOnly.mat");
				}
				else
				{
					_mat_MaskOnly = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_MaskOnly.mat");
				}
				isResetMat = true;
			}


			if (_mat_ToneColor_Normal == null)
			{
				//감마/선형에 따라 다르다
				if (isGammaColorSpace)
				{
					_mat_ToneColor_Normal = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_ToneColor_Texture.mat");
				}
				else
				{
					_mat_ToneColor_Normal = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_ToneColor_Texture.mat");
				}
			}

			if (_mat_ToneColor_Clipped == null)
			{
				//감마/선형에 따라 다르다
				if (isGammaColorSpace)
				{
					_mat_ToneColor_Clipped = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_ToneColor_Clipped.mat");
				}
				else
				{
					_mat_ToneColor_Clipped = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_ToneColor_Clipped.mat");
				}
			}

			if (_mat_Alpha2White == null)
			{
				if (isGammaColorSpace)
				{
					_mat_Alpha2White = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "apMat_Alpha2White.mat");
				}
				else
				{
					_mat_Alpha2White = AssetDatabase.LoadAssetAtPath<Material>(apEditorUtil.ResourcePath_Material + "Linear/apMat_L_Alpha2White.mat");
				}
			}


			if (isResetMat)
			{
				Shader[] shaderSet_Normal = new Shader[4];
				Shader[] shaderSet_VertAdd = new Shader[4];
				//Shader[] shaderSet_Mask = new Shader[4];
				Shader[] shaderSet_Clipped = new Shader[4];
				for (int i = 0; i < 4; i++)
				{
					shaderSet_Normal[i] = _mat_Texture_Normal[i].shader;
					shaderSet_VertAdd[i] = _mat_Texture_VertAdd[i].shader;
					//shaderSet_Mask[i] = _mat_MaskedTexture[i].shader;
					shaderSet_Clipped[i] = _mat_Clipped[i].shader;
				}

				apGL.SetShader(_mat_Color.shader, shaderSet_Normal, shaderSet_VertAdd, /*shaderSet_Mask, */_mat_MaskOnly.shader, shaderSet_Clipped, _mat_GUITexture.shader, _mat_ToneColor_Normal.shader, _mat_ToneColor_Clipped.shader, _mat_Alpha2White.shader);

				apControllerGL.SetShader(_mat_Color.shader, shaderSet_Normal, shaderSet_VertAdd, /*shaderSet_Mask, */_mat_MaskOnly.shader, shaderSet_Clipped, _mat_GUITexture.shader, _mat_ToneColor_Normal.shader, _mat_ToneColor_Clipped.shader, _mat_Alpha2White.shader);

				apTimelineGL.SetShader(_mat_Color.shader, shaderSet_Normal, shaderSet_VertAdd, /*shaderSet_Mask, */_mat_MaskOnly.shader, shaderSet_Clipped, _mat_GUITexture.shader, _mat_ToneColor_Normal.shader, _mat_ToneColor_Clipped.shader, _mat_Alpha2White.shader);
				apAnimCurveGL.SetShader(_mat_Color.shader, shaderSet_Normal, shaderSet_VertAdd, /*shaderSet_Mask, */_mat_MaskOnly.shader, shaderSet_Clipped, _mat_GUITexture.shader, _mat_ToneColor_Normal.shader, _mat_ToneColor_Clipped.shader, _mat_Alpha2White.shader);
			}


			if (_localization == null)
			{
				_localization = new apLocalization();
			}
			if (_localization.CheckToReloadLanguage(_language))//언어 비교
			{
				//언어 다시 로드
				//경로 변경 : "Assets/Editor/AnyPortraitTool/Util/" => apEditorUtil.ResourcePath_Text
				TextAsset textAsset_Dialog = AssetDatabase.LoadAssetAtPath<TextAsset>(apEditorUtil.ResourcePath_Text + "apLangPack.txt");
				TextAsset textAsset_UI = AssetDatabase.LoadAssetAtPath<TextAsset>(apEditorUtil.ResourcePath_Text + "apLangPack_UI.txt");
				_localization.SetTextAsset(_language, textAsset_Dialog, textAsset_UI);

				if (_hierarchy != null) { _hierarchy.ResetAllUnits(); }
				if (_hierarchy_MeshGroup != null) { _hierarchy_MeshGroup.RefreshUnits(); }
				if (_hierarchy_AnimClip != null) { _hierarchy_AnimClip.RefreshUnits(); }

				//추가 19.11.22 : 한번 생성되면 언어가 고정되는 GUIContent들을 전부 리셋해야한다.
				ResetGUIContents();
				if(_selection != null)
				{
					_selection.ResetGUIContents();
				}
			}


			if (_hierarchy == null)
			{
				_hierarchy = new apEditorHierarchy(this);
				_hierarchy.ResetAllUnits();
			}

			if (_hierarchy_MeshGroup == null)
			{
				_hierarchy_MeshGroup = new apEditorMeshGroupHierarchy(this);
			}

			if (_hierarchy_AnimClip == null)
			{
				_hierarchy_AnimClip = new apEditorAnimClipTargetHierarchy(this);
			}

			_gizmos.LoadResources();


			if (_guiTopTabStaus == null || _guiTopTabStaus.Count != 6)
			{
				_guiTopTabStaus = new Dictionary<GUITOP_TAB, bool>();
				_guiTopTabStaus.Add(GUITOP_TAB.Tab1_BakeAndSetting, true);
				_guiTopTabStaus.Add(GUITOP_TAB.Tab2_TRSTools, true);
				_guiTopTabStaus.Add(GUITOP_TAB.Tab3_Visibility, true);
				_guiTopTabStaus.Add(GUITOP_TAB.Tab4_FFD_Soft_Blur, true);
				_guiTopTabStaus.Add(GUITOP_TAB.Tab5_GizmoValue, true);
				_guiTopTabStaus.Add(GUITOP_TAB.Tab6_Capture, true);
			}

			//추가 19.6.1
			if (_materialLibrary == null)
			{
				_materialLibrary = new apMaterialLibrary();
			}
			if (!_materialLibrary.IsLoaded)
			{
				_materialLibrary.Load();//Load!
				_materialLibrary.Save();
			}

			//추가 19.11.21 : GUIStyle 최적화를 위한 코드
			if (_guiStyleWrapper == null)
			{
				_guiStyleWrapper = new apGUIStyleWrapper();
			}
			if (!_guiStyleWrapper.IsInitialized())
			{
				_guiStyleWrapper.Init();
			}
		}




		// GUI 공통 입력 부분
		//--------------------------------------------------------------

		//--------------------------------------------------------------------------------------------
		void OnGUI()
		{

			if (Application.isPlaying)
			{
				int windowWidth = (int)position.width;
				int windowHeight = (int)position.height;

				EditorGUILayout.BeginVertical(GUILayout.Width((int)position.width), GUILayout.Height((int)position.height));

				GUILayout.Space((windowHeight / 2) - 10);
				EditorGUILayout.BeginHorizontal();
				GUIStyle guiStyle_CenterLabel = new GUIStyle(GUI.skin.label);//이건 최적화 대상 아님
				guiStyle_CenterLabel.alignment = TextAnchor.MiddleCenter;

				EditorGUILayout.LabelField("Unity Editor is Playing.", guiStyle_CenterLabel, GUILayout.Width((int)position.width));

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndVertical();

				_isUpdateAfterEditorRunning = true;
				return;
			}

			//int nEvent = Event.GetEventCount();
			//List<EventType> guiEvents = new List<EventType>();
			//guiEvents.Add(Event.current.type);
			//Event nextEvent = new Event();
			//for (int i = 0; i < nEvent; i++)
			//{
			//	if(!Event.PopEvent(nextEvent))
			//	{
			//		guiEvents.Add(nextEvent.type);
			//		break;
			//	}
			//}
			//string strEvent = "Events[" + nEvent+ "] : ";
			//for (int i = 0; i < guiEvents.Count; i++)
			//{
			//	strEvent += guiEvents[i] + " / ";
			//}
			//Debug.Log(strEvent);

			//Undo 체크를 할 수 있도록 만들자
			//apEditorUtil.EnableUndoFrameCheck();

			if (_isUpdateAfterEditorRunning)
			{
				Init(false);
				_isUpdateAfterEditorRunning = false;
			}

			//최신 버전을 체크한다. (1회만 수행)
			CheckCurrentLiveVersion();


			_curEventType = Event.current.type;

			if (Event.current.type == EventType.Repaint)
			{
				//_isCountDeltaTime = true;
				//GUI Repaint 타입의 타이머를 작동한다.
				UpdateFrameCount(FRAME_TIMER_TYPE.Repaint);
			}
			else
			{
				//_isCountDeltaTime = false;
				//이 호출에서는 타이머 종료
				//UpdateFrameCount(FRAME_TIMER_TYPE.None);
			}



			//언어팩 옵션 적용
			//_localization.SetLanguage(_language);


			if (_portrait != null)
			{
				//포커스가 EditorWindow에 없다면 물리 사용을 해제한다.
				_portrait._isPhysicsSupport_Editor = (EditorWindow.focusedWindow == this) && !Onion.IsVisible;
			}





			////return;
			//UnityEngine.Profiling.Profiler.BeginSample("Editor Main GUI [" + Event.current.type + "]");

			try
			{

				CheckEditorResources();
				Controller.CheckInputEvent();

				HotKey.Clear();


				if (_isFirstOnGUI)
				{
					LoadEditorPref();

					if (apVersion.I.IsDemo)
					{
						//데모인 경우 : 항상 나옴
						apDialog_StartPage.ShowDialog(this, ImageSet.Get(apImageSet.PRESET.StartPageLogo_Demo), false);
					}
					else
					{
						if (_startScreenOption_IsShowStartup)
						{
							//데모가 아닌 경우 : 옵션에 따라 완전 처음 또는 매일 한번 나온다.
							//옵션 True + 날짜가 달라야 나온다.
							if (DateTime.Now.Month != _startScreenOption_LastMonth ||
								DateTime.Now.Day != _startScreenOption_LastDay)
							{
								apDialog_StartPage.ShowDialog(this, ImageSet.Get(apImageSet.PRESET.StartPageLogo_Full), true);

								_startScreenOption_LastMonth = DateTime.Now.Month;
								_startScreenOption_LastDay = DateTime.Now.Day;

								SaveEditorPref();
							}
						}
					}

					//업데이트 로그도 띄우자
					if (_updateLogScreen_LastVersion != apVersion.I.APP_VERSION_INT)
					{
						apDialog_UpdateLog.ShowDialog(this);

						_updateLogScreen_LastVersion = apVersion.I.APP_VERSION_INT;

						SaveEditorPref();
					}



					_isFirstOnGUI = false;
				}

				//if (_mouseBtn[MOUSE_BTN_LEFT].Status == apMouse.MouseBtnStatus.Released &&
				//	_mouseBtn[MOUSE_BTN_MIDDLE].Status == apMouse.MouseBtnStatus.Released &&
				//	_mouseBtn[MOUSE_BTN_RIGHT].Status == apMouse.MouseBtnStatus.Released)
				//{
				//	_isMouseEvent = false;
				//}
				//else
				//{
				//	_isMouseEvent = true;
				//}

				//if(Event.current.isKey || Event.current.isMouse)
				//{ Repaint(); }




				//현재 GUI 이벤트가 Layout/Repaint 이벤트인지
				//또는 마우스/키보드 등의 다른 이벤트인지 판별
				if (Event.current.type != EventType.Layout
					&& Event.current.type != EventType.Repaint)
				{
					_isGUIEvent = false;
				}
				else
				{
					_isGUIEvent = true;
				}


				//자동 저장 기능
				Backup.CheckAutoBackup(this, Event.current.type);


				int windowWidth = (int)position.width;
				int windowHeight = (int)position.height;


				//int topHeight = 65;
				bool isTopVisible = true;
				bool isLeftVisible = true;
				bool isRightVisible = true;

				int topHeight = 45;

				//Bottom 레이아웃은 일부 기능에서만 나타난다.
				int bottomHeight = 0;
				bool isBottomVisible = false;

				//추가 Bottom위에 Edit 툴이 나오는 Bottom 2 Layout이 있다.
				int bottom2Height = 45;
				bool isBottom2Render = (Select.ExEditMode != apSelection.EX_EDIT_KEY_VALUE.None) && (_meshGroupEditMode == MESHGROUP_EDIT_MODE.Modifier);

				if (Select.SelectionType == apSelection.SELECTION_TYPE.Animation)
				{
					//TODO : 애니메이션의 하단 레이아웃은 Summary / 4 Line / 7 Line으로 조절 가능하다
					switch (_timelineLayoutSize)
					{
						case TIMELINE_LAYOUTSIZE.Size1:
							//bottomHeight = 250;
							bottomHeight = 200;//<더축소
							break;

						case TIMELINE_LAYOUTSIZE.Size2:
							//bottomHeight = 375;
							bottomHeight = 340;
							break;

						case TIMELINE_LAYOUTSIZE.Size3:
							bottomHeight = 500;
							break;
					}
					isBottomVisible = true;
				}


				int marginHeight = 10;
				int mainHeight = windowHeight - (topHeight + marginHeight * 2 + bottomHeight);
				if (!isBottomVisible)
				{
					mainHeight = windowHeight - (topHeight + marginHeight);
				}
				if (isBottom2Render)
				{
					mainHeight -= (marginHeight + bottom2Height);
				}

				int topWidth = windowWidth;
				int bottomWidth = windowWidth;

				int mainLeftWidth = 250;
				int mainRightWidth = 250;
				int mainRight2Width = 250;
				int mainMarginWidth = 5;
				int mainCenterWidth = windowWidth - (mainLeftWidth + mainMarginWidth * 2 + mainRightWidth);

				bool isRight2Visible = false;

				if (_isFullScreenGUI)
				{
					//< 전체 화면 모드 >
					//추가 : FullScreen이라면 -> Bottom 빼고 다 사라진다.
					//위, 양쪽의 길이는 물론이고, margin 계산도 하단 제외하고 다 빼준다.
					topHeight = 0;
					isTopVisible = false;
					isLeftVisible = false;
					isRightVisible = false;

					mainHeight = windowHeight - (marginHeight * 1 + bottomHeight);
					if (!isBottomVisible)
					{
						mainHeight = windowHeight;
					}
					if (isBottom2Render)
					{
						mainHeight -= (marginHeight + bottom2Height);
					}

					mainLeftWidth = 0;
					mainRightWidth = 0;
					mainRight2Width = 0;

					mainCenterWidth = windowWidth;
				}
				else
				{
					//< 일반 모드 >
					if (isRightVisible)
					{
						//Right 패널이 두개가 필요한 경우 (Right GUI가 보여진다는 가정하에)
						switch (Select.SelectionType)
						{
							case apSelection.SELECTION_TYPE.MeshGroup:
								isRight2Visible = true;
								break;

							case apSelection.SELECTION_TYPE.Animation:
								isRight2Visible = true;
								break;
						}
					}

					//Fold 상태에 따라서 Width가 바뀐다. 19.8/18
					if(_uiFoldType_Left == UI_FOLD_TYPE.Folded)
					{
						mainLeftWidth = 24;
					}

					if(_uiFoldType_Right1 == UI_FOLD_TYPE.Folded)
					{
						mainRightWidth = 24;
					}

					if(_uiFoldType_Right2 == UI_FOLD_TYPE.Folded)
					{
						mainRight2Width = 24;
					}

					if (isRight2Visible)
					{
						mainCenterWidth = windowWidth - (mainLeftWidth + mainMarginWidth * 3 + mainRightWidth + mainRight2Width);
					}
					else
					{
						mainCenterWidth = windowWidth - (mainLeftWidth + mainMarginWidth * 2 + mainRightWidth);
					}
				}


				

				// Layout
				//-----------------------------------------
				// Top (Tab / Toolbar)
				//-----------------------------------------
				//		|						|
				//		|			Main		|
				// Func	|		GUI Editor		| Inspector
				//		|						|
				//		|						|
				//		|						|
				//		|						|
				//-----------------------------------------
				// Bottom (Timeline / Status) : 선택
				//-----------------------------------------

				//Portrait 생성 요청이 있다면 여기서 처리
				if (_isMakePortraitRequest && _curEventType == EventType.Layout)
				{
					MakeNewPortrait();
				}
				if (_isMakePortraitRequestFromBackupFile && _curEventType == EventType.Layout)
				{
					MakePortraitFromBackupFile();
				}

				Color guiBasicColor = GUI.backgroundColor;


				// Top UI : Tab Buttons / Basic Toolbar
				//-------------------------------------------------
				GUILayout.Space(5);

				if (isTopVisible)
				{
					GUI.Box(new Rect(0, 0, topWidth, topHeight), "");

					//Profiler.BeginSample("1. GUI Top");

					GUI_Top(windowWidth, topHeight);

					//Profiler.EndSample();
					//-------------------------------------------------

					GUILayout.Space(marginHeight);
				}

				//HotKey 처리를 두자
				GUI_HotKey_TopRight();//<<렌더여부 상관 없이..

				
				//Rect mainRect_LT = GUILayoutUtility.GetLastRect();

				//-------------------------------------------------
				// Left Func + Main GUI Editor + Right Inspector
				EditorGUILayout.BeginHorizontal(GUILayout.Height(mainHeight));

				if (isLeftVisible)
				{
					// Main Left Layout
					//------------------------------------
					EditorGUILayout.BeginVertical(GUILayout.Width(mainLeftWidth));

					Rect rectMainLeft = new Rect(0, topHeight + marginHeight, mainLeftWidth, mainHeight);
					GUI.Box(rectMainLeft, "");
					GUILayout.BeginArea(rectMainLeft, "");

					//추가 19.8.18 : UI가 Fold될 수 있다. (전체 화면 모드와 다름)
					if (_uiFoldType_Left == UI_FOLD_TYPE.Folded)
					{
						//접혀진 탭을 펼칠 수 있다.
						UI_FOLD_BTN_RESULT foldResult_Left = apEditorUtil.DrawTabFoldTitle_H(this, 0, 0, mainLeftWidth, mainHeight, _uiFoldType_Left, true);
						if (foldResult_Left == UI_FOLD_BTN_RESULT.ToggleFold_Horizontal)
						{
							//Left의 Fold를 변경한다.
							_uiFoldType_Left = UI_FOLD_TYPE.Unfolded;
						}
					}
					else
					{
						//이전 : 기존엔 10짜리 여백
						//GUILayout.Space(10);

						//변경 19.8.17 : 탭 축소 버튼이 위쪽에
						UI_FOLD_BTN_RESULT foldResult_Left = apEditorUtil.DrawTabFoldTitle_H(this, 0, 0, mainLeftWidth, 9, _uiFoldType_Left, true);
						if (foldResult_Left == UI_FOLD_BTN_RESULT.ToggleFold_Horizontal)
						{
							//Left의 Fold를 변경한다.
							_uiFoldType_Left = UI_FOLD_TYPE.Folded;
						}


						EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
						GUILayout.Space(5);
						if (apEditorUtil.ToggledButton(GetUIWord(UIWORD.Hierarchy), _tabLeft == TAB_LEFT.Hierarchy, (mainLeftWidth - 16) / 2, 20))//"Hierarchy"
						{
							_tabLeft = TAB_LEFT.Hierarchy;
							_isHierarchyOrderEditEnabled = false;//<<추가 : SortMode 해제
						}
						if (apEditorUtil.ToggledButton(GetUIWord(UIWORD.Controller), _tabLeft == TAB_LEFT.Controller, (mainLeftWidth - 16) / 2, 20))//"Controller"
						{
							_tabLeft = TAB_LEFT.Controller;
							_isHierarchyOrderEditEnabled = false;//<<추가 : SortMode 해제
						}

						EditorGUILayout.EndHorizontal();

						GUILayout.Space(10);

						int leftUpperHeight = GUI_MainLeft_Upper(mainLeftWidth - 20);

						//int leftHeight = mainHeight - ((20 - 40) + leftUpperHeight);

						_scroll_MainLeft = EditorGUILayout.BeginScrollView(_scroll_MainLeft, false, true);

						EditorGUILayout.BeginVertical(GUILayout.Width(mainLeftWidth - 20), GUILayout.Height(mainHeight - ((20 - 40) + leftUpperHeight)));

						apControllerGL.SetLayoutSize(mainLeftWidth - 20,
											//mainHeight - ((20 - 40) + leftUpperHeight + 60),
											mainHeight - ((20 - 40) + leftUpperHeight + 66),
											(int)rectMainLeft.x,
											//(int)rectMainLeft.y + leftUpperHeight + 38,
											(int)rectMainLeft.y + leftUpperHeight + 39,
											(int)position.width, (int)position.height, _scroll_MainLeft);

						//ControllerGL의 Snap여부를 결정하자.
						//일반적으로는 True이지만, ControlParam을 제어하는 AnimTimeline 작업시에는 False가 된다.
						//bool isAnimControlParamEditing = false;

						if (_selection.SelectionType == apSelection.SELECTION_TYPE.Animation &&
								_selection.AnimClip != null &&
								_selection.ExAnimEditingMode != apSelection.EX_EDIT.None &&
								_selection.AnimTimeline != null &&
								_selection.AnimTimeline._linkType == apAnimClip.LINK_TYPE.ControlParam)
						{
							apControllerGL.SetSnapWhenReleased(false);
						}
						else
						{
							apControllerGL.SetSnapWhenReleased(true);
						}

						//Profiler.BeginSample("2. GUI Left");

						GUI_MainLeft(mainLeftWidth - 20, mainHeight - 20, _scroll_MainLeft, _isGUIEvent);

						//Profiler.EndSample();

						apControllerGL.EndUpdate();

						//switch (_tab)
						//{
						//	case TAB.ProjectSetting: GUI_MainLeft_ProjectSetting(mainLeftWidth - 20, mainHeight - 20); break;
						//	case TAB.MeshEdit: GUI_MainLeft_MeshEdit(mainLeftWidth - 20, mainHeight - 20); break;
						//	case TAB.FaceEdit: GUI_MainLeft_FaceEdit(mainLeftWidth - 20, mainHeight - 20); break;
						//	case TAB.RigEdit: GUI_MainLeft_RigEdit(mainLeftWidth - 20, mainHeight - 20); break;
						//	case TAB.Animation: GUI_MainLeft_Animation(mainLeftWidth - 20, mainHeight - 20); break;
						//}
						EditorGUILayout.EndVertical();

						GUILayout.Space(50);

						EditorGUILayout.EndScrollView();
					}

					GUILayout.EndArea();

					EditorGUILayout.EndVertical();


					//------------------------------------


					GUILayout.Space(mainMarginWidth);
				}

				//Profiler.BeginSample("3. GUI Center");

				// Main Center Layout
				//------------------------------------
				//Rect mainRect_LB = GUILayoutUtility.GetLastRect();
				EditorGUILayout.BeginVertical();

				int guiViewBtnSize = 15;

				GUI.backgroundColor = _colorOption_Background;
				Rect rectMainCenter = new Rect(mainLeftWidth + mainMarginWidth, topHeight + marginHeight, mainCenterWidth, mainHeight);
				if (_isFullScreenGUI)
				{
					//전체 화면일때 화면 구성이 바뀐다.
					rectMainCenter = new Rect(0, 0, mainCenterWidth, mainHeight);
				}



				GUI.Box(new Rect(rectMainCenter.x, rectMainCenter.y, rectMainCenter.width - guiViewBtnSize, rectMainCenter.height - guiViewBtnSize), "", apEditorUtil.WhiteGUIStyle_Box);
				GUILayout.BeginArea(rectMainCenter, "");

				GUI.backgroundColor = guiBasicColor;
				//_scroll_MainCenter = EditorGUILayout.BeginScrollView(_scroll_MainCenter, true, true);

				//_scroll_MainCenter.y = GUI.VerticalScrollbar(new Rect(rectMainCenter.x + mainCenterWidth - 20, rectMainCenter.width, 20, rectMainCenter.height), _scroll_MainCenter.y, 1.0f, -20.0f, 20.0f);

				_scroll_MainCenter.y = GUI.VerticalScrollbar(new Rect(rectMainCenter.width - 15, 0, 20, rectMainCenter.height - guiViewBtnSize), _scroll_MainCenter.y, 5.0f, -500.0f, 500.0f + 5.0f);
				_scroll_MainCenter.x = GUI.HorizontalScrollbar(new Rect(guiViewBtnSize + 110, rectMainCenter.height - 15, rectMainCenter.width - (guiViewBtnSize + guiViewBtnSize + 110), 20), _scroll_MainCenter.x, 5.0f, -500.0f, 500.0f + 5.0f);

				//이전 > 사용 안함
				//GUIStyle guiStyle_GUIViewBtn = new GUIStyle(GUI.skin.button);
				//guiStyle_GUIViewBtn.padding = GUI.skin.label.padding;


				//화면 중심으로 이동하는 버튼
				if (GUI.Button(new Rect(rectMainCenter.width - guiViewBtnSize, rectMainCenter.height - guiViewBtnSize, guiViewBtnSize, guiViewBtnSize),
								new GUIContent(_imageSet.Get(apImageSet.PRESET.GUI_Center), "Reset Zoom and Position"),
								//guiStyle_GUIViewBtn,//이전
								GUIStyleWrapper.Button_LabelPadding//변경
								))
				{
					_scroll_MainCenter = Vector2.zero;
					_iZoomX100 = ZOOM_INDEX_DEFAULT;
				}

				//전체 화면 기능 추가
				Color prevColor = GUI.backgroundColor;

				if (_isFullScreenGUI)
				{
					GUI.backgroundColor = new Color(prevColor.r * 0.2f, prevColor.g * 0.8f, prevColor.b * 1.1f, 1.0f);
				}


				//Alt+W를 눌러서 크기를 바꿀 수 있다.
				AddHotKeyEvent(OnHotKeyEvent_FullScreenToggle, "Toggle Workspace Size", KeyCode.W, false, true, false, null);

				if (GUI.Button(new Rect(0, rectMainCenter.height - guiViewBtnSize, guiViewBtnSize, guiViewBtnSize),
					new GUIContent(_imageSet.Get(apImageSet.PRESET.GUI_FullScreen), "Toogle Workspace Size (Alt+W)"),
					//guiStyle_GUIViewBtn
					GUIStyleWrapper.Button_LabelPadding//변경
					))
				{
					_isFullScreenGUI = !_isFullScreenGUI;
				}
				GUI.backgroundColor = prevColor;

				//추가 : GUI_Top에 있었던 Zoom이 여기로 왔다.
				int minZoom = 0;
				int maxZoom = _zoomListX100.Length - 1;

				//float fZoom = GUILayout.HorizontalSlider(_iZoomX100, minZoom, maxZoom + 0.5f, GUILayout.Width(60));
				float fZoom = GUI.HorizontalSlider(new Rect(guiViewBtnSize + 5, rectMainCenter.height - (guiViewBtnSize + 1), 40, guiViewBtnSize), _iZoomX100, minZoom, maxZoom + 0.5f);
				_iZoomX100 = Mathf.Clamp((int)fZoom, 0, _zoomListX100.Length - 1);

				GUI.Label(new Rect(guiViewBtnSize + 50, rectMainCenter.height - guiViewBtnSize, 60, guiViewBtnSize), _zoomListX100[_iZoomX100] + "%");

				//줌 관련 처리를 해보자
				//bool isZoomControlling = Controller.GUI_Input_ZoomAndScroll();

				apGL.ResetCursorEvent();


				apGL.SetWindowSize(mainCenterWidth, mainHeight,
									_scroll_MainCenter,
									(float)(_zoomListX100[_iZoomX100]) * 0.01f,
									(int)rectMainCenter.x, (int)rectMainCenter.y,
									(int)position.width, (int)position.height);


				Controller.GUI_Input_ZoomAndScroll(
#if UNITY_EDITOR_OSX
					Event.current.command,
#else
					Event.current.control,
#endif
					Event.current.shift,
					Event.current.alt
					);


				EditorGUILayout.BeginVertical(GUILayout.Width(mainCenterWidth - 20), GUILayout.Height(mainHeight - 20));

				_mainGUIRect = new Rect(rectMainCenter.x, rectMainCenter.y, rectMainCenter.width - 20, rectMainCenter.height - 20);

				//추가 4.10. 만약, Capture도중이면 화면 이동/줌이 강제다
				if (_isScreenCaptureRequest
#if UNITY_EDITOR_OSX
					|| _isScreenCaptureRequest_OSXReady
#endif
					)
				{
					if (_screenCaptureRequest != null)
					{
						_scroll_MainCenter = _screenCaptureRequest._screenPosition;
						_iZoomX100 = _screenCaptureRequest._screenZoomIndex;
					}
				}




				//CheckGizmoAvailable();
				if (Event.current.type == EventType.Repaint || Event.current.isMouse || Event.current.isKey)
				{
					_gizmos.ReadyToUpdate();
				}

				GUI_MainCenter(mainCenterWidth - 20, mainHeight - 20);

				if (Event.current.type == EventType.Repaint ||
					Event.current.isMouse ||
					Event.current.isKey ||
					_gizmos.IsDrawFlag)
				{
					_gizmos.EndUpdate();

					_gizmos.GUI_Render_Controller(this);
				}
				if (Event.current.type == EventType.Repaint)
				{
					_gizmos.CheckUpdate(DeltaTime_Repaint);
				}
				else
				{
					_gizmos.CheckUpdate(0.0f);
				}




				EditorGUILayout.EndVertical();
				//EditorGUILayout.EndScrollView();


				GUILayout.EndArea();

				EditorGUILayout.EndVertical();

				//------------------------------------

				//Profiler.EndSample();


				if (isRightVisible)
				{
					GUILayout.Space(mainMarginWidth);


					// Main Right Layout
					//------------------------------------

					//Profiler.BeginSample("4. GUI Right");

					EditorGUILayout.BeginVertical(GUILayout.Width(mainRightWidth));

					GUI.backgroundColor = guiBasicColor;

					bool isRightUpper_Scroll = true;

					bool isRightLower = false;
					bool isRightLower_Scroll = false;
					int rightLowerScrollType = -1;//0 : MeshGroup - Mesh, 1 : MeshGroup - Bone, 2 : Animation
					bool isRightLower_2LineHeader = false;

					int rightUpperHeight = mainHeight;
					int rightLowerHeight = 0;
					
					//Mesh Group / Animation인 경우 우측의 레이아웃이 2개가 된다.
					if (_uiFoldType_Right1 == UI_FOLD_TYPE.Unfolded
						&& 
						(Select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup || Select.SelectionType == apSelection.SELECTION_TYPE.Animation)
						)
					{
						isRightLower = true;
						//if (_rightLowerLayout == RIGHT_LOWER_LAYOUT.Hide)//이전
						if(_uiFoldType_Right1_Lower == UI_FOLD_TYPE.Folded)//변경 19.8.18
						{
							//하단이 Hide일때
							//if (_right_UpperLayout == RIGHT_UPPER_LAYOUT.Show)//이전
							if(_uiFoldType_Right1_Upper == UI_FOLD_TYPE.Unfolded)//변경 19.8.18
							{
								//상단이 Show일때 : 위에만 보일때
								//rightUpperHeight = mainHeight - (36 + marginHeight);
								rightUpperHeight = mainHeight - (15 + marginHeight);
							}
							else
							{
								//상단이 Hide일때 : 둘다 안보일때
								//rightUpperHeight = 36;
								rightUpperHeight = 15;
								isRightUpper_Scroll = false;
							}


							//rightLowerHeight = 36;
							rightLowerHeight = 15;

							//변경 19.8.18 : 상하로 UI가 축소되었을때의 크기가 36에서 15로 변경

							isRightLower_Scroll = false;
							isRightLower_2LineHeader = false;
						}
						else
						{
							//하단이 Show일때
							//if (_right_UpperLayout == RIGHT_UPPER_LAYOUT.Show)//이전
							if(_uiFoldType_Right1_Upper == UI_FOLD_TYPE.Unfolded)//변경 19.8.18
							{
								//상단이 Show일때 : 둘다 보일 때 > 반반
								rightUpperHeight = mainHeight / 2;
							}
							else
							{
								//상단이 Hide일때 : 아래만 보일때
								//rightUpperHeight = 36;
								rightUpperHeight = 15;
								isRightUpper_Scroll = false;
							}

							//변경 19.8.18 : 상하로 UI가 축소되었을때의 크기가 36에서 9로 변경

							rightLowerHeight = mainHeight - (rightUpperHeight + marginHeight);
							isRightLower_Scroll = true;
							rightLowerScrollType = 2;//<<Animation

							if (Select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup)
							{	
								if(_meshGroupEditMode == MESHGROUP_EDIT_MODE.Setting || _meshGroupEditMode == MESHGROUP_EDIT_MODE.Bone)
								{
									isRightLower_2LineHeader = true;
								}
								else
								{
									isRightLower_2LineHeader = false;
								}
								
								if (Select._meshGroupChildHierarchy == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes)
								{
									rightLowerScrollType = 0;
								}
								else
								{
									rightLowerScrollType = 1;
								}
							}
							else
							{
								isRightLower_2LineHeader = false;
							}
						}
					}

					Rect recMainRight = new Rect(mainLeftWidth + mainMarginWidth * 2 + mainCenterWidth, topHeight + marginHeight, mainRightWidth, rightUpperHeight);
					GUI.Box(recMainRight, "");
					GUILayout.BeginArea(recMainRight, "");
					if (_uiFoldType_Right1 == UI_FOLD_TYPE.Folded)
					{
						//Right1 UI가 모두 접힌 경우
						//변경 19.8.17 : 탭 축소 버튼이 위쪽에
						UI_FOLD_BTN_RESULT foldResult_Right = apEditorUtil.DrawTabFoldTitle_HV(this, 0, 0, mainRightWidth, rightUpperHeight, _uiFoldType_Right1, _uiFoldType_Right1_Upper);
						if (foldResult_Right == UI_FOLD_BTN_RESULT.ToggleFold_Horizontal)
						{
							//Right의 Fold를 변경한다.
							_uiFoldType_Right1 = UI_FOLD_TYPE.Unfolded;
						}
					}
					else
					{
						//변경 19.8.17 : 탭 축소 버튼이 위쪽에
						UI_FOLD_BTN_RESULT foldResult_RightUpper = UI_FOLD_BTN_RESULT.None;
						if(isRightLower)
						{
							//세로로 접기 버튼도 존재
							foldResult_RightUpper = apEditorUtil.DrawTabFoldTitle_HV(this, 0, 0, mainRightWidth, (!isRightUpper_Scroll ? rightUpperHeight : 9), _uiFoldType_Right1, _uiFoldType_Right1_Upper);
						}
						else
						{
							//가로 버튼만 존재
							foldResult_RightUpper = apEditorUtil.DrawTabFoldTitle_H(this, 0, 0, mainRightWidth, (!isRightUpper_Scroll ? rightUpperHeight : 9), _uiFoldType_Right1, false);
						}

						if (foldResult_RightUpper == UI_FOLD_BTN_RESULT.ToggleFold_Horizontal)
						{
							//Right의 Fold를 변경한다.
							_uiFoldType_Right1 = UI_FOLD_TYPE.Folded;
						}
						else if (foldResult_RightUpper == UI_FOLD_BTN_RESULT.ToggleFold_Vertical)
						{
							//Right Upper의 Fold를 변경한다.
							_uiFoldType_Right1_Upper = (_uiFoldType_Right1_Upper == UI_FOLD_TYPE.Folded ? UI_FOLD_TYPE.Unfolded : UI_FOLD_TYPE.Folded);
						}
						
						if (isRightUpper_Scroll)
						{
							EditorGUILayout.BeginVertical(GUILayout.Width(mainRightWidth - 20), GUILayout.Height(30));
							//EditorGUILayout.BeginVertical(GUILayout.Width(mainRightWidth - 20), GUILayout.Height(35));
							//GUILayout.Space(6);
						
							GUI_MainRight_Header(mainRightWidth - 8, 20);
							//GUI_MainRight_Header(mainRightWidth - 8, 25);
							EditorGUILayout.EndVertical();

							_scroll_MainRight = EditorGUILayout.BeginScrollView(_scroll_MainRight, false, true);

							EditorGUILayout.BeginVertical(GUILayout.Width(mainRightWidth - 20), GUILayout.Height(rightUpperHeight - (20 + 30)));

							GUI_MainRight(mainRightWidth - 24, rightUpperHeight - (20 + 30));
							#region [미사용 코드]
							//switch (_tab)
							//{
							//	case TAB.ProjectSetting: GUI_MainRight_ProjectSetting(mainRightWidth - 24, mainHeight - 20); break;
							//	case TAB.MeshEdit: GUI_MainRight_MeshEdit(mainRightWidth - 24, mainHeight - 20); break;
							//	case TAB.FaceEdit: GUI_MainRight_FaceEdit(mainRightWidth - 24, mainHeight - 20); break;
							//	case TAB.RigEdit: GUI_MainRight_RigEdit(mainRightWidth - 24, mainHeight - 20); break;
							//	case TAB.Animation: GUI_MainRight_Animation(mainRightWidth - 24, mainHeight - 20); break;
							//} 
							#endregion
							EditorGUILayout.EndVertical();

							GUILayout.Space(500);

							EditorGUILayout.EndScrollView();
						}
					}
					GUILayout.EndArea();


					if (isRightLower)
					{
						GUILayout.Space(marginHeight);

						Rect recMainRight_Lower = new Rect(mainLeftWidth + mainMarginWidth * 2 + mainCenterWidth,
															topHeight + marginHeight + rightUpperHeight + marginHeight,
															mainRightWidth,
															rightLowerHeight);

						GUI.Box(recMainRight_Lower, "");
						GUILayout.BeginArea(recMainRight_Lower, "");

						//추가 19.8.17 : 탭 축소 버튼이 위쪽에
						UI_FOLD_BTN_RESULT foldResult_RightLower = apEditorUtil.DrawTabFoldTitle_V(this, 0, 0, mainRightWidth, (!isRightLower_Scroll ? rightLowerHeight : 9), _uiFoldType_Right1_Lower);
						if(foldResult_RightLower == UI_FOLD_BTN_RESULT.ToggleFold_Vertical)
						{
							//Right Upper의 Fold를 변경한다.
							_uiFoldType_Right1_Lower = (_uiFoldType_Right1_Lower == UI_FOLD_TYPE.Folded ? UI_FOLD_TYPE.Unfolded : UI_FOLD_TYPE.Folded);
						}

						if (isRightLower_Scroll)
						{
							int rightLowerHeaderHeight = 30;
							if (isRightLower_2LineHeader)
							{
								rightLowerHeaderHeight = 65;
							}

							EditorGUILayout.BeginVertical(GUILayout.Width(mainRightWidth - 20), GUILayout.Height(rightLowerHeaderHeight));
							GUILayout.Space(6);
							if (Select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup)
							{
								GUI_MainRight_Lower_MeshGroupHeader(mainRightWidth - 8, rightLowerHeaderHeight - 6, isRightLower_2LineHeader);
							}
							else if (Select.SelectionType == apSelection.SELECTION_TYPE.Animation)
							{
								GUI_MainRight_Lower_AnimationHeader(mainRightWidth - 8, rightLowerHeaderHeight - 6);
							}
							EditorGUILayout.EndVertical();

							Vector2 lowerScrollValue = _scroll_MainRight_Lower_MG_Mesh;
							if (rightLowerScrollType == 0)
							{
								_scroll_MainRight_Lower_MG_Mesh = EditorGUILayout.BeginScrollView(_scroll_MainRight_Lower_MG_Mesh, false, true);
								lowerScrollValue = _scroll_MainRight_Lower_MG_Mesh;
							}
							else if (rightLowerScrollType == 1)
							{
								_scroll_MainRight_Lower_MG_Bone = EditorGUILayout.BeginScrollView(_scroll_MainRight_Lower_MG_Bone, false, true);
								lowerScrollValue = _scroll_MainRight_Lower_MG_Bone;
							}
							else
							{
								_scroll_MainRight_Lower_Anim = EditorGUILayout.BeginScrollView(_scroll_MainRight_Lower_Anim, false, true);
								lowerScrollValue = _scroll_MainRight_Lower_Anim;
							}


							EditorGUILayout.BeginVertical(GUILayout.Width(mainRightWidth - 20), GUILayout.Height(rightLowerHeight - 20));

							GUI_MainRight_Lower(mainRightWidth - 24, rightLowerHeight - 20, lowerScrollValue, _isGUIEvent);

							EditorGUILayout.EndVertical();

							GUILayout.Space(500);

							EditorGUILayout.EndScrollView();
						}
						GUILayout.EndArea();
					}


					EditorGUILayout.EndVertical();



					//------------------------------------
					if (isRight2Visible)
					{
						//SetGUIVisible("Right2GUI", true);
						SetGUIVisible(DELAYED_UI_TYPE.Right2GUI, true);
					}
					else
					{
						//SetGUIVisible("Right2GUI", false);
						SetGUIVisible(DELAYED_UI_TYPE.Right2GUI, false);
					}

					//if (IsDelayedGUIVisible("Right2GUI"))
					if (IsDelayedGUIVisible(DELAYED_UI_TYPE.Right2GUI))
					{
						GUILayout.Space(mainMarginWidth);

						// Main Right Layout
						//------------------------------------
						EditorGUILayout.BeginVertical(GUILayout.Width(mainRight2Width));

						GUI.backgroundColor = guiBasicColor;

						Rect recMainRight2 = new Rect(mainLeftWidth + mainMarginWidth * 3 + mainCenterWidth + mainRightWidth, topHeight + marginHeight, mainRight2Width, mainHeight);
						GUI.Box(recMainRight2, "");
						GUILayout.BeginArea(recMainRight2, "");

						if (_uiFoldType_Right2 == UI_FOLD_TYPE.Folded)
						{
							//탭 축소에서 펼치기
							UI_FOLD_BTN_RESULT foldResult_Right2 = apEditorUtil.DrawTabFoldTitle_H(this, 0, 0, mainRight2Width, mainHeight, _uiFoldType_Right2, false);
							if (foldResult_Right2 == UI_FOLD_BTN_RESULT.ToggleFold_Horizontal)
							{
								//Right의 Fold를 변경한다.
								_uiFoldType_Right2 = UI_FOLD_TYPE.Unfolded;
							}
						}
						else
						{
							//변경 19.8.17 : 탭 축소 버튼이 위쪽에
							UI_FOLD_BTN_RESULT foldResult_Right2 = apEditorUtil.DrawTabFoldTitle_H(this, 0, 0, mainRight2Width, 9, _uiFoldType_Right2, false);
							if (foldResult_Right2 == UI_FOLD_BTN_RESULT.ToggleFold_Horizontal)
							{
								//Right의 Fold를 변경한다.
								_uiFoldType_Right2 = UI_FOLD_TYPE.Folded;
							}

							_scroll_MainRight2 = EditorGUILayout.BeginScrollView(_scroll_MainRight2, false, true);

							EditorGUILayout.BeginVertical(GUILayout.Width(mainRight2Width - 20), GUILayout.Height(mainHeight - 20));

							GUI_MainRight2(mainRight2Width - 24, mainHeight - 20);

							EditorGUILayout.EndVertical();

							GUILayout.Space(500);

							EditorGUILayout.EndScrollView();
						}
						GUILayout.EndArea();


						EditorGUILayout.EndVertical();
					}

					//Profiler.EndSample();//Profiler GUI Right
				}

				EditorGUILayout.EndHorizontal();

				//Profiler.BeginSample("5. GUI Bottom");
				//-------------------------------------------------
				if (isBottom2Render)
				{
					GUILayout.Space(marginHeight);

					EditorGUILayout.BeginVertical(GUILayout.Height(bottom2Height));

					Rect rectBottom2 = new Rect(0, topHeight + marginHeight * 2 + mainHeight, bottomWidth, bottom2Height);
					if (_isFullScreenGUI)
					{
						//전체 화면인 경우 Top을 제외
						rectBottom2 = new Rect(0, marginHeight * 1 + mainHeight, bottomWidth, bottom2Height);
					}

					//Color prevGUIColor = GUI.backgroundColor;
					//if(Select.IsExEditing)
					//{
					//	GUI.backgroundColor = new Color(prevGUIColor.r * 1.5f, prevGUIColor.g * 0.5f, prevGUIColor.b * 0.5f, 1.0f);
					//}

					GUI.Box(rectBottom2, "");

					//GUI.backgroundColor = prevGUIColor;
					GUILayout.BeginArea(rectBottom2, "");

					GUILayout.Space(4);
					EditorGUILayout.BeginHorizontal(GUILayout.Width(bottomWidth), GUILayout.Height(bottom2Height - 10));

					GUI_Bottom2(bottomWidth, bottom2Height);

					EditorGUILayout.EndHorizontal();

					GUILayout.EndArea();

					EditorGUILayout.EndVertical();
				}

				if (isBottomVisible)
				{

					bool isBottomScroll = true;
					switch (Select.SelectionType)
					{
						case apSelection.SELECTION_TYPE.MeshGroup:
							isBottomScroll = false;
							break;

						case apSelection.SELECTION_TYPE.Animation:
							isBottomScroll = false;//<<스크롤은 따로 합니다.
							break;

						default:
							break;
					}

					GUILayout.Space(marginHeight);


					// Bottom Layout (Timeline / Status)
					//-------------------------------------------------
					EditorGUILayout.BeginVertical(GUILayout.Height(bottomHeight));

					int bottomPosY = topHeight + marginHeight * 2 + mainHeight;
					if (_isFullScreenGUI)
					{
						//전체화면인 경우 Top을 제외
						bottomPosY = marginHeight * 1 + mainHeight;
					}
					if (isBottom2Render)
					{
						bottomPosY += marginHeight + bottom2Height;
					}

					Rect rectBottom = new Rect(0, bottomPosY, bottomWidth, bottomHeight);



					GUI.Box(rectBottom, "");
					GUILayout.BeginArea(rectBottom, "");



					if (isBottomScroll)
					{
						_scroll_Bottom = EditorGUILayout.BeginScrollView(_scroll_Bottom, false, true);

						EditorGUILayout.BeginVertical(GUILayout.Width(bottomWidth - 20), GUILayout.Height(bottomHeight - 20));

						GUI_Bottom(bottomWidth - 20, bottomHeight - 20, (int)rectBottom.x, (int)rectBottom.y, (int)position.width, (int)position.height);

						EditorGUILayout.EndVertical();
						EditorGUILayout.EndScrollView();
					}
					else
					{


						EditorGUILayout.BeginVertical(GUILayout.Width(bottomWidth), GUILayout.Height(bottomHeight));

						GUI_Bottom(bottomWidth, bottomHeight, (int)rectBottom.x, (int)rectBottom.y, (int)position.width, (int)position.height);

						EditorGUILayout.EndVertical();
					}
					GUILayout.EndArea();

					EditorGUILayout.EndVertical();
				}

				//-------------------------------------------------
				//Profiler.EndSample();

				//Event e = Event.current;

				if (EditorWindow.focusedWindow == this)
				{

					if (Event.current.type == EventType.KeyDown ||
						Event.current.type == EventType.KeyUp)
					{
						if (GUIUtility.keyboardControl == 0)
						{
							//Down 이벤트 중심으로 변경
							//Up은 키 해제
							//Debug.Log("KeyDown : " + Event.current.keyCode + " + Ctrl : " + Event.current.control);
							//추가 3.24 : Ctrl (Command), Shift, Alt키가 먼저 눌렸다면
#if UNITY_EDITOR_OSX
							bool isCtrl = Event.current.command;
#else
							bool isCtrl = Event.current.control;
#endif

							bool isShift = Event.current.shift;
							bool isAlt = Event.current.alt;

							bool isHotKeyAction = false;

							apHotKey.HotKeyEvent hotkeyEvent = HotKey.OnKeyEvent(Event.current.keyCode, isCtrl, isShift, isAlt, Event.current.type == EventType.KeyDown);

							if (hotkeyEvent != null)
							{
								string strHotKeyEvent = "[ " + hotkeyEvent._label + " ] - ";


								if (hotkeyEvent._isCtrl)
								{
#if UNITY_EDITOR_OSX
									strHotKeyEvent += "Command+";//<<Mac용
#else
									strHotKeyEvent += "Ctrl+";
#endif
								}
								if (hotkeyEvent._isAlt)
								{
									strHotKeyEvent += "Alt+";
								}
								if (hotkeyEvent._isShift)
								{
									strHotKeyEvent += "Shift+";
								}

								strHotKeyEvent += hotkeyEvent._keyCode;
								Notification(strHotKeyEvent, true, false);
								isHotKeyAction = true;
							}


							if (isHotKeyAction)
							{
								//키 이벤트를 사용했다고 알려주자
								Event.current.Use();
								//Repaint();
							}
						}
					}

					//추가 : apGL의 DelayedCursor이벤트를 처리한다.
					apGL.ProcessDelayedCursor();
				}

			}
			catch (Exception ex)
			{
				if (ex is UnityEngine.ExitGUIException)
				{
					//걍 리턴
					//return;
					//무시하자
				}
				else
				{
					Debug.LogError("Exception : " + ex);
				}

			}



			//UnityEngine.Profiling.Profiler.EndSample();

			//_isCountDeltaTime = false;
			//타이머 종료
			UpdateFrameCount(FRAME_TIMER_TYPE.None);

			//GUIPool에서 사용했던 리소스들 모두 정리
			//apGUIPool.Reset();

			//일부 Dialog는 OnGUI가 지나고 호출해야한다.
			if (_dialogShowCall != DIALOG_SHOW_CALL.None && _curEventType == EventType.Layout)
			{
				try
				{
					//Debug.Log("Show Dialog [" + _curEventType + "]");
					switch (_dialogShowCall)
					{
						case DIALOG_SHOW_CALL.Bake:
							apDialog_Bake.ShowDialog(this, _portrait);
							break;

						case DIALOG_SHOW_CALL.Setting:
							apDialog_PortraitSetting.ShowDialog(this, _portrait);
							break;

						case DIALOG_SHOW_CALL.Capture:
							{
								if (apVersion.I.IsDemo)
								{
									EditorUtility.DisplayDialog(
										GetText(TEXT.DemoLimitation_Title),
										GetText(TEXT.DemoLimitation_Body),
										GetText(TEXT.Okay));
								}
								else
								{
									apDialog_CaptureScreen.ShowDialog(this, _portrait);
								}
							}

							break;

					}

				}
				catch (Exception ex)
				{
					Debug.LogError("Dialog Call Exception : " + ex);
				}
				_dialogShowCall = DIALOG_SHOW_CALL.None;
			}

			//Portrait 생성 요청이 들어왔을 때 => Repaint 이벤트 외의 루틴에서 함수를 연산해준다.

		}

		//--------------------------------------------------------------

		private bool _isGizmoGUIVisible_VTF_FFD_Prev = false;
		// Top UI : Tab Buttons / Basic Toolbar
		private void GUI_Top(int width, int height)
		{
			Texture2D imbTabOpen = ImageSet.Get(apImageSet.PRESET.ToolBtn_TabOpen);
			Texture2D imbTabFolded = ImageSet.Get(apImageSet.PRESET.ToolBtn_TabFolded);


			EditorGUILayout.BeginHorizontal(GUILayout.Height(height - 10));

			GUILayout.Space(5);




			if (_portrait == null)
			{
				GUILayout.Space(15);
				EditorGUILayout.BeginVertical(GUILayout.Width(250));
				EditorGUILayout.LabelField(GetUIWord(UIWORD.SelectPortraitFromScene), GUILayout.Width(200));//"Select Portrait From Scene"
				apPortrait nextPortrait = EditorGUILayout.ObjectField(_portrait, typeof(apPortrait), true, GUILayout.Width(200)) as apPortrait;

				//바뀌었다.
				if (_portrait != nextPortrait && nextPortrait != null)
				{
					if (nextPortrait._isOptimizedPortrait)
					{
						//Optimized Portrait는 편집이 불가능하다
						EditorUtility.DisplayDialog(GetText(TEXT.OptPortrait_LoadError_Title),
														GetText(TEXT.OptPortrait_LoadError_Body),
														GetText(TEXT.Okay));

					}
					else
					{
						_portrait = nextPortrait;//선택!

						Controller.InitTmpValues();
						_selection.SetPortrait(_portrait);

						//Portrait의 레퍼런스들을 연결해주자
						Controller.PortraitReadyToEdit();

						SyncHierarchyOrders();

						_hierarchy.ResetAllUnits();
						_hierarchy_MeshGroup.ResetSubUnits();
						_hierarchy_AnimClip.ResetSubUnits();

						//시작은 RootUnit
						_selection.SetOverallDefault();

						OnAnyObjectAddedOrRemoved();
					}
				}
				EditorGUILayout.EndVertical();

				GUILayout.Space(20);

				EditorGUILayout.EndHorizontal();

				return;
			}
			else//if (_portrait != null)
			{
				//>>>>>>>>>>>> Tab1_Bake And Setting >>>>>>>>>>>>>>>>>>>
				bool isGUITab_BakeAndSetting = DrawAndCheckGUITopTab(GUITOP_TAB.Tab1_BakeAndSetting, imbTabOpen, imbTabFolded, height - 10);

				if (isGUITab_BakeAndSetting)
				{
					EditorGUILayout.BeginVertical(GUILayout.Width(140));
					EditorGUILayout.LabelField(GetUIWord(UIWORD.Portrait), GUILayout.Width(140));//"Portrait"
					apPortrait nextPortrait = EditorGUILayout.ObjectField(_portrait, typeof(apPortrait), true, GUILayout.Width(140)) as apPortrait;

					if (_portrait != nextPortrait)
					{
						//바뀌었다.
						if (nextPortrait != null)
						{
							if (nextPortrait._isOptimizedPortrait)
							{
								//Optimized Portrait는 편집이 불가능하다
								EditorUtility.DisplayDialog(GetText(TEXT.OptPortrait_LoadError_Title),
																GetText(TEXT.OptPortrait_LoadError_Body),
																GetText(TEXT.Okay));
							}
							else
							{
								//NextPortrait를 선택
								_portrait = nextPortrait;
								Controller.InitTmpValues();
								_selection.SetPortrait(_portrait);

								//Portrait의 레퍼런스들을 연결해주자
								Controller.PortraitReadyToEdit();




								//Selection.activeGameObject = _portrait.gameObject;
								Selection.activeGameObject = null;//<<선택을 해제해준다. 프로파일러를 도와줘야져

								//시작은 RootUnit
								_selection.SetOverallDefault();

								OnAnyObjectAddedOrRemoved();
							}
						}
						else
						{
							Controller.InitTmpValues();
							_selection.SetNone();
							_portrait = null;
						}

						SyncHierarchyOrders();

						_hierarchy.ResetAllUnits();
						_hierarchy_MeshGroup.ResetSubUnits();
						_hierarchy_AnimClip.ResetSubUnits();
					}
					EditorGUILayout.EndVertical();

					if(_guiContent_TopBtn_Setting == null)
					{
						_guiContent_TopBtn_Setting = apGUIContentWrapper.Make(ImageSet.Get(apImageSet.PRESET.ToolBtn_Setting), "Settings of the Portrait and Editor");
					}
					if(_guiContent_TopBtn_Bake == null)
					{
						_guiContent_TopBtn_Bake = apGUIContentWrapper.Make("  " + GetUIWord(UIWORD.Bake), false, ImageSet.Get(apImageSet.PRESET.ToolBtn_Bake), "Bake to Scene");
					}
					

					if (GUILayout.Button(_guiContent_TopBtn_Setting.Content, GUILayout.Width(height - 14), GUILayout.Height(height - 14)))
					{
						//apDialog_PortraitSetting.ShowDialog(this, _portrait);
						_dialogShowCall = DIALOG_SHOW_CALL.Setting;//<<Delay된 호출을 한다.
					}

					//"  Bake"
					if (GUILayout.Button(_guiContent_TopBtn_Bake.Content, GUILayout.Width(90), GUILayout.Height(height - 14)))
					{
						//_portrait.Bake();
						//Controller.PortraitBake();
						//Notification("[" + _portrait.name + "]\nis Baked", true);

						_dialogShowCall = DIALOG_SHOW_CALL.Bake;
						//apDialog_Bake.ShowDialog(this, _portrait);//<<이건 Delay 해야한다.
					}
				}
			}
			//else
			//{
			//	if (_portrait == null)
			//	{

			//		return;
			//	}
			//}

			//GUILayout.Space(15);
			////GUILayout.Box("", GUILayout.Width(3), GUILayout.Height(height - 20));
			//apEditorUtil.GUI_DelimeterBoxV(height - 15);
			//GUILayout.Space(15);

			int tabBtnHeight = height - (15);
			int tabBtnWidth = tabBtnHeight + 10;

			bool isGizmoUpdatable = Gizmos.IsUpdatable;

			string strCtrlKey = "Ctrl";

#if UNITY_EDITOR_OSX
			strCtrlKey = "Command";
#endif

			//>>>>>>>>>>>> Tab2_TRS Tools >>>>>>>>>>>>>>>>>>>
			bool isGUITab_TRSTools = DrawAndCheckGUITopTab(GUITOP_TAB.Tab2_TRSTools, imbTabOpen, imbTabFolded, height - 10);

			if (isGUITab_TRSTools)
			{
				//if (Gizmos.IsUpdatable)
				if (apEditorUtil.ToggledButton(ImageSet.Get(apImageSet.PRESET.ToolBtn_Select), (_gizmos.ControlType == apGizmos.CONTROL_TYPE.Select), isGizmoUpdatable, tabBtnWidth, tabBtnHeight, "Select Tool (Q)"))
				{
					_gizmos.SetControlType(apGizmos.CONTROL_TYPE.Select);
				}
				if (apEditorUtil.ToggledButton(ImageSet.Get(apImageSet.PRESET.ToolBtn_Move), (_gizmos.ControlType == apGizmos.CONTROL_TYPE.Move), isGizmoUpdatable, tabBtnWidth, tabBtnHeight, "Move Tool (W)"))
				{
					_gizmos.SetControlType(apGizmos.CONTROL_TYPE.Move);
				}
				if (apEditorUtil.ToggledButton(ImageSet.Get(apImageSet.PRESET.ToolBtn_Rotate), (_gizmos.ControlType == apGizmos.CONTROL_TYPE.Rotate), isGizmoUpdatable, tabBtnWidth, tabBtnHeight, "Rotate Tool (E)"))
				{
					_gizmos.SetControlType(apGizmos.CONTROL_TYPE.Rotate);
				}
				if (apEditorUtil.ToggledButton(ImageSet.Get(apImageSet.PRESET.ToolBtn_Scale), (_gizmos.ControlType == apGizmos.CONTROL_TYPE.Scale), isGizmoUpdatable, tabBtnWidth, tabBtnHeight, "Scale Tool (R)"))
				{
					_gizmos.SetControlType(apGizmos.CONTROL_TYPE.Scale);
				}



				GUILayout.Space(5);

				EditorGUILayout.BeginVertical(GUILayout.Width(95));

				EditorGUILayout.LabelField(GetUIWord(UIWORD.Coordinate), GUILayout.Width(95));//"Coordinate"
				apGizmos.COORDINATE_TYPE nextCoordinate = (apGizmos.COORDINATE_TYPE)EditorGUILayout.EnumPopup(Gizmos.Coordinate, GUILayout.Width(95));
				if (nextCoordinate != Gizmos.Coordinate)
				{
					Gizmos.SetCoordinateType(nextCoordinate);
				}
				EditorGUILayout.EndVertical();

			}
			//GUILayout.Space(5);

			//>>>>>>>>>>>> Tab3_Visibility >>>>>>>>>>>>>>>>>>>
			bool isGUITab_Visibility = DrawAndCheckGUITopTab(GUITOP_TAB.Tab3_Visibility, imbTabOpen, imbTabFolded, height - 10);

			if (isGUITab_Visibility)
			{
				// Onion 버튼.
				bool isOnionButtonAvailable = false;
				bool isOnionButtonRecordable = false;
				if (Select.SelectionType == apSelection.SELECTION_TYPE.Animation ||
					Select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup)
				{
					isOnionButtonAvailable = true;
					if (!_onionOption_IsRenderAnimFrames || Select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup)
					{
						isOnionButtonRecordable = true;
					}
				}
				if (apEditorUtil.ToggledButton_2Side_Ctrl(ImageSet.Get(apImageSet.PRESET.ToolBtn_OnionView),
													Onion.IsVisible, isOnionButtonAvailable, tabBtnWidth, tabBtnHeight,
													"Show/Hide Onion Skin (O)", Event.current.control, Event.current.command))
				{
#if UNITY_EDITOR_OSX
				if(Event.current.command)
#else
					if (Event.current.control)
#endif
					{
						apDialog_OnionSetting.ShowDialog(this, _portrait);//<<Onion 설정 다이얼로그를 호출
					}
					else
					{
						Onion.SetVisible(!Onion.IsVisible);
					}

				}




				//만약 Onion이 켜졌다면 => Record 버튼이 활성화된다.
				bool isOnionRecordable = isOnionButtonAvailable && Onion.IsVisible && isOnionButtonRecordable;
				
				//SetGUIVisible("GUI Top Onion Visible", isOnionRecordable);
				SetGUIVisible(DELAYED_UI_TYPE.GUI_Top_Onion_Visible, isOnionRecordable);
				
				//if (IsDelayedGUIVisible("GUI Top Onion Visible"))
				if (IsDelayedGUIVisible(DELAYED_UI_TYPE.GUI_Top_Onion_Visible))
				{
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.ToolBtn_OnionRecord),
													false, true, tabBtnWidth, tabBtnHeight, "Record Onion Skin"))
					{
						//현재 상태를 기록한다.
						Onion.Record(this);
					}

					GUILayout.Space(5);
				}



				// Bone Visible 여부 버튼
				Texture2D iconImg_boneGUI = null;
				if (_boneGUIRenderMode == BONE_RENDER_MODE.RenderOutline)
				{
					iconImg_boneGUI = ImageSet.Get(apImageSet.PRESET.ToolBtn_BoneVisibleOutlineOnly);
				}
				else
				{
					iconImg_boneGUI = ImageSet.Get(apImageSet.PRESET.ToolBtn_BoneVisible);
				}


				bool isBoneVisibleButtonAvailable = _selection.SelectionType == apSelection.SELECTION_TYPE.MeshGroup ||
					_selection.SelectionType == apSelection.SELECTION_TYPE.Animation ||
					_selection.SelectionType == apSelection.SELECTION_TYPE.Overall;


				if (apEditorUtil.ToggledButton_2Side_Ctrl(iconImg_boneGUI,
					_boneGUIRenderMode != BONE_RENDER_MODE.None,
					isBoneVisibleButtonAvailable, tabBtnWidth, tabBtnHeight,
					"Change Bone Visiblity (B) / If you press the button while holding down [" + strCtrlKey + "], the function works in reverse.",
					Event.current.control,
					Event.current.command))
				{
#if UNITY_EDITOR_OSX
				bool isCtrl = Event.current.command;
#else
					bool isCtrl = Event.current.control;
#endif

					//Control 키를 누르면 그냥 누른 것과 반대로 변경된다.
					//Ctrl : Outline -> Render -> None -> Outline
					//그냥 : None -> Render -> Outline -> None


					switch (_boneGUIRenderMode)
					{
						case BONE_RENDER_MODE.None:
							if (isCtrl)
							{
								_boneGUIRenderMode = BONE_RENDER_MODE.RenderOutline;
							}
							else
							{
								_boneGUIRenderMode = BONE_RENDER_MODE.Render;
							}

							break;

						case BONE_RENDER_MODE.Render:
							if (isCtrl)
							{
								_boneGUIRenderMode = BONE_RENDER_MODE.None;
							}
							else
							{
								_boneGUIRenderMode = BONE_RENDER_MODE.RenderOutline;
							}

							break;

						case BONE_RENDER_MODE.RenderOutline:
							if (isCtrl)
							{
								_boneGUIRenderMode = BONE_RENDER_MODE.Render;
							}
							else
							{
								_boneGUIRenderMode = BONE_RENDER_MODE.None;
							}

							break;

					}
					SaveEditorPref();
				}


				//추가 : 메시 렌더링
				if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.ToolBtn_MeshVisible),
					_meshGUIRenderMode == MESH_RENDER_MODE.Render,
					_selection.SelectionType == apSelection.SELECTION_TYPE.MeshGroup ||
					_selection.SelectionType == apSelection.SELECTION_TYPE.Animation ||
					_selection.SelectionType == apSelection.SELECTION_TYPE.Overall, tabBtnWidth, tabBtnHeight,
					"Enable/Disable Mesh Visiblity"))
				{
					if (_meshGUIRenderMode == MESH_RENDER_MODE.None)
					{
						_meshGUIRenderMode = MESH_RENDER_MODE.Render;
					}
					else
					{
						_meshGUIRenderMode = MESH_RENDER_MODE.None;
					}
				}


				//물리 적용 여부
				bool isPhysic = false;

				if (_portrait != null)
				{
					isPhysic = _portrait._isPhysicsPlay_Editor;
				}

				if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.ToolBtn_Physic),
					isPhysic,
					_selection.SelectionType == apSelection.SELECTION_TYPE.MeshGroup ||
					_selection.SelectionType == apSelection.SELECTION_TYPE.Animation ||
					_selection.SelectionType == apSelection.SELECTION_TYPE.Overall, tabBtnWidth, tabBtnHeight,
					"Enable/Disable Physical Effect"))
				{
					if (_portrait != null)
					{
						//물리 기능 토글
						_portrait._isPhysicsPlay_Editor = !isPhysic;
						if (_portrait._isPhysicsPlay_Editor)
						{
							//물리 값을 리셋합시다.
							Controller.ResetPhysicsValues();
						}
					}
				}

			}


			//GUILayout.Space(15);


			//Gizmo에 의해 어떤 UI가 나와야 하는지 판단하자.
			apGizmos.TRANSFORM_UI_VALID gizmoUI_VetexTF = Gizmos.TransformUI_VertexTransform;
			apGizmos.TRANSFORM_UI_VALID gizmoUI_Position2D = Gizmos.TransformUI_Position;
			apGizmos.TRANSFORM_UI_VALID gizmoUI_Rotation = Gizmos.TransformUI_Rotation;
			apGizmos.TRANSFORM_UI_VALID gizmoUI_Scale = Gizmos.TransformUI_Scale;
			apGizmos.TRANSFORM_UI_VALID gizmoUI_Depth = Gizmos.TransformUI_Depth;
			apGizmos.TRANSFORM_UI_VALID gizmoUI_Color = Gizmos.TransformUI_Color;
			apGizmos.TRANSFORM_UI_VALID gizmoUI_Extra = Gizmos.TransformUI_Extra;
			apGizmos.TRANSFORM_UI_VALID gizmoUI_BoneIKController = Gizmos.TransformUI_BoneIKController;


			bool isGizmoGUIVisible_VertexTransform = false;

			bool isGizmoGUIVisible_VTF_FFD = false;
			bool isGizmoGUIVisible_VTF_Soft = false;
			bool isGizmoGUIVisible_VTF_Blur = false;

			bool isGizmoGUIVisible_Position2D = false;
			bool isGizmoGUIVisible_Rotation = false;
			bool isGizmoGUIVisible_Scale = false;
			bool isGizmoGUIVisible_Depth = false;
			bool isGizmoGUIVisible_Color = false;
			bool isGizmoGUIVisible_Extra = false;
			bool isGizmoGUIVisible_BoneIKController = false;

			//Vertex Transform
			//1) FFD
			//2) Soft Selection
			//3) Blur가 있다.

			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__Vertex_Transform, (gizmoUI_VetexTF != apGizmos.TRANSFORM_UI_VALID.Hide));//"Top UI - Vertex Transform"

			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__Position, (gizmoUI_Position2D != apGizmos.TRANSFORM_UI_VALID.Hide));//"Top UI - Position"
			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__Rotation, (gizmoUI_Rotation != apGizmos.TRANSFORM_UI_VALID.Hide));//"Top UI - Rotation"
			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__Scale, (gizmoUI_Scale != apGizmos.TRANSFORM_UI_VALID.Hide));//"Top UI - Scale"
			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__Depth, (gizmoUI_Depth != apGizmos.TRANSFORM_UI_VALID.Hide));//"Top UI - Depth"
			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__Color, (gizmoUI_Color != apGizmos.TRANSFORM_UI_VALID.Hide));//"Top UI - Color"
			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__Extra, (gizmoUI_Extra != apGizmos.TRANSFORM_UI_VALID.Hide));//"Top UI - Extra"

			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__BoneIKController, (gizmoUI_BoneIKController != apGizmos.TRANSFORM_UI_VALID.Hide));//"Top UI - BoneIKController"

			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__VTF_FFD, (gizmoUI_VetexTF != apGizmos.TRANSFORM_UI_VALID.Hide) && Gizmos.IsFFDMode);//"Top UI - VTF FFD"
			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__VTF_Soft, (gizmoUI_VetexTF != apGizmos.TRANSFORM_UI_VALID.Hide) && Gizmos.IsSoftSelectionMode);//"Top UI - VTF Soft"
			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__VTF_Blur, (gizmoUI_VetexTF != apGizmos.TRANSFORM_UI_VALID.Hide) && Gizmos.IsBrushMode);//"Top UI - VTF Blur"

			SetGUIVisible(DELAYED_UI_TYPE.Top_UI__Overall, _selection.SelectionType == apSelection.SELECTION_TYPE.Overall);//"Top UI - Overall"


			isGizmoGUIVisible_VertexTransform = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__Vertex_Transform);//"Top UI - Vertex Transform"

			isGizmoGUIVisible_VTF_FFD = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__VTF_FFD);//"Top UI - VTF FFD"
			if (Event.current.type == EventType.Layout)
			{
				_isGizmoGUIVisible_VTF_FFD_Prev = isGizmoGUIVisible_VTF_FFD;
			}
			isGizmoGUIVisible_VTF_Soft = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__VTF_Soft);//"Top UI - VTF Soft"
			isGizmoGUIVisible_VTF_Blur = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__VTF_Blur);//"Top UI - VTF Blur"

			isGizmoGUIVisible_Position2D = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__Position);//"Top UI - Position"
			isGizmoGUIVisible_Rotation = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__Rotation);//"Top UI - Rotation"
			isGizmoGUIVisible_Scale = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__Scale);//"Top UI - Scale"
			isGizmoGUIVisible_Depth = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__Depth);//"Top UI - Depth"
			isGizmoGUIVisible_Color = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__Color);//"Top UI - Color"
			isGizmoGUIVisible_Extra = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__Extra);//"Top UI - Extra"
			isGizmoGUIVisible_BoneIKController = IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__BoneIKController);//"Top UI - BoneIKController"




			if (isGizmoGUIVisible_VertexTransform)
			{

				//>>>>>>>>>>>> Tab4_FFD Soft Blur >>>>>>>>>>>>>>>>>>>
				bool isGUITab_FFD_Soft_Blur = DrawAndCheckGUITopTab(GUITOP_TAB.Tab4_FFD_Soft_Blur, imbTabOpen, imbTabFolded, height - 10);

				//1. FFD, Soft, Blur 선택 버튼
				if (isGUITab_FFD_Soft_Blur)
				{
					//apEditorUtil.GUI_DelimeterBoxV(height - 15);
					//GUILayout.Space(15);

					if (apEditorUtil.ToggledButton_Ctrl(ImageSet.Get(apImageSet.PRESET.ToolBtn_Transform),
						Gizmos.IsFFDMode, Gizmos.IsFFDModeAvailable, tabBtnWidth, tabBtnHeight,
						"Free Form Deformation / If you press the button while holding down [" + strCtrlKey + "], a dialog appears allowing you to change the number of control points.",
						Event.current.control,
						Event.current.command))
					{

#if UNITY_EDITOR_OSX
					bool isCtrl = Event.current.command;
#else
						bool isCtrl = Event.current.control;
#endif
						if (isCtrl)
						{
							//커스텀 사이즈로 연다.
							_loadKey_FFDStart = apDialog_FFDSize.ShowDialog(this, _portrait, OnDialogEvent_FFDStart, _curFFDSizeX, _curFFDSizeY);
						}
						else
						{


							Gizmos.StartTransformMode(this);//원래는 <이거 기본 3X3
						}

					}

					//2-1) FFD
					if (isGizmoGUIVisible_VTF_FFD)
					{
						if (apEditorUtil.ToggledButton(ImageSet.Get(apImageSet.PRESET.ToolBtn_TransformAdapt),
							false, Gizmos.IsFFDMode, tabBtnWidth, tabBtnHeight, "Apply FFD"))
						{
							//_gizmos.SetControlType(apGizmos.CONTROL_TYPE.Transform);
							//Gizmos.StartTransformMode();
							if (Gizmos.IsFFDMode)
							{
								Gizmos.AdaptTransformObjects(this);
							}
						}

						if (apEditorUtil.ToggledButton(ImageSet.Get(apImageSet.PRESET.ToolBtn_TransformRevert),
							false, Gizmos.IsFFDMode, tabBtnWidth, tabBtnHeight, "Revert FFD"))
						{
							//_gizmos.SetControlType(apGizmos.CONTROL_TYPE.Transform);
							//Gizmos.StartTransformMode();
							if (Gizmos.IsFFDMode)
							{
								Gizmos.RevertTransformObjects(this);
							}
						}

						GUILayout.Space(10);

						_isGizmoGUIVisible_VTF_FFD_Prev = true;
					}
					else if (_isGizmoGUIVisible_VTF_FFD_Prev)
					{
						//만약 Layout이 아닌 이전 이벤트에서 FFD 편집 중이었는데
						//갑자기 사라지는 경우 -> 더미를 출력하자
						apEditorUtil.ToggledButton(ImageSet.Get(apImageSet.PRESET.ToolBtn_TransformAdapt), false, Gizmos.IsFFDMode, tabBtnWidth, tabBtnHeight, "Apply FFD");

						apEditorUtil.ToggledButton(ImageSet.Get(apImageSet.PRESET.ToolBtn_TransformRevert), false, Gizmos.IsFFDMode, tabBtnWidth, tabBtnHeight, "Revert FFD");

						GUILayout.Space(10);
					}

					if (Event.current.type == EventType.Layout)
					{
						_isGizmoGUIVisible_VTF_FFD_Prev = isGizmoGUIVisible_VTF_FFD;
					}

					GUILayout.Space(10);

					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.ToolBtn_SoftSelection),
						Gizmos.IsSoftSelectionMode, Gizmos.IsSoftSelectionModeAvailable, tabBtnWidth, tabBtnHeight,
						"Soft Selection (Adjust brush size with [, ])"))
					{
						if (Gizmos.IsSoftSelectionMode)
						{
							Gizmos.EndSoftSelection();
						}
						else
						{
							Gizmos.StartSoftSelection();
						}
					}


					int labelSize = 70;
					int sliderSize = 200;
					int sliderSetSize = labelSize + sliderSize + 4;

					//2-2) Soft Selection
					if (isGizmoGUIVisible_VTF_Soft)
					{
						//Radius와 Curve
						EditorGUILayout.BeginVertical(GUILayout.Width(sliderSetSize));
						int softRadius = Gizmos.SoftSelectionRadius;
						int softCurveRatio = Gizmos.SoftSelectionCurveRatio;

						EditorGUILayout.BeginHorizontal(GUILayout.Width(sliderSetSize));
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Radius), GUILayout.Width(labelSize));//"Radius"
						softRadius = EditorGUILayout.IntSlider(softRadius, 1, apGizmos.MAX_SOFT_SELECTION_RADIUS, GUILayout.Width(sliderSize));
						EditorGUILayout.EndHorizontal();

						//string strCurveLabel = "Curve";
						string strCurveLabel = GetUIWord(UIWORD.Curve);
						EditorGUILayout.BeginHorizontal(GUILayout.Width(sliderSetSize));
						EditorGUILayout.LabelField(strCurveLabel, GUILayout.Width(labelSize));
						softCurveRatio = EditorGUILayout.IntSlider(softCurveRatio, -100, 100, GUILayout.Width(sliderSize));
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.EndVertical();

						GUILayout.Space(10);

						if (softRadius != Gizmos.SoftSelectionRadius || softCurveRatio != Gizmos.SoftSelectionCurveRatio)
						{
							//TODO : 브러시 미리보기 기동
							Gizmos.RefreshSoftSelectionValue(softRadius, softCurveRatio);
						}
					}


					GUILayout.Space(10);

					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.ToolBtn_Blur),
						//Gizmos.IsBrushMode, 
						_blurEnabled,
						Gizmos.IsBrushModeAvailable, tabBtnWidth, tabBtnHeight,
						"Blur (Adjust brush size with [, ]"))
					{
						if(_blurEnabled)
						//if (Gizmos.IsBrushMode)
						{
							_blurEnabled = false;
							Gizmos.EndBrush();
						}
						else
						{
							_blurEnabled  = true;
							Gizmos.StartBrush();
						}
					}

					//2-3) Blur
					if (isGizmoGUIVisible_VTF_Blur)
					{
						//Range와 Intensity
						//Radius와 Curve
						EditorGUILayout.BeginVertical(GUILayout.Width(sliderSetSize));

						//int blurRadius = Gizmos.BrushRadius;
						//int blurIntensity = Gizmos.BrushIntensity;

						EditorGUILayout.BeginHorizontal(GUILayout.Width(sliderSetSize));
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Radius), GUILayout.Width(labelSize));//"Radius"
						_blurRadius = EditorGUILayout.IntSlider(_blurRadius, 1, apGizmos.MAX_BRUSH_RADIUS, GUILayout.Width(sliderSize));
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal(GUILayout.Width(sliderSetSize));
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Intensity), GUILayout.Width(labelSize));//"Intensity"
						_blurIntensity = EditorGUILayout.IntSlider(_blurIntensity, 0, 100, GUILayout.Width(sliderSize));
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.EndVertical();

						
						//if (blurRadius != Gizmos.BrushRadius || blurIntensity != Gizmos.BrushIntensity)
						//{
						//	//TODO : 브러시 미리보기 기동
						//	Gizmos.RefreshBlurValue(blurRadius, blurIntensity);
						//}


					}

					//GUILayout.Space(15);
				}

			}



			//bool isGizmoGUIVisible_Position = IsDelayedGUIVisible("Top UI - Position");
			//bool isGizmoGUIVisible_Rotation = IsDelayedGUIVisible("Top UI - Rotation");
			//bool isGizmoGUIVisible_Scale = IsDelayedGUIVisible("Top UI - Scale");
			//bool isGizmoGUIVisible_Color = IsDelayedGUIVisible("Top UI - Color");

			//if (Gizmos._isGizmoRenderable)
			if (isGizmoGUIVisible_Position2D
				|| isGizmoGUIVisible_Rotation
				|| isGizmoGUIVisible_Scale
				|| isGizmoGUIVisible_Depth
				|| isGizmoGUIVisible_Color
				|| isGizmoGUIVisible_Extra
				|| isGizmoGUIVisible_BoneIKController)
			{
				//하나라도 Gizmo Transform 이 나타난다면 이 영역을 그려준다.
				//apEditorUtil.GUI_DelimeterBoxV(height - 15);
				//GUILayout.Space(15);

				//>>>>>>>>>>>> Tab5_GizmoValue >>>>>>>>>>>>>>>>>>>
				bool isGUITab_GizmoValue = DrawAndCheckGUITopTab(GUITOP_TAB.Tab5_GizmoValue, imbTabOpen, imbTabFolded, height - 10);

				if (isGUITab_GizmoValue)
				{
					//Transform

					//Position
					apGizmos.TransformParam curTransformParam = Gizmos.GetCurTransformParam();
					Vector2 prevPos = Vector2.zero;
					int prevDepth = 0;
					float prevRotation = 0.0f;
					Vector2 prevScale2 = Vector2.one;
					Color prevColor = Color.black;
					bool prevVisible = true;
					//float prevBoneIKMixWeight = 0.0f;

					if (curTransformParam != null)
					{
						//prevPos = curTransformParam._posW;//<<GUI용으로 변경
						prevPos = curTransformParam._pos_GUI;
						prevDepth = curTransformParam._depth;
						//prevRotation = curTransformParam._angle;
						prevRotation = curTransformParam._angle_GUI;//<<GUI용으로 변경

						//prevScale2 = curTransformParam._scale;
						prevScale2 = curTransformParam._scale_GUI;//GUI 용으로 변경
						prevColor = curTransformParam._color;
						prevVisible = curTransformParam._isVisible;

						//prevBoneIKMixWeight = curTransformParam._boneIKMixWeight;
					}

					Vector2 curPos = prevPos;
					int curDepth = prevDepth;
					float curRotation = prevRotation;
					Vector2 curScale = prevScale2;
					Color curColor = prevColor;
					bool curVisible = prevVisible;
					//float curBoneIKMixWeight = prevBoneIKMixWeight;

					if (_guiContent_Top_GizmoIcon_Move == null)		{ _guiContent_Top_GizmoIcon_Move =		apGUIContentWrapper.Make(ImageSet.Get(apImageSet.PRESET.Transform_Move)); }
					if (_guiContent_Top_GizmoIcon_Depth == null)	{ _guiContent_Top_GizmoIcon_Depth =		apGUIContentWrapper.Make(ImageSet.Get(apImageSet.PRESET.Transform_Depth)); }
					if (_guiContent_Top_GizmoIcon_Rotation == null) { _guiContent_Top_GizmoIcon_Rotation =	apGUIContentWrapper.Make(ImageSet.Get(apImageSet.PRESET.Transform_Rotate)); }
					if (_guiContent_Top_GizmoIcon_Scale == null)	{ _guiContent_Top_GizmoIcon_Scale =		apGUIContentWrapper.Make(ImageSet.Get(apImageSet.PRESET.Transform_Scale)); }
					if (_guiContent_Top_GizmoIcon_Color == null)	{ _guiContent_Top_GizmoIcon_Color =		apGUIContentWrapper.Make(ImageSet.Get(apImageSet.PRESET.Transform_Color)); }
					if (_guiContent_Top_GizmoIcon_Extra == null)	{ _guiContent_Top_GizmoIcon_Extra =		apGUIContentWrapper.Make(ImageSet.Get(apImageSet.PRESET.Transform_ExtraOption)); }

					if (isGizmoGUIVisible_Position2D)
					{
						//이전
						//GUIStyle guiStyle_Label = new GUIStyle(GUI.skin.label);
						//if (gizmoUI_Position2D != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						//{
						//	guiStyle_Label.normal.textColor = Color.gray;
						//}

						
						//EditorGUILayout.LabelField(new GUIContent(ImageSet.Get(apImageSet.PRESET.Transform_Move)), GUILayout.Width(30), GUILayout.Height(30));
						EditorGUILayout.LabelField(_guiContent_Top_GizmoIcon_Move.Content, GUILayout.Width(30), GUILayout.Height(30));

						EditorGUILayout.BeginVertical(GUILayout.Width(130));

						//"Position"
						//이전
						//EditorGUILayout.LabelField(GetUIWord(UIWORD.Position), guiStyle_Label, GUILayout.Width(130));
						
						//변경
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Position), 
							(gizmoUI_Position2D != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled ? GUIStyleWrapper.Label_GrayColor : GUIStyleWrapper.Label),
							GUILayout.Width(130));

						if (gizmoUI_Position2D == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						{
							curPos = apEditorUtil.DelayedVector2Field(prevPos, 130);
						}
						else
						{
							curPos = apEditorUtil.DelayedVector2Field(Vector2.zero, 130);
						}
						EditorGUILayout.EndVertical();

						GUILayout.Space(10);


					}

					if (isGizmoGUIVisible_Depth)
					{
						//Depth와 Position은 같이 묶인다. > Depth는 따로 분리한다. (11.26)
						//이전
						//GUIStyle guiStyle_Label = new GUIStyle(GUI.skin.label);
						//if (gizmoUI_Depth != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						//{
						//	guiStyle_Label.normal.textColor = Color.gray;
						//}

						//EditorGUILayout.LabelField(new GUIContent(ImageSet.Get(apImageSet.PRESET.Transform_Depth)), GUILayout.Width(30), GUILayout.Height(30));
						EditorGUILayout.LabelField(_guiContent_Top_GizmoIcon_Depth.Content, GUILayout.Width(30), GUILayout.Height(30));

						EditorGUILayout.BeginVertical(GUILayout.Width(70));
						
						//"Depth"
						//이전
						//EditorGUILayout.LabelField(GetUIWord(UIWORD.Depth), guiStyle_Label, GUILayout.Width(70));
						
						//변경
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Depth), 
							(gizmoUI_Depth != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled ? GUIStyleWrapper.Label_GrayColor : GUIStyleWrapper.Label),
							GUILayout.Width(70));//"Depth"


						if (gizmoUI_Depth == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						{
							curDepth = EditorGUILayout.DelayedIntField("", prevDepth, GUILayout.Width(70));
						}
						else
						{
							EditorGUILayout.DelayedIntField("", 0, GUILayout.Width(70));
						}
						EditorGUILayout.EndVertical();

						GUILayout.Space(10);
					}

					if (isGizmoGUIVisible_Rotation)
					{
						//이전
						//GUIStyle guiStyle_Label = new GUIStyle(GUI.skin.label);
						//if (gizmoUI_Rotation != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						//{
						//	guiStyle_Label.normal.textColor = Color.gray;
						//}


						//EditorGUILayout.LabelField(new GUIContent(ImageSet.Get(apImageSet.PRESET.Transform_Rotate)), GUILayout.Width(30), GUILayout.Height(30));
						EditorGUILayout.LabelField(_guiContent_Top_GizmoIcon_Rotation.Content, GUILayout.Width(30), GUILayout.Height(30));

						EditorGUILayout.BeginVertical(GUILayout.Width(70));
						//"Rotation"
						//이전
						//EditorGUILayout.LabelField(GetUIWord(UIWORD.Rotation), guiStyle_Label, GUILayout.Width(70));
						
						//변경
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Rotation), 
							(gizmoUI_Rotation != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled ? GUIStyleWrapper.Label_GrayColor : GUIStyleWrapper.Label),
							GUILayout.Width(70));

						if (gizmoUI_Rotation == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						{
							curRotation = EditorGUILayout.DelayedFloatField(prevRotation, GUILayout.Width(70));
						}
						else
						{
							EditorGUILayout.DelayedFloatField(0.0f, GUILayout.Width(70));
						}
						EditorGUILayout.EndVertical();

						GUILayout.Space(10);
					}

					if (isGizmoGUIVisible_Scale)
					{
						//이전
						//GUIStyle guiStyle_Label = new GUIStyle(GUI.skin.label);
						//if (gizmoUI_Scale != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						//{
						//	guiStyle_Label.normal.textColor = Color.gray;
						//}
						
						//EditorGUILayout.LabelField(new GUIContent(ImageSet.Get(apImageSet.PRESET.Transform_Scale)), GUILayout.Width(30), GUILayout.Height(30));
						EditorGUILayout.LabelField(_guiContent_Top_GizmoIcon_Scale.Content, GUILayout.Width(30), GUILayout.Height(30));

						EditorGUILayout.BeginVertical(GUILayout.Width(120));
						//"Scaling"
						//이전
						//EditorGUILayout.LabelField(GetUIWord(UIWORD.Scaling), guiStyle_Label, GUILayout.Width(120));
						//변경
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Scaling), 
							(gizmoUI_Scale != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled ? GUIStyleWrapper.Label_GrayColor : GUIStyleWrapper.Label),
							GUILayout.Width(120));


						if (gizmoUI_Scale == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						{
							//curScale = EditorGUILayout.Vector2Field("", prevScale2, GUILayout.Width(120));
							EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
							curScale.x = EditorGUILayout.DelayedFloatField(prevScale2.x, GUILayout.Width(60 - 2));
							curScale.y = EditorGUILayout.DelayedFloatField(prevScale2.y, GUILayout.Width(60 - 2));
							EditorGUILayout.EndHorizontal();
						}
						else
						{
							//EditorGUILayout.Vector2Field("", Vector2.zero, GUILayout.Width(120));
							EditorGUILayout.BeginHorizontal(GUILayout.Width(120));
							EditorGUILayout.DelayedFloatField(0, GUILayout.Width(60 - 2));
							EditorGUILayout.DelayedFloatField(0, GUILayout.Width(60 - 2));
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndVertical();

						GUILayout.Space(10);
					}


					if (isGizmoGUIVisible_Color)
					{
						//이전
						//GUIStyle guiStyle_Label = new GUIStyle(GUI.skin.label);
						//if (gizmoUI_Color != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						//{
						//	guiStyle_Label.normal.textColor = Color.gray;
						//}

						EditorGUILayout.LabelField(_guiContent_Top_GizmoIcon_Color.Content, GUILayout.Width(30), GUILayout.Height(30));

						EditorGUILayout.BeginVertical(GUILayout.Width(70));
						//"Color"
						//이전
						//EditorGUILayout.LabelField(GetUIWord(UIWORD.Color), guiStyle_Label, GUILayout.Width(70));
						//변경
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Color), 
							(gizmoUI_Color != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled ? GUIStyleWrapper.Label_GrayColor : GUIStyleWrapper.Label),
							GUILayout.Width(70));

						if (gizmoUI_Color == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						{
							curColor = EditorGUILayout.ColorField("", prevColor, GUILayout.Width(70));
						}
						else
						{
							EditorGUILayout.ColorField("", Color.black, GUILayout.Width(70));
						}
						EditorGUILayout.EndVertical();
						EditorGUILayout.BeginVertical(GUILayout.Width(45));
						
						//"Visible"
						//이전
						//EditorGUILayout.LabelField(GetUIWord(UIWORD.Visible), guiStyle_Label, GUILayout.Width(45));
						//변경
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Visible), 
							(gizmoUI_Color != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled ? GUIStyleWrapper.Label_GrayColor : GUIStyleWrapper.Label),
							GUILayout.Width(45));


						if (gizmoUI_Color == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						{
							curVisible = EditorGUILayout.Toggle(prevVisible, GUILayout.Width(45));
						}
						else
						{
							EditorGUILayout.Toggle(false, GUILayout.Width(45));
						}
						EditorGUILayout.EndVertical();

						GUILayout.Space(10);
					}

					if (isGizmoGUIVisible_Extra)
					{
						//이전
						//GUIStyle guiStyle_Label = new GUIStyle(GUI.skin.label);
						//if (gizmoUI_Extra != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled)
						//{
						//	guiStyle_Label.normal.textColor = Color.gray;
						//}

						EditorGUILayout.LabelField(_guiContent_Top_GizmoIcon_Extra.Content, GUILayout.Width(30), GUILayout.Height(30));

						EditorGUILayout.BeginVertical(GUILayout.Width(70));
						//"Extra"
						//이전
						//EditorGUILayout.LabelField(GetUIWord(UIWORD.Extra), guiStyle_Label, GUILayout.Width(70));
						//변경
						EditorGUILayout.LabelField(GetUIWord(UIWORD.Extra), 
							(gizmoUI_Extra != apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled ? GUIStyleWrapper.Label_GrayColor : GUIStyleWrapper.Label), 
							GUILayout.Width(70));

						if (apEditorUtil.ToggledButton_2Side(GetUIWord(UIWORD.Set), GetUIWord(UIWORD.Set), false, (gizmoUI_Extra == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled), 65, 18))
						{
							if (gizmoUI_Extra == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled && curTransformParam != null)
							{
								Gizmos.OnTransformChanged_Extra();
							}
						}
						EditorGUILayout.EndVertical();
					}

					


					if (curTransformParam != null)
					{
						if (gizmoUI_Position2D == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled && prevPos != curPos)
						{
							Gizmos.OnTransformChanged_Position(curPos);
						}
						if (gizmoUI_Rotation == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled && (prevRotation != curRotation))
						{
							Gizmos.OnTransformChanged_Rotate(curRotation);
						}
						if (gizmoUI_Scale == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled && (prevScale2 != curScale))
						{
							Gizmos.OnTransformChanged_Scale(curScale);
						}
						if (gizmoUI_Depth == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled && curDepth != prevDepth)
						{
							Gizmos.OnTransformChanged_Depth(curDepth);
						}
						if (gizmoUI_Color == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled && (prevColor != curColor || prevVisible != curVisible))
						{
							Gizmos.OnTransformChanged_Color(curColor, curVisible);
						}

						//if(gizmoUI_BoneIKController == apGizmos.TRANSFORM_UI_VALID.ShowAndEnabled && (prevBoneIKMixWeight != curBoneIKMixWeight))
						//{
						//	//추가 : BoneIKWeight 변경됨
						//	Gizmos.OnTransformChanged_BoneIKController(curBoneIKMixWeight);
						//}
					}
				}
				//GUILayout.Space(15);
			}


			//변경 4.9
			//Show Frame
			//>> Capture 버튼 및 Dialog 호출은 삭제한다. Capture기능은 우측 탭으로 이동한다.
			if (IsDelayedGUIVisible(DELAYED_UI_TYPE.Top_UI__Overall))//"Top UI - Overall"
			{
				//>>>>>>>>>>>> Tab6_Capture >>>>>>>>>>>>>>>>>>>
				bool isGUITab_Capture = DrawAndCheckGUITopTab(GUITOP_TAB.Tab6_Capture, imbTabOpen, imbTabFolded, height - 10);

				//apEditorUtil.GUI_DelimeterBoxV(height - 15);
				//GUILayout.Space(15);
				if (isGUITab_Capture)
				{
					//"Show Frame", "Show Frame"
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Capture_Frame),
															"  " + GetUIWord(UIWORD.ShowFrame),
															"  " + GetUIWord(UIWORD.ShowFrame),
															_isShowCaptureFrame, true, 130, tabBtnHeight))
					{
						_isShowCaptureFrame = !_isShowCaptureFrame;
					}

					//"Capture" >> 이게 삭제된다.
					//if (GUILayout.Button(GetUIWord(UIWORD.Capture), GUILayout.Width(80), GUILayout.Height(tabBtnHeight)))
					//{

					//	_dialogShowCall = DIALOG_SHOW_CALL.Capture;
					//}

					//추가 19.5.31 : Material Library 설정
					//"  Material Library"
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.ToolBtn_MaterialLibrary),
															"  " + GetUIWord(UIWORD.MaterialLibrary),
															"  " + GetUIWord(UIWORD.MaterialLibrary),
															false, true, 150, tabBtnHeight))
					{
						try
						{
							apDialog_MaterialLibrary.ShowDialog(this, _portrait);
						}
						catch (Exception ex)
						{
							Debug.LogError("Exception : " + ex);
						}

					}
				}
				//GUILayout.Space(20);
			}

			EditorGUILayout.EndHorizontal();
			GUILayout.Space(5);

		}

		//GUI Top에서 UI들을 묶어서 보였다가 안보이게 할 수 있다.
		private bool DrawAndCheckGUITopTab(GUITOP_TAB tabType, Texture2D imgOpen, Texture2D imgFolded, int height)
		{
			GUILayout.Space(8);
			EditorGUILayout.BeginVertical(GUILayout.Width(10), GUILayout.Height(height));
			GUILayout.Space((height - (32)) / 2);

			bool isTabOpen = _guiTopTabStaus[tabType];

			//이전
			//GUIStyle guiStyle = new GUIStyle(GUIStyle.none);
			//guiStyle.margin = new RectOffset(0, 0, 0, 0);
			//guiStyle.padding = new RectOffset(0, 0, 0, 0);

			if(_guiContent_GUITopTab_Open == null)
			{
				_guiContent_GUITopTab_Open = apGUIContentWrapper.Make(imgOpen);
			}
			if(_guiContent_GUITopTab_Folded == null)
			{
				_guiContent_GUITopTab_Folded = apGUIContentWrapper.Make(imgFolded);
			}
			
			//이전
			//if (GUILayout.Button(new GUIContent("", (isTabOpen ? imgOpen : imgFolded)), guiStyle, GUILayout.Width(10), GUILayout.Height(32)))

			//변경
			if (GUILayout.Button((isTabOpen ? _guiContent_GUITopTab_Open.Content : _guiContent_GUITopTab_Folded.Content), GUIStyleWrapper.None_Margin0_Padding0, GUILayout.Width(10), GUILayout.Height(32)))
			{
				_guiTopTabStaus[tabType] = !_guiTopTabStaus[tabType];
			}
			EditorGUILayout.EndVertical();
			GUILayout.Space(2);

			return _guiTopTabStaus[tabType];
		}

		private void GUI_HotKey_TopRight()
		{
			bool isGizmoUpdatable = Gizmos.IsUpdatable;
			//기즈모를 단축키로 넣자
			if (isGizmoUpdatable)
			{
				AddHotKeyEvent(Controller.OnHotKeyEvent_GizmoSelect, "Select", KeyCode.Q, false, false, false, null);
				AddHotKeyEvent(Controller.OnHotKeyEvent_GizmoMove, "Move", KeyCode.W, false, false, false, null);
				AddHotKeyEvent(Controller.OnHotKeyEvent_GizmoRotate, "Rotate", KeyCode.E, false, false, false, null);
				AddHotKeyEvent(Controller.OnHotKeyEvent_GizmoScale, "Scale", KeyCode.R, false, false, false, null);
			}

			//Onion
			bool isOnionButtonAvailable = false;
			if (Select.SelectionType == apSelection.SELECTION_TYPE.Animation ||
				Select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup)
			{
				isOnionButtonAvailable = true;
			}

			//Onion 단축키
			if (isOnionButtonAvailable)
			{
				AddHotKeyEvent(Controller.OnHotKeyEvent_OnionVisibleToggle, "Onion Skin Toggle", KeyCode.O, false, false, false, null);
			}

			//Bone Visible
			bool isBoneVisibleButtonAvailable = _selection.SelectionType == apSelection.SELECTION_TYPE.MeshGroup ||
				_selection.SelectionType == apSelection.SELECTION_TYPE.Animation ||
				_selection.SelectionType == apSelection.SELECTION_TYPE.Overall;

			if (isBoneVisibleButtonAvailable)
			{
				//단축키 B로 Bone 렌더링 정보를 토글할 수 있다.
				AddHotKeyEvent(OnHotKeyEvent_BoneVisibleToggle, "Change Bone Visiblity", KeyCode.B, false, false, false, null);
			}

			//Vertex 제어시 단축키
			apGizmos.TRANSFORM_UI_VALID gizmoUI_VetexTF = Gizmos.TransformUI_VertexTransform;

			if (gizmoUI_VetexTF != apGizmos.TRANSFORM_UI_VALID.Hide && Gizmos.IsSoftSelectionMode)
			{
				//Vertex Transform - Soft 툴
				//크기 조절 단축키
				AddHotKeyEvent(Gizmos.IncreaseSoftSelectionRadius, "Increase Brush Size", KeyCode.RightBracket, false, false, false, null);
				AddHotKeyEvent(Gizmos.DecreaseSoftSelectionRadius, "Decrease Brush Size", KeyCode.LeftBracket, false, false, false, null);
			}
			else if (gizmoUI_VetexTF != apGizmos.TRANSFORM_UI_VALID.Hide && Gizmos.IsBrushMode)
			{
				//Vertex Transform - Blur 툴
				AddHotKeyEvent(OnHotKey_IncBlurBrushRadius, "Increase Brush Size", KeyCode.RightBracket, false, false, false, null);
				AddHotKeyEvent(OnHotKey_DecBlurBrushRadius, "Decreash Brush Size", KeyCode.LeftBracket, false, false, false, null);
			}


			if (Select.SelectionType == apSelection.SELECTION_TYPE.Mesh
				&& Select.Mesh != null
				&& _meshEditMode == MESH_EDIT_MODE.MakeMesh
				&& _meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.Polygon)
			{
				//Delete 키로 Polygon을 삭제하는 단축키
				AddHotKeyEvent(Controller.RemoveSelectedMeshPolygon, "Remove Polygon", KeyCode.Delete, false, false, false, null);
			}
		}



		/// <summary>
		/// 단축키 B를 눌러서 Bone Visible을 바꾼다.
		/// </summary>
		/// <param name="paramObject"></param>
		private void OnHotKeyEvent_BoneVisibleToggle(object paramObject)
		{
			switch (_boneGUIRenderMode)
			{
				case BONE_RENDER_MODE.None: _boneGUIRenderMode = BONE_RENDER_MODE.Render; break;
				case BONE_RENDER_MODE.Render: _boneGUIRenderMode = BONE_RENDER_MODE.RenderOutline; break;
				case BONE_RENDER_MODE.RenderOutline: _boneGUIRenderMode = BONE_RENDER_MODE.None; break;
			}
			SaveEditorPref();
		}

		/// <summary>
		/// 단축키 Alt+W를 눌러서 FullScreen 모드를 바꾼다.
		/// </summary>
		/// <param name="paramObject"></param>
		private void OnHotKeyEvent_FullScreenToggle(object paramObject)
		{
			_isFullScreenGUI = !_isFullScreenGUI;
		}

		private void OnHotKey_IncBlurBrushRadius(object paramObject)
		{
			_blurRadius = Mathf.Clamp(_blurRadius + 10, 1, apGizmos.MAX_BRUSH_RADIUS);
		}
		private void OnHotKey_DecBlurBrushRadius(object paramObject)
		{
			_blurRadius = Mathf.Clamp(_blurRadius - 10, 1, apGizmos.MAX_BRUSH_RADIUS);
		}


		public void ResetHierarchyAll()
		{
			if (_portrait == null)
			{
				return;
			}

			//추가 3.29 : Hierarchy 재정렬
			if (_portrait._objectOrders == null)
			{
				_portrait._objectOrders = new apObjectOrders();
			}
			_portrait._objectOrders.Sync(_portrait);
			switch (_hierarchySortMode)
			{
				case HIERARCHY_SORT_MODE.RegOrder: _portrait._objectOrders.SortByRegOrder(); break;
				case HIERARCHY_SORT_MODE.AlphaNum: _portrait._objectOrders.SortByAlphaNumeric(); break;
				case HIERARCHY_SORT_MODE.Custom: _portrait._objectOrders.SortByCustom(); break;
			}

			Hierarchy.ResetAllUnits();
			_hierarchy_MeshGroup.ResetSubUnits();
			_hierarchy_AnimClip.ResetSubUnits();
		}

		public void RefreshControllerAndHierarchy(bool isRefreshTimeline)//<<인자 추가 19.5.21
		{
			if (_portrait == null)
			{
				//Repaint();
				SetRepaint();
				return;
			}

			//메시 그룹들을 체크한다.
			Controller.RefreshMeshGroups();
			Controller.CheckMeshEdgeWorkRemained();

			//추가 3.29 : Hierarchy 재정렬
			if (_portrait._objectOrders == null)
			{
				_portrait._objectOrders = new apObjectOrders();
			}
			_portrait._objectOrders.Sync(_portrait);
			switch (_hierarchySortMode)
			{
				case HIERARCHY_SORT_MODE.RegOrder: _portrait._objectOrders.SortByRegOrder(); break;
				case HIERARCHY_SORT_MODE.AlphaNum: _portrait._objectOrders.SortByAlphaNumeric(); break;
				case HIERARCHY_SORT_MODE.Custom: _portrait._objectOrders.SortByCustom(); break;
			}

			Hierarchy.RefreshUnits();
			_hierarchy_MeshGroup.RefreshUnits();
			_hierarchy_AnimClip.RefreshUnits();

			//이전
			//RefreshTimelineLayers(false);

			//변경 19.5.21
			if (isRefreshTimeline && Select.SelectionType == apSelection.SELECTION_TYPE.Animation)
			{
				RefreshTimelineLayers(REFRESH_TIMELINE_REQUEST.All, null);
			}


			//통계 재계산 요청
			Select.SetStatisticsRefresh();


			//Repaint();
			SetRepaint();
		}

		//추가 19.5.21 : RefreshTimelineLayers 함수 속도 최적화를 위해서 Request 인자를 받아서 처리한다.
		[Flags]
		public enum REFRESH_TIMELINE_REQUEST
		{
			None = 0,
			Info = 1,
			Timelines = 2,
			LinkKeyframeAndModifier = 4,
			All = 1 | 2 | 4
		}

		//이전
		//public void RefreshTimelineLayers(bool isReset)

		//변경 19.5.21 : 요청 세분화
		public void RefreshTimelineLayers(REFRESH_TIMELINE_REQUEST requestType, apAnimTimelineLayer targetTimelineLayer)
		{
			apAnimClip curAnimClip = Select.AnimClip;
			if (curAnimClip == null)
			{
				_prevAnimClipForTimeline = null;
				_timelineInfoList.Clear();

				//Common Keyframe을 갱신하자
				Select.RefreshCommonAnimKeyframes();
				return;
			}
			if (curAnimClip != _prevAnimClipForTimeline)
			{
				//강제로 리셋한다.
				//이전
				//isReset = true;

				//변경 19.5.21
				requestType |= REFRESH_TIMELINE_REQUEST.All;
				_prevAnimClipForTimeline = curAnimClip;
			}

			bool isRequest_ResetTimelineInfo = (int)(requestType & REFRESH_TIMELINE_REQUEST.Info) != 0;
			bool isRequest_RefreshTimelines = (int)(requestType & REFRESH_TIMELINE_REQUEST.Timelines) != 0;
			bool isRequest_LinkKeyframeAndModifier = (int)(requestType & REFRESH_TIMELINE_REQUEST.LinkKeyframeAndModifier) != 0;


			if (isRequest_RefreshTimelines) //<조건문 추가 19.5.21
			{
				//타임라인값도 리프레시 (Sorting 등)
				curAnimClip.RefreshTimelines(targetTimelineLayer);
			}

			//이전
			//if (isReset || _timelineInfoList.Count == 0)

			//조건문 변경 19.5.21
			if (isRequest_ResetTimelineInfo || _timelineInfoList.Count == 0)
			{
				_timelineInfoList.Clear();
				//AnimClip에 맞게 리스트를 다시 만든다.

				List<apAnimTimeline> timelines = curAnimClip._timelines;
				for (int iTimeline = 0; iTimeline < timelines.Count; iTimeline++)
				{
					apAnimTimeline timeline = timelines[iTimeline];
					apTimelineLayerInfo timelineInfo = new apTimelineLayerInfo(timeline);
					_timelineInfoList.Add(timelineInfo);


					List<apTimelineLayerInfo> subLayers = new List<apTimelineLayerInfo>();
					for (int iLayer = 0; iLayer < timeline._layers.Count; iLayer++)
					{
						apAnimTimelineLayer animLayer = timeline._layers[iLayer];
						subLayers.Add(new apTimelineLayerInfo(animLayer, timeline, timelineInfo));
					}

					//정렬을 여기서 한다.
					switch (_timelineInfoSortType)
					{
						case TIMELINE_INFO_SORT.Registered:
							//정렬을 안한다.
							break;

						case TIMELINE_INFO_SORT.Depth:
							if (timeline._linkType == apAnimClip.LINK_TYPE.AnimatedModifier)
							{
								//Modifier가 Transform을 지원하는 경우
								//Bone이 위쪽에 속한다.
								subLayers.Sort(delegate (apTimelineLayerInfo a, apTimelineLayerInfo b)
								{
									if (a._layerType == b._layerType)
									{
										if (a._layerType == apTimelineLayerInfo.LAYER_TYPE.Transform)
										{
											return b.Depth - a.Depth;
										}
										else
										{
											return a.Depth - b.Depth;
										}

									}
									else
									{
										return (int)a._layerType - (int)b._layerType;
									}
								});
							}
							break;

						case TIMELINE_INFO_SORT.ABC:
							subLayers.Sort(delegate (apTimelineLayerInfo a, apTimelineLayerInfo b)
							{
								return string.Compare(a.DisplayName, b.DisplayName);
							});
							break;
					}

					//정렬 된 걸 넣어주자
					for (int iSub = 0; iSub < subLayers.Count; iSub++)
					{
						_timelineInfoList.Add(subLayers[iSub]);
					}

				}
			}

			//조건문 추가 19.5.21
			//키프레임-모디파이어 연동도 해주자
			if (isRequest_LinkKeyframeAndModifier)
			{
				if (targetTimelineLayer == null)
				{
					//전체 링크
					for (int iTimeline = 0; iTimeline < curAnimClip._timelines.Count; iTimeline++)
					{
						apAnimTimeline timeline = curAnimClip._timelines[iTimeline];
						if (timeline._linkType == apAnimClip.LINK_TYPE.AnimatedModifier &&
							timeline._linkedModifier != null)
						{
							for (int iLayer = 0; iLayer < timeline._layers.Count; iLayer++)
							{
								apAnimTimelineLayer layer = timeline._layers[iLayer];

								apModifierParamSetGroup paramSetGroup = timeline._linkedModifier._paramSetGroup_controller.Find(delegate (apModifierParamSetGroup a)
								{
									return a._keyAnimTimelineLayer == layer;
								});

								if (paramSetGroup != null)
								{
									for (int iKey = 0; iKey < layer._keyframes.Count; iKey++)
									{
										apAnimKeyframe keyframe = layer._keyframes[iKey];
										apModifierParamSet paramSet = paramSetGroup._paramSetList.Find(delegate (apModifierParamSet a)
										{
											return a.SyncKeyframe == keyframe;
										});

										if (paramSet != null && paramSet._meshData.Count > 0)
										{
											keyframe.LinkModMesh_Editor(paramSet, paramSet._meshData[0]);
										}
										else if (paramSet != null && paramSet._boneData.Count > 0)//<<추가 : boneData => ModBone
										{
											keyframe.LinkModBone_Editor(paramSet, paramSet._boneData[0]);
										}
										else
										{
											keyframe.LinkModMesh_Editor(null, null);//<<null, null을 넣으면 ModBone도 Null이 된다.
										}
									}
								}
							}

						}
					}
				}
				else
				{
					//특정 TimelineLayer만 링크
					apAnimTimeline parentTimeline = targetTimelineLayer._parentTimeline;

					if (parentTimeline != null &&
						parentTimeline._linkType == apAnimClip.LINK_TYPE.AnimatedModifier &&
							parentTimeline._linkedModifier != null)
					{
						apModifierParamSetGroup paramSetGroup = parentTimeline._linkedModifier._paramSetGroup_controller.Find(delegate (apModifierParamSetGroup a)
						{
							return a._keyAnimTimelineLayer == targetTimelineLayer;
						});

						if (paramSetGroup != null)
						{
							for (int iKey = 0; iKey < targetTimelineLayer._keyframes.Count; iKey++)
							{
								apAnimKeyframe keyframe = targetTimelineLayer._keyframes[iKey];
								apModifierParamSet paramSet = paramSetGroup._paramSetList.Find(delegate (apModifierParamSet a)
								{
									return a.SyncKeyframe == keyframe;
								});

								if (paramSet != null && paramSet._meshData.Count > 0)
								{
									keyframe.LinkModMesh_Editor(paramSet, paramSet._meshData[0]);
								}
								else if (paramSet != null && paramSet._boneData.Count > 0)//<<추가 : boneData => ModBone
								{
									keyframe.LinkModBone_Editor(paramSet, paramSet._boneData[0]);
								}
								else
								{
									keyframe.LinkModMesh_Editor(null, null);//<<null, null을 넣으면 ModBone도 Null이 된다.
								}
							}
						}
					}
				}

			}


			//Select / Available 체크를 하자 : 이건 상시
			for (int i = 0; i < _timelineInfoList.Count; i++)
			{
				apTimelineLayerInfo info = _timelineInfoList[i];

				info._isSelected = false;
				info._isAvailable = false;

				if (info._isTimeline)
				{
					if (Select.ExAnimEditingMode != apSelection.EX_EDIT.None)
					{
						if (info._timeline == Select.AnimTimeline)
						{ info._isAvailable = true; }
					}
					else
					{
						info._isAvailable = true;
					}

					if (info._isAvailable)
					{
						if (Select.AnimTimelineLayer == null)
						{
							if (info._timeline == Select.AnimTimeline)
							{
								info._isSelected = true;
							}
						}
					}
				}
				else
				{
					if (Select.ExAnimEditingMode != apSelection.EX_EDIT.None)
					{
						if (info._parentTimeline == Select.AnimTimeline)
						{
							info._isAvailable = true;
							info._isSelected = (info._layer == Select.AnimTimelineLayer);

						}
					}
					else
					{
						info._isAvailable = true;
						info._isSelected = (info._layer == Select.AnimTimelineLayer);
					}
				}
			}

			//Common Keyframe을 갱신하자
			Select.RefreshCommonAnimKeyframes();


			//통계 재계산 요청
			Select.SetStatisticsRefresh();

			SetRepaint();
		}

		public void ShowAllTimelineLayers()
		{
			for (int i = 0; i < _timelineInfoList.Count; i++)
			{
				_timelineInfoList[i].ShowLayer();
			}
		}

		public void SyncHierarchyOrders()
		{
			if (_portrait == null)
			{
				return;
			}
			//추가 3.29 : Hierarchy Sort
			if (_portrait._objectOrders == null)
			{
				_portrait._objectOrders = new apObjectOrders();
			}
			_portrait._objectOrders.Sync(_portrait);
			switch (HierarchySortMode)
			{
				case HIERARCHY_SORT_MODE.RegOrder:
					_portrait._objectOrders.SortByRegOrder();
					break;
				case HIERARCHY_SORT_MODE.AlphaNum:
					_portrait._objectOrders.SortByAlphaNumeric();
					break;
				case HIERARCHY_SORT_MODE.Custom:
					_portrait._objectOrders.SortByCustom();
					break;
			}
		}

		#region [미사용 코드] 탭 변환 코드
		//private void ChangeTab(TAB nextTab)
		//{
		//	if (_portrait == null && _tab != TAB.ProjectSetting)
		//	{
		//		Debug.LogError("Portrait Object is not Implemented");

		//		_tab = TAB.ProjectSetting;
		//		_scroll_MainLeft = Vector2.zero;
		//		_scroll_MainCenter = Vector2.zero;
		//		_scroll_MainRight = Vector2.zero;
		//		_scroll_Bottom = Vector2.zero;

		//		Repaint();

		//		return;
		//	}
		//	if(nextTab != TAB.MeshEdit)
		//	{
		//		Controller.CheckMeshEdgeWorkRemained();
		//	}

		//	_tab = nextTab;
		//	_scroll_MainLeft = Vector2.zero;
		//	_scroll_MainCenter = Vector2.zero;
		//	_scroll_MainRight = Vector2.zero;
		//	_scroll_Bottom = Vector2.zero;

		//	_meshEditMode = MESH_EDIT_MODE.None;

		//	Hierarchy.RefreshUnits();
		//	Repaint();
		//} 
		#endregion

		#region [미사용 코드] : Gizmo 출력 여부 관련 코드
		//private void CheckGizmoAvailable()
		//{
		//	bool isLock = false;

		//	//TODO
		//	//언제 기즈모가 나오는지 결정하는 코드

		//	//switch (_tab)
		//	//{
		//	//	case TAB.ProjectSetting:
		//	//		isLock = true;
		//	//		break;

		//	//	case TAB.MeshEdit:
		//	//		switch (_meshEditMode)
		//	//		{
		//	//			case MESH_EDIT_MODE.None:
		//	//			case MESH_EDIT_MODE.Modify:
		//	//			case MESH_EDIT_MODE.AddVertex:
		//	//			case MESH_EDIT_MODE.LinkEdge:
		//	//			case MESH_EDIT_MODE.VolumeWeight:
		//	//			case MESH_EDIT_MODE.PhysicWeight:
		//	//				isLock = true;
		//	//				break;

		//	//			case MESH_EDIT_MODE.PivotEdit:
		//	//				//isLock = false;
		//	//				isLock = false;
		//	//				break;
		//	//		}
		//	//		break;

		//	//	case TAB.FaceEdit:
		//	//		isLock = true;
		//	//		break;

		//	//	case TAB.RigEdit:
		//	//		isLock = false;
		//	//		break;

		//	//	case TAB.Animation:
		//	//		isLock = false;
		//	//		break;
		//	//}
		//	//_gizmos.SetLock(isLock);
		//} 
		#endregion
		//--------------------------------------------------------------


		//--------------------------------------------------------------
		private object _loadKey_MakeNewPortrait = null;
		// 각 레이아웃 별 GUI Render
		private int GUI_MainLeft_Upper(int width)
		{
			if(_guiContent_MainLeftUpper_MakeNewPortrait == null)
			{
				_guiContent_MainLeftUpper_MakeNewPortrait = apGUIContentWrapper.Make("   " + GetUIWord(UIWORD.MakeNewPortrait), ImageSet.Get(apImageSet.PRESET.Hierarchy_MakeNewPortrait), "Create a new Portrait");
			}
			if(_guiContent_MainLeftUpper_RefreshToLoad == null)
			{
				_guiContent_MainLeftUpper_RefreshToLoad = apGUIContentWrapper.Make("   " + GetUIWord(UIWORD.RefreshToLoad), ImageSet.Get(apImageSet.PRESET.Controller_Default), "Search Portraits again in the scene");
			}
			if(_guiContent_MainLeftUpper_LoadBackupFile == null)
			{
				_guiContent_MainLeftUpper_LoadBackupFile = apGUIContentWrapper.Make(GetUIWord(UIWORD.LoadBackupFile), false, "Open a Portrait saved as a backup file. It will be created as a new Portrait");
			}
			

			if (_portrait == null)
			{
				bool isRefresh = false;
				if (GUILayout.Button(_guiContent_MainLeftUpper_MakeNewPortrait.Content, GUILayout.Height(45)))
				{
					//Portrait를 생성하는 Dialog를 띄우자
					_loadKey_MakeNewPortrait = apDialog_NewPortrait.ShowDialog(this, OnMakeNewPortraitResult);
				}

				GUILayout.Space(5);
				if (GUILayout.Button(_guiContent_MainLeftUpper_RefreshToLoad.Content, GUILayout.Height(25)))
				{
					isRefresh = true;
				}
				if (GUILayout.Button(_guiContent_MainLeftUpper_LoadBackupFile.Content, GUILayout.Height(20)))
				{
					if (apVersion.I.IsDemo)
					{
						//추가 : 데모 버전일 때에는 백업 파일을 로드할 수 없다.
						EditorUtility.DisplayDialog(
										GetText(TEXT.DemoLimitation_Title),
										GetText(TEXT.DemoLimitation_Body),
										GetText(TEXT.Okay));
					}
					else
					{
						string strPath = EditorUtility.OpenFilePanel("Load Backup File", "", "bck");
						if (!string.IsNullOrEmpty(strPath))
						{
							//Debug.Log("Load Backup File [" + strPath + "]");

							_isMakePortraitRequestFromBackupFile = true;
							_requestedLoadedBackupPortraitFilePath = strPath;
						}
					}
				}

				if (_portraitsInScene.Count == 0 && !_isPortraitListLoaded)
				{
					isRefresh = true;
				}

				if (isRefresh)
				{
					//씬에 있는 Portrait를 찾는다.
					//추가) 썸네일도 표시
					_portraitsInScene.Clear();
					apPortrait[] portraits = UnityEngine.Object.FindObjectsOfType<apPortrait>();
					if (portraits != null)
					{
						for (int i = 0; i < portraits.Length; i++)
						{
							//Opt Portrait는 생략한다.
							if (portraits[i]._isOptimizedPortrait)
							{
								continue;
							}

							//썸네일을 연결하자
							string thumnailPath = portraits[i]._imageFilePath_Thumbnail;
							if (string.IsNullOrEmpty(thumnailPath))
							{
								portraits[i]._thumbnailImage = null;
							}
							else
							{
								Texture2D thumnailImage = AssetDatabase.LoadAssetAtPath<Texture2D>(thumnailPath);

								if (thumnailImage != null)
								{
									//추가 : 크기가 이상하다. 보정한다.
									int thumbWidth = thumnailImage.width;
									int thumbHeight = thumnailImage.height;
									float thumbAspectRatio = (float)thumbWidth / (float)thumbHeight;
									if (thumbAspectRatio < 1.8f)//원래는 2.0 근처여야 한다.
									{

										TextureImporter ti = TextureImporter.GetAtPath(thumnailPath) as TextureImporter;
										ti.textureCompression = TextureImporterCompression.Uncompressed;
										ti.SaveAndReimport();

										thumnailImage = AssetDatabase.LoadAssetAtPath<Texture2D>(thumnailPath);
										//Debug.Log("iOS에서 크기가 이상하여 보정함 : " + thumbWidth + "x" + thumbHeight + " >> " + thumnailImage.width + "x" + thumnailImage.height);
									}


								}
								portraits[i]._thumbnailImage = thumnailImage;
							}

							_portraitsInScene.Add(portraits[i]);
						}
					}

					_isPortraitListLoaded = true;
				}
				return 85;
			}

			if (_tabLeft == TAB_LEFT.Hierarchy)
			{
				int filterIconSize = (width / 8) - 2;
				int filterIconWidth = filterIconSize + 3;

				//Hierarchy의 필터 선택 버튼들
				//변경 3.28 : All, None 버튼이 사라지고, Sort Mode가 추가되었다.
				EditorGUILayout.BeginHorizontal(GUILayout.Width(width), GUILayout.Height(filterIconSize + 5));
				GUILayout.Space(7);

				if (!_isHierarchyOrderEditEnabled)
				{
					//>> All Filter 삭제
					//if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_All), IsHierarchyFilterContain(HIERARCHY_FILTER.All), true, filterIconSize, filterIconSize, "Show All"))
					//{ SetHierarchyFilter(HIERARCHY_FILTER.All, true); }//All

					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_Root), IsHierarchyFilterContain(HIERARCHY_FILTER.RootUnit), true, filterIconWidth, filterIconSize, "Root Units"))
					{ SetHierarchyFilter(HIERARCHY_FILTER.RootUnit, !IsHierarchyFilterContain(HIERARCHY_FILTER.RootUnit)); } //Root Toggle
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_Image), IsHierarchyFilterContain(HIERARCHY_FILTER.Image), true, filterIconWidth, filterIconSize, "Images"))
					{ SetHierarchyFilter(HIERARCHY_FILTER.Image, !IsHierarchyFilterContain(HIERARCHY_FILTER.Image)); }//Image Toggle
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_Mesh), IsHierarchyFilterContain(HIERARCHY_FILTER.Mesh), true, filterIconWidth, filterIconSize, "Meshes"))
					{ SetHierarchyFilter(HIERARCHY_FILTER.Mesh, !IsHierarchyFilterContain(HIERARCHY_FILTER.Mesh)); }//Mesh Toggle
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_MeshGroup), IsHierarchyFilterContain(HIERARCHY_FILTER.MeshGroup), true, filterIconWidth, filterIconSize, "Mesh Groups"))
					{ SetHierarchyFilter(HIERARCHY_FILTER.MeshGroup, !IsHierarchyFilterContain(HIERARCHY_FILTER.MeshGroup)); }//MeshGroup Toggle
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_Animation), IsHierarchyFilterContain(HIERARCHY_FILTER.Animation), true, filterIconWidth, filterIconSize, "Animation Clips"))
					{ SetHierarchyFilter(HIERARCHY_FILTER.Animation, !IsHierarchyFilterContain(HIERARCHY_FILTER.Animation)); }//Animation Toggle
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_Param), IsHierarchyFilterContain(HIERARCHY_FILTER.Param), true, filterIconWidth, filterIconSize, "Control Parameters"))
					{ SetHierarchyFilter(HIERARCHY_FILTER.Param, !IsHierarchyFilterContain(HIERARCHY_FILTER.Param)); }//Param Toggle

					//>> None Filter 삭제
					//if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_None), IsHierarchyFilterContain(HIERARCHY_FILTER.None), true, filterIconSize, filterIconSize, "Hide All"))
					//{ SetHierarchyFilter(HIERARCHY_FILTER.None, true); }//None
				}
				else
				{
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_SortMode_RegOrder), _hierarchySortMode == HIERARCHY_SORT_MODE.RegOrder, true, filterIconWidth + 8, filterIconSize, "Show in order of registration"))
					{
						_hierarchySortMode = HIERARCHY_SORT_MODE.RegOrder;
						SaveEditorPref();
						RefreshControllerAndHierarchy(false);
					}
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_SortMode_AlphaNum), _hierarchySortMode == HIERARCHY_SORT_MODE.AlphaNum, true, filterIconWidth + 8, filterIconSize, "Show in order of name's alphanumeric"))
					{
						_hierarchySortMode = HIERARCHY_SORT_MODE.AlphaNum;
						SaveEditorPref();
						RefreshControllerAndHierarchy(false);
					}
					if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_SortMode_Custom), _hierarchySortMode == HIERARCHY_SORT_MODE.Custom, true, filterIconWidth + 8, filterIconSize, "Show in order of custom"))
					{
						_hierarchySortMode = HIERARCHY_SORT_MODE.Custom;
						SaveEditorPref();
						RefreshControllerAndHierarchy(false);
					}
					GUILayout.Space(75);
				}
				//추가 3.28 : Hierarchy Sort Mode
				GUILayout.Space(7);
				if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_SortMode), _isHierarchyOrderEditEnabled, true, filterIconWidth + 2, filterIconSize, "Toggle Sort Mode"))
				{
					_isHierarchyOrderEditEnabled = !_isHierarchyOrderEditEnabled;
				}

				EditorGUILayout.EndHorizontal();

				return filterIconSize + 5;
			}
			else
			{
				return Controller.GUI_Controller_Upper(width);
			}
		}


		private void OnMakeNewPortraitResult(bool isSuccess, object loadKey, string name)
		{
			if (!isSuccess || loadKey != _loadKey_MakeNewPortrait)
			{
				_loadKey_MakeNewPortrait = null;
				return;
			}
			_loadKey_MakeNewPortrait = null;

			//Portrait를 만들어준다.
			_isMakePortraitRequest = true;
			_requestedNewPortraitName = name;
			if (string.IsNullOrEmpty(_requestedNewPortraitName))
			{
				_requestedNewPortraitName = "<No Named Portrait>";
			}


		}





		private void GUI_MainLeft(int width, int height, Vector2 scroll, bool isGUIEvent)
		{


			GUILayout.Space(20);

			if (_portrait == null)
			{
				int portraitSelectWidth = width - 10;
				int thumbnailHeight = 60;
				int thumbnailWidth = thumbnailHeight * 2;

				//삭제 > 래퍼 이용
				//GUIStyle guiStyle_Thumb = new GUIStyle(GUI.skin.box);
				//guiStyle_Thumb.margin = GUI.skin.label.margin;
				////guiStyle_Thumb.padding = GUI.skin.label.padding;
				//guiStyle_Thumb.padding = new RectOffset(0, 0, 0, 0);


				int selectBtnWidth = portraitSelectWidth - (thumbnailWidth);
				int portraitSelectHeight = thumbnailHeight + 24;

				for (int i = 0; i < _portraitsInScene.Count; i++)
				{
					if (_portraitsInScene[i] == null)
					{
						_portraitsInScene.Clear();
						break;
					}
					EditorGUILayout.BeginHorizontal(GUILayout.Width(width - 10), GUILayout.Height(portraitSelectHeight));
					GUILayout.Space(5);
					EditorGUILayout.BeginVertical();



					EditorGUILayout.LabelField(_portraitsInScene[i].transform.name, GUILayout.Width(portraitSelectWidth));

					EditorGUILayout.BeginHorizontal();

					GUILayout.Box(	_portraitsInScene[i]._thumbnailImage, 
									//guiStyle_Thumb, 
									GUIStyleWrapper.Box_LabelMargin_Padding0,
									GUILayout.Width(thumbnailWidth), GUILayout.Height(thumbnailHeight));

					if (GUILayout.Button(GetUIWord(UIWORD.Select), GUILayout.Width(selectBtnWidth), GUILayout.Height(thumbnailHeight)))
					{
						//바뀌었다.
						if (!_portraitsInScene[i]._isOptimizedPortrait)
						{
							_portrait = _portraitsInScene[i];
							if (_portrait != null)
							{
								Controller.InitTmpValues();
								_selection.SetPortrait(_portrait);

								//Portrait의 레퍼런스들을 연결해주자
								Controller.PortraitReadyToEdit();


								//Selection.activeGameObject = _portrait.gameObject;
								Selection.activeGameObject = null;//<<선택을 해제해준다. 프로파일러를 도와줘야져

								//시작은 RootUnit
								_selection.SetOverallDefault();

								OnAnyObjectAddedOrRemoved();
							}
							else
							{
								Controller.InitTmpValues();
								_selection.SetNone();
							}

							SyncHierarchyOrders();

							_hierarchy.ResetAllUnits();
							_hierarchy_MeshGroup.ResetSubUnits();
							_hierarchy_AnimClip.ResetSubUnits();
						}
					}
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();

					GUILayout.Space(10);
				}


			}
			else
			{
				if (_tabLeft == TAB_LEFT.Hierarchy)
				{

					Hierarchy.GUI_RenderHierarchy(width, _hierarchyFilter, scroll, height - 20, isGUIEvent, _isHierarchyOrderEditEnabled && _hierarchySortMode == HIERARCHY_SORT_MODE.Custom);
				}
				else
				{

					//이전 코드
					//apControllerGL.SetMouseState(
					//	_mouseBtn[MOUSE_BTN_LEFT_NOT_BOUND].Status,
					//	apMouse.PosNotBound,
					//	Event.current.isMouse
					//	);

					apControllerGL.SetMouseState(
						_mouseSet.GetStatus(apMouseSet.Button.LeftNoBound, apMouseSet.ACTION.ControllerGL),
						_mouseSet.PosNotBound,
						Event.current.isMouse
						);

					//Profiler.BeginSample("Left : Controller UI");
					//컨트롤 파라미터를 처리하자
					Controller.GUI_Controller(width, height, (int)scroll.y);
					//Profiler.EndSample();

				}
			}
		}


		private void GUI_MainCenter(int width, int height)
		{
			if (Event.current.type != EventType.Repaint &&
				!Event.current.isMouse && !Event.current.isKey)
			{
				return;
			}

			float deltaTime = 0.0f;
			if (Event.current.type == EventType.Repaint)
			{
				deltaTime = DeltaTime_Repaint;
			}

			//--------------------------------------------------------
			//      업데이트 / 입력
			//--------------------------------------------------------

			//Input은 여기서 미리 처리한다.
			//--------------------------------------------------
			bool isMeshGroupUpdatable = _isUpdateWhenInputEvent || Event.current.type == EventType.Repaint;
			if (_isUpdateWhenInputEvent)
			{
				_isUpdateWhenInputEvent = false;
			}

			//추가 : GUI Repaint할 시간이 아니더라도, MeshGroup이 변경되었다면
			//GUI를 Repaint해야한다.
			if (!_isValidGUIRepaint)
			{
				if (_isMeshGroupChangedByEditor)
				{
					_isValidGUIRepaint = true;
				}
			}
			_isMeshGroupChangedByEditor = false;


			//추가 : MeshGroup를 업데이트 하기 위한 옵션을 설정한다.
			bool isUpdate_BoneIKMatrix = _selection.IsBoneIKMatrixUpdatable;
			bool isUpdate_BoneIKRigging = _selection.IsBoneIKRiggingUpdatable;
			bool isRender_BoneIK = _selection.IsBoneIKRenderable && isUpdate_BoneIKMatrix;



			//클릭 후 GUIFocust를 릴리즈하자
			Controller.GUI_Input_CheckClickInCenter();

			switch (_selection.SelectionType)
			{
				case apSelection.SELECTION_TYPE.None:
					break;

				case apSelection.SELECTION_TYPE.Overall:
					{

						if (Event.current.type == EventType.Repaint)//업데이트는 Repaint에서만
						{
							if (Select.RootUnit != null)
							{
								//Debug.Log("Draw >>>>>>>>>>>>>>>");
								if (Select.RootUnitAnimClip != null)
								{
									//실행중인 AnimClip이 있다.
									int curFrame = Select.RootUnitAnimClip.CurFrame;

									if (_isValidGUIRepaint)
									{
										_portrait.ForceManager.Update(deltaTime);//<<힘 업데이트 추가

										//애니메이션 재생!!!
										//Profiler.BeginSample("Anim - Root Animation");

										//추가
										if (Select.RootUnitAnimClip._targetMeshGroup != null)
										{
											Select.RootUnitAnimClip._targetMeshGroup.SetBoneIKEnabled(isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging);
										}

										Select.RootUnitAnimClip.Update_Editor(
															deltaTime,
															true,
															isUpdate_BoneIKMatrix,
															isUpdate_BoneIKRigging);
										//Profiler.EndSample();


									}

									//프레임이 바뀌면 AutoScroll 옵션을 적용한다.
									if (curFrame != Select.RootUnitAnimClip.CurFrame)
									{
										if (_isAnimAutoScroll)
										{
											Select.SetAutoAnimScroll();
										}
									}
								}
								else
								{
									//AnimClip이 없다면 자체 실행
									Select.RootUnit.Update(deltaTime, isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging);
								}

							}
						}
					}
					break;

				case apSelection.SELECTION_TYPE.Param:
					break;

				case apSelection.SELECTION_TYPE.ImageRes:
					break;

				case apSelection.SELECTION_TYPE.Mesh:
					if (_selection.Mesh != null)
					{
						switch (_meshEditMode)
						{
							case MESH_EDIT_MODE.Setting:
								break;

							case MESH_EDIT_MODE.Modify:
								Controller.GUI_Input_Modify(deltaTime);
								break;

							case MESH_EDIT_MODE.MakeMesh:
								if (_meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.TRS
									|| _meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.AutoGenerate)
								{
									//자동 생성은 별도의 입력
									Controller.GUI_Input_MakeMesh_AutoGen_TRS(deltaTime);

								}
								else
								{
									Controller.GUI_Input_MakeMesh(_meshEditeMode_MakeMesh);
								}
								break;

							case MESH_EDIT_MODE.PivotEdit:
								Controller.GUI_Input_PivotEdit(deltaTime);
								break;
						}
					}
					break;

				case apSelection.SELECTION_TYPE.MeshGroup:
					{
						switch (_meshGroupEditMode)
						{
							case MESHGROUP_EDIT_MODE.Setting:
								Controller.GUI_Input_MeshGroup_Setting(deltaTime);
								break;

							case MESHGROUP_EDIT_MODE.Bone:
								//Debug.Log("Bone Edit : " + Event.current.type);
								Controller.GUI_Input_MeshGroup_Bone(deltaTime);
								break;

							case MESHGROUP_EDIT_MODE.Modifier:
								{
									apModifierBase.MODIFIER_TYPE modifierType = apModifierBase.MODIFIER_TYPE.Base;
									if (Select.Modifier != null)
									{
										modifierType = Select.Modifier.ModifierType;
									}
									Controller.GUI_Input_MeshGroup_Modifier(modifierType, deltaTime);

								}
								break;
						}

						if (Event.current.type == EventType.Repaint)//업데이트는 Repaint에서만
						{
							if (Select.MeshGroup != null)
							{
								if (_isValidGUIRepaint)
								{

									_portrait.ForceManager.Update(deltaTime);//<<힘 업데이트 추가

									//변경 : 업데이트 가능한 상태에서만 업데이트를 한다.
									if (isMeshGroupUpdatable)
									{
										//Profiler.BeginSample("MeshGroup Update");
										//1. Render Unit을 돌면서 렌더링을 한다.
										Select.MeshGroup.SetBoneIKEnabled(isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging);

										Select.MeshGroup.UpdateRenderUnits(deltaTime, true);

										Select.MeshGroup.SetBoneIKEnabled(false, false);

										//추가 : Bone GUI를 여기서 업데이트한번 해야한다.
										//Select.MeshGroup.BoneGUIUpdate(false);
										//Profiler.EndSample();
									}
								}
							}
						}


					}
					break;

				case apSelection.SELECTION_TYPE.Animation:
					{

						Controller.GUI_Input_Animation(deltaTime);

						if (Event.current.type == EventType.Repaint)//업데이트는 Repaint에서만
						{
							if (Select.AnimClip != null)
							{
								if (_isValidGUIRepaint)
								{
									_portrait.ForceManager.Update(deltaTime);//<<힘 업데이트 추가

									//변경 : 업데이트 가능한 상태에서만 업데이트를 한다.
									if (isMeshGroupUpdatable)
									{
										int curFrame = Select.AnimClip.CurFrame;
										//애니메이션 업데이트를 해야..
										Select.AnimClip.Update_Editor(deltaTime,
											Select.ExAnimEditingMode != apSelection.EX_EDIT.None,
											isUpdate_BoneIKMatrix,
											isUpdate_BoneIKRigging);

										//프레임이 바뀌면 AutoScroll 옵션을 적용한다.
										if (curFrame != Select.AnimClip.CurFrame)
										{
											if (_isAnimAutoScroll)
											{
												Select.SetAutoAnimScroll();
											}
										}
									}
								}
							}
						}


					}
					break;
			}



			if (_isValidGUIRepaint)
			{
				_isValidGUIRepaint = false;
			}



			//--------------------------------------------------------
			//      렌더링
			//--------------------------------------------------------

			//렌더링은 Repaint에서만
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			//만약 렌더링 클리핑을 갱신할 필요가 있다면 처리할 것
			if (_isRefreshClippingGL)
			{
				_isRefreshClippingGL = false;
				apGL.RefreshScreenSizeToBatch();
			}

			apGL.DrawGrid(_colorOption_GridCenter, _colorOption_Grid);



			switch (_selection.SelectionType)
			{
				case apSelection.SELECTION_TYPE.None:
					break;

				case apSelection.SELECTION_TYPE.Overall:
					{
						//_portrait._rootUnit.Update(DeltaFrameTime);

						if (Select.RootUnit != null)
						{
							if (Select.RootUnit._childMeshGroup != null)
							{
								apMeshGroup rootMeshGroup = Select.RootUnit._childMeshGroup;
								#region [미사용 코드] 함수로 대체한다.
								//for (int iUnit = 0; iUnit < rootMeshGroup._renderUnits_All.Count; iUnit++)
								//{
								//	apRenderUnit renderUnit = rootMeshGroup._renderUnits_All[iUnit];
								//	if (renderUnit._unitType == apRenderUnit.UNIT_TYPE.Mesh)
								//	{
								//		if (renderUnit._meshTransform != null)
								//		{
								//			if(!renderUnit._meshTransform._isVisible_Default)
								//			{
								//				continue;
								//			}

								//			if (renderUnit._meshTransform._isClipping_Parent)
								//			{
								//				//Clipping이면 Child (최대3)까지 마스크 상태로 출력한다.
								//				apGL.DrawRenderUnit_ClippingParent(	renderUnit, 
								//													renderUnit._meshTransform._clipChildMeshTransforms, 
								//													renderUnit._meshTransform._clipChildRenderUnits,
								//													VertController, 
								//													Select);
								//			}
								//			else if (renderUnit._meshTransform._isClipping_Child)
								//			{
								//				//렌더링은 생략한다.
								//			}
								//			else
								//			{
								//				apGL.DrawRenderUnit(renderUnit, apGL.RENDER_TYPE.Default, VertController, Select);
								//			}
								//		}
								//	}
								//} 
								#endregion

								RenderMeshGroup(rootMeshGroup, apGL.RENDER_TYPE.Default, apGL.RENDER_TYPE.Default, null, null, true, _boneGUIRenderMode, _meshGUIRenderMode, isRender_BoneIK, BONE_RENDER_TARGET.AllBones);

								//Debug.Log("<<<<<<<<<<<<<<<<<<<<<<<<<<");
							}
						}

					}
					break;

				case apSelection.SELECTION_TYPE.Param:
					if (_selection.Param != null)
					{
						apGL.DrawBox(Vector2.zero, 200, 100, Color.cyan, true);
						apGL.DrawText("Param Edit", new Vector2(-30, 30), 70, Color.yellow);
						apGL.DrawText(_selection.Param._keyName, new Vector2(-30, -15), 200, Color.yellow);
					}
					break;

				case apSelection.SELECTION_TYPE.ImageRes:
					if (_selection.TextureData != null)
					{
						apGL.DrawTexture(_selection.TextureData._image,
											Vector2.zero,
											_selection.TextureData._width,
											_selection.TextureData._height,
											new Color(0.5f, 0.5f, 0.5f, 1.0f));
					}
					break;

				case apSelection.SELECTION_TYPE.Mesh:
					if (_selection.Mesh != null)
					{
						apGL.RENDER_TYPE renderType = apGL.RENDER_TYPE.Default;

						bool isEdgeExpectRender = false;
						bool isEdgeExpectRenderSnapToEdge = false;
						bool isEdgeExpectRenderSnapToVertex = false;
						switch (_meshEditMode)
						{
							case MESH_EDIT_MODE.Setting:
								renderType |= apGL.RENDER_TYPE.Outlines;
								break;

							case MESH_EDIT_MODE.Modify:
								//Controller.GUI_Input_Modify();
								renderType |= apGL.RENDER_TYPE.Vertex;
								renderType |= apGL.RENDER_TYPE.AllEdges;
								renderType |= apGL.RENDER_TYPE.ShadeAllMesh;
								if (_meshEditZDepthView == MESH_EDIT_RENDER_MODE.ZDepth)
								{
									renderType |= apGL.RENDER_TYPE.VolumeWeightColor;
								}
								break;

							case MESH_EDIT_MODE.MakeMesh:
								//case MESH_EDIT_MODE.AddVertex:
								//Controller.GUI_Input_AddVertex();
								if (_meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.Polygon)
								{
									renderType |= apGL.RENDER_TYPE.Vertex;
									renderType |= apGL.RENDER_TYPE.AllEdges;
									renderType |= apGL.RENDER_TYPE.ShadeAllMesh;
									renderType |= apGL.RENDER_TYPE.PolygonOutline;
								}
								else
								{
									renderType |= apGL.RENDER_TYPE.Vertex;
									renderType |= apGL.RENDER_TYPE.AllEdges;
									renderType |= apGL.RENDER_TYPE.ShadeAllMesh;
									if (_meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.VertexAndEdge ||
										_meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.EdgeOnly)
									{
										isEdgeExpectRender = true;
									}
									if (_meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.VertexAndEdge ||
										_meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.VertexOnly)
									{
										isEdgeExpectRenderSnapToEdge = true;
									}
									isEdgeExpectRenderSnapToVertex = true;
								}
								break;

							case MESH_EDIT_MODE.PivotEdit:
								//Controller.GUI_Input_PivotEdit();
								renderType |= apGL.RENDER_TYPE.Vertex;
								renderType |= apGL.RENDER_TYPE.Outlines;
								break;

						}




						apGL.DrawMesh(_selection.Mesh,
								apMatrix3x3.identity,
								new Color(0.5f, 0.5f, 0.5f, 1.0f),
								renderType,
								VertController, this,
								//apMouse.Pos//<<이전
								_mouseSet.Pos//<<변경
								);

						if (_meshEditMode == MESH_EDIT_MODE.MakeMesh && _meshEditMirrorMode == MESH_EDIT_MIRROR_MODE.Mirror)
						{
							MirrorSet.Refresh(Select.Mesh, false);
							if (_meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.TRS)
							{
								apGL.DrawMirrorMeshPreview(Select.Mesh, MirrorSet, this, VertController);
							}
						}

						//추가 8.24 : 메시 자동 생성 기능 미리보기
						if (_meshEditMode == MESH_EDIT_MODE.MakeMesh
							&& _meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.AutoGenerate)
						{
							//자동 생성 기능
							if (MeshGenerator.IsScanned)
							{
								//스캔된 상태
								apGL.DrawMeshAutoGenerationPreview(_selection.Mesh,
															apMatrix3x3.identity,
															this, MeshGenerator);
							}
						}

						if (_meshEditMode == MESH_EDIT_MODE.PivotEdit)
						{
							//가운데 십자선
							apGL.DrawBoldLine(new Vector2(0, -10), new Vector2(0, 10), 5, new Color(0.3f, 0.7f, 1.0f, 0.7f), true);
							apGL.DrawBoldLine(new Vector2(-10, 0), new Vector2(10, 0), 5, new Color(0.3f, 0.7f, 1.0f, 0.7f), true);
						}

						////추가 9.7 : 미러 모드의 축을 출력하자
						//if(_meshEditMode == MESH_EDIT_MODE.MakeMesh && _meshEditMirrorMode == MESH_EDIT_MIRROR_MODE.Mirror)
						//{
						//	apGL.DrawMeshMirror(_selection.Mesh);
						//}


						if (_meshEditMode == MESH_EDIT_MODE.MakeMesh)
						{
							if (isEdgeExpectRenderSnapToEdge && VertController.IsTmpSnapToEdge)
							{
								apGL.DrawMeshWorkEdgeSnap(_selection.Mesh, VertController);
							}
							if (isEdgeExpectRenderSnapToVertex && VertController.LinkedNextVertex != null)
							{
								apGL.DrawMeshWorkSnapNextVertex(_selection.Mesh, VertController);
							}
							if (isEdgeExpectRender && VertController.IsEdgeWireRenderable())
							{
								//마우스 위치에 맞게 Connect를 예측할 수 있게 만들자

								apGL.DrawMeshWorkEdgeWire(_selection.Mesh, apMatrix3x3.identity, VertController, VertController.IsEdgeWireCross(), VertController.IsEdgeWireMultipleCross());

								if (_meshEditMirrorMode == MESH_EDIT_MIRROR_MODE.Mirror
								&& (_meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.VertexAndEdge
									|| _meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.VertexOnly
									|| _meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.EdgeOnly)
									)
								{
									//여기서 한번 더 -> 반전되서 처리 가능한 Mesh Edge Work를 계산한다.
									MirrorSet.RefreshMeshWork(Select.Mesh, VertController);
									//apGL.DrawMeshWorkMirrorEdgeWire(_selection.Mesh, MirrorSet, Mouse.IsPressed(apMouseSet.Button.LeftNoBound, apMouseSet.ACTION.MeshEdit_Make));
									apGL.DrawMeshWorkMirrorEdgeWire(_selection.Mesh, MirrorSet);
								}
							}

						}

					}
					break;

				case apSelection.SELECTION_TYPE.MeshGroup:
					{
						apGL.RENDER_TYPE selectRenderType = apGL.RENDER_TYPE.Default;
						apGL.RENDER_TYPE meshRenderType = apGL.RENDER_TYPE.Default;

						switch (_meshGroupEditMode)
						{
							case MESHGROUP_EDIT_MODE.Setting:
								//Controller.GUI_Input_MeshGroup_Setting();
								selectRenderType |= apGL.RENDER_TYPE.Outlines;

								if (Select.IsMeshGroupSettingChangePivot)
								{
									selectRenderType |= apGL.RENDER_TYPE.TransformBorderLine;
								}

								break;
							case MESHGROUP_EDIT_MODE.Bone:
								//Controller.GUI_Input_MeshGroup_Bone();
								selectRenderType |= apGL.RENDER_TYPE.Vertex;
								selectRenderType |= apGL.RENDER_TYPE.AllEdges;
								break;
							case MESHGROUP_EDIT_MODE.Modifier:
								{
									apModifierBase.MODIFIER_TYPE modifierType = apModifierBase.MODIFIER_TYPE.Base;
									if (Select.Modifier != null)
									{
										modifierType = Select.Modifier.ModifierType;
										switch (Select.Modifier.ModifierType)
										{
											case apModifierBase.MODIFIER_TYPE.Volume:
												selectRenderType |= apGL.RENDER_TYPE.Vertex;
												selectRenderType |= apGL.RENDER_TYPE.Outlines;
												break;

											case apModifierBase.MODIFIER_TYPE.Morph:
												selectRenderType |= apGL.RENDER_TYPE.Vertex;
												selectRenderType |= apGL.RENDER_TYPE.AllEdges;
												break;

											case apModifierBase.MODIFIER_TYPE.AnimatedMorph:
												selectRenderType |= apGL.RENDER_TYPE.Vertex;
												selectRenderType |= apGL.RENDER_TYPE.AllEdges;
												break;

											case apModifierBase.MODIFIER_TYPE.Rigging:
												selectRenderType |= apGL.RENDER_TYPE.Vertex;
												selectRenderType |= apGL.RENDER_TYPE.AllEdges;
												selectRenderType |= apGL.RENDER_TYPE.BoneRigWeightColor;

												meshRenderType |= apGL.RENDER_TYPE.BoneRigWeightColor;
												break;

											case apModifierBase.MODIFIER_TYPE.Physic:
												selectRenderType |= apGL.RENDER_TYPE.Vertex;
												selectRenderType |= apGL.RENDER_TYPE.PhysicsWeightColor;
												selectRenderType |= apGL.RENDER_TYPE.AllEdges;

												meshRenderType |= apGL.RENDER_TYPE.PhysicsWeightColor;
												break;

											case apModifierBase.MODIFIER_TYPE.TF:
												selectRenderType |= apGL.RENDER_TYPE.Vertex;
												selectRenderType |= apGL.RENDER_TYPE.AllEdges;
												selectRenderType |= apGL.RENDER_TYPE.TransformBorderLine;
												break;

											case apModifierBase.MODIFIER_TYPE.AnimatedTF:
												selectRenderType |= apGL.RENDER_TYPE.Vertex;
												selectRenderType |= apGL.RENDER_TYPE.AllEdges;
												selectRenderType |= apGL.RENDER_TYPE.TransformBorderLine;
												break;

											case apModifierBase.MODIFIER_TYPE.FFD:
												selectRenderType |= apGL.RENDER_TYPE.Vertex;
												selectRenderType |= apGL.RENDER_TYPE.AllEdges;
												break;

											case apModifierBase.MODIFIER_TYPE.AnimatedFFD:
												selectRenderType |= apGL.RENDER_TYPE.Vertex;
												selectRenderType |= apGL.RENDER_TYPE.AllEdges;
												break;



											case apModifierBase.MODIFIER_TYPE.Base:
												selectRenderType |= apGL.RENDER_TYPE.Outlines;
												break;
										}
									}
									else
									{
										selectRenderType |= apGL.RENDER_TYPE.Outlines;
									}
									//Controller.GUI_Input_MeshGroup_Modifier(modifierType);

								}
								break;
						}

						if (Select.MeshGroup != null)
						{
							_tmpSelectedMeshTransform.Clear();
							_tmpSelectedMeshGroupTransform.Clear();

							if (Select.SubMeshInGroup != null)
							{
								_tmpSelectedMeshTransform.Add(Select.SubMeshInGroup);
							}

							if (Select.SubMeshGroupInGroup != null)
							{
								_tmpSelectedMeshGroupTransform.Add(Select.SubMeshGroupInGroup);
							}


							//Onion - Behind인 경우
							//if(Onion.IsVisible && Onion.IsRecorded && Event.current.type == EventType.Repaint && _onionOption_IsRenderBehind)
							//{
							//	Select.MeshGroup.SetBoneIKEnabled(isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging);
							//	//Select.MeshGroup.UpdateRenderUnits(0.0f, true);

							//	RenderOnion(Select.MeshGroup,
							//		isRender_BoneIK);

							//	//다시 업데이트
							//	Select.MeshGroup.SetBoneIKEnabled(false, false);

							//	Select.MeshGroup.UpdateRenderUnits(0.0f, true);
							//}
							RenderOnion(Select.MeshGroup,
								null,
								true,
								Event.current.type == EventType.Repaint,
								isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging, isRender_BoneIK, true,
								_tmpSelectedMeshTransform, _tmpSelectedMeshGroupTransform);


							//기본 MeshGroup 렌더링
							RenderMeshGroup(Select.MeshGroup,
								meshRenderType,
								selectRenderType,
								_tmpSelectedMeshTransform,
								_tmpSelectedMeshGroupTransform,
								false,
								_boneGUIRenderMode,
								_meshGUIRenderMode,
								isRender_BoneIK,
								BONE_RENDER_TARGET.Default);


							//Onion - Top인 경우
							//if(Onion.IsVisible && Onion.IsRecorded && Event.current.type == EventType.Repaint && !_onionOption_IsRenderBehind)
							//{
							//	Select.MeshGroup.SetBoneIKEnabled(isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging);
							//	//Select.MeshGroup.UpdateRenderUnits(0.0f, true);

							//	RenderOnion(Select.MeshGroup,
							//		isRender_BoneIK);

							//	//다시 업데이트
							//	Select.MeshGroup.SetBoneIKEnabled(false, false);

							//	Select.MeshGroup.UpdateRenderUnits(0.0f, true);
							//}
							RenderOnion(Select.MeshGroup,
								null,
								false,
								Event.current.type == EventType.Repaint,
								isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging, isRender_BoneIK, true,
								_tmpSelectedMeshTransform, _tmpSelectedMeshGroupTransform);


							//추가:3.22
							//ExMod 편집중인 경우
							//Bone Preview를 할 수도 있다.
							if (Select.ExEditingMode != apSelection.EX_EDIT.None)
							{
								if (Select.Modifier != null)
								{
									if (Select.Modifier.ModifierType != apModifierBase.MODIFIER_TYPE.Rigging)
									{
										//리깅은 안된다.
										bool isBonePreivew = GetModLockOption_BonePreview(Select.ExEditingMode);
										if (isBonePreivew)
										{
											//Bone Preview를 하자
											RenderExEditBonePreview_Modifier(Select.MeshGroup, GetModLockOption_BonePreviewColor());
										}
									}
								}
							}



							//Bone Edit 중이라면
							if (Controller.IsBoneEditGhostBoneDraw)
							{
								apGL.DrawBoldLine(Controller.BoneEditGhostBonePosW_Start,
													Controller.BoneEditGhostBonePosW_End,
													7, new Color(1.0f, 0.0f, 0.5f, 0.8f), true
													);
							}
						}

						if (_meshGroupEditMode == MESHGROUP_EDIT_MODE.Modifier)
						{
							if (GetModLockOption_ListUI(Select.ExEditingMode))
							{
								//추가 3.22
								//현재 상태에 대해서 ListUI를 출력하자
								DrawModifierListUI(0, height / 2, Select.MeshGroup, Select.Modifier, null, null, Select.ExEditingMode);
							}
						}
					}

					if (Select.ExEditingMode != apSelection.EX_EDIT.None)
					{
						//Profiler.BeginSample("MeshGroup Borderline");

						//Editing이라면
						//화면 위/아래에 붉은 라인들 그려주자
						apGL.DrawEditingBorderline();

						//Profiler.EndSample();
					}

					if (Gizmos.IsBrushMode)
					{
						Controller.GUI_PrintBrushCursor(Gizmos);
					}
					break;

				case apSelection.SELECTION_TYPE.Animation:
					{
						if (Select.AnimClip != null && Select.AnimClip._targetMeshGroup != null)
						{
							apGL.RENDER_TYPE renderType = apGL.RENDER_TYPE.Default;


							bool isVertexRender = false;
							if (Select.AnimTimeline != null)
							{
								if (Select.AnimTimeline._linkType == apAnimClip.LINK_TYPE.AnimatedModifier &&
									Select.AnimTimeline._linkedModifier != null)
								{
									if ((int)(Select.AnimTimeline._linkedModifier.CalculatedValueType & apCalculatedResultParam.CALCULATED_VALUE_TYPE.VertexPos) != 0)
									{
										//VertexPos 제어하는 모디파이어와 연결시
										//Vertex를 보여주자
										isVertexRender = true;

									}
								}
							}
							if (isVertexRender)
							{
								renderType |= apGL.RENDER_TYPE.Vertex;
								renderType |= apGL.RENDER_TYPE.AllEdges;
							}
							else
							{
								renderType |= apGL.RENDER_TYPE.Outlines;
							}


							_tmpSelectedMeshTransform.Clear();
							_tmpSelectedMeshGroupTransform.Clear();

							//TODO : 수정중인 Timeline의 종류에 따라 다르게 표시하자
							//TODO : 작업 중일때.. + 현재 Timeline의 종류에 따라..
							if (Select.SubMeshTransformOnAnimClip != null)
							{
								_tmpSelectedMeshTransform.Add(Select.SubMeshTransformOnAnimClip);
							}

							if (Select.SubMeshGroupTransformOnAnimClip != null)
							{
								_tmpSelectedMeshGroupTransform.Add(Select.SubMeshGroupTransformOnAnimClip);
							}
							// Onion - Behind 경우
							// Onion Render 처리를 한다. + 재생중이 아닐때
							//if(Onion.IsVisible && Onion.IsRecorded && !Select.AnimClip.IsPlaying_Editor && Event.current.type == EventType.Repaint && _onionOption_IsRenderBehind)
							//{
							//	//if (Select.AnimClip._targetMeshGroup != null)
							//	//{
							//	//	Select.AnimClip._targetMeshGroup.SetBoneIKEnabled(isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging);
							//	//}
							//	//Select.AnimClip.Update_Editor(0.0f, true, isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging);
							//	RenderOnion(Select.AnimClip._targetMeshGroup, isRender_BoneIK);

							//	Select.AnimClip.Update_Editor(0.0f, true, isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging);
							//}
							if (_onionOption_IsRenderAnimFrames)
							{
								RenderAnimatedOnion(Select.AnimClip, true,
														Event.current.type == EventType.Repaint,
														isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging, isRender_BoneIK,
														_tmpSelectedMeshTransform, _tmpSelectedMeshGroupTransform);
							}
							else
							{
								RenderOnion(Select.AnimClip._targetMeshGroup,
														Select.AnimClip,
														true,
														Event.current.type == EventType.Repaint,
														isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging, isRender_BoneIK, true,
														_tmpSelectedMeshTransform, _tmpSelectedMeshGroupTransform);
							}


							// 애니메이션-메시그룹 렌더링
							RenderMeshGroup(Select.AnimClip._targetMeshGroup,
												apGL.RENDER_TYPE.Default,
												renderType,
												_tmpSelectedMeshTransform,
												_tmpSelectedMeshGroupTransform,
												false,
												_boneGUIRenderMode,
												_meshGUIRenderMode,
												isRender_BoneIK,
												BONE_RENDER_TARGET.Default);

							// Onion - Top인 경우
							// Onion Render 처리를 한다. + 재생중이 아닐때

							if (_onionOption_IsRenderAnimFrames)
							{
								RenderAnimatedOnion(Select.AnimClip, false,
														Event.current.type == EventType.Repaint,
														isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging, isRender_BoneIK,
														_tmpSelectedMeshTransform, _tmpSelectedMeshGroupTransform);
							}
							else
							{
								RenderOnion(Select.AnimClip._targetMeshGroup,
												Select.AnimClip,
												false,
												Event.current.type == EventType.Repaint,
												isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging, isRender_BoneIK, true,
												_tmpSelectedMeshTransform, _tmpSelectedMeshGroupTransform);
							}



							//추가:3.22
							//ExMod 편집중인 경우
							//Bone Preview를 할 수도 있다.
							if (Select.ExAnimEditingMode != apSelection.EX_EDIT.None)
							{
								if (Select.AnimClip != null && Select.AnimClip._targetMeshGroup != null)
								{
									//리깅은 안된다.
									bool isBonePreivew = GetModLockOption_BonePreview(Select.ExAnimEditingMode);
									if (isBonePreivew)
									{
										//Bone Preview를 하자
										RenderExEditBonePreview_Animation(Select.AnimClip, Select.AnimClip._targetMeshGroup, GetModLockOption_BonePreviewColor());

										Select.AnimClip.Update_Editor(0.0f, true, isUpdate_BoneIKMatrix, isUpdate_BoneIKRigging);
									}
								}
							}
						}

						if (GetModLockOption_ListUI(Select.ExAnimEditingMode))
						{
							//추가 3.22
							//현재 상태에 대해서 ListUI를 출력하자
							DrawModifierListUI(0, height / 2, Select.AnimClip._targetMeshGroup, null, Select.AnimClip, Select.AnimTimeline, Select.ExAnimEditingMode);
						}

						//화면 위/아래 붉은 라인을 그려주자
						if (Select.ExAnimEditingMode != apSelection.EX_EDIT.None)
						{
							apGL.DrawEditingBorderline();
						}

						if (Gizmos.IsBrushMode)
						{
							Controller.GUI_PrintBrushCursor(Gizmos);
						}


					}
					break;
			}

			if (_isShowCaptureFrame && _portrait != null && Select.SelectionType == apSelection.SELECTION_TYPE.Overall)
			{

				Vector2 framePos_Center = new Vector2(_captureFrame_PosX + apGL.WindowSizeHalf.x, _captureFrame_PosY + apGL.WindowSizeHalf.y);
				Vector2 frameHalfSize = new Vector2(_captureFrame_SrcWidth / 2, _captureFrame_SrcHeight / 2);
				Vector2 framePos_LT = framePos_Center + new Vector2(-frameHalfSize.x, -frameHalfSize.y);
				Vector2 framePos_RT = framePos_Center + new Vector2(frameHalfSize.x, -frameHalfSize.y);
				Vector2 framePos_LB = framePos_Center + new Vector2(-frameHalfSize.x, frameHalfSize.y);
				Vector2 framePos_RB = framePos_Center + new Vector2(frameHalfSize.x, frameHalfSize.y);
				Color frameColor = new Color(0.3f, 1.0f, 1.0f, 0.7f);

				apGL.BeginBatch_ColoredLine();
				apGL.DrawLineGL(framePos_LT, framePos_RT, frameColor, false);
				apGL.DrawLineGL(framePos_RT, framePos_RB, frameColor, false);
				apGL.DrawLineGL(framePos_RB, framePos_LB, frameColor, false);
				apGL.DrawLineGL(framePos_LB, framePos_LT, frameColor, false);

				//<<< 추가 >>>
				//추가 : 썸네일 프레임을 만들자
				float preferAspectRatio = 2.0f; //256 x 128
				float srcAspectRatio = (float)_captureFrame_SrcWidth / (float)_captureFrame_SrcHeight;

				//긴쪽으로 캡쳐 크기를 맞춘다.
				int srcThumbWidth = _captureFrame_SrcWidth;
				int srcThumbHeight = _captureFrame_SrcHeight;

				//AspectRatio = W / H
				if (srcAspectRatio < preferAspectRatio)
				{
					srcThumbHeight = (int)((srcThumbWidth / preferAspectRatio) + 0.5f);
				}
				else
				{
					srcThumbWidth = (int)((srcThumbHeight * preferAspectRatio) + 0.5f);
				}
				srcThumbWidth /= 2;
				srcThumbHeight /= 2;



				Vector2 thumbFramePos_LT = framePos_Center + new Vector2(-srcThumbWidth, -srcThumbHeight);
				Vector2 thumbFramePos_RT = framePos_Center + new Vector2(srcThumbWidth, -srcThumbHeight);
				Vector2 thumbFramePos_LB = framePos_Center + new Vector2(-srcThumbWidth, srcThumbHeight);
				Vector2 thumbFramePos_RB = framePos_Center + new Vector2(srcThumbWidth, srcThumbHeight);

				Color thumbFrameColor = new Color(1.0f, 1.0f, 0.0f, 0.4f);
				apGL.DrawLineGL(thumbFramePos_LT, thumbFramePos_RT, thumbFrameColor, false);
				apGL.DrawLineGL(thumbFramePos_RT, thumbFramePos_RB, thumbFrameColor, false);
				apGL.DrawLineGL(thumbFramePos_RB, thumbFramePos_LB, thumbFrameColor, false);
				apGL.DrawLineGL(thumbFramePos_LB, thumbFramePos_LT, thumbFrameColor, false);
				apGL.EndBatch();
			}


			int guiLabelY = 20;
			if (_guiOption_isFPSVisible)
			{
				//이전 : 최적화 코드 아님
				//apGL.DrawTextGL("FPS " + FPS, new Vector2(10, guiLabelY), 150, Color.yellow);
				//변경 19.11.23 : 텍스트 최적화
				if(_fpsString == null)
				{
					_fpsString = new apStringWrapper(16);
					_fpsString.SavePresetText("FPS ");
					//_fpsString.SavePresetText(" / Low ");
					//_fpsString.SavePresetText(" / High ");
				}
				_fpsString.Clear();
				_fpsString.AppendPreset(0, false);
				//_fpsString.Append(FPS, true);
				_fpsString.Append(_fpsCounter.AvgFPS, true);
				//FPS 확인용
				//_fpsString.AppendPreset(1, false);
				//_fpsString.Append(_fpsCounter.LowFPS, true);
				//_fpsString.AppendPreset(2, false);
				//_fpsString.Append(_fpsCounter.HighFPS, true);

				apGL.DrawTextGL(_fpsString.ToString(), new Vector2(10, guiLabelY), 150, Color.yellow);

				guiLabelY += 15;
			}

			if (_isNotification_GUI)
			{
				Color notiColor = Color.yellow;
				if (_tNotification_GUI < 1.0f)
				{
					notiColor.a = _tNotification_GUI;
				}
				apGL.DrawTextGL(_strNotification_GUI, new Vector2(10, guiLabelY), 400, notiColor);
				guiLabelY += 15;
			}



			if (Backup.IsAutoSaveWorking() && _isBackupProcessing)
			{
				//자동 저장 중일때
				int iconSize = 28;
				float scaledIconSize = (float)iconSize / apGL.Zoom;

				//int yPos = 35 + 8 + iconSize / 2;
				//if(_isNotification_GUI)
				//{
				//	yPos += 15;
				//}

				int yPos = guiLabelY + 8 + iconSize / 2;
				guiLabelY += iconSize + 4;


				Color labelColor = Color.yellow;
				float alpha = Mathf.Sin((_tBackupProcessing_Label / BACKUP_LABEL_TIME_LENGTH) * Mathf.PI * 2.0f);
				//-1 ~ 1
				//-0.2 ~ 0.2 (x0.2)
				//0.6 ~ 1(+0.8)
				alpha = (alpha * 0.2f) + 0.8f;
				labelColor.a = alpha;


				if (_imgBackupIcon_Frame1 == null || _imgBackupIcon_Frame2 == null)
				{
					_imgBackupIcon_Frame1 = ImageSet.Get(apImageSet.PRESET.AutoSave_Frame1);
					_imgBackupIcon_Frame2 = ImageSet.Get(apImageSet.PRESET.AutoSave_Frame2);
				}


				if (_tBackupProcessing_Icon < BACKUP_ICON_TIME_LENGTH * 0.5f)
				{
					apGL.DrawTextureGL(_imgBackupIcon_Frame1, new Vector2(10 + iconSize / 2, yPos), scaledIconSize, scaledIconSize, Color.gray, 0.0f);
				}
				else
				{
					apGL.DrawTextureGL(_imgBackupIcon_Frame2, new Vector2(10 + iconSize / 2, yPos), scaledIconSize, scaledIconSize, Color.gray, 0.0f);
				}
				apGL.DrawTextGL(Backup.Label, new Vector2(10 + iconSize + 10, yPos - 6), 400, labelColor);
			}

			//추가 3.1 : Low CPU모드일때 아이콘 표시
			if (_lowCPUStatus == LOW_CPU_STATUS.LowCPU_Low || _lowCPUStatus == LOW_CPU_STATUS.LowCPU_Mid)
			{
				int iconSize = 28;

				int yPos = guiLabelY + 8 + iconSize / 2;
				guiLabelY += iconSize + 4;

				float scaledIconSize = (float)iconSize / apGL.Zoom;

				if (_imgLowCPUStatus == null)
				{
					_imgLowCPUStatus = ImageSet.Get(apImageSet.PRESET.LowCPU);
				}

				apGL.DrawTextureGL(_imgLowCPUStatus, new Vector2(10 + iconSize / 2, yPos), scaledIconSize, scaledIconSize, Color.gray, 0.0f);
			}


			if (Select.IsSelectionLockGUI)
			{
				float scaledIconSize = (float)28 / apGL.Zoom;
				apGL.DrawTextureGL(ImageSet.Get(apImageSet.PRESET.GUI_SelectionLock), new Vector2(width - 20, 30), scaledIconSize, scaledIconSize, Color.gray, 0.0f);
			}


			if (_guiOption_isStatisticsVisible)
			{
				//통계 정보를 출력한다.
				//통계 정보는 아래서 출력한다.
				//어떤 메뉴인가에 따라 다르다
				Select.CalculateStatistics();
				if (Select.IsStatisticsCalculated)
				{
					int posY = height - 30;
					if (Select.Statistics_NumKeyframe >= 0)
					{
						apGL.DrawTextGL("Keyframes", new Vector2(10, posY), 120, Color.yellow);
						apGL.DrawTextGL(Select.Statistics_NumKeyframe.ToString(), new Vector2(120, posY), 150, Color.yellow);
						posY -= 15;
					}
					if (Select.Statistics_NumTimelineLayer >= 0)
					{
						apGL.DrawTextGL("Timeline Layers", new Vector2(10, posY), 120, Color.yellow);
						apGL.DrawTextGL(Select.Statistics_NumTimelineLayer.ToString(), new Vector2(120, posY), 150, Color.yellow);
						posY -= 15;
					}
					if (Select.Statistics_NumClippedMesh >= 0)
					{
						apGL.DrawTextGL("Clipped Vertices", new Vector2(10, posY), 120, Color.yellow);
						apGL.DrawTextGL(Select.Statistics_NumClippedVertex.ToString(), new Vector2(120, posY), 150, Color.yellow);
						posY -= 15;

						apGL.DrawTextGL("Clipped Meshes", new Vector2(10, posY), 120, Color.yellow);
						apGL.DrawTextGL(Select.Statistics_NumClippedMesh.ToString(), new Vector2(120, posY), 150, Color.yellow);
						posY -= 15;
					}

					apGL.DrawTextGL("Triangles", new Vector2(10, posY), 120, Color.yellow);
					apGL.DrawTextGL(Select.Statistics_NumTri.ToString(), new Vector2(120, posY), 150, Color.yellow);
					posY -= 15;

					apGL.DrawTextGL("Edges", new Vector2(10, posY), 120, Color.yellow);
					apGL.DrawTextGL(Select.Statistics_NumEdge.ToString(), new Vector2(120, posY), 150, Color.yellow);
					posY -= 15;

					apGL.DrawTextGL("Vertices", new Vector2(10, posY), 120, Color.yellow);
					apGL.DrawTextGL(Select.Statistics_NumVertex.ToString(), new Vector2(120, posY), 150, Color.yellow);
					posY -= 15;

					if (Select.Statistics_NumMesh >= 0)
					{
						apGL.DrawTextGL("Meshes", new Vector2(10, posY), 120, Color.yellow);
						apGL.DrawTextGL(Select.Statistics_NumMesh.ToString(), new Vector2(120, posY), 150, Color.yellow);
						posY -= 15;
					}

					posY -= 5;
					apGL.DrawTextGL("Statistics", new Vector2(10, posY), 120, Color.yellow);
				}
			}

			//화면 캡쳐 이벤트
#if UNITY_EDITOR_OSX
			if(_isScreenCaptureRequest_OSXReady)
			{
				_screenCaptureRequest_Count--;
				if(_screenCaptureRequest_Count < 0)
				{
					_isScreenCaptureRequest = true;
					_isScreenCaptureRequest_OSXReady = false;
					_screenCaptureRequest_Count = 0;
				}
			}
#endif
			if (_isScreenCaptureRequest)
			{
				_isScreenCaptureRequest = false;
#if UNITY_EDITOR_OSX
				_isScreenCaptureRequest_OSXReady = false;
				_screenCaptureRequest_Count = 0;
#endif

				ProcessScreenCapture();
				SetRepaint();
			}

			////물리 설정이 변경되었다면 복구
			//if (_portrait != null)
			//{
			//	_portrait._isPhysicsPlay_Editor = isPrevPhysicsPlay_Editor;
			//}
		}

		private List<apTransform_Mesh> _tmpSelectedMeshTransform = new List<apTransform_Mesh>();
		private List<apTransform_MeshGroup> _tmpSelectedMeshGroupTransform = new List<apTransform_MeshGroup>();
		private List<apRenderUnit> _tmpSelectedRenderUnits = new List<apRenderUnit>();

		/// <summary>
		/// MeshGroup을 렌더링한다.
		/// </summary>
		/// <param name="meshGroup"></param>
		/// <param name="selectedRenderType"></param>
		/// <param name="selectedMeshes"></param>
		/// <param name="selectedMeshGroups"></param>
		/// <param name="isRenderOnlyVisible">단순 재생이면 True, 작업 중이면 False (Alpha가 0인 것도 일단 렌더링을 한다)</param>
		private void RenderMeshGroup(apMeshGroup meshGroup,
										apGL.RENDER_TYPE meshRenderType,
										apGL.RENDER_TYPE selectedRenderType,
										List<apTransform_Mesh> selectedMeshes,
										List<apTransform_MeshGroup> selectedMeshGroups,
										bool isRenderOnlyVisible,
										apEditor.BONE_RENDER_MODE boneRenderMode,
										apEditor.MESH_RENDER_MODE meshRenderMode,
										bool isBoneIKUsing,
										BONE_RENDER_TARGET boneRenderTarget,
										bool isSelectedMeshOnly = false,
										bool isUseBoneToneColor = false)
		{
			//Profiler.BeginSample("MeshGroup Render");

			if (meshRenderMode == MESH_RENDER_MODE.Render)
			{
				_tmpSelectedRenderUnits.Clear();

				List<apRenderUnit> renderUnits = meshGroup.SortedBuffer.SortedRenderUnits;
				int nRenderUnits = renderUnits.Count;

				

				//선택된 렌더유닛을 먼저 선정. 그 후에 다시 렌더링하자
				//for (int iUnit = 0; iUnit < meshGroup._renderUnits_All.Count; iUnit++)//>>이전 코드
				for (int iUnit = 0; iUnit < nRenderUnits; iUnit++)//<<변경
				{
					//apRenderUnit renderUnit = meshGroup._renderUnits_All[iUnit];//>>이전 코드
					apRenderUnit renderUnit = renderUnits[iUnit];//<<변경

					if (renderUnit._unitType == apRenderUnit.UNIT_TYPE.Mesh)
					{
						if (renderUnit._meshTransform != null)
						{
							//if(!renderUnit._meshTransform._isVisible_Default)
							//{
							//	continue;
							//}

							if (selectedMeshes != null && selectedMeshes.Count > 0)
							{
								if (selectedMeshes.Contains(renderUnit._meshTransform))
								{
									//현재 선택중인 메시다.
									_tmpSelectedRenderUnits.Add(renderUnit);
								}
							}

							if (selectedMeshGroups != null && selectedMeshGroups.Count > 0)
							{
								if (selectedMeshGroups.Contains(renderUnit._meshGroupTransform))
								{
									_tmpSelectedRenderUnits.Add(renderUnit);
								}
							}
						}
					}
				}

				// Weight 갱신과 Vert Color 연동
				//----------------------------------
				if ((int)(meshRenderType & apGL.RENDER_TYPE.BoneRigWeightColor) != 0)
				{
					//Rig Weight를 집어넣자.
					//bool isBoneColor = Select._rigEdit_isBoneColorView;
					//apSelection.RIGGING_EDIT_VIEW_MODE rigViewMode = Select._rigEdit_viewMode;
					apRenderVertex renderVert = null;
					apModifiedMesh modMesh = Select.ModMeshOfMod;
					apModifiedVertexRig vertRig = null;
					apModifiedVertexRig.WeightPair weightPair = null;
					apBone selelcedBone = Select.Bone;

					Color colorBlack = Color.black;


					apModifierBase modifier = Select.Modifier;
					if (modifier != null)
					{
						if (modifier._paramSetGroup_controller.Count > 0 &&
							modifier._paramSetGroup_controller[0]._paramSetList.Count > 0)
						{
							List<apModifiedMesh> modMeshes = Select.Modifier._paramSetGroup_controller[0]._paramSetList[0]._meshData;
							for (int iMM = 0; iMM < modMeshes.Count; iMM++)
							{
								modMesh = modMeshes[iMM];
								if (modMesh != null)
								{
									modMesh.RefreshVertexRigs(_portrait);

									//이 렌더 유닛이 선택된 경우에만 RigWeightParam을 계산하자.
									bool isSelectedRenderUnit = _tmpSelectedRenderUnits.Contains(modMesh._renderUnit);

									for (int iRU = 0; iRU < modMesh._renderUnit._renderVerts.Count; iRU++)
									{
										renderVert = modMesh._renderUnit._renderVerts[iRU];
										renderVert._renderColorByTool = colorBlack;
										renderVert._renderWeightByTool = 0.0f;
										renderVert._renderParam = 0;
										renderVert._renderRigWeightParam.Clear();//<<추가 19.7.30
									}

									for (int iVR = 0; iVR < modMesh._vertRigs.Count; iVR++)
									{
										vertRig = modMesh._vertRigs[iVR];
										if (vertRig._renderVertex != null)
										{
											for (int iWP = 0; iWP < vertRig._weightPairs.Count; iWP++)
											{
												weightPair = vertRig._weightPairs[iWP];
												vertRig._renderVertex._renderColorByTool += weightPair._bone._color * weightPair._weight;

												if (weightPair._bone == selelcedBone)
												{
													vertRig._renderVertex._renderWeightByTool += weightPair._weight;
												}

												//선택된 렌더 유닛인 경우 WeightParam에 Rig값을 입력하자.
												if(isSelectedRenderUnit)
												{
													vertRig._renderVertex._renderRigWeightParam.AddRigWeight(weightPair._bone._color, weightPair._bone == selelcedBone, weightPair._weight);
												}
											}
										}
									}

									//선택된 렌더 유닛에 한해서 RigWeight를 계산하자. (19.7.30)
									if (isSelectedRenderUnit)
									{
										for (int iRU = 0; iRU < modMesh._renderUnit._renderVerts.Count; iRU++)
										{
											renderVert = modMesh._renderUnit._renderVerts[iRU];
											renderVert._renderRigWeightParam.Normalize();
										}
									}
								}
							}
						}
					}
				}

				//Physic/Volume Color를 집어넣어보자
				if ((int)(meshRenderType & apGL.RENDER_TYPE.PhysicsWeightColor) != 0 ||
					(int)(meshRenderType & apGL.RENDER_TYPE.VolumeWeightColor) != 0)
				{

					//Rig Weight를 집어넣자.
					//bool isBoneColor = Select._rigEdit_isBoneColorView;
					//apSelection.RIGGING_EDIT_VIEW_MODE rigViewMode = Select._rigEdit_viewMode;
					apRenderVertex renderVert = null;
					apModifiedMesh modMesh = Select.ModMeshOfMod;
					apModifiedVertexWeight vertWeight = null;

					Color colorBlack = Color.black;

					apModifierBase modifier = Select.Modifier;

					bool isPhysic = (int)(modifier.ModifiedValueType & apModifiedMesh.MOD_VALUE_TYPE.VertexWeightList_Physics) != 0;
					//bool isVolume = (int)(modifier.ModifiedValueType & apModifiedMesh.MOD_VALUE_TYPE.VertexWeightList_Volume) != 0;

					if (modifier != null)
					{
						if (modifier._paramSetGroup_controller.Count > 0 &&
							modifier._paramSetGroup_controller[0]._paramSetList.Count > 0)
						{
							List<apModifiedMesh> modMeshes = Select.Modifier._paramSetGroup_controller[0]._paramSetList[0]._meshData;
							for (int iMM = 0; iMM < modMeshes.Count; iMM++)
							{
								modMesh = modMeshes[iMM];
								if (modMesh != null)
								{
									//Refresh를 여기서 하진 말자
									//modMesh.RefreshVertexWeights(_portrait, isPhysic, isVolume);

									for (int iRU = 0; iRU < modMesh._renderUnit._renderVerts.Count; iRU++)
									{
										renderVert = modMesh._renderUnit._renderVerts[iRU];
										renderVert._renderColorByTool = colorBlack;
										renderVert._renderWeightByTool = 0.0f;
										renderVert._renderParam = 0;
									}

									for (int iVR = 0; iVR < modMesh._vertWeights.Count; iVR++)
									{
										vertWeight = modMesh._vertWeights[iVR];
										if (vertWeight._renderVertex != null)
										{
											//그라데이션을 위한 Weight 값을 넣어주자
											vertWeight._renderVertex._renderWeightByTool = vertWeight._weight;

											if (isPhysic)
											{
												if (vertWeight._isEnabled && vertWeight._physicParam._isMain)
												{
													vertWeight._renderVertex._renderParam = 1;//1 : Main
												}
												else if (!vertWeight._isEnabled && vertWeight._physicParam._isConstraint)
												{
													vertWeight._renderVertex._renderParam = 2;//2 : Constraint
												}
											}
										}
									}
								}
							}
						}
					}
				}


				//----------------------------------

				if (!isSelectedMeshOnly)
				{
					//for (int iUnit = 0; iUnit < meshGroup._renderUnits_All.Count; iUnit++)//>>이전 코드
					for (int iUnit = 0; iUnit < nRenderUnits; iUnit++)//<<변경
					{
						//apRenderUnit renderUnit = meshGroup._renderUnits_All[iUnit];//>>이전 코드
						apRenderUnit renderUnit = renderUnits[iUnit];//<<변경


						if (renderUnit._unitType == apRenderUnit.UNIT_TYPE.Mesh)
						{
							if (renderUnit._meshTransform != null)
							{

								if (renderUnit._meshTransform._isClipping_Parent)
								{
									//Profiler.BeginSample("Render - Mask Unit");

									if (!isRenderOnlyVisible || renderUnit._isVisible)
									{

										apGL.DrawRenderUnit_ClippingParent_Renew(renderUnit,
																					meshRenderType,
																					renderUnit._meshTransform._clipChildMeshes,
																					//renderUnit._meshTransform._clipChildMeshTransforms,
																					//renderUnit._meshTransform._clipChildRenderUnits,
																					VertController,
																					this,
																					Select);
									}

									//Profiler.EndSample();
								}
								else if (renderUnit._meshTransform._isClipping_Child)
								{
									//렌더링은 생략한다.
								}
								else
								{
									//Profiler.BeginSample("Render - Normal Unit");

									if (!isRenderOnlyVisible || renderUnit._isVisible)
									{
										//if(_tmpSelectedRenderUnits.Contains(renderUnit))
										apGL.DrawRenderUnit(renderUnit, meshRenderType, VertController, Select, this, _mouseSet.Pos);


									}

									//Profiler.EndSample();
								}

							}
						}
					}
				}
			}


			//Bone을 렌더링하자
			if (boneRenderMode != BONE_RENDER_MODE.None)
			{
				//선택 아웃라인을 밑에 그릴때
				//if(Select.Bone != null)
				//{
				//	apGL.DrawSelectedBone(Select.Bone);
				//}

				bool isDrawBoneOutline = (boneRenderMode == BONE_RENDER_MODE.RenderOutline);
				//Child MeshGroup의 Bone을 먼저 렌더링합니다. (그래야 렌더링시 뒤로 들어감)
				if ((int)(boneRenderTarget & BONE_RENDER_TARGET.AllBones) != 0)
				{
					//>> Bone Render Target이 AllBones인 경우
					//<BONE_EDIT> : Root / Sub 따로
					//if (meshGroup._childMeshGroupTransformsWithBones.Count > 0)
					//{
					//	for (int iChildMG = 0; iChildMG < meshGroup._childMeshGroupTransformsWithBones.Count; iChildMG++)
					//	{
					//		apTransform_MeshGroup meshGroupTransform = meshGroup._childMeshGroupTransformsWithBones[iChildMG];

					//		for (int iRoot = 0; iRoot < meshGroupTransform._meshGroup._boneList_Root.Count; iRoot++)
					//		{
					//			DrawBoneRecursive(meshGroupTransform._meshGroup._boneList_Root[iRoot], isDrawBoneOutline, isBoneIKUsing, isUseBoneToneColor);
					//		}
					//	}
					//}

					//변경 : Bone Set를 이용한다.
					if (meshGroup._boneListSets != null && meshGroup._boneListSets.Count > 0)
					{
						bool isSubBoneSelectable = true;
						if (Select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup && _meshGroupEditMode == MESHGROUP_EDIT_MODE.Bone)
						{
							isSubBoneSelectable = false;
						}
						apMeshGroup.BoneListSet boneSet = null;
						for (int iSet = 0; iSet < meshGroup._boneListSets.Count; iSet++)
						{
							boneSet = meshGroup._boneListSets[iSet];
							if (boneSet._isRootMeshGroup)
							{
								//Root MeshGroup의 Bone이면 패스
								continue;
							}
							for (int iRoot = 0; iRoot < boneSet._bones_Root.Count; iRoot++)
							{
								DrawBoneRecursive(boneSet._bones_Root[iRoot], isDrawBoneOutline, isBoneIKUsing, isUseBoneToneColor, isSubBoneSelectable);
							}
						}
					}


					//Bone도 렌더링 합니당
					if (meshGroup._boneList_Root.Count > 0)
					{
						for (int iRoot = 0; iRoot < meshGroup._boneList_Root.Count; iRoot++)
						{
							//Root 렌더링을 한다.
							DrawBoneRecursive(meshGroup._boneList_Root[iRoot], isDrawBoneOutline, isBoneIKUsing, isUseBoneToneColor, true);
						}
					}
				}
				else if ((int)(boneRenderTarget & BONE_RENDER_TARGET.SelectedOnly) != 0)
				{
					//>> 아니면 선택한 Bone만 렌더링 한다.
					if (Select.Bone != null)
					{
						Select.Bone.GUIUpdate(false, isBoneIKUsing);
						if (Select.Bone.IsGUIVisible)
						{
							apGL.DrawBone(Select.Bone, isDrawBoneOutline, isBoneIKUsing, isUseBoneToneColor, true);
						}
					}
				}

				if ((int)(boneRenderTarget & BONE_RENDER_TARGET.SelectedOutline) != 0)
				{
					if (Select.Bone != null)
					{
						//선택 아웃라인을 위에 그릴때
						if (Select.BoneEditMode == apSelection.BONE_EDIT_MODE.Link)
						{
							if (Controller.BoneEditRollOverBone != null)
							{
								apGL.DrawSelectedBone(Controller.BoneEditRollOverBone, false, isBoneIKUsing);
							}
						}

						apGL.DrawSelectedBone(Select.Bone, true, isBoneIKUsing);
						if (!isDrawBoneOutline)
						{
							apGL.DrawSelectedBonePost(Select.Bone, isBoneIKUsing);
						}
					}
				}

			}

			if (meshRenderMode == MESH_RENDER_MODE.Render)
			{
				//선택된 Render Unit을 그려준다. (Vertex 등)
				if (_tmpSelectedRenderUnits.Count > 0)
				{
					//Profiler.BeginSample("Render - Selected Unit");
					for (int i = 0; i < _tmpSelectedRenderUnits.Count; i++)
					{
						apGL.DrawRenderUnit(_tmpSelectedRenderUnits[i], selectedRenderType, VertController, Select, this, _mouseSet.Pos);
					}
					//Profiler.EndSample();
				}
			}
			//Profiler.EndSample();
		}




		/// <summary>
		/// MeshGroup을 렌더링한다.
		/// </summary>
		/// <param name="meshGroup"></param>
		/// <param name="selectedRenderType"></param>
		/// <param name="selectedMeshes"></param>
		/// <param name="selectedMeshGroups"></param>
		/// <param name="isRenderOnlyVisible">단순 재생이면 True, 작업 중이면 False (Alpha가 0인 것도 일단 렌더링을 한다)</param>
		private void RenderBoneOutlineOnly(apMeshGroup meshGroup,
										Color boneLineColor,
										bool isBoneIKUsing)
		{

			//Bone을 렌더링하자
			//선택 아웃라인을 밑에 그릴때
			//if(Select.Bone != null)
			//{
			//	apGL.DrawSelectedBone(Select.Bone);
			//}

			//<BONE_EDIT> : Root / Sub 따로
			////Child MeshGroup의 Bone을 먼저 렌더링합니다. (그래야 렌더링시 뒤로 들어감)
			//if (meshGroup._childMeshGroupTransformsWithBones.Count > 0)
			//{
			//	for (int iChildMG = 0; iChildMG < meshGroup._childMeshGroupTransformsWithBones.Count; iChildMG++)
			//	{
			//		apTransform_MeshGroup meshGroupTransform = meshGroup._childMeshGroupTransformsWithBones[iChildMG];

			//		for (int iRoot = 0; iRoot < meshGroupTransform._meshGroup._boneList_Root.Count; iRoot++)
			//		{
			//			DrawBoneOutlineRecursive(meshGroupTransform._meshGroup._boneList_Root[iRoot], boneLineColor, isBoneIKUsing);
			//		}
			//	}
			//}


			//변경 : Bone Set를 이용한다.
			if (meshGroup._boneListSets != null && meshGroup._boneListSets.Count > 0)
			{
				apMeshGroup.BoneListSet boneSet = null;
				for (int iSet = 0; iSet < meshGroup._boneListSets.Count; iSet++)
				{
					boneSet = meshGroup._boneListSets[iSet];
					if (boneSet._isRootMeshGroup)
					{
						//Root MeshGroup의 Bone이면 패스
						continue;
					}
					for (int iRoot = 0; iRoot < boneSet._bones_Root.Count; iRoot++)
					{
						DrawBoneOutlineRecursive(boneSet._bones_Root[iRoot], boneLineColor, isBoneIKUsing);
					}
				}
			}


			//Bone도 렌더링 합니당
			if (meshGroup._boneList_Root.Count > 0)
			{
				for (int iRoot = 0; iRoot < meshGroup._boneList_Root.Count; iRoot++)
				{
					//Root 렌더링을 한다.
					DrawBoneOutlineRecursive(meshGroup._boneList_Root[iRoot], boneLineColor, isBoneIKUsing);
				}
			}


		}

		private void DrawBoneRecursive(apBone targetBone, bool isDrawOutline, bool isBoneIKUsing, bool isUseBoneToneColor, bool isBoneAvailable)
		{
			targetBone.GUIUpdate(false, isBoneIKUsing);
			//apGL.DrawBone(targetBone, _selection);

			if (targetBone.IsGUIVisible)
			{
				apGL.DrawBone(targetBone, isDrawOutline, isBoneIKUsing, isUseBoneToneColor, isBoneAvailable);
			}

			for (int i = 0; i < targetBone._childBones.Count; i++)
			{
				DrawBoneRecursive(targetBone._childBones[i], isDrawOutline, isBoneIKUsing, isUseBoneToneColor, isBoneAvailable);
			}
		}


		private void DrawBoneOutlineRecursive(apBone targetBone, Color boneOutlineColor, bool isBoneIKUsing)
		{
			targetBone.GUIUpdate(false, isBoneIKUsing);

			if (targetBone.IsGUIVisible)
			{
				apGL.DrawBoneOutline(targetBone, boneOutlineColor, isBoneIKUsing);
			}

			for (int i = 0; i < targetBone._childBones.Count; i++)
			{
				DrawBoneOutlineRecursive(targetBone._childBones[i], boneOutlineColor, isBoneIKUsing);
			}
		}





		/// <summary>
		/// Onion을 렌더링한다.
		/// 조건문이 모두 포함되있어서 함수만 호출하면 처리 조건에 따라 자동으로 렌더링을 한다.
		/// 편집 중일 때에만 렌더링을 한다.
		/// Top/Behind 옵션이 있으므로 RenderMeshGroup의 전, 후에 모두 호출해야한다.
		/// </summary>
		/// <param name="meshGroup"></param>
		/// <param name="animClip"></param>
		/// <param name="isBoneIKMatrix"></param>
		/// <param name="isBoneIKRigging"></param>
		/// <param name="isBoneIKUsing"></param>
		private void RenderOnion(apMeshGroup meshGroup,
									apAnimClip animClip,
									bool isBehindRendering, bool isRepaintType,
									bool isBoneIKMatrix, bool isBoneIKRigging, bool isBoneIKUsing,
									bool isRecoverUpdate,
									List<apTransform_Mesh> selectedMeshTransforms,
									List<apTransform_MeshGroup> selectedMeshGroupTransforms,
									bool isUseOnionRecord = true)
		{
			if (meshGroup == null || !isRepaintType || !Onion.IsVisible || (isUseOnionRecord && !Onion.IsRecorded) || _onionOption_IsRenderBehind != isBehindRendering)
			{
				return;
			}

			if (_onionOption_IKCalculateForce)
			{
				isBoneIKUsing = true;
			}

			if (animClip != null)
			{
				if (animClip.IsPlaying_Editor)
				{
					//재생중엔 실행되지 않는다.
					return;
				}
			}

			if (isUseOnionRecord)
			{
				//저장된 값을 적용
				Onion.AdaptRecord(this);
			}

			//렌더링
			bool isPrevPhysics = _portrait._isPhysicsPlay_Editor;
			_portrait._isPhysicsPlay_Editor = false;


			if (animClip != null)
			{

				//animClip.Update_Editor(0.0f, true, isBoneIKMatrix, isBoneIKRigging);
				animClip.Update_Editor(0.0f, true, true, true);
			}
			else
			{
				//meshGroup.SetBoneIKEnabled(isBoneIKMatrix, isBoneIKRigging);
				meshGroup.SetBoneIKEnabled(true, true);
				meshGroup.UpdateRenderUnits(0.0f, true);
				meshGroup.SetBoneIKEnabled(false, false);
			}
			//Debug.Log("Render Onion : isBoneIKMatrix : " + isBoneIKMatrix + " / isBoneIKRigging : " + isBoneIKRigging + " / isBoneIKUsing : " + isBoneIKUsing);

			if (_onionOption_IsRenderOnlySelected)
			{
				//선택된 것만 렌더링

				RenderMeshGroup(meshGroup,
									apGL.RENDER_TYPE.ToneColor, apGL.RENDER_TYPE.ToneColor,
									selectedMeshTransforms, selectedMeshGroupTransforms,
									true,
									(_boneGUIRenderMode != BONE_RENDER_MODE.None ? BONE_RENDER_MODE.RenderOutline : BONE_RENDER_MODE.None),
									_meshGUIRenderMode,
									isBoneIKUsing, BONE_RENDER_TARGET.SelectedOnly, true, true);

			}
			else
			{
				//모두 렌더링
				RenderMeshGroup(meshGroup,
									apGL.RENDER_TYPE.ToneColor, apGL.RENDER_TYPE.Default,
									null, null, true,
									(_boneGUIRenderMode != BONE_RENDER_MODE.None ? BONE_RENDER_MODE.RenderOutline : BONE_RENDER_MODE.None),
									_meshGUIRenderMode,
									isBoneIKUsing,
									BONE_RENDER_TARGET.AllBones, false, true);
			}





			if (isUseOnionRecord)
			{
				//원래의 값으로 복구 
				Onion.Recorver(this);
			}

			if (isRecoverUpdate)
			{
				if (animClip != null)
				{
					animClip.Update_Editor(0.0f, true, isBoneIKMatrix, isBoneIKRigging);
				}
				else
				{
					meshGroup.SetBoneIKEnabled(isBoneIKMatrix, isBoneIKRigging);
					meshGroup.UpdateRenderUnits(0.0f, true);
					meshGroup.SetBoneIKEnabled(false, false);
				}

				//<BONE_EDIT> : 이전 코드
				//for (int i = 0; i < meshGroup._boneList_Root.Count; i++)
				//{
				//	meshGroup._boneList_Root[i].GUIUpdate(true, isBoneIKUsing);
				//}

				//if (meshGroup._childMeshGroupTransformsWithBones != null)
				//{
				//	for (int iChild = 0; iChild < meshGroup._childMeshGroupTransformsWithBones.Count; iChild++)
				//	{
				//		apMeshGroup childMeshGroup = meshGroup._childMeshGroupTransformsWithBones[iChild]._meshGroup;
				//		if (childMeshGroup != null)
				//		{
				//			for (int i = 0; i < childMeshGroup._boneList_Root.Count; i++)
				//			{
				//				childMeshGroup._boneList_Root[i].GUIUpdate(true, isBoneIKUsing);
				//			}
				//		}

				//	}
				//}

				//Bone Set으로 통합
				if (meshGroup._boneListSets != null && meshGroup._boneListSets.Count > 0)
				{
					apMeshGroup.BoneListSet boneSet = null;
					for (int iSet = 0; iSet < meshGroup._boneListSets.Count; iSet++)
					{
						boneSet = meshGroup._boneListSets[iSet];

						for (int iRoot = 0; iRoot < boneSet._bones_Root.Count; iRoot++)
						{
							boneSet._bones_Root[iRoot].GUIUpdate(true, isBoneIKUsing);
						}
					}
				}
			}

			_portrait._isPhysicsPlay_Editor = isPrevPhysics;
		}


		private void RenderAnimatedOnion(apAnimClip animClip, bool isBehindRendering, bool isRepaintType,
										bool isBoneIKMatrix, bool isBoneIKRigging, bool isBoneIKUsing,
										List<apTransform_Mesh> selectedMeshTransforms,
										List<apTransform_MeshGroup> selectedMeshGroupTransforms
										)
		{
			if (animClip == null || !isRepaintType || !Onion.IsVisible || _onionOption_IsRenderBehind != isBehindRendering)
			{
				return;
			}
			if (animClip._targetMeshGroup == null || animClip.IsPlaying_Editor)
			{
				return;
			}

			if (_onionOption_IKCalculateForce)
			{
				isBoneIKUsing = true;
			}
			int curFrame = animClip.CurFrame;



			//그려야할 범위를 계산한다.
			//min~max Frame을 CurFrame과 RenderPerFrame 값을 이용해서 계산한다.
			//Loop이면 프레임이 반복된다. 루프가 아니면 종료

			int animLength = (animClip.EndFrame - animClip.StartFrame) + 1;
			if (animLength < 1)
			{
				return;
			}


			bool isLoop = animClip.IsLoop;
			int prevRange = Mathf.Clamp(_onionOption_PrevRange, 0, animLength / 2);
			int nextRange = Mathf.Clamp(_onionOption_NextRange, 0, animLength / 2);
			int renderPerFrame = Mathf.Max(_onionOption_RenderPerFrame, 0);

			if (renderPerFrame == 0 || (prevRange == 0 && nextRange == 0))
			{
				return;
			}

			prevRange = (prevRange / renderPerFrame) * renderPerFrame;
			nextRange = (nextRange / renderPerFrame) * renderPerFrame;

			int minFrame = curFrame - prevRange;
			int maxFrame = curFrame + nextRange;


			bool isPrevPhysics = _portrait._isPhysicsPlay_Editor;
			_portrait._isPhysicsPlay_Editor = false;


			int renderFrame = 0;
			if (prevRange > 0)
			{
				//Min -> Cur 렌더링
				for (int iFrame = minFrame; iFrame < curFrame; iFrame += renderPerFrame)
				{
					renderFrame = iFrame;
					if (renderFrame < animClip.StartFrame)
					{
						if (isLoop)
						{ renderFrame = (renderFrame + animLength) - 1; }
						else
						{ continue; }
					}
					else if (renderFrame > animClip.EndFrame)
					{
						if (isLoop)
						{ renderFrame = (renderFrame - animLength) + 1; }
						else
						{ continue; }
					}

					animClip.SetFrame_EditorNotStop(renderFrame);

					//렌더 설정 변경
					apGL.SetToneOption(_colorOption_OnionAnimPrevColor,
										_onionOption_OutlineThickness,
										_onionOption_IsOutlineRender,
										_onionOption_PosOffsetX * Mathf.Abs(iFrame - curFrame),
										_onionOption_PosOffsetY * Mathf.Abs(iFrame - curFrame),
										_colorOption_OnionBonePrevColor);

					RenderOnion(animClip._targetMeshGroup, animClip,
						isBehindRendering, isRepaintType, isBoneIKMatrix, isBoneIKRigging, isBoneIKUsing,
						false,//<<기본 RenderOnion과 다른 파라미터이다.
						selectedMeshTransforms, selectedMeshGroupTransforms,
						false);
				}
			}

			if (nextRange > 0)
			{
				//Cur <- Max 렌더링. 프레임이 거꾸로 진행한다.
				for (int iFrame = maxFrame; iFrame > curFrame; iFrame -= renderPerFrame)
				{
					renderFrame = iFrame;
					if (renderFrame < animClip.StartFrame)
					{
						if (isLoop)
						{ renderFrame = (renderFrame + animLength) - 1; }
						else
						{ continue; }
					}
					else if (renderFrame > animClip.EndFrame)
					{
						if (isLoop)
						{ renderFrame = (renderFrame - animLength) + 1; }
						else
						{ continue; }
					}

					animClip.SetFrame_EditorNotStop(renderFrame);

					//렌더 설정 변경
					apGL.SetToneOption(_colorOption_OnionAnimNextColor,
										_onionOption_OutlineThickness,
										_onionOption_IsOutlineRender,
										-_onionOption_PosOffsetX * Mathf.Abs(iFrame - curFrame),
										-_onionOption_PosOffsetY * Mathf.Abs(iFrame - curFrame),
										_colorOption_OnionBoneNextColor);

					RenderOnion(animClip._targetMeshGroup, animClip,
						isBehindRendering, isRepaintType, isBoneIKMatrix, isBoneIKRigging, isBoneIKUsing,
						false,//<<기본 RenderOnion과 다른 파라미터이다.
						selectedMeshTransforms, selectedMeshGroupTransforms,
						false);
				}
			}

			//원래의 값으로 복구 
			animClip.SetFrame_EditorNotStop(curFrame);

			apGL.SetToneOption(_colorOption_OnionToneColor,
									_onionOption_OutlineThickness,
									_onionOption_IsOutlineRender,
									_onionOption_PosOffsetX,
									_onionOption_PosOffsetY,
									_colorOption_OnionBoneColor);

			if (animClip != null)
			{
				animClip.Update_Editor(0.0f, true, isBoneIKMatrix, isBoneIKRigging);
			}

			//<BONE_EDIT>
			//for (int i = 0; i < animClip._targetMeshGroup._boneList_Root.Count; i++)
			//{
			//	animClip._targetMeshGroup._boneList_Root[i].GUIUpdate(true, isBoneIKUsing);
			//}
			//if (animClip._targetMeshGroup._childMeshGroupTransformsWithBones != null)
			//{
			//	for (int iChild = 0; iChild < animClip._targetMeshGroup._childMeshGroupTransformsWithBones.Count; iChild++)
			//	{
			//		apMeshGroup childMeshGroup = animClip._targetMeshGroup._childMeshGroupTransformsWithBones[iChild]._meshGroup;
			//		if (childMeshGroup != null)
			//		{
			//			for (int i = 0; i < childMeshGroup._boneList_Root.Count; i++)
			//			{
			//				childMeshGroup._boneList_Root[i].GUIUpdate(true, isBoneIKUsing);
			//			}
			//		}

			//	}
			//}


			//Bone Set으로 통합
			if (animClip._targetMeshGroup._boneListSets != null && animClip._targetMeshGroup._boneListSets.Count > 0)
			{
				apMeshGroup.BoneListSet boneSet = null;
				for (int iSet = 0; iSet < animClip._targetMeshGroup._boneListSets.Count; iSet++)
				{
					boneSet = animClip._targetMeshGroup._boneListSets[iSet];

					for (int iRoot = 0; iRoot < boneSet._bones_Root.Count; iRoot++)
					{
						boneSet._bones_Root[iRoot].GUIUpdate(true, isBoneIKUsing);
					}
				}
			}

			_portrait._isPhysicsPlay_Editor = isPrevPhysics;

		}


		/// <summary>
		/// Bone Preview를 위해서 ExMode를 변형해서 Rendering하는 부분
		/// </summary>
		/// <param name="meshGroup"></param>
		/// <param name="boneOutlineColor"></param>
		private void RenderExEditBonePreview_Modifier(apMeshGroup meshGroup, Color boneOutlineColor)
		{


			apSelection.EX_EDIT prevExEditMode = Select.ExEditingMode;

			bool isExLockSuccess = Select.SetModifierExclusiveEditing_Tmp(apSelection.EX_EDIT.None);
			if (!isExLockSuccess)
			{
				return;
			}
			Select.RefreshMeshGroupExEditingFlags(meshGroup, null, null, null, true);//<<일단 초기화

			meshGroup.SetBoneIKEnabled(true, false);

			meshGroup.RefreshForce();
			RenderBoneOutlineOnly(meshGroup, boneOutlineColor, true);

			//meshGroup.RefreshForce(false, 0.0f, false);//<ExCalculate를 Ignore한다.

			meshGroup.SetBoneIKEnabled(false, false);

			//Debug.Log("Render ExEdit " + prevExEditMode + " > None > " + prevExEditMode);
			Select.SetModifierExclusiveEditing_Tmp(prevExEditMode);
			meshGroup.RefreshForce();

			meshGroup.BoneGUIUpdate(false);
		}


		private void RenderExEditBonePreview_Animation(apAnimClip animClip, apMeshGroup meshGroup, Color boneOutlineColor)
		{
			if (animClip == null || animClip._targetMeshGroup == null)
			{
				return;
			}

			apSelection.EX_EDIT prevExEditMode = Select.ExAnimEditingMode;

			bool isExLockSuccess = Select.SetAnimExclusiveEditing_Tmp(apSelection.EX_EDIT.None, false);
			if (!isExLockSuccess)
			{
				//Debug.Log("Failed");
				return;
			}

			apModifierBase curModifier = null;
			if (Select.AnimTimeline != null)
			{
				curModifier = Select.AnimTimeline._linkedModifier;
			}
			//Debug.Log("Is Modifier Valid : " + (curModifier != null));

			Select.RefreshMeshGroupExEditingFlags(meshGroup, null, null, animClip, true);//<<일단 초기화

			////meshGroup.RefreshForce();//<ExCalculate를 Ignore한다.
			animClip.Update_Editor(0.0f, true, true, false);
			RenderBoneOutlineOnly(meshGroup, boneOutlineColor, true);

			Select.SetAnimExclusiveEditing_Tmp(prevExEditMode, false);
			Select.RefreshMeshGroupExEditingFlags(meshGroup, curModifier, null, animClip, true);//<<일단 초기화

			animClip.Update_Editor(0.0f, true, false, false);

			//<BONE_EDIT>
			//for (int i = 0; i < animClip._targetMeshGroup._boneList_Root.Count; i++)
			//{
			//	animClip._targetMeshGroup._boneList_Root[i].GUIUpdate(true);
			//}

			//>>Bone Set으로 변경
			apMeshGroup.BoneListSet boneSet = null;
			for (int iSet = 0; iSet < animClip._targetMeshGroup._boneListSets.Count; iSet++)
			{
				boneSet = animClip._targetMeshGroup._boneListSets[iSet];
				for (int iRoot = 0; iRoot < boneSet._bones_Root.Count; iRoot++)
				{
					boneSet._bones_Root[iRoot].GUIUpdate(true);
				}
			}


		}



		private void DrawModifierListUI(int posX, int posY,
										apMeshGroup curMeshGroup, apModifierBase curModifier,
										apAnimClip curAnimClip, apAnimTimeline curAnimTimeline,
										apSelection.EX_EDIT exMode)
		{
			if (curMeshGroup == null)
			{
				return;
			}

			if (curMeshGroup._modifierStack._modifiers.Count == 0)
			{
				return;
			}

			apModifierBase mod = null;
			int imgSize = 16;
			int imgSize_Half = imgSize / 2;
			int textWidth = 130;
			int textHeight = 16;
			int startPosY = posY + (textHeight * curMeshGroup._modifierStack._modifiers.Count) / 2;

			bool isCheckAnim = (curAnimClip != null);

			Texture2D imgCursor = null;
			if (exMode != apSelection.EX_EDIT.ExOnly_Edit)
			{
				imgCursor = ImageSet.Get(apImageSet.PRESET.SmallMod_CursorUnlocked);
			}
			else
			{
				imgCursor = ImageSet.Get(apImageSet.PRESET.SmallMod_CursorLocked);
			}


			int curPosY = 0;
			int posX_Cursor = posX + imgSize_Half;
			int posX_Icon = posX_Cursor + imgSize;
			int posX_Title = posX_Icon + imgSize;
			int posX_ExEnabled = posX_Title + 5 + textWidth;
			int posX_ColorEnabled = posX_ExEnabled + 5 + imgSize;
			Color colorGray = Color.gray;

			Color color_Selected = Color.yellow;
			Color color_NotSelected = new Color(0.8f, 0.8f, 0.8f, 1.0f);
			bool isSelected = false;

			Texture2D imgMod = null;
			Texture2D imgEx = null;
			Texture2D imgColor = null;

			float imgSize_Zoom = (float)imgSize / apGL.Zoom;
			for (int i = 0; i < curMeshGroup._modifierStack._modifiers.Count; i++)
			{
				mod = curMeshGroup._modifierStack._modifiers[i];
				curPosY = startPosY - (i * textHeight);

				isSelected = false;
				if (isCheckAnim)
				{
					if (curAnimTimeline != null && mod == curAnimTimeline._linkedModifier)
					{
						isSelected = true;
					}
				}
				else
				{
					if (curModifier == mod)
					{
						isSelected = true;

					}
				}
				if (isSelected)
				{
					apGL.DrawTextureGL(imgCursor, new Vector2(posX_Cursor, curPosY), imgSize_Zoom, imgSize_Zoom, colorGray, 0);
				}
				imgMod = ImageSet.Get(apEditorUtil.GetSmallModIconType(mod.ModifierType));
				apGL.DrawTextureGL(imgMod, new Vector2(posX_Icon, curPosY), imgSize_Zoom, imgSize_Zoom, colorGray, 0);
				if (isSelected)
				{
					apGL.DrawTextGL(mod.DisplayNameShort, new Vector2(posX_Title, curPosY - imgSize_Half), textWidth, color_Selected);
				}
				else
				{
					apGL.DrawTextGL(mod.DisplayNameShort, new Vector2(posX_Title, curPosY - imgSize_Half), textWidth, color_NotSelected);
				}

				switch (mod._editorExclusiveActiveMod)
				{
					case apModifierBase.MOD_EDITOR_ACTIVE.Disabled:
					case apModifierBase.MOD_EDITOR_ACTIVE.OnlyColorEnabled:
						imgEx = ImageSet.Get(apImageSet.PRESET.SmallMod_ExDisabled);
						break;

					case apModifierBase.MOD_EDITOR_ACTIVE.ExclusiveEnabled:
					case apModifierBase.MOD_EDITOR_ACTIVE.Enabled:
						imgEx = ImageSet.Get(apImageSet.PRESET.SmallMod_ExEnabled);
						break;

					case apModifierBase.MOD_EDITOR_ACTIVE.SubExEnabled:
						imgEx = ImageSet.Get(apImageSet.PRESET.SmallMod_ExSubEnabled);
						break;

					default:
						imgEx = null;
						break;
				}

				if (imgEx != null)
				{
					apGL.DrawTextureGL(imgEx, new Vector2(posX_ExEnabled, curPosY), imgSize_Zoom, imgSize_Zoom, colorGray, 0);
				}

				if (mod._isColorPropertyEnabled &&
					(int)(mod.CalculatedValueType & apCalculatedResultParam.CALCULATED_VALUE_TYPE.Color) != 0)
				{
					if (mod._editorExclusiveActiveMod == apModifierBase.MOD_EDITOR_ACTIVE.Disabled)
					{
						imgColor = ImageSet.Get(apImageSet.PRESET.SmallMod_ColorDisabled);
					}
					else
					{
						imgColor = ImageSet.Get(apImageSet.PRESET.SmallMod_ColorEnabled);
					}
					apGL.DrawTextureGL(imgColor, new Vector2(posX_ColorEnabled, curPosY), imgSize_Zoom, imgSize_Zoom, colorGray, 0);
				}



				curPosY -= textHeight;
			}
		}



		private void GUI_MainRight(int width, int height)
		{
			if (_portrait == null)
			{
				return;
			}
			if (!_selection.DrawEditor(width - 2, height))
			{
				if (_portrait != null)
				{
					_selection.SetPortrait(_portrait);

					//시작은 RootUnit
					_selection.SetOverallDefault();

					OnAnyObjectAddedOrRemoved();
				}
				else
				{
					_selection.Clear();
				}

				SyncHierarchyOrders();

				_hierarchy.ResetAllUnits();
				_hierarchy_MeshGroup.ResetSubUnits();
				_hierarchy_AnimClip.ResetSubUnits();
			}
		}

		private void GUI_MainRight_Header(int width, int height)
		{
			if (_portrait == null)
			{
				return;
			}

			_selection.DrawEditor_Header(width - 2, height);

		}


		private void GUI_MainRight_Lower_MeshGroupHeader(int width, int height, bool is2LineLayout)
		{
			width -= 2;
			if (_portrait == null)
			{
				return;
			}
			//1. 타이틀 + Show/Hide
			//수정) 타이틀이 한개가 아니라 Meshes / Bones로 나뉘며 버튼이 된다.
			EditorGUILayout.BeginHorizontal(GUILayout.Width(width), GUILayout.Height(25));
			GUILayout.Space(5);


			bool isChildMesh = Select._meshGroupChildHierarchy == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes;
			bool isBones = Select._meshGroupChildHierarchy == apSelection.MESHGROUP_CHILD_HIERARCHY.Bones;

			//int toggleBtnWidth = ((width - (25 + 2)) / 2) - 2;
			int toggleBtnWidth = (width / 2) - 2;


			if (apEditorUtil.ToggledButton(GetUIWord(UIWORD.Meshes), isChildMesh, toggleBtnWidth, 20))//"Meshes"
			{
				Select._meshGroupChildHierarchy = apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes;
			}
			if (apEditorUtil.ToggledButton(GetUIWord(UIWORD.Bones), isBones, toggleBtnWidth, 20))//"Bones"
			{
				Select._meshGroupChildHierarchy = apSelection.MESHGROUP_CHILD_HIERARCHY.Bones;
			}

			//GUI.backgroundColor = prevColor;

			//삭제 19.8.18 : 상단의 탭으로 Layout 숨기는 버튼이 이동됨
			//bool isOpened = (_rightLowerLayout == RIGHT_LOWER_LAYOUT.ChildList);//이전
			
			//Texture2D btnImage = null;
			//if (isOpened)
			//{ btnImage = ImageSet.Get(apImageSet.PRESET.Hierarchy_OpenLayout); }
			//else
			//{ btnImage = ImageSet.Get(apImageSet.PRESET.Hierarchy_HideLayout); }

			//GUIStyle guiStyle_Btn = new GUIStyle(GUI.skin.label);
			//if (GUILayout.Button(btnImage, guiStyle_Btn, GUILayout.Width(25), GUILayout.Height(25)))
			//{
			//	if (_rightLowerLayout == RIGHT_LOWER_LAYOUT.ChildList)
			//	{
			//		_rightLowerLayout = RIGHT_LOWER_LAYOUT.Hide;
			//	}
			//	else
			//	{
			//		_rightLowerLayout = RIGHT_LOWER_LAYOUT.ChildList;
			//	}
			//}

			EditorGUILayout.EndHorizontal();

			//2. 추가, 레이어 변경 버튼들
			if (is2LineLayout)
			{
				int btnSize = Mathf.Min(height - (25 + 4), (width / 5) - 5);
				//현재 레이어에 대한 버튼들을 출력한다.
				//1) 추가
				//2) 클리핑 (On/Off)
				//3, 4) 레이어 Up/Down
				//5) 삭제
				EditorGUILayout.BeginHorizontal(GUILayout.Width(width), GUILayout.Height(height - (25 + 4)));
				GUILayout.Space(5);

				apTransform_Mesh selectedMesh = Select.SubMeshInGroup;
				apTransform_MeshGroup selectedMeshGroup = Select.SubMeshGroupInGroup;
				apBone selectedBone = Select.Bone;

				int selectedType = 0;
				apMeshGroup curMeshGroup = Select.MeshGroup;
				if (curMeshGroup != null)
				{
					if (selectedMesh != null) { selectedType = 1; }
					else if (selectedMeshGroup != null) { selectedType = 2; }
					else if (selectedBone != null) { selectedType = 3; }
				}

				if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_AddTransform), false, true, btnSize, btnSize, "Add Sub Mesh / Mesh Group"))
				{
					//1) 추가하기
					//_loadKey_AddChildTransform = apDialog_AddChildTransform.ShowDialog(this, curMeshGroup, OnAddChildTransformDialogResult);
					_loadKey_AddChildTransform = apDialog_SelectMultipleObjects.ShowDialog(	this, 
																				curMeshGroup, 
																				apDialog_SelectMultipleObjects.REQUEST_TARGET.MeshAndMeshGroups, 
																				OnAddMultipleChildTransformDialogResult,
																				GetText(TEXT.Add),
																				curMeshGroup,
																				curMeshGroup);
				}

				bool isClipped = false;
				if (selectedType == 1)
				{
					isClipped = selectedMesh._isClipping_Child;
				}
				if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_SetClipping), isClipped, selectedType == 1, btnSize, btnSize, "Set Clipping Mask"))
				{
					//2) 클리핑
					if (selectedType == 1)
					{
						if (selectedMesh._isClipping_Child)
						{
							//Clip -> Release
							Controller.ReleaseClippingMeshTransform(curMeshGroup, selectedMesh);
						}
						else
						{
							//Release -> Clip
							Controller.AddClippingMeshTransform(curMeshGroup, selectedMesh, true);
						}
					}
				}

				if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Modifier_LayerUp), false, selectedType != 0, btnSize, btnSize, "Layer Up"))
				{
					//3) Layer Up
					if (selectedType == 1 || selectedType == 2)
					{
						apRenderUnit targetRenderUnit = null;
						if (selectedType == 1) { targetRenderUnit = curMeshGroup.GetRenderUnit(selectedMesh); }
						else if (selectedType == 2) { targetRenderUnit = curMeshGroup.GetRenderUnit(selectedMeshGroup); }

						if (targetRenderUnit != null)
						{
							apEditorUtil.SetRecord_MeshGroup(apUndoGroupData.ACTION.MeshGroup_DepthChanged, this, curMeshGroup, targetRenderUnit, false, true);

							//curMeshGroup.ChangeRenderUnitDetph(targetRenderUnit, targetRenderUnit._depth + 1);//이전
							curMeshGroup.ChangeRenderUnitDepth(targetRenderUnit, targetRenderUnit.GetDepth() + 1);
							RefreshControllerAndHierarchy(false);
						}
					}
					else
					{
						curMeshGroup.ChangeBoneDepth(selectedBone, selectedBone._depth + 1);
						RefreshControllerAndHierarchy(false);
					}
				}

				if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Modifier_LayerDown), false, selectedType != 0, btnSize, btnSize, "Layer Down"))
				{
					//4) Layer Down
					if (selectedType == 1 || selectedType == 2)
					{
						apRenderUnit targetRenderUnit = null;
						if (selectedType == 1) { targetRenderUnit = curMeshGroup.GetRenderUnit(selectedMesh); }
						else if (selectedType == 2) { targetRenderUnit = curMeshGroup.GetRenderUnit(selectedMeshGroup); }

						if (targetRenderUnit != null)
						{
							apEditorUtil.SetRecord_MeshGroup(apUndoGroupData.ACTION.MeshGroup_DepthChanged, this, curMeshGroup, targetRenderUnit, false, true);

							//curMeshGroup.ChangeRenderUnitDetph(targetRenderUnit, targetRenderUnit._depth - 1);//이전
							curMeshGroup.ChangeRenderUnitDepth(targetRenderUnit, targetRenderUnit.GetDepth() - 1);
							RefreshControllerAndHierarchy(false);
						}
					}
					else
					{
						curMeshGroup.ChangeBoneDepth(selectedBone, selectedBone._depth - 1);
						RefreshControllerAndHierarchy(false);
					}
				}

				if (apEditorUtil.ToggledButton_2Side(ImageSet.Get(apImageSet.PRESET.Hierarchy_RemoveTransform), false, selectedType == 1 || selectedType == 2, btnSize, btnSize, "Remove Sub Mesh / Mesh Group"))
				{

					string strDialogInfo = Localization.GetText(TEXT.Detach_Body);
					if (selectedType == 1)
					{
						strDialogInfo = Controller.GetRemoveItemMessage(
															_portrait,
															selectedMesh,
															5,
															Localization.GetText(TEXT.Detach_Body),
															Localization.GetText(TEXT.DLG_RemoveItemChangedWarning));
					}
					else if (selectedType == 2)
					{
						strDialogInfo = Controller.GetRemoveItemMessage(
															_portrait,
															selectedMeshGroup,
															5,
															Localization.GetText(TEXT.Detach_Body),
															Localization.GetText(TEXT.DLG_RemoveItemChangedWarning));
					}

					//("Detach", "Do you want to detach it?", "Detach", "Cancel");
					bool isResult = EditorUtility.DisplayDialog(Localization.GetText(TEXT.Detach_Title),
																	//Localization.GetText(TEXT.Detach_Body),
																	strDialogInfo,
																	Localization.GetText(TEXT.Detach_Ok),
																	Localization.GetText(TEXT.Cancel)
																	);

					if (isResult)
					{
						//5) Layer Remove
						if (selectedType == 1)
						{
							Controller.DetachMeshInMeshGroup(selectedMesh, curMeshGroup);
						}
						else if (selectedType == 2)
						{
							Controller.DetachMeshGroupInMeshGroup(selectedMeshGroup, curMeshGroup);
						}
					}
				}
				
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(5);
			}
			
			
		}


		private void GUI_MainRight_Lower_AnimationHeader(int width, int height)
		{
			width -= 2;
			if (_portrait == null)
			{
				return;
			}

			if (Select.AnimClip == null)
			{
				return;
			}

			//1. 타이틀 + Show/Hide
			EditorGUILayout.BeginHorizontal(GUILayout.Width(width), GUILayout.Height(25));
			GUILayout.Space(5);

			//GUIStyle guiStyle = new GUIStyle(GUI.skin.box);
			//guiStyle.normal.textColor = Color.white;
			//guiStyle.alignment = TextAnchor.MiddleCenter;

			Color prevColor = GUI.backgroundColor;


			bool isTimelineSelected = false;
			apMeshGroup targetMeshGroup = Select.AnimClip._targetMeshGroup;

			if (Select.AnimTimeline != null)
			{
				isTimelineSelected = true;
			}

			//선택한 타임 라인의 타입에 따라 하단 하이라키가 바뀐다.
			string headerTitle = "";

			bool isBtnHeader = false;//버튼을 두어서 Header 타입을 보여줄 것인가, Box lable로 보여줄 것인가.

			apSelection.MESHGROUP_CHILD_HIERARCHY childHierarchy = Select._meshGroupChildHierarchy_Anim;
			bool isChildMesh = childHierarchy == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes;
			if (targetMeshGroup == null)
			{
				//headerTitle = "(Select MeshGroup)";
				headerTitle = "(" + GetUIWord(UIWORD.SelectMeshGroup) + ")";
			}
			else
			{
				if (!isTimelineSelected)
				{
					if (childHierarchy == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes)
					{
						//headerTitle = "Mesh Group Layers";
						headerTitle = GetUIWord(UIWORD.MeshGroup) + GetUIWord(UIWORD.Layer);
					}
					else
					{
						//TODO : Bone은 어떻게 처리하지?
						//AnimatedModifier인 경우에는 버튼을 두어서 Layer/Bone 처리할 수 있게 하자
						//headerTitle = "Bones";
						headerTitle = GetUIWord(UIWORD.Bones);
					}
					isBtnHeader = true;
				}
				else
				{
					switch (Select.AnimTimeline._linkType)
					{
						case apAnimClip.LINK_TYPE.AnimatedModifier:
							if (childHierarchy == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes)
							{
								//headerTitle = "Mesh Group Layers";
								headerTitle = GetUIWord(UIWORD.MeshGroup) + GetUIWord(UIWORD.Layer);
							}
							else
							{
								//TODO : Bone은 어떻게 처리하지?
								//AnimatedModifier인 경우에는 버튼을 두어서 Layer/Bone 처리할 수 있게 하자
								//headerTitle = "Bones";
								headerTitle = GetUIWord(UIWORD.Bones);
							}
							isBtnHeader = true;
							break;


						//case apAnimClip.LINK_TYPE.Bone:
						//	headerTitle = "Bones";
						//	break;

						case apAnimClip.LINK_TYPE.ControlParam:
							//headerTitle = "Control Parameters";
							headerTitle = GetUIWord(UIWORD.ControlParameters);
							break;
					}
				}
			}

			if (isBtnHeader)
			{
				//int toggleBtnWidth = ((width - (25 + 2)) / 2) - 2;//이전
				int toggleBtnWidth = (width / 2) - 2;
				if (apEditorUtil.ToggledButton(GetUIWord(UIWORD.Meshes), isChildMesh, toggleBtnWidth, 20))//"Meshes"
				{
					Select._meshGroupChildHierarchy_Anim = apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes;
				}
				if (apEditorUtil.ToggledButton(GetUIWord(UIWORD.Bones), !isChildMesh, toggleBtnWidth, 20))//"Bones"
				{
					Select._meshGroupChildHierarchy_Anim = apSelection.MESHGROUP_CHILD_HIERARCHY.Bones;
				}
			}
			else
			{
				//GUI.backgroundColor = new Color(0.0f, 0.2f, 0.3f, 1.0f);
				GUI.backgroundColor = apEditorUtil.ToggleBoxColor_Selected;

				//GUILayout.Box(headerTitle, guiStyle, GUILayout.Width(width - (25 + 2)), GUILayout.Height(20));
				GUILayout.Box(headerTitle, GUIStyleWrapper.Box_MiddleCenter_WhiteColor, GUILayout.Width(width - (25 + 2)), GUILayout.Height(20));

				GUI.backgroundColor = prevColor;
			}


			
			EditorGUILayout.EndHorizontal();

			
		}

		private object _loadKey_AddChildTransform = null;
		private void OnAddChildTransformDialogResult(bool isSuccess, object loadKey, apMesh mesh, apMeshGroup meshGroup)
		{
			if (!isSuccess)
			{
				return;
			}

			if (_loadKey_AddChildTransform != loadKey)
			{
				return;
			}


			_loadKey_AddChildTransform = null;

			if (Select.MeshGroup == null)
			{
				return;
			}
			if (mesh != null)
			{
				apTransform_Mesh addedMeshTransform = Controller.AddMeshToMeshGroup(mesh);
				if (addedMeshTransform != null)
				{
					Select.SetSubMeshInGroup(addedMeshTransform);
					RefreshControllerAndHierarchy(false);
				}

			}
			else if (meshGroup != null)
			{
				apTransform_MeshGroup addedMeshGroupTransform = Controller.AddMeshGroupToMeshGroup(meshGroup, null);
				if (addedMeshGroupTransform != null)
				{
					Select.SetSubMeshGroupInGroup(addedMeshGroupTransform);
					RefreshControllerAndHierarchy(false);
				}
			}
		}

		public void OnAddMultipleChildTransformDialogResult(bool isSuccess, object loadKey, List<object> selectedObjects, object savedObject)
		{
			if(!isSuccess || _loadKey_AddChildTransform == null || _loadKey_AddChildTransform != loadKey || savedObject == null || selectedObjects == null)
			{
				_loadKey_AddChildTransform = null;
				return;
			}

			_loadKey_AddChildTransform = null;

			apMeshGroup savedMeshGroup = null;
			if(savedObject is apMeshGroup)
			{
				savedMeshGroup = savedObject as apMeshGroup;
			}

			if (Select.MeshGroup == null || savedMeshGroup == null || Select.MeshGroup != savedMeshGroup || selectedObjects.Count == 0)
			{
				return;
			}


			apTransform_Mesh finalAdded_MeshTF = null;
			apTransform_MeshGroup finalAdded_MeshGroupTF = null;

			//Undo
			apEditorUtil.SetRecord_MeshGroup(apUndoGroupData.ACTION.MeshGroup_AttachMesh, this, savedMeshGroup, savedMeshGroup, false, false);

			//추가 19.8.20 : 경고 메시지를 위한 객체
			apEditorController.AttachMeshGroupError attachMeshGroupError = new apEditorController.AttachMeshGroupError();

			//하나씩 열어봅시다.
			for (int i = 0; i < selectedObjects.Count; i++)
			{
				object curObject = selectedObjects[i];
				if(curObject == null)
				{
					continue;
				}

				if(curObject is apMesh)
				{
					apMesh targetMesh = curObject as apMesh;
					if(targetMesh == null)
					{
						continue;
					}

					apTransform_Mesh addedMeshTransform = Controller.AddMeshToMeshGroup(targetMesh, false);
					if (addedMeshTransform != null)
					{
						finalAdded_MeshTF = addedMeshTransform;
						finalAdded_MeshGroupTF = null;
					}
				}
				else if(curObject is apMeshGroup)
				{
					apMeshGroup targetMeshGroup = curObject as apMeshGroup;
					if(targetMeshGroup == null)
					{
						continue;
					}

					apTransform_MeshGroup addedMeshGroupTransform = Controller.AddMeshGroupToMeshGroup(targetMeshGroup, attachMeshGroupError, false);
					if (addedMeshGroupTransform != null)
					{
						finalAdded_MeshTF = null;
						finalAdded_MeshGroupTF = addedMeshGroupTransform;
					}
				}
			}

			savedMeshGroup.SetDirtyToReset();
			savedMeshGroup.RefreshForce();

			//마지막으로 추가된 것을 선택하자.
			if(finalAdded_MeshTF != null)
			{
				Select.SetSubMeshInGroup(finalAdded_MeshTF);
			}
			else if(finalAdded_MeshGroupTF != null)
			{
				Select.SetSubMeshGroupInGroup(finalAdded_MeshGroupTF);
			}

			//추가 19.8.20 : 추가된 메시 그룹에 애니메이션 모디파이어가 있다면 경고
			if(attachMeshGroupError._isError)
			{
				string strMsg = null;
				if(attachMeshGroupError._nError == 1)
				{
					//strMsg = "The added Mesh Group [" + attachMeshGroupError._meshGroups[0]._name + "] has animation modifiers.\nAnimations associated with this Mesh Group may not work properly.\nSelect those animations and change the target Mesh Group.";
					strMsg = string.Format(GetText(TEXT.DLG_AttachMeshGroupInfo_Single_Body), attachMeshGroupError._meshGroups[0]._name);
				}
				else
				{
					//strMsg = "Added " + attachMeshGroupError._nError + " Mesh Groups have animation modifiers.\nAnimations associated with these Mesh Groups may not work properly.\nSelect those animations and change the target Mesh Group.";
					strMsg = string.Format(GetText(TEXT.DLG_AttachMeshGroupInfo_Multi_Body), attachMeshGroupError._nError);
				}

				EditorUtility.DisplayDialog(GetText(TEXT.DLG_AttachMeshGroupInfo_Title), strMsg, GetText(TEXT.Okay));
			}
			

			//추가 / 삭제시 요청한다.
			OnAnyObjectAddedOrRemoved();
			ResetHierarchyAll();
			RefreshControllerAndHierarchy(false);
			SetRepaint();
		}


		private void GUI_MainRight_Lower(int width, int height, Vector2 scroll, bool isGUIEvent)
		{
			if (_portrait == null)
			{
				return;
			}
			if (Select.SelectionType == apSelection.SELECTION_TYPE.MeshGroup)
			{
				GUILayout.Space(5);
				
				#region [미사용 코드] 19.8.18부터 다른 변수에 의해서 Layout 출력 여부가 결정된다.
				//switch (_rightLowerLayout)
				//{
				//	case RIGHT_LOWER_LAYOUT.Hide:
				//		break;

				//	case RIGHT_LOWER_LAYOUT.ChildList:

				//		if (Event.current.type == EventType.Layout)
				//		{
				//			SetGUIVisible("GUI MeshGroup Hierarchy Delayed", true);
				//		}
				//		if (IsDelayedGUIVisible("GUI MeshGroup Hierarchy Delayed"))
				//		{
				//			Hierarchy_MeshGroup.GUI_RenderHierarchy(	width, 
				//														Select._meshGroupChildHierarchy == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes, 
				//														scrollX,
				//														isGUIEvent);
				//		}
				//		break;
				//} 
				#endregion

				//추가 19.8.18
				if(_uiFoldType_Right1_Lower == UI_FOLD_TYPE.Unfolded)
				{
					if (Event.current.type == EventType.Layout)
					{
						SetGUIVisible(DELAYED_UI_TYPE.GUI_MeshGroup_Hierarchy_Delayed, true);//"GUI MeshGroup Hierarchy Delayed"
					}
					if (IsDelayedGUIVisible(DELAYED_UI_TYPE.GUI_MeshGroup_Hierarchy_Delayed))//"GUI MeshGroup Hierarchy Delayed"
					{
						Hierarchy_MeshGroup.GUI_RenderHierarchy(width,
																	Select._meshGroupChildHierarchy == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes,
																	scroll,
																	height,
																	isGUIEvent);
					}
				}

			}
			else if (Select.SelectionType == apSelection.SELECTION_TYPE.Animation)
			{
				GUILayout.Space(5);

				//추가 19.8.18
				if (_uiFoldType_Right1_Lower == UI_FOLD_TYPE.Unfolded)
				{
					if (Select.AnimTimeline == null)
					{
						if (Select._meshGroupChildHierarchy_Anim == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes)
						{
							//Mesh 리스트
							if (Event.current.type == EventType.Layout)
							{
								SetGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__Meshes, true);//"GUI Anim Hierarchy Delayed - Meshes"
							}

							if (IsDelayedGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__Meshes))//"GUI Anim Hierarchy Delayed - Meshes"
							{
								Hierarchy_AnimClip.GUI_RenderHierarchy_Transform(width, scroll, height, isGUIEvent);
							}
						}
						else
						{
							//Bone 리스트
							if (Event.current.type == EventType.Layout)
							{
								SetGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__Bone, true);//"GUI Anim Hierarchy Delayed - Bone"
							}
							if (IsDelayedGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__Bone))//"GUI Anim Hierarchy Delayed - Bone"
							{
								Hierarchy_AnimClip.GUI_RenderHierarchy_Bone(width, scroll, height, isGUIEvent);
							}
						}


					}
					else
					{
						switch (Select.AnimTimeline._linkType)
						{
							case apAnimClip.LINK_TYPE.AnimatedModifier:
								//Hierarchy_AnimClip.GUI_RenderHierarchy_Transform(width, scrollX);
								if (Select._meshGroupChildHierarchy_Anim == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes)
								{
									//Mesh 리스트
									if (Event.current.type == EventType.Layout)
									{
										SetGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__Meshes, true);//"GUI Anim Hierarchy Delayed - Meshes"
									}

									if (IsDelayedGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__Meshes))//"GUI Anim Hierarchy Delayed - Meshes"
									{
										Hierarchy_AnimClip.GUI_RenderHierarchy_Transform(width, scroll, height, isGUIEvent);
									}
								}
								else
								{
									//Bone 리스트
									if (Event.current.type == EventType.Layout)
									{
										SetGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__Bone, true);//"GUI Anim Hierarchy Delayed - Bone"
									}
									if (IsDelayedGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__Bone))//"GUI Anim Hierarchy Delayed - Bone"
									{
										Hierarchy_AnimClip.GUI_RenderHierarchy_Bone(width, scroll, height, isGUIEvent);
									}
								}
								break;

							case apAnimClip.LINK_TYPE.ControlParam:
								{
									if (Event.current.type == EventType.Layout)
									{
										SetGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__ControlParam, true);//"GUI Anim Hierarchy Delayed - ControlParam"
									}
									if (IsDelayedGUIVisible(DELAYED_UI_TYPE.GUI_Anim_Hierarchy_Delayed__ControlParam))//"GUI Anim Hierarchy Delayed - ControlParam"
									{
										Hierarchy_AnimClip.GUI_RenderHierarchy_ControlParam(width, scroll, height, isGUIEvent);
									}
								}

								break;



							default:
								//TODO.. 새로운 타입이 추가될 경우
								break;
						}
					}
				}

				#region [미사용 코드] 19.8.18 Layout의 출력 여부를 결정하는 변수가 바뀌었다.
				//switch (_rightLowerLayout)
				//{
				//	case RIGHT_LOWER_LAYOUT.Hide:
				//		break;

				//	case RIGHT_LOWER_LAYOUT.ChildList:
				//		if (Select.AnimTimeline == null)
				//		{
				//			if (Select._meshGroupChildHierarchy_Anim == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes)
				//			{
				//				//Mesh 리스트
				//				if(Event.current.type == EventType.Layout)
				//				{
				//					SetGUIVisible("GUI Anim Hierarchy Delayed - Meshes", true);
				//				}

				//				if (IsDelayedGUIVisible("GUI Anim Hierarchy Delayed - Meshes"))
				//				{
				//					Hierarchy_AnimClip.GUI_RenderHierarchy_Transform(width, scrollX, isGUIEvent);
				//				}
				//			}
				//			else
				//			{
				//				//Bone 리스트
				//				if(Event.current.type == EventType.Layout)
				//				{
				//					SetGUIVisible("GUI Anim Hierarchy Delayed - Bone", true);
				//				}
				//				if (IsDelayedGUIVisible("GUI Anim Hierarchy Delayed - Bone"))
				//				{
				//					Hierarchy_AnimClip.GUI_RenderHierarchy_Bone(width, scrollX, isGUIEvent);
				//				}
				//			}


				//		}
				//		else
				//		{
				//			switch (Select.AnimTimeline._linkType)
				//			{
				//				case apAnimClip.LINK_TYPE.AnimatedModifier:
				//					//Hierarchy_AnimClip.GUI_RenderHierarchy_Transform(width, scrollX);
				//					if (Select._meshGroupChildHierarchy_Anim == apSelection.MESHGROUP_CHILD_HIERARCHY.ChildMeshes)
				//					{
				//						//Mesh 리스트
				//						if (Event.current.type == EventType.Layout)
				//						{
				//							SetGUIVisible("GUI Anim Hierarchy Delayed - Meshes", true);
				//						}

				//						if (IsDelayedGUIVisible("GUI Anim Hierarchy Delayed - Meshes"))
				//						{
				//							Hierarchy_AnimClip.GUI_RenderHierarchy_Transform(width, scrollX, isGUIEvent);
				//						}
				//					}
				//					else
				//					{
				//						//Bone 리스트
				//						if(Event.current.type == EventType.Layout)
				//						{
				//							SetGUIVisible("GUI Anim Hierarchy Delayed - Bone", true);
				//						}
				//						if (IsDelayedGUIVisible("GUI Anim Hierarchy Delayed - Bone"))
				//						{
				//							Hierarchy_AnimClip.GUI_RenderHierarchy_Bone(width, scrollX, isGUIEvent);
				//						}
				//					}
				//					break;

				//				case apAnimClip.LINK_TYPE.ControlParam:
				//					{
				//						if(Event.current.type == EventType.Layout)
				//						{
				//							SetGUIVisible("GUI Anim Hierarchy Delayed - ControlParam", true);
				//						}
				//						if (IsDelayedGUIVisible("GUI Anim Hierarchy Delayed - ControlParam"))
				//						{
				//							Hierarchy_AnimClip.GUI_RenderHierarchy_ControlParam(width, scrollX, isGUIEvent);
				//						}
				//					}

				//					break;



				//				default:
				//					//TODO.. 새로운 타입이 추가될 경우
				//					break;
				//			}
				//		}
				//		break;
				//} 
				#endregion
			}
		}


		private void GUI_MainRight2(int width, int height)
		{
			if (_portrait == null)
			{
				return;
			}
			Select.DrawEditor_Right2(width - 2, height);
			//if(!_selection.DrawEditor(width - 2, height))
			//{
			//	if(_portrait != null)
			//	{
			//		_selection.SetPortrait(_portrait);
			//	}
			//	else
			//	{
			//		_selection.Clear();
			//	}

			//	_hierarchy.ResetAllUnits();
			//	_hierarchy_MeshGroup.ResetMeshGroupSubUnits();
			//}
		}

		private void GUI_Bottom(int width, int height, int layoutX, int layoutY, int windowWidth, int windowHeight)
		{
			GUILayout.Space(5);

			Select.DrawEditor_Bottom(width, height - 8, layoutX, layoutY, windowWidth, windowHeight);
		}

		private void GUI_Bottom2(int width, int height)
		{
			GUILayout.Space(10);

			Select.DrawEditor_Bottom2Edit(width, height - 12);
		}

		//--------------------------------------------------------------
		
		// 추가 19.8.18 : FullScreen, Fold에 대한 개선
		public void UnfoldAllTab()
		{
			_isFullScreenGUI = false;

			_uiFoldType_Left = UI_FOLD_TYPE.Unfolded;
			_uiFoldType_Right1 = UI_FOLD_TYPE.Unfolded;
			_uiFoldType_Right2 = UI_FOLD_TYPE.Unfolded;

			_uiFoldType_Right1_Upper = UI_FOLD_TYPE.Unfolded;
			_uiFoldType_Right1_Lower = UI_FOLD_TYPE.Unfolded;
		}

		
		

		//--------------------------------------------------------------









		// 프레임 카운트
		/// <summary>
		/// 프레임 시간을 계산한다.
		/// Update의 경우 60FPS 기준의 동기화된 시간을 사용한다.
		/// "60FPS으로 동기가 된 상태"인 경우에 true로 리턴을 한다.
		/// </summary>
		/// <param name="frameTimerType"></param>
		/// <returns></returns>
		private bool UpdateFrameCount(FRAME_TIMER_TYPE frameTimerType)
		{
			switch (frameTimerType)
			{
				case FRAME_TIMER_TYPE.None:
					return false;

				case FRAME_TIMER_TYPE.Repaint:
					apTimer.I.CheckTime_Repaint(_lowCPUStatus);

					//이전 방식
					//if(Mathf.Abs(apTimer.I.FPS - _iAvgFPS) > 30 || apTimer.I.FPS < 15)
					//{
					//	//차이가 크거나, 갱신 주기가 너무 긴 경우 빨리 바뀜
					//	_avgFPS = _avgFPS * 0.4f + ((float)apTimer.I.FPS) * 0.6f;
					//}
					//else
					//{
					//	//차이가 작으면 서서히 바뀜 
					//	_avgFPS = _avgFPS * 0.93f + ((float)apTimer.I.FPS) * 0.07f;
					//}
					
					//_iAvgFPS = (int)(_avgFPS + 0.5f);

					//개선된 방식 19.11.23
					if(_fpsCounter == null)
					{
						_fpsCounter = new apFPSCounter();
					}
					_fpsCounter.SetData(apTimer.I.FPS);

					return false;

				case FRAME_TIMER_TYPE.Update:
					return apTimer.I.CheckTime_Update(_lowCPUStatus);
			}
			return false;
			
		}
		

		//----------------------------------------------------------------------------
		//추가 3.1 : CPU가 느리게 재생될 수도 있다.
		private void CheckLowCPUOption()
		{
			if(!_isLowCPUOption)
			{
				_lowCPUStatus = LOW_CPU_STATUS.None;
				return;
			}

			_lowCPUStatus = LOW_CPU_STATUS.Full;
			if(_selection == null)
			{
				return;
			}

			if(_portrait == null)
			{
				_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
				return;
			}

			//메뉴마다 다르다.
			switch (_selection.SelectionType)
			{
				case apSelection.SELECTION_TYPE.Overall:
					{
						//다음의 경우가 아니라면 LowCPU 모드가 작동한다.
						//- 애니메이션이 재생 중
						//- 화면이 캡쳐되는 중
						if(_selection.RootUnitAnimClip != null && _selection.RootUnitAnimClip.IsPlaying_Editor)
						{
							_lowCPUStatus = LOW_CPU_STATUS.Full;
						}
						else if(_isScreenCaptureRequest)
						{
							_lowCPUStatus = LOW_CPU_STATUS.Full;
						}
						else
						{
							
							_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Mid;
						}
					}
					
					break;
				case apSelection.SELECTION_TYPE.ImageRes:
					//이미지 메뉴에서는 항상 LowCPU
					_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
					break;

				case apSelection.SELECTION_TYPE.Mesh:
					{
						//탭마다 LowCPU의 정도가 다르다.
						switch (_meshEditMode)
						{
							case MESH_EDIT_MODE.Setting:
								_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
								break;

							case MESH_EDIT_MODE.MakeMesh:
								if (_meshEditeMode_MakeMesh == MESH_EDIT_MODE_MAKEMESH.AutoGenerate)
								{
									_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
								}
								else
								{
									_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Mid;
								}
								break;

							case MESH_EDIT_MODE.Modify:
								_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Mid;
								break;

							case MESH_EDIT_MODE.PivotEdit:
								_lowCPUStatus = LOW_CPU_STATUS.Full;
								break;
						}
					}
					
					break;

				case apSelection.SELECTION_TYPE.MeshGroup:
					{
						switch (_meshGroupEditMode)
						{
							case MESHGROUP_EDIT_MODE.Setting:
								_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Mid;
								break;

							case MESHGROUP_EDIT_MODE.Modifier:
								if(_selection.Modifier != null)
								{
									//모디파이어에 따라서 CPU가 다르다.
									//Physics > Full (편집 모드 상관 없음)
									//Rigging > Low
									//Morph계열 > 편집 모드 : Mid, 단 Blur 툴 켰을땐 Full / 일반 Low
									//Transform계열 > 편집 모드 : Mid / 일반 Low
									
									bool isEditMode = _selection.ExEditingMode != apSelection.EX_EDIT.None;

									switch (_selection.Modifier.ModifierType)
									{
										case apModifierBase.MODIFIER_TYPE.Physic:
											_lowCPUStatus = LOW_CPU_STATUS.Full;
											break;

										case apModifierBase.MODIFIER_TYPE.Rigging:
											if(Select.IsRigEditBinding && Select.RiggingBrush_Mode != apSelection.RIGGING_BRUSH_TOOL_MODE.None)
											{
												//브러시로 Rigging을 하는 중일때
												_lowCPUStatus = LOW_CPU_STATUS.Full;
											}
											else
											{
												_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
											}
											
											break;

										case apModifierBase.MODIFIER_TYPE.Morph:
										case apModifierBase.MODIFIER_TYPE.AnimatedMorph:
											if(isEditMode)
											{
												if(_gizmos != null && _gizmos.IsBrushMode)
												{
													_lowCPUStatus = LOW_CPU_STATUS.Full;
												}
												else
												{
													_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Mid;
												}
											}
											else
											{
												_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
											}
											break;

										case apModifierBase.MODIFIER_TYPE.TF:
										case apModifierBase.MODIFIER_TYPE.AnimatedTF:
											if(isEditMode)
											{
												_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Mid;
											}
											else
											{
												_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
											}
											break;
									}
								}
								else
								{
									_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
								}
								break;
						}
					}
					break;

				case apSelection.SELECTION_TYPE.Animation:
					if(_selection.AnimClip != null)
					{
						//애니메이션 모드에서는
						//- 재생 중일 때는 항상 Full
						//- 편집 모드에서는 Mid / 단 Blur가 켜질땐 Full
						//- 그 외에는 Low
						bool isEditMode = _selection.ExAnimEditingMode != apSelection.EX_EDIT.None;
						if(_selection.AnimClip.IsPlaying_Editor)
						{
							_lowCPUStatus = LOW_CPU_STATUS.Full;
						}
						else if(isEditMode)
						{
							if(_gizmos != null && _gizmos.IsBrushMode)
							{
								_lowCPUStatus = LOW_CPU_STATUS.Full;
							}
							else
							{
								_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Mid;
							}
						}
						else
						{
							_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
						}
					}
					else
					{
						_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
					}
					break;

				case apSelection.SELECTION_TYPE.Param:
					//컨트롤 파라미터 메뉴에서는 항상 Low
					_lowCPUStatus = LOW_CPU_STATUS.LowCPU_Low;
					break;
			}
		}


		//-----------------------------------------------------------------------
		public void OnHotKeyDown(KeyCode keyCode, bool isCtrl, bool isAlt, bool isShift)
		{
			if (_isHotKeyProcessable)
			{
				_isHotKeyEvent = true;
				_hotKeyCode = keyCode;

				_isHotKey_Ctrl = isCtrl;
				_isHotKey_Alt = isAlt;
				_isHotKey_Shift = isShift;
				_isHotKeyProcessable = false;
			}
			else
			{
				if (!_isHotKeyEvent)
				{
					_isHotKeyProcessable = true;
				}
			}
		}

		public void OnHotKeyUp()
		{
			_isHotKeyProcessable = true;
			_isHotKeyEvent = false;
			_hotKeyCode = KeyCode.A;
			_isHotKey_Ctrl = false;
			_isHotKey_Alt = false;
			_isHotKey_Shift = false;
		}

		public void UseHotKey()
		{
			_isHotKeyEvent = false;
			_hotKeyCode = KeyCode.A;
			_isHotKey_Ctrl = false;
			_isHotKey_Alt = false;
			_isHotKey_Shift = false;
		}

		public bool IsHotKeyEvent { get { return _isHotKeyEvent; } }
		public KeyCode HotKeyCode { get { return _hotKeyCode; } }
		public bool IsHotKey_Ctrl { get { return _isHotKey_Ctrl; } }
		public bool IsHotKey_Alt { get { return _isHotKey_Alt; } }
		public bool IsHotKey_Shift { get { return _isHotKey_Shift; } }



		//-----------------------------------------------------------------------------
		// GUILayout의 Visible 여부가 외부에 의해 결정되는 경우
		// 바로 바뀌면 GUI 에러가 나므로 약간의 지연 처리를 해야한다.
		//-----------------------------------------------------------------------------

		/// <summary>
		/// GUILayout가 Show/Hide 토글시, 그 정보를 저장한다.
		/// 범용으로 쓸 수 있게 하기 위해서 String을 키값으로 Dictionary에 저장한다.
		/// </summary>
		/// <param name="keyName">Dictionary 키값</param>
		/// <param name="isVisible">Visible 값</param>
		public void SetGUIVisible(DELAYED_UI_TYPE keyType, bool isVisible)
		{
			if (_delayedGUIShowList.ContainsKey(keyType))
			{
				if (_delayedGUIShowList[keyType] != isVisible)
				{
					_delayedGUIShowList[keyType] = isVisible;//Visible 조건 값

					//_delayedGUIToggledList는 "Visible 값이 바뀌었을때 그걸 바로 GUI에 적용했는지"를 저장한다.
					//바뀐 순간엔 GUI에 적용 전이므로 "Visible Toggle이 완료되었는지"를 저장하는 리스트엔 False를 넣어둔다.
					_delayedGUIToggledList[keyType] = false;
				}
			}
			else
			{
				_delayedGUIShowList.Add(keyType, isVisible);
				_delayedGUIToggledList.Add(keyType, false);
			}
		}


		/// <summary>
		/// GUILayout 출력 가능한지 여부 알려주는 함수
		/// Hide -> Show 상태에서 GUI Event가 Layout/Repaint가 아니라면 잠시 Hide 상태를 유지해야한다.
		/// </summary>
		/// <param name="keyType">Dictionary 키값</param>
		/// <returns>True이면 GUILayout을 출력할 수 있다. False는 안됨</returns>
		public bool IsDelayedGUIVisible(DELAYED_UI_TYPE keyType)
		{
			//GUI Layout이 출력되려면
			//1. Visible 값이 True여야 한다.
			//2-1. GUI Event가 Layout/Repaint 여야 한다.
			//2-2. GUI Event 종류에 상관없이 계속 Visible 상태였다면 출력 가능하다.


			//1-1. GUI Layout의 Visible 여부를 결정하는 값이 없다면 -> False
			if (!_delayedGUIShowList.ContainsKey(keyType))
			{
				return false;
			}

			//1-2. GUI Layout의 Visible 값이 False라면 -> False
			if (!_delayedGUIShowList[keyType])
			{
				return false;
			}

			//2. (Toggle 처리가 완료되지 않은 상태에서..)
			if (!_delayedGUIToggledList[keyType])
			{
				//2-1. GUI Event가 Layout/Repaint라면 -> 다음 OnGUI까지 일단 보류합니다. False
				if (!_isGUIEvent)
				{
					return false;
				}

				// GUI Event가 유효하다면 Visible이 가능하다고 바꿔줍니다.
				//_delayedGUIToggledList [False -> True]
				_delayedGUIToggledList[keyType] = true;
			}

			//2-2. GUI Event 종류에 상관없이 계속 Visible 상태였다면 출력 가능하다. -> True
			return true;
		}

		/// <summary>
		/// Scroll Position을 리셋한다.
		/// </summary>
		public void ResetScrollPosition(bool isResetLeft, bool isResetCenter, bool isResetRight, bool isResetRight2, bool isResetBottom)
		{
			if (isResetLeft)
			{
				_scroll_MainLeft = Vector2.zero;
			}
			if (isResetCenter)
			{
				_scroll_MainCenter = Vector2.zero;
			}
			if (isResetRight)
			{
				_scroll_MainRight = Vector2.zero;
				_scroll_MainRight_Lower_MG_Mesh = Vector2.zero;
				_scroll_MainRight_Lower_MG_Bone = Vector2.zero;
				_scroll_MainRight_Lower_Anim = Vector2.zero;
			}

			if(isResetRight2)
			{
				_scroll_MainRight2 = Vector2.zero;
			}

			if (isResetBottom)
			{
				_scroll_Bottom = Vector2.zero;
			}

			
		}

		public void SetLeftTab(TAB_LEFT  leftTabType)
		{	
			if(_tabLeft != leftTabType)
			{
				ResetScrollPosition(true, false, false, false, false);
			}
			_tabLeft = leftTabType;
		}

		//------------------------------------------------------------------------
		// Hot Key
		//------------------------------------------------------------------------
		/// <summary>
		/// HotKey 이벤트를 등록합니다.
		/// </summary>
		/// <param name="funcEvent"></param>
		/// <param name="keyCode"></param>
		/// <param name="isShift"></param>
		/// <param name="isAlt"></param>
		/// <param name="isCtrl"></param>
		public void AddHotKeyEvent(apHotKey.FUNC_HOTKEY_EVENT funcEvent, string label, KeyCode keyCode, bool isShift, bool isAlt, bool isCtrl, object paramObject)
		{
			HotKey.AddHotKeyEvent(funcEvent, label, keyCode, isShift, isAlt, isCtrl, paramObject);
		}


		//------------------------------------------------------------------------
		// Hierarchy Filter 제어.
		//------------------------------------------------------------------------
		/// <summary>
		/// Hierarchy Filter 제어.
		/// 수동으로 제어하거나, "어떤 객체가 추가"되었다면 Filter가 열린다.
		/// All, None은 isEnabled의 영향을 받지 않는다. (All은 모두 True, None은 모두 False)
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="isEnabled"></param>
		public void SetHierarchyFilter(HIERARCHY_FILTER filter, bool isEnabled)
		{
			HIERARCHY_FILTER prevFilter = _hierarchyFilter;
			bool isRootUnit = ((int)(_hierarchyFilter & HIERARCHY_FILTER.RootUnit) != 0);
			bool isImage = ((int)(_hierarchyFilter & HIERARCHY_FILTER.Image) != 0);
			bool isMesh = ((int)(_hierarchyFilter & HIERARCHY_FILTER.Mesh) != 0);
			bool isMeshGroup = ((int)(_hierarchyFilter & HIERARCHY_FILTER.MeshGroup) != 0);
			bool isAnimation = ((int)(_hierarchyFilter & HIERARCHY_FILTER.Animation) != 0);
			bool isParam = ((int)(_hierarchyFilter & HIERARCHY_FILTER.Param) != 0);

			switch (filter)
			{
				case HIERARCHY_FILTER.All:
					isRootUnit = true;
					isImage = true;
					isMesh = true;
					isMeshGroup = true;
					isAnimation = true;
					isParam = true;
					break;

				case HIERARCHY_FILTER.RootUnit:
					isRootUnit = isEnabled;
					break;


				case HIERARCHY_FILTER.Image:
					isImage = isEnabled;
					break;

				case HIERARCHY_FILTER.Mesh:
					isMesh = isEnabled;
					break;

				case HIERARCHY_FILTER.MeshGroup:
					isMeshGroup = isEnabled;
					break;

				case HIERARCHY_FILTER.Animation:
					isAnimation = isEnabled;
					break;

				case HIERARCHY_FILTER.Param:
					isParam = isEnabled;
					break;

				case HIERARCHY_FILTER.None:
					isRootUnit = false;
					isImage = false;
					isMesh = false;
					isMeshGroup = false;
					isAnimation = false;
					isParam = false;
					break;
			}

			_hierarchyFilter = HIERARCHY_FILTER.None;

			if (isRootUnit)		{ _hierarchyFilter |= HIERARCHY_FILTER.RootUnit; }
			if (isImage)		{ _hierarchyFilter |= HIERARCHY_FILTER.Image; }
			if (isMesh)			{ _hierarchyFilter |= HIERARCHY_FILTER.Mesh; }
			if (isMeshGroup)	{ _hierarchyFilter |= HIERARCHY_FILTER.MeshGroup; }
			if (isAnimation)	{ _hierarchyFilter |= HIERARCHY_FILTER.Animation; }
			if (isParam)		{ _hierarchyFilter |= HIERARCHY_FILTER.Param; }

			if (prevFilter != _hierarchyFilter && _tabLeft == TAB_LEFT.Hierarchy)
			{
				_scroll_MainLeft = Vector2.zero;
			}

			//이건 설정으로 저장해야한다.
			SaveEditorPref();
		}

		public bool IsHierarchyFilterContain(HIERARCHY_FILTER filter)
		{
			if (filter == HIERARCHY_FILTER.All)
			{
				return _hierarchyFilter == HIERARCHY_FILTER.All;
			}
			return (int)(_hierarchyFilter & filter) != 0;
		}

		//Dialog Event
		//----------------------------------------------------------------------------------
		private object _loadKey_FFDStart = null;
		private int _curFFDSizeX = 3;
		private int _curFFDSizeY = 3;



		

		private void OnDialogEvent_FFDStart(bool isSuccess, object loadKey, int numX, int numY)
		{
			if (_loadKey_FFDStart != loadKey || !isSuccess)
			{
				_loadKey_FFDStart = null;
				return;
			}
			_loadKey_FFDStart = null;

			if(numX < 2)
			{
				numX = 2;
			}
			if(numY < 2)
			{
				numY = 2;
			}
			_curFFDSizeX = numX;
			_curFFDSizeY = numY;

			Gizmos.StartTransformMode(this, _curFFDSizeX, _curFFDSizeY);//원래는 <이거
		}



		//---------------------------------------------------------------------------
		// Portrait 생성
		//---------------------------------------------------------------------------
		private void MakeNewPortrait()
		{
			if (!_isMakePortraitRequest)
			{
				return;
			}
			GameObject newPortraitObj = new GameObject(_requestedNewPortraitName);
			newPortraitObj.transform.position = Vector3.zero;
			newPortraitObj.transform.rotation = Quaternion.identity;
			newPortraitObj.transform.localScale = Vector3.one;

			_portrait = newPortraitObj.AddComponent<apPortrait>();

			//Selection.activeGameObject = newPortraitObj;
			Selection.activeGameObject = null;//<<선택을 해제해준다. 프로파일러를 도와줘야져

			_isMakePortraitRequest = false;
			_requestedNewPortraitName = "";


			//추가
			//초기화시 Important와 Opt 정보는 여기서 별도로 초기화를 하자
			_portrait._isOptimizedPortrait = false;
			_portrait._bakeTargetOptPortrait = null;
			_portrait._bakeSrcEditablePortrait = null;
			_portrait._isImportant = true;


			Controller.InitTmpValues();
			_selection.SetPortrait(_portrait);
			
			//Portrait의 레퍼런스들을 연결해주자
			Controller.PortraitReadyToEdit();




			//Selection.activeGameObject = _portrait.gameObject;
			Selection.activeGameObject = null;//<<선택을 해제해준다. 프로파일러를 도와줘야져

			//시작은 RootUnit
			_selection.SetOverallDefault();

			OnAnyObjectAddedOrRemoved();

			SyncHierarchyOrders();

			_hierarchy.ResetAllUnits();
			_hierarchy_MeshGroup.ResetSubUnits();
			_hierarchy_AnimClip.ResetSubUnits();
		}

		private void MakePortraitFromBackupFile()
		{
			if(!_isMakePortraitRequestFromBackupFile)
			{
				return;
			}

			apPortrait loadedPortrait = Backup.LoadBackup(_requestedLoadedBackupPortraitFilePath);
			if (loadedPortrait != null)
			{
				_portrait = loadedPortrait;

				//초기 설정 추가 (Important는 백업 설정을 따른다)
				_portrait._isOptimizedPortrait = false;
				_portrait._bakeTargetOptPortrait = null;
				_portrait._bakeSrcEditablePortrait = null;

				Selection.activeGameObject = null;//<<선택을 해제해준다. 프로파일러를 도와줘야져

				Controller.InitTmpValues();
				_selection.SetPortrait(_portrait);

				//Portrait의 레퍼런스들을 연결해주자
				Controller.PortraitReadyToEdit();


				//Selection.activeGameObject = _portrait.gameObject;
				Selection.activeGameObject = null;//<<선택을 해제해준다. 프로파일러를 도와줘야져

				//시작은 RootUnit
				_selection.SetOverallDefault();

				OnAnyObjectAddedOrRemoved();

				SyncHierarchyOrders();

				_hierarchy.ResetAllUnits();
				_hierarchy_MeshGroup.ResetSubUnits();
				_hierarchy_AnimClip.ResetSubUnits();

				Notification("Backup File [" + _requestedLoadedBackupPortraitFilePath + "] is loaded", false, false);
			}

			_isMakePortraitRequestFromBackupFile = false;
			_requestedLoadedBackupPortraitFilePath = "";

			

			
		}

		//----------------------------------------------------------------------------------
		// 에디터 옵션을 저장/로드하자
		//----------------------------------------------------------------------------------
		public void SaveEditorPref()
		{
			//Debug.Log("Save Editor Pref");

			EditorPrefs.SetInt("AnyPortrait_HierarchyFilter", (int)_hierarchyFilter);

			if (_selection != null)
			{
				EditorPrefs.SetBool("AnyPortrait_IsAutoNormalize", Select._rigEdit_isAutoNormalize);
				//EditorPrefs.SetBool("AnyPortrait_IsBoneColorView", Select._rigEdit_isBoneColorView);
				//EditorPrefs.SetInt("AnyPortrait_ViewMode", (int)Select._rigEdit_viewMode);
			}

			EditorPrefs.SetInt("AnyPortrait_Language", (int)_language);
			SaveColorPref("AnyPortrait_Color_Backgroud", _colorOption_Background);
			SaveColorPref("AnyPortrait_Color_GridCenter", _colorOption_GridCenter);
			SaveColorPref("AnyPortrait_Color_Grid", _colorOption_Grid);

			SaveColorPref("AnyPortrait_Color_MeshEdge", _colorOption_MeshEdge);
			SaveColorPref("AnyPortrait_Color_MeshHiddenEdge", _colorOption_MeshHiddenEdge);
			SaveColorPref("AnyPortrait_Color_Outline", _colorOption_Outline);
			SaveColorPref("AnyPortrait_Color_TFBorder", _colorOption_TransformBorder);

			SaveColorPref("AnyPortrait_Color_VertNotSelected", _colorOption_VertColor_NotSelected);
			SaveColorPref("AnyPortrait_Color_VertSelected", _colorOption_VertColor_Selected);

			SaveColorPref("AnyPortrait_Color_GizmoFFDLine", _colorOption_GizmoFFDLine);
			SaveColorPref("AnyPortrait_Color_GizmoFFDInnerLine", _colorOption_GizmoFFDInnerLine);

			SaveColorPref("AnyPortrait_Color_OnionToneColor", _colorOption_OnionToneColor);
			SaveColorPref("AnyPortrait_Color_OnionAnimPrevColor", _colorOption_OnionAnimPrevColor);
			SaveColorPref("AnyPortrait_Color_OnionAnimNextColor", _colorOption_OnionAnimNextColor);
			SaveColorPref("AnyPortrait_Color_OnionBoneColor", _colorOption_OnionBoneColor);
			SaveColorPref("AnyPortrait_Color_OnionBonePrevColor", _colorOption_OnionBonePrevColor);
			SaveColorPref("AnyPortrait_Color_OnionBoneNextColor", _colorOption_OnionBoneNextColor);

			EditorPrefs.SetBool("AnyPortrait_Onion_OutlineRender", _onionOption_IsOutlineRender);
			EditorPrefs.SetFloat("AnyPortrait_Onion_OutlineThickness", _onionOption_OutlineThickness);
			EditorPrefs.SetBool("AnyPortrait_Onion_RenderOnlySelected", _onionOption_IsRenderOnlySelected);
			EditorPrefs.SetBool("AnyPortrait_Onion_RenderBehind", _onionOption_IsRenderBehind);
			EditorPrefs.SetBool("AnyPortrait_Onion_RenderAnimFrames", _onionOption_IsRenderAnimFrames);
			EditorPrefs.SetInt("AnyPortrait_Onion_PrevRange", _onionOption_PrevRange);
			EditorPrefs.SetInt("AnyPortrait_Onion_NextRange", _onionOption_NextRange);
			EditorPrefs.SetInt("AnyPortrait_Onion_RenderPerFrame", _onionOption_RenderPerFrame);
			EditorPrefs.SetFloat("AnyPortrait_Onion_PosOffsetX", _onionOption_PosOffsetX);
			EditorPrefs.SetFloat("AnyPortrait_Onion_PosOffsetY", _onionOption_PosOffsetY);
			EditorPrefs.SetBool("AnyPortrait_Onion_IKCalculate", _onionOption_IKCalculateForce);
		
			

			EditorPrefs.SetInt("AnyPortrait_AnimTimelineLayerSort", (int)_timelineInfoSortType);

			EditorPrefs.SetInt("AnyPortrait_Capture_PosX", _captureFrame_PosX);
			EditorPrefs.SetInt("AnyPortrait_Capture_PosY", _captureFrame_PosY);
			EditorPrefs.SetInt("AnyPortrait_Capture_SrcWidth", _captureFrame_SrcWidth);
			EditorPrefs.SetInt("AnyPortrait_Capture_SrcHeight", _captureFrame_SrcHeight);
			EditorPrefs.SetInt("AnyPortrait_Capture_DstWidth", _captureFrame_DstWidth);
			EditorPrefs.SetInt("AnyPortrait_Capture_DstHeight", _captureFrame_DstHeight);

			EditorPrefs.SetInt("AnyPortrait_Capture_SpriteUnitWidth", _captureFrame_SpriteUnitWidth);
			EditorPrefs.SetInt("AnyPortrait_Capture_SpriteUnitHeight", _captureFrame_SpriteUnitHeight);
			EditorPrefs.SetInt("AnyPortrait_Capture_SpriteMargin", _captureFrame_SpriteMargin);
			

			EditorPrefs.SetBool("AnyPortrait_Capture_IsShowFrame", _isShowCaptureFrame);
			SaveColorPref("AnyPortrait_Capture_BGColor", _captureFrame_Color);
			EditorPrefs.SetBool("AnyPortrait_Capture_IsPhysics", _captureFrame_IsPhysics);

			EditorPrefs.SetBool("AnyPortrait_Capture_IsAspectRatioFixed", _isCaptureAspectRatioFixed);
			//EditorPrefs.SetInt("AnyPortrait_Capture_GIFSample", _captureFrame_GIFSampleQuality);
			EditorPrefs.SetInt("AnyPortrait_Capture_GIFQuality", (int)_captureFrame_GIFQuality);
			
			EditorPrefs.SetInt("AnyPortrait_Capture_GIFLoopCount", _captureFrame_GIFSampleLoopCount);

			EditorPrefs.SetInt("AnyPortrait_Capture_SpritePackImageWidth", (int)_captureSpritePackImageWidth);
			EditorPrefs.SetInt("AnyPortrait_Capture_SpritePackImageHeight", (int)_captureSpritePackImageHeight);
			EditorPrefs.SetInt("AnyPortrait_Capture_SpriteTrimSize", (int)_captureSpriteTrimSize);
			EditorPrefs.SetBool("AnyPortrait_Capture_SpriteMeta_XML", _captureSpriteMeta_XML);
			EditorPrefs.SetBool("AnyPortrait_Capture_SpriteMeta_JSON", _captureSpriteMeta_JSON);
			EditorPrefs.SetBool("AnyPortrait_Capture_SpriteMeta_TXT", _captureSpriteMeta_TXT);
			
			EditorPrefs.SetFloat("AnyPortrait_Capture_SpriteScreenPosX", _captureSprite_ScreenPos.x);
			EditorPrefs.SetFloat("AnyPortrait_Capture_SpriteScreenPosY", _captureSprite_ScreenPos.y);
			EditorPrefs.SetInt("AnyPortrait_Capture_SpriteScreenZoomIndex", _captureSprite_ScreenZoom);

			
		
			EditorPrefs.SetInt("AnyPortrait_BoneRenderMode", (int)_boneGUIRenderMode);

			EditorPrefs.SetBool("AnyPortrait_GUI_FPSVisible", _guiOption_isFPSVisible);
			EditorPrefs.SetBool("AnyPortrait_GUI_StatisticsVisible", _guiOption_isStatisticsVisible);


			EditorPrefs.SetBool("AnyPortrait_AutoBackup_Enabled", _backupOption_IsAutoSave);
			EditorPrefs.SetString("AnyPortrait_AutoBackup_Path", _backupOption_BaseFolderName);
			EditorPrefs.SetInt("AnyPortrait_AutoBackup_Time", _backupOption_Minute);

			EditorPrefs.SetString("AnyPortrait_BonePose_Path", _bonePose_BaseFolderName);

			EditorPrefs.SetBool("AnyPortrait_StartScreen_IsShow", _startScreenOption_IsShowStartup);
			EditorPrefs.SetInt("AnyPortrait_StartScreen_LastMonth", _startScreenOption_LastMonth);
			EditorPrefs.SetInt("AnyPortrait_StartScreen_LastDay", _startScreenOption_LastDay);
			EditorPrefs.SetInt("AnyPortrait_UpdateLogScreen_LastVersion", _updateLogScreen_LastVersion);
			

			EditorPrefs.SetBool("AnyPortrait_IsBakeColorSpace_ToGamma", _isBakeColorSpaceToGamma);
			EditorPrefs.SetBool("AnyPortrait_IsUseLWRPShader", _isUseSRP);


			EditorPrefs.SetBool("AnyPortrait_ModLockOp_CalculateIfNotAddedOther",	_modLockOption_CalculateIfNotAddedOther);
			EditorPrefs.SetBool("AnyPortrait_ModLockOp_ColorPreview_Lock",			_modLockOption_ColorPreview_Lock);
			EditorPrefs.SetBool("AnyPortrait_ModLockOp_ColorPreview_Unlock",		_modLockOption_ColorPreview_Unlock);
			EditorPrefs.SetBool("AnyPortrait_ModLockOp_BonePreview_Lock",			_modLockOption_BonePreview_Lock);
			EditorPrefs.SetBool("AnyPortrait_ModLockOp_BonePreview_Unlock",			_modLockOption_BonePreview_Unlock);
			//EditorPrefs.SetBool("AnyPortrait_ModLockOp_MeshPreview_Lock",			_modLockOption_MeshPreview_Lock);
			//EditorPrefs.SetBool("AnyPortrait_ModLockOp_MeshPreview_Unlock",			_modLockOption_MeshPreview_Unlock);
			EditorPrefs.SetBool("AnyPortrait_ModLockOp_ModListUI_Lock",				_modLockOption_ModListUI_Lock);
			EditorPrefs.SetBool("AnyPortrait_ModLockOp_ModListUI_Unlock",			_modLockOption_ModListUI_Unlock);

			//SaveColorPref("AnyPortrait_ModLockOp_MeshPreviewColor", _modLockOption_MeshPreviewColor);
			SaveColorPref("AnyPortrait_ModLockOp_BonePreviewColor", _modLockOption_BonePreviewColor);
			//public Color _modLockOption_MeshPreviewColor = new Color(1.0f, 0.45f, 0.1f, 0.8f);
			//public Color _modLockOption_BonePreviewColor = new Color(1.0f, 0.8f, 0.1f, 0.8f);

			EditorPrefs.SetInt("AnyPortrait_LastCheckLiveVersion_Day", _lastCheckLiveVersion_Day);
			EditorPrefs.SetInt("AnyPortrait_LastCheckLiveVersion_Month", _lastCheckLiveVersion_Month);
			EditorPrefs.SetString("AnyPortrait_LastCheckLiveVersion", _currentLiveVersion);
			EditorPrefs.SetBool("AnyPortrait_CheckLiveVersionEnabled", _isCheckLiveVersion_Option);

			EditorPrefs.SetBool("AnyPortrait_IsVersionCheckIgnored", _isVersionNoticeIgnored);
			EditorPrefs.SetInt("AnyPortrait_VersionCheckIgnored_Year", _versionNoticeIvnored_Year);
			EditorPrefs.SetInt("AnyPortrait_VersionCheckIgnored_Month", _versionNoticeIvnored_Month);
			EditorPrefs.SetInt("AnyPortrait_VersionCheckIgnored_Day", _versionNoticeIvnored_Day);


			EditorPrefs.SetFloat("AnyPortrait_MeshTRSOption_MirrorOffset", _meshTRSOption_MirrorOffset);
			EditorPrefs.SetBool("AnyPortrait_MeshTRSOption_MirrorAddVertOnRuler", _meshTRSOption_MirrorSnapVertOnRuler);
			EditorPrefs.SetBool("AnyPortrait_MeshTRSOption_MirrorRemoved", _meshTRSOption_MirrorRemoved);
			
			EditorPrefs.SetFloat("AnyPortrait_MeshAutoGenOption_AlphaCutOff", _meshAutoGenOption_AlphaCutOff);
			EditorPrefs.SetInt("AnyPortrait_MeshAutoGenOption_GridSize", _meshAutoGenOption_GridDivide);
			EditorPrefs.SetInt("AnyPortrait_MeshAutoGenOption_Margin", _meshAutoGenOption_Margin);
			EditorPrefs.SetInt("AnyPortrait_MeshAutoGenOption_NumPoint_QuadX", _meshAutoGenOption_numControlPoint_ComplexQuad_X);
			EditorPrefs.SetInt("AnyPortrait_MeshAutoGenOption_NumPoint_QuadY", _meshAutoGenOption_numControlPoint_ComplexQuad_Y);
			EditorPrefs.SetInt("AnyPortrait_MeshAutoGenOption_NumPoint_Circle", _meshAutoGenOption_numControlPoint_CircleRing);
			EditorPrefs.SetBool("AnyPortrait_MeshAutoGenOption_IsLockAxis", _meshAutoGenOption_IsLockAxis);
			

			EditorPrefs.SetBool("AnyPortrait_LowCPUOption", _isLowCPUOption);

			EditorPrefs.SetBool("AnyPortrait_SelectionLockOption_RiggingPhysics", _isSelectionLockOption_RiggingPhysics);
			EditorPrefs.SetBool("AnyPortrait_SelectionLockOption_Morph", _isSelectionLockOption_Morph);
			EditorPrefs.SetBool("AnyPortrait_SelectionLockOption_Transform", _isSelectionLockOption_Transform);
			EditorPrefs.SetBool("AnyPortrait_SelectionLockOption_ControlParamTimeline", _isSelectionLockOption_ControlParamTimeline);
			
			EditorPrefs.SetInt("AnyPortrait_HierachySortMode", (int)_hierarchySortMode);

			EditorPrefs.SetBool("AnyPortrait_AmbientCorrectionOption", _isAmbientCorrectionOption);

			EditorPrefs.SetBool("AnyPortrait_AutoSwitchControllerTab_Mod", _isAutoSwitchControllerTab_Mod);
			EditorPrefs.SetBool("AnyPortrait_AutoSwitchControllerTab_Anim", _isAutoSwitchControllerTab_Anim);

			EditorPrefs.SetBool("AnyPortrait_RestoreTempMeshVisbility", _isRestoreTempMeshVisibilityWhenTackEnded);


			EditorPrefs.SetBool("AnyPortrait_RigView_WeightOnly", _rigViewOption_WeightOnly);
			EditorPrefs.SetBool("AnyPortrait_RigView_BoneColor", _rigViewOption_BoneColor);
			EditorPrefs.SetBool("AnyPortrait_RigView_CircleVert", _rigViewOption_CircleVert);

			EditorPrefs.SetBool("AnyPortrait_RigOption_ColorLikeParent", _rigOption_NewChildBoneColorIsLikeParent);
			
			
			//_localization.SetLanguage(_language);

			apGL.SetToneOption(_colorOption_OnionToneColor, _onionOption_OutlineThickness, _onionOption_IsOutlineRender, _onionOption_PosOffsetX, _onionOption_PosOffsetY, _colorOption_OnionBoneColor);
		}

		public void LoadEditorPref()
		{
			//Debug.Log("Load Editor Pref");

			_hierarchyFilter = (HIERARCHY_FILTER)EditorPrefs.GetInt("AnyPortrait_HierarchyFilter", (int)HIERARCHY_FILTER.All);

			if (_selection != null)
			{
				Select._rigEdit_isAutoNormalize = EditorPrefs.GetBool("AnyPortrait_IsAutoNormalize", Select._rigEdit_isAutoNormalize);
				//Select._rigEdit_isBoneColorView = EditorPrefs.GetBool("AnyPortrait_IsBoneColorView", Select._rigEdit_isBoneColorView);
				//Select._rigEdit_viewMode = (apSelection.RIGGING_EDIT_VIEW_MODE)EditorPrefs.GetInt("AnyPortrait_ViewMode", (int)Select._rigEdit_viewMode);
			}

			_language = (LANGUAGE)EditorPrefs.GetInt("AnyPortrait_Language", (int)LANGUAGE.English);

			_colorOption_Background = LoadColorPref("AnyPortrait_Color_Backgroud", new Color(0.2f, 0.2f, 0.2f, 1.0f));
			_colorOption_GridCenter = LoadColorPref("AnyPortrait_Color_GridCenter", new Color(0.7f, 0.7f, 0.3f, 1.0f));
			_colorOption_Grid = LoadColorPref("AnyPortrait_Color_Grid", new Color(0.3f, 0.3f, 0.3f, 1.0f));

			_colorOption_MeshEdge = LoadColorPref("AnyPortrait_Color_MeshEdge", new Color(1.0f, 0.5f, 0.0f, 0.9f));
			_colorOption_MeshHiddenEdge = LoadColorPref("AnyPortrait_Color_MeshHiddenEdge", new Color(1.0f, 1.0f, 0.0f, 0.7f));
			_colorOption_Outline = LoadColorPref("AnyPortrait_Color_Outline", new Color(0.0f, 0.5f, 1.0f, 0.7f));
			_colorOption_TransformBorder = LoadColorPref("AnyPortrait_Color_TFBorder", new Color(0.0f, 1.0f, 1.0f, 1.0f));

			_colorOption_VertColor_NotSelected = LoadColorPref("AnyPortrait_Color_VertNotSelected", new Color(0.0f, 0.3f, 1.0f, 0.6f));
			_colorOption_VertColor_Selected = LoadColorPref("AnyPortrait_Color_VertSelected", new Color(1.0f, 0.0f, 0.0f, 1.0f));

			_colorOption_GizmoFFDLine = LoadColorPref("AnyPortrait_Color_GizmoFFDLine", new Color(1.0f, 0.5f, 0.2f, 0.9f));
			_colorOption_GizmoFFDInnerLine = LoadColorPref("AnyPortrait_Color_GizmoFFDInnerLine", new Color(1.0f, 0.7f, 0.2f, 0.7f));

			_colorOption_OnionToneColor = LoadColorPref("AnyPortrait_Color_OnionToneColor", DefaultColor_OnionToneColor);
			_colorOption_OnionAnimPrevColor = LoadColorPref("AnyPortrait_Color_OnionAnimPrevColor", DefaultColor_OnionAnimPrevColor);
			_colorOption_OnionAnimNextColor = LoadColorPref("AnyPortrait_Color_OnionAnimNextColor", DefaultColor_OnionAnimNextColor);
			_colorOption_OnionBoneColor = LoadColorPref("AnyPortrait_Color_OnionBoneColor", DefaultColor_OnionBoneColor);
			_colorOption_OnionBonePrevColor = LoadColorPref("AnyPortrait_Color_OnionBonePrevColor", DefaultColor_OnionBonePrevColor);
			_colorOption_OnionBoneNextColor = LoadColorPref("AnyPortrait_Color_OnionBoneNextColor", DefaultColor_OnionBoneNextColor);

			_onionOption_IsOutlineRender = EditorPrefs.GetBool("AnyPortrait_Onion_OutlineRender", true);
			_onionOption_OutlineThickness = EditorPrefs.GetFloat("AnyPortrait_Onion_OutlineThickness", 0.5f);
			_onionOption_IsRenderOnlySelected = EditorPrefs.GetBool("AnyPortrait_Onion_RenderOnlySelected", false);
			_onionOption_IsRenderBehind = EditorPrefs.GetBool("AnyPortrait_Onion_RenderBehind", false);
			_onionOption_IsRenderAnimFrames = EditorPrefs.GetBool("AnyPortrait_Onion_RenderAnimFrames", false);
			_onionOption_PrevRange = EditorPrefs.GetInt("AnyPortrait_Onion_PrevRange", 1);
			_onionOption_NextRange = EditorPrefs.GetInt("AnyPortrait_Onion_NextRange", 1);
			_onionOption_RenderPerFrame = EditorPrefs.GetInt("AnyPortrait_Onion_RenderPerFrame", 1);
			_onionOption_PosOffsetX = EditorPrefs.GetFloat("AnyPortrait_Onion_PosOffsetX", 0.0f);
			_onionOption_PosOffsetY = EditorPrefs.GetFloat("AnyPortrait_Onion_PosOffsetY", 0.0f);
			_onionOption_IKCalculateForce = EditorPrefs.GetBool("AnyPortrait_Onion_IKCalculate", false);
			

			_timelineInfoSortType = (TIMELINE_INFO_SORT)EditorPrefs.GetInt("AnyPortrait_AnimTimelineLayerSort", (int)TIMELINE_INFO_SORT.Registered);

			_captureFrame_PosX = EditorPrefs.GetInt("AnyPortrait_Capture_PosX", 0);
			_captureFrame_PosY = EditorPrefs.GetInt("AnyPortrait_Capture_PosY", 0);
			_captureFrame_SrcWidth = EditorPrefs.GetInt("AnyPortrait_Capture_SrcWidth", 500);
			_captureFrame_SrcHeight = EditorPrefs.GetInt("AnyPortrait_Capture_SrcHeight", 500);
			_captureFrame_DstWidth = EditorPrefs.GetInt("AnyPortrait_Capture_DstWidth", 500);
			_captureFrame_DstHeight = EditorPrefs.GetInt("AnyPortrait_Capture_DstHeight", 500);

			_captureFrame_SpriteUnitWidth = EditorPrefs.GetInt("AnyPortrait_Capture_SpriteUnitWidth", 100);
			_captureFrame_SpriteUnitHeight = EditorPrefs.GetInt("AnyPortrait_Capture_SpriteUnitHeight", 100);
			_captureFrame_SpriteMargin = EditorPrefs.GetInt("AnyPortrait_Capture_SpriteMargin", 0);

			_isShowCaptureFrame = EditorPrefs.GetBool("AnyPortrait_Capture_IsShowFrame", true);
			//_captureFrame_GIFSampleQuality = EditorPrefs.GetInt("AnyPortrait_Capture_GIFSample", 10);
			_captureFrame_GIFQuality = (CAPTURE_GIF_QUALITY)EditorPrefs.GetInt("AnyPortrait_Capture_GIFQuality", (int)CAPTURE_GIF_QUALITY.High);
			_captureFrame_GIFSampleLoopCount = EditorPrefs.GetInt("AnyPortrait_Capture_GIFLoopCount", 1);

			_captureFrame_Color = LoadColorPref("AnyPortrait_Capture_BGColor", Color.black);
			_captureFrame_IsPhysics = EditorPrefs.GetBool("AnyPortrait_Capture_IsPhysics", false);

			_captureSpritePackImageWidth = (CAPTURE_SPRITE_PACK_IMAGE_SIZE)EditorPrefs.GetInt("AnyPortrait_Capture_SpritePackImageWidth", (int)(CAPTURE_SPRITE_PACK_IMAGE_SIZE.s1024));
			_captureSpritePackImageHeight = (CAPTURE_SPRITE_PACK_IMAGE_SIZE)EditorPrefs.GetInt("AnyPortrait_Capture_SpritePackImageHeight", (int)(CAPTURE_SPRITE_PACK_IMAGE_SIZE.s1024));
			_captureSpriteTrimSize = (CAPTURE_SPRITE_TRIM_METHOD)EditorPrefs.GetInt("AnyPortrait_Capture_SpriteTrimSize", (int)(CAPTURE_SPRITE_TRIM_METHOD.Fixed));
			_captureSpriteMeta_XML = EditorPrefs.GetBool("AnyPortrait_Capture_SpriteMeta_XML", false);
			_captureSpriteMeta_JSON = EditorPrefs.GetBool("AnyPortrait_Capture_SpriteMeta_JSON", false);
			_captureSpriteMeta_TXT = EditorPrefs.GetBool("AnyPortrait_Capture_SpriteMeta_TXT", false);
			
			_captureSprite_ScreenPos.x = EditorPrefs.GetFloat("AnyPortrait_Capture_SpriteScreenPosX", 0.0f);
			_captureSprite_ScreenPos.y = EditorPrefs.GetFloat("AnyPortrait_Capture_SpriteScreenPosY", 0.0f);
			_captureSprite_ScreenZoom = EditorPrefs.GetInt("AnyPortrait_Capture_SpriteScreenZoomIndex", ZOOM_INDEX_DEFAULT);
			
			


			_isCaptureAspectRatioFixed = EditorPrefs.GetBool("AnyPortrait_Capture_IsAspectRatioFixed", true);

			_boneGUIRenderMode = (BONE_RENDER_MODE)EditorPrefs.GetInt("AnyPortrait_BoneRenderMode", (int)BONE_RENDER_MODE.Render);

			_guiOption_isFPSVisible = EditorPrefs.GetBool("AnyPortrait_GUI_FPSVisible", true);
			_guiOption_isStatisticsVisible = EditorPrefs.GetBool("AnyPortrait_GUI_StatisticsVisible", false);

			_backupOption_IsAutoSave =		EditorPrefs.GetBool("AnyPortrait_AutoBackup_Enabled", true);
			_backupOption_BaseFolderName =	EditorPrefs.GetString("AnyPortrait_AutoBackup_Path", "AnyPortraitBackup");
			_backupOption_Minute =			EditorPrefs.GetInt("AnyPortrait_AutoBackup_Time", 30);
			
			_bonePose_BaseFolderName = EditorPrefs.GetString("AnyPortrait_BonePose_Path", "AnyPortraitBonePose");

			_startScreenOption_IsShowStartup = EditorPrefs.GetBool("AnyPortrait_StartScreen_IsShow", true);
			_startScreenOption_LastMonth = EditorPrefs.GetInt("AnyPortrait_StartScreen_LastMonth", 0);
			_startScreenOption_LastDay = EditorPrefs.GetInt("AnyPortrait_StartScreen_LastDay", 0);

			_updateLogScreen_LastVersion = EditorPrefs.GetInt("AnyPortrait_UpdateLogScreen_LastVersion", 0);

			_isBakeColorSpaceToGamma = EditorPrefs.GetBool("AnyPortrait_IsBakeColorSpace_ToGamma", true);
			_isUseSRP = EditorPrefs.GetBool("AnyPortrait_IsUseLWRPShader", false);

			_modLockOption_CalculateIfNotAddedOther = EditorPrefs.GetBool("AnyPortrait_ModLockOp_CalculateIfNotAddedOther",	false);			
			_modLockOption_ColorPreview_Lock =		EditorPrefs.GetBool("AnyPortrait_ModLockOp_ColorPreview_Lock",		false);
			_modLockOption_ColorPreview_Unlock =	EditorPrefs.GetBool("AnyPortrait_ModLockOp_ColorPreview_Unlock",	true);//<< True 기본값
			_modLockOption_BonePreview_Lock =		EditorPrefs.GetBool("AnyPortrait_ModLockOp_BonePreview_Lock",		false);
			_modLockOption_BonePreview_Unlock =		EditorPrefs.GetBool("AnyPortrait_ModLockOp_BonePreview_Unlock",		true);//<< True 기본값
			//_modLockOption_MeshPreview_Lock =		EditorPrefs.GetBool("AnyPortrait_ModLockOp_MeshPreview_Lock",		false);
			//_modLockOption_MeshPreview_Unlock =		EditorPrefs.GetBool("AnyPortrait_ModLockOp_MeshPreview_Unlock",		false);
			_modLockOption_ModListUI_Lock =			EditorPrefs.GetBool("AnyPortrait_ModLockOp_ModListUI_Lock",			false);
			_modLockOption_ModListUI_Unlock =		EditorPrefs.GetBool("AnyPortrait_ModLockOp_ModListUI_Unlock",		false);

			//_modLockOption_MeshPreviewColor = LoadColorPref("AnyPortrait_ModLockOp_MeshPreviewColor", DefauleColor_ModLockOpt_MeshPreview);
			_modLockOption_BonePreviewColor = LoadColorPref("AnyPortrait_ModLockOp_BonePreviewColor", DefauleColor_ModLockOpt_BonePreview);

			_lastCheckLiveVersion_Day = EditorPrefs.GetInt("AnyPortrait_LastCheckLiveVersion_Day", 0);
			_lastCheckLiveVersion_Month = EditorPrefs.GetInt("AnyPortrait_LastCheckLiveVersion_Month", 0);
			_currentLiveVersion = EditorPrefs.GetString("AnyPortrait_LastCheckLiveVersion", "");
			_isCheckLiveVersion_Option = EditorPrefs.GetBool("AnyPortrait_CheckLiveVersionEnabled", true);

			_isVersionNoticeIgnored = EditorPrefs.GetBool("AnyPortrait_IsVersionCheckIgnored", false);
			_versionNoticeIvnored_Year = EditorPrefs.GetInt("AnyPortrait_VersionCheckIgnored_Year", 0);
			_versionNoticeIvnored_Month = EditorPrefs.GetInt("AnyPortrait_VersionCheckIgnored_Month", 0);
			_versionNoticeIvnored_Day = EditorPrefs.GetInt("AnyPortrait_VersionCheckIgnored_Day", 0);
			
			_meshTRSOption_MirrorOffset = EditorPrefs.GetFloat("AnyPortrait_MeshTRSOption_MirrorOffset", 0.5f);
			_meshTRSOption_MirrorSnapVertOnRuler = EditorPrefs.GetBool("AnyPortrait_MeshTRSOption_MirrorAddVertOnRuler", false);
			_meshTRSOption_MirrorRemoved = EditorPrefs.GetBool("AnyPortrait_MeshTRSOption_MirrorRemoved", false);
			_meshAutoGenOption_AlphaCutOff = EditorPrefs.GetFloat("AnyPortrait_MeshAutoGenOption_AlphaCutOff", 0.02f);
			_meshAutoGenOption_GridDivide = EditorPrefs.GetInt("AnyPortrait_MeshAutoGenOption_GridSize", 2);
			_meshAutoGenOption_Margin = EditorPrefs.GetInt("AnyPortrait_MeshAutoGenOption_Margin", 2);
			_meshAutoGenOption_numControlPoint_ComplexQuad_X = EditorPrefs.GetInt("AnyPortrait_MeshAutoGenOption_NumPoint_QuadX", 3);
			_meshAutoGenOption_numControlPoint_ComplexQuad_Y = EditorPrefs.GetInt("AnyPortrait_MeshAutoGenOption_NumPoint_QuadY", 3);
			_meshAutoGenOption_numControlPoint_CircleRing = EditorPrefs.GetInt("AnyPortrait_MeshAutoGenOption_NumPoint_Circle", 4);
			_meshAutoGenOption_IsLockAxis = EditorPrefs.GetBool("AnyPortrait_MeshAutoGenOption_IsLockAxis", false);

			_isLowCPUOption = EditorPrefs.GetBool("AnyPortrait_LowCPUOption", false);

			_isSelectionLockOption_RiggingPhysics = EditorPrefs.GetBool("AnyPortrait_SelectionLockOption_RiggingPhysics", true);
			_isSelectionLockOption_Morph = EditorPrefs.GetBool("AnyPortrait_SelectionLockOption_Morph", true);
			_isSelectionLockOption_Transform = EditorPrefs.GetBool("AnyPortrait_SelectionLockOption_Transform", true);
			_isSelectionLockOption_ControlParamTimeline = EditorPrefs.GetBool("AnyPortrait_SelectionLockOption_ControlParamTimeline", true);
			
			_hierarchySortMode = (HIERARCHY_SORT_MODE)EditorPrefs.GetInt("AnyPortrait_HierachySortMode", (int)HIERARCHY_SORT_MODE.RegOrder);

			_isAmbientCorrectionOption = EditorPrefs.GetBool("AnyPortrait_AmbientCorrectionOption", true);

			_isAutoSwitchControllerTab_Mod = EditorPrefs.GetBool("AnyPortrait_AutoSwitchControllerTab_Mod", true);
			_isAutoSwitchControllerTab_Anim = EditorPrefs.GetBool("AnyPortrait_AutoSwitchControllerTab_Anim", false);

			_isRestoreTempMeshVisibilityWhenTackEnded = EditorPrefs.GetBool("AnyPortrait_RestoreTempMeshVisbility", true);

			_rigViewOption_WeightOnly = EditorPrefs.GetBool("AnyPortrait_RigView_WeightOnly", false);
			_rigViewOption_BoneColor = EditorPrefs.GetBool("AnyPortrait_RigView_BoneColor", true);
			_rigViewOption_CircleVert = EditorPrefs.GetBool("AnyPortrait_RigView_CircleVert", false);

			_rigOption_NewChildBoneColorIsLikeParent = EditorPrefs.GetBool("AnyPortrait_RigOption_ColorLikeParent", true);


			apGL.SetToneOption(_colorOption_OnionToneColor, _onionOption_OutlineThickness, _onionOption_IsOutlineRender, _onionOption_PosOffsetX, _onionOption_PosOffsetY, _colorOption_OnionBoneColor);
		}

		private void SaveColorPref(string label, Color color)
		{
			EditorPrefs.SetFloat(label + "_R", color.r);
			EditorPrefs.SetFloat(label + "_G", color.g);
			EditorPrefs.SetFloat(label + "_B", color.b);
			EditorPrefs.SetFloat(label + "_A", color.a);
		}

		private Color LoadColorPref(string label, Color defaultValue)
		{
			Color result = Color.black;
			result.r = EditorPrefs.GetFloat(label + "_R", defaultValue.r);
			result.g = EditorPrefs.GetFloat(label + "_G", defaultValue.g);
			result.b = EditorPrefs.GetFloat(label + "_B", defaultValue.b);
			result.a = EditorPrefs.GetFloat(label + "_A", defaultValue.a);
			return result;
		}

		public void RestoreEditorPref()
		{
			_language = LANGUAGE.English;

			//색상 옵션
			//_colorOption_Background = new Color(0.2f, 0.2f, 0.2f, 1.0f);
			//_colorOption_GridCenter = new Color(0.7f, 0.7f, 0.3f, 1.0f);
			//_colorOption_Grid = new Color(0.3f, 0.3f, 0.3f, 1.0f);

			//_colorOption_MeshEdge = new Color(1.0f, 0.5f, 0.0f, 0.9f);
			//_colorOption_MeshHiddenEdge = new Color(1.0f, 1.0f, 0.0f, 0.7f);
			//_colorOption_Outline = new Color(0.0f, 0.5f, 1.0f, 0.7f);
			//_colorOption_TransformBorder = new Color(0.0f, 1.0f, 1.0f, 1.0f);

			//_colorOption_VertColor_NotSelected = new Color(0.0f, 0.3f, 1.0f, 0.6f);
			//_colorOption_VertColor_Selected = new Color(1.0f, 0.0f, 0.0f, 1.0f);

			//_colorOption_GizmoFFDLine = new Color(1.0f, 0.5f, 0.2f, 0.9f);
			//_colorOption_GizmoFFDInnerLine = new Color(1.0f, 0.7f, 0.2f, 0.7f);

			//_colorOption_AtlasBorder = new Color(0.0f, 1.0f, 1.0f, 0.5f);

			_colorOption_Background = DefaultColor_Background;
			_colorOption_GridCenter = DefaultColor_GridCenter;
			_colorOption_Grid = DefaultColor_Grid;

			_colorOption_MeshEdge = DefaultColor_MeshEdge;
			_colorOption_MeshHiddenEdge = DefaultColor_MeshHiddenEdge;
			_colorOption_Outline = DefaultColor_Outline;
			_colorOption_TransformBorder = DefaultColor_TransformBorder;

			_colorOption_VertColor_NotSelected = DefaultColor_VertNotSelected;
			_colorOption_VertColor_Selected = DefaultColor_VertSelected;

			_colorOption_GizmoFFDLine = DefaultColor_GizmoFFDLine;
			_colorOption_GizmoFFDInnerLine = DefaultColor_GizmoFFDInnerLine;
			//_colorOption_OnionToneColor = DefaultColor_OnionToneColor;//<<이 값은 Onion Setting에서 설정하는 것으로 변경

			_colorOption_AtlasBorder = DefaultColor_AtlasBorder;

			_guiOption_isFPSVisible = true;
			_guiOption_isStatisticsVisible = false;

			

			_backupOption_IsAutoSave = true;//자동 백업을 지원하는가
			_backupOption_BaseFolderName = "AnyPortraitBackup";//폴더를 지정해야한다. (프로젝트 폴더 기준 + 씬이름+에셋)
			_backupOption_Minute = 30;//기본은 30분마다 한번씩 저장한다.

			_bonePose_BaseFolderName = "AnyPortraitBonePose";

			_startScreenOption_IsShowStartup = true;
			_startScreenOption_LastMonth = 0;//Restore하면 다음에 실행할때 다시 나오도록 하자
			_startScreenOption_LastDay = 0;
			
			_isLowCPUOption = false;

			_isSelectionLockOption_RiggingPhysics = true;
			_isSelectionLockOption_Morph = true;
			_isSelectionLockOption_Transform = true;
			_isSelectionLockOption_ControlParamTimeline = true;
			
			_isAmbientCorrectionOption = true;

			_isAutoSwitchControllerTab_Mod = true;
			_isAutoSwitchControllerTab_Anim = false;

			_isRestoreTempMeshVisibilityWhenTackEnded = true;

			_rigViewOption_WeightOnly = false;
			_rigViewOption_BoneColor = true;
			_rigViewOption_CircleVert = false;

			_rigOption_NewChildBoneColorIsLikeParent = true;

			SaveEditorPref();
		}

		public static Color DefaultColor_Background { get { return new Color(0.2f, 0.2f, 0.2f, 1.0f); } }
		public static Color DefaultColor_GridCenter { get { return new Color(0.7f, 0.7f, 0.3f, 1.0f); } }
		public static Color DefaultColor_Grid { get { return new Color(0.3f, 0.3f, 0.3f, 1.0f); } }

		public static Color DefaultColor_MeshEdge { get { return new Color(1.0f, 0.5f, 0.0f, 0.9f); } }
		public static Color DefaultColor_MeshHiddenEdge { get { return new Color(1.0f, 1.0f, 0.0f, 0.7f); } }
		public static Color DefaultColor_Outline { get { return new Color(0.0f, 0.5f, 1.0f, 0.7f); } }
		public static Color DefaultColor_TransformBorder { get { return new Color(0.0f, 1.0f, 1.0f, 1.0f); } }

		public static Color DefaultColor_VertNotSelected { get { return new Color(0.0f, 0.3f, 1.0f, 0.6f); } }
		public static Color DefaultColor_VertSelected { get { return new Color(1.0f, 0.0f, 0.0f, 1.0f); } }

		public static Color DefaultColor_GizmoFFDLine { get { return new Color(1.0f, 0.5f, 0.2f, 0.9f); } }
		public static Color DefaultColor_GizmoFFDInnerLine { get { return new Color(1.0f, 0.7f, 0.2f, 0.7f); } }

		public static Color DefaultColor_OnionToneColor { get { return new Color(0.1f, 0.43f, 0.5f, 0.7f); } }
		public static Color DefaultColor_OnionAnimPrevColor { get { return new Color(0.5f, 0.2f, 0.1f, 0.7f); } }
		public static Color DefaultColor_OnionAnimNextColor { get { return new Color(0.1f, 0.5f, 0.2f, 0.7f); } }

		public static Color DefaultColor_OnionBoneColor { get { return new Color(0.4f, 1.0f, 1.0f, 0.9f); } }
		public static Color DefaultColor_OnionBonePrevColor { get { return new Color(1.0f, 0.6f, 0.3f, 0.9f); } }
		public static Color DefaultColor_OnionBoneNextColor { get { return new Color(0.3f, 1.0f, 0.6f, 0.9f); } }

		public static Color DefaultColor_AtlasBorder { get { return new Color(0.0f, 1.0f, 1.0f, 0.5f); } }

		public static Color DefauleColor_ModLockOpt_MeshPreview { get { return new Color(1.0f, 0.45f, 0.1f, 0.8f); } }
		public static Color DefauleColor_ModLockOpt_BonePreview { get { return new Color(1.0f, 0.8f, 0.1f, 0.8f); } }

		//-------------------------------------------------------------------------------
		public string GetText(TEXT textType)
		{
			return Localization.GetText(textType);
		}


		public string GetTextFormat(TEXT textType, params object[] paramList)
		{
			return string.Format(Localization.GetText(textType), paramList);
		}


		public string GetUIWord(UIWORD uiWordType)
		{
			return Localization.GetUIWord(uiWordType);
		}


		public string GetUIWordFormat(UIWORD uiWordType, params object[] paramList)
		{
			return string.Format(Localization.GetUIWord(uiWordType), paramList);
		}

		// 화면 캡쳐 이벤트 요청과 처리
		//-----------------------------------------------------------------------
		public void ScreenCaptureRequest(apScreenCaptureRequest screenCaptureRequest)
		{
			
				
			
#if UNITY_EDITOR_OSX
			//OSX에선 딜레이를 줘야한다.
			_isScreenCaptureRequest_OSXReady = true;
			_isScreenCaptureRequest = false;
			_screenCaptureRequest_Count = SCREEN_CAPTURE_REQUEST_OSX_COUNT;
#else
			//Window에선 바로 캡쳐가능
			_isScreenCaptureRequest = true;
#endif
			
			_screenCaptureRequest = screenCaptureRequest;
		}

		//GUI에서 처리할 것
		private void ProcessScreenCapture()
		{
			_isScreenCaptureRequest = false;

			if(_screenCaptureRequest == null)
			{
				return;
			}

			apScreenCaptureRequest curRequest = _screenCaptureRequest;
			_screenCaptureRequest = null;//<<이건 일단 null

			//유효성 체크
			bool isValid = true;
			if (curRequest._editor != this ||
				Select.SelectionType != apSelection.SELECTION_TYPE.Overall ||
				Select.RootUnit == null ||
				curRequest._meshGroup == null ||
				Select.RootUnit._childMeshGroup != curRequest._meshGroup ||
				curRequest._funcCaptureResult == null ||
				_portrait == null)
			{
				isValid = false;
				
			}

			Texture2D result = null;
			if(isValid)
			{
				apMeshGroup targetMeshGroup = curRequest._meshGroup;
				apAnimClip targetAnimClip = curRequest._animClip;
				

				bool prevPhysics = _portrait._isPhysicsPlay_Editor;
				

				//업데이트를 한다.
				if(curRequest._isAnimClipRequest &&
					targetAnimClip != null)
				{
					//Debug.LogWarning("Capture Frame : " + curRequest._animFrame);
					//_portrait._isPhysicsPlay_Editor = false;//<<물리 금지
					//변경 : 옵션에 따라 바뀐다.
					_portrait._isPhysicsPlay_Editor = curRequest._isPhysics;
					if(targetAnimClip._targetMeshGroup != null)
					{
						targetAnimClip._targetMeshGroup.SetBoneIKEnabled(true, true);
					}
					targetAnimClip.SetFrame_Editor(curRequest._animFrame);

					
				}
				else
				{
					targetMeshGroup.SetBoneIKEnabled(true, true);
					targetMeshGroup.RefreshForce();
					
				}

				result = Exporter.RenderToTexture(
													targetMeshGroup,
													curRequest._winPosX,
													curRequest._winPosY,
													curRequest._srcSizeWidth,
													curRequest._srcSizeHeight,
													curRequest._dstSizeWidth,
													curRequest._dstSizeHeight,
													curRequest._clearColor
													);

				//물리 복구
				_portrait._isPhysicsPlay_Editor = prevPhysics;
				if (curRequest._isAnimClipRequest &&
					targetAnimClip != null)
				{
					if (targetAnimClip._targetMeshGroup != null)
					{
						targetAnimClip._targetMeshGroup.SetBoneIKEnabled(false, false);
					}
				}
				else
				{
					targetMeshGroup.SetBoneIKEnabled(false, false);
				}

				//리! 턴!
				try
				{
					if (result != null)
					{
						curRequest._funcCaptureResult(
													true,
													result,
													curRequest._iProcessStep,
													curRequest._filePath,
													curRequest._loadKey);
					}
					else
					{
						curRequest._funcCaptureResult(
													false,
													null,
													curRequest._iProcessStep,
													curRequest._filePath,
													curRequest._loadKey);
					}

					result = null;
				}
				catch (Exception ex)
				{
					Debug.LogError("Capture Exception : " + ex);

					if (result != null)
					{
						UnityEngine.Object.DestroyImmediate(result);
					}
				}
			}
			else
			{
				if(result != null)
				{
					UnityEngine.Object.DestroyImmediate(result);
				}
				
				//처리가 실패했다.
				if(curRequest != null &&
					curRequest._funcCaptureResult != null)
				{
					//리! 턴!
					try
					{
						curRequest._funcCaptureResult(
							false,
							null,
							curRequest._iProcessStep,
							curRequest._filePath,
							curRequest._loadKey);
					}
					catch(Exception ex)
					{
						Debug.LogError("Capture Exception : " + ex);
					}
				}
			}
				
			//처리 끝!
			

		}

		// Modifier Lock 설정값에 대한 요청
		public bool GetModLockOption_CalculateIfNotAddedOther(apSelection.EX_EDIT exEdit)
		{
			if(exEdit == apSelection.EX_EDIT.General_Edit)
			{
				return _modLockOption_CalculateIfNotAddedOther;
			}
			return false;
		}

		public bool GetModLockOption_ColorPreview(apSelection.EX_EDIT exEdit)
		{
			if(exEdit == apSelection.EX_EDIT.ExOnly_Edit)		{ return _modLockOption_ColorPreview_Lock; }
			else if(exEdit == apSelection.EX_EDIT.General_Edit)	{ return _modLockOption_ColorPreview_Unlock; }
			return false;
		}

		
		public bool GetModLockOption_BonePreview(apSelection.EX_EDIT exEdit)
		{
			if(exEdit == apSelection.EX_EDIT.ExOnly_Edit)		{ return _modLockOption_BonePreview_Lock; }
			else if(exEdit == apSelection.EX_EDIT.General_Edit)	{ return _modLockOption_BonePreview_Unlock; }
			return false;
		}

		//public bool GetModLockOption_MeshPreview(apSelection.EX_EDIT exEdit)
		//{
		//	if(exEdit == apSelection.EX_EDIT.ExOnly_Edit)		{ return _modLockOption_MeshPreview_Lock; }
		//	else if(exEdit == apSelection.EX_EDIT.General_Edit)	{ return _modLockOption_MeshPreview_Unlock; }
		//	return false;
		//}

		public bool GetModLockOption_ListUI(apSelection.EX_EDIT exEdit)
		{
			if(exEdit == apSelection.EX_EDIT.ExOnly_Edit)		{ return _modLockOption_ModListUI_Lock; }
			else if(exEdit == apSelection.EX_EDIT.General_Edit)	{ return _modLockOption_ModListUI_Unlock; }
			return false;
		}

		//public Color GetModLockOption_MeshPreviewColor()
		//{
		//	return _modLockOption_MeshPreviewColor;
		//}

		public Color GetModLockOption_BonePreviewColor()
		{
			return _modLockOption_BonePreviewColor;
		}
		//------------------------------------------------------------------------------------------------
		// 최신 버전 체크하기
		//------------------------------------------------------------------------------------------------
		private void CheckCurrentLiveVersion()
		{
			if(!_isCheckLiveVersion)
			{
				_isCheckLiveVersion = true;

				if(!_isCheckLiveVersion_Option)
				{
					//만약 버전 체크 옵션이 꺼져있으면 처리하지 않는다.
					//Debug.Log("버전 체크를 하지 않는다.");
					return;
				}
				//else
				//{
				//	Debug.Log("버전 체크 시작");
				//}
				//날짜 확인후 요청
				if(string.IsNullOrEmpty(_currentLiveVersion)
					|| _lastCheckLiveVersion_Day != DateTime.Now.Day
					|| _lastCheckLiveVersion_Month != DateTime.Now.Month
					//|| true//<<테스트
					)
				{
					//날짜가 다르거나, 버전이 없으면 요청
					apEditorUtil.RequestCurrentVersion(OnGetCurrentLiveVersion);
				}
				else
				{
					CheckCurrentVersionAndNotification(false);
				}
			}
		}

		private void OnGetCurrentLiveVersion(bool isSuccess, string strResult)
		{
			//Debug.Log("OnGetCurrentLiveVersion : " + isSuccess + " / " + strResult);
			if(isSuccess)
			{
				_currentLiveVersion = strResult;
				//업데이트 버전이 갱신되었다.
				//저장을 하자
				_lastCheckLiveVersion_Day = System.DateTime.Now.Day;
				_lastCheckLiveVersion_Month = System.DateTime.Now.Month;

				SaveEditorPref();

				CheckCurrentVersionAndNotification(true);

			}
		}

		private void CheckCurrentVersionAndNotification(bool isCalledByWebResponse)
		{
			if (string.IsNullOrEmpty(_currentLiveVersion))
			{
				return;
			}
			string strNumeric = "";
			string strText = "";
			for (int i = 0; i < _currentLiveVersion.Length; i++)
			{
				strText = _currentLiveVersion.Substring(i, 1);
				if (strText == "0" || strText == "1" || strText == "2" || strText == "3"
					|| strText == "4" || strText == "5" || strText == "6" || strText == "7"
					|| strText == "8" || strText == "9")
				{
					strNumeric += strText;
				}
			}
			if (int.Parse(strNumeric) <= apVersion.I.APP_VERSION_INT)
			{
				//Debug.Log("Last Version : " + _currentLiveVersion);
			}
			else
			{
				Notification("A new version has been updated! [v" + apVersion.I.APP_VERSION_SHORT + " >> v" + _currentLiveVersion + "]", false, false);

				//추가 : 웹에서 로드했으며, 알람을 줄 수 있는 상황이라면
				if(isCalledByWebResponse && Localization.IsLoaded)
				{
					//알람을 해야하는지 테스트
					bool isNoticeEnabled = false;
					if(!_isVersionNoticeIgnored)
					{
						//일주일간 무시하기 조건이 없다.
						isNoticeEnabled = true;
					}
					else
					{
						if(_versionNoticeIvnored_Year == 0 || _versionNoticeIvnored_Month == 0 || _versionNoticeIvnored_Day == 0)
						{
							//날짜가 잘못 되어 있다.
							isNoticeEnabled = true;
						}
						else
						{
							DateTime ignoredDay = new DateTime(_versionNoticeIvnored_Year, _versionNoticeIvnored_Month, _versionNoticeIvnored_Day);
							int diffDays = (int)ignoredDay.Subtract(DateTime.Now).TotalDays;
							if(diffDays < 0)
							{
								isNoticeEnabled = true;
							}
							//else
							//{
							//	Debug.Log("날짜가 아직 되지 않았다. [오늘 : " + DateTime.Now.ToShortDateString() + "] - [목표 날짜:" + ignoredDay.ToShortDateString() + "] (" + diffDays + ")");
							//}
						}
					}
					if (isNoticeEnabled)
					{
						//일주일간 무시하기 옵션이 사라진다.
						_isVersionNoticeIgnored = false;

						_versionNoticeIvnored_Year = DateTime.Now.Year;
						_versionNoticeIvnored_Month = DateTime.Now.Month;
						_versionNoticeIvnored_Day = DateTime.Now.Day;

						SaveEditorPref();


						//바로 에셋 스토어를 열 수 있게 하자
						int iBtn = EditorUtility.DisplayDialogComplex(GetText(TEXT.DLG_NewVersion_Title),
																		GetText(TEXT.DLG_NewVersion_Body),
																		GetText(TEXT.DLG_NewVersion_OpenAssetStore),
																		GetText(TEXT.DLG_NewVersion_Ignore),
																		GetText(TEXT.Cancel)
																		);

						if (iBtn == 0)
						{
							apEditorUtil.OpenAssetStorePage();
						}
						else if (iBtn == 1)
						{
							//일주일 동안 열지 않음

							_isVersionNoticeIgnored = true;

							DateTime dayAfter7 = DateTime.Now;
							dayAfter7 = dayAfter7.AddDays(7);

							_versionNoticeIvnored_Year = dayAfter7.Year;
							_versionNoticeIvnored_Month = dayAfter7.Month;
							_versionNoticeIvnored_Day = dayAfter7.Day;

							//Debug.Log("7일 동안 알람 안보기 : " + _versionNoticeIvnored_Year + "-" + _versionNoticeIvnored_Month + "-" + _versionNoticeIvnored_Day);
	
							SaveEditorPref();
						}
					}
					
					

				}
				//GetText()
			}
		}


		//----------------------------------------------------------------------------
		// Inspector를 통해서 여는 경우
		//----------------------------------------------------------------------------
		public void SetPortraitByInspector(apPortrait targetPortrait, bool isBakeAndClose)
		{
			try
			{
				if (_portrait != targetPortrait && targetPortrait != null)
				{
					if (targetPortrait._isOptimizedPortrait)
					{
						//Optimized Portrait는 편집이 불가능하다
						return;
					}

					CheckEditorResources();

					_portrait = targetPortrait;//선택!

					Controller.InitTmpValues();
					_selection.SetPortrait(_portrait);

					//Portrait의 레퍼런스들을 연결해주자
					Controller.PortraitReadyToEdit();


					SyncHierarchyOrders();

					_hierarchy.ResetAllUnits();
					_hierarchy_MeshGroup.ResetSubUnits();
					_hierarchy_AnimClip.ResetSubUnits();

					//시작은 RootUnit
					_selection.SetOverallDefault();

					OnAnyObjectAddedOrRemoved();

					if (isBakeAndClose)
					{
						apBakeResult bakeResult = Controller.Bake();


						if (bakeResult.NumUnlinkedExternalObject > 0)
						{
							EditorUtility.DisplayDialog(GetText(TEXT.BakeWarning_Title),
								GetTextFormat(TEXT.BakeWarning_Body, bakeResult.NumUnlinkedExternalObject),
								GetText(TEXT.Okay));
						}

						CloseEditor();
					}
				}
			}
			catch(Exception ex)
			{
				Debug.LogError("AnyPortrait : Open 2D Editor Failed : " + ex);
			}
		}


		//-------------------------------------------------------------------------------------
		// GUIContent들 초기화
		// UI가 추가될때마다 이 함수에 추가해야한다.
		//-------------------------------------------------------------------------------------
		private void ResetGUIContents()
		{
			_guiContent_Notification = null;
			_guiContent_TopBtn_Setting = null;
			_guiContent_TopBtn_Bake = null;
			_guiContent_MainLeftUpper_MakeNewPortrait = null;
			_guiContent_MainLeftUpper_RefreshToLoad = null;
			_guiContent_MainLeftUpper_LoadBackupFile = null;
			_guiContent_GUITopTab_Open = null;
			_guiContent_GUITopTab_Folded = null;
			_guiContent_Top_GizmoIcon_Move = null;
			_guiContent_Top_GizmoIcon_Depth = null;
			_guiContent_Top_GizmoIcon_Rotation = null;
			_guiContent_Top_GizmoIcon_Scale = null;
			_guiContent_Top_GizmoIcon_Color = null;
			_guiContent_Top_GizmoIcon_Extra = null;

			//UI가 추가시 여기에 코드를 작성하자
		}
	}

}