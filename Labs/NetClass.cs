using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Labs
{
    public static class NativeTest
    {

        private const string path = "NetExtenstion.dll";

        [DllImport(path, CallingConvention = CallingConvention.Cdecl)]
        private extern static string getMAC();

        public static string GetNativeMacAdress()
        {
            return getMAC();
        }

        public static string GetMACAdressOnly()
        {
            return NetworkInterface
        .GetAllNetworkInterfaces()
        .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
        .Select(nic => nic.GetPhysicalAddress().ToString())
        .FirstOrDefault();
        }

        public static string GetFullMACAdress()
        {
            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                Console.WriteLine(
                    "Found MAC Address: " + nic.GetPhysicalAddress() +
                    " Type: " + nic.NetworkInterfaceType);

                string tempMac = nic.GetPhysicalAddress().ToString();
                if (nic.Speed > maxSpeed &&
                    !string.IsNullOrEmpty(tempMac) &&
                    tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                {
                    Console.WriteLine("New Max Speed = " + nic.Speed + ", MAC: " + tempMac);
                    maxSpeed = nic.Speed;
                    macAddress = tempMac;
                }
            }

            return macAddress;
        }

        public static string GetWorkgoup()
        {
            ManagementObject computer_system = new ManagementObject(
                    string.Format(
                    "Win32_ComputerSystem.Name='{0}'",
                    Environment.MachineName));
            object result = computer_system["Workgroup"];
            return result.ToString();
        }

        [DllImport(path, CallingConvention = CallingConvention.Cdecl)]
        private extern static bool get_work_group();
        public static bool GetNativeWorkgroup()
        {
            return get_work_group();
        }
    }



}
