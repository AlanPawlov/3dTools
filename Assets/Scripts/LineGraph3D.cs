using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineGraph3D : MonoBehaviour
{
	public float GraphWidth = 10f;
	public float GraphHeight = 5f;
	public float GraphDepth = 1f;
	public List<float> graphPoints = new();

	public Material SideMaterial;
	public Material TopMaterial;

	private Mesh Mesh;

	[Button]
	private void GenerateGraphMesh()
	{
		Clear();
		Mesh = new Mesh();
		Mesh.name = "LineGraph3D";
		var graphCount = graphPoints.Count;

		var topVertCount = graphCount * 2;
		var sideVertCount = graphCount * 4;
		var addSideVertCount = 8;
		var vertCount = topVertCount + sideVertCount + addSideVertCount;
		var vert = new Vector3[vertCount];

		var topTriangles = new int[(graphCount - 1) * 6];
		var sideTriangles = new int[(graphCount - 1) * 18 + 12];
		var aSideTriangles = new int[12];
		var uvs = new Vector2[vertCount];

		for (int i = 0; i < graphCount; i++)
		{
			var t = (float)i / (graphCount - 1);
			var x = t * GraphWidth;
			var y = graphPoints[i] * GraphHeight;

			//top verticles
			vert[sideVertCount + i * 2] = new Vector3(x, y, -GraphDepth / 2);
			vert[sideVertCount + i * 2 + 1] = new Vector3(x, y, GraphDepth / 2);

			//side verticles
			vert[i * 4] = new Vector3(x, y, -GraphDepth / 2);
			vert[i * 4 + 1] = new Vector3(x, y, GraphDepth / 2);
			vert[i * 4 + 2] = new Vector3(x, 0, -GraphDepth / 2);
			vert[i * 4 + 3] = new Vector3(x, 0, GraphDepth / 2);

			//top uvs
			uvs[sideVertCount + i * 2] = new Vector3(x, -GraphDepth / 2);
			uvs[sideVertCount + i * 2 + 1] = new Vector3(x, GraphDepth / 2);

			uvs[i * 4] = new Vector2(x, y);
			uvs[i * 4 + 1] = new Vector2(x, y);
			uvs[i * 4 + 2] = new Vector2(x, 0);
			uvs[i * 4 + 3] = new Vector2(x, 0);

			if (i < graphCount - 1)
			{
				var topIndex = i * 6;
				var sideIndex = i * 18;
				var vertIndex = i * 4;
				var topVertIndex = i * 2;

				//top 
				topTriangles[topIndex] = sideVertCount + topVertIndex;
				topTriangles[topIndex + 1] = sideVertCount + topVertIndex + 1;
				topTriangles[topIndex + 2] = sideVertCount + topVertIndex + 2;

				topTriangles[topIndex + 3] = sideVertCount + topVertIndex + 2;
				topTriangles[topIndex + 4] = sideVertCount + topVertIndex + 1;
				topTriangles[topIndex + 5] = sideVertCount + topVertIndex + 3;

				// bottom
				sideTriangles[sideIndex] = vertIndex + 2;
				sideTriangles[sideIndex + 1] = vertIndex + 6;
				sideTriangles[sideIndex + 2] = vertIndex + 3;

				sideTriangles[sideIndex + 3] = vertIndex + 3;
				sideTriangles[sideIndex + 4] = vertIndex + 6;
				sideTriangles[sideIndex + 5] = vertIndex + 7;

				//left
				sideTriangles[sideIndex + 6] = vertIndex;
				sideTriangles[sideIndex + 7] = vertIndex + 4;
				sideTriangles[sideIndex + 8] = vertIndex + 2;

				sideTriangles[sideIndex + 9] = vertIndex + 2;
				sideTriangles[sideIndex + 10] = vertIndex + 4;
				sideTriangles[sideIndex + 11] = vertIndex + 6;

				//right
				sideTriangles[sideIndex + 12] = vertIndex + 1;
				sideTriangles[sideIndex + 13] = vertIndex + 3;
				sideTriangles[sideIndex + 14] = vertIndex + 5;

				sideTriangles[sideIndex + 15] = vertIndex + 5;
				sideTriangles[sideIndex + 16] = vertIndex + 3;
				sideTriangles[sideIndex + 17] = vertIndex + 7;
			}
		}

		//start vert 
		vert[topVertCount + sideVertCount] = vert[0];
		vert[topVertCount + sideVertCount + 1] = vert[1];
		vert[topVertCount + sideVertCount + 2] = vert[2];
		vert[topVertCount + sideVertCount + 3] = vert[3];

		uvs[topVertCount + sideVertCount] = new Vector2(vert[topVertCount + sideVertCount].z, vert[topVertCount + sideVertCount].y);
		uvs[topVertCount + sideVertCount + 1] = new Vector2(vert[topVertCount + sideVertCount + 1].z, vert[topVertCount + sideVertCount + 1].y);
		uvs[topVertCount + sideVertCount + 2] = new Vector2(vert[topVertCount + sideVertCount + 2].z, vert[topVertCount + sideVertCount + 2].y);
		uvs[topVertCount + sideVertCount + 3] = new Vector2(vert[topVertCount + sideVertCount + 3].z, vert[topVertCount + sideVertCount + 3].y);

		//end vert
		vert[topVertCount + sideVertCount + 4] = vert[sideVertCount - 1];
		vert[topVertCount + sideVertCount + 5] = vert[sideVertCount - 2];
		vert[topVertCount + sideVertCount + 6] = vert[sideVertCount - 3];
		vert[topVertCount + sideVertCount + 7] = vert[sideVertCount - 4];
		
		uvs[topVertCount + sideVertCount + 4] = new Vector2(vert[topVertCount + sideVertCount + 4].z, vert[topVertCount + sideVertCount + 4].y);
		uvs[topVertCount + sideVertCount + 5] = new Vector2(vert[topVertCount + sideVertCount + 5].z, vert[topVertCount + sideVertCount + 5].y);
		uvs[topVertCount + sideVertCount + 6] = new Vector2(vert[topVertCount + sideVertCount + 6].z, vert[topVertCount + sideVertCount + 6].y);
		uvs[topVertCount + sideVertCount + 7] = new Vector2(vert[topVertCount + sideVertCount + 7].z, vert[topVertCount + sideVertCount + 7].y);

		
		//Fill start/end traingles
		aSideTriangles[0] = topVertCount + sideVertCount + 0;
		aSideTriangles[1] = topVertCount + sideVertCount + 2;
		aSideTriangles[2] = topVertCount + sideVertCount + 1;

		aSideTriangles[3] = topVertCount + sideVertCount + 1;
		aSideTriangles[4] = topVertCount + sideVertCount + 2;
		aSideTriangles[5] = topVertCount + sideVertCount + 3;

		aSideTriangles[6] = topVertCount + sideVertCount + 4;
		aSideTriangles[7] = topVertCount + sideVertCount + 5;
		aSideTriangles[8] = topVertCount + sideVertCount + 6;

		aSideTriangles[9] = topVertCount + sideVertCount + 5;
		aSideTriangles[10] = topVertCount + sideVertCount + 7;
		aSideTriangles[11] = topVertCount + sideVertCount + 6;

		Mesh.vertices = vert;
		Mesh.uv = uvs;

		Mesh.subMeshCount = 3;
		Mesh.SetTriangles(topTriangles, 0);
		Mesh.SetTriangles(sideTriangles, 1);
		Mesh.SetTriangles(aSideTriangles, 2);

		Mesh.RecalculateBounds();
		GetComponent<MeshFilter>().mesh = Mesh;
		GetComponent<MeshRenderer>().materials = new[] { TopMaterial, SideMaterial, SideMaterial };
	}

	[Button]
	public void Clear()
	{
		Mesh = null;
		GetComponent<MeshFilter>().mesh = null;
	}
}