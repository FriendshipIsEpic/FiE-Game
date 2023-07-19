using Fie.Voice;
using RogoDigital.Lipsync;
using System.Collections.Generic;
using UnityEngine;

public class FieLipSyncTest : MonoBehaviour
{
	public FieVoiceController testTarget;

	public List<LipSyncData> testDataList = new List<LipSyncData>();

	private float interval = 3f;

	private void Start()
	{
	}

	private void Update()
	{
		if (!testTarget.isPlaying)
		{
			if (interval > 0f)
			{
				interval -= Time.deltaTime;
			}
			else if (testDataList.Count >= 1)
			{
				int index = Random.Range(0, testDataList.Count);
				testTarget.PlayWithLipSyncData(testDataList[index]);
				interval = 3f;
			}
		}
	}
}
