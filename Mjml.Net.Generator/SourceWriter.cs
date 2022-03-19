using System.Text;

namespace Mjml.Net.Generator
{
    public class SourceWriter
    {
        private readonly StringBuilder sb = new StringBuilder();
        private int indent;

        public SourceWriter MoveIn()
        {
            indent++;

            return this;
        }

        public SourceWriter MoveOut()
        {
            indent--;

            return this;
        }

        public SourceWriter AppendLine(string line = "")
        {
            for (var i = 0; i < indent * 4; i++)
            {
                sb.Append(' ');
            }

            sb.AppendLine(line);

            return this;
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
