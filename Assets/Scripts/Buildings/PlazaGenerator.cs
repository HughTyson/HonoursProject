using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlazaGenerator : MonoBehaviour
{
    static public void Generate(Plot plot)
    {
        GameObject obj = new GameObject("Plaza");

        MeshFilter mf = obj.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer mr = obj.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        List<Vector2> list = new List<Vector2>();

        foreach(Vector3 v in plot.vertexes)
        {
            list.Add(new Vector2(v.x, v.z));
        }


        Triangulator tr = new Triangulator(list.ToArray());
        int[] indicies = tr.Triangulate();

        Mesh m = new Mesh();
        m.vertices = plot.vertexes.ToArray();
        m.triangles = indicies;
        m.RecalculateNormals();
        m.RecalculateBounds();

        mf.mesh = m;

        mr.material.color = Color.green;
    }
}
