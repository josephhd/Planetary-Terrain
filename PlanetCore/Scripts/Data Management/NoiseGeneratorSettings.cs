[System.Serializable]
public class NoiseSettings {
    public double amplitude;
    public double frequency;
    public double lacunarity;
    public double persistence;
    public int octaves;
    public int seed;
    public LibNoise.QualityMode quality;
};