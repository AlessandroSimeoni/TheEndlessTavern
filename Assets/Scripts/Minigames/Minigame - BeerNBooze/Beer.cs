using Audio;
using Movement;
using System.Collections;
using UnityEngine;

namespace BeerNBooze
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LinearMovement))]
    [RequireComponent(typeof(Gravity))]
    public class Beer : MonoBehaviour
    {
        public float rangeDetectionOffset = 0.0f;
        public GameObject beerGraphic = null;
        [SerializeField] private AudioClip drinkSFX = null;
        [SerializeField] private AudioClip beerEmptySFX = null;

        private LinearMovement linearMovement = null;
        private Gravity gravity = null;
        private Coroutine drinkingCoroutine = null;
        private float _size = 1.0f;
        private bool inRange = false;
        private AudioSource drinkAudioSource = null;

        public float currentBeerLiters { get; private set; } = 1.0f;

        public float size 
        {
            set 
            {
                _size = value;
                currentBeerLiters = value;
            }
        }
        public float destructionHeight { private get; set; } = 0.1f;
        public float movementSpeed { private get; set; } = 1.0f;
        public Transform pickUpArea { private get; set; } = null;

        public delegate void BeerEvent();
        public event BeerEvent OnEmpty = null;
        public event BeerEvent OnBeerWasted = null;

        public delegate void InRangeEvent(bool inRange, Beer beer);
        public event InRangeEvent OnPlayerRange = null;

        public delegate void DrinkEvent(float currentLiters, float litersDrankInFrame);
        public event DrinkEvent OnDrinking = null;

        private void Awake()
        {
            linearMovement = GetComponent<LinearMovement>();
            gravity = GetComponent<Gravity>();
        }

        private void Start()
        {
            linearMovement.startingPosition = transform.position;
            linearMovement.movementSpeed = movementSpeed;
            linearMovement.Move();
        }

        private void Update()
        {
            // if is in the pick up area the beer is ready to be drank
            if (!inRange && Vector3.Dot(transform.forward, transform.position - pickUpArea.position) > rangeDetectionOffset)
            {
                OnPlayerRange?.Invoke(true, this);
                inRange = true;
            }

            // handle beer destruction
            if (transform.position.y < destructionHeight)
            {
                if (currentBeerLiters == _size)
                    OnBeerWasted?.Invoke();

                OnPlayerRange?.Invoke(false, this); // not in range anymore
                if(drinkAudioSource != null)
                    Destroy(drinkAudioSource.gameObject);
                Destroy(gameObject);
            }
        }

        public void StartDrinking(float drinkingSpeed, float deltaLiters, bool instantDrink)
        {
            // handle the instant drink bonus
            if (instantDrink)
            {
                OnDrinking?.Invoke(0.0f, currentBeerLiters);
                currentBeerLiters = 0.0f;
                OnEmpty?.Invoke();
                AudioPlayer.instance.PlaySFX(beerEmptySFX);
            }
            else
                drinkingCoroutine = StartCoroutine(Drink(drinkingSpeed, deltaLiters));
        }

        public void StopDrinking()
        {
            if (drinkingCoroutine == null)
                return;

            if (drinkAudioSource != null)
                drinkAudioSource.Stop();
            StopCoroutine(drinkingCoroutine);
            drinkingCoroutine = null;
        }

        private IEnumerator Drink(float drinkingSpeed, float deltaLiters)
        {
            float litersToDrinkThisFrame;

            // handle drink sfx, spawn only one time per beer to avoid continuos instantiate/destroy of the sfx gameobject
            if (drinkAudioSource == null)
                drinkAudioSource = AudioPlayer.instance.LoopSFX(drinkSFX);
            else
                drinkAudioSource.Play();

            while (true)
            {
                litersToDrinkThisFrame = Time.deltaTime * (deltaLiters / drinkingSpeed);

                if (litersToDrinkThisFrame > currentBeerLiters)
                    litersToDrinkThisFrame = currentBeerLiters;

                currentBeerLiters -= litersToDrinkThisFrame;

                // update observers on the liters drank
                OnDrinking?.Invoke(currentBeerLiters, litersToDrinkThisFrame);
                
                // handle beer when empty
                if (currentBeerLiters == 0)
                {
                    OnEmpty?.Invoke();
                    if(drinkAudioSource != null)
                        drinkAudioSource.Stop();
                    AudioPlayer.instance.PlaySFX(beerEmptySFX);
                    break;
                }

                yield return null;
            }

            drinkingCoroutine = null;
        }

        public void StopMovement()
        {
            linearMovement.Stop();
            gravity.enabled = false;
        }

        public void EnableGravity()
        {
            gravity.currentAppliedGravity = Vector3.zero;
            gravity.enabled = true;
        }
    }
}
