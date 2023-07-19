using Fie.Ponies;
using Fie.Ponies.RisingSun;
using ParticlePlayground;
using UnityEngine;

public class FieRisingSunEffectManager : MonoBehaviour
{
	private const float HOOF_EFFECT_EMIT_THRESHOLD = 0.6f;

	public FieRisingSun _rising_sun;

	public PlaygroundParticlesC _teleportEffect;

	public PlaygroundParticlesC _teleportEndEffect;

	public Light _hornLight;

	public Transform _submeshTransform;

	public Renderer _submeshRenderer;

	public AudioSource _teleportSoundEffect;

	private bool _nowWarpEffect;

	private WorldObject _worldObject;

	private void Start()
	{
		_worldObject = PlaygroundParticlesC.NewWorldObject(_submeshTransform);
	}

	private void Update()
	{
		if (_rising_sun.getStateMachine().nowStateType() == typeof(FieStateMachinePoniesMove))
		{
			Vector3 velocity = _rising_sun.GetComponent<Rigidbody>().velocity;
			velocity.Normalize();
			float num = Mathf.Abs(velocity.x) * _rising_sun.externalInputForce;
		}
		if (_rising_sun.getStateMachine().nowStateType() == typeof(FieStateMachineRisingSunTeleportation))
		{
			if (!_nowWarpEffect)
			{
				_teleportEndEffect.worldObject = null;
				_teleportEndEffect.Emit(setEmission: false);
				PlaygroundParticlesC.SetParticleTimeNow(_teleportEffect);
				_teleportEffect.worldObject = _worldObject;
				_teleportEffect.source = SOURCEC.WorldObject;
				_teleportEffect.Emit(setEmission: true);
				_rising_sun.submeshObject.enabled = false;
				_nowWarpEffect = true;
				_teleportSoundEffect.Play();
			}
		}
		else if (_nowWarpEffect)
		{
			_teleportEffect.Emit(setEmission: false);
			_rising_sun.submeshObject.enabled = true;
			PlaygroundParticlesC.SetParticleTimeNow(_teleportEndEffect);
			_teleportEndEffect.transform.position = _rising_sun.torsoTransform.position;
			Vector3 position = _teleportEndEffect.transform.position;
			float x = position.x + 0.3f;
			Vector3 position2 = _teleportEndEffect.transform.position;
			float y = position2.y + 0.3f;
			Vector3 position3 = _teleportEndEffect.transform.position;
			Vector3 randomPositionMin = new Vector3(x, y, position3.z + 0.1f);
			Vector3 position4 = _teleportEndEffect.transform.position;
			float x2 = position4.x - 0.3f;
			Vector3 position5 = _teleportEndEffect.transform.position;
			float y2 = position5.y - 0.3f;
			Vector3 position6 = _teleportEndEffect.transform.position;
			Vector3 randomPositionMax = new Vector3(x2, y2, position6.z - 0.1f);
			_teleportEndEffect.Emit(100, randomPositionMin, randomPositionMax, Vector3.zero, Vector3.zero, default(Color32));
			_nowWarpEffect = false;
		}
	}

	private void OnLevelWasLoaded()
	{
		if ((bool)_teleportEffect)
		{
			_teleportEffect.Start();
		}
		if ((bool)_teleportEndEffect)
		{
			_teleportEndEffect.Start();
		}
	}

	private void updateFireEffect()
	{
	}

	private void deleteFireEffect()
	{
	}
}
