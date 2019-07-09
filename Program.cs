using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using salesPrediction.algorithms;
using salesPrediction.View;

namespace salesPrediction
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoDBService.InitiateService("mongodb://localhost:27017", "project", "aksaDeneme");
            
            RunFormApplication();
            //RunConsoleApplication();
        }

        private static void RunConsoleApplication()
        {
            int selection;
            Console.WriteLine("(1)Exponential Smoothing Method\n(2)Moving Average Method\n(3)Simple Linear Regression Method\n(4)Multiple Regression\n(5)Price Annual Average Method\n(6)Fault Test\n(7)Performance Test\n(8)Quit");
            Console.WriteLine("Hangisini seçmek istiyorsan onun numarasını giriniz : ");
            selection = Int32.Parse(Console.ReadLine());

            string targetDate;
            int dataPercentage;
            int movingAveragePeriod;
            double exponentialSmoothingFactor;
            var codeList = algorithmService.GetCodeList();

            string[] dates = {"2009-12","2010-03","2010-06","2010-09","2011-03",
                                "2011-06","2011-09","2011-12","2012-03","2012-09",
                                "2012-12","2013-03","2013-06","2013-09","2013-12",
                                "2014-06","2014-09","2014-12","2015-03","2015-06",
                                "2015-12","2016-03","2016-06","2016-09","2016-12",
                                "2017-06","2017-09","2017-12","2018-03","2018-06"};

            //string[] dates = 

            while (selection != 8)
            {
                List<BsonDocument> data;
                double[] realData;

                if (selection == 1)
                {
                    Console.WriteLine("Enter the year value : ");
                    targetDate = Console.ReadLine();

                    data = algorithmService.GetDataAll("EREGL", targetDate);
                    realData = algorithmService.GetDifferentiatedData(data, "#386-BRUT SATISLAR");

                    Console.WriteLine("Enter factor of exponential smoothing : ");
                    try {
                        exponentialSmoothingFactor = Convert.ToDouble(Console.ReadLine());
                    }
                    catch
                    {
                        Console.WriteLine("Exponential Smoothing factor must be entered as double!");
                    }

                    double[] expSmoot = ExponentialSmoothing.ES("EREGL", targetDate, 0.9, -1, 100);
                    Console.WriteLine("Hedef Değer : " + realData[realData.Length - 1]);
                    Console.WriteLine("Bulunan Değer : " + expSmoot[expSmoot.Length - 1]);
                    Console.WriteLine("Fark : " + (realData[realData.Length - 1] - expSmoot[expSmoot.Length - 1]));
                    Console.WriteLine("Fark(%) : " + (realData[realData.Length - 1] - expSmoot[expSmoot.Length - 1]) / realData[realData.Length - 1]);
                    Console.WriteLine("\n\n\n");

                }


                else if (selection == 2)
                {
                    Console.WriteLine("Enter the year value : ");
                    targetDate = Console.ReadLine();

                    data = algorithmService.GetDataAll("EREGL", targetDate);
                    realData = algorithmService.GetDifferentiatedData(data, "#386-BRUT SATISLAR");

                    double[] movAve = MovingAverage.MA("EREGL", targetDate, 3, -1, 100);
                    Console.WriteLine("Hedef Değer : " + realData[realData.Length - 1]);
                    Console.WriteLine("Bulunan Değer : " + movAve[movAve.Length - 1]);
                    Console.WriteLine("Fark : " + (realData[realData.Length - 1] - movAve[movAve.Length - 1]));
                    Console.WriteLine("Fark(%) : " + (realData[realData.Length - 1] - movAve[movAve.Length - 1]) / realData[realData.Length - 1]);
                    Console.WriteLine("\n\n\n");

                }

                else if (selection == 3)
                {
                    List<BsonDocument> dataSLR = algorithmService.GetDataAll("EREGL", "2009-12");
                    double[] pureDataSLR = algorithmService.GetDifferentiatedData(dataSLR, "#386-BRUT SATISLAR");

                    double[] slr = Regression.SimpleLinearRegression("EREGL", "2009-12");
                    Console.WriteLine("\n\nreal data | simple linear regression data | change | change rate \n\n");
                    for (int i = 0; i < slr.Length; i++)
                    {
                        Console.WriteLine(pureDataSLR[i] + " | " + slr[i] + " | " + (pureDataSLR[i] - slr[i]) + " | " + (pureDataSLR[i] - slr[i]) / pureDataSLR[i]);
                    }



                    List<BsonDocument> dataSLR20 = algorithmService.GetData("EREGL", "2009-12", 20);
                    double[] pureDataSLR20 = algorithmService.GetDifferentiatedData(dataSLR20, "#386-BRUT SATISLAR");

                    double[] slr20 = Regression.SimpleLinearRegression("EREGL", "2009-12", 20);
                    Console.WriteLine("\n\nreal data | simple linear regression data | change | change rate \n\n");
                    for (int i = 0; i < slr20.Length; i++)
                    {
                        Console.WriteLine(pureDataSLR20[i] + " | " + slr20[i] + " | " + (pureDataSLR20[i] - slr20[i]) + " | " + (pureDataSLR20[i] - slr20[i]) / pureDataSLR20[i]);
                    }

                    List<BsonDocument> dataSLR10 = algorithmService.GetData("EREGL", "2009-12", 11);
                    double[] pureDataSLR10 = algorithmService.GetDifferentiatedData(dataSLR20, "#386-BRUT SATISLAR");

                    double[] slr10 = Regression.SimpleLinearRegression("EREGL", "2009-12", 11);
                    Console.WriteLine("\n\nreal data | simple linear regression data | change | change rate \n\n");
                    for (int i = 0; i < slr10.Length; i++)
                    {
                        Console.WriteLine(pureDataSLR10[i] + " | " + slr10[i] + " | " + (pureDataSLR10[i] - slr10[i]) + " | " + (pureDataSLR10[i] - slr10[i]) / pureDataSLR10[i]);
                    }

                    double[] slr20_1 = Regression.SimpleLinearRegression("EREGL", "2009-12", 20, "395-SATIS GELIRLERI", true);
                    Console.WriteLine("\n\nreal data | simple linear regression data | change | change rate \n\n");
                    for (int i = 0; i < slr20_1.Length; i++)
                    {
                        Console.WriteLine(pureDataSLR20[i] + " | " + slr20_1[i] + " | " + (pureDataSLR20[i] - slr20_1[i]) + " | " + (pureDataSLR20[i] - slr20_1[i]) / pureDataSLR20[i]);
                    }

                    double[] slr10_1 = Regression.SimpleLinearRegression("EREGL", "2009-12", 11, "395-SATIS GELIRLERI", true);
                    Console.WriteLine("\n\nreal data | simple linear regression data | change | change rate \n\n");
                    for (int i = 0; i < slr10_1.Length; i++)
                    {
                        Console.WriteLine(pureDataSLR10[i] + " | " + slr10_1[i] + " | " + (pureDataSLR10[i] - slr10_1[i]) + " | " + (pureDataSLR10[i] - slr10_1[i]) / pureDataSLR10[i]);
                    }

                }
                else if (selection == 4)
                {
                    Console.WriteLine("Enter the year value : ");
                    targetDate = Console.ReadLine();

                    data = algorithmService.GetDataAll("EREGL", targetDate);
                    realData = algorithmService.GetDifferentiatedData(data, "#386-BRUT SATISLAR");

                    double multReg = Regression.MultipleLinearRegression("EREGL", "165-TOPLAM AKTIFLER", "395-SATIS GELIRLERI", targetDate, -1, 100);
                    Console.WriteLine("Hedef Değer : " + realData[realData.Length - 1]);
                    Console.WriteLine("Bulunan Değer : " + multReg);
                    Console.WriteLine("Fark : " + (realData[realData.Length - 1] - multReg));
                    Console.WriteLine("Fark(%) : " + (realData[realData.Length - 1] - multReg) / realData[realData.Length - 1]);
                    Console.WriteLine("\n\n\n");

                }
                else if (selection == 5)
                {
                    Console.WriteLine("Enter the year value : ");
                    targetDate = Console.ReadLine();

                    data = algorithmService.GetDataAll("EREGL", targetDate);
                    realData = algorithmService.GetDifferentiatedData(data, "#386-BRUT SATISLAR");

                    double priceAnn = PriceAnnualAverage.PAA("EREGL", targetDate, 100);
                    Console.WriteLine("Hedef Değer : " + realData[realData.Length - 1]);
                    Console.WriteLine("Bulunan Değer : " + priceAnn);
                    Console.WriteLine("Fark : " + (realData[realData.Length - 1] - priceAnn));
                    Console.WriteLine("Fark(%) : " + (realData[realData.Length - 1] - priceAnn) / realData[realData.Length - 1]);
                    Console.WriteLine("\n\n\n");

                }
                else if (selection == 6)
                {
                    double[] result;
                    double finalVal;

                    for (int i = 0; i < dates.Length; i++)
                    {
                        double[] targetValue = algorithmService.GetDifferentiatedData(algorithmService.GetDataAll("EREGL", dates[i]), "#386-BRUT SATISLAR");
                        Console.WriteLine("----------------------" + "\t" + dates[i] + "\t" + "----------------------\n\n");
                        Console.WriteLine("Exponential Smoothing : ");
                        result = ExponentialSmoothing.ES("EREGL", dates[i], 0.9, -1, 100);
                        Console.WriteLine("Hedef Değer : " + targetValue[targetValue.Length - 1]);
                        Console.WriteLine("Bulunan Değer : " + result[result.Length - 1]);
                        Console.WriteLine("Fark : " + (targetValue[targetValue.Length - 1] - result[result.Length - 1]));
                        Console.WriteLine("Fark(%) : " + (targetValue[targetValue.Length - 1] - result[result.Length - 1]) / targetValue[targetValue.Length - 1]);
                        Console.WriteLine("\n\n\n");

                        Console.WriteLine("Moving Average : ");
                        result = MovingAverage.MA("EREGL", dates[i], 3, -1);
                        Console.WriteLine("Hedef Değer : " + targetValue[targetValue.Length - 1]);
                        Console.WriteLine("Bulunan Değer : " + result[result.Length - 1]);
                        Console.WriteLine("Fark : " + (targetValue[targetValue.Length - 1] - result[result.Length - 1]));
                        Console.WriteLine("Fark(%) : " + (targetValue[targetValue.Length - 1] - result[result.Length - 1]) / targetValue[targetValue.Length - 1]);
                        Console.WriteLine("\n\n\n");

                        Console.WriteLine("Multiple Regression : ");
                        finalVal = Regression.MultipleLinearRegression("EREGL", "165-TOPLAM AKTIFLER", "395-SATIS GELIRLERI", dates[i], -1);
                        Console.WriteLine("Hedef Değer : " + targetValue[targetValue.Length - 1]);
                        Console.WriteLine("Bulunan Değer : " + finalVal);
                        Console.WriteLine("Fark : " + (targetValue[targetValue.Length - 1] - finalVal));
                        Console.WriteLine("Fark(%) : " + (targetValue[targetValue.Length - 1] - finalVal) / targetValue[targetValue.Length - 1]);
                        Console.WriteLine("\n\n\n");

                        Console.WriteLine("Price Annual Average Method : ");
                        finalVal = PriceAnnualAverage.PAA("EREGL", dates[i], -1);
                        Console.WriteLine("Hedef Değer : " + targetValue[targetValue.Length - 1]);
                        Console.WriteLine("Bulunan Değer : " + finalVal);
                        Console.WriteLine("Fark : " + (targetValue[targetValue.Length - 1] - finalVal));
                        Console.WriteLine("Fark(%) : " + (targetValue[targetValue.Length - 1] - finalVal) / targetValue[targetValue.Length - 1]);
                        Console.WriteLine("\n\n\n");
                    }
                }
                else if (selection == 7)
                {
                    
                    double[] result;
                    double finalVal;

                    for (int i = 0; i < dates.Length; i++)
                    {
                        Console.WriteLine("----------------------" + "\t" + dates[i] + "\t" + "----------------------\n\n");
                        Console.Write("Moving Average : ");
                        var sw = Stopwatch.StartNew();
                        result = MovingAverage.MA("EREGL", dates[i], 3, -1);
                        long durations = sw.ElapsedMilliseconds;
                        Console.WriteLine(durations + "\n\n");

                        Console.Write("Exponential Smoothing : ");
                        sw = Stopwatch.StartNew();
                        result = ExponentialSmoothing.ES("EREGL", dates[i], 0.9, -1);
                        durations = sw.ElapsedMilliseconds;
                        Console.WriteLine(durations + "\n\n");

                        Console.Write("Multiple Regression : ");
                        sw = Stopwatch.StartNew();
                        finalVal = Regression.MultipleLinearRegression("EREGL", "165-TOPLAM AKTIFLER", "395-SATIS GELIRLERI", dates[i], -1);
                        durations = sw.ElapsedMilliseconds;
                        Console.WriteLine(durations + "\n\n");

                        Console.Write("Price Annual Average : ");
                        sw = Stopwatch.StartNew();
                        finalVal = PriceAnnualAverage.PAA("EREGL", dates[i], -1);
                        durations = sw.ElapsedMilliseconds;
                        Console.WriteLine(durations + "\n\n");
                    }


                }


                Console.WriteLine("Girdiğiniz sayı hatalı! Tekrar giriniz : ");
                selection = Int32.Parse(Console.ReadLine());

            }
        }

        private static void RunFormApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new View.View());
        
        }
        
    }
}

