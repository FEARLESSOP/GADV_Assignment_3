using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ChainLink : MonoBehaviour
{
    [Header("References")]
    public Transform spikeBall;
    public GameObject chainPrefab;

    [Header("Chain Settings")]
    public float chainSpacing = 0.5f;
    public float spikeBallDistance = 3f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 30f;
    public bool rotateClockwise = true;
    public bool rotateInEditMode = true;

    private void OnValidate()
    {
#if UNITY_EDITOR
        //stop if this is a prefab in project
        if (PrefabUtility.IsPartOfPrefabAsset(this))
            return;

        //if not playing, make chain later
        if (!Application.isPlaying)
        {
            EditorApplication.delayCall += () =>
            {
                if (this != null)
                    GenerateChainEditor();
            };
            return;
        }
#endif
        //make chain in play mode
        GenerateChainRuntime();
    }

    private void Awake()
    {
#if UNITY_EDITOR
        //stop if in editor and this is a prefab in project
        if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(this))
            return;
#endif
        //make chain in play or edit
        GenerateChainRuntime();
    }

#if UNITY_EDITOR
    void GenerateChainEditor()
    {
        //stop if no spikeBall or chainPrefab
        if (spikeBall == null || chainPrefab == null) return;

        //set spikeBall position
        spikeBall.localPosition = new Vector3(spikeBallDistance, 0, 0);

        //remove old links
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child == spikeBall) continue;
            DestroyImmediate(child.gameObject);
        }

        //make new chain links
        CreateChainLinks();
        //mark object as changed
        EditorUtility.SetDirty(gameObject);
    }
#endif

    void GenerateChainRuntime()
    {
        //stop if no spikeBall or chainPrefab
        if (spikeBall == null || chainPrefab == null) return;

        //set spikeBall position
        spikeBall.localPosition = new Vector3(spikeBallDistance, 0, 0);

        //remove old links
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child == spikeBall) continue;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
#else
        Destroy(child.gameObject);
#endif
        }

        //make new chain links
        CreateChainLinks();
    }

    void CreateChainLinks()
    {
        //stop if no chainPrefab or spikeBall
        if (chainPrefab == null || spikeBall == null) return;

        //get start and end point
        Vector3 start = transform.position;
        Vector3 end = spikeBall.position;
        //get direction
        Vector3 direction = (end - start).normalized;

        //find distance
        float distance = Vector3.Distance(start, end);
        //find how many links
        int linkCount = Mathf.FloorToInt(distance / chainSpacing);

        //make links
        for (int i = 1; i < linkCount; i++)
        {
            Vector3 pos = start + direction * (i * chainSpacing);

#if UNITY_EDITOR
            Transform parentTransform = PrefabUtility.IsPartOfPrefabAsset(this) ? null : transform;
#else
            Transform parentTransform = transform;
#endif
            GameObject link = Instantiate(chainPrefab, pos, Quaternion.identity, parentTransform);
            link.transform.right = direction;
        }
    }

    private void Update()
    {
        //rotate in play
        if (Application.isPlaying)
        {
            RotateMiddlePoint();
        }
#if UNITY_EDITOR
        //rotate in edit mode if allowed
        else if (rotateInEditMode && !Application.isPlaying)
        {
            RotateMiddlePoint();
            SceneView.RepaintAll();
        }
#endif
    }

    void RotateMiddlePoint()
    {
        //set rotate direction
        float direction = rotateClockwise ? -1f : 1f;
        //rotate object
        transform.Rotate(Vector3.forward * rotationSpeed * direction * Time.deltaTime);
    }
}
