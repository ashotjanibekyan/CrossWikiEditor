using CrossWikiEditor.Models;
using CrossWikiEditor.ViewModels;

namespace CrossWikiEditor.Tests.ViewModels;

public class WhatLinksHereOptionsViewModelTests : BaseTest
{
    [SetUp]
    public void SetUp()
    {
        SetUpServices();
    }

    [TestCase(true)]
    [TestCase(false)]
    public void ChangingIsAllNamespacesChecked_SetAllValues(bool newValue)
    {
        // arrange
        List<WikiNamespace>? namespaces = Fakers.WikiNamespaceFaker.Generate(20);
        var sut = new WhatLinksHereOptionsViewModel(namespaces);
        foreach (WikiNamespace ns in sut.Namespaces)
        {
            ns.IsChecked = !newValue;
        }

        sut.IsAllNamespacesChecked = !newValue;

        // act
        sut.IsAllNamespacesChecked = newValue;

        // assert
        Assert.That(sut.Namespaces.Where(n => n.IsChecked != newValue), Is.Empty);
    }

    [Test]
    public void OkCommand_ShouldCloseWithCorrectValues()
    {
        // arrange
        List<WikiNamespace>? namespaces = Fakers.WikiNamespaceFaker.Generate(20);
        var sut = new WhatLinksHereOptionsViewModel(namespaces);
        sut.Namespaces[2].IsChecked = true;
        sut.Namespaces[7].IsChecked = true;
        sut.IncludeRedirects = true;
        sut.SelectedRedirectFilter = 2;

        // act
        sut.OkCommand.Execute(_dialog);

        // assert
        _dialog.Received(1).Close(Arg.Is<WhatLinksHereOptions>(options =>
            options.IncludeRedirects == sut.IncludeRedirects
            && (int) options.RedirectFilter == sut.SelectedRedirectFilter
            && options.Namespaces.OrderBy(e => e).SequenceEqual(sut.Namespaces.Where(x => x.IsChecked).Select(n => n.Id).OrderBy(e => e))));
    }
}