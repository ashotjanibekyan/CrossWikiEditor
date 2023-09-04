using System.Collections.Immutable;
using Newtonsoft.Json;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils;

/// <summary>
/// This class is mostly copied from WikiClientLibrary itself. Once this class is released and is available via Nuget,
/// We should get rid of this class.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class MagicWordInfo
{
    [JsonProperty] public string Name { get; private set; } = "";

    [JsonProperty] public IReadOnlyCollection<string> Aliases { get; private set; } = ImmutableList<string>.Empty;

    [JsonProperty("case-sensitive")] public bool CaseSensitive { get; private set; }

    public override string ToString()
    {
        return Aliases.Count == 0 ? Name : $"{Name} ({string.Join(',', Aliases)})";
    }
}