using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 

namespace ResistanceCalculator
{
    class Program
    {
        public static void Main(string[] args)
        {
            Calculator calc = new Calculator();
            List<double> resValues = calc.ParseFile(@"C:\Users\ryan_\source\repos\rescalc\stuff\resvalues.txt");

            int goalResistance = 180;

            Dictionary<string, double> solutionsDictionary = new Dictionary<string, double>();

            calc.CalculateSeriesResults(resValues, goalResistance, ref solutionsDictionary);
            calc.CalculateParallelResults(resValues, goalResistance, ref solutionsDictionary);
            foreach (KeyValuePair<string, double> kvp in solutionsDictionary.OrderByDescending(key => key.Value))
            {
                Console.WriteLine($"{kvp.Key}\t{kvp.Value}% from {goalResistance}");
            }

        }

        public class Calculator 
        {
            public Calculator()
            {

            }
            public List<double> ParseFile(string FilePath)
            {
                List<double> file = new List<double>();
                if (File.Exists(FilePath))
                {
                    file = File.ReadAllText(@"C:\Users\ryan_\source\repos\rescalc\stuff\resvalues.txt").Split('\n').Where(x => !String.IsNullOrEmpty(x)).Select(x => double.Parse(x)).ToList();
                    file.Sort();
                }
                return file;
            }

            public void CalculateSeriesResults(List<double> resValues, double goalResistance, ref Dictionary<string, double> solutionsDictionary)
            {
                for (int i = 0; i < resValues.Count; i++)
                {
                    for (int j = 0; j < resValues.Count; j++)
                    {
                        string resistanceSolution = $"{resValues[i]} + {resValues[j]}";
                        double resistanceError = Math.Abs(((resValues[i] + resValues[j]) - goalResistance) / goalResistance * 100);

                        if (!solutionsDictionary.ContainsKey(resistanceSolution))
                        {
                            solutionsDictionary.Add(resistanceSolution, resistanceError);
                        }
                    }
                }
            }

            public void CalculateParallelResults(List<double> resValues, double goalResistance, ref Dictionary<string, double> solutionsDictionary)
            {
                for (int i = 0; i < resValues.Count; i++)
                {
                    for (int j = 0; j < resValues.Count; j++)
                    {
                        string resistanceSolution = $"{resValues[i]} || {resValues[j]}";
                        double resistanceValue = (resValues[i] * resValues[j]) / (resValues[i] + resValues[j]);
                        double resistanceError = Math.Abs((resistanceValue - goalResistance) / goalResistance * 100);

                        if (!solutionsDictionary.ContainsKey(resistanceSolution))
                        {
                            solutionsDictionary.Add(resistanceSolution, resistanceError);
                        }
                    }
                }
            }
        }
    }
}
