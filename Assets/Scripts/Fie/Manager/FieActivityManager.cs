using GameDataEditor;
using System.Collections;
using UnityEngine;

namespace Fie.Manager
{
	public class FieActivityManager : FieManagerBehaviour<FieActivityManager>
	{
		public delegate void FieActivityManagerActivityChangedCallback(GDEConstantTextListData titleTextData, GDEConstantTextListData noteTextData, string titleString = "", string noteString = "");

		public const float DEFAULT_ACTIVITY_SHOWING_TIME = 10f;

		private FieAcvitityQueue _currentActivity;

		private IEnumerator _execQueuesCoroutine;

		private string _replaceFromString = string.Empty;

		private string _replaceToString = string.Empty;

		public event FieActivityManagerActivityChangedCallback activityStartEvent;

		public event FieActivityManagerActivityChangedCallback activityEndEvent;

		private IEnumerator WatchQueuesCoroutine()
		{
			float timeCounter = 0f;
			if (this.activityStartEvent != null)
			{
				if (_currentActivity != null)
				{
					if (_replaceFromString != string.Empty)
					{
						string constantText = FieLocalizeUtility.GetConstantText(_currentActivity.titleTextData.Key);
						string constantText2 = FieLocalizeUtility.GetConstantText(_currentActivity.noteTextData.Key);
						constantText = constantText.Replace(_replaceFromString, _replaceToString);
						constantText2 = constantText2.Replace(_replaceFromString, _replaceToString);
						this.activityStartEvent(_currentActivity.titleTextData, _currentActivity.noteTextData, constantText, constantText2);
						_replaceFromString = string.Empty;
						_replaceToString = string.Empty;
					}
					else
					{
						this.activityStartEvent(_currentActivity.titleTextData, _currentActivity.noteTextData, string.Empty, string.Empty);
					}
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			else
			{
				if (FieManagerBehaviour<FieGUIManager>.I.GetActivityWindowState() != 0)
				{
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				if (_currentActivity != null)
				{
					if (timeCounter <= _currentActivity.showingTime)
					{
						if (_currentActivity != null)
						{
							float num = timeCounter + Time.deltaTime;
							yield return (object)null;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
					else
					{
						if (this.activityEndEvent != null)
						{
							if (_currentActivity == null)
							{
								yield break;
							}
							this.activityEndEvent(_currentActivity.titleTextData, _currentActivity.noteTextData, string.Empty, string.Empty);
						}
						_currentActivity = null;
					}
				}
			}
		}

		public bool IsShowingAnyActivity()
		{
			return _currentActivity != null;
		}

		public void RequestLobbyOnlyActivity(FieGameCharacter gameCharacter, GDEConstantTextListData title, GDEConstantTextListData note, float showingTime = 10f)
		{
			if (FieManagerBehaviour<FieSceneManager>.I.isLobby() && !(FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter == null) && gameCharacter.GetInstanceID() == FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.GetInstanceID())
			{
				RequestActivity(title, note, showingTime);
			}
		}

		public void RequestGameOwnerOnlyActivity(FieGameCharacter gameCharacter, GDEConstantTextListData title, GDEConstantTextListData note, float showingTime = 10f)
		{
			if (!(FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter == null) && gameCharacter.GetInstanceID() == FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.GetInstanceID())
			{
				RequestActivity(title, note, showingTime);
			}
		}

		public void RequestActivity(GDEConstantTextListData title, GDEConstantTextListData note, float showingTime = 10f, bool forcePush = false, bool flushQueue = false)
		{
			if (_execQueuesCoroutine != null)
			{
				StopCoroutine(_execQueuesCoroutine);
				_currentActivity = null;
			}
			_currentActivity = new FieAcvitityQueue(showingTime, title, note);
			_execQueuesCoroutine = WatchQueuesCoroutine();
			StartCoroutine(_execQueuesCoroutine);
		}

		public void RequestToHideActivity()
		{
			if (_execQueuesCoroutine != null)
			{
				StopCoroutine(_execQueuesCoroutine);
				_currentActivity = null;
			}
			if (this.activityEndEvent != null)
			{
				this.activityEndEvent(null, null, string.Empty, string.Empty);
			}
		}

		public void RegistReplaceString(string fromString, string toString)
		{
			_replaceFromString = fromString;
			_replaceToString = toString;
		}
	}
}
