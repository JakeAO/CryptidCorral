using NUnit.Framework;
using UAT_MS539.Code;

namespace UAT_MS539.Tests
{
    [TestFixture]
    public static class NumberEncoderTests
    {
        [TestCase(0, ExpectedResult = "0")]
        [TestCase(6, ExpectedResult = "110")]
        [TestCase(15, ExpectedResult = "1111")]
        [TestCase(100, ExpectedResult = "1100100")]
        [TestCase(500, ExpectedResult = "111110100")]
        [TestCase(1000, ExpectedResult = "1111101000")]
        [TestCase(123, ExpectedResult = "1111011")]
        [TestCase(543, ExpectedResult = "1000011111")]
        [TestCase(9080, ExpectedResult = "10001101111000")]
        public static string Test_Encode_Base2(long value) => NumberEncoder.Encode(value, NumberEncoder.Base2Encoding);
        
        [TestCase("0", ExpectedResult = 0)]
        [TestCase("110", ExpectedResult = 6)]
        [TestCase("1111", ExpectedResult = 15)]
        [TestCase("1100100", ExpectedResult = 100)]
        [TestCase("111110100", ExpectedResult = 500)]
        [TestCase("1111101000", ExpectedResult = 1000)]
        [TestCase("1111011", ExpectedResult = 123)]
        [TestCase("1000011111", ExpectedResult = 543)]
        [TestCase("10001101111000", ExpectedResult = 9080)]
        public static long Test_Decode_Base2(string value) => NumberEncoder.Decode(value, NumberEncoder.Base2Encoding);
        
        [TestCase(0, ExpectedResult = "0")]
        [TestCase(6, ExpectedResult = "6")]
        [TestCase(15, ExpectedResult = "17")]
        [TestCase(100, ExpectedResult = "144")]
        [TestCase(500, ExpectedResult = "764")]
        [TestCase(1000, ExpectedResult = "1750")]
        [TestCase(123, ExpectedResult = "173")]
        [TestCase(543, ExpectedResult = "1037")]
        [TestCase(9080, ExpectedResult = "21570")]
        public static string Test_Encode_Base8(long value) => NumberEncoder.Encode(value, NumberEncoder.Base8Encoding);
        
        [TestCase("0", ExpectedResult = 0)]
        [TestCase("6", ExpectedResult = 6)]
        [TestCase("17", ExpectedResult = 15)]
        [TestCase("144", ExpectedResult = 100)]
        [TestCase("764", ExpectedResult = 500)]
        [TestCase("1750", ExpectedResult = 1000)]
        [TestCase("173", ExpectedResult = 123)]
        [TestCase("1037", ExpectedResult = 543)]
        [TestCase("21570", ExpectedResult = 9080)]
        public static long Test_Decode_Base8(string value) => NumberEncoder.Decode(value, NumberEncoder.Base8Encoding);
        
        [TestCase(0, ExpectedResult = "0")]
        [TestCase(6, ExpectedResult = "6")]
        [TestCase(15, ExpectedResult = "15")]
        [TestCase(100, ExpectedResult = "100")]
        [TestCase(500, ExpectedResult = "500")]
        [TestCase(1000, ExpectedResult = "1000")]
        [TestCase(123, ExpectedResult = "123")]
        [TestCase(543, ExpectedResult = "543")]
        [TestCase(9080, ExpectedResult = "9080")]
        public static string Test_Encode_Base10(long value) => NumberEncoder.Encode(value, NumberEncoder.Base10Encoding);
        
        [TestCase("0", ExpectedResult = 0)]
        [TestCase("6", ExpectedResult = 6)]
        [TestCase("15", ExpectedResult = 15)]
        [TestCase("100", ExpectedResult = 100)]
        [TestCase("500", ExpectedResult = 500)]
        [TestCase("1000", ExpectedResult = 1000)]
        [TestCase("123", ExpectedResult = 123)]
        [TestCase("543", ExpectedResult = 543)]
        [TestCase("9080", ExpectedResult = 9080)]
        public static long Test_Decode_Base10(string value) => NumberEncoder.Decode(value, NumberEncoder.Base10Encoding);
        
        [TestCase(0, ExpectedResult = "0")]
        [TestCase(6, ExpectedResult = "6")]
        [TestCase(15, ExpectedResult = "F")]
        [TestCase(100, ExpectedResult = "64")]
        [TestCase(500, ExpectedResult = "1F4")]
        [TestCase(1000, ExpectedResult = "3E8")]
        [TestCase(123, ExpectedResult = "7B")]
        [TestCase(543, ExpectedResult = "21F")]
        [TestCase(9080, ExpectedResult = "2378")]
        public static string Test_Encode_Base16(long value) => NumberEncoder.Encode(value, NumberEncoder.Base16Encoding);
        
        [TestCase("0", ExpectedResult = 0)]
        [TestCase("6", ExpectedResult = 6)]
        [TestCase("F", ExpectedResult = 15)]
        [TestCase("64", ExpectedResult = 100)]
        [TestCase("1F4", ExpectedResult = 500)]
        [TestCase("3E8", ExpectedResult = 1000)]
        [TestCase("7B", ExpectedResult = 123)]
        [TestCase("21F", ExpectedResult = 543)]
        [TestCase("2378", ExpectedResult = 9080)]
        public static long Test_Decode_Base16(string value) => NumberEncoder.Decode(value, NumberEncoder.Base16Encoding);
        
        [TestCase(0, ExpectedResult = "0")]
        [TestCase(6, ExpectedResult = "6")]
        [TestCase(15, ExpectedResult = "F")]
        [TestCase(100, ExpectedResult = "44")]
        [TestCase(500, ExpectedResult = "KK")]
        [TestCase(1000, ExpectedResult = "1HG")]
        [TestCase(123, ExpectedResult = "53")]
        [TestCase(543, ExpectedResult = "MF")]
        [TestCase(9080, ExpectedResult = "FI8")]
        public static string Test_Encode_Base24(long value) => NumberEncoder.Encode(value, NumberEncoder.Base24Encoding);
        
        [TestCase("0", ExpectedResult = 0)]
        [TestCase("6", ExpectedResult = 6)]
        [TestCase("F", ExpectedResult = 15)]
        [TestCase("44", ExpectedResult = 100)]
        [TestCase("KK", ExpectedResult = 500)]
        [TestCase("1HG", ExpectedResult = 1000)]
        [TestCase("53", ExpectedResult = 123)]
        [TestCase("MF", ExpectedResult = 543)]
        [TestCase("FI8", ExpectedResult = 9080)]
        public static long Test_Decode_Base24(string value) => NumberEncoder.Decode(value, NumberEncoder.Base24Encoding);
    }
}