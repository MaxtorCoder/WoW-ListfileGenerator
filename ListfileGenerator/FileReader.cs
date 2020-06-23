using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ListfileGenerator
{
    public static class FileReader
    {
        public static void Process(string filename)
        {
            if (filename.EndsWith(".m2"))
                ProcessM2(filename);
            if (filename.EndsWith(".adt"))
                ProcessADT(filename);
            if (filename.EndsWith(".wmo"))
                ProcessWMO(filename);
        }

        public static void ProcessM2(string filename)
        {
            using (var stream = File.Open(filename, FileMode.Open))
            using (var reader = new BinaryReader(stream))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var chunkId = new string(reader.ReadChars(4));
                    var chunkSize = reader.ReadUInt32();

                    switch (chunkId)
                    {
                        case "AFID":    // Animation Files
                            for (var i = 0; i < chunkSize / 8; ++i)
                            {
                                reader.ReadUInt32();
                                var animFileId = reader.ReadUInt32();
                                if (animFileId != 0)
                                    Console.WriteLine($"Filename: {Listfile.GetFilename(filename, animFileId)} FiledataId: {animFileId}");
                            }
                            break;
                        case "BFID":    // Bones
                        case "SFID":    // Skin Files
                        case "TXID":    // Texture Files
                            for (var i = 0; i < chunkSize / 4; ++i)
                            {
                                var fileDataId = reader.ReadUInt32();
                                if (fileDataId != 0)
                                    Console.WriteLine($"Filename: {Listfile.GetFilename(filename, fileDataId)} FiledataId: {fileDataId}");
                            }
                            break;
                        default:
                            reader.BaseStream.Position += chunkSize;
                            break;
                    }
                }

                stream.Close();
                reader.Close();
            }
        }

        public static void ProcessWMO(string filename)
        {
            using (var stream = File.Open(filename, FileMode.Open))
            using (var reader = new BinaryReader(stream))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var chunkId = new string(reader.ReadChars(4).Reverse().ToArray());
                    var chunkSize = reader.ReadUInt32();

                    switch (chunkId)
                    {
                        case "MODI":    // Doodads
                            for (var i = 0; i < chunkSize / 4; ++i)
                            {
                                var fileDataId = reader.ReadUInt32();
                                if (fileDataId != 0)
                                    Console.WriteLine($"Filename: {Listfile.GetFilename(filename, fileDataId)} FiledataId: {fileDataId}");
                            }
                            break;
                        case "MOMT":    // Textures
                            for (var i = 0; i < chunkSize / 64; ++i)
                            {
                                // Skip Shader + BlendMode
                                reader.ReadUInt64();
                                reader.ReadUInt32();

                                var fileDataId = reader.ReadUInt32();
                                if (fileDataId != 0)
                                    Console.WriteLine($"Filename: {Listfile.GetFilename(filename, fileDataId)} FiledataId: {fileDataId}");

                                reader.ReadUInt64();

                                fileDataId = reader.ReadUInt32();
                                if (fileDataId != 0)
                                    Console.WriteLine($"Filename: {Listfile.GetFilename(filename, fileDataId)} FiledataId: {fileDataId}");

                                reader.ReadUInt64();

                                fileDataId = reader.ReadUInt32();
                                if (fileDataId != 0)
                                    Console.WriteLine($"Filename: {Listfile.GetFilename(filename, fileDataId)} FiledataId: {fileDataId}");

                                reader.BaseStream.Position += (16 + 4 + 4);
                            }
                            break;
                        default:
                            reader.BaseStream.Position += chunkSize;
                            break;
                    }
                }

                stream.Close();
                reader.Close();
            }
        }

        public static void ProcessADT(string filename)
        {
            using (var stream = File.Open(filename, FileMode.Open))
            using (var reader = new BinaryReader(stream))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var chunkId = new string(reader.ReadChars(4).Reverse().ToArray());
                    var chunkSize = reader.ReadUInt32();

                    switch (chunkId)
                    {
                        // ADT
                        case "MDID":    // Diffuse Textures
                        case "MHID":    // Height Textures
                            for (var i = 0; i < chunkSize / 4; ++i)
                            {
                                var fileDataId = reader.ReadUInt32();
                                if (fileDataId != 0)
                                    Console.WriteLine($"Filename: {Listfile.GetFilename(filename, fileDataId)} FiledataId: {fileDataId}");
                            }
                            break;
                        case "MDDF":    // M2 Models
                            for (var i = 0; i < chunkSize / 36; ++i)
                            {
                                var fileDataId = reader.ReadUInt32();
                                if (fileDataId != 0)
                                    Console.WriteLine($"Filename: {Listfile.GetFilename(filename, fileDataId)} FiledataId: {fileDataId}");

                                reader.BaseStream.Position += sizeof(uint) * 8;
                            }
                            break;
                        case "MODF":    // WMO Models
                            for (var i = 0; i < chunkSize / 64; ++i)
                            {
                                var fileDataId = reader.ReadUInt32();
                                if (fileDataId != 0)
                                    Console.WriteLine($"Filename: {Listfile.GetFilename(filename, fileDataId)} FiledataId: {fileDataId}");

                                reader.BaseStream.Position += sizeof(uint) * 15;
                            }
                            break;
                        default:
                            reader.BaseStream.Position += chunkSize;
                            break;
                    }
                }

                stream.Close();
                reader.Close();
            }
        }
    }
}
