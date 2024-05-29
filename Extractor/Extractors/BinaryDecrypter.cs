﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Extractor.Extractors
{
    internal static class BinaryDecrypter
    {
        private static readonly byte[] Key = new byte[] { 48, 239, 114, 71, 66, 242, 4, 50 };
        private static readonly byte[] Iv = new byte[] { 14, 166, 220, 137, 219, 237, 220, 79 };

        public static void DecryptBinaryFile(string inputPath, Stream outputStream)
        {
            using (var inputFile = File.OpenRead(inputPath))
            {
                var fileBuffer = new byte[inputFile.Length];
                inputFile.Read(fileBuffer, 0, fileBuffer.Length);

                var tDES = new DESCryptoServiceProvider
                {
                    IV = Iv,
                    Mode = CipherMode.CBC,
                    Key = Key
                };
                var outBuffer = tDES.CreateDecryptor().TransformFinalBlock(fileBuffer, 0, fileBuffer.Length);

                const int size = 4096;
                var buffer = new byte[size];
                int bytesRead = 0;
                using (GZipStream decompression = new GZipStream(new MemoryStream(outBuffer), CompressionMode.Decompress))
                {
                    while ((bytesRead = decompression.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }
}