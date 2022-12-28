using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniDown.AST
{
    partial interface IAstElement
    {
        public void EmitHTML(StringBuilder emitter);
    }

    partial interface ITextBlockElement
    {
        public void EmitHTML(StringBuilder emitter);
    }

    partial class TextSnippet
    {
        public void EmitHTML(StringBuilder emitter) => emitter.AppendEscaped(Text);
    }

    partial class CodeSnippet
    {
        public void EmitHTML(StringBuilder emitter)
        {
            emitter.Append("<code>");
            emitter.AppendEscaped(Code);
            emitter.Append("</code>");
        }
    }

    partial class TextBlock
    {
        public virtual void EmitHTML(StringBuilder emitter) => Elements.ForEach((elem) => elem.EmitHTML(emitter));
    }

    partial class Paragraph
    {
        public override void EmitHTML(StringBuilder emitter)
        {
            emitter.Append("<p>");
            base.EmitHTML(emitter);
            emitter.AppendLine("</p>");
        }
    }

    partial class Heading
    {
        public override void EmitHTML(StringBuilder emitter)
        {
            emitter.Append($"<h{Level}>");
            base.EmitHTML(emitter);
            emitter.AppendLine($"</h{Level}>");
        }
    }

    partial class CodeBlock
    {
        public void EmitHTML(StringBuilder emitter)
        {
            emitter.AppendLine("<pre>");
            foreach(string line in Lines)
            {
                emitter.Append('\t');
                emitter.AppendEscaped(line);
                emitter.AppendLine();
            }
            emitter.AppendLine("</pre>");
        }
    }

    partial class BulletedList
    {
        partial class BulletedItem
        {
            public override void EmitHTML(StringBuilder emitter)
            {
                emitter.Append("\t<li>");
                base.EmitHTML(emitter);
                emitter.AppendLine("</li>");
            }
        }

        public void EmitHTML(StringBuilder emitter)
        {
            emitter.AppendLine("<ul>");
            Items.ForEach((item) => item.EmitHTML(emitter));
            emitter.AppendLine("</ul>");
        }
    }
}
