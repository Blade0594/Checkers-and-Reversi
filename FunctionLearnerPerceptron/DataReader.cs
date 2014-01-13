using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace FunctionLearnerPerceptron
{
    class DataReader
    {
        FileStream file;
        StreamReader reader;
        public DataReader(string path)
        {
            try
            {
                file = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            reader = new StreamReader(file);
            
        }
        virtual public Individual GetPerceptronWeights(int featureCount)
        {
            double[] weights = new double[featureCount];
            string[] split = reader.ReadLine().Split(' ');
            for (int index = 0; index < split.Length; index++)
                weights[index] = double.Parse(split[index]);
            return new Individual(weights,"Perceptron",0);
        }
        virtual public List<Individual> GetPopulation(int featureCount)
        {
            string line = reader.ReadLine();
            string []split = line.Split(' ');
            List<double[]> weightsList = new List<double[]>();
            while (line != null)
            {
                split = line.Split(' ');
                double[] weights = new double[featureCount];
                for (int index = 0; index < split.Length; index++)
                    weights[index] = double.Parse(split[index]);
                weights[weights.Length - 1] = 1;
                weightsList.Add(weights);
                line = reader.ReadLine();
              
            }
            List<Individual> newIndivs= new List<Individual>();
            foreach(var elem in weightsList)
                newIndivs.Add(new Individual(elem,string.Empty,weightsList.Count));
            return newIndivs;
        
        }
    }
}
