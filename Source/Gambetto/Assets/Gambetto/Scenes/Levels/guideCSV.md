Possible values for CSV:

- x Empty
- X = Empty
- s = Spawn
- S = Spawn
- e = Exit
- E = Exit
- ♟ = p = P = Pawn
- ♖ = r = R  = Rook
- ♘ = kn = KN = Knight
- ♗ = b = B = Bishop
- ♛ = q = Q = Queen
- ♚ = k = K = King
- pk = PK // powerup knight
- pb = PB // powerup bishop
- pr = PR // powerup rook
- pp = PP // "powerup" pawn
- UNLOCK = is a key "powerup" 
- DOOR is a floor which initially is not present and then spawn after the UNLOCK
  multiple DOOR are associated with the same SINGLE key 
- BOMB

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
  x, , ,,x
  x, ,, ,x
  x,,,,x
  x,,,,x
  x,,,s,x
  x,x,x,x,x
