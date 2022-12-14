using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rca301Emulator.UserInterface
{
    /// <summary>
    /// From http://xpathvisualizer.codeplex.com/SourceControl/changeset/view/42057#XPathVisualizer/CustomControls/RichTextBoxEx.cs
    /// </summary>
    public class RichTextBoxEx : RichTextBox
    {
        private Win32.CHARFORMAT charFormat;
        private IntPtr lParam1;

        private int _savedScrollLine;
        private int _savedSelectionStart;
        private int _savedSelectionEnd;
        private Pen _borderPen;
        private StringFormat _stringDrawingFormat;
        private System.Security.Cryptography.HashAlgorithm alg;   // used for comparing text values

        public RichTextBoxEx()
        {
            charFormat = new Win32.CHARFORMAT()
            {
                cbSize = Marshal.SizeOf(typeof(Win32.CHARFORMAT)),
                szFaceName = new char[32]
            };

            lParam1 = Marshal.AllocCoTaskMem(charFormat.cbSize);

            // defaults
            NumberFont = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

            NumberColor = Color.FromName("DarkGray");
            NumberLineCounting = LineCounting.CRLF;
            NumberAlignment = StringAlignment.Center;
            NumberBorder = SystemColors.ControlDark;
            NumberBorderThickness = 1;
            NumberPadding = 2;
            NumberBackground1 = SystemColors.ControlLight;
            NumberBackground2 = SystemColors.Window;
            SetStringDrawingFormat();

            alg = System.Security.Cryptography.SHA1.Create();
        }

        ~RichTextBoxEx()
        {
            // Free the allocated memory
            Marshal.FreeCoTaskMem(lParam1);
        }

        private void SetStringDrawingFormat()
        {
            _stringDrawingFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = NumberAlignment,
                Trimming = StringTrimming.None,
            };
        }


        protected override void OnTextChanged(EventArgs e)
        {
            NeedRecomputeOfLineNumbers();
            base.OnTextChanged(e);
        }

        public void BeginUpdate()
        {
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.WM_SETREDRAW, 0, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.WM_SETREDRAW, 1, IntPtr.Zero);
        }


        public IntPtr BeginUpdateAndSuspendEvents()
        {
            // Stop redrawing:
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.WM_SETREDRAW, 0, IntPtr.Zero);
            // Stop sending of events:
            IntPtr eventMask = Win32.SendMessage(this.Handle, Win32.Msgs.EM_GETEVENTMASK, 0, IntPtr.Zero);

            return eventMask;
        }

        public void EndUpdateAndResumeEvents(IntPtr eventMask)
        {
            // turn on events
            Win32.SendMessage(this.Handle, Win32.Msgs.EM_SETEVENTMASK, 0, eventMask);
            // turn on redrawing
            Win32.SendMessage(this.Handle, Win32.Msgs.WM_SETREDRAW, 1, IntPtr.Zero);
            NeedRecomputeOfLineNumbers();
            this.Invalidate();
        }



        public void GetSelection(out int start, out int end)
        {
            Win32.SendMessageRef(this.Handle, (int)Win32.Msgs.EM_GETSEL, out start, out end);
        }

        public void SetSelection(int start, int end)
        {
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_SETSEL, start, end);
        }

        public void BeginUpdateAndSaveState()
        {
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.WM_SETREDRAW, 0, IntPtr.Zero);
            // save scroll position
            _savedScrollLine = FirstVisibleDisplayLine;

            // save selection
            GetSelection(out _savedSelectionStart, out _savedSelectionEnd);
        }

        public void EndUpdateAndRestoreState()
        {
            // restore scroll position
            int Line1 = FirstVisibleDisplayLine;
            Scroll(_savedScrollLine - Line1);

            // restore the selection/caret
            SetSelection(_savedSelectionStart, _savedSelectionEnd);

            // allow redraw
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.WM_SETREDRAW, 1, IntPtr.Zero);

            // explicitly ask for a redraw?
            Refresh();
        }

        private String _sformat;
        private int _ndigits;
        private int _lnw = -1;
        private int LineNumberWidth
        {
            get
            {
                if (_lnw > 0) return _lnw;
                if (NumberLineCounting == LineCounting.CRLF)
                {
                    _ndigits = (CharIndexForTextLine.Length == 0)
                        ? 1
                        : (int)(1 + Math.Log((double)CharIndexForTextLine.Length, 10));
                }
                else
                {
                    int n = GetDisplayLineCount();
                    _ndigits = (n == 0)
                        ? 1
                        : (int)(1 + Math.Log((double)n, 10));
                }
                var s = new String('0', _ndigits);
                var b = new Bitmap(400, 400); // in pixels
                var g = Graphics.FromImage(b);
                SizeF size = g.MeasureString(s, NumberFont);
                g.Dispose();
                _lnw = NumberPadding * 2 + 4 + (int)(size.Width + 0.5 + NumberBorderThickness);
                _sformat = "{0:D" + _ndigits + "}";
                return _lnw;
            }
        }


        public bool _lineNumbers;
        public bool ShowLineNumbers
        {
            get
            {
                return _lineNumbers;
            }
            set
            {
                if (value == _lineNumbers) return;
                SetLeftMargin(value ? LineNumberWidth + Margin.Left : Margin.Left);
                _lineNumbers = value;
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        private void NeedRecomputeOfLineNumbers()
        {
            //System.Console.WriteLine("Need Recompute of line numbers...");
            _CharIndexForTextLine = null;
            _Text2 = null;
            _lnw = -1;

            if (_paintingDisabled) return;

            Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
        }

        private Font _NumberFont;
        public Font NumberFont
        {
            get { return _NumberFont; }
            set
            {
                if (_NumberFont == value) return;
                _lnw = -1;
                _NumberFont = value;
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        private LineCounting _NumberLineCounting;
        public LineCounting NumberLineCounting
        {
            get { return _NumberLineCounting; }
            set
            {
                if (_NumberLineCounting == value) return;
                _lnw = -1;
                _NumberLineCounting = value;
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        private StringAlignment _NumberAlignment;
        public StringAlignment NumberAlignment
        {
            get { return _NumberAlignment; }
            set
            {
                if (_NumberAlignment == value) return;
                _NumberAlignment = value;
                SetStringDrawingFormat();
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        private Color _NumberColor;
        public Color NumberColor
        {
            get { return _NumberColor; }
            set
            {
                if (_NumberColor.ToArgb() == value.ToArgb()) return;
                _NumberColor = value;
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        private bool _NumberLeadingZeroes;
        public bool NumberLeadingZeroes
        {
            get { return _NumberLeadingZeroes; }
            set
            {
                if (_NumberLeadingZeroes == value) return;
                _NumberLeadingZeroes = value;
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        private Color _NumberBorder;
        public Color NumberBorder
        {
            get { return _NumberBorder; }
            set
            {
                if (_NumberBorder.ToArgb() == value.ToArgb()) return;
                _NumberBorder = value;
                NewBorderPen();
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        private int _NumberPadding;
        public int NumberPadding
        {
            get { return _NumberPadding; }
            set
            {
                if (_NumberPadding == value) return;
                _lnw = -1;
                _NumberPadding = value;
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        public Single _NumberBorderThickness;
        public Single NumberBorderThickness
        {
            get { return _NumberBorderThickness; }
            set
            {
                if (_NumberBorderThickness == value) return;
                _lnw = -1;
                _NumberBorderThickness = value;
                NewBorderPen();
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        private Color _NumberBackground1;
        public Color NumberBackground1
        {
            get { return _NumberBackground1; }
            set
            {
                if (_NumberBackground1.ToArgb() == value.ToArgb()) return;
                _NumberBackground1 = value;
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }

        private Color _NumberBackground2;
        public Color NumberBackground2
        {
            get { return _NumberBackground2; }
            set
            {
                if (_NumberBackground2.ToArgb() == value.ToArgb()) return;
                _NumberBackground2 = value;
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
            }
        }


        private bool _paintingDisabled;
        public void SuspendLineNumberPainting()
        {
            _paintingDisabled = true;
        }
        public void ResumeLineNumberPainting()
        {
            _paintingDisabled = false;
        }


        private void NewBorderPen()
        {
            _borderPen = new Pen(NumberBorder);
            _borderPen.Width = NumberBorderThickness;
            _borderPen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
        }



        private DateTime _lastMsgRecd = new DateTime(1901, 1, 1);

        protected override void WndProc(ref Message m)
        {
            bool handled = false;
            switch (m.Msg)
            {
                case (int)Win32.Msgs.WM_PAINT:
                    //System.Console.WriteLine("{0}", User32.Mnemonic(m.Msg));
                    //System.Console.Write(".");
                    if (_paintingDisabled) return;
                    if (_lineNumbers)
                    {
                        base.WndProc(ref m);
                        this.PaintLineNumbers();
                        handled = true;
                    }
                    break;

                case (int)Win32.Msgs.WM_CHAR:
                    // the text is being modified
                    NeedRecomputeOfLineNumbers();
                    break;

                    //                 case (int)User32.Msgs.EM_POSFROMCHAR:
                    //                 case (int)User32.Msgs.WM_GETDLGCODE:
                    //                 case (int)User32.Msgs.WM_ERASEBKGND:
                    //                 case (int)User32.Msgs.OCM_COMMAND:
                    //                 case (int)User32.Msgs.OCM_NOTIFY:
                    //                 case (int)User32.Msgs.EM_CHARFROMPOS:
                    //                 case (int)User32.Msgs.EM_LINEINDEX:
                    //                 case (int)User32.Msgs.WM_NCHITTEST:
                    //                 case (int)User32.Msgs.WM_SETCURSOR:
                    //                 case (int)User32.Msgs.WM_KEYUP:
                    //                 case (int)User32.Msgs.WM_KEYDOWN:
                    //                 case (int)User32.Msgs.WM_MOUSEMOVE:
                    //                 case (int)User32.Msgs.WM_MOUSEACTIVATE:
                    //                 case (int)User32.Msgs.WM_NCMOUSEMOVE:
                    //                 case (int)User32.Msgs.WM_NCMOUSEHOVER:
                    //                 case (int)User32.Msgs.WM_NCMOUSELEAVE:
                    //                 case (int)User32.Msgs.WM_NCLBUTTONDOWN:
                    //                     break;
                    //
                    //                   default:
                    //                       // divider
                    //                       var now = DateTime.Now;
                    //                       if ((now - _lastMsgRecd) > TimeSpan.FromMilliseconds(850))
                    //                           System.Console.WriteLine("------------ {0}", now.ToString("G"));
                    //                       _lastMsgRecd = now;
                    //
                    //                       System.Console.WriteLine("{0}", User32.Mnemonic(m.Msg));
                    //                       break;
            }

            if (!handled)
                base.WndProc(ref m);
        }


        int _lastWidth = 0;
        private void PaintLineNumbers()
        {
            //System.Console.WriteLine(">> PaintLineNumbers");
            // To reduce flicker, double-buffer the output

            if (_paintingDisabled) return;

            int w = LineNumberWidth;
            if (w != _lastWidth)
            {
                //System.Console.WriteLine("  WIDTH change {0} != {1}", _lastWidth, w);
                SetLeftMargin(w + Margin.Left);
                _lastWidth = w;
                // Don't bother painting line numbers - the margin isn't wide enough currently.
                // Ask for a new paint, and paint them next time round.
                Win32.SendMessage(this.Handle, Win32.Msgs.WM_PAINT, 0, 0);
                return;
            }

            Bitmap buffer = new Bitmap(w, this.Bounds.Height);
            Graphics g = Graphics.FromImage(buffer);

            Brush forebrush = new SolidBrush(NumberColor);
            var rect = new Rectangle(0, 0, w, this.Bounds.Height);

            bool wantDivider = NumberBackground1.ToArgb() == NumberBackground2.ToArgb();
            Brush backBrush = (wantDivider)
                ? (Brush)new SolidBrush(NumberBackground2)
                : SystemBrushes.Window;

            g.FillRectangle(backBrush, rect);

            int n = (NumberLineCounting == LineCounting.CRLF)
                ? NumberOfVisibleTextLines
                : NumberOfVisibleDisplayLines;

            int first = (NumberLineCounting == LineCounting.CRLF)
                ? FirstVisibleTextLine
                : FirstVisibleDisplayLine + 1;

            int py = 0;
            int w2 = w - 2 - (int)NumberBorderThickness;
            LinearGradientBrush brush;
            Pen dividerPen = new Pen(NumberColor);

            for (int i = 0; i <= n; i++)
            {
                int ix = first + i;
                int c = (NumberLineCounting == LineCounting.CRLF)
                    ? GetCharIndexForTextLine(ix)
                    : GetCharIndexForDisplayLine(ix) - 1;

                var p = GetPosFromCharIndex(c + 1);

                Rectangle r4 = Rectangle.Empty;

                if (i == n) // last line?
                {
                    if (this.Bounds.Height <= py) continue;
                    r4 = new Rectangle(1, py, w2, this.Bounds.Height - py);
                }
                else
                {
                    if (p.Y <= py) continue;
                    r4 = new Rectangle(1, py, w2, p.Y - py);
                }

                if (wantDivider)
                {
                    if (i != n)
                        g.DrawLine(dividerPen, 1, p.Y + 1, w2, p.Y + 1); // divider line
                }
                else
                {
                    // new brush each time for gradient across variable rect sizes
                    brush = new LinearGradientBrush(r4,
                                                     NumberBackground1,
                                                     NumberBackground2,
                                                     LinearGradientMode.Vertical);
                    g.FillRectangle(brush, r4);
                }

                if (NumberLineCounting == LineCounting.CRLF) ix++;

                // conditionally slide down
                if (NumberAlignment == StringAlignment.Near)
                    rect.Offset(0, 3);

                var s = (NumberLeadingZeroes) ? String.Format(_sformat, ix) : ix.ToString();
                g.DrawString(s, NumberFont, forebrush, r4, _stringDrawingFormat);
                py = p.Y;
            }

            if (NumberBorderThickness != 0.0)
            {
                int t = (int)(w - (NumberBorderThickness + 0.5) / 2) - 1;
                g.DrawLine(_borderPen, t, 0, t, this.Bounds.Height);
                //g.DrawLine(_borderPen, w-2, 0, w-2, this.Bounds.Height);
            }

            // paint that buffer to the screen
            Graphics g1 = this.CreateGraphics();
            g1.DrawImage(buffer, new Point(0, 0));
            g1.Dispose();
            g.Dispose();
        }



        private int GetCharIndexFromPos(int x, int y)
        {
            var p = new Win32.POINTL { X = x, Y = y };
            int rawSize = Marshal.SizeOf(typeof(Win32.POINTL));
            IntPtr lParam = Marshal.AllocHGlobal(rawSize);
            Marshal.StructureToPtr(p, lParam, false);
            int r = Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_CHARFROMPOS, 0, lParam);
            Marshal.FreeHGlobal(lParam);
            return r;
        }


        private Point GetPosFromCharIndex(int ix)
        {
            int rawSize = Marshal.SizeOf(typeof(Win32.POINTL));
            IntPtr wParam = Marshal.AllocHGlobal(rawSize);
            int r = Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_POSFROMCHAR, (int)wParam, ix);

            Win32.POINTL p1 = (Win32.POINTL)Marshal.PtrToStructure(wParam, typeof(Win32.POINTL));

            Marshal.FreeHGlobal(wParam);
            var p = new Point { X = p1.X, Y = p1.Y };
            return p;
        }


        private int GetLengthOfLineContainingChar(int charIndex)
        {
            int r = Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_LINELENGTH, 0, 0);
            return r;
        }

        private int GetLineFromChar(int charIndex)
        {
            return Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_LINEFROMCHAR, charIndex, 0);
        }

        private int GetCharIndexForDisplayLine(int line)
        {
            return Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_LINEINDEX, line, 0);
        }

        private int GetDisplayLineCount()
        {
            return Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_GETLINECOUNT, 0, 0);
        }


        /// <summary>
        ///   Sets the color of the characters in the given range.
        /// </summary>
        ///
        /// <remarks>
        /// Calling this is equivalent to calling
        /// <code>
        ///   richTextBox.Select(start, end-start);
        ///   this.richTextBox1.SelectionColor = color;
        /// </code>
        /// ...but without the error and bounds checking.
        /// </remarks>
        ///
        public void SetSelectionColor(int start, int end, Color color)
        {
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_SETSEL, start, end);

            charFormat.dwMask = 0x40000000;
            charFormat.dwEffects = 0;
            charFormat.crTextColor = ColorTranslator.ToWin32(color);

            Marshal.StructureToPtr(charFormat, lParam1, false);
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_SETCHARFORMAT, Win32.SCF_SELECTION, lParam1);
        }


        private void SetLeftMargin(int widthInPixels)
        {
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_SETMARGINS, Win32.EC_LEFTMARGIN,
                               widthInPixels);
        }

        public Tuple<int, int> GetMargins()
        {
            int r = Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_GETMARGINS, 0, 0);
            return new Tuple<int, int>(r & 0x0000FFFF, (int)((r >> 16) & 0x0000FFFF));
        }

        public void Scroll(int delta)
        {
            Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_LINESCROLL, 0, delta);
        }


        private int FirstVisibleDisplayLine
        {
            get
            {
                return Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_GETFIRSTVISIBLELINE, 0, 0);
            }
            set
            {
                // scroll
                int current = FirstVisibleDisplayLine;
                int delta = value - current;
                Win32.SendMessage(this.Handle, (int)Win32.Msgs.EM_LINESCROLL, 0, delta);
            }
        }

        private int NumberOfVisibleDisplayLines
        {
            get
            {
                int topIndex = this.GetCharIndexFromPosition(new Point(1, 1));
                int bottomIndex = this.GetCharIndexFromPosition(new Point(1, this.Height - 1));
                int topLine = this.GetLineFromCharIndex(topIndex);
                int bottomLine = this.GetLineFromCharIndex(bottomIndex);
                int n = bottomLine - topLine + 1;
                return n;
            }
        }

        private int FirstVisibleTextLine
        {
            get
            {
                int c = GetCharIndexFromPos(1, 1);
                for (int i = 0; i < CharIndexForTextLine.Length; i++)
                {
                    if (c < CharIndexForTextLine[i]) return i;
                }
                return CharIndexForTextLine.Length;
            }
        }

        private int LastVisibleTextLine
        {
            get
            {
                int c = GetCharIndexFromPos(1, this.Bounds.Y + this.Bounds.Height);
                for (int i = 0; i < CharIndexForTextLine.Length; i++)
                {
                    if (c < CharIndexForTextLine[i]) return i;
                }
                return CharIndexForTextLine.Length;
            }
        }

        private int NumberOfVisibleTextLines
        {
            get
            {
                return LastVisibleTextLine - FirstVisibleTextLine;
            }
        }


        public int FirstVisibleLine
        {
            get
            {
                if (this.NumberLineCounting == LineCounting.CRLF)
                    return FirstVisibleTextLine;
                else
                    return FirstVisibleDisplayLine;
            }
        }

        public int NumberOfVisibleLines
        {
            get
            {
                if (this.NumberLineCounting == LineCounting.CRLF)
                    return NumberOfVisibleTextLines;
                else
                    return NumberOfVisibleDisplayLines;
            }
        }

        private int GetCharIndexForTextLine(int ix)
        {
            if (ix >= CharIndexForTextLine.Length) return 0;
            if (ix < 0) return 0;
            return CharIndexForTextLine[ix];
        }



        // The char index is expensive to compute.

        private int[] _CharIndexForTextLine;
        private int[] CharIndexForTextLine
        {
            get
            {
                if (_CharIndexForTextLine == null)
                {
                    var list = new List<int>();
                    int ix = 0;
                    foreach (var c in Text2)
                    {
                        if (c == '\n') list.Add(ix);
                        ix++;
                    }
                    _CharIndexForTextLine = list.ToArray();
                }
                return _CharIndexForTextLine;
            }

        }


        private String _Text2;
        private String Text2
        {
            get
            {
                if (_Text2 == null)
                    _Text2 = this.Text;
                return _Text2;
            }
        }

        public enum LineCounting
        {
            CRLF,
            AsDisplayed
        }

    }
}
