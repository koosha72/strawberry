namespace Strawberry.Graphics.Text;

public class Font
{
    public byte Size { get; private set; }

    public Dictionary<ushort, Character> Characters { get; private set; }

    public ITexture Texture { get; private set; }

    public byte Minor { get; private set; }

    public byte Major { get; private set; }

    public bool UseSDF { get; private set; }

    public Font(IGraphicsContext graphicsContext, byte[] data)
    {
        MemoryStream mem = new MemoryStream(data);
        BinaryReader reader = new BinaryReader(mem);

        string[] ver = reader.ReadString().Split('_');

        string minor = ver[1];

        string major = ver[0];
        major = major.Replace("FONTV", "");
        byte temp;
        if (!byte.TryParse(major, out temp))
        {
            throw new Exception("Cannot load the font, the font file may not be valid");
        }

        Major = temp;

        if (!byte.TryParse(minor, out temp))
        {
            throw new Exception("Cannot load the font, the font file may not be valid");
        }

        Minor = temp;

        if (Major > 0)
        {
            throw new Exception("The font file is not valid or it's version is newer");
        }
        if (Minor == 1)
            UseSDF = false;
        else
            UseSDF = reader.ReadBoolean();
        byte ratio = 1;
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int charsCount = reader.ReadInt32();
        Size = reader.ReadByte();
        if (Major == 0 && Minor == 2)
            ratio = reader.ReadByte();
        width /= ratio;
        height /= ratio;
        Size /= ratio;
        Characters = new Dictionary<ushort, Character>();
        for (int i = 0; i < charsCount; i++)
        {
            ushort index = reader.ReadUInt16();
            Character chr;
            chr.Adwidth = reader.ReadDouble() / ratio;
            chr.Adheight = reader.ReadDouble() / ratio;
            chr.Left = reader.ReadDouble() / ratio;
            chr.Top = reader.ReadDouble() / ratio;
            chr.Right = reader.ReadDouble() / ratio;
            chr.Bottom = reader.ReadDouble() / ratio;
            Characters.Add(index, chr);
        }

        byte[] bytes = reader.ReadBytes(width * height);
        Color[] colors = new Color[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            colors[i] = new Color(bytes[i], bytes[i], bytes[i], bytes[i]);
        }

        reader.Close();
        mem.Dispose();

        Texture = graphicsContext.CreateTexture(width, height, colors);
    }

    public Character GetCharacterInfo(ushort chr)
    {
        if (!Characters.ContainsKey(chr))
        {
            if (!Characters.ContainsKey('?'))
            {
                return Characters[' '];
            }
            return Characters['?'];
        }
        return Characters[chr];
    }

    public double GetWidth(string text)
    {
        double w = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (Characters.ContainsKey(text[i]))
                w += Characters[text[i]].Adwidth;
            else
            {
                if (Characters.ContainsKey('?'))
                    w += Characters['?'].Adwidth;
            }
        }
        return w;
    }

    public double GetWidth(string text, float size)
    {
        float m = size / (float)this.Size;
        double w = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (Characters.ContainsKey(text[i]))
                w += Characters[text[i]].Adwidth * m;
            else
            {
                if (Characters.ContainsKey('?'))
                    w += Characters['?'].Adwidth * m;
            }
        }
        return w;
    }
}
