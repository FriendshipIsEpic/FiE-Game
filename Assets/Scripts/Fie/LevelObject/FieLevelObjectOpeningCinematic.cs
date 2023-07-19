using CinemaDirector;
using Fie.Camera;
using Fie.Manager;
using Photon;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.LevelObject
{
	public class FieLevelObjectOpeningCinematic : Photon.MonoBehaviour
	{
		[SerializeField]
		private float _firstTimeDelay = 0.5f;

		[SerializeField]
		private Cutscene _cutscene;

		[SerializeField]
		private ActorTrackGroup _cameraActor;

		[SerializeField]
		private List<GameObject> _cinematicObjects = new List<GameObject>();

		[SerializeField]
		private FieWaveController _thenTriggerWave;

		private bool _isStarted;

		private bool _isFinished;

		private float _delay;

		private void Awake()
		{
			_delay = _firstTimeDelay;
			_cutscene.CutsceneFinished += _cutscene_CutsceneFinished;
			FieManagerBehaviour<FieInGameStateManager>.I.RetryEvent += RetryEvent;
		}

		private void RetryEvent()
		{
			_isStarted = false;
			_isFinished = false;
			if (_cinematicObjects != null && _cinematicObjects.Count > 0)
			{
				foreach (GameObject cinematicObject in _cinematicObjects)
				{
					cinematicObject.SetActive(value: true);
				}
			}
			_delay = _firstTimeDelay;
		}

		public void Reset()
		{
			_isStarted = false;
			_isFinished = false;
		}

		private void _cutscene_CutsceneFinished(object sender, CutsceneEventArgs e)
		{
			if (_cinematicObjects != null && _cinematicObjects.Count > 0)
			{
				foreach (GameObject cinematicObject in _cinematicObjects)
				{
					cinematicObject.SetActive(value: false);
				}
			}
			FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.GetComponent<FieGameCamera>().enabled = true;
			FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.camera.cullingMask &= -524289;
			FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.camera.cullingMask |= 256;
			FieManagerBehaviour<FieGUIManager>.I.ShowHeaderFooter();
			FieManagerBehaviour<FieInputManager>.I.isEnableControll = true;
			if (_thenTriggerWave != null)
			{
				_thenTriggerWave._isEnable = true;
			}
			_isFinished = true;
		}

		private void Update()
		{
			if (_delay > 0f)
			{
				_delay -= Time.deltaTime;
			}
			if (!_isFinished && _cutscene.State == Cutscene.CutsceneState.Playing && FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButton("Menu") && (PhotonNetwork.isMasterClient || PhotonNetwork.offlineMode))
			{
				_cutscene.Skip();
				base.photonView.RPC("SkipCinemacitRPC", PhotonTargets.Others, null);
			}
		}

		[PunRPC]
		public void SkipCinemacitRPC()
		{
			_cutscene.Skip();
		}

		[PunRPC]
		public void StartCinemacitRPC()
		{
			StartCinematic();
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!(_delay > 0f) && !_isStarted)
			{
				FieCollider component = collider.gameObject.GetComponent<FieCollider>();
				if (component != null && component.getParentGameCharacter() != null && collider.gameObject.tag == "Player")
				{
					base.photonView.RPC("StartCinemacitRPC", PhotonTargets.All, null);
					StartCinematic();
				}
			}
		}

		private void StartCinematic()
		{
			if (!_isStarted)
			{
				if (_cinematicObjects != null && _cinematicObjects.Count > 0)
				{
					foreach (GameObject cinematicObject in _cinematicObjects)
					{
						cinematicObject.SetActive(value: true);
					}
				}
				_cameraActor.Actor = FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.transform;
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.camera.cullingMask |= 524288;
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.camera.cullingMask &= -257;
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.GetComponent<FieGameCamera>().enabled = false;
				FieManagerBehaviour<FieGUIManager>.I.HideHeaderFooter();
				_cutscene.Refresh();
				_cutscene.Play();
				FieManagerBehaviour<FieInputManager>.I.isEnableControll = false;
				_isStarted = true;
			}
		}
	}
}
