# Brief Guide to Make a Room

Keep in mind that a level is made up of a chain of "rooms".

Each room is described by the `RoomLayout` class:
- It defines the layout of a single room, described using a CSV file.
- `RoomLayout` also defines default behaviors for pieces in the room.

The `RoomBuilder` class is responsible for constructing the room based on the `RoomLayout`.

More specifically, to build a single level:
- Write CSV tables in an external editor which represent each room.
- Compose a list in the Unity level scene containing the `RoomLayout` objects describing the correct sequence.
- Using CSV files, you can describe the shape of the room, the enemies inside that room, and the "power-ups" in that room.
- The `RoomLayout` objects are very modular and can be reused between multiple levels, so if you like a part of an existing level, you can easily reuse it.
- Each room (except for the first and the last one of the level) has a predecessor room and a successor room.
- At the start of the Unity scene, the entire level is generated if there are no errors in the CSV files. (To avoid confusion, create one room at a time.)

An example of a CSV room is:

```csv
x,,,,x
x,♖,,,x
x, , ,x
x, , ,x
x, , ,x
x,,,,x
x,,,,x
x,,,s,x
x,x,x,x,x
```

Many IDEs like VS Code offer CSV editing extensions. These can display CSV files as tables, making it easier to visualize and edit room layouts quickly. The same room layout as a markdown table:

| x | x | x | x | x |
|---|---|---|---|---|
| x | ♖ |   |   | x |
| x |   |   |   | x |
| x |   |   |   | x |
| x |   |   |   | x |
| x |   |   |   | x |
| x |   |   |   | x |
| x |   |   | s | x |
| x | x | x | x | x |

**Possible Values for CSV**:

- `x` or `X`: Empty
- `s` or `S`: Unique spawn of the player in the **entire level**
- `e` or `E`: Exit of the entire level
- `♟` or `p` or `P`: Enemy pawn
- `♖` or `r` or `R`: Enemy rook
- `♘` or `kn` or `KN`: Enemy knight
- `♗` or `b` or `B`: Enemy bishop
- `♛` or `q` or `Q`: Enemy queen
- `♚` or `k` or `K`: *not used* enemy king
- `pk` or `PK`: Powerup Knight
- `pb` or `PB`: Powerup Bishop
- `pr` or `PR`: Powerup Rook
- `pp` or `PP`: Powerup Pawn
- `UNLOCK`: Cell with a key to unlock doors
- `DOOR`: A cell (a door tile) that appears after unlocking
- `BOMB`: Bomb tile

**Conventions**:

- Each piece can have an identifier for unique behaviors (like `P1` or `♟2`)
- Exit side of the room is specified in RoomLayout.
- Rooms must be surrounded by empty cells except for the exit side.