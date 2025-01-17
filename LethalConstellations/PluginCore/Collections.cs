using LethalLevelLoader;
using System.Collections.Generic;

namespace LethalConstellations.PluginCore
{
    public class Collections
    {
        internal static List<string> ConstellationsList = [];
        internal static Dictionary<string, string> ManualSetupList = [];
        internal static System.Random Rand = new();

        public static List<ClassMapper> ConstellationStuff = []; //public for access from other mods

        internal static Dictionary<string, string> ConstellationCats = [];
        //ConstellationCats.Add(item, $"Route to ConstellationWord {item}\t${price}");

        internal static Dictionary<ExtendedLevel, int> MoonPrices = [];
        //MoonPrices.Add(extendedLevel.NumberlessPlanetName, levelPrice.Value);

        internal static Dictionary<string, string> CNameFix = [];
        //CNameFix.Add(item, newWord);

        internal static string CompanyMoon = "Gordion";

        public static string CurrentConstellation = ""; //easy way to get current constellation

        internal static string ConstellationsWord;

        internal static string ConstellationWord;

        internal static TerminalNode ConstellationsNode;

        internal static List<string> ConstellationsOTP = [];

        internal static void Start()
        {
            MoonPrices.Clear();
        }
    }
}
