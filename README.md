## [Play in the browser on itch.io](https://nauti312.itch.io/six-cards-under)

# Six Cards Under
Delve into an endless dungeon full of undead, demons, and necromancy in this retro dungeon crawler & deck builder hybrid. Six Cards Under is a take on the classic 90s dungeon crawlers with raycast rendering, turn-based combat, and plenty of enemies to burn down. Pick six cards in a drafting system to build your deck, then jump into the depths of the dungeon to see how many floors you can descend each run. Enemies only get harder the deeper you go, but there are a fair amount of pick-ups along the way to keep you alive.


## About the project:

Six Cards Under was a game made for Pixel Game Jam 2025 with a heavy focus on pixel art. No game engine was used in the making of this game, just Blazor Web Assembly and HTML 2D Canvas.  The game uses a raycasting first-person shooter algorithm for scene rendering, and features grid-based movement with turn-based combat. Players draft their deck and use their chosen cards to defeat enemies and progress through an infinite dungeon.


## Known bugs:

Shadow Rendering - Due to the way certain regions handle decimals, IE: using a comma rather than a period, shadows aren't being rendered correctly to create the illusion of distance. This is interfering with the opacity of the alpha layer on many rectangles that are being rendered over the scene to create depth and rendering them as either solid black or not rendering them at all.

Initial player rotation - When the player spawns the rotation is being set so that the start door is behind the player. This occasionally causes issues with movement until the player turns again and resets the rotation.

Projectiles disappearing - Since the world is based on a grid, I had to create a secondary layer in the world for projectiles to move to when their path was blocked by other projectiles so that projectiles can pass each other. This occasionally interferes with rendering projectiles onto the screen, causing them to disappear when on layer 2.

## Notes:
Files in this repo are files of the source code for the project. I didn't include any assets since all assets used in this project were from purchased asset packs.
