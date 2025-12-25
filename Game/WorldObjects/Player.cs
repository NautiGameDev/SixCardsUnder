using System.ComponentModel.Design;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.Entities;
using PixelArtGameJam.Game.Scenes;
using PixelArtGameJam.Game.UIElements;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.WorldObjects
{
    public class Player : WorldObject
    {
        public enum PlayerState { ALIVE, DEAD }
        public PlayerState currentState { get; set; } = PlayerState.ALIVE;

        //References
        protected Camera mainCamera { get; private set; }
        protected Dungeon dungeonReference { get; set; }
        protected HealthBars playerHealthBarRef { get; set; }

        //Player movement
        public float rotation { get; set; } = 180f;
        private float[] rotations = {0f, 90f, 180f, -90f};
        private int rotationIndex = 2;

        //Player card handling
        private Card hoveredCard { get; set; }
        private bool mouseClicked { get; set; } = false;

        //Player Data
        private Hand playerHand { get; set; }
        private int maxHealth { get; set; }
        private int currentHealth { get; set; }
        private int maxShield { get; set; }
        private int currentShield { get; set; }
        private int maxEnergy { get; set; }
        private int currentEnergy { get; set; }
        
        

        public Player(Vector2 position, Camera mainCamera, Dungeon dungeonReference, float rotation) : base(position, ObjectType.PLAYER)
        {
            this.mainCamera = mainCamera;
            this.mainCamera.LockCameraToPlayer(this, dungeonReference); //Pass in player reference so camera can access the player pos/rot
            this.dungeonReference = dungeonReference;

            maxHealth = 100;
            maxShield = 25;            
            maxEnergy = 10;

            if (PlayerData.storedHand == null) //Start fresh
            {
                this.playerHand = new Hand(this.mainCamera.playerUI);
                currentHealth = maxHealth;
                currentShield = maxShield;
                currentEnergy = maxEnergy;  
            }
            else
            {
                this.playerHand = PlayerData.storedHand;
                this.playerHand.ReconnectHand(this.mainCamera.playerUI);
                currentHealth = PlayerData.currentHealth;
                currentShield = PlayerData.currentShield;
                currentEnergy = PlayerData.currentEnergy;
            }

            this.rotation = rotation;
            Console.WriteLine($"Player rotation = {rotation}");

            for (int i = 0; i < rotations.Length; i++)
            {
                if (rotation == rotations[i])
                {
                    rotationIndex = i;
                    break;
                }
            }

        }

        public void HandleKeyboardInput()
        {

            if (mainCamera.currentState == Camera.CameraStates.PLAY)
            {
                if (InputController.OnKeyDown("w"))
                {
                    MovePlayer(-1, -1, rotation);
                }

                else if (InputController.OnKeyDown("a"))
                {
                    int projectedIndex = (rotationIndex - 1 + rotations.Length) % rotations.Length;
                    float angle = rotations[projectedIndex];
                    MovePlayer(-1, -1, angle);
                }
                else if (InputController.OnKeyDown("s"))
                {
                    MovePlayer(1, 1, rotation);
                }
                else if (InputController.OnKeyDown("d"))
                {
                    int projectedIndex = (rotationIndex - 1 + rotations.Length) % rotations.Length;
                    float angle = rotations[projectedIndex];
                    MovePlayer(1, 1, angle);
                }
                else if (InputController.OnKeyDown("q"))
                {
                    RotatePlayer(-1);
                }
                else if (InputController.OnKeyDown("e"))
                {
                    RotatePlayer(1);
                }
            }

            //Open/Close map
            if (InputController.OnKeyDown("m"))
            {
                if(mainCamera.currentState == Camera.CameraStates.MAP)
                {
                    mainCamera.ChangeState(Camera.CameraStates.PLAY);
                }
                else
                {
                    mainCamera.ChangeState(Camera.CameraStates.MAP);
                    float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                    AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);
                }
            }

            if (InputController.OnKeyDown("Escape"))
            {
                if(mainCamera.currentState == Camera.CameraStates.PAUSE)
                {
                    mainCamera.ChangeState(Camera.CameraStates.PLAY);
                }
                else
                {
                    mainCamera.ChangeState(Camera.CameraStates.PAUSE);
                    float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                    AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);
                }
            }
        }

        private void RotatePlayer(int direction)
        {          
            if(direction == -1)
            {
                rotationIndex = (rotationIndex - 1 + rotations.Length) % rotations.Length;
            }
            else
            {
                rotationIndex = (rotationIndex + 1) % rotations.Length;
            }

            rotation = rotations[rotationIndex];
            mainCamera.UpdatePlayerRotation(rotation);

        }

        private void MovePlayer(int x, int y, float angle)
        {
            double rotRadians = angle * Math.PI / 180;

            float dirX = x * (float)Math.Floor(Math.Cos(rotRadians));
            float dirY = y * (float)Math.Floor(Math.Sin(rotRadians));

            Vector2 direction = new Vector2(dirX, dirY);
            Vector2 newPosition = dungeonReference.MoveWorldObject(this, position, direction);

            if (newPosition == position)
            {
                WorldObject objectInPath = dungeonReference.GetObjectAtGridPos(position, direction);

                if (objectInPath.objType == ObjectType.PROJECTILE) //Hotfix for player getting stuck between lines of projectiles
                {
                    Vector2 playerGridPosition = position / dungeonReference.map.gridSize;
                    Vector2 targetPosition = playerGridPosition + direction;

                    Projectile projectileInPath = dungeonReference.GetProjectileAtGridPos(targetPosition);

                    if (projectileInPath != null)
                    {
                        //int damage = projectileInPath.GetDamage();
                        //currentHealth -= damage;
                        //UpdateHealthBar();

                        projectileInPath.EnvironmentUpdate();
                        dungeonReference.RemoveProjectileFromDungeon(projectileInPath, targetPosition);

                        newPosition = dungeonReference.MoveWorldObject(this, position, direction);
                    }
                }
                else if (objectInPath.objType == ObjectType.WALLSTART)
                {
                    InfoBox warning = new InfoBox((float)CanvasController.width / 2, (float)CanvasController.height / 2 - 200, 0, "Why go back? You scared?");
                    mainCamera.playerUI.AddElement(warning);
                    warning.SetLifeSpan(mainCamera.playerUI);
                }
                else if (objectInPath.objType == ObjectType.WALLFINISH)
                {
                    PlayerData.storedHand = playerHand;
                    PlayerData.currentHealth = currentHealth;
                    PlayerData.currentShield = currentShield;
                    PlayerData.currentEnergy = currentEnergy;

                    dungeonReference.StartNewFloor();
                }
                else if (objectInPath.objType == ObjectType.COLLECTABLE)
                {
                    Collectable collectableInPath = (Collectable)dungeonReference.GetObjectAtGridPos(position, direction);
                    collectableInPath.ability.Cast(dungeonReference, position, rotation);
                    dungeonReference.RemoveObjectFromDungeon(collectableInPath, collectableInPath.position);
                }
                else
                {
                    InfoBox warning = new InfoBox((float)CanvasController.width / 2, (float)CanvasController.height / 2 - 200, 0, "Path blocked");
                    mainCamera.playerUI.AddElement(warning);
                    warning.SetLifeSpan(mainCamera.playerUI);
                }
            }
            else
            {
                float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                AudioController.PlaySound("Assets/Audio/footstep.wav", effectsVolume, false);
            }

                UpdatePosition(newPosition);

            EndTurn();
        }

        private void HandleCardInteractions()
        {
            if (!mouseClicked)
            {
                TestCardHover();
            }
            
            if (hoveredCard != null)
            {
                HandleCardDragging();
            }             
        }

        private void TestCardHover()
        {
            if (hoveredCard != null)
            {
                hoveredCard.ChangeScale(new Vector2(1f, 1f));
                hoveredCard = null;
            }

            List<Card> cardsInHand = playerHand.GetCardsInHand();
            Vector2 mousePosition = InputController.GetMousePosition();
                       
            foreach (Card card in cardsInHand)
            {
                if (mousePosition.X > card.position.X &&
                    mousePosition.X < card.position.X + card.sprite.dimensions.X &&
                    mousePosition.Y > card.position.Y  &&
                    mousePosition.Y < card.position.Y + card.sprite.dimensions.Y)
                {

                    hoveredCard = card;
                }
            }

            if (hoveredCard != null)
            {
                hoveredCard.ChangeScale(new Vector2(1.2f, 1.2f));
            }
        }

        private void HandleCardDragging()
        {
            if (InputController.OnMouseDown())
            {
                mouseClicked = true;
            }
            else if (InputController.OnMouseUp())
            {
                mouseClicked = false;

                if (hoveredCard.position.Y + (hoveredCard.sprite.dimensions.Y / 2) > CanvasController.height * 0.6)
                {
                    hoveredCard.SnapBackToHand();
                }
                else
                {
                    double rotRadians = rotation * Math.PI / 180;

                    float playerGridPosX = (float)(Math.Floor(position.X / dungeonReference.map.gridSize));
                    float playerGridPosY = (float)(Math.Floor(position.Y / dungeonReference.map.gridSize));

                    float dirX = -(float)Math.Floor(Math.Cos(rotRadians));
                    float dirY = -(float)Math.Floor(Math.Sin(rotRadians));

                    Vector2 targetPosition = new Vector2(playerGridPosX + dirX, playerGridPosY + dirY);
                    
                    if(dungeonReference.TestGridSpaceEmpty(targetPosition))
                    {
                        if(currentEnergy >= hoveredCard.ability.cost)
                        {
                            currentEnergy -= hoveredCard.ability.cost;
                            UpdateHealthBar();

                            playerHand.RemoveCardFromHand(hoveredCard);
                            hoveredCard.Use(dungeonReference, targetPosition, rotation);
                            EndTurn();
                        }
                        else
                        {
                            hoveredCard.SnapBackToHand();

                            InfoBox warning = new InfoBox((float)CanvasController.width / 2, (float)CanvasController.height / 2 - 200, 0, "Not enough mana!");
                            mainCamera.playerUI.AddElement(warning);
                            warning.SetLifeSpan(mainCamera.playerUI);

                        }
                    }
                    else
                    {
                        hoveredCard.SnapBackToHand();

                        InfoBox warning = new InfoBox((float)CanvasController.width / 2, (float)CanvasController.height / 2 - 200, 0, "Path blocked");
                        mainCamera.playerUI.AddElement(warning);
                        warning.SetLifeSpan(mainCamera.playerUI);


                    }

                    
                }


                hoveredCard = null;
            }

            if (mouseClicked)
            {
                Vector2 mousePosition = InputController.GetMousePosition();
                Vector2 cardDimensions = hoveredCard.sprite.dimensions;

                Vector2 calculatedPosition = mousePosition - (cardDimensions/2);

                hoveredCard.MoveCard(calculatedPosition);
            }
            
        }

        private bool HasCastableCard()
        {
            List<Card> cardsInHand = playerHand.GetCardsInHand();

            foreach (Card card in cardsInHand)
            {
                if (currentEnergy >= card.ability.cost)
                {
                    return true;
                }
            }

            

            return false;
        }

        public async void SetHealthBarReference(HealthBars healthbar)
        {
            this.playerHealthBarRef = healthbar;

            await Task.Delay(2000);
            UpdateHealthBar();
        }

        

        public void UpdateHealthBar()
        {            
            if(playerHealthBarRef == null)
            {
                Console.WriteLine("Health bar not found");
                return;
            }

            playerHealthBarRef.SetPlayerStats(currentHealth, maxHealth, currentShield, maxShield, currentEnergy, maxEnergy);
        }

        public override void ResolveDamage(int damage, int stunFactor, int fearFactor, int burnFactor)
        {
            
            //Convert stun and burn factors into damage
            int chosenDamage = 0;

            if (burnFactor > 0)
            {
                chosenDamage = 5;
            }
            else if (stunFactor > 0)
            {
                chosenDamage = stunFactor;
            }
            else
            {
                chosenDamage = damage;
            }

            //Calculate damage to health and shield
            if ((currentShield - chosenDamage) > 0)
            {
                currentShield -= chosenDamage;
            }
            else if (currentShield > 0)
            {
                int damageToHealth = chosenDamage - currentShield;
                currentShield = 0;
                currentHealth -= damageToHealth;
            }
            else
            {
                currentHealth -= chosenDamage;
            }


            //Handle player death
            if (currentHealth <= 0 && currentState == PlayerState.ALIVE)
            {
                currentHealth = 0;
                currentState = PlayerState.DEAD;

                float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                AudioController.PlaySound("Assets/Audio/death.wav", effectsVolume, false);

                mainCamera.HandlePlayerDeath(dungeonReference);
            }

            UpdateHealthBar();
        }

        public void ResolveHealing(int healAmt)
        {
            currentHealth += healAmt;

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            UpdateHealthBar();
        }

        public void ResolveShielding(int shieldAmount)
        {
            currentShield += shieldAmount;

            if (currentShield > maxShield)
            {
                currentShield = maxShield;
            }

            UpdateHealthBar();
        }

        public void ResolveMana(int energyAmount)
        {
            currentEnergy += energyAmount;

            if (currentEnergy > maxEnergy)
            {
                currentEnergy = maxEnergy;
            }

            UpdateHealthBar();
        }

        public void Update(float deltaTime)
        {
            if (currentState == PlayerState.DEAD)
            {
                return;
            }

            HandleKeyboardInput();

            if (mainCamera.currentState == Camera.CameraStates.PLAY)
            {
                HandleCardInteractions();
            }
            
        }

        public async override Task Render()
        {
            if (currentState == PlayerState.DEAD)
            {
                return;
            }

            if (hoveredCard != null && !mouseClicked)
            {
                ElementReference cardSprite = hoveredCard.sprite.image;
                Vector2 cardGraphicPosition = new Vector2(50, (float)(CanvasController.height / 2) - (hoveredCard.sprite.dimensions.Y / 2));

                await RenderingController.Draw(cardSprite, cardGraphicPosition);
            }
        }

        private void EndTurn()
        {
            if (!HasCastableCard())
            {
                playerHand.DiscardHand();
                currentEnergy = maxEnergy;
                UpdateHealthBar();
            }

            dungeonReference.UpdateEnvironment();
        }
    }
}
