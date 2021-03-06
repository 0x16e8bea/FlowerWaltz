using UnityEngine;

namespace Code.Actors.Boids {
    public class FlockSimulation : MonoBehaviour {
        [SerializeField] private ComputeShader _computeFlock;
        [SerializeField] private int _particleCount = 100;
        [SerializeField] private float _drag = 0.0f;
        [SerializeField] private float _spawnRadius = 10.0f;
        [SerializeField] private float _maxLifeTime = 5.0f;
        [SerializeField] private float _triggerDistance = 5.0f;

        [SerializeField] private float _attractionForce = 1.0f;
        [SerializeField] private float _speed = 1.0f;
        [SerializeField] private float _gravity = 5.0f;
        [SerializeField] private float _scale = 1.0f;
        [SerializeField] private float _neighbourDistance = 1.0f;
        [SerializeField] private float _speedVariation = 1.0f;
        [SerializeField] private Transform _target;
        [SerializeField] private Mesh _instanceMesh;
        [SerializeField] private Material _particleMaterial;

        [SerializeField] private float _noiseFrequency;
        [SerializeField] private float _noiseAmplitude;


        #region Particle Properties

        public ComputeShader ComputeFlock {
            get { return _computeFlock; }
            set { _computeFlock = value; }
        }
        public float TriggerDistance {
            get { return _triggerDistance; }
            set { _triggerDistance = value; }
        }

        public int ParticleCount {
            get { return _particleCount; }
            set { _particleCount = value; }
        }

        public float SpawnRadius {
            get { return _spawnRadius; }
            set { _spawnRadius = value; }
        }

        public float AttractionForce {
            get { return _attractionForce; }
            set { _attractionForce = value; }
        }

        public float Speed {
            get { return _speed; }
            set { _speed = value; }
        }

        public float NeighbourDistance {
            get { return _neighbourDistance; }
            set { _neighbourDistance = value; }
        }

        public float SpeedVariation {
            get { return _speedVariation; }
            set { _speedVariation = value; }
        }

        public Transform Target {
            get { return _target; }
            set { _target = value; }
        }

        public Mesh InstanceMesh {
            get { return _instanceMesh; }
            set { _instanceMesh = value; }
        }

        public Material BoidMaterial {
            get { return _particleMaterial; }
            set { _particleMaterial = value; }
        }

        #endregion

        #region Buffers

        private ComputeBuffer _drawArgsBuffer;
        private ComputeBuffer _positionBuffer;
        private ComputeBuffer _velocityBuffer;
        private ComputeBuffer _rotationBuffer;
        private ComputeBuffer _lifeTimeBuffer;

        #endregion

        #region Shader Properties

        private int _mComputeShaderKernelID;
        private MaterialPropertyBlock _props;
        private const int WAVE_SIZE = 32;
        private int _waveCount;

        #endregion

        private void Start() {
            _waveCount = Mathf.CeilToInt((float) _particleCount / WAVE_SIZE);
            
            // ComputeBuffer used for Graphics.DrawProceduralIndirect,
            // ComputeShader.DispatchIndirect or Graphics.DrawMeshInstancedIndirect
            // arguments.
            _drawArgsBuffer = new ComputeBuffer(
                1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments
            );

            _drawArgsBuffer.SetData(new uint[5] {
                _instanceMesh.GetIndexCount(0), (uint) _particleCount, 0, 0, 0
            });

            // Basic material properties.
            _props = new MaterialPropertyBlock();
            _props.SetFloat("_UniqueID", Random.value);

            // Get the shader kernel called CSMain.
            _mComputeShaderKernelID = _computeFlock.FindKernel("CSMain");

            // Declare and initialize particle data arrays.
            var positionArray = new Vector3[_particleCount];
            var velocityArray = new Vector3[_particleCount];
            var rotationArray = new Vector3[_particleCount];
            var lifeTimeArray = new float[_particleCount];

            for (var i = 0; i < _particleCount; i++) {
                var pos = transform.position + (Random.insideUnitSphere * _spawnRadius);
                var rot = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
                var vel = new Vector3(0, 0, 0);
                positionArray[i] = pos;
                velocityArray[i] = vel;
                rotationArray[i] = new Vector4(rot.eulerAngles.x, rot.eulerAngles.y, rot.eulerAngles.z);

                lifeTimeArray[i] = _maxLifeTime;
            }

            // Instantiate data buffers.
            _positionBuffer = new ComputeBuffer(_particleCount, 12);
            _velocityBuffer = new ComputeBuffer(_particleCount, 12);
            _rotationBuffer = new ComputeBuffer(_particleCount, 12);
            _lifeTimeBuffer = new ComputeBuffer(_particleCount, 4);

            // Set data in buffers.
            _positionBuffer.SetData(positionArray);
            _velocityBuffer.SetData(velocityArray);
            _rotationBuffer.SetData(rotationArray);
            _lifeTimeBuffer.SetData(lifeTimeArray);
        }

        private void Update() {
            _computeFlock.SetFloat("DeltaTime", Time.deltaTime);
            var drag = Mathf.Exp(-_drag * Time.deltaTime);
            _computeFlock.SetFloat("Drag", drag);
            _computeFlock.SetFloat("AttractionForce", _attractionForce);
            _computeFlock.SetFloat("Gravity", _gravity);
            _computeFlock.SetFloat("ParticleSpeed", _speed);
            _computeFlock.SetFloat("MaxLifeTime", _maxLifeTime);
            _computeFlock.SetFloat("TriggerDistance", _triggerDistance);

            _computeFlock.SetFloat("ParticleSpeedVariation", _speedVariation);
            _computeFlock.SetVector("TargetPosition", _target.transform.position);
            _computeFlock.SetVector("_NoiseParams", new Vector2(_noiseFrequency, _noiseAmplitude));
            _computeFlock.SetFloat("NeighbourDistance", _neighbourDistance);
            _computeFlock.SetInt("ParticleCount", _particleCount);

            _computeFlock.SetBuffer(_mComputeShaderKernelID, "PositionBuffer", _positionBuffer);
            _computeFlock.SetBuffer(_mComputeShaderKernelID, "VelocityBuffer", _velocityBuffer);
            _computeFlock.SetBuffer(_mComputeShaderKernelID, "RotationBuffer", _rotationBuffer);
            _computeFlock.SetBuffer(_mComputeShaderKernelID, "LifeTime", _lifeTimeBuffer);
            

            // Dispatch to GPU with thread group size proportional to the particles.
            _computeFlock.Dispatch(_mComputeShaderKernelID, _waveCount, 1, 1);

            // Set shader buffers.
            _particleMaterial.SetBuffer("positionBuffer", _positionBuffer);
            _particleMaterial.SetBuffer("lifeTimeBuffer", _lifeTimeBuffer);
            _particleMaterial.SetBuffer("rotationBuffer", _rotationBuffer);
            _particleMaterial.SetFloat("maxLifeTime", _maxLifeTime);
            _particleMaterial.SetFloat("scale", _scale);

            // Draw the same mesh multiple times using GPU instancing.
            Graphics.DrawMeshInstancedIndirect(
                _instanceMesh, 0, _particleMaterial,
                new Bounds(Vector3.zero, Vector3.one * 1000),
                _drawArgsBuffer, 0, _props
            );
        }

        private void OnDestroy() {
            if (_positionBuffer != null) _positionBuffer.Release();
            if (_rotationBuffer != null) _rotationBuffer.Release();
            if (_lifeTimeBuffer != null) _lifeTimeBuffer.Release();
            if (_drawArgsBuffer != null) _drawArgsBuffer.Release();
        }
    }
}