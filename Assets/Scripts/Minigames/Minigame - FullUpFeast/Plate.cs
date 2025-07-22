using Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Audio;

namespace FullUpFeast
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LinearMovement))]
    public class Plate : MonoBehaviour
    {
        [SerializeField] private FoodPositionSettings foodPositionSettings = null;
        [SerializeField] protected Slider timeSlider = null;
        [Header("Audio")]
        [SerializeField] protected AudioClip plateInSFX = null;
        [SerializeField] private AudioClip plateOutSFX = null;
        [SerializeField] private AudioClip eatSFX = null;

        protected Food[] foodOnPlate = new Food[0];
        protected LinearMovement linearMovement = null;
        protected bool plateServed = false;
        protected int currentFoodToConsume = 0;
        private Coroutine waitingCoroutine = null;
        private AudioSource eatAudioSource = null;

        public delegate void PlateEvent();
        public event PlateEvent OnEmptyPlate = null;
        public event PlateEvent OnFoodPieceConsumed = null;

        public delegate void PlateEscaped(int leftovers);
        public event PlateEscaped OnPlateEscaped = null;

        public int maxPossibleFoods
        {
            get { return foodPositionSettings.foodPosition[foodPositionSettings.foodPosition.Length - 1].numberOfFoodOnPlate; }
        }
        public float maxWaitingTime { private get; set; } = 0.0f;

        private void Awake()
        {
            Assert.IsNotNull<FoodPositionSettings>(foodPositionSettings, "Food positions are required");

            linearMovement = GetComponent<LinearMovement>();
            linearMovement.OnStop += HandleMovementStop;
            linearMovement.OnMovementResumed += HandleMovementResumed;
            linearMovement.OnStartAndStopMovementEnd += HandleEndingMovement;
            linearMovement.OnWaiting += SetTimeSlider;
        }

        public void SetTimeSlider(float ratio) => timeSlider.value = ratio;


        /// <summary>
        /// subscribe to all the food's OnFoodConsumed event to know when a food is completely ate
        /// </summary>
        private void SubscribeToFoodEvent()
        {
            foreach (Food food in foodOnPlate)
                food.OnFoodConsumed += HandleFoodConsumed;
        }

        /// <summary>
        /// called when the plate stops the movement
        /// </summary>
        protected virtual void HandleMovementStop()
        {
            plateServed = true;
            AudioPlayer.instance.PlaySFX(plateInSFX);
        }
        /// <summary>
        /// called when the plate resume the movement 
        /// </summary>
        private void HandleMovementResumed()
        {
            plateServed = false;
            AudioPlayer.instance.PlaySFX(plateOutSFX);
        }
        /// <summary>
        /// called when the plate escapes and reach the destination position
        /// </summary>
        private void HandleEndingMovement()
        {
            int leftovers = 0;
            for (int i = currentFoodToConsume; i < foodOnPlate.Length; i++)
            {
                foodOnPlate[i].OnFoodConsumed -= HandleFoodConsumed;
                leftovers += foodOnPlate[i].leftovers;
            }

            foodOnPlate = null;
            OnPlateEscaped?.Invoke(leftovers);
        }

        /// <summary>
        /// Unsubscribe from OnFoodConsumed event and update the current food to consume
        /// </summary>
        private void HandleFoodConsumed()
        {
            foodOnPlate[currentFoodToConsume++].OnFoodConsumed -= HandleFoodConsumed;

            float remainingWaitingTime = linearMovement.stopTime - linearMovement.waitingTime;
            if (currentFoodToConsume >= foodOnPlate.Length && remainingWaitingTime > maxWaitingTime)
                waitingCoroutine = StartCoroutine(PlateEmptyWaitingCoroutine());

        }

        protected virtual IEnumerator PlateEmptyWaitingCoroutine()
        {
            yield return new WaitForSeconds(maxWaitingTime);
            linearMovement.StopWaitingTime();
            waitingCoroutine = null;
        }

        /// <summary>
        /// Consume the food or invoke the event and do nothing else if the plate is empty
        /// </summary>
        public void ConsumePieceOfFood()
        {
            if (!plateServed)
                return;

            if (currentFoodToConsume >= foodOnPlate.Length)
            {
                OnEmptyPlate?.Invoke();
                return;
            }

            foodOnPlate[currentFoodToConsume].Consume();

            if (eatAudioSource == null)
                eatAudioSource = AudioPlayer.instance.InitSFX(eatSFX);
            else
                eatAudioSource.Stop();

            eatAudioSource.Play();

            OnFoodPieceConsumed?.Invoke();
        }

        /// <summary>
        /// Set up the food on the plate
        /// </summary>
        /// <param name="foodList">the food list to set up</param>
        public void SetUpFood(Food[] foodList)
        {
            currentFoodToConsume = 0;
            foodOnPlate = foodList;
            PositionFood();
            SubscribeToFoodEvent();
            SetTimeSlider(1.0f);
            linearMovement.Move();
        }

        /// <summary>
        /// Randomly position the food on the plate using the foodPositionSettings
        /// </summary>
        private void PositionFood()
        {
            // change parenting so that the food will move with the plate
            foreach (Food food in foodOnPlate)
                food.transform.parent = this.transform;

            int settingsIndex = Array.FindIndex(foodPositionSettings.foodPosition,
                                                x => x.numberOfFoodOnPlate == foodOnPlate.Length);

            // randomly position the food on the plate
            List<Vector3> availablePositions = foodPositionSettings.foodPosition[settingsIndex].position.ToList<Vector3>();
            int pos = 0;
            for(int i = 0; i < foodOnPlate.Length; i++)
            {
                pos = Random.Range(0, availablePositions.Count);
                foodOnPlate[i].transform.localPosition = availablePositions[pos];
                availablePositions.RemoveAt(pos);
            }

            //randomly rotate the food on the plate
            for (int i = 0; i < foodOnPlate.Length; i++)
            {
                Vector3 eulerRotation = foodOnPlate[i].transform.rotation.eulerAngles;

                // rotate around x axis
                if (foodOnPlate[i].xRotationEnabled)
                    eulerRotation.x = Random.Range(foodPositionSettings.xAxisRotation.min, foodPositionSettings.xAxisRotation.max);

                // rotate around y axis
                if (foodOnPlate[i].yRotationEnabled)
                    eulerRotation.y = Random.Range(foodPositionSettings.yAxisRotation.min, foodPositionSettings.yAxisRotation.max);

                // rotate around z axis
                if (foodOnPlate[i].zRotationEnabled)
                    eulerRotation.z = Random.Range(foodPositionSettings.zAxisRotation.min, foodPositionSettings.zAxisRotation.max);

                foodOnPlate[i].transform.rotation = Quaternion.Euler(eulerRotation);
            }
        }

        public void SetEscapeSpeed(float value) => linearMovement.stopTime = value;

        public void Stop()
        {
            linearMovement.Stop();
            plateServed = false;
        }

        public void ForceEscape()
        {
            if (waitingCoroutine != null)
                StopCoroutine(waitingCoroutine);

            AudioPlayer.instance.PlaySFX(plateOutSFX);
            linearMovement.StopWaitingTime();
        }
    }
}
