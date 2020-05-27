namespace UAT_MS539.Core.Code.Cryptid
{
    public class ColorDefinition
    {
        public static readonly ColorDefinition Nil = new ColorDefinition("Nil", "Default", 255, 255, 255);

        public string ColorId;
        public string NameId;
        public byte R;
        public byte G;
        public byte B;

        public ColorDefinition(string colorId, string nameId, byte r, byte g, byte b)
        {
            ColorId = colorId;
            NameId = nameId;
            R = r;
            G = g;
            B = b;
        }
    }
}