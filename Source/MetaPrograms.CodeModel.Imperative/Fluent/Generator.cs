using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;
using static MetaPrograms.CodeModel.Imperative.CodeGeneratorContext;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public static class Generator
    {
        public static void NAMESPACE(string name, Action body)
        {
            using (GetContextOrThrow().PushState(new NamespaceContext(name)))
            {
                body();
            }
        }

        public static ModuleMember MODULE(string name, Action body)
        {
            return MODULE(folderPath: null, name, body);
        }

        public static ModuleMember MODULE(string[] folderPath, string name, Action body)
        {
            var context = GetContextOrThrow();
            var module = new ModuleMember() {
                FolderPath = folderPath ?? new string[0],
                Name = name,
                Status = MemberStatus.Incomplete,
                Visibility = MemberVisibility.Public
            };

            using (context.PushState(module))
            {
                body?.Invoke();
            }

            module.Status = MemberStatus.Generator;
            return module;
        }

        public static IFluentImport IMPORT => new FluentImport();

        public static FluentVisibility PUBLIC => new FluentVisibility(MemberVisibility.Public);
        public static FluentVisibility EXPORT => new FluentVisibility(MemberVisibility.Public);
        public static FluentVisibility PRIVATE => new FluentVisibility(MemberVisibility.Private);
        public static FluentVisibility PROTECTED => new FluentVisibility(MemberVisibility.Protected);
        public static FluentVisibility INTERNAL => new FluentVisibility(MemberVisibility.Internal);

        public static void ATTRIBUTE<T>(params object[] constructorArgumentsAndBody)
        {
            ATTRIBUTE(GetContextOrThrow().FindType<T>(), constructorArgumentsAndBody);
        }

        public static void ATTRIBUTE(TypeMember type, params object[] constructorArgumentsAndBody)
        {
            var context = GetContextOrThrow();
            var attribute = FluentHelpers.BuildAttribute(context, type, constructorArgumentsAndBody);

            if (context.TryPeekState<ParameterContext>(out var parameterContext))
            {
                parameterContext.Parameter.Attributes.Add(attribute);
            }
            else
            {
                var member = context.GetCurrentMember();
                member.Attributes.Add(attribute);
            }
        }

        public static void NAMED(string name, object value)
        {
            var context = GetContextOrThrow();

            context
                .PeekStateOrThrow<AttributeContext>().Attribute
                .PropertyValues.Add(new NamedPropertyValue(
                    name, 
                    context.GetConstantExpression(value)));
        }

        public static void EXTENDS<T>()
        {
            EXTENDS(GetContextOrThrow().FindType<T>());
        }

        public static void EXTENDS(TypeMember type)
        {
            var descendantType = GetContextOrThrow().PeekStateOrThrow<TypeMember>();
            descendantType.BaseType = type;
        }

        public static void IMPLEMENTS<T>()
        {
            IMPLEMENTS(GetContextOrThrow().FindType<T>());
        }

        public static void IMPLEMENTS(TypeMember type)
        {
            var descendantType = GetContextOrThrow().PeekStateOrThrow<TypeMember>();
            descendantType.Interfaces.Add(type);
        }

        public static void PARAMETER<T>(string name, out MethodParameter @ref, Action body = null)
        {
            PARAMETER(GetContextOrThrow().FindType<T>(), name, out @ref, body);
        }

        public static void PARAMETER(string name, out MethodParameter @ref, Action body = null)
        {
            PARAMETER(type: null, name, out @ref, body);
        }

        public static void PARAMETER(TypeMember type, string name, out MethodParameter @ref, Action body = null)
        {
            var context = GetContextOrThrow();
            var method = (MethodMemberBase)context.GetCurrentMember();

            var newParameter = new MethodParameter {
                Name = name,
                Position = method.Signature.Parameters.Count,
                Type = type
            }; 
            
            var parameterContext = new ParameterContext(newParameter);

            using (context.PushState(parameterContext))
            {
                body?.Invoke();
            }

            method.Signature.Parameters.Add(newParameter);
            @ref = newParameter;
        }

        public static void LOCAL(string name, out LocalVariable @ref, AbstractExpression initialValue = null)
        {
            LOCAL(type: null, name, out @ref, initialValue);
        }

        public static void LOCAL(TypeMember type, string name, out LocalVariable @ref, AbstractExpression initialValue = null)
        {
            @ref = new LocalVariable {
                Name = name,
                Type = type
            };
            
            var block = BlockContext.GetBlockOrThrow();
            block.AddLocal(@ref);
            block.AppendStatement(new VariableDeclarationStatement {
                Variable = @ref,
                InitialValue = initialValue
            });
        }

        public static void LOCAL<T>(string name, out LocalVariable @ref, AbstractExpression initialValue = null)
        {
            var type = GetContextOrThrow().FindType<T>();
            LOCAL(type, name, out @ref, initialValue);
        }
        
        public static void FINAL(string name, out LocalVariable @ref, AbstractExpression value)
        {
            FINAL(type: null, name, out @ref, value);
        }

        public static void FINAL(TypeMember type, string name, out LocalVariable @ref, AbstractExpression value)
        {
            @ref = new LocalVariable {
                Name = name,
                Type = type,
                IsFinal = true
            };

            var block = BlockContext.GetBlockOrThrow();
            block.AddLocal(@ref);
            block.AppendStatement(new VariableDeclarationStatement {
                Variable = @ref, 
                InitialValue = value
            });
        }

        public static void FINAL<T>(string name, out LocalVariable @ref, AbstractExpression value)
        {
            var type = GetContextOrThrow().FindType<T>();
            FINAL(type, name, out @ref, value);
        }

        public static LocalVariableExpression USE(string name)
        {
            return new LocalVariableExpression {
                VariableName = name
            };
        }

        public static TupleExpression TUPLE(string name1, out LocalVariable var1)
        {
            var tuple = MakeTuple(new[] { name1 }, types: null, out var variables);
            var1 = variables[0];
            return tuple;
        }

        public static TupleExpression TUPLE(string name1, TypeMember type1, out LocalVariable var1)
        {
            var tuple = MakeTuple(
                new[] { name1 }, 
                types: new[] { type1 }, 
                out var variables);
            
            var1 = variables[0];
            return tuple;
        }

        public static void GET() => throw new NotImplementedException();
        public static void GET(Action body) => throw new NotImplementedException();
        public static void SET(Action<LocalVariable> body) => throw new NotImplementedException();

        public static void ARGUMENT(AbstractExpression value) 
            => GetContextOrThrow().PeekStateOrThrow<InvocationContext>().AddArgument(value);

        public static void ARGUMENT_BYREF(AbstractExpression value)
            => GetContextOrThrow().PeekStateOrThrow<InvocationContext>().AddArgument(value, MethodParameterModifier.Ref);

        public static void ARGUMENT_OUT(AbstractExpression value)
            => GetContextOrThrow().PeekStateOrThrow<InvocationContext>().AddArgument(value, MethodParameterModifier.Out);

        public static AbstractExpression AWAIT(AbstractExpression promiseExpression)
            => new AwaitExpression {
                Expression = promiseExpression,
                Type = promiseExpression.Type
            };

        public static FluentStatement DO 
            => new FluentStatement();

        public static ThisExpression THIS 
            => new ThisExpression {
                Type = GetContextOrThrow().GetCurrentType()
            };

        public static AbstractExpression DOT(this AbstractExpression target, AbstractMember member)
            => new MemberExpression {
                Target = target,
                Member = member
            };

        public static AbstractExpression DOT(this AbstractExpression target, string memberName)
            => new MemberExpression {
                Target = target,
                MemberName = memberName
            };

        public static AbstractExpression DOT(this LocalVariable target, AbstractMember member) => throw new NotImplementedException();
        
        public static AbstractExpression DOT(this LocalVariable target, string memberName) 
            => new MemberExpression {
                Type = target.Type,
                Target = target.AsExpression(),
                MemberName = memberName
            };

        
        public static AbstractExpression DOT(this MethodParameter target, AbstractMember member) => throw new NotImplementedException();

        public static AbstractExpression DOT(this MethodParameter target, string memberName)
            => new MemberExpression {
                Type = target.Type,
                Target = target.AsExpression(),
                MemberName = memberName
            };

        public static AbstractExpression NOT(AbstractExpression value)
            => new UnaryExpression {
                Type = GetContextOrThrow().FindType<bool>(),
                Operator = UnaryOperator.LogicalNot,
                Operand = PopExpression(value) 
            };

        public static AbstractExpression NEW<T>(params object[] constructorArguments)
        {
            var context = GetContextOrThrow();
            var type = context.FindType<T>();
            
            return new NewObjectExpression {
                Type = type,
                ConstructorCall = new MethodCallExpression {
                    Type = type,
                    Arguments = constructorArguments
                        .Select(value => new Argument {
                            Expression = context.GetConstantExpression(value) 
                        })
                        .ToList()
                }
            };
        }

        public static AbstractExpression NEW(TypeMember type, params object[] constructorArguments) => throw new NotImplementedException();

        public static AbstractExpression INITOBJECT(params (string key, AbstractExpression value)[] initializers)
            => new ObjectInitializerExpression {
                PropertyValues = initializers.Select(init => new NamedPropertyValue(init.key, init.value)).ToList() 
            };

        public static AbstractExpression INITOBJECT(Action body)
        {
            var initializerContext = new ObjectInitializerContext();

            using (GetContextOrThrow().PushState(initializerContext))
            {
                body?.Invoke();
            }

            return new ObjectInitializerExpression {
                PropertyValues = initializerContext.PropertyValues
            };
        }

        public static AbstractExpression ASSIGN(this AbstractExpression target, AbstractExpression value) 
            => PushExpression(new AssignmentExpression {
                Left =(IAssignable)PopExpression(target),
                Right = PopExpression(value) 
            });

        public static AbstractExpression ASSIGN(this MemberExpression member, AbstractExpression value) 
            => PushExpression(new AssignmentExpression {
                Left = member,
                Right = PopExpression(value) 
            });

        public static AbstractExpression ASSIGN(this FieldMember target, AbstractExpression value)
            => PushExpression(new AssignmentExpression {
                Left = target.AsThisMemberExpression(),
                Right = PopExpression(value) 
            });

        public static AbstractExpression ASSIGN(this LocalVariable target, AbstractExpression value)
            => PushExpression(new AssignmentExpression {
                Left = target,
                Right = PopExpression(value) 
            });

        public static AbstractExpression INVOKE(this AbstractExpression expression, params AbstractExpression[] arguments)
            => INVOKE(expression, arguments.Select(arg => new Argument {
                Expression = arg 
            }));

        public static AbstractExpression INVOKE(this AbstractExpression expression, IEnumerable<Argument> arguments)
        {
            if (expression is MemberExpression memberExpression)
            {
                var target = memberExpression.Target;
                var method = memberExpression.Member as MethodMember;

                return new MethodCallExpression {
                    Target = target,
                    Method = method,
                    MethodName = memberExpression.MemberName,
                    Type = method?.ReturnType,
                    Arguments = arguments.ToList(),
                };
            }

            return new DelegateInvocationExpression {
                Delegate = expression,
                Type = expression.Type,
                Arguments = arguments.ToList()
            };
        }

        public static AbstractExpression INVOKE(this AbstractExpression expression, Action body)
        {
            var invocation = new InvocationContext();

            using (GetContextOrThrow().PushState(invocation))
            {
                body?.Invoke();
            }

            return INVOKE(expression, invocation.GetArguments());
        }

        public static AnonymousDelegateExpression LAMBDA(Action bodyNoArgs)
            => CreateAnonymousDelegate(
                bodyNoArgs,
                parameters => bodyNoArgs?.Invoke());

        public static AnonymousDelegateExpression LAMBDA(Action<MethodParameter> body1Arg)
            => CreateAnonymousDelegate(
                body1Arg,
                parameters => body1Arg?.Invoke(parameters[0]));

        public static AnonymousDelegateExpression LAMBDA(Action<MethodParameter, MethodParameter> body2Args)
            => CreateAnonymousDelegate(
                body2Args,
                parameters => body2Args?.Invoke(parameters[0], parameters[1]));

        public static AnonymousDelegateExpression LAMBDA(Action<MethodParameter, MethodParameter, MethodParameter> body3Args)
            => CreateAnonymousDelegate(
                body3Args,
                parameters => body3Args?.Invoke(parameters[0], parameters[1], parameters[2]));

        private static AnonymousDelegateExpression CreateAnonymousDelegate(Delegate body, Action<MethodParameter[]> invokeBody)
        {
            var context = GetContextOrThrow();
            
            var parameters = body.Method.GetParameters().Select((info, index) => new MethodParameter {
                Position = index,
                Name = info.Name,
                Type = context.FindType(info.ParameterType)
            }).ToArray();

            var lambda = new AnonymousDelegateExpression {
                Body = new BlockStatement()
            };

            using (context.PushState(new BlockContext(lambda.Body)))
            {
                invokeBody(parameters);
            }

            return lambda;
        }
        
        public static AbstractExpression TYPED(object value)
        {
            var context = GetContextOrThrow();

            return AbstractExpression.FromValue(value, resolveType: type => {
                if (context.TryFindMember<TypeMember>(type, out var typeRef))
                {
                    return typeRef;
                }
                return null;
            });
        }

        public static AbstractExpression ANY(object value)
        {
            return AbstractExpression.FromValue(value, resolveType: t => null);
        }

        private static AbstractExpression PushExpression(AbstractExpression expression)
        {
            return BlockContext.Push(expression);
        }

        private static AbstractExpression PopExpression(AbstractExpression expression)
        {
            return BlockContext.Pop(expression);
        }

        private static TupleExpression MakeTuple(string[] names, TypeMember[] types, out LocalVariable[] variables)
        {
            variables = names.Select((name, index) => new LocalVariable {
                Name = name,
                Type = types?[index]
            }).ToArray();

            return new TupleExpression(variables);
        }
    }
}
