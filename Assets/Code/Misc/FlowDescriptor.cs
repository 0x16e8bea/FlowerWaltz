using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlowDescriptor : MonoBehaviour {
	public Transform[] Trackers;
	
	[SerializeField]
	private int _windowSize = 30;
	
	[SerializeField] private float[] _velocityArray;
	[SerializeField] private float[] _accelerationArray;
	[SerializeField] private float[] _jerkArray;

	[SerializeField] private Queue<float> _jerkWindow;

	private Vector3[] _pPos;
	private float[] _pVel;
	private float[] _pAcc;

	[SerializeField]
	private float _flowDescriptorVal;

	public float FlowDescriptorVal {
		get { return _flowDescriptorVal; }
	}

	// Use this for initialization
	void Awake() {
		if (Trackers.Length != 0) {
			_velocityArray = new float[Trackers.Length];
			_accelerationArray = new float[Trackers.Length];
			_jerkArray = new float[Trackers.Length];
			_jerkWindow = new Queue<float>(_windowSize);
			_pPos = new Vector3[Trackers.Length];
			_pVel = new float[Trackers.Length];
			_pAcc = new float[Trackers.Length];
		}
	}

	// Update is called once per frame
	void FixedUpdate() {

		for (int i = 0; i < Trackers.Length; i++) {
			_velocityArray[i] = Mathf.Lerp(_velocityArray[i],
				((Trackers[i].position - _pPos[i]).magnitude) / Time.deltaTime, Time.deltaTime);
			_pPos[i] = Trackers[i].position;

			_accelerationArray[i] = Mathf.Abs(((_velocityArray[i]) - _pVel[i]) / Time.deltaTime);
			_pVel[i] = _velocityArray[i];

			_jerkArray[i] = Mathf.Abs(((_accelerationArray[i]) - _pAcc[i]) / Time.deltaTime);
			_pAcc[i] = _accelerationArray[i];
			
		}

		if (_jerkWindow.Count < _windowSize) {
			_jerkWindow.Enqueue(_jerkArray.Average());
		} else {
			_jerkWindow.Dequeue();
			_jerkWindow.Enqueue(_jerkArray.Average());
			_flowDescriptorVal = _jerkWindow.Sum() * 1 / _windowSize;
		}
		
		//print(_jerkWindow.Count);
		
	}
}
