using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace First_MVVM.Business
{
    public class NationalCities
    {
        public List<CitiesList> FillDefaultData()
        {
            string path = Directory.GetCurrentDirectory();
            path = path + "\\netcoreapp3.1\\Json\\CityCountyData.json";//netcoreapp3.1
            string jsonString = File.ReadAllText(path);
            List<CitiesList> parameter = JsonSerializer.Deserialize<List<CitiesList>>(jsonString);

            return parameter;
        }

        public List<string> FillCitiesData(List<CitiesList> DefaultData)
        {
            List<string> CitiesList = new List<string>();

            foreach (var item in DefaultData)
            {
                CitiesList.Add(item.CityName);
            }
            return CitiesList;
        }

        public List<string> FillTownshipData(List<CitiesList> DefaultData, int SelectedIndex)
        {
            List<string> TownshipList = new List<string>();

            if (SelectedIndex == -1) return TownshipList;

            foreach (var item in DefaultData[SelectedIndex].AreaList)
            {
                TownshipList.Add(item.AreaName);
            }
            return TownshipList;
        }
    }

    public class CitiesList
    {
        public string CityName { get; set; }
        public string CityEngName { get; set; }
        public IList<AreaList> AreaList { get; set; }
    }

    public class AreaList
    {
        public string ZipCode { get; set; }
        public string AreaName { get; set; }
        public string AreaEngName { get; set; }
    }
}
