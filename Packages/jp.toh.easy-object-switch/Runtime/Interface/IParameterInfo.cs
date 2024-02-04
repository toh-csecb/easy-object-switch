using UnityEngine;

namespace jp.toh.easy_object_switch
{
    public interface IParameterInfo
    {
        
        string baseParameterName { get; set; }

        string parameterSuffix { get; set; }

        bool IsSynced();

        bool IsSaved();

        string GetUniqueParameterName();

        Texture2D GetIcon();

        void SetIcon(Texture2D icon);

    }

}
