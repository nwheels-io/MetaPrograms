using System;
using System.Collections.Generic;
using NWheels.CodeGeneration.CodeModel.Expressions;

namespace NWheels.CodeGeneration.CodeModel.Members
{
    public class AttributeDescription
    {
        public AttributeDescription()
        {
            this.ConstructorArguments = new List<AbstractExpression>();
            this.PropertyValues = new List<PropertyValue>();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            return (AttributeType?.Name ?? base.ToString());
        }

        //------------------------------------------------- ----------------------------------------------------------------------------------------------------

        public TypeMember AttributeType { get; set; }
        public List<AbstractExpression> ConstructorArguments { get; private set; }
        public List<PropertyValue> PropertyValues { get; private set; }
        public Attribute Binding { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------------------

    public class PropertyValue
    {
        public string Name { get; set; }
        public AbstractExpression Value { get; set; }
    }
}
