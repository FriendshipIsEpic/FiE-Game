using System.Collections.Generic;
using UnityEngine;

public abstract class FieInput : MonoBehaviour
{
	protected delegate void updateCallback();

	private List<FieGameCharacter> _controllableCharacters;

	protected updateCallback callback;

	protected List<FieGameCharacter> controllableCharacters => _controllableCharacters;

	public void Awake()
	{
		FieGameCharacter[] array = Object.FindObjectsOfType(typeof(FieGameCharacter)) as FieGameCharacter[];
		_controllableCharacters = new List<FieGameCharacter>();
		if (array.Length > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].intelligenceType == FieGameCharacter.IntelligenceType.Controllable)
				{
					_controllableCharacters.Add(array[i]);
				}
			}
		}
	}

	public void Update()
	{
		UpdateCallback();
	}

	private void UpdateCallback()
	{
		if (callback != null)
		{
			callback();
			callback = null;
		}
	}
}
