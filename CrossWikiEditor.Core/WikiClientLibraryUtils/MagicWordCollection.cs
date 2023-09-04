using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WikiClientLibrary.Infrastructures;

namespace CrossWikiEditor.Core.WikiClientLibraryUtils;

/// <summary>
/// This class is mostly copied from WikiClientLibrary itself. Once this class is released and is available via Nuget,
/// We should get rid of this class.
/// </summary>
public class MagicWordCollection : ReadOnlyCollection<MagicWordInfo>
{
    private static readonly JsonSerializer WikiJsonSerializer = CreateWikiJsonSerializer();

    private static JsonSerializer CreateWikiJsonSerializer()
    {
        var settings =
            new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new WikiJsonContractResolver(),
                Formatting = Formatting.None,
                // https://github.com/JamesNK/Newtonsoft.Json/issues/862
                // https://github.com/CXuesong/WikiClientLibrary/issues/49
                DateParseHandling = DateParseHandling.None,
                Converters =
                {
                    new WikiBooleanJsonConverter(),
                    new WikiStringEnumJsonConverter(),
                    new WikiDateTimeJsonConverter()
                }
            };
        return JsonSerializer.CreateDefault(settings);
    }

    private readonly ILookup<string, MagicWordInfo> magicWordLookup;
    private readonly ILookup<string, MagicWordInfo> magicWordAliasLookup;

    internal MagicWordCollection(JArray jMagicWords)
        : base(jMagicWords.ToObject<IList<MagicWordInfo>>(WikiJsonSerializer))
    {
        magicWordLookup = Items.ToLookup(i => i.Name);
        magicWordAliasLookup = Items
            .SelectMany(i => i.Aliases.Select(a => (Alias: a, Item: i)))
            .ToLookup(p => p.Alias, p => p.Item, StringComparer.InvariantCultureIgnoreCase);
    }

    public MagicWordInfo this[string name]
    {
        get
        {
            MagicWordInfo? match = TryGet(name);
            if (match == null)
            {
                throw new KeyNotFoundException();
            }

            return match;
        }
    }

    public bool ContainsName(string name)
    {
        return TryGet(name) != null;
    }

    public bool ContainsAlias(string alias)
    {
        return TryGetByAlias(alias) != null;
    }

    private MagicWordInfo? TryGet(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        return magicWordLookup[name].FirstOrDefault();
    }

    private MagicWordInfo? TryGetByAlias(string alias)
    {
        if (alias == null)
        {
            throw new ArgumentNullException(nameof(alias));
        }

        return magicWordAliasLookup[alias].FirstOrDefault(i => i.Aliases.Any(a =>
            string.Equals(a, alias, i.CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase)
        ));
    }
}