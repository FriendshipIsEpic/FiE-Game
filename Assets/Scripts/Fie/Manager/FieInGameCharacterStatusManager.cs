using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.ANYTIME_DESTROY)]
	public class FieInGameCharacterStatusManager : FieManagerBehaviour<FieInGameCharacterStatusManager>
	{
		public enum ForcesTag
		{
			PLAYER,
			ENEMY
		}

		public Dictionary<ForcesTag, Dictionary<int, FieGameCharacter>> _characterLists = new Dictionary<ForcesTag, Dictionary<int, FieGameCharacter>>
		{
			{
				ForcesTag.PLAYER,
				new Dictionary<int, FieGameCharacter>()
			},
			{
				ForcesTag.ENEMY,
				new Dictionary<int, FieGameCharacter>()
			}
		};

		protected override void StartUpEntity()
		{
			_characterLists[ForcesTag.PLAYER] = new Dictionary<int, FieGameCharacter>();
			_characterLists[ForcesTag.ENEMY] = new Dictionary<int, FieGameCharacter>();
		}

		public void AssignCharacter(ForcesTag tag, FieGameCharacter character)
		{
			_characterLists[tag][character.GetInstanceID()] = character;
		}

		public void DissociateCharacter(ForcesTag tag, FieGameCharacter character)
		{
			_characterLists[tag].Remove(character.GetInstanceID());
		}

		public FieGameCharacter GetNearbyInjuryAllyCharacter(FieGameCharacter searchingCharacter)
		{
			if (_characterLists[ForcesTag.PLAYER] == null || _characterLists[ForcesTag.PLAYER].Count <= 0)
			{
				return null;
			}
			List<FieGameCharacter> list = new List<FieGameCharacter>();
			foreach (int key in _characterLists[ForcesTag.PLAYER].Keys)
			{
				if (_characterLists[ForcesTag.PLAYER][key].damageSystem.isDead && _characterLists[ForcesTag.PLAYER][key].GetInstanceID() != searchingCharacter.GetInstanceID())
				{
					list.Add(_characterLists[ForcesTag.PLAYER][key]);
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return (from entry in list
			orderby Vector3.Distance(entry.transform.position, searchingCharacter.transform.position)
			select entry)?.FirstOrDefault();
		}

		public bool isAllPlayerDied()
		{
			int num = 0;
			int num2 = 0;
			foreach (int key in _characterLists[ForcesTag.PLAYER].Keys)
			{
				num++;
				if (_characterLists[ForcesTag.PLAYER][key].healthStats.hitPoint <= 0f && _characterLists[ForcesTag.PLAYER][key].friendshipStats.getCurrentFriendshipPoint() < 3)
				{
					num2++;
				}
			}
			if (num2 >= num)
			{
				return true;
			}
			return false;
		}
	}
}
