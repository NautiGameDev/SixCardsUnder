using System.Numerics;
using PixelArtGameJam.Game.Components;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class HealthBars : UIElement
    {
        Sprite sprite { get; set; }
        Sprite portrait { get; set; }

        const float frameScale = 2.5f;
        const float portraitScale = frameScale / 2;

        //Health Bar
        private Vector2 healthBarTopLeft { get; set; }
        private double healthBarWidth { get; set; }
        private double healthBarHeight { get; set; }
        const string healthBarColor = "#aa3131";

        //Shield Bar
        private Vector2 shieldBarTopLeft { get; set; }
        private double shieldBarWidth { get; set; }
        private double shieldBarHeight { get; set; }
        const string shieldBarColor = "#2364a0";

        //Energy bar
        private Vector2 energyBarTopLeft { get; set; }
        private double energyBarWidth { get; set; }
        private double energyBarHeight { get; set; }
        const string energyBarColor = "#287a50";


        //Player stats
        private float percentHealth {  get; set; }
        private float percentShield {  get; set; }
        private float percentEnergy {  get; set; }

        private int currentHealth { get; set; } = 100;
        private int maxHealth { get; set; } = 100;
        private int currentShield { get; set; } = 100;
        private int maxShield { get; set; } = 100;
        private int currentEnergy { get; set; } = 100;
        private int maxEnergy { get; set; } = 100;

        TextElement healthText { get; set; }
        TextElement shieldText { get; set; }
        TextElement energyText { get; set; }

        public HealthBars(float x, float y, float rotation) : base(x, y, rotation)
        {
            sprite = new Sprite();
            sprite.SetImage("Assets/UI/HealthBar_Frame.png");
            sprite.SetDimensions(new Vector2(133, 36));
            sprite.SetScale(new Vector2(frameScale, frameScale));

            portrait = new Sprite();
            portrait.SetImage("Assets/UI/Character_Portrait.png");
            portrait.SetDimensions(new Vector2(64, 64));
            portrait.SetScale(new Vector2(portraitScale, portraitScale));

            healthBarTopLeft = position + new Vector2(41, 4) * frameScale;
            healthBarWidth = 85 * frameScale;
            healthBarHeight = 3 * frameScale;

            shieldBarTopLeft = position + new Vector2(41, 10) * frameScale;
            shieldBarWidth = 68 * frameScale;
            shieldBarHeight = 3 * frameScale;
            
            energyBarTopLeft = position + new Vector2(41, 16) * frameScale;
            energyBarWidth = 57 * frameScale;
            energyBarHeight = 3 * frameScale;

            percentHealth = 1;
            percentShield = 1;
            percentEnergy = 1;

            healthText = new TextElement(healthBarTopLeft.X + ((float)healthBarWidth / 2), healthBarTopLeft.Y + ((float)healthBarHeight / 2) + 2, 0, "100/100", "#ffffff", "#ffffff", "Elv Pixel", "10px");
            shieldText = new TextElement(shieldBarTopLeft.X + ((float)shieldBarWidth / 2), shieldBarTopLeft.Y + ((float)shieldBarHeight / 2) + 2, 0, "20/20", "#ffffff", "#ffffff", "Elv Pixel", "10px");
            energyText = new TextElement(energyBarTopLeft.X + ((float)energyBarWidth / 2), energyBarTopLeft.Y + ((float)energyBarHeight / 2) + 2, 0, "10/10", "#ffffff", "#ffffff", "Elv Pixel", "10px");
        }

        public void SetPlayerStats(int currentHealth, int maxHealth, int currentShield, int maxShield, int currentEnergy, int maxEnergy)
        {
            this.currentHealth = currentHealth;
            this.maxHealth = maxHealth;
            this.currentShield = currentShield;
            this.maxShield = maxShield;
            this.currentEnergy = currentEnergy;
            this.maxEnergy = maxEnergy;
            
            healthText.UpdateMessage($"{currentHealth} / {maxHealth}");
            shieldText.UpdateMessage($"{currentShield} / {maxShield}");
            energyText.UpdateMessage($"{currentEnergy} / {maxEnergy}");

            percentHealth = (float)currentHealth / (float)maxHealth;
            percentShield = (float)currentShield / (float)maxShield;
            percentEnergy = (float)currentEnergy / (float)maxEnergy;
        }

        public async override Task Render()
        {
            //Health Bar
            await RenderingController.DrawRectangles(healthBarColor, healthBarTopLeft.X, healthBarTopLeft.Y, healthBarWidth * percentHealth, healthBarHeight);

            //Shield bar
            await RenderingController.DrawRectangles(shieldBarColor, shieldBarTopLeft.X, shieldBarTopLeft.Y, shieldBarWidth * percentShield, shieldBarHeight);

            //Energy Bar
            await RenderingController.DrawRectangles(energyBarColor, energyBarTopLeft.X, energyBarTopLeft.Y, energyBarWidth * percentEnergy, energyBarHeight);

            await healthText.Render();
            await shieldText.Render();
            await energyText.Render();

            //Portrait
            Vector2 portraitOffset = new Vector2(5, 5);
            await RenderingController.Draw(portrait.image, position + portraitOffset, portrait.dimensions * portrait.scale);

            //Frame
            await RenderingController.Draw(sprite.image, position, sprite.dimensions * sprite.scale);
        }
    }
}
