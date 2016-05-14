using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace net.tschmid.scooring.utils
{

    // Stolen from https://nateshoffner.com/blog/2013/01/c-reusable-portable-application-settings/
    public class PortableSettingsProvider : SettingsProvider
    {
        private const string XMLROOT = "configuration"; // XML Root node
        private const string CONFIGNODE = "configSections"; // Configuration declaration node     
        private const string GROUPNODE = "sectionGroup"; // Configuration section group declaration node
        private const string USERNODE = "userSettings"; // User section node

        // Application Specific Node

        private string APPNODE;

        // Store instace of calling assembly
        private Assembly entryAssembly;

        private XmlDocument xmlDoc;

        public override string ApplicationName
        {
            get { return (entryAssembly.GetName().Name); }
            set { APPNODE = value; }
        }

        public string ConfigDirectory { get; private set; }

        public string ConfigFile
        {
            get
            {
                return Path.Combine(ConfigDirectory, string.Format("{0}.config", ApplicationName));
            }
        }

        private XmlDocument XMLConfig
        {
            get
            {
                // Check if we already have accessed the XML config file. If the xmlDoc object is empty, we have not.
                if (xmlDoc != null)
                    return xmlDoc;

                xmlDoc = new XmlDocument();

                // If we have not loaded the config, try reading the file from disk.
                try
                {
                    xmlDoc.Load(ConfigFile);
                }

                // If the file does not exist on disk, catch the exception then create the XML template for the file.
                catch (Exception)
                {
                    // XML Declaration
                    // <?xml version="1.0" encoding="utf-8"?>
                    var dec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                    xmlDoc.AppendChild(dec);

                    // Create root node and append to the document
                    // <configuration>
                    var rootNode = xmlDoc.CreateElement(XMLROOT);
                    xmlDoc.AppendChild(rootNode);

                    // Create Configuration Sections node and add as the first node under the root
                    // <configSections>
                    var configNode = xmlDoc.CreateElement(CONFIGNODE);
                    xmlDoc.DocumentElement.PrependChild(configNode);

                    // Create the user settings section group declaration and append to the config node above
                    // <sectionGroup name="userSettings"...>
                    var groupNode = xmlDoc.CreateElement(GROUPNODE);
                    groupNode.SetAttribute("name", USERNODE);
                    groupNode.SetAttribute("type", "System.Configuration.UserSettingsGroup");
                    configNode.AppendChild(groupNode);

                    // Create the Application section declaration and append to the groupNode above
                    // <section name="AppName.Properties.Settings"...>
                    var newSection = xmlDoc.CreateElement("section");
                    newSection.SetAttribute("name", APPNODE);
                    newSection.SetAttribute("type", "System.Configuration.ClientSettingsSection");
                    groupNode.AppendChild(newSection);

                    // Create the userSettings node and append to the root node
                    // <userSettings>
                    var userNode = xmlDoc.CreateElement(USERNODE);
                    xmlDoc.DocumentElement.AppendChild(userNode);

                    // Create the Application settings node and append to the userNode above
                    // <AppName.Properties.Settings>
                    var appNode = xmlDoc.CreateElement(APPNODE);
                    userNode.AppendChild(appNode);
                }

                return xmlDoc;
            }
        }

        // Override the Initialize method
        public override void Initialize(string name, NameValueCollection config)
        {
            entryAssembly = Assembly.GetEntryAssembly();

            ConfigDirectory = (new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase))).LocalPath;

            APPNODE = ApplicationName + ".Properties.Settings";

            base.Initialize(ApplicationName, config);
        }


        // Override the ApplicationName property, returning the solution name.  No need to set anything, we just need to
        // retrieve information, though the set method still needs to be defined.


        // Retrieve settings from the configuration file
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext sContext, SettingsPropertyCollection settingsColl)
        {
            // Create a collection of values to return
            var retValues = new SettingsPropertyValueCollection();

            // Create a temporary SettingsPropertyValue to reuse

            // Loop through the list of settings that the application has requested and add them
            // to our collection of return values.
            foreach (SettingsProperty sProp in settingsColl)
            {
                var setVal = new SettingsPropertyValue(sProp) { IsDirty = false, SerializedValue = GetSetting(sProp) };
                retValues.Add(setVal);
            }
            return retValues;
        }

        // Save any of the applications settings that have changed (flagged as "dirty")
        public override void SetPropertyValues(SettingsContext sContext, SettingsPropertyValueCollection settingsColl)
        {
            // Set the values in XML
            foreach (SettingsPropertyValue spVal in settingsColl)
            {
                SetSetting(spVal);
            }

            // Write the XML file to disk
            try
            {
                XMLConfig.Save(ConfigFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "] Error writing configuration file to disk: " + ex.Message);
            }
        }

        // Retrieve values from the configuration file, or if the setting does not exist in the file,
        // retrieve the value from the application's default configuration
        private object GetSetting(SettingsProperty setProp)
        {
            try
            {
                // Search for the specific settings node we are looking for in the configuration file.
                // If it exists, return the InnerText or InnerXML of its first child node, depending on the setting type.

                // If the setting is serialized as a string, return the text stored in the config
                if (setProp.SerializeAs.ToString() == "String")
                {
                    return XMLConfig.SelectSingleNode("//setting[@name='" + setProp.Name + "']").FirstChild.InnerText;
                }

                // This solves the problem with StringCollections throwing a NullReferenceException
                var xmlData = XMLConfig.SelectSingleNode(string.Format("//setting[@name='{0}']", setProp.Name)).FirstChild.InnerXml;
                return string.Format(@"{0}", xmlData);
            }
            catch (Exception)
            {
                // Check to see if a default value is defined by the application.
                // If so, return that value, using the same rules for settings stored as Strings and XML as above
                if ((setProp.DefaultValue == null))
                    return "";

                if (setProp.SerializeAs.ToString() == "String")
                    return setProp.DefaultValue.ToString();

                var settingType = setProp.PropertyType.ToString();
                var xmlData = setProp.DefaultValue.ToString();
                var xs = new XmlSerializer(typeof(string[]));
                var data = (string[])xs.Deserialize(new XmlTextReader(xmlData, XmlNodeType.Element, null));

                switch (settingType)
                {
                    case "System.Collections.Specialized.StringCollection":
                        var sc = new StringCollection();
                        sc.AddRange(data);
                        return sc;
                }

                return "";
            }

        }

        private void SetSetting(SettingsPropertyValue setProp)
        {
            // Define the XML path under which we want to write our settings if they do not already exist
            XmlNode SettingNode;

            try
            {
                // Search for the specific settings node we want to update.
                // If it exists, return its first child node, (the <value>data here</value> node)
                SettingNode = XMLConfig.SelectSingleNode(string.Format("//setting[@name='{0}']", setProp.Name)).FirstChild;
            }
            catch (Exception)
            {
                SettingNode = null;
            }

            // If we have a pointer to an actual XML node, update the value stored there
            if ((SettingNode != null))
            {
                if (setProp.Property.SerializeAs.ToString() == "String")
                {
                    SettingNode.InnerText = setProp.SerializedValue.ToString();
                    return;
                }

                // Write the object to the config serialized as Xml - we must remove the Xml declaration when writing
                // the value, otherwise .Net's configuration system complains about the additional declaration.
                SettingNode.InnerXml = setProp.SerializedValue.ToString().Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
                return;
            }

            // If the value did not already exist in this settings file, create a new entry for this setting

            // Search for the application settings node (<Appname.Properties.Settings>) and store it.
            var tmpNode = XMLConfig.SelectSingleNode(string.Format("//{0}", APPNODE)) ?? XMLConfig.SelectSingleNode(string.Format("//{0}.Properties.Settings", APPNODE));

            // Create a new settings node and assign its name as well as how it will be serialized
            var newSetting = xmlDoc.CreateElement("setting");
            newSetting.SetAttribute("name", setProp.Name);
            newSetting.SetAttribute("serializeAs", setProp.Property.SerializeAs.ToString() == "String" ? "String" : "Xml");

            // Append this node to the application settings node (<Appname.Properties.Settings>)
            tmpNode.AppendChild(newSetting);

            // Create an element under our named settings node, and assign it the value we are trying to save
            var valueElement = xmlDoc.CreateElement("value");
            if (setProp.Property.SerializeAs.ToString() == "String")
            {
                valueElement.InnerText = setProp.SerializedValue.ToString();
            }
            else
            {
                // Write the object to the config serialized as Xml - we must remove the Xml declaration when writing
                // the value, otherwise .Net's configuration system complains about the additional declaration
                valueElement.InnerXml = setProp.SerializedValue.ToString().Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", "");
            }

            //Append this new element under the setting node we created above
            newSetting.AppendChild(valueElement);
        }
    }
}
