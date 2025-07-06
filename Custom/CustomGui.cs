using Dalamud.Interface.ImGuiNotification;
using ImGuiNET;
using OtterGui.Classes;
using OtterGui.Raii;
using OtterGui.Text;
using OtterGui.Widgets;
using OtterGuiInternal.Enums;

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

        DrawLinkButton(message, "加入Discord寻求支持", address, width, $"访问{address}\n插件开发者的频道。\n请注意他们不对国服作支持，有问题最好先去国服MOD频道咨询。");
    }

    /// <summary> Draw a button to open the cn discord server. </summary>
    public static void DrawCNDiscordButton(MessageService message, float width)
    {
        const string address = @"https://discord.gg/QvrVye3";
        using var color = ImRaii.PushColor(ImGuiCol.Button, DiscordColor);

        DrawLinkButton(message, "加入国服MOD频道", address, width, $"访问{address}\n许多MOD的原作者都提供免费下载，\n请不要从倒卖者（某火狐、某鱼、某宝等）处花钱购买。");
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

    public static void DrawKofiPatreonButton(MessageService message, Vector2 size)
    {
        const string kofiAddress    = "https://ko-fi.com/ottermandias";
        const string patreonAddress = "https://www.patreon.com/Ottermandias";
        var          half           = size with { X = size.X / 2 };

        switch (ToggleButton.SplitButton((ImGuiId)5, new ToggleButton.SplitButtonData()
                {
                    Label      = "Ko-Fi"u8,
                    Active     = 0xFF5B5EFFu,
                    Background = 0xFFFFC313u,
                    Hovered    = ImGui.GetColorU32(ImGuiCol.ButtonHovered),
                    Tooltip =
                        "Open Ottermandias' Ko-Fi at https://ko-fi.com/ottermandias in your browser.\n\nAny donations made are entirely voluntary and will not yield any preferential treatment or benefits beyond making Otter happy."u8,
                }, new ToggleButton.SplitButtonData()
                {
                    Label      = "Patreon"u8,
                    Active     = 0xFF492C00u,
                    Hovered    = ImGui.GetColorU32(ImGuiCol.ButtonHovered),
                    Background = 0xFF5467F7u,
                    Tooltip =
                        "Open Ottermandias' Patreon at https://www.patreon.com/Ottermandias in your browser.\n\nAny donations made are entirely voluntary and will not yield any preferential treatment or benefits beyond making Otter happy."u8,
                }, size, MixColors(0xFFFFC313u, 0xFF5467F7u)))
        {
            case 1:
                try
                {
                    var process = new ProcessStartInfo("https://ko-fi.com/ottermandias")
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(process);
                }
                catch
                {
                    message.NotificationMessage($"Could not open Ko-Fi link at {kofiAddress} in external browser", NotificationType.Error);
                }

                break;
            case 2:
                try
                {
                    var process = new ProcessStartInfo(patreonAddress)
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(process);
                }
                catch
                {
                    message.NotificationMessage($"Could not open Patreon link at {patreonAddress} in external browser", NotificationType.Error);
                }

                break;
        }


        uint MixColors(uint x, uint y)
        {
            var r = ((x & 0xFF) + (y & 0xFF)) / 2;
            var g = (((x >> 8) & 0xFF) + ((y >> 8) & 0xFF)) / 2;
            var b = (((x >> 16) & 0xFF) + ((y >> 16) & 0xFF)) / 2;
            return r | (g << 8) | (b << 16) | 0xFF000000u;
        }
    }
}
