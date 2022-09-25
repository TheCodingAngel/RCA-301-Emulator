using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rca301Emulator.Emulator;

namespace Rca301Emulator.UserInterface
{
    partial class OutputPanel : UserControl
    {
        public OutputPanel()
        {
            InitializeComponent();
        }

        public void AddCharacters(Character[] chars)
        {
            //string outputText = tbOutput.Text + chars.DataToString() + Environment.NewLine;
            tbOutput.AppendText(chars.DataToString() + Environment.NewLine);
            /*tbOutput.Text = outputText;
            tbOutput.SelectionStart = outputText.Length;
            tbOutput.SelectionLength = 0;*/
        }

        public void ResetOutput()
        {
            tbOutput.Text = string.Empty;
        }
    }
}
