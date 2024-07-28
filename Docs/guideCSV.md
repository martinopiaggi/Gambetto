
# Brief guide to make a room

Keeping in mind that a level is made by multiple "rooms". 
The `RoomLayout` class:

- define the layout of a single room, described using a CSV file.
- `RoomLayout` also defines default behaviors for pieces in the room.

The `RoomBuilder` class is responsible for constructing the room based on the `RoomLayout`. 

A single level is made by multiple rooms.
A single level can be easily compose in editor, changing its configurations.

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
- `UNLOCK`: Cell with a key to unlook doors
- `DOOR`: A cell (a door tile) that appears after unlocking
- `BOMB`: Bomb tile

**Conventions**:

- Each piece can have an identifier for unique behaviors (like `P1` or `♟2`)
- Exit side of the room specified in RoomLayout.
- Rooms must be surrounded by empty cells except for the exit side.



