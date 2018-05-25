using System;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Members;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative
{
    public static class CodeGenerator
    {
        public static void USE(ImmutableCodeModel codeModel) { }
        
        public static object NAMESPACE(string name, Action body) => null;

        public static StatementContext DO => null;

        public static ModifierContext1 PUBLIC => null;
        public static ModifierContext1 PRIVATE => null;
        public static ModifierContext1 PROTECTED => null;
        public static ModifierContext1 INTERNAL => null;

        public class ModifierContext1 : ModifierContext2
        {
            public ModifierContext2 STATIC => null;
            public ModifierContext2 ABSTRACT => null;
            public ModifierContext2 VIRTUAL => null;
            public ModifierContext2 OVERRIDE => null;
        }

        public class ModifierContext2 : MemberContext
        {
            public MemberContext ASYNC => null;
            public MemberContext READONLY => null;
        }
        
        public class MemberContext
        {
            public void FIELD(TypeMember type, string name, Action body = null) { }
            public void FIELD<TType>(string name, Action body = null) { }
            
            public void CLASS(string name, Action body) { }
            public void STRUCT(string name, Action body) { }
            public void INTERFACE(string name, Action body) { }
            
            public void CONSTRUCTOR(Action body) { }
            
            public void FUNCTION<TReturnType>(string name, Action body) { }
            public void FUNCTION(TypeMember returnType, string name) { }
        }


        public class StatementContext
        {
            
        }
    }
}