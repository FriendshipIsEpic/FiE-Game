using Fie.Enemies.HoovesRaces.Changeling;
using Fie.Utility;
using Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieEnemySpawner : Photon.MonoBehaviour
{
	public const float ENEMY_ARRIVAL_INTERVAL_MIN = 0.6f;

	public const float ENEMY_ARRIVAL_INTERVAL_MAX = 1f;

	public const float SPAWNER_FIRST_DELAY = 2f;

	public List<GameObject> spawnObject;

	public List<Vector3> spawnList = new List<Vector3>();

	private List<FieChangeling> changelingList = new List<FieChangeling>();

	private bool isEnable = true;

	private FieGameCharacter currentSpawnedObject;

	private float _firstDelay = 2f;

	private IEnumerator initializeCoroutine()
	{
		if (PhotonNetwork.isMasterClient)
		{
			Lottery<GameObject> lotter = new Lottery<GameObject>();
			foreach (GameObject item in spawnObject)
			{
				lotter.AddItem(item);
			}
			changelingList = new List<FieChangeling>();
			foreach (Vector3 spawn in spawnList)
			{
				if (spawnObject != null)
				{
					GameObject gameObject = lotter.Lot();
					FieGameCharacter component = gameObject.GetComponent<FieGameCharacter>();
					FiePrefabInfo fiePrefabInfo = (FiePrefabInfo)Attribute.GetCustomAttribute(component.GetType(), typeof(FiePrefabInfo));
					if (fiePrefabInfo != null)
					{
						GameObject gameObject2 = (!PhotonNetwork.offlineMode) ? PhotonNetwork.InstantiateSceneObject(fiePrefabInfo.path, spawn, Quaternion.identity, 0, null) : UnityEngine.Object.Instantiate(gameObject, spawn, Quaternion.identity);
						if (gameObject2 != null)
						{
							FieChangeling component2 = gameObject2.GetComponent<FieChangeling>();
							if (component2 != null)
							{
								component2.InitializeIntelligenceSystem(FieGameCharacter.IntelligenceType.AI);
								component2.gameObject.SetActive(value: true);
								component2.RequestToChangeState<FieStateMachineChangelingArrival>(Vector3.zero, 0f, FieGameCharacter.StateMachineType.Base);
							}
							currentSpawnedObject = component2;
						}
					}
				}
			}
			isEnable = false;
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}

	private void Update()
	{
		if (_firstDelay > 0f)
		{
			_firstDelay -= Time.deltaTime;
		}
		else if (!isEnable && (currentSpawnedObject == null || currentSpawnedObject.damageSystem.isDead))
		{
			isEnable = true;
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		if (!(_firstDelay > 0f) && isEnable && !(collider.gameObject.tag != "Player"))
		{
			FieCollider component = collider.GetComponent<FieCollider>();
			if (!(component == null))
			{
				PhotonView component2 = component.getParentGameCharacter().GetComponent<PhotonView>();
				if (!(component2 == null) && component2.isMine)
				{
					if (PhotonNetwork.isMasterClient)
					{
						StartCreateEnemyJob();
					}
					else if (base.photonView != null)
					{
						base.photonView.RPC("CreateEnemyRPC", PhotonTargets.MasterClient, null);
						_firstDelay = 1f;
					}
				}
			}
		}
	}

	[PunRPC]
	public void CreateEnemyRPC()
	{
		StartCoroutine(initializeCoroutine());
	}

	public void StartCreateEnemyJob()
	{
		StartCoroutine(initializeCoroutine());
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(isEnable);
		}
		else
		{
			isEnable = (bool)stream.ReceiveNext();
		}
	}

	private void InitBufferList()
	{
	}
}
