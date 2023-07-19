using Fie.Manager;
using Fie.Object;
using Spine.Unity;
using UnityEngine;

namespace Fie.UI
{
	[RequireComponent(typeof(SkeletonAnimation))]
	public class FieGameUIIndicator : FieGameUIBase
	{
		private enum IndicatorState
		{
			NONE = -1,
			HIDDEN,
			ENEMY,
			INJURY,
			INJURY_LOCATE
		}

		private const string INDICATOR_ANIMATION_NAME_HIDDEN = "hide";

		private const string INDICATOR_ANIMATION_NAME_ENEMY = "enemy";

		private const string INDICATOR_ANIMATION_NAME_INJURY = "injury";

		private const string INDICATOR_ANIMATION_NAME_INJURY_LOCATE = "injury_located";

		public FieGUIManager.FieUILayer currentLayer = FieGUIManager.FieUILayer.DEFAULT;

		private SkeletonAnimation _skeleton;

		private MeshRenderer _rendere;

		private IndicatorState _state = IndicatorState.NONE;

		public override void Initialize()
		{
			_state = IndicatorState.NONE;
			_skeleton = GetComponent<SkeletonAnimation>();
			_rendere = GetComponent<MeshRenderer>();
			if (_skeleton == null)
			{
				Debug.LogError("Skeleton Animation componet dose not found.");
			}
			else
			{
				UpdateIndicatorAnimation(IndicatorState.HIDDEN);
			}
		}

		public override void Terminate()
		{
			UpdateIndicatorAnimation(IndicatorState.HIDDEN);
		}

		private void UpdateIndicatorAnimation(IndicatorState state)
		{
			if (state != _state)
			{
				switch (state)
				{
				case IndicatorState.HIDDEN:
					_rendere.enabled = false;
					break;
				case IndicatorState.ENEMY:
					_skeleton.AnimationState.SetAnimation(0, "enemy", loop: true);
					_rendere.enabled = true;
					break;
				case IndicatorState.INJURY:
					_skeleton.AnimationState.SetAnimation(0, "injury", loop: true);
					_rendere.enabled = true;
					break;
				case IndicatorState.INJURY_LOCATE:
					_skeleton.AnimationState.SetAnimation(0, "injury_located", loop: true);
					_rendere.enabled = true;
					break;
				}
				_state = state;
			}
		}

		private void Update()
		{
			CheckState(base.ownerCharacter);
		}

		private void CheckState(FieGameCharacter ownerCharacter)
		{
			if (ownerCharacter == null)
			{
				Terminate();
				base.uiActive = false;
			}
			else
			{
				Vector3 screenPosition = Vector3.zero;
				switch (ownerCharacter.forces)
				{
				case FieEmittableObjectBase.EmitObjectTag.PLAYER:
					if (ownerCharacter.healthStats.hitPoint <= 0f)
					{
						if (base.uiCamera.isOnScreen(ownerCharacter.centerTransform.position, ref screenPosition))
						{
							base.transform.position = base.uiCamera.getPositionInUICameraWorld(ownerCharacter.guiPointTransform.position);
							base.transform.rotation = Quaternion.identity;
							base.transform.localScale = Vector3.one;
							UpdateIndicatorAnimation(IndicatorState.INJURY_LOCATE);
						}
						else
						{
							PlaceToScreenCircle(screenPosition);
							UpdateIndicatorAnimation(IndicatorState.INJURY);
						}
						return;
					}
					break;
				case FieEmittableObjectBase.EmitObjectTag.ENEMY:
					if (!base.uiCamera.isOnScreen(ownerCharacter.centerTransform.position, ref screenPosition))
					{
						PlaceToScreenCircle(screenPosition);
						UpdateIndicatorAnimation(IndicatorState.ENEMY);
						return;
					}
					break;
				}
				UpdateIndicatorAnimation(IndicatorState.HIDDEN);
			}
		}

		public float GetAim(Vector2 p1, Vector2 p2)
		{
			float x = p2.x - p1.x;
			float y = p2.y - p1.y;
			float num = Mathf.Atan2(y, x);
			return num * 57.29578f;
		}

		private void PlaceToScreenCircle(Vector3 outsidePosition)
		{
			Vector2 vector = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
			Vector2 normalized = new Vector2((float)Screen.width, (float)Screen.height).normalized;
			Vector2 v = new Vector2(outsidePosition.x, outsidePosition.y) - vector;
			Vector2 normalized2 = v.normalized;
			Vector2 vector2 = vector + new Vector2(normalized2.x * ((float)Screen.width * 0.48f) * normalized.x, normalized2.y * ((float)Screen.height * 0.48f) * normalized.y);
			base.transform.rotation = Quaternion.AngleAxis(GetAim(vector, vector2) + 90f, Vector3.forward);
			base.transform.localScale = Vector3.one * Mathf.Max(0.75f, Mathf.Min(1.5f, (float)(Screen.width + Screen.height) * 0.5f / Mathf.Max(Vector3.Distance(v, normalized2), 1f)));
			base.transform.position = base.uiCamera.camera.ScreenToWorldPoint(vector2);
		}
	}
}
