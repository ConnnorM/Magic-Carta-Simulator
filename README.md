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
- Data is combined with some noise to generate a person's score given the board and target card
- Each player's score is compared for each round, the predicted winner has the higher score

## To Do:
- Part 1 is done (for now)
- Set up Player class
- Finalize what data to collect
- Determine best data storage method

### Extra Ideas:
- Add game controller functionality
- Freeze player movement on incorrect guesses
- Add local multiplayer mode
- Online multiplayer
- Use player behavior profiles to make AI opponents based on real players' behaviors
  
