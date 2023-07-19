using System;

namespace Fie.Scene
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class FieSceneLink : Attribute
	{
		public readonly FieConstValues.DefinedScenes linkedScene;

		public readonly FieSceneType definedSceneType;

		public FieSceneLink(FieConstValues.DefinedScenes linkScene, FieSceneType sceneType)
		{
			linkedScene = linkScene;
			definedSceneType = sceneType;
		}
	}
}
