using System.IO;
using System.Text;
using System;

namespace Net {
    public class ByteBuffer {


        MemoryStream stream = null;
        BinaryWriter writer = null;
        BinaryReader reader = null;


        #region 方法

        //直接封裝
        public ByteBuffer() {
            stream = new MemoryStream();
            writer = new BinaryWriter(stream);
        }

        public ByteBuffer(byte[] data) {
            if (data != null) {
                stream = new MemoryStream(data);
                reader = new BinaryReader(stream);

            } else {
                stream = new MemoryStream();
                writer = new BinaryWriter(stream);
            }
        }

        #endregion

        #region 將data寫入stream中

        public void WriteByte(byte data) {
            writer.Write(data);
        }

        public void WriteInt(int data) {
            writer.Write((int)data);
        }

        public void WriteShort(ushort data) {
            writer.Write((ushort)data);
        }

        public void WriteLong(long data) {
            writer.Write((long)data);
        }

        //陣列
        public void WriteFloat(float data) {
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToSingle(temp, 0));
        }

        public void WriteDouble(double data) {
            byte[] temp = BitConverter.GetBytes(data);
            Array.Reverse(temp);
            writer.Write(BitConverter.ToDouble(temp, 0));
        }

        public void WriteString(string data) {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            writer.Write((ushort)bytes.Length); //存入內容長度
            writer.Write(bytes);    //存入封包內容
        }

        public void WriteBytes(byte[] data) {
            writer.Write((int)data.Length);
            writer.Write(data);
        }

        #endregion






        #region 讀取

        public byte ReadByte() {
            return reader.ReadByte();
        }

        public int ReadInt() {
            return (int)reader.ReadInt32();
        }

        public ushort ReadShort() {
            return (ushort)reader.ReadInt16();
        }

        public long ReadLong() {
            return (long)reader.ReadInt64();
        }


        public float ReadFloat() {
            byte[] temp = BitConverter.GetBytes(reader.ReadSingle());
            Array.Reverse(temp);
            return BitConverter.ToSingle(temp, 0);
        }

        public double ReadDouble() {
            byte[] temp = BitConverter.GetBytes(reader.ReadDouble());
            Array.Reverse(temp);
            return BitConverter.ToDouble(temp, 0);
        }

        public string ReadString() {
            ushort len = ReadShort();   //將reader中的內容轉為Int16存到len中
            byte[] buffer = new byte[len];  //建立一個長度和內容一樣的容器裝內容
            buffer = reader.ReadBytes(len);//讀取len這麼長的內容存到buffer
            return Encoding.UTF8.GetString(buffer);//將buffer中的原始數據變成String後，使用UTF8解析。
        }

        public byte[] ReadBytes() {
            int len = ReadInt();
            return reader.ReadBytes(len);
        }

        #endregion



        #region 其他功能

        public void Close() {
            if (writer != null) writer.Close();
            if (reader != null) reader.Close();

            stream.Close();
            writer = null;
            reader = null;
            stream = null;
        }

        public byte[] ToBytes() {
            writer.Flush();
            return stream.ToArray();
        }

        public void Flush() {
            writer.Flush();
        }

        #endregion
    }

}