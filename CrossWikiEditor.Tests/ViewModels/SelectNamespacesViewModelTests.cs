using CrossWikiEditor.Core;
using CrossWikiEditor.Core.Models;
using CrossWikiEditor.Core.ViewModels;

namespace CrossWikiEditor.Tests.ViewModels;

public sealed class SelectNamespacesViewModelTests
{
    private SelectNamespacesViewModel _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new SelectNamespacesViewModel(new List<WikiNamespace>
        {
            new(0, "namespace name with id 0"),
            new(1, "namespace name with id 1"),
            new(2, "namespace name with id 2"),
            new(3, "namespace name with id 3"),
            new(4, "namespace name with id 4"),
            new(5, "namespace name with id 5"),
            new(6, "namespace name with id 6"),
            new(7, "namespace name with id 7"),
        });
    }

    [Test]
    public void SelectCommand_ShouldCloseTheDialogWithSelectedNamespaceIds()
    {
        // arrange
        _sut.Namespaces[0].IsChecked = true;
        _sut.Namespaces[1].IsChecked = false;
        _sut.Namespaces[2].IsChecked = true;
        _sut.Namespaces[3].IsChecked = false;
        _sut.Namespaces[4].IsChecked = false;
        _sut.Namespaces[5].IsChecked = true;
        _sut.Namespaces[6].IsChecked = false;
        _sut.Namespaces[7].IsChecked = true;
        IDialog dialog = Substitute.For<IDialog>();
        int[] expectedResult = _sut.Namespaces.Where(x => x.IsChecked).Select(x => x.Id).ToArray();

        // act
        _sut.SelectCommand.Execute(dialog);
        
        // assert
        dialog.Received(1).Close(Arg.Is<int[]>(x => x.SequenceEqual(expectedResult)));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void IsAllSelected_ShouldSetAllIsCheckedValues(bool value)
    {
        // arrange
        _sut.Namespaces[0].IsChecked = true;
        _sut.Namespaces[1].IsChecked = false;
        _sut.Namespaces[2].IsChecked = true;
        _sut.Namespaces[3].IsChecked = false;
        _sut.Namespaces[4].IsChecked = false;
        _sut.Namespaces[5].IsChecked = true;
        _sut.Namespaces[6].IsChecked = false;
        _sut.Namespaces[7].IsChecked = true;
        _sut.IsAllSelected = !value;

        // act
        _sut.IsAllSelected = value;

        // assert
        _sut.Namespaces.All(x => x.IsChecked == value).Should().BeTrue();
    }
}