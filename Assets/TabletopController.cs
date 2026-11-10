using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class TabletopController : MonoBehaviour
{
    public static TabletopController Instance { get; private set; }
    public static bool IsTabletopMode => Instance != null && Instance.enableTabletopMode;

    [Header("Dual-Mode Settings")]
    [Tooltip("Toggle between Full VR mode (false) and Tabletop Mixed Reality mode (true).")]
    public bool enableTabletopMode = false;

    [Header("Tabletop Grid Configuration")]
    [Range(11, 32)]
    public int gridTilesX = 11;
    [Range(11, 32)]
    public int gridTilesZ = 11;

    [Tooltip("Physical size in meters of a single tile on the tabletop.")]
    [Range(0.015f, 0.15f)]
    public float tileScale = 0.04f;

    [Tooltip("Width in tiles over which tiles smoothly fade and walls taper at the inner border edge.")]
    [Range(0.1f, 2.5f)]
    public float edgeFadeTiles = 0.85f;

    [Tooltip("Smooth scrolling damping time in seconds when moving tile-by-tile.")]
    [Range(0.01f, 0.5f)]
    public float scrollSmoothTime = 0.14f;

    [Header("Tabletop UI Panel Scale")]
    [Tooltip("Scale of Input, Stats, and Text UI panels when in Tabletop Mode (Full VR is 0.01).")]
    [Range(0.0008f, 0.005f)]
    public float tabletopPanelScale = 0.0018f;

    // Runtime hierarchy references
    private World world;
    private MySmoothFollow smoothFollow;
    private Camera mainCamera;
    private XROrigin xrOrigin;

    private GameObject tabletopAnchor;
    private GameObject worldContainer;
    private GameObject borderObject;
    private MeshFilter borderMeshFilter;
    private MeshRenderer borderMeshRenderer;
    private BoxCollider[] borderColliders = new BoxCollider[4];
    private GameObject voidFloorObject;
    private GameObject northMarkerObject;

    // 4 Edge Handles for adjusting gridTilesX and gridTilesZ (North, South, East, West)
    private GameObject handleNorth;
    private GameObject handleSouth;
    private GameObject handleEast;
    private GameObject handleWest;

    // AR Foundation components for Passthrough & Meta Room Setup
    private GameObject arSessionObject;
    private ARSession arSession;
    private ARCameraManager arCameraManager;
    private ARCameraBackground arCameraBackground;
    private ARPlaneManager arPlaneManager;
    private bool hasSnappedToDetectedTable = false;

    // State tracking for mode switching & smooth scrolling
    private bool appliedTabletopMode = false;
    private int lastBuiltTilesX = -1;
    private int lastBuiltTilesZ = -1;
    private float lastBuiltTileScale = -1f;

    private Vector2 currentFocalTile = new Vector2(128f, 128f);
    private Vector2 focalVelocity = Vector2.zero;
    private U4_Decompiled_AVATAR.MODE lastGameMode = (U4_Decompiled_AVATAR.MODE)(-1);
    private int lastDungeonLevel = -1;

    // Saved Full VR state for clean restoration
    private bool savedFogState = true;
    private CameraClearFlags savedClearFlags = CameraClearFlags.Skybox;
    private Color savedBackgroundColor = Color.black;

    // Sky & Moons references for overhead Tabletop display vs Full VR restoration
    private Transform skyHorizonTransform;
    private Transform trammelSphereTransform;
    private Transform feluccaSphereTransform;
    private bool skyTransformsCached = false;
    private Vector3 savedTrammelLocalPos;
    private Vector3 savedTrammelLocalScale;
    private Vector3 savedFeluccaLocalPos;
    private Vector3 savedFeluccaLocalScale;

    // Saved Full VR UI Panel transforms
    private bool panelTransformsSaved = false;
    private Vector3 savedInputPanelLocalPos;
    private Quaternion savedInputPanelLocalRot;
    private Vector3 savedInputPanelLocalScale = new Vector3(0.01f, 0.01f, 0.01f);
    private Vector3 savedStatsPanelLocalPos;
    private Quaternion savedStatsPanelLocalRot;
    private Vector3 savedStatsPanelLocalScale = new Vector3(0.01f, 0.01f, 0.01f);
    private Vector3 savedTextPanelLocalPos;
    private Quaternion savedTextPanelLocalRot;
    private Vector3 savedTextPanelLocalScale = new Vector3(0.01f, 0.01f, 0.01f);

    // Two-handed & single-handed XR controller grab state
    private Transform leftControllerTransform;
    private Transform rightControllerTransform;
    private bool leftGripPrev = false;
    private bool rightGripPrev = false;

    private enum GrabTarget
    {
        None,
        BoardFrame,
        HandleNorth,
        HandleSouth,
        HandleEast,
        HandleWest
    }

    private GrabTarget leftGrabTarget = GrabTarget.None;
    private GrabTarget rightGrabTarget = GrabTarget.None;

    // Single-hand board grab state
    private Transform activeBoardGrabHand;
    private Vector3 boardGrabOffsetLocal;
    private float boardGrabYawOffset;

    // Two-hand pinch scale & rotate state
    private bool isTwoHandScaling = false;
    private float initialTwoHandDistance = 0.1f;
    private float initialTwoHandTileScale = 0.04f;
    private Vector3 initialTwoHandVector = Vector3.forward;
    private float initialTwoHandBoardYaw = 0f;
    private Vector3 initialTwoHandMidpointOffset = Vector3.zero;

    // Edge handle drag state
    private float handleDragStartCoord = 0f;
    private int handleDragStartTiles = 11;

    // Mode toggle button cooldown
    private bool toggleComboPrev = false;
    private Text modeButtonText;
    private GameObject snapTableButtonObj;

    // Saved XR Controller LineVisual widths for scaling pointer rays in Tabletop Mode
    private XRInteractorLineVisual[] controllerLineVisuals;
    private float[] savedLineWidths;
    private LineRenderer[] controllerLineRenderers;
    private float[] savedLineMultipliers;

    // Shader property IDs
    private static readonly int PropClipEnabled = Shader.PropertyToID("_TabletopClipEnabled");
    private static readonly int PropWorldToLocal = Shader.PropertyToID("_TabletopWorldToLocal");
    private static readonly int PropClipHalfSize = Shader.PropertyToID("_TabletopClipHalfSize");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        Shader.SetGlobalFloat(PropClipEnabled, 0f);
    }

    private void Start()
    {
        world = GetComponent<World>();
        if (world == null)
        {
            world = FindObjectOfType<World>();
        }

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            savedClearFlags = mainCamera.clearFlags;
            savedBackgroundColor = mainCamera.backgroundColor;
        }
        smoothFollow = FindObjectOfType<MySmoothFollow>();
        savedFogState = RenderSettings.fog;

        xrOrigin = FindObjectOfType<XROrigin>();
        FindControllerTransforms();
        CacheSkyTransforms();
        SaveFullVRPanelTransforms();

        CreateTabletopHierarchy();
        CreateInputPanelButtons();

        if (enableTabletopMode)
        {
            ApplyMode(true);
        }
        else
        {
            Shader.SetGlobalFloat(PropClipEnabled, 0f);
        }
    }

    private void OnDestroy()
    {
        Shader.SetGlobalFloat(PropClipEnabled, 0f);
        if (arPlaneManager != null)
        {
            arPlaneManager.planesChanged -= OnPlanesChanged;
        }
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void FindControllerTransforms()
    {
        GameObject leftObj = GameObject.Find("LeftHand Controller");
        if (leftObj != null) leftControllerTransform = leftObj.transform;

        GameObject rightObj = GameObject.Find("RightHand Controller");
        if (rightObj != null) rightControllerTransform = rightObj.transform;

        if (controllerLineVisuals == null || controllerLineVisuals.Length == 0)
        {
            controllerLineVisuals = FindObjectsOfType<XRInteractorLineVisual>(true);
            savedLineWidths = new float[controllerLineVisuals.Length];
            controllerLineRenderers = new LineRenderer[controllerLineVisuals.Length];
            savedLineMultipliers = new float[controllerLineVisuals.Length];

            for (int i = 0; i < controllerLineVisuals.Length; i++)
            {
                if (controllerLineVisuals[i] != null)
                {
                    savedLineWidths[i] = controllerLineVisuals[i].lineWidth;
                    LineRenderer lr = controllerLineVisuals[i].GetComponent<LineRenderer>();
                    controllerLineRenderers[i] = lr;
                    savedLineMultipliers[i] = lr != null ? lr.widthMultiplier : savedLineWidths[i];
                }
            }
        }
    }

    private void ScalePointerRaysForMode(bool tabletop)
    {
        if (controllerLineVisuals == null || controllerLineVisuals.Length == 0)
        {
            FindControllerTransforms();
        }
        if (controllerLineVisuals == null) return;

        // Scale the blue pointer rays proportionally to the tabletop panel scale (0.0018 / 0.01 = 0.18x)
        float rayScale = tabletop ? Mathf.Clamp(tabletopPanelScale / 0.01f, 0.08f, 1f) : 1f;

        for (int i = 0; i < controllerLineVisuals.Length; i++)
        {
            XRInteractorLineVisual lv = controllerLineVisuals[i];
            if (lv != null)
            {
                float baseWidth = (savedLineWidths != null && i < savedLineWidths.Length && savedLineWidths[i] > 0f)
                    ? savedLineWidths[i]
                    : 0.02f;
                lv.lineWidth = baseWidth * rayScale;
            }

            if (controllerLineRenderers != null && i < controllerLineRenderers.Length && controllerLineRenderers[i] != null)
            {
                float baseMult = (savedLineMultipliers != null && i < savedLineMultipliers.Length && savedLineMultipliers[i] > 0f)
                    ? savedLineMultipliers[i]
                    : 0.02f;
                controllerLineRenderers[i].widthMultiplier = baseMult * rayScale;
            }
        }
    }

    private void CacheSkyTransforms()
    {
        if (skyTransformsCached || world == null || world.skyGameObject == null) return;

        Transform skyRoot = world.skyGameObject.transform;
        skyHorizonTransform = skyRoot.Find("Horizon");
        trammelSphereTransform = skyRoot.Find("Trammel");
        feluccaSphereTransform = skyRoot.Find("Felucca");

        if (trammelSphereTransform != null)
        {
            savedTrammelLocalPos = trammelSphereTransform.localPosition;
            savedTrammelLocalScale = trammelSphereTransform.localScale;
        }
        if (feluccaSphereTransform != null)
        {
            savedFeluccaLocalPos = feluccaSphereTransform.localPosition;
            savedFeluccaLocalScale = feluccaSphereTransform.localScale;
        }

        skyTransformsCached = true;
    }

    private void SaveFullVRPanelTransforms()
    {
        if (world == null) return;
        if (world.InputPanel != null)
        {
            savedInputPanelLocalPos = world.InputPanel.transform.localPosition;
            savedInputPanelLocalRot = world.InputPanel.transform.localRotation;
            savedInputPanelLocalScale = world.InputPanel.transform.localScale;
        }
        if (world.StatsPanel != null)
        {
            savedStatsPanelLocalPos = world.StatsPanel.transform.localPosition;
            savedStatsPanelLocalRot = world.StatsPanel.transform.localRotation;
            savedStatsPanelLocalScale = world.StatsPanel.transform.localScale;
        }
        if (world.TextPanel != null)
        {
            savedTextPanelLocalPos = world.TextPanel.transform.localPosition;
            savedTextPanelLocalRot = world.TextPanel.transform.localRotation;
            savedTextPanelLocalScale = world.TextPanel.transform.localScale;
        }
        panelTransformsSaved = true;
    }

    private void ScaleAndPositionPanelsForMode(bool tabletop)
    {
        if (world == null) return;

        if (tabletop)
        {
            // Save current Full VR panel transforms before shrinking them for Tabletop
            SaveFullVRPanelTransforms();
            ArrangePanelsAroundTabletop();
        }
        else if (panelTransformsSaved)
        {
            if (world.InputPanel != null)
            {
                world.InputPanel.transform.localPosition = savedInputPanelLocalPos;
                world.InputPanel.transform.localRotation = savedInputPanelLocalRot;
                world.InputPanel.transform.localScale = savedInputPanelLocalScale;
                ZeroPanelVelocity(world.InputPanel);
            }
            if (world.StatsPanel != null)
            {
                world.StatsPanel.transform.localPosition = savedStatsPanelLocalPos;
                world.StatsPanel.transform.localRotation = savedStatsPanelLocalRot;
                world.StatsPanel.transform.localScale = savedStatsPanelLocalScale;
                ZeroPanelVelocity(world.StatsPanel);
            }
            if (world.TextPanel != null)
            {
                world.TextPanel.transform.localPosition = savedTextPanelLocalPos;
                world.TextPanel.transform.localRotation = savedTextPanelLocalRot;
                world.TextPanel.transform.localScale = savedTextPanelLocalScale;
                ZeroPanelVelocity(world.TextPanel);
            }
        }
    }

    private void ArrangePanelsAroundTabletop()
    {
        if (world == null || tabletopAnchor == null) return;
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 boardPos = tabletopAnchor.transform.position;
        Vector3 forward = tabletopAnchor.transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;
        forward.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        Vector3 viewerPos = mainCamera != null ? mainCamera.transform.position : (boardPos - forward * 0.55f + Vector3.up * 0.35f);

        float halfW = (gridTilesX * tileScale) * 0.5f + 0.05f;
        float halfD = (gridTilesZ * tileScale) * 0.5f + 0.05f;
        Vector3 scaleVec = Vector3.one * tabletopPanelScale;

        // TextPanel on the Left side of the tabletop
        if (world.TextPanel != null)
        {
            Vector3 pos = boardPos - right * (halfW + 0.22f) + forward * 0.06f + Vector3.up * 0.18f;
            world.TextPanel.transform.position = pos;
            world.TextPanel.transform.rotation = LookAtViewer(pos, viewerPos);
            world.TextPanel.transform.localScale = scaleVec;
            ZeroPanelVelocity(world.TextPanel);
        }

        // StatsPanel above/behind the North edge of the tabletop
        if (world.StatsPanel != null)
        {
            Vector3 pos = boardPos + forward * (halfD + 0.12f) + Vector3.up * 0.24f;
            world.StatsPanel.transform.position = pos;
            world.StatsPanel.transform.rotation = LookAtViewer(pos, viewerPos);
            world.StatsPanel.transform.localScale = scaleVec;
            ZeroPanelVelocity(world.StatsPanel);
        }

        // InputPanel on the Right side of the tabletop within easy reach
        if (world.InputPanel != null)
        {
            Vector3 pos = boardPos + right * (halfW + 0.26f) + forward * 0.02f + Vector3.up * 0.18f;
            world.InputPanel.transform.position = pos;
            world.InputPanel.transform.rotation = LookAtViewer(pos, viewerPos);
            world.InputPanel.transform.localScale = scaleVec;
            ZeroPanelVelocity(world.InputPanel);
        }
    }

    private Quaternion LookAtViewer(Vector3 panelPos, Vector3 viewerPos)
    {
        Vector3 dir = panelPos - viewerPos;
        if (dir.sqrMagnitude < 0.001f) dir = Vector3.forward;
        return Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    private void ZeroPanelVelocity(GameObject panel)
    {
        if (panel == null) return;
        Rigidbody rb = panel.GetComponent<Rigidbody>();
        if (rb != null && !rb.isKinematic)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void CreateTabletopHierarchy()
    {
        if (tabletopAnchor != null) return;

        tabletopAnchor = new GameObject("TabletopAnchor");
        tabletopAnchor.transform.position = new Vector3(0f, 0.75f, 0.65f);
        tabletopAnchor.transform.rotation = Quaternion.identity;

        worldContainer = new GameObject("TabletopWorldContainer");
        worldContainer.transform.SetParent(tabletopAnchor.transform, false);
        worldContainer.transform.localPosition = new Vector3(0f, 0.5f * tileScale, 0f);
        worldContainer.transform.localRotation = Quaternion.identity;
        worldContainer.transform.localScale = Vector3.one * tileScale;

        // Opaque pitch-black void floor plate inside the border
        voidFloorObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        voidFloorObject.name = "TabletopVoidFloor";
        voidFloorObject.transform.SetParent(tabletopAnchor.transform, false);
        voidFloorObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        Collider voidCol = voidFloorObject.GetComponent<Collider>();
        if (voidCol != null) Destroy(voidCol);

        Material voidMat = new Material(Shader.Find("Unlit/Color") ?? Shader.Find("Standard"));
        voidMat.color = new Color(0f, 0f, 0f, 1f);
        voidFloorObject.GetComponent<MeshRenderer>().sharedMaterial = voidMat;

        // Procedural stylized wooden/stone border
        borderObject = new GameObject("TabletopBorder");
        borderObject.transform.SetParent(tabletopAnchor.transform, false);
        borderObject.transform.localPosition = Vector3.zero;
        borderObject.transform.localRotation = Quaternion.identity;

        borderMeshFilter = borderObject.AddComponent<MeshFilter>();
        borderMeshRenderer = borderObject.AddComponent<MeshRenderer>();

        Material borderMat = CreateStylizedBorderMaterial();
        borderMeshRenderer.sharedMaterial = borderMat;

        for (int i = 0; i < 4; i++)
        {
            borderColliders[i] = borderObject.AddComponent<BoxCollider>();
        }

        // Compass "N" label on the North rim
        northMarkerObject = new GameObject("NorthCompassMarker");
        northMarkerObject.transform.SetParent(borderObject.transform, false);
        TextMesh tm = northMarkerObject.AddComponent<TextMesh>();
        tm.text = "N";
        tm.characterSize = 0.012f;
        tm.fontSize = 64;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = new Color(0.95f, 0.82f, 0.42f, 1f);

        // Create 4 dedicated tile-count grab handles on North, South, East, West edges
        handleNorth = CreateEdgeHandle("Handle_North_Z", new Color(0.85f, 0.68f, 0.32f, 1f));
        handleSouth = CreateEdgeHandle("Handle_South_Z", new Color(0.85f, 0.68f, 0.32f, 1f));
        handleEast = CreateEdgeHandle("Handle_East_X", new Color(0.85f, 0.68f, 0.32f, 1f));
        handleWest = CreateEdgeHandle("Handle_West_X", new Color(0.85f, 0.68f, 0.32f, 1f));

        RebuildBorderAndFloor();
        tabletopAnchor.SetActive(false);
    }

    private GameObject CreateEdgeHandle(string handleName, Color handleColor)
    {
        GameObject h = GameObject.CreatePrimitive(PrimitiveType.Cube);
        h.name = handleName;
        h.transform.SetParent(borderObject.transform, false);
        MeshRenderer mr = h.GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Standard") ?? Shader.Find("Unlit/Color"));
        mat.color = handleColor;
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.35f);
        mr.sharedMaterial = mat;
        return h;
    }

    private Material CreateStylizedBorderMaterial()
    {
        // Procedural wood/stone grain texture
        int texSize = 64;
        Texture2D tex = new Texture2D(texSize, texSize, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        tex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < texSize; y++)
        {
            for (int x = 0; x < texSize; x++)
            {
                float grain = Mathf.PerlinNoise(x * 0.12f, y * 0.45f) * 0.18f
                            + Mathf.PerlinNoise(x * 0.35f, y * 0.08f) * 0.08f;
                Color baseWood = Color.Lerp(new Color(0.32f, 0.20f, 0.10f, 1f), new Color(0.48f, 0.31f, 0.16f, 1f), grain / 0.26f);
                tex.SetPixel(x, y, baseWood);
            }
        }
        tex.Apply();

        Shader sh = Shader.Find("Standard") ?? Shader.Find("Legacy Shaders/Diffuse") ?? Shader.Find("Unlit/Texture");
        Material mat = new Material(sh);
        mat.mainTexture = tex;
        mat.color = Color.white;
        if (mat.HasProperty("_Glossiness"))
        {
            mat.SetFloat("_Glossiness", 0.25f);
        }
        return mat;
    }

    private void RebuildBorderAndFloor()
    {
        gridTilesX = Mathf.Clamp(gridTilesX, 11, 32);
        gridTilesZ = Mathf.Clamp(gridTilesZ, 11, 32);
        tileScale = Mathf.Clamp(tileScale, 0.015f, 0.15f);

        lastBuiltTilesX = gridTilesX;
        lastBuiltTilesZ = gridTilesZ;
        lastBuiltTileScale = tileScale;

        float innerHalfX = (gridTilesX * tileScale) * 0.5f;
        float innerHalfZ = (gridTilesZ * tileScale) * 0.5f;

        // Update opaque pitch-black void floor plate just under y = 0
        if (voidFloorObject != null)
        {
            voidFloorObject.transform.localPosition = new Vector3(0f, -0.002f, 0f);
            voidFloorObject.transform.localScale = new Vector3(innerHalfX * 2f + 0.004f, innerHalfZ * 2f + 0.004f, 1f);
        }

        // Border dimensions
        float borderWidth = Mathf.Clamp(tileScale * 0.9f, 0.028f, 0.065f);
        float borderHeight = Mathf.Clamp(tileScale * 0.42f, 0.014f, 0.032f);
        float bevelInset = borderWidth * 0.22f;
        float outerHalfX = innerHalfX + borderWidth;
        float outerHalfZ = innerHalfZ + borderWidth;

        Mesh mesh = BuildBeveledFrameMesh(innerHalfX, innerHalfZ, outerHalfX, outerHalfZ, borderHeight, bevelInset);
        if (borderMeshFilter != null)
        {
            if (borderMeshFilter.sharedMesh != null)
            {
                Destroy(borderMeshFilter.sharedMesh);
            }
            borderMeshFilter.sharedMesh = mesh;
        }

        // Update 4 BoxColliders on North, South, East, West bars for easy grabbing
        if (borderColliders != null && borderColliders.Length == 4)
        {
            // North (+Z)
            borderColliders[0].center = new Vector3(0f, borderHeight * 0.4f, innerHalfZ + borderWidth * 0.5f);
            borderColliders[0].size = new Vector3(outerHalfX * 2f, borderHeight * 1.4f, borderWidth);
            // South (-Z)
            borderColliders[1].center = new Vector3(0f, borderHeight * 0.4f, -(innerHalfZ + borderWidth * 0.5f));
            borderColliders[1].size = new Vector3(outerHalfX * 2f, borderHeight * 1.4f, borderWidth);
            // East (+X)
            borderColliders[2].center = new Vector3(innerHalfX + borderWidth * 0.5f, borderHeight * 0.4f, 0f);
            borderColliders[2].size = new Vector3(borderWidth, borderHeight * 1.4f, innerHalfZ * 2f);
            // West (-X)
            borderColliders[3].center = new Vector3(-(innerHalfX + borderWidth * 0.5f), borderHeight * 0.4f, 0f);
            borderColliders[3].size = new Vector3(borderWidth, borderHeight * 1.4f, innerHalfZ * 2f);
        }

        // Position North compass marker on the North border bar
        if (northMarkerObject != null)
        {
            northMarkerObject.transform.localPosition = new Vector3(0f, borderHeight + 0.002f, innerHalfZ + borderWidth * 0.5f);
            northMarkerObject.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            float markerScale = Mathf.Clamp(borderWidth * 0.85f, 0.02f, 0.06f) / 0.035f;
            northMarkerObject.transform.localScale = Vector3.one * markerScale;
        }

        // Position the 4 tile-count adjustment handles outside the North, South, East, West edges
        float handleLength = Mathf.Clamp(borderWidth * 2.2f, 0.06f, 0.14f);
        float handleThickness = Mathf.Clamp(borderWidth * 0.45f, 0.014f, 0.028f);
        float handleOffset = borderWidth + handleThickness * 0.65f;

        if (handleNorth != null)
        {
            handleNorth.transform.localPosition = new Vector3(0f, borderHeight * 0.5f, innerHalfZ + handleOffset);
            handleNorth.transform.localScale = new Vector3(handleLength, borderHeight * 0.9f, handleThickness);
        }
        if (handleSouth != null)
        {
            handleSouth.transform.localPosition = new Vector3(0f, borderHeight * 0.5f, -(innerHalfZ + handleOffset));
            handleSouth.transform.localScale = new Vector3(handleLength, borderHeight * 0.9f, handleThickness);
        }
        if (handleEast != null)
        {
            handleEast.transform.localPosition = new Vector3(innerHalfX + handleOffset, borderHeight * 0.5f, 0f);
            handleEast.transform.localScale = new Vector3(handleThickness, borderHeight * 0.9f, handleLength);
        }
        if (handleWest != null)
        {
            handleWest.transform.localPosition = new Vector3(-(innerHalfX + handleOffset), borderHeight * 0.5f, 0f);
            handleWest.transform.localScale = new Vector3(handleThickness, borderHeight * 0.9f, handleLength);
        }
    }

    private Mesh BuildBeveledFrameMesh(float inX, float inZ, float outX, float outZ, float height, float bevel)
    {
        Mesh m = new Mesh();
        m.name = "ProceduralTabletopBorder";

        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        float botY = -0.004f;
        float topY = height;

        // 4 mitered trapezoidal sides: North (0), East (1), South (2), West (3)
        Vector2[] inCorners = new Vector2[]
        {
            new Vector2(-inX,  inZ), // NW
            new Vector2( inX,  inZ), // NE
            new Vector2( inX, -inZ), // SE
            new Vector2(-inX, -inZ)  // SW
        };
        Vector2[] inTopCorners = new Vector2[]
        {
            new Vector2(-(inX + bevel),  (inZ + bevel)),
            new Vector2( (inX + bevel),  (inZ + bevel)),
            new Vector2( (inX + bevel), -(inZ + bevel)),
            new Vector2(-(inX + bevel), -(inZ + bevel))
        };
        Vector2[] outTopCorners = new Vector2[]
        {
            new Vector2(-(outX - bevel),  (outZ - bevel)),
            new Vector2( (outX - bevel),  (outZ - bevel)),
            new Vector2( (outX - bevel), -(outZ - bevel)),
            new Vector2(-(outX - bevel), -(outZ - bevel))
        };
        Vector2[] outCorners = new Vector2[]
        {
            new Vector2(-outX,  outZ),
            new Vector2( outX,  outZ),
            new Vector2( outX, -outZ),
            new Vector2(-outX, -outZ)
        };

        for (int side = 0; side < 4; side++)
        {
            int next = (side + 1) % 4;

            Vector3 ib0 = new Vector3(inCorners[side].x, botY, inCorners[side].y);
            Vector3 ib1 = new Vector3(inCorners[next].x, botY, inCorners[next].y);

            Vector3 it0 = new Vector3(inTopCorners[side].x, topY, inTopCorners[side].y);
            Vector3 it1 = new Vector3(inTopCorners[next].x, topY, inTopCorners[next].y);

            Vector3 ot0 = new Vector3(outTopCorners[side].x, topY, outTopCorners[side].y);
            Vector3 ot1 = new Vector3(outTopCorners[next].x, topY, outTopCorners[next].y);

            Vector3 ob0 = new Vector3(outCorners[side].x, botY, outCorners[side].y);
            Vector3 ob1 = new Vector3(outCorners[next].x, botY, outCorners[next].y);

            // Inner bevel slope (facing inward/upward toward the map)
            AddQuad(verts, uvs, tris, ib0, ib1, it1, it0);
            // Top flat rim (facing upward +Y)
            AddQuad(verts, uvs, tris, it0, it1, ot1, ot0);
            // Outer bevel slope (facing outward/upward)
            AddQuad(verts, uvs, tris, ot0, ot1, ob1, ob0);
            // Bottom underside (facing downward -Y)
            AddQuad(verts, uvs, tris, ob0, ob1, ib1, ib0);
        }

        m.SetVertices(verts);
        m.SetUVs(0, uvs);
        m.SetTriangles(tris, 0);
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    private void AddQuad(List<Vector3> verts, List<Vector2> uvs, List<int> tris, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int start = verts.Count;
        verts.Add(v0);
        verts.Add(v1);
        verts.Add(v2);
        verts.Add(v3);

        float len = Vector3.Distance(v0, v1) * 4f;
        uvs.Add(new Vector2(0f, 0f));
        uvs.Add(new Vector2(len, 0f));
        uvs.Add(new Vector2(len, 1f));
        uvs.Add(new Vector2(0f, 1f));

        // Clockwise winding in Unity's left-handed coordinate system so normals point outward/upward
        tris.Add(start + 0);
        tris.Add(start + 2);
        tris.Add(start + 1);

        tris.Add(start + 0);
        tris.Add(start + 3);
        tris.Add(start + 2);
    }

    private void CreateInputPanelButtons()
    {
        if (world == null || world.InputPanel == null) return;

        // Find an existing button on InputPanel (e.g. "Quit" or "Pass") to clone its styling and font
        Button templateBtn = null;
        Button[] allButtons = world.InputPanel.GetComponentsInChildren<Button>(true);
        foreach (Button b in allButtons)
        {
            if (b != null && (b.name == "Quit" || b.name == "Pass"))
            {
                templateBtn = b;
                break;
            }
        }
        if (templateBtn == null && allButtons.Length > 0)
        {
            templateBtn = allButtons[0];
        }
        if (templateBtn == null) return;

        Transform panelRoot = world.InputPanel.transform;

        // 1. Mode toggle button ("TABLETOP" / "FULL VR") placed at top-left of Input Panel
        GameObject toggleObj = Instantiate(templateBtn.gameObject, panelRoot, false);
        toggleObj.name = "TABLETOP_MODE_BUTTON";
        toggleObj.SetActive(true);

        RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
        if (toggleRect != null)
        {
            toggleRect.anchorMin = new Vector2(0.5f, 0.5f);
            toggleRect.anchorMax = new Vector2(0.5f, 0.5f);
            toggleRect.pivot = new Vector2(0.5f, 0.5f);
            toggleRect.sizeDelta = new Vector2(115f, 22f);
            toggleRect.anchoredPosition = new Vector2(-68f, 138f);
            toggleRect.localScale = Vector3.one;
            toggleRect.localRotation = Quaternion.identity;
        }

        Clicked2 oldClick2 = toggleObj.GetComponent<Clicked2>();
        if (oldClick2 != null) Destroy(oldClick2);
        Clicked oldClick = toggleObj.GetComponent<Clicked>();
        if (oldClick != null) Destroy(oldClick);

        modeButtonText = toggleObj.GetComponentInChildren<Text>(true);
        if (modeButtonText != null)
        {
            if (world.myTransparentFont != null) modeButtonText.font = world.myTransparentFont;
            modeButtonText.text = enableTabletopMode ? "Full VR" : "Tabletop";
        }

        Button toggleBtn = toggleObj.GetComponent<Button>();
        if (toggleBtn != null)
        {
            toggleBtn.onClick = new Button.ButtonClickedEvent();
            toggleBtn.onClick.AddListener(ToggleTabletopMode);
        }

        // 2. Re-anchor to Table button ("Restore") placed at top-right of Input Panel
        snapTableButtonObj = Instantiate(templateBtn.gameObject, panelRoot, false);
        snapTableButtonObj.name = "RESTORE_TABLE_BUTTON";

        RectTransform snapRect = snapTableButtonObj.GetComponent<RectTransform>();
        if (snapRect != null)
        {
            snapRect.anchorMin = new Vector2(0.5f, 0.5f);
            snapRect.anchorMax = new Vector2(0.5f, 0.5f);
            snapRect.pivot = new Vector2(0.5f, 0.5f);
            snapRect.sizeDelta = new Vector2(115f, 22f);
            snapRect.anchoredPosition = new Vector2(68f, 138f);
            snapRect.localScale = Vector3.one;
            snapRect.localRotation = Quaternion.identity;
        }

        Clicked2 oldSnapClick2 = snapTableButtonObj.GetComponent<Clicked2>();
        if (oldSnapClick2 != null) Destroy(oldSnapClick2);
        Clicked oldSnapClick = snapTableButtonObj.GetComponent<Clicked>();
        if (oldSnapClick != null) Destroy(oldSnapClick);

        Text snapText = snapTableButtonObj.GetComponentInChildren<Text>(true);
        if (snapText != null)
        {
            if (world.myTransparentFont != null) snapText.font = world.myTransparentFont;
            snapText.text = "Restore";
        }

        Button snapBtn = snapTableButtonObj.GetComponent<Button>();
        if (snapBtn != null)
        {
            snapBtn.onClick = new Button.ButtonClickedEvent();
            snapBtn.onClick.AddListener(SnapToPrimaryTableOrFront);
        }

        snapTableButtonObj.SetActive(enableTabletopMode);
    }

    public void ToggleTabletopMode()
    {
        enableTabletopMode = !enableTabletopMode;
        ApplyMode(enableTabletopMode);
    }

    public void SetTabletopMode(bool tabletop)
    {
        if (enableTabletopMode == tabletop && appliedTabletopMode == tabletop) return;
        enableTabletopMode = tabletop;
        ApplyMode(enableTabletopMode);
    }

    private void ApplyMode(bool tabletop)
    {
        appliedTabletopMode = tabletop;

        if (modeButtonText != null)
        {
            modeButtonText.text = tabletop ? "Full VR" : "Tabletop";
        }
        if (snapTableButtonObj != null)
        {
            snapTableButtonObj.SetActive(tabletop);
        }

        if (tabletop)
        {
            // Save current Full VR state
            savedFogState = RenderSettings.fog;
            RenderSettings.fog = false;

            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera != null)
            {
                savedClearFlags = mainCamera.clearFlags;
                savedBackgroundColor = mainCamera.backgroundColor;
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            }

            // Keep XR Origin at its current position so the player and UI panels don't get teleported far away
            if (world != null && world.u4 != null)
            {
                world.u4.surface_party_direction = U4_Decompiled_AVATAR.DIRECTION.NORTH;
            }

            // Enable OpenXR Passthrough & Meta Room Setup ARPlaneManager
            EnablePassthroughAndRoomSetup(true);

            // Activate TabletopAnchor & parent world geometry into WorldContainer
            if (tabletopAnchor != null)
            {
                tabletopAnchor.SetActive(true);
            }

            ReparentWorldObjectsToTabletop(true);
            RebuildBorderAndFloor();

            // Snap to a nearby detected Meta Room Setup TABLE plane, or spawn right in front of the viewer
            if (!TrySnapToDetectedTablePlane())
            {
                PositionBoardInFrontOfPlayer();
            }

            // Scale down UI panels and controller pointer rays to tabletop scale
            ScaleAndPositionPanelsForMode(true);
            ScalePointerRaysForMode(true);

            // Initialize focal point immediately so it doesn't sweep from (0,0)
            currentFocalTile = GetTargetFocalTile();
            focalVelocity = Vector2.zero;
            UpdateWorldContainerTransform();
            UpdateShaderClippingUniforms();
            UpdateOverheadMoonsAndSky();

            if (world != null)
            {
                world.lastModeCheck = (U4_Decompiled_AVATAR.MODE)(-1);
            }
        }
        else
        {
            // Disable Tabletop clipping
            Shader.SetGlobalFloat(PropClipEnabled, 0f);

            // Restore world objects back to World root at 1:1 scale
            ReparentWorldObjectsToTabletop(false);

            if (tabletopAnchor != null)
            {
                tabletopAnchor.SetActive(false);
            }

            // Disable Passthrough & ARPlaneManager
            EnablePassthroughAndRoomSetup(false);

            // Restore Full VR camera and fog settings
            RenderSettings.fog = savedFogState;
            if (mainCamera != null)
            {
                mainCamera.clearFlags = savedClearFlags;
                mainCamera.backgroundColor = savedBackgroundColor;
            }

            // Restore UI panels and controller pointer rays to Full VR scale and positions
            ScaleAndPositionPanelsForMode(false);
            ScalePointerRaysForMode(false);

            // Restore sky, horizon, and moon transforms for Full VR
            RestoreSkyForFullVR();

            // Force World.Update() to immediately regenerate Full VR terrain meshes
            if (world != null)
            {
                world.lastModeCheck = (U4_Decompiled_AVATAR.MODE)(-1);
            }
        }
    }

    private void EnablePassthroughAndRoomSetup(bool enable)
    {
        if (enable)
        {
            if (arSession == null)
            {
                arSession = FindObjectOfType<ARSession>();
                if (arSession == null)
                {
                    arSessionObject = new GameObject("TabletopARSession");
                    arSession = arSessionObject.AddComponent<ARSession>();
                }
            }
            if (arSession != null) arSession.enabled = true;

            if (mainCamera != null)
            {
                arCameraManager = mainCamera.GetComponent<ARCameraManager>();
                if (arCameraManager == null)
                {
                    arCameraManager = mainCamera.gameObject.AddComponent<ARCameraManager>();
                }
                arCameraManager.enabled = true;

                arCameraBackground = mainCamera.GetComponent<ARCameraBackground>();
                if (arCameraBackground == null)
                {
                    arCameraBackground = mainCamera.gameObject.AddComponent<ARCameraBackground>();
                }
                arCameraBackground.enabled = true;
            }

            if (xrOrigin == null) xrOrigin = FindObjectOfType<XROrigin>();
            if (xrOrigin != null)
            {
                arPlaneManager = xrOrigin.GetComponent<ARPlaneManager>();
                if (arPlaneManager == null)
                {
                    arPlaneManager = xrOrigin.gameObject.AddComponent<ARPlaneManager>();
                }
                arPlaneManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
                arPlaneManager.planesChanged -= OnPlanesChanged;
                arPlaneManager.planesChanged += OnPlanesChanged;
                arPlaneManager.enabled = true;
            }
        }
        else
        {
            if (arPlaneManager != null)
            {
                arPlaneManager.planesChanged -= OnPlanesChanged;
                arPlaneManager.enabled = false;
            }
            if (arCameraBackground != null) arCameraBackground.enabled = false;
            if (arCameraManager != null) arCameraManager.enabled = false;
            if (arSession != null) arSession.enabled = false;
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (!enableTabletopMode) return;

        // Hide default debug plane visuals on ARPlanes while using their poses/classifications
        foreach (ARPlane plane in args.added)
        {
            ConfigureDetectedPlane(plane);
        }
        foreach (ARPlane plane in args.updated)
        {
            ConfigureDetectedPlane(plane);
        }

        if (!hasSnappedToDetectedTable)
        {
            if (TrySnapToDetectedTablePlane())
            {
                ArrangePanelsAroundTabletop();
            }
        }
    }

    private void ConfigureDetectedPlane(ARPlane plane)
    {
        if (plane == null) return;
        MeshRenderer mr = plane.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;
        LineRenderer lr = plane.GetComponent<LineRenderer>();
        if (lr != null) lr.enabled = false;
    }

    public void SnapToPrimaryTableOrFront()
    {
        hasSnappedToDetectedTable = false;
        if (!TrySnapToDetectedTablePlane())
        {
            PositionBoardInFrontOfPlayer();
        }
        ArrangePanelsAroundTabletop();
    }

    private bool TrySnapToDetectedTablePlane()
    {
        if (arPlaneManager == null || tabletopAnchor == null) return false;
        if (mainCamera == null) mainCamera = Camera.main;

        Vector3 camPos = mainCamera != null ? mainCamera.transform.position : Vector3.zero;

        ARPlane bestTablePlane = null;
        float bestScore = float.MinValue;

        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            if (plane == null || plane.trackingState == TrackingState.None) continue;

            // Only snap to an explicitly classified Meta Room Setup TABLE surface near the viewer
            if (plane.classification != PlaneClassification.Table) continue;

            Vector3 center = plane.center;
            float horizDist = Vector2.Distance(new Vector2(center.x, center.z), new Vector2(camPos.x, camPos.z));
            float vertDist = Mathf.Abs(center.y - camPos.y);

            // Must be within reach of the viewer (not far away in another coordinate space)
            if (mainCamera != null && (horizDist > 1.8f || vertDist > 1.3f)) continue;

            float area = plane.size.x * plane.size.y;
            float score = area - horizDist * 0.5f;
            if (score > bestScore)
            {
                bestScore = score;
                bestTablePlane = plane;
            }
        }

        if (bestTablePlane == null) return false;

        Vector3 tableCenter = bestTablePlane.center;
        tabletopAnchor.transform.position = new Vector3(tableCenter.x, bestTablePlane.transform.position.y + 0.005f, tableCenter.z);

        // Orient the board's South edge toward the player
        if (mainCamera != null)
        {
            Vector3 toBoard = tabletopAnchor.transform.position - mainCamera.transform.position;
            toBoard.y = 0f;
            if (toBoard.sqrMagnitude > 0.01f)
            {
                tabletopAnchor.transform.rotation = Quaternion.LookRotation(toBoard.normalized, Vector3.up);
            }
        }

        hasSnappedToDetectedTable = true;
        return true;
    }

    private void PositionBoardInFrontOfPlayer()
    {
        if (tabletopAnchor == null) return;
        if (mainCamera == null) mainCamera = Camera.main;

        if (mainCamera != null)
        {
            Vector3 forward = mainCamera.transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;
            forward.Normalize();

            Vector3 camPos = mainCamera.transform.position;
            // Place directly in front of and slightly below the viewer's headset (no absolute world-Y clamp!)
            tabletopAnchor.transform.position = camPos + forward * 0.52f + Vector3.down * 0.30f;
            tabletopAnchor.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
        else
        {
            tabletopAnchor.transform.position = new Vector3(0f, 0.75f, 0.55f);
            tabletopAnchor.transform.rotation = Quaternion.identity;
        }
    }

    public void ReparentWorldObjectsToTabletop(bool toTabletop)
    {
        if (world == null) world = FindObjectOfType<World>();
        if (world == null || worldContainer == null) return;

        Transform targetParent = toTabletop ? worldContainer.transform : world.transform;

        // Reparent all direct map/entity children of World
        List<Transform> childrenToMove = new List<Transform>();
        if (toTabletop)
        {
            for (int i = 0; i < world.transform.childCount; i++)
            {
                Transform child = world.transform.GetChild(i);
                if (child == null) continue;
                if (child.gameObject == world.skyGameObject) continue;
                if (child.gameObject == tabletopAnchor) continue;
                childrenToMove.Add(child);
            }
        }
        else
        {
            for (int i = 0; i < worldContainer.transform.childCount; i++)
            {
                Transform child = worldContainer.transform.GetChild(i);
                if (child != null) childrenToMove.Add(child);
            }
        }

        foreach (Transform child in childrenToMove)
        {
            child.SetParent(targetParent, false);
        }

        // Also handle objects that World or Combat created
        if (world.mainTerrain != null)
        {
            world.mainTerrain.transform.SetParent(targetParent, false);
        }
        if (world.party != null)
        {
            world.party.transform.SetParent(targetParent, false);
        }
        if (world.dungeon != null)
        {
            world.dungeon.transform.SetParent(targetParent, false);
        }
        if (Combat.combatTerrainsRoot != null)
        {
            Combat.combatTerrainsRoot.transform.SetParent(targetParent, false);
        }
    }

    private void Update()
    {
        // Support toggling via Inspector checkbox or F2 key in Editor
        if (enableTabletopMode != appliedTabletopMode)
        {
            ApplyMode(enableTabletopMode);
        }

        var kb = Keyboard.current;
        if (kb != null && kb.f2Key.wasPressedThisFrame)
        {
            ToggleTabletopMode();
        }

        // Check controller combo (both thumbstick clicks or Left Menu button) to toggle Tabletop Mode
        CheckControllerModeToggle();

        if (!enableTabletopMode)
        {
            Shader.SetGlobalFloat(PropClipEnabled, 0f);
            return;
        }

        // Keep camera clearFlags transparent for passthrough while in Tabletop mode
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        }

        // Keyboard shortcuts for testing grid size & scale in Editor
        HandleKeyboardShortcuts();

        // Handle XR controller interactions (1-hand board move, 2-hand pinch scale & rotate, 4 edge tile-count handles)
        HandleXRTabletopInteractions();

        // Rebuild border if grid dimensions or tileScale changed
        if (gridTilesX != lastBuiltTilesX || gridTilesZ != lastBuiltTilesZ || Mathf.Abs(tileScale - lastBuiltTileScale) > 0.0002f)
        {
            RebuildBorderAndFloor();
        }

        // Ensure newly spawned objects (e.g. dungeon level change or tile graphic swap) are parented to WorldContainer
        EnsureDynamicObjectsParented();

        // Smoothly scroll WorldContainer to keep the current game focal point centered in the tabletop grid
        UpdateWorldContainerTransform();

        // Update shader clipping & smooth edge fade uniforms
        UpdateShaderClippingUniforms();

        // Keep moons & sun positioned high overhead in the sky at original scale
        UpdateOverheadMoonsAndSky();
    }

    private void CheckControllerModeToggle()
    {
        UnityEngine.XR.InputDevice leftDev = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        UnityEngine.XR.InputDevice rightDev = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool leftStickClick = false;
        bool rightStickClick = false;
        bool menuBtn = false;

        if (leftDev.isValid)
        {
            leftDev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out leftStickClick);
            leftDev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.menuButton, out menuBtn);
        }
        if (rightDev.isValid)
        {
            rightDev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out rightStickClick);
        }

        bool comboPressed = menuBtn || (leftStickClick && rightStickClick);
        if (comboPressed && !toggleComboPrev)
        {
            ToggleTabletopMode();
        }
        toggleComboPrev = comboPressed;
    }

    private void HandleKeyboardShortcuts()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.leftBracketKey.wasPressedThisFrame) gridTilesX = Mathf.Max(11, gridTilesX - 1);
        if (kb.rightBracketKey.wasPressedThisFrame) gridTilesX = Mathf.Min(32, gridTilesX + 1);
        if (kb.semicolonKey.wasPressedThisFrame) gridTilesZ = Mathf.Max(11, gridTilesZ - 1);
        if (kb.quoteKey.wasPressedThisFrame) gridTilesZ = Mathf.Min(32, gridTilesZ + 1);
        if (kb.minusKey.wasPressedThisFrame || kb.numpadMinusKey.wasPressedThisFrame)
            tileScale = Mathf.Clamp(tileScale - 0.005f, 0.015f, 0.15f);
        if (kb.equalsKey.wasPressedThisFrame || kb.numpadPlusKey.wasPressedThisFrame)
            tileScale = Mathf.Clamp(tileScale + 0.005f, 0.015f, 0.15f);
        if (kb.f3Key.wasPressedThisFrame)
            SnapToPrimaryTableOrFront();
    }

    private void HandleXRTabletopInteractions()
    {
        if (leftControllerTransform == null || rightControllerTransform == null)
        {
            FindControllerTransforms();
        }

        UnityEngine.XR.InputDevice leftDev = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        UnityEngine.XR.InputDevice rightDev = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool leftGrip = false;
        bool rightGrip = false;
        if (leftDev.isValid) leftDev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out leftGrip);
        if (rightDev.isValid) rightDev.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out rightGrip);

        // Left Grip Down
        if (leftGrip && !leftGripPrev && leftControllerTransform != null)
        {
            leftGrabTarget = HitTestTabletopControls(leftControllerTransform);
            OnHandGrabStarted(leftControllerTransform, leftGrabTarget);
        }
        // Left Grip Up
        else if (!leftGrip && leftGripPrev)
        {
            leftGrabTarget = GrabTarget.None;
            isTwoHandScaling = false;
            if (activeBoardGrabHand == leftControllerTransform) activeBoardGrabHand = null;
        }

        // Right Grip Down
        if (rightGrip && !rightGripPrev && rightControllerTransform != null)
        {
            rightGrabTarget = HitTestTabletopControls(rightControllerTransform);
            OnHandGrabStarted(rightControllerTransform, rightGrabTarget);
        }
        // Right Grip Up
        else if (!rightGrip && rightGripPrev)
        {
            rightGrabTarget = GrabTarget.None;
            isTwoHandScaling = false;
            if (activeBoardGrabHand == rightControllerTransform) activeBoardGrabHand = null;
        }

        leftGripPrev = leftGrip;
        rightGripPrev = rightGrip;

        // Check if both hands are grabbing the board frame -> Two-handed pinch scaling & yaw rotation
        if (leftGrabTarget == GrabTarget.BoardFrame && rightGrabTarget == GrabTarget.BoardFrame &&
            leftControllerTransform != null && rightControllerTransform != null)
        {
            if (!isTwoHandScaling)
            {
                isTwoHandScaling = true;
                activeBoardGrabHand = null;
                Vector3 pL = leftControllerTransform.position;
                Vector3 pR = rightControllerTransform.position;
                initialTwoHandDistance = Mathf.Max(0.05f, Vector3.Distance(pL, pR));
                initialTwoHandTileScale = tileScale;

                Vector3 vec = pR - pL;
                vec.y = 0f;
                initialTwoHandVector = vec.sqrMagnitude > 0.0001f ? vec.normalized : Vector3.right;
                initialTwoHandBoardYaw = tabletopAnchor.transform.eulerAngles.y;

                Vector3 mid = (pL + pR) * 0.5f;
                initialTwoHandMidpointOffset = tabletopAnchor.transform.position - mid;
            }
            else
            {
                Vector3 pL = leftControllerTransform.position;
                Vector3 pR = rightControllerTransform.position;
                float dist = Mathf.Max(0.05f, Vector3.Distance(pL, pR));
                float scaleFactor = dist / initialTwoHandDistance;
                tileScale = Mathf.Clamp(initialTwoHandTileScale * scaleFactor, 0.015f, 0.15f);

                Vector3 currentVec = pR - pL;
                currentVec.y = 0f;
                if (currentVec.sqrMagnitude > 0.0001f)
                {
                    float deltaYaw = Vector3.SignedAngle(initialTwoHandVector, currentVec.normalized, Vector3.up);
                    tabletopAnchor.transform.rotation = Quaternion.Euler(0f, initialTwoHandBoardYaw + deltaYaw, 0f);
                }

                Vector3 mid = (pL + pR) * 0.5f;
                tabletopAnchor.transform.position = mid + initialTwoHandMidpointOffset;
            }
            return;
        }

        // Single-hand board reposition
        if (activeBoardGrabHand != null &&
            ((activeBoardGrabHand == leftControllerTransform && leftGrabTarget == GrabTarget.BoardFrame) ||
             (activeBoardGrabHand == rightControllerTransform && rightGrabTarget == GrabTarget.BoardFrame)))
        {
            float handYaw = activeBoardGrabHand.eulerAngles.y;
            Quaternion targetRot = Quaternion.Euler(0f, handYaw + boardGrabYawOffset, 0f);
            tabletopAnchor.transform.rotation = targetRot;
            tabletopAnchor.transform.position = activeBoardGrabHand.position + targetRot * boardGrabOffsetLocal;
        }

        // Dedicated Edge Handles for horizontal (gridTilesX) and vertical (gridTilesZ) tile counts
        if (leftControllerTransform != null && IsEdgeHandle(leftGrabTarget))
        {
            UpdateEdgeHandleDrag(leftControllerTransform, leftGrabTarget);
        }
        else if (rightControllerTransform != null && IsEdgeHandle(rightGrabTarget))
        {
            UpdateEdgeHandleDrag(rightControllerTransform, rightGrabTarget);
        }
    }

    private bool IsEdgeHandle(GrabTarget target)
    {
        return target == GrabTarget.HandleNorth ||
               target == GrabTarget.HandleSouth ||
               target == GrabTarget.HandleEast ||
               target == GrabTarget.HandleWest;
    }

    private GrabTarget HitTestTabletopControls(Transform hand)
    {
        if (hand == null || tabletopAnchor == null) return GrabTarget.None;

        float directRadius = 0.09f;
        Vector3 handPos = hand.position;

        // 1. Check direct proximity to the 4 tile-count edge handles first
        if (handleNorth != null && Vector3.Distance(handPos, handleNorth.transform.position) < directRadius)
            return GrabTarget.HandleNorth;
        if (handleSouth != null && Vector3.Distance(handPos, handleSouth.transform.position) < directRadius)
            return GrabTarget.HandleSouth;
        if (handleEast != null && Vector3.Distance(handPos, handleEast.transform.position) < directRadius)
            return GrabTarget.HandleEast;
        if (handleWest != null && Vector3.Distance(handPos, handleWest.transform.position) < directRadius)
            return GrabTarget.HandleWest;

        // 2. Check direct proximity to the border colliders
        if (borderColliders != null)
        {
            foreach (BoxCollider bc in borderColliders)
            {
                if (bc == null) continue;
                Vector3 closest = bc.ClosestPoint(handPos);
                if (Vector3.Distance(handPos, closest) < directRadius)
                {
                    return GrabTarget.BoardFrame;
                }
            }
        }

        // 3. Check raycast from controller forward (so player can grab handles or border at a distance)
        Ray ray = new Ray(hand.position, hand.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 4.0f);
        float bestDist = float.MaxValue;
        GrabTarget bestTarget = GrabTarget.None;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null) continue;
            GameObject go = hit.collider.gameObject;
            GrabTarget candidate = GrabTarget.None;
            if (go == handleNorth) candidate = GrabTarget.HandleNorth;
            else if (go == handleSouth) candidate = GrabTarget.HandleSouth;
            else if (go == handleEast) candidate = GrabTarget.HandleEast;
            else if (go == handleWest) candidate = GrabTarget.HandleWest;
            else if (go == borderObject) candidate = GrabTarget.BoardFrame;

            if (candidate != GrabTarget.None && hit.distance < bestDist)
            {
                bestDist = hit.distance;
                bestTarget = candidate;
            }
        }

        return bestTarget;
    }

    private void OnHandGrabStarted(Transform hand, GrabTarget target)
    {
        if (target == GrabTarget.BoardFrame)
        {
            activeBoardGrabHand = hand;
            boardGrabYawOffset = Mathf.DeltaAngle(hand.eulerAngles.y, tabletopAnchor.transform.eulerAngles.y);
            Quaternion invRot = Quaternion.Inverse(tabletopAnchor.transform.rotation);
            boardGrabOffsetLocal = invRot * (tabletopAnchor.transform.position - hand.position);
        }
        else if (IsEdgeHandle(target))
        {
            Vector3 localPos = GetHandPositionOnBoardPlane(hand);
            if (target == GrabTarget.HandleEast || target == GrabTarget.HandleWest)
            {
                handleDragStartCoord = Mathf.Abs(localPos.x);
                handleDragStartTiles = gridTilesX;
            }
            else
            {
                handleDragStartCoord = Mathf.Abs(localPos.z);
                handleDragStartTiles = gridTilesZ;
            }
        }
    }

    private void UpdateEdgeHandleDrag(Transform hand, GrabTarget target)
    {
        Vector3 localPos = GetHandPositionOnBoardPlane(hand);
        float currentCoord = (target == GrabTarget.HandleEast || target == GrabTarget.HandleWest)
            ? Mathf.Abs(localPos.x)
            : Mathf.Abs(localPos.z);

        float deltaMeters = currentCoord - handleDragStartCoord;
        int deltaTiles = Mathf.RoundToInt((deltaMeters * 2f) / Mathf.Max(tileScale, 0.01f));

        if (target == GrabTarget.HandleEast || target == GrabTarget.HandleWest)
        {
            gridTilesX = Mathf.Clamp(handleDragStartTiles + deltaTiles, 11, 32);
        }
        else if (target == GrabTarget.HandleNorth || target == GrabTarget.HandleSouth)
        {
            gridTilesZ = Mathf.Clamp(handleDragStartTiles + deltaTiles, 11, 32);
        }
    }

    private Vector3 GetHandPositionOnBoardPlane(Transform hand)
    {
        Plane boardPlane = new Plane(tabletopAnchor.transform.up, tabletopAnchor.transform.position);
        Ray ray = new Ray(hand.position, hand.forward);
        if (boardPlane.Raycast(ray, out float enter) && enter > 0f && enter < 4.0f)
        {
            Vector3 worldHit = ray.GetPoint(enter);
            return tabletopAnchor.transform.InverseTransformPoint(worldHit);
        }
        return tabletopAnchor.transform.InverseTransformPoint(hand.position);
    }

    private void EnsureDynamicObjectsParented()
    {
        if (world == null || worldContainer == null) return;

        if (world.dungeon != null && world.dungeon.transform.parent != worldContainer.transform)
        {
            world.dungeon.transform.SetParent(worldContainer.transform, false);
        }
        if (world.mainTerrain != null && world.mainTerrain.transform.parent != worldContainer.transform)
        {
            world.mainTerrain.transform.SetParent(worldContainer.transform, false);
        }
        if (world.party != null && world.party.transform.parent != worldContainer.transform)
        {
            world.party.transform.SetParent(worldContainer.transform, false);
        }
        if (Combat.combatTerrainsRoot != null && Combat.combatTerrainsRoot.transform.parent != worldContainer.transform)
        {
            Combat.combatTerrainsRoot.transform.SetParent(worldContainer.transform, false);
        }

        // Also check if World got any new direct children (e.g. after changeTiles or activeCharacter creation)
        for (int i = world.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = world.transform.GetChild(i);
            if (child == null) continue;
            if (child.gameObject == world.skyGameObject) continue;
            if (child.gameObject == tabletopAnchor) continue;
            child.SetParent(worldContainer.transform, false);
        }
    }

    private Vector2 GetTargetFocalTile()
    {
        if (world == null || world.u4 == null) return currentFocalTile;

        U4_Decompiled_AVATAR u4 = world.u4;
        U4_Decompiled_AVATAR.MODE mode = u4.current_mode;

        if (mode == U4_Decompiled_AVATAR.MODE.OUTDOORS ||
            mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT)
        {
            return new Vector2(u4.Party._x, 255f - u4.Party._y);
        }
        else if (mode == U4_Decompiled_AVATAR.MODE.DUNGEON ||
                 mode == U4_Decompiled_AVATAR.MODE.COMBAT_ROOM)
        {
            return new Vector2(u4.Party._x * 11f + 5f, (7f - u4.Party._y) * 11f + 5f);
        }
        else if (mode == U4_Decompiled_AVATAR.MODE.COMBAT_CAMP ||
                 mode == U4_Decompiled_AVATAR.MODE.SHRINE)
        {
            return new Vector2(5f, 250f);
        }
        else if (mode == U4_Decompiled_AVATAR.MODE.COMBAT)
        {
            // Surface combat vs Dungeon hallway combat
            if ((u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) &&
                (u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS))
            {
                return new Vector2(u4.Party._x * 11f + 5f, (7f - u4.Party._y) * 11f + 5f);
            }
            else
            {
                return new Vector2(5f, 250f);
            }
        }

        return currentFocalTile;
    }

    private void UpdateWorldContainerTransform()
    {
        if (worldContainer == null || world == null || world.u4 == null) return;

        Vector2 targetFocal = GetTargetFocalTile();
        U4_Decompiled_AVATAR.MODE mode = world.u4.current_mode;
        int dungeonLvl = world.u4.Party._z;

        lastGameMode = mode;
        lastDungeonLevel = dungeonLvl;

        // Keep the tabletop grid an exact integer multiple of the tile grid aligned with tile seams:
        // Each tile is centered at an integer (tx, tz) and spans [tx - 0.5, tx + 0.5].
        // When gridTilesX is odd, centering on an integer tile places the border at integer +/- (odd * 0.5) = integer + 0.5 (an exact seam).
        // When gridTilesX is even, shifting by +0.5 places the border at (integer + 0.5) +/- (even * 0.5) = integer + 0.5 (also an exact seam).
        float alignedTileX = Mathf.Round(targetFocal.x) + ((gridTilesX % 2 == 0) ? 0.5f : 0f);
        float alignedTileZ = Mathf.Round(targetFocal.y) + ((gridTilesZ % 2 == 0) ? 0.5f : 0f);
        currentFocalTile = new Vector2(alignedTileX, alignedTileZ);
        focalVelocity = Vector2.zero;

        worldContainer.transform.localScale = Vector3.one * tileScale;
        worldContainer.transform.localPosition = new Vector3(
            -currentFocalTile.x * tileScale,
            0.5f * tileScale,
            -currentFocalTile.y * tileScale
        );
        worldContainer.transform.localRotation = Quaternion.identity;
    }

    private void UpdateShaderClippingUniforms()
    {
        if (tabletopAnchor == null) return;

        float halfWidth = (gridTilesX * tileScale) * 0.5f;
        float halfDepth = (gridTilesZ * tileScale) * 0.5f;
        float minHeight = -0.02f;
        float fadeWidth = Mathf.Max(0.001f, edgeFadeTiles * tileScale);

        Shader.SetGlobalFloat(PropClipEnabled, 1f);
        Shader.SetGlobalMatrix(PropWorldToLocal, tabletopAnchor.transform.worldToLocalMatrix);
        Shader.SetGlobalVector(PropClipHalfSize, new Vector4(halfWidth, minHeight, halfDepth, fadeWidth));
    }

    private void UpdateOverheadMoonsAndSky()
    {
        if (world == null || world.skyGameObject == null) return;
        CacheSkyTransforms();

        // Hide the 10000x10000 black Horizon circle in Tabletop mode so it never blocks passthrough or slices the scene
        if (skyHorizonTransform != null && skyHorizonTransform.gameObject.activeSelf)
        {
            skyHorizonTransform.gameObject.SetActive(false);
        }

        // Only display sky & moons when on surface
        bool isDungeonLoc = (world.u4 != null) &&
            (world.u4.Party._loc >= U4_Decompiled_AVATAR.LOCATIONS.DECEIT) &&
            (world.u4.Party._loc <= U4_Decompiled_AVATAR.LOCATIONS.THE_GREAT_STYGIAN_ABYSS);

        bool showSky = (world.u4 != null) &&
            (world.u4.current_mode == U4_Decompiled_AVATAR.MODE.OUTDOORS ||
             world.u4.current_mode == U4_Decompiled_AVATAR.MODE.SETTLEMENT ||
             world.u4.current_mode == U4_Decompiled_AVATAR.MODE.SHRINE ||
             (world.u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT_CAMP && !isDungeonLoc) ||
             (world.u4.current_mode == U4_Decompiled_AVATAR.MODE.COMBAT && !isDungeonLoc));

        if (!showSky) return;

        // Keep Sky at 1:1 original scale, upright (never tilt Sky on X or Z!), centered on the viewer
        Vector3 viewerPos = mainCamera != null ? mainCamera.transform.position :
                            (tabletopAnchor != null ? tabletopAnchor.transform.position : Vector3.zero);
        world.skyGameObject.transform.position = viewerPos;
        world.skyGameObject.transform.rotation = Quaternion.identity;
        world.skyGameObject.transform.localScale = Vector3.one;

        // Position Trammel and Felucca high up in the sky at their original scale (20 and 10)
        if (trammelSphereTransform != null)
        {
            trammelSphereTransform.localPosition = new Vector3(-120f, 360f, 280f);
            trammelSphereTransform.localScale = savedTrammelLocalScale;
        }
        if (feluccaSphereTransform != null)
        {
            feluccaSphereTransform.localPosition = new Vector3(120f, 300f, 300f);
            feluccaSphereTransform.localScale = savedFeluccaLocalScale;
        }
    }

    private void RestoreSkyForFullVR()
    {
        if (world == null || world.skyGameObject == null) return;
        CacheSkyTransforms();

        // Ensure Sky is completely upright at 1:1 scale so Horizon never tilts into the Full VR grid
        world.skyGameObject.transform.localRotation = Quaternion.identity;
        world.skyGameObject.transform.localScale = Vector3.one;

        if (skyHorizonTransform != null)
        {
            skyHorizonTransform.gameObject.SetActive(true);
            skyHorizonTransform.localPosition = new Vector3(0f, -1.5f, 0f);
            skyHorizonTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }
        if (trammelSphereTransform != null && skyTransformsCached)
        {
            trammelSphereTransform.localPosition = savedTrammelLocalPos;
            trammelSphereTransform.localScale = savedTrammelLocalScale;
        }
        if (feluccaSphereTransform != null && skyTransformsCached)
        {
            feluccaSphereTransform.localPosition = savedFeluccaLocalPos;
            feluccaSphereTransform.localScale = savedFeluccaLocalScale;
        }
    }
}
