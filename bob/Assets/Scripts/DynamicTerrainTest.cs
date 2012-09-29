using UnityEngine;
using System.Collections.Generic;



/*
 Generates a trail that is always facing upwards using the scriptable mesh interface.
 vertex colors and uv's are generated similar to the builtin Trail Renderer.
 To use it
 1. create an empty game object
 2. attach this script and a MeshRenderer
 3. Then assign a particle material to the mesh renderer
*/
public class DynamicTerrainTest : MonoBehaviour {
	
	class MeshSection
	{
		public Vector3 point;
		public Vector3 upDir;
		public float   time;
		public float heightOverride;
		public bool usesOverride;
		
	}
	
	public class DepthPoint
	{
		public Matrix4x4 matrix;
		public Vector3   point;
	}
	
	public GameObject player;
	public float height = 2.0f;
	public float time = 2.0f;
	public bool alwaysUp = false;
	public float minDistance = 0.1f;
	public int heightMin=10;
	public int heightMax=25;
	public int spacingMin=1;
	public int spacingMax=25;
	public int geomBuffer=10;
	public int extrusionWidth = 10;
	public bool invertFaces = false;
	public PhysicMaterial floorPhysMat = null;
	public GameObject     startObject = null;
	public float manualHeightOverride = 10.0f;
	public bool useManualHeightOverride = true;
	public float startingHeight = 0.0f;
	//private bool startPlaced = false;
	
	MeshExtrusion.Edge[] edges;
	
	int xSpacing;
	
	bool collisionNeedsUpdate;
	
	Quaternion extrudeRotation;
	
	Mesh mesh;
	
	Mesh out_mesh;
	
	Vector3 extrudeDirection;
	
	Vector3 position;
	
	Vector3 newHeightData;
	
	MeshCollider meshc;
	
	private	List<Vector2> uv;
	
	private List<Vector3> vertices;
	
	private List<int> triangles;
	
	private List<Matrix4x4> extrusion;
	
	private List<MeshSection> sections;
	
	private List<DepthPoint>  depthPoints;
	
	private bool startMeshLink;
	
	private System.Random randomizer;
	
	void Start()
	{		
		buildTerrain();
	}
	
	void buildTerrain()
	{
		collisionNeedsUpdate = true;
		extrudeDirection = new Vector3();
		extrudeRotation  = new Quaternion();
		extrusion = new List<Matrix4x4>();
		triangles = new List<int>();
		uv = new List<Vector2>();
		vertices = new List<Vector3>();
		sections = new List<MeshSection>();
		depthPoints = new List<DepthPoint>();
		newHeightData = new Vector3();
		mesh =  GetComponent<MeshFilter>().mesh;
		out_mesh = GetComponent<MeshFilter>().sharedMesh;
		position = transform.position;
		randomizer = new System.Random();
		
		
		if(startObject!= null)
		{
			startMeshLink = true;
		}
		else
		{
			startMeshLink = false;
		}
		
		while(sections.Count<geomBuffer)
		{			
			Restock();
			//startPlaced = true;
		}
		
		if (sections.Count < 2)
			return;
		
		MeshSection currentSection = sections[0];
		Matrix4x4 localSpaceTransform = transform.localToWorldMatrix;
		for (int i=0;i<sections.Count;i++)
		{
			currentSection = sections[i];
			
			Vector3 upDir = currentSection.upDir;	
			
			vertices.Add(localSpaceTransform.MultiplyPoint(currentSection.point));
			if(currentSection.usesOverride)
			{
				newHeightData.Set(currentSection.point.x,currentSection.heightOverride,currentSection.point.z);
			}
			else
			{
				newHeightData.Set(currentSection.point.x,randomizer.Next(heightMin,heightMax),currentSection.point.z);				
			}
			vertices.Add(localSpaceTransform.MultiplyPoint(newHeightData + upDir * height));
			
			
			float u = 0.0f;			
			if (i != 0)
				u = Mathf.Clamp01 ((Time.time - currentSection.time) / time);		
			uv.Add(new Vector2(u, 0));
			uv.Add(new Vector2(u, 1));
			
		}

		ResetTris();
		mesh.Clear();
		if(meshc == null)
		{
			meshc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
			if(floorPhysMat != null)
				meshc.material = floorPhysMat;
		}			

	}
	
	void Update()
	{
		while(sections.Count < geomBuffer)
		{
			Restock();
			collisionNeedsUpdate = true;
		}
		if(sections[1].point.x - player.transform.position.x < 1)
		{	
			Matrix4x4 localSpaceTransform = transform.localToWorldMatrix;
			Vector3 upDir = sections[sections.Count-1].upDir;
			
			newHeightData.Set(sections[sections.Count-1].point.x,randomizer.Next(heightMin,heightMax),sections[sections.Count-1].point.z);
			if(sections[sections.Count-1].usesOverride)
			{
				newHeightData.Set(sections[sections.Count-1].point.x,sections[sections.Count-1].heightOverride,sections[sections.Count-1].point.z);
			}
			else
			{
				newHeightData.Set(sections[sections.Count-1].point.x,randomizer.Next(heightMin,heightMax),sections[sections.Count-1].point.z);				
			}
			vertices.Add(localSpaceTransform.MultiplyPoint(sections[sections.Count-1].point));
			vertices.Add(localSpaceTransform.MultiplyPoint(newHeightData + upDir * height));	
			
			float u = 0.0f;		
			for(int i = 0; i < 2; i++)
			{
				if (i != 0)
					u = Mathf.Clamp01 ((Time.time - sections[sections.Count-1].time) / time);		
				uv.Add(new Vector2(u, 0));
				uv.Add(new Vector2(u, 1));
			}			
			for(int i = 0; i < 4; i++)
			{
				
				if(i==0 || i == 1)
				{
					vertices.RemoveAt(i);			
				}
				uv.RemoveAt(i);
				
			}
			sections.RemoveAt(0);

			
			
		}
		triangles.Clear();
		ResetTris();

		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uv.ToArray();
		mesh.RecalculateNormals();
		edges = MeshExtrusion.BuildManifoldEdges(mesh);
	
		extrusion.Clear();
		extrusion.Add(transform.worldToLocalMatrix*Matrix4x4.TRS(depthPoints[0].point,extrudeRotation,Vector3.one));
		extrusion.Add(transform.worldToLocalMatrix*Matrix4x4.TRS(depthPoints[1].point,extrudeRotation,Vector3.one));
		MeshExtrusion.ExtrudeMesh(mesh,out_mesh,extrusion.ToArray(),edges,invertFaces);	
		
		if(collisionNeedsUpdate == true)
		{
			meshc.sharedMesh = null;
			meshc.sharedMesh = out_mesh;
			if(floorPhysMat!=null)
			{
				meshc.sharedMaterial = null;
				meshc.sharedMaterial = floorPhysMat;
			}
			collisionNeedsUpdate = false;
		}
		
	}
	
	void Restock()
	{
		float now = Time.time;
		MeshSection section = new MeshSection();
		
		if(startMeshLink)
		{
			Vector3 startPos = startObject.transform.position;
			startPos.x =  (startPos.x+(startObject.transform.localScale.x/2));
			startPos.y =  (0);
			section.point = startPos;
			if (alwaysUp)
				section.upDir = Vector3.up;
			else
				section.upDir = transform.TransformDirection(Vector3.up);
			section.time = now;		
			section.heightOverride = startingHeight;
			section.usesOverride = true;
			sections.Add(section);
			startMeshLink = false;
		}
		else
		{
			section.point = position;
			if (alwaysUp)
				section.upDir = Vector3.up;
			else
				section.upDir = transform.TransformDirection(Vector3.up);
			section.time = now;
			if(useManualHeightOverride)
			{
				section.usesOverride = true;
				section.heightOverride = manualHeightOverride;
			}
			else
			{
				section.usesOverride = false;
			}
			sections.Add(section);
		}
		

		xSpacing = randomizer.Next(spacingMin,spacingMax);
		position.x += xSpacing;

		
		DepthPoint dp1 = new DepthPoint();
		DepthPoint dp2 = new DepthPoint();
		
		if(sections.Count>4)
		{
			dp1.point = transform.position;
			dp2.point = transform.position;
			dp2.point.z = extrusionWidth;
			extrudeDirection = dp1.point+dp2.point;
			depthPoints.Clear();
			depthPoints.Add(dp1);
			depthPoints.Add(dp2);
			extrudeRotation = Quaternion.LookRotation(extrudeDirection,Vector3.up);
		}
		else
		{
			dp1.point = sections[0].point;
			dp2.point = sections[0].point;
			dp2.point.z = extrusionWidth;
			extrudeDirection = dp1.point-dp2.point;
			depthPoints.Clear();
			depthPoints.Add(dp1);
			depthPoints.Add(dp2);
			extrudeRotation = Quaternion.LookRotation(extrudeDirection,Vector3.up);
		}
	}
		
	void ResetTris()
	{
		int triLength = (sections.Count-1) * 2 * 3; 
		for (int i=0;i<triLength / 6;i++)
		{	
			triangles.Add(i * 2 + 2);
			triangles.Add(i * 2 + 1);
			triangles.Add(i * 2 + 3);
			
			triangles.Add(i * 2);
			triangles.Add(i * 2 + 1);
			triangles.Add(i * 2 + 2);
			

		}
	}
}
