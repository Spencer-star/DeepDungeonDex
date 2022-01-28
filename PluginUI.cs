using ImGuiNET;
using System.Numerics;
using Dalamud.Game.ClientState;

namespace DeepDungeonDex
{
    public class PluginUI
    {
        public bool IsVisible { get; set; }
        private Configuration config;
        private ClientState clientState;

        public PluginUI(Configuration config, ClientState clientState)
        {
            this.config = config;
            this.clientState = clientState;
        }

        private readonly bool[] cjstun = 
        {
            true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, false, true, false, false, false, false, true, true, false, true, false, true, false, true, true, false, true, false
        };

        private readonly bool[] cjsleep =
        {
            true, false, false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, false, false, false, false, true, false, true, true, false, false, false, true
        };
        
        private readonly bool[] cjbind =
        {
            true, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, true, true, false, false, false, false, true, false, true, false, false
        };
        
        private readonly bool[] cjheavy =
        {
            true, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, true, true, false, false, false, false, true, false, true, false, false
        };
        
        private readonly bool[] cjslow =
        {
            true, true, true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, false, false, false, false, false, true, true, true, true, false, true, false, true, true, true, true, false
        };

        private void PrintSingleVuln(bool? isVulnerable, string message)
        { 
            switch (isVulnerable)
            {
                case true:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF00FF00);
                    ImGui.Text(message);
                    ImGui.PopStyleColor();
                    break;
                case false:
                    if (!config.HideRedVulns)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, 0xFF0000FF);
                        ImGui.Text(message);
                        ImGui.PopStyleColor();
                    }
                    break;
                default:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0x50FFFFFF);
                    ImGui.Text(message);
                    ImGui.PopStyleColor();
                    break;
            }
        }
        public void Draw()
        {
            if (!IsVisible)
                return;
            var cjid = clientState.LocalPlayer == null ? 0 : clientState.LocalPlayer.ClassJob.GameData.RowId;
            var mobData = DataHandler.Mobs(TargetData.NameID);
            if (mobData == null) return;
            var flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar;
            if (config.IsClickthrough)
            {
                flags |= ImGuiWindowFlags.NoInputs;
            }
            ImGui.SetNextWindowSizeConstraints(new Vector2(250, 0), new Vector2(9001, 9001));
            ImGui.SetNextWindowBgAlpha(config.Opacity);
            ImGui.Begin("cool strati window", flags);
            ImGui.Text("怪物名称:\n"+TargetData.Name);
            ImGui.NewLine();
            ImGui.Columns(3, null, false);
            ImGui.Text("攻击类型:\n");
            ImGui.Text(mobData.Aggro.ToString());
            ImGui.NextColumn();
            ImGui.Text("威胁程度:\n");
            switch (mobData.Threat)
            {
                case DataHandler.MobData.ThreatLevel.Easy:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF00FF00);
                    ImGui.Text("简单");
                    ImGui.PopStyleColor();
                    break;
                case DataHandler.MobData.ThreatLevel.Caution:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF00FFFF);
                    ImGui.Text("需小心");
                    ImGui.PopStyleColor();
                    break;
                case DataHandler.MobData.ThreatLevel.Dangerous:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFF0000FF);
                    ImGui.Text("危险");
                    ImGui.PopStyleColor();
                    break;
                case DataHandler.MobData.ThreatLevel.Vicious:
                    ImGui.PushStyleColor(ImGuiCol.Text, 0xFFFF00FF);
                    ImGui.Text("非常危险");
                    ImGui.PopStyleColor();
                    break;
                default:
                    ImGui.Text("未定义难度");
                    break;
            }
            ImGui.NextColumn();
            if (!config.HideBasedOnJob || cjstun[cjid])
            {
                PrintSingleVuln(mobData.Vuln.CanStun, "可以击晕");    
            }
            if (!config.HideBasedOnJob || cjsleep[cjid])
            {
                PrintSingleVuln(mobData.Vuln.CanSleep, "可以睡眠");    
            }
            if (!config.HideBasedOnJob || cjbind[cjid])
            {
                PrintSingleVuln(mobData.Vuln.CanBind, "可以定身");
            }
            if (!config.HideBasedOnJob || cjheavy[cjid])
            {
                PrintSingleVuln(mobData.Vuln.CanHeavy, "可以减速（沉重）");   
            }
            if (!config.HideBasedOnJob || cjslow[cjid])
            {
                PrintSingleVuln(mobData.Vuln.CanSlow, "可以减速（缓行）");   
            }
            if (!(TargetData.NameID >= 7262 && TargetData.NameID <= 7610))
            {
                PrintSingleVuln(mobData.Vuln.IsUndead, "不死族");
            }
            ImGui.NextColumn();
            ImGui.Columns(1);
            ImGui.NewLine();
            ImGui.TextWrapped(mobData.MobNotes);
            ImGui.End();
        }
    }
}
