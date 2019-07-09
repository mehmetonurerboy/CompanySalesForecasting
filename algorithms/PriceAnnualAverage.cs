using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace salesPrediction.algorithms
{
    class PriceAnnualAverage
    {
        public static double PAA(string code, string targetDate, int numberOfData = -1, int percentage = 100)
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

            if (numberOfData > data.Count)
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

            double ave;
            double[] pureData = algorithmService.GetDifferentiatedData(data, "#386-BRUT SATISLAR");

            double[] actualData = getPercentagesDatas(pureData, percentage);
 
            if(numberOfData >= 2)
            {
                
                ave = alternativeCalculation_2(actualData, numberOfData);

                return (pureData[pureData.Length - 2] * (1 + ave));

            }
            else
            {
                Console.WriteLine("Seçilen eleman sayııs 8'den küçük olamaz!");
                return 0;
            }


            
        }

        private static double alternativeCalculation_2(double[] data, int numberofData)
        {
            double totalDifferPercentage = 0;
            
            int count = 0;
            
            for(int i=0; i<data.Length-2; i++)
            {
                if(data[i] != 0)
                {
                    totalDifferPercentage += ((data[i + 1] - data[i]) / data[i]);
                }
                count++;
            }
            return totalDifferPercentage / (numberofData - 2);
        }

        private static double getTotal(double[] data, int indis, int numberOfData)
        {
            double value = 0;
            for(int i=0; i<numberOfData; i++)
            {
                value += data[indis + i];
            }
            return value;
        }

        private static double[] getPercentagesDatas(double[] data, int percentage)
        {
            int numberOfData = data.Length;
            int actualData = Convert.ToInt32(Math.Floor(Convert.ToDecimal(numberOfData * percentage / 100)));

            int difference = numberOfData - actualData;
            double[] result = new double[actualData];

            for (int i = 0; i < actualData; i++)
            {
                result[i] = data[difference + i];
            }
            return result;
        }
    }
}
