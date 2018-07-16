using UnityEngine;

public class RunCompute : MonoBehaviour {
    #region Properties

    public int subMeshIndex;

    [SerializeField] private ComputeShader _compute;

    [SerializeField] private GameObject _boidPrefab;
    [SerializeField] private GameObject _attractorObj;

    [SerializeField] private Mesh _instanceMesh;

    [SerializeField] private int _instanceCount = 1000;

    [SerializeField] private float _spawnRadius = 5;
    [SerializeField] private float _nearbyDis = 5;
    [SerializeField] private float _speed = 10;
    [SerializeField] private float _randomSeed;

    public ComputeShader Compute {
        get { return _compute; }
        set { _compute = value; }
    }

    public GameObject BoidPrefab {
        get { return _boidPrefab; }
        set { _boidPrefab = value; }
    }

    public GameObject AttractorObj {
        get { return _attractorObj; }
        set { _attractorObj = value; }
    }

    public Mesh InstanceMesh {
        get { return _instanceMesh; }
        set { _instanceMesh = value; }
    }

    public float SpawnRadius {
        get { return _spawnRadius; }
        set { _spawnRadius = value; }
    }

    public float NearbyDis {
        get { return _nearbyDis; }
        set { _nearbyDis = value; }
    }

    public float RandomSeed {
        get { return _randomSeed; }
        set { _randomSeed = value; }
    }

    public float Speed {
        get { return _speed; }
        set { _speed = value; }
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

    private ComputeBuffer _drawArgsBuffer;

    private ComputeBuffer _positionBuffer;

    private ComputeBuffer _rotationBuffer;

    private ComputeBuffer _instanceCountBuffer;


    private int cachedInstanceCount = -1;

    private int cachedSubMeshIndex = -1;

    private readonly uint[] args = new uint[5] {0, 0, 0, 0, 0};

    #endregion

    private Vector2 cursorPos;


    // struct


    /// <summary>
    ///     Size in octet of the Particle struct.
    ///     since float = 4 bytes...
    ///     4 floats = 16 bytes
    /// </summary>
    //private const int SIZE_PARTICLE = 24;
    private const int SIZE_PARTICLE = 28; // since property "life" is added...

    /// <summary>
    ///     Number of Particle created in the system.
    /// </summary>
    private readonly int particleCount = 1000000;

    /// <summary>
    ///     Material used to draw the Particle on screen.
    /// </summary>
    public Material material;

    /// <summary>
    ///     Id of the kernel used.
    /// </summary>
    private int mComputeShaderKernelID;

    /// <summary>
    ///     Number of particle per warp.
    /// </summary>
    private const int WARP_SIZE = 256; // TODO?

    /// <summary>
    ///     Number of warp needed.
    /// </summary>
    private int mWarpCount; // TODO?

    //public ComputeShader shader;

    // Use this for initialization
    private void Start() {
        InitComputeShader();
    }

    private void InitComputeShader() {
        mWarpCount = Mathf.CeilToInt((float) particleCount / WARP_SIZE);


        // create compute buffer
        new ComputeBuffer(InstanceCount, 16);
        _drawArgsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        _positionBuffer = new ComputeBuffer(InstanceCount, 16);
        _rotationBuffer = new ComputeBuffer(InstanceCount, 16);

        // find the id of the kernel
        mComputeShaderKernelID = _compute.FindKernel("CSFlock");

        // bind the compute buffer to the shader and the compute shader
        _compute.SetFloat("nearbyDist", _nearbyDis);
        _compute.SetFloat("speed", _speed);

        _compute.SetBuffer(mComputeShaderKernelID, "positionBuffer", _positionBuffer);
        material.SetBuffer("positionBuffer", _positionBuffer);
        _compute.SetBuffer(mComputeShaderKernelID, "rotationBuffer", _rotationBuffer);
        material.SetBuffer("rotationBuffer", _rotationBuffer);
    }

    private void OnRenderObject() {
        material.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, 1, particleCount);
    }

    private void OnDestroy() {
        if (_positionBuffer != null) _positionBuffer.Release();
        if (_rotationBuffer != null) _rotationBuffer.Release();

        if (_drawArgsBuffer != null) _drawArgsBuffer.Release();

        if (_materialCloned) Destroy(_instanceMaterial);
    }

    // Update is called once per frame
    private void Update() {
        float[] attractPosition = {
            _attractorObj.transform.position.x, _attractorObj.transform.position.y, _attractorObj.transform.position.z
        };

        // Send datas to the compute shader
        _compute.SetFloat("deltaTime", Time.deltaTime);
        _compute.SetFloats("attractor", attractPosition);
        _compute.SetInt("instanceCount", _instanceCount);

        // Update the Particles
        _compute.Dispatch(mComputeShaderKernelID, _instanceCount, 1, 1);
        
        
    }

    private void OnGUI() {
        var p = new Vector3();
        var c = Camera.main;
        var e = Event.current;
        var mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane + 14)); // z = 3.

        cursorPos.x = p.x;
        cursorPos.y = p.y;
        /*
        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + c.pixelWidth + ":" + c.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + p.ToString("F3"));
        GUILayout.EndArea();
        */
    }
}