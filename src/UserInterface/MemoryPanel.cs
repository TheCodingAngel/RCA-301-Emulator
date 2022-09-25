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
    class EventArgsAddress : EventArgs
    {
        public readonly AddressPosition Position;
        public readonly int Address;

        public EventArgsAddress(AddressPosition pos, int address)
        {
            Position = pos;
            Address = address;
        }
    }

    [DefaultEvent(nameof(SetNextInstruction))]
    partial class MemoryPanel : UserControl
    {
        const int TOP_ADDRESSES_HEIGHT = 25;
        const int LEFT_ADDRESSES_WIDTH = 50;

        IAssemblerEnvironment mAsmEnvironment;

        ViewType mAddressesType;

        [Browsable(true)]
        [DefaultValue(0)]
        public int Columns
        {
            get { return mMemory.Columns; }
            set { mMemory.Columns = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ViewType AddressesType
        {
            get { return mAddressesType; }
            set
            {
                if (mAddressesType == value)
                    return;

                mAddressesType = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        public event EventHandler<EventArgsAddress> SetNextInstruction;

        public MemoryPanel()
        {
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;

            cbViewType.DataSource = Enum.GetValues(typeof(ViewType));
            cbAddressType.DataSource = Enum.GetValues(typeof(ViewType));
            //cbAddressType.SelectedItem = ViewType.Decimals;
        }

        public void Init(Memory memory, IAssemblerEnvironment asmEnvironment)
        {
            mMemory.Memory = memory;
            mAsmEnvironment = asmEnvironment;
        }

        public void ScrollToAddress(int address)
        {
            mMemory.ScrollToAddress(address);
        }

        public void RefreshMemory()
        {
            mMemory.Refresh();
        }

        void OnToolsPanelSizeChanged(object sender, EventArgs e)
        {
            ResetMemoryControlBounds();
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            ResetMemoryControlBounds();
        }

        void ResetMemoryControlBounds()
        {
            Rectangle clientRect = ClientRectangle;

            mMemory.Bounds = Rectangle.FromLTRB(
                LEFT_ADDRESSES_WIDTH + mMemory.Margin.Left,
                fpTools.Bottom + fpTools.Margin.Bottom + TOP_ADDRESSES_HEIGHT + mMemory.Margin.Top,
                clientRect.Right - mMemory.Margin.Right,
                clientRect.Bottom - mMemory.Margin.Bottom);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle memoryRect = GetMemoryRect();
            Size cellSize = mMemory.CellSize;
            AddressPosition pos = mMemory.ScrollPosition;

            Rectangle topRect = GetTopAddressesRect(memoryRect);

            Rectangle leftRect = GetLeftAddressesRect(memoryRect);

            DrawLeftAddresses(pos.Row * mMemory.Columns, mMemory.Columns, e.Graphics, leftRect, cellSize.Height);

            DrawTopAddresses(pos.Column, e.Graphics, topRect, cellSize.Width);
        }

        void DrawLeftAddresses(int topAddress, int addressesInRow, Graphics g, Rectangle bounds, int cellHeight)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Far;
            stringFormat.LineAlignment = StringAlignment.Near;

            int rowCount = bounds.Height / cellHeight;

            for (int i = 0; i < rowCount; i++)
            {
                Rectangle rect = new Rectangle(bounds.Left, bounds.Top + i * cellHeight, bounds.Width, cellHeight);
                g.DrawString(AddressToString(topAddress + i * addressesInRow, false), Font, Brushes.Black, rect, stringFormat);
            }
        }

        void DrawTopAddresses(int leftAddress, Graphics g, Rectangle bounds, int cellWidth)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Far;

            int columnCount = Math.Min(bounds.Width / cellWidth, mMemory.Columns);

            for (int i = 0; i < columnCount; i++)
            {
                Rectangle rect = new Rectangle(bounds.Left + i * cellWidth, bounds.Top, cellWidth, bounds.Height);
                g.DrawString(AddressToString(leftAddress + i, true), Font, Brushes.Black, rect, stringFormat);
            }
        }

        string AddressToString(int address, bool shortForm)
        {
            switch (mAddressesType)
            {
                case ViewType.Characters:
                    return shortForm ? new Diad(address).DisplayString : new Quad(address).DisplayString;
                case ViewType.Hexadecimals:
                    return address.ToString("X").PadLeft(shortForm ? 2 : 4, '0');
                case ViewType.Decimals:
                    return address.ToString().PadLeft(shortForm ? 2 : 5, '0');
            }
            return "XX";
        }

        Rectangle GetMemoryRect()
        {
            return RectangleToClient(mMemory.GetCellsScreenRect());
        }

        Rectangle GetLeftAddressesRect(Rectangle memoryRect)
        {
            return new Rectangle(mMemory.Left - (LEFT_ADDRESSES_WIDTH + mMemory.Margin.Left), memoryRect.Top,
                LEFT_ADDRESSES_WIDTH, memoryRect.Height);
        }

        Rectangle GetTopAddressesRect(Rectangle memoryRect)
        {
            return new Rectangle(memoryRect.Left, mMemory.Top - (TOP_ADDRESSES_HEIGHT + mMemory.Margin.Top),
                memoryRect.Width, TOP_ADDRESSES_HEIGHT);
        }

        void OnColumnsChanged(object sender, EventArgs e)
        {
            mMemory.Columns = (int)numColumns.Value;
        }

        void OnViewTypeChanged(object sender, EventArgs e)
        {
            mMemory.ViewType = (ViewType)cbViewType.SelectedItem;
        }

        void OnAddressTypeChanged(object sender, EventArgs e)
        {
            AddressesType = (ViewType)cbAddressType.SelectedItem;
        }

        void OnMemoryScroll(object sender, ScrollEventArgs e)
        {
            Rectangle memoryRect = GetMemoryRect();

            Rectangle leftRect = GetLeftAddressesRect(memoryRect);
            Invalidate(leftRect);

            Rectangle topRect = GetTopAddressesRect(memoryRect);
            Invalidate(topRect);
        }

        void OnGoToAddressClicked(object sender, EventArgs e)
        {
            mMemory.ScrollToAddress((int)numAddress.Value);
            mMemory.Focus();
        }

        void OnCurrentAddressChanged(object sender, EventArgs e)
        {
            numAddress.Value = GetAddressFromPosition(mMemory.CurrentAddressPosition);
        }

        void OnSetNextInstructionClick(object sender, EventArgs e)
        {
            int currentAddress = GetAddressFromPosition(mMemory.CurrentAddressPosition);
            SetNextInstruction?.Invoke(this, new EventArgsAddress(mMemory.CurrentAddressPosition, currentAddress));
            mAsmEnvironment?.SelectAssemblySourceLine(currentAddress);
        }

        int GetAddressFromPosition(AddressPosition pos)
        {
            return pos != null ? pos.Row * mMemory.Columns + pos.Column : 0;
        }
    }
}
