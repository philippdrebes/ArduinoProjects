using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace StatusMonitor.Business
{
    public class AppSettingsHelper
    {
        private IFormatter _formatter = new BinaryFormatter();
        private string _filepath = $"{AppDomain.CurrentDomain.BaseDirectory}\\usersettings.stm";

        private static AppSettingsHelper _instance = null;
        public static AppSettingsHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AppSettingsHelper();
                return _instance;
            }
        }

        private AppSettings _settings = null;
        public AppSettings Settings { get { return _settings; } }

        private AppSettingsHelper()
        {
            Initialize();
        }

        private void Initialize()
        {
            try
            {
                if (File.Exists(_filepath))
                {
                    DeserializeSettings(_filepath);
                }
                else
                {
                    _settings = new AppSettings();
                    SerializeSettings(_filepath);
                }
            }
            catch (Exception ex)
            {
                // TODO
            }
        }

        public void Save()
        {
            SerializeSettings(_filepath);
        }

        public void Load()
        {
            DeserializeSettings(_filepath);
        }

        private void SerializeSettings(string fileName)
        {
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Create))
                {
                    _formatter.Serialize(stream, _settings);
                }
            }
            catch (Exception ex)
            {
                // TODO
            }
        }


        private void DeserializeSettings(string fileName)
        {
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    _settings = (AppSettings)_formatter.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                // TODO
            }
        }
    }
}
