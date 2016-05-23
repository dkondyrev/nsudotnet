using System;
using System.IO;

namespace Kondyrev.Nsudotnet.LinesCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                int numLines = CountLines(new DirectoryInfo("."), args[0]);
                Console.WriteLine("Number of lines: {0}", numLines);
            }
            else
            {
                Console.WriteLine("The files type is not specified");
            }
            Console.ReadLine();
        }

        static int CountLines(DirectoryInfo directory, string fileType)
        {
            FileInfo[] files = directory.GetFiles(fileType, SearchOption.AllDirectories);

            int count = 0;
            foreach (FileInfo file in files)
            {
                count += CountFileLines(file);
            }
            return count;
        }

        static int CountFileLines(FileInfo file)
        {
            int count = 0;
            using (StreamReader reader = file.OpenText())
            {
                string line;
                bool commentFlag = false;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == string.Empty)
                    {
                        continue;
                    }
                    ProcessLine(line, ref commentFlag, ref count);
                }
            }
            return count;
        }

        static void ProcessLine(string line, ref bool commentFlag, ref int count)
        {
            bool firstFlag = true;
            for (int i = 0; i < line.Length - 1; ++i)
            {
                if (commentFlag)
                {
                    if (line[i] == '*' && line[i + 1] == '/')
                    {
                        ++i;
                        commentFlag = false;
                    }
                }
                else
                {
                    if (line[i] == '/')
                    {
                        if (line[i + 1] == '*')
                        {
                            ++i;
                            commentFlag = true;
                        }
                        else if (line[i + 1] == '/')
                        {
                            break;
                        }
                    }
                    else if (firstFlag)
                    {
                        firstFlag = false;
                        ++count;
                    }
                }
            }
        }
    }
}
