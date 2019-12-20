using GoogleApi;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GoogleDriveApi.GoogleDriveManagement
{
    class GoogleDriveMemagement
    {
        private readonly IGoogleDriveManager _GoogleDrive;

        /// <summary>
        /// Событие возникающее при создании папки на Google Drive
        /// </summary>
        public event Action<string> CreateFolderEvent;

        /// <summary>
        /// Событие возникающее при загрузки файла на Google Drive
        /// </summary>
        public event Action<string, long?> UploadFileEvent;

        public GoogleDriveMemagement()
        {
            _GoogleDrive = new GoogleDriveManager(new GoogleApi.GoogleDriveApi());
        }

        /// <summary>
        /// Загружает папку на Google Drive
        /// </summary>
        /// <param name="dirPath">Путь к папке</param>
        /// <param name="parent">Родительская папка</param>
        /// <returns></returns>
        public async Task UploadDirectoryAsync(string dirPath, string parent = "root")
        {
            /// Получение имени папки
            string dirName = dirPath.Split('\\').Last();

            /// Создаем папку в Google Drive
            var dirRespons = await _GoogleDrive.CreateDirectoryAsync(dirName, parent);

            CreateFolderEvent?.Invoke(dirRespons.Name);

            FileInfo[] files = null;
            DirectoryInfo[] subDirectories = null;

            try
            {
                /// Получаем все файлы в указанной папке
                files = new DirectoryInfo(dirPath).GetFiles();

                /// Получаем все подкаталоги в указаной папке
                subDirectories = new DirectoryInfo(dirPath).GetDirectories();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            /// Проверяем были ли инициализированы массивы
            if (files != null || subDirectories != null)
            {
                /// Загружаем полученные файлы на Google Drive
                foreach (var item in files)
                {
                    var fileRespons = await _GoogleDrive.UploadFileAsync(item.FullName,
                            MimeMapping.GetMimeMapping(item.FullName), dirRespons.Id);

                    UploadFileEvent?.Invoke(fileRespons.Name, fileRespons.Size);
                }

                /// Рекурсивно проходим по всем найденым папкам
                foreach (var item in subDirectories)
                {
                    await UploadDirectoryAsync(item.FullName, dirRespons.Id);
                }
            }
        }
    }
}
