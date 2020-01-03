using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using Be.IO;
using textsolve = MyLibrary.MyDll;
namespace B2S_text_tool
{
    class BinaryReader2 : BinaryReader
    {
        public BinaryReader2(System.IO.Stream stream) : base(stream) { }

        public override int ReadInt32()
        {

            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public Int16 ReadInt16()
        {
            var data = base.ReadBytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public Int64 ReadInt64()
        {
            var data = base.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }

        public UInt32 ReadUInt32()
        {
            var data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToUInt32(data, 0);
        }

    }
    class Program
    {
        static string LittleEndian(string num)
        {
            int number = Convert.ToInt32(num, 16);
            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }
        public static int GetFirstOccurance(byte byteToFind, byte[] byteArray)
        {
            return Array.IndexOf(byteArray, byteToFind);
        }
        public static String UnicodeStr2HexStr(String strMessage)
        {

            byte[] ba = Encoding.Default.GetBytes(strMessage);
            String strHex = BitConverter.ToString(ba);
            strHex = strHex.Replace("-", "");
            return strHex;
        }
        public static int StringToInt(string intString)
        {
            //string intString = "234";
            int i = 0;
            if (!Int32.TryParse(intString, out i))
            {
                i = -1;
            }
            return i;
        }
        public static string ConvertByteArrayToUtf8(byte[] ba)
        {
            string converted = Encoding.UTF8.GetString(ba, 0, ba.Length);
            return converted;
        }
        public static byte[] RemoveByte255(byte[] ba, byte x)
        {
            ba = ba.Where(val => val != x).ToArray();
            return ba;
        }
        static string UnicodeToUTF8(string from)
        {
            var bytes = Encoding.UTF8.GetBytes(from);
            return new string(bytes.Select(b => (char)b).ToArray());
        }
        public static String HexBytes2UnicodeStr(byte[] ba)
        {
            var strMessage = Encoding.Unicode.GetString(ba, 0, ba.Length);
            return strMessage;
        }
        public static string count_solve(int count)
        {
            string output = "";
            if (count < 10)
            {
                output = "00" + count.ToString();
                return output;
            }
            if (count < 100)
            {
                output = "0" + count.ToString();
                return output;
            }
            if (count < 1000)
            {
                output = count.ToString();
                return output;
            }


            return output;
            
        }
        public static string key_solve(string key)
        {
            string output = "";
            for(int i =0; i < key.Count() - 16; i++)
            {
                output = output + key[i];
            }
            return output;
        }

        public static void extract(String text, String path, string path_export, int count)
        {
            BinaryReader2 BR = new BinaryReader2(new FileStream(path, FileMode.Open));

            string key = textsolve.UnicodeStr2HexStr_Bigend(text);
            Console.WriteLine(key);
            byte[] all_bytes = BR.ReadBytes(Convert.ToInt32(BR.BaseStream.Length));
            string all_bytes_hex = textsolve.ByteArrayToString(all_bytes);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int check_key = all_bytes_hex.IndexOf(key);
            if (check_key == -1)
                return;

            string file_name_ex;
            file_name_ex = Path.GetFileName(path);

            string folder;
            folder = Path.GetDirectoryName(path_export);

            folder = path_export + "\\" + file_name_ex;

            FileStream writeStream;
            writeStream = new FileStream(folder, FileMode.Create);
            BeBinaryWriter BW = new BeBinaryWriter(writeStream);
            int z = 0;
            while (check_key != -1)
            {
                string count_s = count_solve(count);

                check_key = check_key / 2;
                BW.Write(BR.ReadBytes(check_key - Convert.ToInt32(BR.BaseStream.Position)));
                string key1;
                key1 = key_solve(key);
                BW.Write(textsolve.StringToByteArray(key1));
                BW.Write(textsolve.StringToByteArray(textsolve.UnicodeStr2HexStr_Bigend(count_s)));
                BW.Write(textsolve.StringToByteArray(textsolve.UnicodeStr2HexStr_Bigend(z.ToString())));
                z = z + 1;
                if (z == 10)
                    z = 0;
                BR.ReadBytes(text.Count() * 2);

                int curr = Convert.ToInt32(BR.BaseStream.Position);
                byte[] all_bytes1 = BR.ReadBytes(Convert.ToInt32( - BR.BaseStream.Position + BR.BaseStream.Length));
                string all_bytes_hex1 = textsolve.ByteArrayToString(all_bytes1);
                BR.BaseStream.Seek(curr, SeekOrigin.Begin);
                check_key = all_bytes_hex1.IndexOf(key);
                if (check_key == -1)
                    break;
                check_key = check_key + curr * 2;
                //Console.WriteLine(check_key);
                //Console.WriteLine("ok");
            }

            BW.Write(BR.ReadBytes(Convert.ToInt32(-BR.BaseStream.Position + BR.BaseStream.Length)));
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.ForegroundColor = ConsoleColor.White;
            if (args.Length > 0)
            {
                string path = args[0];

                if (args[0] == "-e")
                {
                    string[] filePaths = Directory.GetFiles(args[1], "*.*",
                                           SearchOption.TopDirectoryOnly);


                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        string file_name_ex = Path.GetFileNameWithoutExtension(filePaths[i]);
                        Console.WriteLine(file_name_ex + ".txt");
                        extract(args[3], filePaths[i], args[2], i);
                    }
                    Console.WriteLine("\nDone");
                    Console.ReadKey();
                }
            }
            else
            {

                Console.WriteLine("\n        ====  Detect Text In Game tool ====\n        ============== Cloud ==============\n\nHow to use:\n - Export: Detect_Text_In_Game.exe -e folder_ori folder_new text_replace");
                Console.WriteLine("\n - text_replace >= 4");
                Console.ReadKey();
            }
        }
    }
}
