
namespace jp.toh.easy_object_switch
{

    public enum DefaultState {
        INHERIT,
        OFF,
        ON
    }

    public static class DefaultStateHelper
    {

        public static string[] GetPropertyKeys() { 
            return new string[] {"Enum.Default.State.Inherit", "Enum.Default.State.Off", "Enum.Default.State.On"};
        }

    }

}
