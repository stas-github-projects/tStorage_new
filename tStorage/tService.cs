using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tStorage
{
    
    public partial class tEngine
    {
        internal class tService
        {
            internal tGlobals _globals;
            internal tService(tGlobals _globals)
            { this._globals = _globals; }

            internal string[] _get_split_string(string input)
            {
                return input.Split(_globals.storage_crud_delimeter, StringSplitOptions.RemoveEmptyEntries);
            }

            internal void InsertBytes(ref byte[] where, byte what, int start_pos, int length)
            {

            }
        }

    }

}
