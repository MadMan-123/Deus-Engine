using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeusEngine
{
    class TextComponent : Component
    {
        string sText = "";
        Text text;

        public override void OnStart()
        {
            text = new Text();
            text.DisplayedString = sText;

        }

        public override void OnUpdate()
        {
            text.Position = transform.position;
            text.Rotation = transform.fRotation;
            Application.Instance.Draw(text);
        }

        public void SetText(string Text)
        {
            sText = Text;
        }

    }
}
