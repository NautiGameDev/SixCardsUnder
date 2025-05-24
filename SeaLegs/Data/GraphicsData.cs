using Microsoft.AspNetCore.Components;

namespace SeaLegs.Data
{
    public class GraphicsData
    {
        public static List<string> graphicsList = new List<string>();
        public static Dictionary<string, ElementReference> loadedGraphics = new Dictionary<string, ElementReference>();

        public static void AddGraphicToList(string path)
        {
            graphicsList.Add(path);
        }

        public static List<string> GetGraphicsList()
        {
            return graphicsList;
        }

        public static void SetGraphicsDictionary(Dictionary<string, ElementReference> dict)
        {
            loadedGraphics = dict;
        }

        public static ElementReference FindSprite(string path)
        {
            return loadedGraphics[path];
        }
    }
}
