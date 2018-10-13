﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms;
using MetaPrograms.Extensions;
using MetaPrograms.Members;

namespace MetaPrograms.JavaScript.Writer
{
    public static class JavaScriptClassWriter
    {
        public static void WriteClass(CodeTextBuilder code, TypeMember type)
        {
            code.WriteLine($"class {type.Name} {{");
            code.Indent(1);

            type.Members.ForEach((member, index) => {
                if (index > 0)
                {
                    code.WriteLine();
                }

                JavaScriptMemberWriter.WriteMember(code, member);
            });

            code.Indent(-1);
            code.WriteLine("}");
        }
    }
}
