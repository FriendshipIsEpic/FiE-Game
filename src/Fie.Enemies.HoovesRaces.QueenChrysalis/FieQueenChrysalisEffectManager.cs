using ParticlePlayground;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieQueenChrysalisEffectManager : MonoBehaviour
	{
		public FieQueenChrysalis _chrysalis;

		public PlaygroundParticlesC _teleportEffect;

		public PlaygroundParticlesC _teleportEndEffect;

		public PKFxFX _teleportingEffect;

		public Transform _submeshTransform;

		public AudioSource _teleportSoundEffect;

		private bool _isTeleporting;

		private WorldObject _worldObject;

		private void Start()
		{
			_worldObject = PlaygroundParticlesC.NewWorldObject(_submeshTransform);
		}

		private void Update()
		{
			if (_chrysalis.getStateMachine().nowStateType() == typeof(FieStateMachineQueenChrysalisTeleportation))
			{
				if (!_isTeleporting)
				{
					_teleportEndEffect.worldObject = null;
					_teleportEndEffect.Emit(setEmission: false);
					PlaygroundParticlesC.SetParticleTimeNow(_teleportEffect);
					_teleportEffect.worldObject = _worldObject;
					_teleportEffect.source = SOURCEC.WorldObject;
					_teleportEffect.Emit(setEmission: true);
					_chrysalis.submeshObject.enabled = false;
					_isTeleporting = true;
					_teleportSoundEffect.Play();
					_teleportingEffect.StopEffect();
					_teleportingEffect.StartEffect();
				}
			}
			else if (_isTeleporting)
			{
				_teleportEffect.Emit(setEmission: false);
				_chrysalis.submeshObject.enabled = true;
				PlaygroundParticlesC.SetParticleTimeNow(_teleportEndEffect);
				_teleportEndEffect.worldObject = _worldObject;
				_teleportEndEffect.source = SOURCEC.WorldObject;
				_teleportEndEffect.transform.position = _chrysalis.torsoTransform.position;
				Vector3 position = _teleportEndEffect.transform.position;
				float x = position.x + 0.3f;
				Vector3 position2 = _teleportEndEffect.transform.position;
				float y = position2.y + 0.3f;
				Vector3 position3 = _teleportEndEffect.transform.position;
				Vector3 vector = new Vector3(x, y, position3.z + 0.1f);
				Vector3 position4 = _teleportEndEffect.transform.position;
				float x2 = position4.x - 0.3f;
				Vector3 position5 = _teleportEndEffect.transform.position;
				float y2 = position5.y - 0.3f;
				Vector3 position6 = _teleportEndEffect.transform.position;
				Vector3 vector2 = new Vector3(x2, y2, position6.z - 0.1f);
				_teleportEndEffect.Emit(setEmission: true);
				_isTeleporting = false;
				_teleportingEffect.StopEffect();
			}
		}
	}
}
