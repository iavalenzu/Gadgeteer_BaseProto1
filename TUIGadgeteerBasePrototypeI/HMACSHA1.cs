using System;
using Microsoft.SPOT;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace TUIGadgeteerBasePrototypeI
{

    /**
    * Genera el HMACSHA1 del mensaje usando la llave dada 
    */    
    class HMACSHA1
    {
        static public byte[] hash(byte[] _message, byte[] _key)
        {
            byte[] key_bytes;

            key_bytes = _key;

            if (key_bytes.Length > 64)
            {
                key_bytes = SHA1(key_bytes);
            }

            if (key_bytes.Length < 64)
            {
                byte[] zeros = zero_array(64 - key_bytes.Length);

                key_bytes = concat_arrays(key_bytes, zeros);
            }

            byte[] o_key_pad = xor_arrays(fill_array(0x5c, 64), key_bytes);
            byte[] i_key_pad = xor_arrays(fill_array(0x36, 64), key_bytes);

            byte[] i_hash = SHA1(concat_arrays(i_key_pad, _message));

            byte[] o_hash = SHA1(concat_arrays(o_key_pad, i_hash));

            return o_hash;


        }

        public static string arrayToHexString(byte[] arr)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < arr.Length; ++i)
                sb.Append(arr[i].ToString("x2"));
            return sb.ToString();
        }


        static public byte[] SHA1(byte[] message)
        {

            HashAlgorithm ha = new System.Security.Cryptography.HashAlgorithm(HashAlgorithmType.SHA1);
            return ha.ComputeHash(message);
        }

        static public byte[] zero_array(int num_zeros)
        {
            byte[] res;

            res = new byte[num_zeros];

            for (int i = 0; i < num_zeros; i++)
            {
                res[i] = 0x00;
            }

            return res;
        }

        static public byte[] concat_arrays(byte[] a, byte[] b)
        {
            byte[] res;

            res = new byte[a.Length + b.Length];

            for (int i = 0; i < a.Length; i++)
            {
                res[i] = a[i];
            }

            for (int i = 0; i < b.Length; i++)
            {
                res[i + a.Length] = b[i];
            }

            return res;
        }

        static public byte[] fill_array(byte source, int times)
        {
            byte[] res;

            res = new byte[times];

            for (int i = 0; i < times; i++)
            {
                res[i] = source;
            }

            return res;
        }

        static public byte[] xor_arrays(byte[] a, byte[] b)
        {
            byte[] res;

            res = new byte[a.Length];

            if (a.Length != b.Length)
            {
                Debug.Print("No coinciden los tamaños de los arreglos");
            }
            else
            {

                for (int i = 0; i < res.Length; i++)
                {
                    res[i] = (byte)(a[i] ^ b[i]);
                }

            }

            return res;

        }




    }
}
