using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;

namespace ServoCS.Servo
{
    static class Config
    {
        private static readonly Dictionary<string, string> ConfKV;
        private const string ConfBucket = "Servo/Conf/";

        static Config()
        {
            var confQuery = from x in ConfigurationManager.AppSettings.AllKeys
                            where !string.IsNullOrWhiteSpace(x) && x.StartsWith(ConfBucket)
                            select new { K = x, V = ConfigurationManager.AppSettings[x] };
            ConfKV = confQuery.ToDictionary(x => x.K, x => x.V);
        }

        private static void ReadConf(out string key, out string @value, [CallerMemberName] string name = null)
        {
            key = string.Format("{0}{1}", ConfBucket, name);
            ConfKV.TryGetValue(key, out @value);
        }

        public static string ServiceName
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                return val;
            }
        }

        public static string DisplayName
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                return val;
            }
        }

        public static ServiceStartMode ServiceStartMode
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                ServiceStartMode mode;
                if (!Enum.TryParse(val, out mode)) throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return mode;
            }
        }

        public static string Description
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                return val;
            }
        }

        public static string[] ServicesDependedOn
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val)) return new string[] { };

                return val.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public static bool DelayedAutoStart
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                bool boolVal;
                if (!bool.TryParse(val, out boolVal)) throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return boolVal;
            }
        }

        public static bool CanHandlePowerEvent
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val))
                    //throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");
                    // to maintain backward compatibility
                    val = "false";

                bool boolVal;
                if (!bool.TryParse(val, out boolVal))
                    throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return boolVal;
            }
        }

        public static bool CanHandleSessionChangeEvent
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val))
                    //throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");
                    // to maintain backward compatibility
                    val = "false";

                bool boolVal;
                if (!bool.TryParse(val, out boolVal))
                    throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return boolVal;
            }
        }

        public static bool CanPauseAndContinue
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val))
                    //throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");
                    // to maintain backward compatibility
                    val = "false";

                bool boolVal;
                if (!bool.TryParse(val, out boolVal))
                    throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return boolVal;
            }
        }

        public static bool CanShutdown
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val))
                    //throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");
                    // to maintain backward compatibility
                    val = "false";

                bool boolVal;
                if (!bool.TryParse(val, out boolVal))
                    throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return boolVal;
            }
        }

        public static bool CanStop
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val))
                    //throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");
                    // to maintain backward compatibility
                    val = "true";

                bool boolVal;
                if (!bool.TryParse(val, out boolVal))
                    throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return boolVal;
            }
        }

        public static bool AutoLog
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val))
                    //throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");
                    // to maintain backward compatibility
                    val = "true";

                bool boolVal;
                if (!bool.TryParse(val, out boolVal))
                    throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return boolVal;
            }
        }

        public static bool RunInService
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val)) throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");

                bool boolVal;
                if (!bool.TryParse(val, out boolVal)) throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return boolVal;
            }
        }

        public static int StartTimeout
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val))
                    //throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");
                    // to maintain backward compatibility
                    val = "60";

                int ni;
                if (!int.TryParse(val, out ni)) throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return ni;
            }
        }

        public static int StopTimeout
        {
            get
            {
                string key, val;
                ReadConf(out key, out val);
                if (string.IsNullOrWhiteSpace(val))
                    //throw new SettingsPropertyNotFoundException("app setting " + key + " is not set");
                    // to maintain backward compatibility
                    val = "60";

                int ni;
                if (!int.TryParse(val, out ni)) throw new SettingsPropertyNotFoundException("app setting " + key + " has a wrong value");

                return ni;
            }
        }

        public static string String()
        {
            var pairStrs = ConfKV.Select(x => string.Format("{0}: `{1}`", x.Key, x.Value));
            return string.Join("; ", pairStrs);
        }
    }
}