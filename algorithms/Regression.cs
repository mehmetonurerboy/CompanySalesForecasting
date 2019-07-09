using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;


namespace salesPrediction.algorithms
{
    class Regression
    {
        public static double[] SimpleLinearRegression(string code, string targetDate,int numberOfData = -1, string column = null, bool isColDif = false)
        {
            List<BsonDocument> data;
            if(numberOfData == -1)
            {
                data = algorithmService.GetDataAll(code, targetDate);
                numberOfData = data.Count;
            }
            else
            {
                data = algorithmService.GetData(code, targetDate, numberOfData);
            }

            if (numberOfData > data.Count)
            {
                numberOfData = data.Count;
            }

            double[] xData;
            double[] result;

            if(column == null)
            {
                xData = new double[data.Count];
                for(int i = 0; i<data.Count; i++)
                {
                    xData[i] = i;
                }
            }
            else
            {
                if (isColDif)
                {
                    xData = algorithmService.GetDifferentiatedData(data, column);
                }
                else
                {
                    xData = algorithmService.convertBsonDocumentToDouble(data, column);
                }
            }


            result = calculateSimpleLinearRegression(data, xData, "#386-BRUT SATISLAR");

            return result;
        }

        public static double MultipleLinearRegression(string code, string column1, string column2, string targetDate, int numberOfData = -1, int percentage = 100)
        {
            List<BsonDocument> data;
            if (numberOfData == -1)
            {
                data = algorithmService.GetDataAll(code, targetDate);
                numberOfData = data.Count;
            }
            else
            {
                data = algorithmService.GetData(code, targetDate, numberOfData);
            }

            if(numberOfData > data.Count)
            {
                numberOfData = data.Count;
            }

            if (percentage > 100)
            {
                percentage = 100;
            }
            else if (percentage < 10)
            {
                percentage = 10;
            }

            double result = calculateMultipleLinearRegression(data, column1, column2, "#386-BRUT SATISLAR", percentage);
            return result;
        }

        private static double[] calculateSimpleLinearRegression(List<BsonDocument> data, double[] xData, string column)
        {
            double[] yData = algorithmService.GetDifferentiatedData(data, column);
            double indisAverage = calculateMean(xData);
            Console.WriteLine("average x : " + indisAverage);
            double valueAverage = calculateMean(yData);
            Console.WriteLine("average y : " + valueAverage);

            double coeff = 0.0;
            for(int i = 0; i < yData.Length; i++)
            {
                coeff += (xData[i] - indisAverage) * (yData[i] - valueAverage);
            }
            coeff /= Math.Pow(calculateStandardDeviation(xData, indisAverage), 2); 
            double constant = valueAverage - coeff * indisAverage;
            Console.WriteLine("coefficient : " + coeff);
            Console.WriteLine("constant value : " + constant);

            double[] result = new double[yData.Length];
            for(int i = 0; i<yData.Length; i++)
            {
                result[i] = constant + coeff * xData[i];
            }

            return result;
        }

        private static double calculateMultipleLinearRegression(List<BsonDocument> data, string IndCol1, string IndCol2, string Depcolumn, int percentage)
        {
            List<double[]> variables = new List<double[]>(3);
            variables.Add(getPercentagesDatas(algorithmService.GetDifferentiatedData(data, Depcolumn), percentage));
            
            variables.Add(getPercentagesDatas(algorithmService.GetDifferentiatedData(data, IndCol1), percentage));
            
            variables.Add(getPercentagesDatas(algorithmService.GetDifferentiatedData(data, IndCol2), percentage));

            double[] mean = new double[3];
            double[] std = new double[3];
            double[] correlation = new double[3];

            for (int i = 0; i < 3; i++)
            {
                mean[i] = calculateMean(variables[i]);
                std[i] = calculateStandardDeviation(variables[i], mean[i]);
            }

            correlation[0] = calculateCorrelation(variables[1], variables[2], mean[1], mean[2], std[1], std[2]);
            correlation[1] = calculateCorrelation(variables[0], variables[1], mean[0], mean[1], std[0], std[1]);
            correlation[2] = calculateCorrelation(variables[0], variables[2], mean[0], mean[2], std[0], std[2]);

            double Rvalue = calculateR(correlation);

            double[] independentCoeff = new double[2];
            independentCoeff[0] = calculateVariableCoefficient(correlation, std[1], std[0], 1);
            independentCoeff[1] = calculateVariableCoefficient(correlation, std[2], std[0], 2);

            double regressionCoeff = calculateRegressionCoefficient(mean, independentCoeff);

            double result;

            result = regressionCoeff + independentCoeff[0] * variables[1][variables[1].Length - 1] + independentCoeff[1] * variables[2][variables[2].Length - 1];
            
            return result;
        }

        private static double[] getPercentagesDatas(double[] data, int percentage)
        {
            int numberOfData = data.Length;
            int actualData = Convert.ToInt32(Math.Floor(Convert.ToDecimal(numberOfData * percentage / 100)));

            int difference = numberOfData - actualData;
            double[] result = new double[actualData];

            for(int i=0; i<actualData; i++)
            {
                result[i] = data[difference + i];
            }
            return result;
        }


        private static double calculateMean(double[] list)
        {
            double sum = 0.0;
            for(int i=0; i<list.Length; i++)
            {
                sum += list[i];
            }
            return sum / list.Length;
        }

        private static double calculateStandardDeviation(double[] list, double mean)
        {
            double sum = 0.0;
            for(int i=0; i<list.Length; i++)
            {
                sum += Math.Pow(list[i] - mean,2);
            }
            return Math.Sqrt(sum / (list.Length - 1));
        }

        private static double calculateCorrelation(double[] list1, double[] list2, double mean1, double mean2, double std1, double std2)
        {
            double sum = 0.0;
            for(int i=0; i<list1.Length; i++)
            {
                sum += ((list1[i] - mean1) * (list2[i] - mean2));
            }
            sum /= ((list1.Length-1) * std1 * std2);
            return sum;
        }

        private static double calculateR(double[] correlation)
        {
            double result = Math.Pow(correlation[1], 2) + Math.Pow(correlation[2], 2) - (2 * correlation[0] * correlation[1] * correlation[2]);
            result /= (1 - Math.Pow(correlation[0], 2));
            result = Math.Sqrt(result);
            return result;
        }

        private static double calculateVariableCoefficient(double[] correlation, double stdIndependent, double stdDependent, int indis)
        {
            double coeff = correlation[indis] - correlation[3-indis] * correlation[0];
            coeff /= (1 - Math.Pow(correlation[0], 2));
            coeff *= (stdDependent / stdIndependent);
            return coeff;
        }

        private static double calculateRegressionCoefficient(double[] means, double[] coeffIndependent)
        {
            double result = means[0] - coeffIndependent[0] * means[1] - coeffIndependent[1] * means[2];
            return result;
        }
    }
}
