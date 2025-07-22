using UnityEngine;

namespace EditorScripting
{
    public class MyRangeAttribute : PropertyAttribute
    {
        public float min;
        public float max;

        public MyRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
