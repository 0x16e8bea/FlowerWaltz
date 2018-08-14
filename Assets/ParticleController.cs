using System;
using UnityEngine;
using System.Collections;
using Code.Actors.Boids;

public class ParticleController : MonoBehaviour {

	Renderer objRenderer;
	public AnimationCurve NearDistCurve;
	public AnimationCurve AttractionCurve;
	public AnimationCurve TriggerDistCurve;

	[SerializeField] private FlockSimulation _flockSimulation;

	// Use this for initialization
	void Start() {
		SendPositionOnUpdate.FlowChangeDelegate += ChangeTriggerDist;
		SendPositionOnUpdate.FlowChangeDelegate += ChangeAttraction;

		SendPositionOnUpdate.BodyVolumeDelegate += ChangeNearDist;
		objRenderer = GetComponent<Renderer>();
	}

	void ChangeNearDist(float dist) {
		_flockSimulation.NeighbourDistance = NearDistCurve.Evaluate(dist) * 5;
	}
	
	void ChangeAttraction(float attraction) {
		_flockSimulation.AttractionForce = AttractionCurve.Evaluate(attraction) * 5.16f;
	}

	void ChangeTriggerDist(float dist) {
		_flockSimulation.TriggerDistance = TriggerDistCurve.Evaluate(dist) * 5;
	}

	// Unsubscribing Delegate
	void OnDisable() {
		SendPositionOnUpdate.FlowChangeDelegate -= ChangeTriggerDist;
		SendPositionOnUpdate.FlowChangeDelegate -= ChangeAttraction;

		SendPositionOnUpdate.BodyVolumeDelegate -= ChangeNearDist;
	}

}