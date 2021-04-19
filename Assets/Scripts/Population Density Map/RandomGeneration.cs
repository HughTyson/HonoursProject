using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGeneration : MonoBehaviour
{

    [SerializeField] float scale = 400;
    [Range(0.0f, 1.0f)]
    [SerializeField] float floor_value = 0.6f;

    [SerializeField] List<Peaks> peaks;

    public float GetPointCutOff(float pos_x, float pos_z)
    {
        float x_coord = GM_.Instance.config.seed + (pos_x * scale);
        float y_coord = GM_.Instance.config.seed + (pos_z * scale);

        float value = Mathf.PerlinNoise(x_coord, y_coord);

        if (value < floor_value)
        {
            value -= 0.2f;
            Mathf.Clamp01(value);

        }

        return value;
    }

    public float GetPoint(float pos_x, float pos_z)
    {
        float x_coord = GM_.Instance.config.seed + (pos_x * scale);
        float y_coord = GM_.Instance.config.seed + (pos_z * scale);

        float value = Mathf.PerlinNoise(x_coord, y_coord);

        return value;
    }


    public float GetPointCircle(float pos_x, float pos_y)
    {
        
        int itterator = 0;
        float min_distance = float.MaxValue;

        for(int i = 0; i < peaks.Count; i++)
        {
            float current_distance = Vector2.Distance(peaks[i].centre_position, new Vector2(pos_x, pos_y));

            if(current_distance < min_distance)
            {
                min_distance = current_distance;
                itterator = i;
            }
        }

        float pd = Vector2.Distance(peaks[itterator].centre_position,new Vector2(pos_x, pos_y) ) * 2 / peaks[itterator].radius;
        float return_value = GM_.Instance.config.building_plot_values.minimum_height;
        if(Mathf.Abs(pd) <= 1)
            return_value = (GM_.Instance.config.building_plot_values.maximum_height - GM_.Instance.config.building_plot_values.minimum_height) / 2 + Mathf.Cos(pd * 3.14f) * (GM_.Instance.config.building_plot_values.maximum_height - GM_.Instance.config.building_plot_values.minimum_height) / 2;

        return_value = return_value < GM_.Instance.config.building_plot_values.minimum_height ? GM_.Instance.config.building_plot_values.minimum_height : return_value;

        return return_value;
        
    }
}
