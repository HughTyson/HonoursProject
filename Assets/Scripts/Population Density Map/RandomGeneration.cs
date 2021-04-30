using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGeneration : MonoBehaviour
{
   

    [SerializeField] float scale = 400;
    [Range(0.0f, 1.0f)]
    [SerializeField] float floor_value = 0.6f;

    [SerializeField] List<Peaks> peaks;


    //function takes in a the position in the environement, gets the position relative to the seed of the map and the scale that has been set
    // this relative position is then used to get a perlin noise value
    //if this value is less than a threshold value then it will be decreseased and clamped between 0 and 1
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

    //function takes in a the position in the environement, gets the position relative to the seed of the map and the scale that has been set
    // this relative position is then used to get a perlin noise value
    //this vlaue is returned
    public float GetPoint(float pos_x, float pos_z)
    {
        float x_coord = GM_.Instance.config.seed + (pos_x * scale);
        float y_coord = GM_.Instance.config.seed + (pos_z * scale);

        float value = Mathf.PerlinNoise(x_coord, y_coord);

        return value;
    }

    //function takes in a the position in the environement and gets the closest peak
    //the returned building height is then evaluated using circular noise
    //this vlaue is returned
    public float GetPointCircleBuildingHeight(float pos_x, float pos_y)
    {

        int itterator = 0;
        float min_distance = float.MaxValue;

        GetClosestPeak(ref min_distance, ref itterator, new Vector2(pos_x, pos_y));

        float pd = Vector2.Distance(peaks[itterator].centre_position,new Vector2(pos_x, pos_y) ) * 2 / peaks[itterator].radius;
        float return_value = GM_.Instance.config.building_plot_values.minimum_height;
        if(Mathf.Abs(pd) <= 1)
            return_value = (GM_.Instance.config.building_plot_values.maximum_height - GM_.Instance.config.building_plot_values.minimum_height) / 2 + Mathf.Cos(pd * 3.14f) * (GM_.Instance.config.building_plot_values.maximum_height - GM_.Instance.config.building_plot_values.minimum_height) / 2;

        return_value = return_value < GM_.Instance.config.building_plot_values.minimum_height ? GM_.Instance.config.building_plot_values.minimum_height : return_value;

        return return_value;
    }


    //fucntion generates random peaks, based off of values set in the game manager
    public void RandomlyCreatePeaks()
    {
        Random.InitState(GM_.Instance.config.seed);

        int amount_of_peaks = Random.Range((int)GM_.Instance.config.random_peaks_values.peaks_amount.x, (int)GM_.Instance.config.random_peaks_values.peaks_amount.y);   //determine amount of peaks

        peaks.Clear();  //clear any already potential peaks

        for(int i = 0; i < amount_of_peaks; i++)    //loop for maoutn of peaks
        {
            //create new peak with random radius and position based on user defined vlaues
            Peaks new_peak = new Peaks();
            new_peak.radius = Random.Range(GM_.Instance.config.random_peaks_values.radius_size_range.x, GM_.Instance.config.random_peaks_values.radius_size_range.y);

            float x_pos, y_pos;

            x_pos = Random.Range(-GM_.Instance.config.city_limits_x, GM_.Instance.config.city_limits_x);
            y_pos = Random.Range(-GM_.Instance.config.city_limits_z, GM_.Instance.config.city_limits_z);

            new_peak.centre_position = new Vector2(x_pos, y_pos);

            peaks.Add(new_peak); //save the peak

        }
    }

    private void GetClosestPeak(ref float min_distance, ref int itterator, Vector2 position)
    {
        for (int i = 0; i < peaks.Count; i++) //loop for all peaks
        {
            float current_distance = Vector2.Distance(peaks[i].centre_position, position); //check the distance between this current position and the peak

            if (current_distance < min_distance)    //if it is closer then store the peak
            {
                min_distance = current_distance;
                itterator = i;
            }
        }
    }


}
