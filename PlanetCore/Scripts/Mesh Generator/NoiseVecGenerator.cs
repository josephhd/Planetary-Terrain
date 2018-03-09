using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise.Generator;
using VectorDoubles;

namespace NoiseVecGenerator {
    public class NoiseVector {
        private Perlin p;
        private NoiseSettings _noiseSettings;

        public NoiseVector (NoiseSettings noiseSettings) {
            _noiseSettings = noiseSettings;
            p = new Perlin(noiseSettings.frequency, noiseSettings.lacunarity, noiseSettings.persistence, noiseSettings.octaves, noiseSettings.seed, noiseSettings.quality);
        }

        public double GetNoiseValue(Vector3Double coord) {
            return _noiseSettings.amplitude * p.GetValue(coord.x, coord.y, coord.z);
        } 
    }
}

public enum NoiseType {
    PERLIN,
    VORONOI
};

[System.Serializable]
public class NoiseModule {
    public NoiseType noise;
    /*public NoiseType NoiseSetting {
        get { return noise; }
    }*/

    private Perlin p;
    private Voronoi v;

    public double GetSampleValue (double x, double y) {
        double ret;

        switch (noise) {
        case NoiseType.PERLIN:
                ret = p.GetValue(x, y, 0);
                break;

        case NoiseType.VORONOI:
                ret = v.GetValue(x, y, 0);
                break;

        default:
                ret = p.GetValue(x, y, 0);
                break;
        }

        return ret;
    }

    public float GetSampleValue (Vector3Double coord) {
        return 0;
    }

    public NoiseModule (NoiseType type, NoiseSettings noiseSettings) {
        noise = type;
        switch (noise) {
            case NoiseType.PERLIN:
                p = new Perlin(noiseSettings.frequency, noiseSettings.lacunarity, noiseSettings.persistence, noiseSettings.octaves, noiseSettings.seed, noiseSettings.quality);
                v = null;
                break;

            case NoiseType.VORONOI:
                p = null;
                v = new Voronoi(noiseSettings.frequency, noiseSettings.displacment, noiseSettings.seed, true);
                break;

            default:
                p = new Perlin(0.5, 1, 2, 1, 0, LibNoise.QualityMode.Low);
                v = null;
                break;
        }
    }
}