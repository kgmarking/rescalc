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

            Dictionary<Equation, double> solutionsDictionary = new Dictionary<Equation, double>();

            calc.CalculateSeriesResults(resValues, goalResistance, ref solutionsDictionary);
            calc.CalculateParallelResults(resValues, goalResistance, ref solutionsDictionary);
            foreach (KeyValuePair<Equation, double> kvp in solutionsDictionary.OrderByDescending(key => key.Value))
            {
                Console.WriteLine($"{kvp.Key.Operand1}{kvp.Key.Operation}{kvp.Key.Operand2} {kvp.Value}% from {goalResistance}");
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

            public void CalculateSeriesResults(List<double> resValues, double goalResistance, ref Dictionary<Equation, double> solutionsDictionary)
            {
                string localOperation = "+";
                for (int i = 0; i < resValues.Count; i++)
                {
                    for (int j = 0; j < resValues.Count; j++)
                    {
                        string resistanceSolution = $"{resValues[i]} + {resValues[j]}";
                        double resistanceError = Math.Abs(((resValues[i] + resValues[j]) - goalResistance) / goalResistance * 100);

                        Equation eq = new Equation(resValues[i], resValues[j], localOperation);

                        if (!solutionsDictionary.Keys.Where(x => (((x.Operand1 == resValues[i]) && (x.Operand2 == resValues[j])) ||
                        ((x.Operand1 == resValues[j]) && (x.Operand2 == resValues[i]))) && x.Operation.Equals(localOperation)).Any())
                        {
                            solutionsDictionary.Add(eq, resistanceError);
                        }
                    }
                }
            }

            public void CalculateParallelResults(List<double> resValues, double goalResistance, ref Dictionary<Equation, double> solutionsDictionary)
            {
                string localOperation = "||";
                for (int i = 0; i < resValues.Count; i++)
                {
                    if (resValues[i] < goalResistance)
                    {
                        continue;
                    }
                    for (int j = 0; j < resValues.Count; j++)
                    {
                        if (resValues[j] < goalResistance)
                        {
                            continue;
                        }

                        double resistanceValue = (resValues[i] * resValues[j]) / (resValues[i] + resValues[j]);
                        double resistanceError = Math.Abs((resistanceValue - goalResistance) / goalResistance * 100);

                        Equation eq = new Equation(resValues[i], resValues[j], localOperation);

                        if (!solutionsDictionary.Keys.Where(x => (((x.Operand1 == resValues[i]) && (x.Operand2 == resValues[j])) ||
                        ((x.Operand1 == resValues[j]) && (x.Operand2 == resValues[i]))) && x.Operation.Equals(localOperation)).Any())
                        {
                            solutionsDictionary.Add(eq, resistanceError);
                        }
                    }
                }
            }
        }

        public class Equation
        {
            public string Operation { get; set; }
            public double Operand1 { get; set; }
            public double Operand2 { get; set; }

            public Equation()
            {

            }

            public Equation(double operand1, double operand2, string operation)
            {
                Operand1 = operand1;
                Operand2 = operand2;
                Operation = operation; 
            }
        }

    }
}
