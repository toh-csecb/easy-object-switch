using System;

using UnityEngine;

namespace jp.toh.easy_object_switch
{

    [Serializable]
    public class SwitchTarget : IParameterInfo
    {

        [SerializeField]
        public GameObject target = null;

        [SerializeField]
        public DefaultState defaultState = DefaultState.INHERIT;

        [SerializeField]
        public string menuName = "";

        [SerializeField]
        public bool synced = true;

        [SerializeField]
        public bool saved = true;

        [SerializeField]
        public Texture2D icon = null;

        public string baseParameterName { get; set; }

        public string parameterSuffix { get; set; }

        public string GetUniqueParameterName()
        {
            return baseParameterName + parameterSuffix;
        }

        public bool IsSynced()
        {
            return synced;
        }

        public bool IsSaved()
        {
            return saved;
        }

        public Texture2D GetIcon()
        {
            return icon;
        }

        public void SetIcon(Texture2D icon)
        {
            this.icon = icon;
        }
        
        public void ResetAdvancedSettingItems()
        {
            synced = true;
            saved = true;
            icon = null;
        }
        
    }

}
