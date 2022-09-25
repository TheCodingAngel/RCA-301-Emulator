using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rca301Emulator.UserInterface
{
    abstract class BaseListPanel : DataGridView
    {
        ViewType mViewType;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewType ViewType
        {
            get { return mViewType; }
            set
            {
                if (mViewType == value)
                    return;

                mViewType = value;
                RefreshList();
            }
        }

        public BaseListPanel()
        {
            AutoGenerateColumns = false;
        }

        public abstract void RefreshList();

        protected string GetStringRepresentation(Character[] data)
        {
            if (data == null || data.Length <= 0)
                return string.Empty;

            StringBuilder res = new StringBuilder(3 * data.Length);

            foreach (Character ch in data)
            {
                switch (mViewType)
                {
                    case ViewType.Characters:
                        res.Append(ch.DisplayString);
                        break;
                    case ViewType.Decimals:
                        res.Append(ch.Value.ToString().PadLeft(2, '0'));
                        break;
                    case ViewType.Hexadecimals:
                        res.Append(ch.Value.ToString("X").PadLeft(2, '0'));
                        break;
                }
            }

            return res.ToString();
        }
    }
}
