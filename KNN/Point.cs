using System.Numerics;
using Auios.QuadTree;

namespace KNN;

public enum DogClass
{
    None,
    Alabai,
    York
}

// Implement IQuadTreeObjectBounds<T> interface for the object type to be stored
public class PointBounds : IQuadTreeObjectBounds<Point>
{
    public float GetBottom(Point obj) => (float)obj.Y;
    public float GetTop(Point obj) => (float)obj.Y;
    public float GetLeft(Point obj) => (float)obj.X;
    public float GetRight(Point obj) => (float)obj.X;
}

public class Point
{
    public float X { get; private set; }
    public float Y { get; private set; }

    public DogClass Class { get; private set; }

    public Point(float x, float y, DogClass dogClass)
    {
        X = x;
        Y = y;
        Class = dogClass;
    }

    public Point(float x, float y, KNN knn, float maxDistance, int k=5, float threshold=0.5f)
    {
        X = x;
        Y = y;
        Class = knn.ClassifyPoint(new Vector2(x, y), k, maxDistance, threshold);
    }
    
    public static explicit operator Vector2(Point point)
    {
        return new Vector2(point.X, point.Y);
    }
    
    public static explicit operator ScottPlot.Coordinates(Point point)
    {
        return new ScottPlot.Coordinates(point.X, point.Y);
    }

    public override string ToString()
    {
        return $"{Class.ToString()} at ({X}, {Y})";
    }
}