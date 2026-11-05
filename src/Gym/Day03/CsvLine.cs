using System.Text;

namespace Gym.Day03
{
    public static class CsvLine
    {
        public static List<string> Split(string line)
        {
            ArgumentNullException.ThrowIfNull(line);

            var values = new List<string>();
            int start = 0;

            for(int i = 0; i < line.Length; i++)
            {
                if(line[i] == ',')
                {
                    var field = line.Substring(start, i - start);
                    values.Add(field.Trim());
                    start = i+1;
                }
            }

            values.Add(line.Substring(start, line.Length - start));
            
            return values;
        }

        private enum State { FieldStart, Unquoted, Quoted, AfterQuote }

        public static List<string> SplitQuoted(string line)
        {
            ArgumentNullException.ThrowIfNull(line);

            var fields = new List<string>();
            var field = new StringBuilder();
            var state = State.FieldStart;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                switch (state)
                {
                    case State.FieldStart:
                        if (c == '"') state = State.Quoted;
                        else if (c == ',') fields.Add("");          // empty field, stay in FieldStart
                        else { field.Append(c); state = State.Unquoted; }
                        break;

                    case State.Unquoted:
                        if (c == ',') { fields.Add(field.ToString()); field.Clear(); state = State.FieldStart; }
                        else field.Append(c);                        // a " here is just a character
                        break;

                    case State.Quoted:
                        if (c == '"')
                        {
                            if (i + 1 < line.Length && line[i + 1] == '"')
                            {
                                field.Append('"');                   // "" means one literal "
                                i++;                                 // skip the second quote
                            }
                            else state = State.AfterQuote;           // closing quote
                        }
                        else field.Append(c);                        // commas are plain chars here
                        break;

                    case State.AfterQuote:
                        if (c == ',') { fields.Add(field.ToString()); field.Clear(); state = State.FieldStart; }
                        else throw new FormatException($"Unexpected '{c}' after closing quote at position {i}.");
                        break;
                }
            }

            if (state == State.Quoted)
                throw new FormatException("Quoted field is never closed.");

            fields.Add(field.ToString());                            // the last field, same as Split
            return fields;
        }
    }
}