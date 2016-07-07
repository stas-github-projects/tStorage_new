using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace tStorage
{
    public partial class tEngine
    {
        public class tTree
        {
            /**/
            //private tGlobals _globals;
            public tTree()//tGlobals _globals)
            {
                //this._globals = _globals;
                //ch_delim = _globals.storage_crud_delimeter;
            }
            //*/

            public static bool bool_status = false;
            public static bool bool_exit = false;

            public static string full_key;
            public static string[] arr_keys;
            public static int arr_keys_length = 0;
            //public static char[] ch_delim;// = new char[] { ':', ':' };

            public static int record_item_index = 0;
            public static int data_item_index = 0;

            public static List<Record> lst_records = new List<Record>(10);
            public static int data_items_length = 0;
            public static byte data_item_type;
            public static List<byte[]> lst_data = new List<byte[]>(10);
            public static object data_object;

            public static ulong g_parent_id = 0;

            //save list
            public static List<int> lst_records_to_save = new List<int>(10);
            public static List<int> lst_data_to_save = new List<int>(10);
            public static List<string> lst_uuu = new List<string>();


            public class Record
            {
                internal byte data_type;
                //internal int fixed_length;
                internal int data_length;
                internal long data_pos;
                internal byte is_unix_time;
                internal long created_at;
                internal ulong parent_id;
                internal ulong current_id;
                internal string current_key;

                internal void add(string key, byte data_type, int data_length)
                {
                    this.current_key = key;
                    this.created_at = DateTime.Now.Ticks;
                    this.is_unix_time = 0;
                    this.parent_id = g_parent_id;// parent_id;
                    this.current_id = _globals.storage_records_counter; //_globals.storage_records_counter++;
                    this.data_length = data_length;
                    this.data_pos = 0;
                    this.data_type = data_type;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SplitPath(string path)
            {
                int i = 0, idelimpos = 0, idelimlen = _globals.storage_crud_delimeter.Length - 1, istartdelimpos = -1, istartpos = -1, ipos = 0, ilen = path.Length;
                string s = string.Empty;
                List<string> lst = new List<string>();
                while (ipos < ilen)
                {
                    i = path.IndexOf(_globals.storage_crud_delimeter[idelimpos], ipos);// ipos,StringComparison.OrdinalIgnoreCase);

                    if (i > -1)
                    {
                        if (idelimpos < idelimlen)
                        {
                            if (istartpos == -1) { istartdelimpos = i; istartpos = ipos; ipos = i + 1; }
                            idelimpos++;
                        }
                        else
                        { lst.Add(path.Substring(istartpos, i - istartpos - idelimlen)); ipos = i + 1; idelimpos = 0; istartpos = -1; }
                    }
                    else
                    {
                        if (ipos < ilen)
                        { lst.Add(path.Substring(ipos, ilen - ipos)); ipos = ilen; }
                    }
                }
                arr_keys = lst.ToArray();
                arr_keys_length = arr_keys.Length - 1;
            }

            public class Node
            {
                public NodeCollection Children { get; set; }
                public int record_index = 0;
                public int data_index = 0;
                public ulong this_id = 0, parent_i = 0;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Node()
                {
                    this.this_id = _globals.storage_records_counter; _globals.storage_records_counter++;
                    this.parent_i = g_parent_id;
                    this.Children = new NodeCollection();
                    this.record_index = record_item_index;
                    this.data_index = data_item_index;
                }
            }
            public class NodeCollection : Dictionary<string, Node>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool add(string path, ref object data)//, int fixed_length)
                {
                    /**/
                    //full_key = path;
                    arr_keys = path.Split(_globals.storage_crud_delimeter, StringSplitOptions.RemoveEmptyEntries);
                    arr_keys_length = arr_keys.Length - 1;
                    data_object = data;
                    //*/
                    //SplitPath(path);
                    //lst_data.Add(_globals._data_types.ObjectToBytes(ref data, out data_bytes);
                    record_item_index = lst_records.Count;
                    data_item_index = lst_data.Count;
                    bool_exit = false;
                    g_parent_id = 0;

                    //start
                    add_recursive(0);

                    return bool_status;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private void add_recursive(int ipos)
                {
                    if (bool_exit == true) { return; }
                    Node _node;
                    if (this.ContainsKey(arr_keys[ipos]))
                    {
                        _node = this[arr_keys[ipos]];
                        g_parent_id = _node.this_id;
                    }
                    else
                    {
                        int idatalen = 0;
                        if (ipos == arr_keys_length) //last chunk - add data
                        {
                            byte[] bdata;
                            data_item_type = _globals._data_types.ObjectToBytes(ref data_object, out bdata);
                            idatalen = bdata.Length;
                            data_items_length += idatalen;
                            lst_data.Add(bdata);
                            data_item_index++;
                        }

                        _node = new Node();
                        g_parent_id = _node.this_id;

                            //create record item
                            Record _newitem = new Record();
                            _newitem.add(arr_keys[ipos], data_item_type, idatalen); lst_records.Add(_newitem); 
                            //save
                            lst_records_to_save.Add(record_item_index);
                            lst_data_to_save.Add(data_item_index);
                            //counter
                            record_item_index++; //data_item_index++;
                        //}

                        

                        this.Add(arr_keys[ipos], _node);
                    }
                    //exit
                    if (ipos == arr_keys_length)
                    { bool_exit = true; return; }
                    else
                    { _node.Children.add_recursive(ipos + 1); }

                    return;
                }

                /*/
                public void search_recursive(int ipos)
                {
                    if (bool_exit == true) { return; }
                    if (this.ContainsKey(arr_keys[ipos]))
                    {
                        Node _node = this[arr_keys[ipos]];
                        lst_uuu.Add("this = "+_node.this_id+"; parent = " +_node.parent_i);
                        if (ipos == arr_keys_length)
                        { record_item_index = _node.record_index; bool_exit = true; return; }
                        else
                        { _node.Children.search_recursive(ipos + 1); }
                    }
                    return;
                }
                //*/
            }
        }
    }

}