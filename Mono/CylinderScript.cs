using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CylinderScript : MonoBehaviour
{

    //the outside radius must lager than the inside one
    [Range(0, 100)] public float innerRadius;
    [Range(0, 100)] public float outsideRadius;
    public int blockCounts = 80;          //how many blocks that the obj will be split into?
    public float height = 10;             //the height of the obj.


    private float increment;
    private float currentAngle = 0;


    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = transform.GetComponent<MeshFilter>();

        GenerateMesh();

        //show the order we generate the obj.
        StartCoroutine(SequenceTest());

    }


    private void GenerateMesh()
    {
        //declare all the array we need
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();


        //initialize the parameters
        increment = 2 * Mathf.PI / blockCounts;

        //Generate the vertex we need
        vertices = GenerateVertics();
        //Fill the triangles in order
        triangles = FillTriangles(vertices.Count);

        meshFilter.mesh.vertices = vertices.ToArray();
        meshFilter.mesh.triangles = triangles.ToArray();

    }


    private List<Vector3> GenerateVertics()
    {
        //The order to Generate the vertics :   choose the left bottom as the first point, and assign the id by clockwise, inside circle generate first
        //顶点标号顺序  ： 以左下角为起点，以顺时针顺序给各顶点标号，从内层圆环开始

        List<Vector3> vertices = new List<Vector3>();

        //used to load the radius we have    [Use Array to help us expand the plies]
        float[] radiuses = { innerRadius, outsideRadius };

        //For now this code will generate the inner circle first
        for (int i = 0; i < radiuses.Length; i++)
        {
            for (int j = 0; j < blockCounts * 4; j += 4)
            {
                //TODO : for now ,this script will generate 4 more vertices
                Vector3 v1 = new Vector3(radiuses[i] * Mathf.Sin(currentAngle), 0, radiuses[i] * Mathf.Cos(currentAngle));
                Vector3 v2 = new Vector3(radiuses[i] * Mathf.Sin(currentAngle), height, radiuses[i] * Mathf.Cos(currentAngle));

                //Generate next two vertices
                currentAngle += increment;

                Vector3 v3 = new Vector3(radiuses[i] * Mathf.Sin(currentAngle), height, radiuses[i] * Mathf.Cos(currentAngle));
                Vector3 v4 = new Vector3(radiuses[i] * Mathf.Sin(currentAngle), 0, radiuses[i] * Mathf.Cos(currentAngle));

                vertices.Add(v1);
                vertices.Add(v2);
                vertices.Add(v3);
                vertices.Add(v4);
            }
        }

        return vertices;
    }

    private List<int> FillTriangles(int vertCount)
    {

        List<int> triangles = new List<int>();

        //1.fill the inner && outside surface 
        for (int i = 0; i < vertCount - 2; i += 2)
        {
            if (i == vertCount - 2 || i == vertCount - 1)     //connect with the origin points
            {
                triangles.AddRange(GetTriangleOrder(i, i + 1, i - (blockCounts - 2), i - (blockCounts - 3)));
            }
            else if (i < vertCount / 2)
            {
                //inner circle only needs to see the inside surface
                triangles.AddRange(GetTriangleOrder(i, i + 1, i + 2, i + 3));
            }
            else
            {
                //outside surface
                triangles.AddRange(GetTriangleOrder(i + 3, i + 2, i + 1, i));
            }
        }


        //2.fill the top && bottom surface
        for (int i = 0, j = vertCount / 2; i < vertCount / 2; i += 4, j += 4)
        {

            if (i >= vertCount / 2 - 2)
            {
                triangles.AddRange(GetTriangleOrder(0, vertCount / 2, j, i));
                triangles.AddRange(GetTriangleOrder(i + 1, j + 1, vertCount / 2 + 1, 1));
            }
            else
            {
                triangles.AddRange(GetTriangleOrder(i + 3, j + 3, j, i));
                triangles.AddRange(GetTriangleOrder(i + 1, j + 1, j + 2, i + 2));
            }

        }

        return triangles;

    }


    private List<int> GetTriangleOrder(int p1, int p2, int p3, int p4)     //the input must be from the left bottom corner && sort by clockwise
    {
        //use this code to return a particular order

        List<int> output = new List<int>();

        //Add first triangle
        output.Add(p1);
        output.Add(p2);
        output.Add(p3);

        //Add the second one
        output.Add(p3);
        output.Add(p4);
        output.Add(p1);

        return output;
    }


    IEnumerator SequenceTest()
    {

        float interval = 0.01f;

        for (int i = 0; i < meshFilter.mesh.triangles.Length; i += 3)
        {
            Debug.DrawLine(meshFilter.mesh.vertices[meshFilter.mesh.triangles[i]], meshFilter.mesh.vertices[meshFilter.mesh.triangles[i + 1]], Color.red, 100f);

            yield return new WaitForSeconds(interval);
            Debug.DrawLine(meshFilter.mesh.vertices[meshFilter.mesh.triangles[i + 1]], meshFilter.mesh.vertices[meshFilter.mesh.triangles[i + 2]], Color.yellow, 100f);

            yield return new WaitForSeconds(interval);
            Debug.DrawLine(meshFilter.mesh.vertices[meshFilter.mesh.triangles[i + 2]], meshFilter.mesh.vertices[meshFilter.mesh.triangles[i]], Color.blue, 100f);

            yield return new WaitForSeconds(interval);

        }
    }
}