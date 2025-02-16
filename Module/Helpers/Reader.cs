using System.Text;
using Module.Helpers.ClientData;
using Module.Services;
namespace Module.Helpers
{
    public class Reader
    {
        public static void Init()
        {
            //var _files = Main.Settings.serverVersion;
            var Settings = Main.Settings.Data;

            if (!Directory.Exists(Settings.Path))
            {
                Custom.WriteLine($"Creating folder {Settings.Path}");
                Directory.CreateDirectory(Settings.Path);
            }

            var folderFiles = Directory.GetFiles(Settings.Path);
            var Items = Main.Items;

            foreach (var file in folderFiles)
            {
                if (!file.Contains("itemdata"))
                    continue;

                var data = File.ReadAllLines(file, Encoding.Unicode).ToList();
                for (var i = 0; i < data.Count; i++)
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
                        if (_tempItemData.ID > 0 && !Items.ContainsKey(_tempItemData.ID))
                        {
                            Items[_tempItemData.ID] = _tempItemData;
                        }
                    }
                    catch (Exception ex)
                    {
                        Custom.WriteLine(ex.ToString());
                    }
                }
            }

            if (folderFiles.Length == 0)
            {
                Custom.WriteLine($"Please extract itemdata from Client and put it inside {Settings.Path}");
            }
            else
            {
                Custom.WriteLine($"Items loaded successfully {Items.Count}");
            }
        }
    }
}
