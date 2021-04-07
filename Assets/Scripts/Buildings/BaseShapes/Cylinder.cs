using UnityEngine;




public class Cylinder
{
    
    private int number_vertices, number_triangles;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;

    public Cylinder(BuildingPlot plot, float height, int slices, int slices_skipped, int index_of_skipped)
    {
        number_vertices = (slices + 1 - slices_skipped * 2) * 4 + 2;
        number_triangles = (slices - slices_skipped * 2) * 12;

        vertices = new Vector3[number_vertices];
        triangles = new int[number_triangles];
        uv = new Vector2[number_vertices];
        
        float x;
        float y;
        float radius_x = plot.plot_dimensions.x / 2;
        float radius_y = plot.plot_dimensions.y / 2;

        for (int i = 0, j = 0; i <= slices; i++, j += 2)
        {
            x = radius_x * Mathf.Cos(2 * Mathf.PI * i / slices);
            y = radius_y * Mathf.Sin(2 * Mathf.PI * i / slices);

            uv[j] = new Vector2(1f / slices * i, 0);
            uv[j + ((slices + 1 - slices_skipped * 2)) * 2] = new Vector2(uv[j].x, 1);
            uv[j + 1] = new Vector2(1f / slices * i, 0);
            uv[j + 1 + ((slices + 1 - slices_skipped * 2)) * 2] = new Vector2(uv[j].x, 1);

            if (i == index_of_skipped || i == slices - index_of_skipped - slices_skipped) i += slices_skipped;
            
                vertices[j] = new Vector3(x + 0 / 2, 0, y + 0 / 2);
                vertices[j + ((slices + 1 - slices_skipped * 2)) * 2] = new Vector3(x + 0 / 2, height, y + 0 / 2);
                vertices[j + 1] = vertices[j];
                vertices[j + 1 + ((slices + 1 - slices_skipped * 2)) * 2] = new Vector3(x + 0 / 2, height, y + 0 / 2);
        }

        vertices[number_vertices - 2] = new Vector3(0 / 2, 0, 0 / 2);
        vertices[number_vertices - 1] = new Vector3(0 / 2, height, 0 / 2);
        uv[number_vertices - 2] = new Vector2(0, 0);
        uv[number_vertices - 1] = new Vector2(1, 1);

        int number_triangles_body = number_triangles - ((slices - slices_skipped * 2) * 6);

        for (int i = 0, k = 0; i < number_triangles_body; i += 6, k += 2)
        {
            triangles[i] = triangles[i + 3] = k;
            triangles[i + 1] = k + ((slices + 1 - slices_skipped * 2)) * 2;
            triangles[i + 2] = triangles[i + 4] = k + ((slices - slices_skipped * 2) + 2) * 2;
            triangles[i + 5] = k + 2;
        }

        for (int i = 0, k = 0; k < ((slices - slices_skipped * 2) * 2); i += 6, k += 2)
        {
            triangles[i + number_triangles_body] = k + 1;
            triangles[i + number_triangles_body + 1] = k + 3;
            triangles[i + number_triangles_body + 2] = number_vertices - 2;
            triangles[i + number_triangles_body + 3] = k + (slices + 1 - slices_skipped * 2) * 2 + 3;
            triangles[i + number_triangles_body + 4] = k + (slices + 1 - slices_skipped * 2) * 2 + 1;
            triangles[i + number_triangles_body + 5] = number_vertices - 1;
        }

    }

    public Vector3[] GetVertices()
    {
        return vertices;
    }

    public int[] GetTriangles()
    {
        return triangles;
    }

    public Vector2[] GetUV()
    {
        return uv;
    }

}
