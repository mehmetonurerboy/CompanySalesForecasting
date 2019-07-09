using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace salesPrediction.algorithms
{
    class ExponentialSmoothing
    {
        public static double[] ES(string code, String targetDate, double smoothFactor = 0.5, int numberOfData = -1, int percentage = 100)
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

            if(percentage > 100)
            {
                percentage = 100;
            }
            else if(percentage < 10)
            {
                percentage = 10;
            }

            double[] result = calculateES(numberOfData, data, smoothFactor, percentage);
            return result;
        }

        private static double[] calculateES(int numberOfData, List<BsonDocument> data, double smoothFactor, int percentage)
        {
            double[] result;
            double[] pureData = algorithmService.GetDifferentiatedData(data, "#386-BRUT SATISLAR");

            int actualNumberOfData = Convert.ToInt32(Math.Floor(Convert.ToDecimal(numberOfData * percentage / 100)));
            
            result = new double[actualNumberOfData];

            int difference = numberOfData - actualNumberOfData;

            result[0] = pureData[difference];
            for(int i = 1; i<actualNumberOfData; i++)
            {
                result[i] = smoothFactor * pureData[difference + i-1] + (1 - smoothFactor) * result[i - 1];
            }


            return result;
        }

        
    }
}
