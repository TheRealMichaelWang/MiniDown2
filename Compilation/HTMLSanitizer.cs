using System.Text;

namespace MiniDown
{
    public static class HTMLSanitizer 
    {
        public static void AppendEscaped(this StringBuilder emitter, char c)
        {
            emitter.Append(c switch
            {
                '&' => "&amp;",
                '<' => "&lt;",
                '>' => "&gt;",
                '"' => "&quot;",
                '\'' => "&#39;",
                _ => c.ToString()
            });
        }

        public static void AppendEscaped(this StringBuilder emitter, string str)
        {
            foreach (char c in str)
                AppendEscaped(emitter, c);
        }
    }
}
