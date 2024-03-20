using System.Numerics;
using System.Reflection;
using Auios.QuadTree;

namespace KNN;

public static class Distance
{
    public static float Minkowski(Vector2 a, Vector2 b, float order)
    {
        return (float)Math.Pow(Math.Pow(Math.Abs(a.X - b.X), order) + Math.Pow(Math.Abs(a.Y - b.Y), order), 1/order);
    }
}

// Implementation of K-nearest neighbors algorithm 
public class KNN
{
    public QuadTree<Point> Points;
    public float DistanceOrder = 2;

    public KNN(Point[] points, int width, int height)
    {
        Points = new QuadTree<Point>(width, height, new PointBounds());
        
        foreach (var point in points)
        {
            Points.Insert(point);
        }
    }

    public Tuple<Point, float>[] GetNearestPoints(Vector2 center, int k, float maxDistance)
    {
        List<Tuple<Point, float>> result = new List<Tuple<Point, float>>();

        var NearPoints = GetPointsInArea(
            (int)Math.Floor(center.X - maxDistance), 
            (int)Math.Floor(center.Y - maxDistance), 
            (int)Math.Ceiling(center.X - maxDistance),
            (int)Math.Ceiling(center.Y - maxDistance), false);

        for (int i = 0; i < NearPoints.Length; i++)
        {
            float distance = Distance.Minkowski(center, (Vector2)NearPoints[i], DistanceOrder);
            if (distance <= maxDistance)
            {
                result.Add(new Tuple<Point, float>(NearPoints[i], distance));
            }
        }

        try
        {
            return result.OrderBy(v => v.Item2).ToList().GetRange(0, k).ToArray();
        }
        catch (ArgumentException e)
        {
            return result.OrderBy(v => v.Item2).ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public DogClass ClassifyPoint(Vector2 coords, int k, float maxDistance, float threshold=0.5f)
    {
        var nearestPoints = GetNearestPoints(coords, k, maxDistance);
        var classes = Enum.GetValues(typeof(DogClass));
        var classCount = classes.Length;
        var pointsCount = new float[classCount];

        if (nearestPoints.Length != k)
        {
            // Console.WriteLine($"Nearest points count: {nearestPoints.Length}, k: {k}, Distance limit: {maxDistance}, Max found by distance: {nearestPoints.MaxBy(item => item.Item2)}");
        }
        
        if (nearestPoints.Length == 0)
        {
            return DogClass.None;
        }

        for (int i = 0; i < nearestPoints.Length; i++)
        {
            var pointTuple = nearestPoints[i];
            pointsCount[Array.IndexOf(classes, pointTuple.Item1.Class)] += 1;
        }

        int maxIndex = Array.IndexOf(pointsCount, pointsCount.Max());
        // int maxIndex = 0;
        // for (int i = 0; i < classCount; i++)
        // {
        //     if (pointsCount[i] > pointsCount[maxIndex])
        //         maxIndex = i;
        // }

        return (DogClass)classes.GetValue(maxIndex);
    }
    
    public Point[] GetPointsInArea(int x, int y, int width, int height, bool accurate = false)
    {
        QuadTreeRect searchArea = new QuadTreeRect(x, y, width, height);

        if (!accurate)
        {
            return Points.FindObjects(searchArea);
        }

        List<Point> result = new List<Point>();
        
        foreach (var point in Points.FindObjects(searchArea))
        {
            if (point.X >= x && point.X <= x + width &&
                point.Y >= y && point.Y <= y + height)
            {
                result.Add(point);
            }
        }

        return result.ToArray();
    }
}