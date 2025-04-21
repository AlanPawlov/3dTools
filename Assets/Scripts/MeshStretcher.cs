public class MeshStretchPreservingEnds : MonoBehaviour
{
    [Range(1f, 10f)] public float Size = 2f;

    [Range(0f, 0.5f)] public float EndBorderPercent = 0.1f;

    public bool pivotAtBottom = true;

    private Mesh _originalMesh;
    private Vector3[] _originalVertices;

    [Button]
    private void Start()
    {
        var filter = GetComponent<MeshFilter>();
        _originalMesh = Instantiate(filter.sharedMesh);
        filter.mesh = _originalMesh;
        _originalVertices = _originalMesh.vertices.Clone() as Vector3[];

        ApplyStretch();
    }

    [Button]
    public void ApplyStretch()
    {
        var newVertices = new Vector3[_originalVertices.Length];

        var minY = float.MaxValue;
        var maxY = float.MinValue;

        foreach (var v in _originalVertices)
        {
            minY = Mathf.Min(minY, v.y);
            maxY = Mathf.Max(maxY, v.y);
        }

        var height = maxY - minY;
        var borderHeight = height * EndBorderPercent;

        var centerStart = minY + borderHeight;
        var centerEnd = maxY - borderHeight;

        var centerHeight = centerEnd - centerStart;
        var stretchHeight = height * Size - (2 * borderHeight);

        for (int i = 0; i < _originalVertices.Length; i++)
        {
            var v = _originalVertices[i];
            var newY = v.y;

            if (v.y < centerStart)
            {
                newY = v.y;
            }
            else if (v.y > centerEnd)
            {
                var topOffset = (stretchHeight - centerHeight);
                newY = centerEnd + (v.y - centerEnd) + topOffset;
            }
            else
            {
                var t = (v.y - centerStart) / centerHeight;
                newY = centerStart + t * stretchHeight;
            }

            if (!pivotAtBottom)
            {
                var fullDelta = (Size - height);
                newY -= fullDelta;
            }

            newVertices[i] = new Vector3(v.x, newY, v.z);
        }

        _originalMesh.vertices = newVertices;
        _originalMesh.RecalculateNormals();
        _originalMesh.RecalculateBounds();
    }
}
