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

**[⬆ Back to Top](#What-I-did)**

<a name="Tournament-Mode"></a>
## 2. Tournament mode

**[⬆ Back to Top](#What-I-did)**


<a name="Player-customization"></a>
## 3. Player customization

**[⬆ Back to Top](#What-I-did)**



<a name="Shop"></a>
## 4. Shop

**[⬆ Back to Top](#What-I-did)**






