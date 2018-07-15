using System.IO;
using System.Text;
using System.Xml;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public class JsxTextWriter : XmlTextWriter
    {
        public JsxTextWriter(Stream w, Encoding encoding)
            : base(w, encoding)
        {
        }

        public JsxTextWriter(TextWriter w)
            : base(w)
        {
        }

        public JsxTextWriter(string filename, Encoding encoding)
            : base(filename, encoding)
        {
        }

        public override void WriteAttributes(XmlReader reader, bool defattr)
        {
            //TODO: implement syntax of JSX expression attributes
            base.WriteAttributes(reader, defattr);
        }
    }
}