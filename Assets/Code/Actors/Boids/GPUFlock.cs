using UnityEngine;

namespace Code.Actors.Boids {
    public class GPUFlock : MonoBehaviour {
        #region Properties

        public int subMeshIndex;

        [SerializeField] private ComputeShader _compute;

        [SerializeField] private GameObject _boidPrefab;

        [SerializeField] private GameObject[] _boidObj;

        [SerializeField] private GameObject _attractorObj;

        [SerializeField] private GPUBoid[] _boidsData;

        [SerializeField] private Mesh _instanceMesh;

        [SerializeField] private int _instanceCount = 1000;

        public int instanceCount {
            get { return _instanceCount; }
        }

        [SerializeField] private float _spawnRadius;

        [SerializeField] private float _maxSpeed;

        [SerializeField] private float _nearbyDis;

        [SerializeField] private float _spread;

        [SerializeField] private float _randomSeed;


        public ComputeShader Compute {
            get { return _compute; }
            set { _compute = value; }
        }

        public GameObject BoidPrefab {
            get { return _boidPrefab; }
            set { _boidPrefab = value; }
        }

        public GameObject[] BoidObj {
            get { return _boidObj; }
            set { _boidObj = value; }
        }

        public GameObject AttractorObj {
            get { return _attractorObj; }
            set { _attractorObj = value; }
        }

        public GPUBoid[] BoidsData {
            get { return _boidsData; }
            set { _boidsData = value; }
        }

        public Mesh InstanceMesh {
            get { return _instanceMesh; }
            set { _instanceMesh = value; }
        }

        public float SpawnRadius {
            get { return _spawnRadius; }
            set { _spawnRadius = value; }
        }

        public float MaxSpeed {
            get { return _maxSpeed; }
            set { _maxSpeed = value; }
        }

        public float NearbyDis {
            get { return _nearbyDis; }
            set { _nearbyDis = value; }
        }

        public float Spread {
            get { return _spread; }
            set { _spread = value; }
        }

        public float RandomSeed {
            get { return _randomSeed; }
            set { _randomSeed = value; }
        }

        #endregion

        #region Thread properties

        private const int kThreadCount = 64;

        private int ThreadGroupCount {
            get { return _instanceCount / kThreadCount; }
        }

        private int InstanceCount {
            get { return kThreadCount * ThreadGroupCount; }
        }

        #endregion

        #region Material properties

        [SerializeField] private Material _instanceMaterial;
        private bool _materialCloned;

        public Material InstanceMaterial {
            get { return _instanceMaterial; }
        }

        #endregion

        #region Private members

        private ComputeBuffer _boidBuffer;
        private ComputeBuffer _drawArgsBuffer;
        private ComputeBuffer _positionBuffer;
        private ComputeBuffer _rotationBuffer;
        private ComputeBuffer _velocityBuffer;
        private ComputeBuffer _attractorBuffer;
        private ComputeBuffer _instanceCountBuffer;

        private int cachedInstanceCount = -1;
        private int cachedSubMeshIndex = -1;
        private readonly uint[] args = new uint[5] {0, 0, 0, 0, 0};

        private GPUBoid CreateBoidData() {
            var boidData = new GPUBoid();
            //var pos = transform.position + Random.insideUnitSphere * _spawnRadius;
            //var rot = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
            //boidData.Pos = pos;
            //boidData.Rot = rot.eulerAngles;
//
            //boidData.InstanceCount = InstanceCount;
            
            boidData.MaxSpeed = _maxSpeed;

            return boidData;
        }

        #endregion

        #region Monobehaviour functions

        private void OnDestroy() {
            if (_positionBuffer != null) _positionBuffer.Release();
            if (_rotationBuffer != null) _rotationBuffer.Release();
            if (_velocityBuffer != null) _velocityBuffer.Release();
            if (_attractorBuffer != null) _attractorBuffer.Release();

            if (_boidBuffer != null) _boidBuffer.Release();
            if (_drawArgsBuffer != null) _drawArgsBuffer.Release();

            if (_materialCloned) Destroy(_instanceMaterial);
        }

        private void Start() {
            // Allocate compute buffer.
            _boidBuffer = new ComputeBuffer(InstanceCount, 16);
            _drawArgsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            _positionBuffer = new ComputeBuffer(InstanceCount, 16);
            _rotationBuffer = new ComputeBuffer(InstanceCount, 16);
            _velocityBuffer = new ComputeBuffer(InstanceCount, 16);
            _attractorBuffer = new ComputeBuffer(InstanceCount, 16);

            // Initialize boid data.
            _boidObj = new GameObject[InstanceCount];
            _boidsData = new GPUBoid[InstanceCount];

            for (var i = 0; i < InstanceCount; i++) _boidsData[i] = CreateBoidData();
            
            ResetPositions();

            // Clone the given material before using.
            _instanceMaterial = new Material(_instanceMaterial);
            _instanceMaterial.name += " (cloned)";
            _materialCloned = true;
        }

        private void ResetPositions() {
            if (_positionBuffer == null || _boidBuffer == null) return;

            var kernel = _compute.FindKernel("Init");

            _boidBuffer.SetData(_boidsData);
            _compute.SetBuffer(kernel, "boidBuffer", _boidBuffer);
            _compute.SetBuffer(kernel, "positionBuffer", _positionBuffer);
            _compute.SetBuffer(kernel, "rotationBuffer", _rotationBuffer);
            _compute.SetBuffer(kernel, "velocityBuffer", _velocityBuffer);
            _compute.SetBuffer(kernel, "attractorBuffer", _attractorBuffer);
            _compute.SetVector("attractor", new Vector4(
                _attractorObj.transform.position.x,
                _attractorObj.transform.position.y,
                _attractorObj.transform.position.z,
                _spread));

            _compute.SetFloat("randomSeed", _randomSeed);
            _compute.SetFloat("nearbyDist", _nearbyDis);

            _compute.Dispatch(kernel, InstanceCount, 1, 1);

            // Draw the mesh with instancing.
            _instanceMaterial.SetBuffer("positionBuffer", _positionBuffer);
        }


        private void Update() {
            UpdateParticles();


            UpdateMaterial();
            Graphics.DrawMeshInstancedIndirect(_instanceMesh, subMeshIndex, _instanceMaterial,
                new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), _drawArgsBuffer);
        }

        private void UpdateParticles() {
            var delta = Mathf.Min(Time.deltaTime, 1.0f / 30);

            if (delta > 0) {
                var kernel = _compute.FindKernel("Flock");

                // Ensure submesh index is in range.
                if (_instanceMesh != null)
                    subMeshIndex = Mathf.Clamp(subMeshIndex, 0, _instanceMesh.subMeshCount - 1);

                _boidBuffer.SetData(_boidsData);
                _compute.SetBuffer(kernel, "boidBuffer", _boidBuffer);
                _compute.SetBuffer(kernel, "positionBuffer", _positionBuffer);
                _compute.SetBuffer(kernel, "rotationBuffer", _rotationBuffer);
                _compute.SetBuffer(kernel, "velocityBuffer", _velocityBuffer);
                _compute.SetBuffer(kernel, "attractorBuffer", _attractorBuffer);

                _compute.SetVector("attractor", new Vector4(
                    _attractorObj.transform.position.x,
                    _attractorObj.transform.position.y,
                    _attractorObj.transform.position.z,
                    _spread));

                _compute.SetInt("instanceCount", _instanceCount);
                _compute.SetFloat("deltaTime", delta);
                _compute.SetFloat("randomSeed", _randomSeed);
                _compute.SetFloat("nearbyDist", _nearbyDis);

                _compute.Dispatch(kernel, InstanceCount, 1, 1);
            }
        }

        private void UpdateMaterial() {
            _instanceMaterial.SetBuffer("positionBuffer", _positionBuffer);
            _instanceMaterial.SetBuffer("rotationBuffer", _rotationBuffer);

            // Indirect args
            if (_instanceMesh != null) {
                args[0] = _instanceMesh.GetIndexCount(subMeshIndex);
                args[1] = (uint) InstanceCount;
                args[2] = _instanceMesh.GetIndexStart(subMeshIndex);
                args[3] = _instanceMesh.GetBaseVertex(subMeshIndex);
            }
            else {
                args[0] = args[1] = args[2] = args[3] = 0;
            }

            _drawArgsBuffer.SetData(args);

            cachedInstanceCount = InstanceCount;
            cachedSubMeshIndex = subMeshIndex;
        }

        #endregion
    }
}