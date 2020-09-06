namespace EpsServer.Services
{
    using System.Collections.Generic;
    using System.IO;

    public class CodeGenerator
    {
        public CodeGenerator()
        {

        }

        public HashSet<string> Generate(int count, int length)
        {
            var strings = new HashSet<string>();

            for (var i = 0; i < count; i++)
            {
                while (!strings.Add(this.Get8CharacterRandomString(length))) { }
            }

            return strings;
        }

        private string Get8CharacterRandomString(int length)
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path.Substring(0, length);  // Return 8 character string
        }
    }
}
