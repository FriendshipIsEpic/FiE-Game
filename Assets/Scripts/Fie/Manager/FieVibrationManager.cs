using Fie.Object;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public sealed class FieVibrationManager : FieManagerBehaviour<FieVibrationManager>
	{
		private const int DEFAULT_CONTROLLER_INDEX = 0;

		private const float VIBRATION_SIN_WAVE_SPEED_PER_SEC = 360f;

		private const float VIBRATION_THRESHOLD_SMALL = 0.01f;

		private const float VIBRATION_THRESHOLD_MIDDLE = 0.25f;

		private const float VIBRATION_THRESHOLD_LARGE = 0.5f;

		private Dictionary<int, Coroutine> _currentCoroutines = new Dictionary<int, Coroutine>();

		private IEnumerator VibrationCoroutine(int controllerIndex, float duration, float force)
		{
			int targetControllerIndex = Mathf.Clamp(controllerIndex, 0, 3);
			float sinRate = 0f;
			if (duration > 0f)
			{
				SetVibration(targetControllerIndex, Mathf.Sin(sinRate) * force, Mathf.Sin(sinRate + 45f) * force);
				float num = sinRate + 360f * Time.deltaTime;
				float num2 = duration - Time.deltaTime;
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			SetVibration(targetControllerIndex, 0f, 0f);
		}

		protected override void StartUpEntity()
		{
			if (FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter != null)
			{
				FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.damageSystem.damagedEvent -= DamageCallbackForVibration;
				FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.damageSystem.damagedEvent += DamageCallbackForVibration;
			}
		}

		private void DamageCallbackForVibration(FieGameCharacter attacker, FieDamage damage)
		{
			if (damage.damage > 0f)
			{
				RequestVibration(0.4f, 1f);
			}
		}

		public void RequestVibration(float duration, float force)
		{
			int num = 0;
			if (_currentCoroutines.ContainsKey(num) && _currentCoroutines[num] != null)
			{
				StopCoroutine(_currentCoroutines[num]);
				_currentCoroutines[num] = null;
			}
			Coroutine value = StartCoroutine(VibrationCoroutine(num, duration, Mathf.Clamp(force, 0f, 1f)));
			_currentCoroutines[num] = value;
		}

		private void OnApplicationQuit()
		{
			int controllerId = 0;
			SetVibration(controllerId, 0f, 0f);
		}

		private void SetVibration(int controllerId, float leftMoterForce, float rightMoterForce)
		{
			foreach (Joystick joystick in FieManagerBehaviour<FieInputManager>.I.GetPlayer(controllerId).controllers.Joysticks)
			{
				joystick.SetVibration(leftMoterForce, rightMoterForce);
			}
		}
	}
}
