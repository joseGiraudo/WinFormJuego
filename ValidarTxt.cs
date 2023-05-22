using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormPersonas
{
    internal class ValidarTxt
    {
        public static bool NumInput(KeyPressEventArgs e)
        {
            if (Char.IsNumber(e.KeyChar))
            {
                e.Handled = false;
                return true;
            } else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
                return true;
            } else
            {
                e.Handled= true;
                return false;
            }
        }

    }
}
