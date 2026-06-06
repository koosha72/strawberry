/*
 * Strawberry Game Engine
 * File: TextRenderer.cs
 * Author: Koosha Aabedini Nassab
 *
 * High-level text layouting and shaping utilities for rendering Persian/RTL text.
 */

using Strawberry.Graphics.Layers;
using Strawberry.Math;


namespace Strawberry.Graphics.Text;

public class TextRenderer
{
    enum JoiningForm
    {
        None = 0x00,
        LeftJoin = 0x01,
        RightJoin = 0x02,
        DualJoin = 0x03,
        Transparent = 0x04
    }
    class CharacterFormat
    {
        public TextDirection direction;
        public JoiningForm joining;
        public ushort[] formCodes;
        public string joiningGroup;
        public ushort index;
    }

    string text = "";
    string formattedText = "";
    bool forcePersianDigits = false;
    Font font;
    static Dictionary<ushort, CharacterFormat> chars = new Dictionary<ushort, CharacterFormat>();

    static Dictionary<string, ushort> ligatures = new Dictionary<string, ushort>();

    public string Text
    {
        get { return text; }
        set
        {
            text = value;
            formattedText = Format(text, forcePersianDigits);
        }
    }

    public Font Font
    {
        get { return font; }
        set
        { font = value; }
    }

    public bool ForcePersianDigits
    {
        get
        {
            return forcePersianDigits;
        }
        set
        {
            forcePersianDigits = value;
        }
    }

    public TextRenderer(Font font, string text, bool forcePersianDigits)
    {
        this.font = font;
        this.forcePersianDigits = forcePersianDigits;
        this.Text = text;
    }

    static TextRenderer()
    {
        Stream jStream = typeof(TextRenderer).Assembly.GetManifestResourceStream("Strawberry.Graphics.Text.Joinings.txt");
        StreamReader jReader = new StreamReader(jStream);
        while (!jReader.EndOfStream)
        {
            string str = jReader.ReadLine();
            string[] data = str.Split(';');
            ushort index = ushort.Parse(data[0], System.Globalization.NumberStyles.HexNumber);
            CharacterFormat format = new CharacterFormat();

            format.direction = TextDirection.None;
            format.joining = JoiningForm.None;
            format.joiningGroup = "";
            format.formCodes = new ushort[] { };
            format.index = index;
            data[2] = data[2].Replace(" ", "");
            data[3] = data[3].Replace("No_Joining_Group", "");
            data[3] = data[3].Replace(" ", "");
            if (data[2] == "R")
            {
                format.joining = JoiningForm.RightJoin;
                format.formCodes = new ushort[2] { 0, 0 };
                format.joiningGroup = data[3];
            }
            if (data[2] == "L")
            {
                format.joining = JoiningForm.LeftJoin;
                format.formCodes = new ushort[2] { 0, 0 };
                format.joiningGroup = data[3];
            }
            if (data[2] == "D")
            {
                format.joining = JoiningForm.DualJoin;
                format.formCodes = new ushort[4] { 0, 0, 0, 0 };
                format.joiningGroup = data[3];
            }
            if (data[2] == "T")
            {
                format.joining = JoiningForm.Transparent;
                format.formCodes = new ushort[1] { index };
                format.joiningGroup = data[3];
            }
            chars.Add(index, format);
        }
        jReader.Close();

        Stream unicodeData = typeof(TextRenderer).Assembly.GetManifestResourceStream("Strawberry.Graphics.Text.UnicodeDatabase.txt");
        StreamReader reader = new StreamReader(unicodeData);
        while (!reader.EndOfStream)
        {
            string str = reader.ReadLine();
            string[] data = str.Split(';');
            ushort index = ushort.Parse(data[0], System.Globalization.NumberStyles.HexNumber);
            if (!chars.ContainsKey(index))
            {
                CharacterFormat format = new CharacterFormat();
                format.joining = JoiningForm.None;
                format.joiningGroup = "";
                format.formCodes = new ushort[] { 1 };
                format.index = index;
                chars.Add(index, format);
            }
            if (data[4] == "AL" || data[4] == "R")
                chars[index].direction = TextDirection.RightToLeft;
            else if (data[4] == "CS")
                chars[index].direction = TextDirection.None;
            else
                chars[index].direction = TextDirection.LeftToRight;

            if (data[5].Length > 0)
            {
                string[] joining = data[5].Split(' ');
                if (joining.Length < 3)
                {
                    if (joining[0] == "<isolated>")
                    {
                        ushort temp = ushort.Parse(joining[1],
                            System.Globalization.NumberStyles.HexNumber);
                        if (chars.ContainsKey(temp))
                        {
                            if (chars[temp].joining != JoiningForm.None)
                                chars[temp].formCodes[0] = index;
                        }
                    }
                    if (joining[0] == "<final>")
                    {
                        ushort temp = ushort.Parse(joining[1],
                            System.Globalization.NumberStyles.HexNumber);
                        if (chars.ContainsKey(temp))
                        {
                            CharacterFormat frm = chars[temp];
                            if (frm.joining == JoiningForm.DualJoin || frm.joining == JoiningForm.RightJoin)
                                frm.formCodes[1] = index;
                        }
                    }
                    if (joining[0] == "<initial>")
                    {
                        ushort temp = ushort.Parse(joining[1],
                            System.Globalization.NumberStyles.HexNumber);
                        if (chars.ContainsKey(temp))
                        {
                            CharacterFormat frm = chars[temp];
                            if (frm.joining == JoiningForm.DualJoin)
                                frm.formCodes[2] = index;
                            else if (frm.joining == JoiningForm.LeftJoin)
                                frm.formCodes[1] = index;
                        }
                    }
                    if (joining[0] == "<medial>")
                    {
                        ushort temp = ushort.Parse(joining[1],
                            System.Globalization.NumberStyles.HexNumber);
                        if (chars.ContainsKey(temp))
                        {
                            CharacterFormat frm = chars[temp];
                            if (frm.joining == JoiningForm.DualJoin)
                                frm.formCodes[3] = index;
                        }
                    }
                }
                else
                {
                    ushort ind1 = ushort.Parse(joining[1], System.Globalization.NumberStyles.HexNumber);
                    ushort ind2 = ushort.Parse(joining[2], System.Globalization.NumberStyles.HexNumber);
                    if (chars.ContainsKey(ind1) && chars.ContainsKey(ind2))
                    {
                        CharacterFormat frm1 = chars[ind1];
                        CharacterFormat frm2 = chars[ind2];
                        if (frm1.joiningGroup == "LAM" && frm2.joiningGroup == "ALEF")
                        {
                            if (joining[0] == "<isolated>")
                            {
                                ushort j1 = frm1.formCodes[2];
                                ushort j2 = frm2.formCodes[1];
                                string s = new string(new char[] { (char)j1, (char)j2 });
                                ligatures.Add(s, index);
                            }
                            if (joining[0] == "<final>")
                            {
                                ushort j1 = frm1.formCodes[3];
                                ushort j2 = frm2.formCodes[1];
                                string s = new string(new char[] { (char)j1, (char)j2 });
                                ligatures.Add(s, index);
                            }
                        }
                    }
                }
            }

        }
        reader.Close();
    }

    static string Format(string text, bool forcePersianDigits)
    {
        string str = "";
        for (int i = 0; i < text.Length; i++)
        {
            CharacterFormat nextChar = new CharacterFormat();
            CharacterFormat prevChar = new CharacterFormat();
            ushort code = (ushort)text[i];
            if (!chars.ContainsKey(code))
                continue;
            CharacterFormat chr = chars[code];
            if (forcePersianDigits)
            {
                if (chr.index >= 0x30 && chr.index <= 0x39)
                {
                    ushort ind = (ushort)(code + 0x630);
                    chr = chars[ind];
                }
            }
            CharacterFormat chrForm = chars[chr.index];
            if (chr.formCodes.Length > 0)
                chrForm = chars[chr.formCodes[0]];
            if (chr.joining == JoiningForm.None)
            {
                str += (char)chr.index;
                continue;
            }
            if (chr.joining == JoiningForm.Transparent)
            {
                if (i - 1 >= 0)
                {
                    prevChar = chars[(ushort)text[i - 1]];
                    if (prevChar.joining == JoiningForm.Transparent)
                    {
                        if (chars.ContainsKey(0x25cc))
                        {
                            str += (char)0x25cc;
                        }
                    }
                }
                str += (char)chrForm.index;
                continue;
            }
            int j = 1;
            if (i + j < text.Length)
            {
                if (!chars.ContainsKey((ushort)text[i + j]))
                    continue;
                nextChar = chars[(ushort)text[i + j]];
            }
            while (nextChar.joining == JoiningForm.Transparent)
            {
                j++;
                if (i + j < text.Length && chars[(ushort)text[i + j]].joining != JoiningForm.Transparent)
                {
                    nextChar = chars[(ushort)text[i + j]];
                }
                else
                    break;
            }
            j = -1;
            if (i + j >= 0)
            {
                if (!chars.ContainsKey((ushort)text[i + j]))
                    continue;
                prevChar = chars[(ushort)text[i + j]];
            }
            while (prevChar.joining == JoiningForm.Transparent)
            {
                j--;
                if (i + j >= 0 && chars[(ushort)text[i + j]].joining != JoiningForm.Transparent)
                {
                    prevChar = chars[(ushort)text[i + j]];
                }
                else
                    break;
            }

            if (chr.joining == JoiningForm.DualJoin)
            {
                if ((nextChar.joining == JoiningForm.RightJoin || nextChar.joining == JoiningForm.DualJoin)
                    && (prevChar.joining == JoiningForm.LeftJoin || prevChar.joining == JoiningForm.DualJoin))
                    chrForm = chars[chr.formCodes[3]];
                else if (nextChar.joining == JoiningForm.RightJoin || nextChar.joining == JoiningForm.DualJoin)
                    chrForm = chars[chr.formCodes[2]];
                else if (prevChar.joining == JoiningForm.LeftJoin || prevChar.joining == JoiningForm.DualJoin)
                    chrForm = chars[chr.formCodes[1]];
            }
            else if (chr.joining == JoiningForm.LeftJoin)
            {
                if (nextChar.joining == JoiningForm.RightJoin || nextChar.joining == JoiningForm.DualJoin)
                    chrForm = chars[chr.formCodes[1]];
            }
            else if (chr.joining == JoiningForm.RightJoin)
            {
                if (prevChar.joining == JoiningForm.LeftJoin || prevChar.joining == JoiningForm.DualJoin)
                    chrForm = chars[chr.formCodes[1]];
            }
            str += (char)chrForm.index;
        }

        foreach (string k in ligatures.Keys)
        {
            str = str.Replace(k, ((char)ligatures[k]).ToString());
        }
        return str;
    }

    public static void Draw(SpriteLayer layer, Font font, string text, Vector2 position,
        Color color, TextAlign allign, TextDirection direction, bool forcePersianDigits)
    {
        if (text.Length == 0)
            return;
        string formattedText = Format(text, forcePersianDigits);
        Vector2 spos = new Vector2(position.X, position.Y);
        float w = 0;
        w = (float)font.GetWidth(formattedText);
        if (allign == TextAlign.Center)
        {
            if (direction == TextDirection.LeftToRight)
                spos.X -= (int)System.Math.Round(w / 2);
            else
                spos.X += (int)System.Math.Round(w / 2);
        }
        if (direction == TextDirection.LeftToRight && allign == TextAlign.Right)
            spos.X -= w;
        if (direction == TextDirection.RightToLeft && allign == TextAlign.Left)
            spos.X += w;
        TextDirection temp;
        List<int> indices = new List<int>();
        temp = chars[formattedText[0]].direction;
        char[] common = new char[] { ' ', '-', '،', '_', ':' };
        for (int i = 1; i < formattedText.Length; i++)
        {
            if (chars[formattedText[i]].direction != temp && formattedText[i] != ' ' && chars[formattedText[i]].direction != TextDirection.None)
            {
                temp = chars[formattedText[i]].direction;
                indices.Add(i);
            }
        }
        indices.Add(formattedText.Length);
        int prevIndex = 0;
        string txt = "";
        Vector2 pos = spos;
        foreach (int index in indices)
        {
            if (chars[formattedText[prevIndex]].direction == direction)
            {
                for (int i = prevIndex; i < index; i++)
                {
                    txt += formattedText[i];
                }
            }
            else
            {
                for (int i = index - 1; i >= prevIndex; i--)
                {
                    txt += formattedText[i];
                }
            }
            pos = layer.PushString(txt, font, pos, color, direction);
            prevIndex = index;
            txt = "";
        }
    }

    public static void Draw(SpriteLayer layer, Font font, string text, Vector2 position,
        Color color, TextAlign allign, TextDirection direction, bool forcePersianDigits, float size)
    {
        if (text.Length == 0)
            return;
        string formattedText = Format(text, forcePersianDigits);
        Vector2 spos = new Vector2(position.X, position.Y);
        float w = 0;
        w = (float)font.GetWidth(formattedText, size);
        if (allign == TextAlign.Center)
        {
            if (direction == TextDirection.LeftToRight)
                spos.X -= (int)System.Math.Round(w / 2);
            else
                spos.X += (int)System.Math.Round(w / 2);
        }
        if (direction == TextDirection.LeftToRight && allign == TextAlign.Right)
            spos.X -= w;
        if (direction == TextDirection.RightToLeft && allign == TextAlign.Left)
            spos.X += w;
        TextDirection temp;
        List<int> indices = new List<int>();
        temp = chars[formattedText[0]].direction;
        char[] common = new char[] { ' ', '-', '،', '_', ':' };
        for (int i = 1; i < formattedText.Length; i++)
        {
            if (chars[formattedText[i]].direction != temp && formattedText[i] != ' ' && chars[formattedText[i]].direction != TextDirection.None)
            {
                temp = chars[formattedText[i]].direction;
                indices.Add(i);
            }
        }
        indices.Add(formattedText.Length);
        int prevIndex = 0;
        string txt = "";
        Vector2 pos = spos;
        foreach (int index in indices)
        {
            if (chars[formattedText[prevIndex]].direction == direction)
            {
                for (int i = prevIndex; i < index; i++)
                {
                    txt += formattedText[i];
                }
            }
            else
            {
                for (int i = index - 1; i >= prevIndex; i--)
                {
                    txt += formattedText[i];
                }
            }
            pos = layer.PushString(txt, font, pos, color, direction, size);
            prevIndex = index;
            txt = "";
        }
    }

    public static float GetStringWidth(string text, Font font, bool forcePersianDigits, float size)
    {
        if (text.Length == 0)
            return 0f;
        string formattedText = Format(text, forcePersianDigits);
        float w = 0;
        w = (float)font.GetWidth(formattedText, size);
        return w;
    }
}