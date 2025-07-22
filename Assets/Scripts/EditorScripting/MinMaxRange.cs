namespace EditorScripting
{
    [System.Serializable]
    public class MinMaxRange
    {
        public float min;
        public float max;
        public float rangeMin = 0.0f;
        public float rangeMax = 20.0f;

        public MinMaxRange() { }

        public MinMaxRange(float rangeMin, float rangeMax)
        {
            this.rangeMin = rangeMin;
            this.rangeMax = rangeMax;
        }
    }
}
