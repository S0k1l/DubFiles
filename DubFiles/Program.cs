using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace DubFiles
{
    public class FileDetails
    {
        public string FileName { get; set; }
        public string FileHash { get; set; }
    }

    class Program
    {
        static int bufferSize = 1 * 1024 * 1024;
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<FileDetails> finalDetails = new List<FileDetails>();
            List<string> dubFiles = new List<string>();

            //Вказується шлях папки
            Console.Write("Enter path: ");
            Console.ForegroundColor = ConsoleColor.White;
            string path = @"" + Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Gray;

            //Отримання всіх файлів
            var fileName = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            //Вивід кільклсті файлів у папці
            Console.Write($"Files in folder: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(fileName.Length);

            //Хешування файлів
            Parallel.ForEach(fileName, (item) =>
            {
                using (var stream = new BufferedStream(File.OpenRead(item), bufferSize))
                {
                    var sha1 = new SHA1Managed();
                    byte[] checkSum = sha1.ComputeHash(stream);
                    finalDetails.Add(new FileDetails()
                    {
                        FileName = item.ToString(),
                        FileHash = BitConverter.ToString(checkSum),
                    });
                }
            });

            //Групування по хешу
            var similarList = finalDetails.GroupBy(f => f.FileHash)
                .Select(g => new { FileHash = g.Key, Files = g.Select(z => z.FileName).ToList()});

            //Збкрігає один об'єкт кожної групи як оригінал, а все інше як дублікат 
            dubFiles.AddRange(similarList.SelectMany(f => f.Files.Skip(1)).ToList());
            Console.ForegroundColor = ConsoleColor.Red;

            //Вивід всіх дублікатів
            Console.WriteLine("\n▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼ Duplicate Files ▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼▼");
            if (dubFiles.Count > 0)
            {
                foreach (var item in dubFiles)
                {
                    Console.WriteLine(item);
                    FileInfo fi = new FileInfo(item);
                }
            }
            Console.WriteLine("▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲ Duplicate Files ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲");

            //Вивід кількості дублікованих файлів
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("\nTotal duplicate files: ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(dubFiles.Count);

            //Вивід Таймера
            Console.ForegroundColor = ConsoleColor.White;
            stopWatch.Stop();
            Console.WriteLine("Time elapsed: {0:hh\\:mm\\:ss\\.fffffff}", stopWatch.Elapsed);
            Console.ReadLine();
        }
    }
}