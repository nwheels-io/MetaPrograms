using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;

namespace MetaPrograms.Adapters.Roslyn.Reader
{
    public static class MethodReaderMechanism
    {
        public static MethodSignature ReadSignature(CodeModelBuilder modelBuilder, IMethodSymbol symbol)
        {
            var parameters = symbol.Parameters.Select((p, index) => new MethodParameter(
                name: p.Name,
                position: index + 1,
                type: modelBuilder.TryGetMember<TypeMember>(p.Type),
                modifier: p.GetParameterModifier(),
                attributes: ImmutableList<AttributeDescription>.Empty
            ));

            var hasReturnType = (
                symbol.MethodKind != MethodKind.Constructor &&
                symbol.MethodKind != MethodKind.StaticConstructor &&
                !symbol.ReturnsVoid);
            
            var returnValue = (
                hasReturnType 
                ? new MethodParameter(
                    name: "$retVal", 
                    position: 0,
                    type: modelBuilder.TryGetMember<TypeMember>(symbol.ReturnType),
                    modifier: symbol.GetReturnValueModifier(),
                    attributes: ImmutableList<AttributeDescription>.Empty)
                : null);

            return new MethodSignature(symbol.IsAsync, returnValue, parameters.ToImmutableList());
        }

        public static MethodReader CreateAccessorMethodReader(CodeModelBuilder modelBuilder, IMethodSymbol accessorSymbol)
        {
            if (accessorSymbol != null)
            {
                var reader = new MethodReader(modelBuilder, accessorSymbol);
                reader.ReadDeclaration();
                return reader;
            }

            return null;
        }

    }
}