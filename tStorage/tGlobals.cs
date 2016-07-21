using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace tStorage
{

    public partial class tEngine
    {
        internal class tGlobals
        {
            internal tService _service;
            internal tDataTypes _data_types;
            //internal List<CLevelsPage> _index_pages = new List<CLevelsPage>(10);
            internal tGlobals()
            { 
                _service = new tService(this);
                _data_types = new tDataTypes();
            }

            internal string storage_name = "";
            internal long storage_created = 0;

            internal ulong storage_records_counter = 0;

            internal bool storage_stream_status = false;
            internal FileStream storage_stream;
            internal long storage_stream_length = 0;

            internal char[] storage_crud_delimeter = new char[] { ':', ':' };

            //options
            internal byte[] storage_header = Encoding.ASCII.GetBytes(new char[] { 'T', 'S', 'T', '0', '1' }); //3 (identifier) + 3 (version)
            internal int storage_buf_size = 4096;
            internal int storage_record_key_name_max_length = 10;
            internal int storage_record_item_length = 39;

            //internal ushort index_page_max_records_per_page = 2;
            internal ushort record_page_max_records_per_page = 2;
            internal ushort record_page_free_cells = 0;
            internal long record_page_current_pos = 0;

            internal long storage_virtual_length = 0;


            //store record pages pos
            /*
            internal class CLevelsPage
            {
                private tGlobals _globals;
                internal CLevelsPage(tGlobals _g)
                { _globals = _g; }

                internal ushort freecells = 0;
                internal long next_page_pos = 0;
                internal List<CIndexRecord> lst_page_header = new List<CIndexRecord>(10);
                internal class CIndexRecord
                {
                    internal byte _level;
                    internal ushort _freecells;
                    internal long _page_pos = 0;
                }
            }*/

        }
    }

}
