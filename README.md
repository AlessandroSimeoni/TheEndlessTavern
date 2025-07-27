# The Endless Tavern
![The Endless Tavern Promo Art](https://github.com/AlessandroSimeoni/TheEndlessTavern/blob/main/Feature_Graphic.png)  

Drink, eat and fight! Customize your knight to be the coolest in the tavern!  

The annual tournament called by the king is upon us. Knights from all over the kingdom gather in the capital to compete. Their challenges will be etched in legend!  

Hey, but-where are all the knights? No one has shown up...  

There, in The Endless Tavern!  

Oh, no...they're all DRUNK!  

They have organized their own independent tournament of eating, drinking and fighting!  

## Features
### Customization
Customize your knight with different ARMORS! Open the CHESTS and spin the WHEEL to find them! Find multiple armors of the same type to unlock new, shiny, MATERIALS!

### Tournament mode 
Earn coins and join the underground tournament organized by the knights and climb the LEADERBOARD until you reach first place!

### Modifiers
Buy modifiers to use in the single game for help breaking your RECORDS!  

### ***Watch the trailer [here](https://www.youtube.com/watch?v=OatFO_30prg).***
### ***Watch the gameplay [here](https://www.youtube.com/watch?v=rEZIL9tU2Ew).***
### ***Visit the itch.io page [here](https://digital-bros-game-academy.itch.io/endless-tavern).***
### ***Download the apk [here](https://github.com/AlessandroSimeoni/TheEndlessTavern/releases/tag/TheEndlessTavern_Release).***

<a name="What-I-did"></a>
## What I did
The game consists of 3 endless minigames with modifiers, a tournament mode with a leaderboard (not online), player customization and a shop with lootboxes.  
As a solo programmer I made all the logics of the various features present in the game; the relevant ones are:
* [Minigames](#Minigames)
* [Tournament mode](#Tournament-Mode)
* [Player customization](#Player-customization)
* [Shop](#Shop)
* [Scene loading]()


<a name="Minigames"></a>
## 1. Minigames
There are 3 endless minigames, each of which has an increasing difficulty based on some parameters contained in a scriptable object along with other settings the designers tuned to make them playable.  
From a technical point of view, each minigame has a manager that handles the game flow, the difficulty and the score update.  

* [Beer ‘n Booze](#BeerNBooze)
* [’Till last Tooth](#TillLastTooth)
* [Full up Feast](#FullUpFeast)


<a name="BeerNBooze"></a>
### 1.1 Beer ‘n Booze
In this minigame the player must drink the as much beer as he can without dropping the mugs. Each mug dropped removes 1 HP.  
There are a total of 3 bar counters, each of which has a beer spawner; the manager sees the bar counters and controls the beer spawn by calling the *SpawnBeer* method in a coroutine that spawns beer on a random bar counter every *beerSpawnDelay* seconds.  
When a beer is in range the player can hold the finger on the bar counter to drink it.  
The in range detection has been made using a dot product like this:  
```
            if (!inRange && Vector3.Dot(transform.forward, transform.position - pickUpArea.position) > rangeDetectionOffset)
            {
                OnPlayerRange?.Invoke(true, this);
                inRange = true;
            }
```
I really like using dot products, but I have to admit that I could have done this simply by checking the Z coordinate value of the beer, thus making it even more efficient.  
The drinking of a beer is handled by a coroutine (started when holding the finger and stopped when releasing it) that contains a while loop as follow:
```
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
```
You can see the scripts [here](https://github.com/AlessandroSimeoni/TheEndlessTavern/tree/main/Assets/Scripts/Minigames/Minigame%20-%20BeerNBooze).  


<a name="TillLastTooth"></a>
### 1.2 ’Till last Tooth
In this minigame the player must defeat as much enemys as he can by dodging at the right time the enemy fist attack.  
The enemies HP increases by progressing in the minigame following the settings in the scriptable object.  
The enemies are vulnerable only when they retreat the punch after the attack, so if the player dodge at the right time he automatically hit the enemy causing 1 damage.  
To detect if the player is hit or not I used a dot product as follow:
```
            // player gets damage in the center and in the direction of the hit
            if (Vector3.Dot(impactDirection, playerInstance.transform.position) >= -settings.damageDetectionOffset)
            {
                playerInstance.GetDamage();
                uiManager.UIDamage();
                playerTookDamage = true;
                OnPlayerDamage?.Invoke();
                AudioPlayer.instance.PlaySFX(playerDamageSFX);
            }
```
Both the enemies and the player derive from a `Participant` class, which contains all the variables and methods they have in common.  
When an enemy attacks, a pop-up appears on screen on the side from which the punch is coming.  
During the development I encountered numerous bugs regarding the enemy behaviour and the pop-ups; I think a state machine could have made things easier from this point of view, both for the player and the enemy.  

You can see the scripts [here](https://github.com/AlessandroSimeoni/TheEndlessTavern/tree/main/Assets/Scripts/Minigames/Minigame%20-%20TillLastTooth).  

<a name="FullUpFeast"></a>
### 1.3 Full up Feast
In this minigame the player must eat as much food as he can without leaving any leftovers or without tapping more then needed. With each tap the player eats a piece of food.  
This is the first of the three minigames I developed; by progressing in the minigame the time to eat all the food on the plate decrease and the number of food pieces increases.  
For this minigame I set up an object pooling logic for all the food.  
All the food required for the minigame is instantiated at the start of the game by a logic that takes into account the settings of the minigame:
```
        /// <summary>
        /// Instantiate the required foods in the scene
        /// </summary>
        /// <param name="maxFoodNumber">the maximum possible foods on the plate</param>
        /// <param name="maxTapRequired">the maximum possible number of tap to finish the plate</param>
        public virtual void InstantiateFood(int maxFoodNumber, int maxTapRequired)
        {
            for (int i = 0; i < foodPrefab.Length; i++)
            {
                int spawnQuantity = maxTapRequired / foodPrefab[i].tapRequired;
                if (spawnQuantity > maxFoodNumber)
                    spawnQuantity = maxFoodNumber;

                for (int j = 0; j < spawnQuantity; j++)
                {
                    Food food = Instantiate<Food>(foodPrefab[i], transform);
                    food.gameObject.SetActive(false);
                    food.transform.localPosition = foodDefaultPosition;
                    food.name = $"{foodPrefab[i].name} [{j}]";

                    foodAvailable.Add(food);
                }
            }
        }
```
When the player eats a piece of food this is not destroyed but disabled:
```
        /// <summary>
        /// Eat the food at the nextFoodPiece index in the foodPiece array
        /// </summary>
        public void Consume()
        {
            // if the food is completely consumed, invoke the event
            if (nextFoodPiece == foodPiece.Length - 1)
                OnFoodConsumed?.Invoke();

            // eat a food piece
            foodPiece[nextFoodPiece++].SetActive(false);
        }
```
The plate is prepared as follow by taking into accounts the settings of the minigame:
```
        /// <summary>
        /// Randomly prepare the food that will be put on the plate.
        /// </summary>
        /// <param name="tapRequired">the number of tap required for all the food</param>
        /// <param name="maxFoodNumber">the maximum food number that the plate can support</param>
        /// <returns>An array of Foods</returns>
        public virtual Food[] PrepareFood(int tapRequired, int maxFoodNumber)
        {
            ResetFoodStorage();

            List<Food> foodOutput = new List<Food>();
            List<Food> foodBuffer = new List<Food>();
            int remainingTaps = tapRequired;
            int tap;
            int index;
            int minPossibleTap;

            while (remainingTaps > 0)
            {
                minPossibleTap = GetMinTapRange(remainingTaps, maxFoodNumber);
                tap = Random.Range(minPossibleTap, GetMaxTapRange(remainingTaps) + 1);

                // put all the food's with that tap value in a list
                foreach (Food food in foodAvailable)
                    // if a food is not active, it means that it is available for the choice
                    if (!food.gameObject.activeInHierarchy && food.tapRequired == tap)
                        foodBuffer.Add(food);

                // pick a random food from the buffer list
                index = Random.Range(0, foodBuffer.Count);
                foodOutput.Add(foodBuffer[index]);
                foodBuffer[index].gameObject.SetActive(true);
                foodBuffer[index].ActivateFoodPieces();

                foodBuffer.Clear();
                remainingTaps -= tap;
                maxFoodNumber--;
            }

            return foodOutput.ToArray();
        }
```

Where the functions `GetMaxTapRange` and `GetMinTapRange` are as follow:
```
        /// <summary>
        /// Get the maximum number of possible taps that can be chosen considering the 
        /// remaining number of taps 
        /// </summary>
        /// <param name="remainingTaps">the remaining number of taps</param>
        /// <returns>the maximum number of taps that can be chosen</returns>
        private int GetMaxTapRange(int remainingTaps)
        {
            int max = 0;

            foreach(Food food in foodAvailable)
                // if a food is not active, it means that it is available for the choice
                if (!food.gameObject.activeInHierarchy && food.tapRequired <= remainingTaps && food.tapRequired > max)
                    max = food.tapRequired;

            return max;
        }

        /// <summary>
        /// Get the minimum number of possible tap that can be chosen considering the 
        /// remaining number of taps and the remaining number of food slots.
        /// The goal is that the sum of the tapRequired of all the foods on the plate must be 0 avoiding
        /// wasting a food slot with a food that has a tapRequired too low to achieve this.
        /// 
        /// Example (considering foods with a maximum of 3 tapRequired):
        ///     remainingTaps = 6
        ///     remainingFoodSlots = 2
        ///     
        /// --> if a food with 2 tapRequired is chosen:
        ///     remainingTaps = 6 - 2 = 4
        ///     remainingFoodSlots = 2 - 1 = 1
        ///     
        /// --> having foods with a maximum of 3 tapRequired, now we cannot cover all the remainingTaps with only 1 slot;
        ///     we must avoid this situation!
        /// </summary>
        /// <param name="remainingTaps">the number of remaining taps</param>
        /// <param name="remainingFoodSlots">the number of remaining food slots</param>
        /// <returns>the mininum number of taps that can be chosen</returns>
        private int GetMinTapRange(int remainingTaps, int remainingFoodSlots)
        {
            // if remains only 1 slot then return the remainig taps
            if (remainingFoodSlots == 1)
                return remainingTaps;

            // get the maximum number of tap available between the foods
            int maxFoodTapPossible = GetMaxTapRange(remainingTaps);
            int minFoodTapPossible = remainingTaps;

            // get the minimum tap available
            foreach (Food food in foodAvailable)
                // if a food is not active, it means that it is available for the choice
                if (!food.gameObject.activeInHierarchy && food.tapRequired < minFoodTapPossible)
                    minFoodTapPossible = food.tapRequired;

            
            int output = maxFoodTapPossible;
            for (int i = minFoodTapPossible; i <= maxFoodTapPossible; i++)
            {
                if (Mathf.CeilToInt((remainingTaps - i) / (float)(remainingFoodSlots - 1)) > maxFoodTapPossible)
                    continue;

                output = i;
                break;
            }

            return output;
        }
```

You can see the scripts [here](https://github.com/AlessandroSimeoni/TheEndlessTavern/tree/main/Assets/Scripts/Minigames/Minigame%20-%20FullUpFeast).  

**[⬆ Back to Top](#What-I-did)**

<a name="Tournament-Mode"></a>
## 2. Tournament mode
The tournament consists of playing all the three minigames in sequence with 1 HP for each one of them.  
The score made in each minigame is added to the final score of the tournament. Once the last minigame is finished the total score is put on an offline leaderboard the player can see in the menu.  
The score is saved along with other informations like records, settings and the armor equipped.  

You can see the scripts [here](https://github.com/AlessandroSimeoni/TheEndlessTavern/tree/main/Assets/Scripts/Minigames/Tournament).  

**[⬆ Back to Top](#What-I-did)**


<a name="Player-customization"></a>
## 3. Player customization

**[⬆ Back to Top](#What-I-did)**



<a name="Shop"></a>
## 4. Shop

**[⬆ Back to Top](#What-I-did)**

<a name="SceneLoading"></a>
## 5. Scene Loading

**[⬆ Back to Top](#What-I-did)**




