using GameDataEditor;

namespace Fie.Manager
{
	public class FieAcvitityQueue
	{
		public float showingTime;

		public GDEConstantTextListData titleTextData;

		public GDEConstantTextListData noteTextData;

		public FieAcvitityQueue(float showingTime, GDEConstantTextListData titleTextData, GDEConstantTextListData noteTextData)
		{
			this.showingTime = showingTime;
			this.titleTextData = titleTextData;
			this.noteTextData = noteTextData;
		}
	}
}
