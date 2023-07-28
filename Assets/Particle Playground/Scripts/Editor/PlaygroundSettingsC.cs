using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using ParticlePlaygroundLanguage;
using UnityEditor;

[Serializable]
public class PlaygroundSettingsC : ScriptableObject {

	public static string settingsPath = "Particle Playground/Playground Assets/Settings/Playground Settings.asset";
	public static string playgroundCacheSettingsLocation = "PlaygroundCache/PlaygroundSettingsLocation.txt";
	public static PlaygroundSettingsC reference;
	bool isInstance = false;

	// Playground Wizard
	public bool checkForUpdates = true;
	public bool enableExtensions = true;
	public bool presetsHasPrefabConnection = false;
	public bool hierarchyIcon = true;
	public float extensionIconSize = 1f;
	public float presetIconSize = 1f;

	// Open/Closed tabs Playground Window
	public bool settingsFoldout = false;
	public bool settingsLanguageFoldout = true;
	public bool settingsPathFoldout = false;
	public bool extensionsFoldout = true;
	public bool presetsFoldout = true;
	public bool playgroundManagerFoldout = true;
	public bool particleSystemFoldout = true;
	public bool limitsFoldout = false;

	// Open/Closed tabs Particle System Inspector
	public bool particlesFoldout = true;
	public bool statesFoldout = true;
	public bool sourceFoldout = false;
	public bool createNewStateFoldout = false;
	public bool particleSettingsFoldout = false;
	public bool forcesFoldout = false;
	public bool manipulatorsFoldout = false;
	public bool eventsFoldout = false;
	public bool collisionFoldout = false;
	public bool renderingFoldout = false;
	public bool advancedFoldout = false;
	public bool saveLoadFoldout = false;
	public bool toolboxFoldout = true;
	public bool paintToolboxSettingsFoldout = true;
	public bool sortingFoldout = false;
	public bool textureSheetAnimationFoldout = false;
	public bool splineListFoldout = true;
	public bool transformListFoldout = true;

	public bool particleSettingsParticleCountFoldout = false;
	public bool particleSettingsOverflowOffsetFoldout = false;
	public bool particleSettingsScatterFoldout = false;
	public bool particleSettingsEmissionFoldout = false;
	public bool particleSettingsSizeFoldout = false;
	public bool particleSettingsRotationFoldout = false;
	public bool particleSettingsLifetimeFoldout = false;
	public bool particleSettingsMaskFoldout = false;

	public bool forceAnnihilationFoldout = false;
	public bool forceInitialVelocityFoldout = false;
	public bool forceLifetimeVelocityFoldout = false;
	public bool forceVelocityBendingFoldout = false;
	public bool forceTurbulenceFoldout = false;
	public bool forceConstantForceFoldout = false;
	public bool forceVelocityScaleFoldout = false;

	public bool collisionSettingsFoldout = false;
	public bool collisionPlanesFoldout = false;
	public bool collisionExclusionFoldout = false;

	public bool renderingMaterialFoldout = false;
	public bool renderingColorFoldout = false;
	public bool renderingRenderModeFoldout = false;
	public bool renderingShadowsFoldout = false;

	public bool advancedTimeFoldout = false;
	public bool advancedSimulationFoldout = false;
	public bool advancedRebirthOptionsFoldout = false;
	public bool advancedLocksFoldout = false;
	public bool advancedOnEnableFoldout = false;
	public bool advancedAutoPauseFoldout = false;
	public bool advancedMiscFoldout = false;

	// Open/Closed tabs Playground Manager Inspector
	public bool playgroundFoldout = true;
	public bool particleSystemsFoldout = false;
	public bool globalManipulatorsFoldout = false;
	public bool advancedSettingsFoldout = false;
	public bool manipulatorTargetsFoldout = false;

	// Open/Closed tabs Playground Spline
	public bool playgroundSplineFoldout = true;
	public bool nodesFoldout = false;
	public bool usedByFoldout = false;
	public bool splineAdvancedFoldout = false;

	// Open/Closed tabs Playground Recorder
	public bool playgroundRecorderFoldout = true;
	public bool recorderPlaybackFoldout = true;
	public bool recorderAdvancedFoldout = false;

	// Open/Closed tabs Playground Trails
	public bool playgroundTrailsFoldout = true;
	public bool trailsTimeFoldout = false;
	public bool trailsWidthFoldout = false;
	public bool trailsPointCreationFoldout = false;
	public bool trailsRenderingFoldout = false;
	public bool trailsAdvancedFoldout = false;

	// Open/Closed tabs Playground Follow
	public bool playgroundFollowFoldout = true;
	public bool followFollowersFoldout = false;
	public bool followAdvancedFoldout = false;

	// Paths
	public string playgroundPath = "Particle Playground/";
	public string examplePresetPath = "Playground Assets/Presets/";
	public string presetPath = "Resources/Presets/";
	public string iconPath = "Graphics/Editor/Icons/";
	public string brushPath = "Playground Assets/Brushes/";
	public string languagePath = "Playground Assets/Settings/Languages/";
	public string scriptPath = "Scripts/";
	public string versionUrl = "http://www.polyfied.com/products/playgroundversion.php";
	public string extensionsUrl = "http://www.polyfied.com/products/playground/extensions/extensions.xml";

	// Limits
	public float maximumAllowedLifetime = 100f;							// The maximum value for Particle Lifetime in Editor
	public int maximumAllowedParticles = 100000;						// The maximum value for Particle Count in Editor
	public float maximumAllowedRotation = 360f;							// The maximum value for Minimum- and Maximum Particle Rotation Speed in Editor
	public float maximumAllowedSize = 10f;								// The maximum value for Minimum- and Maximum Particle Size in Editor
	public float maximumAllowedDeltaMovementStrength = 100f;			// The maximum value for Particle Delta Movement Strength in Editor
	public float maximumAllowedScale = 10f;								// The maximum value for Particle Scale in Editor
	public float maximumAllowedDamping = 10f;							// The maximum value for Particle Damping in Editor
	public float maximumAllowedVelocity = 100f;							// The maximum value for Particle Max Velocity in Editor
	public float maximumAllowedDepth = 100f;							// The maximum value for Particle Collision Depth in Editor (collision type Physics2D)
	public float maximumAllowedMass = 100f;								// The maximum value for Particle Mass in Editor
	public float maximumAllowedCollisionRadius = 10f;					// The maximum value for Particle Collision Radius in Editor
	public float maximumAllowedBounciness = 2f;							// The maximum value for Particle Bounciness in Editor
	public int minimumAllowedUpdateRate = 10;							// The minimum value for Particle Update Rate in Editor
	public float maximumAllowedTransitionTime = 10f;					// The maximum value for Particle Transition Time in Editor
	public float maximumAllowedTimescale = 2f;							// The maximum value for Particle Timescale
	public int maximumAllowedPaintPositions = 100000;					// The maximum value for Paint Positions
	public float minimumAllowedBrushScale = .001f;						// The minimum scale of a Brush
	public float maximumAllowedBrushScale = 1f;							// The maximum scale of a Brush
	public float maximumAllowedPaintSpacing = 10f;						// The maximum spacing when painting
	public float maximumAllowedInitialVelocity = 100f;					// The maximum value for Minimum- and Maximum Initial (+Local) Velocity
	public float minimumEraserRadius = .001f;							// The minimum value for Eraser radius
	public float maximumEraserRadius = 100f;							// The maximum value for Eraser radius
	public float maximumRenderSliders = 10f;							// The minimum- and maximum value for sliders in Rendering
	public float maximumAllowedManipulatorSize = 100f;					// The maximum value for Manipulator Size
	public float maximumAllowedManipulatorStrength = 100f;				// The maximum value for Manipulator Strength
	public float maximumAllowedManipulatorStrengthEffectors = 2f;		// The maximum value for Manipulator Strength Effect Scale
	public float maximumAllowedManipulatorZeroVelocity = 10f;			// The maximum value for Manipulator Property Zero Velocity Strength
	public float maximumAllowedSourceScatter = 10f;						// The maximum value for scattering source positions
	public float maximumAllowedTurbulenceTimeScale = 10f;				// The maximum value for turbulence time scale
	public float maximumAllowedTurbulenceStrength = 100f;				// The maximum value for turbulence strength
	public float maximumAllowedTurbulenceScale = 10f;					// The maximum value for turbulence scale
	public float maximumAllowedStretchSpeed = 10f;						// The maximum value for stretch speed
	public int maximumAllowedPrewarmCycles = 128;						// The maximum value for prewarm cycles (resolution)

	// Textures
	public Texture2D playgroundIcon;
	public Texture2D playgroundIconInactive;
	public Texture2D playgroundIconHeavy;

	// Language
	public int selectedLanguage = 0;
	public List<PlaygroundLanguageC> languages = new List<PlaygroundLanguageC>();

	public static PlaygroundLanguageC GetLanguage () {
		if (reference.languages.Count>0)
			reference.selectedLanguage = reference.selectedLanguage%reference.languages.Count;
		else reference.selectedLanguage = 0;
		return GetLanguage (reference.selectedLanguage);
	}
	public static PlaygroundLanguageC GetLanguage (int i) {
		if (reference.languages.Count>0) {
			if (i<reference.languages.Count && reference.languages[i] != null)
				return reference.languages[i];
			else
				return ScriptableObject.CreateInstance<PlaygroundLanguageC>();
		} else return ScriptableObject.CreateInstance<PlaygroundLanguageC>();
	}


	public static PlaygroundSettingsC GetReference () {
		if (reference!=null)
			return reference;
		else
			return SetReference();
	}

	public static PlaygroundSettingsC SetReference () {
		reference = (PlaygroundSettingsC)AssetDatabase.LoadAssetAtPath ("Assets/"+settingsPath, typeof(PlaygroundSettingsC));

		// If reference is null - look in the PlaygroundCache for new file path
		if (reference==null)
		{
			if (Directory.Exists("PlaygroundCache") && File.Exists(playgroundCacheSettingsLocation))
			{
				string text = File.ReadAllText(playgroundCacheSettingsLocation);
				if (text.Length>0)
				{
					settingsPath = text;
					reference = (PlaygroundSettingsC)AssetDatabase.LoadAssetAtPath (settingsPath, typeof(PlaygroundSettingsC));
					if (reference != null)
						reference.isInstance = false;
				}
			}
		}

		// If reference still is null - create a temporary settings instance
		if (reference==null) {
			reference = ScriptableObject.CreateInstance<PlaygroundSettingsC>();
			reference.isInstance = true;
		} else reference.isInstance = false;

		return reference;
	}

	public static void SetReference (object thisRef) {
		reference = (PlaygroundSettingsC)thisRef;
		reference.isInstance = false;
	}

	public static void New () {
		PlaygroundSettingsC newSettings = ScriptableObject.CreateInstance<PlaygroundSettingsC>();
		AssetDatabase.CreateAsset(newSettings, "Assets/"+settingsPath);
		AssetDatabase.SaveAssets();
	}

	public bool IsInstance () {
		return reference.isInstance;
	}

	public static bool SetPlaygroundSettingsLocation ()
	{
		UnityEngine.Object[] settingsFiles = GetAssetsOfType(typeof(PlaygroundSettingsC), ".asset");
		if (settingsFiles.Length>0) {
			reference = (PlaygroundSettingsC)settingsFiles[0];
			reference.isInstance = false;

			// Write this to file so we don't have to search next time
			if (!Directory.Exists("PlaygroundCache"))
				Directory.CreateDirectory("PlaygroundCache");
			if (!File.Exists(playgroundCacheSettingsLocation))
			{
				File.WriteAllText(playgroundCacheSettingsLocation, AssetDatabase.GetAssetPath((UnityEngine.Object)reference));
			}

			return true;
		}
		return false;
	}

	public static Texture2D GetGraphicsAsset (string path, string imageName) {
		Texture2D graphicAsset = (Texture2D)AssetDatabase.LoadAssetAtPath (path+imageName, typeof(Texture2D));
		if (graphicAsset==null) {
			string[] imageFiles = GetAssetPathsOfType(".png");
			foreach (string obj in imageFiles) {
				if (obj.EndsWith(imageName)) {
					return (Texture2D)AssetDatabase.LoadAssetAtPath(obj, typeof(Texture2D));
				}
			}
		}
		return graphicAsset;
	}

	static string[] assetPathsOfType;
	/// <summary>
	/// Searches through the Project folder for specific file types. This returns an array of asset paths.
	/// The results will be temporarily cached for performance.
	/// </summary>
	/// <returns>The asset paths of type.</returns>
	/// <param name="fileExtension">File extension, ex. ".prefab".</param>
	public static string[] GetAssetPathsOfType (string fileExtension) {
		if (assetPathsOfType!=null && assetPathsOfType.Length>0 && assetPathsOfType[0].EndsWith(fileExtension))
			return assetPathsOfType;
		List<string> tempPaths = new List<string>();
		DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
		FileInfo[] goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);
		for (int i = 0; i<goFileInfo.Length; i++) {
			if (goFileInfo[i].FullName.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase)) {
				string tmpFilePath = goFileInfo[i].FullName;
				tmpFilePath = tmpFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
				tempPaths.Add (tmpFilePath);
			}
		}
		assetPathsOfType = tempPaths.ToArray();
		return assetPathsOfType;
	}

	public static void ResetAssetPathCache () {
		assetPathsOfType = null;
	}

	static UnityEngine.Object[] assetsOfType;
	/// <summary>
	/// Searches through the Project folder for specific files. This returns an array of objects.
	/// The results will be temporarily cached for performance.
	/// </summary>
	/// <returns>The assets (objects) of type.</returns>
	/// <param name="type">Type, ex. typeof(GameObject).</param>
	/// <param name="fileExtension">File extension, ex. ".prefab".</param>
	public static UnityEngine.Object[] GetAssetsOfType (System.Type type, string fileExtension) {
		if (assetsOfType!=null && assetsOfType.Length>0 && assetsOfType.GetType()==type)
			return assetsOfType;
		List<UnityEngine.Object> tempObjects = new List<UnityEngine.Object>();
		DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
		FileInfo[] goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);

		FileInfo tempGoFileInfo; string tempFilePath;
		UnityEngine.Object tempGO;
		for (int i = 0; i < goFileInfo.Length; i++) {
			tempGoFileInfo = goFileInfo[i];
			if (tempGoFileInfo == null)
				continue;

			tempFilePath = tempGoFileInfo.FullName;
			tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");

			tempGO = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(UnityEngine.Object));
			if (tempGO == null) {
				continue;
			}
			else if (tempGO.GetType() != type) {
				continue;
			}

			tempObjects.Add(tempGO);
		}
		assetsOfType = tempObjects.ToArray();
		return assetsOfType;
	}

	public static void ResetAssetTypeCache () {
		assetsOfType = null;
	}

	public static void ExportLanguage (int i) {
		ExportLanguage (GetLanguage(i));
	}

	public static void ExportLanguage (PlaygroundLanguageC selectedLanguage) {
		string xmlPath = EditorUtility.SaveFilePanel (selectedLanguage.saveLanguage, "", "Particle Playground Language - "+selectedLanguage.languageNameSeenByEnglish+".xml", "xml");

		if (xmlPath.Length!=0) {
			XmlDocument xml = new XmlDocument();
			XmlNode rootNode = xml.CreateElement ("Playground");
			xml.AppendChild (rootNode);
			SerializedObject serializedLanguage = new SerializedObject(selectedLanguage);
			SerializedProperty languageProperty = serializedLanguage.GetIterator();
			while (languageProperty.Next(true)) {
				if (languageProperty.propertyType==SerializedPropertyType.String) {
					XmlNode newNode = xml.CreateElement(languageProperty.propertyPath);
					newNode.InnerText = languageProperty.stringValue;
					rootNode.AppendChild (newNode);
				}
			}
			xml.Save (xmlPath);
		}
	}

	public static void ImportLanguage (string xmlPath) {
		if (xmlPath.Length!=0) {
			XmlDocument xml = new XmlDocument();
			xml.Load (xmlPath);
			XmlNode nameNode = xml.SelectSingleNode("//languageNameSeenByEnglish");

			if (nameNode!=null) {
				PlaygroundLanguageC newLanguage = PlaygroundLanguageC.New (nameNode.InnerText.Length>0?nameNode.InnerText:"New Language");

				SerializedObject serializedLanguage = new SerializedObject(newLanguage);
				SerializedProperty languageProperty = serializedLanguage.GetIterator();
				XmlNodeList nodeList = xml.FirstChild.ChildNodes;
				while (languageProperty.Next(true)) {
					if (languageProperty.propertyType==SerializedPropertyType.String)
					if (languageProperty.propertyType==SerializedPropertyType.String) {
						foreach (XmlNode node in nodeList) {
							if (node!=null && node.InnerText.Length!=0 && node.NodeType==XmlNodeType.Element && node.LocalName==languageProperty.propertyPath) {
								languageProperty.stringValue = node.InnerText;
							}
						}
					}
				}

				serializedLanguage.ApplyModifiedProperties();
				reference.languages.Add (newLanguage);
				EditorUtility.SetDirty (reference);

			}
		}
	}
}
