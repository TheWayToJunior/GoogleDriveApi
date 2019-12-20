using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GoogleApi
{
    public class GoogleDriveAPI2
    {
        private readonly DriveService service;

        /// <summary>
        /// Права
        /// </summary>
        static string[] Scopes = { DriveService.Scope.Drive, DriveService.Scope.DriveReadonly };
        static string ApplicationName = "MyGoogleDriveAdd";

        public GoogleDriveAPI2()
        {
            service = GetGoogleAPI();
        }

        /// <summary>
        /// Выполняет авторизацию пользователя
        /// </summary>
        /// <returns></returns>
        public UserCredential GoogleAuthorize()
        {
            string apiPath = @"D:\С#\API\GoogleDriveApi\GoogleDriveApi\bin\Debug\credentials.json";
            UserCredential credential;

            using (var stream = new FileStream(apiPath, FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = @"D:\С#\API\GoogleDriveApi\GoogleDriveApi\bin\Debug\token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                Console.WriteLine("Credential file saved to: " + credPath);
            }

            return credential;
        }

        /// <summary>
        /// Создает подключение к Google Drive используя API
        /// </summary>
        /// <returns>Возвращает инициализированный объект сервиса</returns>
        private DriveService GetGoogleAPI()
        {
            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleAuthorize(),
                ApplicationName = ApplicationName,
            });
        }

        /// <summary>
        /// Получает список имен файлов загруженных на Google Drive
        /// </summary>
        public void GetFilseGoogleDrive()
        {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name)";
            //listRequest.PageSize = 10;

            //Список файлов полученных после запроса на сервер.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;

            Console.WriteLine("Files:");

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                    Console.WriteLine("{0} ({1})", file.Name, file.Id);
            }
            else
                Console.WriteLine("No files found.");
        }

        /// <summary>
        /// Создает папку на Google Drive
        /// </summary>
        /// <param name="name">Название папки</param>
        /// <param name="parentsId">
        /// Id родительской папки.
        /// По умолчанию значение равно корневой папке на Google Drive 
        /// </param>
        /// <returns>Возвращает Id созданной папки</returns>
        public string CreateDirectoryDrive(string name, string parentsId = "root")
        {
            var fileMet = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>() { parentsId }
            };

            var request = service.Files.Create(fileMet);
            request.Fields = "id, name";

            var file = request.Execute();

            Console.WriteLine($"Папка: {file.Name}");

            return file.Id;
        }

        /// <summary>
        /// Асинхронная версия метода CreateDirectoryDrive
        /// </summary>
        public async Task<string> CreateDirectoryDriveAsync(string name, string parentsId = "root")
        {
            return await Task.Run(() => CreateDirectoryDrive(name, parentsId));
        }

        /// <summary>
        /// Загружает файл на Google Drive
        /// </summary>
        /// <param name="filePact">Путь к файлу</param>
        /// <param name="mimeType">Тип файла</param>
        /// <param name="parent">Родительская папка</param>
        public void UploadFileGoogleDrive(string filePact, string mimeType, string parent)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = filePact.Split('\\').Last(),
                MimeType = mimeType,
                Parents = new List<string>() { parent }
            };

            FilesResource.CreateMediaUpload request;

            using (var stream = new System.IO.FileStream(filePact, System.IO.FileMode.Open))
            {
                request = service.Files.Create(fileMetadata, stream, mimeType);
                request.Fields = "id, name, size";
                request.Upload();
            }

            try
            {
                var file = request.ResponseBody;
                Console.WriteLine($"\tФайл загружен: {file.Name} \tразмер файла {file.Size / 1024} КБ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Асинхронная версия метода UploadFileGoogleDrive
        /// </summary>
        public async Task UploadFileGoogleDriveAsync(string filePact, string mimeType, string parent)
        {
            await Task.Run(() => UploadFileGoogleDrive(filePact, mimeType, parent));
        }

        /// <summary>
        /// Загружает папку на Google Drive 
        /// </summary>
        /// <param name="pahtDir">Путь к папке</param>
        /// <param name="parent">Id родительской папки</param>
        public async Task UploadDirectoryDriveAsync(string pahtDir, string parent = "root")
        {
            string nameDir = pahtDir.Split('\\').Last();

            string idDir = await this.CreateDirectoryDriveAsync(nameDir, parent); ///! await

            FileInfo[] files = null;
            DirectoryInfo[] directories = null;

            try
            {
                files = new DirectoryInfo(pahtDir).GetFiles();
                directories = new DirectoryInfo(pahtDir).GetDirectories();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (files != null)
            {
                foreach (var item in files) ///! await
                    await this.UploadFileGoogleDriveAsync(item.FullName, MimeMapping.GetMimeMapping(item.FullName), idDir);

                foreach (var item in directories)
                    await UploadDirectoryDriveAsync(item.FullName, idDir);
            }
        }
    }
}
