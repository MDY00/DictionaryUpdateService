using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net;

namespace DictionaryService.FuncClasses;

public static class ApiDownloader
{
    private static readonly string[] LSFinJsonPath = { "data.relationships.dataset.links.related", "data.relationships.resources.links.related" };
    private static readonly string[] RPLinJsonPath = { "data.relationships.dataset.links.related", "data.relationships.resources.links.related" };


    public static void DownloadFromApi(string downloadPath, string dictName)
    {
        try
        {
            var sourceApiDictLink = GetApiLink(dictName);

            using var wc = new WebClient();
            wc.DownloadFile(sourceApiDictLink, downloadPath + GetApiFilename(dictName));
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
    }

    public static string GetApiFilename(string apiDictName)
    {
        var fileName = "";
        var uri = GetApiLink(apiDictName);
        try
        {
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(uri);

                if (!string.IsNullOrEmpty(wc.ResponseHeaders["Content-Disposition"]))
                {
                    fileName = wc.ResponseHeaders["Content-Disposition"].Substring(wc.ResponseHeaders["Content-Disposition"].IndexOf("filename=") + 9).Replace("\"", "");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
        return fileName;
    }

    private static string GetApiLink(string apiDictName)
    {
        string[] apiDictInJsonPath;
        string originalApiDictLink;


        if (apiDictName == "LSF")
        {
            apiDictInJsonPath = LSFinJsonPath;
            originalApiDictLink = AppConfig.LsfLink;
        }
        else if (apiDictName == "RPL")
        {
            apiDictInJsonPath = RPLinJsonPath;
            originalApiDictLink = AppConfig.RplLink;
        }
        else
        {
            apiDictInJsonPath = new string[] { "" };
            originalApiDictLink = "";
        }
        string datasetLinkString = String.Empty;
        int i = 0;
        try
        {
            var apiLinkString = GetLinkFromJson(originalApiDictLink, apiDictInJsonPath[0]);
            datasetLinkString = GetLinkFromJson(apiLinkString, apiDictInJsonPath[1]);

            var jObj = (JObject)JsonConvert.DeserializeObject(GetJson(datasetLinkString));

            for (; i < jObj.Count; i++)
            {
                var resourceAttribute = GetLinkFromJson(datasetLinkString, $"data[{i}].attributes.format");
                if (resourceAttribute == "xml")
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }

        return GetLinkFromJson(datasetLinkString, $"data[{i}].attributes.link");
    }

    private static string GetLinkFromJson(string address, string linkPath)
    {
        string finalAddress = String.Empty;
        try
        {
            using (var wc = new WebClient())
            {
                var json = wc.DownloadString(address);
                var data = (JObject)JsonConvert.DeserializeObject(json);
                finalAddress = data.SelectToken(linkPath).Value<string>();
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
        return finalAddress;
    }

    private static string GetJson(string address)
    {
        string jsonString = String.Empty;
        try
        {
            using (var wc = new WebClient())
                jsonString = wc.DownloadString(address);
        }
        catch (Exception ex)
        {
            Log.Error("Error {ex}", ex.Message);
        }
        return jsonString;
    }
}
