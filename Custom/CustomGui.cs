using Dalamud.Interface.ImGuiNotification;
using ImGuiNET;
using OtterGui.Classes;
using OtterGui.Raii;

namespace OtterGui.Custom;

public static class CustomGui
{
    public const uint DiscordColor     = 0xFFDA8972;
    public const uint ReniColorButton  = 0xFFCC648D;
    public const uint ReniColorHovered = 0xFFB070B0;
    public const uint ReniColorActive  = 0xFF9070E0;

    /// <summary> Draw a button to open the official discord server. </summary>
    public static void DrawDiscordButton(MessageService message, float width)
    {
        const string address = @"https://discord.gg/kVva7DHV4r";
        using var    color   = ImRaii.PushColor(ImGuiCol.Button, DiscordColor);

        DrawLinkButton(message, "加入Discord寻求支持", address, width, $"访问{address}\n插件开发者的频道。");
    }

    /// <summary> Draw a button to open the cn discord server. </summary>
    public static void DrawCNDiscordButton(MessageService message, float width)
    {
        const string address = @"https://discord.gg/QvrVye3";
        using var color = ImRaii.PushColor(ImGuiCol.Button, DiscordColor);

        DrawLinkButton(message, "加入国服MOD频道", address, width, $"访问{address}\n许多MOD的原作者都提供免费下载，\n请不要从倒卖者（如某火狐、某鱼、某宝等）处花钱购买。");
    }

    /// <summary> Draw the button that opens the ReniGuide. </summary>
    public static void DrawGuideButton(MessageService message, float width)
    {
        const string address = @"https://reniguide.info/";
        using var color = ImRaii.PushColor(ImGuiCol.Button, ReniColorButton)
            .Push(ImGuiCol.ButtonHovered, ReniColorHovered)
            .Push(ImGuiCol.ButtonActive,  ReniColorActive);

        DrawLinkButton(message, "新手指南", address, width,
            $"访问{address}\nSerenity制作的基于图像和文本的指南，包含大部分Penumbra功能。\n"
          + "不是插件作者官方指南，但一般不会过时。");
    }

    /// <summary> Draw a button that opens an address in the browser. </summary>
    public static void DrawLinkButton(MessageService message, string text, string address, float width, string? tooltip = null)
    {
        if (ImGui.Button(text, new Vector2(width, 0)))
            try
            {
                var process = new ProcessStartInfo(address)
                {
                    UseShellExecute = true,
                };
                Process.Start(process);
            }
            catch
            {
                message.NotificationMessage($"Could not open {text} at {address} in external browser", NotificationType.Error);
            }

        if (tooltip != null)
            ImGuiUtil.HoverTooltip(tooltip);
    }
}
