using Fie.Manager;
using GameDataEditor;
using HutongGames.PlayMaker;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieActivityManagerForPlaymaker : FsmStateAction
	{
		public enum ActivityActionType
		{
			SHOW,
			HIDE
		}

		[RequiredField]
		public ActivityActionType activityType;

		[RequiredField]
		public string titleTextKey;

		[RequiredField]
		public string noteTextKey;

		[RequiredField]
		public float showingTime;

		[RequiredField]
		public bool forceSet;

		[RequiredField]
		public FsmEvent nextEvent;

		public override void OnUpdate()
		{
			switch (activityType)
			{
			case ActivityActionType.SHOW:
				if (forceSet || !FieManagerBehaviour<FieActivityManager>.I.IsShowingAnyActivity())
				{
					FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(titleTextKey), FieMasterData<GDEConstantTextListData>.I.GetMasterData(noteTextKey), showingTime);
					base.Fsm.Event(nextEvent);
					Finish();
				}
				break;
			case ActivityActionType.HIDE:
				FieManagerBehaviour<FieActivityManager>.I.RequestToHideActivity();
				base.Fsm.Event(nextEvent);
				Finish();
				break;
			}
		}
	}
}
