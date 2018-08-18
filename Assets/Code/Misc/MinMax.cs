using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class MinMax : MonoBehaviour {
	[Title("$MyTitle")] public bool HideProperties;

	[OdinSerialize, HideIf("HideProperties")]
	public string MyTitle;


	[SerializeField] private float _minFloat;
	[SerializeField] private float _maxFloat;

	public float MinFloat {
		get { return _minFloat; }
		set { _minFloat = value; }
	}

	public float MaxFloat {
		get { return _maxFloat; }
		set { _maxFloat = value; }
	}


	public void CalculateMaxMinFloat(float val) {
		_minFloat = Mathf.Min(_minFloat, val);
		_maxFloat = Mathf.Max(_maxFloat, val);
	}

	public void GetMaxMinFloat(out float min, out float max) {
		min = _minFloat;
		max = _maxFloat;
	}

	[SerializeField] private Vector2 _minVec2;
	[SerializeField] private Vector2 _maxVec2;

	public Vector2 MinVec2 {
		get { return _minVec2; }
		set { _minVec2 = value; }
	}

	public Vector2 MaxVec2 {
		get { return _maxVec2; }
		set { _maxVec2 = value; }
	}

	public void CalculateMaxMinVec2(Vector2 vec) {
		_minVec2.x = Mathf.Min(_minVec2.x, vec.x);
		_minVec2.y = Mathf.Min(_minVec2.x, vec.x);

		_maxVec2.x = Mathf.Max(_maxVec2.x, vec.x);
		_maxVec2.y = Mathf.Max(_maxVec2.y, vec.y);
	}


	public void GetMaxMinVec2(out Vector2 min, out Vector2 max) {
		min = _minVec2;
		max = _maxVec2;
	}

	[SerializeField] private Vector3 _minVec3;
	[SerializeField] private Vector3 _maxVec3;

	public Vector3 MinVec3 {
		get { return _minVec3; }
		set { _minVec3 = value; }
	}

	public Vector3 MaxVec3 {
		get { return _maxVec3; }
		set { _maxVec3 = value; }
	}

	public void CalculateMaxMinVec3(Vector3 vec) {
		_minVec3.x = Mathf.Min(_minVec3.x, vec.x);
		_minVec3.y = Mathf.Min(_minVec3.x, vec.x);
		_minVec3.z = Mathf.Min(_minVec3.x, vec.x);

		_maxVec3.x = Mathf.Max(_maxVec3.x, vec.x);
		_maxVec3.y = Mathf.Max(_maxVec3.y, vec.y);
		_maxVec3.z = Mathf.Max(_maxVec3.z, vec.z);
	}

	public void GetMaxMinVec3(out Vector3 min, out Vector3 max) {
		min = _minVec3;
		max = _maxVec3;
	}
}