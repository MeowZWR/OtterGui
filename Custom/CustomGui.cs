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
    public const uint ChineseRedColor  = 0xFF6B7280;
    public const uint XmaColor         = 0xFFDA8972;
    public const uint HeliosphereColor = 0xFF6B7280;

    private static uint MixColors(uint x, uint y)
    {
        var r = ((x & 0xFF) + (y & 0xFF)) / 2;
        var g = (((x >> 8) & 0xFF) + ((y >> 8) & 0xFF)) / 2;
        var b = (((x >> 16) & 0xFF) + ((y >> 16) & 0xFF)) / 2;
        return r | (g << 8) | (b << 16) | 0xFF000000u;
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
                        "在浏览器中打开Ottermandias的Ko-Fi页面 https://ko-fi.com/ottermandias\n\n所有捐赠都是完全自愿的，不会获得任何优先待遇或特殊福利，只是让Otter开心。"u8,
                }, new ToggleButton.SplitButtonData()
                {
                    Label      = "Patreon"u8,
                    Active     = 0xFF492C00u,
                    Hovered    = ImGui.GetColorU32(ImGuiCol.ButtonHovered),
                    Background = 0xFF5467F7u,
                    Tooltip =
                        "在浏览器中打开Ottermandias的Patreon页面 https://www.patreon.com/Ottermandias\n\n所有捐赠都是完全自愿的，不会获得任何优先待遇或特殊福利，只是让Otter开心。"u8,
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
    }

    /// <summary> Draw a split button for international and Chinese Discord servers. </summary>
    public static void DrawDiscordSplitButton(MessageService message, Vector2 size)
    {
        const string internationalAddress = @"https://discord.gg/kVva7DHV4r";
        const string chineseAddress       = @"https://discord.gg/QvrVye3";

        switch (ToggleButton.SplitButton((ImGuiId)6, new ToggleButton.SplitButtonData()
                {
                    Label      = "国际服"u8,
                    Active     = 0xFF5B5EFFu,
                    Background = DiscordColor,
                    Hovered    = ImGui.GetColorU32(ImGuiCol.ButtonHovered),
                    Tooltip =
                        "访问 https://discord.gg/kVva7DHV4r\n插件开发者的频道。\n请注意这里不对国服作支持，有问题推荐先去国服MOD频道咨询。"u8,
                }, new ToggleButton.SplitButtonData()
                {
                    Label      = "国服"u8,
                    Active     = 0xFF492C00u,
                    Hovered    = ImGui.GetColorU32(ImGuiCol.ButtonHovered),
                    Background = ImGui.GetColorU32(ImGuiCol.Button),
                    Tooltip =
                        "访问 https://discord.gg/QvrVye3\n国服模组社群。\n许多MOD都提供免费下载，\n请不要从倒卖者处花钱购买。"u8,
                }, size, MixColors(DiscordColor, ImGui.GetColorU32(ImGuiCol.Button))))
        {
            case 1:
                try
                {
                    var process = new ProcessStartInfo(internationalAddress)
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(process);
                }
                catch
                {
                    message.NotificationMessage($"Could not open international Discord link at {internationalAddress} in external browser", NotificationType.Error);
                }

                break;
            case 2:
                try
                {
                    var process = new ProcessStartInfo(chineseAddress)
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(process);
                }
                catch
                {
                    message.NotificationMessage($"Could not open Chinese Discord link at {chineseAddress} in external browser", NotificationType.Error);
                }

                break;
        }
    }

    /// <summary> Draw a single guide button. </summary>
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

    /// <summary> Draw a split button for guide and tutorial reset. </summary>
    public static void DrawGuideTutorialSplitButton(MessageService message, Vector2 size, Action resetTutorialAction)
    {
        const string guideAddress = @"https://reniguide.info/";

        switch (ToggleButton.SplitButton((ImGuiId)7, new ToggleButton.SplitButtonData()
                {
                    Label      = "新手指南"u8,
                    Active     = 0xFF5B5EFFu,
                    Background = ReniColorButton,
                    Hovered    = ImGui.GetColorU32(ImGuiCol.ButtonHovered),
                    Tooltip =
                        "访问https://reniguide.info/\nSerenity制作的基于图像和文本的指南，包含大部分Penumbra功能。\n不是插件作者官方指南，但一般不会过时。"u8,
                }, new ToggleButton.SplitButtonData()
                {
                    Label      = "重启教程"u8,
                    Active     = 0xFF492C00u,
                    Hovered    = ImGui.GetColorU32(ImGuiCol.ButtonHovered),
                    Background = ImGui.GetColorU32(ImGuiCol.Button),
                    Tooltip =
                        "重新启动Penumbra的教程系统，重置所有教程步骤。"u8,
                }, size, MixColors(ReniColorButton, ImGui.GetColorU32(ImGuiCol.Button))))
        {
            case 1:
                try
                {
                    var process = new ProcessStartInfo(guideAddress)
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(process);
                }
                catch
                {
                    message.NotificationMessage($"Could not open guide link at {guideAddress} in external browser", NotificationType.Error);
                }

                break;
            case 2:
                resetTutorialAction();
                break;
        }
    }

    /// <summary> Draw a split button for mod sites: XIV Mod Archive and Heliosphere. </summary>
    public static void DrawModSitesSplitButton(MessageService message, Vector2 size)
    {
        const string xivmodUrl = "https://www.xivmodarchive.com/";
        const string heliosphereUrl = "https://heliosphere.app/";

        switch (ToggleButton.SplitButton((ImGuiId)8, new ToggleButton.SplitButtonData()
                {
                    Label      = "XMA"u8,
                    Active     = 0xFF5B5EFFu,
                    Background = XmaColor,
                    Hovered    = ImGui.GetColorU32(ImGuiCol.ButtonHovered),
                    Tooltip    = "访问XIV Mod Archive模组站点"u8,
                }, new ToggleButton.SplitButtonData()
                {
                    Label      = "HSphere"u8,
                    Active     = 0xFF23AEE2,
                    Background = ImGui.GetColorU32(ImGuiCol.Button),
                    Hovered    = ImGui.GetColorU32(ImGuiCol.ButtonHovered),
                    Tooltip    = "访问Heliosphere模组站点，支持通过Heliosphere插件一键安装"u8,
                }, size, MixColors(XmaColor, ImGui.GetColorU32(ImGuiCol.Button))))
        {
            case 1:
                try
                {
                    var process = new ProcessStartInfo(xivmodUrl)
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(process);
                }
                catch
                {
                    message.NotificationMessage($"无法在浏览器中打开 {xivmodUrl}", NotificationType.Error);
                }
                break;
            case 2:
                try
                {
                    var process = new ProcessStartInfo(heliosphereUrl)
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(process);
                }
                catch
                {
                    message.NotificationMessage($"无法在浏览器中打开 {heliosphereUrl}", NotificationType.Error);
                }
                break;
        }
    }
}
