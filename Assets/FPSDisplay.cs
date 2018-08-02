using UnityEngine;

public class FPSDisplay : MonoBehaviour {
	private float _deltaTime = 0.0f;
	private float _unscaledTime;
	private float _accumFps;
	private float _averageFps;
	private int _counter;

	readonly GUIStyle _style1 = new GUIStyle();
	readonly GUIStyle _style2 = new GUIStyle();

	private void Start() {
		_style2.normal.textColor = Color.white;
	}


	void Update() {
		_deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
		_unscaledTime += Time.unscaledDeltaTime;
	}

	void OnGUI() {
		int w = Screen.width, h = Screen.height;

		Rect rect = new Rect(0, 0, w, h * 2 / 100);

		_style1.alignment = TextAnchor.UpperLeft;
		_style1.fontSize = h * 2 / 50;
		_style1.normal.textColor = Color.white;

		_style2.alignment = TextAnchor.UpperCenter;
		_style2.fontSize = h * 2 / 50;

		float msec = _deltaTime * 1000.0f;
		float fps = 1.0f / _deltaTime;

		if (_unscaledTime > 5 && _unscaledTime < 35) {
			_accumFps += fps;
			_counter++;
			_averageFps = _accumFps / _counter;
		}

		if (_unscaledTime > 35) {
			_style2.normal.textColor = Color.green;
		}

		string text1 = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		string text2 = string.Format("({0:0} avg. fps)", _averageFps);

		GUI.Label(rect, text1, _style1);
		GUI.Label(rect, text2, _style2);
	}
}
