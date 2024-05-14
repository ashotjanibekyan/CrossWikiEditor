namespace CrossWikiEditor.Core.Settings;

/// <summary>
/// Corresponds to the <see cref="OptionsViewModel"/>
/// </summary>
public sealed class GeneralOptions
{
    public bool AutoTag { get; set; }
    public bool ApplyGeneralFixes { get; set; }
    public bool UnicodifyWholePage { get; set; }
    public bool FindAndReplace { get; set; }
    public bool SkipIfNoReplacement { get; set; }
    public bool SkipIfOnlyMinorReplacementMade { get; set; }
    public bool RegexTypoFixing { get; set; }
    public bool SkipIfNoTypoFixed { get; set; }
    public NormalFindAndReplaceRules NormalFindAndReplaceRules { get; set; } = [];
}
