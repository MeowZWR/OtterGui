namespace OtterGui.Filesystem;

public enum SortMode
{
    FoldersFirst,
    Lexicographical,
    InverseFoldersFirst,
    InverseLexicographical,
    FoldersLast,
    InverseFoldersLast,
    InternalOrder,
    InverseInternalOrder,
}

public interface ISortMode<T> where T : class
{
    string Name        { get; }
    string Description { get; }

    IEnumerable<FileSystem<T>.IPath> GetChildren(FileSystem<T>.Folder folder);

    public static readonly ISortMode<T> FoldersFirst           = new FoldersFirstT();
    public static readonly ISortMode<T> Lexicographical        = new LexicographicalT();
    public static readonly ISortMode<T> InverseFoldersFirst    = new InverseFoldersFirstT();
    public static readonly ISortMode<T> InverseLexicographical = new InverseLexicographicalT();
    public static readonly ISortMode<T> FoldersLast            = new FoldersLastT();
    public static readonly ISortMode<T> InverseFoldersLast     = new InverseFoldersLastT();
    public static readonly ISortMode<T> InternalOrder          = new InternalOrderT();
    public static readonly ISortMode<T> InverseInternalOrder   = new InverseInternalOrderT();

    private struct FoldersFirstT : ISortMode<T>
    {
        public string Name
            => "折叠组优先";

        public string Description
            => "按字典顺序，优先为折叠组排序，然后排序剩下的。";

        public IEnumerable<FileSystem<T>.IPath> GetChildren(FileSystem<T>.Folder folder)
            => folder.GetSubFolders().Cast<FileSystem<T>.IPath>().Concat(folder.GetLeaves());
    }

    private struct LexicographicalT : ISortMode<T>
    {
        public string Name
            => "字典顺序";

        public string Description
            => "统一按字典顺序排序。";

        public IEnumerable<FileSystem<T>.IPath> GetChildren(FileSystem<T>.Folder folder)
            => folder.Children;
    }

    private struct InverseFoldersFirstT : ISortMode<T>
    {
        public string Name
            => "折叠组优先(反向)";

        public string Description
            => "按字典顺序反向排序，先排序折叠组，然后排序剩下的。";

        public IEnumerable<FileSystem<T>.IPath> GetChildren(FileSystem<T>.Folder folder)
            => folder.GetSubFolders().Cast<FileSystem<T>.IPath>().Reverse().Concat(folder.GetLeaves().Reverse());
    }

    public struct InverseLexicographicalT : ISortMode<T>
    {
        public string Name
            => "字典顺序(反向)";

        public string Description
            => "统一按字典顺序反向排序。";

        public IEnumerable<FileSystem<T>.IPath> GetChildren(FileSystem<T>.Folder folder)
            => folder.Children.Cast<FileSystem<T>.IPath>().Reverse();
    }

    public struct FoldersLastT : ISortMode<T>
    {
        public string Name
            => "折叠组最后";

        public string Description
            => "按字典顺序，优先为子文件排序，然后才是折叠组。";

        public IEnumerable<FileSystem<T>.IPath> GetChildren(FileSystem<T>.Folder folder)
            => folder.GetLeaves().Cast<FileSystem<T>.IPath>().Concat(folder.GetSubFolders());
    }

    public struct InverseFoldersLastT : ISortMode<T>
    {
        public string Name
            => "折叠组最后(反向)";

        public string Description
            => "按字典顺序反向排序，优先为子文件排序，然后才是折叠组。";

        public IEnumerable<FileSystem<T>.IPath> GetChildren(FileSystem<T>.Folder folder)
            => folder.GetLeaves().Cast<FileSystem<T>.IPath>().Reverse().Concat(folder.GetSubFolders().Reverse());
    }

    public struct InternalOrderT : ISortMode<T>
    {
        public string Name
            => "内部顺序";

        public string Description
            => "每个折叠组下，其子项按标识符进行排序（即，按其在文件系统中创建的顺序排序）。";

        public IEnumerable<FileSystem<T>.IPath> GetChildren(FileSystem<T>.Folder folder)
            => folder.Children.OrderBy(c => c.Identifier);
    }

    public struct InverseInternalOrderT : ISortMode<T>
    {
        public string Name
            => "内部顺序(反向)";

        public string Description
            => "每个折叠组下，其子项按标识符反向进行排序（即，按其在文件系统中创建的顺序反向排序）。";

        public IEnumerable<FileSystem<T>.IPath> GetChildren(FileSystem<T>.Folder folder)
            => folder.Children.OrderByDescending(c => c.Identifier);
    }
}
