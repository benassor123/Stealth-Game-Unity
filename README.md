# Five Floors

A top-down 2D stealth action game built in Unity. You play a vigilante infiltrating a five floor villain's lair, working up through patrolling guards, security cameras, lasers and a tracker bot to reach the control room at the top and take on the boss.

Every floor can be approached two ways. Play it quietly with silent takedowns from behind, or go loud with smoke bombs and a gun. Ammo, health cans and keycards carry between levels and are deliberately scarce, so the choice on floor 2 changes what you can afford to do on floor 4.

All code was written from scratch. No AI tools were used to generate any of it.

> Built for COMP4002 Games at the University of Nottingham.

## Controls

| Key | Action |
| --- | --- |
| `WASD` | Move |
| `G` | Hold gun |
| `Left click` | Shoot if the gun is equipped, otherwise punch |
| `E` | Silent takedown from behind an enemy, or open a chest if you have a keycard |
| `T` | Throw smoke bomb |
| `H` | Use health canister |

## The five floors

Each level runs on a timer, so hesitating has a cost.

1. **Tutorial.** Three enemy types with reduced damage and weaker AI. Popups trigger as you approach enemies and chests. Sneaking up behind the stationary worker enemies saves ammo, but going loud here is a cheap way to learn the combat.
2. **Cameras.** A larger map. Cameras have their own line of sight and raise an alert rather than attacking you directly. Enemy damage and awareness both step up.
3. **The tracker bot.** Enemies now communicate properly across a bigger map, and the tracker bot uses predictive AI to aim at where you are going rather than where you were. Static lasers block routes until you destroy them. Pure stealth becomes impractical here.
4. **Moving lasers.** Lasers patrol the level, so you have to keep moving. Enemy communication is at its peak and bullets hurt, which makes health cans the resource that decides the run.
5. **Boss fight.** One on one, surrounded by chests and keys that all cost you time. The boss has ten times the health of a regular enemy, and at half health it enters rage mode: bigger, faster, more damage, and spawning armoured reinforcements every five seconds. Kill it fast or get buried.

## How it works

**Custom A\* pathfinding.** Written from scratch across three scripts rather than using Unity's NavMesh. `Node` represents a grid cell, `PathFindingGrid` builds the grid and marks which cells are walls, and `Pathfinder` runs the search and returns a list of waypoints that routes around walls.

**Enemy architecture.** `EnemyBase` holds the shared behaviour: the state machine, sight, hearing, chase logic and communication between enemies. `PatrolEnemy`, `StationaryEnemy`, `ArmouredEnemy` and `TrackerBot` each extend it. Attacks use composition instead of inheritance, so `RangedAttack` is a separate component dropped onto whichever enemies need to shoot. Each enemy type has its own prefab.

**Persistent resources.** Ammo, smoke bombs, health cans and keycards live in a static game state class with snapshot and restore methods, which is what lets progress carry across levels and makes the scarcity meaningful.

**Tuning.** Detection ranges, fire rates, level timers, punch damage and chase speed are exposed as public fields, so the difficulty curve across the five floors was tuned by playtesting values in the inspector rather than recompiling.

## Running it

Clone the repo and open the project folder in Unity `[VERSION]`, then open the level 1 scene and press play. A standalone build is available under [Releases / the builds folder].

Developed on macOS.

## Credits

Art assets are from the following sources. All game code is my own.

- [Kenney, Top Down Shooter Pack](https://kenney.nl/assets/top-down-shooter) for the player, enemies, floors, walls and weapon icons
- [Kenney, Pixel Platformer](https://kenney.nl/assets/pixel-platformer) for the chests
- [FreeIconsPNG](https://www.freeiconspng.com/img/26603) for the key
- [Flaticon](https://www.flaticon.com/free-icon/smoke-bomb_5329629) for the smoke bomb HUD icon
- [Flaticon](https://www.flaticon.com/free-icon/cctv-camera_10682263) for the camera
- [Flaticon](https://www.flaticon.com/free-icon/ammo_8970337) for the in-game bullet
- [Magnific](https://www.magnific.com/icon/bullet_11868042) for the bullet HUD icon
- [PNGTree](https://pngtree.com/freepng/cartoon-workbench-a-craftsman-s-still-life_23006843.html) for the workbench
- PNGTree for the health potion HUD icon and the armoured enemy's shield
