using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Livesplit_Server_Autosplitter
{
    public static class Memory
    {
        [StructLayout(LayoutKind.Sequential)]
        unsafe struct iovec
        {
            public void* iov_base;
            public int iov_len;
        }

        [DllImport("libc")]
        private static extern unsafe int process_vm_readv(int pid,
            iovec* local_iov,
            ulong liovcnt,
            iovec* remote_iov,
            ulong riovcnt,
            ulong flags);

        public unsafe static bool Read<T>(IntPtr address, out T value, Process _process) where T : unmanaged
        {
            int size = Unsafe.SizeOf<T>();
            byte* ptr = stackalloc byte[size];
            iovec localIo = new iovec
            {
                iov_base = ptr,
                iov_len = size
            };
            
            iovec remoteIo = new iovec
            {
                iov_base = address.ToPointer(),
                iov_len = size
            };

            var res = process_vm_readv(_process.Id, &localIo, 1, &remoteIo, 1, 0);
            value = *(T*)ptr;
            return res != -1;
        }

        public unsafe static bool ReadArray(IntPtr address, int count, out byte[] value, Process _process)
        {
            int size = Unsafe.SizeOf<byte>() * count;
            byte* ptr = stackalloc byte[size];
            iovec localIo = new iovec
            {
                iov_base = ptr,
                iov_len = size
            };
            
            iovec remoteIo = new iovec
            {
                iov_base = address.ToPointer(),
                iov_len = size
            };

            var res = process_vm_readv(_process.Id, &localIo, 1, &remoteIo, 1, 0);
            value = new byte[count];
            Marshal.Copy((IntPtr)ptr, value, 0, count);
            return res != -1;
        }

        public static bool ReadString(IntPtr address, int count, out string value, Process process)
        {
            value = null;
            if (!ReadArray(address, count, out byte[] bytes, process))
                return false;

            value = Encoding.ASCII.GetString(bytes);
            return true;
        }

        public static IntPtr GetAddress(this Process process, int baseAddress, params int[] offsets)
        {
            int tempValue;
            Read<int>((IntPtr)baseAddress, out tempValue, process);
            for (int i = 0; i < offsets.Length -1; i++)
            {
                int currAddress = tempValue + offsets[i];
                Read<int>((IntPtr)currAddress, out tempValue, process);
            }

            return new IntPtr(tempValue + offsets[offsets.Length - 1]);
        }
    }
}