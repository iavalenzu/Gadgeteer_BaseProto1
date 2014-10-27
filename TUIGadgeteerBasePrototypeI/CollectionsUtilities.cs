using System;
using System.Collections;
using Microsoft.SPOT;

namespace TUIGadgeteerBasePrototypeI
{
    class CollectionsUtilities
    {

        public static String[] orderHashTableKeys(Hashtable hash_table) 
        {
            String[] hash_keys;             

            hash_keys = new String[hash_table.Count];

            hash_table.Keys.CopyTo(hash_keys, 0);

            return orderArray(hash_keys);
        
        } 

        public static String[] orderArray(String[] array)
        {
            return bubbleSort(array);
        }

        public static String[] bubbleSort(String[] array)
        {
            String tmp;

            for (int t = 0; t < array.Length - 1; t++)
            {
                for (int i = 0; i < array.Length - t - 1; i++)
                {
                    if (array[i + 1].CompareTo(array[i]) < 0)
                    {
                        tmp = array[i];
                        array[i] = array[i + 1];
                        array[i + 1] = tmp;
                    }
                }
            }

            return array;
        } 


    }
}
