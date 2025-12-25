using System.Numerics;
using System.Resources;
using Microsoft.AspNetCore.Components;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.Scenes;
using PixelArtGameJam.Game.UIElements;
using PixelArtGameJam.Game.WorldObjects;
using SeaLegs.Controllers;
using SeaLegs.Data;

namespace PixelArtGameJam.Game.Entities
{
    public class Camera : Entity
    {
        public enum CameraStates { PLAY, PAUSE, MAP };
        public CameraStates currentState = CameraStates.PLAY;

        FadeEffect fadeEffect {  get; set; }

        protected Player player { get; private set; }
        
        public UICanvas playerUI { get; private set; }
        public UICanvas pauseMenu { get; private set; }
        public UICanvas mapUI { get; private set; }
        public UICanvas endCanvas { get; private set; }
        public DeathScreen deathScreen { get; set; }

        private Raycast[] raycasts { get; set; }

        private List<WorldObject> objectsToRender = new List<WorldObject>();

        private VertBounds floorTexture { get; set; }
        private VertBounds ceilingTexture { get; set; }

        //Raycasting variables
        private double FOV = 70;
        private int numRays = (int)CanvasController.width / int.Parse(PlayerSettings.GetSetting("Rendering")); //Update with player settings later
        private int maxDepth = 1500;


        public Camera(Vector2 position) : base(position)
        {
            
        }

        public void CreateFadeInEffect()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEIN);
        }

        public async Task CreatePlayerUI()
        {
            playerUI = new UICanvas();
            mapUI = new UICanvas();
            pauseMenu = new UICanvas();
            
        }

        public async Task CreateFloorAndCeiling(string floorPath, string ceilingPath)
        {
            Vector2 floorPosition = new Vector2(0, (float)CanvasController.height / 2);
            floorTexture = new VertBounds(floorPosition, WorldObject.ObjectType.WALL, VertBounds.BoundsType.FLOOR, maxDepth);
            floorTexture.sprite.SetImage(floorPath);
            floorTexture.sprite.SetDimensions(new Vector2(1080, 360));

            Vector2 ceilingPosition = new Vector2(0, 0);
            ceilingTexture = new VertBounds(ceilingPosition, WorldObject.ObjectType.WALL, VertBounds.BoundsType.CEILING, maxDepth);
            ceilingTexture.sprite.SetImage(ceilingPath);
            ceilingTexture.sprite.SetDimensions(new Vector2(1080, 360));
        }

        public async Task InstantiateRayCasts(WorldObject[][] worldObjects, WorldObject[][] woLayerTwo, int gridSize)
        {
            //Called from the dungeon during loading
            numRays = (int)CanvasController.width / int.Parse(PlayerSettings.GetSetting("Rendering"));
            raycasts = new Raycast[numRays];

            for (int i = 0; i < numRays; i++)
            {
                Raycast ray = new Raycast(i, worldObjects, woLayerTwo, objectsToRender, FOV, numRays, maxDepth, gridSize);
                ray.UpdateRayAngle(player.rotation);
                raycasts[i] = ray;
            }
        }

        public void LockCameraToPlayer(Player player, Dungeon dungeonRef)
        {
            this.player = player;

            //Health bars
            HealthBars playerHealthDisplay = new HealthBars(25f, 25f, 0f);
            playerUI.AddElement(playerHealthDisplay);

            player.SetHealthBarReference(playerHealthDisplay);

            //Minimap Setup
            float minimapX = (float)(CanvasController.width - 192 - 25);
            float minimapY = 25;

            Minimap minimap = new Minimap(minimapX, 25, 0, dungeonRef);
            playerUI.AddElement(minimap);

            //Map UI Set-up
            DungeonMap dungeonMapRef = new DungeonMap(0, 0, 0, dungeonRef);
            mapUI.AddElement(dungeonMapRef);

            //Pause menu set-up
            PauseMenu pauseMenuElement = new PauseMenu(0, 0, 0, dungeonRef);
            pauseMenu.AddElement(pauseMenuElement);

        }

        public void UpdatePlayerRotation(float rotationDegrees)
        {
            foreach (Raycast ray in raycasts)
            {
                ray.UpdateRayAngle(rotationDegrees);
            }
        }

        public async void HandlePlayerDeath(Dungeon dungeonRef)
        {
            deathScreen = new DeathScreen(0, 0, 0, dungeonRef);
            await AudioController.StopSound("Assets/Audio/GameplayMusic.ogg");
        }
        public async void ChangeState(CameraStates state)
        {
            if (currentState == CameraStates.PLAY)
            {
                fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
                await Task.Delay(1000);
            }           


            if (state == CameraStates.PLAY)
            {
                
                fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEIN);
            }
            
            currentState = state;            
        }

        public async Task RenderWorld(float deltaTime)
        {
            switch (currentState)
            {
                case CameraStates.PLAY:
                    await RenderPlayState(deltaTime);
                    break;
                case CameraStates.PAUSE:
                    await RenderPauseState();
                    break;
                case CameraStates.MAP:
                    await RenderMapState();
                    break;
            }

            //Handle fade effects
            if (fadeEffect != null)
            {
                await fadeEffect.Render();

                if (fadeEffect.readyForDeletion)
                {
                    fadeEffect = null;
                }
            }
        }

        private async Task RenderPlayState(float deltaTime)
        {
            if (floorTexture != null)
            {
                await floorTexture.Render();
                await floorTexture.RenderShadows();
            }

            if (ceilingTexture != null)
            {
                await ceilingTexture.Render();
                await ceilingTexture.RenderShadows();
            }

            objectsToRender.Clear();

            List<Raycast> tempRaycasts = raycasts.ToList();

            foreach (Raycast raycast in tempRaycasts)
            {
                await raycast.Cast(player.rotation, player.position);
            }

            List<WorldObject> sortedList = await SortWorldObjects();

            foreach (WorldObject wObj in sortedList)
            {
                await wObj.Render();
                await wObj.RenderShadows();
            }

            if (playerUI != null)
            {
                await playerUI.Render();
            }

            if (player != null)
            {
                await player.Render(); //Renders cards when hovered over from player class (Yes this is sloppy coding :(  )
            }

            if (deathScreen != null)
            {
                await deathScreen.Render();
                deathScreen.HandleAnimations(deltaTime);

            }
        }

        private async Task<List<WorldObject>> SortWorldObjects()
        {
            List<WorldObject> sortedList = objectsToRender.OrderByDescending(wObj => wObj.castedDistance).ToList();
            return sortedList;
        }

        private async Task RenderMapState()
        {
            await mapUI.Render();
        }

        private async Task RenderPauseState()
        {
            await pauseMenu.Render();
        }
    }
}
