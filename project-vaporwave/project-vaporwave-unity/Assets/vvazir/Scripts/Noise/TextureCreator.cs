﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreator : MonoBehaviour
{
    [Range(2, 512)]
    public int resolution = 256;
    [Range(1, 3)]
    public int dimensions = 3;
    public float frequency = 128f;
    [Range(1, 8)]
    public int octaves = 1;
    [Range(1f, 4f)]
    public float lacunarity = 2f;
    [Range(0f, 1f)]
    public float persistence = 0.5f;
    public Gradient coloring;
    public NoiseMethodType type;
    private Texture2D texture;

    private void Update()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            FillTexture();
        }
    }


    private void OnEnable()
    {
        if (texture == null)
        {
            texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true)
            {
                name = "Procedural Texture",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Trilinear,
                anisoLevel = 9
            };
            GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
        FillTexture();
    }
    
    public void FillTexture()
    {
        if (texture.width != resolution)
        {
            texture.Resize(resolution, resolution);
        }

        Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f, -0.5f));
        Vector3 point10 = transform.TransformPoint(new Vector3(0.5f, -0.5f));
        Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
        Vector3 point11 = transform.TransformPoint(new Vector3(0.5f, 0.5f));

        float stepSize = 1f / resolution;
        NoiseMethod method = Noise.noiseMethods[(int)type][dimensions - 1];
        for (int y = 0; y < resolution; y++)
        {
            Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
            Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);

            for (int x = 0; x < resolution; x++)
            {
                Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
                float sample = Noise.Sum(method,point, frequency,octaves,lacunarity,persistence).value;
                if (type != NoiseMethodType.Value)
                {
                    sample = sample * 0.5f + 0.5f;
                }
                texture.SetPixel(x, y,
                    //new Color(point.x,point.y,point.z)
                    coloring.Evaluate(sample)
                    );
            }
        }
        texture.Apply();
    }
}
