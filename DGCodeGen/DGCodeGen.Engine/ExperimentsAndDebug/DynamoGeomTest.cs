using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dy = Autodesk.DesignScript.Geometry;
using gh = Rhino.Geometry;

namespace DGCodeGen.Engine.ExperimentsAndDebug
{
    //public class DynamoGeomTest
    //{
    //    public void Test()
    //    {
    //        dy.Point dyPt = dy.Point.ByCoordinates(0,0);
    //        var dyX = dyPt.X;
    //        var dySum = 2 * dyPt;

    //        gh.Point3d ghPt = new gh.Point3d();
    //        ghPt
    //    }
    //}
}

/*
    public class Point : Geometry
    {
        [Runtime.Scaling]
        public double Z { get; }
        [Runtime.Scaling]
        public double X { get; }
        [Runtime.Scaling]
        public double Y { get; }

        public static Point ByCartesianCoordinates([DefaultArgument("CoordinateSystem.ByOrigin(0, 0, 0)")] CoordinateSystem cs, [Runtime.Scaling] double x = 0, [Runtime.Scaling] double y = 0, [Runtime.Scaling] double z = 0);
        public static Point ByCoordinates([Runtime.Scaling] double x = 0, [Runtime.Scaling] double y = 0);
        public static Point ByCoordinates([Runtime.Scaling] double x = 0, [Runtime.Scaling] double y = 0, [Runtime.Scaling] double z = 0);
        public static Point ByCylindricalCoordinates([DefaultArgument("CoordinateSystem.ByOrigin(0, 0, 0)")] CoordinateSystem cs, double angle = 0, [Runtime.Scaling] double elevation = 0, [Runtime.Scaling] double radius = 1);
        public static Point BySphericalCoordinates([DefaultArgument("CoordinateSystem.ByOrigin(0, 0, 0)")] CoordinateSystem cs, double phi = 0, double theta = 0, [Runtime.Scaling] double radius = 1);
        public static Point Origin();
        public static Point[] PruneDuplicates(IEnumerable<Point> points, [Runtime.Scaling] double tolerance = 0.001);
        public Point Add(Vector vectorToAdd);
        public Vector AsVector();
        public Geometry[] Project(Geometry baseGeometry, Vector projectionDirection);
        public Point Subtract(Vector vectorToSubtract);
        public override string ToString();
        protected override int ComputeHashCode();
        protected override bool Equals(DesignScriptEntity other);
    }

    //
    // Summary:
    //     Represents the three coordinates of a point in three-dimensional space, using
    //     System.Double-precision floating point values.
    [DebuggerDisplay("({m_x}, {m_y}, {m_z})")]
    [DefaultMember("Item")]
    public struct Point3d : ISerializable, IEquatable<Point3d>, IComparable<Point3d>, IComparable, IEpsilonComparable<Point3d>, ICloneable, IValidable
    {
        //
        // Summary:
        //     Initializes a new point by copying coordinates from the components of a vector.
        //
        // Parameters:
        //   vector:
        //     A vector.
        public Point3d(Vector3d vector);
        //
        // Summary:
        //     Initializes a new point by copying coordinates from a single-precision point.
        //
        // Parameters:
        //   point:
        //     A point.
        public Point3d(Point3f point);
        //
        // Summary:
        //     Initializes a new point by copying coordinates from another point.
        //
        // Parameters:
        //   point:
        //     A point.
        public Point3d(Point3d point);
        //
        // Summary:
        //     Initializes a new point by copying coordinates from a four-dimensional point.
        //     The first three coordinates are divided by the last one. If the W (fourth) dimension
        //     of the input point is zero, then it will be just discarded.
        //
        // Parameters:
        //   point:
        //     A point.
        public Point3d(Point4d point);
        //
        // Summary:
        //     Initializes a new point by defining the X, Y and Z coordinates.
        //
        // Parameters:
        //   x:
        //     The value of the X (first) coordinate.
        //
        //   y:
        //     The value of the Y (second) coordinate.
        //
        //   z:
        //     The value of the Z (third) coordinate.
        public Point3d(double x, double y, double z);

        //
        // Summary:
        //     Gets or sets an indexed coordinate of this point.
        //
        // Parameters:
        //   index:
        //     The coordinate index. Valid values are:
        //     0 = X coordinate
        //     1 = Y coordinate
        //     2 = Z coordinate
        //     .
        public double this[int index] { get; set; }

        //
        // Summary:
        //     Gets the value of a point at location RhinoMath.UnsetValue,RhinoMath.UnsetValue,RhinoMath.UnsetValue.
        public static Point3d Unset { get; }
        //
        // Summary:
        //     Gets the value of a point at location 0,0,0.
        public static Point3d Origin { get; }
        //
        // Summary:
        //     Each coordinate of the point must pass the Rhino.RhinoMath.IsValidDouble(System.Double)
        //     test.
        public bool IsValid { get; }
        //
        // Summary:
        //     Gets or sets the Z (third) coordinate of this point.
        public double Z { get; set; }
        //
        // Summary:
        //     Gets or sets the Y (second) coordinate of this point.
        public double Y { get; set; }
        //
        // Summary:
        //     Gets or sets the X (first) coordinate of this point.
        public double X { get; set; }
        //
        // Summary:
        //     Gets the smallest (both positive and negative) coordinate value in this point.
        public double MinimumCoordinate { get; }
        //
        // Summary:
        //     Gets the largest (both positive and negative) valid coordinate in this point,
        //     or RhinoMath.UnsetValue if no coordinate is valid.
        public double MaximumCoordinate { get; }

        //
        // Summary:
        //     Sums up a point and a vector, and returns a new point.
        //     (Provided for languages that do not support operator overloading. You can use
        //     the + operator otherwise)
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   vector:
        //     A vector.
        //
        // Returns:
        //     A new point that results from the addition of point and vector.
        public static Point3d Add(Point3d point, Vector3d vector);
        //
        // Summary:
        //     Sums up a point and a vector, and returns a new point.
        //     (Provided for languages that do not support operator overloading. You can use
        //     the + operator otherwise)
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   vector:
        //     A vector.
        //
        // Returns:
        //     A new point that results from the addition of point and vector.
        public static Point3d Add(Point3d point, Vector3f vector);
        //
        // Summary:
        //     Sums up a point and a vector, and returns a new point.
        //     (Provided for languages that do not support operator overloading. You can use
        //     the + operator otherwise)
        //
        // Parameters:
        //   vector:
        //     A vector.
        //
        //   point:
        //     A point.
        //
        // Returns:
        //     A new point that results from the addition of point and vector.
        public static Point3d Add(Vector3d vector, Point3d point);
        //
        // Summary:
        //     Sums two Rhino.Geometry.Point3d instances.
        //     (Provided for languages that do not support operator overloading. You can use
        //     the + operator otherwise)
        //
        // Parameters:
        //   point1:
        //     A point.
        //
        //   point2:
        //     A point.
        //
        // Returns:
        //     A new point that results from the addition of point1 and point2.
        public static Point3d Add(Point3d point1, Point3d point2);
        //
        // Summary:
        //     Determines whether a set of points is coplanar within a given tolerance.
        //
        // Parameters:
        //   points:
        //     A list, an array or any enumerable of Rhino.Geometry.Point3d.
        //
        //   tolerance:
        //     A tolerance value. A default might be RhinoMath.ZeroTolerance.
        //
        // Returns:
        //     true if points are on the same plane; false otherwise.
        public static bool ArePointsCoplanar(IEnumerable<Point3d> points, double tolerance);
        //
        // Summary:
        //     Removes duplicates in the supplied set of points.
        //
        // Parameters:
        //   points:
        //     A list, an array or any enumerable of Rhino.Geometry.Point3d.
        //
        //   tolerance:
        //     The minimum distance between points.
        //     Points that fall within this tolerance will be discarded.
        //     .
        //
        // Returns:
        //     An array of points without duplicates; or null on error.
        public static Point3d[] CullDuplicates(IEnumerable<Point3d> points, double tolerance);
        //
        // Summary:
        //     Divides a Rhino.Geometry.Point3d by a number.
        //     (Provided for languages that do not support operator overloading. You can use
        //     the / operator otherwise)
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   t:
        //     A number.
        //
        // Returns:
        //     A new point that is coordinatewise divided by t.
        public static Point3d Divide(Point3d point, double t);
        //
        // Summary:
        //     Converts a single-precision point in a double-precision point.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        // Returns:
        //     The resulting point.
        public static Point3d FromPoint3f(Point3f point);
        //
        // Summary:
        //     Multiplies a Rhino.Geometry.Point3d by a number.
        //     (Provided for languages that do not support operator overloading. You can use
        //     the * operator otherwise)
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   t:
        //     A number.
        //
        // Returns:
        //     A new point that is coordinatewise multiplied by t.
        public static Point3d Multiply(Point3d point, double t);
        //
        // Summary:
        //     Multiplies a Rhino.Geometry.Point3d by a number.
        //     (Provided for languages that do not support operator overloading. You can use
        //     the * operator otherwise)
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   t:
        //     A number.
        //
        // Returns:
        //     A new point that is coordinatewise multiplied by t.
        public static Point3d Multiply(double t, Point3d point);
        //
        // Summary:
        //     Orders a set of points so they will be connected in a "reasonable polyline" order.
        //     Also, removes points from the list if their common distance exceeds a specified
        //     threshold.
        //
        // Parameters:
        //   points:
        //     A list, an array or any enumerable of Rhino.Geometry.Point3d.
        //
        //   minimumDistance:
        //     Minimum allowed distance among a pair of points. If points are closer than this,
        //     only one of them will be kept.
        //
        // Returns:
        //     The new array of sorted and culled points.
        public static Point3d[] SortAndCullPointList(IEnumerable<Point3d> points, double minimumDistance);
        //
        // Summary:
        //     Subtracts a vector from a point.
        //     (Provided for languages that do not support operator overloading. You can use
        //     the - operator otherwise)
        //
        // Parameters:
        //   vector:
        //     A vector.
        //
        //   point:
        //     A point.
        //
        // Returns:
        //     A new point that is the difference of point minus vector.
        public static Point3d Subtract(Point3d point, Vector3d vector);
        //
        // Summary:
        //     Subtracts a point from another point.
        //     (Provided for languages that do not support operator overloading. You can use
        //     the - operator otherwise)
        //
        // Parameters:
        //   point1:
        //     A point.
        //
        //   point2:
        //     Another point.
        //
        // Returns:
        //     A new vector that is the difference of point minus vector.
        public static Vector3d Subtract(Point3d point1, Point3d point2);
        //
        // Summary:
        //     Converts the string representation of a point to the equivalent Point3d structure.
        //
        // Parameters:
        //   input:
        //     The point to convert.
        //
        //   result:
        //     The structure that will contain the parsed value.
        //
        // Returns:
        //     true if successful, false otherwise.
        public static bool TryParse(string input, out Point3d result);
        //
        // Summary:
        //     Compares this Rhino.Geometry.Point3d with another Rhino.Geometry.Point3d.
        //     Component evaluation priority is first X, then Y, then Z.
        //
        // Parameters:
        //   other:
        //     The other Rhino.Geometry.Point3d to use in comparison.
        //
        // Returns:
        //     0: if this is identical to other
        //     -1: if this.X < other.X
        //     -1: if this.X == other.X and this.Y < other.Y
        //     -1: if this.X == other.X and this.Y == other.Y and this.Z < other.Z
        //     +1: otherwise.
        [ConstOperationAttribute]
        public int CompareTo(Point3d other);
        //
        // Summary:
        //     Computes the distance between two points.
        //
        // Parameters:
        //   other:
        //     Other point for distance measurement.
        //
        // Returns:
        //     The length of the line between this and the other point; or 0 if any of the points
        //     is not valid.
        [ConstOperationAttribute]
        public double DistanceTo(Point3d other);
        //
        // Summary:
        //     Computes the square of the distance between two points.
        //     This method is usually largely faster than DistanceTo().
        //
        // Parameters:
        //   other:
        //     Other point for squared distance measurement.
        //
        // Returns:
        //     The squared length of the line between this and the other point; or 0 if any
        //     of the points is not valid.
        [ConstOperationAttribute]
        public double DistanceToSquared(Point3d other);
        //
        // Summary:
        //     Check that all values in other are within epsilon of the values in this
        //
        // Parameters:
        //   other:
        //
        //   epsilon:
        [ConstOperationAttribute]
        public bool EpsilonEquals(Point3d other, double epsilon);
        //
        // Summary:
        //     Determines whether the specified Rhino.Geometry.Point3d has the same values as
        //     the present point.
        //
        // Parameters:
        //   point:
        //     The specified point.
        //
        // Returns:
        //     true if point has the same coordinates as this; otherwise false.
        [ConstOperationAttribute]
        public bool Equals(Point3d point);
        //
        // Summary:
        //     Determines whether the specified System.Object is a Rhino.Geometry.Point3d and
        //     has the same values as the present point.
        //
        // Parameters:
        //   obj:
        //     The specified object.
        //
        // Returns:
        //     true if obj is a Point3d and has the same coordinates as this; otherwise false.
        [ConstOperationAttribute]
        public override bool Equals(object obj);
        //
        // Summary:
        //     Computes a hash code for the present point.
        //
        // Returns:
        //     A non-unique integer that represents this point.
        [ConstOperationAttribute]
        public override int GetHashCode();
        //
        // Summary:
        //     Interpolate between two points.
        //
        // Parameters:
        //   pA:
        //     First point.
        //
        //   pB:
        //     Second point.
        //
        //   t:
        //     Interpolation parameter. If t=0 then this point is set to pA. If t=1 then this
        //     point is set to pB. Values of t in between 0.0 and 1.0 result in points between
        //     pA and pB.
        public void Interpolate(Point3d pA, Point3d pB, double t);
        //
        // Summary:
        //     Constructs the string representation for the current point.
        //
        // Returns:
        //     The point representation in the form X,Y,Z.
        [ConstOperationAttribute]
        public override string ToString();
        //
        // Summary:
        //     Transforms the present point in place. The transformation matrix acts on the
        //     left of the point. i.e.,
        //     result = transformation*point
        //
        // Parameters:
        //   xform:
        //     Transformation to apply.
        public void Transform(Transform xform);

        //
        // Summary:
        //     Sums up a point and a vector, and returns a new point.
        //
        // Parameters:
        //   vector:
        //     A vector.
        //
        //   point:
        //     A point.
        //
        // Returns:
        //     A new point that results from the addition of point and vector.
        public static Point3d operator +(Vector3d vector, Point3d point);
        //
        // Summary:
        //     Sums up a point and a vector, and returns a new point.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   vector:
        //     A vector.
        //
        // Returns:
        //     A new point that results from the addition of point and vector.
        public static Point3d operator +(Point3d point, Vector3d vector);
        //
        // Summary:
        //     Sums two Rhino.Geometry.Point3d instances.
        //
        // Parameters:
        //   point1:
        //     A point.
        //
        //   point2:
        //     A point.
        //
        // Returns:
        //     A new point that results from the addition of point1 and point2.
        public static Point3d operator +(Point3d point1, Point3d point2);
        //
        // Summary:
        //     Sums up a point and a vector, and returns a new point.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   vector:
        //     A vector.
        //
        // Returns:
        //     A new point that results from the addition of point and vector.
        public static Point3d operator +(Point3d point, Vector3f vector);
        //
        // Summary:
        //     Computes the additive inverse of all coordinates in the point, and returns the
        //     new point.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        // Returns:
        //     A point value that, when summed with the point input, yields the Rhino.Geometry.Point3d.Origin.
        public static Point3d operator -(Point3d point);
        //
        // Summary:
        //     Subtracts a vector from a point.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   vector:
        //     A vector.
        //
        // Returns:
        //     A new point that is the difference of point minus vector.
        public static Point3d operator -(Point3d point, Vector3d vector);
        //
        // Summary:
        //     Subtracts a point from another point.
        //
        // Parameters:
        //   point1:
        //     A point.
        //
        //   point2:
        //     Another point.
        //
        // Returns:
        //     A new vector that is the difference of point minus vector.
        public static Vector3d operator -(Point3d point1, Point3d point2);
        //
        // Summary:
        //     Multiplies a Rhino.Geometry.Point3d by a number.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   t:
        //     A number.
        //
        // Returns:
        //     A new point that is coordinatewise multiplied by t.
        public static Point3d operator *(double t, Point3d point);
        //
        // Summary:
        //     Multiplies a Rhino.Geometry.Point3d by a number.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   t:
        //     A number.
        //
        // Returns:
        //     A new point that is coordinatewise multiplied by t.
        public static Point3d operator *(Point3d point, double t);
        //
        // Summary:
        //     Divides a Rhino.Geometry.Point3d by a number.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        //   t:
        //     A number.
        //
        // Returns:
        //     A new point that is coordinatewise divided by t.
        public static Point3d operator /(Point3d point, double t);
        //
        // Summary:
        //     Determines whether two Point3d have equal values.
        //
        // Parameters:
        //   a:
        //     The first point.
        //
        //   b:
        //     The second point.
        //
        // Returns:
        //     true if the coordinates of the two points are exactly equal; otherwise false.
        public static bool operator ==(Point3d a, Point3d b);
        //
        // Summary:
        //     Determines whether two Point3d have different values.
        //
        // Parameters:
        //   a:
        //     The first point.
        //
        //   b:
        //     The second point.
        //
        // Returns:
        //     true if the two points differ in any coordinate; false otherwise.
        public static bool operator !=(Point3d a, Point3d b);
        //
        // Summary:
        //     Determines whether the first specified point comes before (has inferior sorting
        //     value than) the second point.
        //     Coordinates evaluation priority is first X, then Y, then Z.
        //
        // Parameters:
        //   a:
        //     The first point.
        //
        //   b:
        //     The second point.
        //
        // Returns:
        //     true if a.X is smaller than b.X, or a.X == b.X and a.Y is smaller than b.Y, or
        //     a.X == b.X and a.Y == b.Y and a.Z is smaller than b.Z; otherwise, false.
        public static bool operator <(Point3d a, Point3d b);
        //
        // Summary:
        //     Determines whether the first specified point comes after (has superior sorting
        //     value than) the second point.
        //     Coordinates evaluation priority is first X, then Y, then Z.
        //
        // Parameters:
        //   a:
        //     The first point.
        //
        //   b:
        //     The second point.
        //
        // Returns:
        //     true if a.X is larger than b.X, or a.X == b.X and a.Y is larger than b.Y, or
        //     a.X == b.X and a.Y == b.Y and a.Z is larger than b.Z; otherwise, false.
        public static bool operator >(Point3d a, Point3d b);
        //
        // Summary:
        //     Determines whether the first specified point comes before (has inferior sorting
        //     value than) the second point, or it is equal to it.
        //     Coordinates evaluation priority is first X, then Y, then Z.
        //
        // Parameters:
        //   a:
        //     The first point.
        //
        //   b:
        //     The second point.
        //
        // Returns:
        //     true if a.X is smaller than b.X, or a.X == b.X and a.Y is smaller than b.Y, or
        //     a.X == b.X and a.Y == b.Y and a.Z <= b.Z; otherwise, false.
        public static bool operator <=(Point3d a, Point3d b);
        //
        // Summary:
        //     Determines whether the first specified point comes after (has superior sorting
        //     value than) the second point, or it is equal to it.
        //     Coordinates evaluation priority is first X, then Y, then Z.
        //
        // Parameters:
        //   a:
        //     The first point.
        //
        //   b:
        //     The second point.
        //
        // Returns:
        //     true if a.X is larger than b.X, or a.X == b.X and a.Y is larger than b.Y, or
        //     a.X == b.X and a.Y == b.Y and a.Z >= b.Z; otherwise, false.
        public static bool operator >=(Point3d a, Point3d b);

        //
        // Summary:
        //     Converts a single-precision point in a double-precision point, without needing
        //     casting.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        // Returns:
        //     The resulting point.
        public static implicit operator Point3d(Point3f point);
        //
        // Summary:
        //     Converts a point in a control point, without needing casting.
        //
        // Parameters:
        //   pt:
        //     The point.
        //
        // Returns:
        //     The control point.
        public static implicit operator ControlPoint(Point3d pt);
        //
        // Summary:
        //     Converts a point in a vector, needing casting.
        //
        // Parameters:
        //   point:
        //     A point.
        //
        // Returns:
        //     The resulting vector.
        public static explicit operator Vector3d(Point3d point);
        //
        // Summary:
        //     Converts a vector in a point, needing casting.
        //
        // Parameters:
        //   vector:
        //     A vector.
        //
        // Returns:
        //     The resulting point.
        public static explicit operator Point3d(Vector3d vector);
    }


 
 */