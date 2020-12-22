﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel.Design;


namespace CS_ClassLibraryTester
{
    //[INFO]
    //ThemeBase Creator: Aeonhack
    //Site: *********
    //[DATE]
    //Created: 08/02/2011
    //Changed: 12/06/2011
    //[VERSION]
    //ThemeBase Version: 1.5.4
    //
    //[INFO]
    //Theme Creator: Novi
    //Theme Name: CarbonOrainsTheme
    //[DATE]
    //Created: 7/14/2013
    //Changed: 7/26/2013
    //Released: 7/27/2013
    //[VERSION]
    //Version: 1.1
    //[CREDITS]
    //Thanks to Mavamaarten for the tut =))
    //Thanks to Aeonhack for the important ThemeBase154
    //--------[/CREDITS]------------
   
    #region "THEMEBASE"
    abstract class ThemeContainer154 : ContainerControl
    {

        #region " Initialization "

        protected Graphics G;

        protected Bitmap B;
        public ThemeContainer154()
        {
            SetStyle((ControlStyles)139270, true);

            _ImageSize = Size.Empty;
            Font = new Font("Verdana", 8);

            MeasureBitmap = new Bitmap(1, 1);
            MeasureGraphics = Graphics.FromImage(MeasureBitmap);

            DrawRadialPath = new GraphicsPath();

            InvalidateCustimization();
        }

        protected override sealed void OnHandleCreated(EventArgs e)
        {
            if (DoneCreation)
                InitializeMessages();

            InvalidateCustimization();
            ColorHook();

            if (!(_LockWidth == 0))
                Width = _LockWidth;
            if (!(_LockHeight == 0))
                Height = _LockHeight;
            if (!_ControlMode)
                base.Dock = DockStyle.Fill;

            Transparent = _Transparent;
            if (_Transparent && _BackColor)
                BackColor = Color.Transparent;

            base.OnHandleCreated(e);
        }

        private bool DoneCreation;
        protected override sealed void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (Parent == null)
                return;
            _IsParentForm = Parent is Form;

            if (!_ControlMode)
            {
                InitializeMessages();

                if (_IsParentForm)
                {
                    ParentForm.FormBorderStyle = _BorderStyle;
                    ParentForm.TransparencyKey = _TransparencyKey;

                    if (!DesignMode)
                    {
                        ParentForm.Shown += FormShown;
                    }
                }

                Parent.BackColor = BackColor;
            }

            OnCreation();
            DoneCreation = true;
            InvalidateTimer();
        }

        #endregion

        private void DoAnimation(bool i)
        {
            OnAnimation();
            if (i)
                Invalidate();
        }

        protected override sealed void OnPaint(PaintEventArgs e)
        {
            if (Width == 0 || Height == 0)
                return;

            if (_Transparent && _ControlMode)
            {
                PaintHook();
                e.Graphics.DrawImage(B, 0, 0);
            }
            else {
                G = e.Graphics;
                PaintHook();
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ThemeShare.RemoveAnimationCallback(DoAnimation);
            base.OnHandleDestroyed(e);
        }

        private bool HasShown;
        private void FormShown(object sender, EventArgs e)
        {
            if (_ControlMode || HasShown)
                return;

            if (_StartPosition == FormStartPosition.CenterParent || _StartPosition == FormStartPosition.CenterScreen)
            {
                Rectangle SB = Screen.PrimaryScreen.Bounds;
                Rectangle CB = ParentForm.Bounds;
                ParentForm.Location = new Point(SB.Width / 2 - CB.Width / 2, SB.Height / 2 - CB.Width / 2);
            }

            HasShown = true;
        }


        #region " Size Handling "

        private Rectangle Frame;
        protected override sealed void OnSizeChanged(EventArgs e)
        {
            if (_Movable && !_ControlMode)
            {
                Frame = new Rectangle(7, 7, Width - 14, _Header - 7);
            }

            InvalidateBitmap();
            Invalidate();

            base.OnSizeChanged(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!(_LockWidth == 0))
                width = _LockWidth;
            if (!(_LockHeight == 0))
                height = _LockHeight;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        #endregion

        #region " State Handling "

        protected MouseState State;
        private void SetState(MouseState current)
        {
            State = current;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized))
            {
                if (_Sizable && !_ControlMode)
                    InvalidateMouse();
            }

            base.OnMouseMove(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
                SetState(MouseState.None);
            else
                SetState(MouseState.Block);
            base.OnEnabledChanged(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            SetState(MouseState.Over);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            SetState(MouseState.Over);
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            SetState(MouseState.None);

            if (GetChildAtPoint(PointToClient(MousePosition)) != null)
            {
                if (_Sizable && !_ControlMode)
                {
                    Cursor = Cursors.Default;
                    Previous = 0;
                }
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SetState(MouseState.Down);

            if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized || _ControlMode))
            {
                if (_Movable && Frame.Contains(e.Location))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc( ref Messages[0]);
                }
                else if (_Sizable && !(Previous == 0))
                {
                    Capture = false;
                    WM_LMBUTTONDOWN = true;
                    DefWndProc(ref Messages[Previous]);
                }
            }

            base.OnMouseDown(e);
        }

        private bool WM_LMBUTTONDOWN;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (WM_LMBUTTONDOWN && m.Msg == 513)
            {
                WM_LMBUTTONDOWN = false;

                SetState(MouseState.Over);
                if (!_SmartBounds)
                    return;

                if (IsParentMdi)
                {
                    CorrectBounds(new Rectangle(Point.Empty, Parent.Parent.Size));
                }
                else {
                    CorrectBounds(Screen.FromControl(Parent).WorkingArea);
                }
            }
        }

        private Point GetIndexPoint;
        private bool B1;
        private bool B2;
        private bool B3;
        private bool B4;
        private int GetIndex()
        {
            GetIndexPoint = PointToClient(MousePosition);
            B1 = GetIndexPoint.X < 7;
            B2 = GetIndexPoint.X > Width - 7;
            B3 = GetIndexPoint.Y < 7;
            B4 = GetIndexPoint.Y > Height - 7;

            if (B1 && B3)
                return 4;
            if (B1 && B4)
                return 7;
            if (B2 && B3)
                return 5;
            if (B2 && B4)
                return 8;
            if (B1)
                return 1;
            if (B2)
                return 2;
            if (B3)
                return 3;
            if (B4)
                return 6;
            return 0;
        }

        private int Current;
        private int Previous;
        private void InvalidateMouse()
        {
            Current = GetIndex();
            if (Current == Previous)
                return;

            Previous = Current;
            switch (Previous)
            {
                case 0:
                    Cursor = Cursors.Default;
                    break;
                case 1:
                case 2:
                    Cursor = Cursors.SizeWE;
                    break;
                case 3:
                case 6:
                    Cursor = Cursors.SizeNS;
                    break;
                case 4:
                case 8:
                    Cursor = Cursors.SizeNWSE;
                    break;
                case 5:
                case 7:
                    Cursor = Cursors.SizeNESW;
                    break;
            }
        }

        private Message[] Messages = new Message[9];
        private void InitializeMessages()
        {
            Messages[0] = Message.Create(Parent.Handle, 161, new IntPtr(2), IntPtr.Zero);
            for (int I = 1; I <= 8; I++)
            {
                Messages[I] = Message.Create(Parent.Handle, 161, new IntPtr(I + 9), IntPtr.Zero);
            }
        }

        private void CorrectBounds(Rectangle bounds)
        {
            if (Parent.Width > bounds.Width)
                Parent.Width = bounds.Width;
            if (Parent.Height > bounds.Height)
                Parent.Height = bounds.Height;

            int X = Parent.Location.X;
            int Y = Parent.Location.Y;

            if (X < bounds.X)
                X = bounds.X;
            if (Y < bounds.Y)
                Y = bounds.Y;

            int Width = bounds.X + bounds.Width;
            int Height = bounds.Y + bounds.Height;

            if (X + Parent.Width > Width)
                X = Width - Parent.Width;
            if (Y + Parent.Height > Height)
                Y = Height - Parent.Height;

            Parent.Location = new Point(X, Y);
        }

        #endregion


        #region " Base Properties "

        public override DockStyle Dock
        {
            get { return base.Dock; }
            set
            {
                if (!_ControlMode)
                    return;
                base.Dock = value;
            }
        }

        private bool _BackColor;
        [Category("Misc")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (value == base.BackColor)
                    return;

                if (!IsHandleCreated && _ControlMode && value == Color.Transparent)
                {
                    _BackColor = true;
                    return;
                }

                base.BackColor = value;
                if (Parent != null)
                {
                    if (!_ControlMode)
                        Parent.BackColor = value;
                    ColorHook();
                }
            }
        }

        public override Size MinimumSize
        {
            get { return base.MinimumSize; }
            set
            {
                base.MinimumSize = value;
                if (Parent != null)
                    Parent.MinimumSize = value;
            }
        }

        public override Size MaximumSize
        {
            get { return base.MaximumSize; }
            set
            {
                base.MaximumSize = value;
                if (Parent != null)
                    Parent.MaximumSize = value;
            }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }

        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                Invalidate();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get { return Color.Empty; }
            set { }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get { return null; }
            set { }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return ImageLayout.None; }
            set { }
        }

        #endregion

        #region " Public Properties "

        private bool _SmartBounds = true;
        public bool SmartBounds
        {
            get { return _SmartBounds; }
            set { _SmartBounds = value; }
        }

        private bool _Movable = true;
        public bool Movable
        {
            get { return _Movable; }
            set { _Movable = value; }
        }

        private bool _Sizable = true;
        public bool Sizable
        {
            get { return _Sizable; }
            set { _Sizable = value; }
        }

        private Color _TransparencyKey;
        public Color TransparencyKey
        {
            get
            {
                if (_IsParentForm && !_ControlMode)
                    return ParentForm.TransparencyKey;
                else
                    return _TransparencyKey;
            }
            set
            {
                if (value == _TransparencyKey)
                    return;
                _TransparencyKey = value;

                if (_IsParentForm && !_ControlMode)
                {
                    ParentForm.TransparencyKey = value;
                    ColorHook();
                }
            }
        }

        private FormBorderStyle _BorderStyle;
        public FormBorderStyle BorderStyle
        {
            get
            {
                if (_IsParentForm && !_ControlMode)
                    return ParentForm.FormBorderStyle;
                else
                    return _BorderStyle;
            }
            set
            {
                _BorderStyle = value;

                if (_IsParentForm && !_ControlMode)
                {
                    ParentForm.FormBorderStyle = value;

                    if (!(value == FormBorderStyle.None))
                    {
                        Movable = false;
                        Sizable = false;
                    }
                }
            }
        }

        private FormStartPosition _StartPosition;
        public FormStartPosition StartPosition
        {
            get
            {
                if (_IsParentForm && !_ControlMode)
                    return ParentForm.StartPosition;
                else
                    return _StartPosition;
            }
            set
            {
                _StartPosition = value;

                if (_IsParentForm && !_ControlMode)
                {
                    ParentForm.StartPosition = value;
                }
            }
        }

        private bool _NoRounding;
        public bool NoRounding
        {
            get { return _NoRounding; }
            set
            {
                _NoRounding = value;
                Invalidate();
            }
        }

        private Image _Image;
        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == null)
                    _ImageSize = Size.Empty;
                else
                    _ImageSize = value.Size;

                _Image = value;
                Invalidate();
            }
        }

        private Dictionary<string, Color> Items = new Dictionary<string, Color>();
        public Bloom[] Colors
        {
            get
            {
                List<Bloom> T = new List<Bloom>();
                Dictionary<string, Color>.Enumerator E = Items.GetEnumerator();

                while (E.MoveNext())
                {
                    T.Add(new Bloom(E.Current.Key, E.Current.Value));
                }

                return T.ToArray();
            }
            set
            {
                foreach (Bloom B in value)
                {
                    if (Items.ContainsKey(B.Name))
                        Items[B.Name] = B.Value;
                }

                InvalidateCustimization();
                ColorHook();
                Invalidate();
            }
        }

        private string _Customization;
        public string Customization
        {
            get { return _Customization; }
            set
            {
                if (value == _Customization)
                    return;

                byte[] Data = null;
                Bloom[] Items = Colors;

                try
                {
                    Data = Convert.FromBase64String(value);
                    for (int I = 0; I <= Items.Length - 1; I++)
                    {
                        Items[I].Value = Color.FromArgb(BitConverter.ToInt32(Data, I * 4));
                    }
                }
                catch
                {
                    return;
                }

                _Customization = value;

                Colors = Items;
                ColorHook();
                Invalidate();
            }
        }

        private bool _Transparent;
        public bool Transparent
        {
            get { return _Transparent; }
            set
            {
                _Transparent = value;
                if (!(IsHandleCreated || _ControlMode))
                    return;

                if (!value && !(BackColor.A == 255))
                {
                    throw new Exception("Unable to change value to false while a transparent BackColor is in use.");
                }

                SetStyle(ControlStyles.Opaque, !value);
                SetStyle(ControlStyles.SupportsTransparentBackColor, value);

                InvalidateBitmap();
                Invalidate();
            }
        }

        #endregion

        #region " Private Properties "

        private Size _ImageSize;
        protected Size ImageSize
        {
            get { return _ImageSize; }
        }

        private bool _IsParentForm;
        protected bool IsParentForm
        {
            get { return _IsParentForm; }
        }

        protected bool IsParentMdi
        {
            get
            {
                if (Parent == null)
                    return false;
                return Parent.Parent != null;
            }
        }

        private int _LockWidth;
        protected int LockWidth
        {
            get { return _LockWidth; }
            set
            {
                _LockWidth = value;
                if (!(LockWidth == 0) && IsHandleCreated)
                    Width = LockWidth;
            }
        }

        private int _LockHeight;
        protected int LockHeight
        {
            get { return _LockHeight; }
            set
            {
                _LockHeight = value;
                if (!(LockHeight == 0) && IsHandleCreated)
                    Height = LockHeight;
            }
        }

        private int _Header = 24;
        protected int Header
        {
            get { return _Header; }
            set
            {
                _Header = value;

                if (!_ControlMode)
                {
                    Frame = new Rectangle(7, 7, Width - 14, value - 7);
                    Invalidate();
                }
            }
        }

        private bool _ControlMode;
        protected bool ControlMode
        {
            get { return _ControlMode; }
            set
            {
                _ControlMode = value;

                Transparent = _Transparent;
                if (_Transparent && _BackColor)
                    BackColor = Color.Transparent;

                InvalidateBitmap();
                Invalidate();
            }
        }

        private bool _IsAnimated;
        protected bool IsAnimated
        {
            get { return _IsAnimated; }
            set
            {
                _IsAnimated = value;
                InvalidateTimer();
            }
        }

        #endregion


        #region " Property Helpers "

        protected Pen GetPen(string name)
        {
            return new Pen(Items[name]);
        }
        protected Pen GetPen(string name, float width)
        {
            return new Pen(Items[name], width);
        }

        protected SolidBrush GetBrush(string name)
        {
            return new SolidBrush(Items[name]);
        }

        protected Color GetColor(string name)
        {
            return Items[name];
        }

        protected void SetColor(string name, Color value)
        {
            if (Items.ContainsKey(name))
               Items[name] = value;
            else
                Items.Add(name, value);
        }
        protected void SetColor(string name, byte r, byte g, byte b)
        {
            SetColor(name, Color.FromArgb(r, g, b));
        }
        protected void SetColor(string name, byte a, byte r, byte g, byte b)
        {
            SetColor(name, Color.FromArgb(a, r, g, b));
        }
        protected void SetColor(string name, byte a, Color value)
        {
            SetColor(name, Color.FromArgb(a, value));
        }

        private void InvalidateBitmap()
        {
            if (_Transparent && _ControlMode)
            {
                if (Width == 0 || Height == 0)
                    return;
                B = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
                G = Graphics.FromImage(B);
            }
            else {
                G = null;
                B = null;
            }
        }

        private void InvalidateCustimization()
        {
            MemoryStream M = new MemoryStream(Items.Count * 4);

            foreach (Bloom B in Colors)
            {
                M.Write(BitConverter.GetBytes(B.Value.ToArgb()), 0, 4);
            }

            M.Close();
            _Customization = Convert.ToBase64String(M.ToArray());
        }

        private void InvalidateTimer()
        {
            if (DesignMode || !DoneCreation)
                return;

            if (_IsAnimated)
            {
                ThemeShare.AddAnimationCallback(DoAnimation);
            }
            else {
                ThemeShare.RemoveAnimationCallback(DoAnimation);
            }
        }

        #endregion


        #region " User Hooks "

        protected abstract void ColorHook();
        protected abstract void PaintHook();

        protected virtual void OnCreation()
        {
        }

        protected virtual void OnAnimation()
        {
        }

        #endregion


        #region " Offset "

        private Rectangle OffsetReturnRectangle;
        protected Rectangle Offset(Rectangle r, int amount)
        {
            OffsetReturnRectangle = new Rectangle(r.X + amount, r.Y + amount, r.Width - (amount * 2), r.Height - (amount * 2));
            return OffsetReturnRectangle;
        }

        private Size OffsetReturnSize;
        protected Size Offset(Size s, int amount)
        {
            OffsetReturnSize = new Size(s.Width + amount, s.Height + amount);
            return OffsetReturnSize;
        }

        private Point OffsetReturnPoint;
        protected Point Offset(Point p, int amount)
        {
            OffsetReturnPoint = new Point(p.X + amount, p.Y + amount);
            return OffsetReturnPoint;
        }

        #endregion

        #region " Center "


        private Point CenterReturn;
        protected Point Center(Rectangle p, Rectangle c)
        {
            CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X + c.X, (p.Height / 2 - c.Height / 2) + p.Y + c.Y);
            return CenterReturn;
        }
        protected Point Center(Rectangle p, Size c)
        {
            CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X, (p.Height / 2 - c.Height / 2) + p.Y);
            return CenterReturn;
        }

        protected Point Center(Rectangle child)
        {
            return Center(Width, Height, child.Width, child.Height);
        }
        protected Point Center(Size child)
        {
            return Center(Width, Height, child.Width, child.Height);
        }
        protected Point Center(int childWidth, int childHeight)
        {
            return Center(Width, Height, childWidth, childHeight);
        }

        protected Point Center(Size p, Size c)
        {
            return Center(p.Width, p.Height, c.Width, c.Height);
        }

        protected Point Center(int pWidth, int pHeight, int cWidth, int cHeight)
        {
            CenterReturn = new Point(pWidth / 2 - cWidth / 2, pHeight / 2 - cHeight / 2);
            return CenterReturn;
        }

        #endregion

        #region " Measure "

        private Bitmap MeasureBitmap;

        private Graphics MeasureGraphics;
        protected Size Measure()
        {
            lock (MeasureGraphics)
            {
                return MeasureGraphics.MeasureString(Text, Font, Width).ToSize();
            }
        }
        protected Size Measure(string text)
        {
            lock (MeasureGraphics)
            {
                return MeasureGraphics.MeasureString(text, Font, Width).ToSize();
            }
        }

        #endregion


        #region " DrawPixel "


        private SolidBrush DrawPixelBrush;
        protected void DrawPixel(Color c1, int x, int y)
        {
            if (_Transparent)
            {
                B.SetPixel(x, y, c1);
            }
            else {
                DrawPixelBrush = new SolidBrush(c1);
                G.FillRectangle(DrawPixelBrush, x, y, 1, 1);
            }
        }

        #endregion

        #region " DrawCorners "


        private SolidBrush DrawCornersBrush;
        protected void DrawCorners(Color c1, int offset)
        {
            DrawCorners(c1, 0, 0, Width, Height, offset);
        }
        protected void DrawCorners(Color c1, Rectangle r1, int offset)
        {
            DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height, offset);
        }
        protected void DrawCorners(Color c1, int x, int y, int width, int height, int offset)
        {
            DrawCorners(c1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }

        protected void DrawCorners(Color c1)
        {
            DrawCorners(c1, 0, 0, Width, Height);
        }
        protected void DrawCorners(Color c1, Rectangle r1)
        {
            DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height);
        }
        protected void DrawCorners(Color c1, int x, int y, int width, int height)
        {
            if (_NoRounding)
                return;

            if (_Transparent)
            {
                B.SetPixel(x, y, c1);
                B.SetPixel(x + (width - 1), y, c1);
                B.SetPixel(x, y + (height - 1), c1);
                B.SetPixel(x + (width - 1), y + (height - 1), c1);
            }
            else {
                DrawCornersBrush = new SolidBrush(c1);
                G.FillRectangle(DrawCornersBrush, x, y, 1, 1);
                G.FillRectangle(DrawCornersBrush, x + (width - 1), y, 1, 1);
                G.FillRectangle(DrawCornersBrush, x, y + (height - 1), 1, 1);
                G.FillRectangle(DrawCornersBrush, x + (width - 1), y + (height - 1), 1, 1);
            }
        }

        #endregion

        #region " DrawBorders "

        protected void DrawBorders(Pen p1, int offset)
        {
            DrawBorders(p1, 0, 0, Width, Height, offset);
        }
        protected void DrawBorders(Pen p1, Rectangle r, int offset)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
        }
        protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
        {
            DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }

        protected void DrawBorders(Pen p1)
        {
            DrawBorders(p1, 0, 0, Width, Height);
        }
        protected void DrawBorders(Pen p1, Rectangle r)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
        }
        protected void DrawBorders(Pen p1, int x, int y, int width, int height)
        {
            G.DrawRectangle(p1, x, y, width - 1, height - 1);
        }

        #endregion

        #region " DrawText "

        private Point DrawTextPoint;

        private Size DrawTextSize;
        protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
        {
            DrawText(b1, Text, a, x, y);
        }
        protected void DrawText(Brush b1, string text, HorizontalAlignment a, int x, int y)
        {
            if (text.Length == 0)
                return;

            DrawTextSize = Measure(text);
            DrawTextPoint = new Point(Width / 2 - DrawTextSize.Width / 2, Header / 2 - DrawTextSize.Height / 2);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawString(text, Font, b1, x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawString(text, Font, b1, DrawTextPoint.X + x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawString(text, Font, b1, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
                    break;
            }
        }

        protected void DrawText(Brush b1, Point p1)
        {
            if (Text.Length == 0)
                return;
            G.DrawString(Text, Font, b1, p1);
        }
        protected void DrawText(Brush b1, int x, int y)
        {
            if (Text.Length == 0)
                return;
            G.DrawString(Text, Font, b1, x, y);
        }

        #endregion

        #region " DrawImage "


        private Point DrawImagePoint;
        protected void DrawImage(HorizontalAlignment a, int x, int y)
        {
            DrawImage(_Image, a, x, y);
        }
        protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
        {
            if (image == null)
                return;
            DrawImagePoint = new Point(Width / 2 - image.Width / 2, Header / 2 - image.Height / 2);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawImage(image, x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
            }
        }

        protected void DrawImage(Point p1)
        {
            DrawImage(_Image, p1.X, p1.Y);
        }
        protected void DrawImage(int x, int y)
        {
            DrawImage(_Image, x, y);
        }

        protected void DrawImage(Image image, Point p1)
        {
            DrawImage(image, p1.X, p1.Y);
        }
        protected void DrawImage(Image image, int x, int y)
        {
            if (image == null)
                return;
            G.DrawImage(image, x, y, image.Width, image.Height);
        }

        #endregion

        #region " DrawGradient "

        private LinearGradientBrush DrawGradientBrush;

        private Rectangle DrawGradientRectangle;
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle);
        }
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(ColorBlend blend, Rectangle r)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, 90f);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }


        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle);
        }
        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(Color c1, Color c2, Rectangle r)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, 90f);
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
            G.FillRectangle(DrawGradientBrush, r);
        }

        #endregion

        #region " DrawRadial "

        private GraphicsPath DrawRadialPath;
        private PathGradientBrush DrawRadialBrush1;
        private LinearGradientBrush DrawRadialBrush2;

        private Rectangle DrawRadialRectangle;
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, width / 2, height / 2);
        }
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, Point center)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, center.X, center.Y);
        }
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, int cx, int cy)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, cx, cy);
        }

        public void DrawRadial(ColorBlend blend, Rectangle r)
        {
            DrawRadial(blend, r, r.Width / 2, r.Height / 2);
        }
        public void DrawRadial(ColorBlend blend, Rectangle r, Point center)
        {
            DrawRadial(blend, r, center.X, center.Y);
        }
        public void DrawRadial(ColorBlend blend, Rectangle r, int cx, int cy)
        {
            DrawRadialPath.Reset();
            DrawRadialPath.AddEllipse(r.X, r.Y, r.Width - 1, r.Height - 1);

            DrawRadialBrush1 = new PathGradientBrush(DrawRadialPath);
            DrawRadialBrush1.CenterPoint = new Point(r.X + cx, r.Y + cy);
            DrawRadialBrush1.InterpolationColors = blend;

            if (G.SmoothingMode == SmoothingMode.AntiAlias)
            {
                G.FillEllipse(DrawRadialBrush1, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3);
            }
            else {
                G.FillEllipse(DrawRadialBrush1, r);
            }
        }


        protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(c1, c2, DrawGradientRectangle);
        }
        protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(c1, c2, DrawGradientRectangle, angle);
        }

        protected void DrawRadial(Color c1, Color c2, Rectangle r)
        {
            DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, 90f);
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawRadial(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, angle);
            G.FillEllipse(DrawGradientBrush, r);
        }

        #endregion

        #region " CreateRound "

        private GraphicsPath CreateRoundPath;

        private Rectangle CreateRoundRectangle;
        public GraphicsPath CreateRound(int x, int y, int width, int height, int slope)
        {
            CreateRoundRectangle = new Rectangle(x, y, width, height);
            return CreateRound(CreateRoundRectangle, slope);
        }

        public GraphicsPath CreateRound(Rectangle r, int slope)
        {
            CreateRoundPath = new GraphicsPath(FillMode.Winding);
            CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0f, 90f);
            CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90f, 90f);
            CreateRoundPath.CloseFigure();
            return CreateRoundPath;
        }

        #endregion

    }

    abstract class ThemeControl154 : Control
    {


        #region " Initialization "

        protected Graphics G;

        protected Bitmap B;
        public ThemeControl154()
        {
            SetStyle((ControlStyles)139270, true);

            _ImageSize = Size.Empty;
            Font = new Font("Verdana", 8);

            MeasureBitmap = new Bitmap(1, 1);
            MeasureGraphics = Graphics.FromImage(MeasureBitmap);

            DrawRadialPath = new GraphicsPath();

            InvalidateCustimization();
            //Remove?
        }

        protected override sealed void OnHandleCreated(EventArgs e)
        {
            InvalidateCustimization();
            ColorHook();

            if (!(_LockWidth == 0))
                Width = _LockWidth;
            if (!(_LockHeight == 0))
                Height = _LockHeight;

            Transparent = _Transparent;
            if (_Transparent && _BackColor)
                BackColor = Color.Transparent;

            base.OnHandleCreated(e);
        }

        private bool DoneCreation;
        protected override sealed void OnParentChanged(EventArgs e)
        {
            if (Parent != null)
            {
                OnCreation();
                DoneCreation = true;
                InvalidateTimer();
            }

            base.OnParentChanged(e);
        }

        #endregion

        private void DoAnimation(bool i)
        {
            OnAnimation();
            if (i)
                Invalidate();
        }

        protected override sealed void OnPaint(PaintEventArgs e)
        {
            if (Width == 0 || Height == 0)
                return;

            if (_Transparent)
            {
                PaintHook();
                e.Graphics.DrawImage(B, 0, 0);
            }
            else {
                G = e.Graphics;
                PaintHook();
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ThemeShare.RemoveAnimationCallback(DoAnimation);
            base.OnHandleDestroyed(e);
        }

        #region " Size Handling "

        protected override sealed void OnSizeChanged(EventArgs e)
        {
            if (_Transparent)
            {
                InvalidateBitmap();
            }

            Invalidate();
            base.OnSizeChanged(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!(_LockWidth == 0))
                width = _LockWidth;
            if (!(_LockHeight == 0))
                height = _LockHeight;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        #endregion

        #region " State Handling "

        private bool InPosition;
        protected override void OnMouseEnter(EventArgs e)
        {
            InPosition = true;
            SetState(MouseState.Over);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (InPosition)
                SetState(MouseState.Over);
            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                SetState(MouseState.Down);
            base.OnMouseDown(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            InPosition = false;
            SetState(MouseState.None);
            base.OnMouseLeave(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled)
                SetState(MouseState.None);
            else
                SetState(MouseState.Block);
            base.OnEnabledChanged(e);
        }

        protected MouseState State;
        private void SetState(MouseState current)
        {
            State = current;
            Invalidate();
        }

        #endregion


        #region " Base Properties "

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get { return Color.Empty; }
            set { }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get { return null; }
            set { }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return ImageLayout.None; }
            set { }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                Invalidate();
            }
        }
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                Invalidate();
            }
        }

        private bool _BackColor;
        [Category("Misc")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                if (!IsHandleCreated && value == Color.Transparent)
                {
                    _BackColor = true;
                    return;
                }

                base.BackColor = value;
                if (Parent != null)
                    ColorHook();
            }
        }

        #endregion

        #region " Public Properties "

        private bool _NoRounding;
        public bool NoRounding
        {
            get { return _NoRounding; }
            set
            {
                _NoRounding = value;
                Invalidate();
            }
        }

        private Image _Image;
        public Image Image
        {
            get { return _Image; }
            set
            {
                if (value == null)
                {
                    _ImageSize = Size.Empty;
                }
                else {
                    _ImageSize = value.Size;
                }

                _Image = value;
                Invalidate();
            }
        }

        private bool _Transparent;
        public bool Transparent
        {
            get { return _Transparent; }
            set
            {
                _Transparent = value;
                if (!IsHandleCreated)
                    return;

                if (!value && !(BackColor.A == 255))
                {
                    throw new Exception("Unable to change value to false while a transparent BackColor is in use.");
                }

                SetStyle(ControlStyles.Opaque, !value);
                SetStyle(ControlStyles.SupportsTransparentBackColor, value);

                if (value)
                    InvalidateBitmap();
                else
                    B = null;
                Invalidate();
            }
        }

        private Dictionary<string, Color> Items = new Dictionary<string, Color>();
        public Bloom[] Colors
        {
            get
            {
                List<Bloom> T = new List<Bloom>();
                Dictionary<string, Color>.Enumerator E = Items.GetEnumerator();

                while (E.MoveNext())
                {
                    T.Add(new Bloom(E.Current.Key, E.Current.Value));
                }

                return T.ToArray();
            }
            set
            {
                foreach (Bloom B in value)
                {
                    if (Items.ContainsKey(B.Name))
                        Items[B.Name] = B.Value;
                }

                InvalidateCustimization();
                ColorHook();
                Invalidate();
            }
        }

        private string _Customization;
        public string Customization
        {
            get { return _Customization; }
            set
            {
                if (value == _Customization)
                    return;

                byte[] Data = null;
                Bloom[] Items = Colors;

                try
                {
                    Data = Convert.FromBase64String(value);
                    for (int I = 0; I <= Items.Length - 1; I++)
                    {
                        Items[I].Value = Color.FromArgb(BitConverter.ToInt32(Data, I * 4));
                    }
                }
                catch
                {
                    return;
                }

                _Customization = value;

                Colors = Items;
                ColorHook();
                Invalidate();
            }
        }

        #endregion

        #region " Private Properties "

        private Size _ImageSize;
        protected Size ImageSize
        {
            get { return _ImageSize; }
        }

        private int _LockWidth;
        protected int LockWidth
        {
            get { return _LockWidth; }
            set
            {
                _LockWidth = value;
                if (!(LockWidth == 0) && IsHandleCreated)
                    Width = LockWidth;
            }
        }

        private int _LockHeight;
        protected int LockHeight
        {
            get { return _LockHeight; }
            set
            {
                _LockHeight = value;
                if (!(LockHeight == 0) && IsHandleCreated)
                    Height = LockHeight;
            }
        }

        private bool _IsAnimated;
        protected bool IsAnimated
        {
            get { return _IsAnimated; }
            set
            {
                _IsAnimated = value;
                InvalidateTimer();
            }
        }

        #endregion


        #region " Property Helpers "

        protected Pen GetPen(string name)
        {
            return new Pen(Items[name]);
        }
        protected Pen GetPen(string name, float width)
        {
            return new Pen(Items[name], width);
        }

        protected SolidBrush GetBrush(string name)
        {
            return new SolidBrush(Items[name]);
        }

        protected Color GetColor(string name)
        {
            return Items[name];
        }

        protected void SetColor(string name, Color value)
        {
            if (Items.ContainsKey(name))
                Items[name] = value;
            else
                Items.Add(name, value);
        }
        protected void SetColor(string name, byte r, byte g, byte b)
        {
            SetColor(name, Color.FromArgb(r, g, b));
        }
        protected void SetColor(string name, byte a, byte r, byte g, byte b)
        {
            SetColor(name, Color.FromArgb(a, r, g, b));
        }
        protected void SetColor(string name, byte a, Color value)
        {
            SetColor(name, Color.FromArgb(a, value));
        }

        private void InvalidateBitmap()
        {
            if (Width == 0 || Height == 0)
                return;
            B = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
            G = Graphics.FromImage(B);
        }

        private void InvalidateCustimization()
        {
            MemoryStream M = new MemoryStream(Items.Count * 4);

            foreach (Bloom B in Colors)
            {
                M.Write(BitConverter.GetBytes(B.Value.ToArgb()), 0, 4);
            }

            M.Close();
            _Customization = Convert.ToBase64String(M.ToArray());
        }

        private void InvalidateTimer()
        {
            if (DesignMode || !DoneCreation)
                return;

            if (_IsAnimated)
            {
                ThemeShare.AddAnimationCallback(DoAnimation);
            }
            else {
                ThemeShare.RemoveAnimationCallback(DoAnimation);
            }
        }
        #endregion


        #region " User Hooks "

        protected abstract void ColorHook();
        protected abstract void PaintHook();

        protected virtual void OnCreation()
        {
        }

        protected virtual void OnAnimation()
        {
        }

        #endregion


        #region " Offset "

        private Rectangle OffsetReturnRectangle;
        protected Rectangle Offset(Rectangle r, int amount)
        {
            OffsetReturnRectangle = new Rectangle(r.X + amount, r.Y + amount, r.Width - (amount * 2), r.Height - (amount * 2));
            return OffsetReturnRectangle;
        }

        private Size OffsetReturnSize;
        protected Size Offset(Size s, int amount)
        {
            OffsetReturnSize = new Size(s.Width + amount, s.Height + amount);
            return OffsetReturnSize;
        }

        private Point OffsetReturnPoint;
        protected Point Offset(Point p, int amount)
        {
            OffsetReturnPoint = new Point(p.X + amount, p.Y + amount);
            return OffsetReturnPoint;
        }

        #endregion

        #region " Center "


        private Point CenterReturn;
        protected Point Center(Rectangle p, Rectangle c)
        {
            CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X + c.X, (p.Height / 2 - c.Height / 2) + p.Y + c.Y);
            return CenterReturn;
        }
        protected Point Center(Rectangle p, Size c)
        {
            CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X, (p.Height / 2 - c.Height / 2) + p.Y);
            return CenterReturn;
        }

        protected Point Center(Rectangle child)
        {
            return Center(Width, Height, child.Width, child.Height);
        }
        protected Point Center(Size child)
        {
            return Center(Width, Height, child.Width, child.Height);
        }
        protected Point Center(int childWidth, int childHeight)
        {
            return Center(Width, Height, childWidth, childHeight);
        }

        protected Point Center(Size p, Size c)
        {
            return Center(p.Width, p.Height, c.Width, c.Height);
        }

        protected Point Center(int pWidth, int pHeight, int cWidth, int cHeight)
        {
            CenterReturn = new Point(pWidth / 2 - cWidth / 2, pHeight / 2 - cHeight / 2);
            return CenterReturn;
        }

        #endregion

        #region " Measure "

        private Bitmap MeasureBitmap;
        //TODO: Potential issues during multi-threading.
        private Graphics MeasureGraphics;

        protected Size Measure()
        {
            return MeasureGraphics.MeasureString(Text, Font, Width).ToSize();
        }
        protected Size Measure(string text)
        {
            return MeasureGraphics.MeasureString(text, Font, Width).ToSize();
        }

        #endregion


        #region " DrawPixel "


        private SolidBrush DrawPixelBrush;
        protected void DrawPixel(Color c1, int x, int y)
        {
            if (_Transparent)
            {
                B.SetPixel(x, y, c1);
            }
            else {
                DrawPixelBrush = new SolidBrush(c1);
                G.FillRectangle(DrawPixelBrush, x, y, 1, 1);
            }
        }

        #endregion

        #region " DrawCorners "


        private SolidBrush DrawCornersBrush;
        protected void DrawCorners(Color c1, int offset)
        {
            DrawCorners(c1, 0, 0, Width, Height, offset);
        }
        protected void DrawCorners(Color c1, Rectangle r1, int offset)
        {
            DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height, offset);
        }
        protected void DrawCorners(Color c1, int x, int y, int width, int height, int offset)
        {
            DrawCorners(c1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }

        protected void DrawCorners(Color c1)
        {
            DrawCorners(c1, 0, 0, Width, Height);
        }
        protected void DrawCorners(Color c1, Rectangle r1)
        {
            DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height);
        }
        protected void DrawCorners(Color c1, int x, int y, int width, int height)
        {
            if (_NoRounding)
                return;

            if (_Transparent)
            {
                B.SetPixel(x, y, c1);
                B.SetPixel(x + (width - 1), y, c1);
                B.SetPixel(x, y + (height - 1), c1);
                B.SetPixel(x + (width - 1), y + (height - 1), c1);
            }
            else {
                DrawCornersBrush = new SolidBrush(c1);
                G.FillRectangle(DrawCornersBrush, x, y, 1, 1);
                G.FillRectangle(DrawCornersBrush, x + (width - 1), y, 1, 1);
                G.FillRectangle(DrawCornersBrush, x, y + (height - 1), 1, 1);
                G.FillRectangle(DrawCornersBrush, x + (width - 1), y + (height - 1), 1, 1);
            }
        }

        #endregion

        #region " DrawBorders "

        protected void DrawBorders(Pen p1, int offset)
        {
            DrawBorders(p1, 0, 0, Width, Height, offset);
        }
        protected void DrawBorders(Pen p1, Rectangle r, int offset)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
        }
        protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
        {
            DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
        }

        protected void DrawBorders(Pen p1)
        {
            DrawBorders(p1, 0, 0, Width, Height);
        }
        protected void DrawBorders(Pen p1, Rectangle r)
        {
            DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
        }
        protected void DrawBorders(Pen p1, int x, int y, int width, int height)
        {
            G.DrawRectangle(p1, x, y, width - 1, height - 1);
        }

        #endregion

        #region " DrawText "

        private Point DrawTextPoint;

        private Size DrawTextSize;
        protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
        {
            DrawText(b1, Text, a, x, y);
        }
        protected void DrawText(Brush b1, string text, HorizontalAlignment a, int x, int y)
        {
            if (text.Length == 0)
                return;

            DrawTextSize = Measure(text);
            DrawTextPoint = Center(DrawTextSize);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawString(text, Font, b1, x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawString(text, Font, b1, DrawTextPoint.X + x, DrawTextPoint.Y + y);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawString(text, Font, b1, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
                    break;
            }
        }

        protected void DrawText(Brush b1, Point p1)
        {
            if (Text.Length == 0)
                return;
            G.DrawString(Text, Font, b1, p1);
        }
        protected void DrawText(Brush b1, int x, int y)
        {
            if (Text.Length == 0)
                return;
            G.DrawString(Text, Font, b1, x, y);
        }

        #endregion

        #region " DrawImage "


        private Point DrawImagePoint;
        protected void DrawImage(HorizontalAlignment a, int x, int y)
        {
            DrawImage(_Image, a, x, y);
        }
        protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
        {
            if (image == null)
                return;
            DrawImagePoint = Center(image.Size);

            switch (a)
            {
                case HorizontalAlignment.Left:
                    G.DrawImage(image, x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
                case HorizontalAlignment.Center:
                    G.DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
                case HorizontalAlignment.Right:
                    G.DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y, image.Width, image.Height);
                    break;
            }
        }

        protected void DrawImage(Point p1)
        {
            DrawImage(_Image, p1.X, p1.Y);
        }
        protected void DrawImage(int x, int y)
        {
            DrawImage(_Image, x, y);
        }

        protected void DrawImage(Image image, Point p1)
        {
            DrawImage(image, p1.X, p1.Y);
        }
        protected void DrawImage(Image image, int x, int y)
        {
            if (image == null)
                return;
            G.DrawImage(image, x, y, image.Width, image.Height);
        }

        #endregion

        #region " DrawGradient "

        private LinearGradientBrush DrawGradientBrush;

        private Rectangle DrawGradientRectangle;
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle);
        }
        protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(blend, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(ColorBlend blend, Rectangle r)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, 90f);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
            DrawGradientBrush.InterpolationColors = blend;
            G.FillRectangle(DrawGradientBrush, r);
        }


        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle);
        }
        protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawGradientRectangle = new Rectangle(x, y, width, height);
            DrawGradient(c1, c2, DrawGradientRectangle, angle);
        }

        protected void DrawGradient(Color c1, Color c2, Rectangle r)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, 90f);
            G.FillRectangle(DrawGradientBrush, r);
        }
        protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
            G.FillRectangle(DrawGradientBrush, r);
        }

        #endregion

        #region " DrawRadial "

        private GraphicsPath DrawRadialPath;
        private PathGradientBrush DrawRadialBrush1;
        private LinearGradientBrush DrawRadialBrush2;

        private Rectangle DrawRadialRectangle;
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, width / 2, height / 2);
        }
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, Point center)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, center.X, center.Y);
        }
        public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, int cx, int cy)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(blend, DrawRadialRectangle, cx, cy);
        }

        public void DrawRadial(ColorBlend blend, Rectangle r)
        {
            DrawRadial(blend, r, r.Width / 2, r.Height / 2);
        }
        public void DrawRadial(ColorBlend blend, Rectangle r, Point center)
        {
            DrawRadial(blend, r, center.X, center.Y);
        }
        public void DrawRadial(ColorBlend blend, Rectangle r, int cx, int cy)
        {
            DrawRadialPath.Reset();
            DrawRadialPath.AddEllipse(r.X, r.Y, r.Width - 1, r.Height - 1);

            DrawRadialBrush1 = new PathGradientBrush(DrawRadialPath);
            DrawRadialBrush1.CenterPoint = new Point(r.X + cx, r.Y + cy);
            DrawRadialBrush1.InterpolationColors = blend;

            if (G.SmoothingMode == SmoothingMode.AntiAlias)
            {
                G.FillEllipse(DrawRadialBrush1, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3);
            }
            else {
                G.FillEllipse(DrawRadialBrush1, r);
            }
        }


        protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(c1, c2, DrawRadialRectangle);
        }
        protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height, float angle)
        {
            DrawRadialRectangle = new Rectangle(x, y, width, height);
            DrawRadial(c1, c2, DrawRadialRectangle, angle);
        }

        protected void DrawRadial(Color c1, Color c2, Rectangle r)
        {
            DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, 90f);
            G.FillEllipse(DrawRadialBrush2, r);
        }
        protected void DrawRadial(Color c1, Color c2, Rectangle r, float angle)
        {
            DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, angle);
            G.FillEllipse(DrawRadialBrush2, r);
        }

        #endregion

        #region " CreateRound "

        private GraphicsPath CreateRoundPath;

        private Rectangle CreateRoundRectangle;
        public GraphicsPath CreateRound(int x, int y, int width, int height, int slope)
        {
            CreateRoundRectangle = new Rectangle(x, y, width, height);
            return CreateRound(CreateRoundRectangle, slope);
        }

        public GraphicsPath CreateRound(Rectangle r, int slope)
        {
            CreateRoundPath = new GraphicsPath(FillMode.Winding);
            CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270f, 90f);
            CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0f, 90f);
            CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90f, 90f);
            CreateRoundPath.CloseFigure();
            return CreateRoundPath;
        }

        #endregion

    }

    static class ThemeShare
    {

        #region " Animation "

        private static int Frames;
        private static bool Invalidate;

        public static PrecisionTimer ThemeTimer = new PrecisionTimer();
        //1000 / 50 = 20 FPS
        public static int FPS = 20;

        public static int Rate = 50;
        public delegate void AnimationDelegate(bool invalidate);


        private static List<AnimationDelegate> Callbacks = new List<AnimationDelegate>();
        private static void HandleCallbacks(IntPtr state, bool reserve)
        {
            Invalidate = (Frames >= FPS);
            if (Invalidate)
                Frames = 0;

            lock (Callbacks)
            {
                for (int I = 0; I <= Callbacks.Count - 1; I++)
                {
                    Callbacks[I].Invoke(Invalidate);
                }
            }

            Frames += Rate;
        }

        private static void InvalidateThemeTimer()
        {
            if (Callbacks.Count == 0)
            {
                ThemeTimer.Delete();
            }
            else {
                ThemeTimer.Create(0, Convert.ToUInt32(Rate), HandleCallbacks);
            }
        }

        public static void AddAnimationCallback(AnimationDelegate callback)
        {
            lock (Callbacks)
            {
                if (Callbacks.Contains(callback))
                    return;

                Callbacks.Add(callback);
                InvalidateThemeTimer();
            }
        }

        public static void RemoveAnimationCallback(AnimationDelegate callback)
        {
            lock (Callbacks)
            {
                if (!Callbacks.Contains(callback))
                    return;

                Callbacks.Remove(callback);
                InvalidateThemeTimer();
            }
        }

        #endregion

    }

    enum MouseState : byte
    {
        None = 0,
        Over = 1,
        Down = 2,
        Block = 3
    }

    struct Bloom
    {

        public string _Name;
        public string Name
        {
            get { return _Name; }
        }

        private Color _Value;
        public Color Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public string ValueHex
        {
            get { return string.Concat("#", _Value.R.ToString("X2", null), _Value.G.ToString("X2", null), _Value.B.ToString("X2", null)); }
            set
            {
                try
                {
                    _Value = ColorTranslator.FromHtml(value);
                }
                catch
                {
                    return;
                }
            }
        }


        public Bloom(string name, Color value)
        {
            _Name = name;
            _Value = value;
        }
    }

    //------------------
    //Creator: aeonhack
    //Site: elitevs.net
    //Created: 11/30/2011
    //Changed: 11/30/2011
    //Version: 1.0.0
    //------------------
    class PrecisionTimer : IDisposable
    {

        private bool _Enabled;
        public bool Enabled
        {
            get { return _Enabled; }
        }

        private IntPtr Handle;

        private TimerDelegate TimerCallback;
        [DllImport("kernel32.dll", EntryPoint = "CreateTimerQueueTimer")]
        private static extern bool CreateTimerQueueTimer(ref IntPtr handle, IntPtr queue, TimerDelegate callback, IntPtr state, uint dueTime, uint period, uint flags);

        [DllImport("kernel32.dll", EntryPoint = "DeleteTimerQueueTimer")]
        private static extern bool DeleteTimerQueueTimer(IntPtr queue, IntPtr handle, IntPtr callback);

        public delegate void TimerDelegate(IntPtr r1, bool r2);

        public void Create(uint dueTime, uint period, TimerDelegate callback)
        {
            if (_Enabled)
                return;

            TimerCallback = callback;
            bool Success = CreateTimerQueueTimer(ref Handle, IntPtr.Zero, TimerCallback, IntPtr.Zero, dueTime, period, 0);

            if (!Success)
                ThrowNewException("CreateTimerQueueTimer");
            _Enabled = Success;
        }

        public void Delete()
        {
            if (!_Enabled)
                return;
            bool Success = DeleteTimerQueueTimer(IntPtr.Zero, Handle, IntPtr.Zero);

            if (!Success && !(Marshal.GetLastWin32Error() == 997))
            {
                //ThrowNewException("DeleteTimerQueueTimer")
            }

            _Enabled = !Success;
        }

        private void ThrowNewException(string name)
        {
            throw new Exception(string.Format("{0} failed. Win32Error: {1}", name, Marshal.GetLastWin32Error()));
        }

        public void Dispose()
        {
            Delete();
        }
    }
    #endregion
    class CarbonFiberTheme : ThemeContainer154
    {
        #region "Properties"
        private Icon _Icon;
        public Icon Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }
        private bool _ShowIcon;
        public bool ShowIcon
        {
            get { return _ShowIcon; }
            set
            {
                _ShowIcon = value;
                Invalidate();
            }
        }

        public CarbonFiberTheme()
        {
            BackColor = Color.FromArgb(22, 22, 22);
            TransparencyKey = Color.Fuchsia;
            Font = new Font("Verdana", 8);
            Header = 30;
        }

        protected override void ColorHook()
        {
            // Color hook is just a waste of time haha !!
            //
            //
        }
        #endregion
        #region "Color of Control"
        protected override void PaintHook()
        {
            //This G.Clear does not need ^^
            G.Clear(Color.FromArgb(31, 31, 31));

            ///''''''' Gradient the Body '''''''
            LinearGradientBrush GradientBG = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(25, 25, 25), Color.FromArgb(22, 22, 22), -270);
            G.FillRectangle(GradientBG, new Rectangle(0, 0, Width - 1, Height - 1));

            ///''''''' Draw Body '''''''
            HatchBrush BodyHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
            G.FillRectangle(BodyHatch, new Rectangle(0, 0, Width - 1, Height - 1));
            // G.FillRectangle(New SolidBrush(Color.FromArgb(32, 32, 32)), New Rectangle(10, 10, Width - 21, Height - 21))
            G.DrawRectangle(new Pen(Color.FromArgb(32, 32, 32)), new Rectangle(10, 32, Width - 21, Height - 43));
            G.DrawRectangle(new Pen(Color.FromArgb(6, 6, 6)), new Rectangle(9, 31, Width - 19, Height - 41));


            ///''''''' Draw Header '''''''
            LinearGradientBrush Header = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, 30), Color.FromArgb(25, 25, 25), Color.FromArgb(40, 40, 40), 270);
            G.FillRectangle(Header, new Rectangle(0, 0, Width - 1, 30));
            HatchBrush HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
            G.FillRectangle(HeaderHatch, new Rectangle(0, 0, Width - 1, 30));
            G.FillRectangle(new SolidBrush(Color.FromArgb(15, Color.White)), 0, 0, Width - 1, 15);

            ///''''''' Draw Header Seperator ''''''
            //G.DrawLine(New Pen(Color.FromArgb(18, 18, 18)), 0, 15, Width + 9000, 15) ' Please dont use 9000 above ^^
            G.DrawLine(new Pen(Color.FromArgb(42, 42, 42)), 0, 15, Width - 1, 15);
            // Cuz it has a bug dont worry i will fix it =)

            ///''''''' Draw Header Border '''''''
            //DrawGradient(BlendColor, New Rectangle(0, 0, Width - 1, 32), 0.0F)
            G.FillRectangle(new SolidBrush(Color.FromArgb(22, 22, 22)), new Rectangle(11, 33, Width - 23, Height - 45));
            G.DrawRectangle(Pens.Black, new Rectangle(0, 0, Width - 1, Height - 1));
            G.DrawRectangle(new Pen(Color.FromArgb(49, 49, 49)), new Rectangle(1, 1, Width - 3, Height - 3));

            ///''''''' Reduce Corners '''''''


            ///''''''' Draw Icon and Text '''''''
            if (_ShowIcon == false)
            {
                G.DrawString(Text, Font, new SolidBrush(Color.Black), new Point(8, 7));
                // Text Shadow
                G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(30, 255, 0)), new Point(8, 8));
            }
            else {
                G.DrawIcon(FindForm().Icon, new Rectangle(new Point(9, 7), new Size(16, 16)));
                G.DrawString(Text, Font, new SolidBrush(Color.Black), new Point(28, 7));
                // Text Shadow
                G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(30, 255, 0)), new Point(28, 8));
            }

        }
        #endregion
    }
    class CarbonFiberLabel : ThemeControl154
    {
        #region "Properties"
        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            int textSize = 0;
            int textSize1 = 0;
            textSize = (int)this.CreateGraphics().MeasureString(Text, Font).Width;
            textSize1 = (int)this.CreateGraphics().MeasureString(Text, Font).Height;

            this.Width = 5 + textSize;
            this.Height = textSize1;
        }
        public CarbonFiberLabel()
        {
            Transparent = true;
            BackColor = Color.Transparent;
            this.Size = new Size(50, 16);
            //MinimumSize = New Size(50, 16)
            //MaximumSize = New Size(600, 16)
        }
        protected override void ColorHook()
        {
            // bleh bleh bleh waste of time !!
        }
        #endregion
        #region "Color Of Control"
        protected override void PaintHook()
        {
            G.Clear(BackColor);

            G.DrawString(Text, Font, new SolidBrush(Color.Black), new Point(1, 0));
            // Text Shadow
            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(30, 255, 0)), new Point(1, 1));
        }
        #endregion
    }
    class CarbonFiberButton : ThemeControl154
    {
        #region "Properties"
        public CarbonFiberButton()
        {
            this.Size = new Size(142, 29);
        }
        protected override void ColorHook()
        {
            // blah blah blah waste of time !!
        }
        #endregion
        #region "Color Of Control"

        protected override void PaintHook()
        {
            G.Clear(Color.FromArgb(22, 22, 22));

            LinearGradientBrush Header = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(25, 25, 25), Color.FromArgb(42, 42, 42), 270);
            G.FillRectangle(Header, new Rectangle(0, 0, Width - 1, Height - 1));


            HatchBrush HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
            G.FillRectangle(HeaderHatch, new Rectangle(0, 0, Width - 1, Height - 1));

            switch (State)
            {
                case MouseState.Over:
                    LinearGradientBrush Header1 = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(25, 25, 25), Color.FromArgb(50, 50, 50), 270);
                    G.FillRectangle(Header1, new Rectangle(0, 0, Width - 1, Height - 1));


                    HatchBrush HeaderHatch1 = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
                    G.FillRectangle(HeaderHatch1, new Rectangle(0, 0, Width - 1, Height - 1));

                    break;
                case MouseState.Down:
                    Header = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(25, 25, 25), Color.FromArgb(35, 35, 35), 270);
                    G.FillRectangle(Header, new Rectangle(0, 0, Width - 1, Height - 1));


                    HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
                    G.FillRectangle(HeaderHatch, new Rectangle(0, 0, Width - 1, Height - 1));
                    break;
            }

            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(6, 6, 6)), new Rectangle(-1, -1, Width - 1, Height - 1), new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });
            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(30, 255, 0)), new Rectangle(0, 0, Width - 1, Height - 1), new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            });

            DrawBorders(Pens.Black);
            DrawBorders(new Pen(Color.FromArgb(32, 32, 32)), 1);

            DrawCorners(Color.FromArgb(22, 22, 22), 1);
            DrawCorners(Color.FromArgb(22, 22, 22));
        }
        #endregion
    }
    class CarbonFiberListBox : ListBox
    {
        #region "Properties"
        public CarbonFiberListBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            DoubleBuffered = true;
            DrawMode = DrawMode.OwnerDrawFixed;
            BackColor = Color.FromArgb(22, 22, 22);
            BorderStyle = BorderStyle.None;
            ItemHeight = 15;
        }
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 15)
                CustomPaint();
        }
        // if you dont call this border will not show ^^
        public void CustomPaint()
        {
            CreateGraphics().DrawRectangle(new Pen(Color.FromArgb(6, 6, 6)), new Rectangle(1, 1, Width - 3, Height - 3));
            CreateGraphics().DrawRectangle(new Pen(Color.FromArgb(32, 32, 32)), new Rectangle(0, 0, Width - 1, Height - 1));
        }
        #endregion
        #region "Color of Control"
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Graphics G = e.Graphics;
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.FillRectangle(new SolidBrush(BackColor), new Rectangle(e.Bounds.X, e.Bounds.Y - 1, e.Bounds.Width, e.Bounds.Height + 3));

            if (e.State.ToString().Contains("Selected,"))
            {
                LinearGradientBrush MainBody = new LinearGradientBrush(new Rectangle(e.Bounds.X, e.Bounds.Y + 1, e.Bounds.Width, e.Bounds.Height), Color.FromArgb(25, 25, 25), Color.FromArgb(50, 50, 50), 270);
                G.FillRectangle(MainBody, new Rectangle(e.Bounds.X, e.Bounds.Y + 1, e.Bounds.Width, e.Bounds.Height));
                G.DrawRectangle(new Pen(Color.FromArgb(6, 6, 6)), new Rectangle(e.Bounds.X, e.Bounds.Y + 1, e.Bounds.Width, e.Bounds.Height));
                HatchBrush HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
                G.FillRectangle(HeaderHatch, new Rectangle(e.Bounds.X, e.Bounds.Y + 1, e.Bounds.Width, e.Bounds.Height));
                //G.FillRectangle(New SolidBrush(Color.FromArgb(5, Color.White)), New Rectangle(e.Bounds.X, e.Bounds.Y + 1, e.Bounds.Width, e.Bounds.Height - 8))
            }
            else {
                G.FillRectangle(new SolidBrush(BackColor), e.Bounds);
            }

            try
            {
                // put a space cuz the text will stick into the left
                G.DrawString(" " + Items[e.Index].ToString(), Font, new SolidBrush(Color.FromArgb(100, Color.Black)), e.Bounds.X, e.Bounds.Y);
                G.DrawString(" " + Items[e.Index].ToString(), Font, new SolidBrush(Color.FromArgb(30, 255, 0)), e.Bounds.X, e.Bounds.Y + 1);
            }
            catch
            {
            }
            G.DrawRectangle(new Pen(Color.FromArgb(6, 6, 6)), new Rectangle(1, 1, Width - 3, Height - 3));
            G.DrawRectangle(new Pen(Color.FromArgb(32, 32, 32)), new Rectangle(0, 0, Width - 1, Height - 1));
            base.OnDrawItem(e);
        }
        #endregion
    }
    class CarbonFiberGroupBox : ThemeContainer154
    {
        #region "Properties"
        public CarbonFiberGroupBox()
        {
            ControlMode = true;
            TransparencyKey = Color.Fuchsia;
            Font = new Font("Verdana", 8);
            this.Size = new Size(172, 105);
        }

        protected override void ColorHook()
        {
            // another waste of time HAHA !!
        }
        #endregion
        #region "Color of Control"

        protected override void PaintHook()
        {
            G.Clear(Color.FromArgb(22, 22, 22));

            ///''''''' Draw Header '''''''

            G.DrawRectangle(new Pen(Color.FromArgb(32, 32, 32)), new Rectangle(1, 1, Width - 3, Height - 3));

            LinearGradientBrush Header = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, 26), Color.FromArgb(25, 25, 25), Color.FromArgb(40, 40, 40), 270);
            G.FillRectangle(Header, new Rectangle(0, 0, Width - 1, 26));

            HatchBrush HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
            G.FillRectangle(HeaderHatch, new Rectangle(0, 0, Width - 1, 26));
            G.FillRectangle(new SolidBrush(Color.FromArgb(13, Color.White)), 0, 0, Width - 1, 13);

            G.DrawLine(new Pen(Color.FromArgb(42, 42, 42)), 0, 13, Width - 1, 13);
            // Cuz it has a bug dont worry i will fix it =)

            G.DrawRectangle(new Pen(Color.FromArgb(6, 6, 6)), new Rectangle(0, 0, Width - 1, Height - 1));
            // Draw Border
            //G.DrawRectangle(New Pen(Color.FromArgb(6, 6, 6)), New Rectangle(0, 0, Width - 1, 27))
            //G.DrawRectangle(New Pen(Color.FromArgb(32, 32, 32)), New Rectangle(0, 0, Width - 1, Height - 1))


            G.DrawRectangle(new Pen(Color.FromArgb(6, 6, 6)), new Rectangle(1, 1, Width - 3, 25));
            G.DrawRectangle(new Pen(Color.FromArgb(32, 32, 32)), new Rectangle(1, 1, Width - 3, 24));

            ///''''''' Draw Text and Shadw '''''''
            //G.DrawString(Text, Font, New SolidBrush(Color.Black), New Point(9, 7)) ' Text Shadow
            //G.DrawString(Text, Font, New SolidBrush(Color.FromArgb(30, 255, 0)), New Point(8, 6))

            DrawText(new SolidBrush(Color.Black), HorizontalAlignment.Center, 1, 1);
            DrawText(new SolidBrush(Color.FromArgb(30, 255, 0)), HorizontalAlignment.Center, 2, 2);

            //DrawCorners(Color.FromArgb(22, 22, 22), 1)
            //DrawCorners(Color.FromArgb(22, 22, 22))
        }
        #endregion

    }
    [DefaultEvent("CheckedChanged")]
    #region "Properties"
    class CarbonFiberCheckbox : ThemeControl154
    {
        private bool _Checked;

        private int X;
        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender);

        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                Invalidate();
                if (CheckedChanged != null)
                {
                    CheckedChanged(this);
                }
            }
        }

        protected override void ColorHook()
        {
            // again another waste of time >.<
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            int textSize = 0;
            textSize = (int)this.CreateGraphics().MeasureString(Text, Font).Width;
            this.Width = 20 + textSize;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            X = e.X;
            Invalidate();
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (_Checked == true)
                _Checked = false;
            else
                _Checked = true;
        }
        #endregion
        #region "Color of Control"
        protected override void PaintHook()
        {
            G.Clear(BackColor);
            G.SmoothingMode = SmoothingMode.HighQuality;
            G.DrawRectangle(new Pen(Color.FromArgb(29, 29, 29)), 1, 1, 14, 13);

            if (State == MouseState.Over)
            {
                G.DrawString("a", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(13, Color.White)), new Point(-2, 0));
            }

            if (_Checked)
            {
                HatchBrush HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(50, Color.Black), Color.Transparent);
                G.FillRectangle(new SolidBrush(Color.FromArgb(20, Color.White)), 2, 2, 12, 6);
                //Gloss
                G.FillRectangle(HeaderHatch, new Rectangle(2, 2, 12, 12));
                G.DrawString("a", new Font("Marlett", 12), new SolidBrush(Color.FromArgb(30, 255, 0)), new Point(-2, 0));
            }
            else {
                // Do Nothing ^^
            }

            G.DrawRectangle(new Pen(Color.FromArgb(6, 6, 6)), 0, 0, 16, 15);
            G.DrawRectangle(new Pen(Color.FromArgb(6, 6, 6)), 2, 2, 12, 11);
            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(0, 0, 0)), 17, 0);
            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(30, 255, 0)), 18, 1);
        }

        public CarbonFiberCheckbox()
        {
            this.Size = new Size(50, 16);
            MinimumSize = new Size(50, 16);
            MaximumSize = new Size(600, 16);
            BackColor = Color.Transparent;
        }
        #endregion
    }
    class CarbonFiberCustomBox : ThemeContainer154
    {
        #region "Properties"
        public CarbonFiberCustomBox()
        {
            ControlMode = true;
            Size = new Size(150, 100);
            BackColor = Color.FromArgb(22, 22, 22);
        }



        protected override void ColorHook()
        {
        }
        #endregion
        #region "Color of Control"
        protected override void PaintHook()
        {
            G.Clear(BackColor);
            G.FillRectangle(new SolidBrush(Color.FromArgb(22, 22, 22)), ClientRectangle);
            DrawBorders(new Pen(Color.FromArgb(6, 6, 6)), 1);
            DrawBorders(new Pen(Color.FromArgb(32, 32, 32)));
        }
        #endregion

    }
    class CarbonFiberTabControl : TabControl
    {
        #region "Properties"
        public CarbonFiberTabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            DoubleBuffered = true;

        }
        protected override void CreateHandle()
        {
            base.CreateHandle();
            Alignment = TabAlignment.Top;
        }
        // BackColor
        Color C1 = Color.FromArgb(22, 22, 22);
        // ' OUter Black
        Color C2 = Color.FromArgb(6, 6, 6);
        // ' Inner Border
        Color C3 = Color.FromArgb(32, 32, 32);
        #endregion
        #region "Color Of Control"
        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap B = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(B);
            try
            {
                SelectedTab.BackColor = C1;
            }
            catch
            {
            }
            G.Clear(Parent.BackColor);
            for (int i = 0; i <= TabCount - 1; i++)
            {
                if (!(i == SelectedIndex))
                {
                    Rectangle x2 = new Rectangle(GetTabRect(i).X - 1, GetTabRect(i).Y + 1, GetTabRect(i).Width + 2, GetTabRect(i).Height);
                    LinearGradientBrush G1 = new LinearGradientBrush(new Point(x2.X, x2.Y), new Point(x2.X, x2.Y + x2.Height), Color.FromArgb(22, 22, 22), Color.FromArgb(22, 22, 22));
                    G.FillRectangle(G1, x2);
                    G1.Dispose();
                    G.DrawRectangle(new Pen(C3), x2);
                    G.DrawRectangle(new Pen(C2), new Rectangle(x2.X + 1, x2.Y + 1, x2.Width - 2, x2.Height));
                    G.DrawString(TabPages[i].Text, Font, new SolidBrush(Color.FromArgb(250, 150, 0)), x2, new StringFormat
                    {
                        LineAlignment = StringAlignment.Center,
                        Alignment = StringAlignment.Center
                    });
                    //
                }
            }

            G.FillRectangle(new SolidBrush(C1), 0, ItemSize.Height, Width, Height);
            G.DrawRectangle(new Pen(C2), 0, ItemSize.Height, Width - 1, Height - ItemSize.Height - 1);
            G.DrawRectangle(new Pen(C3), 1, ItemSize.Height + 1, Width - 3, Height - ItemSize.Height - 3);
            if (!(SelectedIndex == -1))
            {
                Rectangle x1 = new Rectangle(GetTabRect(SelectedIndex).X - 2, GetTabRect(SelectedIndex).Y, GetTabRect(SelectedIndex).Width + 3, GetTabRect(SelectedIndex).Height);
                G.FillRectangle(new SolidBrush(C1), new Rectangle(x1.X + 2, x1.Y + 2, x1.Width - 2, x1.Height));
                G.DrawLine(new Pen(C2), new Point(x1.X, x1.Y + x1.Height - 2), new Point(x1.X, x1.Y));
                G.DrawLine(new Pen(C2), new Point(x1.X, x1.Y), new Point(x1.X + x1.Width, x1.Y));
                G.DrawLine(new Pen(C2), new Point(x1.X + x1.Width, x1.Y), new Point(x1.X + x1.Width, x1.Y + x1.Height - 2));

                G.DrawLine(new Pen(C3), new Point(x1.X + 1, x1.Y + x1.Height - 1), new Point(x1.X + 1, x1.Y + 1));
                G.DrawLine(new Pen(C3), new Point(x1.X + 1, x1.Y + 1), new Point(x1.X + x1.Width - 1, x1.Y + 1));
                G.DrawLine(new Pen(C3), new Point(x1.X + x1.Width - 1, x1.Y + 1), new Point(x1.X + x1.Width - 1, x1.Y + x1.Height - 1));

                G.DrawString(TabPages[SelectedIndex].Text, Font, new SolidBrush(Color.FromArgb(30, 255, 0)), x1, new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                });
            }

            e.Graphics.DrawImage((Bitmap)B.Clone(), 0, 0);
            G.Dispose();
            B.Dispose();

        }
        #endregion
    }
    [DefaultEvent("CheckedChanged")]
    #region "Properties"
    class CarbonFiberRadioButton : ThemeControl154
    {
        private int X;

        private bool _Checked;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                _Checked = value;
                InvalidateControls();
                if (CheckedChanged != null)
                {
                    CheckedChanged(this);
                }
                Invalidate();
            }
        }

        public event CheckedChangedEventHandler CheckedChanged;
        public delegate void CheckedChangedEventHandler(object sender);

        protected override void OnCreation()
        {
            InvalidateControls();
        }

        private void InvalidateControls()
        {
            if (!IsHandleCreated || !_Checked)
                return;

            foreach (Control C in Parent.Controls)
            {
                if (!object.ReferenceEquals(C, this) && C is CarbonFiberRadioButton)
                {
                    ((CarbonFiberRadioButton)C).Checked = false;
                }
            }
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!_Checked)
                Checked = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            X = e.X;
            Invalidate();
        }


        protected override void ColorHook()
        {
            // again and again another waste of time >.<
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            base.OnTextChanged(e);
            int textSize = 0;
            textSize = (int)CreateGraphics().MeasureString(Text, Font).Width;
            this.Width = 20 + textSize;
        }
        #endregion
        #region "Color Of Control"
        protected override void PaintHook()
        {
            G.Clear(Color.FromArgb(22, 22, 22));
            G.SmoothingMode = SmoothingMode.HighQuality;

            if (State == MouseState.Over)
            {
                G.FillEllipse(new SolidBrush(Color.FromArgb(29, 29, 29)), new Rectangle(3, 3, 10, 10));
                G.DrawEllipse(new Pen(Color.FromArgb(22, 22, 22)), 5, 5, 6, 6);
            }

            if (_Checked)
            {
                G.FillEllipse(new SolidBrush(Color.FromArgb(30, 255, 0)), 5, 5, 6, 6);
            }
            else {
            }

            G.DrawEllipse(new Pen(Color.FromArgb(6, 6, 6)), 0, 0, 16, 16);
            G.DrawEllipse(new Pen(Color.FromArgb(29, 29, 29)), 1, 1, 14, 14);
            G.DrawEllipse(new Pen(Color.FromArgb(6, 6, 6)), new Rectangle(2, 2, 12, 12));

            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(0, 0, 0)), 17, 0);
            G.DrawString(Text, Font, new SolidBrush(Color.FromArgb(30, 255, 0)), 18, 1);
        }

        public CarbonFiberRadioButton()
        {
            this.Size = new Size(50, 17);
            MinimumSize = new Size(50, 17);
            MaximumSize = new Size(600, 17);
        }
        #endregion
    }
    class CarbonFiberControlButton : ThemeControl154
    {
        #region "Properties"
        public CarbonFiberControlButton()
        {
            this.Size = new Size(26, 20);
            this.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }
        private bool _StateMinimize = false;
        public bool StateMinimize
        {
            get { return _StateMinimize; }
            set
            {
                _StateMinimize = value;
                Invalidate();
            }
        }

        private bool _StateClose = false;
        public bool StateClose
        {
            get { return _StateClose; }
            set
            {
                _StateClose = value;
                Invalidate();
            }
        }

        private bool _StateMaximize = false;
        public bool StateMaximize
        {
            get { return _StateMaximize; }
            set
            {
                _StateMaximize = value;
                Invalidate();
            }
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            this.Size = new Size(26, 20);
        }
        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (_StateMinimize == true)
            {
                FindForm().WindowState = FormWindowState.Minimized;
                // true
                // Else
                _StateClose = false;
                // false
                _StateMaximize = false;
            }
            if (_StateClose == true)
            {
                FindForm().Close();
                //Else
                _StateMinimize = false;
                _StateMaximize = false;
            }
            if (_StateMaximize == true)
            {
                if (FindForm().WindowState != FormWindowState.Maximized)
                    FindForm().WindowState = FormWindowState.Maximized;
                else
                    FindForm().WindowState = FormWindowState.Normal;

                _StateClose = false;
                // false
                _StateMinimize = false;
            }
        }


        protected override void ColorHook()
        {
        }
        #endregion
        #region "Color Of Control"
        protected override void PaintHook()
        {
            G.Clear(Color.FromArgb(22, 22, 22));
            G.SmoothingMode = SmoothingMode.HighQuality;

            LinearGradientBrush Header = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(22, 22, 22), Color.FromArgb(35, 35, 35), 270);
            G.FillRectangle(Header, new Rectangle(0, 0, Width - 1, Height - 1));

            HatchBrush HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
            G.FillRectangle(HeaderHatch, new Rectangle(0, 0, Width - 1, Height - 1));

            G.FillRectangle(new SolidBrush(Color.FromArgb(8, Color.White)), 0, 0, Width - 1, 10);
            G.DrawLine(new Pen(Color.FromArgb(33, 33, 33)), 0, 9, Width - 1, 10);
            // Cuz it has a bug dont worry i will fix it =)

            switch (State)
            {
                case MouseState.Over:
                    Header = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(25, 25, 25), Color.FromArgb(40, 40, 40), 270);
                    G.FillRectangle(Header, new Rectangle(0, 0, Width - 1, Height - 1));
                    HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
                    G.FillRectangle(HeaderHatch, new Rectangle(0, 0, Width - 1, Height - 1));
                    G.FillRectangle(new SolidBrush(Color.FromArgb(10, Color.White)), 0, 0, Width - 1, 10);
                    G.DrawLine(new Pen(Color.FromArgb(38, 38, 38)), 0, 9, Width - 1, 10);
                    // Cuz it has a bug dont worry i will fix it =)
                    break;
                case MouseState.Down:
                    Header = new LinearGradientBrush(new Rectangle(0, 0, Width - 1, Height - 1), Color.FromArgb(25, 25, 25), Color.FromArgb(35, 35, 35), 270);
                    G.FillRectangle(Header, new Rectangle(0, 0, Width - 1, Height - 1));
                    HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
                    G.FillRectangle(HeaderHatch, new Rectangle(0, 0, Width - 1, Height - 1));
                    G.FillRectangle(new SolidBrush(Color.FromArgb(8, Color.White)), 0, 0, Width - 1, 10);
                    G.DrawLine(new Pen(Color.FromArgb(35, 35, 35)), 0, 9, Width - 1, 10);
                    // Cuz it has a bug dont worry i will fix it =)
                    break;

            }
            //Draw Text


            if (_StateMinimize == true)
            {
                G.DrawString("0", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(30, 255, 0)), new Point(6, 4));
                _StateClose = false;
                // false
                _StateMaximize = false;
            }
            if (_StateClose == true)
            {
                G.DrawString("r", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(30, 255, 0)), new Point(6, 4));
                _StateMinimize = false;
                _StateMaximize = false;
            }

            if (_StateMaximize == true)
            {
                if (FindForm().WindowState != FormWindowState.Maximized)
                    G.DrawString("1", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(30, 255, 0)), new Point(6, 4));
                else
                    G.DrawString("2", new Font("Marlett", 8), new SolidBrush(Color.FromArgb(30, 255, 0)), new Point(6, 4));
                _StateClose = false;
                // false
                _StateMinimize = false;
            }


            //Draw Gloss
            //Draw Border
            DrawBorders(Pens.Black);
            // DrawBorders(New Pen(Color.FromArgb(32, 32, 32)))
        }
        #endregion
    }
    class CarbonFiberSeparatorVertical : ThemeControl154
    {
        #region "Properties"
        public CarbonFiberSeparatorVertical()
        {
            LockWidth = 10;
        }

        protected override void ColorHook()
        {

        }

        protected override void PaintHook()
        {
            G.Clear(Color.FromArgb(22, 22, 22));

            G.FillRectangle(new SolidBrush(Color.FromArgb(6, 6, 6)), new Rectangle(4, 0, 1, Height - 1));
            G.FillRectangle(new SolidBrush(Color.FromArgb(32, 32, 32)), new Rectangle(5, 0, 1, Height - 1));
        }
        #endregion
    }
    class CarbonFiberSeparatorHorizontal : ThemeControl154
    {
        #region "Properties"
        public CarbonFiberSeparatorHorizontal()
        {
            LockHeight = 10;
        }

        protected override void ColorHook()
        {

        }

        protected override void PaintHook()
        {
            G.Clear(Color.FromArgb(22, 22, 22));
            G.DrawLine(new Pen(Color.FromArgb(6, 6, 6)), 0, 4, Width - 1, 4);
            G.DrawLine(new Pen(Color.FromArgb(32, 32, 32)), 0, 5, Width - 1, 5);
        }
        #endregion
    }
    //------------------
    //ProgressBar Component By: Aeonhack
    //TextBox Component By: Mavamaarten
    //------------------
    //Credits by Aeonhack and Mavamaarten
    [DefaultEvent("TextChanged")]
    class CarbonFiberTextBox : ThemeControl154
    {
        #region "Properties"
        private HorizontalAlignment _TextAlign = HorizontalAlignment.Left;
        public HorizontalAlignment TextAlign
        {
            get { return _TextAlign; }
            set
            {
                _TextAlign = value;
                if (Base != null)
                {
                    Base.TextAlign = value;
                }
            }
        }
        private int _MaxLength = 32767;
        public int MaxLength
        {
            get { return _MaxLength; }
            set
            {
                _MaxLength = value;
                if (Base != null)
                {
                    Base.MaxLength = value;
                }
            }
        }
        private bool _ReadOnly;
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set
            {
                _ReadOnly = value;
                if (Base != null)
                {
                    Base.ReadOnly = value;
                }
            }
        }
        private bool _UseSystemPasswordChar;
        public bool UseSystemPasswordChar
        {
            get { return _UseSystemPasswordChar; }
            set
            {
                _UseSystemPasswordChar = value;
                if (Base != null)
                {
                    Base.UseSystemPasswordChar = value;
                }
            }
        }
        private bool _Multiline;
        public bool Multiline
        {
            get { return _Multiline; }
            set
            {
                _Multiline = value;
                if (Base != null)
                {
                    Base.Multiline = value;

                    if (value)
                    {
                        LockHeight = 0;
                        Base.Height = Height - 11;
                    }
                    else {
                        LockHeight = Base.Height + 11;
                    }
                }
            }
        }
        public override string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                if (Base != null)
                {
                    Base.Text = value;
                }
            }
        }
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                if (Base != null)
                {
                    Base.Font = value;
                    Base.Location = new Point(3, 5);
                    Base.Width = Width - 6;

                    if (!_Multiline)
                    {
                        LockHeight = Base.Height + 11;
                    }
                }
            }
        }

        protected override void OnCreation()
        {
            if (!Controls.Contains(Base))
            {
                Controls.Add(Base);
            }
        }

        private TextBox Base;
        public CarbonFiberTextBox()
        {
            Base = new TextBox();

            Base.Font = Font;
            Base.Text = Text;
            Base.MaxLength = _MaxLength;
            Base.Multiline = _Multiline;
            Base.ReadOnly = _ReadOnly;
            Base.UseSystemPasswordChar = _UseSystemPasswordChar;

            Base.BorderStyle = BorderStyle.None;

            Base.Location = new Point(5, 5);
            Base.Width = Width - 10;

            Base.BackColor = Color.FromArgb(22, 22, 22);
            Base.ForeColor = Color.FromArgb(30, 255, 0);
            if (_Multiline)
            {
                Base.Height = Height - 11;
            }
            else {
                LockHeight = Base.Height + 11;
            }

            Base.TextChanged += OnBaseTextChanged;
            Base.KeyDown += OnBaseKeyDown;
        }

        #endregion
        #region "Color of Control"

        protected override void ColorHook()
        {
        }

        protected override void PaintHook()
        {
            G.Clear(Color.FromArgb(22, 22, 22));

            DrawBorders(new Pen(Color.FromArgb(6, 6, 6)));
            DrawBorders(new Pen(Color.FromArgb(32, 32, 32)), 1);

        }
        private void OnBaseTextChanged(object s, EventArgs e)
        {
            Text = Base.Text;
        }
        private void OnBaseKeyDown(object s, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                Base.SelectAll();
                e.SuppressKeyPress = true;
            }
        }
        protected override void OnResize(EventArgs e)
        {
            Base.Location = new Point(5, 5);
            Base.Width = Width - 10;

            if (_Multiline)
            {
                Base.Height = Height - 11;
            }


            base.OnResize(e);
        }
        #endregion
    }
    class CarbonFiberProgressBar : Control
    {
        #region " Properties "
        public CarbonFiberProgressBar()
        {
            Size = new Size(419, 27);
        }
        private double _Maximum;
        public double Maximum
        {
            get { return _Maximum; }
            set
            {
                _Maximum = value;
                Progress = _Current / value * 100;
                Invalidate();
            }
        }


        private double _Current;
        public double Current
        {
            get { return _Current; }
            set
            {
                _Current = value;
                Progress = value / _Maximum * 100;
                Invalidate();
            }
        }
        private double _Progress;
        public double Progress
        {
            get { return _Progress; }
            set
            {
                if (value < 0)
                    value = 0;
                else
                    if (value > 100)
                    value = 100;
                _Progress = value;
                _Current = value * 0.01 * _Maximum;
                Invalidate();
            }
        }

        private bool _ShowPercentage = true;
        public bool ShowPercentage
        {
            get { return _ShowPercentage; }
            set
            {
                _ShowPercentage = value;
                Invalidate();
            }
        }
        #endregion
        #region "Color Of Control"
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            using (Bitmap B = new Bitmap(Width, Height))
            {
                Graphics G = Graphics.FromImage(B);
                
                    G.Clear(Color.FromArgb(22, 22, 22));
                    LinearGradientBrush Glow = new LinearGradientBrush(new Rectangle(3, 3, Width - 7, Height - 7), Color.FromArgb(22, 22, 22), Color.FromArgb(27, 27, 27), -270);
                    G.FillRectangle(Glow, new Rectangle(3, 3, Width - 7, Height - 7));
                    G.DrawRectangle(Pens.Black, new Rectangle(3, 3, Width - 7, Height - 7));



                    dynamic W = Convert.ToInt32(_Progress * 0.01 * Width);

                    Rectangle R = new Rectangle(3, 3, W - 6, Height - 6);

                    LinearGradientBrush Header = new LinearGradientBrush(R, Color.FromArgb(25, 25, 25), Color.FromArgb(50, 50, 50), 270);
                    G.FillRectangle(Header, R);
                    HatchBrush HeaderHatch = new HatchBrush(HatchStyle.Trellis, Color.FromArgb(35, Color.Black), Color.Transparent);
                    G.FillRectangle(HeaderHatch, R);

                    if (_ShowPercentage)
                    {
                        G.DrawString(Convert.ToString(string.Concat(Progress, "%")), Font, new SolidBrush(Color.FromArgb(6, 6, 6)), new Rectangle(1, 2, Width - 1, Height - 1), new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                        G.DrawString(Convert.ToString(string.Concat(Progress, "%")), Font, new SolidBrush(Color.FromArgb(30, 255, 0)), new Rectangle(0, 1, Width - 1, Height - 1), new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                    

                    G.FillRectangle(new SolidBrush(Color.FromArgb(3, Color.White)), R.X, R.Y, R.Width, Convert.ToInt32(R.Height * 0.45));
                    G.DrawRectangle(new Pen(Color.FromArgb(32, 32, 32)), new Rectangle(4, 4, Width - 9, Height - 9));
                    G.DrawRectangle(new Pen(Color.FromArgb(10, 10, 10)), R.X, R.X, R.Width - 1, R.Height - 1);
                    e.Graphics.DrawImage(B, 0, 0);
                }
            }
            base.OnPaint(e);
        }
        #endregion
    }








    //------------------
    //Creator: aeonhack
    //Site: elitevs.net
    //Created: 08/02/2011
    //Changed: 12/06/2011
    //Version: 1.5.4
    //------------------

    //abstract class ThemeContainer154 : ContainerControl
    //{

    //    #region " Initialization "

    //    protected Graphics G;

    //    protected Bitmap B;
    //    public ThemeContainer154()
    //    {
    //        SetStyle((ControlStyles)139270, true);

    //        _ImageSize = Size.Empty;
    //        Font = new Font("Verdana", 8);

    //        MeasureBitmap = new Bitmap(1, 1);
    //        MeasureGraphics = Graphics.FromImage(MeasureBitmap);

    //        DrawRadialPath = new GraphicsPath();

    //        InvalidateCustimization();
    //    }

    //    protected override sealed void OnHandleCreated(EventArgs e)
    //    {
    //        if (DoneCreation)
    //            InitializeMessages();

    //        InvalidateCustimization();
    //        ColorHook();

    //        if (!(_LockWidth == 0))
    //            Width = _LockWidth;
    //        if (!(_LockHeight == 0))
    //            Height = _LockHeight;
    //        if (!_ControlMode)
    //            base.Dock = DockStyle.Fill;

    //        Transparent = _Transparent;
    //        if (_Transparent && _BackColor)
    //            BackColor = Color.Transparent;

    //        base.OnHandleCreated(e);
    //    }

    //    private bool DoneCreation;
    //    protected override sealed void OnParentChanged(EventArgs e)
    //    {
    //        base.OnParentChanged(e);

    //        if (Parent == null)
    //            return;
    //        _IsParentForm = Parent is Form;

    //        if (!_ControlMode)
    //        {
    //            InitializeMessages();

    //            if (_IsParentForm)
    //            {
    //                ParentForm.FormBorderStyle = _BorderStyle;
    //                ParentForm.TransparencyKey = _TransparencyKey;

    //                if (!DesignMode)
    //                {
    //                    ParentForm.Shown += FormShown;
    //                }
    //            }

    //            Parent.BackColor = BackColor;
    //        }

    //        OnCreation();
    //        DoneCreation = true;
    //        InvalidateTimer();
    //    }

    //    #endregion

    //    private void DoAnimation(bool i)
    //    {
    //        OnAnimation();
    //        if (i)
    //            Invalidate();
    //    }

    //    protected override sealed void OnPaint(PaintEventArgs e)
    //    {
    //        if (Width == 0 || Height == 0)
    //            return;

    //        if (_Transparent && _ControlMode)
    //        {
    //            PaintHook();
    //            e.Graphics.DrawImage(B, 0, 0);
    //        }
    //        else {
    //            G = e.Graphics;
    //            PaintHook();
    //        }
    //    }

    //    protected override void OnHandleDestroyed(EventArgs e)
    //    {
    //        ThemeShare.RemoveAnimationCallback(DoAnimation);
    //        base.OnHandleDestroyed(e);
    //    }

    //    private bool HasShown;
    //    private void FormShown(object sender, EventArgs e)
    //    {
    //        if (_ControlMode || HasShown)
    //            return;

    //        if (_StartPosition == FormStartPosition.CenterParent || _StartPosition == FormStartPosition.CenterScreen)
    //        {
    //            Rectangle SB = Screen.PrimaryScreen.Bounds;
    //            Rectangle CB = ParentForm.Bounds;
    //            ParentForm.Location = new Point(SB.Width / 2 - CB.Width / 2, SB.Height / 2 - CB.Width / 2);
    //        }

    //        HasShown = true;
    //    }


    //    #region " Size Handling "

    //    private Rectangle Frame;
    //    protected override sealed void OnSizeChanged(EventArgs e)
    //    {
    //        if (_Movable && !_ControlMode)
    //        {
    //            Frame = new Rectangle(7, 7, Width - 14, _Header - 7);
    //        }

    //        InvalidateBitmap();
    //        Invalidate();

    //        base.OnSizeChanged(e);
    //    }

    //    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    //    {
    //        if (!(_LockWidth == 0))
    //            width = _LockWidth;
    //        if (!(_LockHeight == 0))
    //            height = _LockHeight;
    //        base.SetBoundsCore(x, y, width, height, specified);
    //    }

    //    #endregion

    //    #region " State Handling "

    //    protected MouseState State;
    //    private void SetState(MouseState current)
    //    {
    //        State = current;
    //        Invalidate();
    //    }

    //    protected override void OnMouseMove(MouseEventArgs e)
    //    {
    //        if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized))
    //        {
    //            if (_Sizable && !_ControlMode)
    //                InvalidateMouse();
    //        }

    //        base.OnMouseMove(e);
    //    }

    //    protected override void OnEnabledChanged(EventArgs e)
    //    {
    //        if (Enabled)
    //            SetState(MouseState.None);
    //        else
    //            SetState(MouseState.Block);
    //        base.OnEnabledChanged(e);
    //    }

    //    protected override void OnMouseEnter(EventArgs e)
    //    {
    //        SetState(MouseState.Over);
    //        base.OnMouseEnter(e);
    //    }

    //    protected override void OnMouseUp(MouseEventArgs e)
    //    {
    //        SetState(MouseState.Over);
    //        base.OnMouseUp(e);
    //    }

    //    protected override void OnMouseLeave(EventArgs e)
    //    {
    //        SetState(MouseState.None);

    //        if (GetChildAtPoint(PointToClient(MousePosition)) != null)
    //        {
    //            if (_Sizable && !_ControlMode)
    //            {
    //                Cursor = Cursors.Default;
    //                Previous = 0;
    //            }
    //        }

    //        base.OnMouseLeave(e);
    //    }

    //    protected override void OnMouseDown(MouseEventArgs e)
    //    {
    //        if (e.Button == MouseButtons.Left)
    //            SetState(MouseState.Down);

    //        if (!(_IsParentForm && ParentForm.WindowState == FormWindowState.Maximized || _ControlMode))
    //        {
    //            if (_Movable && Frame.Contains(e.Location))
    //            {
    //                Capture = false;
    //                WM_LMBUTTONDOWN = true;
    //                DefWndProc(ref Messages[0]);
    //            }
    //            else if (_Sizable && !(Previous == 0))
    //            {
    //                Capture = false;
    //                WM_LMBUTTONDOWN = true;
    //                DefWndProc(ref Messages[Previous]);
    //            }
    //        }

    //        base.OnMouseDown(e);
    //    }

    //    private bool WM_LMBUTTONDOWN;
    //    protected override void WndProc(ref Message m)
    //    {
    //        base.WndProc(ref m);

    //        if (WM_LMBUTTONDOWN && m.Msg == 513)
    //        {
    //            WM_LMBUTTONDOWN = false;

    //            SetState(MouseState.Over);
    //            if (!_SmartBounds)
    //                return;

    //            if (IsParentMdi)
    //            {
    //                CorrectBounds(new Rectangle(Point.Empty, Parent.Parent.Size));
    //            }
    //            else {
    //                CorrectBounds(Screen.FromControl(Parent).WorkingArea);
    //            }
    //        }
    //    }

    //    private Point GetIndexPoint;
    //    private bool B1;
    //    private bool B2;
    //    private bool B3;
    //    private bool B4;
    //    private int GetIndex()
    //    {
    //        GetIndexPoint = PointToClient(MousePosition);
    //        B1 = GetIndexPoint.X < 7;
    //        B2 = GetIndexPoint.X > Width - 7;
    //        B3 = GetIndexPoint.Y < 7;
    //        B4 = GetIndexPoint.Y > Height - 7;

    //        if (B1 && B3)
    //            return 4;
    //        if (B1 && B4)
    //            return 7;
    //        if (B2 && B3)
    //            return 5;
    //        if (B2 && B4)
    //            return 8;
    //        if (B1)
    //            return 1;
    //        if (B2)
    //            return 2;
    //        if (B3)
    //            return 3;
    //        if (B4)
    //            return 6;
    //        return 0;
    //    }

    //    private int Current;
    //    private int Previous;
    //    private void InvalidateMouse()
    //    {
    //        Current = GetIndex();
    //        if (Current == Previous)
    //            return;

    //        Previous = Current;
    //        switch (Previous)
    //        {
    //            case 0:
    //                Cursor = Cursors.Default;
    //                break;
    //            case 1:
    //            case 2:
    //                Cursor = Cursors.SizeWE;
    //                break;
    //            case 3:
    //            case 6:
    //                Cursor = Cursors.SizeNS;
    //                break;
    //            case 4:
    //            case 8:
    //                Cursor = Cursors.SizeNWSE;
    //                break;
    //            case 5:
    //            case 7:
    //                Cursor = Cursors.SizeNESW;
    //                break;
    //        }
    //    }

    //    private Message[] Messages = new Message[9];
    //    private void InitializeMessages()
    //    {
    //        Messages[0] = Message.Create(Parent.Handle, 161, new IntPtr(2), IntPtr.Zero);
    //        for (int I = 1; I <= 8; I++)
    //        {
    //            Messages[I] = Message.Create(Parent.Handle, 161, new IntPtr(I + 9), IntPtr.Zero);
    //        }
    //    }

    //    private void CorrectBounds(Rectangle bounds)
    //    {
    //        if (Parent.Width > bounds.Width)
    //            Parent.Width = bounds.Width;
    //        if (Parent.Height > bounds.Height)
    //            Parent.Height = bounds.Height;

    //        int X = Parent.Location.X;
    //        int Y = Parent.Location.Y;

    //        if (X < bounds.X)
    //            X = bounds.X;
    //        if (Y < bounds.Y)
    //            Y = bounds.Y;

    //        int Width = bounds.X + bounds.Width;
    //        int Height = bounds.Y + bounds.Height;

    //        if (X + Parent.Width > Width)
    //            X = Width - Parent.Width;
    //        if (Y + Parent.Height > Height)
    //            Y = Height - Parent.Height;

    //        Parent.Location = new Point(X, Y);
    //    }

    //    #endregion


    //    #region " Base Properties "

    //    public override DockStyle Dock
    //    {
    //        get { return base.Dock; }
    //        set
    //        {
    //            if (!_ControlMode)
    //                return;
    //            base.Dock = value;
    //        }
    //    }

    //    private bool _BackColor;
    //    [Category("Misc")]
    //    public override Color BackColor
    //    {
    //        get { return base.BackColor; }
    //        set
    //        {
    //            if (value == base.BackColor)
    //                return;

    //            if (!IsHandleCreated && _ControlMode && value == Color.Transparent)
    //            {
    //                _BackColor = true;
    //                return;
    //            }

    //            base.BackColor = value;
    //            if (Parent != null)
    //            {
    //                if (!_ControlMode)
    //                    Parent.BackColor = value;
    //                ColorHook();
    //            }
    //        }
    //    }

    //    public override Size MinimumSize
    //    {
    //        get { return base.MinimumSize; }
    //        set
    //        {
    //            base.MinimumSize = value;
    //            if (Parent != null)
    //                Parent.MinimumSize = value;
    //        }
    //    }

    //    public override Size MaximumSize
    //    {
    //        get { return base.MaximumSize; }
    //        set
    //        {
    //            base.MaximumSize = value;
    //            if (Parent != null)
    //                Parent.MaximumSize = value;
    //        }
    //    }

    //    public override string Text
    //    {
    //        get { return base.Text; }
    //        set
    //        {
    //            base.Text = value;
    //            Invalidate();
    //        }
    //    }

    //    public override Font Font
    //    {
    //        get { return base.Font; }
    //        set
    //        {
    //            base.Font = value;
    //            Invalidate();
    //        }
    //    }

    //    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //    public override Color ForeColor
    //    {
    //        get { return Color.Empty; }
    //        set { }
    //    }
    //    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //    public override Image BackgroundImage
    //    {
    //        get { return null; }
    //        set { }
    //    }
    //    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //    public override ImageLayout BackgroundImageLayout
    //    {
    //        get { return ImageLayout.None; }
    //        set { }
    //    }

    //    #endregion

    //    #region " Public Properties "

    //    private bool _SmartBounds = true;
    //    public bool SmartBounds
    //    {
    //        get { return _SmartBounds; }
    //        set { _SmartBounds = value; }
    //    }

    //    private bool _Movable = true;
    //    public bool Movable
    //    {
    //        get { return _Movable; }
    //        set { _Movable = value; }
    //    }

    //    private bool _Sizable = true;
    //    public bool Sizable
    //    {
    //        get { return _Sizable; }
    //        set { _Sizable = value; }
    //    }

    //    private Color _TransparencyKey;
    //    public Color TransparencyKey
    //    {
    //        get
    //        {
    //            if (_IsParentForm && !_ControlMode)
    //                return ParentForm.TransparencyKey;
    //            else
    //                return _TransparencyKey;
    //        }
    //        set
    //        {
    //            if (value == _TransparencyKey)
    //                return;
    //            _TransparencyKey = value;

    //            if (_IsParentForm && !_ControlMode)
    //            {
    //                ParentForm.TransparencyKey = value;
    //                ColorHook();
    //            }
    //        }
    //    }

    //    private FormBorderStyle _BorderStyle;
    //    public FormBorderStyle BorderStyle
    //    {
    //        get
    //        {
    //            if (_IsParentForm && !_ControlMode)
    //                return ParentForm.FormBorderStyle;
    //            else
    //                return _BorderStyle;
    //        }
    //        set
    //        {
    //            _BorderStyle = value;

    //            if (_IsParentForm && !_ControlMode)
    //            {
    //                ParentForm.FormBorderStyle = value;

    //                if (!(value == FormBorderStyle.None))
    //                {
    //                    Movable = false;
    //                    Sizable = false;
    //                }
    //            }
    //        }
    //    }

    //    private FormStartPosition _StartPosition;
    //    public FormStartPosition StartPosition
    //    {
    //        get
    //        {
    //            if (_IsParentForm && !_ControlMode)
    //                return ParentForm.StartPosition;
    //            else
    //                return _StartPosition;
    //        }
    //        set
    //        {
    //            _StartPosition = value;

    //            if (_IsParentForm && !_ControlMode)
    //            {
    //                ParentForm.StartPosition = value;
    //            }
    //        }
    //    }

    //    private bool _NoRounding;
    //    public bool NoRounding
    //    {
    //        get { return _NoRounding; }
    //        set
    //        {
    //            _NoRounding = value;
    //            Invalidate();
    //        }
    //    }

    //    private Image _Image;
    //    public Image Image
    //    {
    //        get { return _Image; }
    //        set
    //        {
    //            if (value == null)
    //                _ImageSize = Size.Empty;
    //            else
    //                _ImageSize = value.Size;

    //            _Image = value;
    //            Invalidate();
    //        }
    //    }

    //    private Dictionary<string, Color> Items = new Dictionary<string, Color>();
    //    public Bloom[] Colors
    //    {
    //        get
    //        {
    //            List<Bloom> T = new List<Bloom>();
    //            Dictionary<string, Color>.Enumerator E = Items.GetEnumerator();

    //            while (E.MoveNext())
    //            {
    //                T.Add(new Bloom(E.Current.Key, E.Current.Value));
    //            }

    //            return T.ToArray()();
    //        }
    //        set
    //        {
    //            foreach (Bloom B in value)
    //            {
    //                if (Items.ContainsKey(B.Name))
    //                    Items[B.Name] = B.Value;
    //            }

    //            InvalidateCustimization();
    //            ColorHook();
    //            Invalidate();
    //        }
    //    }

    //    private string _Customization;
    //    public string Customization
    //    {
    //        get { return _Customization; }
    //        set
    //        {
    //            if (value == _Customization)
    //                return;

    //            byte[] Data = null;
    //            Bloom[] Items = Colors;

    //            try
    //            {
    //                Data = Convert.FromBase64String(value);
    //                for (int I = 0; I <= Items.Length - 1; I++)
    //                {
    //                    Items[I].Value = Color.FromArgb(BitConverter.ToInt32(Data, I * 4));
    //                }
    //            }
    //            catch
    //            {
    //                return;
    //            }

    //            _Customization = value;

    //            Colors = Items;
    //            ColorHook();
    //            Invalidate();
    //        }
    //    }

    //    private bool _Transparent;
    //    public bool Transparent
    //    {
    //        get { return _Transparent; }
    //        set
    //        {
    //            _Transparent = value;
    //            if (!(IsHandleCreated || _ControlMode))
    //                return;

    //            if (!value && !(BackColor.A == 255))
    //            {
    //                throw new Exception("Unable to change value to false while a transparent BackColor is in use.");
    //            }

    //            SetStyle(ControlStyles.Opaque, !value);
    //            SetStyle(ControlStyles.SupportsTransparentBackColor, value);

    //            InvalidateBitmap();
    //            Invalidate();
    //        }
    //    }

    //    #endregion

    //    #region " Private Properties "

    //    private Size _ImageSize;
    //    protected Size ImageSize
    //    {
    //        get { return _ImageSize; }
    //    }

    //    private bool _IsParentForm;
    //    protected bool IsParentForm
    //    {
    //        get { return _IsParentForm; }
    //    }

    //    protected bool IsParentMdi
    //    {
    //        get
    //        {
    //            if (Parent == null)
    //                return false;
    //            return Parent.Parent != null;
    //        }
    //    }

    //    private int _LockWidth;
    //    protected int LockWidth
    //    {
    //        get { return _LockWidth; }
    //        set
    //        {
    //            _LockWidth = value;
    //            if (!(LockWidth == 0) && IsHandleCreated)
    //                Width = LockWidth;
    //        }
    //    }

    //    private int _LockHeight;
    //    protected int LockHeight
    //    {
    //        get { return _LockHeight; }
    //        set
    //        {
    //            _LockHeight = value;
    //            if (!(LockHeight == 0) && IsHandleCreated)
    //                Height = LockHeight;
    //        }
    //    }

    //    private int _Header = 24;
    //    protected int Header
    //    {
    //        get { return _Header; }
    //        set
    //        {
    //            _Header = value;

    //            if (!_ControlMode)
    //            {
    //                Frame = new Rectangle(7, 7, Width - 14, value - 7);
    //                Invalidate();
    //            }
    //        }
    //    }

    //    private bool _ControlMode;
    //    protected bool ControlMode
    //    {
    //        get { return _ControlMode; }
    //        set
    //        {
    //            _ControlMode = value;

    //            Transparent = _Transparent;
    //            if (_Transparent && _BackColor)
    //                BackColor = Color.Transparent;

    //            InvalidateBitmap();
    //            Invalidate();
    //        }
    //    }

    //    private bool _IsAnimated;
    //    protected bool IsAnimated
    //    {
    //        get { return _IsAnimated; }
    //        set
    //        {
    //            _IsAnimated = value;
    //            InvalidateTimer();
    //        }
    //    }

    //    #endregion


    //    #region " Property Helpers "

    //    protected Pen GetPen(string name)
    //    {
    //        return new Pen(Items[name]);
    //    }
    //    protected Pen GetPen(string name, float width)
    //    {
    //        return new Pen(Items[name], width);
    //    }

    //    protected SolidBrush GetBrush(string name)
    //    {
    //        return new SolidBrush(Items[name]);
    //    }

    //    protected Color GetColor(string name)
    //    {
    //        return Items[name];
    //    }

    //    protected void SetColor(string name, Color value)
    //    {
    //        if (Items.ContainsKey(name))
    //            Items[name] = value;
    //        else
    //            Items.Add(name, value);
    //    }
    //    protected void SetColor(string name, byte r, byte g, byte b)
    //    {
    //        SetColor(name, Color.FromArgb(r, g, b));
    //    }
    //    protected void SetColor(string name, byte a, byte r, byte g, byte b)
    //    {
    //        SetColor(name, Color.FromArgb(a, r, g, b));
    //    }
    //    protected void SetColor(string name, byte a, Color value)
    //    {
    //        SetColor(name, Color.FromArgb(a, value));
    //    }

    //    private void InvalidateBitmap()
    //    {
    //        if (_Transparent && _ControlMode)
    //        {
    //            if (Width == 0 || Height == 0)
    //                return;
    //            B = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
    //            G = Graphics.FromImage(B);
    //        }
    //        else {
    //            G = null;
    //            B = null;
    //        }
    //    }

    //    private void InvalidateCustimization()
    //    {
    //        MemoryStream M = new MemoryStream(Items.Count * 4);

    //        foreach (Bloom B in Colors)
    //        {
    //            M.Write(BitConverter.GetBytes(B.Value.ToArgb()), 0, 4);
    //        }

    //        M.Close();
    //        _Customization = Convert.ToBase64String(M.ToArray()());
    //    }

    //    private void InvalidateTimer()
    //    {
    //        if (DesignMode || !DoneCreation)
    //            return;

    //        if (_IsAnimated)
    //        {
    //            ThemeShare.AddAnimationCallback(DoAnimation);
    //        }
    //        else {
    //            ThemeShare.RemoveAnimationCallback(DoAnimation);
    //        }
    //    }

    //    #endregion


    //    #region " User Hooks "

    //    protected abstract void ColorHook();
    //    protected abstract void PaintHook();

    //    protected virtual void OnCreation()
    //    {
    //    }

    //    protected virtual void OnAnimation()
    //    {
    //    }

    //    #endregion


    //    #region " Offset "

    //    private Rectangle OffsetReturnRectangle;
    //    protected Rectangle Offset(Rectangle r, int amount)
    //    {
    //        OffsetReturnRectangle = new Rectangle(r.X + amount, r.Y + amount, r.Width - (amount * 2), r.Height - (amount * 2));
    //        return OffsetReturnRectangle;
    //    }

    //    private Size OffsetReturnSize;
    //    protected Size Offset(Size s, int amount)
    //    {
    //        OffsetReturnSize = new Size(s.Width + amount, s.Height + amount);
    //        return OffsetReturnSize;
    //    }

    //    private Point OffsetReturnPoint;
    //    protected Point Offset(Point p, int amount)
    //    {
    //        OffsetReturnPoint = new Point(p.X + amount, p.Y + amount);
    //        return OffsetReturnPoint;
    //    }

    //    #endregion

    //    #region " Center "


    //    private Point CenterReturn;
    //    protected Point Center(Rectangle p, Rectangle c)
    //    {
    //        CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X + c.X, (p.Height / 2 - c.Height / 2) + p.Y + c.Y);
    //        return CenterReturn;
    //    }
    //    protected Point Center(Rectangle p, Size c)
    //    {
    //        CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X, (p.Height / 2 - c.Height / 2) + p.Y);
    //        return CenterReturn;
    //    }

    //    protected Point Center(Rectangle child)
    //    {
    //        return Center(Width, Height, child.Width, child.Height);
    //    }
    //    protected Point Center(Size child)
    //    {
    //        return Center(Width, Height, child.Width, child.Height);
    //    }
    //    protected Point Center(int childWidth, int childHeight)
    //    {
    //        return Center(Width, Height, childWidth, childHeight);
    //    }

    //    protected Point Center(Size p, Size c)
    //    {
    //        return Center(p.Width, p.Height, c.Width, c.Height);
    //    }

    //    protected Point Center(int pWidth, int pHeight, int cWidth, int cHeight)
    //    {
    //        CenterReturn = new Point(pWidth / 2 - cWidth / 2, pHeight / 2 - cHeight / 2);
    //        return CenterReturn;
    //    }

    //    #endregion

    //    #region " Measure "

    //    private Bitmap MeasureBitmap;

    //    private Graphics MeasureGraphics;
    //    protected Size Measure()
    //    {
    //        lock (MeasureGraphics)
    //        {
    //            return MeasureGraphics.MeasureString(Text, Font, Width).ToSize()();
    //        }
    //    }
    //    protected Size Measure(string text)
    //    {
    //        lock (MeasureGraphics)
    //        {
    //            return MeasureGraphics.MeasureString(text, Font, Width).ToSize()();
    //        }
    //    }

    //    #endregion


    //    #region " DrawPixel "


    //    private SolidBrush DrawPixelBrush;
    //    protected void DrawPixel(Color c1, int x, int y)
    //    {
    //        if (_Transparent)
    //        {
    //            B.SetPixel(x, y, c1);
    //        }
    //        else {
    //            DrawPixelBrush = new SolidBrush(c1);
    //            G.FillRectangle(DrawPixelBrush, x, y, 1, 1);
    //        }
    //    }

    //    #endregion

    //    #region " DrawCorners "


    //    private SolidBrush DrawCornersBrush;
    //    protected void DrawCorners(Color c1, int offset)
    //    {
    //        DrawCorners(c1, 0, 0, Width, Height, offset);
    //    }
    //    protected void DrawCorners(Color c1, Rectangle r1, int offset)
    //    {
    //        DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height, offset);
    //    }
    //    protected void DrawCorners(Color c1, int x, int y, int width, int height, int offset)
    //    {
    //        DrawCorners(c1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
    //    }

    //    protected void DrawCorners(Color c1)
    //    {
    //        DrawCorners(c1, 0, 0, Width, Height);
    //    }
    //    protected void DrawCorners(Color c1, Rectangle r1)
    //    {
    //        DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height);
    //    }
    //    protected void DrawCorners(Color c1, int x, int y, int width, int height)
    //    {
    //        if (_NoRounding)
    //            return;

    //        if (_Transparent)
    //        {
    //            B.SetPixel(x, y, c1);
    //            B.SetPixel(x + (width - 1), y, c1);
    //            B.SetPixel(x, y + (height - 1), c1);
    //            B.SetPixel(x + (width - 1), y + (height - 1), c1);
    //        }
    //        else {
    //            DrawCornersBrush = new SolidBrush(c1);
    //            G.FillRectangle(DrawCornersBrush, x, y, 1, 1);
    //            G.FillRectangle(DrawCornersBrush, x + (width - 1), y, 1, 1);
    //            G.FillRectangle(DrawCornersBrush, x, y + (height - 1), 1, 1);
    //            G.FillRectangle(DrawCornersBrush, x + (width - 1), y + (height - 1), 1, 1);
    //        }
    //    }

    //    #endregion

    //    #region " DrawBorders "

    //    protected void DrawBorders(Pen p1, int offset)
    //    {
    //        DrawBorders(p1, 0, 0, Width, Height, offset);
    //    }
    //    protected void DrawBorders(Pen p1, Rectangle r, int offset)
    //    {
    //        DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
    //    }
    //    protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
    //    {
    //        DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
    //    }

    //    protected void DrawBorders(Pen p1)
    //    {
    //        DrawBorders(p1, 0, 0, Width, Height);
    //    }
    //    protected void DrawBorders(Pen p1, Rectangle r)
    //    {
    //        DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
    //    }
    //    protected void DrawBorders(Pen p1, int x, int y, int width, int height)
    //    {
    //        G.DrawRectangle(p1, x, y, width - 1, height - 1);
    //    }

    //    #endregion

    //    #region " DrawText "

    //    private Point DrawTextPoint;

    //    private Size DrawTextSize;
    //    protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
    //    {
    //        DrawText(b1, Text, a, x, y);
    //    }
    //    protected void DrawText(Brush b1, string text, HorizontalAlignment a, int x, int y)
    //    {
    //        if (text.Length == 0)
    //            return;

    //        DrawTextSize = Measure(text);
    //        DrawTextPoint = new Point(Width / 2 - DrawTextSize.Width / 2, Header / 2 - DrawTextSize.Height / 2);

    //        switch (a)
    //        {
    //            case HorizontalAlignment.Left:
    //                G.DrawString(text, Font, b1, x, DrawTextPoint.Y + y);
    //                break;
    //            case HorizontalAlignment.Center:
    //                G.DrawString(text, Font, b1, DrawTextPoint.X + x, DrawTextPoint.Y + y);
    //                break;
    //            case HorizontalAlignment.Right:
    //                G.DrawString(text, Font, b1, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
    //                break;
    //        }
    //    }

    //    protected void DrawText(Brush b1, Point p1)
    //    {
    //        if (Text.Length == 0)
    //            return;
    //        G.DrawString(Text, Font, b1, p1);
    //    }
    //    protected void DrawText(Brush b1, int x, int y)
    //    {
    //        if (Text.Length == 0)
    //            return;
    //        G.DrawString(Text, Font, b1, x, y);
    //    }

    //    #endregion

    //    #region " DrawImage "


    //    private Point DrawImagePoint;
    //    protected void DrawImage(HorizontalAlignment a, int x, int y)
    //    {
    //        DrawImage(_Image, a, x, y);
    //    }
    //    protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
    //    {
    //        if (image == null)
    //            return;
    //        DrawImagePoint = new Point(Width / 2 - image.Width / 2, Header / 2 - image.Height / 2);

    //        switch (a)
    //        {
    //            case HorizontalAlignment.Left:
    //                G.DrawImage(image, x, DrawImagePoint.Y + y, image.Width, image.Height);
    //                break;
    //            case HorizontalAlignment.Center:
    //                G.DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y, image.Width, image.Height);
    //                break;
    //            case HorizontalAlignment.Right:
    //                G.DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y, image.Width, image.Height);
    //                break;
    //        }
    //    }

    //    protected void DrawImage(Point p1)
    //    {
    //        DrawImage(_Image, p1.X, p1.Y);
    //    }
    //    protected void DrawImage(int x, int y)
    //    {
    //        DrawImage(_Image, x, y);
    //    }

    //    protected void DrawImage(Image image, Point p1)
    //    {
    //        DrawImage(image, p1.X, p1.Y);
    //    }
    //    protected void DrawImage(Image image, int x, int y)
    //    {
    //        if (image == null)
    //            return;
    //        G.DrawImage(image, x, y, image.Width, image.Height);
    //    }

    //    #endregion

    //    #region " DrawGradient "

    //    private LinearGradientBrush DrawGradientBrush;

    //    private Rectangle DrawGradientRectangle;
    //    protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
    //    {
    //        DrawGradientRectangle = new Rectangle(x, y, width, height);
    //        DrawGradient(blend, DrawGradientRectangle);
    //    }
    //    protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
    //    {
    //        DrawGradientRectangle = new Rectangle(x, y, width, height);
    //        DrawGradient(blend, DrawGradientRectangle, angle);
    //    }

    //    protected void DrawGradient(ColorBlend blend, Rectangle r)
    //    {
    //        DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, 90f);
    //        DrawGradientBrush.InterpolationColors = blend;
    //        G.FillRectangle(DrawGradientBrush, r);
    //    }
    //    protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
    //    {
    //        DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
    //        DrawGradientBrush.InterpolationColors = blend;
    //        G.FillRectangle(DrawGradientBrush, r);
    //    }


    //    protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
    //    {
    //        DrawGradientRectangle = new Rectangle(x, y, width, height);
    //        DrawGradient(c1, c2, DrawGradientRectangle);
    //    }
    //    protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
    //    {
    //        DrawGradientRectangle = new Rectangle(x, y, width, height);
    //        DrawGradient(c1, c2, DrawGradientRectangle, angle);
    //    }

    //    protected void DrawGradient(Color c1, Color c2, Rectangle r)
    //    {
    //        DrawGradientBrush = new LinearGradientBrush(r, c1, c2, 90f);
    //        G.FillRectangle(DrawGradientBrush, r);
    //    }
    //    protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
    //    {
    //        DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
    //        G.FillRectangle(DrawGradientBrush, r);
    //    }

    //    #endregion

    //    #region " DrawRadial "

    //    private GraphicsPath DrawRadialPath;
    //    private PathGradientBrush DrawRadialBrush1;
    //    private LinearGradientBrush DrawRadialBrush2;

    //    private Rectangle DrawRadialRectangle;
    //    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(blend, DrawRadialRectangle, width / 2, height / 2);
    //    }
    //    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, Point center)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(blend, DrawRadialRectangle, center.X, center.Y);
    //    }
    //    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, int cx, int cy)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(blend, DrawRadialRectangle, cx, cy);
    //    }

    //    public void DrawRadial(ColorBlend blend, Rectangle r)
    //    {
    //        DrawRadial(blend, r, r.Width / 2, r.Height / 2);
    //    }
    //    public void DrawRadial(ColorBlend blend, Rectangle r, Point center)
    //    {
    //        DrawRadial(blend, r, center.X, center.Y);
    //    }
    //    public void DrawRadial(ColorBlend blend, Rectangle r, int cx, int cy)
    //    {
    //        DrawRadialPath.Reset();
    //        DrawRadialPath.AddEllipse(r.X, r.Y, r.Width - 1, r.Height - 1);

    //        DrawRadialBrush1 = new PathGradientBrush(DrawRadialPath);
    //        DrawRadialBrush1.CenterPoint = new Point(r.X + cx, r.Y + cy);
    //        DrawRadialBrush1.InterpolationColors = blend;

    //        if (G.SmoothingMode == SmoothingMode.AntiAlias)
    //        {
    //            G.FillEllipse(DrawRadialBrush1, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3);
    //        }
    //        else {
    //            G.FillEllipse(DrawRadialBrush1, r);
    //        }
    //    }


    //    protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(c1, c2, DrawGradientRectangle);
    //    }
    //    protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height, float angle)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(c1, c2, DrawGradientRectangle, angle);
    //    }

    //    protected void DrawRadial(Color c1, Color c2, Rectangle r)
    //    {
    //        DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, 90f);
    //        G.FillRectangle(DrawGradientBrush, r);
    //    }
    //    protected void DrawRadial(Color c1, Color c2, Rectangle r, float angle)
    //    {
    //        DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, angle);
    //        G.FillEllipse(DrawGradientBrush, r);
    //    }

    //    #endregion

    //    #region " CreateRound "

    //    private GraphicsPath CreateRoundPath;

    //    private Rectangle CreateRoundRectangle;
    //    public GraphicsPath CreateRound(int x, int y, int width, int height, int slope)
    //    {
    //        CreateRoundRectangle = new Rectangle(x, y, width, height);
    //        return CreateRound(CreateRoundRectangle, slope);
    //    }

    //    public GraphicsPath CreateRound(Rectangle r, int slope)
    //    {
    //        CreateRoundPath = new GraphicsPath(FillMode.Winding);
    //        CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180f, 90f);
    //        CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270f, 90f);
    //        CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0f, 90f);
    //        CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90f, 90f);
    //        CreateRoundPath.CloseFigure();
    //        return CreateRoundPath;
    //    }

    //    #endregion

    //}

    //abstract class ThemeControl154 : Control
    //{


    //    #region " Initialization "

    //    protected Graphics G;

    //    protected Bitmap B;
    //    public ThemeControl154()
    //    {
    //        SetStyle((ControlStyles)139270, true);

    //        _ImageSize = Size.Empty;
    //        Font = new Font("Verdana", 8);

    //        MeasureBitmap = new Bitmap(1, 1);
    //        MeasureGraphics = Graphics.FromImage(MeasureBitmap);

    //        DrawRadialPath = new GraphicsPath();

    //        InvalidateCustimization();
    //        //Remove?
    //    }

    //    protected override sealed void OnHandleCreated(EventArgs e)
    //    {
    //        InvalidateCustimization();
    //        ColorHook();

    //        if (!(_LockWidth == 0))
    //            Width = _LockWidth;
    //        if (!(_LockHeight == 0))
    //            Height = _LockHeight;

    //        Transparent = _Transparent;
    //        if (_Transparent && _BackColor)
    //            BackColor = Color.Transparent;

    //        base.OnHandleCreated(e);
    //    }

    //    private bool DoneCreation;
    //    protected override sealed void OnParentChanged(EventArgs e)
    //    {
    //        if (Parent != null)
    //        {
    //            OnCreation();
    //            DoneCreation = true;
    //            InvalidateTimer();
    //        }

    //        base.OnParentChanged(e);
    //    }

    //    #endregion

    //    private void DoAnimation(bool i)
    //    {
    //        OnAnimation();
    //        if (i)
    //            Invalidate();
    //    }

    //    protected override sealed void OnPaint(PaintEventArgs e)
    //    {
    //        if (Width == 0 || Height == 0)
    //            return;

    //        if (_Transparent)
    //        {
    //            PaintHook();
    //            e.Graphics.DrawImage(B, 0, 0);
    //        }
    //        else {
    //            G = e.Graphics;
    //            PaintHook();
    //        }
    //    }

    //    protected override void OnHandleDestroyed(EventArgs e)
    //    {
    //        ThemeShare.RemoveAnimationCallback(DoAnimation);
    //        base.OnHandleDestroyed(e);
    //    }

    //    #region " Size Handling "

    //    protected override sealed void OnSizeChanged(EventArgs e)
    //    {
    //        if (_Transparent)
    //        {
    //            InvalidateBitmap();
    //        }

    //        Invalidate();
    //        base.OnSizeChanged(e);
    //    }

    //    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    //    {
    //        if (!(_LockWidth == 0))
    //            width = _LockWidth;
    //        if (!(_LockHeight == 0))
    //            height = _LockHeight;
    //        base.SetBoundsCore(x, y, width, height, specified);
    //    }

    //    #endregion

    //    #region " State Handling "

    //    private bool InPosition;
    //    protected override void OnMouseEnter(EventArgs e)
    //    {
    //        InPosition = true;
    //        SetState(MouseState.Over);
    //        base.OnMouseEnter(e);
    //    }

    //    protected override void OnMouseUp(MouseEventArgs e)
    //    {
    //        if (InPosition)
    //            SetState(MouseState.Over);
    //        base.OnMouseUp(e);
    //    }

    //    protected override void OnMouseDown(MouseEventArgs e)
    //    {
    //        if (e.Button == MouseButtons.Left)
    //            SetState(MouseState.Down);
    //        base.OnMouseDown(e);
    //    }

    //    protected override void OnMouseLeave(EventArgs e)
    //    {
    //        InPosition = false;
    //        SetState(MouseState.None);
    //        base.OnMouseLeave(e);
    //    }

    //    protected override void OnEnabledChanged(EventArgs e)
    //    {
    //        if (Enabled)
    //            SetState(MouseState.None);
    //        else
    //            SetState(MouseState.Block);
    //        base.OnEnabledChanged(e);
    //    }

    //    protected MouseState State;
    //    private void SetState(MouseState current)
    //    {
    //        State = current;
    //        Invalidate();
    //    }

    //    #endregion


    //    #region " Base Properties "

    //    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //    public override Color ForeColor
    //    {
    //        get { return Color.Empty; }
    //        set { }
    //    }
    //    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //    public override Image BackgroundImage
    //    {
    //        get { return null; }
    //        set { }
    //    }
    //    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //    public override ImageLayout BackgroundImageLayout
    //    {
    //        get { return ImageLayout.None; }
    //        set { }
    //    }

    //    public override string Text
    //    {
    //        get { return base.Text; }
    //        set
    //        {
    //            base.Text = value;
    //            Invalidate();
    //        }
    //    }
    //    public override Font Font
    //    {
    //        get { return base.Font; }
    //        set
    //        {
    //            base.Font = value;
    //            Invalidate();
    //        }
    //    }

    //    private bool _BackColor;
    //    [Category("Misc")]
    //    public override Color BackColor
    //    {
    //        get { return base.BackColor; }
    //        set
    //        {
    //            if (!IsHandleCreated && value == Color.Transparent)
    //            {
    //                _BackColor = true;
    //                return;
    //            }

    //            base.BackColor = value;
    //            if (Parent != null)
    //                ColorHook();
    //        }
    //    }

    //    #endregion

    //    #region " Public Properties "

    //    private bool _NoRounding;
    //    public bool NoRounding
    //    {
    //        get { return _NoRounding; }
    //        set
    //        {
    //            _NoRounding = value;
    //            Invalidate();
    //        }
    //    }

    //    private Image _Image;
    //    public Image Image
    //    {
    //        get { return _Image; }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                _ImageSize = Size.Empty;
    //            }
    //            else {
    //                _ImageSize = value.Size;
    //            }

    //            _Image = value;
    //            Invalidate();
    //        }
    //    }

    //    private bool _Transparent;
    //    public bool Transparent
    //    {
    //        get { return _Transparent; }
    //        set
    //        {
    //            _Transparent = value;
    //            if (!IsHandleCreated)
    //                return;

    //            if (!value && !(BackColor.A == 255))
    //            {
    //                throw new Exception("Unable to change value to false while a transparent BackColor is in use.");
    //            }

    //            SetStyle(ControlStyles.Opaque, !value);
    //            SetStyle(ControlStyles.SupportsTransparentBackColor, value);

    //            if (value)
    //                InvalidateBitmap();
    //            else
    //                B = null;
    //            Invalidate();
    //        }
    //    }

    //    private Dictionary<string, Color> Items = new Dictionary<string, Color>();
    //    public Bloom[] Colors
    //    {
    //        get
    //        {
    //            List<Bloom> T = new List<Bloom>();
    //            Dictionary<string, Color>.Enumerator E = Items.GetEnumerator();

    //            while (E.MoveNext())
    //            {
    //                T.Add(new Bloom(E.Current.Key, E.Current.Value));
    //            }

    //            return T.ToArray()();
    //        }
    //        set
    //        {
    //            foreach (Bloom B in value)
    //            {
    //                if (Items.ContainsKey(B.Name))
    //                    Items[B.Name] = B.Value;
    //            }

    //            InvalidateCustimization();
    //            ColorHook();
    //            Invalidate();
    //        }
    //    }

    //    private string _Customization;
    //    public string Customization
    //    {
    //        get { return _Customization; }
    //        set
    //        {
    //            if (value == _Customization)
    //                return;

    //            byte[] Data = null;
    //            Bloom[] Items = Colors;

    //            try
    //            {
    //                Data = Convert.FromBase64String(value);
    //                for (int I = 0; I <= Items.Length - 1; I++)
    //                {
    //                    Items[I].Value = Color.FromArgb(BitConverter.ToInt32(Data, I * 4));
    //                }
    //            }
    //            catch
    //            {
    //                return;
    //            }

    //            _Customization = value;

    //            Colors = Items;
    //            ColorHook();
    //            Invalidate();
    //        }
    //    }

    //    #endregion

    //    #region " Private Properties "

    //    private Size _ImageSize;
    //    protected Size ImageSize
    //    {
    //        get { return _ImageSize; }
    //    }

    //    private int _LockWidth;
    //    protected int LockWidth
    //    {
    //        get { return _LockWidth; }
    //        set
    //        {
    //            _LockWidth = value;
    //            if (!(LockWidth == 0) && IsHandleCreated)
    //                Width = LockWidth;
    //        }
    //    }

    //    private int _LockHeight;
    //    protected int LockHeight
    //    {
    //        get { return _LockHeight; }
    //        set
    //        {
    //            _LockHeight = value;
    //            if (!(LockHeight == 0) && IsHandleCreated)
    //                Height = LockHeight;
    //        }
    //    }

    //    private bool _IsAnimated;
    //    protected bool IsAnimated
    //    {
    //        get { return _IsAnimated; }
    //        set
    //        {
    //            _IsAnimated = value;
    //            InvalidateTimer();
    //        }
    //    }

    //    #endregion


    //    #region " Property Helpers "

    //    protected Pen GetPen(string name)
    //    {
    //        return new Pen(Items[name]);
    //    }
    //    protected Pen GetPen(string name, float width)
    //    {
    //        return new Pen(Items[name], width);
    //    }

    //    protected SolidBrush GetBrush(string name)
    //    {
    //        return new SolidBrush(Items[name]);
    //    }

    //    protected Color GetColor(string name)
    //    {
    //        return Items[name];
    //    }

    //    protected void SetColor(string name, Color value)
    //    {
    //        if (Items.ContainsKey(name))
    //            Items[name] = value;
    //        else
    //            Items.Add(name, value);
    //    }
    //    protected void SetColor(string name, byte r, byte g, byte b)
    //    {
    //        SetColor(name, Color.FromArgb(r, g, b));
    //    }
    //    protected void SetColor(string name, byte a, byte r, byte g, byte b)
    //    {
    //        SetColor(name, Color.FromArgb(a, r, g, b));
    //    }
    //    protected void SetColor(string name, byte a, Color value)
    //    {
    //        SetColor(name, Color.FromArgb(a, value));
    //    }

    //    private void InvalidateBitmap()
    //    {
    //        if (Width == 0 || Height == 0)
    //            return;
    //        B = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
    //        G = Graphics.FromImage(B);
    //    }

    //    private void InvalidateCustimization()
    //    {
    //        MemoryStream M = new MemoryStream(Items.Count * 4);

    //        foreach (Bloom B in Colors)
    //        {
    //            M.Write(BitConverter.GetBytes(B.Value.ToArgb()), 0, 4);
    //        }

    //        M.Close();
    //        _Customization = Convert.ToBase64String(M.ToArray()());
    //    }

    //    private void InvalidateTimer()
    //    {
    //        if (DesignMode || !DoneCreation)
    //            return;

    //        if (_IsAnimated)
    //        {
    //            ThemeShare.AddAnimationCallback(DoAnimation);
    //        }
    //        else {
    //            ThemeShare.RemoveAnimationCallback(DoAnimation);
    //        }
    //    }
    //    #endregion


    //    #region " User Hooks "

    //    protected abstract void ColorHook();
    //    protected abstract void PaintHook();

    //    protected virtual void OnCreation()
    //    {
    //    }

    //    protected virtual void OnAnimation()
    //    {
    //    }

    //    #endregion


    //    #region " Offset "

    //    private Rectangle OffsetReturnRectangle;
    //    protected Rectangle Offset(Rectangle r, int amount)
    //    {
    //        OffsetReturnRectangle = new Rectangle(r.X + amount, r.Y + amount, r.Width - (amount * 2), r.Height - (amount * 2));
    //        return OffsetReturnRectangle;
    //    }

    //    private Size OffsetReturnSize;
    //    protected Size Offset(Size s, int amount)
    //    {
    //        OffsetReturnSize = new Size(s.Width + amount, s.Height + amount);
    //        return OffsetReturnSize;
    //    }

    //    private Point OffsetReturnPoint;
    //    protected Point Offset(Point p, int amount)
    //    {
    //        OffsetReturnPoint = new Point(p.X + amount, p.Y + amount);
    //        return OffsetReturnPoint;
    //    }

    //    #endregion

    //    #region " Center "


    //    private Point CenterReturn;
    //    protected Point Center(Rectangle p, Rectangle c)
    //    {
    //        CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X + c.X, (p.Height / 2 - c.Height / 2) + p.Y + c.Y);
    //        return CenterReturn;
    //    }
    //    protected Point Center(Rectangle p, Size c)
    //    {
    //        CenterReturn = new Point((p.Width / 2 - c.Width / 2) + p.X, (p.Height / 2 - c.Height / 2) + p.Y);
    //        return CenterReturn;
    //    }

    //    protected Point Center(Rectangle child)
    //    {
    //        return Center(Width, Height, child.Width, child.Height);
    //    }
    //    protected Point Center(Size child)
    //    {
    //        return Center(Width, Height, child.Width, child.Height);
    //    }
    //    protected Point Center(int childWidth, int childHeight)
    //    {
    //        return Center(Width, Height, childWidth, childHeight);
    //    }

    //    protected Point Center(Size p, Size c)
    //    {
    //        return Center(p.Width, p.Height, c.Width, c.Height);
    //    }

    //    protected Point Center(int pWidth, int pHeight, int cWidth, int cHeight)
    //    {
    //        CenterReturn = new Point(pWidth / 2 - cWidth / 2, pHeight / 2 - cHeight / 2);
    //        return CenterReturn;
    //    }

    //    #endregion

    //    #region " Measure "

    //    private Bitmap MeasureBitmap;
    //    //TODO: Potential issues during multi-threading.
    //    private Graphics MeasureGraphics;

    //    protected Size Measure()
    //    {
    //        return MeasureGraphics.MeasureString(Text, Font, Width).ToSize()();
    //    }
    //    protected Size Measure(string text)
    //    {
    //        return MeasureGraphics.MeasureString(text, Font, Width).ToSize()();
    //    }

    //    #endregion


    //    #region " DrawPixel "


    //    private SolidBrush DrawPixelBrush;
    //    protected void DrawPixel(Color c1, int x, int y)
    //    {
    //        if (_Transparent)
    //        {
    //            B.SetPixel(x, y, c1);
    //        }
    //        else {
    //            DrawPixelBrush = new SolidBrush(c1);
    //            G.FillRectangle(DrawPixelBrush, x, y, 1, 1);
    //        }
    //    }

    //    #endregion

    //    #region " DrawCorners "


    //    private SolidBrush DrawCornersBrush;
    //    protected void DrawCorners(Color c1, int offset)
    //    {
    //        DrawCorners(c1, 0, 0, Width, Height, offset);
    //    }
    //    protected void DrawCorners(Color c1, Rectangle r1, int offset)
    //    {
    //        DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height, offset);
    //    }
    //    protected void DrawCorners(Color c1, int x, int y, int width, int height, int offset)
    //    {
    //        DrawCorners(c1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
    //    }

    //    protected void DrawCorners(Color c1)
    //    {
    //        DrawCorners(c1, 0, 0, Width, Height);
    //    }
    //    protected void DrawCorners(Color c1, Rectangle r1)
    //    {
    //        DrawCorners(c1, r1.X, r1.Y, r1.Width, r1.Height);
    //    }
    //    protected void DrawCorners(Color c1, int x, int y, int width, int height)
    //    {
    //        if (_NoRounding)
    //            return;

    //        if (_Transparent)
    //        {
    //            B.SetPixel(x, y, c1);
    //            B.SetPixel(x + (width - 1), y, c1);
    //            B.SetPixel(x, y + (height - 1), c1);
    //            B.SetPixel(x + (width - 1), y + (height - 1), c1);
    //        }
    //        else {
    //            DrawCornersBrush = new SolidBrush(c1);
    //            G.FillRectangle(DrawCornersBrush, x, y, 1, 1);
    //            G.FillRectangle(DrawCornersBrush, x + (width - 1), y, 1, 1);
    //            G.FillRectangle(DrawCornersBrush, x, y + (height - 1), 1, 1);
    //            G.FillRectangle(DrawCornersBrush, x + (width - 1), y + (height - 1), 1, 1);
    //        }
    //    }

    //    #endregion

    //    #region " DrawBorders "

    //    protected void DrawBorders(Pen p1, int offset)
    //    {
    //        DrawBorders(p1, 0, 0, Width, Height, offset);
    //    }
    //    protected void DrawBorders(Pen p1, Rectangle r, int offset)
    //    {
    //        DrawBorders(p1, r.X, r.Y, r.Width, r.Height, offset);
    //    }
    //    protected void DrawBorders(Pen p1, int x, int y, int width, int height, int offset)
    //    {
    //        DrawBorders(p1, x + offset, y + offset, width - (offset * 2), height - (offset * 2));
    //    }

    //    protected void DrawBorders(Pen p1)
    //    {
    //        DrawBorders(p1, 0, 0, Width, Height);
    //    }
    //    protected void DrawBorders(Pen p1, Rectangle r)
    //    {
    //        DrawBorders(p1, r.X, r.Y, r.Width, r.Height);
    //    }
    //    protected void DrawBorders(Pen p1, int x, int y, int width, int height)
    //    {
    //        G.DrawRectangle(p1, x, y, width - 1, height - 1);
    //    }

    //    #endregion

    //    #region " DrawText "

    //    private Point DrawTextPoint;

    //    private Size DrawTextSize;
    //    protected void DrawText(Brush b1, HorizontalAlignment a, int x, int y)
    //    {
    //        DrawText(b1, Text, a, x, y);
    //    }
    //    protected void DrawText(Brush b1, string text, HorizontalAlignment a, int x, int y)
    //    {
    //        if (text.Length == 0)
    //            return;

    //        DrawTextSize = Measure(text);
    //        DrawTextPoint = Center(DrawTextSize);

    //        switch (a)
    //        {
    //            case HorizontalAlignment.Left:
    //                G.DrawString(text, Font, b1, x, DrawTextPoint.Y + y);
    //                break;
    //            case HorizontalAlignment.Center:
    //                G.DrawString(text, Font, b1, DrawTextPoint.X + x, DrawTextPoint.Y + y);
    //                break;
    //            case HorizontalAlignment.Right:
    //                G.DrawString(text, Font, b1, Width - DrawTextSize.Width - x, DrawTextPoint.Y + y);
    //                break;
    //        }
    //    }

    //    protected void DrawText(Brush b1, Point p1)
    //    {
    //        if (Text.Length == 0)
    //            return;
    //        G.DrawString(Text, Font, b1, p1);
    //    }
    //    protected void DrawText(Brush b1, int x, int y)
    //    {
    //        if (Text.Length == 0)
    //            return;
    //        G.DrawString(Text, Font, b1, x, y);
    //    }

    //    #endregion

    //    #region " DrawImage "


    //    private Point DrawImagePoint;
    //    protected void DrawImage(HorizontalAlignment a, int x, int y)
    //    {
    //        DrawImage(_Image, a, x, y);
    //    }
    //    protected void DrawImage(Image image, HorizontalAlignment a, int x, int y)
    //    {
    //        if (image == null)
    //            return;
    //        DrawImagePoint = Center(image.Size);

    //        switch (a)
    //        {
    //            case HorizontalAlignment.Left:
    //                G.DrawImage(image, x, DrawImagePoint.Y + y, image.Width, image.Height);
    //                break;
    //            case HorizontalAlignment.Center:
    //                G.DrawImage(image, DrawImagePoint.X + x, DrawImagePoint.Y + y, image.Width, image.Height);
    //                break;
    //            case HorizontalAlignment.Right:
    //                G.DrawImage(image, Width - image.Width - x, DrawImagePoint.Y + y, image.Width, image.Height);
    //                break;
    //        }
    //    }

    //    protected void DrawImage(Point p1)
    //    {
    //        DrawImage(_Image, p1.X, p1.Y);
    //    }
    //    protected void DrawImage(int x, int y)
    //    {
    //        DrawImage(_Image, x, y);
    //    }

    //    protected void DrawImage(Image image, Point p1)
    //    {
    //        DrawImage(image, p1.X, p1.Y);
    //    }
    //    protected void DrawImage(Image image, int x, int y)
    //    {
    //        if (image == null)
    //            return;
    //        G.DrawImage(image, x, y, image.Width, image.Height);
    //    }

    //    #endregion

    //    #region " DrawGradient "

    //    private LinearGradientBrush DrawGradientBrush;

    //    private Rectangle DrawGradientRectangle;
    //    protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height)
    //    {
    //        DrawGradientRectangle = new Rectangle(x, y, width, height);
    //        DrawGradient(blend, DrawGradientRectangle);
    //    }
    //    protected void DrawGradient(ColorBlend blend, int x, int y, int width, int height, float angle)
    //    {
    //        DrawGradientRectangle = new Rectangle(x, y, width, height);
    //        DrawGradient(blend, DrawGradientRectangle, angle);
    //    }

    //    protected void DrawGradient(ColorBlend blend, Rectangle r)
    //    {
    //        DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, 90f);
    //        DrawGradientBrush.InterpolationColors = blend;
    //        G.FillRectangle(DrawGradientBrush, r);
    //    }
    //    protected void DrawGradient(ColorBlend blend, Rectangle r, float angle)
    //    {
    //        DrawGradientBrush = new LinearGradientBrush(r, Color.Empty, Color.Empty, angle);
    //        DrawGradientBrush.InterpolationColors = blend;
    //        G.FillRectangle(DrawGradientBrush, r);
    //    }


    //    protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height)
    //    {
    //        DrawGradientRectangle = new Rectangle(x, y, width, height);
    //        DrawGradient(c1, c2, DrawGradientRectangle);
    //    }
    //    protected void DrawGradient(Color c1, Color c2, int x, int y, int width, int height, float angle)
    //    {
    //        DrawGradientRectangle = new Rectangle(x, y, width, height);
    //        DrawGradient(c1, c2, DrawGradientRectangle, angle);
    //    }

    //    protected void DrawGradient(Color c1, Color c2, Rectangle r)
    //    {
    //        DrawGradientBrush = new LinearGradientBrush(r, c1, c2, 90f);
    //        G.FillRectangle(DrawGradientBrush, r);
    //    }
    //    protected void DrawGradient(Color c1, Color c2, Rectangle r, float angle)
    //    {
    //        DrawGradientBrush = new LinearGradientBrush(r, c1, c2, angle);
    //        G.FillRectangle(DrawGradientBrush, r);
    //    }

    //    #endregion

    //    #region " DrawRadial "

    //    private GraphicsPath DrawRadialPath;
    //    private PathGradientBrush DrawRadialBrush1;
    //    private LinearGradientBrush DrawRadialBrush2;

    //    private Rectangle DrawRadialRectangle;
    //    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(blend, DrawRadialRectangle, width / 2, height / 2);
    //    }
    //    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, Point center)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(blend, DrawRadialRectangle, center.X, center.Y);
    //    }
    //    public void DrawRadial(ColorBlend blend, int x, int y, int width, int height, int cx, int cy)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(blend, DrawRadialRectangle, cx, cy);
    //    }

    //    public void DrawRadial(ColorBlend blend, Rectangle r)
    //    {
    //        DrawRadial(blend, r, r.Width / 2, r.Height / 2);
    //    }
    //    public void DrawRadial(ColorBlend blend, Rectangle r, Point center)
    //    {
    //        DrawRadial(blend, r, center.X, center.Y);
    //    }
    //    public void DrawRadial(ColorBlend blend, Rectangle r, int cx, int cy)
    //    {
    //        DrawRadialPath.Reset();
    //        DrawRadialPath.AddEllipse(r.X, r.Y, r.Width - 1, r.Height - 1);

    //        DrawRadialBrush1 = new PathGradientBrush(DrawRadialPath);
    //        DrawRadialBrush1.CenterPoint = new Point(r.X + cx, r.Y + cy);
    //        DrawRadialBrush1.InterpolationColors = blend;

    //        if (G.SmoothingMode == SmoothingMode.AntiAlias)
    //        {
    //            G.FillEllipse(DrawRadialBrush1, r.X + 1, r.Y + 1, r.Width - 3, r.Height - 3);
    //        }
    //        else {
    //            G.FillEllipse(DrawRadialBrush1, r);
    //        }
    //    }


    //    protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(c1, c2, DrawRadialRectangle);
    //    }
    //    protected void DrawRadial(Color c1, Color c2, int x, int y, int width, int height, float angle)
    //    {
    //        DrawRadialRectangle = new Rectangle(x, y, width, height);
    //        DrawRadial(c1, c2, DrawRadialRectangle, angle);
    //    }

    //    protected void DrawRadial(Color c1, Color c2, Rectangle r)
    //    {
    //        DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, 90f);
    //        G.FillEllipse(DrawRadialBrush2, r);
    //    }
    //    protected void DrawRadial(Color c1, Color c2, Rectangle r, float angle)
    //    {
    //        DrawRadialBrush2 = new LinearGradientBrush(r, c1, c2, angle);
    //        G.FillEllipse(DrawRadialBrush2, r);
    //    }

    //    #endregion

    //    #region " CreateRound "

    //    private GraphicsPath CreateRoundPath;

    //    private Rectangle CreateRoundRectangle;
    //    public GraphicsPath CreateRound(int x, int y, int width, int height, int slope)
    //    {
    //        CreateRoundRectangle = new Rectangle(x, y, width, height);
    //        return CreateRound(CreateRoundRectangle, slope);
    //    }

    //    public GraphicsPath CreateRound(Rectangle r, int slope)
    //    {
    //        CreateRoundPath = new GraphicsPath(FillMode.Winding);
    //        CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180f, 90f);
    //        CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270f, 90f);
    //        CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0f, 90f);
    //        CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90f, 90f);
    //        CreateRoundPath.CloseFigure();
    //        return CreateRoundPath;
    //    }

    //    #endregion

    //}

    //static class ThemeShare
    //{

    //    #region " Animation "

    //    private static int Frames;
    //    private static bool Invalidate;

    //    public static PrecisionTimer ThemeTimer = new PrecisionTimer();
    //    //1000 / 50 = 20 FPS
    //    private const int FPS = 50;

    //    private const int Rate = 10;
    //    public delegate void AnimationDelegate(bool invalidate);


    //    private static List<AnimationDelegate> Callbacks = new List<AnimationDelegate>();
    //    private static void HandleCallbacks(IntPtr state, bool reserve)
    //    {
    //        Invalidate = (Frames >= FPS);
    //        if (Invalidate)
    //            Frames = 0;

    //        lock (Callbacks)
    //        {
    //            for (int I = 0; I <= Callbacks.Count - 1; I++)
    //            {
    //                Callbacks[I].Invoke(Invalidate);
    //            }
    //        }

    //        Frames += Rate;
    //    }

    //    private static void InvalidateThemeTimer()
    //    {
    //        if (Callbacks.Count == 0)
    //        {
    //            ThemeTimer.Delete();
    //        }
    //        else {
    //            ThemeTimer.Create(0, Rate, HandleCallbacks);
    //        }
    //    }

    //    public static void AddAnimationCallback(AnimationDelegate callback)
    //    {
    //        lock (Callbacks)
    //        {
    //            if (Callbacks.Contains(callback))
    //                return;

    //            Callbacks.Add(callback);
    //            InvalidateThemeTimer();
    //        }
    //    }

    //    public static void RemoveAnimationCallback(AnimationDelegate callback)
    //    {
    //        lock (Callbacks)
    //        {
    //            if (!Callbacks.Contains(callback))
    //                return;

    //            Callbacks.Remove(callback);
    //            InvalidateThemeTimer();
    //        }
    //    }

    //    #endregion

    //}

    //enum MouseState : byte
    //{
    //    None = 0,
    //    Over = 1,
    //    Down = 2,
    //    Block = 3
    //}

    //struct Bloom
    //{

    //    public string _Name;
    //    public string Name
    //    {
    //        get { return _Name; }
    //    }

    //    private Color _Value;
    //    public Color Value
    //    {
    //        get { return _Value; }
    //        set { _Value = value; }
    //    }

    //    public string ValueHex
    //    {
    //        get { return string.Concat("#", _Value.R.ToString("X2", null), _Value.G.ToString("X2", null), _Value.B.ToString("X2", null)); }
    //        set
    //        {
    //            try
    //            {
    //                _Value = ColorTranslator.FromHtml(value);
    //            }
    //            catch
    //            {
    //                return;
    //            }
    //        }
    //    }


    //    public Bloom(string name, Color value)
    //    {
    //        _Name = name;
    //        _Value = value;
    //    }
    //}

    ////------------------
    ////Creator: aeonhack
    ////Site: elitevs.net
    ////Created: 11/30/2011
    ////Changed: 11/30/2011
    ////Version: 1.0.0
    ////------------------
    //class PrecisionTimer : IDisposable
    //{

    //    private bool _Enabled;
    //    public bool Enabled
    //    {
    //        get { return _Enabled; }
    //    }

    //    private IntPtr Handle;

    //    private TimerDelegate TimerCallback;
    //    [DllImport("kernel32.dll", EntryPoint = "CreateTimerQueueTimer")]
    //    private static extern bool CreateTimerQueueTimer(ref IntPtr handle, IntPtr queue, TimerDelegate callback, IntPtr state, uint dueTime, uint period, uint flags);

    //    [DllImport("kernel32.dll", EntryPoint = "DeleteTimerQueueTimer")]
    //    private static extern bool DeleteTimerQueueTimer(IntPtr queue, IntPtr handle, IntPtr callback);

    //    public delegate void TimerDelegate(IntPtr r1, bool r2);

    //    public void Create(uint dueTime, uint period, TimerDelegate callback)
    //    {
    //        if (_Enabled)
    //            return;

    //        TimerCallback = callback;
    //        bool Success = CreateTimerQueueTimer(ref Handle, IntPtr.Zero, TimerCallback, IntPtr.Zero, dueTime, period, 0);

    //        if (!Success)
    //            ThrowNewException("CreateTimerQueueTimer");
    //        _Enabled = Success;
    //    }

    //    public void Delete()
    //    {
    //        if (!_Enabled)
    //            return;
    //        bool Success = DeleteTimerQueueTimer(IntPtr.Zero, Handle, IntPtr.Zero);

    //        if (!Success && !(Marshal.GetLastWin32Error() == 997))
    //        {
    //            ThrowNewException("DeleteTimerQueueTimer");
    //        }

    //        _Enabled = !Success;
    //    }

    //    private void ThrowNewException(string name)
    //    {
    //        throw new Exception(string.Format("{0} failed. Win32Error: {1}", name, Marshal.GetLastWin32Error()));
    //    }

    //    public void Dispose()
    //    {
    //        Delete();
    //    }
    //}

}
