using Fie.AI;
using Fie.Enemies.HoovesRaces.Changeling;
using Fie.Object;
using GameDataEditor;
using System;

namespace Fie.Enemies.HoovesRaces.Flightling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Flightling/Flightling")]
	public class FieFlightling : FieChangeling
	{
		private const float FLIGHTLING_DEFAULT_MOVE_FORCE = 400f;

		protected new void Awake()
		{
			base.Awake();
			base.animationManager = new FieSkeletonAnimationController(base.skeletonUtility, new FieFlightlingAnimationContainer());
			PreAssignEmittableObject<FieEmitObjectFlightlingConcentration>();
			PreAssignEmittableObject<FieEmitObjectFlightlingShot>();
			PreAssignEmittableObject<FieEmitObjectFlightlingBurst>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesArrivalParticleEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesArrivalFireEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesDeadEffect>();
		}

		public override string getDefaultName()
		{
			return FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_ENEMY_NAME_FLIGHTLING);
		}

		public override FieConstValues.FieGameCharacter getGameCharacterID()
		{
			return FieConstValues.FieGameCharacter.FLIGHTLING;
		}

		public override Type getDefaultAttackState()
		{
			return typeof(FieStateMachineChangelingBaseAttack);
		}

		public override float getDefaultMoveSpeed()
		{
			return 400f;
		}

		public override Type getDefaultAITask()
		{
			return typeof(FieAITaskFlightlingIdle);
		}

		public override int getArrivalAnimationID()
		{
			return 15;
		}

		public override GDEEnemyTableData GetEnemyMasterData()
		{
			return FieMasterData<GDEEnemyTableData>.I.GetMasterData(GDEItemKeys.EnemyTable_ENEMY_TABLE_FLIGHTLING);
		}

		public override FieConstValues.FieEnemy GetEnemyMasterDataID()
		{
			return FieConstValues.FieEnemy.ENEMY_TABLE_FLIGHTLING;
		}
	}
}
