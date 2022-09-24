using DictionaryService.DictManagement;
using DictionaryService.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DictionaryService.Services;

public class DictService : IDictService
{
    public DictService()
    {

    }

    public string GetDictsToUpdate()
    {
        string dictsToUpdateJson = JsonConvert.SerializeObject(DictCompare.Compare());
        return dictsToUpdateJson;
    }

    public void PostDicts(string dictsToUpdateJson)
    {
        List<Dictionary>? dictsToUpdate = JsonConvert.DeserializeObject<List<Dictionary>>(dictsToUpdateJson);

        foreach (var dictionary in dictsToUpdate)
            dictionary.UpdateDictionary();
    }

    public static void GetConfig()
    {
        var config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
        var section = config.GetSection("AppSettings");
        _ = section.Get<AppConfig>();
        var listSection = config.GetSection("Dictionaries");
        Dictionary.AllDictList = listSection.Get(typeof(List<Dictionary>)) as List<Dictionary>;
    }
}
