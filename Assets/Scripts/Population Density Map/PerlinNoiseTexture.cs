using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseTexture : MonoBehaviour
{


    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;

    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    private bool changed = false;

    // Start is called before the first frame update
    void Start()
    {
       rend = GetComponent<Renderer>();

        // Set up the texture and a Color array to hold pixels during processing.
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.mainTexture = noiseTex;
        
        

    }

    // Update is called once per frame
    void Update()
    {
        CalcNoise();
    }

    int octaves = 3;
    float persistance = 0.5f;
    float lacunarity = 2;


    float max_noise_height = float.MinValue;
    float min_noise_height = float.MaxValue;
    void CalcNoise()
    {
        // For each pixel in the texture...
        float y = 0.0F;

        while (y < noiseTex.height)
        {
            float x = 0.0F;
            while (x < noiseTex.width)
            {

                float sample = 0;

                float xCoord = (GM_.Instance.config.seed + (x /scale));
                float yCoord = (GM_.Instance.config.seed + (y / scale));
                sample = Mathf.PerlinNoise(xCoord, yCoord);


                if (sample < 0.6)
                {
                    sample -= 0.2f;
                    Mathf.Clamp01(sample);

                    //sample = Mathf.Floor(sample);
                }
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }



                // Copy the pixel data to the texture and load it into the GPU.
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }


}
