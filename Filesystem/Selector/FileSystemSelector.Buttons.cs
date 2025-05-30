using Dalamud.Interface;
using ImGuiNET;
using OtterGui.Classes;
using OtterGui.Filesystem;
using OtterGui.Raii;

namespace OtterGui.FileSystem.Selector;

public partial class FileSystemSelector<T, TStateStorage>
{
    // Add a button to the bottom-list. Should be an object that does not exceed the size parameter.
    // Buttons are sorted from left to right on priority, then subscription order.
    public void AddButton(Action<Vector2> action, int priority)
        => AddPrioritizedDelegate(_buttons, action, priority);

    // Remove a button from the bottom-list by reference equality.
    public void RemoveButton(Action<Vector2> action)
        => RemovePrioritizedDelegate(_buttons, action);

    // List sorted on priority, then subscription order.
    private readonly List<(Action<Vector2>, int)> _buttons = new(1);

    /// <summary> Number of buttons to draw. </summary>
    public int ButtonCount
        => _buttons.Count;

    // Draw all subscribed buttons.
    private void DrawButtons()
    {
        var buttonWidth = new Vector2(_currentWidth / Math.Max(_buttons.Count, 1), ImGui.GetFrameHeight());
        using var style = ImRaii.PushStyle(ImGuiStyleVar.FrameRounding, 0f)
            .Push(ImGuiStyleVar.ItemSpacing, Vector2.Zero);

        foreach (var button in _buttons)
        {
            button.Item1.Invoke(buttonWidth);
            ImGui.SameLine();
        }

        ImGui.NewLine();
    }

    // Draw necessary popups from buttons outside of pushed styles.
    protected virtual void DrawPopups()
    { }

    // Protected so it can be removed.
    protected void FolderAddButton(Vector2 size)
    {
        const string newFolderName = "folderName";

        if (ImGuiUtil.DrawDisabledButton(FontAwesomeIcon.FolderPlus.ToIconString(), size,
                "新建一个空折叠组，名称中可以包含反斜杠'/'来创建目录结构。", false, true))
            ImGui.OpenPopup(newFolderName);

        // Does not need to be delayed since it is not in the iteration itself.
        FileSystem<T>.Folder? folder = null;
        if (ImGuiUtil.OpenNameField(newFolderName, ref _newName) && _newName.Length > 0)
            try
            {
                folder   = FileSystem.FindOrCreateAllFolders(_newName);
                _newName = string.Empty;
            }
            catch
            {
                // Ignored
            }

        if (folder != null)
            _filterDirty |= ExpandAncestors(folder);
    }

    protected void DeleteSelectionButton(Vector2 size, DoubleModifier modifier, string singular, string plural, Action<T> delete)
    {
        var keys        = modifier.IsActive();
        var anySelected = _selectedPaths.Count > 1 || SelectedLeaf != null;
        var name        = _selectedPaths.Count > 1 ? plural : singular;
        var tt = !anySelected
            ? $"没有选中{plural}。"
            : $"从驱动器删除选中的{name}。\n"
          + "注意，无法恢复。";
        if (!keys)
            tt += $"\n按住{modifier}并点击来删除{name}。";

        if (ImGuiUtil.DrawDisabledButton(FontAwesomeIcon.Trash.ToIconString(), size, tt, !anySelected || !keys, true))
        {
            if (Selected != null)
                delete(Selected);
            else
                foreach (var leaf in _selectedPaths.OfType<FileSystem<T>.Leaf>())
                    delete(leaf.Value);
        }
    }

    private void InitDefaultButtons()
    {
        AddButton(FolderAddButton, 50);
    }
}
