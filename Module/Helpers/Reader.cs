using System.Text;
using Module.Engine.Classes;
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
            var fileCount = folderFiles.Length;

            

            for (var i = 0; i < fileCount; i++)
            {
                var file = folderFiles[i];

                file = file.ToLower();

                bool isItemData = false, isSkillData = false;

                if (file.Contains("itemdata"))
                    isItemData = true;
                else if(file.Contains("skilldata.txt"))
                    isSkillData = true;

                var data = File.ReadAllLines(file, Encoding.Unicode).ToList();
                var linesCount = data.Count;
                for (var d = 0; d < linesCount; d++)
                {
                    var line = data[d];
                    var searchBetweenTabs = line.Split('\t');

                    if (isItemData)
                    {
                        ProcessItemData(searchBetweenTabs);
                    }
                    else if (isSkillData)
                    {
                        ProcessSkillData(searchBetweenTabs);
                    }
                }
            }

            Custom.WriteLine($"Items loaded successfully {Main.Items.Count}");
            Custom.WriteLine($"Skills loaded successfully {Main.Skills.Count}");
        }

        private static void ProcessSkillData(string[] searchBetweenTabs)
        {
            var _temp = new SkillData();

            var enabled = searchBetweenTabs[(int)SkillField.Service].Trim();
            if (enabled == "0")
                return;

            try
            {
                // Get all properties of ItemData
                var properties = typeof(SkillData).GetProperties();

                // Loop through all properties and set values dynamically
                foreach (var property in properties)
                {
                    if (Enum.IsDefined(typeof(SkillField), property.Name)) // Ensure property exists in ItemField
                    {
                        int index = (int)Enum.Parse(typeof(SkillField), property.Name); // Get index from ItemField enum
                        if (index >= searchBetweenTabs.Length) continue; // Prevent out-of-bounds errors

                        string value = searchBetweenTabs[index].Trim();

                        // Convert value based on property type
                        if (property.PropertyType == typeof(int) && int.TryParse(value, out int intValue))
                        {
                            property.SetValue(_temp, intValue);
                        }
                        else if (property.PropertyType == typeof(float) && float.TryParse(value, out float floatValue))
                        {
                            property.SetValue(_temp, floatValue);
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(_temp, value);
                        }
                    }
                }
                // Ensure we have a valid ID before adding to the dictionary
                if (_temp.ID > 0 && !Main.Skills.ContainsKey(_temp.ID))
                {
                    Main.Skills[_temp.ID] = _temp;
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine(ex.ToString());
            }
        }

        private static void ProcessItemData(string[] searchBetweenTabs)
        {
            var _temp = new ItemData();

            var enabled = searchBetweenTabs[(int)ItemField.Service].Trim();
            if (enabled == "0")
                return;

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
                            property.SetValue(_temp, intValue);
                        }
                        else if (property.PropertyType == typeof(float) && float.TryParse(value, out float floatValue))
                        {
                            property.SetValue(_temp, floatValue);
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(_temp, value);
                        }
                    }
                }
                // Ensure we have a valid ID before adding to the dictionary
                if (_temp.ID > 0 && !Main.Items.ContainsKey(_temp.ID))
                {
                    Main.Items[_temp.ID] = _temp;
                }
            }
            catch (Exception ex)
            {
                Custom.WriteLine(ex.ToString());
            }
        }
    }
}
