using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWikiEditor.ViewModels;
public interface IDialogViewModel<TResult>
{
    TResult Result { get; set; }
}
