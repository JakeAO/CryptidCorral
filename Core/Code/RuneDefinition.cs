namespace Core.Code
{
    public class RuneDefinition
    {
        public uint RuneId;
        public string NameId;
        public string ArtId;

        public RuneDefinition(uint runeId, string nameId, string artId)
        {
            RuneId = runeId;
            NameId = nameId;
            ArtId = artId;
        }
    }
}