using Fie.Utility;
using Fie.Voice;
using GameDataEditor;
using System.Collections.Generic;

namespace Fie.Manager
{
	public class FieDialogManager : FieManagerBehaviour<FieDialogManager>
	{
		public delegate void FieDialogManagerDialogChangedCallback(FieGameCharacter actor, GDEWordScriptsListData dialogData);

		private Queue<KeyValuePair<GDEWordScriptsListData, FieGameCharacter>> _dialogQueues = new Queue<KeyValuePair<GDEWordScriptsListData, FieGameCharacter>>();

		private FieGameCharacter _currentActor;

		private GDEWordScriptsListData _currentWordScript;

		public bool _isEnableDialog = true;

		public bool isEnableDialg
		{
			get
			{
				return _isEnableDialog;
			}
			set
			{
				if (_isEnableDialog != value)
				{
					if (!isEnableDialg)
					{
						this.dialogEndEvent(null, null);
					}
					_isEnableDialog = value;
				}
			}
		}

		public event FieDialogManagerDialogChangedCallback dialogStartEvent;

		public event FieDialogManagerDialogChangedCallback dialogEndEvent;

		public void RequestDialog(FieGameCharacter actor, GDEWordScriptsListData dialogData)
		{
			if (!(actor == null) && !(actor.voiceController == null) && dialogData != null && actor.isSpeakable && (!(_currentActor != null) || !(_currentActor.voiceController != null) || !_currentActor.voiceController.isPlaying || _currentWordScript == null || dialogData.Trigger.enableStaking || dialogData.Trigger.priority < _currentWordScript.Trigger.priority))
			{
				_dialogQueues.Enqueue(new KeyValuePair<GDEWordScriptsListData, FieGameCharacter>(dialogData, actor));
				if (_dialogQueues.Count == 1)
				{
					SetDialogFromQueue();
				}
			}
		}

		public void RequestDialog(GDEWordScriptTriggerTypeData triggerTypeData, GDEGameCharacterTypeData gameCharacterTypeData = null)
		{
			if (triggerTypeData != null)
			{
				List<GDEWordScriptsListData> list = FieMasterData<GDEWordScriptsListData>.FindMasterDataList(delegate(GDEWordScriptsListData wordScriptData)
				{
					if (triggerTypeData.Key != wordScriptData.Trigger.Key)
					{
						return false;
					}
					if (gameCharacterTypeData != null && gameCharacterTypeData.Key != wordScriptData.Actor.Key)
					{
						return false;
					}
					return true;
				});
				if (list != null && list.Count > 0)
				{
					GDEWordScriptsListData gDEWordScriptsListData = null;
					Lottery<GDEWordScriptsListData> lottery = new Lottery<GDEWordScriptsListData>();
					lottery.InitializeFromListData(list);
					if (lottery.IsExecutable())
					{
						gDEWordScriptsListData = lottery.Lot();
						FieGameCharacter existingGameCharacterRandom = FieManagerBehaviour<FieUserManager>.I.GetExistingGameCharacterRandom(gDEWordScriptsListData.Actor);
						RequestDialog(existingGameCharacterRandom, gDEWordScriptsListData);
					}
				}
			}
		}

		private void VoiceController_startEvent(FieVoiceController controller)
		{
			FieManagerBehaviour<FieAudioManager>.I.ChangeMixerVolume(-1f, 0.2f, FieAudioManager.FieAudioMixerType.BGM, FieAudioManager.FieAudioMixerType.SE);
		}

		private void VoiceController_endEvent(FieVoiceController controller)
		{
			FieManagerBehaviour<FieAudioManager>.I.ChangeMixerVolume(0f, 0.2f, FieAudioManager.FieAudioMixerType.BGM, FieAudioManager.FieAudioMixerType.SE);
			if (_currentWordScript != null && _currentWordScript.Next != null)
			{
				GDEGameCharacterTypeData actor = _currentWordScript.Next.Actor;
				if (actor != null)
				{
					FieGameCharacter existingGameCharacterRandom = FieManagerBehaviour<FieUserManager>.I.GetExistingGameCharacterRandom(actor);
					if (existingGameCharacterRandom != null)
					{
						RequestDialog(existingGameCharacterRandom, _currentWordScript.Next);
					}
				}
			}
			if (this.dialogEndEvent != null)
			{
				this.dialogEndEvent(_currentActor, _currentWordScript);
			}
			_currentActor = null;
			_currentWordScript = null;
			SetDialogFromQueue();
		}

		private void SetDialogFromQueue()
		{
			if (_dialogQueues.Count > 0)
			{
				KeyValuePair<GDEWordScriptsListData, FieGameCharacter> keyValuePair = _dialogQueues.Dequeue();
				_currentWordScript = keyValuePair.Key;
				_currentActor = keyValuePair.Value;
				if (_currentActor == null)
				{
					if (_dialogQueues.Count <= 0)
					{
						return;
					}
					SetDialogFromQueue();
				}
				if (_isEnableDialog)
				{
					_currentActor.voiceController.startEvent += VoiceController_startEvent;
					_currentActor.voiceController.endEvent += VoiceController_endEvent;
					_currentActor.voiceController.Play(_currentWordScript.Key, isForceSet: true);
					if (this.dialogStartEvent != null)
					{
						this.dialogStartEvent(_currentActor, _currentWordScript);
					}
				}
			}
		}
	}
}
