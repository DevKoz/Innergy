using Innergy.Services;
using System;
using System.Text;

namespace Innergy
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = string.Empty;

            StringBuilder sb = new StringBuilder();
            var line = string.Empty;
            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                if (!line.StartsWith('#'))
                {
                    sb.AppendLine(line);
                }
            }

            input = sb.ToString();

            var service = new ParseDataService();

            var result=service.ParseData(input);

            Console.Write(result);

            Console.ReadKey();
        }
    }
}
