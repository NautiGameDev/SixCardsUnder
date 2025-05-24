using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.UIElements;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Scenes
{
    public class DraftingMenu : Scene
    {
        Random random {  get; set; }
        DungeonCrawler dCrawlerRef {  get; set; }
        FadeEffect fadeEffect { get; set; }

        Sprite background {  get; set; }
        
        InfoBox header { get; set; }
        Button startButton { get; set; }
        Button backButton { get; set; }

        Dictionary<string, string> cardDataDict { get; set; }
        List<DraftingCard> draftingCards { get; set; }
        List<DraftingCard> selectedCards { get; set; }

        int maxCards = 6;

        public DraftingMenu(DungeonCrawler dCrawlerRef)
        {
            random = new Random();
            this.dCrawlerRef = dCrawlerRef;
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEIN);

            cardDataDict = CardData.GetCardData();

            draftingCards = new List<DraftingCard>();
            selectedCards = new List<DraftingCard>();

            LoadGraphics();
            DrawThreeCards();
        }

        private void LoadGraphics()
        {
            background = new Sprite();
            background.SetImage("Assets/UI/Background_Dungeon.png");
            background.SetDimensions(new Vector2(368, 208));
            background.SetScale(new Vector2(4f, 4f));

            string chooseCardMsg = "Choose a Card";
            header = new InfoBox((float)(CanvasController.width / 2), 50, 0, chooseCardMsg);

            //Set-up buttons
            float xIncrement = (float)CanvasController.width / 3;
            float yPos = (float)CanvasController.height - 50;

            backButton = new Button(xIncrement * 1, yPos, 0,  "Back", OnClickBackButton);
            startButton = new Button(xIncrement * 2, yPos, 0, "Start Run", OnClickStartButton);
            startButton.SetButtonDisabled(true);
            
        }

        public async void OnClickBackButton()
        {
            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            startButton.SetButtonDisabled(true);
            backButton.SetButtonDisabled(true);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);

            await Task.Delay(1000);
            MainMenu newScene = new MainMenu(dCrawlerRef);
            dCrawlerRef.LoadNewScene(newScene);
        }


        public async void OnClickStartButton()
        {
            Deck.playerDeck = new Dictionary<string, int>();

            foreach (DraftingCard card in selectedCards)
            {
                Deck.playerDeck[card.cardName] = 4;
            }            

            fadeEffect = new FadeEffect(0, 0, 0, FadeEffect.EffectDir.FADEOUT);
            startButton.SetButtonDisabled(true);
            backButton.SetButtonDisabled(true);

            float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
            AudioController.PlaySound("Assets/Audio/menu.wav", effectsVolume, false);
                        
            await Task.Delay(1000);

            await AudioController.StopSound("Assets/Audio/MainMenuMusic.ogg");
            await AudioController.PlaySound("Assets/Audio/GameplayMusic.ogg", AudioController.MusicVolume, true);

            Dungeon newScene = new Dungeon(dCrawlerRef);
            dCrawlerRef.LoadNewScene(newScene);
        }

        private async Task DrawThreeCards()
        {   
            double xIncrement = CanvasController.width / 4;

            List<string> cardList = cardDataDict.Keys.ToList();


            for (int i = 0; i < 3; i++)
            {
                if (selectedCards.Count == maxCards)
                {
                    draftingCards.Clear();
                    break;
                }


                float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                AudioController.PlaySound("Assets/Audio/click.wav", effectsVolume, false);

                int randomCardIndex = random.Next(0, cardList.Count);

                string randomCardName = cardList[randomCardIndex];
                string cardImagePath = cardDataDict[randomCardName];
                cardList.Remove(randomCardName);

                DraftingCard newCard = new DraftingCard((float)xIncrement * (i + 1), 250, 0, randomCardName, MoveCardToSelected);
                newCard.isSelected = true;
                newCard.sprite.SetImage(cardImagePath);
                newCard.sprite.SetDimensions(new Vector2(195, 284));
                newCard.sprite.SetScale(new Vector2(1f, 1f));

                draftingCards.Add(newCard);

                await Task.Delay(200);
            }

            if (draftingCards.Count > 0) //Prevents fast clicking bug in drafting stage
            {
                foreach (DraftingCard card in draftingCards)
                {
                    card.isSelected = false;
                }
            }
        }

        public async void MoveCardToSelected(DraftingCard card)
        {
            draftingCards.Clear();
            card.isSelected = true;

            if (selectedCards.Count < maxCards)
            {   
                string cardName = card.cardName;
                cardDataDict.Remove(cardName);
                selectedCards.Add(card);
                RepositionSelectedCards();
            }
            
            if (selectedCards.Count == maxCards)
            {
                startButton.SetButtonDisabled(false);
                DisplaySelectedCards();
                header.ChangeMessage("Selected Cards");
            }
            else
            {
                await DrawThreeCards();
            }
        }

        private void RepositionSelectedCards()
        {
            float xIncrement = (float)CanvasController.width / 7;

            for (int i = 0; i < selectedCards.Count; i++)
            {
                float xPos = xIncrement * (i + 1);
                float yPos = (float)CanvasController.height - 200;

                Vector2 newPosition = new Vector2(xPos, yPos);
                selectedCards[i].UpdatePosition(newPosition);

                selectedCards[i].sprite.SetScale(new Vector2(0.6f, 0.6f));
            }
        }

        private async void DisplaySelectedCards()
        {
            float xIncrement = (float)CanvasController.width / 4;
            int firstRowCards = 0;

            for (int i = 0; i < selectedCards.Count; i++)
            {
                firstRowCards++;

                float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                AudioController.PlaySound("Assets/Audio/card.wav", effectsVolume, false);

                float xPos = xIncrement * (i + 1);
                float yPos = 250;

                if (firstRowCards > 3)
                {
                    xPos = xIncrement * (i + 1 - 3);
                    yPos = 500;
                }

                Vector2 newPosition = new Vector2(xPos, yPos);
                selectedCards[i].UpdatePosition(newPosition);

                selectedCards[i].sprite.SetScale(new Vector2(0.8f, 0.8f));

                await Task.Delay(200);
            }
        }



        public async override Task Update(float deltaTime)
        {
            await RenderingController.Draw(background.image, Vector2.Zero, background.dimensions * background.scale);
            await header.Render();
            await startButton.Render();
            await backButton.Render();

            if (draftingCards.Count > 0)
            {
                List<DraftingCard> tempList = draftingCards.ToList();

                foreach (DraftingCard card in tempList)
                {
                    await card.Render();
                }
            }

            if (selectedCards.Count > 0)
            {
                List<DraftingCard> tempList = selectedCards.ToList();
                foreach (DraftingCard card in tempList)
                {
                    await card.Render();
                }
            }


            if (fadeEffect != null)
            {
                await fadeEffect.Render();

                if (fadeEffect.readyForDeletion)
                {
                    fadeEffect = null;
                }
            }
        }
    }
}
