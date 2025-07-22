using System.Collections;
using UnityEngine;

namespace Camera
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private float shakeTime = 0.5f;
        [SerializeField] private AnimationCurve shakeCurveXAxis;
        [SerializeField] private AnimationCurve shakeCurveYAxis;

        private Vector3 startingPosition = Vector3.zero;

        [ContextMenu("Shake")]
        public void Shake() => StartCoroutine(ShakeCoroutine());

        private IEnumerator ShakeCoroutine()
        {
            float xPos;
            float yPos;
            float timer = 0.0f;

            startingPosition = transform.position;

            while (timer <= shakeTime)
            {
                xPos = shakeCurveXAxis.Evaluate(timer / shakeTime);
                yPos = shakeCurveYAxis.Evaluate(timer / shakeTime);
                transform.position = new Vector3 (startingPosition.x + xPos, startingPosition.y + yPos, startingPosition.z);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            yield return null;
            transform.position = startingPosition;
        }
    }
}
