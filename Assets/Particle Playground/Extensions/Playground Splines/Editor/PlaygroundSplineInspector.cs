using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using PlaygroundSplines;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

[CustomEditor(typeof(PlaygroundSpline))]
public class PlaygroundSplineInspector : Editor {
	
	private const int stepsPerCurve = 10;
	private const float directionScale = 0.5f;
	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;
	
	private PlaygroundSpline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;
	private Quaternion bezierHandleRotation;
	private int selectedIndex = -1;
	private int selectedNode = -1;
	private int selectedBezier = -1;
	private bool selectedIndexIsNode;
	private UnityEditor.Tool lastActiveTool = UnityEditor.Tool.None;
	
	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;
	public static GUIStyle boxStyle;
	
	void OnEnable () {
		spline = target as PlaygroundSpline;
		
		playgroundSettings = PlaygroundSettingsC.GetReference();
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
		
		lastActiveTool = UnityEditor.Tools.current;
		
		UpdateUserList();
	}
	
	void OnDisable () {
		UnityEditor.Tools.current = lastActiveTool;
	}
	
	void UpdateUserList () {
		
		// Check that any user isn't null
		for (int i = 0; i<spline.usedBy.Count; i++) {
			if (spline.usedBy[i]==null || !spline.usedBy[i].GetComponent<PlaygroundParticlesC>().splines.Contains(spline)) {
				spline.usedBy.RemoveAt (i);
				i--;
			}
		}
	}
	
	public override void OnInspectorGUI () {
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		
		EditorGUILayout.BeginVertical(boxStyle);
		playgroundSettings.playgroundSplineFoldout = GUILayout.Toggle(playgroundSettings.playgroundSplineFoldout, playgroundLanguage.playgroundSpline, EditorStyles.foldout);
		if (playgroundSettings.playgroundSplineFoldout) {
			
			selectedNode = (selectedIndex+1)/3;
			selectedBezier = selectedIndex<3?0:(((selectedIndex)%3))%2;
			selectedIndexIsNode = selectedIndex==0||selectedIndex%3==0;
			
			EditorGUILayout.Separator();
			
			bool currentLoop = spline.Loop;
			GUI.enabled = spline.NodeCount>1;
			bool loop = EditorGUILayout.Toggle(playgroundLanguage.loop, spline.Loop);
			GUI.enabled = true;
			spline.reverse = EditorGUILayout.Toggle(playgroundLanguage.reverse, spline.reverse);
			spline.timeOffset = EditorGUILayout.Slider (playgroundLanguage.timeOffset, spline.timeOffset, 0, 1f);
			spline.positionOffset = EditorGUILayout.Vector3Field (playgroundLanguage.positionOffset, spline.positionOffset);
			
			EditorGUILayout.Separator();
			EditorGUILayout.BeginVertical(boxStyle);
			if (selectedIndex < spline.ControlPointCount) {
				DrawSelectedPointInspector();
			}
			if (currentLoop!=loop) {
				Undo.RecordObject(spline, "Toggle Loop");
				EditorUtility.SetDirty(spline);
				spline.Loop = loop;
			}
			EditorGUILayout.EndVertical();
			
			// List of nodes
			if (GUILayout.Button(playgroundLanguage.nodes+" ("+(spline.NodeCount+1)+")", EditorStyles.toolbarDropDown)) playgroundSettings.nodesFoldout=!playgroundSettings.nodesFoldout;
			if (playgroundSettings.nodesFoldout) {
				
				EditorGUILayout.Separator();
				
				EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
				for (int i = 0; i<=spline.NodeCount; i++) {
					if (i==selectedNode && selectedIndex>-1) GUI.backgroundColor = new Color(1f,1f,.2f);
					EditorGUILayout.BeginVertical(boxStyle);
					GUI.backgroundColor = Color.white;
					EditorGUILayout.BeginHorizontal();
					
					// Node
					if (i==selectedNode && selectedIndexIsNode)
						EditorGUILayout.LabelField("", EditorStyles.foldout, GUILayout.Width(14));
					else if (i==selectedNode && selectedIndex!=-1) GUILayout.Space (19f);
					if (GUILayout.Button(playgroundLanguage.node+" "+i+" ("+spline.GetControlPointMode(i*3).ToString()+")", EditorStyles.label, GUILayout.MaxWidth(130)))
						selectedIndex = SelectIndex(i*3);
					GUILayout.Space(3f);
					if (GUILayout.Button(spline.transformNodes[i*3].enabled?playgroundLanguage.transform:playgroundLanguage.vector3, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						spline.transformNodes[i*3].enabled = !spline.transformNodes[i*3].enabled;
					}
					
					EditorGUI.BeginChangeCheck();
					EditorGUIUtility.labelWidth = 1f;
					Vector3 point; 
					if (spline.transformNodes[i*3].enabled) {
						spline.transformNodes[i*3].transform = (Transform)EditorGUILayout.ObjectField(spline.transformNodes[i*3].transform, typeof(Transform), true);
						if (spline.transformNodes[i*3].IsAvailable())
							point = spline.transformNodes[i*3].GetPosition();
						else point = spline.GetControlPoint(i*3);
					} else point = EditorGUILayout.Vector3Field(" ", spline.GetControlPoint(i*3));
					
					EditorGUIUtility.labelWidth = 0;
					if (EditorGUI.EndChangeCheck()) {
						Undo.RecordObject(spline, "Move Point");
						EditorUtility.SetDirty(spline);
						spline.SetControlPoint(i*3, point);
					}
					if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						
					}
					if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						
					}
					EditorGUI.BeginChangeCheck();
					if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						Undo.RecordObject(spline, "Add Node");
						spline.AddNode(i);
						EditorUtility.SetDirty(spline);
						selectedIndex = SelectIndex((i+1)*3);
					}
					GUI.enabled = (!spline.Loop&&spline.NodeCount>1||spline.Loop&&spline.NodeCount>2);
					EditorGUI.BeginChangeCheck();
					if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						Undo.RecordObject(spline, "Remove Node");
						spline.RemoveNode(i);
						EditorUtility.SetDirty(spline);
						selectedIndex-=3;
						if (selectedIndex<0)
							selectedIndex = SelectIndex(0);
						return;
					}
					GUI.enabled = true;
					EditorGUILayout.EndHorizontal();
					
					if (i==selectedNode && selectedIndex>-1) {
						
						int thisBez;
						
						// Bezier 0
						if (selectedNode>0) {
							thisBez = selectedNode>0?(i*3)-1:(spline.NodeCount*3)-1;
							EditorGUILayout.BeginHorizontal();
							if (selectedBezier==0 && !selectedIndexIsNode)
								EditorGUILayout.LabelField("", EditorStyles.foldout, GUILayout.Width(14));
							else GUILayout.Space (19f);
							if (GUILayout.Button(playgroundLanguage.bezier+" 0", EditorStyles.label, GUILayout.MaxWidth(133)))
								selectedIndex = SelectIndex(thisBez);
							EditorGUI.BeginChangeCheck();
							EditorGUIUtility.labelWidth = 1f;
							if (GUILayout.Button(spline.transformNodes[thisBez].enabled?playgroundLanguage.transform:playgroundLanguage.vector3, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
								spline.transformNodes[thisBez].enabled = !spline.transformNodes[thisBez].enabled;
							}
							if (spline.transformNodes[thisBez].enabled) {
								spline.transformNodes[thisBez].transform = (Transform)EditorGUILayout.ObjectField(spline.transformNodes[thisBez].transform, typeof(Transform), true);
								if (spline.transformNodes[thisBez].IsAvailable())
									point = spline.transformNodes[thisBez].GetPosition();
								else point = spline.GetControlPoint(thisBez);
							} else point = EditorGUILayout.Vector3Field(" ", spline.GetControlPoint(thisBez));
							
							GUILayout.Space (35f);
							if (EditorGUI.EndChangeCheck()) {
								Undo.RecordObject(spline, "Move Point");
								EditorUtility.SetDirty(spline);
								spline.SetControlPoint(selectedNode>0?(i*3)-1:(spline.NodeCount*3)-1, point);
							}
							EditorGUILayout.EndHorizontal();
							EditorGUIUtility.labelWidth = 0;
						}
						
						// Bezier 1
						if (selectedNode<spline.NodeCount) {
							thisBez = selectedNode<spline.NodeCount?(i*3)+1:1;
							EditorGUILayout.BeginHorizontal();
							if ((selectedIndex==1||selectedBezier==1) && !selectedIndexIsNode)
								EditorGUILayout.LabelField("", EditorStyles.foldout, GUILayout.Width(14));
							else GUILayout.Space (19f);
							if (GUILayout.Button(selectedNode>0?playgroundLanguage.bezier+" 1":playgroundLanguage.bezier+" 0", EditorStyles.label, GUILayout.MaxWidth(133)))
								selectedIndex = SelectIndex(thisBez);
							EditorGUI.BeginChangeCheck();
							EditorGUIUtility.labelWidth = 1f;
							if (GUILayout.Button(spline.transformNodes[thisBez].enabled?playgroundLanguage.transform:playgroundLanguage.vector3, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
								spline.transformNodes[thisBez].enabled = !spline.transformNodes[thisBez].enabled;
							}
							if (spline.transformNodes[thisBez].enabled) {
								spline.transformNodes[thisBez].transform = (Transform)EditorGUILayout.ObjectField(spline.transformNodes[thisBez].transform, typeof(Transform), true);
								if (spline.transformNodes[thisBez].IsAvailable())
									point = spline.transformNodes[thisBez].GetPosition();
								else point = spline.GetControlPoint(thisBez);
							} else point = EditorGUILayout.Vector3Field(" ", spline.GetControlPoint(thisBez));
							GUILayout.Space (35f);
							if (EditorGUI.EndChangeCheck()) {
								Undo.RecordObject(spline, "Move Point");
								EditorUtility.SetDirty(spline);
								spline.SetControlPoint(thisBez, point);
							}
							EditorGUILayout.EndHorizontal();
							EditorGUIUtility.labelWidth = 0;
						}
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
					Undo.RecordObject(spline, "Add Node");
					selectedNode = spline.NodeCount;
					spline.AddNode(selectedNode);
					EditorUtility.SetDirty(spline);
					selectedIndex = SelectIndex((selectedNode+1)*3);
				}
				EditorGUILayout.Separator();
				if (GUILayout.Button(playgroundLanguage.convertAllToTransforms, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
					Transform pTrans = new GameObject("Nodes").transform;
					pTrans.parent = spline.splineTransform;
					pTrans.localPosition = Vector3.zero;
					Transform[] transforms = spline.ExportToTransforms();
					spline.positionOffset = Vector3.zero;
					for (int i = 0; i<transforms.Length; i++) {
						spline.transformNodes[i].transform = transforms[i];
						spline.transformNodes[i].enabled = true;
						if (transforms[i].parent==spline.splineTransform)
							transforms[i].parent = pTrans;
					}
					Selection.activeTransform = pTrans;
				}
				if (GUILayout.Button(playgroundLanguage.convertAllToVector3, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
					spline.SetPoints(spline.ExportToVector3());
					spline.positionOffset = Vector3.zero;
					for (int i = 0; i<spline.ControlPointCount; i++) {
						spline.transformNodes[i].enabled = false;
					}
				}
				EditorGUILayout.Separator();
				if (GUILayout.Button(playgroundLanguage.reverseAllNodes, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
					spline.ReverseAllNodes();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			
			// List of users
			if (GUILayout.Button(playgroundLanguage.usedBy+" ("+spline.usedBy.Count+")", EditorStyles.toolbarDropDown)) playgroundSettings.usedByFoldout=!playgroundSettings.usedByFoldout;
			if (playgroundSettings.usedByFoldout) {
				
				EditorGUILayout.Separator();
				
				if (spline.usedBy.Count>0) {
					EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
					for (int i = 0; i<spline.usedBy.Count; i++) {
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button(spline.usedBy[i].name, EditorStyles.label))
							Selection.activeTransform = spline.usedBy[i];
						EditorGUILayout.Separator();
						if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
							if (EditorUtility.DisplayDialog(playgroundLanguage.removeUserTitle, playgroundLanguage.removeUserMessage, playgroundLanguage.yes, playgroundLanguage.no)) {
								PlaygroundParticlesC ps = spline.usedBy[i].GetComponent<PlaygroundParticlesC>();
								Undo.RecordObjects(new Object[]{spline,ps}, "Remove User");
								if (ps!=null) {
									Transform user = spline.usedBy[i];
									if (ps.splines.Contains(spline)) {
										spline.RemoveUser (user);
										ps.splines.Remove(spline);
									}
									foreach (ManipulatorObjectC m in ps.manipulators) {
										ManipulatorPropertyC mp = m.property;
										if (mp.splineTarget!=null && mp.splineTarget==spline)
											spline.RemoveUser (user);
										foreach (ManipulatorPropertyC mps in m.properties)
											if (mps.splineTarget!=null && mps.splineTarget==spline)
												spline.RemoveUser (user);
									}
								}
							}
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();
				} else {
					EditorGUILayout.HelpBox(playgroundLanguage.noSplineUserMessage, MessageType.Info);
				}
			}
			
			// Advanced
			if (GUILayout.Button(playgroundLanguage.advanced, EditorStyles.toolbarDropDown)) playgroundSettings.splineAdvancedFoldout=!playgroundSettings.splineAdvancedFoldout;
			if (playgroundSettings.splineAdvancedFoldout) {
				EditorGUILayout.Separator();
				spline.fixedVelocityOnNewNode = EditorGUILayout.FloatField (playgroundLanguage.velocityOnNewNode, spline.fixedVelocityOnNewNode);
				spline.moveTransformsAsBeziers = EditorGUILayout.Toggle (playgroundLanguage.moveTransformsAsBeziers, spline.moveTransformsAsBeziers);
				spline.exportWithNodeStructure = EditorGUILayout.Toggle (playgroundLanguage.exportWithNodeStructure, spline.exportWithNodeStructure);
				EditorGUILayout.Separator();
				spline.drawGizmo = EditorGUILayout.Toggle (playgroundLanguage.drawBezierGizmo, spline.drawGizmo);
				spline.bezierWidth = EditorGUILayout.FloatField (playgroundLanguage.bezierWidth, spline.bezierWidth);
				if (spline.bezierWidth<0) spline.bezierWidth = 0;
			}
		}
		EditorGUILayout.EndVertical();
		SceneView.RepaintAll();
	}
	
	private void DrawSelectedPointInspector() {
		if (selectedIndex>=spline.ControlPointCount) selectedIndex = SelectIndex(spline.ControlPointCount-1);
		EditorGUILayout.PrefixLabel(playgroundLanguage.selection);
		EditorGUILayout.BeginHorizontal(boxStyle);
		if (GUILayout.Button(playgroundLanguage.spline, EditorStyles.label, GUILayout.ExpandWidth(false))) {
			selectedIndex = SelectIndex(-1);
			SceneView.RepaintAll();
		}
		if (selectedIndex>=0) {
			GUILayout.Label(">", EditorStyles.label, GUILayout.MaxWidth(16));
			if (GUILayout.Button(playgroundLanguage.node+" "+selectedNode.ToString(), EditorStyles.label, GUILayout.ExpandWidth(false))) {
				selectedIndex = SelectIndex(selectedNode*3);
			}
		}
		if (selectedIndex>=0 && !selectedIndexIsNode) {
			GUILayout.Label(">", EditorStyles.label, GUILayout.MaxWidth(16));
			GUILayout.Label(playgroundLanguage.bezier+" "+selectedBezier.ToString(), EditorStyles.label, GUILayout.ExpandWidth(false));
		}
		if (selectedIndex>=0) {
			if (spline.transformNodes[selectedIndex].IsAvailable()) {
				if (GUILayout.Button(" ("+playgroundLanguage.transform+") ", EditorStyles.label, GUILayout.ExpandWidth(false)))
					Selection.activeTransform = spline.transformNodes[selectedIndex].transform;
			} else {
				GUILayout.Label(" ("+playgroundLanguage.vector3+")", EditorStyles.label, GUILayout.ExpandWidth(false));
			}
		}
		EditorGUILayout.Separator();
		if (GUILayout.Button ("<", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
			selectedIndex = SelectIndex(selectedIndex-1);
			if (selectedIndex<0)
				selectedIndex = SelectIndex(spline.ControlPointCount-1);
		}
		if (GUILayout.Button (">", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
			selectedIndex = SelectIndex(selectedIndex+1);
			if (selectedIndex>spline.ControlPointCount-1)
				selectedIndex = SelectIndex(0);
		}
		EditorGUILayout.EndHorizontal();
		EditorGUI.BeginChangeCheck();
		
		// A node or bezier is selected
		if (selectedIndex>=0) {
			Vector3 point = EditorGUILayout.Vector3Field(playgroundLanguage.position, spline.GetControlPoint(selectedIndex));
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(selectedIndex, point);
			}
			EditorGUI.BeginChangeCheck();
			PlaygroundSplines.BezierControlPointMode mode = (PlaygroundSplines.BezierControlPointMode)EditorGUILayout.EnumPopup(playgroundLanguage.bezierMode, spline.GetControlPointMode(selectedIndex));
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Change Point Mode");
				spline.SetControlPointMode(selectedIndex, mode);
				EditorUtility.SetDirty(spline);
			}
		}
		
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button(playgroundLanguage.addNode, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
			Undo.RecordObject(spline, "Add Node");
			if (selectedIndex<0) selectedNode = spline.NodeCount;
			spline.AddNode(selectedNode);
			EditorUtility.SetDirty(spline);
			selectedIndex = SelectIndex((selectedNode+1)*3);
		}
		EditorGUILayout.Separator();
		GUI.enabled = (!spline.Loop&&spline.NodeCount>1||spline.Loop&&spline.NodeCount>2);
		if (GUILayout.Button(playgroundLanguage.removeSelectedNode, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
			Undo.RecordObject(spline, "Remove Node");
			spline.RemoveNode(selectedNode);
			EditorUtility.SetDirty(spline);
			selectedIndex-=3;
			if (selectedIndex<0)
				selectedIndex = SelectIndex(0);
		}
		EditorGUILayout.EndHorizontal();
		GUI.enabled = true;
	}
	
	private int SelectIndex (int newIndex) {
		if (newIndex>-1)
			UnityEditor.Tools.current = UnityEditor.Tool.None;
		else UnityEditor.Tools.current = lastActiveTool;
		selectedNode = (newIndex+1)/3;
		selectedBezier = newIndex<3?0:(((newIndex)%3))%2;
		selectedIndexIsNode = newIndex==0||newIndex%3==0;
		return newIndex;
	}
	
	private int foldoutHeight = 0;
	private bool toolboxFoldout = true;
	private bool callForAddNode = false;
	private bool callForRemoveNode = false;
	private void OnSceneGUI () {
		
		callForAddNode = (selectedIndex>-1 && Event.current.control && !Event.current.shift && Event.current.type==EventType.mouseUp);
		callForRemoveNode = (selectedIndex>-1 && Event.current.control && Event.current.shift && Event.current.type==EventType.mouseUp);
		
		handleTransform = spline.transform;
		handleRotation = UnityEditor.Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
		bezierHandleRotation = Camera.current.transform.rotation;
		
		Event e = Event.current;
		if (toolboxFoldout)
			if (selectedIndex<0)
				foldoutHeight = 68;
		else foldoutHeight = 120;
		else
			foldoutHeight = 0;
		Rect toolboxRect = new Rect(10f,Screen.height-(70f+foldoutHeight),300f,103f+foldoutHeight);
		
		// Don't deselect upon click
		if (toolboxFoldout && e.type == EventType.Layout) {
			HandleUtility.AddDefaultControl(0);
		}
		
		// Toolbox
		Handles.BeginGUI();
		GUILayout.BeginArea(toolboxRect);
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		GUILayout.BeginVertical(boxStyle);
		toolboxFoldout = GUILayout.Toggle(toolboxFoldout, playgroundLanguage.playgroundSpline, EditorStyles.foldout);
		if (toolboxFoldout) {
			DrawSelectedPointInspector();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		Handles.EndGUI();
		
		Vector3 p0 = ShowPoint(0);
		for (int i = 1; i < spline.ControlPointCount; i += 3) {
			Vector3 p1 = ShowPoint(i);
			Vector3 p2 = ShowPoint(i + 1);
			Vector3 p3 = ShowPoint(i + 2);
			
			Handles.color = new Color(1f,.8f,0f);
			Handles.DrawLine(p0, p1);
			Handles.DrawLine(p2, p3);
			
			p0 = p3;
		}
		
		if (callForAddNode) {
			Undo.RecordObject(spline, "Add Node");
			spline.AddNode(selectedNode);
			EditorUtility.SetDirty(spline);
			selectedIndex = SelectIndex((selectedNode+1)*3);
		}
		if (callForRemoveNode) {
			Undo.RecordObject(spline, "Remove Node");
			spline.RemoveNode(selectedNode);
			EditorUtility.SetDirty(spline);
			if (selectedIndex>=spline.ControlPointCount)
				selectedIndex = SelectIndex(spline.ControlPointCount-1);
		}
	}
	
	private void ShowDirections () {
		Handles.color = Color.green;
		Vector3 point = spline.GetPoint(0f);
		Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
		int steps = stepsPerCurve * spline.NodeCount;
		for (int i = 1; i <= steps; i++) {
			point = spline.GetPoint(i / (float)steps);
			Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
		}
	}
	
	private Vector3 ShowPoint (int index) {
		Vector3 pointWithOffset = spline.transformNodes[index].IsAvailable()? spline.GetPoint(index)+spline.positionOffset : handleTransform.TransformPoint(spline.GetInversePoint(index)+spline.positionOffset);
		float size = HandleUtility.GetHandleSize(pointWithOffset);
		if (index == 0 && spline.Loop) {
			size *= 2f;
		}
		Handles.color = new Color(1f,.5f,0f);
		if ((index==0||index%3==0)) {
			if (Handles.Button(pointWithOffset, handleRotation, size * handleSize, size * pickSize, Handles.DotCap)) {
				selectedIndex = SelectIndex(index);
				Repaint();
			}
		} else {
			if (Handles.Button(pointWithOffset, bezierHandleRotation, size * handleSize, size * pickSize, Handles.CircleCap)) {
				selectedIndex = SelectIndex(index);
				Repaint();
			}
		}
		
		if (selectedIndex == index) {
			EditorGUI.BeginChangeCheck();
			pointWithOffset = Handles.DoPositionHandle(pointWithOffset, handleRotation);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(index, spline.transformNodes[index].IsAvailable()?pointWithOffset:handleTransform.InverseTransformPoint(pointWithOffset), spline.positionOffset);
			}
		}
		return pointWithOffset;
	}
}