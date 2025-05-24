using System.Data.SqlTypes;
using System.Numerics;
using System.Runtime.CompilerServices;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.UIElements
{
    public class SettingsCanvas : UICanvas
    {
        Action<bool> setModifiedCallBack { get; set; }

        InfoBox header {  get; set; }
        Sprite canvasFrame {  get; set; }
        SliderBar masterVolume {  get; set; }
        TextElement masterVolumeText { get; set; }
        SliderBar musicVolume { get; set; }
        TextElement musicVolumeText { get; set; }
        SliderBar effectsVolume { get; set; }
        TextElement effectsVolumeText { get; set; }
        SliderBar graphics { get; set; }
        TextElement graphicsText { get; set; }

        float newMasterVolume { get; set; } = 0;
        float newMusicVolume { get; set; } = 0;
        float newEffectsVolume { get; set; } = 0;
        int newRenderingQuality { get; set; } = 0;

        public SettingsCanvas(Action<bool> setModifiedCallBack)
        {
            this.setModifiedCallBack = setModifiedCallBack;
            LoadGraphics();
        }

        private void LoadGraphics()
        {
            canvasFrame = new Sprite();
            canvasFrame.SetImage("Assets/UI/Map_Border.png");
            canvasFrame.SetDimensions(new Vector2(80, 64));
            canvasFrame.SetScale(new Vector2(8.5f, 8.5f));

            header = new InfoBox((float)CanvasController.width / 2, 50, 0, "Game Settings");

            float screenCenterX = (float)CanvasController.width / 2;

            float mVolumeY = 150f;

            masterVolumeText = new TextElement(screenCenterX, mVolumeY, 0, "Master Volume", "#a88d75", "a88d75", "Elv Pixel", "36px");

            int mVolumeValue = (int)(float.Parse(PlayerSettings.GetSetting("Master Volume")) * 10);
            masterVolume = new SliderBar(screenCenterX, mVolumeY + 50, 0, OnChange_MasterVolume, mVolumeValue);

            float musicVolumeY = 275f;
            musicVolumeText = new TextElement(screenCenterX, musicVolumeY, 0, "Music Volume", "#a88d75", "a88d75", "Elv Pixel", "36px");
            int musicVolumeValue = (int)(float.Parse(PlayerSettings.GetSetting("Music Volume")) * 10);
            musicVolume = new SliderBar(screenCenterX, musicVolumeY + 50, 0, OnChange_MusicVolume, musicVolumeValue);

            float effectsVolumeY = 400f;
            effectsVolumeText = new TextElement(screenCenterX, effectsVolumeY, 0, "Effects Volume", "#a88d75", "a88d75", "Elv Pixel", "36px");
            int effectsVolumeValue = (int)(float.Parse(PlayerSettings.GetSetting("Effects Volume")) * 10);
            effectsVolume = new SliderBar(screenCenterX, effectsVolumeY + 50, 0, OnChange_EffectsVolume, effectsVolumeValue);

            float graphicsSettingsY = 525f;
            graphicsText = new TextElement(screenCenterX, graphicsSettingsY, 0, "Rendering Quality", "#a88d75", "a88d75", "Elv Pixel", "36px");
            int numRayCasts = int.Parse(PlayerSettings.GetSetting("Rendering"));
            int graphicsSettingsValue = ConvertRayCastToValue(numRayCasts);
            graphics = new SliderBar(screenCenterX, graphicsSettingsY + 50, 0, OnChange_RenderingQuality, graphicsSettingsValue);

        }

        private int ConvertRayCastToValue(int numRayCasts)
        {
            switch (numRayCasts)
            {
                case 36:
                    return 1;
                case 32:
                    return 2;
                case 28:
                    return 3;
                case 24:
                    return 4;
                case 20:
                    return 5;
                case 16:
                    return 6;
                case 12:
                    return 7;
                case 8:
                    return 8;
                case 4:
                    return 9;
                case 2:
                    return 10;
                default:
                    return 9;
            }

            
        }

        private int RenderingValueToRaycasts(int value)
        {
            Dictionary<int, int> valueToRaycasts = new Dictionary<int, int>()
            {
                { 1, 36 },
                { 2, 32 },
                { 3, 28 },
                { 4, 24 },
                { 5, 20 },
                { 6, 16 },
                { 7, 12 },
                { 8, 8 },
                { 9, 4 },
                { 10, 2 },
            };

            return valueToRaycasts[value];
        }



        public void OnChange_MasterVolume(int value)
        {
            setModifiedCallBack?.Invoke(true);
            newMasterVolume = value;
        }

        public void OnChange_MusicVolume(int value)
        {
            setModifiedCallBack?.Invoke(true);
            newMusicVolume = value;
        }

        public void OnChange_EffectsVolume(int value)
        {
            setModifiedCallBack?.Invoke(true);
            newMusicVolume = value;
        }

        public void OnChange_RenderingQuality(int value)
        {
            setModifiedCallBack?.Invoke(true);
            newRenderingQuality = RenderingValueToRaycasts(value);

        }

        public void CommitChanges()
        {
            if (newMasterVolume != 0)
            {
                if (newMasterVolume == 1)
                {
                    newMasterVolume = 0;
                }
                else
                {
                    newMasterVolume = newMasterVolume / 10;
                }
                
                PlayerSettings.UpdateSetting("Master Volume", newMasterVolume.ToString());
            }

            if (newMusicVolume != 0)
            {
                if (newMusicVolume == 1)
                {
                    newMusicVolume = 0;
                }
                else
                {
                    newMusicVolume = newMusicVolume / 10;
                }

                PlayerSettings.UpdateSetting("Music Volume", newMusicVolume.ToString());
            }

            if (newEffectsVolume != 0)
            {
                if (newEffectsVolume == 1)
                {
                    newEffectsVolume = 0;
                }
                else
                {
                    newEffectsVolume = newMusicVolume / 10;
                }

                PlayerSettings.UpdateSetting("Effects Volume", newEffectsVolume.ToString());
            }

            if (newRenderingQuality != 0)
            {
                PlayerSettings.UpdateSetting("Rendering", newRenderingQuality.ToString());
            }
        }

        public async Task Render()
        {
            await header.Render();            

            float frameX = (float)(CanvasController.width / 2) - (canvasFrame.dimensions.X * canvasFrame.scale.X) / 2;
            float frameY = (float)(CanvasController.height / 2) - (canvasFrame.dimensions.Y * canvasFrame.scale.Y) / 2;
            Vector2 framePos = new Vector2(frameX, frameY);

            await RenderingController.DrawRectangles("rgba(0, 0, 0, 0.75", frameX, frameY, canvasFrame.dimensions.X * canvasFrame.scale.X, canvasFrame.dimensions.Y * canvasFrame.scale.Y);
            await RenderingController.Draw(canvasFrame.image, framePos, canvasFrame.dimensions * canvasFrame.scale);


            await masterVolumeText.Render();
            await masterVolume.Render();

            await musicVolumeText.Render();
            await musicVolume.Render();

            await effectsVolumeText.Render();
            await effectsVolume.Render();

            await graphicsText.Render();
            await graphics.Render();
        }

    }
}
