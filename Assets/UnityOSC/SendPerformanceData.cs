using UnityEngine;
using System.Collections;
using Code.Misc;
using UnityEngine.AI;

public class SendPerformanceData : MonoBehaviour {
	[SerializeField] private OSC _osc;
	[SerializeField] private FlowDescriptor _flowDescriptor;

	[SerializeField] private CombinedBounds _handBounds;
	[SerializeField] private CombinedBounds _legBounds;
	[SerializeField] private CombinedBounds _bodyBounds;

	[SerializeField] private MinMax _minMaxFlow;
	[SerializeField] private MinMax _minMaxBodyVol;
	[SerializeField] private MinMax _minMaxHandVol;

	[SerializeField] private bool _start;
	[SerializeField] private bool _calibrate;

	[ContextMenu("SetNames")]
	void SetNames() {
		_minMaxFlow.MyTitle = "Flow Descriptor";
		_minMaxBodyVol.MyTitle = "Body Volume";
		_minMaxHandVol.MyTitle = "Hand Volume";
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.S)) {
			_start = !_start;
			print("Start: " + _start);
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			_calibrate = !_calibrate;
			print("Calibrate: " + _calibrate);
		}

		if (_calibrate) {
			_minMaxFlow.CalculateMaxMinFloat(_flowDescriptor.FlowDescriptorVal);
			_minMaxBodyVol.CalculateMaxMinFloat(_bodyBounds.BoundingBox.size.magnitude);
			_minMaxHandVol.CalculateMaxMinFloat(_handBounds.BoundingBox.size.magnitude);
		}

		if (!_start) return;

		//print(HandBounds.BoundingBox.size.magnitude / max);

		OscMessage message = new OscMessage();

		//NewValue = (((OldValue - OldMin) * (NewMax - NewMin)) / (OldMax - OldMin)) + NewMin
		
		message.address = "/Flow";
		message.values.Add(Mathf.Clamp01((_flowDescriptor.FlowDescriptorVal - _minMaxFlow.MinFloat) / (_minMaxFlow.MaxFloat - _minMaxFlow.MinFloat)));
		
		_osc.Send(message);

		message = new OscMessage();
		message.address = "/BodyVol";
		message.values.Add(Mathf.Clamp01((_bodyBounds.BoundingBox.size.magnitude - _minMaxBodyVol.MinFloat) / (_minMaxBodyVol.MaxFloat - _minMaxBodyVol.MinFloat)));
		
		_osc.Send(message);

		message = new OscMessage();
		message.address = "/HandVol";
		message.values.Add(Mathf.Clamp01((_handBounds.BoundingBox.size.magnitude - _minMaxHandVol.MinFloat) / (_minMaxHandVol.MaxFloat - _minMaxHandVol.MinFloat)));
		
		_osc.Send(message);


		OnFlowChange();
		OnBodyVolumeChange();
	}

	public delegate void OnFlowChangeDelegate(float val);

	public static OnFlowChangeDelegate FlowChangeDelegate;

	public delegate void OnBodyVolumeDelegate(float val);

	public static OnBodyVolumeDelegate BodyVolumeDelegate;


	private void OnFlowChange() {
		FlowChangeDelegate(_flowDescriptor.FlowDescriptorVal / _minMaxFlow.MaxFloat);
	}

	private void OnBodyVolumeChange() {
		BodyVolumeDelegate((_bodyBounds.BoundingBox.size.magnitude / _minMaxBodyVol.MaxFloat) *
		                   (_flowDescriptor.FlowDescriptorVal / _minMaxFlow.MaxFloat));
	}
}


 
 
