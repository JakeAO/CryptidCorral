namespace UAT_MS539.Core.Code.Utility
{
    public readonly struct RunePattern
    {
        public readonly string FilePath;
        public readonly string Md5;
        public readonly string RunicHash;
        public readonly uint[] RuneValues;

        public RunePattern(string filePath, string md5, string runicHash)
        {
            FilePath = filePath;
            Md5 = md5;
            RunicHash = runicHash;
            RuneValues = new uint[runicHash.Length];
            for (int i = 0; i < RuneValues.Length; i++)
            {
                RuneValues[i] = (uint)NumberEncoder.Decode(RunicHash[i].ToString(), NumberEncoder.Base24Encoding);
            }
        }
    }
}
