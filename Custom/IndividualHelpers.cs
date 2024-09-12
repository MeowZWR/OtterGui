using Dalamud.Game.ClientState.Objects.Enums;
using ImGuiNET;
using OtterGui.Raii;

namespace OtterGui.Custom;

public static class IndividualHelpers
{
    public static bool DrawObjectKindCombo(float width, ObjectKind current, out ObjectKind result, IEnumerable<ObjectKind> kinds)
    {
        ImGui.SetNextItemWidth(width);
        using var combo = ImRaii.Combo("##newKind", current.ToName());
        result = current;
        if (!combo)
            return false;

        var ret = false;
        foreach (var kind in kinds)
        {
            if (!ImGui.Selectable(kind.ToName(), current == kind))
                continue;

            result = kind;
            ret    = true;
        }

        return ret;
    }

    public static string ToName(this ObjectKind kind)
        => kind switch
        {
            ObjectKind.None      => "未知",
            ObjectKind.BattleNpc => "战斗NPC",
            ObjectKind.EventNpc  => "事件NPC",
            ObjectKind.MountType => "坐骑",
            ObjectKind.Companion => "宠物",
            ObjectKind.Ornament  => "时尚配饰",
            _                    => kind.ToString(),
        };
}
