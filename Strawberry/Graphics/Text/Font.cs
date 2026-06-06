/*
 * Strawberry Game Engine
 * File: Font.cs
 * Author: Koosha Aabedini Nassab
 *
 * Font resource and glyph atlas used by the text renderer.
 */


namespace Strawberry.Graphics.Text;

/// <summary>
/// Font resource and glyph atlas used by the text renderer.
/// </summary>
public class Font
{
    /// <summary>
    /// Gets the native size of the font
    /// </summary>
    public byte Size { get; private set; }
    /// <summary>
    /// Gets the character map of the font
    /// </summary>
    public Dictionary<ushort, Character> Characters { get; private set; }
    /// <summary>
    /// Gets the texture atlas of the font
    /// </summary>
    public Texture Texture { get; private set; }
    /// <summary>
    /// Gets the minor version of the font
    /// </summary>
    public byte Minor { get; private set; }
    /// <summary>
    /// Gets the major version of the font file
    /// </summary>
    public byte Major { get; private set; }
    /// <summary>
    /// Gets whether this font uses signed-distance field rendering or not
    /// </summary>
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

    /// <summary>
    /// Get character info
    /// </summary>
    /// <param name="chr">Code of the requested character</param>
    /// <returns></returns>
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

    /// <summary>
    /// Measures the width of the given text using font's native size
    /// </summary>
    /// <param name="text">The text to be measured</param>
    /// <returns>The width of the given string in pixels</returns>
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

    /// <summary>
    /// Measures the width of the given text using the specified size
    /// </summary>
    /// <param name="text">The text to be measured</param>
    /// <param name="size">Size of the font</param>
    /// <returns>The width of the given string in pixels</returns>
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
