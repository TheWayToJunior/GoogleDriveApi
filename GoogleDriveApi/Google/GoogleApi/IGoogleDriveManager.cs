using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleApi
{
    public interface IGoogleDriveManager
    {
        /// <summary>
        /// Возвращает все файлы на Google Drive
        /// </summary>
        IList<File> GetFilses();

        /// <summary>
        /// Создает папку на Google Drive
        /// </summary>
        File CreateDirectory(string name, string parentsId = "root");

        /// <summary>
        ///  Асинхронная версия метода CreateDirectory
        /// </summary>
        Task<File> CreateDirectoryAsync(string name, string parentsId = "root");

        /// <summary>
        /// Загружает файл на Google Drive
        /// </summary>
        File UploadFile(string filePact, string mimeType, string parent);

        /// <summary>
        ///  Асинхронная версия метода UploadFile
        /// </summary>
        Task<File> UploadFileAsync(string filePact, string mimeType, string parent);
    }
}
