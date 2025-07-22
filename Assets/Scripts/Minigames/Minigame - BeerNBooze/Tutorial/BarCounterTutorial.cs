using System.Collections;
using UnityEngine;

namespace BeerNBooze
{
    public class BarCounterTutorial : BarCounter
    {
        [Header("TapArea")]
        [SerializeField] private MeshRenderer tapAreaRenderer = null;
        [SerializeField] private float targetAlpha = 0.20f;
        [SerializeField] private float alphaFrequency = 1.0f;

        private Coroutine tapAreaCoroutine = null;

        private const float BEER_DETECTION_OFFSET = 0.55f;

        public delegate void BeerOutOfRange(bool empty);
        public event BeerOutOfRange OnBeerOutOfRange = null;
        
        public delegate void BeerInRange();
        public event BeerInRange OnBeerInRange = null;

        protected override void InitBeer(Beer beer)
        {
            base.InitBeer(beer);
            beer.rangeDetectionOffset = BEER_DETECTION_OFFSET;
        }

        protected override void HandleBeerInPlayerRange(bool inRange, Beer beer)
        {
            beer.StopMovement();

            base.HandleBeerInPlayerRange(inRange, beer);

            if (inRange)
            {
                tapAreaCoroutine = StartCoroutine(HighlightTapArea());
                OnBeerInRange?.Invoke();
            }
            else
                OnBeerOutOfRange?.Invoke(beer.currentBeerLiters == 0.0f);
        }

        public override void DrinkBeer(float drinkingSpeed, float deltaLiters, bool instantDrink)
        {
            StopCoroutine(tapAreaCoroutine);
            tapAreaRenderer.gameObject.SetActive(false);
            base.DrinkBeer(drinkingSpeed, deltaLiters, instantDrink);
        }

        private IEnumerator HighlightTapArea()
        {
            float interpolation;
            float currentTime = 3/(4*alphaFrequency);   // sin evaluates to -1 here, in this way the interpolation can start from 0
            Material mat = tapAreaRenderer.material;

            Color startingColor = mat.color;
            startingColor.a = 0.0f;
            mat.color = startingColor;

            Color targetColor = mat.color;
            targetColor.a = targetAlpha;

            tapAreaRenderer.gameObject.SetActive(true);

            while (true)
            {
                currentTime = (currentTime + Time.deltaTime) % (1.0f/alphaFrequency);
                interpolation = (Mathf.Sin(2 * Mathf.PI * alphaFrequency * currentTime) + 1) * 0.5f;
                mat.color = Color.Lerp(startingColor, targetColor, interpolation);
                yield return null;
            }
        }
    }
}
