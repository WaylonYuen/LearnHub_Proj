using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Setup;
using Util;

namespace LearnHub.Data {

	public static class LocalData {

        /// <summary>
        /// ID讀寫: 資料保存在PlayerID.txt 0～8 byte
        /// </summary>
        public static long Identity {

            get {
                ClientUtil clientUtil = new ClientUtil();

                byte[] ID_Byte = clientUtil.FileSet(SrcPath.ID, null, 8, FileMode.Open, FileAccess.Read, 0, 0); //讀取資料
                long ID = BitConverter.ToInt64(ID_Byte, 0);                              //轉換型態 Byte -> long

                return ID;
            }

            set {
                ClientUtil clientUtil = new ClientUtil();
                clientUtil.FileSet(SrcPath.ID, BitConverter.GetBytes(value), 8, FileMode.Open, FileAccess.Write, 0, 0);
            }
        }

        /// <summary>
        /// PW讀寫: 資料保存在PlayerID.txt [長度: 8～12 byte][內容: 12～n]
        /// </summary>
        public static string[] Password {

            get {
                ClientUtil clientUtil = new ClientUtil();

                string[] PW = { "N/A" };            //Password容器

                byte[] length_Byte = clientUtil.FileSet(SrcPath.ID, null, 4, FileMode.OpenOrCreate, FileAccess.Read, 0, 8);
                int length = BitConverter.ToInt32(length_Byte, 0);

                byte[] PW_Byte = clientUtil.FileSet(SrcPath.ID, null, length, FileMode.OpenOrCreate, FileAccess.Read, 0, 12);
                PW[0] = Encoding.UTF8.GetString(PW_Byte);

                return PW;
            }

            set {
                ClientUtil clientUtil = new ClientUtil();

                string[] PW = value;
                byte[] PW_Byte = Encoding.UTF8.GetBytes(PW[0]);

                clientUtil.FileSet(SrcPath.ID, BitConverter.GetBytes(PW_Byte.Length), 4, FileMode.Open, FileAccess.Write, 0, 8);
                clientUtil.FileSet(SrcPath.ID, PW_Byte, PW_Byte.Length, FileMode.Open, FileAccess.Write, 0, 12);

            }
        }

    }

}
