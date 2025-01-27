using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        //리드버퍼
        ArraySegment<byte> _buffer;
        int _readPos;
        int _writePos;

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return _writePos- _readPos; } }
        public int FreeSize { get { return _buffer.Count -_writePos; } }

        public ArraySegment<byte> ReadSegment //데이터의 유효범위, w r 차이
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }
        public ArraySegment<byte> WriteSegment //받을때 어디서부터 어디까지가 유효인지
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean() // r w 를 초기로 땡겨주는 녀석
        {
            int dataSize = DataSize;
            if(dataSize == 0)
            {
                //남은 데이터가 없으면 복사하지 않고 커서 위치 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                // 남은 데이터가 있다.
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }

        }

        public bool OnRead(int numOfBytes)
        {
            // 성공적으로 처리하면 커서 이동
            if (numOfBytes > DataSize)
                return false;

            _readPos += numOfBytes;
            return true;
        }
        public bool OnWirte(int numOfBytes)
        {
            // 성공적으로 처리하면 커서 이동
            if (numOfBytes > FreeSize)
                return false;

            _writePos += numOfBytes;
            return true;
        }
    }
}
