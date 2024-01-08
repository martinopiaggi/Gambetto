# Gambetto

## Team

- Martino Piaggi: Developer -> [martinopiaggi](https://github.com/martinopiaggi), martino.piaggi@mail.polimi.it
- Lorenzo Morelli: Developer-> [lorenzo-morelli](https://github.com/lorenzo-morelli), lorenzo1.morelli@mail.polimi.it
- Matteo Laini: Developer -> [matteolaini](https://github.com/matteolaini), matteo.laini@mail.polimi.it
- Milo Brontesi: Developer -> [zibasPk](https://github.com/zibasPk), milo.brontesi@mail.polimi.it
- Mario Vallone: Developer -> [Mario2414](https://github.com/Mario2414), mario.vallone@mail.polimi.it

## Overview and Vision Statement

Gambetto combines the strategic depth of chess with puzzle, arcade and some rhythm games.
Players take on the role of a chess pawn facing various challenges. As they navigate different types of dungeons, they’ll need to face enemy pieces and to get to the end of different rooms. Players can transform using power-ups and gain the abilities of other chess pieces, introducing layers of strategy and gameplay depth.
All pieces in the game will move following a certain rhythm; players will need to time their actions to choose moves.
The player also needs to avoid falling out from the chessboard, otherwise he will have to restart the level.
Levels are designed and ordered with increasing difficulty.
Any level will be considered completed only if the player reaches the end tile where he will transform in a king piece and ascend. As a consequence, the next level will be unlocked and playable.

### Genre

Strategy puzzle Hybrid with Rhythm and Arcade elements.

### Platforms

PC, Mobile

### Market Analysis

The game's visual aesthetics will draw inspiration from Monument Valley while blending rhythm-based elements with arcade mechanics reminiscent of games like Crossy Road.

## Gameplay

### Game Flow

In a typical game players commence a level in the role of a pawn, and they must strategize their moves by carefully timing their inputs. The available move is visually highlighted on the ground, and changes cycling through an array of moves over a set duration.
Using this game mechanic, players navigate the dungeon, progressing through various levels while avoiding enemy pieces and environmental hazards. Certain rooms contain power-ups essential for progressing through the map. Completion of each level grants access to the next one.

### Core Mechanics

- Chess-inspired Movement: Navigate using the movement rules of chess pieces.
- Movement Rhythms: Sync your moves with the ticking of an in-game clock. Mistime your move, and face the consequences. Selecting trigger all enemies to move at the same time as the player. Enemy movement will also be triggered by the end of a move cycle.
- Chess Power-ups: Go on tiles representing various chess pieces to gain their movement abilities.
- Quick Levels: Short, intense levels, especially during the initial phases.

### Challenges

- **Time Pressure**: player must make their move within a time limit
  - To be stationary increase the chances to be captured by an enemy piece
  - Selecting the wrong move can result into falling out from the chessboard or crashing into an enemy piece
- **Dynamic Enemies**: various types of enemies
  - Different enemies have different move patterns
  - Enemies move in sync with the player , some try to eat the player others just follow a predefined pattern.
  - Enemies that follow you have an activation distance they will start to follow you only if you alert them by getting close enough.
- **Powerup exploitations**: Player must make the most of powerups
  - Wrong powerups could increase difficulty

### Level Design

Levels will each have different challenges and hazards, levels are made of chess board style tiles and are surrounded by a foggy void.
Levels exhibit unique, irregular shapes and may incorporate openings within their design. They are set apart by alterations in tile patterns, lighting, and fog effects, presenting varying colors to immerse players in diverse environments. Within each room, enemies are strategically positioned, and players will have to find a way to traverse them end reach the end goal, a highlighted tile that "crwons the player as King".

## User Interface

There will be a main menu and a level selection interface.

The in-game user interface will maintain a minimalistic design, featuring only a select few elements, including a pause button.

## Characters

- The Pawn (Player): The main character who embarks on this journey to become a King, harnessing the powers of other pieces.
- Enemy Pieces: Various chess pieces, each with unique movement patterns, challenge the player throughout the journey.

## Story

In a surreal chess world, a lone pawn seeks to become a king. To do so, it must navigate a maze of challenges and adversaries, learning from other pieces and growing in strength.

## World

The setting is an abstract, ever-changing chessboard. Each level is a unique dungeon challenging the player's spatial awareness.
In each level the player will face a chessboard with different shape compared to the previous one. Lighting and chess board colors may vary too.

## Art and Visual Style

The game's artistic style embraces a Low Poly Minimalist approach, infusing it with a hint of surrealism. Inspired by "Monument Valley".
The game camera will have an isometric view of the map and will follow the player.

## Music

The game's music sets the mood with an ambient composition, where rhythmic elements echoing the in-game clock's tick, creating a soothing and immersive experience for players.

## Sound Effects

Discrete, emphasizing movements, captures, and transformations.

## Technical Specification

- **Engine**: Unity (ideal for both PC and Mobile platforms).
- **Graphics**: 3D low poly
- **Controls**: The player can use the space key/mouse click (or screen tap on mobile) to select his move. He needs to press it when the light displayed on the terrain corresponds to his desired direction. Initially the pawn can only move one step in each direction (excluding diagonal moves), but the game will provide some power-ups to increase the variety of the movement. In this last case the player will be also able to select the destination cell always in the unforgiving way that distinguishes the game.

### Project Structure Overview

Materials

- Two distinct chessboard materials: Dark and Light. These could represent the two different colored squares on a chessboard.
- background fog
- gradient background
- maybe the player can choose to be white/black piece. And if black it's hard because we can make that for the first turn only the enemies (white) moves (like in chess white move first)

Scenes
A main menu for game navigation.
Several testing or development scenes, like "Prova" and "SampleScene", indicating ongoing development and experimentation.

Scripts

- Main Game Logic:
  Scripts for audio management, cell interactions, grid management, level flow, main menu functionality, and UI management.

- Chess Piece Logic:
  A script for the basic behavior of chess pieces.

- Room Logic:
  Scripts related to room behavior and layout, indicating dynamic and varied level designs.
  Manually design rooms and concatenate rooms layouts/specifications easily in each level

- Clock Logic:

  arrow conventions:

```mermaid

sequenceDiagram
		A-)B: Asynchronous Call
		A ->> B: Synchronous Call
		B-->> A: Synchronous Response
		B--)A: Asynchronous Response


```

<br><br><br>

```mermaid

sequenceDiagram

    participant C as Clock

    participant G as GridManager
		participant PC as PlayerController
		participant CB as CPUBehaviour
		actor P as Player
		participant PI as Pieces
		loop Every cycle
		C-)+G: clockEvent
		G ->>+ PI: updatePositions

		par GridManager to PlayerController
		G -)+ PC: startChoosing(piece, currentPosition)
		PC->>+ P: cycleThroughPossibleMoves
		P -->>- PC: choose move
		PC --)- G: chosenMove
		and GridManager to CPUBehaviour
		G -)+ CB: chooseMoves(grid,pieces)
		CB --)- G: chosenMoves
		end
		end
```

- Utilities:
  Several utility scripts for constants, debugging, and movement directions, among others. These are crucial for game functionality and development efficiency.

- Sounds:
  Audio files for theme music and sound effects, which will contribute to the game's ambiance and player feedback.

## Deadlines

### Week 1 (October 10 - October 17, 2023)

- **Team formation** and initial brainstorming sessions.
- **Task Assignment** to start game's core mechanics.

### Week 2 (October 17 - October 24, 2023)

- Continuing **Core Mechanics Development**
- **Aesthetics Brainstorm**
- Hold a team meeting to discuss progress and potential changes.

### Week 3-4 (October 24 - November 6, 2023)

- **Experimenting layout of levels**
- **Milestone Review**
- **Draft of GDD**
- Send the game design document and git repository link to pierluca.lanzi@polimi.it.

### Week 5-9 (November 6 - December 12, 2023)

- **Prototype Development**
- **Aesthetics Implementation**
- **Regular Playtesting**, testing and refining gameplay mechanics
- **Prototype Submission** (December 10)

### Week 10 (December 12 - December 20, 2023)

- **Prototype Evaluation**
- **Feedback Gathering**

### Week 11-13 (December 20, 2023 - January 13, 2024)

- **Beta Development**
- **Bug Fixing**

### Week 14-16 (January 13 - February 23, 2024)

- **Beta Testing**
- **Feedback Implementation**

### Week 17 (February 23 - February 27, 2024)

- Final touches and last-minute refinements.
- Submit the final project.
