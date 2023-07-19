using GameDataEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieSkillTreeEndPoint : Selectable, IPointerEnterHandler, IPointerDownHandler, IEventSystemHandler
	{
		public enum EndPointState
		{
			INITIALIZED = -1,
			LOCKED,
			UNLOCKED
		}

		public delegate void FieSkillTreeEndPointClickCallback(FieSkillTreeEndPoint clickedEndPoint);

		[SerializeField]
		private PKFxFX _enabledEffect;

		[SerializeField]
		private PKFxFX _disabledEffect;

		[SerializeField]
		private PKFxFX _lineObject;

		[SerializeField]
		private UnityEngine.Camera _eventCamera;

		private PKFxManager.Attribute _enabledEffectColorAttribute = new PKFxManager.Attribute("RGB", Vector4.one);

		private PKFxManager.Attribute _lineEffectBeginColorAttribute = new PKFxManager.Attribute("RGBstart", Vector3.one);

		private PKFxManager.Attribute _lineEffectEndColorAttribute = new PKFxManager.Attribute("RGBend", Vector3.one);

		private PKFxManager.Attribute _lineEffectTargetVectorAttribute = new PKFxManager.Attribute("Target", Vector3.one);

		private FieSkillTreeSelectablePoint _parent;

		private GDESkillTreeData _assigendSkillData;

		private BoxCollider _collider;

		private EndPointState _state = EndPointState.INITIALIZED;

		public FieSkillTreeSelectablePoint parent => _parent;

		public EndPointState state
		{
			get
			{
				return _state;
			}
			set
			{
				UpdateEffectState(value);
			}
		}

		public GDESkillTreeData assigendSkillData => _assigendSkillData;

		public event FieSkillTreeEndPointClickCallback clickedEvent;

		private void UpdateEffectState(EndPointState targetState)
		{
			if (targetState != _state)
			{
				switch (targetState)
				{
				case EndPointState.UNLOCKED:
					if (_state == EndPointState.LOCKED)
					{
						_disabledEffect.StopEffect();
					}
					_enabledEffect.SetAttribute(_enabledEffectColorAttribute);
					_enabledEffect.StopEffect();
					_enabledEffect.StartEffect();
					break;
				case EndPointState.LOCKED:
					if (_state == EndPointState.UNLOCKED)
					{
						_enabledEffect.StopEffect();
					}
					_disabledEffect.StopEffect();
					_disabledEffect.StartEffect();
					break;
				}
			}
			_state = targetState;
		}

		public void AssignSkillData(GDESkillTreeData skillData)
		{
			if (skillData != null)
			{
				_assigendSkillData = skillData;
				if (_assigendSkillData.UIColor != null)
				{
					_enabledEffectColorAttribute.ValueFloat4 = new Vector4(_assigendSkillData.UIColor.R, _assigendSkillData.UIColor.G, _assigendSkillData.UIColor.B, _assigendSkillData.UIColor.W);
				}
			}
		}

		private new void Awake()
		{
			base.Awake();
			_parent = base.gameObject.GetComponentInParent<FieSkillTreeSelectablePoint>();
			_collider = base.gameObject.GetComponent<BoxCollider>();
		}

		public new void OnPointerEnter(PointerEventData eventData)
		{
			if (base.interactable)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}
		}

		public new void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			GetComponent<EventTrigger>().OnSubmit(eventData);
		}

		private void Update()
		{
			if (_state == EndPointState.INITIALIZED)
			{
				state = EndPointState.LOCKED;
			}
		}

		private new void OnDisable()
		{
			base.OnDisable();
			_state = EndPointState.INITIALIZED;
			_enabledEffect.StopEffect();
			_disabledEffect.StopEffect();
			_lineObject.StopEffect();
		}

		public void DisableLineEffct()
		{
			_lineObject.StopEffect();
		}

		public void ConnectLineToEndPoint(FieSkillTreeEndPoint fieSkillTreeEndPoint)
		{
			_lineObject.StopEffect();
			if (_state == EndPointState.INITIALIZED)
			{
				state = EndPointState.LOCKED;
			}
			if (!(fieSkillTreeEndPoint == null))
			{
				if (state == EndPointState.UNLOCKED)
				{
					_lineEffectBeginColorAttribute.ValueFloat3 = new Vector4(_assigendSkillData.UIColor.R, _assigendSkillData.UIColor.G, _assigendSkillData.UIColor.B);
					_lineEffectEndColorAttribute.ValueFloat3 = new Vector3(fieSkillTreeEndPoint.assigendSkillData.UIColor.R, fieSkillTreeEndPoint.assigendSkillData.UIColor.G, fieSkillTreeEndPoint.assigendSkillData.UIColor.B);
				}
				else if (state == EndPointState.LOCKED)
				{
					_lineEffectBeginColorAttribute.ValueFloat3 = new Vector3(0.05f, 0.05f, 0.05f);
					_lineEffectEndColorAttribute.ValueFloat3 = new Vector3(0.05f, 0.05f, 0.05f);
				}
				Vector3 vector = base.transform.position - fieSkillTreeEndPoint.transform.position;
				_lineEffectTargetVectorAttribute.ValueFloat3 = fieSkillTreeEndPoint.transform.position - base.transform.position;
				_lineObject.SetAttribute(_lineEffectTargetVectorAttribute);
				_lineObject.SetAttribute(_lineEffectBeginColorAttribute);
				_lineObject.SetAttribute(_lineEffectEndColorAttribute);
				_lineObject.StopEffect();
				_lineObject.StartEffect();
			}
		}
	}
}
