using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.JavaScript.Writer
{
    public static class JavaScriptMemberWriter
    {
        public static void WriteMember(CodeTextBuilder code, AbstractMember member)
        {
            WriteExportModifiers(code, member);

            if (member is TypeMember type && type.TypeKind == TypeMemberKind.Class)
            {
                JavaScriptClassWriter.WriteClass(code, type);
            }
            else if (member is MethodMember method)
            {
                JavaScriptFunctionWriter.WriteFunction(code, method);
            }
            else if (member is FieldMember field)
            {
                //WriteField(field);
            }
        }

        private static void WriteExportModifiers(CodeTextBuilder code, AbstractMember member)
        {
            if (member.Visibility == MemberVisibility.Public && member.DeclaringType == null)
            {
                code.Write("export ");
            }

            if (member.IsDefaultExport)
            {
                code.Write("default ");
            }
        }
    }
}
