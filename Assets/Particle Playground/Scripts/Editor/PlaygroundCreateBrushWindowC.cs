using UnityEditor;
using UnityEngine;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

public class PlaygroundCreateBrushWindowC : EditorWindow {

	public static Texture2D brushTexture;
	public static string brushName = "";
	public static float brushScale = 1f;
	public static BRUSHDETAILC brushDetail;
	public static float distance = 10000f;
	public static float spacing = .1f;
	public static bool exceedMaxStopsPaint = false;
	
	public static EditorWindow window;
	private Vector2 scrollPosition;

	public static PlaygroundLanguageC playgroundLanguage;
	
	public static void ShowWindow () {
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
		window = EditorWindow.GetWindow<PlaygroundCreateBrushWindowC>(true, playgroundLanguage.brushWizard);
        window.Show();
	}
	
	void OnEnable () {
	}
	
	void OnGUI () {
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField(playgroundLanguage.playgroundBrushWizard, EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();
		
		GUILayout.BeginVertical("box");
		EditorGUILayout.HelpBox(playgroundLanguage.brushWizardText, MessageType.Info);
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(playgroundLanguage.brushTexture);
		Texture2D selectedBrushTexture = brushTexture;
		brushTexture = EditorGUILayout.ObjectField(brushTexture, typeof(Texture2D), false) as Texture2D;
		GUILayout.EndHorizontal();
		if (selectedBrushTexture!=brushTexture)
			brushName = brushTexture.name;
		brushName = EditorGUILayout.TextField(playgroundLanguage.nameText, brushName);
		brushScale = EditorGUILayout.FloatField(playgroundLanguage.scale, brushScale);
		brushDetail = (BRUSHDETAILC)EditorGUILayout.EnumPopup(playgroundLanguage.detail, brushDetail);
		distance = EditorGUILayout.FloatField(playgroundLanguage.distance, distance);
		spacing = EditorGUILayout.FloatField(playgroundLanguage.paintSpacing, spacing);
		exceedMaxStopsPaint = EditorGUILayout.Toggle(playgroundLanguage.exceedMaxStopsPaint, exceedMaxStopsPaint);
		
		if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
			brushName = brushName.Trim();
			if (brushTexture && brushName!="") {
				if (AssetDatabase.LoadAssetAtPath("Assets/"+PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+PlaygroundParticleSystemInspectorC.playgroundSettings.brushPath+brushName+".prefab", typeof(GameObject))) {
					if (EditorUtility.DisplayDialog(playgroundLanguage.brushSameName, 
						brushName+" "+playgroundLanguage.brushSameNameText, 
                        playgroundLanguage.yes, 
                        playgroundLanguage.no))
							CreateBrush();
				} else CreateBrush();
			}
		}
		
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		EditorGUILayout.EndVertical();
	}
	
	public void CreateBrush () {
		GameObject brushObject = new GameObject(brushName);
		PlaygroundBrushPresetC brushPreset = brushObject.AddComponent<PlaygroundBrushPresetC>();
		brushPreset.presetName = brushName;
		brushPreset.texture = brushTexture;
		brushPreset.scale = brushScale;
		brushPreset.detail = brushDetail;
		brushPreset.distance = distance;
		brushPreset.spacing = spacing;
		brushPreset.exceedMaxStopsPaint = exceedMaxStopsPaint;
		
		PrefabUtility.CreatePrefab("Assets/"+PlaygroundParticleSystemInspectorC.playgroundSettings.playgroundPath+PlaygroundParticleSystemInspectorC.playgroundSettings.brushPath+brushName+".prefab", brushObject, ReplacePrefabOptions.Default);
		DestroyImmediate(brushObject);
		
		PlaygroundParticleSystemInspectorC.LoadBrushes();
		
		window.Close();
	}
}