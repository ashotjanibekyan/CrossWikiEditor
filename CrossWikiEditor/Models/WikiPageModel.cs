using System;
using WikiClientLibrary.Pages;

namespace CrossWikiEditor.Models;

public class WikiPageModel : IEquatable<WikiPageModel>, IComparable<WikiPageModel>
{
    public WikiPage? WikiPage { get; set; }
    public string Title { get; init; }
    public int NamespaceId { get; init; }

    public WikiPageModel(WikiPage wikiPage) : this(wikiPage.Title, wikiPage.NamespaceId)
    {
        WikiPage = wikiPage;
    }

    public WikiPageModel(string title, int namespaceId)
    {
        Title = title;
        NamespaceId = namespaceId;
    }

    public void Deconstruct(out string title, out int namespaceId)
    {
        title = Title;
        namespaceId = NamespaceId;
    }

    public bool Equals(WikiPageModel? other)
    {
        return this == other;
    }

    public override bool Equals(object? obj)
    {
        if (obj is WikiPageModel wikiPageModel)
        {
            return this == wikiPageModel;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Title);
    }

    public static bool operator ==(WikiPageModel? left, WikiPageModel? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Title == right.Title;
    }

    public static bool operator !=(WikiPageModel? left, WikiPageModel? right)
    {
        return !(left == right);
    }

    public int CompareTo(WikiPageModel? other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (ReferenceEquals(null, other))
        {
            return 1;
        }

        return string.Compare(Title, other.Title, StringComparison.Ordinal);
    }
}