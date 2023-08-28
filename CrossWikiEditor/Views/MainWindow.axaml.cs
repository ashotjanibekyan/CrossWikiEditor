using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CrossWikiEditor.Models;
using CrossWikiEditor.Services.WikiServices;

namespace CrossWikiEditor.Views;

public sealed partial class MainWindow : Window
{
    private static string HtmlContnet = """
                                        <tr>
                                          <td colspan="2" class="diff-lineno">Տող  1.</td>
                                          <td colspan="2" class="diff-lineno">Տող  1.</td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker" data-marker="−"></td>
                                          <td class="diff-deletedline diff-side-deleted"><div>Lorem <del class="diffchange diffchange-inline">ipsum</del> dolor sit amet, consectetur <del class="diffchange diffchange-inline">adipiscing</del> elit. Sed mattis maximus eros, ut <del class="diffchange diffchange-inline">aliquet</del> ipsum. Donec dui nulla, tristique auctor pharetra a, egestas <del class="diffchange diffchange-inline">id</del> lacus. Aliquam maximus <del class="diffchange diffchange-inline">justo</del> ante, sed lobortis nulla pellentesque ac. Suspendisse id ornare nisl, vitae <del class="diffchange diffchange-inline">aliquam</del> ante. Donec pulvinar mi libero, luctus vulputate dui tincidunt at. Praesent elementum cursus nibh nec <del class="diffchange diffchange-inline">faucibus</del>. <del class="diffchange diffchange-inline">Aliquam</del> euismod luctus mi nec <del class="diffchange diffchange-inline">tincidunt</del>. Duis id odio ut leo <del class="diffchange diffchange-inline">scelerisque</del> semper id et arcu. Ut molestie sed enim et interdum. Integer <del class="diffchange diffchange-inline">pellentesque</del> feugiat sapien. Aliquam id rutrum velit. Sed posuere leo lectus, non finibus nisl pulvinar et.</div></td>
                                          <td class="diff-marker" data-marker="+"></td>
                                          <td class="diff-addedline diff-side-added"><div>Lorem <ins class="diffchange diffchange-inline">ipswm</ins> dolor sit amet, consectetur <ins class="diffchange diffchange-inline">adipiscwng</ins> elit. Sed mattis maximus eros, ut <ins class="diffchange diffchange-inline">awiquet</ins> ipsum. Donec dui nulla, tristique auctor pharetra a, egestas <ins class="diffchange diffchange-inline">iw</ins> lacus. Aliquam maximus <ins class="diffchange diffchange-inline">jesto</ins> ante, sed lobortis nulla pellentesque ac. Suspendisse id ornare nisl, vitae <ins class="diffchange diffchange-inline">alifuam</ins> ante. Donec pulvinar mi libero, luctus vulputate dui tincidunt at. Praesent elementum cursus nibh nec <ins class="diffchange diffchange-inline">fafcibus</ins>. <ins class="diffchange diffchange-inline">Aliqwam</ins> euismod luctus mi nec <ins class="diffchange diffchange-inline">tinciwunt</ins>. Duis id odio ut leo <ins class="diffchange diffchange-inline">scewerisque</ins> semper id et arcu. Ut molestie sed enim et interdum. Integer <ins class="diffchange diffchange-inline">pellentewque</ins> feugiat sapien. Aliquam id rutrum velit. Sed posuere leo lectus, non finibus nisl pulvinar et.</div></td>
                                        </tr>
                                        <tr>
                                          <td colspan="2" class="diff-empty diff-side-deleted"></td>
                                          <td class="diff-marker" data-marker="+"></td>
                                          <td class="diff-addedline diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td colspan="2" class="diff-empty diff-side-deleted"></td>
                                          <td class="diff-marker"><a class="mw-diff-movedpara-right" title="Paragraph was moved. Click to jump to old location." href="#movedpara_5_0_lhs">&#x26AB;</a></td>
                                          <td class="diff-addedline diff-side-added"><div><a name="movedpara_1_1_rhs"></a>Fusce fermentum<ins class="diffchange diffchange-inline"> fewfwe</ins> pulvinar nunc, eu accumsan orci hendrerit in. Quisque consequat elit eu neque dignissim, vel molestie neque euismod. Praesent odio libero, euismod<ins class="diffchange diffchange-inline">  fwefwe</ins> sit<ins class="diffchange diffchange-inline"> fwefew</ins> amet sem ac, molestie condimentum velit.<ins class="diffchange diffchange-inline"> fewfwe</ins> Aliquam sit amet tempus quam. Class aptent taciti sociosqu ad litora torquent per conubia nostra,<ins class="diffchange diffchange-inline"> fwefwe</ins> per inceptos himenaeos. Cras sapien tortor,<ins class="diffchange diffchange-inline"> wefwe</ins> interdum a tortor a, pharetra venenatis mi. Aliquam iaculis urna non mauris placerat feugiat.</div></td>
                                        </tr>
                                        <tr>
                                          <td colspan="2" class="diff-empty diff-side-deleted"></td>
                                          <td class="diff-marker" data-marker="+"></td>
                                          <td class="diff-addedline diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td colspan="2" class="diff-empty diff-side-deleted"></td>
                                          <td class="diff-marker" data-marker="+"></td>
                                          <td class="diff-addedline diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td colspan="2" class="diff-empty diff-side-deleted"></td>
                                          <td class="diff-marker" data-marker="+"></td>
                                          <td class="diff-addedline diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td colspan="2" class="diff-empty diff-side-deleted"></td>
                                          <td class="diff-marker" data-marker="+"></td>
                                          <td class="diff-addedline diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-deleted"><br /></td>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker" data-marker="−"></td>
                                          <td class="diff-deletedline diff-side-deleted"><div>Integer hendrerit blandit enim, et auctor erat euismod id. Duis velit sapien, luctus et suscipit porttitor, bibendum eu nisl. Donec in odio non neque condimentum maximus. Fusce vel diam leo. Praesent a scelerisque massa. Morbi dictum convallis dolor, vel ullamcorper lacus ornare ac. Phasellus condimentum in velit quis accumsan. Donec viverra massa vitae elit mollis vulputate. Proin porttitor pharetra arcu, et ultrices risus varius vitae. Vivamus vel sagittis tortor. Curabitur a scelerisque urna, non maximus erat. Vivamus lacinia tempor nulla sed tincidunt. Suspendisse vulputate eget diam eu consectetur. Integer vestibulum luctus nisi, eu pulvinar massa. Duis dignissim iaculis facilisis.</div></td>
                                          <td colspan="2" class="diff-empty diff-side-added"></td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-deleted"><br /></td>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker"><a class="mw-diff-movedpara-left" title="Paragraph was moved. Click to jump to new location." href="#movedpara_1_1_rhs">&#x26AB;</a></td>
                                          <td class="diff-deletedline diff-side-deleted"><div><a name="movedpara_5_0_lhs"></a>Fusce fermentum pulvinar nunc, eu accumsan orci hendrerit in. Quisque consequat elit eu neque dignissim, vel molestie neque euismod. Praesent odio libero, euismod sit amet sem ac, molestie condimentum velit. Aliquam sit amet tempus quam. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Cras sapien tortor, interdum a tortor a, pharetra venenatis mi. Aliquam iaculis urna non mauris placerat feugiat.</div></td>
                                          <td colspan="2" class="diff-empty diff-side-added"></td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-deleted"><br /></td>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-deleted"><div>Fusce cursus tristique ornare. Suspendisse potenti. Quisque pharetra sapien velit. In hac habitasse platea dictumst. Interdum et malesuada fames ac ante ipsum primis in faucibus. Nulla suscipit magna id odio blandit, sit amet mattis diam semper. Donec non erat sit amet elit lacinia congue a et erat. Donec sit amet placerat dolor. Nullam ac odio risus. Nulla auctor nunc finibus metus vehicula aliquet. Duis sit amet accumsan sapien, non sodales turpis. Curabitur quis nibh non lacus lobortis ultrices eu sed risus. Duis est ligula, viverra eu magna eu, gravida vestibulum mi.</div></td>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-added"><div>Fusce cursus tristique ornare. Suspendisse potenti. Quisque pharetra sapien velit. In hac habitasse platea dictumst. Interdum et malesuada fames ac ante ipsum primis in faucibus. Nulla suscipit magna id odio blandit, sit amet mattis diam semper. Donec non erat sit amet elit lacinia congue a et erat. Donec sit amet placerat dolor. Nullam ac odio risus. Nulla auctor nunc finibus metus vehicula aliquet. Duis sit amet accumsan sapien, non sodales turpis. Curabitur quis nibh non lacus lobortis ultrices eu sed risus. Duis est ligula, viverra eu magna eu, gravida vestibulum mi.</div></td>
                                        </tr>
                                        <tr>
                                          <td colspan="2" class="diff-lineno">Տող  11.</td>
                                          <td colspan="2" class="diff-lineno">Տող  15.</td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-deleted"><div>Ut eu blandit dolor, ac ullamcorper tortor. Duis justo libero, egestas vitae dignissim non, maximus at dui. Aliquam fringilla metus ac lorem maximus accumsan placerat a nisl. Suspendisse eu elit urna. Praesent efficitur pretium gravida. Duis vitae libero ac turpis efficitur finibus. Quisque lobortis erat ex, ut efficitur arcu vestibulum id. Quisque mollis leo sapien, in sagittis ante efficitur ac.</div></td>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-added"><div>Ut eu blandit dolor, ac ullamcorper tortor. Duis justo libero, egestas vitae dignissim non, maximus at dui. Aliquam fringilla metus ac lorem maximus accumsan placerat a nisl. Suspendisse eu elit urna. Praesent efficitur pretium gravida. Duis vitae libero ac turpis efficitur finibus. Quisque lobortis erat ex, ut efficitur arcu vestibulum id. Quisque mollis leo sapien, in sagittis ante efficitur ac.</div></td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-deleted"><br /></td>
                                          <td class="diff-marker"></td>
                                          <td class="diff-context diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td class="diff-marker" data-marker="−"></td>
                                          <td class="diff-deletedline diff-side-deleted"><div>Vestibulum rhoncus convallis sagittis. Ut scelerisque, ipsum in varius ornare, magna urna sodales lectus, accumsan tincidunt mi erat at orci. Etiam sed dolor a libero pulvinar tempor id id mauris. Curabitur in lorem commodo, semper dui suscipit,<del class="diffchange diffchange-inline"> hendrerit</del> lectus. Cras et arcu at quam ultricies fringilla vel sed erat. Donec vehicula<del class="diffchange diffchange-inline"> sapien</del> vel orci sollicitudin, eget placerat nulla aliquet. Phasellus eget ligula dignissim mauris faucibus elementum et non ipsum. Quisque euismod ante vel risus semper<del class="diffchange diffchange-inline"> facilisis</del>. In hac habitasse platea dictumst. Integer sed venenatis lectus. Morbi id nisl nec dui viverra sodales.</div></td>
                                          <td class="diff-marker" data-marker="+"></td>
                                          <td class="diff-addedline diff-side-added"><div>Vestibulum rhoncus convallis sagittis. Ut scelerisque, ipsum in varius ornare, magna urna sodales lectus, accumsan tincidunt mi erat at orci. Etiam sed dolor a libero pulvinar tempor id id mauris. Curabitur in lorem commodo, semper dui suscipit, lectus. Cras et arcu at quam ultricies fringilla vel sed erat. Donec vehicula vel orci sollicitudin, eget placerat nulla aliquet. Phasellus eget ligula dignissim mauris faucibus elementum et non ipsum. Quisque euismod ante vel risus semper. In hac habitasse platea dictumst. Integer sed venenatis lectus. Morbi id nisl nec dui viverra sodales.</div></td>
                                        </tr>
                                        <tr>
                                          <td colspan="2" class="diff-empty diff-side-deleted"></td>
                                          <td class="diff-marker" data-marker="+"></td>
                                          <td class="diff-addedline diff-side-added"><br /></td>
                                        </tr>
                                        <tr>
                                          <td colspan="2" class="diff-empty diff-side-deleted"></td>
                                          <td class="diff-marker" data-marker="+"></td>
                                          <td class="diff-addedline diff-side-added"><div>Suspendisse potenti. Vivamus mattis tempus elit, in aliquet purus facilisis sit amet. Donec a ullamcorper mi. Ut fringilla consequat risus in mollis. Nunc interdum gravida fringilla. Donec pharetra nec mi et consectetur. Aenean arcu augue, condimentum id augue eu, mollis placerat est.</div></td>
                                        </tr>

                                        """;
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var sv = new WikiCompareResultParserService();
        List<DiffRow> result = sv.ParseCompareHtml(HtmlContnet);

        MyGrid.Children.Clear();
        MyGrid.ColumnDefinitions = new ColumnDefinitions("30,*,30,*");
        MyGrid.RowDefinitions = new RowDefinitions(string.Join(',', result.Select(x => "49")));
        for (int i = 0; i < result.Count; i++)
        {
            var first = result[i].First.GetTextBlock();
            var second = result[i].Second.GetTextBlock();
            var third = result[i].Third.GetTextBlock();
            var forth = result[i].Forth.GetTextBlock();
            MyGrid.Children.Add(first);
            MyGrid.Children.Add(second);
            MyGrid.Children.Add(third);
            MyGrid.Children.Add(forth);

            Grid.SetRow(first, i);
            Grid.SetColumn(first, 0);
            if (result[i].First.ColSpan == 2)
            {
                Grid.SetColumnSpan(first, 2);
            }
            else
            {
                Grid.SetRow(second, i);
                Grid.SetColumn(second, 1);
            }
            if (result[i].Second.ColSpan == 2)
            {
                Grid.SetColumnSpan(second, 2);
            }
            else
            {
                Grid.SetRow(third, i);
                Grid.SetColumn(third, 2);
            }
            if (result[i].Third.ColSpan == 2)
            {
                Grid.SetColumnSpan(third, 2);
            }
            else
            {
                Grid.SetRow(forth, i);
                Grid.SetColumn(forth, 3);
            }
            if (result[i].Forth.ColSpan == 2)
            {
                Grid.SetColumnSpan(forth, 2);
            }
            
        }
        Console.WriteLine(23);
    }
}