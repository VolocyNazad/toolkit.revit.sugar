using Autodesk.Revit.DB;

namespace Revit.Sugar;

/// <summary>
/// Extension members for <see cref="Autodesk.Revit.DB.ElementId"/> that hide the difference
/// between <see cref="Autodesk.Revit.DB.ElementId.IntegerValue"/> (Revit versions before 2024)
/// and <c>ElementId.Value</c> (Revit 2024+).
/// </summary>
public static class ElementIdExtensions
{
    extension(ElementId id)
    {
        /// <summary>
        /// Determines whether this <see cref="Autodesk.Revit.DB.ElementId"/> identifies the
        /// given built-in category, independent of the Revit version's underlying id
        /// representation.
        /// </summary>
        /// <param name="category">The built-in category to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="id"/> is the id of <paramref name="category"/>.</returns>
        public bool IsCategory(BuiltInCategory category)
        {
#if BEFORE2024
            return id.IntegerValue == (int)category;
#else
            return id.Value == (long)category;
#endif
        }

        /// <summary>
        /// Reinterprets this <see cref="Autodesk.Revit.DB.ElementId"/> as a
        /// <see cref="Autodesk.Revit.DB.BuiltInCategory"/>, independent of the Revit version's
        /// underlying id representation.
        /// </summary>
        /// <returns>The <see cref="Autodesk.Revit.DB.BuiltInCategory"/> corresponding to this id.</returns>
        public BuiltInCategory ToBuiltInCategory()
        {
#if BEFORE2024
            return (BuiltInCategory)id.IntegerValue;
#else
            return (BuiltInCategory)id.Value;
#endif
        }

#if BEFORE2024
        /// <summary>
        /// Gets the numeric value of this id, as an <see langword="int"/> on Revit versions
        /// before 2024 (backed by <see cref="Autodesk.Revit.DB.ElementId.IntegerValue"/>).
        /// </summary>
        /// <returns>The numeric value of this id.</returns>
        public int Value()
        {
            return id.IntegerValue;
        }
#else
        /// <summary>
        /// Gets the numeric value of this id, as a <see langword="long"/> on Revit 2024+
        /// (backed by <c>ElementId.Value</c>).
        /// </summary>
        /// <returns>The numeric value of this id.</returns>
        public long Value()
        {
            return id.Value;
        }
#endif
    }
}
