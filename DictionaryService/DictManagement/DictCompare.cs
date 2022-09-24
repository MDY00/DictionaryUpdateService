using DictionaryService.FuncClasses;
using Serilog;

namespace DictionaryService.DictManagement;

public static class DictCompare
{
    public static List<Dictionary> Compare()
    {
        var resultList = new List<Dictionary>();
        var ftpList = GetListFromFTP();
        var dbList = DbConnection.GetDictsDataFromDB();

        ftpList.Sort((firstDict, secondDict) => firstDict.DictName.CompareTo(secondDict.DictName));
        dbList.Sort((firstDict, secondDict) => firstDict.DictName.CompareTo(secondDict.DictName));

        for (int i = 0; i < ftpList.Count; i++)
        {
            resultList.Add(ftpList[i]);

            if (ftpList[i].DictVersion != dbList[i].DictVersion)
                resultList[i].UpdateNeeded = true;
        }
        Log.Information("Comparing dictionaries finished");
        return resultList;
    }

    private static List<Dictionary> GetListFromFTP()
    {
        var FTPList = new List<Dictionary>();

        foreach (var dictionary in Dictionary.AllDictList)
        {
            switch (dictionary.DictName)
            {
                case "RPL":
                    FTPList.Add(new Dictionary("RPL", StripPatterns.GetApiDictDate(ApiDownloader.GetApiFilename("RPL")), dictionary.DownloadPath));
                    break;

                case "LSF":
                    FTPList.Add(new Dictionary("LSF", StripPatterns.GetApiDictDate(ApiDownloader.GetApiFilename("LSF")), dictionary.DownloadPath));
                    break;

                default:
                    FTPList.Add(new Dictionary(dictionary.DictName, StripPatterns.GetFtpDictVersion(FtpConfiguration.GetFullDictName(dictionary.DictName)), dictionary.DownloadPath));
                    break;
            }
        }
        Log.Information("FTP list downloaded");
        return FTPList;
    }
}
