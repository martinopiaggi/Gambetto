using UnityEngine;

namespace POLIMIGameCollective.Scripts.UniqueIdentifier
{
    public class UniqueIdentifier
    {
        public static string Identifier()
        {
            if (SystemInfo.deviceUniqueIdentifier != SystemInfo.unsupportedIdentifier)
            {
                return SystemInfo.deviceUniqueIdentifier;
            }
            else
            {
                string DeviceID = PlayerPrefs.GetString("DeviceID", "null");
                if (DeviceID == "null")
                {
                    DeviceID = Utility.Utility.RandomString(20);
                    PlayerPrefs.SetString("DeviceId", DeviceID);
                }

                return DeviceID;
            }
        }
    }
}
