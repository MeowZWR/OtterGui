using ImGuiNET;
using OtterGui.Extensions;
using OtterGui.Filesystem;
using OtterGui.Raii;
using OtterGui.Text;

namespace OtterGui.FileSystem.Selector;

public partial class FileSystemSelector<T, TStateStorage>
{
    // Add a right-click context menu item to folder context menus at the given priority.
    // Context menu items are sorted from top to bottom on priority, then subscription order.
    public void SubscribeRightClickFolder(Action<FileSystem<T>.Folder> action, int priority = 0)
        => AddPrioritizedDelegate(_rightClickOptionsFolder, action, priority);

    // Add a right-click context menu item to leaf context menus at the given priority.
    // Context menu items are sorted from top to bottom on priority, then subscription order.
    public void SubscribeRightClickLeaf(Action<FileSystem<T>.Leaf> action, int priority = 0)
        => AddPrioritizedDelegate(_rightClickOptionsLeaf, action, priority);

    // Add a right-click context menu item to the main context menu at the given priority.
    // Context menu items are sorted from top to bottom on priority, then subscription order.
    public void SubscribeRightClickMain(Action action, int priority = 0)
        => AddPrioritizedDelegate(_rightClickOptionsMain, action, priority);

    // Remove a right-click context menu item from the folder context menu by reference equality.
    public void UnsubscribeRightClickFolder(Action<FileSystem<T>.Folder> action)
        => RemovePrioritizedDelegate(_rightClickOptionsFolder, action);

    // Remove a right-click context menu item from the leaf context menu by reference equality.
    public void UnsubscribeRightClickLeaf(Action<FileSystem<T>.Leaf> action)
        => RemovePrioritizedDelegate(_rightClickOptionsLeaf, action);

    // Remove a right-click context menu item from the main context menu by reference equality.
    public void UnsubscribeRightClickMain(Action action)
        => RemovePrioritizedDelegate(_rightClickOptionsMain, action);

    // Draw all context menu items for folders.
    private void RightClickContext(FileSystem<T>.Folder folder)
    {
        using var _ = ImRaii.Popup(folder.Identifier.ToString());
        if (!_)
            return;

        foreach (var action in _rightClickOptionsFolder)
            action.Item1.Invoke(folder);
    }

    // Draw all context menu items for leaves.
    private void RightClickContext(FileSystem<T>.Leaf leaf)
    {
        using var _ = ImRaii.Popup(leaf.Identifier.ToString());
        if (!_)
            return;

        foreach (var action in _rightClickOptionsLeaf)
            action.Item1.Invoke(leaf);
    }

    // Draw all context menu items for the main context.
    private void RightClickMainContext()
    {
        foreach (var action in _rightClickOptionsMain)
            action.Item1.Invoke();
    }


    // Lists are sorted on priority, then subscription order.
    private readonly List<(Action<FileSystem<T>.Folder>, int)> _rightClickOptionsFolder = new(4);
    private readonly List<(Action<FileSystem<T>.Leaf>, int)>   _rightClickOptionsLeaf   = new(1);
    private readonly List<(Action, int)>                       _rightClickOptionsMain   = new(4);

    private void InitDefaultContext()
    {
        SubscribeRightClickFolder(ExpandAllDescendants,   100);
        SubscribeRightClickFolder(CollapseAllDescendants, 100);
        SubscribeRightClickFolder(SetLocked,              900);
        SubscribeRightClickFolder(DissolveFolder,         999);
        SubscribeRightClickFolder(RenameFolder,           1000);
        SubscribeRightClickLeaf(SetLocked,  900);
        SubscribeRightClickLeaf(RenameLeaf, 1000);
        SubscribeRightClickMain(ExpandAll,   1);
        SubscribeRightClickMain(CollapseAll, 1);
    }

    // Default entries for the folder context menu.
    // Protected so they can be removed by inheritors.
    protected void SetLocked(FileSystem<T>.IPath path)
    {
        if (ImUtf8.MenuItem(path.IsLocked ? "Unlock"u8 : "Lock"u8))
            FileSystem.ChangeLockState(path, !path.IsLocked);
        ImUtf8.HoverTooltip(
            "Locking an item prevents this item from being dragged to other positions. It does not prevent any other manipulations of the item."u8);
    }

    protected void DissolveFolder(FileSystem<T>.Folder folder)
    {
        if (ImGui.MenuItem("解除折叠组"))
            _fsActions.Enqueue(() => FileSystem.Merge(folder, folder.Parent));
        ImGuiUtil.HoverTooltip("如果可以，将此折叠组的所有文件移至上级折叠组。");
    }

    protected void ExpandAllDescendants(FileSystem<T>.Folder folder)
    {
        if (ImGui.MenuItem("展开所有下级折叠组"))
        {
            var idx = _currentIndex;
            _fsActions.Enqueue(() => ToggleDescendants(folder, idx, true));
        }

        ImGuiUtil.HoverTooltip("展开此折叠组及其包含的下级折叠组。");
    }

    protected void CollapseAllDescendants(FileSystem<T>.Folder folder)
    {
        if (ImGui.MenuItem("收起所有下级折叠组"))
        {
            var idx = _currentIndex;
            _fsActions.Enqueue(() => ToggleDescendants(folder, idx, false));
        }

        ImGuiUtil.HoverTooltip("收起此折叠组及其包含的下级折叠组。");
    }

    protected void RenameFolder(FileSystem<T>.Folder folder)
    {
        var currentPath = folder.FullName();
        if (ImGui.InputText("##Rename", ref currentPath, 256, ImGuiInputTextFlags.EnterReturnsTrue))
            _fsActions.Enqueue(() =>
            {
                FileSystem.RenameAndMove(folder, currentPath);
                _filterDirty |= ExpandAncestors(folder);
            });

        ImGuiUtil.HoverTooltip("在此输入完整路径来移动或重命名折叠组，如果达成条件，则创建折叠组。");
    }

    protected void SetQuickMove(FileSystem<T>.Folder folder, int which, string current, Action<string> onSelect)
    {
        if (ImGui.MenuItem($"设置为快速移动折叠组 #{which + 1}"))
            onSelect(folder.FullName());
        ImGuiUtil.HoverTooltip($"设置此折叠组为快速移动位置{(current.Length > 0 ? $"取代 {current} 。" : "。")}");
    }

    protected void ClearQuickMove(int which, string current, Action onSelect)
    {
        if (current.Length == 0)
            return;

        if (ImGui.MenuItem($"清除快速移动折叠组 #{which + 1}"))
            onSelect();
        ImGuiUtil.HoverTooltip($"清除当前分配的快速移动折叠组：{current}。");
    }

    protected void QuickMove(FileSystem<T>.Leaf leaf, params string[] folders)
    {
        var currentName = leaf.Name;
        var currentPath = leaf.FullName();
        foreach (var (folder, idx) in folders.WithIndex().Where(s => s.Value.Length > 0))
        {
            using var id         = ImRaii.PushId(idx);
            var       targetPath = $"{folder}/{currentName}";
            if (FileSystem.Equal(targetPath, currentPath))
                continue;

            if (ImGui.MenuItem($"移动到 {folder}"))
                _fsActions.Enqueue(() =>
                {
                    foreach (var path in _selectedPaths.OfType<FileSystem<T>.Leaf>())
                        FileSystem.RenameAndMove(path, $"{folder}/{path.Name}");
                    FileSystem.RenameAndMove(leaf, targetPath);
                    _filterDirty |= ExpandAncestors(leaf);
                });
        }

        ImGuiUtil.HoverTooltip("如果可能，将此模组移动到先前设置的快速移动折叠组。");
    }

    protected void RenameLeaf(FileSystem<T>.Leaf leaf)
    {
        var currentPath = leaf.FullName();
        if (ImGui.IsWindowAppearing())
            ImGui.SetKeyboardFocusHere(0);
        ImGui.TextUnformatted("重命名搜索路径或移动：");
        if (ImGui.InputText("##Rename", ref currentPath, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            _fsActions.Enqueue(() =>
            {
                FileSystem.RenameAndMove(leaf, currentPath);
                _filterDirty |= ExpandAncestors(leaf);
            });
            ImGui.CloseCurrentPopup();
        }

        ImGuiUtil.HoverTooltip(
            "在此输入完整路径来移动或重命名折叠组，如果达成条件，则创建折叠组。\n\n不会重命名实际数据！");
    }

    protected void ExpandAll()
    {
        if (ImGui.Selectable("展开全部折叠组"))
            _fsActions.Enqueue(() => ToggleDescendants(FileSystem.Root, -1, true));
    }

    protected void CollapseAll()
    {
        if (ImGui.Selectable("收起全部折叠组"))
            _fsActions.Enqueue(() =>
            {
                ToggleDescendants(FileSystem.Root, -1, false);
                AddDescendants(FileSystem.Root, -1);
            });
    }
}
