using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using nadena.dev.modular_avatar.core;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace jp.toh.easy_object_switch
{

    [DisallowMultipleComponent]
    [AddComponentMenu("Easy Object Switch/Easy Object Switch")]
    public class EasyObjectSwitch : AvatarTagComponent, IParameterInfo
    {

        public VRCExpressionsMenu installTo = null;

        public GenerateMode generateMode = GenerateMode.SWITCH_ALL;

        public string menuName = "";

        public bool synced = true;

        public bool saved = true;

        public bool groupingIntoSubmenu = false;

        public string submenuName = "";
        
        [SerializeField]
        public List<SwitchTarget> switchTargets = new List<SwitchTarget>();

        public bool advancedMode = false;

        public Texture2D icon = null;
        
        public Texture2D submenuIcon = null;

        public bool matchAvatarWriteDefaults = true;

        public bool writeDefaults = false;

        public bool parametersWithoutUUID = false;

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
            submenuIcon = null;
            matchAvatarWriteDefaults = true;
            writeDefaults = false;
            parametersWithoutUUID = false;
            foreach (SwitchTarget switchTarget in switchTargets) {
                switchTarget.ResetAdvancedSettingItems();
            }
        }

        private void Reset()
        {
            AutoDetectVRCAvatarDescriptor();
        }

        public void AutoDetectVRCAvatarDescriptor()
        {
            if (installTo == null) {
                Transform currentPath = this.transform;
                while (currentPath != null)
                {
                    if (currentPath.gameObject.GetComponent<VRCAvatarDescriptor>()) {
                        break;
                    }
                    currentPath = currentPath.parent;
                }
                if (currentPath != null) {
                    VRCAvatarDescriptor avatarDescriptor = currentPath.gameObject.GetComponent<VRCAvatarDescriptor>();
                    installTo = avatarDescriptor.expressionsMenu;
                }
            }
        }

        public void AutoDetectSwitchTargets()
        {
            // Find default switch targets (target is attached Renderer, ParticleSystem or Light)
            foreach (var gameObject in 
                Array.ConvertAll(GetComponentsInChildren(typeof(Renderer), true), new Converter<Component, GameObject>(e => e.gameObject))
                    .Union(Array.ConvertAll(GetComponentsInChildren(typeof(ParticleSystem), true), new Converter<Component, GameObject>(e => e.gameObject)))
                    .Union(Array.ConvertAll(GetComponentsInChildren(typeof(Light), true), new Converter<Component, GameObject>(e => e.gameObject))))
            {
                switchTargets.Add(new SwitchTarget {
                    target = gameObject
                });
            }
        }

    }

}
