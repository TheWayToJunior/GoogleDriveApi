using GoogleDriveApi.GoogleDriveManagement;
using System;
using System.Diagnostics;

namespace GoogleApi
{
    class ConsolePrint
    {
        public static async void TestTime(string path)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            GoogleDriveAPI2 google = new GoogleDriveAPI2();

            await google.UploadDirectoryDriveAsync(path, "root");

            stopwatch.Stop();

            Console.WriteLine($"\nВремязагрузки: {stopwatch.Elapsed} ----- Готово!!!");
        }

        public static async void GoogleInfo(string path)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            GoogleDriveMemagement google = new GoogleDriveMemagement();
            Console.WriteLine("Подключение к вашему Google Drive создано!");

            google.CreateFolderEvent += (n) => Console.WriteLine($"Папка: {n}");
            google.UploadFileEvent += (n, s) => Console.WriteLine($"\tФайл загружен: {n} \tразмер файла {s / 1024} КБ");

            await google.UploadDirectoryAsync(path);

            stopwatch.Stop();
            Console.WriteLine($"\nВремя загрузки: {stopwatch.Elapsed} ----- Готово!!!");
        }
    }

    class Program
    {
         static void Main(string[] args)
        {
            //ConsolePrint.TestTime(args[0]);
            //TestTime(args[0]);
            //var google = new GoogleApi.GoogleDriveApi();
            //var drive = google.GetGoogleService();


            ConsolePrint.GoogleInfo(args[0]);

            Console.ReadKey();
        }
    }
}
