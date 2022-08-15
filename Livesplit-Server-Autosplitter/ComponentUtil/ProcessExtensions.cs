using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Livesplit_Server_Autosplitter;

#pragma warning disable 1591

// Note: Please be careful when modifying this because it could break existing components!
// http://stackoverflow.com/questions/1456785/a-definitive-guide-to-api-breaking-changes-in-net

namespace LiveSplit.ComponentUtil
{
    using SizeT = UIntPtr;

    public enum ReadStringType
    {
        AutoDetect,
        ASCII,
        UTF8,
        UTF16
    }

    public static class ExtensionMethods
    {
        public static bool Is64Bit(this Process process)
        {
            return true;
        }

        public static bool ReadValue<T>(this Process process, IntPtr addr, out T val) where T : struct
        {
            var type = typeof(T);
            type = type.IsEnum ? Enum.GetUnderlyingType(type) : type;

            val = default(T);
            object val2;
            if (!ReadValue(process, addr, type, out val2))
                return false;

            val = (T)val2;

            return true;
        }

        public static bool ReadValue(Process process, IntPtr addr, Type type, out object val)
        {
            byte[] bytes;

            val = null;
            int size = type == typeof(bool) ? 1 : Marshal.SizeOf(type);
            if (!ReadBytes(process, addr, size, out bytes))
                return false;

            val = ResolveToType(bytes, type);

            return true;
        }

        public static bool ReadBytes(this Process process, IntPtr addr, int count, out byte[] val)
        {
            var bytes = new byte[count];

            SizeT read;
            val = null;
            if (!Memory.ReadArrayHeap(addr, count, out bytes, process))
                return false;

            val = bytes;

            return true;
        }

        public static T ReadValue<T>(this Process process, IntPtr addr, T default_ = default(T)) where T : struct
        {
            T val;
            if (!process.ReadValue(addr, out val))
                val = default_;
            return val;
        }

        public static byte[] ReadBytes(this Process process, IntPtr addr, int count)
        {
            byte[] bytes;
            if (!process.ReadBytes(addr, count, out bytes))
                return null;
            return bytes;
        }

        static object ResolveToType(byte[] bytes, Type type)
        {
            object val;

            if (type == typeof(int))
            {
                val = BitConverter.ToInt32(bytes, 0);
            }
            else if (type == typeof(uint))
            {
                val = BitConverter.ToUInt32(bytes, 0);
            }
            else if (type == typeof(float))
            {
                val = BitConverter.ToSingle(bytes, 0);
            }
            else if (type == typeof(double))
            {
                val = BitConverter.ToDouble(bytes, 0);
            }
            else if (type == typeof(byte))
            {
                val = bytes[0];
            }
            else if (type == typeof(bool))
            {
                if (bytes == null)
                    val = false;
                else
                    val = (bytes[0] != 0);
            }
            else if (type == typeof(short))
            {
                val = BitConverter.ToInt16(bytes, 0);
            }
            else // probably a struct
            {
                var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                try
                {
                    val = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type);
                }
                finally
                {
                    handle.Free();
                }
            }

            return val;
        }

        public static float ToFloatBits(this uint i)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
        }

        public static uint ToUInt32Bits(this float f)
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes(f), 0);
        }

        public static bool BitEquals(this float f, float o)
        {
            return ToUInt32Bits(f) == ToUInt32Bits(o);
        }
    }
}
