
# Gambetto

## Team

- Martino Piaggi: Developer -> [martinopiaggi](https://github.com/martinopiaggi), martino.piaggi@mail.polimi.it
- Lorenzo Morelli: Developer-> [lorenzo-morelli](https://github.com/lorenzo-morelli), lorenzo.morelli@mail.polimi.it
- Matteo Laini: Developer -> [matteolaini](https://github.com/matteolaini), matteo.laini@mail.polimi.it
- Milo Brontesi: Developer -> [zibasPk](https://github.com/zibasPk),milo.brontesi@mail.polimi.it
- Mario Vallone: Developer -> [Mario2414](https://github.com/Mario2414), mario.vallone@mail.polimi.it 


## Overview and Vision Statement

Gambetto combines the strategic depth of chess with the frenetic pace of arcade and rhythm games.
Players take on the role of a chess pawn facing various challenges. As they navigate different types of dungeons, they’ll need to face enemy pieces and to get to the end of different rooms. Players can transform using power-ups and gain the abilities of other chess pieces, introducing layers of strategy and gameplay depth.
All pieces in the game will move following a certain rhythm; players will need to time their actions to choose moves.
The player also needs to avoid falling out from the chessboard, otherwise he will restart the level from the latest checkpoint.
Levels are designed and ordered with increasing difficulty.
Any level will be considered completed only if the player reaches the end uneaten. As a consequence, the next level will be unlocked and playable.


### Genre

Arcade-Strategy Hybrid with Rhythm elements

### Platforms

PC, Mobile

### Market Analysis
The game's visual aesthetics will draw inspiration from Monument Valley while blending rhythm-based elements with arcade mechanics reminiscent of games like Crossy Road.


## Gameplay

### Game Flow
In a typical game players commence a level in the role of a pawn, and they must strategize their moves by carefully timing their inputs. The available move is visually highlighted on the ground, and changes cycling through an array of moves over a set duration.
Using this game mechanic, players navigate the dungeon, progressing through various rooms while avoiding enemy pieces and environmental hazards. If a player's character succumbs to these challenges, they are reset to a designated checkpoint at the start of the room they were in. Certain rooms contain power-ups essential for progressing through the map. Completion of each level grants access to the next one.

### Core Mechanics

- Chess-inspired Movement: Navigate using the movement rules of chess pieces.
- Movement Rhythms: Sync your moves with the ticking of an in-game clock. Mistime your move, and face the consequences.
- Chess Power-ups: Collect transparent chess pieces to temporarily gain their movement abilities.
- Enemy Elimination: Rare power-ups allow players to eliminate certain enemies.
- Quick Levels: Short, intense levels, especially during the initial phases.
- Check Points: Checkpoints between different sections (rooms) of a level.


### Challenges
- Time Pressure: Players must make their move within a time limit or remain stationary.
- Dynamic Enemies: Some move in sync with the player, while others have their unique rhythms.
- Dual-Pawn Control: Maybe in specific levels, control two pawns simultaneously, adding a layer of complexity and strategy.
- Rotating Rooms: Dynamic rooms that periodically rotate, altering paths and challenges.

### Level Design
Levels will be formed by a succession of rooms each with different challenges and hazards,
rooms are made of chess board style tiles and are surrounded by a foggy void.
Rooms exhibit unique, irregular shapes and may incorporate openings within their design. Distinct levels are set apart by alterations in tile patterns, lighting, and fog effects, presenting varying colors to immerse players in diverse environments. Within each room, enemies are strategically positioned, and players will find checkpoints to return to upon death.

## User Interface
The main menu will double as the level selection interface, offering players a side view of rooms that exhibit characteristics related to each level, similar to the design seen in the game Smash Hit. [Smash Hit example](https://www.youtube.com/watch?v=8Sb8wIWeM2E)

The in-game user interface will maintain a minimalistic design, featuring only a select few elements, including a pause button and a timer that tracks the elapsed time within the level.

## Characters

- The Pawn (Player): The main character who embarks on this journey, harnessing the powers of other pieces.
- Enemy Pieces: Various chess pieces, each with unique movement patterns, challenge the player throughout the journey.

## Story

In a surreal chess world, a lone pawn seeks to become a king. To do so, it must navigate a maze of challenges and adversaries, learning from other pieces and growing in strength.

## World

The setting is an abstract, ever-changing chessboard. Each level is a unique dungeon, some of which rotate or shift, challenging the player's spatial awareness.
In each level the player will face a chessboard with different dimension and sometimes different colors compared to the previous one

## Art and Visual Style
The game's artistic style embraces a Low Poly Minimalist approach, infusing it with a hint of surrealism. Inspired by "Monument Valley".
Lighting and chess board colors may vary depending on the level.
The game camera will have an isometric view of the map and will follow the player.

## Music
The game's music sets the mood with an ambient composition, where rhythmic elements echoing the in-game clock's tick, creating a soothing and immersive experience for players.

## Sound Effects
Discrete, emphasizing movements, captures, and transformations.


Gambetto Project Structure Overview

1. Materials
- Two distinct chessboard materials: Dark and Light. These could represent the two different colored squares on a chessboard.
- background fog 
- gradient background 
- maybe the player can choose to be white/black piece. And if black it's hard because we can make that for the first turn only the enemies (white) moves (like in chess white move first)


Scenes

- Multiple scenes including:
- A main menu for game navigation.
- Several testing or development scenes, like "Prova" and "SampleScene", indicating ongoing development and experimentation.

Scripts
- Main Game Logic:
- Scripts for audio management, cell interactions, grid management, level flow, main menu functionality, and UI management.

- Chess Piece Logic:
- A script for the basic behavior of chess pieces.

- Room Logic:
- Scripts related to room behavior and layout, indicating dynamic and varied level designs.
- Manually design rooms and concatenate rooms layouts/specifications easily in each level 

- Utilities:
- Several utility scripts for constants, debugging, and movement directions, among others. These are crucial for game functionality and development efficiency.

- Sounds
- Audio files for theme music and sound effects, which will contribute to the game's ambiance and player feedback.


## Technical Specification
- Engine: Unity (ideal for both PC and Mobile platforms).

- Graphics: 3D low poly

- Controls:
The player can only use one keyboard key (or screen tap) to select his move. He needs to press it when the light displayed on the terrain corresponds to his desired direction.
Initially the pawn can only move one step in each direction (excluding diagonal moves), but the game will provide some power-ups to increase the variety of the movement.
In this last case the player will be also able to select the destination cell always in the frenetic way that distinguishes the game.

## Deadlines

### Week 1 (October 10 - October 17, 2023)
- Team formation and initial brainstorming sessions.
- Begin work on the game's core mechanics.
- Assign specific roles and tasks to team members.

Team Formation
Initial Brainstorming
Task Assignment


### Week 2 (October 17 - October 24, 2023)
- Finalize and solidify game mechanics.
- Begin work on game aesthetics and sound.
- Hold a team meeting to discuss progress and potential changes.

Core Mechanics Development
Aesthetics Brainstorm
Sound Conceptualization

### Week 3 (October 24 - October 31, 2023)
- Refine the game's core mechanics and gameplay loop.
- Expand on the game's aesthetics and sound design.
- Create the initial layout of levels and challenges.

Level Design
Mechanics Testing
Feedback Integration

### Week 4 (October 31 - November 6, 2023)
- Finalize the game design document.
- Set up the git repository and ensure all members have access.
- Send the game design document and git repository link to pierluca.lanzi@polimi.it.

Documentation
Repository Setup
Milestone Review



### Week 5-9 (November 6 - December 10, 2023)
- Develop the game prototype.
- Test and refine gameplay mechanics.
- Implement feedback from team members and playtesters.

Prototype Development
Aesthetics Implementation
Regular Playtesting
Notion Task Reviews



### Week 10 (December 10 - December 20, 2023)
- Submit the game prototype.
- Await feedback and prepare for potential revisions.

Prototype Submission
Feedback Gathering


### Week 11-13 (December 20, 2023 - January 13, 2024)
- Refine and expand the game based on feedback.
- Develop the beta version of the game.
- Test and polish game elements for the beta submission.
Beta Development
Bug Fixing
Notion Task Updates


Beta Testing
Feedback Implementation
Final Task Review



### Week 14-16 (January 13 - February 23, 2024)
- Await feedback on the beta.
- Implement changes and refinements based on feedback.

Beta Testing
Feedback Implementation
Final Task Review


### Week 17 (February 23 - February 27, 2024)
- Final touches and last-minute refinements.
- Submit the final project.
