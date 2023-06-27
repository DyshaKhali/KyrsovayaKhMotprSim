using System;
using System.Collections.Generic;

namespace KyrsovayaKhMotprSim
{
    public class Simplex
    {
        double[,] table; //симплекс таблица

        int rows, columns;

        List<int> basis; //список базисных переменных

        public bool ior = false;

        public bool optiSol = true;

        public string typeTask = "n";

        public Simplex(double[,] source, List<int> sourceBasis) //source - симплекс таблица без базисных переменных
        {
            rows = source.GetLength(0); // Количество строк
            columns = source.GetLength(1); // Количество столбцов
            table = new double[rows, columns];
            basis = new List<int>(new int[rows - 1]);

            table = source;
            basis = sourceBasis;

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("");
            }
            if (IOR())
            {
                Console.WriteLine("ИОР найдено\n");
                for (int i = 0; i < table.GetLength(0); i++)
                {
                    for (int j = 0; j < table.GetLength(1); j++)
                    {
                        Console.Write("|" + Math.Round(table[i, j], 3) + "|");
                    }
                    Console.WriteLine("\n");
                }
                ior = true;
            }
        }

        private bool IOR()
        {
            //индексы строки и столбца для выбора первого ненулевого элемента в строке, не разрешенной относительно базисной переменной. Значения -1 используются для обозначения неприсвоенных значений.
            int row = 0;
            int column;
            //1.Цикл для поиска первого ненулевого положительного элемента в строке, не разрешенной относительно базисной переменной
            for (int i = 0; i < table.GetLength(0) - 1; i++)
            {
                if (basis[i] != 0 && table[i, 0] > 0)//переменная уже является базисной переменной, и переходим к следующей итерации цикла
                {
                    continue;
                }
                for (int j = 1; j < table.GetLength(1); j++)//проходим через каждый элемент в строке(кроме свободных членов) и ищем первый ненулевой положительный элемент.
                {
                    if (table[i, j] != 0) //Выбираем не нулевой элемент
                    {
                        row = i;
                        column = j;
                        basis[i] = j;
                        PivotCalc(row, column); // Применяем операцию выбора опорного элемента (пересчет симплекс-таблицы)
                        break;
                    }
                }
            }
            int minBIndex;// индекс базисной строки, соответствующей наименьшему отрицательному значению в столбце свободных членов
            while (OptSolByOtchenSrt()) //2.Цикл выполняет улучшение опорного решения до тех пор, пока условие оптимальности не будет выполнено (все св.чл. ограничений не отрицательны)
            { //3.Если после приведения системы ограничений к станд.виду некоторые из B оказались отрицательными
                minBIndex = 0;
                for (int i = 0; i < table.GetLength(0) - 1; i++) // 3.1 Найдем среди свободных членов ограничений наибольший по абсолютной величине отрицательный свободный член
                {
                    if (table[i, 0] < minBIndex)
                    {
                        minBIndex = i;
                    }
                }

                basis[minBIndex] = 0;
                MinusStrB(minBIndex); //3.2 - 3.3 Выполняем пересчет
                //4. Находим опорный столбец, путем поиска первого положительного элемента в базисной строке
                column = -1;
                //Находим опорный столбец column путем поиска первого положительного элемента в базисной строке
                for (int j = 1; j < table.GetLength(1); j++)
                {
                    if (table[minBIndex, j] > 0)
                    {
                        column = j;
                        break;
                    }
                }
                if (column == -1)//если опор столбец не найден, т.е. нет полож.
                {
                    Console.WriteLine("Нет ИОР");
                    return false;
                }

                row = findMainrow(column); //5. Находим опорную строку по симплекс методу

                basis[row] = column;//Обновляем информацию о базисной переменной
                PivotCalc(row, column); // Применяем операцию выбора опорного элемента(пересчет)
            }
            return true;
        }

        private void PivotCalc(int pivotrow, int pivotcolumnumn)//выполняет операцию выбора ведущего элемента (пересчет симплекс-таблицы). Он приводит значение опорного элемента к 1 и обновляет остальные элементы в соответствии с правилами симплекс-метода
        {
            double pivotValue = table[pivotrow, pivotcolumnumn];

            // Делим элементы опорной строки на значение опорного элемента (приводим его к 1)
            for (int j = 0; j < columns; j++)
            {
                table[pivotrow, j] /= pivotValue;
            }

            // Выполняем операции преобразования строк
            for (int i = 0; i < rows; i++)
            {
                // Пропускаем опорную строку
                if (i != pivotrow)
                {
                    double factor = table[i, pivotcolumnumn];

                    // Обновляем элементы вне опорной строки путем вычитания произведения фактора и соответствующих элементов опорной строки
                    for (int j = 0; j < columns; j++)
                    {
                        table[i, j] -= factor * table[pivotrow, j];
                    }
                }
            }
        }

        private bool OptSolByOtchenSrt()
        {
            // Проверяем условие оптимальности
            // Если в оценоч строке есть отрицательные эл, столбцы которых содержат полож коэфф-ты.решение не опт, и может быть улучшено за счет ввода одной изсвободных переменных в базис
            for (int i = 0; i < table.GetLength(0) - 1; i++)
            {
                if (table[i, 0] < 0)
                {
                    return true;
                }
            }
            return false;
        }

        public double[,] Calculate(double[] result) //result - в этот массив будут записаны полученные значения X
        {
            int maincolumn, mainrow; //ведущие столбец и строка

            while (!IsItEnd() && optiSol)
            {
                maincolumn = findMaincolumn(); //Ищем разрешающий столбец
                mainrow = findMainrow(maincolumn); //Ищем разрешающую строку
                basis[mainrow] = maincolumn; //Записываем введенную базисную переменную

                double[,] newTable = new double[rows, columns]; //Составляем новую таблицу

                for (int j = 0; j < columns; j++)
                {
                    newTable[mainrow, j] = table[mainrow, j] / table[mainrow, maincolumn]; //Делим все элементы разрешающей строки на разрешающий элемент
                }

                for (int i = 0; i < rows; i++)
                {
                    if (i == mainrow) //Скипаем разрешающую строку
                    {
                        continue;
                    }

                    for (int j = 0; j < columns; j++) //Пересчитываем элементы
                    {
                        newTable[i, j] = table[i, j] - table[i, maincolumn] * newTable[mainrow, j];
                    }
                }
                table = newTable;
            }

            //заносим в result найденные значения X
            for (int i = 0; i < result.Length; i++)
            {
                int k = basis.IndexOf(i + 1);
                if (k != -1)
                    result[i] = table[k, 0];
                else
                    result[i] = 0;
            }

            return table;
        }

        private void MinusStrB(int _MinB_Index)
        {
            // Выполняем вычитание базисной строки из остальных строк и инвертируем базисную строку

            for (int i = 0; i < table.GetLength(0) - 1; i++)//3.2 Вычтем почленно s-ое уравн из всех уравнений с отриц-ыми свободными членами
            {
                if (table[i, 0] >= 0 || i == _MinB_Index)//Если значение свободного члена неотрицательное или строка совпадает с мин.баз.св.ч,след итерация цикла.
                {
                    continue;
                }
                for (int j = 1; j < table.GetLength(1); j++)//проходим через каждый элемент столбца начиная с индекса 1 и вычитаем соответствующее значение из строки
                {
                    table[i, j] = table[i, j] - table[_MinB_Index, j];
                }
            }//3.3 Умножим элементы s-го уравнения на -1,чтобы св чл стал положительным
            for (int j = 0; j < table.GetLength(1); j++) //Проходим через каждый элемент столбца симплекс-таблицы для строки с мин.баз.св.ч
            {
                table[_MinB_Index, j] = table[_MinB_Index, j] * -1; //Умножаем каждый элемент на -1, чтобы изменить знак элемента
            }
        }

        private bool IsItEnd() //Проверка на оптимальность
        {
            bool flag = true;
            bool tempCheckPlusKoef = false; //Переменная для проверки столбца с положительной оценкой на отрицательные коэффициенты в столбце
            bool flagPlus = false;
            bool flagOtr = false;

            if (typeTask == "n")
            {

                for (int j = 1; j < columns; j++)
                {
                    if (table[rows - 1, j] < 0) //Проверка оценок на отрацательность
                    {
                        flag = false;
                    }

                    if (table[rows - 1, j] > 0)
                    {
                        flagPlus = true;
                        for (int i = 0; i < rows - 1; i++)
                        {
                            if (table[i, j] > 0)
                            {
                                tempCheckPlusKoef = true;
                                break;
                            }
                        }
                    }
                }

                if (!tempCheckPlusKoef && flagPlus)
                {
                    optiSol = false;
                }

                return flag;
            }
            else
            {
                for (int j = 1; j < columns; j++)
                {
                    if (table[rows - 1, j] < 0) //Проверка оценок на отрацательность
                    {
                        flag = false;
                        flagOtr = true;
                        for (int i = 0; i < rows - 1; i++)
                        {
                            if (table[i, j] > 0)
                            {
                                tempCheckPlusKoef = true;
                                break;
                            }
                        }
                    }
                }

                if (!tempCheckPlusKoef && flagOtr)
                {
                    optiSol = false;
                }

                return flag;
            }
        }

        private int findMaincolumn() //Из свободных переменных выбираем ту, оценка которой наименьшая
        {
            int maincolumn = 1;

            for (int j = 2; j < columns; j++)
            {
                if (table[rows - 1, j] < table[rows - 1, maincolumn])
                {
                    maincolumn = j;
                }
            }
            return maincolumn;
        }

        private int findMainrow(int maincolumn)
        {
            int mainrow = 0;

            for (int i = 0; i < rows - 1; i++)
            {
                if (table[i, maincolumn] > 0)
                {
                    mainrow = i;
                    break;
                }
            }

            for (int i = mainrow + 1; i < rows - 1; i++)
            {
                if ((table[i, maincolumn] > 0) && ((table[i, 0] / table[i, maincolumn]) < (table[mainrow, 0] / table[mainrow, maincolumn])))
                {
                    mainrow = i;
                }
            }
            return mainrow;
        }
    }
}
