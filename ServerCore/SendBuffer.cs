using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
	public class SendBufferHelper
	{
		// 쓰레드로컬은 전역이지만 나만 사용가능.
		public static ThreadLocal<SendBuffer> CurrentBuffer = 
			new ThreadLocal<SendBuffer>(() => { return null; });

		public static int ChunkSize { get; set; } = 4096 * 100;

		public static ArraySegment<byte> Open(int reserveSize)
		{
			if (CurrentBuffer.Value == null)//사용한거없음
				CurrentBuffer.Value = new SendBuffer(ChunkSize);

			if (CurrentBuffer.Value.FreeSize < reserveSize) //있는데 범위를 넘어감
				CurrentBuffer.Value = new SendBuffer(ChunkSize); // 다시 새로만들기

			return CurrentBuffer.Value.Open(reserveSize);
		}

		public static ArraySegment<byte> Close(int usedSize)
		{
			return CurrentBuffer.Value.Close(usedSize);
		}
	}

	public class SendBuffer
	{
		byte[] _buffer;
		int _usedSize = 0;

		public int FreeSize { get { return _buffer.Length - _usedSize; } }

		public SendBuffer(int chunkSize)
		{
			_buffer = new byte[chunkSize];
		}

		public ArraySegment<byte> Open(int reserveSize)
		{
			if (reserveSize > FreeSize)
				return default;

			return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
		}

		public ArraySegment<byte> Close(int usedSize)
		{
			ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
			_usedSize += usedSize;
			return segment;
		}
	}
}
