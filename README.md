# Magic Carta Simulation: Players with Imperfect Recall
## Description: 
This project contains 3 parts:
1. A replica of Magic Carta from Tales of Graces f (based on the card game, Karuta: [Karuta Wikipedia Page](https://en.wikipedia.org/wiki/Karuta))
3. Data collection for each player. Various statistics are tracked for each player in order to generate the player's behavior profile
4. Magic Carta Solver. Given the state of the game (the player, the set of cards on the board, each card's position, etc.), predict how long it will take the player to select the target card.

## Quick Resources:
- [What is Karuta and How Do You Play It?](https://en.wikipedia.org/wiki/Karuta)
- [All official Magic Carta cards + Quotes from the Wiki](https://aselia.fandom.com/wiki/ToG_-_Magic_Carta)
- [All Magic Carta Quotes and Sound Effects Youtube Video](https://www.youtube.com/watch?v=_45cjfqUoJA)

## State Description
- From 90 possible cards, 25 are randomly selected and placed on the game board in random locations and rotations
- Each round, a target card is chosen from the first 5 cards selected for the board
  - When a target is correctly selected, it is removed from the board and a new target is selected. 5 Rounds are played per game)
  - 21 <= Number of Cards <= 25
- 1 player

## Data Collection
In each round, data is collected for each player:
### Implemented
- session_id: unique session for each game (5 rounds)
- player_id: unique tag for players used to generate their player profile
- round: which round in the game (1-5)
- target_card_id: id # for the target card
- target_card_name: the name of the target card
- found_target: 1 if the player found the card, 0 otherwise
- time_to_correct: time from quote1 playing to selecting the target card (blank if the player lost the round)
- distance: euclidean distance from player's cursor's starting position to the target card
- editor: 1 if playing in the Unity Editor, 0 otherwise
### To Do:
- target_location_x: x position of the target on screen
- target_location_y: y position of the target on screen
- target_rotation: rotation of the target card
- count_incorrect_guesses: how many incorrect guesses were made
- incorrect_guesses: list of cards that were guessed incorrectly
- target_visibility: percentage of the target card that is visible (cards can overlap)
- cursor_input_type: mouse, joystick, gamepad, keyboard, etc.
- time_to_first_move: time from quote1 begin to user's first directional input
- Player -> Card Affinity Mapping
  - For each card, has the player played that game?
- Card Similarity Mapping
  - Character's age, gender, hair color, dominant colors,
- num_distractors: how many distractors are on the board 

## Simulation / Prediction
Goal is to simulate a particular player's behavior by predicting how long it will take them to select the target card
- Rolling window: more heavily weight recent performance for learning/forgetting/recency bias
- Add random noise: hesitation, distractor effects, position of click on target, 
- Data is combined with some noise to generate a person's score given the board and target card

## To Do:
- Determine simulation/prediction strategy
  - Giant formula with weights
      - Use MC to generate weights?
- Finish bare minimum data collection
- Set up simulation/prediction and data visualization

## Presentation Details:
- Make a story board: what did you try and why?
- Rules overview / game demo
- This is different: it's a memory game, not a strategy game
  - "best response" is to click the right answer
    - show the ai that just instantly gets it right as fast as possible - not very interesting
  - Real question for the solver is: what does a person do when they don't know the answer?
- goal is to model real players' profiles
- Data collection explanation

### Post-Project Features:
- Tune card placement on board
- Target spins away from hand on select
- Hand-cursor shadow
- P1/2/3/4 wins graphics
- Animate points
- Menu sounds
- Main menu, playerid setting, mode, etc.
- Fix song looping (base song is quiet at the end)
- Text fonts
- Return cursor graphic vs where the click registers
- Add local multiplayer mode
- Online multiplayer
- Use player behavior profiles to make AI opponents based on real players' behaviors
