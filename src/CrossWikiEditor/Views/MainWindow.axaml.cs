using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Platform;
using CrossWikiEditor.Core;
using Newtonsoft.Json.Linq;
using TheArtOfDev.HtmlRenderer.Avalonia;
using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace CrossWikiEditor.Views;

public sealed partial class MainWindow : Window, IOwner
{
    public MainWindow()
    {
        InitializeComponent();

        _htmlPanel.StylesheetLoad += _htmlPanel_StylesheetLoad;
        string url =
            "https://hy.wikipedia.org/w/api.php?action=compare&format=json&fromrev=9956865&torev=9955494&toslots=&prop=diff&difftype=table&formatversion=2";
        string jsonResult = GetJsonFromUrl(url);
        var parsed = JObject.Parse(jsonResult);
        JToken? result = parsed["compare"]?["body"];
        _htmlPanel.Text = $"""
                           <html>
                               <head>
                                   <link rel="Stylesheet" href="StyleSheet" />
                               </head>
                               <body>
                                   <table>
                                       {result}
                                   </table>
                               </body>
                           </html>
                           """;
    }

    private void _htmlPanel_StylesheetLoad(object? sender, HtmlRendererRoutedEventArgs<HtmlStylesheetLoadEventArgs> e)
    {
        using Stream? resource = AssetLoader.Open(new Uri(@"avares://CrossWikiEditor/Assets/MwDiff.css"));
        using var reader = new StreamReader(resource);

        string? contents = reader.ReadToEnd();
        e.Event.SetStyleSheet = contents;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
    }

    private static string GetJsonFromUrl(string url)
    {
        try
        {
            using var client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}