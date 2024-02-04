using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using nadena.dev.modular_avatar.core;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace jp.toh.easy_object_switch.editor
{

    [CustomEditor(typeof(EasyObjectSwitch))]
    public class EasyObjectSwitchCustomEditor : Editor
    {

        private ReorderableList reorderableList;

        private static readonly int EMPTY_ITEM_SIZE = 2;

        private static readonly int SHORT_ITEM_SIZE = 2;

        private static readonly int LONG_ITEM_SIZE = 3;

        private static readonly int ADVANCED_ITEM_SIZE = 3;

        private static readonly int MAX_ICON_SIZE = 256;

        void OnEnable()
        {
            // Prepare reorderableList
            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(EasyObjectSwitch.switchTargets));
            reorderableList = new ReorderableList(serializedObject, serializedProperty) {
                draggable = true,
                onAddCallback = (e) => {
                    // Add a new element
                    int index = e.serializedProperty.arraySize;
                    e.serializedProperty.InsertArrayElementAtIndex(index);
                    // Set default value
                    SerializedProperty addedProperty = e.serializedProperty.GetArrayElementAtIndex(index);
                    addedProperty.FindPropertyRelative(nameof(SwitchTarget.target)).objectReferenceValue = null;
                    addedProperty.FindPropertyRelative(nameof(SwitchTarget.defaultState)).enumValueIndex = (int)DefaultState.INHERIT;
                    addedProperty.FindPropertyRelative(nameof(SwitchTarget.menuName)).stringValue = "";
                    addedProperty.FindPropertyRelative(nameof(SwitchTarget.synced)).boolValue = true;
                },
                drawElementCallback = (rect, index, isActive, isFocused) => {
                    SerializedProperty element = serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element);
                },
                drawNoneElementCallback = (rect) => {
                    Rect labelRect = new Rect(rect) {
                        height = EditorGUIUtility.singleLineHeight,
                        y = rect.y + SwitchTargetDrawer.ELEMENT_MARGIN
                    };
                    Rect buttonRect = new Rect(rect) {
                        height = EditorGUIUtility.singleLineHeight,
                        y = rect.y + EditorGUIUtility.singleLineHeight + SwitchTargetDrawer.ELEMENT_MARGIN * 2
                    };
                    EditorGUI.LabelField(labelRect, PropertyUtil.GetProperty("Target.List.Caption.List.Is.Empty"));
                    if (GUI.Button(buttonRect, PropertyUtil.GetProperty("Target.List.Button.Auto.Detect"))) {
                        ((EasyObjectSwitch)target).AutoDetectSwitchTargets();
                    }
                },
                drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, PropertyUtil.GetProperty("Caption.Target.Object.List"))
            };
            // Read language settings
            string language = EditorUserSettings.GetConfigValue("toh.csecb.common.language");
            Language languageEnum;
            if (Enum.TryParse(language, out languageEnum)) {
                PropertyUtil.language = languageEnum;
            } else {
                PropertyUtil.SetDefaultLanguage();
            }
        }

        public override void OnInspectorGUI()
        {

            EditorGUI.BeginChangeCheck();
            Language selectedLanguageEnum = (Language)EditorGUILayout.EnumPopup(new GUIContent("Language", "Select a component language"), PropertyUtil.language);
            if (EditorGUI.EndChangeCheck())
            {
                PropertyUtil.language = selectedLanguageEnum;
                EditorUserSettings.SetConfigValue("toh.csecb.common.language", selectedLanguageEnum.ToString());
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            serializedObject.Update();

            if (!findVRCAvatarDescriptorInParent(((EasyObjectSwitch)target).gameObject)) {
                EditorGUILayout.HelpBox(PropertyUtil.GetProperty("Warning.VRC.Avatar.Descriptor.Check"), MessageType.Warning);
            }
            if (((EasyObjectSwitch)target).gameObject.GetComponent<ModularAvatarParameters>()) {
                EditorGUILayout.HelpBox(PropertyUtil.GetProperty("Info.Conflict.Modular.Avatar.Parameters"), MessageType.Info);
            }
            if (serializedObject.FindProperty(nameof(EasyObjectSwitch.installTo)).objectReferenceValue == null) {
                EditorGUILayout.HelpBox(PropertyUtil.GetProperty("Info.Install.To.Is.Null"), MessageType.Info);
            }

            bool advancedMode = serializedObject.FindProperty(nameof(EasyObjectSwitch.advancedMode)).boolValue;

            serializedObject.FindProperty(nameof(EasyObjectSwitch.installTo)).objectReferenceValue = EditorGUILayout.ObjectField(
                PropertyUtil.GetProperty("Caption.Install.To"),
                serializedObject.FindProperty(nameof(EasyObjectSwitch.installTo)).objectReferenceValue,
                typeof(VRCExpressionsMenu),
                true);

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            serializedObject.FindProperty(nameof(EasyObjectSwitch.generateMode)).enumValueIndex = EditorGUILayout.Popup(
                PropertyUtil.GetProperty("Caption.Mode"),
                serializedObject.FindProperty(nameof(EasyObjectSwitch.generateMode)).enumValueIndex,
                Array.ConvertAll(GenerateModeHelper.GetPropertyKeys(), new Converter<string, string>(PropertyUtil.GetProperty)));
            GenerateMode selectedGenerateMode = (GenerateMode)Enum.ToObject(typeof(GenerateMode), serializedObject.FindProperty(nameof(EasyObjectSwitch.generateMode)).enumValueIndex);
            if (selectedGenerateMode == GenerateMode.SWITCH_ALL) {
                EditorGUI.indentLevel++;
                serializedObject.FindProperty(nameof(EasyObjectSwitch.menuName)).stringValue = EditorGUILayout.TextField(
                    new GUIContent(PropertyUtil.GetProperty("Caption.Menu.Name"), PropertyUtil.GetProperty("Tooltip.Menu.Name")),
                    serializedObject.FindProperty(nameof(EasyObjectSwitch.menuName)).stringValue);
                if (advancedMode) {
                    Texture2D menuIcon = (Texture2D)serializedObject.FindProperty(nameof(EasyObjectSwitch.icon)).objectReferenceValue;
                    serializedObject.FindProperty(nameof(EasyObjectSwitch.icon)).objectReferenceValue = EditorGUILayout.ObjectField(
                        PropertyUtil.GetProperty("Caption.Icon"),
                        menuIcon,
                        typeof(Texture2D),
                        false,
                        GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                    if (menuIcon != null && (menuIcon.height > MAX_ICON_SIZE || menuIcon.width > MAX_ICON_SIZE)) {
                        EditorGUILayout.HelpBox(PropertyUtil.GetProperty("Error.Icon.Size.Too.Large"), MessageType.Error);
                    }
                    serializedObject.FindProperty(nameof(EasyObjectSwitch.saved)).boolValue = EditorGUILayout.Toggle(
                        new GUIContent(PropertyUtil.GetProperty("Caption.Saved"), PropertyUtil.GetProperty("Tooltip.Saved")),
                        serializedObject.FindProperty(nameof(EasyObjectSwitch.saved)).boolValue);
                    serializedObject.FindProperty(nameof(EasyObjectSwitch.synced)).boolValue = EditorGUILayout.Toggle(
                        new GUIContent(PropertyUtil.GetProperty("Caption.Synced"), PropertyUtil.GetProperty("Tooltip.Synced")),
                        serializedObject.FindProperty(nameof(EasyObjectSwitch.synced)).boolValue);
                }
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            serializedObject.FindProperty(nameof(EasyObjectSwitch.groupingIntoSubmenu)).boolValue = EditorGUILayout.Toggle(
                new GUIContent(PropertyUtil.GetProperty("Caption.Grouping.Into.Submenu"), PropertyUtil.GetProperty("Tooltip.Grouping.Into.Submenu")),
                serializedObject.FindProperty(nameof(EasyObjectSwitch.groupingIntoSubmenu)).boolValue);
            if (serializedObject.FindProperty(nameof(EasyObjectSwitch.groupingIntoSubmenu)).boolValue) {
                EditorGUI.indentLevel++;
                serializedObject.FindProperty(nameof(EasyObjectSwitch.submenuName)).stringValue = EditorGUILayout.TextField(
                    new GUIContent(PropertyUtil.GetProperty("Caption.Submenu.Name"), PropertyUtil.GetProperty("Tooltip.Submenu.Name")),
                    serializedObject.FindProperty(nameof(EasyObjectSwitch.submenuName)).stringValue);
                if (advancedMode) {
                    Texture2D submenuIcon = (Texture2D)serializedObject.FindProperty(nameof(EasyObjectSwitch.submenuIcon)).objectReferenceValue;
                    serializedObject.FindProperty(nameof(EasyObjectSwitch.submenuIcon)).objectReferenceValue = EditorGUILayout.ObjectField(
                        PropertyUtil.GetProperty("Caption.Icon"),
                        submenuIcon,
                        typeof(Texture2D),
                        false,
                        GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                    if (submenuIcon != null && (submenuIcon.height > MAX_ICON_SIZE || submenuIcon.width > MAX_ICON_SIZE)) {
                        EditorGUILayout.HelpBox(PropertyUtil.GetProperty("Error.Icon.Size.Too.Large"), MessageType.Error);
                    }
                }
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            SwitchTargetDrawer.generateMode = selectedGenerateMode;
            SwitchTargetDrawer.advancedMode = advancedMode;
            int itemSize;
            if (reorderableList.serializedProperty.arraySize == 0) {
                itemSize = EMPTY_ITEM_SIZE;
            } else if (selectedGenerateMode == GenerateMode.SWITCH_ALL) {
                itemSize = SHORT_ITEM_SIZE;
            } else {
                itemSize = LONG_ITEM_SIZE;
                if (advancedMode) {
                    itemSize += ADVANCED_ITEM_SIZE;
                }
            }
            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight * itemSize + SwitchTargetDrawer.ELEMENT_MARGIN * (itemSize + 1);
            reorderableList.DoLayoutList();

            List<string> validateResult = validate();
            foreach (string errorMessage in validateResult) {
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
            }

            serializedObject.FindProperty(nameof(EasyObjectSwitch.advancedMode)).boolValue = EditorGUILayout.Toggle(
                new GUIContent(PropertyUtil.GetProperty("Caption.Advanced.Settings"), PropertyUtil.GetProperty("Tooltip.Advanced.Settings")),
                advancedMode);
            if (advancedMode) {
                EditorGUI.indentLevel++;
                serializedObject.FindProperty(nameof(EasyObjectSwitch.matchAvatarWriteDefaults)).boolValue = EditorGUILayout.Toggle(
                    new GUIContent(PropertyUtil.GetProperty("Caption.Advanced.Match.Avatar.Write.Defaults"), PropertyUtil.GetProperty("Tooltip.Advanced.Match.Avatar.Write.Defaults")),
                    serializedObject.FindProperty(nameof(EasyObjectSwitch.matchAvatarWriteDefaults)).boolValue);
                serializedObject.FindProperty(nameof(EasyObjectSwitch.writeDefaults)).boolValue = EditorGUILayout.Toggle(
                    new GUIContent(PropertyUtil.GetProperty("Caption.Advanced.Write.Defaults"), PropertyUtil.GetProperty("Tooltip.Advanced.Write.Defaults")),
                    serializedObject.FindProperty(nameof(EasyObjectSwitch.writeDefaults)).boolValue);
                serializedObject.FindProperty(nameof(EasyObjectSwitch.parametersWithoutUUID)).boolValue = EditorGUILayout.Toggle(
                    new GUIContent(PropertyUtil.GetProperty("Caption.Advanced.Parameters.Without.UUID"), PropertyUtil.GetProperty("Tooltip.Advanced.Parameters.Without.UUID")),
                    serializedObject.FindProperty(nameof(EasyObjectSwitch.parametersWithoutUUID)).boolValue);
                EditorGUI.indentLevel--;
            }
            serializedObject.ApplyModifiedProperties();
        }

        private bool findVRCAvatarDescriptorInParent(GameObject checkTarget)
        {
            Transform currentPath = checkTarget.transform;
            while (currentPath != null)
            {
                if (currentPath.gameObject.GetComponent<VRCAvatarDescriptor>()) {
                    return true;
                }
                currentPath = currentPath.parent;
            }
            return false;
        }

        private List<string> validate()
        {
            List<string> result = new List<string>();
            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(EasyObjectSwitch.switchTargets));
            for (int i = 0; i < serializedProperty.arraySize; i++) {
                Texture2D icon = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(SwitchTarget.icon)).objectReferenceValue as System.Object as Texture2D;
                if (icon != null && (icon.height > MAX_ICON_SIZE || icon.width > MAX_ICON_SIZE)) {
                    result.Add(string.Format(PropertyUtil.GetProperty("Error.Icon.Size.Too.Large.For.Items"), i + 1));
                }
                GameObject checkTarget = serializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(SwitchTarget.target)).objectReferenceValue as System.Object as GameObject;
                if (checkTarget == null) {
                    result.Add(string.Format(PropertyUtil.GetProperty("Error.Target.Is.Null"), i + 1));
                    continue;
                }
                if (!haveDescendantRelationship(((EasyObjectSwitch)target).gameObject, checkTarget)) {
                    result.Add(string.Format(PropertyUtil.GetProperty("Error.Target.Is.Not.Descendant"), i + 1));
                }
            }
            return result;
        }

        private bool haveDescendantRelationship(GameObject baseObject, GameObject descendantCandidate)
        {
            Transform currentPath = descendantCandidate.transform;
            while (currentPath != null)
            {
                if (currentPath.gameObject == baseObject) {
                    return true;
                }
                currentPath = currentPath.parent;
            }
            return false;
        }

    }

    [CustomPropertyDrawer(typeof(SwitchTarget))]
    public class SwitchTargetDrawer : PropertyDrawer
    {

        internal static GenerateMode generateMode = GenerateMode.SWITCH_ALL;

        internal static bool advancedMode = false;

        public static readonly int ELEMENT_MARGIN = 6;

        public static readonly int LABEL_WIDTH = 80;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                EditorGUIUtility.labelWidth = LABEL_WIDTH;
                Rect baseRect = new Rect(position) {
                    height = EditorGUIUtility.singleLineHeight
                };
                Rect[] rects = new Rect[6];
                for (int i = 0; i < rects.Length; i++) {
                    rects[i] = new Rect(baseRect) {
                        y = baseRect.y + baseRect.height * i + ELEMENT_MARGIN * ( i + 1 )
                    };
                }
                SerializedProperty targetProperty = property.FindPropertyRelative(nameof(SwitchTarget.target));
                SerializedProperty defaultStateProperty = property.FindPropertyRelative(nameof(SwitchTarget.defaultState));
                SerializedProperty menuNameProperty = property.FindPropertyRelative(nameof(SwitchTarget.menuName));
                SerializedProperty savedProperty = property.FindPropertyRelative(nameof(SwitchTarget.saved));
                SerializedProperty syncedProperty = property.FindPropertyRelative(nameof(SwitchTarget.synced));
                SerializedProperty iconProperty = property.FindPropertyRelative(nameof(SwitchTarget.icon));
                int rectIndex = 0;
                targetProperty.objectReferenceValue = EditorGUI.ObjectField(
                    rects[rectIndex++],
                    new GUIContent(PropertyUtil.GetProperty("Target.List.Caption.Target"), PropertyUtil.GetProperty("Target.List.Tooltip.Target")),
                    targetProperty.objectReferenceValue,
                    typeof(GameObject),
                    true);
                defaultStateProperty.enumValueIndex = (int)(DefaultState)EditorGUI.Popup(
                    rects[rectIndex++],
                    new GUIContent(PropertyUtil.GetProperty("Target.List.Caption.Default"), PropertyUtil.GetProperty("Target.List.Tooltip.Default")),
                    defaultStateProperty.enumValueIndex,
                    Array.ConvertAll(DefaultStateHelper.GetPropertyKeys(), new Converter<string, GUIContent>( e => { return new GUIContent(PropertyUtil.GetProperty(e)); } )));
                if (generateMode == GenerateMode.SWITCH_INDIVIDUALLY) {
                    menuNameProperty.stringValue = EditorGUI.TextField(
                        rects[rectIndex++],
                        new GUIContent(PropertyUtil.GetProperty("Caption.Menu.Name"), PropertyUtil.GetProperty("Tooltip.Menu.Name")),
                        menuNameProperty.stringValue);
                    if (advancedMode) {
                        iconProperty.objectReferenceValue = EditorGUI.ObjectField(
                            rects[rectIndex++],
                            PropertyUtil.GetProperty("Caption.Icon"),
                            iconProperty.objectReferenceValue,
                            typeof(Texture2D),
                            false);
                        savedProperty.boolValue = EditorGUI.Toggle(
                            rects[rectIndex++],
                            new GUIContent(PropertyUtil.GetProperty("Caption.Saved"), PropertyUtil.GetProperty("Tooltip.Saved")),
                            savedProperty.boolValue);
                        syncedProperty.boolValue = EditorGUI.Toggle(
                            rects[rectIndex++],
                            new GUIContent(PropertyUtil.GetProperty("Caption.Synced"), PropertyUtil.GetProperty("Tooltip.Synced")),
                            syncedProperty.boolValue);
                    }
                }
            }
        }

    }

}
