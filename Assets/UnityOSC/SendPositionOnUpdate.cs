using UnityEngine;
using System.Collections;
using Code.Misc;
using UnityEngine.AI;

public class SendPositionOnUpdate : MonoBehaviour {
	[SerializeField] private OSC _osc;
	[SerializeField] private CombinedBounds _handBounds;
	[SerializeField] private CombinedBounds _legBounds;
	[SerializeField] private CombinedBounds _bodyBounds;
	[SerializeField] private FlowDescriptor _flowDescriptor;

	private readonly MinMax _minMaxFlow = new MinMax();
	private float _maxFlow;
	private float _minFlow;

	private bool _start;

	private MinMax _minMaxBodyVol = new MinMax();
	private float _maxBodyVol;
	private float _minBodyVol;

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.S)) {
			_start = !_start;
			print(_start);
		}

		if (!_start) return;

		OscMessage message = new OscMessage();

		_minMaxFlow.GetMaxMinFloat(_flowDescriptor.FlowDescriptorVal, out _minFlow, out _maxFlow);
		_minMaxBodyVol.GetMaxMinFloat(_bodyBounds.BoundingBox.size.magnitude, out _minBodyVol, out _maxBodyVol);

		//print(HandBounds.BoundingBox.size.magnitude / max);

		message.address = "/Flow";
		message.values.Add(_flowDescriptor.FlowDescriptorVal / _maxFlow);
		_osc.Send(message);
		
		message.address = "/BodyVol";
		message.values.Add(_bodyBounds.BoundingBox.size.magnitude / _maxBodyVol);
		_osc.Send(message);

		OnFlowChange();
		OnBodyVolumeChange();
	}

	public delegate void OnFlowChangeDelegate(float val);

	public static OnFlowChangeDelegate FlowChangeDelegate;

	public delegate void OnBodyVolumeDelegate(float val);

	public static OnBodyVolumeDelegate BodyVolumeDelegate;


	private void OnFlowChange() {
		FlowChangeDelegate(_flowDescriptor.FlowDescriptorVal / _maxFlow);
	}

	private void OnBodyVolumeChange() {
		BodyVolumeDelegate((_bodyBounds.BoundingBox.size.magnitude / _maxBodyVol) *
		                   (_flowDescriptor.FlowDescriptorVal / _maxFlow));
	}
}


 
 
