using Code.Actors.Boids;
using UnityEngine;

public class ParticleController : MonoBehaviour {
    private Renderer objRenderer;

    [SerializeField] private AnimationCurve _nearDistCurve;
    [SerializeField] private AnimationCurve _attractionCurve;
    [SerializeField] private AnimationCurve _triggerDistCurve;

    [SerializeField] private FlockSimulation _flockSimulation;

    [SerializeField] private float _maxNeighbourDistance;
    [SerializeField] private float _minNeighbourDistance;

    [SerializeField] private float _maxAttractionForce;
    [SerializeField] private float _minAttractionForce;

    [SerializeField] private float _maxTriggerDistance;
    [SerializeField] private float _minTriggerDistance;


    // Use this for initialization
    private void Start() {
        SendPerformanceData.FlowChangeDelegate += ChangeTriggerDist;
        SendPerformanceData.FlowChangeDelegate += ChangeAttraction;

        SendPerformanceData.BodyVolumeDelegate += ChangeNearDist;
        objRenderer = GetComponent<Renderer>();
    }

    private void ChangeNearDist(float dist) {
        _flockSimulation.NeighbourDistance = _minNeighbourDistance +
                                             _nearDistCurve.Evaluate(dist) *
                                             (_maxNeighbourDistance - _minNeighbourDistance);
    }

    private void ChangeAttraction(float attraction) {
        _flockSimulation.AttractionForce = _minAttractionForce +
                                           _attractionCurve.Evaluate(attraction) *
                                           (_maxAttractionForce - _minAttractionForce);
    }

    private void ChangeTriggerDist(float dist) {
        _flockSimulation.TriggerDistance = _minTriggerDistance +
                                           _triggerDistCurve.Evaluate(dist) *
                                           (_maxTriggerDistance - _minTriggerDistance);
    }

    // Unsubscribing Delegate
    private void OnDisable() {
        SendPerformanceData.FlowChangeDelegate -= ChangeTriggerDist;
        SendPerformanceData.FlowChangeDelegate -= ChangeAttraction;
        SendPerformanceData.BodyVolumeDelegate -= ChangeNearDist;
    }    
}