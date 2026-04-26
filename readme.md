# OVERCHARGE

OVERCHARGE is a fast-paced arcade racing game inspired by F-Zero GX and Mario Kart with gameplay focusing on speed, skill, and resource management. It features tracks that are 'anti-gravity' where the player can race in any orientation, allowing unique track layouts to be made. For example, a Mobius strip, or tunnel where you can race on every surface. The player can pick different characters to play as and ships to drive, which have different stats and driving characteristics.
## Bugs
 - Player should stop quicker when no buttons are pressed

- Camera jitters (maybe from the difference between fixed and frame updates?)
- Player gets stuck on walls
- Remove bottom face of charge pad model
## Next Steps
- Gates
- Look into making procedural tracks
## Development Path

### First full through line (Pre-Alpha)

- (x) Dev track
- (x) Dev ship
- (x) Simple driving physics
- (x) Ship can charge
- (x) Ship can boost
- (x) Ship can be 'disabled'
- (x) Charge pads
- ( ) Track Gates
- ( ) Lap Timer
- ( ) Track Position
- ( ) Race can finish
- ( ) Starter boost
- ( ) Normalized Stats
- ( ) Time trials
- ( ) After race leaderboard
- ( ) Pause Menu
- ( ) Simple Menu
- ( ) Release first pre-alpha build

### First Demo (Alpha)

- ( ) More Track Models
- ( ) Can pick tracks
- ( ) CPU drivers
- ( ) Track Cups
- ( ) After cup leaderboard
- ( ) Classic Mode
- ( ) More Ship Models
- ( ) Character Models
- ( ) Can pick character + ships
- ( ) Release first 'complete' alpha build

### After Demo (Beta)

- ( ) Items
- ( ) Chaos Mode
- ( ) Add more tracks/cups
- ( ) Add more ships
- ( ) Add more characters
- ( ) Beautify

### Full release (Delta)

- ( ) Polish areas when it needs polish
- ( ) Release 1.0

## Menu

### Character Selection
Grid selection to the left with the model of the character on the right
### Ship Selection
![[IMG_5764.png]]

## Loading
Racing Scene
Track and details are a prefab
Game manager (which also controls the timer) will load the prefab
## Movement

### Air control

Players can slightly turn while in the air

### Start boost

Dial spins at start of race with a 'good' area, if player throttles while in the good area, they get a slight boost at start of race

### Specialty track boost

Characters have a specialty track type and they have a speed boost when on their track

### Character - Ship match

If a character and ship both have the same specicalty, their recover time slightly decreases and their rechargeRate slightly increases

### Drifting

While drifting the back of the ship steps out giving them a slightly better turn in

## Controls

- Throttle: A
- Break: B
- Turning: L Stick
- Boost: RB
- R Trick: R Trig
- L Trick: L Trig
- F Trick: Both Trigs
- Item: LB

## Race Types

### Classic

Race 3 or 4 tracks in a cup

### Chaos

Same as Classic but with items

### Time Attack

No items or other players, constant RNG

### Online

Online version of Classic and chaos

## Track Type

### Mobius

Track does a 180 degree twist so you drive on both sides of the track, aka a mobius strip

### Speed

Track focuses on strights and charge pads to maximize speed

### Inclosed

Track has a tunnel like section where ships can drive on all sides

### End-to-end

Track starts at point A and ends at a different point B

### One-Sided

Track has only one drivable side, aka not a mobius loop

## Ship Stats

- Speed
- Accel
- Weight
- Handling

### Hidden Stats

- Max Charge
- Recharge rate
- Recover speed
- Specialty

## Character Stats

- Skill (Recovery Speed)

### Hidden stats

- Specialty

## Charge

Ship has charge

Ships have different max charges

Charge Pads recharge ship

If charge is full, charge pads give boost

Charge can be used for temp boost

## 'Tricks'

### Tricks uses some charge for landing boost

### Roll left/right

Slightly rotates and translates ship in direction of roll

### Forward trick (no idea for animations rn)

Gives forward boost in air

## Items

### Bombs

Explodes after a set amount of time and stuns player if hit

### Missile

Shoots in one direction and stuns player if hit

### Heat tracking missile

Same as normal missles but tracks closest player

### Charge pack

Gives player some charge when used

### Resistor bomb

Creates a AOE attack that lowers ships charge when going through

### Frequency Jammer

Makes player invisible to any tracking item

## Camera Effects

### FOV

FOV increases with speed

Final stretch before end of race increases FOV

### Shake

Shake increases with speed
