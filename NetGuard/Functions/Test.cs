using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

public class SecureProtection
{
    private static Mutex mutex = null;

    // Importing necessary functions from Windows API
    [DllImport("kernel32.dll")]
    private static extern bool IsDebuggerPresent();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern void OutputDebugString(string lpOutputString);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern uint GetLastError();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern int NtQueryInformationProcess(IntPtr processHandle, PROCESSINFOCLASS processInformationClass, ref int processInformation, int processInformationLength, out int returnLength);

    [DllImport("ntdll.dll", ExactSpelling = true, SetLastError = false)]
    private static extern IntPtr NtCurrentTeb();

    [DllImport("ntdll.dll")]
    private static extern uint NtRaiseHardError(uint ErrorStatus, uint NumberOfParameters, IntPtr UnicodeStringParameterMask, IntPtr Parameters, uint ValidResponseOptions, out uint Response);

    private enum PROCESSINFOCLASS
    {
        ProcessDebugPort = 7
    }

    private const uint STATUS_ACCESS_VIOLATION = 0xC0000005;
    private const uint OptionShutdownSystem = 6;

    // Method to check if a debugger is attached using various techniques
    public static bool IsDebuggerAttached()
    {
        // Check Debugger.IsAttached
        if (Debugger.IsAttached)
        {
            return true;
        }

        // Check IsDebuggerPresent
        if (IsDebuggerPresent())
        {
            return true;
        }

        // Check OutputDebugString
        OutputDebugString("Test");
        if (GetLastError() == 0)
        {
            return true;
        }

        // Check CheckRemoteDebuggerPresent
        bool isRemoteDebuggerPresent = false;
        CheckRemoteDebuggerPresent(GetCurrentProcess(), ref isRemoteDebuggerPresent);
        if (isRemoteDebuggerPresent)
        {
            return true;
        }

        // Check Debug Port
        int debugPort = 0;
        int returnLength;
        int status = NtQueryInformationProcess(GetCurrentProcess(), PROCESSINFOCLASS.ProcessDebugPort, ref debugPort, Marshal.SizeOf(debugPort), out returnLength);
        if (status == 0 && debugPort != 0)
        {
            return true;
        }

        // Check Debugger Flag in PEB
        IntPtr teb = NtCurrentTeb();
        IntPtr peb = Marshal.ReadIntPtr(teb, 0x30);
        byte beingDebugged = Marshal.ReadByte(peb, 0x02);
        if (beingDebugged != 0)
        {
            return true;
        }

        return false;
    }

    // Method to induce a blue screen (for educational purposes only)
    public static void InduceBlueScreen()
    {
        uint response;
        NtRaiseHardError(STATUS_ACCESS_VIOLATION, 0, IntPtr.Zero, IntPtr.Zero, OptionShutdownSystem, out response);
    }

    // Method to get the Windows installation date
    public static DateTime GetWindowsInstallationDate()
    {
        try
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    string installDate = obj["InstallDate"].ToString();
                    return ManagementDateTimeConverter.ToDateTime(installDate);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving Windows installation date: " + ex.Message);
        }

        return DateTime.MinValue;
    }

    // Method to get the MAC addresses of all network interfaces
    public static string GetMACAddresses()
    {
        try
        {
            string macAddresses = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString() + ";";
                }
            }
            return macAddresses.TrimEnd(';');
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving MAC addresses: " + ex.Message);
        }

        return string.Empty;
    }

    // Method to get the serial numbers of all hard drives
    public static string GetHardDriveSerialNumbers()
    {
        try
        {
            string serialNumbers = "";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    serialNumbers += obj["SerialNumber"].ToString().Trim() + ";";
                }
            }
            return serialNumbers.TrimEnd(';');
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving hard drive serial numbers: " + ex.Message);
        }

        return string.Empty;
    }

    // Method to get a unique hardware ID
    public static string GetHardwareID()
    {
        string hardwareID = "";
        try
        {
            hardwareID += "WindowsInstallDate:" + GetWindowsInstallationDate().ToString("yyyy-MM-ddTHH:mm:ss") + ";";
            hardwareID += "MAC:" + GetMACAddresses() + ";";
            hardwareID += "HDD:" + GetHardDriveSerialNumbers() + ";";

            // Additional hardware identifiers (optional)
            hardwareID += "CPUID:" + GetCPUID() + ";";
            hardwareID += "MotherboardID:" + GetMotherboardID() + ";";
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating hardware ID: " + ex.Message);
        }

        return hardwareID;
    }

    // Method to ensure single instance of the application
    public static bool EnsureSingleInstance(string mutexName)
    {
        bool createdNew;
        mutex = new Mutex(true, mutexName, out createdNew);
        return createdNew;
    }

    // Method to compute the hash of a file
    public static string ComputeFileHash(string filePath)
    {
        using (FileStream stream = File.OpenRead(filePath))
        {
            SHA256 sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

    // Method to verify the integrity of the executable
    public static bool VerifyExecutableIntegrity(string expectedHash)
    {
        try
        {
            string currentPath = Process.GetCurrentProcess().MainModule.FileName;
            string currentHash = ComputeFileHash(currentPath);
            return currentHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error verifying executable integrity: " + ex.Message);
        }

        return false;
    }

    public static string Encrypt(string clearText, string key)
    {
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x43, 0x87, 0x23, 0x72, 0x20, 0x5A, 0xB4, 0x47 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }

    public static string Decrypt(string cipherText, string key)
    {
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, new byte[] { 0x43, 0x87, 0x23, 0x72, 0x20, 0x5A, 0xB4, 0x47 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

    // Method to get the CPU ID
    public static string GetCPUID()
    {
        try
        {
            string cpuID = "";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    cpuID += obj["ProcessorId"].ToString();
                }
            }
            return cpuID;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving CPU ID: " + ex.Message);
        }

        return string.Empty;
    }

    // Method to get the Motherboard ID
    public static string GetMotherboardID()
    {
        try
        {
            string motherboardID = "";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    motherboardID += obj["SerialNumber"].ToString();
                }
            }
            return motherboardID;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error retrieving motherboard ID: " + ex.Message);
        }

        return string.Empty;
    }

    // Method to check if known tampering tools are in the process tree
    public static bool IsTamperingToolPresent()
    {
        string[] tamperingTools = { "ollydbg", "x64dbg", "ida", "windbg", "Ghidra", "Cheat Engine", "Radare2", "Immunity Debugger", "PE Explorer", "Binary Ninja", "Hopper Disassembler", "GDB", "dnSpy" };


        try
        {
            foreach (Process process in Process.GetProcesses())
            {
                foreach (string tool in tamperingTools)
                {
                    if (process.ProcessName.ToLower().Contains(tool))
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error checking for tampering tools: " + ex.Message);
        }

        return false;
    }

    // Method to check if network analysis tools are present
    public static bool IsNetworkAnalysisToolPresent()
    {
        string[] networkTools = { "wireshark", "fiddler", "charles", "tcpdump", "Ettercap", "Cain & Abel", "Nmap", "ZAP", "Tshark", "Nessus", "Burp Suite", "Snort", "Suricata", "Metasploit", "Zenmap" };

        try
        {
            foreach (Process process in Process.GetProcesses())
            {
                foreach (string tool in networkTools)
                {
                    if (process.ProcessName.ToLower().Contains(tool))
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error checking for network analysis tools: " + ex.Message);
        }

        return false;
    }
}

class Program
{
    // Replace this with the actual hash of your executable
    private const string ExpectedHash = "your_precomputed_hash_here";
    // Encryption key for securing data
    private const string EncryptionKey = "your_encryption_key_here";

    static void Main32()
    {
        string mutexName = "MyUniqueApplicationMutexName";

        if (!SecureProtection.EnsureSingleInstance(mutexName))
        {
            Console.WriteLine("Another instance of the application is already running.");
            return;
        }

        if (SecureProtection.IsDebuggerAttached())
        {
            Console.WriteLine("Debugger is attached.");
            SecureProtection.InduceBlueScreen(); // For educational purposes only
            return;
        }

        if (!SecureProtection.VerifyExecutableIntegrity(ExpectedHash))
        {
            Console.WriteLine("Executable integrity check failed. The file may have been tampered with.");
            return;
        }

        string hardwareID = SecureProtection.GetHardwareID();
        Console.WriteLine("Hardware ID: " + hardwareID);

        // Encrypt the hardware ID
        string encryptedHardwareID = SecureProtection.Encrypt(hardwareID, EncryptionKey);
        Console.WriteLine("Encrypted Hardware ID: " + encryptedHardwareID);

        // Decrypt the hardware ID
        string decryptedHardwareID = SecureProtection.Decrypt(encryptedHardwareID, EncryptionKey);
        Console.WriteLine("Decrypted Hardware ID: " + decryptedHardwareID);

        // Check for known tampering tools
        if (SecureProtection.IsTamperingToolPresent())
        {
            Console.WriteLine("Known tampering tools detected in the process tree.");
            // Optionally take appropriate action such as terminating the application
            return;
        }

        // Check for network analysis tools
        if (SecureProtection.IsNetworkAnalysisToolPresent())
        {
            Console.WriteLine("Network analysis tools detected.");
            // Optionally take appropriate action such as terminating the application
            return;
        }

        // Keep the application running to demonstrate the single instance enforcement
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        // Release the mutex when the application exits
        //Mutex.ReleaseMutex();
    }
}