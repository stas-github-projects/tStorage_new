using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace tStorage
{

    public partial class tEngine
    {
        private static tGlobals _globals = new tGlobals();
        private tTree.NodeCollection _tree;// = new tTree();

        public tEngine()
        { _tree = new tTree.NodeCollection(); _globals.record_page_free_cells = _globals.record_page_max_records_per_page; }

        public bool open_storage(string storage_name)
        {
            bool bool_ret = false;

            _globals.storage_name = storage_name + ".tst";

            FileInfo finfo = new FileInfo(_globals.storage_name);
            if (finfo.Exists) //load
            { bool_ret = _open_storage(); }
            else //create
            { bool_ret = _new_storage(); }

            if (bool_ret == false)
            { _globals.storage_stream_status = false; _globals.storage_stream = null; _globals.storage_stream_length = 0; }
            else
            { _globals.storage_stream_status = true; _globals.storage_stream_length = _globals.storage_stream.Length; }

            return bool_ret;
        }

        private bool _open_storage()
        {
            bool bool_ret = false;
            byte[] bheader = new byte[6];
            int ilen = 0;

            try
            {
                _globals.storage_stream = new FileStream(_globals.storage_name, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, _globals.storage_buf_size, true);

                if (_globals.storage_stream.Length < _globals.storage_header.Length) { return false; } //error

                _globals.storage_stream.Position = 0;
                ilen = _globals.storage_stream.Read(bheader, 0, ilen);

                if (ilen == 0) { return false; } //error

                _globals.storage_header = bheader;
                bool_ret = true;
                //check for version changes
                //...

            }
            catch (Exception e) { bool_ret = false; }

            return bool_ret;
        }

        private bool _new_storage()
        {
            bool bool_ret = false;
            int ilen = 0;

            try
            {
                _globals.storage_stream = new FileStream(_globals.storage_name, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, _globals.storage_buf_size, true);

                ilen = _globals.storage_header.Length;
                _globals.storage_stream.Write(_globals.storage_header, 0, ilen);
                _globals.storage_stream.Position = ilen;
                bool_ret = true;

            }
            catch (Exception e) { bool_ret = false; }

            return bool_ret;
        }

        //
        // C.R.U.D.
        //

        public bool create(string key ="", object data = null, int reseved_length=0)
        {
            bool bool_ret = false;

            bool_ret = _tree.add(key, ref data);//, reseved_length);
            //bool_ret = _tree._get_status();

            return bool_ret;
        }

        public bool commit()
        {
            bool bool_ret = false;

            int i = 0, ireccount = tTree.lst_records_to_save.Count, ibyteslength = (ireccount * (_globals.storage_record_item_length + _globals.storage_record_key_name_max_length));

            if (ireccount == 0) { return false; }
            //start creating
            byte[] b_records = new byte[ireccount]; //buffer for all records


            return bool_ret;
        }

    }


}
