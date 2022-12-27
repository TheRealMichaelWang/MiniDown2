using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniDown.AST
{
    public interface ITextBlockElement
    {

    }

    public sealed partial class TextSnippet : ITextBlockElement
    {
        public readonly string Text;

        public TextSnippet(string text)
        {
            Debug.Assert(!text.Contains('\n'));
            
            Text = text;
        }
    }

    public sealed partial class CodeSnippet : ITextBlockElement
    {
        public readonly string Code;

        public CodeSnippet(string code)
        {
            Debug.Assert(!code.Contains('\n'));

            Code = code;
        }
    }

    public partial class TextBlock
    {
        public readonly List<ITextBlockElement> Elements;

        public TextBlock(List<ITextBlockElement> elements)
        {
            Elements = elements;
        }
    }
}
