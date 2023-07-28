using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

class PlaygroundParticleWindowC : EditorWindow {
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticleWindow variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// Update check
	bool hasInternet;
	WWW versionRequest;
	string onlineVersion;

	// Extensions download
	public static WWW extensionsRequest;
	public static string extensionsText;
	public static bool extensionsAvailable;
	public static bool canDisplayExtensions;
	public static List<ExtensionObjectC> extensionObjects;
	public static int iconDownloadIndex;

	// Presets
	public static List<PresetObjectC> presetObjects;
	public static List<PresetCategory> presetCategories;
	public static List<string> presetNames;

	// Settings
	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;

	// Editor Window specific
	public static Vector2 scrollPosition;
	public static GUIStyle presetButtonStyle;
	public static string searchString = "";
	public static string searchStringExtensions = "";
	public static GUIStyle toolbarSearchSkin;
	public static GUIStyle toolbarSearchButtonSkin;
	public static GUIStyle boxStyle;
	public static bool didSendVersionCheck = false;
	public static bool updateAvailable = false;
	public static bool assetsFound = true;
	public static bool didSendExtensionsCheck = false;

	[MenuItem ("Window/Particle Playground")]
	public static void ShowWindow () {
		PlaygroundParticleWindowC window = GetWindow<PlaygroundParticleWindowC>();
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
		window.title = "Playground";
#else
		window.titleContent.text = "Playground";
#endif
        window.Show();
	}
	
	public void OnEnable () {
		Initialize();
	}
	
	public void OnProjectChange () {
		Initialize();
	}
	
	public void Initialize () {
		presetButtonStyle = new GUIStyle();
		presetButtonStyle.stretchWidth = true;
		presetButtonStyle.stretchHeight = true;

		// Load settings
		playgroundSettings = PlaygroundSettingsC.SetReference();
		PlaygroundInspectorC.playgroundSettings = playgroundSettings;
		PlaygroundParticleSystemInspectorC.playgroundSettings = playgroundSettings;

		// Load language
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
		PlaygroundInspectorC.playgroundLanguage = playgroundLanguage;
		PlaygroundParticleSystemInspectorC.playgroundLanguage = playgroundLanguage;

		// Get all asset presets
		string assetsDataPath = Application.dataPath;
		if (Directory.Exists (assetsDataPath+"/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath)) {
			assetsFound = true;
		} else {
			assetsFound = false;
			playgroundSettings.settingsFoldout = true;
			playgroundSettings.settingsPathFoldout = true;
			return;
		}

		// Set default particle image
		Texture2D particleImageDefault = AssetDatabase.LoadAssetAtPath("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.iconPath+"Default.png", typeof(Texture2D)) as Texture2D;

		// Prepare any previously entered search words to filter presets
		string[] searchSplits = searchString.Split(new string[]{" "}, System.StringSplitOptions.None);

		// Get the list of categories and presets
		string[] editorPresetCategories = Directory.GetDirectories(assetsDataPath+"/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath);
		presetCategories = new List<PresetCategory>();

		// A linear list of preset objects is kept for easier access
		presetObjects = new List<PresetObjectC>();

		// A linear list of preset names is kept for easier access
		presetNames = new List<string>();

		// List all categories in asset folders
		for (int i = 0; i<editorPresetCategories.Length; i++)
		{
			// Create the category
			string categoryName = editorPresetCategories[i].Substring(editorPresetCategories[i].LastIndexOf('/')+1);
			presetCategories.Add(new PresetCategory(categoryName));

			// Assign the preset objects to the category
			string[] presetsInCategory = Directory.GetFiles (editorPresetCategories[i]);
			for (int x = 0; x<presetsInCategory.Length; x++)
			{
				string convertedPresetPath = presetsInCategory[x].Substring(assetsDataPath.Length-6);
				Object presetPathObject = (Object)AssetDatabase.LoadAssetAtPath(convertedPresetPath, typeof(Object));
				if (presetPathObject!=null && (presetPathObject.GetType().Name)=="GameObject") {
					presetCategories[i].presetObjects.Add (new PresetObjectC(presetPathObject));

					int presetCountInCategory = presetCategories[i].presetObjects.Count-1;

					// Add to list of preset names
					presetNames.Add (categoryName+" - "+presetCategories[i].presetObjects[presetCountInCategory].presetObject.name);

					// Assign an image to the preset object
					Texture2D particleImage = AssetDatabase.LoadAssetAtPath("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.iconPath+presetCategories[i].presetObjects[presetCountInCategory].presetObject.name+".png", typeof(Texture2D)) as Texture2D;

					// Try the asset location if we didn't find an image in regular editor folder
					if (particleImage==null)
						particleImage = AssetDatabase.LoadAssetAtPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(presetCategories[i].presetObjects[presetCountInCategory].presetObject as UnityEngine.Object))+"/"+presetCategories[i].presetObjects[presetCountInCategory].presetObject.name+".png", typeof(Texture2D)) as Texture2D;
					
					// Finally use the specified icon (or the default)
					if (particleImage!=null)
						presetCategories[i].presetObjects[presetCountInCategory].presetImage = particleImage;
					else if (particleImageDefault!=null)
						presetCategories[i].presetObjects[presetCountInCategory].presetImage = particleImageDefault;

					// Set this to Asset
					presetCategories[i].presetObjects[presetCountInCategory].example = true;

					// Filter the previous search
					foreach (string split in searchSplits)
						presetCategories[i].presetObjects[presetCountInCategory].unfiltered = (searchString.Length==0?true:presetNames[i].ToLower().Contains(split.ToLower()));

					// Add to linear list of preset objects
					presetObjects.Add (presetCategories[i].presetObjects[presetCountInCategory]);
				}
			}
		}

		// List any loose presets
		string[] loosePresets = Directory.GetFiles (assetsDataPath+"/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath);
		bool loosePresetsHasGo = false;
		for (int i = 0; i<loosePresets.Length; i++)
		{
			string convertedPresetPath = loosePresets[i].Substring(assetsDataPath.Length-6);
			Object obj = (Object)AssetDatabase.LoadAssetAtPath(convertedPresetPath, typeof(Object));
			if (obj!= null && obj.GetType().Name=="GameObject")
			{
				loosePresetsHasGo = true;
				break;
			}
		}
		if (loosePresets.Length>0 && loosePresetsHasGo) {
			presetCategories.Add(new PresetCategory("Uncategorized"));
			int categoryCount = presetCategories.Count-1;
			for (int x = 0; x<loosePresets.Length; x++)
			{
				string convertedPresetPath = loosePresets[x].Substring(assetsDataPath.Length-6);
				Object presetPathObject = (Object)AssetDatabase.LoadAssetAtPath(convertedPresetPath, typeof(Object));
				if (presetPathObject!=null && (presetPathObject.GetType().Name)=="GameObject") {
					presetCategories[categoryCount].presetObjects.Add (new PresetObjectC(presetPathObject));
					int presetCountInCategory = presetCategories[categoryCount].presetObjects.Count-1;
					presetNames.Add ("Uncategorized - "+presetCategories[categoryCount].presetObjects[presetCountInCategory].presetObject.name);
					Texture2D particleImage = AssetDatabase.LoadAssetAtPath("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.iconPath+presetCategories[categoryCount].presetObjects[presetCountInCategory].presetObject.name+".png", typeof(Texture2D)) as Texture2D;
					if (particleImage==null)
						particleImage = AssetDatabase.LoadAssetAtPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(presetCategories[categoryCount].presetObjects[presetCountInCategory].presetObject as UnityEngine.Object))+"/"+presetCategories[categoryCount].presetObjects[presetCountInCategory].presetObject.name+".png", typeof(Texture2D)) as Texture2D;
					if (particleImage!=null)
						presetCategories[categoryCount].presetObjects[presetCountInCategory].presetImage = particleImage;
					else if (particleImageDefault!=null)
						presetCategories[categoryCount].presetObjects[presetCountInCategory].presetImage = particleImageDefault;
					presetCategories[categoryCount].presetObjects[presetCountInCategory].example = true;
					foreach (string split in searchSplits)
						presetCategories[categoryCount].presetObjects[presetCountInCategory].unfiltered = (searchString==""?true:presetNames[categoryCount].ToLower().Contains(split.ToLower()));
					presetObjects.Add (presetCategories[categoryCount].presetObjects[presetCountInCategory]);
				}
			}
		}

		// List any resources presets
		Object[] resourcePrefabs = (Object[])Resources.LoadAll ("Presets", typeof(GameObject));
		if (resourcePrefabs.Length > 0) {
			presetCategories.Add(new PresetCategory("Resources"));
			int categoryCount = presetCategories.Count-1;
			for (int i = 0; i<resourcePrefabs.Length; i++)
			{
				presetCategories[categoryCount].presetObjects.Add (new PresetObjectC(resourcePrefabs[i]));
				int presetCountInCategory = presetCategories[categoryCount].presetObjects.Count-1;
				presetNames.Add ("Resources - "+presetCategories[categoryCount].presetObjects[presetCountInCategory].presetObject.name);
				Texture2D particleImage = AssetDatabase.LoadAssetAtPath("Assets/"+playgroundSettings.playgroundPath+playgroundSettings.iconPath+presetCategories[categoryCount].presetObjects[presetCountInCategory].presetObject.name+".png", typeof(Texture2D)) as Texture2D;
				if (particleImage==null)
					particleImage = AssetDatabase.LoadAssetAtPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(presetCategories[categoryCount].presetObjects[presetCountInCategory].presetObject as UnityEngine.Object))+"/"+presetCategories[categoryCount].presetObjects[presetCountInCategory].presetObject.name+".png", typeof(Texture2D)) as Texture2D;
				if (particleImage!=null)
					presetCategories[categoryCount].presetObjects[presetCountInCategory].presetImage = particleImage;
				else if (particleImageDefault!=null)
					presetCategories[categoryCount].presetObjects[presetCountInCategory].presetImage = particleImageDefault;
				presetCategories[categoryCount].presetObjects[presetCountInCategory].example = false;
				foreach (string split in searchSplits)
					presetCategories[categoryCount].presetObjects[presetCountInCategory].unfiltered = (searchString==""?true:presetNames[categoryCount].ToLower().Contains(split.ToLower()));
				presetObjects.Add (presetCategories[categoryCount].presetObjects[presetCountInCategory]);
			}
		}

		// Check for internet connection
		hasInternet = CheckForInternetConnection();
	}

	bool CheckForInternetConnection ()
	{
		System.Net.WebClient client = new System.Net.WebClient();
		System.Uri versionUrl = new System.Uri (playgroundSettings.versionUrl);
		try {
			 client.OpenReadAsync(versionUrl);
			return true;
		} catch {
			return false;
		} finally {
			if(client != null)
				client.Dispose();
		}
	}

	void CheckUpdate () {

		// Check if an update is available
		if (!didSendVersionCheck) {
			if (versionRequest==null)
				versionRequest = new WWW(playgroundSettings.versionUrl);
			if (versionRequest.isDone) {
				if (versionRequest.error==null) {
					onlineVersion = versionRequest.text;
					updateAvailable = (onlineVersion!="" && float.Parse (onlineVersion)>PlaygroundC.version);
				}
				didSendVersionCheck = true;
			}
		}
	}

	string extensionsXmlLocation = "PlaygroundCache/extensions.xml";
	void CheckExtensions () {
		if (!playgroundSettings.enableExtensions) return;
		
		// Download extensions list
		if (!didSendExtensionsCheck) {
			if (extensionsRequest==null)
				extensionsRequest = new WWW(playgroundSettings.extensionsUrl);
			if (extensionsRequest.isDone) {
				if (extensionsRequest.error==null) {
					extensionsText = extensionsRequest.text;

					LoadExtensionItems(true);
				} else {
					if (File.Exists(extensionsXmlLocation)) {
						extensionsText = File.ReadAllText(extensionsXmlLocation);
						if (extensionsText!=null || extensionsText!="")
							LoadExtensionItems(false);
					}
				}
				didSendExtensionsCheck = true;
			}
		}
	}

	void LoadExtensionItems (bool saveXml) {

		XmlDocument xml = new XmlDocument();
		xml.LoadXml(extensionsText);
		if (saveXml) {
			Directory.CreateDirectory ("PlaygroundCache/");
			xml.Save (extensionsXmlLocation);
		}
		IEnumerator xmlRoot = xml.DocumentElement.GetEnumerator();
		extensionObjects = new List<ExtensionObjectC>();
		while (xmlRoot.MoveNext()) {
			ExtensionObjectC extObj = new ExtensionObjectC();
			extensionObjects.Add (extObj);
			XmlElement xmlElement = (XmlElement)xmlRoot.Current;
			extObj.title = xmlElement.SelectSingleNode("title").InnerText;
			extObj.id = xmlElement.SelectSingleNode("id").InnerText;
			extObj.iconUrl = xmlElement.SelectSingleNode("icon").InnerText;
			extObj.iconId = xmlElement.SelectSingleNode("iconid").InnerText;
			extObj.publisher = xmlElement.SelectSingleNode("publisher").InnerText;
			extObj.publisherUrl = xmlElement.SelectSingleNode("publisherurl").InnerText;
			extObj.category = xmlElement.SelectSingleNode("category").InnerText;

			if (File.Exists ("PlaygroundCache/"+extObj.iconId+".png")) {
				byte[] fileData = File.ReadAllBytes("PlaygroundCache/"+extObj.iconId+".png");
				extObj.icon = new Texture2D(2, 2);
				extObj.icon.LoadImage(fileData);
			} else {
				extObj.PrepareDownload();
			}

			extObj.searchMeta = extObj.title+" \n"+extObj.id+" \n"+extObj.category+" \n"+extObj.publisher;
			extObj.unfiltered = (searchStringExtensions==""?true:extObj.searchMeta.ToLower().Contains(searchStringExtensions.ToLower()));
		}

		extensionsAvailable = true;

		GetWindow<PlaygroundParticleWindowC>().Repaint();
	}

	void OnSelectionChange() {

		// Detect selection changes to make the Playground Component buttons responsive
		Transform currentSelection = Selection.activeTransform;
		if (currentSelection != null)
		{
			GetWindow<PlaygroundParticleWindowC>().Repaint();
			Selection.activeTransform = currentSelection;
			EditorApplication.ExecuteMenuItem("Window/Hierarchy");
		}
	}

	void OnGUI () {

		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		if (toolbarSearchSkin==null) {
			toolbarSearchSkin = GUI.skin.FindStyle("ToolbarSeachTextField");
			if (toolbarSearchButtonSkin==null)
				toolbarSearchButtonSkin = GUI.skin.FindStyle("ToolbarSeachCancelButton");
		}
		EditorGUILayout.BeginVertical();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		EditorGUILayout.BeginVertical(boxStyle);
		EditorGUILayout.Separator();
		EditorGUILayout.LabelField(playgroundLanguage.playgroundName+" "+PlaygroundC.version+PlaygroundC.specialVersion, EditorStyles.largeLabel, GUILayout.Height(20));
		EditorGUILayout.Separator();

		// Playground Settings is an instance (give option to search and assign the stored reference)
		if (playgroundSettings.IsInstance())
		{
			GUI.backgroundColor = Color.red;
			EditorGUILayout.BeginVertical(boxStyle);
			EditorGUILayout.HelpBox (playgroundLanguage.findPlaygroundSettingsMsg, MessageType.Warning);
			if (GUILayout.Button(playgroundLanguage.findPlaygroundSettings, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
				PlaygroundSettingsC.SetPlaygroundSettingsLocation();
			}
			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
		}

		// New version message
		if (hasInternet)
		{
			if (playgroundSettings.checkForUpdates && !didSendVersionCheck)
				CheckUpdate();
			if (playgroundSettings.checkForUpdates && updateAvailable) {
				GUI.backgroundColor = Color.yellow;
				EditorGUILayout.BeginVertical(boxStyle);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(playgroundLanguage.updateAvailable);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("x", EditorStyles.miniButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)})){
					updateAvailable = false;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.LabelField(onlineVersion+" "+playgroundLanguage.updateAvailableText, EditorStyles.wordWrappedMiniLabel);
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(playgroundLanguage.unityAssetStore, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
					Application.OpenURL("com.unity3d.kharma:content/13325");
				}
				GUI.backgroundColor = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				EditorGUILayout.Separator();
			}
		}

		EditorGUILayout.BeginVertical(boxStyle);
		
		// Create New-buttons
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button(playgroundLanguage.newParticlePlaygroundSystem, EditorStyles.toolbarButton)){
			if (PlaygroundC.reference==null)
				CreateManager();
			PlaygroundParticlesC newParticlePlayground = PlaygroundC.Particle();
			newParticlePlayground.EditorYieldSelect();
		}
		GUI.enabled = PlaygroundC.reference==null;
		if(GUILayout.Button(playgroundLanguage.playgroundManager, EditorStyles.toolbarButton)){
			PlaygroundC.ResourceInstantiate("Playground Manager");
		}
		GUI.enabled = true;
		if(GUILayout.Button(playgroundLanguage.presetWizard, EditorStyles.toolbarButton)){
			PlaygroundCreatePresetWindowC.ShowWindow();
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		// Components menu
		EditorGUILayout.BeginVertical(boxStyle);
		EditorGUILayout.LabelField(playgroundLanguage.playgroundComponents);
		EditorGUILayout.BeginHorizontal();
		GameObject currentSelection = Selection.activeGameObject;
		bool hasSelection = currentSelection != null;
		bool hasSpline = hasSelection? currentSelection.GetComponent<PlaygroundSplines.PlaygroundSpline>() != null : false;
		bool hasFollow = hasSelection? currentSelection.GetComponent<PlaygroundFollow>() != null : false;
		bool hasTrail = hasSelection? currentSelection.GetComponent<PlaygroundTrails>() != null : false;
		bool hasRecorder = hasSelection? currentSelection.GetComponent<PlaygroundRecorder>() != null : false;

		GUI.enabled = hasSelection;

		// Playground Spline
		GUI.color = hasSpline?Color.green : Color.white;
		if(GUILayout.Button(hasSpline? playgroundLanguage.playgroundSpline+" (Attached)" : playgroundLanguage.playgroundSpline, EditorStyles.toolbarButton)){
			if (!hasSpline)
				currentSelection.AddComponent<PlaygroundSplines.PlaygroundSpline>();
			else
				if (EditorUtility.DisplayDialog(playgroundLanguage.removeComponent, playgroundLanguage.removeComponentMsg, playgroundLanguage.yes, playgroundLanguage.no))
					DestroyImmediate(currentSelection.GetComponent<PlaygroundSplines.PlaygroundSpline>());
		}

		// Playground Trails
		GUI.color = hasTrail?Color.green : Color.white;
		if(GUILayout.Button(hasTrail? playgroundLanguage.playgroundTrails+" (Attached)" : playgroundLanguage.playgroundTrails, EditorStyles.toolbarButton)){
			if (!hasTrail)
				currentSelection.AddComponent<PlaygroundTrails>();
			else
				if (EditorUtility.DisplayDialog(playgroundLanguage.removeComponent, playgroundLanguage.removeComponentMsg, playgroundLanguage.yes, playgroundLanguage.no))
					DestroyImmediate(currentSelection.GetComponent<PlaygroundTrails>());
		}

		// Playground Follow
		GUI.color = hasFollow?Color.green : Color.white;
		if(GUILayout.Button(hasFollow? playgroundLanguage.playgroundFollow+" (Attached)" : playgroundLanguage.playgroundFollow, EditorStyles.toolbarButton)){
			if (!hasFollow)
				currentSelection.AddComponent<PlaygroundFollow>();
			else
				if (EditorUtility.DisplayDialog(playgroundLanguage.removeComponent, playgroundLanguage.removeComponentMsg, playgroundLanguage.yes, playgroundLanguage.no))
					DestroyImmediate(currentSelection.GetComponent<PlaygroundFollow>());
		}

		// Playground Recorder
		GUI.color = hasRecorder?Color.green : Color.white;
		if(GUILayout.Button(hasRecorder? playgroundLanguage.playgroundRecorder+" (Attached)" : playgroundLanguage.playgroundRecorder, EditorStyles.toolbarButton)){
			if (!hasRecorder)
				currentSelection.AddComponent<PlaygroundRecorder>();
			else
				if (EditorUtility.DisplayDialog(playgroundLanguage.removeComponent, playgroundLanguage.removeComponentMsg, playgroundLanguage.yes, playgroundLanguage.no))
					DestroyImmediate(currentSelection.GetComponent<PlaygroundRecorder>());
		}

		GUI.enabled = true;
		EditorGUILayout.EndHorizontal();
		GUI.color = Color.white;
		EditorGUILayout.EndVertical();

		EditorGUILayout.EndVertical();

		if (assetsFound) {

			// Presets
			EditorGUILayout.BeginVertical(boxStyle);
			playgroundSettings.presetsFoldout = GUILayout.Toggle(playgroundSettings.presetsFoldout, playgroundLanguage.presets, EditorStyles.foldout);
			if (playgroundSettings.presetsFoldout) {
				EditorGUILayout.BeginHorizontal("Toolbar");
				
				// Search
				string prevSearchString = searchString;
				searchString = GUILayout.TextField(searchString, toolbarSearchSkin, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(Mathf.FloorToInt(Screen.width)-120), GUILayout.MinWidth(100)});
				if (GUILayout.Button("", toolbarSearchButtonSkin)) {
					searchString = "";
					for (int i = 0; i<presetObjects.Count; i++)
						presetObjects[i].unfiltered = true;
					GUI.FocusControl(null);
				}

				// Set the filter based on search
				if (prevSearchString!=searchString) {
					string[] searchSplits = searchString.Split(new string[]{" "}, System.StringSplitOptions.None);
					for (int p = 0; p<presetNames.Count; p++)
					{
						foreach (string split in searchSplits)
						{
							presetObjects[p].unfiltered = (searchString.Length==0?true:presetNames[p].ToLower().Contains(split.ToLower()));
							if (!presetObjects[p].unfiltered)
								break;
						}
					}
				}
				
				EditorGUILayout.Separator();
				playgroundSettings.presetIconSize = GUILayout.HorizontalSlider (playgroundSettings.presetIconSize, .5f, 1f, GUILayout.MaxWidth(60));
				int iconSize = Mathf.RoundToInt(75*playgroundSettings.presetIconSize);
				bool listSize = playgroundSettings.presetIconSize<=.5;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginVertical(boxStyle);

				// List all presets
				if (presetCategories.Count>0) {
					for (int c = 0; c<presetCategories.Count; c++) {

						if (searchString.Length == 0)
						{
							GUI.enabled = presetCategories[c].presetObjects.Count>0;
							if (GUILayout.Button(presetCategories[c].categoryName+" ("+presetCategories[c].presetObjects.Count+")", EditorStyles.toolbarDropDown)) presetCategories[c].foldout = !presetCategories[c].foldout;
						}
						else presetCategories[c].foldout = true;

						if (presetCategories[c].foldout && GUI.enabled) {

							if (!listSize) EditorGUILayout.BeginHorizontal();
							int rows = 1;
							int iconwidths = 0;
							int skippedPresets = 0;

							for (int i = 0; i<presetCategories[c].presetObjects.Count; i++) {
								
								// Filter out by search
								if (!presetCategories[c].presetObjects[i].unfiltered) {
									skippedPresets++;
									continue;
								}
								// Preset Object were destroyed
								if (presetCategories[c].presetObjects[i].presetObject==null) {
									Initialize();
									return;
								}
								
								// List
								if (listSize) {
									EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(24));
									EditorGUILayout.BeginHorizontal();
									GUILayout.Label(i.ToString(), EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.Width(18)});
									EditorGUILayout.LabelField(presetCategories[c].presetObjects[i].example?"("+presetCategories[c].categoryName+")":"("+playgroundLanguage.resources+")", EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.MaxWidth(80)});
									GUILayout.Label (presetCategories[c].presetObjects[i].presetImage, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(24)});
									if (GUILayout.Button (presetCategories[c].presetObjects[i].presetObject.name, EditorStyles.label)) {
										CreatePresetObject(c, i);
									}
									EditorGUILayout.Separator();
									if(GUILayout.Button(presetCategories[c].presetObjects[i].example?playgroundLanguage.convertTo+" "+playgroundLanguage.resources:playgroundLanguage.convertTo+" "+playgroundLanguage.asset, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Height(16)})){
										if (presetCategories[c].presetObjects[i].example) {
											AssetDatabase.MoveAsset (AssetDatabase.GetAssetPath(presetCategories[c].presetObjects[i].presetObject), "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.presetPath+presetCategories[c].presetObjects[i].presetObject.name+".prefab");
										} else {
											AssetDatabase.MoveAsset (AssetDatabase.GetAssetPath(presetCategories[c].presetObjects[i].presetObject), "Assets/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath+presetCategories[c].presetObjects[i].presetObject.name+".prefab");
										}
									}
									if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
										RemovePreset(presetCategories[c].presetObjects[i].presetObject);
										return;
									}
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.EndVertical();
								} 
								else
								{
									// Break row for icons
									rows = Mathf.FloorToInt(Screen.width/ (342+iconSize));
									iconwidths+=342+iconSize;
									if (iconwidths>Screen.width && i>0) {
										iconwidths=342+iconSize;
										EditorGUILayout.EndHorizontal();
										EditorGUILayout.BeginHorizontal();
									}
									if (Screen.width>=644) {
										EditorGUILayout.BeginVertical(boxStyle, GUILayout.MaxWidth (Mathf.CeilToInt(Screen.width/(rows))-(44/(rows))));
									} else
										EditorGUILayout.BeginVertical(boxStyle);
									EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(46));
									if(GUILayout.Button(presetCategories[c].presetObjects[i].presetImage, EditorStyles.miniButton, new GUILayoutOption[]{GUILayout.Width(iconSize+12), GUILayout.Height(iconSize+12)})){
										CreatePresetObject(c, i);
									}
									EditorGUILayout.BeginVertical();

									if (GUILayout.Button(presetCategories[c].presetObjects[i].presetObject.name, EditorStyles.label, GUILayout.Height(18)))
										CreatePresetObject(c, i);
									EditorGUILayout.LabelField(presetCategories[c].categoryName, EditorStyles.miniLabel);
									EditorGUILayout.EndVertical();
									GUILayout.FlexibleSpace();
									EditorGUILayout.BeginVertical();

									if(GUILayout.Button("x", EditorStyles.miniButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(18)})){
										RemovePreset(presetCategories[c].presetObjects[i].presetObject);
										return;
									}
									EditorGUILayout.EndVertical();
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.EndVertical();

								}
							}

							GUI.enabled = true;

							if (skippedPresets==presetObjects.Count) {
								if (searchString!="") {
									EditorGUILayout.HelpBox(playgroundLanguage.searchNoPresetFound+" \""+searchString+"\".", MessageType.Info);
								} else {
									EditorGUILayout.HelpBox(playgroundLanguage.noPresetsFound, MessageType.Info);
								}
							}
							if (!listSize) EditorGUILayout.EndHorizontal();
						}
					}
				} else {
					EditorGUILayout.HelpBox(playgroundLanguage.noPresetsFoundInProject+" \""+"Assets/"+playgroundSettings.playgroundPath+playgroundSettings.examplePresetPath+"\"", MessageType.Info);
				}

				GUI.enabled = true;

				EditorGUILayout.Separator();

				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
					PlaygroundCreatePresetWindowC.ShowWindow();
				}
				EditorGUILayout.Separator();
				if (GUILayout.Button(playgroundLanguage.refresh, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
					AssetDatabase.Refresh();
					Initialize();
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		} else {
			EditorGUILayout.HelpBox (playgroundLanguage.noAssetsFoundMessage, MessageType.Warning);
		}

		// Extension check
		if (hasInternet) {
			if (playgroundSettings.enableExtensions && !didSendExtensionsCheck)
				CheckExtensions();
			
			// Extension icon download
			if (playgroundSettings.enableExtensions && extensionObjects!=null && iconDownloadIndex<extensionObjects.Count) {
				if (extensionObjects[iconDownloadIndex].IsExpectingDownload())
					extensionObjects[iconDownloadIndex].IconDownloadRoutine();
				else {
					iconDownloadIndex++;
				}
			}
		}

		// Extend Your Playground GUI
		if (playgroundSettings.enableExtensions) {
			EditorGUILayout.BeginVertical(boxStyle);
			playgroundSettings.extensionsFoldout = GUILayout.Toggle(playgroundSettings.extensionsFoldout, playgroundLanguage.extendYourPlayground, EditorStyles.foldout);
			if (playgroundSettings.extensionsFoldout) {
				EditorGUILayout.BeginHorizontal("Toolbar");
				
				// Search
				string prevSearchString = searchStringExtensions;
				searchStringExtensions = GUILayout.TextField(searchStringExtensions, toolbarSearchSkin, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(Mathf.FloorToInt(Screen.width)-120), GUILayout.MinWidth(100)});
				if (GUILayout.Button("", toolbarSearchButtonSkin)) {
					searchStringExtensions = "";
					GUI.FocusControl(null);
				}
				
				if (prevSearchString!=searchStringExtensions) {
					for (int p = 0; p<extensionObjects.Count; p++)
						extensionObjects[p].unfiltered = (searchStringExtensions==""?true:extensionObjects[p].searchMeta.ToLower().Contains(searchStringExtensions.ToLower()));
				}
				
				EditorGUILayout.Separator();
				
				playgroundSettings.extensionIconSize = GUILayout.HorizontalSlider (playgroundSettings.extensionIconSize, .5f, 1f, GUILayout.MaxWidth(60));
				EditorGUILayout.EndHorizontal();
				
				if (didSendExtensionsCheck) {
					if (canDisplayExtensions) {
						EditorGUILayout.BeginHorizontal();
						int rows = 1;
						int iconwidths = 0;
						int iconSize = Mathf.RoundToInt(75*playgroundSettings.extensionIconSize);
						int skippedExtensions = 0;
						bool smallSize = playgroundSettings.extensionIconSize<=.65;
						bool listSize = playgroundSettings.extensionIconSize<=.5;
						bool didLoad = false;
						
						// List of extensions
						if (listSize) {
							EditorGUILayout.BeginVertical();
							for (int i = 0; i<extensionObjects.Count; i++) {
								
								// Filter out by search
								if (!extensionObjects[i].unfiltered) {
									skippedExtensions++;
									continue;
								}
								
								if (!didLoad)
									didLoad = extensionObjects[i].CheckAvailability();
								
								EditorGUILayout.BeginHorizontal(boxStyle);
								GUILayout.Label (extensionObjects[i].icon, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(24)});
								if (GUILayout.Button(extensionObjects[i].title, EditorStyles.boldLabel, GUILayout.ExpandWidth(true)))
									OpenUAS(extensionObjects[i].id);
								
								if (GUILayout.Button(extensionObjects[i].publisher, EditorStyles.miniLabel, GUILayout.ExpandWidth(false))) {
									searchStringExtensions = extensionObjects[i].publisher;
									for (int p = 0; p<extensionObjects.Count; p++)
										extensionObjects[p].unfiltered = (searchStringExtensions==""?true:extensionObjects[p].searchMeta.ToLower().Contains(searchStringExtensions.ToLower()));
								}
								
								if (GUILayout.Button(extensionObjects[i].category, boxStyle, GUILayout.ExpandWidth(false))) {
									searchStringExtensions = extensionObjects[i].category;
									for (int p = 0; p<extensionObjects.Count; p++)
										extensionObjects[p].unfiltered = (searchStringExtensions==""?true:extensionObjects[p].searchMeta.ToLower().Contains(searchStringExtensions.ToLower()));
								}
								
								EditorGUILayout.EndHorizontal();
							}
							EditorGUILayout.EndVertical();
						} else {
						
							for (int i = 0; i<extensionObjects.Count; i++) {
								
								// Filter out by search
								if (!extensionObjects[i].unfiltered) {
									skippedExtensions++;
									continue;
								}
								
								if (!didLoad)
									didLoad = extensionObjects[i].CheckAvailability();
								
								// Break row for icons
								rows = Mathf.FloorToInt(Screen.width/322);
								iconwidths+=322;
								if (iconwidths>Screen.width && i>0) {
									iconwidths=322;
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.BeginHorizontal();
								}
								
								if (Screen.width>=644) {
									EditorGUILayout.BeginVertical(boxStyle, GUILayout.MaxWidth (Mathf.CeilToInt(Screen.width/rows)-(45/rows)));
								} else
									EditorGUILayout.BeginVertical(boxStyle);
								EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(iconSize+16));
								if(GUILayout.Button(extensionObjects[i].icon, EditorStyles.miniButton, new GUILayoutOption[]{GUILayout.Width(iconSize+12), GUILayout.Height(iconSize+12)})){
									OpenUAS(extensionObjects[i].id);
								}
								EditorGUILayout.BeginVertical();
								
								EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(iconSize+10));
								if (GUILayout.Button(extensionObjects[i].title, EditorStyles.boldLabel, GUILayout.ExpandWidth(true)))
									OpenUAS(extensionObjects[i].id);
								
								if (smallSize) {
									EditorGUILayout.BeginHorizontal();
								}
								
								if (GUILayout.Button(extensionObjects[i].category, EditorStyles.label, GUILayout.ExpandWidth(false))) {
									searchStringExtensions = extensionObjects[i].category;
									for (int p = 0; p<extensionObjects.Count; p++)
										extensionObjects[p].unfiltered = (searchStringExtensions==""?true:extensionObjects[p].searchMeta.ToLower().Contains(searchStringExtensions.ToLower()));
								}
								
								if (GUILayout.Button(extensionObjects[i].publisher, EditorStyles.miniLabel, GUILayout.ExpandWidth(false))) {
									searchStringExtensions = extensionObjects[i].publisher;
									for (int p = 0; p<extensionObjects.Count; p++)
										extensionObjects[p].unfiltered = (searchStringExtensions==""?true:extensionObjects[p].searchMeta.ToLower().Contains(searchStringExtensions.ToLower()));
								}
								
								if (smallSize) {
									EditorGUILayout.EndHorizontal();
								}
								
								EditorGUILayout.EndHorizontal();
								EditorGUILayout.EndVertical();
								EditorGUILayout.EndVertical();
								EditorGUILayout.EndVertical();
							}
						}
						
						EditorGUILayout.EndHorizontal();
						
						// No extensions found by search
						if (skippedExtensions==extensionObjects.Count) {
							if (searchStringExtensions!="") {
								EditorGUILayout.HelpBox(playgroundLanguage.searchNoExtensionsFound+" \""+searchStringExtensions+"\".", MessageType.Info);
							}
						} else if (skippedExtensions>0 && searchStringExtensions!="")
							EditorGUILayout.HelpBox(playgroundLanguage.listHasBeenFilteredWith+": \""+searchStringExtensions+"\".", MessageType.Info);
						
						
					} else EditorGUILayout.HelpBox (playgroundLanguage.extensionsDownloadError, MessageType.Warning);
				} else EditorGUILayout.HelpBox (playgroundLanguage.extensionsDownloading, MessageType.Info);
			}
			EditorGUILayout.EndVertical();
		}

		PlaygroundInspectorC.RenderPlaygroundSettings();

		// Playground Settings
		EditorGUILayout.BeginVertical(boxStyle);
		playgroundSettings.settingsFoldout = GUILayout.Toggle(playgroundSettings.settingsFoldout, playgroundLanguage.settings, EditorStyles.foldout);
		if (playgroundSettings.settingsFoldout) {
			EditorGUILayout.BeginVertical(boxStyle);
			if (playgroundSettings==null || playgroundSettings.IsInstance()) {
				EditorGUILayout.HelpBox(playgroundLanguage.noSettingsFile+" \""+PlaygroundSettingsC.settingsPath+"\".", MessageType.Warning);
				if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
					PlaygroundSettingsC.New();
					Initialize();
					return;
				}
			} else {
				playgroundSettings.checkForUpdates = EditorGUILayout.Toggle (playgroundLanguage.checkForUpdates, playgroundSettings.checkForUpdates);
				playgroundSettings.enableExtensions = EditorGUILayout.Toggle (playgroundLanguage.extendYourPlayground, playgroundSettings.enableExtensions);
				playgroundSettings.presetsHasPrefabConnection = EditorGUILayout.Toggle (playgroundLanguage.prefabConnection, playgroundSettings.presetsHasPrefabConnection);

				EditorGUI.BeginChangeCheck();
				playgroundSettings.hierarchyIcon = EditorGUILayout.Toggle (playgroundLanguage.hierarchyIcon, playgroundSettings.hierarchyIcon);
				if (EditorGUI.EndChangeCheck()) {
					PlaygroundHierarchyIcon.Set();
				}
				EditorGUILayout.Separator();

				EditorGUILayout.BeginVertical(boxStyle);
				playgroundSettings.settingsLanguageFoldout = GUILayout.Toggle(playgroundSettings.settingsLanguageFoldout, playgroundLanguage.language, EditorStyles.foldout);
				if (playgroundSettings.settingsLanguageFoldout) {
					if (playgroundSettings.languages.Count>0) {


						bool setThisLoadFrom = false;
						int currentLanguageCount = playgroundSettings.languages.Count;
						for (int i = 0; i<playgroundSettings.languages.Count; i++) {
							if (currentLanguageCount!=playgroundSettings.languages.Count) {
								Initialize();
								return;
							}
							setThisLoadFrom = false;
							EditorGUILayout.BeginHorizontal(boxStyle);

							if (playgroundSettings.languages[i]==null) {
								playgroundSettings.languages.RemoveAt (i);
								Initialize();
								return;
							}

							if (playgroundSettings.selectedLanguage == i) {
								EditorGUILayout.Toggle (true, EditorStyles.radioButton, GUILayout.Width(14));
							} else
								setThisLoadFrom = EditorGUILayout.Toggle (setThisLoadFrom, EditorStyles.radioButton, GUILayout.Width(14));
							if (setThisLoadFrom || GUILayout.Button (playgroundSettings.languages[i].languageName+" ("+playgroundSettings.languages[i].languageNameSeenByEnglish+")", EditorStyles.label)) {
								playgroundSettings.selectedLanguage = i;
								playgroundLanguage = PlaygroundSettingsC.GetLanguage();
								PlaygroundInspectorC.playgroundLanguage = playgroundLanguage;
								PlaygroundParticleSystemInspectorC.playgroundLanguage = playgroundLanguage;
							}

							if(GUILayout.Button(playgroundLanguage.edit, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Height(16)}))
								Selection.activeObject = (Object)playgroundSettings.languages[i];
							if(GUILayout.Button(playgroundLanguage.export, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Height(16)})) 
								PlaygroundSettingsC.ExportLanguage(i);
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								if (EditorUtility.DisplayDialog(
									playgroundLanguage.remove+" "+playgroundSettings.languages[i].languageName+"?",
									playgroundLanguage.removeLanguage+" "+playgroundSettings.languages[i].languageName+"?", 
									playgroundLanguage.yes, playgroundLanguage.no)) {
									AssetDatabase.MoveAssetToTrash (AssetDatabase.GetAssetPath (playgroundSettings.languages[i]));
									playgroundSettings.languages.RemoveAt (i);
									if (playgroundSettings.selectedLanguage == i)
										playgroundSettings.selectedLanguage = 0;
									Initialize();
									return;
								}
							}

							EditorGUILayout.EndHorizontal();
						}
					} else {
						EditorGUILayout.HelpBox(playgroundLanguage.noLanguageFound, MessageType.Warning);
					}
					EditorGUILayout.BeginHorizontal();
					if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						playgroundSettings.languages.Add (PlaygroundLanguageC.New());
						EditorUtility.SetDirty(playgroundSettings);
						Initialize();
						return;
					}
					if(GUILayout.Button(playgroundLanguage.install, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						PlaygroundInstallLanguageWindowC.ShowWindow();
					}
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.EndVertical();

				// Limits
				EditorGUILayout.BeginVertical(boxStyle);
				playgroundSettings.limitsFoldout = GUILayout.Toggle(playgroundSettings.limitsFoldout, playgroundLanguage.editorLimits, EditorStyles.foldout);
				if (playgroundSettings.limitsFoldout) {
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedTransitionTime = EditorGUILayout.FloatField(playgroundLanguage.transitionTime, playgroundSettings.maximumAllowedTransitionTime);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedParticles = EditorGUILayout.IntField(playgroundLanguage.particleCount, playgroundSettings.maximumAllowedParticles);
					playgroundSettings.maximumAllowedLifetime = EditorGUILayout.FloatField(playgroundLanguage.particleLifetime, playgroundSettings.maximumAllowedLifetime);
					playgroundSettings.maximumAllowedRotation = EditorGUILayout.FloatField(playgroundLanguage.particleRotation, playgroundSettings.maximumAllowedRotation);
					playgroundSettings.maximumAllowedSize = EditorGUILayout.FloatField(playgroundLanguage.particleSize, playgroundSettings.maximumAllowedSize);
					playgroundSettings.maximumAllowedScale = EditorGUILayout.FloatField(playgroundLanguage.particleScale, playgroundSettings.maximumAllowedScale);
					playgroundSettings.maximumAllowedSourceScatter = EditorGUILayout.FloatField(playgroundLanguage.sourceScatter, playgroundSettings.maximumAllowedSourceScatter);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedDeltaMovementStrength = EditorGUILayout.FloatField(playgroundLanguage.deltaMovementStrength, playgroundSettings.maximumAllowedDeltaMovementStrength);
					playgroundSettings.maximumAllowedDamping = EditorGUILayout.FloatField(playgroundLanguage.damping, playgroundSettings.maximumAllowedDamping);
					playgroundSettings.maximumAllowedInitialVelocity = EditorGUILayout.FloatField(playgroundLanguage.initialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
					playgroundSettings.maximumAllowedVelocity = EditorGUILayout.FloatField(playgroundLanguage.velocity, playgroundSettings.maximumAllowedVelocity);
					playgroundSettings.maximumAllowedStretchSpeed = EditorGUILayout.FloatField(playgroundLanguage.stretchSpeed, playgroundSettings.maximumAllowedStretchSpeed);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedCollisionRadius = EditorGUILayout.FloatField(playgroundLanguage.collisionRadius, playgroundSettings.maximumAllowedCollisionRadius);
					playgroundSettings.maximumAllowedMass = EditorGUILayout.FloatField(playgroundLanguage.mass, playgroundSettings.maximumAllowedMass);
					playgroundSettings.maximumAllowedBounciness = EditorGUILayout.FloatField(playgroundLanguage.bounciness, playgroundSettings.maximumAllowedBounciness);
					playgroundSettings.maximumAllowedDepth = EditorGUILayout.FloatField(playgroundLanguage.depth2D, playgroundSettings.maximumAllowedDepth);
					EditorGUILayout.Separator();
					playgroundSettings.minimumAllowedUpdateRate = EditorGUILayout.IntField(playgroundLanguage.updateRate, playgroundSettings.minimumAllowedUpdateRate);
					playgroundSettings.maximumAllowedTimescale = EditorGUILayout.FloatField (playgroundLanguage.timeScale, playgroundSettings.maximumAllowedTimescale);
					playgroundSettings.maximumAllowedPrewarmCycles = EditorGUILayout.IntField (playgroundLanguage.prewarmCyclesResolution, playgroundSettings.maximumAllowedPrewarmCycles);
					playgroundSettings.maximumRenderSliders = EditorGUILayout.FloatField(playgroundLanguage.renderSliders, playgroundSettings.maximumRenderSliders);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedPaintPositions = EditorGUILayout.IntField(playgroundLanguage.paintPositions, playgroundSettings.maximumAllowedPaintPositions);
					playgroundSettings.minimumAllowedBrushScale = EditorGUILayout.FloatField(playgroundLanguage.brushSizeMin, playgroundSettings.minimumAllowedBrushScale);
					playgroundSettings.maximumAllowedBrushScale = EditorGUILayout.FloatField(playgroundLanguage.brushSizeMax, playgroundSettings.maximumAllowedBrushScale);
					playgroundSettings.minimumEraserRadius = EditorGUILayout.FloatField(playgroundLanguage.eraserSizeMin, playgroundSettings.minimumEraserRadius);
					playgroundSettings.maximumEraserRadius = EditorGUILayout.FloatField(playgroundLanguage.eraserSizeMax, playgroundSettings.maximumEraserRadius);
					playgroundSettings.maximumAllowedPaintSpacing = EditorGUILayout.FloatField(playgroundLanguage.paintSpacing, playgroundSettings.maximumAllowedPaintSpacing);
					EditorGUILayout.Separator();
					playgroundSettings.maximumAllowedManipulatorSize = EditorGUILayout.FloatField(playgroundLanguage.manipulatorSize, playgroundSettings.maximumAllowedManipulatorSize);
					playgroundSettings.maximumAllowedManipulatorStrength = EditorGUILayout.FloatField(playgroundLanguage.manipulatorStrength, playgroundSettings.maximumAllowedManipulatorStrength);
					playgroundSettings.maximumAllowedManipulatorStrengthEffectors = EditorGUILayout.FloatField (playgroundLanguage.manipulatorStrengthEffectors, playgroundSettings.maximumAllowedManipulatorStrengthEffectors);
					playgroundSettings.maximumAllowedManipulatorZeroVelocity = EditorGUILayout.FloatField(playgroundLanguage.manipulatorZeroVelocityStrength, playgroundSettings.maximumAllowedManipulatorZeroVelocity);
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();

				// Paths
				EditorGUILayout.BeginVertical(boxStyle);
				playgroundSettings.settingsPathFoldout = GUILayout.Toggle(playgroundSettings.settingsPathFoldout, playgroundLanguage.paths, EditorStyles.foldout);
				if (playgroundSettings.settingsPathFoldout) {
					string currentPlaygroundPath = playgroundSettings.playgroundPath;
					string currentExamplePresetPath = playgroundSettings.examplePresetPath;
					string currentPresetPath = playgroundSettings.presetPath;

					playgroundSettings.playgroundPath = EditorGUILayout.TextField(playgroundLanguage.playgroundPath, playgroundSettings.playgroundPath);
					playgroundSettings.languagePath = EditorGUILayout.TextField(playgroundLanguage.languagesPath, playgroundSettings.languagePath);
					playgroundSettings.presetPath = EditorGUILayout.TextField(playgroundLanguage.resourcesPresetPath, playgroundSettings.presetPath);
					playgroundSettings.examplePresetPath = EditorGUILayout.TextField(playgroundLanguage.assetsPresetPath, playgroundSettings.examplePresetPath);
					playgroundSettings.iconPath = EditorGUILayout.TextField(playgroundLanguage.presetIconPath, playgroundSettings.iconPath);
					playgroundSettings.brushPath = EditorGUILayout.TextField(playgroundLanguage.brushPath, playgroundSettings.brushPath);
					playgroundSettings.scriptPath = EditorGUILayout.TextField(playgroundLanguage.scriptPath, playgroundSettings.scriptPath);
					playgroundSettings.versionUrl = EditorGUILayout.TextField(playgroundLanguage.updateUrl, playgroundSettings.versionUrl);
					playgroundSettings.extensionsUrl = EditorGUILayout.TextField(playgroundLanguage.extensionsUrl, playgroundSettings.extensionsUrl);

					if (currentPlaygroundPath!=playgroundSettings.playgroundPath || currentExamplePresetPath!=playgroundSettings.examplePresetPath || currentPresetPath!=playgroundSettings.presetPath)
						Initialize();
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
		
		GUILayout.FlexibleSpace();
		
		EditorGUILayout.BeginVertical(boxStyle);
		EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(playgroundLanguage.officialSite, EditorStyles.toolbarButton)) {
				Application.OpenURL ("http://playground.polyfied.com/");
			}
			if (GUILayout.Button(playgroundLanguage.assetStore, EditorStyles.toolbarButton)) {
				Application.OpenURL("com.unity3d.kharma:content/13325");
			}
			if (GUILayout.Button(playgroundLanguage.manual, EditorStyles.toolbarButton)) {
				Application.OpenURL ("http://www.polyfied.com/products/Particle-Playground-3-Next-Manual.pdf");
			}
			if (GUILayout.Button(playgroundLanguage.supportForum, EditorStyles.toolbarButton)) {
				Application.OpenURL ("http://forum.unity3d.com/threads/215154-Particle-Playground");
			}
			if (GUILayout.Button(playgroundLanguage.mailSupport, EditorStyles.toolbarButton)) {
				Application.OpenURL ("mailto:support@polyfied.com");
			}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
		
		GUILayout.EndScrollView();
		EditorGUILayout.EndVertical();

		if (GUI.changed)
			EditorUtility.SetDirty(playgroundSettings);
	}

	void Update ()
	{
		canDisplayExtensions = extensionsAvailable && extensionObjects!=null;
	}

	public static void OpenUAS (string id) {
		Application.OpenURL("com.unity3d.kharma:content/"+id);
	}

	public void RemovePreset (Object presetObject) {
		if (EditorUtility.DisplayDialog(playgroundLanguage.removePreset, 
		                                presetObject.name+" "+playgroundLanguage.removePresetText, 
		                                playgroundLanguage.yes, 
		                                playgroundLanguage.no)) {
			AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(presetObject));
		}
	}

	// User created preset
	public void CreatePresetObject (int categoryIndex, int presetIndex) {
		PlaygroundParticlesC instantiatedPreset = InstantiateEditorPreset(presetCategories[categoryIndex].presetObjects[presetIndex].presetObject);
		if (instantiatedPreset!=null)
			instantiatedPreset.EditorYieldSelect();
	}
	
	// Instantiate a preset by name reference
	public static PlaygroundParticlesC InstantiateEditorPreset (Object presetObject) {
		GameObject presetGo;
		if (playgroundSettings.presetsHasPrefabConnection) {
			presetGo = (GameObject)PrefabUtility.InstantiatePrefab(presetObject);
		} else {
			presetGo = (GameObject)Instantiate(presetObject);
		}
		PlaygroundParticlesC presetParticles = presetGo.GetComponent<PlaygroundParticlesC>();
		if (presetParticles!=null) {
			if (PlaygroundC.reference==null)
				PlaygroundC.ResourceInstantiate("Playground Manager");
			if (PlaygroundC.reference) {
				if (PlaygroundC.reference.autoGroup && presetParticles.particleSystemTransform.parent==null)
					presetParticles.particleSystemTransform.parent = PlaygroundC.referenceTransform;
				PlaygroundC.particlesQuantity++;
				presetParticles.particleSystemId = PlaygroundC.particlesQuantity;
			}
			presetGo.name = presetObject.name;
			return presetParticles;
		} else return null;
	}
	
	public void CreateManager () {
		GameObject pmGo = PlaygroundC.ResourceInstantiate("Playground Manager");
		PlaygroundC pm = pmGo.GetComponent<PlaygroundC>();
		PlaygroundC.reference = pm;
	}
}

public class PresetCategory {
	public string categoryName = "";
	public bool foldout = false;
	public List<PresetObjectC> presetObjects = new List<PresetObjectC>();

	public PresetCategory (string categoryName) {
		this.categoryName = categoryName;
	}
}

public class PresetObjectC {
	public Object presetObject;
	public Texture2D presetImage;
	public string presetPath;
	public bool unfiltered = true;
	public bool example = false;

	public PresetObjectC () {}

	public PresetObjectC (Object presetObject)
	{
		this.presetObject = presetObject;
	}
}

[System.Serializable]
public class ExtensionObjectC {

	public string title;
	public string id;
	public string publisher;
	public string publisherUrl;
	public string category;
	public string iconUrl;
	public string iconId;
	public Texture2D icon;
	public WWW iconDownload;

	public bool unfiltered = true;
	public string searchMeta = "";

	bool shouldDownloadIcon;
	bool downloadStarted;
	bool downloadFinished;

	public void PrepareDownload () {
		shouldDownloadIcon = true;
	}

	public bool IsExpectingDownload () {
		return shouldDownloadIcon;
	}

	public bool IconDownloadRoutine () {
		if (!shouldDownloadIcon) {
			return true;
		}
		if (!downloadStarted)
			StartIconDownload();
		if (iconDownload.error!=null) {
			shouldDownloadIcon = false;
			downloadFinished = true;
			iconDownload.Dispose();
			return true;
		}
		if (!downloadFinished && iconDownload.isDone) {
			icon = new Texture2D(75,75,TextureFormat.ARGB32,false);
			icon = iconDownload.texture;
			SaveIconToCache();
			shouldDownloadIcon = false;
			downloadFinished = true;
			iconDownload.Dispose();
		}
		return downloadFinished;
	}

	public void StartIconDownload () {
		downloadStarted = true;
		iconDownload = new WWW(iconUrl);
	}

	public void SaveIconToCache () {
		byte[] bytes = icon.EncodeToPNG();
		FileStream file = File.Create("PlaygroundCache/"+iconId+".png");
		BinaryWriter binary = new BinaryWriter(file);
		binary.Write(bytes);
		file.Close();
	}

	public bool CheckAvailability () {
		if (icon==null && iconId!=null) {
			if (File.Exists ("PlaygroundCache/"+iconId+".png")) {
				byte[] fileData = File.ReadAllBytes("PlaygroundCache/"+iconId+".png");
				icon = new Texture2D(2, 2);
				icon.LoadImage(fileData);
				return true;
			}
			return false;
		}
		return false;
	}
}