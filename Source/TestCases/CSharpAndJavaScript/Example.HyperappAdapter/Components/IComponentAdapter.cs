﻿using System.Xml.Linq;
using Example.WebUIModel.Metadata;
using MetaPrograms.Expressions;

namespace Example.HyperappAdapter.Components
{
    public interface IComponentAdapter
    {
        void GenerateStateKeys();
        void GenerateActionKeys();
        XElement GenerateViewMarkup(AbstractExpression @model, AbstractExpression @actions);
        WebComponentMetadata Metadata { get; }
    }
}
