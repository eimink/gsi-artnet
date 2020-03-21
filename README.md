# gsi-artnet
Small utility to convert events from CS:GO GSI to ArtNet events.

When event is triggered in GSI, this software sends 255 to channel according to mapping below.

## ArtNet Channel mapping
|Channel|    Event    |
|:-----:|-------------|
|   1   |Bomb  planted|
|   2   |Defusing Bomb|
|   3   | Bomb defused|
|   4   |Bomb exploded|
|   5   |CT Wins round|
|   6   |T Wins round |
|   7   |Map phase: Live|
|   8   |Map phase: Intermission|
|   9   |Map phase: Game Over|
