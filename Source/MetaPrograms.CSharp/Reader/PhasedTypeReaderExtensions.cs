using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.Members;

namespace MetaPrograms.CSharp.Reader
{
    public static class PhasedTypeReaderExtensions
    {
        public static TypeMember ReadAll(this IPhasedTypeReader reader)
        {
            RunAllPhases(new List<IPhasedTypeReader> { reader });
            return reader.TypeMember;
        }

        public static void RunAllPhases(this List<IPhasedTypeReader> readers)
        {
            readers.ForEach(t => t.ReadName());
            readers.ForEach(t => t.RegisterProxy());
            readers.ForEach(t => t.ReadGenerics());
            readers.ForEach(t => t.ReadAncestors());
            readers.ForEach(t => t.ReadMemberDeclarations());
            readers.ForEach(t => t.ReadAttributes());
            readers.ForEach(t => t.ReadMemberImplementations());
            readers.ForEach(t => t.RegisterReal());
        }
    }
}
