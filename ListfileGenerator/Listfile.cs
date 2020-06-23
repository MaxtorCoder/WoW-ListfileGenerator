using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ListfileGenerator
{
    public static class Listfile
    {
        private static Dictionary<uint, string> listfileEntries = new Dictionary<uint, string>();
        private static Dictionary<string, List<ListfileEntry>> readfileEntries = new Dictionary<string, List<ListfileEntry>>();

        public static void Prepare()
        {
            using (var reader = new StreamReader("listfile.csv"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var lineSplit = line.Split(";", StringSplitOptions.RemoveEmptyEntries);

                    listfileEntries.Add(uint.Parse(lineSplit[0]), lineSplit[1]);
                }
            }
        }

        public static string GetFilename(string filename, uint fileDataId)
        {
            if (!listfileEntries.TryGetValue(fileDataId, out var fileEntryName))
            {
                Console.WriteLine($"{fileDataId} does not exist in the listfile");
                return string.Empty;
            }

            filename = filename.Replace("\\", "/");
            if (!readfileEntries.ContainsKey(filename))
                readfileEntries.Add(filename, new List<ListfileEntry>());

            readfileEntries[filename].Add(new ListfileEntry
            {
                FileDataId = fileDataId,
                Filename = fileEntryName
            });

            return fileEntryName;
        }

        public static void FinishListfile()
        {
            using (var writer = new StreamWriter("exported-listfile.csv"))
            {
                foreach (var entry in readfileEntries)
                {
                    // writer.WriteLine(entry.Key);
                    foreach (var fileEntry in entry.Value)
                    {
                        writer.WriteLine($"{fileEntry.FileDataId};{ fileEntry.Filename}");
                    }

                    writer.WriteLine();
                }
            }
        }
    }

    public struct ListfileEntry
    {
        public string Filename;
        public uint FileDataId;
    }
}
