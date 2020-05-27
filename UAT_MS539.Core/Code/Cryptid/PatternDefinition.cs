namespace UAT_MS539.Core.Code.Cryptid
{
    public class PatternDefinition
    {
        public static readonly PatternDefinition Nil = new PatternDefinition("Nil", "Default", string.Empty);

        public string PatternId;
        public string NameId;
        public string ArtId;

        public PatternDefinition(string patternId, string nameId, string artId)
        {
            PatternId = patternId;
            NameId = nameId;
            ArtId = artId;
        }
    }
}