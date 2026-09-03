using Autodesk.Revit.DB;

namespace Revit.Sugar;

/// <summary>
/// Extension members for <see cref="Autodesk.Revit.DB.FilteredElementCollector"/> that add
/// generic, type-safe shortcuts over the string/<see cref="Type"/>-based collector API.
/// </summary>
public static class FilteredElementCollectorExtensions
{
    extension(FilteredElementCollector collector)
    {
        /// <summary>
        /// Restricts the collector to elements whose runtime type is <typeparamref name="T"/>.
        /// Equivalent to <see cref="Autodesk.Revit.DB.FilteredElementCollector.OfClass(Type)"/>
        /// with <c>typeof(T)</c>.
        /// </summary>
        /// <typeparam name="T">The Revit element type to filter by.</typeparam>
        /// <returns>The same collector, for chaining.</returns>
        public FilteredElementCollector OfClass<T>() where T : Element
            => collector.OfClass(typeof(T));

        /// <summary>
        /// Applies native class and element/type filters for the requested Revit element type.
        /// </summary>
        /// <typeparam name="T">
        /// The Revit element type to filter by. If <typeparamref name="T"/> derives from
        /// <see cref="Autodesk.Revit.DB.ElementType"/>, the collector is restricted to element
        /// types (via <see cref="Autodesk.Revit.DB.FilteredElementCollector.WhereElementIsElementType"/>);
        /// otherwise it is restricted to element instances (via
        /// <see cref="Autodesk.Revit.DB.FilteredElementCollector.WhereElementIsNotElementType"/>).
        /// </typeparam>
        /// <returns>The same collector, for chaining.</returns>
        public FilteredElementCollector WhereElementIs<T>() where T : Element
        {
            collector.OfClass<T>();

            return typeof(ElementType).IsAssignableFrom(typeof(T))
                ? collector.WhereElementIsElementType()
                : collector.WhereElementIsNotElementType();
        }
    }
}
