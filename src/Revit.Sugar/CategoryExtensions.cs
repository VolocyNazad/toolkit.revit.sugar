#if BEFORE2024
using Autodesk.Revit.DB;

namespace Revit.Sugar;

/// <summary>
/// Extension members for <see cref="Autodesk.Revit.DB.Category"/>, for Revit versions before
/// 2024 where <see cref="Autodesk.Revit.DB.Category"/> has no built-in <c>BuiltInCategory</c>
/// accessor of its own.
/// </summary>
public static class CategoryExtensions
{
    extension(Category category)
    {
        /// <summary>
        /// Gets the <see cref="Autodesk.Revit.DB.BuiltInCategory"/> corresponding to this
        /// category's <see cref="Autodesk.Revit.DB.Category.Id"/>.
        /// </summary>
        public BuiltInCategory BuiltInCategory => (BuiltInCategory)category.Id.IntegerValue;
    }
}
#endif
