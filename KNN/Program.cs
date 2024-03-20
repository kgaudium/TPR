using System.Globalization;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using LumenWorks.Framework.IO.Csv;
using ScottPlot;

namespace KNN;
static class KnnProgram
{
    public static void Main()
    {
        List<Point> alabaiPoints = new List<Point>();
        List<Point> yorkPoints = new List<Point>();
        
        // Parsing alabai set
        using (CsvReader csvReader = new CsvReader(new StreamReader("./csv/alabai.csv"), true))
        {
            while (csvReader.ReadNextRecord())
            {
                alabaiPoints.Add(new Point(float.Parse(csvReader[0], CultureInfo.InvariantCulture), 
                    float.Parse(csvReader[1], CultureInfo.InvariantCulture),
                    DogClass.Alabai));
            }
            
        }
        
        // Parsing York set
        using (CsvReader csvReader = new CsvReader(new StreamReader("./csv/york.csv"), true))
        {
            while (csvReader.ReadNextRecord())
            {
                yorkPoints.Add(new Point(float.Parse(csvReader[0], CultureInfo.InvariantCulture), 
                    float.Parse(csvReader[1], CultureInfo.InvariantCulture),
                    DogClass.York));
            }
            
        }

        // Part of training set
        float trainSetShare = 0.8f;
        
        // Define colors
        Color alabaiColor = Color.FromHex("fcdb03");
        Color yorkColor = Color.FromHex("9003fc");
        Color newAlabaiColor = Color.FromHex("fc2c03");
        Color newYorkColor = Color.FromHex("00eeff");

        // Define Sets
        int alabaiTrainCount = (int)(alabaiPoints.Count * trainSetShare);
        int yorkTrainCount = (int)(yorkPoints.Count * trainSetShare);
        
        var alabaiTrainSet = alabaiPoints.GetRange(0, alabaiTrainCount);
        var alabaiTestSet = alabaiPoints.GetRange(alabaiTrainCount+1, alabaiPoints.Count-alabaiTrainCount-1);
        
        var yorkTrainSet = yorkPoints.GetRange(0, yorkTrainCount);
        var yorkTestSet = yorkPoints.GetRange(yorkTrainCount+1, yorkPoints.Count-yorkTrainCount-1);
        
        // Loads only Training Set
        var allTrainPoints = alabaiTrainSet.Concat(yorkTrainSet).ToList();
        KNN knn = new KNN(allTrainPoints.ToArray(), 90, 120);

        // int i = 0;
        // foreach (var point in knn.GetPointsInArea(49, 60, 2, 5))
        // {
        //     i++;
        //     Console.WriteLine(point);
        // }
        //
        // Console.WriteLine(i);
        
        Plot myPlot = new();

        // Draw Alabai Train points
        for (int i = 0; i < alabaiTrainCount; i++)
        {
            var point = alabaiPoints[i];
            myPlot.Add.Scatter((Coordinates)point, alabaiColor);
        }
        
        // Draw York Train points
        for (int i = 0; i < yorkTrainCount; i++)
        {
            var point = yorkPoints[i];
            myPlot.Add.Scatter((Coordinates)point, yorkColor);
        }
        
        //
        // myPlot.Add.Scatter(Array.ConvertAll(allPoints.ToArray(), item => (ScottPlot.Coordinates)item));
        //
        myPlot.SavePng("trainPoints.png", 1080, 1080);

        // Test Alabai classification
        int alabaiCorrect = 0;
        for (int i = 0; i < alabaiTestSet.Count; i++)
        {
            Point newPoint = new Point(alabaiTestSet[i].X, alabaiTestSet[i].Y, knn, 5);
            if (newPoint.Class == DogClass.Alabai)
            {
                myPlot.Add.Scatter((Coordinates)newPoint, newAlabaiColor);
                alabaiCorrect++;
            }
            else if (newPoint.Class == DogClass.York)
            {
                myPlot.Add.Scatter((Coordinates)newPoint, newYorkColor);
            }
            else
            {
                Console.WriteLine("Alabai cannot classified!");
                myPlot.Add.Scatter((Coordinates)newPoint, Colors.Black);

            }

        }
        
        // Test York classification
        int yorkCorrect = 0;
        for (int i = 0; i < yorkTestSet.Count; i++)
        {
            Point newPoint = new Point(yorkTestSet[i].X, yorkTestSet[i].Y, knn, 5);
            if (newPoint.Class == DogClass.York)
            {
                myPlot.Add.Scatter((Coordinates)newPoint, newYorkColor);
                yorkCorrect++;
            }
            else if (newPoint.Class == DogClass.Alabai)
            {
                myPlot.Add.Scatter((Coordinates)newPoint, newAlabaiColor);
            }
            else
            {
                Console.WriteLine("York cannot classified!");
                myPlot.Add.Scatter((Coordinates)newPoint, Colors.Black);

            }
        }
        
        myPlot.SavePng("testPoints.png", 1080, 1080);

        // Calculate Accuracy
        Console.WriteLine($"Accuracy for Alabais: {alabaiCorrect/(float)alabaiTestSet.Count} - ({alabaiCorrect}/{alabaiTestSet.Count})");
        Console.WriteLine($"Accuracy for Yorks: {yorkCorrect/(float)yorkTestSet.Count} - ({yorkCorrect}/{yorkTestSet.Count})");

        
        Console.WriteLine($"Point class: " +
                          $"{knn.ClassifyPoint(new Vector2(48.2f, 52), 2, 3)}");
        while (true)
        {
            Console.Write(">>> ");
            var input = Console.ReadLine().Split(' ');

            if (input[0].ToLower() == "order")
            {
                Console.WriteLine($"Enter newOrder");
                Console.Write    ($"      float\n>> ");
                var newInput = Console.ReadLine().Split(' ');
                try
                {
                    knn.DistanceOrder = Single.Parse(newInput[1]);
                    Console.WriteLine($"Distance order changed to {newInput[1]}!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error! {e}");
                }
                continue;
            }
                
            if (input[0].ToLower() == "nearest" || input[0].ToLower() == "near")
            {
                Console.WriteLine($"Enter x,     y,     k,   maxDist");
                Console.Write    ($"      float  float  int  float\n>> ");
                var newInput = Console.ReadLine().Split(' ');
                try
                {
                    var nearest =
                        knn.GetNearestPoints(new Vector2(Single.Parse(newInput[0]), Single.Parse(newInput[1])),
                            int.Parse(newInput[2]), Single.Parse(newInput[3]));

                    foreach (var tuple in  nearest)
                    {
                        Console.WriteLine($"{tuple.Item1}, distance: {tuple.Item2}");
                    }
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error! {e}");
                }
                continue;
            }
            
            if (input[0].ToLower() == "point")
            {
                Console.WriteLine($"Enter x,     y,     maxDist");
                Console.Write    ($"      float  float  (float)=5.\n>> ");
                var newInput = Console.ReadLine().Split(' ');
                try
                {
                    float maxDist = 5;
                    float x = float.Parse(newInput[0]);
                    float y = float.Parse(newInput[1]);

                    if (newInput.Length == 3)
                    {
                        maxDist = float.Parse(newInput[2]);
                    }
                    
                    Console.WriteLine(new Point(x, y, knn, maxDist));
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error! {e}");
                }
                continue;
            }

            if (input[0].ToLower() == "help")
            {
                Console.WriteLine("Available commands:\n\tpoint (x, y, maxDist=5) - classifies a point.\n\tnear (or nearest) (x, y, k, maxDist) - shows a list of nearest points to specified coords\n\torder (newOrder) - changes the order of the Minkowski distance\n\thelp - shows this message\n");
                continue;
            }


            
        }
    }
}