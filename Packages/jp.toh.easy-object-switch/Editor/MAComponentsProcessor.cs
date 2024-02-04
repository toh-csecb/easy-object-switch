using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using nadena.dev.ndmf;
using nadena.dev.modular_avatar.core;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

using jp.toh.easy_object_switch;

namespace jp.toh.easy_object_switch.editor
{

    public class EasyObjectSwitchProcessor
    {

        private BuildContext context;
        
        public EasyObjectSwitchProcessor(BuildContext context)
        {
            this.context = context;
        }

        public void Process()
        {
            // Iterate EasyObjectSwitch components
            foreach (EasyObjectSwitch targetComponent in context.AvatarRootObject.GetComponentsInChildren<EasyObjectSwitch>(true)) {
                // Set advanced items to the default value if turn off the advanced settings
                if (!targetComponent.advancedMode) {
                    targetComponent.ResetAdvancedSettingItems();
                }
                // Add MA components
                AttachMAComponents(targetComponent, GenerateAnimatorController(targetComponent));
                // Remove EasyObjectSwitch component
                GameObject.DestroyImmediate(targetComponent);
            }
        }

        private AnimatorController GenerateAnimatorController(EasyObjectSwitch targetComponent)
        {
            AnimatorController animatorController = new AnimatorController();
            bool firstElement = true;
            string parameterName = "";
            // Iterate target objects
            foreach (SwitchTarget switchTarget in targetComponent.switchTargets) {
                // Add parameter
                switchTarget.baseParameterName = String.IsNullOrEmpty(switchTarget.menuName) ? switchTarget.target.name : switchTarget.menuName;
                switchTarget.parameterSuffix = targetComponent.parametersWithoutUUID ? "" : GetUUID();
                if (targetComponent.generateMode == GenerateMode.SWITCH_ALL) {
                    if (firstElement) {
                        targetComponent.baseParameterName = String.IsNullOrEmpty(targetComponent.menuName) ? targetComponent.gameObject.name : targetComponent.menuName;
                        targetComponent.parameterSuffix = targetComponent.parametersWithoutUUID ? "" : GetUUID();
                        parameterName = targetComponent.GetUniqueParameterName();
                        if (!animatorController.parameters.Any(e => e.name == parameterName)) {
                            animatorController.AddParameter(parameterName, AnimatorControllerParameterType.Bool);
                        }
                    }
                } else if (targetComponent.generateMode == GenerateMode.SWITCH_INDIVIDUALLY) {
                    parameterName = switchTarget.GetUniqueParameterName();
                    if (!animatorController.parameters.Any(e => e.name == parameterName)) {
                        animatorController.AddParameter(parameterName, AnimatorControllerParameterType.Bool);
                    }
                }
                firstElement = false;
                // Add layer
                AnimatorControllerLayer layer = new AnimatorControllerLayer() {
                    stateMachine = new AnimatorStateMachine(),
                    defaultWeight = 1.0f,
                    name = switchTarget.GetUniqueParameterName()
                };
                animatorController.AddLayer(layer);
                // Add states
                AnimatorStateMachine stateMachine = layer.stateMachine;
                string path = GetRelativePath(targetComponent.gameObject, switchTarget.target);
                bool isDefaultOn = false;
                if (switchTarget.defaultState == DefaultState.INHERIT) {
                    isDefaultOn = switchTarget.target.activeSelf;
                } else if (switchTarget.defaultState == DefaultState.OFF) {
                    isDefaultOn = false;
                } else if (switchTarget.defaultState == DefaultState.ON) {
                    isDefaultOn = true;
                }
                switchTarget.target.SetActive(isDefaultOn); // Set to the same active state as default
                AnimatorState defaultState;
                AnimatorState notDefaultState;
                if (isDefaultOn) {
                    defaultState = AddOnState(stateMachine, path, targetComponent.writeDefaults);
                    notDefaultState = AddOffState(stateMachine, path, targetComponent.writeDefaults);
                } else {
                    defaultState = AddOffState(stateMachine, path, targetComponent.writeDefaults);
                    notDefaultState = AddOnState(stateMachine, path, targetComponent.writeDefaults);
                }
                // Add transitions
                AnimatorStateTransition transitionToNotDefault = new AnimatorStateTransition() {
                    destinationState = notDefaultState,
                    hasExitTime = false,
                    exitTime = 1.0f,
                    hasFixedDuration = false,
                    duration = 0.0f
                };
                transitionToNotDefault.AddCondition(AnimatorConditionMode.If, 0, parameterName);
                defaultState.AddTransition(transitionToNotDefault);
                AnimatorStateTransition transitionToDefault = new AnimatorStateTransition() {
                    destinationState = defaultState,
                    hasExitTime = false,
                    exitTime = 1.0f,
                    hasFixedDuration = false,
                    duration = 0.0f
                };
                transitionToDefault.AddCondition(AnimatorConditionMode.IfNot, 0, parameterName);
                notDefaultState.AddTransition(transitionToDefault);
            }
            return animatorController;
        }

        private AnimatorState AddOnState(AnimatorStateMachine stateMachine, string path, bool writeDefaults)
        {
            return AddState(stateMachine, path, true, writeDefaults);
        }

        private AnimatorState AddOffState(AnimatorStateMachine stateMachine, string path, bool writeDefaults)
        {
            return AddState(stateMachine, path, false, writeDefaults);
        }

        private AnimatorState AddState(AnimatorStateMachine stateMachine, string path, bool active, bool writeDefaults)
        {
            AnimationClip animation = new AnimationClip();
            AnimationUtility.SetEditorCurve(
                animation,
                EditorCurveBinding.FloatCurve(path, typeof(GameObject), "m_IsActive"),
                AnimationCurve.Linear(0, active ? 1 : 0, 1 / 60f, active ? 1 : 0)
            );
            AnimatorState state = stateMachine.AddState(active ? "ON" : "OFF");
            state.motion = animation;
            state.writeDefaultValues = writeDefaults;
            return state;
        }
        
        private string GetRelativePath(GameObject root, GameObject child)
        {
            List<string> paths = new List<string>();
            Transform currentPath = child.transform;
            while (currentPath != null && currentPath != root.transform)
            {
                paths.Add(currentPath.name);
                currentPath = currentPath.parent;
            }
            paths.Reverse();
            return string.Join("/", paths);
        }

        private string GetUUID()
        {
            var guid = System.Guid.NewGuid();
            return guid.ToString();
        }

        private void AttachMAComponents(EasyObjectSwitch targetComponent, AnimatorController animator)
        {
            // Prepare parameter name list
            List<IParameterInfo> parametersInfo = new List<IParameterInfo>();
            if (targetComponent.generateMode == GenerateMode.SWITCH_ALL) {
                parametersInfo.Add((IParameterInfo)targetComponent);
            } else if (targetComponent.generateMode == GenerateMode.SWITCH_INDIVIDUALLY) {
                parametersInfo = targetComponent.switchTargets.Cast<IParameterInfo>().ToList();
            }
            // ModularAvatarMergeAnimator
            ModularAvatarMergeAnimator mergeAnimator = targetComponent.gameObject.AddComponent<ModularAvatarMergeAnimator>() as ModularAvatarMergeAnimator;
            mergeAnimator.animator = animator;
            mergeAnimator.layerType = VRCAvatarDescriptor.AnimLayerType.FX;
            mergeAnimator.deleteAttachedAnimator = false;
            mergeAnimator.pathMode = MergeAnimatorPathMode.Relative;
            mergeAnimator.matchAvatarWriteDefaults = targetComponent.matchAvatarWriteDefaults;
            // ModularAvatarMenuInstaller
            ModularAvatarMenuInstaller menuInstaller = targetComponent.gameObject.AddComponent<ModularAvatarMenuInstaller>() as ModularAvatarMenuInstaller;
            if (targetComponent.groupingIntoSubmenu) {
                string submenuName = String.IsNullOrEmpty(targetComponent.submenuName) ? targetComponent.gameObject.name : targetComponent.submenuName;
                menuInstaller.menuToAppend = WrapExpressionMenu(GenerateExpressionMenu(parametersInfo), submenuName, targetComponent.submenuIcon);
            } else {
                menuInstaller.menuToAppend = GenerateExpressionMenu(parametersInfo);
            }
            menuInstaller.installTargetMenu = targetComponent.installTo;
            // ModularAvatarParameters
            ModularAvatarParameters parameters;
            if (targetComponent.gameObject.GetComponent<ModularAvatarParameters>()) {
                parameters = targetComponent.gameObject.GetComponent<ModularAvatarParameters>() as ModularAvatarParameters;
            } else {
                parameters = targetComponent.gameObject.AddComponent<ModularAvatarParameters>() as ModularAvatarParameters;
            }
            bool localOnly = !targetComponent.IsSynced();
            bool saved = targetComponent.IsSaved();
            foreach (var parameterInfo in parametersInfo) {
                Debug.Log(parameterInfo.GetUniqueParameterName());
                if (targetComponent.generateMode == GenerateMode.SWITCH_INDIVIDUALLY) {
                    localOnly = !parameterInfo.IsSynced();
                    saved = parameterInfo.IsSaved();
                }
                ParameterConfig parameterConfig = new ParameterConfig() {
                    nameOrPrefix = parameterInfo.GetUniqueParameterName(),
                    remapTo = "",
                    internalParameter = false,
                    isPrefix = false,
                    syncType = ParameterSyncType.Bool,
                    localOnly = localOnly,
                    defaultValue = 0.0f,
                    saved = saved
                };
                parameters.parameters.Add(parameterConfig);
            }
        }

        private VRCExpressionsMenu GenerateExpressionMenu(List<IParameterInfo> parametersInfo)
        {
            VRCExpressionsMenu expressionMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            foreach (var parameterInfo in parametersInfo) {
                Texture2D icon = parameterInfo.GetIcon();
                VRCExpressionsMenu.Control.Parameter expressionParameter = new VRCExpressionsMenu.Control.Parameter() {
                    name = parameterInfo.GetUniqueParameterName()
                };
                VRCExpressionsMenu.Control expressionMenuControl = new VRCExpressionsMenu.Control() {
                    name = parameterInfo.baseParameterName,
                    icon = icon,
                    type = VRCExpressionsMenu.Control.ControlType.Toggle,
                    parameter = expressionParameter,
                    value = 1
                };
                expressionMenu.controls.Add(expressionMenuControl);
            }
            return expressionMenu;
        }

        private VRCExpressionsMenu WrapExpressionMenu(VRCExpressionsMenu targetExpressionMenu, string menuName, Texture2D icon)
        {
            VRCExpressionsMenu expressionMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            VRCExpressionsMenu.Control expressionMenuControl = new VRCExpressionsMenu.Control() {
                name = menuName,
                icon = icon,
                type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                subMenu = targetExpressionMenu
            };
            expressionMenu.controls.Add(expressionMenuControl);
            return expressionMenu;
        }

    }

}
