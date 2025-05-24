using System.Numerics;
using PixelArtGameJam.Game.Abilities;
using PixelArtGameJam.Game.Components;
using PixelArtGameJam.Game.UIElements;
using PixelArtGameJam.Game.WorldObjects;

namespace PixelArtGameJam.Game.Data
{
    public class Deck
    {
        private static List<Card> playerCards = new List<Card>();
        private static Stack<Card> mainDeck = new Stack<Card>();
        private static Stack<Card> discardPile = new Stack<Card>();


        public static Dictionary<string, int> playerDeck = new Dictionary<string, int>()
        {
            
        };

        public static void SeedPlayerCards()
        {
            foreach (string cardType in playerDeck.Keys)
            {
                for (int i=0; i < playerDeck[cardType]; i++)
                {
                    string imagePath = CardData.cardImagePaths[cardType];

                    Ability cardAbility = GetCardAbility(cardType);

                    Card card = new Card(0, 0, 0, cardAbility);
                    card.sprite.SetImage(imagePath);
                    card.sprite.SetDimensions(new Vector2(195, 284));
                    card.sprite.SetScale(new Vector2(1, 1));

                    playerCards.Add(card);
                }
            }

            ShufflePlayerCardsIntoDeck();
        }

        public static Ability GetCardAbility(string cardName)
        {
            switch (cardName)
            {
                case "Energize":
                    Ability energizeAbility = new Energize(Projectile.ProjSource.PLAYER);
                    return energizeAbility;
                case "Spark":
                    Ability sparkAbility = new Spark(Projectile.ProjSource.PLAYER);
                    return sparkAbility;
                case "Siphon":
                    Ability siphonAbility = new Siphon(Projectile.ProjSource.PLAYER);
                    return siphonAbility;
                case "Burst":
                    Ability burstAbility = new Burst(Projectile.ProjSource.PLAYER);
                    return burstAbility;
                case "Fear":
                    Ability fearAbility = new Fear(Projectile.ProjSource.PLAYER);
                    return fearAbility;
                case "Spikes":
                    Ability spikesAbility = new Spikes(Projectile.ProjSource.PLAYER);
                    return spikesAbility;
                case "Fireball":
                    Ability fireballAbility = new Fireball(Projectile.ProjSource.PLAYER);
                    return fireballAbility;
                case "Heal":
                    Ability healAbility = new Heal(Projectile.ProjSource.PLAYER);
                    return healAbility;
                case "Block":
                    Ability blockAbility = new Block(Projectile.ProjSource.PLAYER);
                    return blockAbility;
                case "Bolt":
                    Ability boltAbility = new Bolt(Projectile.ProjSource.PLAYER);
                    return boltAbility;
                case "Surge":
                    Ability surgeAbility = new Surge(Projectile.ProjSource.PLAYER);
                    return surgeAbility;
                case "Specter":
                    Ability specterAbility = new Specter(Projectile.ProjSource.PLAYER);
                    return specterAbility;
                case "Firewall":
                    Ability firewallAbility = new Firewall(Projectile.ProjSource.PLAYER);
                    return firewallAbility;
                case "Comet":
                    Ability cometAbility = new Comet(Projectile.ProjSource.PLAYER);
                    return cometAbility;
                case "Frost":
                    Ability frostAbility = new Frost(Projectile.ProjSource.PLAYER);
                    return frostAbility;
                default:
                    return null;
            }
        }

        public static void ShufflePlayerCardsIntoDeck()
        {
            Random random = new Random();
            List<Card> shuffledCards = playerCards.OrderBy(x => random.Next()).ToList();

            foreach (Card card in shuffledCards)
            {
                mainDeck.Push(card);
            }
        }

        public static void ShuffleDiscardPileIntoDeck()
        {
            Random random = new Random();
            List<Card> shuffledCards = discardPile.OrderBy(x => random.Next()).ToList();

            foreach (Card card in shuffledCards)
            {
                mainDeck.Push(card);
            }

            discardPile.Clear();
        }

        public static Card DrawCard()
        {
            Card newCard = mainDeck.Peek();
            mainDeck.Pop();

            if (mainDeck.Count == 0)
            {
                ShuffleDiscardPileIntoDeck();
            }

            return newCard;
        }

        public static void DiscardCard(Card cardToDiscard)
        {
            discardPile.Push(cardToDiscard);
        }
    }
}
