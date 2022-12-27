using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniDown.AST
{
    public interface IAstElement
    {

    }

    public sealed partial class Heading : TextBlock, IAstElement
    {
        public int Level { get; private set; }

        public Heading(int level, List<ITextBlockElement> elements) : base(elements)
        {
            Level = level;
        }
    }

    public sealed partial class BulletedList : IAstElement
    {
        public sealed partial class BulletedItem : TextBlock
        {
            public BulletedItem(List<ITextBlockElement> elements) : base(elements)
            {

            }
        }

        public readonly List<BulletedItem> Items;

        public BulletedList(List<BulletedItem> items)
        {
            Debug.Assert(items.Count > 0);

            Items = items;
        }
    }

    public sealed partial class CodeBlock : IAstElement
    {
        public readonly List<string> Lines;

        public CodeBlock(List<string> lines)
        {
            Debug.Assert(lines.TrueForAll((line) => !line.Contains('\n')));

            Lines = lines;
        }
    }

    public sealed partial class Paragraph : TextBlock, IAstElement
    {
        public Paragraph(List<ITextBlockElement> elements) : base(elements)
        {

        }
    }
}