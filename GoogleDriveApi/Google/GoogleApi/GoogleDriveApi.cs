using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace GoogleApi
{
    public class GoogleDriveApi : IGoogleConnetc
    {
        /// <summary>
        /// Права
        /// </summary>
        private static string[] _Scopes = { DriveService.Scope.Drive, DriveService.Scope.DriveReadonly };
        private static string _ApplicationName = "MyGoogleDriveAdd";

        /// <summary>
        /// Возвращает название проекта
        /// </summary>
        public string GetApplicationName
        {
            get
            {
                return _ApplicationName;
            }
        }

        /// <summary>
        /// Выполняет авторизацию пользователя
        /// </summary>
        /// <returns></returns>
        private UserCredential GoogleAuthorize()
        {
            string apiPath = @"D:\С#\API\GoogleDriveApi\GoogleDriveApi\bin\Debug\credentials.json";

            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            string credPath = @"D:\С#\API\GoogleDriveApi\GoogleDriveApi\bin\Debug\token.json";

            UserCredential credential = null;

            try
            {
                using (var stream = new FileStream(apiPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        _Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException($"Не удалось найти фаёл {apiPath.Split('\\').Last()}", apiPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return credential;
        }

        /// <summary>
        /// Создает подключение к Google Drive используя API
        /// </summary>
        /// <returns>Возвращает инициализированный объект сервиса</returns>
        public IClientService GetGoogleService()
        {
            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = this.GoogleAuthorize(),
                ApplicationName = _ApplicationName,
            });
        }
    }
}
