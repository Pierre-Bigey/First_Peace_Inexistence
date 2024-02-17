namespace FirstPeace.Scripts
{
    public class CustomRandom
    {
        public class GaussianRandom
        {
            private float _mean;
            private float _stdDev;
            private System.Random _random;

            public GaussianRandom(float mean, float stdDev)
            {
                _mean = mean;
                _stdDev = stdDev;
                _random = new System.Random();
            }

            public float Next()
            {
                double u1 = 1.0 - _random.NextDouble();
                double u2 = 1.0 - _random.NextDouble();
                double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                                      System.Math.Sin(2.0 * System.Math.PI * u2);
                return (float)(_mean + _stdDev * randStdNormal);
            }
            
            public float Next(float mean, float stdDev)
            {
                double u1 = 1.0 - _random.NextDouble();
                double u2 = 1.0 - _random.NextDouble();
                double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                                       System.Math.Sin(2.0 * System.Math.PI * u2);
                return (float)(mean + stdDev * randStdNormal);
            }
        }
    }
}