using Fie.Enemies;
using Fie.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.ANYTIME_DESTROY)]
	public class FieInGameEnemyManager : FieManagerBehaviour<FieInGameEnemyManager>
	{
		private Dictionary<Type, GameObject> _resouceCache = new Dictionary<Type, GameObject>();

		private List<FieGameCharacter> _currentEnemies = new List<FieGameCharacter>();

		protected override void StartUpEntity()
		{
			_currentEnemies = new List<FieGameCharacter>();
		}

		private void CleanupEnemyList()
		{
			_currentEnemies.RemoveAll((FieGameCharacter enemy) => enemy == null);
		}

		public void Assign(FieGameCharacter enemy)
		{
			CleanupEnemyList();
			int index = -1;
			for (int i = 0; i < _currentEnemies.Count; i++)
			{
				if (_currentEnemies[i] == null)
				{
					_currentEnemies[index] = enemy;
					return;
				}
			}
			_currentEnemies.Add(enemy);
		}

		public FieGameCharacter GetNearestEnemy(Vector3 worldPosition)
		{
			CleanupEnemyList();
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int i = 0; i < _currentEnemies.Count; i++)
			{
				if (!(_currentEnemies[i] == null))
				{
					dictionary[i] = Vector3.Distance(worldPosition, _currentEnemies[i].position);
				}
			}
			return _currentEnemies[(from x in dictionary
			select (x)).Min().Key];
		}

		public FieGameCharacter CreateEnemyOnlyMasterClienty(Type enemyType, Vector3 position)
		{
			if (!PhotonNetwork.isMasterClient)
			{
				return null;
			}
			return CreateEnemy(enemyType, position);
		}

		public FieGameCharacter CreateEnemy(Type enemyType, Vector3 position)
		{
			if (!enemyType.IsSubclassOf(typeof(FieObjectEnemies)))
			{
				return null;
			}
			FiePrefabInfo fiePrefabInfo = (FiePrefabInfo)Attribute.GetCustomAttribute(enemyType, typeof(FiePrefabInfo));
			if (fiePrefabInfo == null)
			{
				return null;
			}
			GameObject gameObject = null;
			if (PhotonNetwork.offlineMode)
			{
				GameObject gameObject2 = null;
				if (!_resouceCache.ContainsKey(enemyType))
				{
					gameObject2 = (Resources.Load(fiePrefabInfo.path) as GameObject);
					_resouceCache[enemyType] = gameObject2;
				}
				else
				{
					gameObject2 = _resouceCache[enemyType];
				}
				gameObject = UnityEngine.Object.Instantiate(gameObject2, position, Quaternion.identity);
			}
			else
			{
				gameObject = PhotonNetwork.InstantiateSceneObject(fiePrefabInfo.path, position, Quaternion.identity, 0, null);
			}
			if (gameObject != null)
			{
				FieObjectEnemies component = gameObject.GetComponent<FieObjectEnemies>();
				if (component != null)
				{
					component.InitializeIntelligenceSystem(FieGameCharacter.IntelligenceType.AI);
					component.gameObject.SetActive(value: true);
					component.RequestArrivalState();
					component.forces = FieEmittableObjectBase.EmitObjectTag.ENEMY;
				}
				return component;
			}
			return null;
		}
	}
}
