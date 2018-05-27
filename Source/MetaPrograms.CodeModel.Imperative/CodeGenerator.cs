using System;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative
{
    public static class CodeGenerator
    {
        public static CodeGeneratorSession NewSession()
        {
            return new CodeGeneratorSession();
        }
        
        public static void USE(ImmutableCodeModel codeModel) { }
        public static ImmutableCodeModel CodeModel => null;

        public static void NAMESPACE(string name, Action body) { }

        public static void ATTRIBUTE<T>(params object[] constructorArgumentsAndBody) { }
        public static void ATTRIBUTE(TypeMember type, params object[] constructorArgumentsAndBody) { }

        public static void EXTENDS<T>() { }
        public static void EXTENDS(TypeMember type) { }

        public static ModifierContext1 PUBLIC => null;
        public static ModifierContext1 PRIVATE => null;
        public static ModifierContext1 PROTECTED => null;
        public static ModifierContext1 INTERNAL => null;

        public static void PARAMETER<T>(string name, out MethodParameter @ref, Action body = null)
        {
            @ref = null;
        }

        public static void PARAMETER(TypeMember type, string name, out MethodParameter @ref, Action body = null)
        {
            @ref = null;
        }

        public static void LOCAL(TypeMember type, string name, out LocalVariable @ref)
        {
            @ref = null;
        }

        public static void LOCAL<T>(string name, out LocalVariable @ref)
        {
            @ref = null;
        }

        public static void AUTOMATIC() { }
        public static void AUTOMATIC(FieldMember field) { }
        
        public static void ARGUMENT(AbstractExpression value) { }

        public static AbstractExpression AWAIT(AbstractExpression promiseExpression) => null;
        public static StatementContext DO => null;
        public static ThisExpression THIS => null;
        
        public static AbstractExpression DOT(this AbstractExpression target, AbstractMember member) => null;
        public static AbstractExpression DOT(this AbstractExpression target, string memberName) => null;
        public static AbstractExpression DOT(this LocalVariable target, AbstractMember member) => null;
        public static AbstractExpression DOT(this LocalVariable target, string memberName) => null;
        public static AbstractExpression DOT(this MethodParameter target, AbstractMember member) => null;
        public static AbstractExpression DOT(this MethodParameter target, string memberName) => null;

        public static AbstractExpression NOT(AbstractExpression value) => null;
        public static AbstractExpression NEW<T>(params object[] constructorArguments) => null;
        public static AbstractExpression NEW(TypeMember type, params object[] constructorArguments) => null;
        
        public static AbstractExpression ASSIGN(this AbstractExpression target, AbstractExpression value) => null;
        public static AbstractExpression ASSIGN(this AbstractMember target, AbstractExpression value) => null;
        public static AbstractExpression ASSIGN(this IAssignable target, AbstractExpression value) => null;

        public static AbstractExpression INVOKE(this AbstractExpression target, params AbstractExpression[] arguments) => null;
        public static AbstractExpression INVOKE(this AbstractExpression target, Action body) => null;

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
            public void FIELD(TypeMember type, string name, out FieldMember @ref, Action body = null)
            {
                @ref = null;
            }

            public void FIELD<TType>(string name, out FieldMember @ref, Action body = null)
            {
                @ref = null;
            }

            public TypeMember CLASS(string name, Action body) => null;
            public TypeMember STRUCT(string name, Action body) => null;
            public TypeMember INTERFACE(string name, Action body) => null;
            
            public void CONSTRUCTOR(Action body) { }
            
            public void FUNCTION<TReturnType>(string name, Action body) { }
            public void FUNCTION(TypeMember returnType, string name) { }

            public void VOID(string name, Action body) { }
            public void VOID(MethodMember ancestorMethod, Action body) { }

            public PropertyContext PROPERTY<T>(string name, Action body = null) => null;
            public PropertyContext PROPERTY(TypeMember type, string name, Action body = null) => null;
        }


        public class StatementContext
        {
            public void RETURN(AbstractExpression value) { }
            public IfContext IF(AbstractExpression condition) => null;
        }

        public class PropertyContext
        {
            
        }

        public class IfContext
        {
            public ElseContext THEN(Action body) => null;
        }

        public class ElseContext
        {
            public ElseContext ELSEIF(AbstractExpression condition) => null;
        }
    }
}