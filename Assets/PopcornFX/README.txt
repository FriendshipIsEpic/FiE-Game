
                '      .'
           |   '|    .'
          . \_/  \_.',
    ..,_.' _____    (
     `.   |  __ \   '..o    o    o    o    o    o    o    o    o    o    o    o   o
       :  | |__) | ,' ___    _ __     ___    ___    _ __   _ __     _____  __   __
   ._-'   |  ___/ /  / _ \  | '_ \   / __/  / _ \  | '__| | '_ \   |  ___| \ \ / /
     `.   | |     ( | (_) | | |_) | | (_   | (_) | | |    | | | |  | |_     \ V /
     ,'   |_|    _`. \___/  | .__/   \___\  \___/  |_|    |_| |_|  |  _|     ) (
    ,.'"\  _.._ (  `'       |_|                                    | |      / ' \
   :'   | /    `' o    o    o    o    o    o    o    o    o    o   |_|     /_/ \_\
 ,"     ',
'       '
                                                      Realtime Particle FX Solution


    This program is the property of Persistant Studios SARL.

    PopcornFX :
        http://www.popcornfx.com
        http://www.facebook.com/3D.PopcornFX
        http://www.twitter.com/popcornfx

=================================================================================

PopcornFX is a 3D realtime FX Solution for Games & Interactive applications.
PopcornFX is for studios, 3D FX artist, Technical Artist who are looking for a
powerful and complete tool exclusively dedicated to the creation of realtime
effects.

This plugin is to be used with the totally free PopcornFX editor which will
change your way of creating particle effects: better FX faster !!

The plugin allows you to run the FX created with the PopcornFX dedicated editor
in your Unity games.
We also have a free discovery FX pack available to get you started.
Modify it with the PopcornFX editor or directly use its effects in your games.

This plugin requires Unity Pro for versions of Unity anterior to Unity 5.

Online Documentation : http://wiki.popcornfx.com/index.php/Unity

For bug reports/support, checkout answers.popcornfx.com or our Unity forum thread
forum.unity3d.com/threads/pkfx.297938/

=================================================================================

Popcorn Unity Plugin v2.9

    Supported platforms :
        - Win (D3D9, D3D11, OpenGL2/Core)
        - Mac OSX (OpenGL2/Core)
        - Linux (OpenGL Core)
        - Android (with NEON support)
        - iOS (OpenGLES)

=================================================================================

CHANGELOG

	v2.9p3
		- Build scene mesh at runtime on all platforms.
		- Can register multiple camera.
		- Fixed 1 frame delay on registered camera.
		- Do not all the HBOs at application start.
		- StopEffect() became TerminateEffect().
		- StopEffect() stop the emission but continue to update the transform and attributes until all particles died.
		- Load Attribute Samplers when the FX start.
		- Fixed a crash if an Attribute Sampler Skinned Mesh is not in the first position of the samplers list.
		- Add Support for Unity 5.6
		- Sync. PopcornFX runtime v1.11.3

	v2.9p2
		- Attribute samplers : handle ETC2 texture format.
		- Fixed Sample scene custom shaders.
		- Sync. PopcornFX runtime v1.11.0

	v2.9p1
		- Attribute samplers : Fixed an error when unsupported samplers are above the others.
		- Support the reversed depth buffer in Unity 5.5
		- Sync. PopcornFX runtime v1.10.8

	v2.9
		- Attribute samplers : added getter/setters for easier access via scripts
        - Attribute sampler image : Logs an error when texture read is disabled.
        - Attribute sampler image : Added PVRTC support.
        - Attribute sampler shape : allow to drag an drop a MeshFilter on a
        sampler mesh.
		- Attribute samplers : Fixed broken list with unsupported samplers.
        - Attributes : fixed leak with attributes names when getting descriptors.
        - Attributes : now display their descriptions as tooltips.
        - Scene mesh builder : now creates path if it doesn't already exist.
        - Fixed a #define preventing the plugin to load on OSX when targetting
        iOS.
        - Added DeepReset : like Reset but also unloads FXs and clean the render
        mediums.
        - Now attempts to load the pack from relative path to prevent issues with
        non-ascii paths.
        - Added a "reset attributes to default values" button on PKFxFX.
        - Fixed checks for attribute samplers OnDestroy.
        - Fixed attribute sampler mesh : don't pin non-blittable types (bool).
        - Fixed sampler shape mesh : not feed with the mesh after StopEffect
        StartEffect.
        - Implemented native OSX assert catcher.
        - Sync. PopcornFX runtime v1.10.4

	v2.8
		- Attribute samplers : shapes, textures, curves...
		- Fixed orthographic cameras support (see PopcornFX Preferences...)
		- Fixed glitchy billboarding on Android.
		- Added multi-object editing for FX components.
		- Fixed conf file loading when loading from a read-only location.
		- Fixed moving pkfx files breaking components association.
		- Fixed scene reset when destroying the first camera.
		- Moved from C# assemblies to sources (see updater).
		- Added audio sampling named audiogroup.
		- Stability improvements.
		- Renamed native libraries from HH prefix to PK prefix, reflecting a
		name change that happened even before the plugin was released.
		- Added OpenGL Core support.

    v2.7
        - New rendering pipeline, fully integrated with command buffers (Unity
        5.2 and up). Includes retrieving of the scene's depth without resorting
        to an offline camera (Unity 5.0 and up).
        - Added support for distortion/soft particles in VR.
        - Added support for Android multi-threaded rendering (Unity 5.3+).
        - Depth texture now public.
        - Custom shaders (DX11, DX9, OpenGL/ES).
        - Sync with PopcornFX 1.9.0.
        - Fixed PKFxManager's Debug class that was hiding Unity's in some
        versions of the plugin.
		- Fixed KillEffect's behaviour on trails.
		- Added TransformAllParticles() to apply a global transform on all
		particles (useful for floating origin setups).

    v2.6
        - Added PKFxFX.GetAttr(string) and integer overloads for the
        PKFxManager.Attribute class.
        - Added PKFxFX.IsPlayable().
        - Added possibility to overload the OnAudioSpectrum and OnAudioWaveform
        callbacks.
        - Added possibility to use Application.PersistentDataPath instead of the
        default Application.StreamingAssetsPath (useful for adding effects post-
        install).
        - Added support for Windows XP.
        - Moved rendering to the new command buffer interface (Unity >= 5.2).
        This fixes image effects bugs and lens flare layers bugs.
        - Changed the PopcornFX Settings menu. It's now a window exposing the
        individual effect killing, logging, PackFX location and rendering event
        settings.
        - Destroying an effect now calls StopFX().
        - Added a control button on PKFxFX components to force-reload attributes.
        - Fixed a crash when destroying the PKFxRenderingPlugin component of a
        scene and calling PKFxFX.KillEffect in the same frame.
        - Fixed native VR detection.
        - Fixed PNG loading on iOS (bug or crash depending on the version).

    v2.5
        - Fixed a crash when deleting hot-reloaded effects.
        - Fixed a crash when calling StopEffect in OnDisable.
        - Fixed a crash when getting in and out of a scene with effects using
        spacial layers.
        - Fixed race-condition issues.
        - Fixed a bug where localspace effects and attributes were 1 frame behind
        - Added an Alive() method to the PKFxFX component that returns true as
        long as an effect is spawning particles.
        - Added the possibility to enable the killing of individual effects.
        - Added a main conf file for settings (holds the log in file and effects
        killing settings so far).
        - Added support of PopcornFX's sound layers (see online documentation).
        - Added support for audio waveform sampling.
        - Added support for audio sampling (waveform and spectrum) for iOS.
        - Added PKFxManager.UnloadEffect(string path);
        - Fixed Unity 5.1's native VR double update bug (soft particles and
        distortions still not available) (2.5p1).
        - Fixed empty log file bug (2.5p1).
        - Added global time multiplier for slow-mo/fast forwarding particles
        (2.5p1).
        - Sync with PopcornFX Editor 1.8.2 (fixes issues with mesh sampling)
        (2.5p1).
        - Fixed OpenGL2 mesh renderer texturing issues (2.5p2).

    v2.4
        - Synchronized with PopcornFX Editor v1.8.X (make sure to upgrade your
          packs).
        - New fat library for ios64 support.
        - Fixed a bug where PopcornFX components would interfere with other
          plugins' components.
        - Effects hot-reload in Windows and MacOSX editor mode.
        - Int/Int2/3/4 attributes support.
        - Float attributes now support min/max values.

    v2.3
        - Fixed a bug where fx wouldn't load in specific pack architectures.
        - Added PKFxManager.GetStats() to retrieve stats from the PopcornFX run-
          time.
        - Added a link to the online documentation in the Help menu ("PopcornFX
          Wiki").
        - Added a version identifier in the PKFxRenderingPlugin component's
          inspector view.
        - Added "PopcornFX Settings" in the Edit menu with options to enable or
          disable the runtime log to a file (requires a restart to take effect).
        - Added "Create PopcornFX" in the GameObject menu to create empty
          effects or PopcornFX-enabled cameras.
        - Fixed UI refresh after drag'n'drop of a pkfx file in the FX field of
          a PKFxFX component.
        - Added a warning when in editor if the color space is not set to linear

    v2.2
        - Fixed audio sampling on MacosX
        - Fixed DX11 LOD bias
        - OSX binaries now Universal (x86 and x86_64)
        - Soft animation blending now supported in GL/DX9/DX11
        - Fixed DX11 depth texture update bug (soft particles and distortion)
        - Fixed component.camera api deprecation for Unity 5
        - Drag'n'drop .pkfx files on FX components instead of the unconvenient
          list

    v2.1
        - Proper color space detection (sRGB/gamma correction)
        - Distortions blur pass (Blue channel)
        - Fixed distortions bug
        - Massive renames to comply with naming convention
          /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\
          /!\/!\/!\ THIS WILL BREAK MANY PREEXISTING FX COMPONENT!    /!\/!\/!\
          /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\
        - DX11 bugfixes, proper distortions and soft particles
        - Fixed mobile crash on some sampling functions
        - Masked event ids to prevent interfering with other native plugins
        - Fixed bug where additive meshes were never culled
	    - Fixed lost devices in DX9
	    - Fixed depth/distortion FOV bug
	    - Fixed distortion viewport bug
	    - Windows x32/x64 support.

    v2.0
        - Distortions!
        - Mobile depth-related rendering features (soft particles, distortion).

        - PackFX hierarchy : effects are now accessible at any pack subdirectory
        - Packs must now be baked in the StreamingAssets directory.
          /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\
          /!\/!\/!\ THIS WILL BREAK ANY PREEXISTING FX COMPONENT!     /!\/!\/!\
          /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\   /!\

    v1.08
        - Refactoring of the PKFxManager static class to account for iOS' static
        libraries limitation.

=================================================================================

Known bugs.

    - VelocityCapsuleAligned billboarders produce visual glitches on Android.
