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
                    if (!commentFlag)
                    {
                        if (!line.StartsWith(@"//"))
                        {
                            if (line.Contains(@"/*"))
                            {
                                if (!line.StartsWith(@"/*"))
                                {
                                    ++count;
                                }
                                commentFlag = true;
                            }
                            else
                            {
                                ++count;
                            }
                        }
                    }
                    else
                    {
                        if (line.Contains(@"*/"))
                        {
                            commentFlag = false;
                            if (!line.EndsWith(@"*/"))
                            {
                                ++count;
                            }
                        }
                    }
                }
            }
            return count;
        }
    }
}
