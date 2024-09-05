using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Clipper2Lib;
using Clipper2 = Clipper2Lib.Clipper;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;

namespace OnlyInvalid.Polygon.Clipper_API
{
    /// <summary>
    /// This class is designed to integrate the clipper library with Unity data types.
    /// </summary>
    public static class Clipper
    {
        #region Intersection
        /// <summary>
        /// Applys a square grid intersection to a polygon
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="dimensions"></param>
        /// <param name="scaler"></param>
        /// <returns></returns>
        public static IList<IList<Vector3>> Split(this IEnumerable<Vector3> polygon, Vector2Int dimensions, Vector2 scale)
        {
            IList<Vector3[]> grid = PolygonMaker.Grid(dimensions);

            foreach (var square in grid)
            {
                Vector3 pos = square.Centroid();

                Matrix4x4 position = Matrix4x4.Translate(pos) * Matrix4x4.Scale(scale) * Matrix4x4.Translate(-pos);

                for (int i = 0; i < square.Length; i++)
                {
                    square[i] = position.MultiplyPoint3x4(square[i]);
                }
            }

            IList<IList<Vector3>> clips = grid.Select(x => (IList<Vector3>)x.ToList()).ToList();

            return Intersect(polygon, clips);
        }

        public static IList<IList<Vector3>> Intersect(IEnumerable<Vector3> subject, IList<IList<Vector3>> clips, int precision = 7)
        {
            return Clipper2.Intersect(subject.ToPathsD(), clips.ToPathsD(), FillRule.NonZero, precision).ToPolygons();
        }
        public static IList<Vector3> Intersect(IEnumerable<Vector3> subject, IEnumerable<Vector3> clip, int precision = 7)
        {
            return Clipper2.Intersect(subject.ToPathsD(), clip.ToPathsD(), FillRule.NonZero, precision).ToPolygon();
        }
        #endregion

        #region Union
        public static IList<Vector3> Union(IEnumerable<Vector3> subject, IEnumerable<Vector3> clip, int precision = 7)
        {
            return Clipper2.Union(subject.ToPathsD(), clip.ToPathsD(), FillRule.NonZero, precision).ToPolygon();
        }
        public static IList<IList<Vector3>> Union(IEnumerable<Vector3> subject, IList<IList<Vector3>> clips, int precision = 7)
        {
            return Clipper2.Union(subject.ToPathsD(), clips.ToPathsD(), FillRule.NonZero, precision).ToPolygons();
        }
        #endregion

        #region Difference
        public static IList<Vector3> Difference(IEnumerable<Vector3> subject, IEnumerable<Vector3> clip, int precision = 7)
        {
            return Clipper2.Difference(subject.ToPathsD(), clip.ToPathsD(), FillRule.NonZero, precision).ToPolygon();
        }
        public static IList<IList<Vector3>> Difference(IEnumerable<Vector3> subject, IList<IList<Vector3>> clips, int precision = 7)
        {
            return Clipper2.Difference(subject.ToPathsD(), clips.ToPathsD(), FillRule.NonZero, precision).ToPolygons();
        }
        #endregion

        #region Casting
        private static PathD ToPathD(this IEnumerable<Vector3> polygon)
        {
            return new PathD(polygon.Select(point => point.ToPointD()));
        }
        private static PointD ToPointD(this Vector3 point)
        {
            double x = (double)point.x;
            double y = (double)point.y;

            return new PointD(x, y);
        }
        private static PathsD ToPathsD(this IEnumerable<Vector3> polygon)
        {
            return new PathsD()
        {
            ToPathD(polygon)
        };
        }
        private static PathsD ToPathsD(this IList<IList<Vector3>> polygons)
        {
            PathsD result = new PathsD();

            foreach (var polygon in polygons)
            {
                result.Add(ToPathD(polygon));
            }

            return result;
        }
        private static IList<Vector3> ToPolygon(this PathD path)
        {
            return path.Select(point => new Vector3((float)point.x, (float)point.y)).ToList();
        }
        private static IList<Vector3> ToPolygon(this PathsD path)
        {
            IList<Vector3> result = new List<Vector3>();

            foreach (var subPath in path)
            {
                result.AddRange(subPath.ToPolygon());
            }

            return result;
        }
        private static IList<IList<Vector3>> ToPolygons(this PathsD path)
        {
            IList<IList<Vector3>> result = new List<IList<Vector3>>();

            foreach (var subPath in path)
            {
                result.Add(subPath.ToPolygon());
            }

            return result;
        }
        #endregion
    }
}