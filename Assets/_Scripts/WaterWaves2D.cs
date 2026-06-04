using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider2D))]
public class WaterWaves2D : MonoBehaviour
{
    [Header("Spring Settings")]
    public float tension = 0.025f;
    public float damping = 0.025f;
    public float spread = 0.25f;
    [Tooltip("Multiplier for forces applied by objects entering/exiting water")]
    public float splashForceMultiplier = 0.05f;

    [Header("Mesh Settings")]
    [Tooltip("Number of points on the water surface")]
    public int resolution = 50;

    [Header("Ripple Settings")]
    public bool enableRipples = true;
    public float minRippleInterval = 1f;
    public float maxRippleInterval = 3f;
    public float minRippleForce = 0.05f;
    public float maxRippleForce = 0.2f;

    [Header("Debug")]
    [Tooltip("Draws the spring points in the Scene view")]
    public bool showDebugGizmos = true;
    [Range(0.01f, 2f)]
    public float gizmoSize = 0.1f;
    public Color gizmoColor = Color.cyan;

    private float[] heights;
    private float[] velocities;
    private float[] accelerations;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private BoxCollider2D boxCollider;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    private float rippleTimer;

    private void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        GenerateMesh();
    }

    [ContextMenu("Generate Water Mesh")]
    private void GenerateMesh()
    {
        if (meshFilter == null || boxCollider == null) return;

        Mesh m = new Mesh();
        m.name = "WaterMesh";
        m.MarkDynamic();
        
        float width = boxCollider.size.x;
        float height = boxCollider.size.y;
        
        int vertexCount = resolution * 2;
        vertices = new Vector3[vertexCount];
        uvs = new Vector2[vertexCount];
        triangles = new int[(resolution - 1) * 6];

        heights = new float[resolution];
        velocities = new float[resolution];
        accelerations = new float[resolution];

        float startX = -width / 2f;
        float topY = height / 2f;
        float bottomY = -height / 2f;
        
        float offsetX = boxCollider.offset.x;
        float offsetY = boxCollider.offset.y;

        for (int i = 0; i < resolution; i++)
        {
            float xPos = startX + width * ((float)i / (resolution - 1));
            
            vertices[i] = new Vector3(xPos + offsetX, topY + offsetY, 0);
            uvs[i] = new Vector2((float)i / (resolution - 1), 1f);
            
            vertices[i + resolution] = new Vector3(xPos + offsetX, bottomY + offsetY, 0);
            uvs[i + resolution] = new Vector2((float)i / (resolution - 1), 0f);

            heights[i] = topY;
        }

        int t = 0;
        for (int i = 0; i < resolution - 1; i++)
        {
            triangles[t++] = i;
            triangles[t++] = i + resolution;
            triangles[t++] = i + 1;

            triangles[t++] = i + 1;
            triangles[t++] = i + resolution;
            triangles[t++] = i + resolution + 1;
        }

        m.vertices = vertices;
        m.uv = uvs;
        m.triangles = triangles;
        m.RecalculateBounds();
        
        if (Application.isPlaying)
        {
            mesh = m;
            meshFilter.mesh = mesh;
        }
        else
        {
            // In Edit Mode, use sharedMesh to prevent memory leaks and allow viewing
            meshFilter.sharedMesh = m;
        }
    }

    private void Update()
    {
        if (!Application.isPlaying) return; // Only simulate physics in Play mode

        float dt = Time.deltaTime;
        if (dt <= 0) return;

        UpdateSprings(dt);
        HandleRipples(dt);
        UpdateMesh();
    }

    private void UpdateSprings(float dt)
    {
        float targetHeight = boxCollider.size.y / 2f;
        float speedScale = dt * 60f; 
        
        for (int i = 0; i < resolution; i++)
        {
            float displacement = heights[i] - targetHeight;
            accelerations[i] = -tension * displacement - damping * velocities[i];
            velocities[i] += accelerations[i] * speedScale;
            heights[i] += velocities[i] * speedScale;
        }

        float[] leftDeltas = new float[resolution];
        float[] rightDeltas = new float[resolution];

        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < resolution; i++)
            {
                if (i > 0)
                {
                    leftDeltas[i] = spread * (heights[i] - heights[i - 1]);
                    velocities[i - 1] += leftDeltas[i] * speedScale;
                }
                if (i < resolution - 1)
                {
                    rightDeltas[i] = spread * (heights[i] - heights[i + 1]);
                    velocities[i + 1] += rightDeltas[i] * speedScale;
                }
            }

            for (int i = 0; i < resolution; i++)
            {
                if (i > 0) heights[i - 1] += leftDeltas[i] * speedScale;
                if (i < resolution - 1) heights[i + 1] += rightDeltas[i] * speedScale;
            }
        }
    }

    private void HandleRipples(float dt)
    {
        if (!enableRipples) return;
        
        rippleTimer -= dt;
        if (rippleTimer <= 0f)
        {
            int index = Random.Range(0, resolution);
            float force = Random.Range(minRippleForce, maxRippleForce);
            if (Random.value > 0.5f) force = -force;
            
            velocities[index] -= force;
            
            rippleTimer = Random.Range(minRippleInterval, maxRippleInterval);
        }
    }

    private void UpdateMesh()
    {
        float offsetY = boxCollider.offset.y;
        
        for (int i = 0; i < resolution; i++)
        {
            Vector3 v = vertices[i];
            v.y = heights[i] + offsetY;
            vertices[i] = v;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds(); 
    }

    public void Splash(float xPositionWorld, float force)
    {
        float startX = (-boxCollider.size.x / 2f) + boxCollider.offset.x;
        float width = boxCollider.size.x;
        
        float localX = transform.InverseTransformPoint(new Vector3(xPositionWorld, 0, 0)).x;
        
        float t = (localX - startX) / width;
        int index = Mathf.RoundToInt(t * (resolution - 1));
        
        if (index >= 0 && index < resolution)
        {
            velocities[index] += force;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            float force = rb.linearVelocity.y * splashForceMultiplier;
            force = Mathf.Clamp(force, -1f, 1f);
            Splash(collision.bounds.center.x, force);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            float force = rb.linearVelocity.y * splashForceMultiplier;
            force = Mathf.Clamp(force, -1f, 1f);
            Splash(collision.bounds.center.x, force);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && meshFilter != null && boxCollider != null)
        {
            // Delay the call to avoid "SendMessage cannot be called during Awake, CheckConsistency, or OnValidate" warnings
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null) GenerateMesh();
            };
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || vertices == null || boxCollider == null) return;
        
        Gizmos.color = gizmoColor;
        
        // The springs are the first 'resolution' vertices in the array.
        // We convert them from local space to world space to draw them.
        for (int i = 0; i < resolution; i++)
        {
            if (i >= vertices.Length) break;
            
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            Gizmos.DrawSphere(worldPos, gizmoSize);
            
            // Draw a line connecting the springs
            if (i > 0)
            {
                Vector3 prevWorldPos = transform.TransformPoint(vertices[i - 1]);
                Gizmos.DrawLine(prevWorldPos, worldPos);
            }
        }
    }
#endif
}
