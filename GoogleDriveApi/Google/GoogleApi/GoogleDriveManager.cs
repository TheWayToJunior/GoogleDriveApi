using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleApi
{
    class GoogleDriveManager : IGoogleDriveManager
    {
        private readonly DriveService _Service;

        /// <summary>
        /// Селектор, указывающий, какие поля следует включить в частичный ответ для CreateRequest
        /// </summary>
        public string RequestSelector { get; set; } = "id, name";

        /// <summary>
        /// Селектор, указывающий, какие поля следует включить в частичный ответ для CreateMediaUpload
        /// </summary>
        public string MediaUploadSelector { get; set; } = "id, name, size";

        public GoogleDriveManager(IGoogleConnetc googleConnetc)
        {
            _Service = googleConnetc.GetGoogleService() as DriveService;
        }

        public IList<File> GetFilses()
        {
            FilesResource.ListRequest listRequest = _Service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name)";
            //listRequest.PageSize = 10;

            ///Список файлов полученных после запроса на сервер.
            return listRequest.Execute().Files;
        }

        /// <param name="name">Имя папки</param>
        /// <param name="parentsId">по умолчанию задана корневая папка Google Drive</param>
        /// <returns>Возвращает выборку параметров из запроса к серверу</returns>
        public File CreateDirectory(string name, string parentsId = "root")
        {
            var fileMet = new File()
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>() { parentsId }
            };

            var request = _Service.Files.Create(fileMet);
            request.Fields = this.RequestSelector;

            return request.Execute();
        }

        public async Task<File> CreateDirectoryAsync(string name, string parentsId = "root")
        {
            return await Task.Run(() => CreateDirectory(name, parentsId));
        }

        /// <param name="filePact">Путь к файлу</param>
        /// <param name="mimeType">Тип файла</param>
        /// <param name="parent">Родительская папка</param>
        /// <returns>Возвращает выборку параметров из запроса к серверу</returns>
        public File UploadFile(string filePact, string mimeType, string parent)
        {
            var fileMetadata = new File()
            {
                Name = filePact.Split('\\').Last(),
                MimeType = mimeType,
                Parents = new List<string>() { parent }
            };

            FilesResource.CreateMediaUpload request;

            using (var stream = new System.IO.FileStream(filePact, System.IO.FileMode.Open))
            {
                request = _Service.Files.Create(fileMetadata, stream, mimeType);
                request.Fields = this.MediaUploadSelector;
                request.Upload();
            }

            return request.ResponseBody;
        }

        public async Task<File> UploadFileAsync(string filePact, string mimeType, string parent)
        {
            return await Task.Run(() => UploadFile(filePact, mimeType, parent));
        }
    }
}
