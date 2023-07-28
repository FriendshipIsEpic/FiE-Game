using UnityEditor;
using UnityEngine;
using System.Collections;
using ParticlePlaygroundLanguage;

public class PlaygroundInstallLanguageWindowC : EditorWindow {

	public static EditorWindow window;
	public static Vector2 scrollPosition;
	public static LanguageInstallType installType;
	public static PlaygroundLanguageC languageFile;
	public static string languageLocation = "";

	public static void ShowWindow () {
		window = EditorWindow.GetWindow<PlaygroundInstallLanguageWindowC>(true, PlaygroundParticleWindowC.playgroundLanguage.languageInstallWizard);
		window.Show();
	}

	void OnGUI () {
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField(PlaygroundParticleWindowC.playgroundLanguage.languageInstallWizard, EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();
		
		EditorGUILayout.HelpBox(PlaygroundParticleWindowC.playgroundLanguage.installText, MessageType.Info);
		EditorGUILayout.Separator();

		installType = (LanguageInstallType)EditorGUILayout.EnumPopup(PlaygroundParticleWindowC.playgroundLanguage.installType, installType);
		if (installType==LanguageInstallType.Asset) {
			languageFile = (PlaygroundLanguageC)EditorGUILayout.ObjectField(PlaygroundParticleWindowC.playgroundLanguage.languageFile, languageFile, typeof(PlaygroundLanguageC), false);
		} else {
			EditorGUILayout.BeginHorizontal();
			languageLocation = EditorGUILayout.TextField(PlaygroundParticleWindowC.playgroundLanguage.languageLocation, languageLocation);
			if (GUILayout.Button (PlaygroundParticleWindowC.playgroundLanguage.browse, EditorStyles.toolbarButton, GUILayout.ExpandWidth (false))) {
				string newLanguageLocation = EditorUtility.OpenFilePanel (PlaygroundParticleWindowC.playgroundLanguage.languageLocation, "", "xml");
				if (newLanguageLocation.Length!=0) {
					languageLocation = newLanguageLocation;
				}
			}
			EditorGUILayout.EndHorizontal();
		}
		
		EditorGUILayout.Separator();
		
		GUI.enabled = (installType==LanguageInstallType.Asset && languageFile!=null || installType==LanguageInstallType.Xml && languageLocation.Length>0 && languageLocation.ToLower().Contains (".xml"));
		if (GUILayout.Button (PlaygroundParticleWindowC.playgroundLanguage.install, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
			if (installType==LanguageInstallType.Asset) {
				if (!PlaygroundParticleWindowC.playgroundSettings.languages.Contains (languageFile)) {
					AssetDatabase.MoveAsset( AssetDatabase.GetAssetPath ((Object)languageFile), "Assets/"+PlaygroundParticleWindowC.playgroundSettings.playgroundPath+PlaygroundParticleWindowC.playgroundSettings.languagePath+languageFile.name);
					PlaygroundParticleWindowC.playgroundSettings.languages.Add (languageFile);
					EditorUtility.SetDirty(PlaygroundParticleWindowC.playgroundSettings);
				}
			} else {
				PlaygroundSettingsC.ImportLanguage(languageLocation);
			}
			window.Close();
		}
		GUI.enabled = true;

		GUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}

	public enum LanguageInstallType {
		Asset,
		Xml
	}
}
