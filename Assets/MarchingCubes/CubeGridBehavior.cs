using UnityEngine;

[ExecuteInEditMode]
public class CubeGridBehavior : MonoBehaviour {
    public Vector2Int Size = new Vector2Int(8, 4);

    [SerializeField]
    private float Scale = 24f;
    [SerializeField]
    private float IsoSurfaceLevel = 0.5f;
    [SerializeField]
    private Vector3 offset;
    private float settingsCacheVal; // Junk just to gate updating unless user changes a value
    private float settingsVal = -1;
    private float[] noiseMap;
    private MarchingCubes marcher;
    private MeshFilter meshFilter;

    private void Awake() {
        Debug.Log("Initializing Marching Cubes Grid Behavior");

        meshFilter = GetComponent<MeshFilter>();
    }

    private void Update() {
        settingsVal = Size.magnitude + IsoSurfaceLevel + Scale + offset.magnitude;

        if (settingsCacheVal == settingsVal) {
            return;
        }

        settingsCacheVal = settingsVal;

        noiseMap = CubeNoise.TwoD.GenNoise(Size, offset, Scale);
        marcher = new MarchingCubes(transform.position, Size, IsoSurfaceLevel);
        marcher.ClearMesh();
        marcher.MarchNoise(noiseMap);
        meshFilter.mesh = marcher.BuildMesh();
    }

}
