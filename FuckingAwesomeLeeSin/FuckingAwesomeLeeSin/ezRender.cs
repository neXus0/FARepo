using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace FuckingAwesomeLeeSin
{
    internal class ezText
    {
        public String Name { set; get; }
        public String Text { set; get; }
        public bool Draw { get { return addedtoMenu ? RenderMenu.Item("draw" + Name).GetValue<bool>() : false; } }
        public  int offsetX
        {
            get { return addedtoMenu ? RenderMenu.Item("offsetX" + Name).GetValue<Slider>().Value : 0; }
        }
        private int OffX { set; get; }
        private int OffY { set; get; }
        public  int offsetY
        {
            get { return addedtoMenu ? RenderMenu.Item("offsetY" + Name).GetValue<Slider>().Value : 0; }
        }
        private bool outline
        {
            get { return addedtoMenu ? RenderMenu.Item("drawOutline" + Name).GetValue<bool>() : false; }
        }
        private System.Drawing.Color Color { get { return addedtoMenu ? RenderMenu.Item("Colour" + Name).GetValue<Circle>().Color : System.Drawing.Color.FromArgb(RenderText.Color.R, RenderText.Color.G, RenderText.Color.B); } }
        public  Render.Text RenderText { set; get; }
        
        private Menu RenderMenu;
        private bool addedtoMenu;
        private bool textRendered;
        public bool drawCondtion = true;

        public ezText(String name, String text, Render.Text renderText, int offX = 0, int offY = 0)
        {
            Name = name;
            Text = text;
            RenderText = renderText;
            OffX = offX;
            OffY = offY;
        }

        public void Tick()
        {
            if (!Draw || !drawCondtion)
            {
                RenderText.Remove();
                textRendered = false;
                return;
            }
            RenderText.Color = new ColorBGRA(Color.R, Color.G, Color.B, Color.A);
            RenderText.text = Text;
            RenderText.OutLined = outline;
            RenderText.Offset = new Vector2(offsetX-Drawing.Width+OffX, offsetY-Drawing.Height+OffY);
            if (!textRendered && drawCondtion)
            {
                RenderText.Add();
                textRendered = true;
                return;
            }
            if (textRendered && drawCondtion)
            {
                Utility.DelayAction.Add(10, (() => RenderText.Remove()));
                textRendered = false;
            }
        }

        public void AddToMenu(Menu menu)
        {
            if (addedtoMenu) return;
            RenderMenu = menu;
            var sMenu = menu.AddSubMenu(new Menu(Name, Name));
            sMenu.AddItem(new MenuItem("draw" + Name, "Draw " + Name).SetValue(true));
            sMenu.AddItem(new MenuItem("drawOutline" + Name, "Draw Outline").SetValue(true));
            sMenu.AddItem(new MenuItem("offsetX" + Name, "X Offset").SetValue(new Slider(Drawing.Width, 0, Drawing.Width*2)));
            sMenu.AddItem(new MenuItem("offsetY" + Name, "Y Offset").SetValue(new Slider(Drawing.Height, 0, Drawing.Height*2)));
            sMenu.AddItem(new MenuItem("Colour" + Name, "Colour")).SetValue(new Circle(true, System.Drawing.Color.FromArgb(RenderText.Color.R, RenderText.Color.G, RenderText.Color.B)));
            addedtoMenu = true;
        }
    }
    internal class ezCircle
    {
        public String Name { set; get; }
        public bool Draw { get { return addedtoMenu ? RenderMenu.Item("draw" + Name).GetValue<bool>() : false; } }
        private System.Drawing.Color Color { get { return addedtoMenu ? RenderMenu.Item("Colour" + Name).GetValue<Circle>().Color : RenderCircle.Color; } }
        public  Render.Circle RenderCircle { set; get; }
        public int Thickness { get
        {
            return addedtoMenu ? RenderMenu.Item("thickness" + Name).GetValue<Slider>().Value : 1;
        } }

        private System.Drawing.Color prevColor;
        private int prevThickness;
        private Menu RenderMenu;
        private  bool addedtoMenu;
        private bool circRendered = false;
        private bool drawn = false;
        public bool drawCondtion = true;

        public ezCircle(string name, Render.Circle circle)
        {
            Name = name;
            RenderCircle = circle;
        }

        public void Tick()
        {
            if (!Draw && drawn || !drawCondtion)
            {
                RenderCircle.Dispose();
                RenderCircle.Remove();
                drawn = false;
            }
            if (!drawn && Draw && drawCondtion)
            {
                RenderCircle.Dispose();
                RenderCircle.Remove();
                RenderCircle.Color = Color;
                RenderCircle.Width = Thickness;
                prevColor = Color;
                prevThickness = Thickness;
                RenderCircle.Add();
                drawn = true;
            }
            if (Color != prevColor || Thickness != prevThickness && drawCondtion)
            {
                RenderCircle.Dispose();
                RenderCircle.Remove();
                RenderCircle.Color = Color;
                RenderCircle.Width = Thickness;
                prevColor = Color;
                prevThickness = Thickness;
                RenderCircle.Add();
            }
        }
        

        public void AddToMenu(Menu menu)
        {
            if (addedtoMenu) return;
            RenderMenu = menu;
            var sMenu = menu.AddSubMenu(new Menu(Name, Name));
            sMenu.AddItem(new MenuItem("draw" + Name, "Draw " + Name).SetValue(true));
            sMenu.AddItem(new MenuItem("thickness" + Name, "Thickness").SetValue(new Slider(7, 1, 20)));
            sMenu.AddItem(new MenuItem("Colour" + Name, "Colour")).SetValue(new Circle(true, RenderCircle.Color));
            addedtoMenu = true;
            if (Draw)
            {
                RenderCircle.Add();
            }
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_DomainUnload;
            prevColor = Color;
            prevThickness = Thickness;
        }

        void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            RenderCircle.Dispose();
        }
    }
}
