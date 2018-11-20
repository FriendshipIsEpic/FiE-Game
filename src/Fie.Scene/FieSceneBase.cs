using System;

namespace Fie.Scene
{
	[FieSceneLink(FieConstValues.DefinedScenes.UNDEFINED, FieSceneType.UNDEFINED)]
	public abstract class FieSceneBase
	{
		public FieSceneLink GetSceneLinkInfo()
		{
			FieSceneLink fieSceneLink = (FieSceneLink)Attribute.GetCustomAttribute(GetType(), typeof(FieSceneLink));
			if (fieSceneLink != null)
			{
				return fieSceneLink;
			}
			return null;
		}

		public abstract void StartUp();
	}
}
