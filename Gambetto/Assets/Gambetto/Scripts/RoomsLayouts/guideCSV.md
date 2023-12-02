Possible values for CSV:

-  x Empty
-  X = Empty
-  s = Spawn
-  S = Spawn
-  e = Exit
-  E = Exit
-  ♟ = Pawn
-  ♖ = Rook
-  ♘ = Knight
-  ♗ = Bishop
-  ♛ = Queen
-  ♚ = King
-  pk = PK
-  pb = PB
-  pr = PR 

If for each piece symbol (♟♘♗♛♚) 
you write an identifier (a number from 1 )
you can uniquely identify it and then assign a specific behaviour

Conventions: 

- List of behaviors (patterns or "asleep" enemies) are inserted in scriptableObject
- Exit side of t he room is specified onthe RoomLayout 
- In the csv rooms must be sorrounded by empty cells (except from the exit side of the room)"x"
    Example with "north" exit side: 
    x,,,,x
    x,♖,,,x
    x,, , ,x
    x,  , ,,x
    x, ,, ,x
    x,,,,x
    x,,,,x
    x,,,s,x
    x,x,x,x,x