using Fie.Manager;
using Fie.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieGameUIGameOverWindow : FieGameUIBase
	{
		private delegate void tweenFinishedCallback();

		[SerializeField]
		private GameObject _rootPanelGameObject;

		[SerializeField]
		private Button _retryButton;

		[SerializeField]
		private Button _quitButton;

		[SerializeField]
		private AudioSource _gameOverSoundEffect;

		[SerializeField]
		private GameObject _objectsForHostPlayer;

		[SerializeField]
		private GameObject _objectsForClientPlayer;

		private Tweener<TweenTypesOutSine> _scaleTweener = new Tweener<TweenTypesOutSine>();

		private tweenFinishedCallback _tweenFinishedCallback;

		private bool _isEnable;

		private bool _isFinishedAnimation = true;

		public bool isEnable
		{
			get
			{
				return _isEnable;
			}
			set
			{
				if (value != _isEnable)
				{
					if (value)
					{
						_scaleTweener.InitTweener(0.5f, 0f, 1f);
						if (PhotonNetwork.offlineMode || PhotonNetwork.isMasterClient)
						{
							_objectsForHostPlayer.SetActive(value: true);
							_objectsForClientPlayer.SetActive(value: false);
						}
						else
						{
							_objectsForHostPlayer.SetActive(value: false);
							_objectsForClientPlayer.SetActive(value: true);
						}
						_rootPanelGameObject.SetActive(value: true);
						_gameOverSoundEffect.Play();
					}
					else
					{
						_scaleTweener.InitTweener(0.5f, 1f, 0f);
					}
					_isEnable = value;
					_isFinishedAnimation = false;
				}
			}
		}

		public override void Initialize()
		{
			_isEnable = false;
			_isFinishedAnimation = true;
			_retryButton.interactable = true;
			_quitButton.interactable = true;
			_scaleTweener.InitTweener(0.5f, 0f, 0f);
			_rootPanelGameObject.transform.localScale = new Vector3(1f, 0f, 1f);
		}

		private void Update()
		{
			if (!_scaleTweener.IsEnd())
			{
				float y = _scaleTweener.UpdateParameterFloat(Time.deltaTime);
				_rootPanelGameObject.transform.localScale = new Vector3(1f, y, 1f);
			}
			else if (!_isFinishedAnimation)
			{
				if (_isEnable)
				{
					_retryButton.Select();
				}
				else
				{
					_rootPanelGameObject.SetActive(value: false);
				}
				if (_tweenFinishedCallback != null)
				{
					_tweenFinishedCallback();
				}
				_tweenFinishedCallback = null;
				_isFinishedAnimation = true;
				_retryButton.interactable = true;
				_quitButton.interactable = true;
			}
		}

		public void OnClickRetry()
		{
			isEnable = false;
			_tweenFinishedCallback = delegate
			{
				FieManagerBehaviour<FieInGameStateManager>.I.SetGameState(FieInGameStateManager.FieInGameState.STATE_IS_RETRYING);
			};
			_retryButton.interactable = false;
		}

		public void OnClickQuit()
		{
			isEnable = false;
			_tweenFinishedCallback = delegate
			{
				FieManagerBehaviour<FieInGameStateManager>.I.SetGameState(FieInGameStateManager.FieInGameState.STATE_ON_QUIT);
			};
			_quitButton.interactable = false;
		}
	}
}
