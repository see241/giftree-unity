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
	/// 기존의 optModifiedMesh에서 Vertex 부분만 가져온 클래스
	/// 최적화를 위해서 분리가 되었다.
	/// 데이터가 훨씬 더 최적화되었다.
	/// </summary>
	[Serializable]
	public class apOptModifiedMesh_Vertex
	{
		// Members
		//--------------------------------------------
		//[NonSerialized]
		//private apOptModifiedMeshSet _parentModMeshSet = null;

		[SerializeField]
		public int _nVerts = 0;
		
		//개선점 : 기존의 apOptModifiedVertex에서 오직 Vector2만 남긴다.
		[SerializeField]
		public Vector2[] _vertDeltaPos = null;


		// Init
		//--------------------------------------------
		public apOptModifiedMesh_Vertex()
		{

		}

		public void Link(apOptModifiedMeshSet parentModMeshSet)
		{
			//_parentModMeshSet = parentModMeshSet;
		}


		// Init - Bake
		//--------------------------------------------
		public void Bake(List<apModifiedVertex> modVerts)
		{
			_nVerts = modVerts.Count;
			_vertDeltaPos = new Vector2[_nVerts];

			for (int i = 0; i < _nVerts; i++)
			{
				_vertDeltaPos[i] = modVerts[i]._deltaPos;
			}
		}
	}
}