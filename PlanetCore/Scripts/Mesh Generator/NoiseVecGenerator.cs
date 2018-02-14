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
