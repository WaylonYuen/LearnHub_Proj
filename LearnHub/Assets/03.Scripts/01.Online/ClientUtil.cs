using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Util {

    /// <summary>
    /// 客戶端工具包
    /// </summary>
    public class ClientUtil {

        /// <summary>
        /// 文件讀寫
        /// </summary>
        /// <param name="srcPath">讀取路徑</param>
        /// <param name="Save_Byte">需寫入的內容</param>
        /// <param name="DataSize">內容大小</param>
        /// <param name="fileMode">處理模式</param>
        /// <param name="fileAccess">處理方法</param>
        /// <param name="Offset">讀寫偏差值</param>
        /// <param name="ReadPosition">讀寫起點</param>
        /// <returns>讀取到的資料</returns>
        public byte[] FileSet(string srcPath, byte[] Save_Byte, int DataSize, FileMode fileMode, FileAccess fileAccess, int Offset, long ReadPosition) {

            byte[] data_Byte = new byte[DataSize];   //創建容器

            if (Save_Byte != null) data_Byte = Save_Byte;

            if (File.Exists(srcPath)) {
                using (FileStream fileStream = new FileStream(srcPath, fileMode, fileAccess)) {
                    fileStream.Position = ReadPosition;
                    if(fileAccess == FileAccess.Read)   fileStream.Read(data_Byte, Offset, data_Byte.Length);
                    if(fileAccess == FileAccess.Write)  fileStream.Write(data_Byte, Offset, data_Byte.Length);
                }
            } else {
                Debug.Log($"#  Warning: Cannot found the Path\t Info [Path: {srcPath} ]");
            }
            return data_Byte;
        }

    }

}
