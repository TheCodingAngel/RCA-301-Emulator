using Rca301Emulator.Emulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rca301Emulator.UserInterface
{
    class MemoryControl : Control
    {
        const int DESIGN_TIME_SAMPLE_CHARACTERS = 16;

        enum DataArea
        {
            Tables,
            Code,
            Data,
        }

        CellParam mCellParam;
        Memory mMemory;

        ViewType mViewType;

        int mColumns;

        int mPageColumns;
        int mPageRows;

        int mVerticalPos;
        int mMaxVerticalPos;

        int mHorizontalPos;
        int mMaxHorizontalPos;

        AddressPosition mActiveCell;
        Point mCaretPos;

        [Browsable(true)]
        public Padding CellPadding
        {
            get { return mCellParam.Padding; }
            set
            {
                if (mCellParam.Padding == value)
                    return;

                mCellParam.Padding = value;
                ResetInnerLayout();
                Invalidate();
            }
        }

        public bool ShouldSerializeCellPadding()
        {
            return mCellParam.Padding != CellParam.DEFAULT_PADDING;
        }

        [Browsable(true)]
        [DefaultValue(0)]
        public int Columns
        {
            get { return mColumns; }
            set
            {
                if (mColumns == value)
                    return;

                mColumns = value;

                ResetInnerLayout();
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Memory Memory
        {
            get { return mMemory; }
            set
            {
                if (mMemory == value)
                    return;

                mMemory = value;
                ResetInnerLayout();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AddressPosition ScrollPosition => new AddressPosition(mVerticalPos, mHorizontalPos);

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AddressPosition CurrentAddressPosition => mActiveCell;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size CellSize => mCellParam.Size;

        Point CaretPos
        {
            get { return mCaretPos; }
            set
            {
                if (mCaretPos == value)
                    return;

                mCaretPos = value;

                Win32.SetCaretPos(value.X, value.Y);
            }
        }

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
                Invalidate();
            }
        }

        [Category("Action")]
        [Browsable(true)]
        public event ScrollEventHandler Scroll;

        [Category("Action")]
        [Browsable(true)]
        public event EventHandler CurrentAddressPositionChanged;

        public MemoryControl()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            DoubleBuffered = true;

            mCellParam = new CellParam();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        public Rectangle GetCellsScreenRect()
        {
            return RectangleToScreen(GetDisplayRectangle());
        }

        public void EnsureVisibleAddress(AddressPosition addressPosition)
        {
            int row = mVerticalPos;
            if (addressPosition.Row < mVerticalPos)
                row = addressPosition.Row;
            else if (addressPosition.Row >= mVerticalPos + mPageRows)
                row = addressPosition.Row - (mPageRows - 1);

            int column = mHorizontalPos;
            if (addressPosition.Column < mHorizontalPos)
                column = addressPosition.Column;
            else if (addressPosition.Column >= mHorizontalPos + mPageColumns)
                column = addressPosition.Column - (mPageColumns - 1);

            if (row != mVerticalPos || column != mHorizontalPos)
            {
                ScrollTo(row, column);
            }
        }

        public void ScrollToAddress(int address)
        {
            SetActiveCell(new AddressPosition(address / mMemory.CharactersInRow, address % mMemory.CharactersInRow));
            EnsureVisibleAddress(mActiveCell);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams res = base.CreateParams;
                //res.ExStyle |= (int)Win32.StylesEx.WS_EX_CLIENTEDGE;
                res.Style |= (int)Win32.Styles.WS_BORDER;
                res.Style |= (int)(Win32.Styles.WS_HSCROLL | Win32.Styles.WS_VSCROLL);
                return res;
            }
        }

        protected override void CreateHandle()
        {
            base.CreateHandle();

            mCellParam.Init(Handle, Font);
            ResetInnerLayout();
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            ResetInnerLayout();
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);

            ResetInnerLayout();
            Invalidate();
        }

        void ResetInnerLayout()
        {
            if (!mCellParam.IsInitialized || mMemory == null)
                return;

            Size visibleSize = GetDisplayRectangle().Size;
            if (visibleSize.Width > 0 && visibleSize.Height > 0)
            {
                mPageColumns = visibleSize.Width / mCellParam.Size.Width;
                mPageRows = visibleSize.Height / mCellParam.Size.Height;

                if (mColumns > 0)
                    mMemory.SetCharactersInRow(mColumns);
                else
                    mMemory.SetCharactersInRow(mPageColumns);

                mMaxHorizontalPos = Math.Max(0, mMemory.CharactersInRow - mPageColumns);
                mMaxVerticalPos = Math.Max(0, mMemory.NumRows - mPageRows);

                mHorizontalPos = Math.Min(mHorizontalPos, mMaxHorizontalPos);
                mVerticalPos = Math.Min(mVerticalPos, mMaxVerticalPos);

                Win32.SetScrollInfo(Handle, Win32.SBFlags.SB_HORZ, 0, mMemory.CharactersInRow - 1, (uint)mPageColumns, mHorizontalPos);
                Win32.SetScrollInfo(Handle, Win32.SBFlags.SB_VERT, 0, mMemory.NumRows - 1, (uint)mPageRows, mVerticalPos);
            }
        }

        void WmScroll(ref Message m, ScrollOrientation scrollOrientation)
        {
            int maxScrollValue = scrollOrientation == ScrollOrientation.VerticalScroll ? mMaxVerticalPos : mMaxHorizontalPos;

            int oldValue = scrollOrientation == ScrollOrientation.VerticalScroll ? mVerticalPos : mHorizontalPos;
            int num = oldValue;

            ScrollEventType type = (ScrollEventType)m.WParam.LOWORD();
            switch (type)
            {
                case ScrollEventType.SmallDecrement:
                    num--;
                    break;
                case ScrollEventType.SmallIncrement:
                    num++;
                    break;
                case ScrollEventType.LargeDecrement:
                    num -= CalcPageScroll(scrollOrientation);
                    break;
                case ScrollEventType.LargeIncrement:
                    num += CalcPageScroll(scrollOrientation);
                    break;
                case ScrollEventType.ThumbPosition:
                case ScrollEventType.ThumbTrack:
                    num = m.WParam.HIWORD();
                    break;
                case ScrollEventType.First:
                    num = 0;
                    break;
                case ScrollEventType.Last:
                    num = maxScrollValue;
                    break;
            }

            if (num > maxScrollValue) num = maxScrollValue;
            if (num < 0) num = 0;

            if (scrollOrientation == ScrollOrientation.VerticalScroll)
                mVerticalPos = num;
            else
                mHorizontalPos = num;

            SetActiveCell();

            // Update the scroll bars
            Win32.SetScrollPos(Handle, scrollOrientation, num, true);

            Invalidate();

            if (type != ScrollEventType.EndScroll)
            {
                ScrollEventArgs se = new ScrollEventArgs(type, oldValue, num, scrollOrientation);
                OnScroll(se);
            }
        }

        void OnScroll(ScrollEventArgs se)
        {
            Scroll?.Invoke(this, se);
        }

        int CalcPageScroll(ScrollOrientation scrollOrientation)
        {
            int res = scrollOrientation == ScrollOrientation.VerticalScroll ? mPageRows : mPageColumns;
            res--; // scroll less so the user will not be confused
            return res;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Win32.HideCaret(Handle);

            base.OnPaint(e);

            if (mMemory == null)
            {
                if (DesignMode)
                    DesignTimePaint(e.Graphics);
                return;
            }

            Rectangle visibleRect = GetDisplayRectangle();

            int rows = Math.Min(mVerticalPos + mPageRows, mMemory.NumRows);

            AddressPosition programOffsetPosition = mMemory.ProgramOffsetPosition;
            AddressPosition dataOffsetPosition = mMemory.DataOffsetPosition;

            for (int i = 0; i < rows - mVerticalPos; i++)
            {
                int absoluteRow = i + mVerticalPos;
                
                Character[] chars = mMemory.GetRowOfCharacters(absoluteRow);
                int columns = Math.Min(mHorizontalPos + mPageColumns, chars.Length);

                for (int j = 0; j < columns - mHorizontalPos; j++)
                {
                    int absoluteColumn = j + mHorizontalPos;

                    Character ch = chars[absoluteColumn];

                    string text = CharacterToString(ch);

                    /*Brush brush = Brushes.Black;
                    switch (GetDataArea(absoluteRow, absoluteColumn, programOffsetPosition, dataOffsetPosition))
                    {
                        case DataArea.Tables:
                            brush = Brushes.DarkRed;
                            break;
                        case DataArea.Code:
                            brush = Brushes.DarkBlue;
                            break;
                    }

                    e.Graphics.DrawString(text, Font, brush,
                        visibleRect.Left + j * mCellParam.Size.Width + mCellParam.Padding.Left,
                        visibleRect.Top + i * mCellParam.Size.Height + mCellParam.Padding.Top);*/

                    Color backgroundColor = BackColor;
                    switch (GetDataArea(absoluteRow, absoluteColumn, programOffsetPosition, dataOffsetPosition))
                    {
                        case DataArea.Tables:
                            backgroundColor = Color.Yellow;
                            break;
                        case DataArea.Code:
                            backgroundColor = Color.LightGreen;
                            break;
                    }

                    Point textPosition = new Point(visibleRect.Left + j * mCellParam.Size.Width + mCellParam.Padding.Left,
                        visibleRect.Top + i * mCellParam.Size.Height + mCellParam.Padding.Top);

                    TextRenderer.DrawText(e.Graphics, text,
                        Font, textPosition, Color.Black, backgroundColor,
                        TextFormatFlags.Default | TextFormatFlags.NoPrefix);
                }
            }

            //Win32.ShowCaret(Handle);
        }

        DataArea GetDataArea(int absoluteRow, int absoluteColumn,
            AddressPosition programOffsetPosition, AddressPosition dataOffsetPosition)
        {
            if (absoluteRow < programOffsetPosition.Row)
                return DataArea.Tables;

            if (absoluteRow == programOffsetPosition.Row && absoluteColumn < programOffsetPosition.Column)
                return DataArea.Tables;

            if (absoluteRow < dataOffsetPosition.Row)
                return DataArea.Code;

            if (absoluteRow == dataOffsetPosition.Row)
            {
                if (absoluteColumn < dataOffsetPosition.Column)
                    return DataArea.Code;
            }

            return DataArea.Data;
        }

        string CharacterToString(Character ch)
        {
            switch (mViewType)
            {
                case ViewType.Characters:
                    return ch.DisplayString;
                case ViewType.Hexadecimals:
                    return ch.OverflownValue.ToString("X").PadLeft(2, '0');
                case ViewType.Decimals:
                    return ch.OverflownValue.ToString().PadLeft(2, '0');
            }
            return "XX";
        }

        void DesignTimePaint(Graphics g)
        {
            Rectangle visibleRect = GetDisplayRectangle();

            for (int i = 0; i < DESIGN_TIME_SAMPLE_CHARACTERS; i++)
            {
                for (int j = 0; j < DESIGN_TIME_SAMPLE_CHARACTERS; j++)
                {
                    byte val = (byte)(i * DESIGN_TIME_SAMPLE_CHARACTERS + j);
                    string valStr = val.ToString("X").PadLeft(2, '0');

                    g.DrawString(valStr, Font, Brushes.Black,
                        visibleRect.Left + j * mCellParam.Size.Width,
                        visibleRect.Top + i * mCellParam.Size.Height);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        Rectangle GetDisplayRectangle()
        {
            Rectangle rect = ClientRectangle;
            Padding pad = Padding;
            return new Rectangle(rect.X + pad.Left, rect.Y + pad.Top,
                rect.Width - pad.Horizontal, rect.Height - pad.Vertical);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Win32.CreateCaret(Handle, IntPtr.Zero, mCellParam.Size.Width, mCellParam.Size.Height);

            // ensure setting caret at non-zero position
            mCaretPos = new Point(0, 0);
            SetActiveCell();
            Win32.ShowCaret(Handle);

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Win32.DestroyCaret();

            base.OnLostFocus(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Focus();
            SetActiveCell(FromScreenToAbsolutePos(GetCellFromClientPosition(e.Location)));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            SetActiveCell(FromScreenToAbsolutePos(GetCellFromClientPosition(e.Location)));
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Right:
                    SetActiveCell(mActiveCell.Offest(0, 1));
                    EnsureVisibleAddress(mActiveCell);
                    break;
                case Keys.Left:
                    SetActiveCell(mActiveCell.Offest(0, -1));
                    EnsureVisibleAddress(mActiveCell);
                    break;
                case Keys.Down:
                    SetActiveCell(mActiveCell.Offest(1, 0));
                    EnsureVisibleAddress(mActiveCell);
                    break;
                case Keys.Up:
                    SetActiveCell(mActiveCell.Offest(-1, 0));
                    EnsureVisibleAddress(mActiveCell);
                    break;
            }
        }

        void ScrollTo(int row, int column)
        {
            row = Math.Max(0, Math.Min(mMaxVerticalPos, row));
            if (row != mVerticalPos)
                Win32.SendMessage(Handle, Win32.Msgs.WM_VSCROLL, (int)ScrollEventType.ThumbPosition + (row << 16), 0);

            column = Math.Max(0, Math.Min(mMaxHorizontalPos, column));
            if (column != mHorizontalPos)
                Win32.SendMessage(Handle, Win32.Msgs.WM_HSCROLL, (int)ScrollEventType.ThumbPosition + (column << 16), 0);
        }

        void ScrollWith(int dr, int dc)
        {
            ScrollTo(mVerticalPos + dr, mHorizontalPos + dc);
        }

        bool IsVisible(AddressPosition absolutePos)
        {
            AddressPosition scrolledPos = FromAbsoluteToScreenPos(absolutePos);

            return scrolledPos.Row >= 0 && scrolledPos.Row <= mPageRows - 1 &&
                scrolledPos.Column >= 0 && scrolledPos.Column <= mPageColumns - 1; 
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)Win32.Msgs.WM_VSCROLL:
                    WmScroll(ref m, ScrollOrientation.VerticalScroll);
                    return;
                case (int)Win32.Msgs.WM_HSCROLL:
                    WmScroll(ref m, ScrollOrientation.HorizontalScroll);
                    return;
            }

            base.WndProc(ref m);
        }

        AddressPosition GetCellFromClientPosition(Point pos)
        {
            int row = (pos.Y - Padding.Top) / mCellParam.Size.Height;
            int column = (pos.X - Padding.Left) / mCellParam.Size.Width;
            return new AddressPosition(row, column);
        }

        Point GetCaretFromAddressPosition(AddressPosition ap)
        {
            int x = (ap != null ? ap.Column : 0) * mCellParam.Size.Width + Padding.Left;
            int y = (ap != null ? ap.Row : 0) * mCellParam.Size.Height + Padding.Top;
            return new Point(x + mCellParam.Padding.Left, y + mCellParam.Padding.Top);
        }

        AddressPosition FromAbsoluteToScreenPos(AddressPosition absolutePos)
        {
            return absolutePos.Offest(-mVerticalPos, -mHorizontalPos);
        }

        AddressPosition FromScreenToAbsolutePos(AddressPosition absolutePos)
        {
            return absolutePos.Offest(mVerticalPos, mHorizontalPos);
        }

        void SetActiveCell(AddressPosition cellPosition = null)
        {
            if (cellPosition != null)
            {
                int row = Math.Max(0, Math.Min(mMemory.NumRows - 1, cellPosition.Row));
                int column = Math.Max(0, Math.Min(mMemory.CharactersInRow - 1, cellPosition.Column));
                mActiveCell = new AddressPosition(row, column);
            }

            CaretPos = GetCaretFromAddressPosition(mActiveCell != null ?
                FromAbsoluteToScreenPos(mActiveCell) : null);

            CurrentAddressPositionChanged?.Invoke(this, EventArgs.Empty);
        }

        class CellParam
        {
            public static Padding DEFAULT_PADDING = new Padding(2);

            const int NUM_CHARACTERS = 2;
            const int MIN_CHARACTER_SIZE = 1;

            bool mIsInitialized;

            int mCharacterWidth = MIN_CHARACTER_SIZE;
            int mCharacterHeight = MIN_CHARACTER_SIZE;

            Padding mPadding = DEFAULT_PADDING;
            Size mSize;

            public bool IsInitialized => mIsInitialized;
            public int CharacterWidth => mCharacterWidth;
            public int CharacterHeight => mCharacterHeight;
            public Size Size => mSize;

            public Padding Padding
            {
                get { return mPadding; }
                set
                {
                    if (mPadding == value)
                        return;

                    mPadding = value;
                    Update();
                }
            }

            public void Init(IntPtr hWnd, Font f)
            {
                using (Graphics g = Graphics.FromHwnd(hWnd))
                {
                    SizeF size = g.MeasureString("M", f);
                    mCharacterWidth = (int)Math.Ceiling(size.Width);
                    mCharacterHeight = (int)Math.Ceiling(size.Height);
                }

                mIsInitialized = true;

                Update();
            }

            void Update()
            {
                mSize = new Size(mCharacterWidth * NUM_CHARACTERS + mPadding.Horizontal,
                    mCharacterHeight + mPadding.Vertical);
            }
        }
    }
}
