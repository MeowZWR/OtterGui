using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Raii;
using OtterGui.Text;

namespace OtterGui.Widgets;

public class FontAwesomeCheckbox(FontAwesomeIcon icon, uint? color = null) : FontAwesomeCheckbox<bool>
{
    public readonly FontAwesomeIcon Icon  = icon;
    public readonly uint?           Color = color;

    protected override (FontAwesomeIcon? Icon, uint? Color) GetIcon(bool value)
        => value ? (Icon, Color) : (null, null);

    protected override bool NextValue(bool value)
        => !value;

    protected override bool PreviousValue(bool value)
        => !value;
}

public abstract class FontAwesomeCheckbox<T> : MultiStateCheckbox<T>
{
    protected abstract (FontAwesomeIcon? Icon, uint? Color) GetIcon(T value);

    protected override void RenderSymbol(T value, Vector2 position, float size)
    {
        var (maybeIcon, color) = GetIcon(value);
        if (!maybeIcon.HasValue)
            return;

        var icon = maybeIcon.Value.Bytes();

        // FIXME honor size parameter if possible
        using var font = ImRaii.PushFont(UiBuilder.IconFont);

        var iconSize     = ImUtf8.CalcTextSize(icon.Span);
        var iconPosition = position + (new Vector2(size) - iconSize) * 0.5f;

        ImGui.GetWindowDrawList().AddText(icon.Span, iconPosition, color ?? ImGui.GetColorU32(ImGuiCol.CheckMark));
    }
}
