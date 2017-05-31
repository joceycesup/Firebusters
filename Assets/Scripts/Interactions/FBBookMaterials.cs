using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pair {
	public int k;
	public int v;
}

public class FBBookMaterials : MonoBehaviour {
	public static List<Material> materials;
	private static List<Pair> _limits;
	[SerializeField]
	public List<Pair> limits = new List<Pair> ();

	void Awake () {
		if (materials == null) {
			materials = new List<Material> ();
			Material loadingMaterial;
			int i = 1;
			while ((loadingMaterial = Resources.Load<Material> ("Textures/livre/livre" + i)) != null) {
				materials.Add (loadingMaterial);
				i++;
			}
			_limits = limits;
			foreach (Pair pair in _limits) {
				pair.k--;
			}
		}
		Destroy (this);
	}

	public static Material Get () {
		int index = UnityEngine.Random.Range (0, materials.Count);
		Material res = materials[index];
		int found = -1;
		for (int i = 0; i < _limits.Count; ++i) {
			if (_limits[i].k == index) {
				_limits[i].v--;
				found = i;
				break;
			}
		}
		if (found >= 0) {
			if (_limits[found].v <= 0) {
				materials.RemoveAt (_limits[found].k);
				_limits.RemoveAt (found);
				foreach (Pair pair in _limits) {
					if (pair.k > index) {
						pair.k--;
					}
				}
			}
		}
		return res;
	}
}
