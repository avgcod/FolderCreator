using CsvHelper.Configuration;

namespace Folder_Creator.Types;

public sealed record FolderNumberHolder(string FolderNumber);

public sealed class FolderNumberHolderMap : ClassMap<FolderNumberHolder> {
    public FolderNumberHolderMap() => Parameter("FolderNumber").Name("Base Document Reference");
}