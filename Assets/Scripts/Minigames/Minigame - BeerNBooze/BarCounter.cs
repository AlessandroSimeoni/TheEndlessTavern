using UnityEngine;

namespace BeerNBooze
{
    public class BarCounter : MonoBehaviour
    {
        [SerializeField] private BeerSpawner beerSpawner = null;
        [SerializeField] private Transform pickUpArea = null;
        [SerializeField] private TapArea tapArea = null;
        [SerializeField] private Transform beerHoldTransform = null;
        [SerializeField] private Transform _playerPosition = null;

        private int _ID = -1;
        protected Beer inRangeBeer = null;
        private Beer drinkingBeer = null;
        private bool stop = false;

        public delegate void BeerLitersEvent(float currentBeerLiters, float litersDrankThisFrame);
        public event BeerLitersEvent OnBeerLitersUpdate = null;

        public delegate void BarCounterEvent();
        public event BarCounterEvent OnBeerWasted = null;
        public event BarCounterEvent OnBeerEmpty = null;
        public event BarCounterEvent OnStop = null;

        public bool beerAvailable { get; protected set; } = false;
        public Vector3 playerPosition { get { return _playerPosition.position; } }
        public bool recatchBeer { get; set; } = false;

        public int ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                tapArea.ID = value;
            }
        }

        public float speed { get; set; } = 1.0f;

        public void SpawnBeer(float size, float destroyHeight)
        {
            Beer beer = beerSpawner.SpawnBeer(speed, pickUpArea, size, destroyHeight);
            InitBeer(beer);
        }

        protected virtual void InitBeer(Beer beer)
        {
            beer.OnPlayerRange += HandleBeerInPlayerRange;
            beer.OnEmpty += StopDrinking;
            beer.OnDrinking += HandleBeerLiters;
            beer.OnBeerWasted += HandleBeerWasted;
            OnStop += beer.StopMovement;
        }

        /// <summary>
        /// called when a beer fall on the ground and is not empty
        /// </summary>
        private void HandleBeerWasted() => OnBeerWasted?.Invoke();

        /// <summary>
        /// called when drinking a beer
        /// </summary>
        /// <param name="currentBeerLiters">the current liters of the beer</param>
        /// <param name="litersDrankThisFrame">the liters drank in the frame</param>
        private void HandleBeerLiters(float currentBeerLiters, float litersDrankThisFrame)
        {
            OnBeerLitersUpdate?.Invoke(currentBeerLiters, litersDrankThisFrame);
        }

        /// <summary>
        /// called when a beer is in the range of the player
        /// </summary>
        /// <param name="inRange">true if is in range; false otherwise (beer has fallen)</param>
        /// <param name="beer">the beer</param>
        protected virtual void HandleBeerInPlayerRange(bool inRange, Beer beer)
        {
            if (!inRange)
                OnStop -= beer.StopMovement;

            if (stop)
                return;

            if (inRange || (!inRange && inRangeBeer == beer))
            {
                inRangeBeer = inRange ? beer : null;
                beerAvailable = inRange;
            }
        }

        public virtual void DrinkBeer(float drinkingSpeed, float deltaLiters, bool instantDrink)
        {
            inRangeBeer.StopMovement();
            inRangeBeer.transform.position = beerHoldTransform.position;
            drinkingBeer = inRangeBeer;
            inRangeBeer.StartDrinking(drinkingSpeed, deltaLiters, instantDrink);
            beerAvailable = recatchBeer;
        }

        public void StopDrinking()
        {
            if (drinkingBeer == null)
                return;

            drinkingBeer.StopDrinking();
            drinkingBeer.EnableGravity();
            drinkingBeer = null;
            OnBeerEmpty?.Invoke();
        }

        public void Stop()
        {
            stop = true;
            OnStop?.Invoke();
            beerAvailable = false;
        }
    }
}
