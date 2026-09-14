using Autodesk.Revit.DB;

namespace Revit.Sugar;

/// <summary>
/// Extension methods for <see cref="BoundingBoxXYZ"/>.
/// </summary>
public static class BoundingBoxXyzExtensions
{
    private const double DefaultTolerance = 1e-9;

    extension(BoundingBoxXYZ boundingBox)
    {
        /// <summary>
        /// Determines whether the world-aligned bounds of two bounding boxes overlap.
        /// Suitable as a broad-phase check before an exact geometry intersection.
        /// </summary>
        /// <param name="other">The bounding box to test against.</param>
        /// <param name="tolerance">
        /// The allowed gap between the boxes that still counts as an overlap (e.g. touching
        /// boundaries). Defaults to <c>1e-9</c>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the two boxes' world-space extents intersect (within
        /// <paramref name="tolerance"/>); otherwise <see langword="false"/>.
        /// </returns>
        public bool Overlaps(BoundingBoxXYZ other, double tolerance = DefaultTolerance)
        {
            boundingBox = Guard.ThrowIfNull(boundingBox, nameof(boundingBox));
            other = Guard.ThrowIfNull(other, nameof(other));

            if (boundingBox.Transform.IsIdentity && other.Transform.IsIdentity)
            {
                return !(boundingBox.Max.X < other.Min.X - tolerance
                    || other.Max.X < boundingBox.Min.X - tolerance
                    || boundingBox.Max.Y < other.Min.Y - tolerance
                    || other.Max.Y < boundingBox.Min.Y - tolerance
                    || boundingBox.Max.Z < other.Min.Z - tolerance
                    || other.Max.Z < boundingBox.Min.Z - tolerance);
            }

            GetWorldBounds(boundingBox, out XYZ min, out XYZ max);
            GetWorldBounds(other, out XYZ otherMin, out XYZ otherMax);

            return max.X >= otherMin.X - tolerance && min.X <= otherMax.X + tolerance
                && max.Y >= otherMin.Y - tolerance && min.Y <= otherMax.Y + tolerance
                && max.Z >= otherMin.Z - tolerance && min.Z <= otherMax.Z + tolerance;
        }
    }

    /// <summary>
    /// Computes the axis-aligned world-space min/max corners of <paramref name="boundingBox"/>,
    /// accounting for its (possibly non-identity) <see cref="BoundingBoxXYZ.Transform"/>
    /// by transforming all 8 local corners and taking their bounds.
    /// </summary>
    /// <param name="boundingBox">The bounding box whose world bounds are computed.</param>
    /// <param name="min">The resulting world-space minimum corner.</param>
    /// <param name="max">The resulting world-space maximum corner.</param>
    private static void GetWorldBounds(BoundingBoxXYZ boundingBox, out XYZ min, out XYZ max)
    {
        if (boundingBox.Transform.IsIdentity)
        {
            min = boundingBox.Min;
            max = boundingBox.Max;
            return;
        }

        XYZ localMin = boundingBox.Min;
        XYZ localMax = boundingBox.Max;
        Transform transform = boundingBox.Transform;
        double minX = double.PositiveInfinity;
        double minY = double.PositiveInfinity;
        double minZ = double.PositiveInfinity;
        double maxX = double.NegativeInfinity;
        double maxY = double.NegativeInfinity;
        double maxZ = double.NegativeInfinity;

        for (int index = 0; index < 8; index++)
        {
            var vertex = new XYZ(
                (index & 1) == 0 ? localMin.X : localMax.X,
                (index & 2) == 0 ? localMin.Y : localMax.Y,
                (index & 4) == 0 ? localMin.Z : localMax.Z);
            XYZ worldVertex = transform.OfPoint(vertex);

            minX = Math.Min(minX, worldVertex.X);
            minY = Math.Min(minY, worldVertex.Y);
            minZ = Math.Min(minZ, worldVertex.Z);
            maxX = Math.Max(maxX, worldVertex.X);
            maxY = Math.Max(maxY, worldVertex.Y);
            maxZ = Math.Max(maxZ, worldVertex.Z);
        }

        min = new XYZ(minX, minY, minZ);
        max = new XYZ(maxX, maxY, maxZ);
    }
}
