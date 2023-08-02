using System.Collections.Generic;
using System.Linq;

namespace CrossWikiEditor.ViewModels.ControlViewModels;

public class SkipViewModel : ViewModelBase
{

    private static void LinqTests<T>(T myArgs) where T : MySpecialT
    {
        myArgs.Specials.Select(e => e.StartsWith('3')).Where(e => e != null);
    }
}

class A : MySpecialT
{
    public List<string> Specials => new List<string> { "324", "324" };
}

class B : MySpecialT
{
    public List<string> Specials => new List<string> { "fwefw", "gwerer" };
}

interface MySpecialT
{
    List<string> Specials { get; }
}