using UnityEngine;

public class MinMax {
	
    private float _minFloat;
    private float _maxFloat;

	private void CalculateMaxMinFloat(float val) {
		_minFloat = Mathf.Min(_minFloat, val);
		_maxFloat = Mathf.Max(_maxFloat, val);
    }

	public void GetMaxMinFloat(float val, out float min, out float max) {
        CalculateMaxMinFloat(val);

        min = _minFloat;
        max = _maxFloat;
    }

    private Vector2 _minVec2;
    private Vector2 _maxVec2;

	private void CalculateMaxMinVec2(Vector2 vec) {
        _minVec2.x = Mathf.Min(_minVec2.x, vec.x);
        _minVec2.y = Mathf.Min(_minVec2.x, vec.x);

        _maxVec2.x = Mathf.Max(_maxVec2.x, vec.x);
        _maxVec2.y = Mathf.Max(_maxVec2.y, vec.y);
    }


	public void GetMaxMinVec2(Vector2 vec, out Vector2 min, out Vector2 max) {
        CalculateMaxMinVec2(vec);

        min = _minVec2;
        max = _maxVec2;
    }

    private Vector3 _minVec3;
    private Vector3 _maxVec3;

	/// <summary>
	///     Calculate the maximum and minimum value of the Vector3 input.
	/// </summary>
	/// <param name="vec"></param>
	private void CalculateMaxMinVec3(Vector3 vec) {
        _minVec3.x = Mathf.Min(_minVec3.x, vec.x);
        _minVec3.y = Mathf.Min(_minVec3.x, vec.x);
        _minVec3.z = Mathf.Min(_minVec3.x, vec.x);

        _maxVec3.x = Mathf.Max(_maxVec3.x, vec.x);
        _maxVec3.y = Mathf.Max(_maxVec3.y, vec.y);
        _maxVec3.z = Mathf.Max(_maxVec3.z, vec.z);
    }

	/// <summary>
	///     Return two three-dimensional vectors for minimum and maximum values.
	/// </summary>
	/// <param name="vec"></param>
	/// <param name="min"></param>
	/// <param name="max"></param>
	/// <returns></returns>
	public void GetMaxMinVec3(Vector3 vec, out Vector3 min, out Vector3 max) {
        CalculateMaxMinVec3(vec);

        min = _minVec3;
        max = _maxVec3;
    }
}