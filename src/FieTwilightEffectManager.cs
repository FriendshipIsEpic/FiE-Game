using Fie.Ponies;
using Fie.Ponies.Twilight;
using ParticlePlayground;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FieTwilightEffectManager : MonoBehaviour
{
	private const float HOOF_EFFECT_EMIT_THRESHOLD = 0.6f;

	public FieTwilight _twilight;

	public Transform _hornBaseTransform;

	public Transform _hornEntityTransform;

	public PlaygroundParticlesC _teleportEffect;

	public PlaygroundParticlesC _teleportEndEffect;

	public PlaygroundParticlesC _baseShotConcentrationPkFx;

	public PKFxFX _baseShotLv2ConcentrationPkFx;

	public Light _hornLight;

	public Transform _submeshTransform;

	public Renderer _submeshRenderer;

	public AudioSource _teleportSoundEffect;

	private bool _nowWarpEffect;

	private WorldObject _worldObject;

	private GameObject _baseShotConcentrationGo;

	private bool _isConcentratingBaseShot;

	private int _chargingEffectCount;

	private void Awake()
	{
		SceneManager.sceneLoaded += SceneManager_sceneLoaded;
	}

	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		if ((bool)_baseShotConcentrationPkFx)
		{
			_baseShotConcentrationPkFx.Start();
		}
		if ((bool)_teleportEffect)
		{
			_teleportEffect.Start();
		}
		if ((bool)_teleportEndEffect)
		{
			_teleportEndEffect.Start();
		}
	}

	private void Start()
	{
		_worldObject = PlaygroundParticlesC.NewWorldObject(_submeshTransform);
		_baseShotConcentrationGo = _baseShotConcentrationPkFx.gameObject;
	}

	private void Update()
	{
		if (_twilight.baseAttackChargedForce > 0.15f)
		{
			if (!_isConcentratingBaseShot)
			{
				_baseShotConcentrationPkFx.Emit(setEmission: true);
				_isConcentratingBaseShot = true;
			}
			else
			{
				_baseShotConcentrationPkFx.scale = (float)(_twilight.chargedCount + 1);
			}
		}
		if (_twilight.baseAttackChargedForce <= 0.15f && _isConcentratingBaseShot)
		{
			_baseShotConcentrationPkFx.Emit(setEmission: false);
			_isConcentratingBaseShot = false;
		}
		if (_twilight.chargedCount != _chargingEffectCount)
		{
			if (_twilight.chargedCount == 2)
			{
				_baseShotLv2ConcentrationPkFx.StartEffect();
			}
			if (_chargingEffectCount == 2)
			{
				_baseShotLv2ConcentrationPkFx.StopEffect();
			}
			_chargingEffectCount = _twilight.chargedCount;
		}
		if (_twilight.getStateMachine().nowStateType() == typeof(FieStateMachinePoniesMove))
		{
			Vector3 velocity = _twilight.GetComponent<Rigidbody>().velocity;
			velocity.Normalize();
			float num = Mathf.Abs(velocity.x) * _twilight.externalInputForce;
		}
		if (_twilight.getStateMachine().nowStateType() == typeof(FieStateMachineTwilightTeleportation))
		{
			if (!_nowWarpEffect)
			{
				_teleportEndEffect.worldObject = null;
				_teleportEndEffect.Emit(setEmission: false);
				PlaygroundParticlesC.SetParticleTimeNow(_teleportEffect);
				_teleportEffect.worldObject = _worldObject;
				_teleportEffect.source = SOURCEC.WorldObject;
				_teleportEffect.Emit(setEmission: true);
				_twilight.submeshObject.enabled = false;
				_nowWarpEffect = true;
				_teleportSoundEffect.Play();
			}
		}
		else if (_nowWarpEffect)
		{
			_teleportEffect.Emit(setEmission: false);
			_twilight.submeshObject.enabled = true;
			PlaygroundParticlesC.SetParticleTimeNow(_teleportEndEffect);
			_teleportEndEffect.transform.position = _twilight.torsoTransform.position;
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

	private void LateUpdate()
	{
		_hornEntityTransform.position = _hornBaseTransform.position;
		_baseShotConcentrationGo.transform.position = _twilight.hornTransform.position;
	}

	private void updateFireEffect()
	{
	}

	private void deleteFireEffect()
	{
	}
}
