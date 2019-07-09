using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace salesPrediction.algorithms
{
    class MovingAverage
    {
        public static double[] MA(string code, string targetDate, int period = 3, int numberOfData = -1, int percentage = 100)
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

            double[] result = calculateMovingAverage(data, numberOfData, period, percentage);

            return result;
        }

        private static double[] calculateMovingAverage(List<BsonDocument> data, int numberOfData, int period, int percentage)
        {
            int actualNumberOfData = Convert.ToInt32(Math.Floor(Convert.ToDecimal(numberOfData * percentage / 100)));

            double[] result;
            result = new double[actualNumberOfData - period];
            double[] pureData;
            pureData = new double[data.Count];
            pureData = algorithmService.GetDifferentiatedData(data, "#386-BRUT SATISLAR");

            int difference = numberOfData - actualNumberOfData;

            double sum = 0;
            for(int i = 0; i<period; i++)
            {
                sum += pureData[difference + i];
            }
            result[0] = sum / period;
            for(int i = period; i<actualNumberOfData; i++)
            {

                sum += pureData[i + difference];
                sum -= pureData[i - period + difference];
                result[i-period] = sum / period;
                
            }
            return result;
        }
    }
}
