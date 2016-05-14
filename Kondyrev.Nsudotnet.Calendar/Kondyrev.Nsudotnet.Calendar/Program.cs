using System;

namespace Kondyrev.Nsudotnet.Calendar
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime date;
            if (args.Length > 0 && DateTime.TryParse(args[0], out date))
            {
                PrintCalendar(date);
            }
            else
            {
                Console.WriteLine("Incorrect parameters");
            }
            Console.ReadLine();
        }

        static void PrintCalendar(DateTime date)
        {
            PrintDaysOfWeek(date);
            DateTime firstDay = new DateTime(date.Year, date.Month, 1);

            int dayOfWeek = (int)firstDay.DayOfWeek;
            int count = (dayOfWeek + 6) % 7;
            Skip(count);

            DateTime tmp = new DateTime(date.Year, date.Month + 1, 1);
            tmp = tmp.AddDays(-1);
            int daysInMonth = tmp.Day;

            ConsoleColor background = Console.BackgroundColor;
            ConsoleColor foreground = Console.ForegroundColor;
            bool backgroundChanged = false;

            int workingDays = 0;

            for (int i = 1; i <= daysInMonth; ++i)
            {
                dayOfWeek = count % 7;
                if (dayOfWeek == 5 || dayOfWeek == 6)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    ++workingDays;
                }
                if (i == date.Day)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    backgroundChanged = true;
                }
                else if (i == DateTime.Today.Day && date.Month == DateTime.Today.Month && date.Year == DateTime.Today.Year)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    backgroundChanged = true;
                }
                Console.Write(i);
                ++count;

                if (backgroundChanged)
                {
                    Console.BackgroundColor = background;
                    backgroundChanged = false;
                }
                Console.Write("\t");

                if (count % 7 == 0)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = foreground;
                }
            }

            Console.WriteLine("\nNumber of working days: {0}", workingDays);
        }

        static void PrintDaysOfWeek(DateTime date)
        {
            int dayOfWeek = (int)date.DayOfWeek;
            DateTime day = date.AddDays(1 - dayOfWeek);

            for (int i = 0; i < 7; ++i)
            {
                Console.Write("{0}\t", day.ToString("ddd"));
                day = day.AddDays(1);
            }
            Console.WriteLine();
        }

        static void Skip(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                Console.Write("\t");
            }
        }
    }
}
