
namespace jp.toh.easy_object_switch
{

    public enum GenerateMode {
        SWITCH_ALL,
        SWITCH_INDIVIDUALLY
    }

    public static class GenerateModeHelper
    {

        public static string[] GetPropertyKeys() { 
            return new string[] {"Enum.Generate.Mode.Switch.All", "Enum.Generate.Mode.Switch.Individually"};
        }

    }

}
