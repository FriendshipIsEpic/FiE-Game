using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class FieSortingOrder : MonoBehaviour
{
	[SerializeField]
	private string layerName = "Default";

	[SerializeField]
	private int orderInLayer;

	private Renderer _renderer;

	public string LayerName
	{
		get
		{
			return layerName;
		}
		set
		{
			layerName = value;
			Renderer[] components = GetComponents<Renderer>();
			foreach (Renderer renderer in components)
			{
				renderer.sortingLayerName = layerName;
			}
		}
	}

	public int OrderInLayer
	{
		get
		{
			return orderInLayer;
		}
		set
		{
			orderInLayer = value;
			Renderer[] components = GetComponents<Renderer>();
			foreach (Renderer renderer in components)
			{
				renderer.sortingOrder = orderInLayer;
			}
		}
	}

	private void Awake()
	{
		LayerName = layerName;
		OrderInLayer = orderInLayer;
	}

	private void OnValidate()
	{
		LayerName = layerName;
		OrderInLayer = orderInLayer;
	}
}
