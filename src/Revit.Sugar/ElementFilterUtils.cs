using Autodesk.Revit.DB;

namespace Revit.Sugar;

/// <summary>
/// Ready-made factories for common <see cref="Autodesk.Revit.DB.ElementFilter"/> shapes that
/// Revit's API doesn't expose directly.
/// </summary>
public static class ElementFilterUtils
{
    /// <summary>
    /// Creates a filter that matches every element (both element instances and element types).
    /// </summary>
    /// <returns>An <see cref="Autodesk.Revit.DB.ElementFilter"/> that passes any element.</returns>
    public static ElementFilter AllFilter() =>
        new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false));

    /// <summary>
    /// Creates a filter that matches no element at all (the logical opposite of
    /// <see cref="AllFilter"/>). Useful as a safe default/placeholder filter.
    /// </summary>
    /// <returns>An <see cref="Autodesk.Revit.DB.ElementFilter"/> that never passes any element.</returns>
    public static ElementFilter EmptyFilter() =>
        new LogicalAndFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false));

    /// <summary>
    /// Creates a filter that matches elements belonging to categories of the given
    /// <paramref name="categoryType"/> (e.g. only "Model" categories).
    /// </summary>
    /// <param name="doc">The document whose categories are inspected.</param>
    /// <param name="categoryType">The category type to filter by. Defaults to <see cref="Autodesk.Revit.DB.CategoryType.Model"/>.</param>
    /// <param name="inverse">
    /// When <see langword="true"/>, the filter matches elements whose category is
    /// <b>not</b> of <paramref name="categoryType"/> instead.
    /// </param>
    /// <returns>An <see cref="Autodesk.Revit.DB.ElementMulticategoryFilter"/> built from the matching categories.</returns>
    public static ElementFilter CategoryTypeFilter(Document doc, CategoryType categoryType = CategoryType.Model, bool inverse = false)
    {
        doc = Guard.ThrowIfNull(doc, nameof(doc));

        Categories categories = doc.Settings.Categories;
        ICollection<ElementId> ids = new List<ElementId>(categories.Size);
        foreach (Category category in categories)
        {
            if (category.CategoryType == categoryType) ids.Add(category.Id);
        }

        return new ElementMulticategoryFilter(ids, inverse);
    }
}
