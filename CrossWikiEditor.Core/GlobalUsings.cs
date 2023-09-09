global using System.Collections.Concurrent;
global using System.Collections.Immutable;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;

global using System.ComponentModel;
global using System.Globalization;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Web;
global using System.Xml;
global using System.Xml.Serialization;

global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;
global using CommunityToolkit.Mvvm.Messaging;
global using CommunityToolkit.Mvvm.Messaging.Messages;

global using CrossWikiEditor.Core.ListProviders;
global using CrossWikiEditor.Core.ListProviders.BaseListProviders;
global using CrossWikiEditor.Core.Messages;
global using CrossWikiEditor.Core.Models;
global using CrossWikiEditor.Core.Repositories;
global using CrossWikiEditor.Core.Services;
global using CrossWikiEditor.Core.Services.WikiServices;
global using CrossWikiEditor.Core.Settings;
global using CrossWikiEditor.Core.Utils;
global using CrossWikiEditor.Core.ViewModels;
global using CrossWikiEditor.Core.ViewModels.ControlViewModels;
global using CrossWikiEditor.Core.ViewModels.MenuViewModels;
global using CrossWikiEditor.Core.ViewModels.ReportViewModels;
global using CrossWikiEditor.Core.WikiClientLibraryUtils;
global using CrossWikiEditor.Core.WikiClientLibraryUtils.Generators;

global using HtmlAgilityPack;
global using Serilog;

global using WikiClientLibrary;
global using WikiClientLibrary.Client;
global using WikiClientLibrary.Generators;
global using WikiClientLibrary.Generators.Primitive;
global using WikiClientLibrary.Infrastructures;
global using WikiClientLibrary.Pages;
global using WikiClientLibrary.Sites;