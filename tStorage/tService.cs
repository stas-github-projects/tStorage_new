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

            internal void InsertBytes(byte[] _src, byte[] _what, int _pos = 0, int _length = 0)
            {
                InsertBytes(_src, ref _what, _pos, _length);
            }
            internal void InsertBytes(byte[] _src, byte _what, int _pos = 0, int _length = 0)
            {
                int i = _src.Length;
                if (_pos < 0) { _pos = 0; }
                if (_length == 0) { _length = 1; }// _what.Length; }
                if (_pos + _length > i) { return; }//out of dimensions
                //Buffer.BlockCopy(_what, 0, _src, _pos, _length);
                _src[_pos] = _what;
            }
            internal void InsertBytes(byte[] _src, ref byte[] _what, int _pos = 0, int _length = 0)
            {
                int i = _src.Length;
                if (_pos < 0) { _pos = 0; }
                if (_length == 0) { _length = _what.Length; }
                if (_pos + _length > i) { return; }//out of dimensions
                Buffer.BlockCopy(_what, 0, _src, _pos, _length);
            }
            internal byte[] GetBytes(byte[] _src, int _pos = 0, int _length = 0)
            {
                byte[] b_out;
                int ilen = _src.Length;
                if (_length < 1) { _length = ilen; }
                if (_pos < 0) { _pos = 0; }
                if (ilen >= (_pos + _length))
                {
                    b_out = new byte[_length];
                    Buffer.BlockCopy(_src, _pos, b_out, 0, _length);//copy piece
                    return b_out;
                }
                else
                { return _src; }
            }
            internal byte GetByte(byte[] _src, int _pos = 0)
            {
                byte b_out;
                int ilen = _src.Length, _length = 1;
                if (_length < 1) { _length = ilen; }
                if (_pos < 0) { _pos = 0; }
                if (ilen >= (_pos + _length))
                {
                    b_out = _src[_pos];// Buffer.BlockCopy(_src, _pos, b_out, 0, _length);//copy piece
                    return b_out;
                }
                else
                { return 0; }
            }
        }

    }

}
