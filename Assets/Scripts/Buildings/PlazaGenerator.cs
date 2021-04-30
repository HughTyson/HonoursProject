using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlazaGenerator : MonoBehaviour
{
    //statuic function whihc is called when a city blokc is ot be a plaza
    //create a new gameobejct and mesh which the player can collide with,
    //mesh fills the whole area
    static public void Generate(Plot plot)
    {
        GameObject obj = new GameObject("Plaza");
        obj.layer = 8;
       
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
        MeshCollider mc = obj.AddComponent(typeof(MeshCollider)) as MeshCollider;
        
        mr.material.color = Color.green;
    }
}
