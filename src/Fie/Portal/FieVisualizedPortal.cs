using UnityEngine;

namespace Fie.Portal
{
	public abstract class FieVisualizedPortal : FiePortalBase
	{
		[SerializeField]
		private AnimationCurve _fxCurve;

		[SerializeField]
		private PKFxFX _gaugeFx;

		[SerializeField]
		private PKFxFX _portalParticleFx;

		[SerializeField]
		private PKFxFX _portalHoleFx;

		[SerializeField]
		private AudioSource _soundFx;

		private PKFxManager.Attribute _progessAttr;

		private PKFxManager.Attribute _teleportStartupAttr;

		private PKFxManager.Attribute _portalHoleAttr;

		private float _latestProgress = -1f;

		public void Awake()
		{
			_progessAttr = new PKFxManager.Attribute("Value", 0f);
			_teleportStartupAttr = new PKFxManager.Attribute("TeleportStartup", 0f);
			_portalHoleAttr = new PKFxManager.Attribute("Scale", 0f);
		}

		public void OnEnable()
		{
		}

		public new void Update()
		{
			base.Update();
			if (_latestProgress != base.progress)
			{
				float num = _fxCurve.Evaluate(base.progress / base.triggeringSec);
				_progessAttr.ValueFloat = base.progress / base.triggeringSec;
				_teleportStartupAttr.ValueFloat = num;
				_portalHoleAttr.ValueFloat = num;
				if (_gaugeFx != null)
				{
					_gaugeFx.SetAttribute(_progessAttr);
				}
				if (_portalParticleFx != null)
				{
					_portalParticleFx.SetAttribute(_teleportStartupAttr);
				}
				if (_portalHoleFx != null)
				{
					_portalHoleFx.SetAttribute(_portalHoleAttr);
				}
				if (_soundFx != null)
				{
					_soundFx.volume = num;
				}
				_latestProgress = base.progress;
			}
		}
	}
}
