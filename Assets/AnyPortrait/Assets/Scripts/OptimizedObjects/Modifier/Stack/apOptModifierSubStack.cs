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


using AnyPortrait;

namespace AnyPortrait
{

	/// <summary>
	/// (Root)MeshGroup -> ModiferStack -> Modifier -> ParamSet...으로 이어지는 단계중 [ModiferStack]의 리얼타임버전.
	/// 이후 빠른 처리를 위해 MainStack으로 다시 값을 전달해준다.
	/// </summary>
	[Serializable]
	public class apOptModifierSubStack
	{
		// Members
		//--------------------------------------------
		[SerializeField]
		public List<apOptModifierUnitBase> _modifiers = new List<apOptModifierUnitBase>();

		[NonSerialized]
		public apPortrait _portrait = null;

		[NonSerialized]
		public apOptTransform _parentTransform = null;

		[SerializeField]
		public int _parentTransformID = -1;

		[SerializeField]
		private int _nModifiers = 0;

		// Init
		//--------------------------------------------
		public apOptModifierSubStack()
		{

		}

		public void Bake(apModifierStack modStack, apPortrait portrait, bool isUseModMesh)
		{
			_portrait = portrait;
			_modifiers.Clear();

			_parentTransformID = -1;
			_parentTransform = null;

			if (modStack._parentMeshGroup != null)
			{
				//MeshGroup 타입의 OptTransform을 찾자
				apOptTransform optTransform = _portrait.GetOptTransformAsMeshGroup(modStack._parentMeshGroup._uniqueID);
				if (optTransform != null)
				{
					_parentTransformID = optTransform._transformID;
					_parentTransform = optTransform;
				}
			}
			if(_parentTransform == null)
			{
				Debug.LogError("Opt Modifier Stack -> Null Opt Transform");
			}

			//Modifier를 Bake해주자
			for (int i = 0; i < modStack._modifiers.Count; i++)
			{
				apModifierBase modifier = modStack._modifiers[i];

				apOptModifierUnitBase optMod = new apOptModifierUnitBase();

				////ModifierType에 맞게 상속된 클래스로 생성한다.
				//switch (modifier.ModifierType)
				//{
				//	case apModifierBase.MODIFIER_TYPE.Base:
				//		optMod = new apOptModifierUnitBase();
				//		break;

				//	case apModifierBase.MODIFIER_TYPE.AnimatedMorph:
				//		break;

				//	case apModifierBase.MODIFIER_TYPE.Morph:
				//		optMod = new apOptModifierUnit_Morph();//Morph
				//		break;

				//	case apModifierBase.MODIFIER_TYPE.Physic:
				//		break;

				//	case apModifierBase.MODIFIER_TYPE.Rigging:
				//		break;

				//	case apModifierBase.MODIFIER_TYPE.Volume:
				//		break;

				//	default:
				//		Debug.LogError("apOptModifierSubStack Bake : 알 수 없는 Mod 타입 : " + modifier.ModifierType);
				//		break;

				//}

				if (optMod != null)
				{
					optMod.Bake(modifier, _portrait, isUseModMesh);
					optMod.Link(_portrait, _parentTransform);

					_modifiers.Add(optMod);
				}
			}

			_nModifiers = _modifiers.Count;
		}



		public void Link(apPortrait portrait, apOptTransform parentTransform)
		{
			_portrait = portrait;

			//_parentTransform = _portrait.GetOptTransform(_parentTransformID);
			_parentTransform = parentTransform;
			

			for (int i = 0; i < _modifiers.Count; i++)
			{
				_modifiers[i].Link(portrait, _parentTransform);
			}

			_nModifiers = _modifiers.Count;
		}

		public IEnumerator LinkAsync(apPortrait portrait, apOptTransform parentTransform, apAsyncTimer asyncTimer)
		{
			_portrait = portrait;

			//_parentTransform = _portrait.GetOptTransform(_parentTransformID);
			_parentTransform = parentTransform;
			

			for (int i = 0; i < _modifiers.Count; i++)
			{
				yield return _modifiers[i].LinkAsync(portrait, _parentTransform, asyncTimer);
			}

			_nModifiers = _modifiers.Count;

			if(asyncTimer.IsYield())
			{
				yield return asyncTimer.WaitAndRestart();
			}
		}


		public void ClearAllCalculateParam()
		{
			for (int i = 0; i < _modifiers.Count; i++)
			{
				_modifiers[i]._calculatedResultParams.Clear();
			}
		}

		//Link가 모두 끝난 후 실행시켜준다.
		//Modifier -> Target Tranform (=RenderUnit)을 CalculatedParam을 이용해 연결해준다.
		public void LinkModifierStackToRenderUnitCalculateStack(bool isRoot, apOptTransform rootOptTransform, bool isRecursive)
		{
			//RenderUnit => OptTransform
			//전체 Modifier중에서 RenderUnit을 포함한 Modifer를 찾는다.
			//그 중, RenderUnit에 대한것만 처리할 CalculateResultParam을 만들고 연동한다.
			//ResultParam을 RenderUnit의 CalculateStack에 넣는다.

			//if (_parentTransform != null)
			//{
			//	Debug.Log(">> [Opt] LinkModifierStackToRenderUnitCalculateStack - " + _parentTransform._name + "(Root : " + rootOptTransform._name + ")");
			//}
			//else if(isRoot)
			//{
			//	Debug.Log(">> [Opt] LinkModifierStackToRenderUnitCalculateStack - " + rootOptTransform._name + " - Root");
			//}
			//여기서 버그가 발생되었다.
			if(_nModifiers != _modifiers.Count)
			{
				_nModifiers = _modifiers.Count;
			}

			for (int iMod = 0; iMod < _nModifiers; iMod++)
			{
				//Modifier ->..
				apOptModifierUnitBase modifier = _modifiers[iMod];

				bool isUseModMeshSet = modifier._isUseModMeshSet;//19.5.24 추가

				List<apOptParamSetGroup> paramSetGroups = modifier._paramSetGroupList;

				for (int iGroup = 0; iGroup < paramSetGroups.Count; iGroup++)
				{
					//Modifier -> ParamSetGroup ->..
					apOptParamSetGroup paramSetGroup = paramSetGroups[iGroup];

					List<apOptParamSet> paramSets = paramSetGroup._paramSetList;

					for (int iParam = 0; iParam < paramSets.Count; iParam++)
					{
						//Modifier -> ParamSetGroup -> ParamSet ->...
						apOptParamSet paramSet = paramSets[iParam];

						List<apOptModifiedMesh> modMeshes = paramSet._meshData;
						List<apOptModifiedBone> modBones = paramSet._boneData;

						//추가 19.5.24 < ModMesh대신 ModMeshSet을 사용하자
						List<apOptModifiedMeshSet> modMeshSets = paramSet._meshSetData;

						//변경 19.5.24
						//기존의 ModMesh가 불필요하게 데이터를 모두 가지고 있어서 용량이 많았다.
						//개선된 ModMeshSet으로 변경하여 최적화
						if (isUseModMeshSet)
						{
							//개선된 ModMeshSet을 사용하는 경우
							apOptModifiedMeshSet modMeshSet = null;
							for (int iModMeshSet = 0; iModMeshSet < modMeshSets.Count; iModMeshSet++)
							{
								modMeshSet = modMeshSets[iModMeshSet];
								if(modMeshSet._targetTransform != null)
								{
									//이미 만든 Calculate Param이 있는지 확인
									apOptCalculatedResultParam existParam = modifier.GetCalculatedResultParam(modMeshSet._targetTransform);

									if (existParam != null)
									{
										//이미 존재하는 Calculated Param이 있다.
										existParam.AddParamSetAndModifiedValue(paramSetGroup, paramSet, null, null, modMeshSet);

										//추가 12.5
										//이미 추가된 ResultParam에 ParamKeyValue가 추가될 때 CalculateStack이 갱신되는 경우가 있다.
										modMeshSet._targetTransform.CalculatedStack.OnParamKeyValueAddedOnCalculatedResultParam(existParam);
									}
									else
									{
										//새로 Calculated Param을 만들어야 한다.
										apOptCalculatedResultParam newCalParam = new apOptCalculatedResultParam(
											modifier._calculatedValueType,
											modifier._calculatedSpace,
											modifier,
											modMeshSet._targetTransform,
											modMeshSet._targetTransform,
											modMeshSet._targetMesh,
											null
											//weightedVertexData//<<사용 안함 19.5.20
											);

										newCalParam.AddParamSetAndModifiedValue(paramSetGroup, paramSet, null, null, modMeshSet);

										//Modifier에 등록하고
										modifier._calculatedResultParams.Add(newCalParam);

										//OptTranform에도 등록하자
										modMeshSet._targetTransform.CalculatedStack.AddCalculatedResultParam(newCalParam);
									}
								}
							}
						}
						else
						{
							//기존의 ModMesh를 사용하는 경우

							for (int iModMesh = 0; iModMesh < modMeshes.Count; iModMesh++)
							{
								//[핵심]
								//Modifier -> ParamSetGroup -> ParamSet -> ModMeh 
								//이제 이 ModMesh와 타겟 Transform을 연결하자.
								//연결할땐 Calculated 오브젝트를 만들어서 연결
								apOptModifiedMesh modMesh = modMeshes[iModMesh];

								if (modMesh._targetTransform != null)
								{
									//이미 만든 Calculate Param이 있는지 확인
									apOptCalculatedResultParam existParam = modifier.GetCalculatedResultParam(modMesh._targetTransform);

									// 삭제 19.5.20 : apOptParamSetGroupVertWeight는 더이상 사용하지 않음
									//apOptParamSetGroupVertWeight weightedVertexData = null;
									//if (modMesh._targetMesh != null)
									//{
									//	weightedVertexData = paramSetGroup.GetWeightVertexData(modMesh._targetTransform);
									//}

									if (existParam != null)
									{
										//이미 존재하는 Calculated Param이 있다.
										existParam.AddParamSetAndModifiedValue(paramSetGroup, paramSet, modMesh, null, null);

										//추가 12.5
										//이미 추가된 ResultParam에 ParamKeyValue가 추가될 때 CalculateStack이 갱신되는 경우가 있다.
										modMesh._targetTransform.CalculatedStack.OnParamKeyValueAddedOnCalculatedResultParam(existParam);
									}
									else
									{
										//새로 Calculated Param을 만들어야 한다.
										apOptCalculatedResultParam newCalParam = new apOptCalculatedResultParam(
											modifier._calculatedValueType,
											modifier._calculatedSpace,
											modifier,
											modMesh._targetTransform,
											modMesh._targetTransform,
											modMesh._targetMesh,
											null
											//weightedVertexData//<<사용 안함 19.5.20
											);

										newCalParam.AddParamSetAndModifiedValue(paramSetGroup, paramSet, modMesh, null, null);

										//Modifier에 등록하고
										modifier._calculatedResultParams.Add(newCalParam);

										//OptTranform에도 등록하자
										modMesh._targetTransform.CalculatedStack.AddCalculatedResultParam(newCalParam);
									}
								}
							}

						}
						

						//변경 10.2 :
						//기존 : ModBone의 _meshGroup_Bone에 해당하는 OptTransform에 연결했다.
						//변경 : ModBone은 항상 RootBone에 연결한다.
						for (int iModBone = 0; iModBone < modBones.Count; iModBone++)
						{
							apOptModifiedBone modBone = modBones[iModBone];
							if (modBone._bone == null || modBone._meshGroup_Bone == null)
							{
								Debug.LogError("ModBone -> Calculate Link (Opt) 실패");
								continue;
							}

							//<BONE_EDIT>
							//apOptCalculatedResultParam existParam = modifier.GetCalculatedResultParam_Bone(
							//										modBone._meshGroup_Bone, modBone._bone);

							//변경
							apOptCalculatedResultParam existParam = modifier.GetCalculatedResultParam_Bone(
																	rootOptTransform, modBone._bone, modBone._meshGroup_Bone);

							
							if (existParam != null)
							{
								//이미 있다면 ModBone만 추가해주자
								existParam.AddParamSetAndModifiedValue(paramSetGroup, paramSet, null, modBone, null);
								//Debug.LogWarning(" < Exist > - Mod Bone [" + modBone._bone._name + " << " + modBone._meshGroup_Bone._name + "]");
							}
							else
							{
								//Debug.Log("Mod Bone -> Calculate Param 등록");
								//새로 CalculateParam을 만들고
								//<BONE_EDIT>
								//apOptCalculatedResultParam newCalParam = new apOptCalculatedResultParam(
								//	modifier._calculatedValueType,
								//	modifier._calculatedSpace,
								//	modifier,
								//	modBone._meshGroup_Bone,
								//	modBone._meshGroup_Bone._childMesh,
								//	modBone._bone,
								//	null//WeightedVertex
								//	);

								//Debug.LogError(" < New > - Mod Bone [" + modBone._bone._name + " << " + modBone._meshGroup_Bone._name + "]");

								apOptCalculatedResultParam newCalParam = new apOptCalculatedResultParam(
									modifier._calculatedValueType,
									modifier._calculatedSpace,
									modifier,
									rootOptTransform,//<<변경
									modBone._meshGroup_Bone,//추가
									modBone._meshGroup_Bone._childMesh,
									modBone._bone
									//null//WeightedVertex > 19.5.20 : 삭제
									);

								newCalParam.AddParamSetAndModifiedValue(paramSetGroup, paramSet, null, modBone, null);

								// Modifier에 등록하고
								modifier._calculatedResultParams.Add(newCalParam);

								//RenderUnit에도 등록을 하자
								//<BONE_EDIT>
								//modBone._meshGroup_Bone.CalculatedStack.AddCalculatedResultParam(newCalParam);

								//변경
								rootOptTransform.CalculatedStack.AddCalculatedResultParam(newCalParam);
							}
						}

					}
				}

				//Modifier에서
				//SubList를 한번 정렬하자
				for (int iCal = 0; iCal < modifier._calculatedResultParams.Count; iCal++)
				{
					modifier._calculatedResultParams[iCal].SortSubList();
				}
			}

			
			//추가>>
			//하위 객체에 대해서도 Link를 자동으로 수행한다.
			//다 끝나고 Sort
			apOptTransform childTransform = null;
			if (_parentTransform != null)
			{
				if (isRecursive)
				{
					if (_parentTransform._childTransforms != null && _parentTransform._childTransforms.Length > 0)
					{
						for (int i = 0; i < _parentTransform._childTransforms.Length; i++)
						{
							childTransform = _parentTransform._childTransforms[i];
							if (childTransform._unitType == apOptTransform.UNIT_TYPE.Group)
							{
								if (childTransform != _parentTransform)
								{
									//<<여기서도 같이 수행
									
									childTransform._modifierStack.LinkModifierStackToRenderUnitCalculateStack(false, rootOptTransform, true);
								}
							}
						}
					}
				}

				if (isRoot)
				{
					//Root인 경우
					//RenderUnit들을 검사하면서 Calculated Stack에 대해서 Sort를 해주자
					SortAllCalculatedStack(_parentTransform);
				}
			}
			else
			{
				Debug.LogError("<<<< Error : Mod Link시 Parent Transform이 연결이 안되었다");
			}
		}


		private void SortAllCalculatedStack(apOptTransform parentTransform)
		{
			if (parentTransform == null)
			{
				return;
			}


			parentTransform.CalculatedStack.Sort();
			apOptTransform childTransform = null;

			if (parentTransform._childTransforms != null && parentTransform._childTransforms.Length > 0)
			{
				for (int i = 0; i < parentTransform._childTransforms.Length; i++)
				{
					childTransform = parentTransform._childTransforms[i];

					if (childTransform._unitType == apOptTransform.UNIT_TYPE.Group && childTransform != parentTransform)
					{
						SortAllCalculatedStack(childTransform);
					}
				}
			}
		}

		// Functions
		//--------------------------------------------
		public void Update_Pre(float tDelta)
		{
			//Debug.Log("Pre >>");
			for (int i = 0; i < _nModifiers; i++)
			{
				if (!_modifiers[i].IsPreUpdate)
				{
					//Post-Update라면 패스
					continue;
				}
				if (_modifiers[i]._isActive)
				{
					//Debug.Log("[" + i + "] Pre Update - " + _modifiers[i]._modifierType + " (IsPre:" + _modifiers[i].IsPreUpdate + ")");
					_modifiers[i].Calculate(tDelta);//<<
				}
				else
				{
					_modifiers[i].InitCalcualte(tDelta);
				}
			}
			//Debug.Log(">> Pre");
		}


		public void Update_Post(float tDelta)
		{
			//Debug.Log("Post >>");
			for (int i = 0; i < _nModifiers; i++)
			{
				if (_modifiers[i].IsPreUpdate)
				{
					//Pre-Update라면 패스
					continue;
				}

				if (_modifiers[i]._isActive)
				{
					_modifiers[i].Calculate(tDelta);//<<
				}
				else
				{
					_modifiers[i].InitCalcualte(tDelta);
				}
			}
			//Debug.Log(">> Post");
		}


		// Get / Set
		//--------------------------------------------
		public apOptModifierUnitBase GetModifier(int uniqueID)
		{
			return _modifiers.Find(delegate (apOptModifierUnitBase a)
			{
				return a._uniqueID == uniqueID;
			});
		}
	}
}