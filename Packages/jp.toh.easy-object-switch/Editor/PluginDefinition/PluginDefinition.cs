using System;

using UnityEngine;

using nadena.dev.ndmf;

[assembly: ExportsPlugin(
    typeof(jp.toh.easy_object_switch.editor.plugin.PluginDefinition)
)]

namespace jp.toh.easy_object_switch.editor.plugin
{

    class PluginDefinition : Plugin<PluginDefinition>
    {
        public override string QualifiedName => "jp.toh.easy-object-switch";

        public override string DisplayName => "Easy Object Switch";

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .BeforePlugin("nadena.dev.modular-avatar")
                .Run("EasyObjectSwitch: Create animation clips, animator controllers and Modular Avatar components for objetct switch", ctx => {
                    Debug.Log("EasyObjectSwitch: Process start");
                    new EasyObjectSwitchProcessor(ctx).Process();
                    Debug.Log("EasyObjectSwitch: Process finished");
                });
        }
    }
    
}
