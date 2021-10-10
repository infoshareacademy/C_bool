﻿using System;
using System.Collections.Generic;

namespace C_bool.ConsoleApp
{
    public class Menu
    {
        private static readonly char _indicator = '►';
        private static bool _isWorking = true;
        private static readonly List<string> _dataSourceList = new()
        {
            {_indicator + " Plik"},
            {"  Czysta aplikacja"},
            {"  Zamknij aplikację"}
        };
        private static readonly List<string> _menuList = new()
        {
            {_indicator + " Wyświetl klasyfikację użytkowników"},
            {"  Dodaj nowego użytkownika"},
            {"  Edytuj wybranego użytkownika"},
            {"  Wyświetl wszystkie miejsca w okolicy"},
            {"  Wyświetl miejsca w określonym zasięgu"},
            {"  Wyświetl miejsca określonego typu"},
            {"  Edytuj wybrane miejsce"},
            {"  Zamknij aplikację"}
        };
        

        public static void StartProgram()
        {
            
            
            var isDataSourceSelected = false;
            
            while (_isWorking)
            {
                Console.Clear();
                Console.WriteLine("+++ WITAJ W APLIKACJI C_bool +++\n");

                if (!isDataSourceSelected)
                {
                    SelectDataSource();
                    isDataSourceSelected = true;
                }
                else
                {
                    SelectActionFromMenu();
                }
            }
        }

        private static void SelectActionFromMenu()
        {
            Console.WriteLine("MENU:\n");

            switch (SelectPositionFromMenu(_menuList))
            {
                case 0:
                    _isWorking = false;
                    break;
                case 1:
                    _isWorking = false;
                    break;
                case 2:
                    _isWorking = false;
                    break;
                case 3:
                    _isWorking = false;
                    break;
                case 4:
                    _isWorking = false;
                    break;
                case 5:
                    _isWorking = false;
                    break;
                case 6:
                    _isWorking = false;
                    break;
                case 7:
                    _isWorking = false;
                    break;
            }
        }

        private static void SelectDataSource()
        {
            Console.WriteLine("Wybierz źródło danych dla aplikacji:\n");

            switch (SelectPositionFromMenu(_dataSourceList))
            {
                    
                case 0:
                    Console.WriteLine("Wczytuje plik");
                    break;
                case 1:
                    Console.WriteLine("Nie wczytuje pliku");
                    break;
                case 2:
                    _isWorking = false;
                    break;
            }
        }

        private static int SelectPositionFromMenu(List<string> menu)
        {
            var index = 0;

            while (true)
            {
                Console.SetCursorPosition(0, 4);

                PrintMenuPositions(menu);
                PrintMoveLegend();

                var move = Console.ReadKey(true).Key;

                if (move == ConsoleKey.DownArrow && index < menu.Count - 1)
                {
                    menu[index] = menu[index].Replace(_indicator, ' ');
                    index++;
                    menu[index] = _indicator + menu[index].Substring(1);
                }
                else if (move == ConsoleKey.UpArrow && index > 0)
                {
                    menu[index] = menu[index].Replace(_indicator, ' ');
                    index--;
                    menu[index] = _indicator + menu[index].Substring(1);
                }
                else if (move == ConsoleKey.Enter)
                {
                    return index;
                }
            }
        }

        private static void PrintMenuPositions(List<string> menu)
        {
            foreach (var position in menu)
            {
                if (position.Contains("Zamknij aplikację") ||
                    position.Contains("Wyświetl wszystkie miejsca w okolicy"))
                {
                    Console.WriteLine();
                }

                if (position.Contains(_indicator))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\r{position}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"\r{position}");
                }
            }
        }

        private static void PrintMoveLegend()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
                $"\n\n '{ConsoleKey.UpArrow}' - w górę || '{ConsoleKey.DownArrow}' - w dół || '{ConsoleKey.Enter.ToString().ToUpper()}' - wybierz ");
            Console.ResetColor();
        }
    }
}