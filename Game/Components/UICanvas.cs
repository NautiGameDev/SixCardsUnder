using System.Numerics;

namespace PixelArtGameJam.Game.Components
{
    public class UICanvas
    {
        public Vector2 position { get; private set; }
        public List<UIElement> uiElements { get; private set; }

        public UICanvas()
        {
            position = new Vector2(0, 0);
            uiElements = new List<UIElement>();
        }

        public void AddElement(UIElement element)
        {
            uiElements.Add(element);
            Console.WriteLine("Adding card to canvas");
        }

        public void RemoveElement(UIElement element)
        {
            uiElements.Remove(element);
        }

        public async Task Render()
        {
            if (uiElements.Count == 0) { return; }

            List<UIElement> tempList = uiElements.ToList();

            foreach (UIElement element in tempList)
            {
                await element.Render();
            }
        }
    }
}
