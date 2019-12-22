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
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif
using AnyPortrait;

#if UNITY_EDITOR
namespace AnyPortrait
{
	/// <summary>
	/// MaterialSet을 설정하는데 도움을 주는 Library 클래스
	/// apEditor에서 관리하고 있다.
	/// Portrait에 있는 MaterialSet 정보와 파일 정보를 열어서 편집할 수 있게 해준다.
	/// </summary>
	public class apMaterialLibrary
	{
		// Members
		//------------------------------------------------------
		private List<apMaterialSet> _presets = new List<apMaterialSet>();
		public List<apMaterialSet> Presets { get {  return _presets; } }

		private bool _isFirstLoad = false;

		//예약된 재질 설정
		public const int RESERVED_MAT_ID__Unlit = 0;
		public const int RESERVED_MAT_ID__Lit = 11;
		public const int RESERVED_MAT_ID__Bumped = 12;
		public const int RESERVED_MAT_ID__Bumped_Specular = 13;
		public const int RESERVED_MAT_ID__Bumped_Specular_Emissive = 14;
		public const int RESERVED_MAT_ID__Bumped_Rimlight = 15;
		public const int RESERVED_MAT_ID__Bumped_Ramp = 16;
		public const int RESERVED_MAT_ID__LWRP_Unlit = 51;
		public const int RESERVED_MAT_ID__LWRP_2D_Lit = 52;
		public const int RESERVED_MAT_ID__KeepAlpha_Unlit = 61;
		public const int RESERVED_MAT_ID__KeepAlpha_Lit = 62;
		public const int RESERVED_MAT_ID__VR_Unlit = 71;
		public const int RESERVED_MAT_ID__VR_Lit = 72;
		public const int RESERVED_MAT_ID__VR_Unlit_ScaleOffset = 73;
		public const int RESERVED_MAT_ID__VR_Lit_ScaleOffset = 74;

		// Init
		//------------------------------------------------------
		public apMaterialLibrary()
		{
			if(_presets == null)
			{
				_presets = new List<apMaterialSet>();
			}
			ClearPresets();

			_isFirstLoad = false;
		}


		public void ClearPresets()
		{
			//Debug.Log("Clear MaterialLibrary Presets");
			_presets.Clear();
		}


		// Functions
		//------------------------------------------------------
		private void MakeReservedPresets()
		{
			//Debug.Log("Make Reserved MaterialLibrary Presets");
			//ClearPresets();

			//1. 기본 Unlit 타입의 MaterialSet
			MakeReservedPreset(RESERVED_MAT_ID__Unlit, "Unlit (Default)", apMaterialSet.ICON.Unlit,
												"apShader_Transparent",
												"apShader_Transparent_Additive",
												"apShader_Transparent_SoftAdditive",
												"apShader_Transparent_Multiplicative",
												"apShader_ClippedWithMask",
												"apShader_ClippedWithMask_Additive",
												"apShader_ClippedWithMask_SoftAdditive",
												"apShader_ClippedWithMask_Multiplicative",
												"Linear/apShader_L_Transparent",
												"Linear/apShader_L_Transparent_Additive",
												"Linear/apShader_L_Transparent_SoftAdditive",
												"Linear/apShader_L_Transparent_Multiplicative",
												"Linear/apShader_L_ClippedWithMask",
												"Linear/apShader_L_ClippedWithMask_Additive",
												"Linear/apShader_L_ClippedWithMask_SoftAdditive",
												"Linear/apShader_L_ClippedWithMask_Multiplicative",
												"apShader_AlphaMask",
												true);

			


			//Advanced 패키지가 로드되었다는 가정하에
			if(	GetPresetUnit(RESERVED_MAT_ID__Lit) != null ||
				GetPresetUnit(RESERVED_MAT_ID__Bumped) != null ||
				GetPresetUnit(RESERVED_MAT_ID__Bumped_Specular) != null ||
				GetPresetUnit(RESERVED_MAT_ID__Bumped_Specular_Emissive) != null ||
				GetPresetUnit(RESERVED_MAT_ID__Bumped_Rimlight) != null ||
				GetPresetUnit(RESERVED_MAT_ID__Bumped_Ramp) != null)
			{
				MakeReserved_Advanced(true);
			}

			//LWRP 패키지가 로드되었다는 가정하에
			if(GetPresetUnit(RESERVED_MAT_ID__LWRP_Unlit) != null)
			{	
				MakeReserved_LWRPUnlit(true);
			}

			//LWRP 2D 패키지가 로드되었다는 가정하에
			if(GetPresetUnit(RESERVED_MAT_ID__LWRP_2D_Lit) != null)
			{	
				MakeReserved_LWRP_2DLit(true);
			}

			//추가 19.10.6 : VR 패키지가 로드되었다는 가정하에
			if(GetPresetUnit(RESERVED_MAT_ID__VR_Unlit) != null ||
				GetPresetUnit(RESERVED_MAT_ID__VR_Lit) != null ||
				GetPresetUnit(RESERVED_MAT_ID__VR_Unlit_ScaleOffset) != null ||
				GetPresetUnit(RESERVED_MAT_ID__VR_Lit_ScaleOffset) != null)
			{	
				MakeReserved_VR(true);
			}

			//추가 19.10.27 : KeepAlpha 패키지 확인 후 초기화
			if(GetPresetUnit(RESERVED_MAT_ID__KeepAlpha_Unlit) != null ||
				GetPresetUnit(RESERVED_MAT_ID__KeepAlpha_Lit) != null)
			{	
				MakeReserved_KeepAlpha(true);
			}
		}


		private apMaterialSet MakeReservedPreset(int uniqueID, 
											string name, 
											apMaterialSet.ICON icon,
											string shaderPath_Normal_AlphaBlend,
											string shaderPath_Normal_Additive,
											string shaderPath_Normal_SoftAdditive,
											string shaderPath_Normal_Multiplicative,
											string shaderPath_Clipped_AlphaBlend,
											string shaderPath_Clipped_Additive,
											string shaderPath_Clipped_SoftAdditive,
											string shaderPath_Clipped_Multiplicative,
											string shaderPath_L_Normal_AlphaBlend,
											string shaderPath_L_Normal_Additive,
											string shaderPath_L_Normal_SoftAdditive,
											string shaderPath_L_Normal_Multiplicative,
											string shaderPath_L_Clipped_AlphaBlend,
											string shaderPath_L_Clipped_Additive,
											string shaderPath_L_Clipped_SoftAdditive,
											string shaderPath_L_Clipped_Multiplicative,
											string shaderPath_AlphaMask,
											bool isNeedToSetBlackColoredAmbient,
											bool isVRSupported = false)
		{

			apMaterialSet materialSet = GetPresetUnit(uniqueID);
			if(materialSet == null)
			{
				materialSet = new apMaterialSet();
				_presets.Add(materialSet);
			}

			materialSet.MakeReserved(	uniqueID, 
										name, 
										icon,
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_Normal_AlphaBlend + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_Normal_Additive + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_Normal_SoftAdditive + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_Normal_Multiplicative + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_Clipped_AlphaBlend + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_Clipped_Additive + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_Clipped_SoftAdditive + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_Clipped_Multiplicative + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_L_Normal_AlphaBlend + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_L_Normal_Additive + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_L_Normal_SoftAdditive + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_L_Normal_Multiplicative + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_L_Clipped_AlphaBlend + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_L_Clipped_Additive + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_L_Clipped_SoftAdditive + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_L_Clipped_Multiplicative + ".shader",
										"Assets/AnyPortrait/Assets/Shaders/" + shaderPath_AlphaMask + ".shader",
										isNeedToSetBlackColoredAmbient);

			

			//공통된 기본 프로퍼티 추가
			materialSet.AddProperty("_Color", true, apMaterialSet.SHADER_PROP_TYPE.Color);
			materialSet.AddProperty("_MainTex", true, apMaterialSet.SHADER_PROP_TYPE.Texture);
			if(!isVRSupported)
			{
				materialSet.AddProperty("_MaskTex", true, apMaterialSet.SHADER_PROP_TYPE.Texture);
			}
			else
			{
				materialSet.AddProperty("_MaskTex_L", true, apMaterialSet.SHADER_PROP_TYPE.Texture);
				materialSet.AddProperty("_MaskTex_R", true, apMaterialSet.SHADER_PROP_TYPE.Texture);
			}
			
			materialSet.AddProperty("_MaskScreenSpaceOffset", true, apMaterialSet.SHADER_PROP_TYPE.Vector);

			return materialSet;
		}
		




		public apMaterialSet AddNewPreset(apMaterialSet srcMatSet, bool isFromPreset, string name)
		{
			int newID = GetNewCustomID();
			if (newID < 0)
			{
				return null;
			}

			if (string.IsNullOrEmpty(name))
			{
				name = "<No Name>";
			}

			apMaterialSet matSet = new apMaterialSet();
			
			if(srcMatSet != null)
			{
				matSet.CopyFromSrc(srcMatSet, newID, isFromPreset, true, false);
				matSet._name = name;
			}
			else
			{
				matSet.Init();

				//초기화 후 ID와 이름 설정
				matSet._uniqueID = newID;
				matSet._name = name;

				matSet.AddProperty("_Color", true, apMaterialSet.SHADER_PROP_TYPE.Color);
				matSet.AddProperty("_MainTex", true, apMaterialSet.SHADER_PROP_TYPE.Texture);
				matSet.AddProperty("_MaskTex", true, apMaterialSet.SHADER_PROP_TYPE.Texture);
				matSet.AddProperty("_MaskScreenSpaceOffset", true, apMaterialSet.SHADER_PROP_TYPE.Vector);
			}
			
			_presets.Add(matSet);

			
			Save();

			return matSet;
		}

		public void RemovePreset(int targetUniqueID)
		{
			//apPhysicsPresetUnit targetUnit = GetPresetUnit(targetUniqueID);
			//if (targetUnit == null)
			//{
			//	return;
			//}
			//if (targetUnit._isReserved)
			//{
			//	return;
			//}

			//_units.Remove(targetUnit);
			Save();

		}


		private int GetNewCustomID()
		{
			//사용자 ID는 100부터 시작
			//랜덤값으로 만듭시다.
			//랜덤은 1000부터 99999
			//에러시 ID는 200부터 999
			int cnt = 0;
			while (true)
			{
				int nextID = UnityEngine.Random.Range(1000, 999999);
				if (GetPresetUnit(nextID) == null)
				{
					//엥 없네영. 사용 가능
					return nextID;
				}

				cnt++;
				if(cnt > 100)
				{
					//100번이 되도록 ID가 중복이 되었다구요?
					break;
				}
			}
			//최대 999
			//200부터 하나씩 돌면서 체크
			for (int iCurID = 200; iCurID <= 999; iCurID++)
			{
				int nextID = iCurID;
				if (GetPresetUnit(nextID) == null)
				{
					//엥 없네영. 사용 가능
					return nextID;
				}
			}
			return -1;//ID 얻기 실패
		}



		// 외부에서 추가된 Shader로 Reserved Preset 만들기
		//------------------------------------------------------------------------------------------
		/// <summary>
		/// Advanced Shader들이 포함된 프리셋을 만든다.
		/// 인자 : true이면 이미 등록된 경우에만 갱신, false이면 강제로 만든다.
		/// </summary>
		/// <param name="makeWhenAlreadyAdded">true이면 이미 등록된 경우에만 갱신, false이면 강제로 만든다.</param>
		public void MakeReserved_Advanced(bool makeWhenAlreadyAdded)
		{
			// 2. Lit
			if ((!makeWhenAlreadyAdded) || (GetPresetUnit(RESERVED_MAT_ID__Lit) != null))
			{
				//Lit를 추가하거나 갱신할 수 있을 때
				MakeReservedPreset(RESERVED_MAT_ID__Lit, "Lit", apMaterialSet.ICON.Lit,
												"Advanced/Lit/apShader_Lit_T_Alpha",
												"Advanced/Lit/apShader_Lit_T_Add",
												"Advanced/Lit/apShader_Lit_T_Soft",
												"Advanced/Lit/apShader_Lit_T_Mul",
												"Advanced/Lit/apShader_Lit_C_Alpha",
												"Advanced/Lit/apShader_Lit_C_Add",
												"Advanced/Lit/apShader_Lit_C_Soft",
												"Advanced/Lit/apShader_Lit_C_Mul",
												"Advanced/Lit/Linear/apShader_L_Lit_T_Alpha",
												"Advanced/Lit/Linear/apShader_L_Lit_T_Add",
												"Advanced/Lit/Linear/apShader_L_Lit_T_Soft",
												"Advanced/Lit/Linear/apShader_L_Lit_T_Mul",
												"Advanced/Lit/Linear/apShader_L_Lit_C_Alpha",
												"Advanced/Lit/Linear/apShader_L_Lit_C_Add",
												"Advanced/Lit/Linear/apShader_L_Lit_C_Soft",
												"Advanced/Lit/Linear/apShader_L_Lit_C_Mul",
												"apShader_AlphaMask",
												false);
			}

			// 3. Bumped
			if ((!makeWhenAlreadyAdded) || (GetPresetUnit(RESERVED_MAT_ID__Bumped) != null))
			{
				//Bumped를 추가하거나 갱신할 수 있을 때
				apMaterialSet matSet_Bumped = MakeReservedPreset(RESERVED_MAT_ID__Bumped, "Bumped", apMaterialSet.ICON.Lit,
													"Advanced/Bumped/apShader_Bumped_T_Alpha",
													"Advanced/Bumped/apShader_Bumped_T_Add",
													"Advanced/Bumped/apShader_Bumped_T_Soft",
													"Advanced/Bumped/apShader_Bumped_T_Mul",
													"Advanced/Bumped/apShader_Bumped_C_Alpha",
													"Advanced/Bumped/apShader_Bumped_C_Add",
													"Advanced/Bumped/apShader_Bumped_C_Soft",
													"Advanced/Bumped/apShader_Bumped_C_Mul",
													"Advanced/Bumped/Linear/apShader_L_Bumped_T_Alpha",
													"Advanced/Bumped/Linear/apShader_L_Bumped_T_Add",
													"Advanced/Bumped/Linear/apShader_L_Bumped_T_Soft",
													"Advanced/Bumped/Linear/apShader_L_Bumped_T_Mul",
													"Advanced/Bumped/Linear/apShader_L_Bumped_C_Alpha",
													"Advanced/Bumped/Linear/apShader_L_Bumped_C_Add",
													"Advanced/Bumped/Linear/apShader_L_Bumped_C_Soft",
													"Advanced/Bumped/Linear/apShader_L_Bumped_C_Mul",
													"apShader_AlphaMask",
													false);

				matSet_Bumped.AddProperty_Texture("_BumpMap", false, false);
			}


			// 4. Bumped Specular
			if ((!makeWhenAlreadyAdded) || (GetPresetUnit(RESERVED_MAT_ID__Bumped_Specular) != null))
			{
				//Bumped Specular를 추가하거나 갱신할 수 있을 때
				apMaterialSet matSet_BumpedSpecular = MakeReservedPreset(RESERVED_MAT_ID__Bumped_Specular, "Bumped Specular", apMaterialSet.ICON.LitSpecular,
												"Advanced/BumpedSpecular/apShader_BumpSpec_T_Alpha",
												"Advanced/BumpedSpecular/apShader_BumpSpec_T_Add",
												"Advanced/BumpedSpecular/apShader_BumpSpec_T_Soft",
												"Advanced/BumpedSpecular/apShader_BumpSpec_T_Mul",
												"Advanced/BumpedSpecular/apShader_BumpSpec_C_Alpha",
												"Advanced/BumpedSpecular/apShader_BumpSpec_C_Add",
												"Advanced/BumpedSpecular/apShader_BumpSpec_C_Soft",
												"Advanced/BumpedSpecular/apShader_BumpSpec_C_Mul",
												"Advanced/BumpedSpecular/Linear/apShader_L_BumpSpec_T_Alpha",
												"Advanced/BumpedSpecular/Linear/apShader_L_BumpSpec_T_Add",
												"Advanced/BumpedSpecular/Linear/apShader_L_BumpSpec_T_Soft",
												"Advanced/BumpedSpecular/Linear/apShader_L_BumpSpec_T_Mul",
												"Advanced/BumpedSpecular/Linear/apShader_L_BumpSpec_C_Alpha",
												"Advanced/BumpedSpecular/Linear/apShader_L_BumpSpec_C_Add",
												"Advanced/BumpedSpecular/Linear/apShader_L_BumpSpec_C_Soft",
												"Advanced/BumpedSpecular/Linear/apShader_L_BumpSpec_C_Mul",
												"apShader_AlphaMask",
												false);

				matSet_BumpedSpecular.AddProperty_Texture("_BumpMap", false, false);
				matSet_BumpedSpecular.AddProperty("_SpecularPower", false, apMaterialSet.SHADER_PROP_TYPE.Float).SetFloat(5.0f);
				matSet_BumpedSpecular.AddProperty_Texture("_SpecularMap", false, false);
			}


			// 5. Bumped Specular Emission
			if ((!makeWhenAlreadyAdded) || (GetPresetUnit(RESERVED_MAT_ID__Bumped_Specular_Emissive) != null))
			{
				//Bumped Specular Emission를 추가하거나 갱신할 수 있을 때

				apMaterialSet matSet_BSE = MakeReservedPreset(RESERVED_MAT_ID__Bumped_Specular_Emissive, "Bumped Specular Emissison", apMaterialSet.ICON.LitSpecularEmission,
													"Advanced/BumpedSpecularEmission/apShader_BSE_T_Alpha",
													"Advanced/BumpedSpecularEmission/apShader_BSE_T_Add",
													"Advanced/BumpedSpecularEmission/apShader_BSE_T_Soft",
													"Advanced/BumpedSpecularEmission/apShader_BSE_T_Mul",
													"Advanced/BumpedSpecularEmission/apShader_BSE_C_Alpha",
													"Advanced/BumpedSpecularEmission/apShader_BSE_C_Add",
													"Advanced/BumpedSpecularEmission/apShader_BSE_C_Soft",
													"Advanced/BumpedSpecularEmission/apShader_BSE_C_Mul",
													"Advanced/BumpedSpecularEmission/Linear/apShader_L_BSE_T_Alpha",
													"Advanced/BumpedSpecularEmission/Linear/apShader_L_BSE_T_Add",
													"Advanced/BumpedSpecularEmission/Linear/apShader_L_BSE_T_Soft",
													"Advanced/BumpedSpecularEmission/Linear/apShader_L_BSE_T_Mul",
													"Advanced/BumpedSpecularEmission/Linear/apShader_L_BSE_C_Alpha",
													"Advanced/BumpedSpecularEmission/Linear/apShader_L_BSE_C_Add",
													"Advanced/BumpedSpecularEmission/Linear/apShader_L_BSE_C_Soft",
													"Advanced/BumpedSpecularEmission/Linear/apShader_L_BSE_C_Mul",
													"apShader_AlphaMask",
													false);

				matSet_BSE.AddProperty_Texture("_BumpMap", false, false);
				matSet_BSE.AddProperty("_SpecularPower", false, apMaterialSet.SHADER_PROP_TYPE.Float).SetFloat(5.0f);
				matSet_BSE.AddProperty_Texture("_SpecularMap", false, false);
				matSet_BSE.AddProperty("_EmissionColor", false, apMaterialSet.SHADER_PROP_TYPE.Color).SetColor(Color.white);
				matSet_BSE.AddProperty_Texture("_EmissionMap", false, false);
			}



			// 6. Bumped Rimlight
			if ((!makeWhenAlreadyAdded) || (GetPresetUnit(RESERVED_MAT_ID__Bumped_Rimlight) != null))
			{
				//Bumped Rimlight를 추가하거나 갱신할 수 있을 때
				apMaterialSet matSet_BRimlight = MakeReservedPreset(RESERVED_MAT_ID__Bumped_Rimlight, "Bumped Rimlight", apMaterialSet.ICON.LitRimlight,
													"Advanced/BumpedRimlight/apShader_BumpRim_T_Alpha",
													"Advanced/BumpedRimlight/apShader_BumpRim_T_Add",
													"Advanced/BumpedRimlight/apShader_BumpRim_T_Soft",
													"Advanced/BumpedRimlight/apShader_BumpRim_T_Mul",
													"Advanced/BumpedRimlight/apShader_BumpRim_C_Alpha",
													"Advanced/BumpedRimlight/apShader_BumpRim_C_Add",
													"Advanced/BumpedRimlight/apShader_BumpRim_C_Soft",
													"Advanced/BumpedRimlight/apShader_BumpRim_C_Mul",
													"Advanced/BumpedRimlight/Linear/apShader_L_BumpRim_T_Alpha",
													"Advanced/BumpedRimlight/Linear/apShader_L_BumpRim_T_Add",
													"Advanced/BumpedRimlight/Linear/apShader_L_BumpRim_T_Soft",
													"Advanced/BumpedRimlight/Linear/apShader_L_BumpRim_T_Mul",
													"Advanced/BumpedRimlight/Linear/apShader_L_BumpRim_C_Alpha",
													"Advanced/BumpedRimlight/Linear/apShader_L_BumpRim_C_Add",
													"Advanced/BumpedRimlight/Linear/apShader_L_BumpRim_C_Soft",
													"Advanced/BumpedRimlight/Linear/apShader_L_BumpRim_C_Mul",
													"apShader_AlphaMask",
													false);

				matSet_BRimlight.AddProperty_Texture("_BumpMap", false, false);
				matSet_BRimlight.AddProperty("_RimPower", false, apMaterialSet.SHADER_PROP_TYPE.Float).SetFloat(2.0f);
				matSet_BRimlight.AddProperty("_RimColor", false, apMaterialSet.SHADER_PROP_TYPE.Color).SetColor(Color.white);
			}


			// 7. Bumped Ramp
			if ((!makeWhenAlreadyAdded) || (GetPresetUnit(RESERVED_MAT_ID__Bumped_Ramp) != null))
			{
				//Bumped Ramp를 추가하거나 갱신할 수 있을 때
				apMaterialSet matSet_BumpRamp = MakeReservedPreset(RESERVED_MAT_ID__Bumped_Ramp, "Bumped Ramp", apMaterialSet.ICON.LitRamp,
													"Advanced/BumpedRamp/apShader_BumpRamp_T_Alpha",
													"Advanced/BumpedRamp/apShader_BumpRamp_T_Add",
													"Advanced/BumpedRamp/apShader_BumpRamp_T_Soft",
													"Advanced/BumpedRamp/apShader_BumpRamp_T_Mul",
													"Advanced/BumpedRamp/apShader_BumpRamp_C_Alpha",
													"Advanced/BumpedRamp/apShader_BumpRamp_C_Add",
													"Advanced/BumpedRamp/apShader_BumpRamp_C_Soft",
													"Advanced/BumpedRamp/apShader_BumpRamp_C_Mul",
													"Advanced/BumpedRamp/Linear/apShader_L_BumpRamp_T_Alpha",
													"Advanced/BumpedRamp/Linear/apShader_L_BumpRamp_T_Add",
													"Advanced/BumpedRamp/Linear/apShader_L_BumpRamp_T_Soft",
													"Advanced/BumpedRamp/Linear/apShader_L_BumpRamp_T_Mul",
													"Advanced/BumpedRamp/Linear/apShader_L_BumpRamp_C_Alpha",
													"Advanced/BumpedRamp/Linear/apShader_L_BumpRamp_C_Add",
													"Advanced/BumpedRamp/Linear/apShader_L_BumpRamp_C_Soft",
													"Advanced/BumpedRamp/Linear/apShader_L_BumpRamp_C_Mul",
													"apShader_AlphaMask",
													false);

				matSet_BumpRamp.AddProperty_Texture("_BumpMap", false, false);
				matSet_BumpRamp.AddProperty_Texture("_RampMap", false, true);
			}
		}

		public bool MakeReserved_LWRPUnlit(bool isCheckShader)
		{
			//필요한 Shader가 모두 존재하는지 확인
			if (isCheckShader)
			{
				if (!IsShaderImported("Advanced/LWRP Unlit/apShader_LWRPUnlit_AlphaMask") ||

					!IsShaderImported("Advanced/LWRP Unlit/apShader_LWRPUnlit_T_Alpha") ||
					!IsShaderImported("Advanced/LWRP Unlit/apShader_LWRPUnlit_T_Additive") ||
					!IsShaderImported("Advanced/LWRP Unlit/apShader_LWRPUnlit_T_SoftAdditive") ||
					!IsShaderImported("Advanced/LWRP Unlit/apShader_LWRPUnlit_T_Multiplicative") ||
					!IsShaderImported("Advanced/LWRP Unlit/apShader_LWRPUnlit_C_Alpha") ||
					!IsShaderImported("Advanced/LWRP Unlit/apShader_LWRPUnlit_C_Additive") ||
					!IsShaderImported("Advanced/LWRP Unlit/apShader_LWRPUnlit_C_SoftAdditive") ||
					!IsShaderImported("Advanced/LWRP Unlit/apShader_LWRPUnlit_C_Multiplicative") ||

					!IsShaderImported("Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_T_Alpha") ||
					!IsShaderImported("Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_T_Additive") ||
					!IsShaderImported("Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_T_SoftAdditive") ||
					!IsShaderImported("Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_T_Multiplicative") ||
					!IsShaderImported("Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_C_Alpha") ||
					!IsShaderImported("Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_C_Additive") ||
					!IsShaderImported("Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_C_SoftAdditive") ||
					!IsShaderImported("Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_C_Multiplicative")
					)
				{
					//하나라도 존재하지 않으면 False
					return false;
				}
			}

			MakeReservedPreset(RESERVED_MAT_ID__LWRP_Unlit, "LWRP Unlit", apMaterialSet.ICON.Unlit, 
								"Advanced/LWRP Unlit/apShader_LWRPUnlit_T_Alpha",
								"Advanced/LWRP Unlit/apShader_LWRPUnlit_T_Additive",
								"Advanced/LWRP Unlit/apShader_LWRPUnlit_T_SoftAdditive",
								"Advanced/LWRP Unlit/apShader_LWRPUnlit_T_Multiplicative",
								"Advanced/LWRP Unlit/apShader_LWRPUnlit_C_Alpha",
								"Advanced/LWRP Unlit/apShader_LWRPUnlit_C_Additive",
								"Advanced/LWRP Unlit/apShader_LWRPUnlit_C_SoftAdditive",
								"Advanced/LWRP Unlit/apShader_LWRPUnlit_C_Multiplicative",

								"Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_T_Alpha",
								"Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_T_Additive",
								"Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_T_SoftAdditive",
								"Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_T_Multiplicative",
								"Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_C_Alpha",
								"Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_C_Additive",
								"Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_C_SoftAdditive",
								"Advanced/LWRP Unlit/Linear/apShader_L_LWRPUnlit_C_Multiplicative",
								"Advanced/LWRP Unlit/apShader_LWRPUnlit_AlphaMask",
								true);

			return true;
		}

		public bool MakeReserved_LWRP_2DLit(bool isCheckShader)
		{
			//필요한 Shader가 모두 존재하는지 확인
			if (isCheckShader)
			{
				if (!IsShaderImported("Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_AlphaMask") ||

					!IsShaderImported("Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_AlphaBlend") ||
					!IsShaderImported("Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_Additive") ||
					!IsShaderImported("Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_SoftAdditive") ||
					!IsShaderImported("Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_Multiplicative") ||
					!IsShaderImported("Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_AlphaBlend") ||
					!IsShaderImported("Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_Additive") ||
					!IsShaderImported("Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_SoftAdditive") ||
					!IsShaderImported("Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_Multiplicative")
					)
				{
					//하나라도 존재하지 않으면 False
					return false;
				}
			}

			MakeReservedPreset(RESERVED_MAT_ID__LWRP_2D_Lit, "LWRP 2D Lit", apMaterialSet.ICON.Lit, 
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_AlphaBlend",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_Additive",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_SoftAdditive",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_Multiplicative",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_AlphaBlend",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_Additive",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_SoftAdditive",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_Multiplicative",

								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_AlphaBlend",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_Additive",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_SoftAdditive",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_T_Multiplicative",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_AlphaBlend",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_Additive",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_SoftAdditive",
								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_Lit_C_Multiplicative",

								"Advanced/LWRP 2D Lit (Experimental)/apShader_LWRP2D_AlphaMask",
								false);

			return true;
		}


		public void MakeReserved_VR(bool isCheckShader)
		{
			//Unlit
			if ((!isCheckShader) || (GetPresetUnit(RESERVED_MAT_ID__VR_Unlit) != null))
			{
				MakeReservedPreset(RESERVED_MAT_ID__VR_Unlit, "VR Unlit", apMaterialSet.ICON.UnlitVR, 
								"Advanced/UnlitVR/apShader_UnlitVR_T_Alpha",
								"Advanced/UnlitVR/apShader_UnlitVR_T_Add",
								"Advanced/UnlitVR/apShader_UnlitVR_T_Soft",
								"Advanced/UnlitVR/apShader_UnlitVR_T_Mul",
								"Advanced/UnlitVR/apShader_UnlitVR_C_Alpha",
								"Advanced/UnlitVR/apShader_UnlitVR_C_Add",
								"Advanced/UnlitVR/apShader_UnlitVR_C_Soft",
								"Advanced/UnlitVR/apShader_UnlitVR_C_Mul",

								"Advanced/UnlitVR/Linear/apShader_L_UnlitVR_T_Alpha",
								"Advanced/UnlitVR/Linear/apShader_L_UnlitVR_T_Add",
								"Advanced/UnlitVR/Linear/apShader_L_UnlitVR_T_Soft",
								"Advanced/UnlitVR/Linear/apShader_L_UnlitVR_T_Mul",
								"Advanced/UnlitVR/Linear/apShader_L_UnlitVR_C_Alpha",
								"Advanced/UnlitVR/Linear/apShader_L_UnlitVR_C_Add",
								"Advanced/UnlitVR/Linear/apShader_L_UnlitVR_C_Soft",
								"Advanced/UnlitVR/Linear/apShader_L_UnlitVR_C_Mul",

								"apShader_AlphaMask", 
								true,
								true);
			}
			
			//Lit
			if ((!isCheckShader) || (GetPresetUnit(RESERVED_MAT_ID__VR_Lit) != null))
			{
				MakeReservedPreset(RESERVED_MAT_ID__VR_Lit, "VR Lit", apMaterialSet.ICON.LitVR, 
								"Advanced/LitVR/apShader_LitVR_T_Alpha",
								"Advanced/LitVR/apShader_LitVR_T_Add",
								"Advanced/LitVR/apShader_LitVR_T_Soft",
								"Advanced/LitVR/apShader_LitVR_T_Mul",
								"Advanced/LitVR/apShader_LitVR_C_Alpha",
								"Advanced/LitVR/apShader_LitVR_C_Add",
								"Advanced/LitVR/apShader_LitVR_C_Soft",
								"Advanced/LitVR/apShader_LitVR_C_Mul",

								"Advanced/LitVR/Linear/apShader_L_LitVR_T_Alpha",
								"Advanced/LitVR/Linear/apShader_L_LitVR_T_Add",
								"Advanced/LitVR/Linear/apShader_L_LitVR_T_Soft",
								"Advanced/LitVR/Linear/apShader_L_LitVR_T_Mul",
								"Advanced/LitVR/Linear/apShader_L_LitVR_C_Alpha",
								"Advanced/LitVR/Linear/apShader_L_LitVR_C_Add",
								"Advanced/LitVR/Linear/apShader_L_LitVR_C_Soft",
								"Advanced/LitVR/Linear/apShader_L_LitVR_C_Mul",

								"apShader_AlphaMask", 
								false,
								true);
			}

			//Unlit (with ScaleOffset)
			if ((!isCheckShader) || (GetPresetUnit(RESERVED_MAT_ID__VR_Unlit_ScaleOffset) != null))
			{
				MakeReservedPreset(RESERVED_MAT_ID__VR_Unlit_ScaleOffset, "VR Unlit (with ScaleOffset)", apMaterialSet.ICON.UnlitVR, 
								"Advanced/UnlitVRWithScaleOffset/apShader_UnlitVRSO_T_Alpha",
								"Advanced/UnlitVRWithScaleOffset/apShader_UnlitVRSO_T_Add",
								"Advanced/UnlitVRWithScaleOffset/apShader_UnlitVRSO_T_Soft",
								"Advanced/UnlitVRWithScaleOffset/apShader_UnlitVRSO_T_Mul",
								"Advanced/UnlitVRWithScaleOffset/apShader_UnlitVRSO_C_Alpha",
								"Advanced/UnlitVRWithScaleOffset/apShader_UnlitVRSO_C_Add",
								"Advanced/UnlitVRWithScaleOffset/apShader_UnlitVRSO_C_Soft",
								"Advanced/UnlitVRWithScaleOffset/apShader_UnlitVRSO_C_Mul",

								"Advanced/UnlitVRWithScaleOffset/Linear/apShader_L_UnlitVRSO_T_Alpha",
								"Advanced/UnlitVRWithScaleOffset/Linear/apShader_L_UnlitVRSO_T_Add",
								"Advanced/UnlitVRWithScaleOffset/Linear/apShader_L_UnlitVRSO_T_Soft",
								"Advanced/UnlitVRWithScaleOffset/Linear/apShader_L_UnlitVRSO_T_Mul",
								"Advanced/UnlitVRWithScaleOffset/Linear/apShader_L_UnlitVRSO_C_Alpha",
								"Advanced/UnlitVRWithScaleOffset/Linear/apShader_L_UnlitVRSO_C_Add",
								"Advanced/UnlitVRWithScaleOffset/Linear/apShader_L_UnlitVRSO_C_Soft",
								"Advanced/UnlitVRWithScaleOffset/Linear/apShader_L_UnlitVRSO_C_Mul",

								"apShader_AlphaMask", 
								true,
								true);
			}
			
			//Lit (with ScaleOffset)
			if ((!isCheckShader) || (GetPresetUnit(RESERVED_MAT_ID__VR_Lit_ScaleOffset) != null))
			{
				MakeReservedPreset(RESERVED_MAT_ID__VR_Lit_ScaleOffset, "VR Lit (with ScaleOffset)", apMaterialSet.ICON.LitVR, 
								"Advanced/LitVRWithScaleOffset/apShader_LitVRSO_T_Alpha",
								"Advanced/LitVRWithScaleOffset/apShader_LitVRSO_T_Add",
								"Advanced/LitVRWithScaleOffset/apShader_LitVRSO_T_Soft",
								"Advanced/LitVRWithScaleOffset/apShader_LitVRSO_T_Mul",
								"Advanced/LitVRWithScaleOffset/apShader_LitVRSO_C_Alpha",
								"Advanced/LitVRWithScaleOffset/apShader_LitVRSO_C_Add",
								"Advanced/LitVRWithScaleOffset/apShader_LitVRSO_C_Soft",
								"Advanced/LitVRWithScaleOffset/apShader_LitVRSO_C_Mul",

								"Advanced/LitVRWithScaleOffset/Linear/apShader_L_LitVRSO_T_Alpha",
								"Advanced/LitVRWithScaleOffset/Linear/apShader_L_LitVRSO_T_Add",
								"Advanced/LitVRWithScaleOffset/Linear/apShader_L_LitVRSO_T_Soft",
								"Advanced/LitVRWithScaleOffset/Linear/apShader_L_LitVRSO_T_Mul",
								"Advanced/LitVRWithScaleOffset/Linear/apShader_L_LitVRSO_C_Alpha",
								"Advanced/LitVRWithScaleOffset/Linear/apShader_L_LitVRSO_C_Add",
								"Advanced/LitVRWithScaleOffset/Linear/apShader_L_LitVRSO_C_Soft",
								"Advanced/LitVRWithScaleOffset/Linear/apShader_L_LitVRSO_C_Mul",

								"apShader_AlphaMask", 
								false,
								true);
			}
		}


		//추가 19.10.27 : RenderTexture를 카메라의 렌더 타겟으로 지정하는 경우, 이 재질을 이용해야한다.
		public void MakeReserved_KeepAlpha(bool isCheckShader)
		{
			//Unlit
			if ((!isCheckShader) || (GetPresetUnit(RESERVED_MAT_ID__KeepAlpha_Unlit) != null))
			{
				MakeReservedPreset(RESERVED_MAT_ID__KeepAlpha_Unlit, "KeepAlpha Unlit", apMaterialSet.ICON.Unlit, 
								"Advanced/KeepAlphaUnlit/apShader_KAUnlit_T_Alpha",
								"Advanced/KeepAlphaUnlit/apShader_KAUnlit_T_Add",
								"Advanced/KeepAlphaUnlit/apShader_KAUnlit_T_Soft",
								"Advanced/KeepAlphaUnlit/apShader_KAUnlit_T_Mul",
								"Advanced/KeepAlphaUnlit/apShader_KAUnlit_C_Alpha",
								"Advanced/KeepAlphaUnlit/apShader_KAUnlit_C_Add",
								"Advanced/KeepAlphaUnlit/apShader_KAUnlit_C_Soft",
								"Advanced/KeepAlphaUnlit/apShader_KAUnlit_C_Mul",

								"Advanced/KeepAlphaUnlit/Linear/apShader_L_KAUnlit_T_Alpha",
								"Advanced/KeepAlphaUnlit/Linear/apShader_L_KAUnlit_T_Add",
								"Advanced/KeepAlphaUnlit/Linear/apShader_L_KAUnlit_T_Soft",
								"Advanced/KeepAlphaUnlit/Linear/apShader_L_KAUnlit_T_Mul",
								"Advanced/KeepAlphaUnlit/Linear/apShader_L_KAUnlit_C_Alpha",
								"Advanced/KeepAlphaUnlit/Linear/apShader_L_KAUnlit_C_Add",
								"Advanced/KeepAlphaUnlit/Linear/apShader_L_KAUnlit_C_Soft",
								"Advanced/KeepAlphaUnlit/Linear/apShader_L_KAUnlit_C_Mul",

								"apShader_AlphaMask", 
								true);
			}
			
			//Lit
			if ((!isCheckShader) || (GetPresetUnit(RESERVED_MAT_ID__KeepAlpha_Lit) != null))
			{
				MakeReservedPreset(RESERVED_MAT_ID__KeepAlpha_Lit, "KeepAlpha Lit", apMaterialSet.ICON.Lit, 
								"Advanced/KeepAlphaLit/apShader_KALit_T_Alpha",
								"Advanced/KeepAlphaLit/apShader_KALit_T_Add",
								"Advanced/KeepAlphaLit/apShader_KALit_T_Soft",
								"Advanced/KeepAlphaLit/apShader_KALit_T_Mul",
								"Advanced/KeepAlphaLit/apShader_KALit_C_Alpha",
								"Advanced/KeepAlphaLit/apShader_KALit_C_Add",
								"Advanced/KeepAlphaLit/apShader_KALit_C_Soft",
								"Advanced/KeepAlphaLit/apShader_KALit_C_Mul",

								"Advanced/KeepAlphaLit/Linear/apShader_L_KALit_T_Alpha",
								"Advanced/KeepAlphaLit/Linear/apShader_L_KALit_T_Add",
								"Advanced/KeepAlphaLit/Linear/apShader_L_KALit_T_Soft",
								"Advanced/KeepAlphaLit/Linear/apShader_L_KALit_T_Mul",
								"Advanced/KeepAlphaLit/Linear/apShader_L_KALit_C_Alpha",
								"Advanced/KeepAlphaLit/Linear/apShader_L_KALit_C_Add",
								"Advanced/KeepAlphaLit/Linear/apShader_L_KALit_C_Soft",
								"Advanced/KeepAlphaLit/Linear/apShader_L_KALit_C_Mul",

								"apShader_AlphaMask", 
								false);
			}
			
		}





		private bool IsShaderImported(string shaderPath)
		{
			string fullPath = "Assets/AnyPortrait/Assets/Shaders/" + shaderPath + ".shader";

			bool result = (AssetDatabase.LoadAssetAtPath<Shader>(fullPath) != null);
			//if(!result)
			//{
			//	Debug.LogError("Shader [" + shaderPath + "] is not imported");
			//}
			return result;
		}


		// Save / Load
		//------------------------------------------------------------------------------------------
		public void Save()
		{
			FileStream fs = null;
			StreamWriter sw = null;

			string filePath = Application.dataPath + "/../AnyPortrait_MaterialLibrary.txt";

			//Debug.Log("Save MaterialLibrary Presets");

			try
			{
				MakeReservedPresets();

				fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
				sw = new StreamWriter(fs);

				for (int i = 0; i < _presets.Count; i++)
				{
					_presets[i].Save(sw);

					if (i < _presets.Count - 1)
					{
						sw.WriteLine("--");//구분자
					}
				}

				sw.Flush();

				if(sw != null)
				{
					sw.Close();
					sw = null;
				}
				if(fs != null)
				{
					fs.Close();
					fs = null;
				}
			}
			catch(Exception ex)
			{
				Debug.LogError("MaterialLibrary Save Exception : " + ex);

				if(sw != null)
				{
					sw.Close();
					sw = null;
				}
				if(fs != null)
				{
					fs.Close();
					fs = null;
				}
			}
		}

		public void Load()
		{
			FileStream fs = null;
			StreamReader sr = null;

			//Debug.Log("Load MaterialLibrary Presets");


			_isFirstLoad = true;
			//string defaultPath = Application.dataPath;
			//string filePath = defaultPath.Substring(0, defaultPath.Length - 6) + "/AnyPortrait_MaterialLibrary.txt";
			string filePath = Application.dataPath + "/../AnyPortrait_MaterialLibrary.txt";

			try
			{
				ClearPresets();
				MakeReservedPresets();//Reserved가 추가되지 않았으면 자동으로 미리 추가하자

				fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				sr = new StreamReader(fs);

				List<string> strData = new List<string>();
				//유효한 데이터를 긁어온 후,
				//구분자를 만나면>>
				//하나씩 Unit으로 만들어준다.
				//일단 새로 하나 생성 후, 로드한 뒤,
				//겹치는게 있으면... 패스 (Save를 먼저 하세요)

				while (true)
				{
					if (sr.Peek() < 0)
					{
						//남은게 있으면 이것도 처리
						if (strData.Count > 0)
						{
							apMaterialSet newUnit = new apMaterialSet();
							newUnit.Load(strData);

							if (newUnit._uniqueID < 0)
							{
								continue;
							}

							//이제 추가 가능한 데이터인지 확인하자
							if (GetPresetUnit(newUnit._uniqueID) == null)
							{
								_presets.Add(newUnit);//추가!
							}
							strData.Clear();
						}
						break;
					}
					string strRead = sr.ReadLine();
					if (strRead.Length < 4)
					{
						//구분자를 만난 듯 하다.
						apMaterialSet newUnit = new apMaterialSet();
						newUnit.Load(strData);

						if (newUnit._uniqueID < 0)
						{
							continue;
						}

						//이제 추가 가능한 데이터인지 확인하자
						if (GetPresetUnit(newUnit._uniqueID) == null)
						{
							_presets.Add(newUnit);//추가!
						}

						strData.Clear();
					}
					else
					{
						//데이터 누적
						strData.Add(strRead);
					}
				}



				if (sr != null)
				{
					sr.Close();
					sr = null;
				}

				if (fs != null)
				{
					fs.Close();
					fs = null;
				}
			}
			catch (Exception ex)
			{
				if (ex is FileNotFoundException)
				{

				}
				else
				{
					Debug.LogError("MaterialLibrary Load Exception : " + ex);
				}


				if (sr != null)
				{
					sr.Close();
					sr = null;
				}

				if (fs != null)
				{
					fs.Close();
					fs = null;
				}

				//일단 저장을 한번 더 하자 (파일이 없을 수 있음)
				Save();
			}
		}



		// Get / Set
		//------------------------------------------------------
		public apMaterialSet GetPresetUnit(int uniqueID)
		{
			return _presets.Find(delegate (apMaterialSet a)
			{
				return a._uniqueID == uniqueID;
			});
		}


		public bool IsLoaded
		{
			get
			{
				return _isFirstLoad && _presets.Count > 0;
			}
		}
	}
}
#endif