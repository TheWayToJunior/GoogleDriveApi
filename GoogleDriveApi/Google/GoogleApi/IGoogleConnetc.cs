using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace GoogleApi
{
    public interface IGoogleConnetc
    {
        /// <summary>
        /// Создает подключение к Google Drive
        /// </summary>
        /// <returns></returns>
        IClientService GetGoogleService();
    }
}
