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
        {
            _tree = new tTree.NodeCollection();
            _globals.record_page_free_cells = _globals.record_page_max_records_per_page;
        }

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

            if (_globals.storage_virtual_length == 0) //saves storage length before first add
            { _globals.storage_virtual_length = _globals.storage_stream_length; }

            bool_ret = _tree.add(key, ref data);//, reseved_length);
            //bool_ret = _tree._get_status();

            return bool_ret;
        }


        public bool commit()
        {
            bool bool_ret = false, bool_save = false, bool_prev_page = true;

            int i = 0, ipos = 0, ireccount = tTree.lst_records_to_save.Count, ifullrecordlength = _globals.storage_record_item_length + _globals.storage_record_key_name_max_length; //(ireccount * (_globals.storage_record_item_length + _globals.storage_record_key_name_max_length));

            if (ireccount == 0) { return false; }
            //start creating
            byte[] b_records = new byte[0];// = new byte[ireccount]; //buffer for all records

            //make pages
            List<byte[]> lst_pages_to_save = new List<byte[]>();
            List<byte[]> lst_data_to_save = new List<byte[]>();

            for (i = 0; i < ireccount; i++)
            {
                if (_globals.record_page_free_cells == _globals.record_page_max_records_per_page) //create new record
                {
                    //if (i == 0) { bool_prev_page = false; }
                    create_new_records_page(i, ifullrecordlength, bool_prev_page);
                    bool_save = true;
                }
                else if (_globals.record_page_free_cells == 0) //save existing data
                {
                    //add to list
                    lst_pages_to_save.Add(b_records);
                    _globals.record_page_free_cells = _globals.record_page_max_records_per_page;
                    //create new
                    create_new_records_page(i, ifullrecordlength, bool_prev_page);
                    bool_save = true;
                }
                else
                {
                    _globals._service.InsertBytes(b_records, tTree.lst_records[tTree.lst_records_to_save[i]].get_bytes(), ipos);
                    ipos += ifullrecordlength;
                    _globals.record_page_free_cells--;
                    bool_save = true;
                }
            }//for
            //additional checking
            if (bool_save == true) //save existing data
            {
                lst_pages_to_save.Add(b_records);
            }

            //save pages
            save_records_pages();

            //clear
            _globals.storage_virtual_length = 0;
            tTree.lst_data_to_save.Clear(); tTree.lst_records_to_save.Clear();

            return bool_ret;
        }

        private byte[] create_new_records_page(int i, int ifullrecordlength, bool bool_has_prev_page)
        {
            int ipos = 2 + 8, ibufsize = (_globals.record_page_max_records_per_page * ifullrecordlength) + ipos, ibufdata = 0;
            byte[] b_records = new byte[ibufsize];
            byte[] b_data;

            //set prev. page freecells & pos
            if (bool_has_prev_page == true)
            {
                _globals._service.InsertBytes(b_records, BitConverter.GetBytes(_globals.record_page_free_cells), 0);

                //data
                if (i > 0)
                {
                    //get data length
                    List<int> lst_length = new List<int>(10);
                    for (int j = 0; j < i; j++)
                    {
                        int ilen = tTree.lst_data[tTree.lst_data_to_save[j]].Length;
                        lst_length.Add(ilen); ibufdata += ilen;
                    }
                    //write to buffer
                    ipos = 0;
                    b_data = new byte[ibufdata];
                    for (int j = 0; j < i; j++)
                    {
                        _globals._service.InsertBytes(b_data, tTree.lst_data[tTree.lst_data_to_save[j]], ipos);
                        ipos += lst_length[j];
                    }
                }

                _globals.storage_virtual_length += (ibufsize + ibufdata);
                _globals._service.InsertBytes(b_records, BitConverter.GetBytes(_globals.storage_virtual_length), 2);
            }

            _globals._service.InsertBytes(b_records, tTree.lst_records[tTree.lst_records_to_save[i]].get_bytes(), ipos);
            ipos += ifullrecordlength;
            _globals.record_page_free_cells--;

            return b_records;
        }

        private void save_records_pages()
        {


        }

    }


}
