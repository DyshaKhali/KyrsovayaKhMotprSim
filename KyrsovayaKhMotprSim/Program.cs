using System;
using System.Collections.Generic;

namespace KyrsovayaKhMotprSim
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Входная матрица
            //св.чл x1 x2 x3 x4 0 0
            //св.чл x1 x2 x3 0 x5 0
            //св.чл x1 x2 x3 0 0 x6
            //Целевая функция ввиде 0 +- x1 +- x2 +- x3 = 0 (*-1 - max)

            //double[,] table = new double[,] //Тест 1
            //{
            //    { 24, 2, 1, -2, 1, 0, 0 },
            //    { 22, 1, 2, 4, 0, 1, 0 },
            //    { 10, 1, -1, 2, 0, 0, -1 },
            //    { 0, -2, 3, -6, -1, 0, 0 }
            //};

            //double[,] table = new double[,] //Тест 2
            //{
            //    { 10, 1, -2, 1, 0, 0, 0 },
            //    { 18, -2, -1, 0, -2, -1, 0},
            //    { 36, 3, 2, 0, 1, 0, -1},
            //    { 0, 2, -1, 0, -1, 0, 0 }
            //};

            //double[,] table = new double[,] //Тест 3
            //{
            //    { 20, 1, 2, -4, 0, 1, 0 },
            //    { 10, 1, -1, 2, 0, 0, -1 },
            //    { 24, 2, 1, -2, 1, 0, 0 },
            //    { 0, -2, 3, -6, -1, 0, 0 }
            //};

            //double[,] table = new double[,] //Задача 1
            //{
            //    { 252200, 2.25, 1.2, 3.16, 6.24, 11.52, 1, 0, 0},
            //    { 60000000, 757, 468, 873, 1666, 3774, 0, 1, 0},
            //    { 50000000, 1620, 300, 1135, 2430, 4680, 0, 0, -1},
            //    { 0, -1400, -1500, -5900, -13100, -25000, 0, 0, 0}
            //};

            double[,] table = new double[,] //Задача 2
            {
                { 252200, 2.25, 1.2, 3.16, 6.24, 11.52, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //+x6
                { 60000000, 757, 468, 873, 1666, 3774, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //+x7
                { 50000000, 1620, 300, 1135, 2430, 4680, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0}, //+x8
                { 8000, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0}, //+x9
                { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0}, //+x10
                { 17000, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0}, //+x11
                { 12000, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0}, //+x12
                { 10700, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0}, //+x13
                { 17000, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0}, //+x14
                { 15000, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0}, //+x15
                { 7000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0}, //+x16
                { 6000, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1}, //+x17
                { 0, -1400, -1500, -5900, -13100, -25000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            };

            int rows = table.GetLength(0);
            int columns = table.GetLength(1);
            List<int> basis = new List<int>(new int[rows - 1]);

            //basis[0] = 6;//Задаем позиции базисных элементов
            //basis[1] = 7;
            //basis[2] = 0;

            basis[0] = 6;//Задаем позиции базисных элементов
            basis[1] = 7;
            basis[2] = 0;
            basis[3] = 9;
            basis[4] = 0;
            basis[5] = 0;
            basis[6] = 12;
            basis[7] = 0;
            basis[8] = 14;
            basis[9] = 0;
            basis[10] = 16;
            basis[11] = 0;


            double[] resultOptimal = new double[columns - 1];
            double[,] resultTable;

            Simplex S = new Simplex(table, basis);

            Console.Write("Это задача максимизации (y/n): ");
            S.typeTask = Console.ReadLine();

            if (S.ior)
            {
                resultTable = S.Calculate(resultOptimal);

                if (S.optiSol)
                {
                    Console.WriteLine("\nРешенная симплекс-таблица:");

                    for (int i = 0; i < resultTable.GetLength(0); i++)
                    {
                        for (int j = 0; j < resultTable.GetLength(1); j++)
                        {
                            Console.Write("|" + Math.Round(resultTable[i, j], 2) + "|");
                        }
                        Console.WriteLine("\n");
                    }

                    Console.WriteLine("\nРешение:");
                    for (int i = 0; i < columns - 1; i++)
                    {
                        Console.WriteLine($"X{i + 1} = {resultOptimal[i]}");
                    }
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("Оптимального решения не существует");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Решение не найдено");
                Console.ReadKey();
            }
        }
    }
}
