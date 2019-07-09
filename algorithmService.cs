using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;

namespace salesPrediction
{
    class algorithmService
    {
        private static BsonDocument sort = new BsonDocument("1-Period", -1);

        private algorithmService() { }

        public static List<BsonDocument> GetCodeList()
        {
            var collection = MongoDBService.GetService().GetCollection();

            PipelineDefinition<BsonDocument, BsonDocument> pipeline = new BsonDocument[]
            {
                new BsonDocument("$group", new BsonDocument("_id", "$2-Kod"))
            };

            List<BsonDocument> codeList = null;
            try
            {
                codeList = collection.Aggregate(pipeline).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Thread.Sleep(5000);
                Environment.Exit(1);
            }

            return codeList;
        }

        public static int DataCount(string code, string targetDate)
        {
            return MongoDBService.GetService().Count(new BsonDocument { { "2-Kod", code }, { "1-Period", targetDate } });
        }

        public static List<BsonDocument> GetData(string code, string targetDate)
        {
            BsonDocument filter = new BsonDocument { { "2-Kod", code }, { "1-Period", targetDate } };
            return MongoDBService.GetService().FindManySort(filter, sort);
        }

        public static List<BsonDocument> GetDataAll(string code, string targetDate)
        {
            BsonDocument filter = new BsonDocument { { "2-Kod", code }, { "1-Period", new BsonDocument("$lte", targetDate) } };
            return MongoDBService.GetService().FindManySort(filter, sort);
        }

        public static List<BsonDocument> GetData(string code, string targetDate, int limit)
        {
            BsonDocument filter = new BsonDocument { { "2-Kod", code }, { "1-Period", new BsonDocument("$lte", targetDate) } };
            return MongoDBService.GetService().FindManySortLimit(filter, sort, limit);
        }

        public static List<BsonDocument> GetData(string code, string targetDate, string projectionField)
        {
            BsonDocument filter = new BsonDocument { { "2-Kod", code }, { "1-Period", new BsonDocument("$lte", targetDate) } };
            BsonDocument projection = new BsonDocument { { "_id", 0 }, { projectionField, 1 } };
            return MongoDBService.GetService().FindManySortProject(filter, sort, projection);
        }

        public static List<BsonDocument> GetData(string code, string targetDate, string[] projectionFields)
        {
            BsonDocument filter = new BsonDocument { { "2-Kod", code }, { "1-Period", new BsonDocument("$lte", targetDate) } };
            BsonDocument projection = new BsonDocument("_id", 0);
            foreach (string field in projectionFields) projection.Add(new BsonElement(field, 1));
            return MongoDBService.GetService().FindManySortProject(filter, sort, projection);
        }

        public static List<BsonDocument> GetData(string code, string targetDate, string projectionField, int limit)
        {
            BsonDocument filter = new BsonDocument { { "2-Kod", code }, { "1-Period", new BsonDocument("$lte", targetDate) } };
            BsonDocument projection = new BsonDocument { { "_id", 0 }, { projectionField, 1 } };
            return MongoDBService.GetService().FindManySortProjectLimit(filter, sort, projection, limit);
        }

        public static List<BsonDocument> GetData(string code, string targetDate, string[] projectionFields, int limit)
        {
            BsonDocument filter = new BsonDocument { { "2-Kod", code }, { "1-Period", new BsonDocument("$lte", targetDate) } };
            BsonDocument projection = new BsonDocument("_id", 0);
            foreach (string field in projectionFields) projection.Add(new BsonElement(field, 1));
            return MongoDBService.GetService().FindManySortProjectLimit(filter, sort, projection, limit);
        }

        public static double[] GetDifferentiatedData(List<BsonDocument> data, string column)
        {
            double[] result;
            result = new double[data.Count];

            for (int i = 1; i < data.Count; i++)
            {
                if (data.ElementAt(i - 1).GetElement(column).Value.ToDouble() - data.ElementAt(i).GetElement(column).Value.ToDouble() > 0)
                {
                    if(data.ElementAt(i).GetElement(column).Value.ToDouble() == 0)
                    {
                        result[data.Count - i] = getReasonableValue(data.ElementAt(i - 1).GetElement("1-Period").Value.ToString(), data.ElementAt(i - 1).GetElement(column).Value.ToDouble());
                    }
                    else
                    {
                        result[data.Count - i] = data.ElementAt(i - 1).GetElement(column).Value.ToDouble() - data.ElementAt(i).GetElement(column).Value.ToDouble();
                    }
                }
                else
                {
                    result[data.Count - i] = data.ElementAt(i - 1).GetElement(column).Value.ToDouble();
                }
            }
            result[0] = getReasonableValue(data.ElementAt(data.Count - 1).GetElement("1-Period").Value.ToString(), data.ElementAt(data.Count - 1).GetElement(column).Value.ToDouble());
            
            for(int i = 0; i<result.Length; i++)
            {
                if (result[i] == 0.0)
                {
                    if (i > 0)
                    {
                        result[i] = result[i - 1];
                    }
                    else
                    {
                        result[i] = result[i + 1];
                    }
                }
            }
            return result;
        }

        public static double getReasonableValue(string period, double value)
        {
            string quarter = period.Split('-').Last();
            //Console.WriteLine("period : " + period + "\tquarter: " + quarter + "\tvalue\n\n" + value);
            if(string.Compare(quarter,"12") == 0)
            {
                return value / 4;
            }
            else if(string.Compare(quarter,"09") == 0)
            {
                return value / 3;
            }
            else if(string.Compare(quarter,"06") == 0) 
            {
                return value / 2;
            }
            else
            {
                return value;
            }
        }

        public static double[] convertBsonDocumentToDouble(List<BsonDocument> data, string column)
        {
            double[] result = new double[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                result[data.Count - i - 1] = data.ElementAt(i).GetElement(column).Value.ToDouble();
            }
            return result;
        }
    }
}
