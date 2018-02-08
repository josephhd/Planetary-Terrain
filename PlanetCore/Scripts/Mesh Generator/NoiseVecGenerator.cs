using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise.Generator;
using VectorDoubles;

namespace NoiseVecGenerator {
    public class NoiseVector {
        private Perlin p;

        public NoiseVector (NoiseSettings noiseSettings) {
            p = new Perlin(noiseSettings.frequency, noiseSettings.lacunarity, noiseSettings.persistence, noiseSettings.octaves, noiseSettings.seed, noiseSettings.quality);
        }

        public Vector3Double GetNoiseVector (Vector3Double coord) {
            return coord * p.GetValue(coord.x, coord.y, coord.z);
        } 
    }
}
