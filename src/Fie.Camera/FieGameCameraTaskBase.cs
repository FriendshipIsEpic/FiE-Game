namespace Fie.Camera
{
	public abstract class FieGameCameraTaskBase
	{
		public virtual void Initialize(FieGameCamera gameCamera)
		{
		}

		public virtual void Terminate(FieGameCamera gameCamera)
		{
		}

		public virtual void TargetChanged(FieGameCamera gameCamera, FieGameCharacter fromCharacter, FieGameCharacter toCharacter)
		{
		}

		public virtual void TargetMissed(FieGameCamera gameCamera)
		{
		}

		public abstract void CameraUpdate(FieGameCamera gameCamera);
	}
}
