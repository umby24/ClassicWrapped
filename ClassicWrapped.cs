using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace ClassicWrapped
{
    public class ClassicWrapped
    {
        NetworkStream _Stream;
        Byte[] Buffer;

        public ClassicWrapped(NetworkStream Stream) {
            _Stream = Stream;
        }

        /*
         * Byte
         * 
         */
        public Byte readByte() {
            return (byte)_Stream.ReadByte();
        }
        public SByte readSByte() {
            return (SByte)_Stream.ReadByte();
        }

        public void writeByte(byte write) {
            _Stream.WriteByte(write);
        }
        public void writeSByte(SByte write) {
            _Stream.WriteByte((byte)write);
        }

        /*
         * Short *
         */

        public short readShort() {
            byte[] bytes = readByteArray(2);
            Array.Reverse(bytes);

            return BitConverter.ToInt16(bytes, 0);
        }
        public void writeShort(short sending) {
            byte[] bytes = BitConverter.GetBytes(sending);
            Array.Reverse(bytes);
            _Stream.Write(bytes, 0, 2);
        }

        /*
         * String
         */

        public string readString() {
            return BitConverter.ToString(readByteArray(64),0);
        }
        public void writeString(string sending) {
            _Stream.Write(Encoding.ASCII.GetBytes(sending), 0, sending.Length);
        }

        // Byte Arrays

        public void writeByteArray(byte[] bytes) {
            _Stream.Write(bytes, 0, bytes.Length);
        }
        public Byte[] readByteArray(int size) {
            byte[] myBytes = new byte[size];
            int BytesRead;

            BytesRead = _Stream.Read(myBytes, 0, size);

            while (true) {
                if (BytesRead != size) {
                    int newSize = size - BytesRead;
                    int byteRead = _Stream.Read(myBytes, BytesRead - 1, newSize);

                    if (byteRead != newSize) {
                        size = newSize;
                        BytesRead = byteRead;
                    } else
                        break;

                } else
                    break;
            }

            return myBytes;
        }

        // "Send" and Purge

        void send(byte[] bArray) {
            if (Buffer != null) {
                int tempLength = Buffer.Length + bArray.Length;
                byte[] tempBuff = new byte[tempLength];

                Array.Copy(Buffer, tempBuff, Buffer.Length);
                Array.Copy(bArray, 0, tempBuff, Buffer.Length, bArray.Length);

                Buffer = tempBuff;
            } else {
                Buffer = bArray;
            }
        }

        public void Purge() {
            _Stream.Write(Buffer, 0, Buffer.Length);
            Buffer = null;
        }
    }
}
