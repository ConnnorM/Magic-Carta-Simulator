# Magic Carta Simulation: Players with Imperfect Recall
## Description: 
This project contains 3 parts:
1. A basic replica of Magic Carta from Tales of Graces f (based on the card game, Karuta: [Karuta Wikipedia Page](https://en.wikipedia.org/wiki/Karuta))
3. Data collection for each player. Various statistics are tracked for each player in order to generate the player's behavior profile
4. Magic Carta Solver. Given the state of the game (the players, the set of cards on the board, the set of ordered target cards, etc.), predict which player will win the overall game

## Quick Resources:
- [What is Karuta and How Do You Play It?](https://en.wikipedia.org/wiki/Karuta)
- [All official Magic Carta cards + Quotes from the Wiki](https://aselia.fandom.com/wiki/ToG_-_Magic_Carta)
- [All Magic Carta Quotes and Sound Effects Youtube Video](https://www.youtube.com/watch?v=_45cjfqUoJA)

## State Description
- From 90 possible cards, 25 are randomly selected and placed on the game board
- Each round, a target card is chosen from the first 5 cards selected for the board
- 21 <= Number of Cards <= 25
  - When a target is correctly selected, it is removed from the board and a new target is selected. 5 Rounds are played per game)
- 1 Player when collecting data, 2 when simulating outcomes
- Each player has a set of data associated with them:
  - For each card:
    - Time to get correct
    - Location: Quadrant + Distance from center of screen
    - How does rotation affect accuracy?
    - Which card? 1st, 2nd, 5th?
    - Num of incorrect guesses
    - When wrong, what was the correct guess (common confusions?)
    - How much of the card is covered up by another one? Does this slow down response time? Linearly? Exponentially?
    - rolling window: more heavily weight recent performance for learning/forgetting/recency bias
- Data is combined with some noise to generate a person's score given the board and target card
- Each player's score is compared for each round, the predicted winner has the higher score

## To Do:
- Part 1 is done (for now)
- Set up Player class
- Finalize what data to collect
  - Each player has a set of probabilities for each card
  - Time to find for each card
  - Hesitation is randomized
  - If unknown card, eliminate known cards, randomize over probabilities and unknown cards (distance-based?)
  - did the person click on the same wrong card multiple times?
  - Confusion w/ other cards
  - Randomize time to move mouse/controller and click in a 'random' spot on the card
  - keep track of anything the other player has already tried?
- Determine best data storage method
- Compare to random selection/other player profiles

## Presentation Details:
- Make a story board: what did you try and why?
- Rules overview / game demo
- This is different: it's a memory game, not a strategy game
  - "best response" is to click the right answer
    - show the ai that just instantly gets it right as fast as possible - not very interesting
  - Real question for the solver is: what does a person do when they don't know the answer?
- goal is to model real players' profiles
- Data collection explanation
- 

### Extra Ideas:
- Add game controller functionality
- Freeze player movement on incorrect guesses
- Add local multiplayer mode
- Online multiplayer
- Use player behavior profiles to make AI opponents based on real players' behaviors
  
