using System;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.Fluent.Generator;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public class FluentAsyncLambda
    {
        public AnonymousDelegateExpression LAMBDA(Action bodyNoArgs)
            => PushExpression(CreateAnonymousDelegate(
                bodyNoArgs,
                parameters => bodyNoArgs?.Invoke(),
                isAsync: true));

        public AnonymousDelegateExpression LAMBDA(Action<MethodParameter> body1Arg)
            => CreateAnonymousDelegate(
                body1Arg,
                parameters => body1Arg?.Invoke(parameters[0]),
                isAsync: true);

        public AnonymousDelegateExpression LAMBDA(Action<MethodParameter, MethodParameter> body2Args)
            => CreateAnonymousDelegate(
                body2Args,
                parameters => body2Args?.Invoke(parameters[0], parameters[1]),
                isAsync: true);

        public AnonymousDelegateExpression LAMBDA(Action<MethodParameter, MethodParameter, MethodParameter> body3Args)
            => CreateAnonymousDelegate(
                body3Args,
                parameters => body3Args?.Invoke(parameters[0], parameters[1], parameters[2]),
                isAsync: true);
    }
}
