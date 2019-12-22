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
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using System.Diagnostics;//자체 타이머를 만들자
#endif


using AnyPortrait;

namespace AnyPortrait
{

	/// <summary>
	/// It is an animation clip created in the editor.
	/// It has the data to play the animation and connects to apAnimPlayUnit to perform the update.
	/// (Since most functions are used in the editor or used in manger classes, it is recommended that you refer to "apAnimPlayUnit" instead of using this class.)
	/// </summary>
	[Serializable]
	public class apAnimClip
	{
		// Members
		//---------------------------------------------
		[SerializeField]
		public int _uniqueID = -1;

		[SerializeField]
		public string _name = "";


		//연결된 객체들
		[NonSerialized]
		public apPortrait _portrait = null;


		//연결된 객체들 - 에디터
		[SerializeField]
		public int _targetMeshGroupID = -1;

		[NonSerialized]
		public apMeshGroup _targetMeshGroup = null;

		[NonSerialized]
		public apOptTransform _targetOptTranform = null;


		public enum LINK_TYPE
		{
			AnimatedModifier = 0,
			//Bone,//<<이게 빠지고 AnimatedModifier에 포함된다. Transform(Animation) Modifier에서 Bone 제어가 가능하다
			ControlParam = 1
		}

		// 애니메이션 기본 정보
		[SerializeField]
		private int _FPS = 30;

		[SerializeField]
		private float _secPerFrame = 1.0f / 30.0f;

		[SerializeField]
		private int _startFrame = 0;
		[SerializeField]
		private int _endFrame = 100;
		[SerializeField]
		private bool _isLoop = false;

		public int FPS { get { return _FPS; } }
		public int StartFrame { get { return _startFrame; } }
		public int EndFrame { get { return _endFrame; } }
		public bool IsLoop { get { return _isLoop; } }



		//Timeline 리스트
		[SerializeField]
		public List<apAnimTimeline> _timelines = new List<apAnimTimeline>();


		private float _tUpdate = 0.0f;
		private float _tUpdateTotal = 0.0f;
		private int _curFrame = 0;
		public float TimePerFrame { get { return _secPerFrame; } }
		public float TotalPlayTime { get { return _tUpdateTotal; } }

		
		private bool _isPlaying = false;

		/// <summary>에디터에서 적용하는 [현재 프레임]</summary>
		public int CurFrame { get { return _curFrame; } }

		/// <summary>
		/// 실행시 정확한 보간이 되는 프레임 (실수형)
		/// 게임 프레임에 동기화된다. (정확한 정수형 프레임 값은 안나온다)
		/// </summary>
		public float CurFrameFloat { get { return _curFrame + (_tUpdate / TimePerFrame); } }

		
		public bool IsPlaying_Editor { get { return _isPlaying; } }
		public bool IsPlaying_Opt
		{
			get
			{
				//return _isPlaying;
				if(_parentPlayUnit == null)
				{
					return false;
				}
				//TODO : Loop가 아닌 경우도 체크해야한다. (RemainPlayTime을 체크)
				return _parentPlayUnit.PlayStatus == apAnimPlayUnit.PLAY_STATUS.Play && !_parentPlayUnit._isPause;
			}
		}

		public bool IsHasValidPlayUnit
		{
			get { return _parentPlayUnit != null && _parentPlayUnit.PlayStatus == apAnimPlayUnit.PLAY_STATUS.Play; }
		}

		public float TimeLength { get { return (float)Mathf.Max(_endFrame - _startFrame, 0) * TimePerFrame; } }

		///// <summary>
		///// 재생된 결과가 반영되는 가중치값
		///// 단순 재생시에는 이 값이 1이지만 Layer, Queue 플레이시 Weight가 바뀌며, 그 값에 따라 데이터가 적용된다.
		///// </summary>
		//private float _playWeight = 1.0f;


		[SerializeField]
		public List<apAnimEvent> _animEvents = new List<apAnimEvent>();

		[NonSerialized]
		public List<apAnimControlParamResult> _controlParamResult = new List<apAnimControlParamResult>();


		//리얼타임에서
		/// <summary>
		/// 리얼타임에서 이 AnimClip을 사용중인 PlayUnit. 이 값은 PlayUnit 생성마다 갱신되며 소유권을 알려준다.
		/// </summary>
		[NonSerialized]
		public apAnimPlayUnit _parentPlayUnit = null;


		//에디터에서
		[NonSerialized]
		public bool _isSelectedInEditor = false;

#if UNITY_EDITOR
		private Stopwatch _stopWatch_Editor = new Stopwatch();
		private float _tDelta_Editor = -1;
#endif

		[SerializeField, NonBackupField]
		public AnimationClip _animationClipForMecanim = null;
		

		//추가 : 배속
		//런타임 + 메카님이 아닌 스크립트에 의한 재생인 경우
		[NonSerialized]
		public float _speedRatio = 1.0f;

		


		// Init
		//---------------------------------------------
		public apAnimClip()
		{

		}


		public void Init(apPortrait portrait, string name, int ID)
		{
			_portrait = portrait;
			_name = name;
			_uniqueID = ID;
			_targetMeshGroupID = -1;
			_targetMeshGroup = null;

			_timelines.Clear();
			_controlParamResult.Clear();

		}

		public void LinkEditor(apPortrait portrait)
		{
			_portrait = portrait;
			_targetMeshGroup = _portrait.GetMeshGroup(_targetMeshGroupID);


			//ID를 등록해주자
			//_portrait.RegistUniqueID_AnimClip(_uniqueID);
			_portrait.RegistUniqueID(apIDManager.TARGET.AnimClip, _uniqueID);

			//v1.1.7 버그 수정
			if(_endFrame <= _startFrame)
			{	
				UnityEngine.Debug.LogError("AnyPortrait : End Frame of Animation Clip [" + _name + "] is not set up normally. End Frame is changed from " + _endFrame + " to " + (_startFrame + 1) + ".\nThis action is temporary, so please modify the frame settings of this Animation Clip.");
				_endFrame = _startFrame + 1;
			}

			//TODO : 멤버 추가시 값을 추가하자
			for (int i = 0; i < _timelines.Count; i++)
			{
				_timelines[i].Link(this);
			}
		}


		public void RemoveUnlinkedTimeline()
		{
			_timelines.RemoveAll(delegate (apAnimTimeline a)
			{
				if (a._linkType == LINK_TYPE.AnimatedModifier)
				{
					if (a._modifierUniqueID < 0)
					{
						return true;
					}
				}
				return false;

			});

			for (int i = 0; i < _timelines.Count; i++)
			{
				_timelines[i].RemoveUnlinkedLayer();
			}
		}

		public void LinkOpt(apPortrait portrait)
		{
			_portrait = portrait;

			
			if(_targetMeshGroupID < 0)
			{
				//연결되지 않았네요.
				UnityEngine.Debug.LogError("AnyPortrait : Animation Clip [" + _name + "] : No MeshGroup Linked");
				return;
			}


			_targetOptTranform = _portrait.GetOptTransformAsMeshGroup(_targetMeshGroupID);

			//추가 : 여기서 FPS 관련 코드 확인
			if(_FPS < 1)
			{
				_FPS = 1;
			}
			_secPerFrame = 1.0f / (float)_FPS;
			

			if (_targetOptTranform == null)
			{
				//UnityEngine.Debug.LogError("AnimClip이 적용되는 Target Opt Transform이 Null이다. [" + _targetMeshGroupID + "] (" + _name + ")");
				//이 AnimClip을 사용하지 맙시다.
				return;
				
			}

			
			for (int i = 0; i < _timelines.Count; i++)
			{
				_timelines[i].LinkOpt(this);
			}
		}



		public IEnumerator LinkOptAsync(apPortrait portrait, apAsyncTimer asyncTimer)
		{
			_portrait = portrait;

			
			if(_targetMeshGroupID < 0)
			{
				//연결되지 않았네요.
				UnityEngine.Debug.LogError("AnyPortrait : Animation Clip [" + _name + "] : No MeshGroup Linked");
				yield break;
			}


			_targetOptTranform = _portrait.GetOptTransformAsMeshGroup(_targetMeshGroupID);

			//추가 : 여기서 FPS 관련 코드 확인
			if(_FPS < 1)
			{
				_FPS = 1;
			}
			_secPerFrame = 1.0f / (float)_FPS;
			

			if (_targetOptTranform == null)
			{
				//UnityEngine.Debug.LogError("AnimClip이 적용되는 Target Opt Transform이 Null이다. [" + _targetMeshGroupID + "] (" + _name + ")");
				//이 AnimClip을 사용하지 맙시다.
				yield break;
				
			}

			
			for (int i = 0; i < _timelines.Count; i++)
			{
				yield return _timelines[i].LinkOptAsync(this, asyncTimer);
			}

			if(asyncTimer.IsYield())
			{
				yield return asyncTimer.WaitAndRestart();
			}
		}




		// Update / 플레이 제어
		//---------------------------------------------
#if UNITY_EDITOR
		private DateTime _sampleDateTime = new DateTime();
#endif

		/// <summary>
		/// [Editor] 업데이트를 한다.
		/// FPS에 맞게 프레임을 증가시킨다.
		/// MeshGroup은 자동으로 재생시킨다.
		/// (재생중이 아닐때는 MeshGroup 자체의 FPS에 맞추어 업데이트를 한다.)
		/// </summary>
		/// <param name="tDelta"></param>
		/// <param name="isUpdateVertsAlways">단순 재생에는 False, 작업시에는 True로 설정</param>
		public void Update_Editor(float tDelta, bool isUpdateVertsAlways, bool isBoneIKMatrix, bool isBoneIKRigging)
		{


#if UNITY_EDITOR
			//시간을 따로 계산하자
			float multiply = 1.0f;
			if (_tDelta_Editor < 0)
			{
				_stopWatch_Editor.Start();
				_tDelta_Editor = tDelta;
				_sampleDateTime = DateTime.Now;

			}
			else
			{
				_stopWatch_Editor.Stop();
				_tDelta_Editor = (float)(_stopWatch_Editor.ElapsedMilliseconds / 1000.0);

				if (_tDelta_Editor > 0)
				{
					float dateTimeMillSec = (float)(DateTime.Now.Subtract(_sampleDateTime).TotalMilliseconds / 1000.0);
					multiply = dateTimeMillSec / _tDelta_Editor;
					//UnityEngine.Debug.Log("Update Anim / StopWatch : " + _tDelta_Editor + " / DateTime Span : " + dateTimeMillSec + " / Mul : " + multiply);
				}
				_sampleDateTime = DateTime.Now;

				_stopWatch_Editor.Reset();
				_stopWatch_Editor.Start();
			}
			tDelta = _tDelta_Editor * multiply;




#endif

			if (!_isPlaying)
			{
				if (_targetMeshGroup != null)
				{
					_targetMeshGroup.SetBoneIKEnabled(isBoneIKMatrix, isBoneIKRigging);
				}
				UpdateMeshGroup_Editor(false, tDelta, isUpdateVertsAlways);//<<강제로 업데이트 하지 않는다.

				if (_targetMeshGroup != null)
				{
					_targetMeshGroup.SetBoneIKEnabled(false, false);
				}
				return;
			}


			_tUpdate += tDelta;
			if (_tUpdate >= TimePerFrame)
			{
				_curFrame++;
				//_tUpdate -= TimePerFrame;

				if (_curFrame >= _endFrame)
				{
					if (_isLoop)
					{
						//루프가 되었당
						//루프는 endframe을 찍지 않고, 바로 startFrame으로 가야한다.
						_curFrame = _startFrame;
					}
					else
					{
						//endframe에서 정지
						_curFrame = _endFrame;
						_isPlaying = false;
						//Debug.Log("Stop In Last Frame");
					}
				}


				//Debug.Log("Update AnimClip : " + _name);

				//1. Control Param을 먼저 업데이트를 하고 [Control Param]
				UpdateControlParam(true);


				//2. Mesh를 업데이트한다. [Animated Modifier + Bone]
				//UpdateMeshGroup_Editor(true, _tUpdate, isUpdateVertsAlways);//강제로 업데이트하자
				//UpdateMeshGroup_Editor(false, tDelta, isUpdateVertsAlways);//일반 업데이트

				//Debug.Log("Anim Update : " + (int)(1.0f / _tUpdate) + "FPS");

				_tUpdate -= TimePerFrame;

			}

			if (_targetMeshGroup != null)
			{
				_targetMeshGroup.SetBoneIKEnabled(isBoneIKMatrix, isBoneIKRigging);
			}

			UpdateMeshGroup_Editor(true, tDelta, isUpdateVertsAlways);

			if (_targetMeshGroup != null)
			{
				_targetMeshGroup.SetBoneIKEnabled(false, false);
			}
		}

		/// <summary>
		/// [Editor] 플레이를 정지한다.
		/// 첫 프레임으로 자동으로 돌아간다.
		/// </summary>
		public void Stop_Editor(bool isRefreshMeshAndControlParam = true)
		{
			_isPlaying = false;
			_tUpdate = 0.0f;
			_curFrame = _startFrame;//<첫 프레임으로 돌아간다.

			if (isRefreshMeshAndControlParam)
			{
				UpdateControlParam(true);
				UpdateMeshGroup_Editor(true, 0.0f, true);//강제로 업데이트하자
			}
		}

		/// <summary>
		/// [Editor] 플레이를 일시중지한다.
		/// 프레임은 현재 위치에서 정지
		/// </summary>
		public void Pause_Editor()
		{
			_isPlaying = false;
			_curFrame = Mathf.Clamp(_curFrame, _startFrame, _endFrame);

			UpdateControlParam(true);
			UpdateMeshGroup_Editor(true, 0.0f, true);//강제로 업데이트하자
		}

		/// <summary>
		/// [Editor] 애니메이션을 재생한다.
		/// </summary>
		/// <param name="isResetFrame">True면 첫 프레임으로 돌려서 시작한다. False면 현재 프레임에서 재개</param>
		public void Play_Editor(bool isResetFrame = false)
		{
			_isPlaying = true;
			_tUpdate = 0.0f;
			_curFrame = Mathf.Clamp(_curFrame, _startFrame, _endFrame);

			UpdateControlParam(true);
			UpdateMeshGroup_Editor(true, 0.0f, true);//강제로 업데이트하자
		}

		/// <summary>
		/// [Editor] 프레임을 지정한다. (자동으로 메시 업데이트가 된다)
		/// </summary>
		/// <param name="frame"></param>
		public void SetFrame_Editor(int frame)
		{
			_curFrame = Mathf.Clamp(frame, _startFrame, _endFrame);
			_isPlaying = false;//<<Set Frame시에는 자동으로 Pause한다.
			_tUpdate = 0.0f;

			UpdateControlParam(true);
			UpdateMeshGroup_Editor(true, 0.0f, true);//강제로 업데이트하자
		}

		/// <summary>
		/// [Editor] 프레임을 지정한다. Min-Max를 가리지 않고,
		/// 재생 여부에 제한을 두지 않는다.
		/// </summary>
		/// <param name="frame"></param>
		public void SetFrame_EditorNotStop(int frame)
		{
			_curFrame = frame;
			
			UpdateControlParam(true);
			UpdateMeshGroup_Editor(true, 0.0f, true);//강제로 업데이트하자
		}


		/// <summary>
		/// [Editor] 업데이트 중 Control Param 제어 Timeline에 대해 업데이트 후 적용을 한다.
		/// [Runtime] isAdaptToWeight = false로 두고 나머지 처리를 한다.
		/// </summary>
		/// <param name="isAdaptToWeight1">[Editor]에서 Weight=1로 두고 적용을 한다</param>
		public void UpdateControlParam(bool isAdaptToWeight1, int optLayer = 0, float optWeight = 1.0f, apAnimPlayUnit.BLEND_METHOD optBlendMethod = apAnimPlayUnit.BLEND_METHOD.Interpolation)
		{
			if (_controlParamResult.Count == 0)
			{
				return;
			}

			for (int i = 0; i < _controlParamResult.Count; i++)
			{
				_controlParamResult[i].Init();
			}

			apAnimTimeline timeline = null;
			apAnimTimelineLayer layer = null;

			int curFrame = CurFrame;
			float curFrameF = CurFrameFloat;

			//apAnimKeyframe firstKeyframe = null;
			//apAnimKeyframe lastKeyframe = null;

			apAnimKeyframe curKeyframe = null;
			apAnimKeyframe prevKeyframe = null;
			apAnimKeyframe nextKeyframe = null;

			int lengthFrames = _endFrame - _startFrame;
			int tmpCurFrame = 0;

			apAnimControlParamResult cpResult = null;

			for (int iTL = 0; iTL < _timelines.Count; iTL++)
			{
				timeline = _timelines[iTL];
				if (timeline._linkType != LINK_TYPE.ControlParam)
				{
					continue;
				}

				for (int iL = 0; iL < timeline._layers.Count; iL++)
				{
					layer = timeline._layers[iL];
					if (layer._linkedControlParam == null || layer._linkedControlParamResult == null)
					{
						continue;
					}

					cpResult = layer._linkedControlParamResult;

					//firstKeyframe = layer._firstKeyFrame;
					//lastKeyframe = layer._lastKeyFrame;

					for (int iK = 0; iK < layer._keyframes.Count; iK++)
					{
						curKeyframe = layer._keyframes[iK];
						prevKeyframe = curKeyframe._prevLinkedKeyframe;
						nextKeyframe = curKeyframe._nextLinkedKeyframe;

						if (curFrame == curKeyframe._frameIndex ||
							((curKeyframe._isLoopAsStart || curKeyframe._isLoopAsEnd) && curKeyframe._loopFrameIndex == curFrame)
							)
						{
							cpResult.SetKeyframeResult(curKeyframe, 1.0f);
						}
						else if (curKeyframe.IsFrameIn(curFrame, apAnimKeyframe.LINKED_KEY.Prev))
						{
							//Prev - Cur 범위 안에 들었다.
							if (prevKeyframe != null)
							{
								//Prev 키가 있다면
								tmpCurFrame = curFrame;
								if (tmpCurFrame > curKeyframe._frameIndex)
								{
									//한바퀴 돌았다면
									tmpCurFrame -= lengthFrames;
								}

								//TODO : 여길 나중에 "정식 Curve로 변경"할 것 
								//float itp = apAnimCurve.GetCurvedRelativeInterpolation(curKeyframe._curveKey, prevKeyframe._curveKey, tmpCurFrame, true);

								//>> 변경
								float itp = curKeyframe._curveKey.GetItp_Int(tmpCurFrame, true);

								cpResult.SetKeyframeResult(curKeyframe, itp);
							}
							else
							{
								//Prev 키가 없다면 이게 100%다
								cpResult.SetKeyframeResult(curKeyframe, 1.0f);
							}
						}
						else if (curKeyframe.IsFrameIn(curFrame, apAnimKeyframe.LINKED_KEY.Next))
						{
							//Cur - Next 범위 안에 들었다.
							if (nextKeyframe != null)
							{
								//Next 키가 있다면
								tmpCurFrame = curFrame;
								if (tmpCurFrame < curKeyframe._frameIndex)
								{
									//한바퀴 돌았다면
									tmpCurFrame += lengthFrames;
								}

								//TODO : 여길 나중에 "정식 Curve로 변경"할 것 
								//float itp = apAnimCurve.GetCurvedRelativeInterpolation(curKeyframe._curveKey, nextKeyframe._curveKey, tmpCurFrame, false);

								//>> 변경
								float itp = curKeyframe._curveKey.GetItp_Int(tmpCurFrame, false);

								cpResult.SetKeyframeResult(curKeyframe, itp);
							}
							else
							{
								//Prev 키가 없다면 이게 100%다
								cpResult.SetKeyframeResult(curKeyframe, 1.0f);
							}
						}
					}
				}
			}


			//Control Param에 적용을 해야한다.
			if (isAdaptToWeight1)
			{
				//Editor인 경우 Weight 1로 강제한다.
				for (int i = 0; i < _controlParamResult.Count; i++)
				{
					_controlParamResult[i].AdaptToControlParam();
				}
			}
			else
			{
				//Runtime인 경우 지정된 Weight, Layer로 처리한다.
				for (int i = 0; i < _controlParamResult.Count; i++)
				{
					//Debug.Log("AnimClip [" + _name + " > Control Param : " + _controlParamResult[i]._targetControlParam._keyName + " ]");
					_controlParamResult[i].AdaptToControlParam_Opt(optWeight, optBlendMethod);
				}

			}
		}


		/// <summary>
		/// [Editor] 모든 애니메이션 처리를 포함한 MeshGroup 업데이트를 한다.
		/// </summary>
		/// <param name="isForce"></param>
		/// <param name="tDelta"></param>
		/// <param name="isUpdateVertsAlways">단순 재생시에는 False, 작업시에는 True로 설정한다.</param>
		public void UpdateMeshGroup_Editor(bool isForce, float tDelta, bool isUpdateVertsAlways, bool isDepthChanged = false)
		{
			if (_targetMeshGroup == null)
			{
				//Debug.LogError("Update Failed : No Target Mesh Group");
				return;
			}
			if (isForce)
			{
				//_targetMeshGroup.SetAllRenderUnitForceUpdate();
				_targetMeshGroup.RefreshForce(isDepthChanged, tDelta);
			}
			else
			{
				_targetMeshGroup.UpdateRenderUnits(tDelta, isUpdateVertsAlways);
			}

		}


		// Opt용 Update / Opt 플레이 제어
		//---------------------------------------------
		// Opt용 Update 함수들은 "플레이 상태"의 영향을 받지 않는다.
		// AnimPlayUnit에 래핑된 상태이므로 모든 제어에 대해서 바로 처리한다.
		// Editor와 달리 "실수형 CurFrame"을 이용하며, Reverse 처리도 가능하다
		// MeshGroup과 ControlParam을 직접 제어하진 않는다. (AnimPlayUnit이 한다)

		/// <summary>
		/// [Opt 실행] Delta만큼 업데이트를 한다.
		/// Keyframe Weight와 Control Param Result를 만든다.
		/// </summary>
		/// <param name="tDelta">재생 시간. 음수면 Reverse가 된다.</param>
		/// <returns>Update가 종료시 True, 그 외에는 False이다.</returns>
		public bool Update_Opt(float tDelta)
		{
			_tUpdate += tDelta;
			_tUpdateTotal += tDelta;
			bool isEnd = false;

			
			if (tDelta > 0)
			{
				//Speed Ratio가 크면 프레임이 한번에 여러개 이동할 수 있다.
				while (_tUpdate > TimePerFrame)
				{
					//프레임이 증가한다.
					_curFrame++;
					_tUpdate -= TimePerFrame;

					if (_curFrame >= _endFrame)
					{
						if (_isLoop)
						{
							//루프일 경우 -> 첫 프레임으로 돌아간다.
							_curFrame = _startFrame;
							_tUpdateTotal -= TimeLength;

							//Animation 이벤트도 리셋한다. (루프에 의한 것이므로 전체 리셋)
							//이전
							//if (_animEvents != null && _animEvents.Count > 0)
							//{
							//	for (int i = 0; i < _animEvents.Count; i++)
							//	{
							//		_animEvents[i].ResetCallFlag();
							//	}
							//}
							//변경
							ResetEvents();
						}
						else
						{
							//루프가 아닐 경우
							_curFrame = _endFrame;
							_tUpdate = 0.0f;
							_tUpdateTotal = TimeLength;
							isEnd = true;
							break;
						}
					}

					//UpdateControlParam(false, _parentPlayUnit._layer, _parentPlayUnit.UnitWeight, _parentPlayUnit.BlendMethod);
				}
			}
			else if (tDelta < 0)
			{
				while (_tUpdate < 0.0f)
				{
					//프레임이 감소한다.
					_curFrame--;
					_tUpdate += TimePerFrame;

					if (_curFrame <= _startFrame)
					{
						if (_isLoop)
						{
							//루프일 경우 -> 마지막 프레임으로 돌아간다.
							_curFrame = _endFrame;
							_tUpdateTotal += TimeLength;

							//Animation 이벤트도 리셋한다. (루프에 의한 것이므로 전체 리셋)
							//이전
							//if (_animEvents != null && _animEvents.Count > 0)
							//{
							//	for (int i = 0; i < _animEvents.Count; i++)
							//	{
							//		_animEvents[i].ResetCallFlag();
							//	}
							//}
							//변경
							ResetEvents();
						}
						else
						{
							//루프가 아닐 경우
							_curFrame = _startFrame;
							_tUpdate = 0.0f;
							_tUpdateTotal = 0.0f;
							isEnd = true;
							break;
						}
					}

					//UpdateControlParam(false, _parentPlayUnit._layer, _parentPlayUnit.UnitWeight, _parentPlayUnit.BlendMethod);
				}
			}

			float unitWeight = _parentPlayUnit.UnitWeight;

			//이거 문제 생기면 끈다.
			//if (_parentPlayUnit._playOrder == 0)
			//{
			//	unitWeight = 1.0f;
			//}
			//??이거요?

			//UpdateControlParam(false, _parentPlayUnit._layer, _parentPlayUnit.UnitWeight, _parentPlayUnit.BlendMethod);
			UpdateControlParam(false, _parentPlayUnit._layer, unitWeight, _parentPlayUnit.BlendMethod);

			//추가
			//AnimEvent도 업데이트 하자
			if(_animEvents != null && _animEvents.Count > 0)
			{	
				apAnimEvent animEvent = null;
				for (int i = 0; i < _animEvents.Count; i++)
				{
					animEvent = _animEvents[i];
					animEvent.Calculate(CurFrameFloat, CurFrame, (tDelta > 0.0f), Mathf.Abs(tDelta) > 0.0001f, tDelta, _speedRatio);
					if(animEvent.IsEventCallable())
					{
						if(_portrait._optAnimEventListener != null)
						{
							//애니메이션 이벤트를 호출해줍시다.
							//UnityEngine.Debug.Log("Animation Event : " + animEvent._eventName + " / CurFrameFloat : " + CurFrameFloat + " / tDelta : " + tDelta);
							_portrait._optAnimEventListener.SendMessage(animEvent._eventName, animEvent.GetCalculatedParam(), SendMessageOptions.DontRequireReceiver);
							
						}
					}
				}
			}

			//if (isEnd)
			//{
			//	//Animation 이벤트도 리셋한다.
			//	if (_animEvents != null && _animEvents.Count > 0)
			//	{
			//		for (int i = 0; i < _animEvents.Count; i++)
			//		{
			//			_animEvents[i].ResetCallFlag();
			//		}
			//	}
			//}

			return isEnd;
		}




		// 변경 1.17 : 메카님은 다르게 처리해야한다.
		public bool UpdateMecanim_Opt(float tDelta, float stateSpeed)
		{
			_tUpdate += tDelta;
			_tUpdateTotal += tDelta;
			bool isEnd = false;

			
			if (tDelta > 0)
			{
				//Speed Ratio가 크면 프레임이 한번에 여러개 이동할 수 있다.
				while (_tUpdate > TimePerFrame)
				{
					//프레임이 증가한다.
					_curFrame++;
					_tUpdate -= TimePerFrame;

					if (_curFrame >= _endFrame)
					{
						if (_isLoop)
						{
							//루프일 경우 -> 첫 프레임으로 돌아간다.
							_curFrame = _startFrame;
							_tUpdateTotal -= TimeLength;

							if (stateSpeed > 0.0f)
							{	
								//UnityEngine.Debug.LogWarning("Frame Over the Length (Forward)");
								//만약 밖에서 이벤트 초기화가 안되었다면 여기서 하자
								ResetEvents();
							}
						}
						else
						{
							//루프가 아닐 경우
							_curFrame = _endFrame;
							_tUpdate = 0.0f;
							_tUpdateTotal = TimeLength;
							isEnd = true;
							break;
						}
					}
				}
			}
			else if (tDelta < 0)
			{
				while (_tUpdate < 0.0f)
				{
					//프레임이 감소한다.
					_curFrame--;
					_tUpdate += TimePerFrame;

					if (_curFrame <= _startFrame)
					{
						if (_isLoop)
						{
							//루프일 경우 -> 마지막 프레임으로 돌아간다.
							_curFrame = _endFrame;
							_tUpdateTotal += TimeLength;

							if (stateSpeed < 0.0f)
							{
								//UnityEngine.Debug.LogWarning("Frame Over the Length (Backward)");
								//만약 밖에서 이벤트 초기화가 안되었다면 여기서 하자
								ResetEvents();
							}
						}
						else
						{
							//루프가 아닐 경우
							_curFrame = _startFrame;
							_tUpdate = 0.0f;
							_tUpdateTotal = 0.0f;
							isEnd = true;
							break;
						}
					}
				}
			}

#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				if (_parentPlayUnit == null)
				{
					return true;
				}
			}
#endif

			float unitWeight = _parentPlayUnit.UnitWeight;

			UpdateControlParam(false, _parentPlayUnit._layer, unitWeight, _parentPlayUnit.BlendMethod);

			//추가
			//AnimEvent도 업데이트 하자
			if(_animEvents != null && _animEvents.Count > 0)
			{	
				apAnimEvent animEvent = null;
				for (int i = 0; i < _animEvents.Count; i++)
				{
					animEvent = _animEvents[i];
					//animEvent.Calculate(CurFrameFloat, CurFrame, (tDelta > 0.0f), Mathf.Abs(tDelta) > 0.0001f, tDelta, _speedRatio);//기존
					animEvent.Calculate(CurFrameFloat, CurFrame, (stateSpeed > 0.0f), Mathf.Abs(tDelta) > 0.0001f, tDelta, stateSpeed);//메카님용
					if(animEvent.IsEventCallable())
					{
						if(_portrait._optAnimEventListener != null)
						{
							//애니메이션 이벤트를 호출해줍시다.
							//UnityEngine.Debug.Log("Animation Event : " + animEvent._eventName + " / CurFrameFloat : " + CurFrameFloat + " / tDelta : " + tDelta);
							_portrait._optAnimEventListener.SendMessage(animEvent._eventName, animEvent.GetCalculatedParam(), SendMessageOptions.DontRequireReceiver);
							
						}
					}
				}
			}

			return isEnd;
		}








		/// <summary>
		/// [Opt 실행] 특정 프레임으로 이동한다.
		/// Keyframe Weight와 Control Param Result를 만든다.
		/// Start - End 프레임 사이의 값으로 강제된다.
		/// </summary>
		/// <param name="frame"></param>
		public void SetFrame_Opt(int frame, bool isResetAnimEventByFrame)
		{
			_curFrame = Mathf.Clamp(frame, _startFrame, _endFrame);
			_tUpdate = 0.0f;

			_tUpdateTotal = (_curFrame - _startFrame) * TimePerFrame;

			UpdateControlParam(false, _parentPlayUnit._layer, _parentPlayUnit.UnitWeight, _parentPlayUnit.BlendMethod);

			
			//프레임 이동시에 AnimEvent를 다시 리셋한다.
			//이전
			//if (_animEvents != null && _animEvents.Count > 0)
			//{
			//	for (int i = 0; i < _animEvents.Count; i++)
			//	{
			//		_animEvents[i].ResetCallFlag();
			//	}
			//}

			if(!isResetAnimEventByFrame)
			{
				ResetEvents();
			}
			else
			{
				ResetEventsBasedFrame(frame, _speedRatio > 0.0f);
			}
		}


		/// <summary>
		/// [Opt 실행] 플레이를 정지한다.
		/// 첫 프레임으로 자동으로 돌아간다.
		/// </summary>
		public void Stop_Opt(bool isRefreshMeshAndControlParam = true)
		{
			_isPlaying = false;
			_tUpdate = 0.0f;
			_curFrame = _startFrame;//<첫 프레임으로 돌아간다.

			if (isRefreshMeshAndControlParam)
			{
				UpdateControlParam(true);
			}

			//Animation 이벤트도 리셋한다. (전체)
			//이전
			//if (_animEvents != null && _animEvents.Count > 0)
			//{
			//	for (int i = 0; i < _animEvents.Count; i++)
			//	{
			//		_animEvents[i].ResetCallFlag();
			//	}
			//}
			ResetEvents();
		}

		/// <summary>
		/// [Opt 실행] 플레이를 일시중지한다.
		/// 프레임은 현재 위치에서 정지
		/// </summary>
		public void Pause_Opt()
		{
			_isPlaying = false;
			_curFrame = Mathf.Clamp(_curFrame, _startFrame, _endFrame);

			UpdateControlParam(true);
		}

		/// <summary>
		/// [Opt 실행] 다른 처리 없이 프레임만 "시작 프레임"옮긴다.
		/// 애니메이션 처리가 끝났을 때, 처음 실행 될 때 이 함수를 먼저 호출해주자
		/// </summary>
		public void ResetFrame()
		{
			_curFrame = _startFrame;
			_tUpdate = 0.0f;
		}

		

		/// <summary>
		/// _isPlaying 변수를 제어 한다. 단지 그뿐
		/// </summary>
		public void SetPlaying_Opt(bool isPlaying)
		{
			_isPlaying = isPlaying;
		}







		// Functions
		//---------------------------------------------
		//변경 19.5.21 : 항상 모든 타임라인 레이어를 Refresh 할게 아니라, 필요한 것만 업데이트를 하자.
		public void RefreshTimelines(apAnimTimelineLayer targetTimelineLayer)
		{
			if (targetTimelineLayer == null)
			{
				for (int i = 0; i < _timelines.Count; i++)
				{
					_timelines[i].RefreshLayers(null);
				}
			}
			else
			{
				//타겟만 Refresh
				if(targetTimelineLayer._parentTimeline != null
					&& _timelines.Contains(targetTimelineLayer._parentTimeline))
				{
					targetTimelineLayer._parentTimeline.RefreshLayers(targetTimelineLayer);
				}
			}
			

			//추가
			//Control Param Result 객체와 연결을 하자
			MakeAndLinkControlParamResults();
		}


		/// <summary>
		/// AnimClip이 Control Param을 제어하기 위해서는 이 함수를 호출하여 업데이트를 할 수 있게 해야한다.
		/// [Opt에서는 AnimPlayData를 링크할때 만들어주자]
		/// </summary>
		public void MakeAndLinkControlParamResults()
		{
			if (_controlParamResult == null)
			{
				_controlParamResult = new List<apAnimControlParamResult>();
			}
			_controlParamResult.Clear();

			apAnimTimeline timeline = null;
			apAnimTimelineLayer layer = null;

			for (int iTL = 0; iTL < _timelines.Count; iTL++)
			{
				timeline = _timelines[iTL];
				if (timeline._linkType != LINK_TYPE.ControlParam)
				{
					continue;
				}

				for (int iL = 0; iL < timeline._layers.Count; iL++)
				{
					layer = timeline._layers[iL];

					if (layer._linkedControlParam == null)
					{
						continue;
					}

					apAnimControlParamResult cpResult = GetControlParamResult(layer._linkedControlParam);
					if (cpResult == null)
					{
						cpResult = new apAnimControlParamResult(layer._linkedControlParam);
						cpResult.Init();
						_controlParamResult.Add(cpResult);
					}

					//레이어와도 연동해주자
					//ControlParam <- CPResult <- Layer
					//     ^------------------------]

					layer._linkedControlParamResult = cpResult;
				}
			}
		}

		public apAnimControlParamResult GetControlParamResult(apControlParam targetControlParam)
		{
			return _controlParamResult.Find(delegate (apAnimControlParamResult a)
			{
				return a._targetControlParam == targetControlParam;
			});
		}

		//1.16 추가 : 속도 조절 함수
		/// <summary>
		/// 애니메이션의 속도를 제어하는 함수. 속도의 부호가 반전되면 애니메이션 이벤트를 갱신한다.
		/// </summary>
		/// <param name="speed"></param>
		public void SetSpeed(float speed)
		{
			bool isSignInverted = (_speedRatio * speed) < 0.0f;
			_speedRatio = speed;

			if(isSignInverted)
			{
				OnSpeedSignInverted();
			}
		}

		// 1.16 추가 : 이벤트 처리를 위한 초기화
		//1. 그냥 전부 초기화 (ResetCallFlag 호출)
		//public void ResetAnimEventCallFlagAll()
		//{
		//	//프레임 이동시에 AnimEvent를 다시 리셋한다.
		//	if (_animEvents == null || _animEvents.Count == 0)
		//	{
		//		return;
		//	}
		//	for (int i = 0; i < _animEvents.Count; i++)
		//	{
		//		_animEvents[i].ResetCallFlag();
		//	}
		//}
		public void ResetEvents()
		{
			//Animation 이벤트도 리셋한다.
			if (_animEvents != null && _animEvents.Count > 0)
			{
				for (int i = 0; i < _animEvents.Count; i++)
				{
					_animEvents[i].ResetCallFlag();
				}
			}
		}

		public void ResetEventsBasedFrame(int frame, bool isForward)
		{
			//프레임 이동시에 AnimEvent를 다시 리셋한다.
			if (_animEvents == null || _animEvents.Count == 0)
			{
				return;
			}
			apAnimEvent animEvent = null;
			int minFrame = -1;
			int maxFrame = -1;
			for (int i = 0; i < _animEvents.Count; i++)
			{
				animEvent = _animEvents[i];
				//일단 초기화
				animEvent.ResetCallFlag();

				if(animEvent._callType == apAnimEvent.CALL_TYPE.Once)
				{
					minFrame = animEvent._frameIndex;
					maxFrame = animEvent._frameIndex;
				}
				else
				{
					minFrame = animEvent._frameIndex;
					maxFrame = animEvent._frameIndex_End;
				}

				if(isForward)
				{
					//현재 프레임보다 작다면 모두 Lock
					if(minFrame < frame && maxFrame < frame)
					{
						animEvent.Lock();
						//UnityEngine.Debug.LogError(" >> Lock [" + animEvent._eventName + "] " + animEvent._frameIndex);
					}
				}
				else
				{
					//현재 프레임보다 모두 크다면 모두 Lock
					if(minFrame > frame && maxFrame > frame)
					{
						animEvent.Lock();
						//UnityEngine.Debug.LogError(" >> Lock [" + animEvent._eventName + "] " + animEvent._frameIndex);
					}
				}
			}
		}

		//추가 1.16 : ResetEventsBasedFrame()과 비슷한 역할의 함수이다.
		//Speed의 방향이 바뀌었다면 강제로 이벤트가 리셋되어야 한다.
		//AnimEvent에서 처리하자
		public void OnSpeedSignInverted()
		{
			if (_animEvents == null || _animEvents.Count == 0)
			{
				return;
			}
			for (int i = 0; i < _animEvents.Count; i++)
			{
				_animEvents[i].OnSpeedSignInverted(_curFrame, (_speedRatio > 0.0f), 0.0f, _speedRatio);
			}
		}

		


		// Get / Set
		//---------------------------------------------
		public bool IsTimelineContain(apAnimTimeline animTimeline)
		{
			return _timelines.Contains(animTimeline);
		}
		public bool IsTimelineContain(LINK_TYPE linkType, int modifierID)
		{
			return _timelines.Exists(delegate (apAnimTimeline a)
			{
				if (linkType == LINK_TYPE.AnimatedModifier)
				{
					return a._linkType == linkType && a._modifierUniqueID == modifierID;
				}
				else
				{
					return a._linkType == linkType;
				}
			});
		}

		public apAnimTimeline GetTimeline(int timelineID)
		{
			return _timelines.Find(delegate (apAnimTimeline a)
			{
				return a._uniqueID == timelineID;
			});
		}


		public void SetOption_FPS(int fps)
		{
			_FPS = fps;
			if (_FPS < 1)
			{
				_FPS = 1;
			}
			_secPerFrame = 1.0f / (float)_FPS;
		}

		public void SetOption_StartFrame(int startFrame)
		{
			_startFrame = startFrame;
		}

		public void SetOption_EndFrame(int endFrame)
		{
			_endFrame = endFrame;
		}

		public void SetOption_IsLoop(bool isLoop)
		{
			_isLoop = isLoop;
		}

		//---------------------------------------------------------------------------------------
		// Copy For Bake
		//---------------------------------------------------------------------------------------
		public void CopyFromAnimClip(apAnimClip srcAnimClip)
		{
			_uniqueID = srcAnimClip._uniqueID;
			_name = srcAnimClip._name;

			_targetMeshGroupID = srcAnimClip._targetMeshGroupID;
			_targetMeshGroup = null;
			_targetOptTranform = null;

			_FPS = srcAnimClip._FPS;
			_secPerFrame = srcAnimClip._secPerFrame;//수정 3.31 : 이게 버그의 원인
			_startFrame = srcAnimClip._startFrame;
			_endFrame = srcAnimClip._endFrame;
			_isLoop = srcAnimClip._isLoop;

			//Timeline 복사
			_timelines.Clear();
			for (int iTimeline = 0; iTimeline < srcAnimClip._timelines.Count; iTimeline++)
			{
				apAnimTimeline srcTimeline = srcAnimClip._timelines[iTimeline];

				//Timeline을 복사하자.
				//내부에서 차례로 Layer, Keyframe도 복사된다.
				apAnimTimeline newTimeline = new apAnimTimeline();
				newTimeline.CopyFromTimeline(srcTimeline, this);

				_timelines.Add(newTimeline);
			}

			//AnimEvent 복사
			_animEvents.Clear();
			for (int iEvent = 0; iEvent < srcAnimClip._animEvents.Count; iEvent++)
			{
				apAnimEvent srcEvent = srcAnimClip._animEvents[iEvent];

				//Event 복사하자
				apAnimEvent newEvent = new apAnimEvent();
				newEvent.CopyFromAnimEvent(srcEvent);

				_animEvents.Add(newEvent);
			}
		}
	}

}