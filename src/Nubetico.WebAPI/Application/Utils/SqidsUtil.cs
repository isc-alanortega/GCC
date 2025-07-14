using Sqids;

namespace Nubetico.WebAPI.Application.Utils
{
    public static class SqidsUtil
    {
        private static string alphabet = "wAaj3tYVKUoiGIHu49rfmD107hOF8SNMpcCWv5lReJXsgB6bQzdny2qEPZxTLk";
        private static int sqidLength = 8;

        private static readonly SqidsEncoder<int> sqids = new(new SqidsOptions
        {
            Alphabet = alphabet,
            MinLength = sqidLength,
        });

        public static string Encode(int value)
        {
            return sqids.Encode(value);
        }

        public static int? Decode(string value)
        {
            var decoded = sqids.Decode(value);
            return decoded.Count > 0 ? decoded[0] : null;
        }
    }
}
