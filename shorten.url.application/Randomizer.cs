using System.Security.Cryptography;

namespace shorten.url.application
{
    public static class Randomizer
    {
        const int DEFAULT_MIN_RANDOM_LENGTH = 4;

        const int DEFAULT_MAX_RANDOM_LENGTH = 5;

        const string RANDOM_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";

        const string RANDOM_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";

        const string RANDOM_CHARS_NUMERIC = "0123456789";

        const string RANDOM_CHARS_SPECIAL = "*@$_&!";

        public static string Generate()
        {
            return Generate(DEFAULT_MIN_RANDOM_LENGTH, DEFAULT_MAX_RANDOM_LENGTH);
        }

        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        public static string GenerateNumbersOnly(int lenght)
        {
            return Generate(lenght, lenght, false, false, true, false);
        }
        public static string Generate(int minLength, int maxLength, bool useUCase = true, bool useLCase = true, bool useNumeric = true, bool useSpecialChars = true)
        {
            //  Make sure that input parameters are valid.
            if (((minLength <= 0) || ((maxLength <= 0) || (minLength > maxLength))))
            {
                return "";
            }

            var charGroups = new List<char[]>();
            if (useUCase)
            {
                charGroups.Add(RANDOM_CHARS_UCASE.ToCharArray());
            }
            if (useLCase)
            {
                charGroups.Add(RANDOM_CHARS_LCASE.ToCharArray());
            }
            if (useNumeric)
            {
                charGroups.Add(RANDOM_CHARS_NUMERIC.ToCharArray());
            }
            if (useSpecialChars)
            {
                charGroups.Add(RANDOM_CHARS_SPECIAL.ToCharArray());
            }


            //  Use this array to track the number of unused characters in each
            //  character group.
            int[] charsLeftInGroup = new int[charGroups.Count];

            // Initially, all characters in each group are not used.
            int i;
            for (i = 0; i <= charsLeftInGroup.Length - 1; i++)
            {
                charsLeftInGroup[i] = charGroups[i].Length;
            }

            //  Use this array to track (iterate through) unused character groups.
            int[] leftGroupsOrder = new int[charGroups.Count];
            for (i = 0; i <= leftGroupsOrder.Length - 1; i++)
            {
                leftGroupsOrder[i] = i;
            }

            //  Because we cannot use the default randomizer, which is based on the
            //  current time (it will produce the same "random" number within a
            //  second), we will use a random number generator to seed the
            //  randomizer.

            //  Use a 4-byte array to fill it with random bytes and convert it then
            //  to an integer value.
            byte[] randomBytes = new byte[4];

            // Generate 4 random bytes.
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            //  Convert 4 bytes into a 32-bit integer value.
            int seed = ((randomBytes[0] & 127) << 24) | randomBytes[1] << 16 | randomBytes[2] << 8 | randomBytes[3];

            // Now, this is real randomization.
            Random random = new Random(seed);
            //  This array will hold random characters.
            char[] password = null;
            if (minLength < maxLength)
            {
                password = new char[random.Next(minLength, maxLength + 1)];
            }
            else
            {
                password = new char[maxLength];
            }

            //  Index of the next character to be added to password.
            int nextCharIdx;

            //  Index of the next character group to be processed.
            int nextGroupIdx;

            //  Index which will be used to track not processed character groups.
            int nextLeftGroupsOrderIdx;

            //  Index of the last non-processed character in a group.
            int lastCharIdx;

            //  Index of the last non-processed group.
            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            //  Generate password characters one at a time.
            for (i = 0; i <= password.Length - 1; i++)
            {
                //  If only one character group remained unprocessed, process it;
                //  otherwise, pick a random character group from the unprocessed
                //  group list. To allow a special character to appear in the
                //  first position, increment the second parameter of the Next
                //  function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                if (lastLeftGroupsOrderIdx == 0)
                {
                    nextLeftGroupsOrderIdx = 0;
                }
                else
                {
                    nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);
                }

                //  Get the actual index of the character group, from which we will
                //  pick the next character.
                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                //  Get the index of the last unprocessed characters in this group.
                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                //  If only one unprocessed character is left, pick it; otherwise,
                //  get a random character from the unused character list.
                if (lastCharIdx == 0)
                {
                    nextCharIdx = 0;
                }
                else
                {
                    nextCharIdx = random.Next(0, lastCharIdx + 1);
                }

                //  Add this character to the password.
                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                //  If we processed the last character in this group, start over.
                if (lastCharIdx == 0)
                {
                    charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    //  There are more unprocessed characters left.
                }
                else
                {
                    //  Swap processed character with the last unprocessed character
                    //  so that we don't pick it until we process all characters in
                    //  this group.
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] = charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }

                    //  Decrement the number of unprocessed characters in
                    //  this group.
                    charsLeftInGroup[nextGroupIdx] = charsLeftInGroup[nextGroupIdx] - 1;
                }

                //  If we processed the last group, start all over.
                if ((lastLeftGroupsOrderIdx == 0))
                {
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    //  There are more unprocessed groups left.
                }
                else
                {
                    //  Swap processed group with the last unprocessed group
                    //  so that we don't pick it until we process all groups.
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] = leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }

                    //  Decrement the number of unprocessed groups.
                    lastLeftGroupsOrderIdx = lastLeftGroupsOrderIdx - 1;
                }
            }

            //  Convert password characters into a string and return the result.
            return new string(password);
        }

        public static string GenerateUniqueUrl()
        {
            return Generate(DEFAULT_MIN_RANDOM_LENGTH, DEFAULT_MAX_RANDOM_LENGTH, true, true, false, false);
        }

        public static string GenerateApiKey(string prefix = "")
        {
            var bytes = new byte[128];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            string base64String = Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_");

            var keyLength = 128 - prefix.Length;

            return prefix + base64String[..keyLength];
        }
    }
}
