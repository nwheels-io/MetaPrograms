using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.Reflection.Reader
{
    public class ClrTypeReader
    {
        private const BindingFlags LookupBindingFlags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private readonly Type _clrType;
        private readonly TypeMemberBuilder _builder;
        private readonly ImperativeCodeModel _codeModel;
        private readonly int _distance;

        public ClrTypeReader(Type clrType, TypeMemberBuilder builder, ImperativeCodeModel codeModel, int distance)
        {
            _clrType = clrType;
            _builder = builder;
            _codeModel = codeModel;
            _distance = distance;
        }

        public void ReadNameOnly()
        {
            ReadName();
        }

        public void ReadAll()
        {
            ReadName();

            if (_builder.TypeKind != TypeMemberKind.GenericParameter)
            {
                ReadGenerics();
                ReadAncestors();
                ReadAttributes();

                if (_builder.TypeKind != TypeMemberKind.Primitive)
                {
                    ReadMembers();
                }
            }

            _builder.Status = MemberStatus.Compiled;
        }

        private MemberRef<TypeMember> ResolveType(Type type)
        {
            return ClrTypeResolver.ResolveType(type, _codeModel, _distance + 1);
        }
        
        private AbstractExpression GetConstantExpression(object value)
        {
            return AbstractExpression.FromValue(value, ResolveType);
        }

        private void ReadName()
        {
            _builder.TypeKind = _clrType.GetTypeMemberKind();
            _builder.Name = _clrType.Name;
            _builder.Namespace = _clrType.Namespace;
            _builder.AssemblyName = _clrType.Assembly.GetName().Name;
            _builder.Visibility = _clrType.GetMemberVisibility();
            _builder.Modifier = _clrType.GetMemberModifier();
        }

        private void ReadGenerics()
        {
            _builder.IsGenericType = _clrType.IsGenericType;
            _builder.IsGenericDefinition = _clrType.IsGenericTypeDefinition;
            _builder.IsGenericParameter = _clrType.IsGenericParameter;

            if (_clrType.IsGenericType)
            {
                if (_clrType.IsGenericTypeDefinition)
                {
                    _builder.GenericParameters.AddRange(_clrType.GetGenericArguments().Select(ResolveType));
                }
                else
                {
                    _builder.GenericDefinition = ResolveType(_clrType.GetGenericTypeDefinition());
                    _builder.GenericParameters.AddRange(_builder.GenericDefinition.Get().GenericParameters);
                    _builder.GenericArguments.AddRange(_clrType.GetGenericArguments().Select(ResolveType));
                }
            }
        }

        private void ReadAncestors()
        {
            if (_clrType.BaseType != typeof(object))
            {
                _builder.BaseType = ResolveType(_clrType.BaseType);
            }

            foreach (var clrInterface in _clrType.GetInterfaces())
            {
                _builder.Interfaces.Add(ResolveType(clrInterface));
            }
        }

        private void ReadAttributes()
        {
            foreach (var clrAttribute in _clrType.GetCustomAttributesData())
            {
                _builder.Attributes.Add(ReadAttribute(clrAttribute));
            }
        }

        private ImmutableList<AttributeDescription> ReadAttributes(MemberInfo info)
        {
            return ReadAttributes(info.GetCustomAttributesData());
        }

        private ImmutableList<AttributeDescription> ReadAttributes(IEnumerable<CustomAttributeData> attributes)
        {
            var result = new List<AttributeDescription>();
            
            foreach (var clrAttribute in attributes)
            {
                result.Add(ReadAttribute(clrAttribute));
            }

            return result.ToImmutableList();
        }

        private AttributeDescription ReadAttribute(CustomAttributeData clrAttribute)
        {
            var attributeType = ResolveType(clrAttribute.AttributeType); 
            
            var constructorArguments = clrAttribute.ConstructorArguments
                .Select(arg => GetConstantExpression(arg.Value));

            var namedArguments = clrAttribute.NamedArguments
                .Select(arg => new NamedPropertyValue(
                    arg.MemberName, 
                    GetConstantExpression(arg.TypedValue.Value)));

            return new AttributeDescription(
                attributeType,
                constructorArguments.ToImmutableList(),
                namedArguments.ToImmutableList());
        }

        private void ReadMembers()
        {
            ReadFields();
            ReadConstructors();
            ReadMethods();
            ReadProperties();
            ReadEvents();
        }

        private void ReadFields()
        {
            foreach (var info in _clrType.GetFields(LookupBindingFlags))
            {
                _builder.Members.Add(ReadField(info).GetAbstractRef());
            }
        }

        private FieldMember ReadField(FieldInfo info)
        {
            return new FieldMember(
                info.Name,
                _builder.GetTemporaryProxy().GetRef(),
                MemberStatus.Compiled,
                info.GetMemberVisibility(),
                info.GetMemberModifier(),
                ReadAttributes(info),
                ResolveType(info.FieldType),
                isReadOnly: false,
                initializer: null);
        }

        private void ReadConstructors()
        {
            foreach (var info in _clrType.GetConstructors(LookupBindingFlags))
            {
                _builder.Members.Add(ReadConstructor(info).GetAbstractRef());
            }
        }

        private ConstructorMember ReadConstructor(ConstructorInfo info)
        {
            return new ConstructorMember(
                info.Name,
                _builder.GetTemporaryProxy().GetRef(),
                MemberStatus.Compiled,
                info.GetMemberVisibility(),
                info.GetMemberModifier(),
                ReadAttributes(info),
                ReadMethodSignature(info),
                body: null,
                callThisConstructor: null,
                callBaseConstructor: null);
        }

        private void ReadMethods()
        {
            foreach (var info in _clrType.GetMethods(LookupBindingFlags))
            {
                
            }
        }

        private void ReadProperties()
        {
            foreach (var info in _clrType.GetProperties(LookupBindingFlags))
            {
                
            }
        }

        private void ReadEvents()
        {
            foreach (var info in _clrType.GetEvents(LookupBindingFlags))
            {
                
            }
        }
        
        private MethodSignature ReadMethodSignature(MethodBase method)
        {
            var parameters = method.GetParameters().Select(ReadParameter).ToImmutableList();
            MethodParameter returnParameter = null;
            
            if (method is MethodInfo nonConstructor && nonConstructor.ReturnType != typeof(void))
            {
                returnParameter = ReadParameter(nonConstructor.ReturnParameter);
            }
            
            return new MethodSignature(isAsync: false, returnParameter, parameters);
        }

        private MethodParameter ReadParameter(ParameterInfo info)
        {
            return new MethodParameter(
                info.Name, 
                info.Position, 
                ResolveType(info.ParameterType), 
                info.GetMethodParameterModifier(),
                ReadAttributes(info.GetCustomAttributesData()));
        }
    }
}
