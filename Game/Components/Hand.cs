using System.Numerics;
using PixelArtGameJam.Game.Data;
using PixelArtGameJam.Game.UIElements;
using SeaLegs.Controllers;

namespace PixelArtGameJam.Game.Components
{
    public class Hand
    {
        private UICanvas playerCanvasReference { get; set; }

        private List<Card> cardsInHand { get; set; }

        const int MaxCardsInHand = 5;

        public Hand(UICanvas playerCanvas)
        {
            this.playerCanvasReference = playerCanvas;
            this.cardsInHand = new List<Card>();
            SeedPlayerDeck();
        }

        public async void SeedPlayerDeck()
        {
            Deck.SeedPlayerCards();

            for (int i = 0; i < MaxCardsInHand; i++)
            {
                DrawCard();
                UpdateCardPositions();

                await Task.Delay(100);
            }

            
        }

        public void ReconnectHand(UICanvas playerCanvas) //Connects the players hand to new canvas reference when switching floors
        {
            this.playerCanvasReference = playerCanvas;

            foreach (Card card in cardsInHand)
            {
                playerCanvasReference.AddElement(card);
            }
        }

        public void DrawCard()
        {
            if (cardsInHand.Count >= MaxCardsInHand) { return; }

            Card drawnCard = Deck.DrawCard();
            cardsInHand.Add(drawnCard);
            if (playerCanvasReference != null)
            {
                playerCanvasReference.AddElement(drawnCard);
                float effectsVolume = float.Parse(PlayerSettings.GetSetting("Effects Volume"));
                AudioController.PlaySound("Assets/Audio/card.wav", effectsVolume, false);
            }
            else
            {
                Console.WriteLine("Player canvas reference null");
            }
        }

        public void UpdateCardPositions()
        {
            Vector2 pivotPosition = new Vector2(
                    (float)CanvasController.width / 2f,
                    (float)CanvasController.height + 450f);

            float fanAngle = 12 * cardsInHand.Count;
            float angleIncrement = fanAngle / cardsInHand.Count;
            float startingAngle = -fanAngle / 2f;

            for (int i = 0; i < cardsInHand.Count; i++)
            {
                Card card = cardsInHand[i];
                               
                float currentAngle = startingAngle + (i * angleIncrement);
                
                float rotationInRadians = (float)((currentAngle - 90) * Math.PI / 180);
                float newX = pivotPosition.X + (float)Math.Cos(rotationInRadians) * 700f;
                float newY = pivotPosition.Y + (float)Math.Sin(rotationInRadians) * 500f + (10 * i);

                Vector2 newPosition = new Vector2(newX - (card.sprite.dimensions.X / 2), newY - (card.sprite.dimensions.Y / 2));
                
                float rotation = currentAngle + 5;

                card.UpdateCardPosition(newPosition, rotation);
            }
        }

        public async void RemoveCardFromHand(Card cardToRemove)
        {
            cardsInHand.Remove(cardToRemove);
            playerCanvasReference.RemoveElement(cardToRemove);
            UpdateCardPositions();
            Deck.DiscardCard(cardToRemove);

            //Refill hand
            if (cardsInHand.Count == 0)
            {
                for (int i = 0; i < MaxCardsInHand; i++)
                {
                    DrawCard();
                    UpdateCardPositions();

                    await Task.Delay(100);
                }
            }
        }

        public async void DiscardHand()
        {
            

            List<Card> tempCards = cardsInHand.ToList();

            foreach (Card card in tempCards)
            {
                RemoveCardFromHand(card);
            }
        }

        public List<Card> GetCardsInHand()
        {
            return cardsInHand;
        }
    }
}
