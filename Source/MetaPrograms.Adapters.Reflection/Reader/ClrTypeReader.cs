using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using CommonExtensions;
using MetaPrograms;
using MetaPrograms.Expressions;
using MetaPrograms.Members;

namespace MetaPrograms.Adapters.Reflection.Reader
{
    public class ClrTypeReader
    {
        private const BindingFlags LookupBindingFlags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly string GenericTypeArityBackQuote = "`";

        private readonly Type _clrType;
        private readonly TypeMember _typeMember;
        private readonly ImperativeCodeModel _codeModel;
        private readonly int _distance;

        public ClrTypeReader(Type clrType, TypeMember typeMember, ImperativeCodeModel codeModel, int distance)
        {
            _clrType = clrType;
            _typeMember = typeMember;
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

            if (_typeMember.TypeKind != TypeMemberKind.GenericParameter)
            {
                ReadGenerics();
                ReadAncestors();
                ReadAttributes();

                if (_typeMember.TypeKind != TypeMemberKind.Primitive)
                {
                    ReadMembers();
                }
            }

            _typeMember.Status = MemberStatus.Compiled;
        }

        private TypeMember ResolveType(Type type)
        {
            return ClrTypeResolver.ResolveType(type, _codeModel, _distance + 1);
        }
        
        private AbstractExpression GetConstantExpression(object value)
        {
            return AbstractExpression.FromValue(value, ResolveType);
        }

        private void ReadName()
        {
            _typeMember.TypeKind = _clrType.GetTypeMemberKind();
            _typeMember.Name = _clrType.Name.TrimEndStartingWith(GenericTypeArityBackQuote);
            _typeMember.Namespace = _clrType.Namespace;
            _typeMember.AssemblyName = _clrType.Assembly.GetName().Name;
            _typeMember.Visibility = _clrType.GetMemberVisibility();
            _typeMember.Modifier = _clrType.GetMemberModifier();
        }

        private void ReadGenerics()
        {
            _typeMember.IsGenericType = _clrType.IsGenericType;
            _typeMember.IsGenericDefinition = _clrType.IsGenericTypeDefinition;
            _typeMember.IsGenericParameter = _clrType.IsGenericParameter;

            if (_clrType.IsGenericType)
            {
                if (_clrType.IsGenericTypeDefinition)
                {
                    _typeMember.GenericParameters.AddRange(_clrType.GetGenericArguments().Select(ResolveType));
                }
                else
                {
                    _typeMember.GenericTypeDefinition = ResolveType(_clrType.GetGenericTypeDefinition());
                    _typeMember.GenericParameters.AddRange(_typeMember.GenericTypeDefinition.GenericParameters);
                    _typeMember.GenericArguments.AddRange(_clrType.GetGenericArguments().Select(ResolveType));
                }
            }
        }

        private void ReadAncestors()
        {
            if (_clrType.BaseType != null && _clrType.BaseType != typeof(object))
            {
                _typeMember.BaseType = ResolveType(_clrType.BaseType);
            }

            foreach (var clrInterface in _clrType.GetInterfaces())
            {
                _typeMember.Interfaces.Add(ResolveType(clrInterface));
            }
        }

        private void ReadAttributes()
        {
            foreach (var clrAttribute in _clrType.GetCustomAttributesData())
            {
                _typeMember.Attributes.Add(ReadAttribute(clrAttribute));
            }
        }

        private List<AttributeDescription> ReadAttributes(MemberInfo info)
        {
            return ReadAttributes(info.GetCustomAttributesData());
        }

        private List<AttributeDescription> ReadAttributes(IEnumerable<CustomAttributeData> attributes)
        {
            var result = new List<AttributeDescription>();
            
            foreach (var clrAttribute in attributes)
            {
                result.Add(ReadAttribute(clrAttribute));
            }

            return result.ToList();
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

            return new AttributeDescription {
                AttributeType = attributeType,
                ConstructorArguments = constructorArguments.ToList(),
                PropertyValues = namedArguments.ToList()
            };
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
                _typeMember.Members.Add(ReadField(info));
            }
        }

        private FieldMember ReadField(FieldInfo info)
        {
            return new FieldMember {
                DeclaringType = _typeMember,
                Name = info.Name,
                Type = ResolveType(info.FieldType),
                Status = MemberStatus.Compiled,
                Visibility = info.GetMemberVisibility(),
                Modifier = info.GetMemberModifier(),
                Attributes = ReadAttributes(info)
            };
        }

        private void ReadConstructors()
        {
            foreach (var info in _clrType.GetConstructors(LookupBindingFlags))
            {
                _typeMember.Members.Add(ReadConstructor(info));
            }
        }

        private ConstructorMember ReadConstructor(ConstructorInfo info)
        {
            return new ConstructorMember {
                DeclaringType = _typeMember,
                Status =  MemberStatus.Compiled,
                Visibility = info.GetMemberVisibility(),
                Modifier = info.GetMemberModifier(),
                Attributes = ReadAttributes(info),
                Signature = ReadMethodSignature(info)
            };
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
            var parameters = method.GetParameters().Select(ReadParameter).ToList();
            MethodParameter returnParameter = null;
            
            if (method is MethodInfo nonConstructor && nonConstructor.ReturnType != typeof(void))
            {
                returnParameter = ReadParameter(nonConstructor.ReturnParameter);
            }
            
            return new MethodSignature {
                Parameters = parameters,
                ReturnValue = returnParameter
            };
        }

        private MethodParameter ReadParameter(ParameterInfo info)
        {
            return new MethodParameter {
                Name = info.Name,
                Position = info.Position,
                Type = ResolveType(info.ParameterType),
                Modifier = info.GetMethodParameterModifier(),
                Attributes = ReadAttributes(info.GetCustomAttributesData())
            };
        }
    }
}
