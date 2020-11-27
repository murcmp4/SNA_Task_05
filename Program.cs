using System;
using System.IO;

namespace SNA_Task_05
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" ЛАБИРИНТ");
            Console.WriteLine("\n Press Enter");
            Console.ReadKey();
            Console.Clear();
            //Правила игры
            Console.WriteLine(" Правила игры:" +
                            "\n\n Двигайте персонажа на клавиши стрелок" +
                            "\n Клавиша ESC - выход из программы" +
                            "\n Доступные пути на карте отображаются символом #" +
                            "\n Стены - *, враги - !, ваш герой - @" +
                            "\n Ваш персонаж оставляет за собой след - S" +
                            "\n Если вы попытаетесь повернуть назад, то проиграете" +
                            "\n Если вы потеряете всё ХП, то проиграете" +
                            "\n Необходимо дойти до финиша (символ %)" +
                            "\n\nPress Enter");

            Console.ReadKey();
            Console.Clear();

            Map map = new Map("map");

            //Цикл выполнения игры
            while (map.Init()) ;

            Console.Clear();
            Console.WriteLine("Вы проиграли! Попробуйте еще раз");

            Console.ReadKey();
        }
    }
}
public class Map
{
    //Конструктор с параметрами
    public Map(string mapName)
    {
        // Считываем карту из файла
        ReadMap(mapName);
        //Задаём изначальное здоровье
        m_HP = 10;

        //Рисуем в самом начале карту и ХП-бар
        DrawMap();
        DrawBar();

        //Получаем положение ХП-бара в консоли
        m_barX = Console.CursorLeft + 10;
        m_barY = Console.CursorTop - 1;
    }

    public bool Init()
    {

        //Если у персонажа закончилось здоровье
        if (m_HP <= 0)
        {
            return false;
        }

        //Если персонаж встал на символ % (выход).
        if (m_map[m_performerY, m_performerX] == '%')
        {
            return false;
        }

        // Выполняем передвижение персонажа.
        Move();

        return true;
    }

    //Метод чтения карты из файла
    private void ReadMap(string mapName)
    {
        m_performerX = 0;
        m_performerY = 0;

        string[] newFile = File.ReadAllLines($"maps/{mapName}.txt");
        m_map = new char[newFile.Length, newFile[0].Length];

        for (int i = 0; i < m_map.GetLength(0); i++)
        {
            for (int j = 0; j < m_map.GetLength(1); j++)
            {
                m_map[i, j] = newFile[i][j];

                //@ - символ персонажа
                if (m_map[i, j] == '@')
                {
                    //Задаём начальные позиции
                    m_performerX = i;
                    m_performerY = j;
                }
            }
        }
    }

    //Метод рисования карты в консоли
    private void DrawMap()
    {
        for (int i = 0; i < m_map.GetLength(0); i++)
        {
            for (int j = 0; j < m_map.GetLength(1); j++)
            {
                Console.Write(m_map[i, j]);
            }
            Console.WriteLine();
        }
    }

    //Метод рисования жизни в консоли
    private void DrawBar()
    {
        Console.Write(">>HP<<" +
            "\n[");

        for (byte i = 0; i < m_HP; i++)
        {
            Console.Write('#');
        }

        Console.WriteLine(']');
    }

    //Проверка на нахождение персонажа на финише
    private void CheckExit(int Y, int X)
    {
        if (m_map[Y, X] == '%')
        {
            Console.WriteLine(" You win! ");
            Console.ReadKey();
            System.Environment.Exit(0);
        }
    }

    //Проверка наличия врага на координатах
    private void CheckEnemy(int Y, int X)
    {
        //Если на положении, куда двигается персонаж, стоит враг
        if (m_map[Y, X] == '!')
        {
            m_HP--;
            Console.SetCursorPosition(m_barX--, m_barY);
            Console.Write('_');
        }

        //Если игрок встает на свой след
        if (m_map[Y, X] == 'S')
        {
            Console.Clear();
            Console.WriteLine("На те же грабли!" +
                            "\nYou Lose");
            Console.ReadKey();
            System.Environment.Exit(0);
        }
    }

    //Обработка нажатия клавиш
    private void Move()
    {
        ConsoleKeyInfo key;

        key = Console.ReadKey();
        switch (key.Key)
        {
            case ConsoleKey.DownArrow:
                //Если персонаж двигается не в стену
                if (m_map[m_performerY + 1, m_performerX] != '*')
                {
                    //Проверяем наличие врага
                    CheckEnemy(m_performerY + 1, m_performerX);

                    //Меняем положение персонажа
                    Console.SetCursorPosition(m_performerX, m_performerY);
                    //Рисуем след за персонажем
                    Console.Write('S');
                    m_map[m_performerY, m_performerX] = 'S';
                    Console.SetCursorPosition(m_performerX, ++m_performerY);
                    Console.Write('@'); //Рисуем новое положение персонажа

                    //Проверяем, оказался ли наш персонаж на финише
                    CheckExit(m_performerY, m_performerX);
                }
                break;

            case ConsoleKey.UpArrow:
                //Если персонаж двигается не в стену
                if (m_map[m_performerY - 1, m_performerX] != '*')
                {
                    //Проверяем наличие врага
                    CheckEnemy(m_performerY - 1, m_performerX);

                    //Меняем положение персонажа
                    Console.SetCursorPosition(m_performerX, m_performerY);
                    //Рисуем след за персонажем
                    Console.Write('S');
                    m_map[m_performerY, m_performerX] = 'S';
                    Console.SetCursorPosition(m_performerX, --m_performerY);
                    Console.Write('@');

                    //Проверяем, оказался ли наш персонаж на финише
                    CheckExit(m_performerY, m_performerX);
                }
                break;

            case ConsoleKey.LeftArrow:
                //Если персонаж двигается не в стену
                if (m_map[m_performerY, m_performerX - 1] != '*')
                {
                    //Проверяем наличие врага
                    CheckEnemy(m_performerY, m_performerX - 1);

                    //Меняем положение персонажа
                    Console.SetCursorPosition(m_performerX, m_performerY);
                    //Рисуем след за персонажем
                    Console.Write('S');
                    m_map[m_performerY, m_performerX] = 'S';
                    Console.SetCursorPosition(--m_performerX, m_performerY);
                    Console.Write('@');

                    //Проверяем, оказался ли наш персонаж на финише
                    CheckExit(m_performerY, m_performerX);

                }
                break;

            case ConsoleKey.RightArrow:
                //Если персонаж двигается не в стену
                if (m_map[m_performerY, m_performerX + 1] != '*')
                {
                    //Проверяем наличие врага
                    CheckEnemy(m_performerY, m_performerX + 1);

                    //Меняем положение персонажа
                    Console.SetCursorPosition(m_performerX, m_performerY);
                    //Рисуем след за персонажем
                    Console.Write('S');
                    m_map[m_performerY, m_performerX] = 'S';
                    Console.SetCursorPosition(++m_performerX, m_performerY);
                    Console.Write('@');

                    //Проверяем, оказался ли наш персонаж на финише
                    CheckExit(m_performerY, m_performerX);
                }
                break;

            case ConsoleKey.Escape:
                Console.Clear();
                Console.WriteLine("П Пока!");
                Console.ReadKey();
                System.Environment.Exit(0);
                break;

            default:
                break;
        }
    }
    private char[,] m_map;
    private int m_performerX, m_performerY;
    private int m_barX, m_barY;
    private int m_HP;
}