using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

[CustomEditor (typeof(PlaygroundRecorder))]
public class PlaygroundRecorderInspector : Editor {

	// References
	PlaygroundRecorder recorder;

	// GUI
	public static GUIStyle boxStyle;
	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;

	// Trimming
	private bool _inTrimming;
	private float _leftTrimPos = 0;
	private float _rightTrimPos = 1f;
	private bool _leftWasLastMoved = true;
	
	void OnEnable () 
	{
		// Set references
		recorder = target as PlaygroundRecorder;

		// Load settings
		playgroundSettings = PlaygroundSettingsC.GetReference();
		
		// Load language
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();

		// Load data
		if (recorder.recorderData != null)
			recorder.LoadAsync();
		else
			recorder.recordedFrames = new List<RecordedFrame>();
	}
	
	public override void OnInspectorGUI ()
	{
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");

		bool hasParticleSystem = recorder.playgroundSystem != null;
		if (!hasParticleSystem)
			EditorGUILayout.HelpBox(playgroundLanguage.missingParticleSystemWarning, MessageType.Warning);
		bool hasRecorderData = recorder.recorderData != null;

		EditorGUILayout.BeginVertical (boxStyle);
		playgroundSettings.playgroundRecorderFoldout = GUILayout.Toggle(playgroundSettings.playgroundRecorderFoldout, playgroundLanguage.playgroundRecorder, EditorStyles.foldout);
		if (playgroundSettings.playgroundRecorderFoldout) 
		{
			EditorGUILayout.BeginVertical (boxStyle);

			// Playback & Recorder foldout
			if (GUILayout.Button(playgroundLanguage.player, EditorStyles.toolbarDropDown)) playgroundSettings.recorderPlaybackFoldout=!playgroundSettings.recorderPlaybackFoldout;
			if (playgroundSettings.recorderPlaybackFoldout) 
			{
				EditorGUILayout.Separator();
				if (!hasRecorderData)
				{
					EditorGUILayout.BeginVertical(boxStyle);
					EditorGUILayout.HelpBox(playgroundLanguage.missingRecorderDataWarning, MessageType.Warning);
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(playgroundLanguage.createNew, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
					{
						PlaygroundRecorderData newData = CreateNewRecorderDataDialogue();
						if (newData!=null)
							recorder.recorderData = newData;
					}
					recorder.recorderData = (PlaygroundRecorderData)EditorGUILayout.ObjectField(recorder.recorderData, typeof(PlaygroundRecorderData), false);
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
					EditorGUILayout.Separator();
				}


				EditorGUILayout.BeginHorizontal();

				GUI.enabled = !recorder.IsRecording() && hasParticleSystem && recorder.HasRecordedFrames();
				if (GUILayout.Button (recorder.IsReplaying()? playgroundLanguage.pauseSymbol : playgroundLanguage.playSymbol, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
				{
					if (recorder.IsReplaying())
						recorder.Pause();
					else
						recorder.Play(recorder.playHead, recorder.playbackSpeed, recorder.loopPlayback);
				}
				GUI.enabled = hasParticleSystem && recorder.HasRecordedFrames();

				if (GUILayout.Button (playgroundLanguage.stopSymbol, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
				{
					if (recorder.IsInPlayback())
						recorder.SetParticleSystemAsCurrentPlayback();
					else
					{
						recorder.StopAndSerialize();
						recorder.playHead = 0;
					}
				}

				GUI.enabled = hasParticleSystem;

				if (recorder.IsRecording())
					GUI.color = Color.red;

				GUILayout.Space(4f);

				if (GUILayout.Button (playgroundLanguage.recordSymbol, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
				{
					if (!recorder.IsRecording())
						recorder.StartRecording();
					else
					{
						recorder.StopAndSerialize();
						recorder.playHead = 0;
					}
				}

				GUI.color = Color.white;

				GUI.enabled = recorder.HasRecordedFrames() && hasParticleSystem;
				if (GUILayout.Button (playgroundLanguage.clear, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
				{
					if (EditorUtility.DisplayDialog(playgroundLanguage.clearRecording, playgroundLanguage.clearRecordingMsg, playgroundLanguage.yes, playgroundLanguage.no))
						recorder.ClearRecording();
				}

				GUILayout.FlexibleSpace();
				
				_inTrimming = GUILayout.Toggle (_inTrimming, playgroundLanguage.trim, EditorStyles.toolbarButton);

				GUI.enabled = true;

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Separator();

				GUI.enabled = recorder.HasRecordedFrames();

				float currentPlayHead = recorder.playHead;

				if (!_inTrimming)
					recorder.playHead = EditorGUILayout.Slider(playgroundLanguage.playHeadPosition, recorder.playHead, 0, 1f);
				else
				{
					GUILayout.BeginHorizontal();
					EditorGUILayout.Separator();
					if (GUILayout.Button (playgroundLanguage.trimOuter, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
					{
						string framesRemoved = "[0-"+recorder.GetFrameAtTime(_leftTrimPos)+"] - ["+recorder.GetFrameAtTime(_rightTrimPos)+"-"+recorder.FrameCount()+"]";
						if (EditorUtility.DisplayDialog(playgroundLanguage.trim, playgroundLanguage.trimMsg+framesRemoved+"?", playgroundLanguage.yes, playgroundLanguage.no))
							if (recorder.Trim(_leftTrimPos, _rightTrimPos))
							{
								_leftTrimPos = 0;
								_rightTrimPos = 1f;
							}
					}
					if (GUILayout.Button (playgroundLanguage.trimInner, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
					{
						string framesRemoved = "["+recorder.GetFrameAtTime(_leftTrimPos)+"-"+recorder.GetFrameAtTime(_rightTrimPos)+"]";
						if (EditorUtility.DisplayDialog(playgroundLanguage.trim, playgroundLanguage.trimMsg+framesRemoved+"?", playgroundLanguage.yes, playgroundLanguage.no))
						{
							recorder.TrimInner(_leftTrimPos, _rightTrimPos);
							_leftTrimPos = 0;
							_rightTrimPos = 1f;
						}
					}
					GUILayout.EndHorizontal();

					float l = _leftTrimPos;
					float r = _rightTrimPos;
					EditorGUILayout.MinMaxSlider(ref _leftTrimPos, ref _rightTrimPos, 0, 1f);
					

					if (!Mathf.Approximately(_leftTrimPos, l))
						_leftWasLastMoved = true;
					if (!Mathf.Approximately(_rightTrimPos, r))
						_leftWasLastMoved = false;

					if (!recorder.IsReplaying())
						recorder.playHead = _leftWasLastMoved? _leftTrimPos : _rightTrimPos;
					else
						_leftWasLastMoved = true;

					if (recorder.playHead >= _rightTrimPos && recorder.IsReplaying())
						recorder.playHead = _leftTrimPos;
					if (recorder.playHead < _leftTrimPos && recorder.IsReplaying() && recorder.playbackSpeed>0)
					    recorder.playHead = _leftTrimPos;
					if (recorder.playHead <= _leftTrimPos && (!_leftWasLastMoved || recorder.IsReplaying() && recorder.playbackSpeed<0))
						recorder.playHead = _rightTrimPos;
				}

				if (currentPlayHead != recorder.playHead)
					recorder.Scrub (recorder.playHead);

				string playbackStatus = "No Recording";
				string playbackData = "";
				float recordedSeconds = ((recorder.FrameCount()*1f) * recorder.keyframeInterval) / recorder.playbackSpeed;

				if (_inTrimming)
				{
					playbackStatus = "TRIMMING";
					GUI.color = Color.yellow;
				}
				else if (recorder.IsRecording())
				{
					playbackStatus = "RECORDING";
					GUI.color = Color.red;
				}
				else if (recorder.IsInPlayback())
				{
					playbackStatus = "In Playback";
					GUI.color = Color.green;
				}
				else if (!recorder.IsInPlayback() && recorder.HasRecordedFrames())
				{
					playbackStatus = "Live Particles";
					GUI.color = Color.cyan;
				}
				if (recorder.HasRecordedFrames() && recorder.IsInPlayback())
				{
					if (_inTrimming)
						playbackData = " (Left: " + recorder.GetFrameAtTime(_leftTrimPos) + " | Right: " + recorder.GetFrameAtTime(_rightTrimPos) + ")";
					else if (!recorder.IsRecording())
						playbackData = " (" + (recordedSeconds*recorder.playHead).ToString("F1") + "/" + recordedSeconds.ToString("F1") + " s)";
					else if (!_inTrimming)
						playbackData = " (" + (recordedSeconds).ToString("F1") + " s)";
				}
				PlaybackBar(recorder.HasRecordedFrames()? (recorder.IsRecording()? 1f : recorder.playHead) : 0, playbackStatus + playbackData, Screen.width - 56f);

				GUI.color = Color.white;
				GUI.backgroundColor = Color.white;
				GUILayout.Space (-10f);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Time: " + (recorder.HasRecordedFrames()?(recordedSeconds*recorder.playHead).ToString("F1") + " / " + recordedSeconds.ToString("F1") + " s" + " (" + recorder.playbackSpeed.ToString("F1") + "x)" : "-"), EditorStyles.objectFieldThumb, GUILayout.Width ((Screen.width/2f)-30f));
				EditorGUILayout.LabelField("Frame: " + (recorder.HasRecordedFrames()?recorder.GetFrameAtTime(recorder.playHead).ToString() + " / " + recorder.FrameCount() : "-"), EditorStyles.objectFieldThumb, GUILayout.Width ((Screen.width/2f)-30f));
				EditorGUILayout.EndHorizontal();

				GUI.enabled = true;

				EditorGUILayout.Separator();
			}

			// Advanced foldout
			if (GUILayout.Button(playgroundLanguage.advanced, EditorStyles.toolbarDropDown)) playgroundSettings.recorderAdvancedFoldout=!playgroundSettings.recorderAdvancedFoldout;
			if (playgroundSettings.recorderAdvancedFoldout) 
			{
				EditorGUILayout.Separator();
				recorder.playgroundSystem = (PlaygroundParticlesC)EditorGUILayout.ObjectField(playgroundLanguage.particleSystem, recorder.playgroundSystem, typeof(PlaygroundParticlesC), true);
				recorder.recorderData = (PlaygroundRecorderData)EditorGUILayout.ObjectField(playgroundLanguage.recorderData, recorder.recorderData, typeof(PlaygroundRecorderData), false);
				recorder.keyframeInterval = EditorGUILayout.FloatField(playgroundLanguage.keyframeInterval, recorder.keyframeInterval);
				recorder.playbackSpeed = EditorGUILayout.FloatField(playgroundLanguage.playbackSpeed, recorder.playbackSpeed);
				recorder.loopPlayback = GUILayout.Toggle (recorder.loopPlayback, playgroundLanguage.loop);
				recorder.fadeIn = GUILayout.Toggle (recorder.fadeIn, playgroundLanguage.fadeIn);
				recorder.sizeIn = GUILayout.Toggle (recorder.sizeIn, playgroundLanguage.sizeIn);
				recorder.skipInterpolationOnEndFrames = GUILayout.Toggle (recorder.skipInterpolationOnEndFrames, playgroundLanguage.skipInterpolationOnEndFrames);
				recorder.localSpaceOnPlayback = GUILayout.Toggle (recorder.localSpaceOnPlayback, playgroundLanguage.setLocalSpaceOnPlayback);
				recorder.multithreading = GUILayout.Toggle (recorder.multithreading, playgroundLanguage.multithreading);
				EditorGUILayout.Separator();
			}

			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndVertical();
	}

	public void PlaybackBar (float val, string label, float width) {
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		rect.width = width;
		rect.height = 16;
		if (val<0) val = 0;
		EditorGUI.ProgressBar (rect, val, label);
		EditorGUILayout.Space ();
	}

	public static PlaygroundRecorderData CreateNewRecorderDataDialogue ()
	{
		string dataPath = EditorUtility.SaveFilePanelInProject(playgroundLanguage.newPlaygroundRecording, "PlaygroundRecording", "asset", playgroundLanguage.newPlaygroundRecordingMsg);
		if (dataPath.Length>0)
		{
			PlaygroundRecorderData newData = PlaygroundRecorderData.New();
			AssetDatabase.CreateAsset(newData, dataPath);
			AssetDatabase.Refresh();

			return newData;
		}
		return null;
	}
}
