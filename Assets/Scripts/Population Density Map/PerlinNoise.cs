using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{

    [SerializeField] float scale = 400;
    [Range(0.0f, 1.0f)]
    [SerializeField] float floor_value = 0.6f;


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
}
