using System.Collections.Generic;
using System.Collections.Immutable;

namespace jp.toh.easy_object_switch.editor
{
    public static class PropertyUtil
    {

        public static Language language = Language.JAPANESE;
        
        public static readonly ImmutableDictionary<Language, ImmutableDictionary<string, string>> properties = new Dictionary<Language, ImmutableDictionary<string, string>>()
        {
            {
                Language.JAPANESE,
                new Dictionary<string, string>() {
                    {"Error.Icon.Size.Too.Large", "アイコンサイズが大きすぎます。\nアイコンサイズは 256x256 以下である必要があります。"},
                    {"Warning.VRC.Avatar.Descriptor.Check", "Easy Object Switch は VRC Avatar Descriptor の中にないと機能しません。"},
                    {"Info.Conflict.Modular.Avatar.Parameters", "Easy Object Switch は MA Parameters に設定を追加するので、既存の項目と干渉する可能性があります。"},
                    {"Info.Install.To.Is.Null", "インストール先が設定されていません。\nその場合 VRC Avatar Descriptor で設定されたメニューに追加されます。"},
                    {"Caption.Install.To", "メニューの追加先"},
                    {"Caption.Mode", "切替モード"},
                    {"Enum.Generate.Mode.Switch.All", "全て一斉に切り替える"},
                    {"Enum.Generate.Mode.Switch.Individually", "個別に切り替える"},
                    {"Caption.Menu.Name", "メニュー名"},
                    {"Tooltip.Menu.Name", "ON/OFF を切り替えるメニューの名前を設定します。\n空欄の場合は GameObject の名前を使用します。"},
                    {"Caption.Saved", "切替を保存"},
                    {"Tooltip.Saved", "アバターを切り替えたりしたても ON/OFF の状態を保存するか選択します。"},
                    {"Caption.Synced", "切替を同期"},
                    {"Tooltip.Synced", "ON/OFF を他のプレイヤーにも同期するか選択します。"},
                    {"Caption.Icon", "アイコン"},
                    {"Caption.Grouping.Into.Submenu", "サブメニューにまとめる"},
                    {"Tooltip.Grouping.Into.Submenu", "チェックをつけると「メニューの追加先」で指定したメニューにサブメニューを作成し、その中に切り替えメニューを作成します。"},
                    {"Caption.Submenu.Name", "サブメニュー名"},
                    {"Tooltip.Submenu.Name", "空欄の場合はこの GameObject の名前を使用します。"},
                    {"Caption.Target.Object.List", "対象のオブジェクトリスト"},
                    {"Target.List.Caption.Target", "対象オブジェクト"},
                    {"Target.List.Tooltip.Target", "ON/OFF を切り替えるオブジェクトを選びます。"},
                    {"Target.List.Caption.Default", "デフォルト状態"},
                    {"Target.List.Tooltip.Default", "デフォルトの状態を選択します。\n「対象オブジェクトの状態に合わせる」を選ぶと、対象がアクティブならON、非アクティブならOFFにします。"},
                    {"Enum.Default.State.Inherit", "対象オブジェクトの状態に合わせる"},
                    {"Enum.Default.State.Off", "OFF"},
                    {"Enum.Default.State.On", "ON"},
                    {"Target.List.Caption.List.Is.Empty", "リストは空です"},
                    {"Target.List.Button.Auto.Detect", "オブジェクトを自動検出"},
                    {"Caption.Advanced.Settings", "詳細設定を有効化"},
                    {"Tooltip.Advanced.Settings", "パラメータの同期や保存、アイコンの設定、その他細かい設定が可能になります。"},
                    {"Caption.Advanced.Match.Avatar.Write.Defaults", "アバターの Write Defaults 設定に合わせる"},
                    {"Tooltip.Advanced.Match.Avatar.Write.Defaults", "チェックをつけるとアバターのアニメーターの Write Defaults 設定に合わせます。"},
                    {"Caption.Advanced.Write.Defaults", "Write Defaults を ON"},
                    {"Tooltip.Advanced.Write.Defaults", "チェックをつけると Write Defaults を ON の状態で Animator Controller を作成します。\n「アバターの Write Defaults 設定に合わせる」がOFFの場合のみ有効です。"},
                    {"Caption.Advanced.Parameters.Without.UUID", "パラメーターに UUID を追加しない"},
                    {"Tooltip.Advanced.Parameters.Without.UUID", "チェックをつけると、他のパラメーターと干渉することを防ぐために自動的に追加される UUID を追加しないようになります。"},
                    {"Error.Icon.Size.Too.Large.For.Items", "{0} 番目の項目で、アイコンサイズが大きすぎます。\nアイコンサイズは 256x256 以下である必要があります。"},
                    {"Error.Target.Is.Null", "{0} 番目の項目で、対象オブジェクトが指定されていません。\nGameObject を指定するか、項目を削除してください。"},
                    {"Error.Target.Is.Not.Descendant", "{0} 番目の項目で、対象オブジェクトはこの GameObject の子孫要素ではありません。\nこの GameObject に含まれている GameObject を選択してください。"}
                }.ToImmutableDictionary()
            },
            {
                Language.ENGLISH,
                new Dictionary<string, string>() {
                    {"Error.Icon.Size.Too.Large", "Icon size is too large.\nIcon size must be 256x256 or smaller."},
                    {"Warning.VRC.Avatar.Descriptor.Check", "Easy Object Switch must be inside a VRC Avatar Descriptor to work."},
                    {"Info.Conflict.Modular.Avatar.Parameters", "Easy Object Switch may conflict with existing items because Easy Object Switch adds settings to MA Parameters."},
                    {"Info.Install.To.Is.Null", "\"Install To\" is not set.\nIn this case, the menu will be added to the menu set in VRC Avatar Descriptor."},
                    {"Caption.Install.To", "Install to"},
                    {"Caption.Mode", "Mode"},
                    {"Enum.Generate.Mode.Switch.All", "Switch all"},
                    {"Enum.Generate.Mode.Switch.Individually", "Switch individually"},
                    {"Caption.Menu.Name", "Menu name"},
                    {"Tooltip.Menu.Name", "Set a ON/OFF switching menu name.\nUse GameObject name if empty."},
                    {"Caption.Saved", "Saved"},
                    {"Tooltip.Saved", "Select whether to save the ON/OFF status."},
                    {"Caption.Synced", "Synced"},
                    {"Tooltip.Synced", "Select whether to sync the ON/OFF status."},
                    {"Caption.Icon", "Icon"},
                    {"Caption.Grouping.Into.Submenu", "Grouping into submenu"},
                    {"Tooltip.Grouping.Into.Submenu", "A submenu is created in the menu specified in \"Install to\" and a switching menu is created in the submenu when checked."},
                    {"Caption.Submenu.Name", "Submenu name"},
                    {"Tooltip.Submenu.Name", "Use this GameObject name if empty."},
                    {"Caption.Target.Object.List", "Target object list"},
                    {"Target.List.Caption.Target", "Target"},
                    {"Target.List.Tooltip.Target", "Select the object to switching ON/OFF"},
                    {"Target.List.Caption.Default", "Default"},
                    {"Target.List.Tooltip.Default", "Select the default state.\n\"Inherit target object state\" means ON if the target is active and OFF if it is inactive."},
                    {"Enum.Default.State.Inherit", "Inherit target object state"},
                    {"Enum.Default.State.Off", "OFF"},
                    {"Enum.Default.State.On", "ON"},
                    {"Target.List.Caption.List.Is.Empty", "List is empty"},
                    {"Target.List.Button.Auto.Detect", "Auto detect target objects"},
                    {"Caption.Advanced.Settings", "Enable advanced settings"},
                    {"Tooltip.Advanced.Settings", "Enables param sync, param save, icon, and etc. settings."},
                    {"Caption.Advanced.Match.Avatar.Write.Defaults", "Match avatar Write Defaults settings"},
                    {"Tooltip.Advanced.Match.Avatar.Write.Defaults", "Match the Write Defaults settings to the avatar's animation controller."},
                    {"Caption.Advanced.Write.Defaults", "Write Defaults"},
                    {"Tooltip.Advanced.Write.Defaults", "The Animator Controller will be created with Write Defaults ON when checked.\nThis setting is enable only if \"Match avatar Write Defaults settings\" is disabled."},
                    {"Caption.Advanced.Parameters.Without.UUID", "Do not add UUID to parameters"},
                    {"Tooltip.Advanced.Parameters.Without.UUID", "If checked, do not add UUID that are automatically added to prevent interference with other parameters."},
                    {"Error.Icon.Size.Too.Large.For.Items", "In the {0}st/nd/rd/th item, icon size is too large.\nIcon size must be 256x256 or smaller."},
                    {"Error.Target.Is.Null", "In the {0}st/nd/rd/th item, the target object is not specified.\nSpecify GameObject or delete the item."},
                    {"Error.Target.Is.Not.Descendant", "In the {0}st/nd/rd/th item, the target object is not a descendant element of this GameObject.\nSelect the GameObject contained in this GameObject."}
                }.ToImmutableDictionary()
            }
        }.ToImmutableDictionary();

        public static string GetProperty(string property)
        {
            string result;
            if (properties[language].TryGetValue(property, out result)) {
                return result;
            } else {
                return $"Property is not found: key={property}, language={language.ToString()}";
            }
        }

        public static void SetDefaultLanguage()
        {
            language = Language.JAPANESE;
        }

    }

    public enum Language
    {
        JAPANESE,
        ENGLISH
    }

}