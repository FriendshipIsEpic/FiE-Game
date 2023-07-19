using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightLaserChild")]
	public class FieEmitObjectTwilightLaserChild : FieEmittableObjectBase
	{
		[SerializeField]
		private float LaserChildDuration = 0.3f;

		[SerializeField]
		private float LaserChildMaxScale = 1.2f;

		private Tweener<TweenTypesOutSine> _startScaleTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesOutSine> _endScaleTweener = new Tweener<TweenTypesOutSine>();

		private float _outputRate = 1f;

		private float _lifeTimeCount;

		private float _laserWidthRate = 1f;

		public override void awakeEmitObject()
		{
			_startScaleTweener.InitTweener(LaserChildDuration * 0.5f, 0f, LaserChildMaxScale);
			_endScaleTweener.InitTweener(LaserChildDuration * 0.5f, LaserChildMaxScale, 0f);
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
			if (base.ownerCharacter != null)
			{
				GDESkillTreeData skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_SPARKLY_CANNON_LV4__DEFFENSIVE_CANNON);
				if (skill != null)
				{
					defaultDamage += baseDamage * skill.Value2;
				}
				GDESkillTreeData skill2 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_SPARKLY_CANNON_LV2_2);
				if (skill2 != null)
				{
					_laserWidthRate += skill2.Value1;
				}
			}
		}

		public void SetOutputRate(float outputRate)
		{
			_outputRate = outputRate;
		}

		public void Update()
		{
			_lifeTimeCount += Time.deltaTime;
			if (_lifeTimeCount > LaserChildDuration)
			{
				destoryEmitObject();
			}
			float num = 0f;
			num = (_startScaleTweener.IsEnd() ? _endScaleTweener.UpdateParameterFloat(Time.deltaTime) : _startScaleTweener.UpdateParameterFloat(Time.deltaTime));
			num *= _laserWidthRate;
			base.transform.localScale = new Vector3(num * _outputRate, num * _outputRate, 0.2f);
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
		}
	}
}
