using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

//
// POINT3
//
// Represents a location on a 3d grid.  
// Hybrid of the XNA Point and Vector3 classes.
//
[System.Serializable, StructLayout(LayoutKind.Sequential)]
public struct Point3 
    : IEquatable<Point3>
{
    public int X;
    public int Y;
    public int Z;

    private static Point3 _zero;
    private static Point3 _one;
    private static Point3 _unitX;
    private static Point3 _unitY;
    private static Point3 _unitZ;
    private static Point3 _up;
    private static Point3 _down;
    private static Point3 _right;
    private static Point3 _left;
    private static Point3 _forward;
    private static Point3 _backward;

    public static Point3 Zero { get { return _zero; } }
    public static Point3 One { get { return _one; } }
    public static Point3 UnitX { get { return _unitX; } }
    public static Point3 UnitY { get { return _unitY; } }
    public static Point3 UnitZ { get { return _unitZ; } }
    public static Point3 Up { get { return _up; } }
    public static Point3 Down { get { return _down; } }
    public static Point3 Right { get { return _right; } }
    public static Point3 Left { get { return _left; } }
    public static Point3 Forward { get { return _forward; } }
    public static Point3 Backward { get { return _backward; } }

    public Point3(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public bool Equals(Point3 other)
    {
        return ((this.X == other.X) && (this.Y == other.Y) && (this.Z == other.Z));
    }

    public override bool Equals(object obj)
    {
        bool flag = false;
        if (obj is Point3)
        {
            flag = this.Equals((Point3) obj);
        }
        return flag;
    }

    public override int GetHashCode()
    {
        return (this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode());
    }

    public override string ToString()
    {
        System.Globalization.CultureInfo currentCulture = 
            System.Globalization.CultureInfo.CurrentCulture;
            
        return string.Format(
            currentCulture, 
            "{{X:{0} Y:{1} Z:{2}}}", 
            new object[] 
            { 
                this.X.ToString(currentCulture), 
                this.Y.ToString(currentCulture), 
                this.Z.ToString(currentCulture)  
            }
        );
    }

    public static bool operator ==(Point3 a, Point3 b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(Point3 a, Point3 b)
    {
        if (a.X == b.X)
        {
            if (a.Y == b.Y)
            {
                return (a.Z != b.Z);
            }
        }
        return true;
    }

    static Point3()
    {
        _zero = new Point3();
        _one = new Point3(1, 1, 1);
        _unitX = new Point3(1, 0, 0);
        _unitY = new Point3(0, 1, 0);
        _unitZ = new Point3(0, 0, 1);
        _up = new Point3(0, 1, 0);
        _down = new Point3(0, -1, 0);
        _right = new Point3(1, 0, 0);
        _left = new Point3(-1, 0, 0);
        _forward = new Point3(0, 0, -1);
        _backward = new Point3(0, 0, 1);
    }


    public static Point3 operator -(Point3 value)
    {
        Point3 point = Point3.Zero;
        point.X = -value.X;
        point.Y = -value.Y;
        point.Z = -value.Z;
        return point;
    }

    public static Point3 operator +(Point3 value1, Point3 value2)
    {
        Point3 point = Point3.Zero;
        point.X = value1.X + value2.X;
        point.Y = value1.Y + value2.Y;
        point.Z = value1.Z + value2.Z;
        return point;
    }

    public static Point3 operator -(Point3 value1, Point3 value2)
    {
        Point3 point = Point3.Zero;
        point.X = value1.X - value2.X;
        point.Y = value1.Y - value2.Y;
        point.Z = value1.Z - value2.Z;
        return point;
    }

    public static Point3 operator *(Point3 value1, Point3 value2)
    {
        Point3 point = Point3.Zero;
        point.X = value1.X * value2.X;
        point.Y = value1.Y * value2.Y;
        point.Z = value1.Z * value2.Z;
        return point;
    }

    public static Point3 operator *(Point3 value, int scaleFactor)
    {
        Point3 point = Point3.Zero;
        point.X = value.X * scaleFactor;
        point.Y = value.Y * scaleFactor;
        point.Z = value.Z * scaleFactor;
        return point;
    }

    public static Point3 operator *(int scaleFactor, Point3 value)
    {
        Point3 point = Point3.Zero;
        point.X = value.X * scaleFactor;
        point.Y = value.Y * scaleFactor;
        point.Z = value.Z * scaleFactor;
        return point;
    }

    public static Point3 operator /(Point3 value1, Point3 value2)
    {
        Point3 point = Point3.Zero;
        point.X = value1.X / value2.X;
        point.Y = value1.Y / value2.Y;
        point.Z = value1.Z / value2.Z;
        return point;
    }

    public static Point3 operator /(Point3 value, int divider)
    {
        Point3 point = Point3.Zero;
        point.X = value.X / divider;
        point.Y = value.Y / divider;
        point.Z = value.Z / divider;
        return point;
    }



}
