using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Kondyrev.Nsudotnet.Enigma
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 4)
            {
                using (SymmetricAlgorithm algorithm = ChooseAlgorithm(args[2].ToLower()))
                {
                    if (algorithm == null)
                    {
                        Console.WriteLine("Incorrect algorithm");
                        return;
                    }
                    try
                    {
                        switch (args[0].ToLower())
                        {
                            case "encrypt":
                                Encrypt(new FileInfo(args[1]), algorithm, new FileInfo(args[3]));
                                break;
                            case "decrypt":
                                if (args.Length < 5)
                                {
                                    Console.WriteLine("Incorrect parameters");
                                    return;
                                }
                                Decrypt(new FileInfo(args[1]), new FileInfo(args[3]), algorithm, new FileInfo(args[4]));
                                break;
                            default:
                                Console.WriteLine("Incorrect parameters");
                                return;
                        }
                    }
                    catch (CryptographicException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else
            {
                Console.WriteLine("Incorrect parameters");
            }
            Console.ReadLine();
        }

        static SymmetricAlgorithm ChooseAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "aes":
                    return Aes.Create();
                case "des":
                    return DES.Create();
                case "rc2":
                    return RC2.Create();
                case "rijndael":
                    return Rijndael.Create();
                default:
                    return null;
            }
        }

        static void Encrypt(FileInfo inputFile, SymmetricAlgorithm algorithm, FileInfo outputFile)
        {
            ICryptoTransform transform = algorithm.CreateEncryptor();
            try
            {
                using (FileStream output = outputFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    using (CryptoStream encryptor = new CryptoStream(output, transform, CryptoStreamMode.Write))
                    {
                        using (FileStream input = inputFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            input.CopyTo(encryptor);
                        }

                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            FileInfo key = new FileInfo(Path.GetFileNameWithoutExtension(inputFile.FullName) + ".key.txt");
            WriteKey(key, algorithm.Key, algorithm.IV);
        }

        static void Decrypt(FileInfo inputFile, FileInfo keyFile, SymmetricAlgorithm algorithm, FileInfo outputFile)
        {
            byte[][] info = ReadInfo(keyFile);
            if (info == null)
            {
                return;
            }
            algorithm.Key = info[0];
            algorithm.IV = info[1];

            ICryptoTransform transform = algorithm.CreateDecryptor();

            try
            {
                using (FileStream output = outputFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    using (CryptoStream decryptor = new CryptoStream(output, transform, CryptoStreamMode.Write))
                    {
                        using (FileStream input = inputFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            input.CopyTo(decryptor);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static byte[][] ReadInfo(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    byte[] lengthBuffer = new byte[8];
                    stream.Read(lengthBuffer, 0, lengthBuffer.Length);
                    int length = BitConverter.ToInt32(FromBase64(lengthBuffer), 0);
                    byte[] keyBuffer = new byte[length];
                    stream.Read(keyBuffer, 0, keyBuffer.Length);
                    keyBuffer = FromBase64(keyBuffer);

                    stream.Read(lengthBuffer, 0, lengthBuffer.Length);
                    length = BitConverter.ToInt32(FromBase64(lengthBuffer), 0);
                    byte[] ivBuffer = new byte[length];
                    stream.Read(ivBuffer, 0, ivBuffer.Length);
                    ivBuffer = FromBase64(ivBuffer);

                    byte[][] info = new byte[2][];
                    info[0] = keyBuffer;
                    info[1] = ivBuffer;
                    return info;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static void WriteKey(FileInfo file, byte[] key, byte[] iv)
        {
            using (FileStream stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                key = ToBase64(key);
                byte[] keyLength = BitConverter.GetBytes(key.Length);
                keyLength = ToBase64(keyLength);
                stream.Write(keyLength, 0, keyLength.Length);
                stream.Write(key, 0, key.Length);

                iv = ToBase64(iv);
                byte[] ivLength = BitConverter.GetBytes(iv.Length);
                ivLength = ToBase64(ivLength);
                stream.Write(ivLength, 0, ivLength.Length);
                stream.Write(iv, 0, iv.Length);
            }
        }

        static byte[] ToBase64(byte[] data)
        {
            string str = Convert.ToBase64String(data);
            return Encoding.ASCII.GetBytes(str);
        }

        static byte[] FromBase64(byte[] data)
        {
            return Convert.FromBase64String(Encoding.ASCII.GetString(data));
        }
    }
}
