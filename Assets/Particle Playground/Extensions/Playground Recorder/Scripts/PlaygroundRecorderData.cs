using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;

[Serializable]
public class PlaygroundRecorderData : ScriptableObject 
{
	[HideInInspector] public float version;
	public SerializedFrame[] serializedFrames;
	
	public void Serialize (List<RecordedFrame> recordedFrames)
	{
		version = PlaygroundC.version;
		serializedFrames = null;
		serializedFrames = new SerializedFrame[recordedFrames.Count];
		for (int i = 0; i<serializedFrames.Length; i++)
			serializedFrames[i] = recordedFrames[i].CloneAsSerializedFrame();
		#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
		#endif
	}
	
	public void SerializeAsync (List<RecordedFrame> recordedFrames)
	{
		version = PlaygroundC.version;
		PlaygroundC.RunAsync(() => {
			serializedFrames = null;
			serializedFrames = new SerializedFrame[recordedFrames.Count];
			for (int i = 0; i<serializedFrames.Length; i++)
				serializedFrames[i] = recordedFrames[i].CloneAsSerializedFrame();
		});
		#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
		#endif
	}
	
	public List<RecordedFrame> CloneAsRecordedFrames ()
	{
		if (serializedFrames == null)
			return null;
		List<RecordedFrame> recordedFrames = new List<RecordedFrame>();
		for (int i = 0; i<serializedFrames.Length; i++)
		{
			recordedFrames.Add(serializedFrames[i].CloneAsRecordedFrame());
		}
		return recordedFrames;
	}

	public void Clear ()
	{
		serializedFrames = null;
	}
	
	public static PlaygroundRecorderData New () {
		PlaygroundRecorderData newData = ScriptableObject.CreateInstance<PlaygroundRecorderData>();
		return newData;
	}
}