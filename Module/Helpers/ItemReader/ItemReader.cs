using System.Text;
using Module.Services;
namespace Module.Helpers.ItemReader
{
    public class ItemReader
    {


        public static void Init()
        {
            //var _files = Main._settings.serverVersion;
            var _settings = Main._settings.Data;

            if(!Directory.Exists(_settings.Path))
            {
                Custom.WriteLine($"Creating folder {_settings.Path}");
                Directory.CreateDirectory(_settings.Path);
            }

            var folderFiles = Directory.GetFiles(_settings.Path);
            var _items = Main._items;

            foreach (var file in folderFiles)
            {
                if (!file.Contains("itemdata"))
                    continue;

                var data = File.ReadAllLines(file, Encoding.Unicode).ToList();
                for (var i = 0; i < data.Count(); i++)
                {
                    var line = data[i];
                    var searchBetweenTabs = line.Split('\t');

                    var enabled = searchBetweenTabs[(int)ItemField.Service].Trim();
                    if (enabled == "0")
                        continue;

                    var _tempItemData = new ItemData();

                    try
                    {
                        // Get all properties of ItemData
                        var properties = typeof(ItemData).GetProperties();

                        // Loop through all properties and set values dynamically
                        foreach (var property in properties)
                        {
                            if (Enum.IsDefined(typeof(ItemField), property.Name)) // Ensure property exists in ItemField
                            {
                                int index = (int)Enum.Parse(typeof(ItemField), property.Name); // Get index from ItemField enum
                                if (index >= searchBetweenTabs.Length) continue; // Prevent out-of-bounds errors

                                string value = searchBetweenTabs[index].Trim();

                                // Convert value based on property type
                                if (property.PropertyType == typeof(int) && int.TryParse(value, out int intValue))
                                {
                                    property.SetValue(_tempItemData, intValue);
                                }
                                else if (property.PropertyType == typeof(float) && float.TryParse(value, out float floatValue))
                                {
                                    property.SetValue(_tempItemData, floatValue);
                                }
                                else if (property.PropertyType == typeof(string))
                                {
                                    property.SetValue(_tempItemData, value);
                                }
                            }
                        }
                        // Ensure we have a valid ID before adding to the dictionary
                        if (_tempItemData.ID > 0 && !_items.ContainsKey(_tempItemData.ID))
                        {
                            _items[_tempItemData.ID] = _tempItemData;
                        }
                    }
                    catch(Exception ex)
                    {
                        Custom.WriteLine(ex.ToString());
                    }
                }
            }

            if(folderFiles.Count() == 0)
            {
                Custom.WriteLine($"Please extract itemdata from Client and put it inside {_settings.Path}");
            }
            else
            {
                Custom.WriteLine($"Items loaded successfully {_items.Count}");
            }
        }
    }
}
