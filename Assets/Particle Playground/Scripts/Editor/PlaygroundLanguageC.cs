using UnityEngine;
using System;

using UnityEditor;


namespace ParticlePlaygroundLanguage {
	[Serializable]
	public class PlaygroundLanguageC : ScriptableObject {

		public string languageNameSeenByEnglish					= "American";
		public string languageName 								= "English";
		public string playgroundName 							= "Particle Playground";

		public string newParticlePlaygroundSystem				= "New Particle Playground System";
		public string playgroundWizard							= "Playground Wizard";
		public string playgroundPresetWizard					= "Playground Preset Wizard";
		public string presetWizard								= "Preset Wizard";
		public string playgroundCopyWizard						= "Playground Copy Wizard";
		public string copyWizard								= "Copy Wizard";
		public string languageInstallWizard						= "Language Install Wizard";
		public string playgroundBrushWizard						= "Playground Brush Wizard";
		public string brushWizard								= "Brush Wizard";
		public string preset									= "Preset";
		public string presets									= "Presets";
		public string publish									= "Publish";
		public string all										= "All";
		public string user										= "User";
		public string example									= "Example";
		public string examples									= "Examples";
		public string assets									= "Assets";
		public string resources									= "Resources";
		public string icon										= "Icon";
		public string icons										= "Icons";
		public string list										= "List";
		public string create									= "Create";
		public string createPreset								= "Create Preset";
		public string convertTo									= "Convert to";
		public string edit										= "Edit";
		public string export									= "Export";
		public string settings									= "Settings";
		public string none										= "None";
		public string fix										= "Fix";
		public string save										= "Save";
		public string load										= "Load";
		public string rebuild									= "Rebuild";
		public string refresh									= "Refresh";
		public string xml										= "Xml";
		public string asset										= "Asset";
		public string browse									= "Browse";
		public string category									= "Category";

		public string paths										= "Paths";
		public string playgroundPath							= "Playground Path";
		public string languagesPath								= "Languages Path";
		public string resourcesPresetPath						= "Resources Preset Path";
		public string assetsPresetPath							= "Assets Preset Path";
		public string presetIconPath							= "Preset Icon Path";
		public string brushPath									= "Brush Path";
		public string scriptPath								= "Script Path";
		public string updateUrl									= "Update URL";
		public string extensionsUrl								= "Extensions URL";
		public string extendYourPlayground						= "Extend Your Playground";

		public string officialSite								= "Official Site";
		public string assetStore								= "Asset Store";
		public string manual									= "Manual";
		public string supportForum								= "Support Forum";
		public string mailSupport								= "Mail Support";

		public string updateAvailable							= "Update Available";
		public string updateAvailableText						= "is available. Please visit the Unity Asset Store to download the new version."; // New version number always added first
		public string unityAssetStore							= "Unity Asset Store";

		public string searchNoPresetFound						= "No preset found containing";
		public string noPresetsFound							= "No presets found. Press \"Create\" to make a new preset.";
		public string noResourcesPresetsFound					= "No resource presets found in any \"Resources/Presets\" folder. Press \"Create\" to make a new preset.";
		public string noAssetPresetsFound						= "No asset presets found. Make sure they are stored in"; // Folder path always added last
		public string noPresetsFoundInProject					= "No presets found. Make sure that the path to the presets are set to"; // Folder path always added last
		public string noSettingsFile							= "No settings file could be found. You must have a settings file at";
		public string noParticleSystems							= "No particle systems created.";
		public string noManipulators							= "No manipulators created.";
		public string noProperties								= "No properties created.";
		public string noTarget									= "No target";
		public string noTargets									= "No targets created.";
		public string noEvents									= "No events created.";
		public string noMesh									= "No Mesh";
		public string noCollisionPlanes							= "No collision planes created.";
		public string noSnapshots								= "No snapshots created.";
		public string noStates									= "No states created.";
		public string allTargets								= "All targets must be assigned.";
		public string noPlaygroundManager						= "The Playground Manager runs all Particle Playground Systems in the scene, you need to create one to get started.";
		public string missingTransform							= "Missing Transform!";
		public string noAlphaColorInPaint						= "You have no alpha in the color. No particle positions will be painted.";
		public string noLifetimeColors							= "No lifetime colors created.";
		public string someFeaturesInScript						= "Some features are inactivated as this Particle Playground System is running in script mode.";
		public string notSnapshot								= "is not a snapshot."; // Snapshot name always added first
		public string notSnapshotText							= "You can only add snapshots into this slot. First create a snapshot from the new particle system you wish to assign.";
		public string projectionOnlyWorldSpace					= "Projection can only run in world space.";
		public string particleSystemEventAssignErrorSelf		= "A particle system cannot send events to itself. Please choose another particle system in your Scene.";
		public string particleSystemEventAssignErrorSnapshot	= "A particle system cannot send events to a snapshot. Please choose another particle system in your Scene.";
		public string enableCollisionToSendEvents				= "You must enable collision on this particle system to send collision events.";

		public string couldNotReadTexture						= "Could not read the Import Settings of the selected texture.";
		public string couldNotReadMesh							= "Could not read the Import Settings of the selected mesh.";

		public string notReadable								= "is not readable. Please change Read/Write Enabled on its Import Settings."; // Object name always added first
		public string editFromHierarchyOnly						= "Please edit this from Hierarchy only.";
		public string localScaleWarning							= "A local scale of anything else than Vector3 (1,1,1) may result in Shuriken component not rendering.";
		public string rotationSimulationSpace					= "Would you like to simulate the particle system in local space? This will let source positions and force settings apply along with the particle system's rotation.";
		public string setLocalSpaceSimulation					= "Set Local Space Simulation";

		public string checkForUpdates							= "Check For Updates";
		public string language									= "Language";
		public string install									= "Install";
		public string installType								= "Install Type";
		public string installText								= "The Playground Language Wizard allows you to add a new language to your Playground Settings. Installing as an Asset you need the language file to already be imported to your project.";
		public string languageFile								= "Language File";
		public string languageLocation							= "Language Location";
		public string saveLanguage								= "Save Language";
		public string loadLanguage								= "Load Language";
		public string noLanguageFound							= "No language could be found.";

		public string remove									= "Remove";
		public string removeAll									= "Remove all";
		public string removeLanguage							= "Are you sure you want to remove the language"; // Language name always added last
		public string removePreset								= "Permanently delete this preset?";
		public string removePresetText							= "will be removed. Are you sure?"; // Preset name always added first
		public string removeParticlePlaygroundSystem			= "Are you sure you want to remove this Particle Playground System?";
		public string removeManipulator							= "Are you sure you want to remove the Manipulator assigned to"; // GameObject name always added last
		public string removeEvent								= "Remove the events from"; // Object name always added last
		public string removeEventsText1							= "All events connected to this particle system from"; // Object name in the middle of Text1 and Text2
		public string removeEventsText2							= "will be removed. Are you sure you want to remove them?"; // Object name in the middle of Text1 and Text2
		public string removeEventInEventList					= "Are you sure you want to remove this event?";
		public string removeSnapshot							= "Are you sure you want to remove the snapshot"; // Snapshot name always added last
		public string removeAllSnapshots						= "Remove all snapshots?";
		public string removeAllSnapshotsText					= "Are you sure you want to remove all snapshot?";
		public string removeState								= "Are you sure you want to remove the state"; // State name always added after
		public string deleteBrush								= "Permanently delete this brush?";
		public string deleteBrushText							= "will be removed from your brushes, are you sure?"; // Brush name always added first
		public string gameObjectIntact							= "(GameObject in Scene will remain intact)";
		public string switchToScriptMode						= "Switch to Script Mode?";
		public string switchToScriptModeText1					= "The event target of"; // Target name after
		public string switchToScriptModeText2					= "is running in"; // Target name before, source mode after
		public string switchToScriptModeText3					= "mode. All events must be received by Script Mode."; // Source mode before
		public string yes										= "Yes";
		public string no										= "No";
		public string ok										= "Ok";
		public string switchText								= "Switch";
		public string cancel									= "Cancel";

		public string openPlaygroundWizard						= "Open Playground Wizard";
		public string playgroundParticles 						= "Playground Particles";
		public string playgroundManager 						= "Playground Manager";
		public string eventControlled							= "Event Controlled";
		public string controlledByAnotherEvent					= "This Particle Playground System is controlled by events from another particle system.";
		public string controlledByScript						= "This Particle Playground System is controlled by script. You can only emit particles from script in this source mode using PlaygroundParticlesC.Emit(position, velocity, color) or let another particle system control it by events. Please see the manual for more details.";
		public string emissionIndex								= "Emission Index";
		public string emissionRate								= "Emission Rate";
		public string emit										= "Emit";
		public string emitParticles								= "Emit Particles";
		public string snapshot									= "Snapshot";
		public string source 									= "Source";
		public string sourceDescription							= "Source is the target method for the particles in this Particle Playground System.\n\n" +
																	"State: Target position and color in a stored state\n\nTransform: Target transforms live in the scene\n\n" +
																	"WorldObject: Target each vertex in a mesh live in the scene\n\n" +
																	"SkinnedWorldObject: Target each vertex in a skinned mesh live in the scene\n\n" +
																	"Script: Behaviour controlled by custom scripts\n\n" +
																	"Paint: Target painted positions and colors made with a brush\n\n" +
																	"Projection: Target projected positions and colors made with a texture";
		public string setParticleCount							= "Set Particle Count";
		public string proceduralOptions							= "Procedural Options";
		public string meshVerticesUpdate						= "Mesh Vertices Update";
		public string meshVerticesUpdateDescription				= "Enable this if the object's mesh is procedural and changes vertices over time.";
		public string meshNormalsUpdate							= "Mesh Normals Update";
		public string meshNormalsUpdateDescription				= "Enable this if the object's mesh is procedural and changes normals over time.";
		public string sourceDownResolution						= "Source Down Resolution";
		public string particleSettings							= "Particle Settings";
		public string forces									= "Forces";
		public string collision									= "Collision";
		public string rendering									= "Rendering";
		public string sortingLayer								= "Sorting Layer";
		public string orderInLayer								= "Order In Layer";
		public string manipulator								= "Manipulator";
		public string manipulators								= "Manipulators";
		public string state										= "State";
		public string states									= "States";
		public string eventName									= "Event";
		public string events									= "Events";
		public string eventListener								= "Event Listener";
		public string typeOfEvent								= "The type of event.";
		public string snapshots									= "Snapshots";
		public string advanced									= "Advanced";
		public string particleSystem							= "Particle System";
		public string particleSystems							= "Particle Systems";
		public string globalManipulators						= "Global Manipulators";
		public string target									= "Target";
		public string targets									= "Targets";
		public string transition								= "Transition";
		public string size										= "Size";
		public string strength									= "Strength";
		public string shape										= "Shape";
		public string sphere									= "Sphere";
		public string box										= "Box";
		public string bounds									= "Bounds";
		public string nullName									= "Null";
		public string affects									= "Affects";
		public string type										= "Type";
		public string time										= "Time";
		public string presetText								= "Create a Particle Playground Preset by selecting a Particle Playground System and an icon (optional). The icon must be in png-format and preferably 32x32 pixels. All connected objects will be childed to the Particle Playground System.";
		public string publishPresetText							= "Prepare your preset(s) for packaging by selecting the Particle Playground Preset(s) in the list. All connected meshes, images and/or cached values will be distributed along with your Particle Playground System(s). The icon(s) will automatically be included if existing. Please check all dependencies before you distribute your asset. You agree that Polyfied cannot be held responsible for the quality of your asset when publishing assets connected to the Particle Playground framework.";
		public string publishingGuide							= "Publishing Guide";
		public string childConnected							= "Child Connected Objects";
		public string presetWithSameNameFound					= "Preset with same name found!";
		public string presetWithSameNameFoundText				= "already exists. Do you want to overwrite it?"; // Preset name always first
		public string gameObjectIsNotPlayground					= "GameObject is not a Particle Playground System.";
		public string brushWizardText							= "Create a Particle Playground Brush by selecting a texture and edit its settings. The texture must have Read/Write Enabled and use True Color (non-compressed) in its import settings.";
		public string brushTexture								= "Brush Texture";
		public string brushSameName								= "Brush with same name found!";
		public string brushSameNameText							= "already exists. Do you want to overwrite it?";
		public string playgroundCopyWizardText					= "The Playground Copy Wizard allows you to copy a particle system into another Particle Playground system. You may need to tweak your target Particle Playground system after copying a Shuriken component.";
		public string from										= "From";
		public string fromType									= "From Type";
		public string to										= "To";
		public string copy										= "Copy";
		public string copyErrorMessageIsPlayground				= "You have specified a Shuriken component from a Particle Playground system. This is not allowed as it will result in a corrupt copy.";
		public string copyErrorMessageSameSystem				= "You have specified the same particle system to copy from. This is an unnecessary procedure.";

		public string enabled									= "Enabled";
		public string transform									= "Transform";
		public string transforms								= "Transforms";
		public string vector3									= "Vector3";
		public string position									= "Position";
		public string localPosition								= "Local Position";
		public string rotation									= "Rotation";
		public string localRotation								= "Local Rotation";
		public string scale										= "Scale";
		public string distance									= "Distance";
		public string radius									= "Radius";
		public string points									= "Points";
		public string worldObject								= "World Object";
		public string skinnedWorldObject						= "Skinned World Object";
		public string material									= "Material";

		public string attractor									= "Attractor";
		public string gravitational								= "Gravitational";
		public string repellent									= "Repellent";
		public string property									= "Property";
		public string combined									= "Combined";
		public string vortex									= "Vortex";
		public string turbulence								= "Turbulence";
		public string turbulenceType							= "Turbulence Type";

		public string calculateParticles						= "Calculate Particles";
		public string calculateParticlesDescription				= "Calculate forces on PlaygroundParticles objects. Disabling this overrides independently set values and halts all PlaygroundParticles objects.";
		public string groupAutomatically						= "Group Automatically";
		public string groupAutomaticallyDescription				= "Automatically parent a PlaygroundParticles object to Playground Manager if it has no parent.";
		public string buildZeroAlphaPixels						= "Build Zero Alpha Pixels";
		public string buildZeroAlphaPixelsDescription			= "Turn this on if you want to build particles from 0 alpha pixels into states.";
		public string sceneGizmos								= "Scene Gizmos";
		public string sceneGizmosDescription					= "Show gizmos in Scene View for Playground objects.";
		public string sourcePositions							= "Source Positions";
		public string sourcePositionsDescription				= "Show gizmos in Scene View for particle source positions.";
		public string wireframes								= "Wireframes";
		public string wireframesDescription						= "Draw wireframes around particles in Scene View.";
		public string paintToolbox								= "Paint Toolbox";
		public string paintToolboxDescription					= "Show Paint toolbox in Scene View when Source is set to Paint";
		public string showShuriken								= "Show Shuriken";
		public string showShurikenDescription					= "Show the Shuriken component in Inspector.";
		public string advancedSnapshots							= "Advanced Snapshots";
		public string advancedSnapshotsDescription				= "Show the snapshots of a particle system in Hierarchy and expose more advanced controls through settings.";
		public string pixelFilterMode							= "Pixel Filter Mode";
		public string pixelFilterModeDescription				= "Color filtering mode when creating particles from pixels in an image.";
		public string timeSimulation							= "Time Simulation";
		public string reset										= "Reset";
		public string editorLimits								= "Editor Limits";
		public string transitionTime							= "Transition Time";
		public string transitionType							= "Transition Type";
		public string findClosestPosition						= "Find Closest Position";
		public string transitionTimeMultiplier					= "Transition Time Multiplier";
		public string loadFromStart								= "Load From Start";
		public string setMaterialAfterTransition				= "Set Material After Transition";

		public string particleCount								= "Particle Count";
		public string particleLifetime							= "Particle Lifetime";
		public string particleRotation							= "Particle Rotation";
		public string particleSize								= "Particle Size";
		public string particleScale								= "Particle Scale";
		public string particleVelocity							= "Particle Velocity";
		public string particleColor								= "Particle Color";
		public string positionScale								= "Position Scale";
		public string particles									= "particles";
		public string colorStrength								= "Color Strength";
		public string sourceScatter								= "Source Scatter";
		public string deltaMovement								= "Delta Movement";
		public string deltaMovementStrength						= "Delta Movement Strength";
		public string damping									= "Damping";
		public string velocity									= "Velocity";
		public string velocityStrength							= "Velocity Strength";
		public string velocityScale								= "Velocity Scale";
		public string initialVelocity							= "Initial Velocity";
		public string initialLocalVelocity						= "Initial Local Velocity";
		public string initialRotation							= "Initial Rotation";
		public string stretchSpeed								= "Stretch Speed";
		public string collisionRadius							= "Collision Radius";
		public string mass										= "Mass";
		public string bounciness								= "Bounciness";
		public string depth										= "Depth";
		public string depth2D									= "Depth (2D)";
		public string color										= "Color";
		public string updateRate								= "Update Rate";
		public string renderSliders								= "Render Sliders";
		public string paintPositions							= "Paint Positions";
		public string brushSizeMin								= "Brush Size Min";
		public string brushSizeMax								= "Brush Size Max";
		public string eraserSizeMin								= "Eraser Size Min";
		public string eraserSizeMax								= "Eraser Size Max";
		public string paintSpacing								= "Paint Spacing";
		public string manipulatorSize							= "Manipulator Size";
		public string manipulatorStrength						= "Manipulator Strength";
		public string manipulatorZeroVelocityStrength			= "Zero Velocity Strength";
		public string manipulatorStrengthEffectors				= "Strength Effectors";
		public string trackParticles							= "Track Particles";
		public string sendBirthEvents							= "Send Birth Events";
		public string sendDeathEvents							= "Send Death Events";
		public string sendEnterEvents							= "Send Enter Events";
		public string sendExitEvents							= "Send Exit Events";
		public string sendCollisionEvents						= "Send Collision Events";
		public string smoothingEffect							= "Smoothing Effect";
		public string distanceEffect							= "Distance Effect";
		public string lifetimeFilter							= "Lifetime Filter";
		public string particleFilter							= "Particle Filter";
		public string inverseBounds								= "Inverse Bounds";
		public string propertyType								= "Property Type";
		public string onlyColorInRange							= "Only Color In Range";
		public string onlyPositionInRange						= "Only Position In Range";
		public string keepColorAlphas							= "Keep Color Alphas";
		public string sizeStrength								= "Size Strength";
		public string targetStrength							= "Target Strength";
		public string deathStrength								= "Death Strength";
		public string attractorStrength							= "Attractor Strength";
		public string gravitationalStrength						= "Gravitational Strength";
		public string repellentStrength							= "Repellent Strength";
		public string vortexStrength							= "Vortex Strength";
		public string turbulenceStrength						= "Turbulence Strength";
		public string meshTarget								= "Mesh Target";
		public string skinnedMeshTarget							= "Skinned Mesh Target";
		public string targetSorting								= "Target Sorting";
		public string mesh										= "Mesh";
		public string image										= "Image";
		public string lifetimeColor								= "Lifetime Color";
		public string paintMode									= "Paint Mode";
		public string dot										= "Dot";
		public string brush										= "Brush";
		public string eraser									= "Eraser";
		public string brushPresets								= "Brush Presets";
		public string brushShape								= "Brush Shape";
		public string brushScale								= "Brush Scale";
		public string detail									= "Detail";
		public string brushDistance								= "Brush Distance";
		public string useBrushColor								= "Use Brush Color";
		public string eraserRadius								= "Eraser Radius";
		public string paintCollisionType						= "Paint Collision Type";
		public string paint										= "Paint";
		public string paintMask									= "Paint Mask";
		public string maxPaintPositions							= "Max Paint Positions";
		public string exceedMaxStopsPaint						= "Exceed Max Stops Paint";
		public string exceededMaxPaint							= "You have exceeded max positions. No new paint positions are possible when Exceed Max Stops Paint is enabled.";
		public string morePaintThanPositions					= "You have more paint positions than particles. Increase Particle Count to see all painted positions.";
		public string start										= "Start";
		public string stop										= "Stop";
		public string clear										= "Clear";
		public string clearPaint								= "Clear Paint?";
		public string clearPaintText							= "Are you sure you want to remove all painted source positions?";
		public string playgroundPaint							= "Playground Paint";
		public string overflowMode								= "Overflow Mode";
		public string overflowModeDescription					= "The method to align the Overflow Offset by.";
		public string overflowOffset							= "Overflow Offset";
		public string loop										= "Loop";
		public string disableOnDone								= "Disable On Done";
		public string lifetimeSize								= "Lifetime Size";
		public string rotateTowardsDirection					= "Rotate Towards Direction";
		public string rotationNormal							= "Rotation Normal";
		public string lifetimeMethod							= "Lifetime Method";
		public string lifetime									= "Lifetime";
		public string lifetimeSorting							= "Lifetime Sorting";
		public string lifetimeSortingDescription				= "Determines how the particles are ordered on rebirth.\n" +
																	"Scrambled: Randomly placed.\n" +
																	"Scrambled Linear: Randomly placed but never at the same time.\n" +
																	"Burst: Alfa and Omega.\n" +
																	"Linear: Alfa to Omega.\n" +
																	"NearestNeighbor: Closest to furthest point.\n" +
																	"NearestNeighborReversed: Furthest to closest point.\n" +
																	"Reversed: Omega to Alfa.\n" +
																	"User: Specified by AnimationCurve";
		public string sortOrigin								= "Sort Origin";
		public string customSorting								= "Custom Sorting";
		public string lifetimeOffset							= "Lifetime Offset";
		public string particleMask								= "Particle Mask";
		public string maskTime									= "Mask Time";
		public string maskSorting								= "Mask Sorting";
		public string off										= "Off";
		public string forceAnnihilation							= "Force Annihilation";
		public string onlySourcePositions						= "Only Source Positions";
		public string onlySourcePositionsDescription			= "Particles are bound to their source position during their lifetime.";
		public string assignTransformDeltaMovement				= "Assign a transform to the active state to enable Delta Movement.";
		public string lifetimePositioning						= "Lifetime Positioning";
		public string onlyLifetimePositioningDescription		= "Particle movement is bound to the lifetime positioning.";
		public string useSourceDirection						= "Use Source Normal Direction";
		public string axisConstraintsDescription				= "All forces are disabled as all axis constraints is applied.";
		public string lifetimeVelocity							= "Lifetime Velocity";
		public string initialLocalVelocityControlledByScript	= "Initial Local Velocity is controlled by passed in velocity to Emit() in script mode.";
		public string assignTransformLocalVelocity				= "Assign a transform to the active state to enable Initial Local Velocity.";
		public string initialVelocityShape						= "Initial Velocity Shape";
		public string velocityBending							= "Velocity Bending";
		public string bending									= "Bending";
		public string timeScale									= "Time Scale";
		public string lifetimeStrength							= "Lifetime Strength";
		public string particleLifetimeStrength					= "Particle Lifetime Strength";
		public string gravity									= "Gravity";
		public string maxVelocity								= "Max Velocity";
		public string axisConstraints							= "Axis Constraints";
		public string threeDimensional							= "3D";
		public string twoDimensional							= "2D";
		public string collisionType								= "Collision Type";
		public string collisionMask								= "Collision Mask";
		public string collideWithRigidbodies					= "Collide With Rigidbodies";
		public string lifetimeLoss								= "Lifetime Loss";
		public string randomBounce								= "Random Bounce";
		public string collisionPlanes							= "Collision Planes";
		public string sticky									= "Sticky";
		public string stickyMask								= "Sticky Mask";
		public string collisionPrecision						= "Collision Precision";
		public string forceCollisionCaching						= "Force Collision Caching";
		public string gizmoScale								= "Gizmo Scale";
		public string colorSource								= "Color Source";
		public string sourceUsesLifetimeAlpha					= "Source Uses Lifetime Alpha";
		public string renderMode								= "Render Mode";
		public string cameraScale								= "Camera Scale";
		public string speedScale								= "Speed Scale";
		public string lengthScale								= "Length Scale";
		public string startStretch								= "Start Stretch";
		public string lifetimeStretch							= "Lifetime Stretch";
		public string maxParticleSize							= "Max Particle Size";
		public string settingsAndParticles						= "Settings & Particles";
		public string settingsOnly								= "Settings Only";
		public string particlesOnly								= "Particles Only";
		public string simple									= "Simple";
		public string newSnapshotName							= "New Snapshot";
		public string simulationSpace							= "Simulation Space";
		public string localSpace								= "Local Space";
		public string globalSpace								= "Global Space";
		public string movementCompensation						= "Movement Compensation";
		public string movementCompensationLifetimeStrength		= "Compensation Lifetime Strength";
		public string rebirthOptions							= "Rebirth Options";
		public string randomLifetime							= "Random Lifetime";
		public string randomSize								= "Random Size";
		public string randomRotation							= "Random Rotation";
		public string randomScatter								= "Random Scatter";
		public string randomVelocity							= "Random Velocity";
		public string forceInitialColor							= "Force Initial Color";
		public string lockPosition								= "Lock Position";
		public string positionIsLocal							= "Position Is Local";
		public string lockRotation								= "Lock Rotation";
		public string rotationIsLocal							= "Rotation Is Local";
		public string lockScale									= "Lock Scale";
		public string syncParticlesToMainThread					= "Sync Particles To Main-Thread";
		public string multithreadedManipulators					= "Multithreaded Manipulators";
		public string autoPauseCalculation						= "Auto-Pause Calculation";
		public string particlePool								= "Particle Pool";
		public string broadcastType								= "Broadcast Type";
		public string broadcastTypeDescription					= "Set to broadcast to a Target and/or Event Listeners.";
		public string collisionThreshold						= "Collision Threshold";
		public string inheritancePosition						= "The inheritance for position.";
		public string inheritanceVelocity						= "The inheritance for velocity.";
		public string inheritanceColor							= "The inheritance for color.";
		public string velocityMultiplier						= "Velocity Multiplier";
		public string activeState								= "Active State";
		public string nameText									= "Name";
		public string stateMeshDescription						= "The source mesh to construct particles from vertices. When a mesh is used the texture is used to color each vertex.";
		public string stateTextureDescription					= "The source texture to construct particles from pixels. When a mesh is used this texture is used to color each vertex.";
		public string stateDepthmapDescription					= "The source texture to apply depthmap onto Texture's pixels. Not compatible with meshes.";
		public string stateDepthmapStrengthDescription			= "How much the grayscale of the depthmap will affect Z-value.";
		public string stateTransformDescription					= "The transform to parent this state.";
		public string stateScaleDescription						= "The scale of width-height ratio.";
		public string stateOffsetDescription					= "The offset from Particle System origin.";
		public string brushNameDescription						= "The name of this brush preset";
		public string brushShapeDescription						= "The texture to construct this Brush from";
		public string brushScaleDescription						= "The scale of this Brush (measured in Units)";
		public string brushDetail								= "Brush Detail";
		public string brushDetailDescription					= "The detail level of this brush";
		public string brushDistanceDescription					= "The distance the brush reaches";
		public string paintSpacingDescription					= "The required space between the last and current paint position";
		public string exceedMaxStopsPaintDescription			= "Should painting stop when paintPositions is equal to maxPositions (if false paint positions will be removed from list when painting new ones)";
		public string texture									= "Texture";
		public string depthmap									= "Depthmap";
		public string depthmapStrength							= "Depthmap Strength";
		public string offset									= "Offset";
		public string createState								= "Create State";
		public string projectionTexture							= "Projection Texture";
		public string liveUpdate								= "Live Update";
		public string originOffset								= "Origin Offset";
		public string projectionDistance						= "Projection Distance";
		public string projectionScale							= "Projection Scale";
		public string surfaceOffset								= "Surface Offset";
		public string projectionMask							= "Projection Mask";
		public string projectionCollisionType					= "Projection Collision Type";
		public string collisionDisabledDescription				= "Collisions are disabled due to your Force settings.";
		public string onlySizeInRange							= "Only Size In Range";
		public string sendToManager								= "Send To Manager";
		public string clearOnStop								= "Clear On Stop";
		public string multithreading							= "Multithreading";
		public string particleThreadMethod						= "Particle Thread Method";
		public string skinnedMeshThreadMethod					= "Skinned Mesh Thread Method";
		public string turbulenceThreadMethod					= "Turbulence Thread Method";
		public string threadMethodDescription					= "Determines which multithreading method should be used.";
		public string maxThreads								= "Max Threads";
		public string maxThreadsDescription						= "The maximum amount of threads that can be created. The amount of created threads will never exceed available CPUs.";
		public string threadInfo01								= "No calculation threads will be created. This will in most cases have a negative impact on performance as Particle Playground will calculate along all other logic on the main-thread. Use this for debug purposes or if you know there's no multi- or hyperthreading possibilities on your target platform.";
		public string threadInfo02								= "One calculation thread per particle system will be created. Use this when having heavy particle systems in your scene. Note that this method will never bundle calculation calls.";
		public string threadInfo03								= "One calculation thread for all particle systems will be created. Use this if you have other multithreaded logic which has higher performance priority than Particle Playground or your project demands strict use of garbage collection.";
		public string threadInfo04								= "Calculation threads will distribute evenly for all particle systems in your scene. This will bundle calculation calls to match the platform's SystemInfo.processorCount. This is the recommended and overall fastest method to calculate particle systems.";
		public string thread									= "Thread";
		public string assignATexture							= "You need to assign a texture.";
		public string useAnotherSourceTransform					= "Use another Source Transform than your particle system to enable this effect.";
		public string activeThreads								= "active threads";
		public string threadPoolMethod							= "Thread Pool Method";
		public string threadPoolMethodDescription				= "The method to use for pooling threads. The Thread Pool is utilizing the managed .NET thread pool, the Playground Pool is a self-managed pool which can improve the amount of GC allocations.";
		public string processors								= "processors";
		public string prefabConnection							= "Prefab Connection";
		public string noAssetsFoundMessage						= "The Particle Playground assets couldn't be found in the specified path. Please edit the paths to match your current project.";
		public string skinnedMeshOptimizeGameObjectsMessage		= "Consider to disable Optimize Game Objects for the Import Settings of your skinned mesh. Having Optimize Game Objects enabled will be a lot slower for a Particle Playground system to calculate as it must extract the vertices each frame.";

		public string castShadows								= "Cast Shadows";
		public string receiveShadows							= "Receive Shadows";

		public string sorting									= "Sorting";
		public string sortMode									= "Sort Mode";
		public string sortingFudge								= "Sorting Fudge";

		public string textureSheetAnimation						= "Texture Sheet Animation";
		public string tiles										= "Tiles";
		public string animation									= "Animation";
		public string randomRow									= "Random Row";
		public string row										= "Row";
		public string animationMethod							= "Animation Method";
		public string frameOverTime								= "Frame Over Time";
		public string chromaKey									= "Chroma Key";
		public string spread									= "Spread";

		public string spline									= "Spline";
		public string splines									= "Splines";
		public string newSplineMessage							= "Assign an existing Playground Spline component from your scene or press Create to start working with a new Playground Spline (will be parented to this particle system). To edit a Playground Spline you select it in Hierarchy.";
		public string allSplinesMustBeAssignedMessage			= "All splines must be assigned and enabled.";
		public string treatAsOneSpline							= "Treat As One Spline";
		public string playgroundSpline							= "Playground Spline";
		public string selection									= "Selection";
		public string node										= "Node";
		public string nodes										= "Nodes";
		public string bezier									= "Bezier";
		public string selectedNode								= "Selected Node";
		public string selectedBezierHandle						= "Selected Bezier Handle";
		public string bezierMode								= "Bezier Mode";
		public string addNode									= "Add Node";
		public string removeSelectedNode						= "Remove Selected Node";
		public string usedBy									= "Used By";
		public string noUser									= "No User";
		public string easing									= "Easing";
		public string easingVelocity							= "Easing Velocity";
		public string timeOffset								= "Time Offset";
		public string positionOffset							= "Position Offset";
		public string reverse									= "Reverse";
		public string targetMethod								= "Target Method";
		public string overflowOffsetSplineMessage				= "Overflow Offset can't be applied when using Spline as Source.";
		public string noSplineUserMessage						= "No Particle Playground system is using this spline.";
		public string removeUserTitle							= "Remove user?";
		public string removeUserMessage							= "Do you want to remove this spline from the Particle Playground system?";
		public string convertAllToTransforms					= "Convert All To Transforms";
		public string convertAllToVector3						= "Convert All To Vector3";
		public string reverseAllNodes							= "Reverse All Nodes";
		public string velocityOnNewNode							= "Velocity On New Node";
		public string moveTransformsAsBeziers					= "Move Transforms As Beziers";
		public string exportWithNodeStructure					= "Export With Node Structure";
		public string repeatTimeModulus							= "Repeat Time (Modulus)";
		public string drawSplinePreview							= "Spline Preview";
		public string drawSplinePreviewDescription				= "Draws the shape of a spline assigned to a particle system when selected in Hierarchy.";
		public string newEmpty									= "New Empty";
		public string noSplinesCreated							= "No splines are created.";

		public string colorMethod								= "Color Method";
		public string arrayColor								= "Array Color";
		public string arrayAlpha								= "Array Alpha";
		public string applyArrayAlpha							= "Apply Array Alpha";

		public string collisionExclusion						= "Collision Exclusion";
		public string collisionExclusionMessage					= "No objects are excluded from collision.";
		public string bypassMaskedParticles						= "Bypass Masked Particles";

		public string treatAsOneTransform						= "Treat As One Transform";
		public string allTransformsMustBeAssignedMessage		= "All transforms must be assigned.";
		public string assignTransformMessage					= "Please assign a transform.";
		public string method									= "Method";
		public string range										= "Range";
		public string sectorA									= "Sector A";
		public string sectorB									= "Sector B";

		public string lifetimeEmission							= "Lifetime Emission";
		public string prewarm									= "Prewarm";
		public string prewarmLifetimeCycles						= "Prewarm Lifetime Cycles";
		public string prewarmCyclesResolution					= "Prewarm Cycles (Resolution)";
		public string inverseRigidbodyCollision					= "Inverse Rigidbody Collision";
		public string locks										= "Locks";
		public string onEnable									= "On Enable";
		public string misc										= "Misc";
		public string deltaPositionAdjustment					= "Delta Position Adjustment";
		public string arraySize									= "Array Size";
		public string sourcePoint								= "Source Point";
		public string emission									= "Emission";
		public string on										= "On";
		public string constantForce								= "Constant Force";
		public string initialGlobalVelocity						= "Initial Global Velocity";
		public string shadows									= "Shadows";
		public string collisionSettings							= "Collision Settings";
		public string sync										= "Sync";
		public string transitionBackToSource					= "Transition Back To Source";
		public string transitionAmount							= "Transition Amount";
		public string calculateManipulator						= "Calculate Manipulator";
		public string renderer									= "Renderer";
		public string hierarchyIcon								= "Hierarchy Icon";
		public string minimumShurikenLifetime					= "Minimum Shuriken Lifetime";
		public string calculationTrigger						= "Calculation Trigger";
		public string calculationTriggerSize					= "Calculation Trigger Size";
		public string calculationTriggerOffset					= "Calculation Trigger Offset";
		public string drawGizmo									= "Draw Gizmo";
		public string drawBezierGizmo							= "Draw Bezier Gizmo";
		public string bezierWidth								= "Bezier Width";
		public string outOfView									= "Out Of View";
		public string autoPause									= "Auto-Pause";
		public string forceUpdateOnMainThread					= "Force Update On Main-Thread";
		public string forceUpdateOnMainThreadDescription		= "Enable this to ensure that skinned meshes are in absolute sync with their source positions. This may give a noticable hit on performance.";
		public string trackingMethod							= "Tracking Method";
		public string animationTimeMethod						= "Animation Time Method";

		public string upSymbol									= "U";
		public string downSymbol								= "D";
		public string playSymbol								= "Play";
		public string pauseSymbol								= "Pause";
		public string stopSymbol								= "Stop";
		public string recordSymbol								= "Record";
		public string trim										= "Trim";
		public string trimInner									= "Trim Inner";
		public string trimOuter									= "Trim Outer";
		public string trimMsg									= "Are you sure you want to remove the frames ";
		public string fadeIn									= "Fade In";
		public string sizeIn									= "Size In";
		public string skipInterpolationOnEndFrames				= "Skip Interpolation On End Frames";

		public string playgroundRecorder						= "Playground Recorder";
		public string player									= "Player";
		public string playbackSpeed								= "Playback Speed";
		public string keyframeInterval							= "Keyframe Interval";
		public string playHeadPosition							= "Playhead Position";
		public string clearRecording							= "Clear Recording";
		public string clearRecordingMsg							= "Are you sure that you want to remove all the recorded frames?";
		public string missingParticleSystemWarning				= "There is no Particle Playground system assigned.";
		public string newPlaygroundRecording					= "New Playground Recording";
		public string newPlaygroundRecordingMsg					= "Please choose where to store your Playground Recorder Data";
		public string recorderData								= "Recorder Data";
		public string missingRecorderDataWarning				= "There is no Playground Recorder Data file assigned. You need to assign one to store your recordings.";
		public string createNew									= "Create New";

		public string playgroundComponents						= "Playground Components";
		public string removeComponent							= "Remove Component?";
		public string removeComponentMsg						= "Are you sure you want to remove the component?";

		public string particleProperty							= "Particle Property";
		public string clamp										= "Clamp";
		public string floor										= "Floor";
		public string ceil										= "Ceil";
		public string value										= "Value";
		public string rate										= "Rate";
		public string infinite									= "Infinite";

		public string sourceParticleSystem						= "Source Particle System";
		public string sourceBirthMethod							= "Source Birth Method";
		public string scaleMethod								= "Scale Method";
		public string globalTimeScale							= "Global Time Scale";
		public string globalTimeScaleDescription				= "If Global Time Scale is enabled Time.timeScale will affect the particle systems simulation time.";

		public string extensionsDownloading						= "Retrieving list of extensions...";
		public string extensionsDownloadError					= "Could not retrieve the list of extensions.";
		public string searchNoExtensionsFound					= "No extensions found containing";
		public string listHasBeenFilteredWith					= "List has been filtered with";
		public string findPlaygroundSettings					= "Find Playground Settings";
		public string findPlaygroundSettingsMsg					= "The Playground Settings.asset could not be found, you won't be able to store your settings. It is recommended that you search for the Playground Settings.asset file.";

		public string playgroundTrails							= "Playground Trails";
		public string pointCreation								= "Point Creation";
		public string width										= "Width";
		public string timeWidth									= "Time Width";
		public string widthScale								= "Width Scale";
		public string billboardTransform						= "Billboard Transform";
		public string customRenderScale							= "Custom Render Scale";
		public string minimumVertexDistance						= "Minimum Vertex Distance";
		public string maximumVertexDistance						= "Maximum Vertex Distance";
		public string maximumPathDeviation						= "Maximum Path Deviation";
		public string maximumPoints								= "Maximum Points";
		public string createFirstPointOnParticleBirth			= "Create First Point On Particle Birth";
		public string createLastPointOnParticleDeath			= "Create Last Point On Particle Death";
		public string createPointsOnCollision					= "Create Points On Collision";
		public string colorMode									= "Color Mode";
		public string uvMode									= "Uv Mode";
		public string pointArrayAlpha							= "Point Array Alpha";
		public string playgroundFollow							= "Playground Follow";
		public string setLocalSpaceOnPlayback					= "Set Local Space On Playback";
		public string followers									= "Followers";
		public string unassigned								= "Unassigned";
		public string referenceObject							= "Reference Object";
		public string cacheSize									= "Cache Size";
		public string sendEvents								= "Send Events";
		public string multithreadedStartup						= "Multithreaded Startup";


		public static PlaygroundLanguageC New () {
			return New ("New Language");
		}
		public static PlaygroundLanguageC New (string newName) {
			PlaygroundLanguageC newLanguage = ScriptableObject.CreateInstance<PlaygroundLanguageC>();
			newLanguage.languageName = "New Language";
			newLanguage.languageNameSeenByEnglish = newName;
			int i = 0;
			if (AssetDatabase.LoadAssetAtPath ("Assets/"+PlaygroundSettingsC.reference.playgroundPath+PlaygroundSettingsC.reference.languagePath+newName+" "+i.ToString()+".asset", typeof(object))) {
				while (AssetDatabase.LoadAssetAtPath ("Assets/"+PlaygroundSettingsC.reference.playgroundPath+PlaygroundSettingsC.reference.languagePath+newName+" "+i.ToString()+".asset", typeof(object)))
					i++;
			}
			AssetDatabase.CreateAsset(newLanguage, "Assets/"+PlaygroundSettingsC.reference.playgroundPath+PlaygroundSettingsC.reference.languagePath+newName+" "+i.ToString()+".asset");
			AssetDatabase.SaveAssets();
			return (PlaygroundLanguageC)AssetDatabase.LoadAssetAtPath ("Assets/"+PlaygroundSettingsC.reference.playgroundPath+PlaygroundSettingsC.reference.languagePath+newName+" "+i.ToString()+".asset", typeof(PlaygroundLanguageC));
		}




	}


}
